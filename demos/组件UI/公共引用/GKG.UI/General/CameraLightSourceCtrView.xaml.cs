using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Newtonsoft.JsonG.Linq;
using ReactiveUI;

namespace GKG.UI.General
{
    /// <summary>
    /// 相机与光源控制-视图
    /// </summary>
    public partial class CameraLightSourceCtrView : UserControl
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public CameraLightSourceCtrView()
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
    /// 相机与光源控制-视图模型
    /// </summary>
    public class CameraLightSourceCtrViewModel : ReactiveObject
    {
        /// <summary>
        /// 页面布局根视图引用（用于UI上下文关联）
        /// </summary>
        private Control? _viewReference;
        private Control? _cameraPreControl;
        /// <summary>
        /// 相机视频显示控件
        /// </summary>
        public Control? CameraPreControl
        {
            get => _cameraPreControl;
            set => this.RaiseAndSetIfChanged(ref _cameraPreControl, value);
        }

        /// <summary>
        /// 是否开启刻度-开关按钮ViewModel
        /// </summary>
        public ToggleSwitchViewModel EnabledScaleViewModel { get; }

        /// <summary>
        /// 是否鼠标移动-开关按钮ViewModel
        /// </summary>
        public ToggleSwitchViewModel EnabledMouseMoveViewModel { get; }

        /// <summary>
        /// 设置模板和搜索框-下拉框ViewModel
        /// </summary>
        public ComboxViewModel SetTemplateSearchBoxViewModel { get; }
        /// <summary>
        /// 坐标轴配置信息
        /// </summary>
        public AxisViewModel AxisViewModel { set; get; }
        /// <summary>
        /// 下光源信息
        /// </summary>
        public LightSourceViewModel DownLightSourceViewModel { set; get; }
        /// <summary>
        /// 上光源视图模型
        /// </summary>
        public UpLightSourceViewModel UpLightSourceViewModel { set; get; }
        /// <summary>
        /// 光源参数视图模型
        /// </summary>
        public LightSourceParamViewModel LightSourceParamViewModel { set; get; }

        /// <summary>
        /// 是否开启刻度
        /// </summary>
        public bool EnabledScale
        {
            get => EnabledScaleViewModel.IsChecked;
            set
            {
                EnabledScaleViewModel.IsChecked = value;
                this.RaisePropertyChanged(nameof(EnabledScale));
            }
        }

        /// <summary>
        /// 是否鼠标移动
        /// </summary>
        public bool EnabledMouseMove
        {
            get => EnabledMouseMoveViewModel.IsChecked;
            set
            {
                EnabledMouseMoveViewModel.IsChecked = value;
                this.RaisePropertyChanged(nameof(EnabledMouseMove));
            }
        }

        /// <summary>
        /// 设置模板和搜索框枚举
        /// </summary>
        public SetTemplateSearchBox SetTemplateSearchBox
        {
            get
            {
                if (SetTemplateSearchBoxViewModel.ItemsSource == null)
                    return SetTemplateSearchBox.TemplateBox;

                var selectedItem = SetTemplateSearchBoxViewModel.SelectedItem as ComBoxItem;
                return selectedItem != null ? (SetTemplateSearchBox)selectedItem.Value : SetTemplateSearchBox.TemplateBox;
            }
            set
            {
                if (SetTemplateSearchBoxViewModel.ItemsSource == null)
                    return;

                var targetItem = SetTemplateSearchBoxViewModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (SetTemplateSearchBox)o.Value == value);
                if (targetItem != null)
                    SetTemplateSearchBoxViewModel.SelectedItem = targetItem;

                this.RaisePropertyChanged(nameof(SetTemplateSearchBox));

            }
        }

        /// <summary>
        /// 事件
        /// </summary>
        public event EventHandler? AfterModified;
        /// <summary>
        /// 构造函数
        /// </summary>
        public CameraLightSourceCtrViewModel()
        {
            AxisViewModel = new AxisViewModel();
            DownLightSourceViewModel = new LightSourceViewModel();
            UpLightSourceViewModel = new UpLightSourceViewModel();
            LightSourceParamViewModel = new LightSourceParamViewModel();

            EnabledScaleViewModel = new ToggleSwitchViewModel { IsChecked = false };
            EnabledMouseMoveViewModel = new ToggleSwitchViewModel { IsChecked = false };

            SetTemplateSearchBoxViewModel = new ComboxViewModel();
            var reactionDisplayNames = new Dictionary<SetTemplateSearchBox, string>
            {
                { SetTemplateSearchBox.TemplateBox, "设置模板框" },
                { SetTemplateSearchBox.SearchBox, "设置搜索框" }
            };
            var templateSearchItems = EnumExtensions.ToEnumItems<SetTemplateSearchBox>(reactionDisplayNames);
            SetTemplateSearchBoxViewModel.ItemsSource = templateSearchItems;
            SetTemplateSearchBoxViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            SetTemplateSearchBoxViewModel.SelectedItem = templateSearchItems.Cast<ComBoxItem>().FirstOrDefault();
            subscribeChildViewModelEvents();
        }

        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="cameraLightSourceCtrCfgInfo">相机显示配置信息</param>
        public void CopyFrom(CameraLightSourceCtrCfgInfo cameraLightSourceCtrCfgInfo)
        {
            EnabledScale = cameraLightSourceCtrCfgInfo.EnabledScale;
            EnabledMouseMove = cameraLightSourceCtrCfgInfo.EnabledMouseMove;
            SetTemplateSearchBox = cameraLightSourceCtrCfgInfo.SetTemplateSearchBox;

            AxisViewModel.Init(cameraLightSourceCtrCfgInfo.AxisCfgInfo);
            DownLightSourceViewModel.Init(cameraLightSourceCtrCfgInfo.DownLightSourceCfgInfo);
            UpLightSourceViewModel.Init(cameraLightSourceCtrCfgInfo.UpLightSourceCfgInfo);
            LightSourceParamViewModel.Init(cameraLightSourceCtrCfgInfo.LightSourceParamCfgInfo);
            
        }
        /// <summary>
        /// 复制到指定相机显示配置信息
        /// </summary>
        /// <param name="cameraLightSourceCtrCfgInfo">相机显示配置信息</param>
        public void CopyTo(CameraLightSourceCtrCfgInfo cameraLightSourceCtrCfgInfo)
        {
            cameraLightSourceCtrCfgInfo.EnabledScale = EnabledScale;
            cameraLightSourceCtrCfgInfo.EnabledMouseMove = EnabledMouseMove;
            cameraLightSourceCtrCfgInfo.SetTemplateSearchBox = SetTemplateSearchBox;

            DownLightSourceViewModel.Extract(cameraLightSourceCtrCfgInfo.DownLightSourceCfgInfo);
            AxisViewModel.Extract(cameraLightSourceCtrCfgInfo.AxisCfgInfo);
            UpLightSourceViewModel.Extract(cameraLightSourceCtrCfgInfo.UpLightSourceCfgInfo);
            LightSourceParamViewModel.Extract(cameraLightSourceCtrCfgInfo.LightSourceParamCfgInfo);
        }
        /// <summary>
        /// 订阅子ViewModel的事件
        /// </summary>
        private void subscribeChildViewModelEvents()
        {
            DownLightSourceViewModel.AfterModified += viewModel_ValueChanged;
        }
      
        /// <summary>
        /// 界面配置信息改变事件
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void viewModel_ValueChanged(object? sender, EventArgs e)
        {
            onAfterModified();
        }

        /// <summary>
        /// 触发配置修改事件
        /// </summary>
        private void onAfterModified()
        {
            AfterModified?.Invoke(this, EventArgs.Empty);
        }
    }


    #region 子页面视图模型

    #region 上光源视图模型
    /// <summary>
    /// 上光源视图模型
    /// </summary>
    public class UpLightSourceViewModel : ReactiveObject
    {
        /// <summary>
        /// 数据变更事件
        /// </summary>
        public event EventHandler? AfterModified;

        #region 响应式属性
        /// <summary>
        /// Up 通道（
        /// </summary>
        public SliderNumericViewModel UpViewModel { get; }

        /// <summary>
        /// Down 通道
        /// </summary>
        public SliderNumericViewModel DownViewModel { get; }

        /// <summary>
        /// Left 通道
        /// </summary>
        public SliderNumericViewModel LeftViewModel { get; }

        /// <summary>
        /// Right 通道
        /// </summary>
        public SliderNumericViewModel RightViewModel { get; }
        #endregion

        #region 属性
        /// <summary>
        /// Up 通道值
        /// </summary>
        public decimal Up
        {
            get => UpViewModel.Value;
            set => UpViewModel.Value = value;
        }

        /// <summary>
        /// Down 通道值
        /// </summary>
        public decimal Down
        {
            get => DownViewModel.Value;
            set => DownViewModel.Value = value;
        }

        /// <summary>
        /// Left 通道值
        /// </summary>
        public decimal Left
        {
            get => LeftViewModel.Value;
            set => LeftViewModel.Value = value;
        }

        /// <summary>
        /// Right 通道值
        /// </summary>
        public decimal Right
        {
            get => RightViewModel.Value;
            set => RightViewModel.Value = value;
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public UpLightSourceViewModel()
        {
            // 初始化通道参数：范围0-255，步长1
            UpViewModel = new SliderNumericViewModel { Minimum = 0, Maximum = 255, Increment = 1, Value = 0 };
            DownViewModel = new SliderNumericViewModel { Minimum = 0, Maximum = 255, Increment = 1, Value = 0 };
            LeftViewModel = new SliderNumericViewModel { Minimum = 0, Maximum = 255, Increment = 1, Value = 0 };
            RightViewModel = new SliderNumericViewModel { Minimum = 0, Maximum = 255, Increment = 1, Value = 0 };

            // 订阅子ViewModel事件
            subscribeChildViewModelEvents();
        }

        /// <summary>
        /// 从配置模型加载数据
        /// </summary>
        /// <param name="cfgInfo">上光源配置信息</param>
        public void Init(UpLightSourceCfgInfo cfgInfo)
        {
            if (cfgInfo == null) return;

            Up = cfgInfo.Up;
            Down = cfgInfo.Down;
            Left = cfgInfo.Left;
            Right = cfgInfo.Right;
        }

        /// <summary>
        /// 提取数据到配置模型
        /// </summary>
        /// <param name="cfgInfo">上光源配置信息</param>
        public void Extract(UpLightSourceCfgInfo cfgInfo)
        {
            if (cfgInfo == null) return;

            cfgInfo.Up = Up;
            cfgInfo.Down = Down;
            cfgInfo.Left = Left;
            cfgInfo.Right = Right;
        }

        /// <summary>
        /// 订阅子ViewModel事件
        /// </summary>
        private void subscribeChildViewModelEvents()
        {
            UpViewModel.ValueChanged += onChannelValueChanged;
            DownViewModel.ValueChanged += onChannelValueChanged;
            LeftViewModel.ValueChanged += onChannelValueChanged;
            RightViewModel.ValueChanged += onChannelValueChanged;
        }

        /// <summary>
        /// 通道值变更事件统一处理
        /// </summary>
        private void onChannelValueChanged(object? sender, ValueChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            onAfterModified();
        }

        /// <summary>
        /// 触发数据变更事件
        /// </summary>
        private void onAfterModified()
        {
            AfterModified?.Invoke(this, EventArgs.Empty);
        }
    }
    #endregion

    #region 光源参数视图模型
    /// <summary>
    /// 光源参数视图模型
    /// </summary>
    public class LightSourceParamViewModel : ReactiveObject
    {
        /// <summary>
        /// 数据变更事件
        /// </summary>
        public event EventHandler? AfterModified;

        #region 响应式属性
        /// <summary>
        /// 曝光值（ms）
        /// </summary>
        public SliderNumericViewModel ExposureValueViewModel { get; }

        /// <summary>
        /// 曝光增益（倍数）
        /// </summary>
        public SliderNumericViewModel ExposureGainViewModel { get; }

        /// <summary>
        /// 对比度（%）
        /// </summary>
        public SliderNumericViewModel ContrastViewModel { get; }

        /// <summary>
        /// 对比度阈值（灰度值0-255）
        /// </summary>
        public SliderNumericViewModel ContrastThresholdViewModel { get; }
        #endregion

        #region 对外暴露的属性
        /// <summary>
        /// 曝光值（ms）
        /// </summary>
        public decimal ExposureValue
        {
            get => ExposureValueViewModel.Value;
            set => ExposureValueViewModel.Value = value;
        }

        /// <summary>
        /// 曝光增益
        /// </summary>
        public decimal ExposureGain
        {
            get => ExposureGainViewModel.Value;
            set => ExposureGainViewModel.Value = value;
        }

        /// <summary>
        /// 对比度（%）
        /// </summary>
        public decimal Contrast
        {
            get => ContrastViewModel.Value;
            set => ContrastViewModel.Value = value;
        }

        /// <summary>
        /// 对比度阈值（灰度值0-255）
        /// </summary>
        public decimal ContrastThreshold
        {
            get => ContrastThresholdViewModel.Value;
            set => ContrastThresholdViewModel.Value = value;
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public LightSourceParamViewModel()
        {
            ExposureValueViewModel = new SliderNumericViewModel
            {
                Minimum = 1,          // 最小1ms
                Maximum = 1000,       // 最大1000ms
                Increment = 1,        // 步长1ms
                DecimalPlaces = 0,    // 无小数
                Value = 100           // 默认值100ms
            };

            ExposureGainViewModel = new SliderNumericViewModel
            {
                Minimum = 1.0m,       // 最小1.0倍
                Maximum = 10.0m,      // 最大10.0倍
                Increment = 0.1m,     // 步长0.1倍
                DecimalPlaces = 1,    // 1位小数，
                Value = 1.0m          // 默认值1.0倍
            };

            ContrastViewModel = new SliderNumericViewModel
            {
                Minimum = 0,          // 最小0%
                Maximum = 100,        // 最大100%
                Increment = 1,        // 步长1%
                DecimalPlaces = 0,    // 无小数，
                Value = 50            // 默认值50%
            };

            ContrastThresholdViewModel = new SliderNumericViewModel
            {
                Minimum = 0,          // 最小0（纯黑）
                Maximum = 255,        // 最大255（纯白）
                Increment = 1,        // 步长1
                DecimalPlaces = 0,    // 无小数
                Value = 128           // 默认值128（中间灰度）
            };

            // 订阅子ViewModel事件
            subscribeChildViewModelEvents();
        }

        /// <summary>
        /// 从配置模型加载数据
        /// </summary>
        /// <param name="paramCfgInfo">光源参数配置信息</param>
        public void Init(LightSourceParamCfgInfo paramCfgInfo)
        {
            if (paramCfgInfo == null) return;

            ExposureValue = paramCfgInfo.ExposureValue;
            ExposureGain = paramCfgInfo.ExposureGain;
            Contrast = paramCfgInfo.Contrast;
            ContrastThreshold = paramCfgInfo.ContrastThreshold;
        }

        /// <summary>
        /// 提取数据到配置模型
        /// </summary>
        /// <param name="paramCfgInfo">光源参数配置信息</param>
        public void Extract(LightSourceParamCfgInfo paramCfgInfo)
        {
            if (paramCfgInfo == null) return;

            paramCfgInfo.ExposureValue = ExposureValue; 
            paramCfgInfo.ExposureGain = ExposureGain;
            paramCfgInfo.Contrast = Contrast;
            paramCfgInfo.ContrastThreshold = ContrastThreshold;
        }

        /// <summary>
        /// 订阅子ViewModel事件
        /// </summary>
        private void subscribeChildViewModelEvents()
        {
            ExposureValueViewModel.ValueChanged += onParamValueChanged;
            ExposureGainViewModel.ValueChanged += onParamValueChanged;
            ContrastViewModel.ValueChanged += onParamValueChanged;
            ContrastThresholdViewModel.ValueChanged += onParamValueChanged;
        }

        /// <summary>
        /// 参数值变更事件统一处理
        /// </summary>
        private void onParamValueChanged(object? sender, ValueChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            // 触发外部变更事件
            onAfterModified();
        }

        /// <summary>
        /// 触发数据变更事件
        /// </summary>
        private void onAfterModified()
        {
            AfterModified?.Invoke(this, EventArgs.Empty);
        }
    }
    #endregion
    #endregion
    #endregion

    #region 相机与光源控制配置信息
    /// <summary>
    /// 相机与光源控制配置信息
    /// </summary>
    public class CameraLightSourceCtrCfgInfo
    {
        /// <summary>
        /// 三维坐标位置配置信息
        /// </summary>
        public AxisCfgInfo AxisCfgInfo { set; get; } = new AxisCfgInfo();

        /// <summary>
        /// 是否开启刻度
        /// </summary>
        public bool EnabledScale { set; get; }

        /// <summary>
        /// 是否鼠标移动
        /// </summary>
        public bool EnabledMouseMove { set; get; }

        /// <summary>
        /// 设置模板和搜索框枚举
        /// </summary>
        public SetTemplateSearchBox SetTemplateSearchBox { set; get; }

        /// <summary>
        /// 下光源信息
        /// </summary>
        public LightSourceCfgInfo DownLightSourceCfgInfo { set; get; } = new LightSourceCfgInfo();

        /// <summary>
        /// 上光源信息
        /// </summary>
        public UpLightSourceCfgInfo UpLightSourceCfgInfo { set; get; } = new UpLightSourceCfgInfo();

        /// <summary>
        /// 光源参数信息
        /// </summary>
        public LightSourceParamCfgInfo LightSourceParamCfgInfo { set; get; } = new LightSourceParamCfgInfo();

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            // 嵌套对象
            AxisCfgInfo.FromJObject(jObject["AxisCfgInfo"] as JObject);
            DownLightSourceCfgInfo.FromJObject(jObject["DownLightSourceCfgInfo"] as JObject);
            UpLightSourceCfgInfo.FromJObject(jObject["UpLightSourceCfgInfo"] as JObject);
            LightSourceParamCfgInfo.FromJObject(jObject["LightSourceParamCfgInfo"] as JObject);

            // 基础类型 + 枚举
            EnabledScale = jObject["EnabledScale"]?.Value<bool>() ?? false;
            EnabledMouseMove = jObject["EnabledMouseMove"]?.Value<bool>() ?? false;
            SetTemplateSearchBox = jObject["SetTemplateSearchBox"] != null
                ? (SetTemplateSearchBox)jObject["SetTemplateSearchBox"].Value<int>()
                : SetTemplateSearchBox.TemplateBox;
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public JObject ToJObject()
        {
            return new JObject
            {
                { "AxisCfgInfo", AxisCfgInfo.ToJObject() },
                { "EnabledScale", EnabledScale },
                { "EnabledMouseMove", EnabledMouseMove },
                { "SetTemplateSearchBox", (int)SetTemplateSearchBox },
                { "DownLightSourceCfgInfo", DownLightSourceCfgInfo.ToJObject() },
                { "UpLightSourceCfgInfo", UpLightSourceCfgInfo.ToJObject() },
                { "LightSourceParamCfgInfo", LightSourceParamCfgInfo.ToJObject() }
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public void CopyFrom(CameraLightSourceCtrCfgInfo source)
        {
            if (source == null) return;

            AxisCfgInfo.CopyFrom(source.AxisCfgInfo);
            DownLightSourceCfgInfo.CopyFrom(source.DownLightSourceCfgInfo);
            UpLightSourceCfgInfo.CopyFrom(source.UpLightSourceCfgInfo);
            LightSourceParamCfgInfo.CopyFrom(source.LightSourceParamCfgInfo);

            EnabledScale = source.EnabledScale;
            EnabledMouseMove = source.EnabledMouseMove;
            SetTemplateSearchBox = source.SetTemplateSearchBox;
        }
    }

    /// <summary>
    /// 三维坐标配置信息
    /// </summary>
    public class AxisCfgInfo
    {
        /// <summary>
        /// X轴坐标（mm）
        /// </summary>
        public decimal X { get; set; } = 0.0m;

        /// <summary>
        /// Y轴坐标（mm）
        /// </summary>
        public decimal Y { get; set; } = 0.0m;

        /// <summary>
        /// Z轴坐标（mm）
        /// </summary>
        public decimal Z { get; set; } = 0.0m;

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;
            X = jObject["X"]?.Value<decimal>() ?? 0.0m;
            Y = jObject["Y"]?.Value<decimal>() ?? 0.0m;
            Z = jObject["Z"]?.Value<decimal>() ?? 0.0m;
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public JObject ToJObject()
        {
            return new JObject
            {
                { "X", X },
                { "Y", Y },
                { "Z", Z }
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public void CopyFrom(AxisCfgInfo source)
        {
            if (source == null) return;

            X = source.X;
            Y = source.Y;
            Z = source.Z;
        }
    }
    

    /// <summary>
    /// 上光源信息
    /// </summary>
    public class UpLightSourceCfgInfo
    {
        /// <summary>
        /// Up通道（0-255）
        /// </summary>
        public decimal Up { set; get; } = 0.0m;

        /// <summary>
        /// Down通道（0-255）
        /// </summary>
        public decimal Down { set; get; } = 0.0m;

        /// <summary>
        /// Left通道（0-255）
        /// </summary>
        public decimal Left { set; get; } = 0.0m;

        /// <summary>
        /// Right通道（0-255）
        /// </summary>
        public decimal Right { set; get; } = 0.0m;

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;
            Up = jObject["Up"]?.Value<decimal>() ?? 0.0m;
            Down = jObject["Down"]?.Value<decimal>() ?? 0.0m;
            Left = jObject["Left"]?.Value<decimal>() ?? 0.0m;
            Right = jObject["Right"]?.Value<decimal>() ?? 0.0m;
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public JObject ToJObject()
        {
            return new JObject
            {
                { "Up", Up },
                { "Down", Down },
                { "Left", Left },
                { "Right", Right }
            };
        }
        /// <summary>
        /// 复制
        /// </summary>
        public void CopyFrom(UpLightSourceCfgInfo source)
        {
            if (source == null) return;

            Up = source.Up;
            Down = source.Down;
            Left = source.Left;
            Right = source.Right;
        }
    }

    /// <summary>
    /// 光源参数信息
    /// </summary>
    public class LightSourceParamCfgInfo
    {
        /// <summary>
        /// 曝光值（单位：ms）
        /// </summary>
        public decimal ExposureValue { get; set; } = 100;

        /// <summary>
        /// 曝光增益（倍数）
        /// </summary>
        public decimal ExposureGain { get; set; } = 1.0m;

        /// <summary>
        /// 对比度（%）
        /// </summary>
        public decimal Contrast { get; set; } = 50;

        /// <summary>
        /// 对比度阈值（灰度值0-255）
        /// </summary>
        public decimal ContrastThreshold { get; set; } = 128;

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;
            ExposureValue = jObject["ExposureValue"]?.Value<decimal>() ?? 100;
            ExposureGain = jObject["ExposureGain"]?.Value<decimal>() ?? 1.0m;
            Contrast = jObject["Contrast"]?.Value<decimal>() ?? 50;
            ContrastThreshold = jObject["ContrastThreshold"]?.Value<decimal>() ?? 128;
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public JObject ToJObject()
        {
            return new JObject
            {
                { "ExposureValue", ExposureValue },
                { "ExposureGain", ExposureGain },
                { "Contrast", Contrast },
                { "ContrastThreshold", ContrastThreshold }
            };
        }
        /// <summary>
        /// 复制
        /// </summary>
        public void CopyFrom(LightSourceParamCfgInfo source)
        {
            if (source == null) return;

            ExposureValue = source.ExposureValue;
            ExposureGain = source.ExposureGain;
            Contrast = source.Contrast;
            ContrastThreshold = source.ContrastThreshold;
        }
    }
   

    /// <summary>
    /// 设置模板和搜索框枚举
    /// </summary>
    public enum SetTemplateSearchBox
    {
        /// <summary>
        /// 设置模板框
        /// </summary>
        TemplateBox,
        /// <summary>
        /// 设置搜索框
        /// </summary>
        SearchBox
    }
    #endregion
}
