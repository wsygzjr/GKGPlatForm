using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using GKG.Map.LogFuncCtlMapCell.ViewModel;

namespace GKG.Map.LogFuncCtlMapCell.View;

public partial class LogView : ReactiveUserControl<LogViewModel>
{
    public LogView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
    }
}