using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using GKG.Map.MapCell.Generic.Control.MapCell_Calendar.Objects;
using GKG.Map.MapCell.Generic.Control.MapCell_Calendar.ViewModels;
using ReactiveUI;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Calendar.Views
{
    public partial class CalendarView : ReactiveUserControl<CalendarViewModel>
    {
        private bool _suppressCalendarEvents;
        private const string CalendarBackgroundBrushResourceKey = "CalendarBackgroundBrush";
        private const string CalendarForegroundBrushResourceKey = "CalendarForegroundBrush";

        public CalendarView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                if (ViewModel == null)
                    return;

                ViewModel.ReloadFromModel();
                ApplyAllFromViewModel(reloadPending: true);

                PropertyChangedEventHandler propertyChangedHandler = (_, e) =>
                {
                    bool reloadPending = string.IsNullOrWhiteSpace(e.PropertyName) || e.PropertyName == nameof(CalendarViewModel.MiscInfo);
                    ApplyAllFromViewModel(reloadPending);
                };
                ViewModel.PropertyChanged += propertyChangedHandler;
                Disposable.Create(() => ViewModel.PropertyChanged -= propertyChangedHandler).DisposeWith(disposables);

                NotifyCollectionChangedEventHandler selectedDatesChangedHandler = (_, __) => HandleCalendarSelectionChanged();
                ((INotifyCollectionChanged)CalendarHost.SelectedDates).CollectionChanged += selectedDatesChangedHandler;
                Disposable.Create(() => ((INotifyCollectionChanged)CalendarHost.SelectedDates).CollectionChanged -= selectedDatesChangedHandler).DisposeWith(disposables);

                EventHandler<CalendarModeChangedEventArgs> displayModeChangedHandler = (_, e) =>
                {
                    if (_suppressCalendarEvents || ViewModel == null)
                        return;

                    ViewModel.UpdateDisplayMode(ToDisplayMode(e.OldMode), ToDisplayMode(e.NewMode));
                };
                CalendarHost.DisplayModeChanged += displayModeChangedHandler;
                Disposable.Create(() => CalendarHost.DisplayModeChanged -= displayModeChangedHandler).DisposeWith(disposables);

                CalendarHost.GetObservable(Calendar.DisplayDateProperty)
                    .Skip(1)
                    .Subscribe(displayDate =>
                    {
                        if (_suppressCalendarEvents || ViewModel == null)
                            return;

                        // 日历翻页先只更新待确认显示日期；隐藏底部按钮时再直接提交。
                        ViewModel.SetPendingDisplayDate(displayDate.Date);
                        if (!ViewModel.CommonInfo.ShowButtonPanel)
                            ViewModel.CommitPendingSelection();
                    })
                    .DisposeWith(disposables);

                Observable.FromEventPattern<RoutedEventArgs>(
                        addHandler: handler => TodayButton.Click += handler,
                        removeHandler: handler => TodayButton.Click -= handler)
                    .Subscribe(_ =>
                    {
                        if (ViewModel == null)
                            return;

                        ViewModel.ApplyTodayPending();
                        ApplyPendingStateToCalendar();
                    })
                    .DisposeWith(disposables);

                Observable.FromEventPattern<RoutedEventArgs>(
                        addHandler: handler => ConfirmButton.Click += handler,
                        removeHandler: handler => ConfirmButton.Click -= handler)
                    .Subscribe(_ => ViewModel?.CommitPendingSelection())
                    .DisposeWith(disposables);

            });
        }

        private void HandleCalendarSelectionChanged()
        {
            if (_suppressCalendarEvents || ViewModel == null)
                return;

            List<DateTime> selectedDates = CalendarHost.SelectedDates.Cast<DateTime>().Select(item => item.Date).Distinct().OrderBy(item => item).ToList();
            DateTime? selectedDate = CalendarHost.SelectedDate?.Date;
            if (!selectedDate.HasValue && selectedDates.Count == 1)
                selectedDate = selectedDates[0];

            // 视图交互先写入待确认状态；是否立刻提交由 ShowButtonPanel 控制。
            ViewModel.SetPendingSelection(selectedDate, selectedDates, CalendarHost.DisplayDate.Date);
            if (!ViewModel.CommonInfo.ShowButtonPanel)
                ViewModel.CommitPendingSelection();
        }

        private void ApplyAllFromViewModel(bool reloadPending)
        {
            if (ViewModel == null)
                return;

            if (reloadPending)
                ViewModel.SyncPendingFromModel();

            ApplyVisualState();
            ApplyCalendarConfiguration();
            ApplyPendingStateToCalendar();
        }

        private void ApplyVisualState()
        {
            if (ViewModel == null)
                return;

            SolidColorBrush backgroundBrush = new(ViewModel.BrushInfo.BackgroundColor);
            SolidColorBrush borderBrush = new(ViewModel.BrushInfo.BorderColor);
            SolidColorBrush foregroundBrush = new(ViewModel.TextInfo.FontColor);

            // Calendar 内部模板层级较深，统一通过资源把基础前景色/背景色透传进去。
            Resources[CalendarBackgroundBrushResourceKey] = backgroundBrush;
            Resources[CalendarForegroundBrushResourceKey] = foregroundBrush;

            RootBorder.Background = backgroundBrush;
            RootBorder.BorderBrush = borderBrush;
            RootBorder.BorderThickness = new Thickness(
                Math.Max(0, ViewModel.AppearanceInfo.BorderThicknessLeft),
                Math.Max(0, ViewModel.AppearanceInfo.BorderThicknessTop),
                Math.Max(0, ViewModel.AppearanceInfo.BorderThicknessRight),
                Math.Max(0, ViewModel.AppearanceInfo.BorderThicknessBottom));

            ButtonPanelBorder.Background = backgroundBrush;
            ButtonPanelBorder.BorderBrush = borderBrush;
            CalendarHost.Background = backgroundBrush;
            CalendarHost.Foreground = foregroundBrush;

            Opacity = ViewModel.OpacityValue;
            MinWidth = Math.Max(0, ViewModel.LayoutInfo.MinWidth);
            MinHeight = Math.Max(0, ViewModel.LayoutInfo.MinHeight);
            MaxWidth = ViewModel.LayoutInfo.MaxWidth <= 0 || ViewModel.LayoutInfo.MaxWidth == int.MaxValue ? double.PositiveInfinity : ViewModel.LayoutInfo.MaxWidth;
            MaxHeight = ViewModel.LayoutInfo.MaxHeight <= 0 || ViewModel.LayoutInfo.MaxHeight == int.MaxValue ? double.PositiveInfinity : ViewModel.LayoutInfo.MaxHeight;
            Margin = new Thickness(ViewModel.LayoutInfo.MarginLeft, ViewModel.LayoutInfo.MarginTop, ViewModel.LayoutInfo.MarginRight, ViewModel.LayoutInfo.MarginBottom);

            HorizontalAlignment = ViewModel.LayoutInfo.HorizontalAlignment switch
            {
                CalendarHorizontalAlignmentType.Left => Avalonia.Layout.HorizontalAlignment.Left,
                CalendarHorizontalAlignmentType.Center => Avalonia.Layout.HorizontalAlignment.Center,
                CalendarHorizontalAlignmentType.Right => Avalonia.Layout.HorizontalAlignment.Right,
                _ => Avalonia.Layout.HorizontalAlignment.Stretch
            };
            VerticalAlignment = ViewModel.LayoutInfo.VerticalAlignment switch
            {
                CalendarVerticalAlignmentType.Top => Avalonia.Layout.VerticalAlignment.Top,
                CalendarVerticalAlignmentType.Center => Avalonia.Layout.VerticalAlignment.Center,
                CalendarVerticalAlignmentType.Bottom => Avalonia.Layout.VerticalAlignment.Bottom,
                _ => Avalonia.Layout.VerticalAlignment.Stretch
            };

            Cursor cursor = new(ToStandardCursor(ViewModel.CommonInfo.HoverCursor));
            Cursor = cursor;
            RootBorder.Cursor = cursor;
            CalendarHost.Cursor = cursor;

            string fontFamilyName = CalendarTextInfo.ToFontFamilyName(ViewModel.TextInfo.FontFamily);
            if (!string.IsNullOrWhiteSpace(fontFamilyName))
                CalendarHost.FontFamily = new FontFamily(fontFamilyName);
            CalendarHost.FontSize = ViewModel.TextInfo.FontSize;
            CalendarHost.FontStyle = ViewModel.TextInfo.IsItalic ? FontStyle.Italic : FontStyle.Normal;
            CalendarHost.FontWeight = ViewModel.TextInfo.IsBold ? FontWeight.Bold : FontWeight.Normal;
            CalendarHost.Foreground = foregroundBrush;

            ButtonPanelBorder.IsVisible = ViewModel.CommonInfo.ShowButtonPanel;
            TodayButton.IsVisible = ViewModel.CommonInfo.ShowButtonPanel;
            ConfirmButton.IsVisible = ViewModel.CommonInfo.ShowButtonPanel;
            TodayButton.IsEnabled = ViewModel.CommonInfo.Enabled;
            ConfirmButton.IsEnabled = ViewModel.CommonInfo.Enabled;
        }

        private void ApplyCalendarConfiguration()
        {
            if (ViewModel == null)
                return;

            _suppressCalendarEvents = true;
            try
            {
                CalendarHost.IsEnabled = ViewModel.CommonInfo.Enabled;
                CalendarHost.SelectionMode = ViewModel.MiscInfo.SelectionMode switch
                {
                    CalendarSelectionModeType.SingleRange => CalendarSelectionMode.SingleRange,
                    CalendarSelectionModeType.MultipleRange => CalendarSelectionMode.MultipleRange,
                    CalendarSelectionModeType.None => CalendarSelectionMode.None,
                    _ => CalendarSelectionMode.SingleDate
                };
                CalendarHost.DisplayMode = ViewModel.MiscInfo.DisplayMode switch
                {
                    CalendarDisplayModeType.Month => CalendarMode.Year,
                    CalendarDisplayModeType.Year => CalendarMode.Decade,
                    _ => CalendarMode.Month
                };
                CalendarHost.IsTodayHighlighted = ViewModel.MiscInfo.IsTodayHighlighted;
                CalendarHost.FirstDayOfWeek = (DayOfWeek)ViewModel.MiscInfo.FirstDayOfWeek;
                CalendarHost.DisplayDateStart = CalendarValueHelpers.TryParseDate(ViewModel.MiscInfo.DisplayDateStart, out DateTime displayStart) ? displayStart.Date : null;
                CalendarHost.DisplayDateEnd = CalendarValueHelpers.TryParseDate(ViewModel.MiscInfo.DisplayDateEnd, out DateTime displayEnd) ? displayEnd.Date : null;

                CalendarHost.BlackoutDates.Clear();
                foreach (CalendarBlackoutRangeDto range in CalendarValueHelpers.ParseBlackoutRanges(ViewModel.MiscInfo.BlackoutDates))
                {
                    if (!CalendarValueHelpers.TryParseDate(range.Start, out DateTime start))
                        continue;
                    if (!CalendarValueHelpers.TryParseDate(range.End, out DateTime end))
                        continue;
                    CalendarHost.BlackoutDates.Add(new CalendarDateRange(start.Date, end.Date));
                }
            }
            finally
            {
                _suppressCalendarEvents = false;
            }
        }

        private void ApplyPendingStateToCalendar()
        {
            if (ViewModel == null)
                return;

            _suppressCalendarEvents = true;
            try
            {
                CalendarHost.SelectedDates.Clear();
                CalendarHost.SelectedDate = null;

                foreach (DateTime date in ViewModel.PendingSelectedDates)
                    CalendarHost.SelectedDates.Add(date.Date);

                if (ViewModel.PendingSelectedDate.HasValue)
                    CalendarHost.SelectedDate = ViewModel.PendingSelectedDate.Value.Date;

                CalendarHost.DisplayDate = ViewModel.PendingDisplayDate.Date;
            }
            finally
            {
                _suppressCalendarEvents = false;
            }
        }

        private static CalendarDisplayModeType ToDisplayMode(CalendarMode mode)
        {
            return mode switch
            {
                CalendarMode.Year => CalendarDisplayModeType.Month,
                CalendarMode.Decade => CalendarDisplayModeType.Year,
                _ => CalendarDisplayModeType.Day
            };
        }

        private static StandardCursorType ToStandardCursor(CalendarCursorType cursorType)
        {
            return cursorType switch
            {
                CalendarCursorType.IBeam => StandardCursorType.Ibeam,
                CalendarCursorType.Wait => StandardCursorType.Wait,
                CalendarCursorType.Cross => StandardCursorType.Cross,
                CalendarCursorType.UpArrow => StandardCursorType.UpArrow,
                CalendarCursorType.SizeWestEast => StandardCursorType.SizeWestEast,
                CalendarCursorType.SizeNorthSouth => StandardCursorType.SizeNorthSouth,
                CalendarCursorType.SizeAll => StandardCursorType.SizeAll,
                CalendarCursorType.No => StandardCursorType.No,
                CalendarCursorType.Hand => StandardCursorType.Hand,
                CalendarCursorType.AppStarting => StandardCursorType.AppStarting,
                CalendarCursorType.Help => StandardCursorType.Help,
                _ => StandardCursorType.Arrow
            };
        }
    }
}
