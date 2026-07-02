using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GKG.UI.General;

/// <summary>
/// 通用外接扫码器视图
/// </summary>
public partial class ExternalScannerView : UserControl
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public ExternalScannerView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}