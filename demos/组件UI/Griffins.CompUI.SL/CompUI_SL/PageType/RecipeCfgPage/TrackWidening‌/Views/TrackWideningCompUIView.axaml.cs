using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Griffins.CompUI.SL.RecipeCfgPage.ViewModels;

namespace Griffins.CompUI.SL.RecipeCfgPage.Views;

public partial class TrackWideningCompUIView : UserControl
{
    public TrackWideningCompUIView()
    {
        InitializeComponent();
        DataContext = new TrackWideningCompUIViewModel(true, null);
    }
}