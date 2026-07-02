using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Griffins.CompUI.SL.RecipeCfgPage.ViewModels;

namespace Griffins.CompUI.SL.RecipeCfgPage.Views;

public partial class MotorSpeedCompUIView : UserControl
{
    public MotorSpeedCompUIView()
    {
        InitializeComponent();
        DataContext = new MotorSpeedCompUIViewModel(true, null);
    }
}