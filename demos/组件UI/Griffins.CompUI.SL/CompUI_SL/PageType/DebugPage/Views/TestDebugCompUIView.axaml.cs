using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Griffins.CompUI.SL.DebugPage.ViewModel;

namespace Griffins.CompUI.SL.DebugPage.View;

public partial class TestDebugCompUIView : ReactiveUserControl<TestDebugCompUIViewModel>
{
    public TestDebugCompUIView()
    {
        InitializeComponent();
    }
}