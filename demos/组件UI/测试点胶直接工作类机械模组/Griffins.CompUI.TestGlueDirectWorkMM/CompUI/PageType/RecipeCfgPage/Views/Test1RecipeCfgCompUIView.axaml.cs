using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModel;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.View;

public partial class Test1RecipeCfgCompUIView : ReactiveUserControl<Test1RecipeCfgCompUIViewModel>
{
    public Test1RecipeCfgCompUIView()
    {
        InitializeComponent();
    }
}