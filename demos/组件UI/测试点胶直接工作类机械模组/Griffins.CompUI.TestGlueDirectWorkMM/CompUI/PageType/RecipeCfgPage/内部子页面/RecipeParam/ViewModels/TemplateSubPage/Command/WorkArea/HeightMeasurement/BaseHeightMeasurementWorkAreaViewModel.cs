using Avalonia.Controls;
using ReactiveUI;
using System.Collections.ObjectModel;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Command;
using Griffins.UI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Griffins.CompUI.TestGlueDirectWorkMM.CompUI.PageType;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Command;
/// <summary>
/// 基准版测高工作区-视图模型
/// </summary>
public class BaseHeightMeasurementWorkAreaViewModel : ReactiveObject
{
    /// <summary>
    /// 页面布局对象
    /// </summary>
    private Control? _viewReference;

    /// <summary>
    /// 值改变事件
    /// </summary>
    public EventHandler? AfterModified;
    /// <summary>
    /// 测高点位置列表-视图模型
    /// </summary>

    public ObservableCollection<BaseHeightMeasurementPositionViewModel> MeasurementHeightPositionViewModels { get; }
    #region 响应式属性
    /// <summary>
    /// 测试值（mm）
    /// </summary>
    public TextBlockViewModel TestValueLabelViewModel { get; }

    /// <summary>
    /// 测高模式-下拉框
    /// </summary>
    public ComboxViewModel MeasurementModeViewModel { get; }

    /// <summary>
    /// 下限（mm）
    /// </summary>
    public NumericViewModel LowerLimitViewModel { get; }

    /// <summary>
    /// 上限（mm）
    /// </summary>
    public NumericViewModel UpperLimitViewModel { get; }

    /// <summary>
    /// 只用于防呆-开关按钮
    /// </summary>
    public ToggleSwitchViewModel IsOnlyForFoolproofViewModel { get; }

    /// <summary>
    /// 三点偏差（mm）
    /// </summary>
    public NumericViewModel ThreePointDeviationViewModel { get; }

    /// <summary>
    /// 测试值标签文本
    /// </summary>
    public string TestValueLabel
    {
        get => TestValueLabelViewModel.Text;
        set => TestValueLabelViewModel.Text = value;
    }

    /// <summary>
    /// 测高模式
    /// </summary>
    public HeightMeasurementMode MeasurementMode
    {
        get
        {
            if (MeasurementModeViewModel.ItemsSource == null)
                return HeightMeasurementMode.SinglePoint;

            var selectedItem = MeasurementModeViewModel.SelectedItem as ComBoxItem;
            return selectedItem != null ? (HeightMeasurementMode)selectedItem.Value : HeightMeasurementMode.SinglePoint;
        }
        set
        {
            if (MeasurementModeViewModel.ItemsSource == null)
                return;

            var targetItem = MeasurementModeViewModel.ItemsSource.Cast<ComBoxItem>()
                .FirstOrDefault(o => (HeightMeasurementMode)o.Value == value);
            if (targetItem != null)
            {
                MeasurementModeViewModel.SelectedItem = targetItem;
                updateMeasurementHeightPositionViewModel((HeightMeasurementMode)targetItem.Value);
                this.RaisePropertyChanged(nameof(MeasurementMode));
            }
               
        }
    }

    /// <summary>
    /// 下限（mm）
    /// </summary>
    public decimal LowerLimit
    {
        get => LowerLimitViewModel.Value;
        set => LowerLimitViewModel.Value = value;
    }

    /// <summary>
    /// 上限（mm）
    /// </summary>
    public decimal UpperLimit
    {
        get => UpperLimitViewModel.Value;
        set => UpperLimitViewModel.Value = value;
    }

    /// <summary>
    /// 只用于防呆
    /// </summary>
    public bool IsOnlyForFoolproof
    {
        get => IsOnlyForFoolproofViewModel.IsChecked;
        set => IsOnlyForFoolproofViewModel.IsChecked = value;
    }

    /// <summary>
    /// 三点偏差（mm）
    /// </summary>
    public decimal ThreePointDeviation
    {
        get => ThreePointDeviationViewModel.Value;
        set => ThreePointDeviationViewModel.Value = value;
    }
    #endregion

    /// <summary>
    /// 构造函数
    /// </summary>
    public BaseHeightMeasurementWorkAreaViewModel()
    {
        MeasurementHeightPositionViewModels = new ObservableCollection<BaseHeightMeasurementPositionViewModel>();

        // 初始化测试值标签
        TestValueLabelViewModel = new TextBlockViewModel
        {
            Text = "0",
        };

        // 初始化测高模式下拉框
        MeasurementModeViewModel = new ComboxViewModel();
        var modeItems = new Dictionary<HeightMeasurementMode, string>
            {
                { HeightMeasurementMode.SinglePoint, "单点测高" },
                { HeightMeasurementMode.ThreePoint, "三点测高" }
            };
        MeasurementModeViewModel.ItemsSource = EnumExtensions.ToEnumItems(modeItems);
        MeasurementModeViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
        // 默认选中第一个选项
        MeasurementModeViewModel.SelectedItem = MeasurementModeViewModel.ItemsSource?.Cast<ComBoxItem>().FirstOrDefault();
        MeasurementModeViewModel.ValueChanged += onMeasurementModeValueChanged;
        // 初始化下限（mm）：默认0.000，精度3位小数，步长0.001
        LowerLimitViewModel = new NumericViewModel
        {
            Increment = 0.001m,
            DecimalPlaces = 3,
            Minimum = 0.000m,
            Maximum = 100.000m,
            Value = 0.000m
        };

        // 初始化上限（mm）：默认50.000，精度3位小数，步长0.001
        UpperLimitViewModel = new NumericViewModel
        {
            Increment = 0.001m,
            DecimalPlaces = 3,
            Minimum = 0.001m,
            Maximum = 100.000m,
            Value = 50.000m
        };

        // 初始化只用于防呆开关（默认关闭）
        IsOnlyForFoolproofViewModel = new ToggleSwitchViewModel { IsChecked = false };

        // 初始化三点偏差（mm）：默认0.000，精度3位小数，步长0.001
        ThreePointDeviationViewModel = new NumericViewModel
        {
            Increment = 0.001m,
            DecimalPlaces = 3,
            Minimum = 0.000m,
            Maximum = 10.000m,
            Value = 0.000m
        };

        // 订阅值变更事件
        subscribeValueChanges();
    }

    private void onMeasurementModeValueChanged(object? sender, ValueChangedEventArgs e)
    {
        if(e.NewValue is ComBoxItem comBoxItem)
        {
            MeasurementMode = (HeightMeasurementMode)comBoxItem.Value;
        }
    }

    /// <summary>
    /// 从数据模型复制数据到ViewModel
    /// </summary>
    /// <param name="command"></param>
    public void CopyFrom(BaseHeightMeasurementCommandSequence command)
    {
        if (command == null) return;
       
        // 加载标签文本
        TestValueLabel = command.TestValue;
        // 加载其他业务数据
        MeasurementMode = command.MeasurementMode;
        LowerLimit = command.LowerLimit;
        UpperLimit = command.UpperLimit;
        IsOnlyForFoolproof = command.IsOnlyForFoolproof;
        ThreePointDeviation = command.ThreePointDeviation;
        //有保存数据则从配置中加载，否则不加载，防止默认的数据项MeasurementHeightPositionViewModels为清空
        if (command.MeasurementHeightPositionInfoes.Count!=0)
        {
            MeasurementHeightPositionViewModels.Clear();
            int index = 1;
            foreach (var item in command.MeasurementHeightPositionInfoes)
            {
                var measurementHeightPositionViewModel = new BaseHeightMeasurementPositionViewModel();
                measurementHeightPositionViewModel.SetViewReference(_viewReference!);
                measurementHeightPositionViewModel.HightShowName = $"Point {index}";
                measurementHeightPositionViewModel.CopyFrom(item);
                MeasurementHeightPositionViewModels.Add(measurementHeightPositionViewModel);
            }
        }
       

    }

    /// <summary>
    ///从ViewModel复制数据到数据模型
    /// </summary>
    /// <param name="command">基准版测高指令</param>
    public void CopyTo(BaseHeightMeasurementCommandSequence command)
    {
        if (command == null) return;

        // 保存标签文本（若允许动态修改）
        command.TestValue = TestValueLabel;
        // 保存其他业务数据
        command.MeasurementMode = MeasurementMode;
        command.LowerLimit = LowerLimit;
        command.UpperLimit = UpperLimit;
        command.IsOnlyForFoolproof = IsOnlyForFoolproof;
        command.ThreePointDeviation = ThreePointDeviation;

        command.MeasurementHeightPositionInfoes.Clear();
        foreach (var item in MeasurementHeightPositionViewModels)
        {
            var basePositionInfo = new BasePositionInfo();
            item.CopyTo(basePositionInfo);
            command.MeasurementHeightPositionInfoes.Add(basePositionInfo);
        }
    }
    /// <summary>
    /// 设置视图引用
    /// </summary>
    public  void SetViewReference(Control view)
    {
        _viewReference = view;
    }
    private void updateMeasurementHeightPositionViewModel(HeightMeasurementMode measurementMode)
    {
        MeasurementHeightPositionViewModels.Clear();
        switch (measurementMode)
        {
            case HeightMeasurementMode.SinglePoint:
                var data = new BaseHeightMeasurementPositionViewModel();
                data.HightShowName = "Point 1";
                data.SetViewReference(_viewReference!);
                MeasurementHeightPositionViewModels.Add(data);
                break;
            case HeightMeasurementMode.ThreePoint:
                for (int i = 0; i < 3; i++)
                {
                    var threePoint = new BaseHeightMeasurementPositionViewModel();
                    threePoint.HightShowName = $"Point {i + 1}";
                    threePoint.SetViewReference(_viewReference!);
                    MeasurementHeightPositionViewModels.Add(threePoint);
                }
                break;
            default:
                break;
        }
    }
    #region 值改变事件
    /// <summary>
    /// 订阅值改变事件
    /// </summary>
    private void subscribeValueChanges()
    {
        TestValueLabelViewModel.ValueChanged += onValueChanged;
        MeasurementModeViewModel.ValueChanged += onValueChanged;
        LowerLimitViewModel.ValueChanged += onValueChanged;
        UpperLimitViewModel.ValueChanged += onValueChanged;
        IsOnlyForFoolproofViewModel.ValueChanged += onValueChanged;
        ThreePointDeviationViewModel.ValueChanged += onValueChanged;
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