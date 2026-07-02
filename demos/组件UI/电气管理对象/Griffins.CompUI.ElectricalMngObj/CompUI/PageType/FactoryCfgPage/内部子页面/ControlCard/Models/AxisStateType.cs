namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Models
{
    /// <summary>
    /// 运控卡轴状态类型
    /// </summary>
    public enum AxisStateType
    {
        /// <summary>
        /// 原点信号
        /// </summary>
        OriginSignal,
        /// <summary>
        /// 报警信号
        /// </summary>
        AlarmSignal,
        /// <summary>
        /// 正限位信号
        /// </summary>
        PositiveLimitSignal,
        /// <summary>
        /// 负限位信号
        /// </summary>
        NegativeLimitSignal,
        /// <summary>
        /// Inpi信号
        /// </summary>
        InpiSignal,
        /// <summary>
        /// EZ信号
        /// </summary>
        EZSignal,
        /// <summary>
        /// 使能信号
        /// </summary>
        EnableSignal,
        /// <summary>
        /// 准备完成信号
        /// </summary>
        PrepareCompleteSignal,

    }
}