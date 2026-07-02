using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GKG.UI.General;

/// <summary>
/// 测高串口调试 视图
/// </summary>
public partial class HeightSerialPortView : UserControl
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public HeightSerialPortView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}