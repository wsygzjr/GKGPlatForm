using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Griffins.CompUI.Calibration.ViewModels;

namespace Griffins.CompUI.Calibration.Views;

public partial class LaserAltimetryCalibrationView : UserControl
{
    public LaserAltimetryCalibrationView()
    {
        InitializeComponent();

        //// 关键：在 Loaded 事件中读取资源（避免构造函数时机过早）
        //Loaded += LaserAltimetryCalibrationView_Loaded;
    }

    private void LaserAltimetryCalibrationView_Loaded(object? sender, RoutedEventArgs e)
    {
        // 1. 读取 View 自身 Resources 中的 IconPath_Circle 字符串
        string iconPathCircle = string.Empty;
        if (Resources.TryGetResource("IconPath_Delete", ActualThemeVariant, out object? value) && value is string pathStr)
        {
            iconPathCircle = pathStr;
        }

        // 2. 获取 ViewModel 并传递资源字符串
        if (DataContext is LaserAltimetryCalibrationViewModel vm)
        {
            vm.IconPathCircle = StreamGeometry.Parse(iconPathCircle); // 赋值给 ViewModel
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}