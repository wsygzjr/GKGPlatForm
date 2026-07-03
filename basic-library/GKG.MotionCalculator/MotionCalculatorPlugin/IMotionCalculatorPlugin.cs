namespace GKG
{
    namespace MotionControl
    {
        /// <summary>
        /// 运动计算插件接口。
        /// </summary>
        public interface IMotionCalculatorPlugin
        {
            /// <summary>
            /// 运动计算插件名称。
            /// </summary>
            string MotionCalculatorName { get; }

            /// <summary>
            /// 驱动名称。
            /// </summary>
            string DriverName { get; }

            /// <summary>
            /// 插件初始化。
            /// </summary>
            /// <param name="pluginFileName">插件文件路径</param>
            void Init(string pluginFileName);

            /// <summary>
            /// 创建运动计算驱动对象。
            /// </summary>
            IMotionCalculatorDriver CreateMotionCalculatorDriverObj();
        }
    }
}
