using Avalonia.ReactiveUI;
using GKG.Map.ProductionInformationFuncCtlMapCell.ViewModel;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace GKG.Map.ProductionInformationFuncCtlMapCell.View
{
    /// <summary>
    /// 配方选择弹窗视图代码后置 (Code-Behind)
    /// 负责处理弹窗的生命周期、通用报警交互管线以及自身安全关闭逻辑。
    /// </summary>
    public partial class RecipeSelectionView : ReactiveWindow<RecipeSelectionViewModel>
    {
        public RecipeSelectionView()
        {
            InitializeComponent();

            // 视图生命周期激活器，安全绑定管线避免内存泄漏
            this.WhenActivated(disposables =>
            {
                // 监听绑定的 ViewModel 变更，确保动态注册交互管线
                this.WhenAnyValue(x => x.ViewModel)
                    .Where(vm => vm != null)
                    .Subscribe(vm =>
                    {
                        // 1. 注册错误弹窗管线：拦截下发断网或内部异常，并拉起子级报错提示框
                        vm!.CommonInteraction.RegisterHandler(DoShowDialogAsync)
                                             .DisposeWith(disposables);

                        // 2. 注册窗体关闭管线：接收到 ViewModel 业务完成信号后，安全销毁当前窗体
                        vm.CloseDialog.RegisterHandler(context =>
                        {
                            this.Close(context.Input);
                            context.SetOutput(System.Reactive.Unit.Default);
                        }).DisposeWith(disposables);
                    })
                    .DisposeWith(disposables);
            });
        }

        /// <summary>
        /// 拦截并显示独立的系统异常提示框
        /// </summary>
        private async Task DoShowDialogAsync(IInteractionContext<ReactiveObject, bool> context)
        {
            if (context.Input is MessageDialogViewModel msgVM)
            {
                var dialog = new MessageDialogView
                {
                    DataContext = msgVM
                };

                // 以当前配方选择窗口为宿主父级，拉起模态错误提示框
                var result = await dialog.ShowDialog<bool>(this);
                context.SetOutput(result);
            }
            else
            {
                context.SetOutput(false);
            }
        }
    }
}