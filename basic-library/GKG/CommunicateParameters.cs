
using RJCP.IO.Ports;

namespace GKG
{
    public enum EModbusType
    {
        RS232,
        RS485,
    }
    /// <summary>
    /// 串口初始化参数（适配 SerialPortStream 跨平台版本）
    /// </summary>
    public struct SerialConfig
    {
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public int DataBits { get; set; }
        // 关键修改：替换为 RJCP.IO.Ports 命名空间下的枚举
        public StopBits StopBits { get; set; }
        public Parity Parity { get; set; }
        public bool IsEnableCRC16 { get; set; }
        public EModbusType ModbusType { get; set; }
    }
}