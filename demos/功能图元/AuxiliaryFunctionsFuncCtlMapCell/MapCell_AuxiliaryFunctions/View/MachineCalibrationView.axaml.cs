using Avalonia.Controls;
using Avalonia.ReactiveUI;
using GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.ViewModel;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.View
{
    public partial class MachineCalibrationView : ReactiveWindow<MachineCalibrationViewModel>
    {
        public MachineCalibrationView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // ==========================================
                // 通道 1：响应 ViewModel 弹出的 WaitingBox 等待框
                // (由于 WaitingBox 是异步动作，我们使用 async lambda 响应)
                // ==========================================
                this.BindInteraction(ViewModel,
                    vm => vm.ShowWaitingBoxDialog,
                    async interaction =>
                    {
                        var context = interaction.Input;
                        var window = (Window)this.VisualRoot;

                        // 弹出真正的等待框，阻塞 UI 直到后台任务跑完
                        await WaitingBoxView.ShowAsync(window, context.WorkTask, context.Message);

                        // 标记交互成功
                        interaction.SetOutput(Unit.Default);
                    }).DisposeWith(disposables);

                // ==========================================
                // 修复：响应 ViewModel 的关闭请求
                // ==========================================
                this.BindInteraction(ViewModel,
                    vm => vm.RequestCloseDialog,
                    interaction =>
                    {
                        // 收到请求，安全关闭当前窗口
                        this.Close();

                        // 标记交互完成
                        interaction.SetOutput(Unit.Default);
                        return Task.CompletedTask;
                    }).DisposeWith(disposables);
            });
        }
    }
}