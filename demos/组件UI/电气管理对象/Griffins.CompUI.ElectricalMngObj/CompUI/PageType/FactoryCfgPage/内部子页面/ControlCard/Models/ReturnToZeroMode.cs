namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Models
{
    /// <summary>
    /// 回零模式选项
    /// </summary>
    public enum ReturnToZeroMode
    {
        /// <summary>
        /// 1次原点回零
        /// </summary>
        OneOrigin,
        /// <summary>
        /// 2次原点回零
        /// </summary>
        TowOrigin,
        /// <summary>
        /// 负极限回零
        /// </summary>
        NegativeLimit,
        /// <summary>
        /// EZ回零
        /// </summary>
        EZ,
        /// <summary>
        /// 找一个EZ停止
        /// </summary>
        EZStop,
        /// <summary>
        /// 找一个EZ锁存回找
        /// </summary>
        EZLockAndRetrieve,
    }
}