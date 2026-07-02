using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Griffins.CompUI.SL.InitCfgPage.ViewModels;

namespace Griffins.CompUI.SL.InitCfgPage.Views;

public partial class TransportMotorCompUIView : ReactiveUserControl<TransportMotorCompUIViewModel>
{
    public TransportMotorCompUIView()
    {
        InitializeComponent();
        DataContext = new TransportMotorCompUIViewModel(true, null);
    }
}