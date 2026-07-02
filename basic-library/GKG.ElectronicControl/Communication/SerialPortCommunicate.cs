using GF_Gereric;
using RJCP.IO.Ports; // 替换为跨平台串口库
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace GKG
{
    namespace ElectronicControl
    {
        public class SerialPortCommunicate : IBaseCommunicate
        {
            private SerialConfig serialConfig;
            private SerialPortStream serialPort; // 核心类替换为 SerialPortStream
            private readonly object _lock = new();
            private readonly Queue<byte> _receiveBuffer = new Queue<byte>();
            private string _receiveStr = "";

            public bool IsOpen
            {
                get => serialPort == null ? false : serialPort.IsOpen;
            }

            public void Init(byte[] initCfg)
            {
                if (serialPort != null)
                    return;

                // 初始化跨平台串口对象
                serialPort = new SerialPortStream();
                serialConfig = JsonObjConvert.FromJSonBytes<SerialConfig>(initCfg);

                // 绑定数据接收事件（API 名称略有差异，但逻辑一致）
                // serialPort.DataReceived += SerialPort_DataReceived;
            }

            /// <summary>
            /// 打开串口对象
            /// </summary>
            /// <param name="timeOut">读写延时</param>
            /// <returns></returns>
            public bool Open(int timeOut)
            {
                try
                {
                    if (serialPort.IsOpen)
                        serialPort.Close();

                    // 串口参数配置（与原逻辑完全一致）
                    serialPort.PortName = serialConfig.PortName;
                    serialPort.BaudRate = serialConfig.BaudRate;
                    serialPort.Parity = serialConfig.Parity;
                    serialPort.StopBits = serialConfig.StopBits;
                    serialPort.DataBits = serialConfig.DataBits;

                    // 超时设置（SerialPortStream 的超时配置方式）
                    serialPort.WriteTimeout = timeOut;
                    serialPort.ReadTimeout = timeOut;
                    serialPort.ReadBufferSize = 1024;

                    // SerialPortStream 没有 ReceivedBytesThreshold 属性，注释掉（不影响核心功能）
                    // serialPort.ReceivedBytesThreshold = 10;

                    // 绑定数据接收事件（打开串口前绑定）
                    serialPort.DataReceived += SerialPort_DataReceived;

                    // 打开串口
                    serialPort.Open();
                    ClearReadBuffer();
                }
                catch (Exception ex)
                {
                    return false;
                }

                return true;
            }

            /// <summary>
            /// 关闭串口对象
            /// </summary>
            /// <returns></returns>
            public bool Close()
            {
                if (serialPort.IsOpen)
                {
                    // 解除事件绑定
                    serialPort.DataReceived -= SerialPort_DataReceived;
                    serialPort.Close();
                    serialPort.Dispose();
                }
                return true;
            }

            public void SetCommuParam(string commuParam)
            {
                ;
            }

            public bool Write(byte[] sendBytes)
            {
                if (serialPort == null || !serialPort.IsOpen)
                    return false;

                if (serialConfig.IsEnableCRC16 == true)
                    sendBytes = AppendCRC16(sendBytes);

                ClearReadBuffer();
                // 写入字节数组（API 完全兼容）
                serialPort.Write(sendBytes, 0, sendBytes.Length);
                return true;
            }

            public bool Write(string sendString)
            {
                if (serialPort == null || !serialPort.IsOpen)
                    return false;

                if (serialConfig.ModbusType == EModbusType.RS485)
                {
                    byte[] sendBytes = StringToHexBytes(sendString);
                    return Write(sendBytes);
                }
                else
                {
                    ClearReadBuffer();
                    // 写入字符串（API 完全兼容）
                    serialPort.Write(sendString);
                }
                return true;
            }

            /// <summary>
            /// 读取规定时间内的数据
            /// </summary>
            /// <param name="timeOut">超时时间(ms)</param>
            /// <param name="readBytes"></param>
            /// <returns></returns>
            public bool ReadTimeout(int timeOut, out byte[] readBytes)
            {
                readBytes = Array.Empty<byte>();
                if (serialPort == null || !serialPort.IsOpen)
                {
                    return false;
                }

                Thread.Sleep(20); //避免回调事件未被触发
                var sw = Stopwatch.StartNew();
                sw.Start();

                while (sw.ElapsedMilliseconds < timeOut)
                {
                    if (_receiveBuffer.Count > 0)
                    {
                        lock (_lock)
                        {
                            readBytes = new byte[_receiveBuffer.Count];
                            for (int i = 0; i < readBytes.Length; i++)
                            {
                                readBytes[i] = _receiveBuffer.Dequeue();
                            }
                        }
                        return true;
                    }

                    // SerialPortStream 使用 BytesToRead 属性（兼容原 API）
                    if (serialPort.BytesToRead > 0)
                    {
                        ForceReadPendingData();
                        continue; // 再次检查缓冲区
                    }
                }
                return false;
            }

            /// <summary>
            /// 读取固定长度数据
            /// </summary>
            /// <param name="length">长度</param>
            /// <param name="readBytes"></param>
            /// <returns></returns>
            public bool ReadLength(int length, out byte[] readBytes)
            {
                readBytes = Array.Empty<byte>();
                if (serialPort == null || !serialPort.IsOpen)
                {
                    return false;
                }

                var sw = Stopwatch.StartNew();
                sw.Start();

                while (sw.ElapsedMilliseconds <= 5000)
                {
                    if (_receiveBuffer.Count >= length)
                    {
                        lock (_lock)
                        {
                            readBytes = new byte[length];
                            for (int i = 0; i < readBytes.Length; i++)
                            {
                                readBytes[i] = _receiveBuffer.Dequeue();
                            }
                        }
                        return true;
                    }
                }
                return false;
            }

            public void ClearReadBuffer()
            {
                try
                {
                    lock (_lock)
                    {
                        _receiveBuffer.Clear();
                    }

                    // SerialPortStream 的缓冲区清理（适配 API 差异）
                    if (serialPort.IsOpen)
                    {
                        serialPort.ReadExisting();                         // 读取缓冲全部数据
                        serialPort.DiscardInBuffer();                     // 清除接收缓冲区
                        serialPort.DiscardOutBuffer();                    // 清除发送缓冲区
                    }
                }
                catch
                {
                    throw new Exception("清除缓冲区失败");
                }
            }

            private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
            {
                if (!serialPort.IsOpen) return;

                try
                {
                    // SerialPortStream 的 BytesToRead 属性兼容原逻辑
                    int bytesToRead = serialPort.BytesToRead;
                    if (bytesToRead <= 0) return;

                    byte[] buffer = new byte[bytesToRead];
                    int bytesRead = serialPort.Read(buffer, 0, bytesToRead);
                    string str = Convert.ToHexString(buffer);

                    lock (_lock)
                    {
                        for (int i = 0; i < bytesRead; i++)
                        {
                            _receiveBuffer.Enqueue(buffer[i]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"接收数据错误：{ex.Message}");
                }
            }

            private void ForceReadPendingData()
            {
                try
                {
                    if (serialPort.IsOpen && serialPort.BytesToRead > 0)
                    {
                        int bytesToRead = serialPort.BytesToRead;
                        byte[] buffer = new byte[bytesToRead];
                        int bytesRead = serialPort.Read(buffer, 0, bytesToRead);

                        lock (_lock)
                        {
                            for (int i = 0; i < bytesRead; i++)
                            {
                                _receiveBuffer.Enqueue(buffer[i]);
                            }
                        }
                    }
                }
                catch
                {
                    /* 忽略异常 */
                }
            }

            /// <summary>
            /// 字符串转十六进制数组
            /// </summary>
            /// <param name="str"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentException"></exception>
            private byte[] StringToHexBytes(string str)
            {
                str = str.Replace(" ", "");
                if (str.Length % 2 != 0)
                    throw new ArgumentException("输入字符串长度必须为偶数");

                int len = str.Length / 2;
                byte[] byteHex = new byte[len];

                for (int i = 0; i < len; i++)
                {
                    string byteValue = str.Substring(i * 2, 2);
                    byteHex[i] = Convert.ToByte(byteValue, 16);
                }
                return byteHex;
            }

            /// <summary>
            /// 为串口命令添加CRC16校验码
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            private byte[] AppendCRC16(byte[] data)
            {
                ushort crc = ComputeCRC16(data);
                byte[] result = new byte[data.Length + 2];
                Array.Copy(data, result, data.Length);
                result[result.Length - 2] = (byte)(crc & 0xFF);        // CRC 低字节
                result[result.Length - 1] = (byte)((crc >> 8) & 0xFF); // CRC 高字节
                return result;
            }

            /// <summary>
            /// 计算CRC16校验码
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            private ushort ComputeCRC16(byte[] data)
            {
                ushort crc = 0xFFFF;
                for (int i = 0; i < data.Length; i++)
                {
                    crc ^= data[i];
                    for (int j = 0; j < 8; j++)
                    {
                        if ((crc & 0x0001) != 0)
                            crc = (ushort)((crc >> 1) ^ 0xA001);
                        else
                            crc >>= 1;
                    }
                }
                return crc;
            }
        }
    }
}