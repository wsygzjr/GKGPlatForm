using DynamicData;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;

namespace NonMainFrameViewModel.ViewModels.WorkArea;

/// <summary>
/// 工作区显示VM
/// </summary>
public class WorkAreaViewModel : ReactiveObject
{
    private object? _currentContent = null!;
    /// <summary>
    /// 工作区内容
    /// </summary>
    public object? CurrentContent
    {
        get => _currentContent;
        set => this.RaiseAndSetIfChanged(ref _currentContent, value);
    }
    /// <summary>
    /// 构造函数
    /// </summary>
    public WorkAreaViewModel()
    {
    }
}
