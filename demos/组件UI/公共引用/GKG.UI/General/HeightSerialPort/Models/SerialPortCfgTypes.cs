namespace GKG.UI.General
{
    /// <summary>
    /// 串口配置相关枚举
    /// </summary>
    public static class SerialPortCfgTypes
    {
        /// <summary>
        /// 波特率枚举
        /// </summary>
        public enum BaudRate
        {
            /// <summary>
            /// 4800
            /// </summary>
            BaudRate4800 = 4800,

            /// <summary>
            /// 9600
            /// </summary>
            BaudRate9600 = 9600,

            /// <summary>
            /// 19200
            /// </summary>
            BaudRate19200 = 19200,

            /// <summary>
            /// 38400
            /// </summary>
            BaudRate38400 = 38400,

            /// <summary>
            /// 57600
            /// </summary>
            BaudRate57600 = 57600,

            /// <summary>
            /// 115200
            /// </summary>
            BaudRate115200 = 115200
        }

        /// <summary>
        /// 数据位枚举
        /// </summary>
        public enum DataBits
        {
            /// <summary>
            /// 5位
            /// </summary>
            Five = 5,

            /// <summary>
            /// 6位
            /// </summary>
            Six = 6,

            /// <summary>
            /// 7位
            /// </summary>
            Seven = 7,

            /// <summary>
            /// 8位
            /// </summary>
            Eight = 8
        }

        /// <summary>
        /// 停止位枚举
        /// </summary>
        public enum StopBits
        {
            /// <summary>
            /// 无
            /// </summary>
            None = 0,

            /// <summary>
            /// 1位
            /// </summary>
            One = 1,

            /// <summary>
            /// 2位
            /// </summary>
            Two = 2,

            /// <summary>
            /// 1.5位
            /// </summary>
            OnePointFive = 3
        }

        /// <summary>
        /// 校验类型枚举
        /// </summary>
        public enum Parity
        {
            /// <summary>
            /// 无
            /// </summary>
            None,

            /// <summary>
            /// 奇校验
            /// </summary>
            Odd,

            /// <summary>
            /// 偶校验
            /// </summary>
            Even,

            /// <summary>
            /// 标记
            /// </summary>
            Mark,

            /// <summary>
            /// 空格
            /// </summary>
            Space
        }
    }
}