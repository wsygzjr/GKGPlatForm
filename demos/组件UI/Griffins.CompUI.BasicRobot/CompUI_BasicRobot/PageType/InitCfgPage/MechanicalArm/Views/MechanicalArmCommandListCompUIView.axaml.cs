using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Griffins.CompUI.BasicRobot.CompUI_BasicRobot.PageType.InitCfgPage.MechanicalArm.ViewModels;

namespace Griffins.CompUI.BasicRobot.CompUI_BasicRobot.PageType.InitCfgPage.MechanicalArm.Views;

public partial class MechanicalArmCommandListCompUIView : ReactiveUserControl<MechanicalArmCompUIViewModel>
{
    public MechanicalArmCommandListCompUIView()
    {
        InitializeComponent();
    }
}
