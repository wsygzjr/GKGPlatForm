namespace GKG.ElectronicControl
{
    /// <summary>
    /// 运动控制驱动默认配置。
    /// 用于在不实际初始化板卡的情况下，暴露驱动的基础能力规模。
    /// </summary>
    public sealed class MotionControlDriverDefaultConfig
    {
        /// <summary>
        /// 轴数量。
        /// </summary>
        public int SupportAxisNum { get; }

        /// <summary>
        /// 状态量数量。
        /// </summary>
        public int SupportIoStateNum { get; }

        /// <summary>
        /// 模拟量数量。
        /// </summary>
        public int SupportAnalogNum { get; }
        /// <summary>
        /// 运动卡接口类型
        /// </summary>
        public MotionControlCardType MotionControlCardType { get; }
        /// <summary>
        /// MotionControlDriverDefaultConfig默认构造函数
        /// </summary>
        /// <param name="motionControlCardType">运动卡接口类型</param>
        /// <param name="supportAxisNum">轴数量</param>
        /// <param name="supportIoStateNum">状态量数量</param>
        /// <param name="supportAnalogNum">模拟量数量</param>
        public MotionControlDriverDefaultConfig(MotionControlCardType motionControlCardType, int supportAxisNum, int supportIoStateNum, int supportAnalogNum)
        {
            this.MotionControlCardType = motionControlCardType;
            this.SupportAxisNum = supportAxisNum;
            this.SupportIoStateNum = supportIoStateNum;
            this.SupportAnalogNum = supportAnalogNum;
        }
    }
    /// <summary>
    /// 运动控制卡驱动信息
    /// </summary>
    public sealed class MontionControlDriverTypeInfo
    {
        /// <summary>
        /// 运动卡型号
        /// </summary>
        public MotionCardType MotionCardType { get; }
        /// <summary>
        /// 运动卡名称
        /// </summary>
        public string MotionControlName { get; }
        /// <summary>
        /// 运动卡接口类型
        /// </summary>
        public MotionControlCardType MotionControlCardType { get; }
        /// <summary>
        /// MontionControlDriverTypeInfo默认构造函数
        /// </summary>
        /// <param name="motionControlCardType">运动卡接口类型</param>
        /// <param name="motionCardType">运动卡型号</param>
        /// <param name="motionControlName">运动卡名称</param>
        public MontionControlDriverTypeInfo(MotionControlCardType motionControlCardType, MotionCardType motionCardType, string motionControlName)
        {
            this.MotionControlCardType = motionControlCardType;
            this.MotionCardType = motionCardType;
            this.MotionControlName = motionControlName;
        }
    }

}
