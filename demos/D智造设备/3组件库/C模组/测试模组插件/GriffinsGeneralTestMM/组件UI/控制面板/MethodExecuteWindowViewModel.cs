using ReactiveUI;
using Griffins.UI.Test;

namespace GriffinsGeneralTestMM
{
    /// <summary>
    /// 执行方法弹窗 视图模型
    /// </summary>
    public class MethodExecuteWindowViewModel : ReactiveObject
    {
        /// <summary>
        /// 方法执行 视图模型
        /// </summary>
        public MethodExecuteViewModel MethodExecuteViewModel { get; }
        public MethodExecuteWindowViewModel(MethodExecuteViewModel methodExecuteViewModel)
        {
            MethodExecuteViewModel = methodExecuteViewModel;
        }
    }
}