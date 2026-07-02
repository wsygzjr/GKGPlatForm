using Avalonia.ReactiveUI;
using GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.ViewModel;
using ReactiveUI;

namespace GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.View
{
    public partial class OutGlueWaxSetWindowView : ReactiveWindow<OutGlueWaxSetViewModel>
    {
        public OutGlueWaxSetWindowView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                if (ViewModel != null)
                {
                    // 将界面的 Close 方法丢给 ViewModel 去调用
                    ViewModel.CloseAction = () => this.Close();
                }
            });
        }
    }
}