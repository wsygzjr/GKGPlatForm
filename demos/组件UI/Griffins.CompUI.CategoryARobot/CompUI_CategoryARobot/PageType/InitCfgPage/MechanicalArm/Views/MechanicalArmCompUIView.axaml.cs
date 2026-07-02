using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Griffins.CompUI.CategoryARobot.CompUI_CategoryARobot.PageType.InitCfgPage.MechanicalArm.ViewModels;

namespace Griffins.CompUI.CategoryARobot.CompUI_CategoryARobot.PageType.InitCfgPage.MechanicalArm.Views;

public partial class MechanicalArmCompUIView : ReactiveUserControl<MechanicalArmCompUIViewModel>
{
    public MechanicalArmCompUIView()
    {
        InitializeComponent();
        DataContext = new MechanicalArmCompUIViewModel(true);
    }
}
