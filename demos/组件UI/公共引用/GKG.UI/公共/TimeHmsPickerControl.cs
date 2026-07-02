using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using ReactiveUI;
using System.Reactive.Disposables;

namespace GKG.UI
{
    /// <summary>
    /// 带标题的时分秒选择器控件
    /// </summary>
    public class TimeHmsPickerControl : BasicControl<TimeHmsPickerViewModel>
    {
        private TimeHmsPicker? _timePicker;

        /// <summary>
        /// 构造函数
        /// </summary>
        public TimeHmsPickerControl() : base()
        {
            initializeTextBox();
        }

        private void initializeTextBox()
        {
            // 1. 创建自定义时分秒控件
            _timePicker = new TimeHmsPicker
            {
                Margin = new Thickness(0), 
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            }; 

            base.SetContent(_timePicker, Avalonia.Layout.HorizontalAlignment.Stretch);

            this.WhenActivated(disposables =>
            {
                this.Bind(ViewModel, vm => vm.SelectedTime, v => v._timePicker!.SelectedTime)
                    .DisposeWith(disposables); 

                // 禁用
                this.Bind(ViewModel, vm => vm.IsEnabled, v => v._timePicker!.IsEnabled)
                    .DisposeWith(disposables); 

                // 显示
                this.Bind(ViewModel, vm => vm.IsVisible, v => v._timePicker!.IsVisible)
                    .DisposeWith(disposables);

            });
        }
    }

    /// <summary>
    /// 带标题的时分秒选择器控件-视图模型
    /// </summary>
    public class TimeHmsPickerViewModel : BasicControlViewModel
    {
        private TimeSpan _selectedTime  ;
        private bool _isEnabled = true;

        /// <summary>
        /// 显示的时分秒
        /// </summary>
        public TimeSpan SelectedTime
        {
            get => _selectedTime;
            set
            {
                if (_selectedTime != value)
                {
                    var oldValue = _selectedTime; // 记录旧值
                    this.RaiseAndSetIfChanged(ref _selectedTime, value);
                    OnValueChanged(oldValue, value); // 触发基类事件
                }
            }
        } 

        /// <summary>
        /// 是否禁用，缺省不禁用true
        /// </summary> 
        public bool IsEnabled
        {
            get => _isEnabled;
            set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
        }

        private bool _isVisible = true;
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            set => this.RaiseAndSetIfChanged(ref _isVisible, value);
        }

    }

}