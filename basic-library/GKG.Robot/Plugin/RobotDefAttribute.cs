namespace GKG
{
    namespace MotionControl
    {
        /// <summary>
        /// 机器人插件定义特性。
        /// </summary>
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
        public class RobotDefAttribute : Attribute
        {
            public const string PLUGINKIND_Str = "{7D5D34C2-0A77-4C0D-A9EE-3FA53F16B2D2}";

            public static readonly Guid PLUGINKIND = new Guid(PLUGINKIND_Str);

            public RobotDefAttribute(string robotIDStr)
            {
                RobotID = new Guid(robotIDStr);
            }

            public Guid RobotID { get; }
        }
    }
}
