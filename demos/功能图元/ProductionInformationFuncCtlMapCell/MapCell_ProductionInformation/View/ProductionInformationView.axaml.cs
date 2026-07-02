using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using GKG.Map.ProductionInformationFuncCtlMapCell.ViewModel;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace GKG.Map.ProductionInformationFuncCtlMapCell.View
{
    /// <summary>
    /// 生产信息主界面视图代码后置 (Code-Behind)
    /// 负责承接 ViewModel 的通用弹窗管线请求，并安全地映射到具体的子窗体实例。
    /// </summary>
    public partial class ProductionInformationView : ReactiveUserControl<ProductionInformationViewModel>
    {
        public ProductionInformationView()
        {
            InitializeComponent();

            if (Design.IsDesignMode) return;

            // 视图生命周期激活器，安全绑定管线避免内存泄漏
            this.WhenActivated(disposables =>
            {
                // 监听绑定的 ViewModel 变更，确保动态注册交互管线
                this.WhenAnyValue(x => x.ViewModel)
                    .Where(vm => vm != null)
                    .Subscribe(vm =>
                    {
                        // 注册弹窗管线响应处理器
                        vm!.CommonInteraction.RegisterHandler(ShowDialogAsync).DisposeWith(disposables);
                    })
                    .DisposeWith(disposables);
            });
        }

        /// <summary>
        /// 智能弹窗路由器
        /// 依据接收到的子 ViewModel 类型，动态映射并展示对应的独立窗体视图。
        /// </summary>
        private async Task ShowDialogAsync<TInput, TOutput>(IInteractionContext<TInput, TOutput> context)
        {
            // 通过模式匹配自动派发视图
            Window? dialogView = context.Input switch
            {
                RecipeSelectionViewModel => new RecipeSelectionView(),
                ProductionDetailsViewModel => new ProductionDetailsView(),
                MessageDialogViewModel => new MessageDialogView(),
                MachineStatusRecordViewModel => new MachineStatusRecordView(),
                _ => null
            };

            if (dialogView == null)
            {
                throw new InvalidOperationException($"视图映射失败：未找到视图模型 {context.Input?.GetType().Name} 对应的窗体。");
            }

            dialogView.DataContext = context.Input;

            // 在视觉树中向上溯源获取承载此图元的宿主 Window
            var parentWindow = this.GetVisualRoot() as Window;

            // 安全防线：若图元处于脱离视觉树的状态，安全取消弹窗请求
            if (parentWindow == null)
            {
                System.Diagnostics.Debug.WriteLine($"[警告] 未能获取宿主 Window，弹窗请求已拦截取消。类型: {context.Input?.GetType().Name}");
                context.SetOutput(default(TOutput)!);
                return;
            }

            // 根据业务场景拆分模态与非模态展示逻辑
            if (context.Input is MachineStatusRecordViewModel || context.Input is ProductionDetailsViewModel)
            {
                // 查阅型功能窗体：非模态展示，不阻塞宿主画面的实时更新监控
                dialogView.Show(parentWindow);
                context.SetOutput(default(TOutput)!);
            }
            else
            {
                // 操作型功能窗体 (报警、配方下发确认)：采用模态拦截，锁死背景图元操作
                var result = await dialogView.ShowDialog<TOutput>(parentWindow);
                context.SetOutput(result);
            }
        }
    }
}