using Avalonia.ReactiveUI;
using Griffins.CompUI.GD.InitCfgPage.ViewModels;

namespace Griffins.CompUI.GD.InitCfgPage.Views
{
    /// <summary>
    /// 单层轨道工位视图
    /// </summary>
    public partial class SingleTrackStationCompUIView : ReactiveUserControl<SingleTrackStationCompUIViewModel>
    {
        public SingleTrackStationCompUIView()
        {
            InitializeComponent();
            DataContext = new SingleTrackStationCompUIViewModel(true, null);
        }
    }
}
