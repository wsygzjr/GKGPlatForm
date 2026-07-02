using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.ViewModel;

namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.View;

public partial class TestFactoryCfgCompUIView : ReactiveUserControl<TestFactoryCfgCompUIViewModel>
{
    public TestFactoryCfgCompUIView()
    {
        InitializeComponent();
    }
}