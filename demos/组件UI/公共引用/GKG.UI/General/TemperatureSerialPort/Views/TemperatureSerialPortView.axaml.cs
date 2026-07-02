using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GKG.UI.General;

/// <summary>
/// 温度控制串口调试 视图
/// </summary>
public partial class TemperatureSerialPortView : UserControl
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public TemperatureSerialPortView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}