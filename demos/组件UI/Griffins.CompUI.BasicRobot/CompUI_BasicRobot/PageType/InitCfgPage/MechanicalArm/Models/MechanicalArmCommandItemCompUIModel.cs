namespace Griffins.CompUI.BasicRobot.CompUI_BasicRobot.PageType.InitCfgPage.MechanicalArm.Models
{
    /// <summary>
    /// 机械手-指令项配置
    /// </summary>
    public class MechanicalArmCommandItemCompUIModel
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 范围
        /// </summary>
        public string Range { get; set; } = string.Empty;

        /// <summary>
        /// 速度
        /// </summary>
        public decimal Speed { get; set; }

        /// <summary>
        /// 加速度
        /// </summary>
        public decimal Acceleration { get; set; }
    }
}
