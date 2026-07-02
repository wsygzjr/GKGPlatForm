using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Newtonsoft.JsonG.Linq;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;

namespace GKG.UI.General
{
    /// <summary>
    /// 预点胶功能参数视图
    /// </summary>
    public partial class PreDispensingCfgView : UserControl
    {
        /// <summary>
        /// 预点胶功能参数视图
        /// </summary>
        public PreDispensingCfgView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    #region 视图模型

    /// <summary>
    /// 预点胶功能参数配置-视图模型
    /// </summary>
    public class PreDispensingViewModel : ReactiveObject
    {
        #region 子ViewModel（对应UI控件）
        /// <summary>
        /// 功能头编号ViewModel
        /// </summary>
        public NumericViewModel FunctionHeadNumberViewModel { get; }

        /// <summary>
        /// 开启预点胶ViewModel
        /// </summary>
        public ToggleSwitchViewModel EnablePreDispensingViewModel { get; }

        /// <summary>
        /// 开启实时测高ViewModel
        /// </summary>
        public ToggleSwitchViewModel EnableRealTimeHeightMeasurementViewModel { get; }

        /// <summary>
        /// 开启2DViewModel
        /// </summary>
        public ToggleSwitchViewModel Enable2DViewModel { get; }

        /// <summary>
        /// 开启自动重置ViewModel
        /// </summary>
        public ToggleSwitchViewModel EnableAutoResetViewModel { get; }

        /// <summary>
        /// 单次预点胶个数ViewModel
        /// </summary>
        public NumericViewModel SinglePreDispensingCountViewModel { get; }

        /// <summary>
        /// 预点胶类型ViewModel
        /// </summary>
        public ComboxViewModel PreDispensingTypeViewModel { get; }

        /// <summary>
        /// 预点胶点数X个数ViewModel
        /// </summary>
        public NumericViewModel PreDispensingPointCountXViewModel { get; }

        /// <summary>
        /// 预点胶点数Y个数ViewModel
        /// </summary>
        public NumericViewModel PreDispensingPointCountYViewModel { get; }

        /// <summary>
        /// 点胶样式ViewModel
        /// </summary>
        public ComboxViewModel DispensingStyleViewModel { get; }

        /// <summary>
        /// 点胶重量ViewModel
        /// </summary>
        public NumericViewModel DispensingWeightViewModel { get; }

        /// <summary>
        /// 缓存测高值ViewModel
        /// </summary>
        public NumericViewModel CachedHeightValueViewModel { get; }

        /// <summary>
        /// 点阵左上位置-视图模型
        /// </summary>

        public BasePositionViewModel DotMatrix_LeftUpperPositionViewModel { get; }
        /// <summary>
        /// 点阵右上位置-视图模型
        /// </summary>

        public BasePositionViewModel DotMatrix_RightUpperPositionViewModel { get; }
        /// <summary>
        /// 点阵右下位置-视图模型
        /// </summary>

        public BasePositionViewModel DotMatrix_RightLowerPositionViewModel { get; }
        #endregion

        #region 绑定属性
        /// <summary>
        /// 功能头编号
        /// </summary>
        public int FunctionHeadNumber
        {
            get => (int)FunctionHeadNumberViewModel.Value;
            set => FunctionHeadNumberViewModel.Value = value;
        }

        /// <summary>
        /// 开启预点胶
        /// </summary>
        public bool EnablePreDispensing
        {
            get => EnablePreDispensingViewModel.IsChecked;
            set => EnablePreDispensingViewModel.IsChecked = value;
        }

        /// <summary>
        /// 开启实时测高
        /// </summary>
        public bool EnableRealTimeHeightMeasurement
        {
            get => EnableRealTimeHeightMeasurementViewModel.IsChecked;
            set => EnableRealTimeHeightMeasurementViewModel.IsChecked = value;
        }

        /// <summary>
        /// 开启2D
        /// </summary>
        public bool Enable2D
        {
            get => Enable2DViewModel.IsChecked;
            set => Enable2DViewModel.IsChecked = value;
        }

        /// <summary>
        /// 开启自动重置
        /// </summary>
        public bool EnableAutoReset
        {
            get => EnableAutoResetViewModel.IsChecked;
            set => EnableAutoResetViewModel.IsChecked = value;
        }

        /// <summary>
        /// 单次预点胶个数
        /// </summary>
        public int SinglePreDispensingCount
        {
            get => (int)SinglePreDispensingCountViewModel.Value;
            set => SinglePreDispensingCountViewModel.Value = value;
        }

        /// <summary>
        /// 预点胶类型
        /// </summary>
        public DispensingType PreDispensingType
        {
            get
            {
                if (PreDispensingTypeViewModel.SelectedItem is ComBoxItem item)
                    return (DispensingType)item.Value;
                return DispensingType.Point; // 默认"点"类型
            }
            set
            {
                PreDispensingTypeViewModel.SelectedItem = PreDispensingTypeViewModel.ItemsSource!
                    .Cast<ComBoxItem>()
                    .FirstOrDefault(item => (DispensingType)item.Value == value);
            }
        }

        /// <summary>
        /// 预点胶点数X个数
        /// </summary>
        public int PreDispensingPointCountX
        {
            get => (int)PreDispensingPointCountXViewModel.Value;
            set => PreDispensingPointCountXViewModel.Value = value;
        }

        /// <summary>
        /// 预点胶点数Y个数
        /// </summary>
        public int PreDispensingPointCountY
        {
            get => (int)PreDispensingPointCountYViewModel.Value;
            set => PreDispensingPointCountYViewModel.Value = value;
        }

        /// <summary>
        /// 点胶样式
        /// </summary>
        public string DispensingStyle
        {
            get
            {
                if (DispensingStyleViewModel.SelectedItem is ComBoxItem item)
                    return item.DisplayName;
                return string.Empty;
            }
            set
            {
                DispensingStyleViewModel.SelectedItem = DispensingStyleViewModel.ItemsSource!
                    .Cast<ComBoxItem>()
                    .FirstOrDefault(item => item.DisplayName == value);
            }
        }

        /// <summary>
        /// 点胶重量（单位：g）
        /// </summary>
        public decimal DispensingWeight
        {
            get => DispensingWeightViewModel.Value;
            set => DispensingWeightViewModel.Value = value;
        }

        /// <summary>
        /// 缓存测高值（单位：mm）
        /// </summary>
        public decimal CachedHeightValue
        {
            get => CachedHeightValueViewModel.Value;
            set => CachedHeightValueViewModel.Value = value;
        }
        #endregion

        /// <summary>
        /// 数据变更事件（通知外部）
        /// </summary>
        public event EventHandler? AfterModified;

        /// <summary>
        /// 重置参数命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> ResetCommand { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public PreDispensingViewModel()
        {
            // 初始化数字输入框ViewModel（设置默认范围和初始值）
            FunctionHeadNumberViewModel = new NumericViewModel
            {
                Minimum = 1,
                Maximum = 10,
                Increment = 1,
                DecimalPlaces = 0,
                Value = 1
            };

            SinglePreDispensingCountViewModel = new NumericViewModel
            {
                Minimum = 1,
                Maximum = 100,
                Increment = 1,
                DecimalPlaces = 0,
                Value = 1
            };

            PreDispensingPointCountXViewModel = new NumericViewModel
            {
                Minimum = 1,
                Maximum = 50,
                Increment = 1,
                DecimalPlaces = 0,
                Value = 1
            };

            PreDispensingPointCountYViewModel = new NumericViewModel
            {
                Minimum = 1,
                Maximum = 50,
                Increment = 1,
                DecimalPlaces = 0,
                Value = 1
            };

            DispensingWeightViewModel = new NumericViewModel
            {
                Minimum = 0.1m,
                Maximum = 10.0m,
                Increment = 0.1m, 
                DecimalPlaces = 1,
                Value = 1.0m
            };

            CachedHeightValueViewModel = new NumericViewModel
            {
                Minimum = 0.0m,
                Maximum = 50.0m,
                Increment = 0.1m,
                DecimalPlaces = 1,
                Value = 0.0m
            };

            // 初始化开关按钮ViewModel（默认关闭）
            EnablePreDispensingViewModel = new ToggleSwitchViewModel { IsChecked = false };
            EnableRealTimeHeightMeasurementViewModel = new ToggleSwitchViewModel { IsChecked = false };
            Enable2DViewModel = new ToggleSwitchViewModel { IsChecked = false };
            EnableAutoResetViewModel = new ToggleSwitchViewModel { IsChecked = false };

            // 初始化预点胶类型下拉框（绑定DispensingType枚举）
            PreDispensingTypeViewModel = new ComboxViewModel
            {
                ItemsSource = new ObservableCollection<ComBoxItem>
                {
                    new ComBoxItem { DisplayName = "点", Value = DispensingType.Point },
                    new ComBoxItem { DisplayName = "直线", Value = DispensingType.Line }
                },
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
            };

            // 初始化点胶样式下拉框（示例选项，可根据实际需求扩展）
            DispensingStyleViewModel = new ComboxViewModel
            {
                ItemsSource = new ObservableCollection<ComBoxItem>
                {
                    new ComBoxItem { DisplayName = "样式一" },
                    new ComBoxItem { DisplayName = "样式二" },
                    new ComBoxItem { DisplayName = "样式三" }
                },
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
            };

            DotMatrix_LeftUpperPositionViewModel = new BasePositionViewModel();
            DotMatrix_RightUpperPositionViewModel = new BasePositionViewModel();
            DotMatrix_RightLowerPositionViewModel = new BasePositionViewModel();

            // 初始化命令
            ResetCommand = ReactiveCommand.Create(resetParameters);

            // 订阅子ViewModel变更事件
            subscribeChildViewModelEvents();
        }

        /// <summary>
        /// 从数据模型加载数据
        /// </summary>
        /// <param name="cfgInfo">预点胶参数配置模型</param>
        public void CopyFrom(PreDispensingCfgInfo cfgInfo)
        {
            if (cfgInfo == null) return;

            FunctionHeadNumber = cfgInfo.FunctionHeadNumber;
            EnablePreDispensing = cfgInfo.EnablePreDispensing;
            EnableRealTimeHeightMeasurement = cfgInfo.EnableRealTimeHeightMeasurement;
            Enable2D = cfgInfo.Enable2D;
            EnableAutoReset = cfgInfo.EnableAutoReset;
            SinglePreDispensingCount = cfgInfo.SinglePreDispensingCount;
            PreDispensingType = cfgInfo.PreDispensingType;
            PreDispensingPointCountX = cfgInfo.PreDispensingPointCountX;
            PreDispensingPointCountY = cfgInfo.PreDispensingPointCountY;
            DispensingStyle = cfgInfo.DispensingStyle;
            DispensingWeight = cfgInfo.DispensingWeight;
            CachedHeightValue = cfgInfo.CachedHeightValue;
            DotMatrix_LeftUpperPositionViewModel.CopyFrom(cfgInfo.DotMatrix_LeftUpperPositionInfo);
            DotMatrix_RightUpperPositionViewModel.CopyFrom(cfgInfo.DotMatrix_RightUpperPositionInfo);
            DotMatrix_RightLowerPositionViewModel.CopyFrom(cfgInfo.DotMatrix_RightLowerPositionInfo);
        }

        /// <summary>
        /// 将数据回写到数据模型
        /// </summary>
        /// <param name="cfgInfo">待填充的预点胶参数配置模型</param>
        public void CopyTo(PreDispensingCfgInfo cfgInfo)
        {
            if (cfgInfo == null) return;

            cfgInfo.FunctionHeadNumber = FunctionHeadNumber;
            cfgInfo.EnablePreDispensing = EnablePreDispensing;
            cfgInfo.EnableRealTimeHeightMeasurement = EnableRealTimeHeightMeasurement;
            cfgInfo.Enable2D = Enable2D;
            cfgInfo.EnableAutoReset = EnableAutoReset;
            cfgInfo.SinglePreDispensingCount = SinglePreDispensingCount;
            cfgInfo.PreDispensingType = PreDispensingType;
            cfgInfo.PreDispensingPointCountX = PreDispensingPointCountX;
            cfgInfo.PreDispensingPointCountY = PreDispensingPointCountY;
            cfgInfo.DispensingStyle = DispensingStyle;
            cfgInfo.DispensingWeight = DispensingWeight;
            cfgInfo.CachedHeightValue = CachedHeightValue;
            DotMatrix_LeftUpperPositionViewModel.CopyTo(cfgInfo.DotMatrix_LeftUpperPositionInfo);
            DotMatrix_RightUpperPositionViewModel.CopyTo(cfgInfo.DotMatrix_RightUpperPositionInfo);
            DotMatrix_RightLowerPositionViewModel.CopyTo(cfgInfo.DotMatrix_RightLowerPositionInfo);
        }

        #region 私有方法
        /// <summary>
        /// 订阅子ViewModel的变更事件
        /// </summary>
        private void subscribeChildViewModelEvents()
        {
            // 数字输入框事件订阅
            FunctionHeadNumberViewModel.ValueChanged += onChildValueChanged;
            SinglePreDispensingCountViewModel.ValueChanged += onChildValueChanged;
            PreDispensingPointCountXViewModel.ValueChanged += onChildValueChanged;
            PreDispensingPointCountYViewModel.ValueChanged += onChildValueChanged;
            DispensingWeightViewModel.ValueChanged += onChildValueChanged;
            CachedHeightValueViewModel.ValueChanged += onChildValueChanged;

            // 开关按钮事件订阅
            EnablePreDispensingViewModel.ValueChanged += onChildValueChanged;
            EnableRealTimeHeightMeasurementViewModel.ValueChanged += onChildValueChanged;
            Enable2DViewModel.ValueChanged += onChildValueChanged;
            EnableAutoResetViewModel.ValueChanged += onChildValueChanged;

            // 下拉框事件订阅
            PreDispensingTypeViewModel.ValueChanged += onChildSelectionChanged;
            DispensingStyleViewModel.ValueChanged += onChildSelectionChanged;
        }

        /// <summary>
        /// 子ViewModel值变更时触发全局事件
        /// </summary>
        private void onChildValueChanged(object? sender, ValueChangedEventArgs e)
        {
            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 子ViewModel选中项变更时触发全局事件
        /// </summary>
        private void onChildSelectionChanged(object? sender, EventArgs e)
        {
            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 重置参数到默认值
        /// </summary>
        private void resetParameters()
        {
            FunctionHeadNumber = 1;
            EnablePreDispensing = false;
            EnableRealTimeHeightMeasurement = false;
            Enable2D = false;
            EnableAutoReset = false;
            SinglePreDispensingCount = 1;
            PreDispensingType = DispensingType.Point;
            PreDispensingPointCountX = 1;
            PreDispensingPointCountY = 1;
            DispensingStyle = "样式一";
            DispensingWeight = 1.0m;
            CachedHeightValue = 0.0m;
        }
        #endregion
    }

    #endregion

    #region 配置信息
    /// <summary>
    /// 预点胶功能参数配置信息
    /// </summary>
    public class PreDispensingCfgInfo
    {
        /// <summary>
        /// 功能头编号
        /// </summary>
        public int FunctionHeadNumber { get; set; }

        /// <summary>
        /// 开启预点胶
        /// </summary>
        public bool EnablePreDispensing { get; set; }

        /// <summary>
        /// 开启实时测高
        /// </summary>
        public bool EnableRealTimeHeightMeasurement { get; set; }

        /// <summary>
        /// 开启2D
        /// </summary>
        public bool Enable2D { get; set; }

        /// <summary>
        /// 开启自动重置
        /// </summary>
        public bool EnableAutoReset { get; set; }

        /// <summary>
        /// 单次预点胶个数
        /// </summary>
        public int SinglePreDispensingCount { get; set; }

        /// <summary>
        /// 预点胶类型（选项：点、直线）
        /// </summary>
        public DispensingType PreDispensingType { get; set; }

        /// <summary>
        /// 预点胶点数X个数
        /// </summary>
        public int PreDispensingPointCountX { get; set; }

        /// <summary>
        /// 预点胶点数Y个数
        /// </summary>
        public int PreDispensingPointCountY { get; set; }

        /// <summary>
        /// 点胶样式（如：样式一、样式二）
        /// </summary>
        public string DispensingStyle { get; set; } = string.Empty;

        /// <summary>
        /// 点胶重量（单位：g，建议取值范围 0.1-10.0）
        /// </summary>
        public decimal DispensingWeight { get; set; }

        /// <summary>
        /// 缓存测高值（单位：mm，建议取值范围 0-50.0）
        /// </summary>
        public decimal CachedHeightValue { get; set; }


        /// <summary>
        /// 点阵左上位置
        /// </summary>
        public BasePositionInfo DotMatrix_LeftUpperPositionInfo { get; set; }

        /// <summary>
        /// 点阵右上位置
        /// </summary>
        public BasePositionInfo DotMatrix_RightUpperPositionInfo { get; set; }
        /// <summary>
        /// 点阵右下位置
        /// </summary>
        public BasePositionInfo DotMatrix_RightLowerPositionInfo { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public PreDispensingCfgInfo()
        {
            DotMatrix_LeftUpperPositionInfo = new BasePositionInfo();
            DotMatrix_RightUpperPositionInfo = new BasePositionInfo();
            DotMatrix_RightLowerPositionInfo = new BasePositionInfo();
        }
        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            // 基础类型
            FunctionHeadNumber = jObject["FunctionHeadNumber"]?.Value<int>() ?? 0;
            EnablePreDispensing = jObject["EnablePreDispensing"]?.Value<bool>() ?? false;
            EnableRealTimeHeightMeasurement = jObject["EnableRealTimeHeightMeasurement"]?.Value<bool>() ?? false;
            Enable2D = jObject["Enable2D"]?.Value<bool>() ?? false;
            EnableAutoReset = jObject["EnableAutoReset"]?.Value<bool>() ?? false;
            SinglePreDispensingCount = jObject["SinglePreDispensingCount"]?.Value<int>() ?? 0;

            // 枚举类型
            PreDispensingType = jObject["PreDispensingType"]?.Value<string>() switch
            {
                "Line" => DispensingType.Line,
                "Point" => DispensingType.Point,
                _ => DispensingType.Point // 默认值
            };

            PreDispensingPointCountX = jObject["PreDispensingPointCountX"]?.Value<int>() ?? 0;
            PreDispensingPointCountY = jObject["PreDispensingPointCountY"]?.Value<int>() ?? 0;
            DispensingStyle = jObject["DispensingStyle"]?.Value<string>() ?? string.Empty;
            DispensingWeight = jObject["DispensingWeight"]?.Value<decimal>() ?? 0;
            CachedHeightValue = jObject["CachedHeightValue"]?.Value<decimal>() ?? 0;

            // 嵌套位置对象
            if (jObject["DotMatrix_LeftUpperPositionInfo"] is JObject leftUpperObj)
            {
                DotMatrix_LeftUpperPositionInfo ??= new BasePositionInfo();
                DotMatrix_LeftUpperPositionInfo.FromJObject(leftUpperObj);
            }
            if (jObject["DotMatrix_RightUpperPositionInfo"] is JObject rightUpperObj)
            {
                DotMatrix_RightUpperPositionInfo ??= new BasePositionInfo();
                DotMatrix_RightUpperPositionInfo.FromJObject(rightUpperObj);
            }
            if (jObject["DotMatrix_RightLowerPositionInfo"] is JObject rightLowerObj)
            {
                DotMatrix_RightLowerPositionInfo ??= new BasePositionInfo();
                DotMatrix_RightLowerPositionInfo.FromJObject(rightLowerObj);
            }
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public JObject ToJObject()
        {
            var jObject = new JObject();

            // 基础类型
            jObject["FunctionHeadNumber"] = FunctionHeadNumber;
            jObject["EnablePreDispensing"] = EnablePreDispensing;
            jObject["EnableRealTimeHeightMeasurement"] = EnableRealTimeHeightMeasurement;
            jObject["Enable2D"] = Enable2D;
            jObject["EnableAutoReset"] = EnableAutoReset;
            jObject["SinglePreDispensingCount"] = SinglePreDispensingCount;

            // 枚举类型（序列化为字符串）
            jObject["PreDispensingType"] = PreDispensingType.ToString();

            jObject["PreDispensingPointCountX"] = PreDispensingPointCountX;
            jObject["PreDispensingPointCountY"] = PreDispensingPointCountY;
            jObject["DispensingStyle"] = DispensingStyle;
            jObject["DispensingWeight"] = DispensingWeight;
            jObject["CachedHeightValue"] = CachedHeightValue;

            // 嵌套位置对象
            jObject["DotMatrix_LeftUpperPositionInfo"] = DotMatrix_LeftUpperPositionInfo?.ToJObject() ?? new JObject();
            jObject["DotMatrix_RightUpperPositionInfo"] = DotMatrix_RightUpperPositionInfo?.ToJObject() ?? new JObject();
            jObject["DotMatrix_RightLowerPositionInfo"] = DotMatrix_RightLowerPositionInfo?.ToJObject() ?? new JObject();

            return jObject;
        }
    }
    /// <summary>
    /// 点胶类型枚举
    /// </summary>
    public enum DispensingType
    {
        /// <summary>
        /// 点
        /// </summary>
        Point,
        /// <summary>
        /// 直线
        /// </summary>
        Line,
    } 
    #endregion
}
