using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Griffins.CompUI.Calibration.Views;

public partial class ZeroDegreevalvePositionView : UserControl
{
    public ZeroDegreevalvePositionView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}