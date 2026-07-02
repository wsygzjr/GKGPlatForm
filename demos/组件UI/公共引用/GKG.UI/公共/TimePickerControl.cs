using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using ReactiveUI;
using System;
using System.Collections;
using System.Reactive.Disposables;

namespace GKG.UI
{
    /// <summary>
    /// 带标题的时分选择器
    /// </summary>
    public class TimePickerControl : BasicControl<TimePickerViewModel>
    { 
        private TimePicker _timePicker;

        /// <summary>
        /// 
        /// </summary>
        public TimePickerControl() : base()
        {
            _timePicker = new TimePicker
            {
                Name = "TimePicker1",
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                ClockIdentifier = "24HourClock",
                MinuteIncrement = 1,
                SelectedTime = new TimeSpan(15, 30, 0),

            };
            initializeControl();
        }

        private void initializeControl()
        {
            base.SetContent(_timePicker, Avalonia.Layout.HorizontalAlignment.Left);

            this.WhenActivated(disposables =>
            { 
                this.Bind(ViewModel, vm => vm.SelectedTime, v => v._timePicker!.SelectedTime)
                    .DisposeWith(disposables);
                 
                this.Bind(ViewModel, vm => vm.MinuteIncrement, v => v._timePicker!.MinuteIncrement)
                   .DisposeWith(disposables);  
            });
             
        }
    }

    /// <summary>
    /// 数字输入框控件-视图模型
    /// </summary>
    public class TimePickerViewModel : BasicControlViewModel
    {
        private TimeSpan _selectedTime;
        private int _minuteIncrement;

        /// <summary>
        /// 当前值 
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
        /// 设置分钟选择的步长（例如 15 表示只能选择 0、15、30、45 分）。默认值为 1  
        /// </summary> 
        public int MinuteIncrement
        {
            get => _minuteIncrement;
            set => this.RaiseAndSetIfChanged(ref _minuteIncrement, value);
        }
    }

}