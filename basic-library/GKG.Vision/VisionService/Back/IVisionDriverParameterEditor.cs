namespace GKG
{
    namespace Vision
    {
        /// <summary>
        /// 视觉驱动参数编辑接口。
        /// 用于给前端提供参数编辑控件，以及参数的序列化读写能力。
        /// </summary>
        public interface IVisionDriverParameterEditor
        {
            /// <summary>
            /// 前端参数编辑控件。
            /// 这里使用 object 以避免核心库直接绑定具体 UI 框架(WPF/WinForms/Avalonia)。
            /// </summary>
            object? View { get; }

            /// <summary>
            /// 给前端控件设置参数。
            /// 参数使用序列化后的字节流。
            /// </summary>
            /// <param name="data">序列化后的参数数据</param>
            void SetData(byte[] data);

            /// <summary>
            /// 从前端控件获取参数。
            /// 返回序列化后的字节流。
            /// </summary>
            /// <returns>序列化后的参数数据</returns>
            byte[] GetData();

            /// <summary>
            /// 参数被前端编辑修改后触发。
            /// </summary>
            event EventHandler? AfterModify;
        }
    }
}
