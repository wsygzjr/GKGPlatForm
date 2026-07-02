using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Griffins.CompUI.Calibration.Views;

public partial class CameraVsLaserView : UserControl
{
    public CameraVsLaserView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}