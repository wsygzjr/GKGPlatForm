namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Models
{
    /// <summary>
    /// 脉冲输出模式
    /// </summary>
    public enum PulseOutputMode
    {
        /// <summary>
        /// 脉冲低+方向低
        /// </summary>
        lowPulse_LowDirection = 0,
        /// <summary>
        /// 脉冲高+方向高
        /// </summary>
        HighPulse_HighDirection,
        /// <summary>
        /// 脉冲高+方向低
        /// </summary>
        HighPulse_LowDirection,
        /// <summary>
        /// 脉冲低+方向高
        /// </summary>
        LowPulse_HighDirection,
        /// <summary>
        /// 双脉冲高
        /// </summary>
        DoublePulse_High,
        /// <summary>
        /// 双脉冲低
        /// </summary>
        DoublePulse_Low,
        /// <summary>
        /// 模拟量
        /// </summary>
        AnalogQuantity,
    }
}