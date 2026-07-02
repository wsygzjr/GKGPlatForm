using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.ViewModel;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.View;

public partial class WaitingBoxView : ReactiveWindow<WaitingBoxViewModel>
{
    public WaitingBoxView()
    {
        InitializeComponent();

        // 1. ReactiveUI 标准绑定
        this.WhenActivated(disposables =>
        {
            this.Bind(ViewModel, vm => vm.Message, v => v.MessageTextBlock.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.ProgressValue, v => v.BottomProgressBar.Value)
                .DisposeWith(disposables);
        });

        // 2. 窗口显示后，立刻触发 ViewModel 的任务
        this.Opened += async (s, e) =>
        {
            if (ViewModel != null)
            {
                await ViewModel.StartWorkAsync();
                // 任务跑完后，自我关闭
                this.Close();
            }
        };
    }

    // 3. 拦截特定的键盘操作
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        // TODO: 替换为你原本的 GlobalData 判断逻辑
        // if (GlobalData.bAutoRun) return;

        switch (e.Key)
        {
            case Key.F8:
                // GlobalData.wndMain.QuitUILevel();
                e.Handled = true; // 标记为已处理，拦截按键
                break;
            case Key.F2:
                // GlobalData.wndMain.OutGlue_F2F3(EnumValve.MainValve);
                e.Handled = true;
                break;
            case Key.F3:
                // GlobalData.wndMain.OutGlue_F2F3(EnumValve.ViceValve);
                e.Handled = true;
                break;
        }
    }

    /// <summary>
    /// 提供给外部调用的现代 Show 异步方法
    /// </summary>
    public static async Task ShowAsync(Window owner, Func<Task> action, string message = "有一种幸福，叫做等待...")
    {
        // 多语言逻辑可以提取到这里
        // if (mes == "请稍候..." && ...) { message = ...; }

        var vm = new WaitingBoxViewModel(action, message);
        var dialog = new WaitingBoxView { ViewModel = vm };

        // 以模态方式展示，并等待窗口关闭
        await dialog.ShowDialog(owner);
    }
}