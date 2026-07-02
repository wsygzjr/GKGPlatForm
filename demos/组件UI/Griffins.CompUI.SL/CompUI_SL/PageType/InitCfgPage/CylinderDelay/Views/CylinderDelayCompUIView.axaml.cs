using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Griffins.CompUI.SL.InitCfgPage.ViewModels;

namespace Griffins.CompUI.SL.InitCfgPage.Views;

/// <summary>
/// 气缸延时组件UI视图
/// </summary>
public partial class CylinderDelayCompUIView : UserControl
{
    #region 构造函数

    /// <summary>
    /// 构造函数
    /// </summary>
    public CylinderDelayCompUIView()
    {
        InitializeComponent();
        DataContext = new CylinderDelayCompUIViewModel(true, null);
    }

    #endregion
}