namespace GKG
{
    namespace MotionControl
    {
        /// <summary>
        /// 机器人插件接口。
        /// </summary>
        public interface IRobotPlugin
        {
            string RobotName { get; }

            string DriverName { get; }

            void Init(string pluginFileName);

            IRobotDriver CreateRobotDriverObj();
        }
    }
}
