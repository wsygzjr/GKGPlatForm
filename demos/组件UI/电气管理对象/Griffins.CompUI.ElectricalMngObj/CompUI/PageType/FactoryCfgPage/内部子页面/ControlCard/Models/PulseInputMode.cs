namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Models
{
    /// <summary>
    /// 脉冲输入模式
    /// </summary>
    public enum PulseInputMode
    {
        /// <summary>
        /// 4倍AB相
        /// </summary>
        Times4ABPhase = 0,
        /// <summary>
        /// 2倍AB相
        /// </summary>
        Times2ABPhase,
        /// <summary>
        /// 1倍AB相
        /// </summary>
        Times1ABPhase,
        /// <summary>
        /// 双脉冲
        /// </summary>
        DoublePulse,
        /// <summary>
        /// 脉冲+方向
        /// </summary>
        PulseDirection,
    }
}