using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Griffins.CompUI.SL.InitCfgPage.ViewModels;

namespace Griffins.CompUI.SL.InitCfgPage.Views;

public partial class TrackWideningCompUIView : UserControl
{
    public TrackWideningCompUIView()
    {
        InitializeComponent();
        DataContext = new TrackWideningCompUIViewModel(true, null);
    }
}