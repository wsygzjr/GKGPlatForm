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
    /// 机台状态统计记录视图代码后置 (Code-Behind)
    /// 继承自 ReactiveWindow 以提供强类型的 ViewModel 绑定。
    /// 负责处理窗体生命周期，并在安全的响应式管线中注册弹窗路由交互。
    /// </summary>
    public partial class MachineStatusRecordView : ReactiveWindow<MachineStatusRecordViewModel>
    {
        /// <summary>
        /// 实例化机台状态统计视图
        /// </summary>
        public MachineStatusRecordView()
        {
            InitializeComponent();

            // 视图生命周期激活器，安全绑定管线避免内存泄漏
            this.WhenActivated(disposables =>
            {
                // 确保无论是构造时注入，还是后续由外部路由器延迟赋值，管线都能被 100% 准确拦截并注册。
                this.WhenAnyValue(x => x.ViewModel)
                    .Where(vm => vm != null)
                    .Subscribe(vm =>
                    {
                        // 注册弹窗异常管线：拦截底层查询发生的异常，并挂载提示子窗体
                        vm!.CommonInteraction.RegisterHandler(DoShowDialogAsync)
                                             .DisposeWith(disposables);
                    })
                    .DisposeWith(disposables);
            });
        }

        /// <summary>
        /// 拦截并显示独立的系统异常或业务提示框
        /// </summary>
        private async Task DoShowDialogAsync(IInteractionContext<ReactiveObject, bool> context)
        {
            // 通过模式匹配自动派发业务视图
            if (context.Input is MessageDialogViewModel msgVM)
            {
                var dialog = new MessageDialogView
                {
                    DataContext = msgVM
                };

                // 以当前机台状态窗体 (this) 为宿主父级，拉起带灰色遮罩的模态错误提示框
                var result = await dialog.ShowDialog<bool>(this);

                context.SetOutput(result);
            }
            else
            {
                // 安全兜底：遇到无法处理的 ViewModel 类型时静默放行，避免死锁
                context.SetOutput(false);
            }
        }
    }
}