using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Griffins.CompUI.TestGlueDirectWorkMM.FactoryCfgPage.ViewModel;

namespace Griffins.CompUI.TestGlueDirectWorkMM.FactoryCfgPage.View;

public partial class TestFactoryCfgCompUIView : ReactiveUserControl<TestFactoryCfgCompUIViewModel>
{
    public TestFactoryCfgCompUIView()
    {
        InitializeComponent();
    }
}