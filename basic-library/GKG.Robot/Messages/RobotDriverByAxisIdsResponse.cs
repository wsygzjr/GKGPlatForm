using Griffins.PF;

namespace GKG
{
    /// <summary>
    /// 根据轴ID列表构建得到的 RobotDriver 响应。
    /// </summary>
    public class RobotDriverByAxisIdsResponse : MutualInfoResponseBase
    {
        /// <summary>
        /// 创建出来并完成轴绑定的机器人驱动。
        /// </summary>
        public GKG.MotionControl.IRobotDriver? RobotDriver { get; set; }
    }
}
