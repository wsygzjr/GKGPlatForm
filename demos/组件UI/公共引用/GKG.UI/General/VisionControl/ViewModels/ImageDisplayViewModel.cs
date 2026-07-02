using Avalonia.Media;
using AvaloniaVisionControl;
using ReactiveUI;

namespace GKG.UI.General
{
    /// <summary>
    /// 图像显示设置配置信息
    /// </summary>
    public class ImageDisplayViewModel : ReactiveObject
    {
        /// <summary>
        /// 是否开启参照圆-数据模型
        /// </summary>
        public ToggleSwitchViewModel EnabledReferenceCircleViewModel { get; }

        /// <summary>
        /// 是否开启刻度-数据模型
        /// </summary>
        public ToggleSwitchViewModel EnabledScaleViewModel { get; }
        /// <summary>
        /// 是否开启调试亮度-数据模型
        /// </summary>
        public ToggleSwitchViewModel EnabledBrightnessDebuggingViewModel { get; }

        /// <summary>
        /// 刻度（mm）-下拉框数据模型
        /// </summary>
        public ComboxViewModel ScaleComboxViewModel { get; }

        /// <summary>
        /// 参照圆（mm）-数据模型
        /// </summary>
        public NumericWithLableViewModel ReferenceCircleViewModel { get; }

        /// <summary>
        /// 刻度选项数据源（预设可选的刻度值）
        /// </summary>
        private List<ComBoxItem> _scaleItems;

        private bool _enabledScale; 
        /// <summary>
        /// 是否开启刻度（开关按钮）
        /// </summary>
        public bool EnabledScale
        {
            get => _enabledScale;
            set
            {
                if (_enabledScale != value)
                {
                    var oldValue = _enabledScale; // 记录旧值
                    this.RaiseAndSetIfChanged(ref _enabledScale, value);
                }
            }
        }

        private bool _enabledReferenceCircle;  
        /// <summary>
        /// 是否开启参照圆（开关按钮）
        /// </summary>
        public bool EnabledReferenceCircle
        {
            get => _enabledReferenceCircle;
            set
            {
                if (_enabledReferenceCircle != value)
                {
                    var oldValue = _enabledReferenceCircle; // 记录旧值
                    this.RaiseAndSetIfChanged(ref _enabledReferenceCircle, value);
                    enabledDrawReferenceCircle(value);
                }
            }
        }
  
        private bool _enabledBrightnessDebugging; 
        /// <summary>
        /// 是否开启调试亮度（开关按钮）
        /// </summary>     
        public bool EnabledBrightnessDebugging
        {
            get => _enabledBrightnessDebugging;
            set
            {
                if (_enabledBrightnessDebugging != value)
                {
                    var oldValue = _enabledBrightnessDebugging; // 记录旧值
                    this.RaiseAndSetIfChanged(ref _enabledBrightnessDebugging, value);
                }
            }
        }

        /// <summary>
        /// 当前选中的刻度值（mm）
        /// </summary>
        public decimal Scale
        {
            get
            {
                // 从下拉框选中项中获取刻度值
                if (ScaleComboxViewModel.SelectedItem is ComBoxItem selectedItem)
                {
                    return (decimal)selectedItem.Value;
                }
                return 0; // 默认值
            }
            set
            {
                // 根据值设置下拉框选中项
                var targetItem = _scaleItems.FirstOrDefault(item => (decimal)item.Value == value);
                if (targetItem != null)
                {
                    ScaleComboxViewModel.SelectedItem = targetItem;
                }
            }
        }

        private decimal _referenceCircle; 
        /// <summary>
        /// 参照圆（mm）
        /// </summary>
        public decimal ReferenceCircle
        {
            get => _referenceCircle;
            set
            {
                if (_referenceCircle != value)
                {
                    var oldValue = _referenceCircle; // 记录旧值
                    this.RaiseAndSetIfChanged(ref _referenceCircle, value);
                }
            }
        }

        /// <summary>
        /// 事件（通知外部数据变更）
        /// </summary>
        public event EventHandler? AfterModified;
        /// <summary>
        /// 构造函数
        /// </summary>
        public ImageDisplayViewModel()
        {
            // 初始化开关数据模型
            EnabledScaleViewModel = new ToggleSwitchViewModel();
            EnabledReferenceCircleViewModel = new ToggleSwitchViewModel();
            EnabledBrightnessDebuggingViewModel = new ToggleSwitchViewModel();

            // 初始化刻度下拉框数据模型
            ScaleComboxViewModel = new ComboxViewModel();
            _scaleItems = getScaleItems(); // 获取预设刻度选项
            ScaleComboxViewModel.ItemsSource = _scaleItems;
            ScaleComboxViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            ScaleComboxViewModel.SelectedItem = _scaleItems.FirstOrDefault();
            // 初始化参照圆数据模型
            ReferenceCircleViewModel = new NumericWithLableViewModel
            {
                LableText = "mm",
                Minimum = 0.1m,
                Increment = 0.1m,
                DecimalPlaces = 1,
                Value = 5.0m
            };

            // 订阅所有子ViewModel的变更事件
            subscribeChildViewModelEvents();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageDisplayCfgInfo"></param>
        public void Init(ImageDisplayCfgInfo imageDisplayCfgInfo)
        {
            this.EnabledScale = imageDisplayCfgInfo.EnabledScale;
            this.EnabledReferenceCircle = imageDisplayCfgInfo.EnabledReferenceCircle;
            this.Scale = imageDisplayCfgInfo.Scale;
            this.ReferenceCircle = imageDisplayCfgInfo.ReferenceCircle;
            this.EnabledBrightnessDebugging = imageDisplayCfgInfo.EnabledBrightnessDebugging;
        }

        /// <summary>
        /// 提取参数
        /// </summary>
        /// <param name="imageDisplayCfgInfo"></param>
        public void Extract(ImageDisplayCfgInfo imageDisplayCfgInfo)
        {
            imageDisplayCfgInfo.EnabledScale = this.EnabledScale;
            imageDisplayCfgInfo.EnabledReferenceCircle = this.EnabledReferenceCircle;
            imageDisplayCfgInfo.Scale = this.Scale;
            imageDisplayCfgInfo.ReferenceCircle = this.ReferenceCircle;
            imageDisplayCfgInfo.EnabledBrightnessDebugging = this.EnabledBrightnessDebugging;
        }


        /// <summary>
        /// 是否开启参照圆
        /// </summary>
        /// <param name="enabledReferenceCircle"></param>
        private void enabledDrawReferenceCircle(bool enabledReferenceCircle)
        {
            if (enabledReferenceCircle)
                addElements();
            else
                clearElements();
        }
        /// <summary>
        /// 添加图元
        /// </summary>
        private void addElements()
        {
            var imageControl = GlobalVisionViewModel.CurActiveViewImageControl;
            if (imageControl == null) return;

            var elements = new List<PaintElement>();

            // 添加一个绿色矩形
            elements.Add(new PaintElement
            {
                Type = PaintElementType.Rectangle,
                Pts = new List<double> { -20.0, -20.0, 20.0, 20.0 },
                Color = Colors.Green,
                LineWidth = 0.5,
                IsFill = false,
                Visible = true
            });

            // 添加一个蓝色十字（中心点）
            elements.Add(new PaintElement
            {
                Type = PaintElementType.Cross,
                Pts = new List<double> { 0.0, 0.0 },
                Color = Colors.Blue,
                LineWidth = 0.5,
                Visible = true
            });
            imageControl.SetPaintElements(elements);
            imageControl.CtlShowPaintStatus = ImageElementCtlStatus.ShowAll;
            imageControl.ReFresh();
        }
        /// <summary>
        /// 清除图元
        /// </summary>
        private void clearElements()
        {
            var imageControl = GlobalVisionViewModel.CurActiveViewImageControl;
            if (imageControl == null) return;
            imageControl.SetPaintElements(new List<PaintElement>());
            imageControl.ReFresh();
        }
        /// <summary>
        /// 预设刻度选项
        /// </summary>
        private List<ComBoxItem> getScaleItems()
        {
            return new List<ComBoxItem>
        {
            new ComBoxItem { DisplayName = "0.1mm", Value =(decimal) 0.1 },
            new ComBoxItem { DisplayName = "0.2mm", Value = (decimal)0.2 },
        };
        }

        /// <summary>
        /// 订阅子ViewModel的事件
        /// </summary>
        private void subscribeChildViewModelEvents()
        {
            EnabledScaleViewModel.ValueChanged += viewModel_ValueChanged;
            EnabledBrightnessDebuggingViewModel.ValueChanged += viewModel_ValueChanged;
            ScaleComboxViewModel.ValueChanged += viewModel_ValueChanged;
            ReferenceCircleViewModel.ValueChanged += viewModel_ValueChanged;
        }

        /// <summary>
        /// 界面配置信息改变事件
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void viewModel_ValueChanged(object? sender, ValueChangedEventArgs e)
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
}
