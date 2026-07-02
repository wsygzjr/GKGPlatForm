using Avalonia.Controls;
using Griffins.Map.UI;
using Griffins.UI.General;
using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;

namespace Griffins.CompUI.ElectricalMngObj.DebuggingPage
{
    /// <summary>
    /// 轴调试子页面的视图模型
    /// </summary>
    public class AxisDebuggingSubPageViewModel : ReactiveObject
    {
        /// <summary>
        /// 页面布局对象
        /// </summary>
        protected Control? _viewReference;
        private ICompUIRunTimeCallBack? callBack;
        public ReactiveCommand<Unit, Unit> ButtonClickCommand { get; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public AxisDebuggingSubPageViewModel()
        {
            ButtonClickCommand = ReactiveCommand.CreateFromTask(OnButtonClicked);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="callBack">回调接口</param>
        public void Init(ICompUIRunTimeCallBack callBack)
        {
            this.callBack = callBack;
        }
        /// <summary>
        /// 执行服务命令
        /// </summary>
        private async Task OnButtonClicked()
        {
            try
            {
                string cmdID = "Test";
                var response = this.callBack.ExecConfigSvrCtlCmd(cmdID, null);
            }
            catch (System.Exception ex)
            {
                await MessageBox.ShowErrorDialog("错误", ex.Message, _viewReference);
            }
        }
        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view;
        }
    }
}