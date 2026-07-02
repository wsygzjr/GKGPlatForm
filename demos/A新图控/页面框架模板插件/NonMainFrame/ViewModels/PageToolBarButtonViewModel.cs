using Avalonia.Media.Imaging;
using DynamicData;
using Griffins.Map;
using Griffins.Map.UI;
using NonMainFrameViewModel.ViewModels.Comon;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;

namespace NonMainFrameViewModel.ViewModels.ToolbarMenu;

/// <summary>
/// 页面工具栏视图模型
/// </summary>
public class PageToolBarButtonViewModel : ReactiveObject
{
    private ObservableCollection<PageToolBarButtonInfo> _toolbarMenuItems = new ObservableCollection<PageToolBarButtonInfo>();
    /// <summary>
    /// 快捷工具栏项集合
    /// </summary>
    public ObservableCollection<PageToolBarButtonInfo> ToolbarMenuItems
    {
        get => _toolbarMenuItems;
        set => this.RaiseAndSetIfChanged(ref _toolbarMenuItems, value);
    }

    /// <summary>
    /// 工具栏命令
    /// </summary>
    public ReactiveCommand<string, Unit> ToolbarCommand { get; } = ReactiveCommand.Create<string>(_ => { });

    /// <summary>
    /// 无参构造函数
    /// </summary>
    public PageToolBarButtonViewModel() { }

    /// <summary>
    /// 带回调的构造函数
    /// </summary>
    /// <param name="onToolbar">工具栏按钮点击回调（不可为null）</param>
    public PageToolBarButtonViewModel(Action<string> onToolbar)
    {
        ToolbarCommand = ReactiveCommand.Create<string>(onToolbar);

        // 初始触发第一个按钮的命令（如果有）
        if (ToolbarMenuItems.Count > 0)
        {
            onToolbar(ToolbarMenuItems[0].ButtonID);
        }
    }

    /// <summary>
    /// 从配置信息填充到VM
    /// </summary>
    /// <param name="MenuButtons">工具栏按钮列表</param>
    public void FillToVM(PageToolBarButtonInfoList? MenuButtons)
    {
        ToolbarMenuItems.Clear();

        if (MenuButtons != null && MenuButtons.Any())
        {
            ToolbarMenuItems.AddRange(MenuButtons);
        }
    }
}
