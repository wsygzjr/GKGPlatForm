using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GKG.UI.General;
/// <summary>
/// 气缸延时配置界面
/// </summary>
public partial class CylinderDelayView : UserControl
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public CylinderDelayView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}