using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Griffins.CompUI.Calibration.Views;

public partial class CameraVsGluevalveView : UserControl
{
    public CameraVsGluevalveView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}