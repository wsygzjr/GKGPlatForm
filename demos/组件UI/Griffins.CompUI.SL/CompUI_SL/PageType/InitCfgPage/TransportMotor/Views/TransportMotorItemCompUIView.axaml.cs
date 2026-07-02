using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Griffins.CompUI.SL.InitCfgPage.ViewModels;

namespace Griffins.CompUI.SL.InitCfgPage.Views;

public partial class TransportMotorItemCompUIView : ReactiveUserControl<TransportMotorItemCompUIViewModel>
{
    public TransportMotorItemCompUIView()
    {
        InitializeComponent();
        DataContext = new TransportMotorItemCompUIViewModel(true, null);
    }
}