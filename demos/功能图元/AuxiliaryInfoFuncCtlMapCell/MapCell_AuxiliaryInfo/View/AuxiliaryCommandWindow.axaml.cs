using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using GKG.Map.AuxiliaryInfoFuncCtlMapCell.ViewModel;

namespace GKG.Map.AuxiliaryInfoFuncCtlMapCell.View;

public partial class AuxiliaryCommandWindow : ReactiveWindow<AuxiliaryCommandViewModel>
{
    public AuxiliaryCommandWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
