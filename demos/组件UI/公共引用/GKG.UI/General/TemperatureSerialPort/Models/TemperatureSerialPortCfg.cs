using Newtonsoft.JsonG.Linq;

namespace GKG.UI.General
{
    /// <summary>
    /// 温度控制串口配置 数据模型
    /// </summary>
    public class TemperatureSerialPortCfg
    {
        /// <summary>
        /// 串口号
        /// </summary>
        public string PortName { get; set; } = "Com1";

        /// <summary>
        /// 波特率
        /// </summary>
        public SerialPortCfgTypes.BaudRate BaudRate { get; set; } = SerialPortCfgTypes.BaudRate.BaudRate9600;

        /// <summary>
        /// 数据位
        /// </summary>
        public SerialPortCfgTypes.DataBits DataBits { get; set; } = SerialPortCfgTypes.DataBits.Eight;

        /// <summary>
        /// 停止位
        /// </summary>
        public SerialPortCfgTypes.StopBits StopBits { get; set; } = SerialPortCfgTypes.StopBits.One;

        /// <summary>
        /// 串口校验类型
        /// </summary>
        public SerialPortCfgTypes.Parity Parity { get; set; } = SerialPortCfgTypes.Parity.None;

        /// <summary>
        /// 是否启用CRC16
        /// </summary>
        public bool EnableCRC16 { get; set; } = false;

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            PortName = jObject["PortName"]?.Value<string>() ?? "Com1";
            BaudRate = jObject["BaudRate"]?.Value<SerialPortCfgTypes.BaudRate>() ?? SerialPortCfgTypes.BaudRate.BaudRate9600;
            DataBits = jObject["DataBits"]?.Value<SerialPortCfgTypes.DataBits>() ?? SerialPortCfgTypes.DataBits.Eight;
            StopBits = jObject["StopBits"]?.Value<SerialPortCfgTypes.StopBits>() ?? SerialPortCfgTypes.StopBits.One;
            Parity = jObject["Parity"]?.Value<SerialPortCfgTypes.Parity>() ?? SerialPortCfgTypes.Parity.None;
            EnableCRC16 = jObject["EnableCRC16"]?.Value<bool>() ?? false;
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public JObject ToJObject()
        {
            return new JObject
            {
                { "PortName", PortName },
                { "BaudRate", (int)BaudRate },
                { "DataBits", (int)DataBits },
                { "StopBits", (int)StopBits },
                { "Parity", (int)Parity },
                { "EnableCRC16", EnableCRC16 }
            };
        }

        /// <summary>
        /// 从另一个实例复制数据
        /// </summary>
        /// <param name="src">数据源</param>
        public void CopyFrom(TemperatureSerialPortCfg src)
        {
            if (src == null)
            {
                return;
            }

            PortName = src.PortName;
            BaudRate = src.BaudRate;
            DataBits = src.DataBits;
            StopBits = src.StopBits;
            Parity = src.Parity;
            EnableCRC16 = src.EnableCRC16;
        }
    }
}