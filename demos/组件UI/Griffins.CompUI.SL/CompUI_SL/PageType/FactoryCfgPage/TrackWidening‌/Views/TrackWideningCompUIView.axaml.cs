using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Griffins.CompUI.SL.FactoryCfgPage.ViewModels;

namespace Griffins.CompUI.SL.FactoryCfgPage.Views;

public partial class TrackWideningCompUIView : UserControl
{
    public TrackWideningCompUIView()
    {
        InitializeComponent();
        DataContext = new TrackWideningCompUIViewModel(true, null);
    }
}