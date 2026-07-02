using Avalonia.ReactiveUI;
using GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.ViewModel;
using ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.View
{
    public partial class GlueMonitorView : ReactiveWindow<GlueMonitorViewModel>
    {
        public GlueMonitorView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // 将 ViewModel 中的 NextStepCommand 绑定到底部按钮
                this.BindCommand(ViewModel, vm => vm.NextStepCommand, v => v.NextStepButton)
                    .DisposeWith(disposables);

                // 当 NextStepCommand 成功执行后，关闭当前窗口
                ViewModel.NextStepCommand.Subscribe(_ =>
                {
                    this.Close();
                }).DisposeWith(disposables);
            });
        }
    }
}