namespace GKG
{
    namespace Vision
    {
        /// <summary>
        /// GVision 参数编辑器实现。
        /// 直接返回 Avalonia 控件供前端使用。
        /// </summary>
        public class GVisionDriverParameterEditor : IVisionDriverParameterEditor
        {
            private readonly GVisionDriverParameterEditorView view = new GVisionDriverParameterEditorView();

            public GVisionDriverParameterEditor()
            {
                view.AfterModify += (_, _) => AfterModify?.Invoke(this, EventArgs.Empty);
            }

            /// <summary>
            /// 前端 Avalonia 控件。
            /// </summary>
            public object? View => view;

            public event EventHandler? AfterModify;

            public void SetData(byte[] data)
            {
                view.SetData(data);
            }

            public byte[] GetData()
            {
                return view.GetData();
            }
        }
    }
}
