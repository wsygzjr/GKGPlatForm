namespace GKG
{
    namespace MotionControl
    {
        /// <summary>
        /// 运动计算插件定义特性。
        /// </summary>
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
        public class MotionCalculatorDefAttribute : Attribute
        {
            /// <summary>
            /// 运动计算插件种类标识。
            /// </summary>
            public const string PLUGINKIND_Str = "{A2D1A7CE-3D50-4F61-89D2-6CBEDEAF7E31}";

            /// <summary>
            /// 运动计算插件种类标识。
            /// </summary>
            public static readonly Guid PLUGINKIND = new Guid(PLUGINKIND_Str);

            public MotionCalculatorDefAttribute(string motionCalculatorIDStr)
            {
                MotionCalculatorID = new Guid(motionCalculatorIDStr);
            }

            /// <summary>
            /// 运动计算插件ID。
            /// </summary>
            public Guid MotionCalculatorID { get; }
        }
    }
}
