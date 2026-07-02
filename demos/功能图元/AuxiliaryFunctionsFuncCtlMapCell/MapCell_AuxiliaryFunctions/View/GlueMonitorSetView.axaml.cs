using Avalonia.ReactiveUI;
using GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.ViewModel;
using ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.View
{
    public partial class GlueMonitorSetView : ReactiveWindow<GlueMonitorSetViewModel>
    {
        public GlueMonitorSetView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                if (ViewModel != null)
                {
                    // 订阅 ViewModel 发出的关闭信号
                    ViewModel.CloseAction = (result) =>
                    {
                        // 这里的 result 可以在主界面通过 await dialog.ShowDialog 接收
                        this.Close(result);
                    };

                    // 初始化拉取硬件/后台数据
                    // 建议在真实的异步环境中使用 RxApp.MainThreadScheduler 防止卡顿
                    ViewModel.InitDataAsync();
                }
            });
        }
    }
}