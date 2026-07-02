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
    /// 生产详情记录视图代码后置 (Code-Behind)
    /// 负责处理独立弹窗的生命周期，以及异常交互管线的安全注册。
    /// </summary>
    public partial class ProductionDetailsView : ReactiveWindow<ProductionDetailsViewModel>
    {
        public ProductionDetailsView()
        {
            InitializeComponent();

            // 视图生命周期激活器，安全绑定管线避免内存泄漏
            this.WhenActivated(disposables =>
            {
                // 动态监听绑定的 ViewModel 变更，确保即使 ViewModel 被延迟赋值也能准确注册交互管线
                this.WhenAnyValue(x => x.ViewModel)
                    .Where(vm => vm != null)
                    .Subscribe(vm =>
                    {
                        // 注册异常弹窗管线：拦截底层查询异常，并拉起子级报错提示框
                        vm!.CommonInteraction.RegisterHandler(DoShowDialogAsync)
                                             .DisposeWith(disposables);
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

                // 以当前生产详情窗口为宿主父级，拉起模态错误提示框
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