namespace GKG
{
    namespace MotionControl
    {
        /// <summary>
        /// 可接收 Robot 规划上下文的驱动接口。
        /// </summary>
        public interface IRobotContextAware
        {
            void SetPlanContext(RobotPlanContext context);
        }
    }
}
