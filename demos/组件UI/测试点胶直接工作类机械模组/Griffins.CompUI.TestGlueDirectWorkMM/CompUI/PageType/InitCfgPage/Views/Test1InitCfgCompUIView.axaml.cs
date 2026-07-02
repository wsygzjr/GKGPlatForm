using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.ViewModel;

namespace Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.View;

public partial class Test1InitCfgCompUIView : ReactiveUserControl<Test1InitCfgCompUIViewModel>
{
    public Test1InitCfgCompUIView()
    {
        InitializeComponent();
    }
}