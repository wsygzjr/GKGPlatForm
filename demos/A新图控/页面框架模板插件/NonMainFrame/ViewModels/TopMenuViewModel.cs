using Avalonia.Media.Imaging;
using DynamicData;
using NonMainFrameViewModel.Models;
using NonMainFrameViewModel.ViewModels.Comon;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;

namespace NonMainFrameViewModel.ViewModels.TopMenu;

/// <summary>
/// 顶部菜单栏ViewModel
/// </summary>
public class TopMenuViewModel : ReactiveObject
{
    private string _tileName = "";
    private Bitmap? _gotoParentPageIcon;
    private Bitmap? _gotoRootPageIconBitmap;
    private Action<TopMenuCmdKind> _onTopMenu;
    /// <summary>
    /// 跳转到上级页面图标位图对象
    /// </summary>
    public Bitmap? GotoParentPageIconBitmap
    {
        get => _gotoParentPageIcon;
        set => this.RaiseAndSetIfChanged(ref _gotoParentPageIcon, value);
    }

    /// <summary>
    /// 跳转到首页图标位图对象
    /// </summary>
    public Bitmap? GotoRootPageIconBitmap
    {
        get => _gotoRootPageIconBitmap;
        set => this.RaiseAndSetIfChanged(ref _gotoRootPageIconBitmap, value);
    }
    /// <summary>
    /// 标题名称
    /// </summary>
    public string TileName
    {
        get => _tileName;
        set => this.RaiseAndSetIfChanged(ref _tileName, value);
    }

    /// <summary>
    ///  跳转到首页命令
    /// </summary>
    public ReactiveCommand<Unit, Unit> GotoRootPageCommand { get; }

    /// <summary>
    /// 返回上一页
    /// </summary>
    public ReactiveCommand<Unit, Unit> GotoParentPageCommand { get; }
   
    /// <summary>
    /// 带菜单回调的构造函数
    /// </summary>
    /// <param name="onTopMenu">菜单点击处理回调（不可为null）</param>
    public TopMenuViewModel(Action<TopMenuCmdKind> onTopMenu)
    {
        ArgumentNullException.ThrowIfNull(onTopMenu, nameof(onTopMenu));
        _onTopMenu= onTopMenu;
        GotoParentPageCommand = ReactiveCommand.Create(onGotoParentPage);
        GotoRootPageCommand = ReactiveCommand.Create(onGotoRootPage);
    }

    /// <summary>
    /// 从配置信息填充到VM
    /// </summary>
    /// <param name="topMenuCfgInfoes">顶部菜单配置信息</param>
    public void FillToVM(GeneralConfigInfo generalConfigInfo)
    {
        GotoParentPageIconBitmap = generalConfigInfo.GotoParentPageIcon;
        GotoRootPageIconBitmap = generalConfigInfo.GotoRootPageIcon;
        this.TileName = generalConfigInfo.TileName;

    }
    private void onGotoRootPage()
    {
        _onTopMenu?.Invoke(TopMenuCmdKind.GotoRootPage);
    }

    private void onGotoParentPage()
    {
        _onTopMenu?.Invoke(TopMenuCmdKind.GotoParentPage);
    }
}
