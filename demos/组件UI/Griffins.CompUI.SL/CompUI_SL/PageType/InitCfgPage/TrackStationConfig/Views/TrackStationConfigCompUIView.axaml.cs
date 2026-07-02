using Avalonia.Controls;
using Griffins.CompUI.SL.InitCfgPage.ViewModels;

namespace Griffins.CompUI.SL.InitCfgPage.Views;

public partial class TrackStationConfigCompUIView : UserControl
{
    public TrackStationConfigCompUIView()
    {
        InitializeComponent();
        if (Design.IsDesignMode)
        {
            this.DataContext = new TrackStationConfigCompUIViewModel(true, null);
        }
    }
}
