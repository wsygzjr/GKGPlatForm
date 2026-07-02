using Avalonia;
using Avalonia.Controls;
using ReactiveUI;
using System.Reactive.Disposables;

namespace GKG.UI
{
    /// <summary>
    /// 带标题的日期选择器控件
    /// </summary>
    public class CalendarDatePickerControl : BasicControl<CalendarDatePickerViewModel>
    { 
        private CalendarDatePicker _calendarDatePicker;

        /// <summary>
        /// 
        /// </summary>
        public CalendarDatePickerControl() : base()
        {
            _calendarDatePicker = new CalendarDatePicker
            {
                Name = "CalendarDatePicker1",
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                SelectedDateFormat = CalendarDatePickerFormat.Long,
            }; 
            initializeControl();
        }

        private void initializeControl()
        {

            base.SetContent(_calendarDatePicker , Avalonia.Layout.HorizontalAlignment.Stretch);

            this.WhenActivated(disposables =>
            { 
                this.Bind(ViewModel, vm => vm.SelectedDate, v => v._calendarDatePicker!.SelectedDate)
                    .DisposeWith(disposables);
                 
                this.Bind(ViewModel, vm => vm.DisplayDateStart, v => v._calendarDatePicker!.DisplayDateStart)
                   .DisposeWith(disposables);
                 
                this.Bind(ViewModel, vm => vm.DisplayDateEnd , v => v._calendarDatePicker!.DisplayDateEnd)
                   .DisposeWith(disposables); 

                this.Bind(ViewModel, vm => vm.DisplayDate, v => v._calendarDatePicker!.DisplayDate)
                    .DisposeWith(disposables);

                // 禁用
                this.Bind(ViewModel, vm => vm.IsEnabled, v => v._calendarDatePicker!.IsEnabled)
                    .DisposeWith(disposables); 

                // 显示
                this.Bind(ViewModel, vm => vm.IsVisible, v => v._calendarDatePicker!.IsVisible)
                    .DisposeWith(disposables);

            });
             
        }
    }

    /// <summary>
    /// 数字输入框控件-视图模型
    /// </summary>
    public class CalendarDatePickerViewModel : BasicControlViewModel
    {
        private DateTime _selectedDate;
        private DateTime _displayDateStart; 
        private DateTime _displayDateEnd;
        private DateTime _displayDate;
        private bool _isEnabled = true;

        /// <summary>
        /// 当前值 
        /// </summary>
        public DateTime SelectedDate
        {
            get => _selectedDate; 
            set
            {
                if (_selectedDate != value)
                {
                    var oldValue = _selectedDate; // 记录旧值
                    this.RaiseAndSetIfChanged(ref _selectedDate , value);
                    OnValueChanged(oldValue, value); // 触发基类事件
                }
            }
        }
        /// <summary>
        /// 日期范围开始时间  
        /// </summary> 
        public DateTime DisplayDateStart
        {
            get => _displayDateStart;
            set => this.RaiseAndSetIfChanged(ref _displayDateStart, value);
        }
        /// <summary>
        /// 日期范围结束时间 
        /// </summary> 
        public DateTime DisplayDateEnd
        {
            get => _displayDateEnd;
            set => this.RaiseAndSetIfChanged(ref _displayDateEnd, value);
        }

        /// <summary>
        /// 指定日历弹窗默认显示的日期（不一定是选中日期）
        /// </summary> 
        public DateTime DisplayDate
        {
            get => _displayDate;
            set => this.RaiseAndSetIfChanged(ref _displayDate, value);
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