using Avalonia.ReactiveUI;
using GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.ViewModel;
using ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.View
{
    public partial class TipShowWindowView : ReactiveWindow<TipShowWindowViewModel>
    {
        public TipShowWindowView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // 当 ViewModel 里的 CancelCommand 被触发时，关闭物理窗体
                ViewModel.CancelCommand.Subscribe(new Action<System.Reactive.Unit>(_ =>
                {
                    this.Close();
                })).DisposeWith(disposables);
            });
        }
    }
}