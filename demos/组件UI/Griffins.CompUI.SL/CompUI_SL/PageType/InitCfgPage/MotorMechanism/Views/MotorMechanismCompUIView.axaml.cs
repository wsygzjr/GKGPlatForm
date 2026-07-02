using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Griffins.CompUI.SL.InitCfgPage.ViewModels;

namespace Griffins.CompUI.SL.InitCfgPage.Views;

public partial class MotorMechanismCompUIView : UserControl
{
    public MotorMechanismCompUIView()
    {
        InitializeComponent();
        DataContext = new MotorMechanismCompUIViewModel(true, null);
    }
}