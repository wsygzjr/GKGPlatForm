using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GKG.UI.General;

/// <summary>
/// IO状态量初始化参数配置界面
/// </summary>
public partial class IOStateInitView : UserControl
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public IOStateInitView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
