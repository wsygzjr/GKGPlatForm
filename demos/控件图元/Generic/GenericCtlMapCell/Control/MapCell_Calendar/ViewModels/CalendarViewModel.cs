using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GKG.Map.MapCell.Generic.Control.MapCell_Calendar.Objects;
using ReactiveUI;
using System.Reactive;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Calendar.ViewModels
{
    public class CalendarViewModel : PropertyModels.ComponentModel.ReactiveObject, IDisposable
    {
        private readonly bool _designTime;
        private readonly CalendarPropertyModelEdit _propertyModel;
        private readonly Action _selectedDatesChangedAction;
        private readonly Action<CalendarDisplayModeType, CalendarDisplayModeType> _displayModeChangedAction;
        private bool _disposed;
        // 日历交互先落到待确认状态，点“确定”后再统一写回模型。
        private DateTime? _pendingSelectedDate;
        private List<DateTime> _pendingSelectedDates = new();
        private DateTime _pendingDisplayDate = DateTime.Today;

        public bool IsDesignTime => _designTime;

        public CalendarPropertyModelEdit Model => _propertyModel;

        public CalendarBrushInfo BrushInfo => _propertyModel.BrushInfo;
        public CalendarAppearanceInfo AppearanceInfo => _propertyModel.AppearanceInfo;
        public CalendarCommonInfo CommonInfo => _propertyModel.CommonInfo;
        public CalendarLayoutInfo LayoutInfo => _propertyModel.LayoutInfo;
        public CalendarTextInfo TextInfo => _propertyModel.TextInfo;
        public CalendarMiscInfo MiscInfo => _propertyModel.MiscInfo;

        public ReactiveCommand<Unit, Unit> TodayCommand { get; }
        public ReactiveCommand<Unit, Unit> ConfirmCommand { get; }

        public double OpacityValue => 1.0 - AppearanceInfo.Opacity / 100.0;

        public DateTime? PendingSelectedDate => _pendingSelectedDate;

        public IReadOnlyList<DateTime> PendingSelectedDates => _pendingSelectedDates;

        public DateTime PendingDisplayDate => _pendingDisplayDate;

        public CalendarViewModel(
            bool designTime,
            CalendarPropertyModelEdit propertyModel,
            Action selectedDatesChangedAction,
            Action<CalendarDisplayModeType, CalendarDisplayModeType> displayModeChangedAction)
        {
            _designTime = designTime;
            _propertyModel = propertyModel;
            _selectedDatesChangedAction = selectedDatesChangedAction;
            _displayModeChangedAction = displayModeChangedAction;

            TodayCommand = ReactiveCommand.Create(ApplyTodayPending);
            ConfirmCommand = ReactiveCommand.Create(CommitPendingSelection);

            _propertyModel.PropertyChanged += PropertyModel_PropertyChanged;
            _propertyModel.BrushInfo.PropertyChanged += ChildPropertyChanged;
            _propertyModel.AppearanceInfo.PropertyChanged += ChildPropertyChanged;
            _propertyModel.CommonInfo.PropertyChanged += ChildPropertyChanged;
            _propertyModel.LayoutInfo.PropertyChanged += ChildPropertyChanged;
            _propertyModel.TextInfo.PropertyChanged += ChildPropertyChanged;
            _propertyModel.MiscInfo.PropertyChanged += ChildPropertyChanged;

            SyncPendingFromModel();
        }

        public void ReloadFromModel()
        {
            SyncPendingFromModel();
            RaisePropertyChanged(nameof(BrushInfo));
            RaisePropertyChanged(nameof(AppearanceInfo));
            RaisePropertyChanged(nameof(CommonInfo));
            RaisePropertyChanged(nameof(LayoutInfo));
            RaisePropertyChanged(nameof(TextInfo));
            RaisePropertyChanged(nameof(MiscInfo));
            RaisePropertyChanged(nameof(OpacityValue));
            RaisePropertyChanged(nameof(PendingSelectedDate));
            RaisePropertyChanged(nameof(PendingSelectedDates));
            RaisePropertyChanged(nameof(PendingDisplayDate));
        }

        public void SyncPendingFromModel()
        {
            // 同时兼容单选字符串和多选日期数组两种持久化来源。
            _pendingSelectedDates = CalendarValueHelpers.ParseDateList(MiscInfo.SelectedDates);
            if (_pendingSelectedDates.Count == 0 && CalendarValueHelpers.TryParseDate(MiscInfo.SelectedDate, out DateTime singleDate))
                _pendingSelectedDates.Add(singleDate.Date);

            _pendingSelectedDates = _pendingSelectedDates
                .Select(item => item.Date)
                .Distinct()
                .OrderBy(item => item)
                .ToList();

            _pendingSelectedDate = CalendarValueHelpers.TryParseDate(MiscInfo.SelectedDate, out DateTime selectedDate)
                ? selectedDate.Date
                : _pendingSelectedDates.FirstOrDefault();

            _pendingDisplayDate = CalendarValueHelpers.TryParseDate(MiscInfo.DisplayDate, out DateTime displayDate)
                ? displayDate.Date
                : (_pendingSelectedDate ?? DateTime.Today).Date;

            RaisePropertyChanged(nameof(PendingSelectedDate));
            RaisePropertyChanged(nameof(PendingSelectedDates));
            RaisePropertyChanged(nameof(PendingDisplayDate));
        }

        public void SetPendingSelection(DateTime? selectedDate, IEnumerable<DateTime> selectedDates, DateTime displayDate)
        {
            _pendingSelectedDate = selectedDate?.Date;
            _pendingSelectedDates = (selectedDates ?? Enumerable.Empty<DateTime>())
                .Select(item => item.Date)
                .Distinct()
                .OrderBy(item => item)
                .ToList();
            _pendingDisplayDate = displayDate.Date;

            RaisePropertyChanged(nameof(PendingSelectedDate));
            RaisePropertyChanged(nameof(PendingSelectedDates));
            RaisePropertyChanged(nameof(PendingDisplayDate));
        }

        public void SetPendingDisplayDate(DateTime displayDate)
        {
            _pendingDisplayDate = displayDate.Date;
            RaisePropertyChanged(nameof(PendingDisplayDate));
        }

        public void ApplyTodayPending()
        {
            DateTime today = DateTime.Today;
            if (MiscInfo.SelectionMode == CalendarSelectionModeType.None)
            {
                SetPendingSelection(null, Array.Empty<DateTime>(), today);
                return;
            }

            SetPendingSelection(today, new[] { today }, today);
        }

        public void CommitPendingSelection()
        {
            // 只有确认提交时，才真正更新模型并触发 SelectedDatesChanged 事件。
            MiscInfo.SelectedDate = CalendarValueHelpers.FormatDate(_pendingSelectedDate);
            MiscInfo.SelectedDates = CalendarValueHelpers.SerializeDateList(_pendingSelectedDates);
            MiscInfo.DisplayDate = CalendarValueHelpers.FormatDate(_pendingDisplayDate);
            RaisePropertyChanged(nameof(MiscInfo));
            _selectedDatesChangedAction?.Invoke();
        }

        public void UpdateDisplayMode(CalendarDisplayModeType oldMode, CalendarDisplayModeType newMode)
        {
            if (oldMode == newMode)
                return;

            MiscInfo.DisplayMode = newMode;
            RaisePropertyChanged(nameof(MiscInfo));
            _displayModeChangedAction?.Invoke(oldMode, newMode);
        }

        private void PropertyModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        private void ChildPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // 文本颜色和画笔前景色对外表现为同一套语义，这里保持双向同步。
            if (sender is CalendarTextInfo && e.PropertyName == nameof(CalendarTextInfo.FontColor))
            {
                _propertyModel.BrushInfo.ForegroundColor = _propertyModel.TextInfo.FontColor;
            }
            else if (sender is CalendarBrushInfo && e.PropertyName == nameof(CalendarBrushInfo.ForegroundColor))
            {
                _propertyModel.TextInfo.FontColor = _propertyModel.BrushInfo.ForegroundColor;
            }
            else if (sender is CalendarAppearanceInfo && e.PropertyName == nameof(CalendarAppearanceInfo.Opacity))
            {
                RaisePropertyChanged(nameof(OpacityValue));
            }
            else if (sender is CalendarMiscInfo)
            {
                SyncPendingFromModel();
            }

            string propertyName = sender switch
            {
                CalendarBrushInfo => nameof(BrushInfo),
                CalendarAppearanceInfo => nameof(AppearanceInfo),
                CalendarCommonInfo => nameof(CommonInfo),
                CalendarLayoutInfo => nameof(LayoutInfo),
                CalendarTextInfo => nameof(TextInfo),
                CalendarMiscInfo => nameof(MiscInfo),
                _ => string.Empty
            };

            if (!string.IsNullOrWhiteSpace(propertyName))
                RaisePropertyChanged(propertyName);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _propertyModel.PropertyChanged -= PropertyModel_PropertyChanged;
                _propertyModel.BrushInfo.PropertyChanged -= ChildPropertyChanged;
                _propertyModel.AppearanceInfo.PropertyChanged -= ChildPropertyChanged;
                _propertyModel.CommonInfo.PropertyChanged -= ChildPropertyChanged;
                _propertyModel.LayoutInfo.PropertyChanged -= ChildPropertyChanged;
                _propertyModel.TextInfo.PropertyChanged -= ChildPropertyChanged;
                _propertyModel.MiscInfo.PropertyChanged -= ChildPropertyChanged;
            }

            _disposed = true;
        }
    }
}
