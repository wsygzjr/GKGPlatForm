namespace GKG
{
    namespace ElectronicControl
    {
        /// <summary>
        /// 模拟量接口
        /// </summary>
        public interface IAnalogQuantity
        {
            public ChannelParametersList ChannelParameters { get; }

            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="serialConfig">串口配置</param>
            /// <param name="stationId">站号</param>
            public void Init(byte[] serialConfig, int stationId);

            /// <summary>
            /// 设置模拟量值
            /// </summary>
            /// <param name="channelId">通道号</param>
            /// <param name="value">模拟量值</param>
            public void SetValue(string channelId, double value);

            /// <summary>
            /// 获取模拟量值
            /// </summary>
            /// <param name="channelId">通道号</param>
            /// <returns>模拟量值</returns>
            /// <exception cref="GKGException">异常（错误码，错误描述）</exception>
            public double GetValue(string channelId);
        }
    }
}