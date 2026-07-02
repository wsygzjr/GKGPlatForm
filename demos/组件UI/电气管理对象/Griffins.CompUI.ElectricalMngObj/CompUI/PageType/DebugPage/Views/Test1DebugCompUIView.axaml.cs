using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Griffins.CompUI.ElectricalMngObj.DebugPage.ViewModel;

namespace Griffins.CompUI.ElectricalMngObj.DebugPage.View;

public partial class Test1DebugCompUIView : ReactiveUserControl<Test1DebugCompUIViewModel>
{
    public Test1DebugCompUIView()
    {
        InitializeComponent();
    }
}