using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Newtonsoft.JsonG.Linq;
using System;
using System.Globalization;

namespace GKG.UI
{
    /// <summary>
    /// 无标题的时分秒选择器
    /// </summary>
    public class TimeHmsPicker : UserControl
    { 
        /// <summary>
        /// 核心依赖属性：选中的完整时间
        /// </summary>
        public static readonly StyledProperty<TimeSpan> SelectedTimeProperty =
            AvaloniaProperty.Register<TimeHmsPicker, TimeSpan>(
                nameof(SelectedTime),
                defaultValue: TimeSpan.Zero,
                defaultBindingMode: BindingMode.TwoWay);
         
        /// <summary>
        /// 时依赖属性：（内部绑定用）
        /// </summary>
        public static readonly StyledProperty<int> SelectedHourProperty =
            AvaloniaProperty.Register<TimeHmsPicker, int>(nameof(SelectedHour), 0);
        /// <summary>
        /// 分依赖属性：（内部绑定用）
        /// </summary>
        public static readonly StyledProperty<int> SelectedMinuteProperty =
            AvaloniaProperty.Register<TimeHmsPicker, int>(nameof(SelectedMinute), 0);
        /// <summary>
        /// 秒依赖属性：（内部绑定用）
        /// </summary>
        public static readonly StyledProperty<int> SelectedSecondProperty =
            AvaloniaProperty.Register<TimeHmsPicker, int>(nameof(SelectedSecond), 0);

        // UI元素实例
        private readonly Grid _rootGrid;
        private readonly NumericUpDown _hourPicker;
        private readonly NumericUpDown _minutePicker;
        private readonly NumericUpDown _secondPicker;

        // 标记：避免循环更新
        private bool _isUpdatingFromSelectedTime;

        /// <summary>
        /// 构造函数
        /// </summary>
        public TimeHmsPicker()
        {
            // ========== 1. 初始化UI布局 ==========
            _rootGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Parse("10")),
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Parse("10")),
                    new ColumnDefinition(GridLength.Star)
                },
                RowDefinitions = { new RowDefinition(GridLength.Auto) },
                //RowDefinitions = { new RowDefinition(GridLength.Parse("30")) },
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center, 
                Margin = new Thickness(0),
                ColumnSpacing = 0,
                Background = new SolidColorBrush(Colors.Gray),
                Width = 360,
                Height = 30,
            };

            // 小时选择器
            _hourPicker = CreateNumericUpDown(0, 23);
            Grid.SetColumn(_hourPicker, 0);

            var _hourPickerAfterTextBlock = CreateTextBlock(":");
            Grid.SetColumn(_hourPickerAfterTextBlock, 1);

            // 分钟选择器
            _minutePicker = CreateNumericUpDown(0, 59);
            Grid.SetColumn(_minutePicker, 2);

            var _minutePickerAfterTextBlock = CreateTextBlock(":");
            Grid.SetColumn(_minutePickerAfterTextBlock, 3);

            // 秒选择器
            _secondPicker = CreateNumericUpDown(0, 59);
            Grid.SetColumn(_secondPicker, 4);

            // 添加到根布局
            _rootGrid.Children.Add(_hourPicker);
            _rootGrid.Children.Add(_hourPickerAfterTextBlock);
            _rootGrid.Children.Add(_minutePicker);
            _rootGrid.Children.Add(_minutePickerAfterTextBlock);
            _rootGrid.Children.Add(_secondPicker);

            // 设置控件内容
            Content = _rootGrid;
            Height = 32;
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center ;
            VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            Background = new SolidColorBrush(Colors.Red);

            // ========== 2. 绑定依赖属性  ========== 
            _hourPicker.Bind(
                NumericUpDown.ValueProperty,
                new Binding(nameof(SelectedHour))
                {
                    Source = this,
                    Mode = BindingMode.TwoWay, 
                    // 禁用绑定的更新源触发（避免循环）
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

            _minutePicker.Bind(
                NumericUpDown.ValueProperty,
                new Binding(nameof(SelectedMinute))
                {
                    Source = this,
                    Mode = BindingMode.TwoWay, 
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

            _secondPicker.Bind(
                NumericUpDown.ValueProperty,
                new Binding(nameof(SelectedSecond))
                {
                    Source = this,
                    Mode = BindingMode.TwoWay, 
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

            // ========== 3. 监听逻辑（ 避免循环） ==========
            // 监听时分秒变化 → 更新SelectedTime 
            PropertyChanged += (s, e) =>
            {
                // 若当前是从SelectedTime同步过来的，跳过（避免循环更新）
                if (_isUpdatingFromSelectedTime) return;

                if (e.Property == SelectedHourProperty ||
                    e.Property == SelectedMinuteProperty ||
                    e.Property == SelectedSecondProperty)
                {
                    UpdateSelectedTime();
                }
            };

            // 监听SelectedTime变化 → 同步到时分秒选择器
            PropertyChanged += (s, e) =>
            {
                if (e.Property == SelectedTimeProperty)
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        // 标记：开始从SelectedTime同步，避免触发上面的监听
                        _isUpdatingFromSelectedTime = true;
                        try
                        {
                            SetCurrentValue(SelectedHourProperty, SelectedTime.Hours);
                            SetCurrentValue(SelectedMinuteProperty, SelectedTime.Minutes);
                            SetCurrentValue(SelectedSecondProperty, SelectedTime.Seconds);
                        }
                        finally
                        {
                            // 取消标记
                            _isUpdatingFromSelectedTime = false;
                        }
                    });
                }
            };
        }

        // ========== 辅助方法：创建NumericUpDown ==========
        private NumericUpDown CreateNumericUpDown(int min, int max)
        {
            return new NumericUpDown
            {
                Minimum = min,
                Maximum = max,
                Width = 100,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Increment = 1, 
                FormatString = "F0",
                Watermark = $"{min:D2}",
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(0), 
                Margin = new Thickness(0),
            };
        }

        // ========== 辅助方法：创建TextBlock ==========
        private TextBlock CreateTextBlock(string text)
        {
            return new TextBlock
            {
                Text = text,
                Margin = new Thickness(0),
                FontSize = 14,
                Width = 10,
                Foreground = Brushes.Black,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center,
            };
        }

        // ========== 核心方法：更新SelectedTime（确保通知绑定） ==========
        private void UpdateSelectedTime()
        {
            var newTime = new TimeSpan(
                Math.Clamp(SelectedHour, 0, 23),
                Math.Clamp(SelectedMinute, 0, 59),
                Math.Clamp(SelectedSecond, 0, 59)
            );
            if (SelectedTime != newTime)
            {
                // 使用SetCurrentValue确保属性变更通知被触发
                SetCurrentValue(SelectedTimeProperty, newTime); 
            }
        }

        // ========== 公开属性 ==========
        /// <summary>
        /// 选中的完整时间的时间戳
        /// </summary>
        public TimeSpan SelectedTime
        {
            get => GetValue(SelectedTimeProperty);
            set => SetValue(SelectedTimeProperty, value);
        }
        /// <summary>
        /// 选中的小时
        /// </summary>
        public int SelectedHour
        {
            get => GetValue(SelectedHourProperty);
            set => SetValue(SelectedHourProperty, Math.Clamp(value, 0, 23));
        }
        /// <summary>
        /// 选中的分钟
        /// </summary>
        public int SelectedMinute
        {
            get => GetValue(SelectedMinuteProperty);
            set => SetValue(SelectedMinuteProperty, Math.Clamp(value, 0, 59));
        }
        /// <summary>
        /// 选中的秒钟
        /// </summary>
        public int SelectedSecond
        {
            get => GetValue(SelectedSecondProperty);
            set => SetValue(SelectedSecondProperty, Math.Clamp(value, 0, 59));
        }

        // ========== 设计时尺寸 ==========
        /// <summary>
        /// 设计时尺寸
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(365, 42);
        }
    } 
}