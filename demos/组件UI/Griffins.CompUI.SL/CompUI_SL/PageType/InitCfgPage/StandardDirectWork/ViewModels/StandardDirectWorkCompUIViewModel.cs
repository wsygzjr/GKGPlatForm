using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using GKG.UI;
using Griffins.CompUI.SL.InitCfgPage.Models;
using Griffins.Map;
using Griffins.Map.UI;
using ReactiveUI;
using BarcodeType = Griffins.CompUI.SL.InitCfgPage.Models.BarcodeType;

namespace Griffins.CompUI.SL.InitCfgPage.ViewModels;

/// <summary>
/// 
/// </summary>
public class StandardDirectWorkCompUIViewModel : ReactiveObject, IDisposable
{
    #region 私有字段

    private bool isDesign;

    private ICompUIRunTimeCallBack callBack;

    private bool _suppressIsSelectAll;

    #endregion

    #region 响应式属性

    private object _viewTag;
    /// <summary>
    /// 对应View的Tag属性（支持双向绑定）
    /// </summary>
    public object ViewTag
    {
        get => _viewTag;
        set => this.RaiseAndSetIfChanged(ref _viewTag, value);
    }

    private bool _readOnly;
    /// <summary>
    /// 只读
    /// </summary>
    public bool ReadOnly
    {
        get => _readOnly;
        set => this.RaiseAndSetIfChanged(ref _readOnly, value);
    }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool ScannerEnabled
    {
        get => ScannerEnabledViewModel.IsChecked;
        set
        {
            ScannerEnabledViewModel.IsChecked = value;
            this.RaisePropertyChanged(nameof(ScannerEnabled));
        }
    }

    /// <summary>
    /// 条码检验字符
    /// </summary>
    public string BarcodeCheckChar
    {
        get => BarcodeCheckCharViewModel.Text ?? string.Empty;
        set
        {
            BarcodeCheckCharViewModel.Text = value;
            this.RaisePropertyChanged(nameof(BarcodeCheckChar));
        }
    }

    /// <summary>
    /// 条码类型
    /// </summary>
    public BarcodeType BarcodeType
    {
        get => (BarcodeType)((BarcodeTypeViewModel.SelectedItem as ComBoxItem)?.Value ?? BarcodeType.Code39);
        set
        {
            if (BarcodeTypeViewModel.ItemsSource != null)
            {
                var targetItem = BarcodeTypeViewModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (BarcodeType)o.Value == value);
                if (targetItem != null)
                {
                    BarcodeTypeViewModel.SelectedItem = targetItem;
                }
            }
            this.RaisePropertyChanged(nameof(BarcodeType));
        }
    }

    /// <summary>
    /// 扫码器类型
    /// </summary>
    public string ScannerType
    {
        get => ScannerTypeViewModel.Text ?? string.Empty;
        set
        {
            ScannerTypeViewModel.Text = value;
            this.RaisePropertyChanged(nameof(ScannerType));
        }
    }

    /// <summary>
    /// 测高延时(ms)
    /// </summary>
    public int HeightDelayMs
    {
        get => (int)HeightDelayMsViewModel.Value;
        set
        {
            HeightDelayMsViewModel.Value = value;
            this.RaisePropertyChanged(nameof(HeightDelayMs));
        }
    }

    /// <summary>
    /// 固定时间(ms)
    /// </summary>
    public int LiftFixedTimeMs
    {
        get => (int)LiftFixedTimeMsViewModel.Value;
        set
        {
            LiftFixedTimeMsViewModel.Value = value;
            this.RaisePropertyChanged(nameof(LiftFixedTimeMs));
        }
    }

    /// <summary>
    /// 解除固定时间(ms)
    /// </summary>
    public int LiftReleaseFixedTimeMs
    {
        get => (int)LiftReleaseFixedTimeMsViewModel.Value;
        set
        {
            LiftReleaseFixedTimeMsViewModel.Value = value;
            this.RaisePropertyChanged(nameof(LiftReleaseFixedTimeMs));
        }
    }

    /// <summary>
    /// 是否检测开启
    /// </summary>
    public bool TempDetectEnabled
    {
        get => TempDetectEnabledViewModel.IsChecked;
        set
        {
            TempDetectEnabledViewModel.IsChecked = value;
            this.RaisePropertyChanged(nameof(TempDetectEnabled));
        }
    }

    /// <summary>
    /// 是否工作开启
    /// </summary>
    public bool TempWorkEnabled
    {
        get => TempWorkEnabledViewModel.IsChecked;
        set
        {
            TempWorkEnabledViewModel.IsChecked = value;
            this.RaisePropertyChanged(nameof(TempWorkEnabled));
        }
    }

    /// <summary>
    /// 是否预加热
    /// </summary>
    public bool TempPreheatEnabled
    {
        get => TempPreheatEnabledViewModel.IsChecked;
        set
        {
            TempPreheatEnabledViewModel.IsChecked = value;
            this.RaisePropertyChanged(nameof(TempPreheatEnabled));
        }
    }

    private ObservableCollection<CustomStationSensor> _customStationSensors;
    /// <summary>
    /// 工位自定义感应器列表
    /// </summary>
    public ObservableCollection<CustomStationSensor> CustomStationSensors
    {
        get => _customStationSensors;
        set
        {
            if (_customStationSensors == value) return;
            UnsubscribeFromList(_customStationSensors);
            this.RaiseAndSetIfChanged(ref _customStationSensors, value);
            SubscribeToList(_customStationSensors);
            UpdateIsSelectAll();
        }
    }

    private CustomStationSensor? _selectedItem;
    /// <summary>
    /// 当前选中的数据项
    /// </summary>
    public CustomStationSensor? SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }

    private bool _isSelectAll;
    /// <summary>
    /// 是否全选（头部复选框）
    /// </summary>
    public bool IsSelectAll
    {
        get => _isSelectAll;
        set
        {
            if (_isSelectAll == value) return;

            _isSelectAll = value;
            this.RaisePropertyChanged(nameof(IsSelectAll));

            if (CustomStationSensors == null || CustomStationSensors.Count == 0)
            {
                return;
            }

            try
            {
                _suppressIsSelectAll = true;
                foreach (var item in CustomStationSensors)
                {
                    SetIsSelected(item, value);
                }
            }
            finally
            {
                _suppressIsSelectAll = false;
            }
        }
    }

    #endregion

    #region UI组件模型

    /// <summary>
    /// 扫码器启用-开关按钮视图模型
    /// </summary>
    public ToggleSwitchViewModel ScannerEnabledViewModel { get; }

    /// <summary>
    /// 条码检验字符-文本块视图模型
    /// </summary>
    public TextInputViewModel BarcodeCheckCharViewModel { get; }

    /// <summary>
    /// 条码类型-下拉框视图模型
    /// </summary>
    public ComboxViewModel BarcodeTypeViewModel { get; }

    /// <summary>
    /// 扫码器类型-文本块视图模型
    /// </summary>
    public TextInputViewModel ScannerTypeViewModel { get; }

    /// <summary>
    /// 测高延时-数字输入框视图模型
    /// </summary>
    public NumericWithLableViewModel HeightDelayMsViewModel { get; }

    /// <summary>
    /// 固定时间-数字输入框视图模型
    /// </summary>
    public NumericWithLableViewModel LiftFixedTimeMsViewModel { get; }

    /// <summary>
    /// 解除固定时间-数字输入框视图模型
    /// </summary>
    public NumericWithLableViewModel LiftReleaseFixedTimeMsViewModel { get; }

    /// <summary>
    /// 是否检测开启-开关按钮视图模型
    /// </summary>
    public ToggleSwitchViewModel TempDetectEnabledViewModel { get; }

    /// <summary>
    /// 是否工作开启-开关按钮视图模型
    /// </summary>
    public ToggleSwitchViewModel TempWorkEnabledViewModel { get; }

    /// <summary>
    /// 是否预加热-开关按钮视图模型
    /// </summary>
    public ToggleSwitchViewModel TempPreheatEnabledViewModel { get; }

    #endregion

    #region 命令

    public ReactiveCommand<Unit, Unit> ButtonClickCommand { get; }

    /// <summary>
    /// 添加命令
    /// </summary>
    public ReactiveCommand<Unit, Unit> AddCommand { get; }

    /// <summary>
    /// 删除命令
    /// </summary>
    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

    #endregion

    #region 构造函数
    /// <summary>
    /// 构造函数（初始化）
    /// </summary>
    public StandardDirectWorkCompUIViewModel(bool isDesign, ICompUIRunTimeCallBack callBack)
    {
        this.isDesign = isDesign;
        this.callBack = callBack;

        #region 初始化UI组件模型

        ScannerEnabledViewModel = new() { IsChecked = false };
        TempDetectEnabledViewModel = new() { IsChecked = false };
        TempWorkEnabledViewModel = new() { IsChecked = false };
        TempPreheatEnabledViewModel = new() { IsChecked = false };
        BarcodeCheckCharViewModel = new() { Text = string.Empty };
        ScannerTypeViewModel = new() { Text = string.Empty };

        // 初始化条码类型下拉框
        BarcodeTypeViewModel = new();
        var barcodeTypeDisplayNames = new Dictionary<BarcodeType, string>
        {
            { BarcodeType.Code39, "Code39" },
            { BarcodeType.Code128, "Code128" }
        };
        BarcodeTypeViewModel.ItemsSource = EnumExtensions.ToEnumItems(barcodeTypeDisplayNames);
        BarcodeTypeViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);

        HeightDelayMsViewModel = new()// 测高延时
        {
            LableText = " ms",
            Minimum = 0,
            Maximum = 1000,
            DecimalPlaces = 0,
            Value = 100,
            Increment = 1
        };

        LiftFixedTimeMsViewModel = new()// 固定时间
        {
            LableText = " ms",
            Minimum = 0,
            Maximum = 1000,
            DecimalPlaces = 0,
            Value = 100,
            Increment = 1
        };

        LiftReleaseFixedTimeMsViewModel = new() // 解除固定时间
        {
            LableText = " ms",
            Minimum = 0,
            Maximum = 1000,
            DecimalPlaces = 0,
            Value = 100,
            Increment = 1
        };


        #endregion

        ButtonClickCommand = ReactiveCommand.Create(OnButtonClicked);

        // 列表初始化
        CustomStationSensors = new ObservableCollection<CustomStationSensor>();

        // 列表命令
        AddCommand = ReactiveCommand.Create(OnAddItems);
        DeleteCommand = ReactiveCommand.Create(OnDeleteItems);
    }
    #endregion

    #region 公共方法



    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="model">数据源</param>
    public void SetData(StandardDirectWorkCompUIModel model)
    {
        if (model?.BarcodeScanner == null)
        {
            return;
        }

        // 扫码器
        ScannerEnabled = model.BarcodeScanner.Enabled;
        BarcodeCheckChar = model.BarcodeScanner.BarcodeCheckChar ?? string.Empty;
        BarcodeType = model.BarcodeScanner.BarcodeType;
        ScannerType = model.BarcodeScanner.ScannerType;

        // 测高
        HeightDelayMs = model.HeightMeasurement?.DelayMs ?? 0;

        // 顶升固定时间参数
        LiftFixedTimeMs = model.LiftFixedTime?.FixedTimeMs ?? 0;
        LiftReleaseFixedTimeMs = model.LiftFixedTime?.ReleaseFixedTimeMs ?? 0;

        // 温度控制
        TempDetectEnabled = model.TemperatureControl?.DetectEnabled ?? false;
        TempWorkEnabled = model.TemperatureControl?.WorkEnabled ?? false;
        TempPreheatEnabled = model.TemperatureControl?.PreheatEnabled ?? false;

        // 工位自定义感应器列表
        CustomStationSensors = model.CustomStationSensors ?? new ObservableCollection<CustomStationSensor>();
    }

    /// <summary>
    /// 获取数据
    /// </summary>
    /// <returns>数据模型</returns>
    public StandardDirectWorkCompUIModel GetData()
    {
        var model = new StandardDirectWorkCompUIModel();

        // 扫码器
        model.BarcodeScanner.Enabled = ScannerEnabled;
        model.BarcodeScanner.BarcodeCheckChar = BarcodeCheckChar ?? string.Empty;
        model.BarcodeScanner.BarcodeType = BarcodeType;
        model.BarcodeScanner.ScannerType = ScannerType;

        // 测高
        model.HeightMeasurement.DelayMs = HeightDelayMs;

        // 顶升固定时间参数
        model.LiftFixedTime.FixedTimeMs = LiftFixedTimeMs;
        model.LiftFixedTime.ReleaseFixedTimeMs = LiftReleaseFixedTimeMs;

        // 温度控制
        model.TemperatureControl.DetectEnabled = TempDetectEnabled;
        model.TemperatureControl.WorkEnabled = TempWorkEnabled;
        model.TemperatureControl.PreheatEnabled = TempPreheatEnabled;

        // 工位自定义感应器列表
        model.CustomStationSensors = CustomStationSensors ?? new ObservableCollection<CustomStationSensor>();

        return model;
    }

    /// <summary>
    /// 释放非托管资源
    /// </summary>
    public void Dispose()
    {
        UnsubscribeFromList(CustomStationSensors);
    }

    #endregion

    {
        LableText = " ms",
        Minimum = 0,
        Maximum = 1000,
        DecimalPlaces = 0,
        Value = 100,
        Increment = 1
    };


    #endregion

    ButtonClickCommand = ReactiveCommand.Create(OnButtonClicked);
        var nextIndex = CustomStationSensors.Count > 0 ? CustomStationSensors.Max(x => x.Index) + 1 : 1;
        var item = new CustomStationSensor
        {
            Index = nextIndex,
            SensorId = string.Empty,
            MotionCardId = string.Empty,
            IoChannel = string.Empty,
        };

        CustomStationSensors.Add(item);
        SelectedItem = item;
        UpdateIsSelectAll();
    }

    private void OnDeleteItems()
    {
        if (CustomStationSensors == null || CustomStationSensors.Count == 0)
        {
            return;
        }

        // 优先删除勾选项（如果存在 IsSelected 字段/属性）
        var toDelete = CustomStationSensors.Where(GetIsSelected).ToList();
        if (toDelete.Count == 0 && SelectedItem != null)
        {
            toDelete.Add(SelectedItem);
        }

        foreach (var item in toDelete)
        {
            CustomStationSensors.Remove(item);
        }

        SelectedItem = null;
        UpdateIndexes();
        UpdateIsSelectAll();
    }

    private void UpdateIndexes()
    {
        if (CustomStationSensors == null) return;
        for (int i = 0; i < CustomStationSensors.Count; i++)
        {
            CustomStationSensors[i].Index = i + 1;
        }
    }

    private void SubscribeToList(ObservableCollection<CustomStationSensor>? list)
    {
        if (list == null)
        {
            return;
        }

        list.CollectionChanged += CustomStationSensors_CollectionChanged;
        foreach (var item in list)
        {
            if (item is INotifyPropertyChanged inpc)
            {
                inpc.PropertyChanged += CustomStationSensor_PropertyChanged;
            }
        }
    }

    private void UnsubscribeFromList(ObservableCollection<CustomStationSensor>? list)
    {
        if (list == null)
        {
            return;
        }

        list.CollectionChanged -= CustomStationSensors_CollectionChanged;
        foreach (var item in list)
        {
            if (item is INotifyPropertyChanged inpc)
            {
                inpc.PropertyChanged -= CustomStationSensor_PropertyChanged;
            }
        }
    }

    private void CustomStationSensors_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (var item in e.NewItems)
            {
                if (item is INotifyPropertyChanged inpc)
                {
                    inpc.PropertyChanged += CustomStationSensor_PropertyChanged;
                }
            }
        }

        if (e.OldItems != null)
        {
            foreach (var item in e.OldItems)
            {
                if (item is INotifyPropertyChanged inpc)
                {
                    inpc.PropertyChanged -= CustomStationSensor_PropertyChanged;
                }
            }
        }

        UpdateIsSelectAll();
    }

    private void CustomStationSensor_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_suppressIsSelectAll)
        {
            return;
        }

        if (e.PropertyName == "IsSelected")
        {
            UpdateIsSelectAll();
        }
    }

    private void UpdateIsSelectAll()
    {
        if (_suppressIsSelectAll)
        {
            return;
        }

        if (CustomStationSensors == null || CustomStationSensors.Count == 0)
        {
            if (_isSelectAll)
            {
                _isSelectAll = false;
                this.RaisePropertyChanged(nameof(IsSelectAll));
            }
            return;
        }

        var newValue = CustomStationSensors.All(GetIsSelected);
        if (_isSelectAll != newValue)
        {
            _isSelectAll = newValue;
            this.RaisePropertyChanged(nameof(IsSelectAll));
        }
    }

    private static bool GetIsSelected(CustomStationSensor sensor)
    {
        var prop = sensor.GetType().GetProperty("IsSelected");
        if (prop?.PropertyType == typeof(bool) && prop.GetGetMethod() != null)
        {
            return (bool)(prop.GetValue(sensor) ?? false);
        }
        return false;
    }

    private static void SetIsSelected(CustomStationSensor sensor, bool value)
    {
        var prop = sensor.GetType().GetProperty("IsSelected");
        if (prop?.PropertyType == typeof(bool) && prop.GetSetMethod() != null)
        {
            prop.SetValue(sensor, value);
        }
    }

    #endregion
}