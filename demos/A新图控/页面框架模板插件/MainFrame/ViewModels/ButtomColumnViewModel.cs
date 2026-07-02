using Avalonia.Layout;
using Avalonia.Threading;
using Griffins.Map.UI;
using MainFrame.Models;
using ReactiveUI;
using System;
using System.Timers;

namespace MainFrame.ViewModels;

/// <summary>
/// 底部栏ViewModel（显示系统时间和程序地址等信息）
/// </summary>
public class ButtomColumnViewModel : ReactiveObject, IWorkAreaContentUpdater
{

    //底部工作区信息
    private WorkAreaInfo? _workAreaInfo;

    /// <summary>
    /// 命令执行策略
    /// </summary>
    private ICommandExecutionStrategy _commandStrategy;

    private object? _currentContent = null!;
    /// <summary>
    /// 工作区内容
    /// </summary>
    public object? CurrentContent
    {
        get => _currentContent;
        set => this.RaiseAndSetIfChanged(ref _currentContent, value);
    }

    private HorizontalAlignment _horizontalContentAlignment;
    /// <summary>
    ///工作区内部子内容在自身的水平对齐
    /// </summary>
    public HorizontalAlignment HorizontalContentAlignment
    {
        get => _horizontalContentAlignment;
        set => _horizontalContentAlignment = value;
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
    /// 构造函数
    /// </summary>
    public ButtomColumnViewModel()
    {
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
        _commandStrategy = new RuntimeCommandStrategy(this, runtimeCallback, null);
        if (_workAreaInfo != null)
            _commandStrategy.ShowWorkAreaInfo(_workAreaInfo);
    }

    /// <summary>
    /// 从配置信息填充到VM
    /// </summary>
    /// <param name="workAreaInfo">底部菜单配置信息</param>
    public void FillToVM(WorkAreaInfo workAreaInfo, bool isRunTime = false)
    {
        this._workAreaInfo = workAreaInfo;
        var workAreaCfgInfo = WorkAreaCfgInfo.FromJSonBytes(workAreaInfo.CfgInfo);
        HorizontalContentAlignment = workAreaCfgInfo.HorizontalContentAlignment;
        VerticalContentAlignment = workAreaCfgInfo.VerticalContentAlignment;

        //默认加载设计时配置的区域：显示文本
        if (!isRunTime)
            _commandStrategy.ShowWorkAreaInfo(workAreaInfo);
    }

    /// <summary>
    /// 实现IWorkAreaContentUpdater接口，供策略更新工作区内容
    /// </summary>
    void IWorkAreaContentUpdater.UpdateWorkAreaContent(object? content)
    {
        CurrentContent = content;
    }
}
