using Avalonia.ReactiveUI;
using Griffins.CompUI.SL.InitCfgPage.ViewModels;

namespace Griffins.CompUI.SL.InitCfgPage.Views
{
    /// <summary>
    /// 单层轨道工位视图
    /// </summary>
    public partial class SingleTrackStationItemCompUIView : ReactiveUserControl<SingleTrackStationItemCompUIViewModel>
    {
        public SingleTrackStationItemCompUIView()
        {
            InitializeComponent();
            // Do not set DataContext here - it should be provided by parent or design-time XAML
        }
    }
}
