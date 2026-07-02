using System.Collections.Generic;

namespace Griffins.CompUI.BasicRobot.CompUI_BasicRobot.PageType.InitCfgPage.MechanicalArm.Models
{
    /// <summary>
    /// 机械手组件配置
    /// </summary>
    public class MechanicalArmCompUIModel
    {
        /// <summary>
        /// 点位运动模式
        /// </summary>
        public MechanicalArmPointMoveMode PointMoveMode { get; set; } = MechanicalArmPointMoveMode.Linked;

        /// <summary>
        /// 轴号
        /// </summary>
        public int AxisNumber { get; set; } = 1;

        /// <summary>
        /// 分段速度模式
        /// </summary>
        public MechanicalArmSegmentSpeedMode SegmentSpeedMode { get; set; } = MechanicalArmSegmentSpeedMode.High;

        /// <summary>
        /// 指令列表
        /// </summary>
        public List<MechanicalArmCommandItemCompUIModel> Commands { get; set; } = new();
    }
}
