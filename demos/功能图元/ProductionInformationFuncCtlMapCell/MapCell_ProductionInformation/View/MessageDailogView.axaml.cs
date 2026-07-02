using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using GKG.Map.ProductionInformationFuncCtlMapCell.ViewModel;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace GKG.Map.ProductionInformationFuncCtlMapCell.View
{
    /// <summary>
    /// 通用消息弹窗视图代码后置 (Code-Behind)
    /// 负责处理弹窗的绝对定位遮罩计算，以及安全接管 ViewModel 的交互状态流。
    /// </summary>
    public partial class MessageDialogView : ReactiveWindow<MessageDialogViewModel>
    {
        public MessageDialogView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // 无论 ViewModel 是立刻注入还是延迟注入，此管道都能 100% 精准接管其事件
                this.WhenAnyValue(x => x.ViewModel)
                    .Where(vm => vm != null)
                    .Subscribe(vm =>
                    {
                        // 核心闭环控制：将三个按钮的命令流合并，并转换为布尔值结果。
                        // 订阅命中后，触发 this.Close 彻底终结弹窗生命周期并回传结果。
                        Observable.Merge(
                            vm!.YesCommand.Select(_ => true),
                            vm.OkCommand.Select(_ => true),
                            vm.NoCommand.Select(_ => false)
                        )
                        .Subscribe(result => this.Close(result))
                        .DisposeWith(disposables);
                    })
                    .DisposeWith(disposables);
            });
        }

        /// <summary>
        /// 当窗口打开时触发：执行完美自适应与防裁剪机制
        /// 拦截生命周期，强制计算自身的边界大小和屏幕绝对坐标。
        /// </summary>
        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);

            try
            {
                if (this.Owner is Window currentOwner)
                {
                    Window targetWindow = currentOwner;

                    // 智能逃逸机制：若父窗口过小，强行在此容器内弹出可能导致严重形变或截断
                    if (currentOwner.Bounds.Width < 400 || currentOwner.Bounds.Height < 300)
                    {
                        // 采用 C# 9+ 模式匹配语法糖，安全、优雅地提取全局主窗口实例
                        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime
                            && lifetime.MainWindow != null
                            && lifetime.MainWindow != currentOwner)
                        {
                            targetWindow = lifetime.MainWindow;
                            System.Diagnostics.Debug.WriteLine("[架构提示] 父级视口过小，MessageDialog 已智能逃逸至全局主窗口。");
                        }
                    }

                    // 修正尺寸：准确获取父窗口内部可用内容区（刨去系统边框与标题栏）
                    this.Width = targetWindow.ClientSize.Width;
                    this.Height = targetWindow.ClientSize.Height;

                    // 修正坐标：将父窗口可用内容区的左上角相对坐标 (0,0) 映射为屏幕绝对物理坐标
                    this.Position = targetWindow.PointToScreen(new Avalonia.Point(0, 0));
                }
            }
            catch (Exception ex)
            {
                // 健壮性防御：防止因操作系统 DPI 缩放异构或句柄未完全就绪导致的位置计算异常
                System.Diagnostics.Debug.WriteLine($"[视图警告] 弹窗自适应定位计算发生异常：{ex.Message}");
            }
        }
    }
}