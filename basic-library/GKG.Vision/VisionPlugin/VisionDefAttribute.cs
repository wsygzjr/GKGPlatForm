namespace GKG
{
    namespace Vision
    {
        /// <summary>
        /// 视觉插件定义特性。
        /// </summary>
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
        public class VisionDefAttribute : Attribute
        {
            /// <summary>
            /// 视觉插件种类标识。
            /// </summary>
            public const string PLUGINKIND_Str = "{F1F2A53D-5CBB-4E0A-9A7B-8E53D33A9C11}";

            /// <summary>
            /// 视觉插件种类标识。
            /// </summary>
            public static readonly Guid PLUGINKIND = new Guid(PLUGINKIND_Str);

            public VisionDefAttribute(string visionIDStr)
            {
                VisionID = new Guid(visionIDStr);
            }

            /// <summary>
            /// 视觉插件ID。
            /// </summary>
            public Guid VisionID { get; }
        }
    }
}
