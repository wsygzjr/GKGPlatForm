using Avalonia.Layout;
using Avalonia.Media.Imaging;
using Griffins.Map.UI;
using MainFrame.Models;
using ReactiveUI;
using System.Reactive;

namespace MainFrame.ViewModels;

/// <summary>
/// 顶部菜单栏ViewModel
/// </summary>
public class TopMenuViewModel : ReactiveObject, IWorkAreaContentUpdater
{
    private object? _currentContent = null!;
    private Bitmap? _minimizeIcon;
    private Bitmap? _closeIcon;
    private bool _isMinimizeButtonEnabled;
    //顶部工作区信息
    private WorkAreaInfo? _workAreaInfo;
    /// <summary>
    /// 命令执行策略
    /// </summary>
    private ICommandExecutionStrategy _commandStrategy;
    /// <summary>
    /// 工作区内容
    /// </summary>
    public object? CurrentContent
    {
        get => _currentContent;
        set => this.RaiseAndSetIfChanged(ref _currentContent, value);
    }
    /// <summary>
    /// 最小化按钮图标位图
    /// </summary>
    public Bitmap? MinimizeIcon
    {
        get => _minimizeIcon;
        set => this.RaiseAndSetIfChanged(ref _minimizeIcon, value);
    }
    /// <summary>
    /// 关闭按钮图标位图
    /// </summary>
    public Bitmap? CloseIcon
    {
        get => _closeIcon;
        set => this.RaiseAndSetIfChanged(ref _closeIcon, value);
    }
    /// <summary>
    /// 最小化按钮启用状态
    /// </summary>
    public bool IsMinimizeButtonEnabled
    {
        get => _isMinimizeButtonEnabled;
        set => this.RaiseAndSetIfChanged(ref _isMinimizeButtonEnabled, value);
    }
    private HorizontalAlignment _horizontalContentAlignment;

    /// <summary>
    ///工作区内部子内容在自身的水平对齐
    /// </summary>
    public HorizontalAlignment HorizontalContentAlignment
    {
        get => _horizontalContentAlignment;
        set => _horizontalContentAlignment=value;
    }
    private VerticalAlignment _verticalContentAlignment;
    /// <summary>
    /// 工作区内部子内容在自身的垂直对齐
    /// </summary>
    public VerticalAlignment VerticalContentAlignment
    {
        get => _verticalContentAlignment;
        set => _verticalContentAlignment = value;
    }
    /// <summary>
    /// 关闭窗口命令
    /// </summary>
    public ReactiveCommand<Unit, Unit> CloseWindowCommand { get; }

    /// <summary>
    /// 最小化窗口命令
    /// </summary>
    public ReactiveCommand<Unit, Unit> MinimizeWinowWindowCommand { get; }

    /// <summary>
    /// 无参构造函数
    /// </summary>
    public TopMenuViewModel()
    {
        MinimizeWinowWindowCommand = ReactiveCommand.Create(onMinimizeWinowWindow);
        CloseWindowCommand = ReactiveCommand.Create(onCloseWindow);

        // 初始化设计时策略
        _commandStrategy = new DesignTimeCommandStrategy(this);
    }
    /// <summary>
    /// 设置运行时回调和策略
    /// </summary>
    /// <param name="runtimeCallback">运行时回调接口（不可为null）</param>
    public void SetRuntimeCallback(IPageFrameTemplateRunTimeCallBack runtimeCallback)
    {
        // 运行时策略并传递工作区更新器
        _commandStrategy = new RuntimeCommandStrategy(this, runtimeCallback,null);
        if(_workAreaInfo!=null)
            _commandStrategy.ShowWorkAreaInfo(_workAreaInfo);
    }
    /// <summary>
    /// 从配置信息填充到VM
    /// </summary>
    /// <param name="menuButton">顶部菜单配置信息</param>
    public void FillToVM(WorkAreaInfo workAreaInfo, GeneralConfigInfo generalConfigInfo, bool isRunTime = false)
    {
        this._workAreaInfo = workAreaInfo;
        var workAreaCfgInfo = WorkAreaCfgInfo.FromJSonBytes(workAreaInfo.CfgInfo);
        HorizontalContentAlignment = workAreaCfgInfo.HorizontalContentAlignment;
        VerticalContentAlignment = workAreaCfgInfo.VerticalContentAlignment;
        CloseIcon = generalConfigInfo.CloseIcon;
        MinimizeIcon = generalConfigInfo.MinimizeIcon;
        IsMinimizeButtonEnabled = generalConfigInfo.IsMinimizeButtonEnabled;
        //默认加载设计时配置的区域：显示文本
        if (!isRunTime)
            _commandStrategy.ShowWorkAreaInfo(workAreaInfo);
    }

    private void onCloseWindow()
    {
        _commandStrategy.HandleTopMenuCommand(TopMenuCmdKind.CloseWindow);
    }

    private void onMinimizeWinowWindow()
    {
        _commandStrategy.HandleTopMenuCommand(TopMenuCmdKind.MinimizeWinow);
    }
    /// <summary>
    /// 实现IWorkAreaContentUpdater接口，供策略更新工作区内容
    /// </summary>
    void IWorkAreaContentUpdater.UpdateWorkAreaContent(object? content)
    {
        CurrentContent = content;
    }
}
