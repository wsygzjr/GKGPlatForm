using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Griffins.CompUI.Calibration.Views;

public partial class CameraPositionView : UserControl
{
    public CameraPositionView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}