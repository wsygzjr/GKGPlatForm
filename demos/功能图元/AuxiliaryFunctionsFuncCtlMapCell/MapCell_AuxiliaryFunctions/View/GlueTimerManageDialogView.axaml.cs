using Avalonia.ReactiveUI;
using GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.ViewModel;
using ReactiveUI;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.View
{
    public partial class GlueTimerManageDialogView : ReactiveWindow<GlueTimerManageViewModel>
    {
        public GlueTimerManageDialogView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                if (ViewModel != null)
                {
                    // 1. 订阅关闭事件，并将 true/false 作为 DialogResult 返回给调用方
                    ViewModel.CloseAction = (result) =>
                    {
                        this.Close(result);
                    };

                    // 2. 窗口激活时，异步加载数据
                    // 使用 RxApp.MainThreadScheduler 确保在 UI 线程外启动，不卡界面
                    RxApp.MainThreadScheduler.Schedule( async () =>
                    {
                        await ViewModel.InitDataAsync();
                    });
                }
            });
        }
    }
}