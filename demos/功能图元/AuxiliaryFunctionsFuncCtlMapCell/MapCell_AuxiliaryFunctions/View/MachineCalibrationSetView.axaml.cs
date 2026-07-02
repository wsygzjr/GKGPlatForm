using Avalonia.ReactiveUI;
using GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.ViewModel;
using ReactiveUI;
using System.Reactive.Disposables;

namespace GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.View
{
    public partial class MachineCalibrationSetView : ReactiveWindow<MachineCalibrationSetViewModel>
    {
        public MachineCalibrationSetView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                if (ViewModel != null)
                {
                    // 接收 ViewModel 发来的关闭信号
                    ViewModel.CloseAction = () => this.Close();
                }
            });
        }
    }
}