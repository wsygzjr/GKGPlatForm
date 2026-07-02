using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Griffins.CompUI.SL.InitCfgPage.ViewModels;

namespace Griffins.CompUI.SL.InitCfgPage.Views;

public partial class TrackBasicParamCompUIView : ReactiveUserControl<TrackBasicParamCompUIViewModel>
{
    public TrackBasicParamCompUIView()
    {
        InitializeComponent();
        DataContext = new TrackBasicParamCompUIViewModel(true, null);
    }
}