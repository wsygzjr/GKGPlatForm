using Avalonia.Controls;
using ReactiveUI;

namespace Griffins.CompUI.ElectricalMngObj.DebuggingPage
{
    /// <summary>
    /// IO-In调试子页面的视图模型
    /// </summary>
    public class IOInDebuggingSubPageViewModel : ReactiveObject
    {
        /// <summary>
        /// 页面布局对象
        /// </summary>
        private Control? _viewReference;
        /// <summary>
        /// 构造函数
        /// </summary>
        public IOInDebuggingSubPageViewModel()
        {
        }

        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Control view) => _viewReference = view;

    }
}