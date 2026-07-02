using Avalonia.Controls;
using Griffins.UI;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Command;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Griffins.CompUI.TestGlueDirectWorkMM.CompUI.PageType;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Command;
/// <summary>
/// 测厚度工作区-视图模型
/// </summary>
public class ThicknessMeasurementWorkAreaViewModel : ReactiveObject
{
    private Control? _viewReference;
    /// <summary>
    /// 值改变事件
    /// </summary>
    public EventHandler? AfterModified;
    /// <summary>
    /// 测试值（mm）
    /// </summary>
    public TextBlockViewModel TestValueLabelViewModel { get; }
    /// <summary>
    /// 厚度值（mm）
    /// </summary>
    public TextBlockViewModel ThicknessLabelViewModel { get; }
    /// <summary>
    /// 基准点-视图模型
    /// </summary>

    public MeasurementPositionViewModel BasePositionViewModel { get; }
    /// <summary>
    /// 厚度点位置列表-视图模型
    /// </summary>

    public ObservableCollection<ThicknessMeasurementPositionViewModel> ThicknessMeasurementPositionViewModels { get; }
    #region 响应式属性

    /// <summary>
    /// 测试值标签文本
    /// </summary>
    public string TestValueLabel
    {
        get => TestValueLabelViewModel.Text;
        set => TestValueLabelViewModel.Text = value;
    }

    /// <summary>
    ///厚度值标签文本
    /// </summary>
    public string Thickness
    {
        get => ThicknessLabelViewModel.Text;
        set => ThicknessLabelViewModel.Text = value;
    }
    #endregion

    /// <summary>
    /// 添加厚度点位置
    /// </summary>
    public ReactiveCommand<Unit, Unit> AddThicknessPositionCommand { get; }
    /// <summary>
    /// 删除厚度点位置
    /// </summary>
    public ReactiveCommand<ThicknessMeasurementPositionViewModel, Unit> DeleteThicknessPositionCommand { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    public ThicknessMeasurementWorkAreaViewModel()
    {
        ThicknessMeasurementPositionViewModels = new ObservableCollection<ThicknessMeasurementPositionViewModel>();
        BasePositionViewModel=new MeasurementPositionViewModel();
        // 初始化测试值标签
        TestValueLabelViewModel = new TextBlockViewModel
        {
            Text = "0",
        };
        ThicknessLabelViewModel = new TextBlockViewModel
        {
            Text = "0",
        };
        AddThicknessPositionCommand = ReactiveCommand.Create(addThicknessPositionCommand, Observable.Return(true));
        DeleteThicknessPositionCommand = ReactiveCommand.Create<ThicknessMeasurementPositionViewModel>(deleteThicknessPositionCommand, Observable.Return(true));

        // 订阅值变更事件
        subscribeValueChanges();
    }

    /// <summary>
    /// 从数据模型复制数据到ViewModel
    /// </summary>
    /// <param name="command">基准版测高指令</param>
    public void CopyFrom(ThicknessMeasurementCommandSequence command)
    {
        if (command == null) return;
       
        // 加载标签文本
        TestValueLabel = command.TestValue;
        Thickness = command.Thickness.ToString();

        BasePositionViewModel.CopyFrom(command.BasePositionInfo);
        BasePositionViewModel.HightShowName = "基准点";
        ThicknessMeasurementPositionViewModels.Clear();
        int index = 1;
        foreach (var item in command.ThicknessPositionInfoes)
        {
            var measurementHeightPositionViewModel = new ThicknessMeasurementPositionViewModel();
            measurementHeightPositionViewModel.HightShowName = $"测厚点 {index}";
            measurementHeightPositionViewModel.SetViewReference(_viewReference!);
            measurementHeightPositionViewModel.CopyFrom(item);
            ThicknessMeasurementPositionViewModels.Add(measurementHeightPositionViewModel);
        }

    }

    /// <summary>
    /// 从ViewModel复制数据到数据模型
    /// </summary>
    /// <param name="command">基准版测高指令</param>
    public void CopyTo(ThicknessMeasurementCommandSequence command)
    {
        if (command == null) return;

        // 保存标签文本（若允许动态修改）
        command.TestValue = TestValueLabel;
        command.Thickness = decimal.Parse(Thickness);

        BasePositionViewModel.CopyTo(command.BasePositionInfo);

        command.ThicknessPositionInfoes.Clear();
        foreach (var item in ThicknessMeasurementPositionViewModels)
        {
            var thicknessPositionInfo = new ThicknessPositionInfo();
            item.CopyTo(thicknessPositionInfo);
            command.ThicknessPositionInfoes.Add(thicknessPositionInfo);
        }
    }
    /// <summary>
    /// 设置视图引用（用于弹窗、对话框等UI操作）
    /// </summary>
    public void SetViewReference(Control view)
    {
        BasePositionViewModel.SetViewReference(view);
        _viewReference = view;
    }
    /// <summary>
    /// 添加厚度点位置
    /// </summary>
    private void addThicknessPositionCommand()
    {
        var prefix = "测厚点";

        var existingNames = ThicknessMeasurementPositionViewModels.Select(item => item.HightShowName).ToList();
        int newSerialNumber = SerialNumberGenerator.GetMinUnusedSerialNumber(prefix, existingNames);

        var newThicknessMeasurementPositionViewModel = new ThicknessMeasurementPositionViewModel();
        newThicknessMeasurementPositionViewModel.HightShowName = $"{prefix}{newSerialNumber}";
        newThicknessMeasurementPositionViewModel.SetViewReference(_viewReference!);
        newThicknessMeasurementPositionViewModel.CopyFrom(new ThicknessPositionInfo());
        newThicknessMeasurementPositionViewModel.AfterModified += onAfterModified;
        ThicknessMeasurementPositionViewModels.Add(newThicknessMeasurementPositionViewModel);
    }
    /// <summary>
    /// 删除厚度点位置
    /// </summary>
    private void deleteThicknessPositionCommand(ThicknessMeasurementPositionViewModel item)
    {
        item.AfterModified -= onAfterModified;
        ThicknessMeasurementPositionViewModels.Remove(item);
    }
    #region 值改变事件
    /// <summary>
    /// 订阅值改变事件
    /// </summary>
    private void subscribeValueChanges()
    {
        //BasePositionViewModel.AfterModified += onAfterModified;
        

        TestValueLabelViewModel.ValueChanged += onValueChanged;
        ThicknessLabelViewModel.ValueChanged += onValueChanged;
    }

    /// <summary>
    /// 值改变事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void onValueChanged(object? sender, ValueChangedEventArgs e)
    {
        AfterModified?.Invoke(sender, e);
    }
    /// <summary>
    /// 值改变事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void onAfterModified(object? sender, EventArgs e)
    {
        AfterModified?.Invoke(sender, e);
    }
    #endregion
}