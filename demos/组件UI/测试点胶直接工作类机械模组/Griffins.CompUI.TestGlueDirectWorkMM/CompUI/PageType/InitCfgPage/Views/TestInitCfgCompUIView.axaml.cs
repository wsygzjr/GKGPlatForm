using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.ViewModel;

namespace Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.View;

public partial class TestInitCfgCompUIView : ReactiveUserControl<TestInitCfgCompUIViewModel>
{
    public TestInitCfgCompUIView()
    {
        InitializeComponent();
    }
}