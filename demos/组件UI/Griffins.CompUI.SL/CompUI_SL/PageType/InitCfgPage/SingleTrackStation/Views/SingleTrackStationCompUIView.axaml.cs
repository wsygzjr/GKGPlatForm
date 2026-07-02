using Avalonia.Controls;
using Griffins.CompUI.SL.InitCfgPage.ViewModels;

namespace Griffins.CompUI.SL.InitCfgPage.Views;

public partial class SingleTrackStationCompUIView : UserControl
{
    public SingleTrackStationCompUIView()
    {
        InitializeComponent();
        DataContext = new SingleTrackStationCompUIViewModel(true, null);
    }
}