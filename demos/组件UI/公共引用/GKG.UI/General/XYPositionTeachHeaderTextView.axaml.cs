using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GKG.UI;

namespace GKG.UI.General;

public partial class XYPositionTeachHeaderTextView : UserControl
{
    public XYPositionTeachHeaderTextView()
    {
        InitializeComponent();
        if (this.FindControl<TextBlockControl>("XHeaderLabel") is { } xLbl)
            xLbl.ViewModel = new TextBlockViewModel { Text = "X（mm）" };
        if (this.FindControl<TextBlockControl>("YHeaderLabel") is { } yLbl)
            yLbl.ViewModel = new TextBlockViewModel { Text = "Y（mm）" };
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}