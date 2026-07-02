using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Griffins.CompUI.Calibration.Views;

public partial class CameraVsGluevalveComView : UserControl
{
    public CameraVsGluevalveComView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}