using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Griffins.CompUI.TestGlueDirectWorkMM.DebugPage.ViewModel;

namespace Griffins.CompUI.TestGlueDirectWorkMM.DebugPage.View;

public partial class Test1DebugCompUIView : ReactiveUserControl<Test1DebugCompUIViewModel>
{
    public Test1DebugCompUIView()
    {
        InitializeComponent();
    }
}