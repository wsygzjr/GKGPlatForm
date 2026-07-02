namespace GKG
{
    namespace Vision
    {
        /// <summary>
        /// 视觉插件接口。
        /// 参考 MotionControl 插件模型，用于统一管理视觉驱动插件。
        /// </summary>
        public interface IVisionPlugin
        {
            /// <summary>
            /// 视觉插件名称。
            /// </summary>
            string VisionName { get; }

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
            /// 创建视觉驱动对象。
            /// </summary>
            IVisionDriver CreateVisionDriverObj();

            /// <summary>
            /// 创建视觉驱动参数编辑器对象。
            /// 用于前端参数编辑，不参与底层驱动运行逻辑。
            /// </summary>
            IVisionDriverParameterEditor CreateVisionDriverParameterEditorObj();

            /// <summary>
            /// 创建前端视觉服务对象。
            /// 用于将视觉后端事件桥接给前端/UI 层。
            /// 不支持前端视觉服务的插件可返回 null。
            /// </summary>
            IFrontendVisionService? CreateFrontendVisionServiceObj();
        }
    }
}
