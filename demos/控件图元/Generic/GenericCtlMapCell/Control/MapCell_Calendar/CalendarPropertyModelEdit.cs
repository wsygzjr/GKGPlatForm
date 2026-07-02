using System;
using System.ComponentModel;
using GKG.Map.MapCell.Generic.Control.MapCell_Calendar.Objects;
using Griffins;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Calendar
{
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("图元信息", 1)]
    [CategoryPriority("画笔", 2)]
    [CategoryPriority("外观", 3)]
    [CategoryPriority("公共", 4)]
    [CategoryPriority("布局", 5)]
    [CategoryPriority("文本", 6)]
    [CategoryPriority("杂项", 7)]
    public class CalendarPropertyModelEdit : ControlCellPropertyModelEdit
    {
        private CalendarBrushInfo _brushInfo = new();
        private CalendarAppearanceInfo _appearanceInfo = new();
        private CalendarCommonInfo _commonInfo = new();
        private CalendarLayoutInfo _layoutInfo = new();
        private CalendarTextInfo _textInfo = new();
        private CalendarMiscInfo _miscInfo = new();

        [DisplayName("画笔设置")]
        [Category("画笔")]
        [PropertySortOrder(1)]
        public CalendarBrushInfo BrushInfo
        {
            get => _brushInfo;
            set => SetProperty(ref _brushInfo, value);
        }

        [DisplayName("外观设置")]
        [Category("外观")]
        [PropertySortOrder(1)]
        public CalendarAppearanceInfo AppearanceInfo
        {
            get => _appearanceInfo;
            set => SetProperty(ref _appearanceInfo, value);
        }

        [DisplayName("公共设置")]
        [Category("公共")]
        [PropertySortOrder(1)]
        public CalendarCommonInfo CommonInfo
        {
            get => _commonInfo;
            set => SetProperty(ref _commonInfo, value);
        }

        [DisplayName("布局设置")]
        [Category("布局")]
        [PropertySortOrder(1)]
        public CalendarLayoutInfo LayoutInfo
        {
            get => _layoutInfo;
            set => SetProperty(ref _layoutInfo, value);
        }

        [DisplayName("文本设置")]
        [Category("文本")]
        [PropertySortOrder(1)]
        public CalendarTextInfo TextInfo
        {
            get => _textInfo;
            set => SetProperty(ref _textInfo, value);
        }

        [DisplayName("杂项设置")]
        [Category("杂项")]
        [PropertySortOrder(1)]
        public CalendarMiscInfo MiscInfo
        {
            get => _miscInfo;
            set => SetProperty(ref _miscInfo, value);
        }

        public override bool SetPropertyValue(string propertyID, MapCellPropValue propertyVal)
        {
            if (string.Equals(propertyID, nameof(CalendarMiscInfo.SelectedDate), StringComparison.Ordinal))
            {
                _miscInfo ??= new CalendarMiscInfo();
                _miscInfo.SelectedDate = propertyVal?.ToPrimitiveValue<string>() ?? string.Empty;
                RaisePropertyChanged(nameof(MiscInfo));
                return true;
            }

            if (string.Equals(propertyID, nameof(CalendarMiscInfo.BlackoutDates), StringComparison.Ordinal))
            {
                _miscInfo ??= new CalendarMiscInfo();
                _miscInfo.BlackoutDates = propertyVal?.ToPrimitiveValue<string>() ?? "[]";
                RaisePropertyChanged(nameof(MiscInfo));
                return true;
            }

            if (string.Equals(propertyID, nameof(BrushInfo), StringComparison.Ordinal))
            {
                var src = propertyVal != null ? DeserializeObject<CalendarBrushInfo>(propertyVal) : new CalendarBrushInfo();
                _brushInfo ??= new CalendarBrushInfo();
                _brushInfo.BackgroundColor = src.BackgroundColor;
                _brushInfo.BorderColor = src.BorderColor;
                _brushInfo.ForegroundColor = src.ForegroundColor;
                RaisePropertyChanged(nameof(BrushInfo));
                return true;
            }

            if (string.Equals(propertyID, nameof(AppearanceInfo), StringComparison.Ordinal))
            {
                var src = propertyVal != null ? DeserializeObject<CalendarAppearanceInfo>(propertyVal) : new CalendarAppearanceInfo();
                _appearanceInfo ??= new CalendarAppearanceInfo();
                _appearanceInfo.Opacity = src.Opacity;
                _appearanceInfo.BorderThicknessLeft = src.BorderThicknessLeft;
                _appearanceInfo.BorderThicknessTop = src.BorderThicknessTop;
                _appearanceInfo.BorderThicknessRight = src.BorderThicknessRight;
                _appearanceInfo.BorderThicknessBottom = src.BorderThicknessBottom;
                RaisePropertyChanged(nameof(AppearanceInfo));
                return true;
            }

            if (string.Equals(propertyID, nameof(CommonInfo), StringComparison.Ordinal))
            {
                var src = propertyVal != null ? DeserializeObject<CalendarCommonInfo>(propertyVal) : new CalendarCommonInfo();
                _commonInfo ??= new CalendarCommonInfo();
                _commonInfo.HoverCursor = src.HoverCursor;
                _commonInfo.Enabled = src.Enabled;
                _commonInfo.ShowButtonPanel = src.ShowButtonPanel;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }

            if (string.Equals(propertyID, nameof(LayoutInfo), StringComparison.Ordinal))
            {
                var src = propertyVal != null ? DeserializeObject<CalendarLayoutInfo>(propertyVal) : new CalendarLayoutInfo();
                _layoutInfo ??= new CalendarLayoutInfo();
                // 宽高主数据统一迁到父类 Width/Height，LayoutInfo 不再承载宽高。
                _layoutInfo.HorizontalAlignment = src.HorizontalAlignment;
                _layoutInfo.VerticalAlignment = src.VerticalAlignment;
                _layoutInfo.MarginLeft = src.MarginLeft;
                _layoutInfo.MarginTop = src.MarginTop;
                _layoutInfo.MarginRight = src.MarginRight;
                _layoutInfo.MarginBottom = src.MarginBottom;
                _layoutInfo.MinWidth = src.MinWidth;
                _layoutInfo.MaxWidth = src.MaxWidth;
                _layoutInfo.MinHeight = src.MinHeight;
                _layoutInfo.MaxHeight = src.MaxHeight;
                RaisePropertyChanged(nameof(LayoutInfo));
                return true;
            }

            if (string.Equals(propertyID, nameof(TextInfo), StringComparison.Ordinal))
            {
                var src = propertyVal != null ? DeserializeObject<CalendarTextInfo>(propertyVal) : new CalendarTextInfo();
                _textInfo ??= new CalendarTextInfo();
                _textInfo.FontFamily = src.FontFamily;
                _textInfo.FontColor = src.FontColor;
                _textInfo.FontSize = src.FontSize;
                _textInfo.IsItalic = src.IsItalic;
                _textInfo.IsBold = src.IsBold;
                RaisePropertyChanged(nameof(TextInfo));
                return true;
            }

            if (string.Equals(propertyID, nameof(MiscInfo), StringComparison.Ordinal))
            {
                var src = propertyVal != null ? DeserializeObject<CalendarMiscInfo>(propertyVal) : new CalendarMiscInfo();
                _miscInfo ??= new CalendarMiscInfo();
                _miscInfo.SelectedDate = src.SelectedDate;
                _miscInfo.SelectedDates = src.SelectedDates;
                _miscInfo.DisplayDate = src.DisplayDate;
                _miscInfo.DisplayDateStart = src.DisplayDateStart;
                _miscInfo.DisplayDateEnd = src.DisplayDateEnd;
                _miscInfo.FirstDayOfWeek = src.FirstDayOfWeek;
                _miscInfo.SelectionMode = src.SelectionMode;
                _miscInfo.IsTodayHighlighted = src.IsTodayHighlighted;
                _miscInfo.DisplayMode = src.DisplayMode;
                _miscInfo.BlackoutDates = src.BlackoutDates;
                RaisePropertyChanged(nameof(MiscInfo));
                return true;
            }

            return false;
        }

        public void CopyFrom(CalendarPropertyModelEdit? source)
        {
            if (source == null)
                return;

            BrushInfo.BackgroundColor = source.BrushInfo?.BackgroundColor ?? CalendarBrushInfo.Default.BackgroundColor;
            BrushInfo.BorderColor = source.BrushInfo?.BorderColor ?? CalendarBrushInfo.Default.BorderColor;
            BrushInfo.ForegroundColor = source.BrushInfo?.ForegroundColor ?? CalendarBrushInfo.Default.ForegroundColor;

            AppearanceInfo.Opacity = source.AppearanceInfo?.Opacity ?? CalendarAppearanceInfo.Default.Opacity;
            AppearanceInfo.BorderThicknessLeft = source.AppearanceInfo?.BorderThicknessLeft ?? CalendarAppearanceInfo.Default.BorderThicknessLeft;
            AppearanceInfo.BorderThicknessTop = source.AppearanceInfo?.BorderThicknessTop ?? CalendarAppearanceInfo.Default.BorderThicknessTop;
            AppearanceInfo.BorderThicknessRight = source.AppearanceInfo?.BorderThicknessRight ?? CalendarAppearanceInfo.Default.BorderThicknessRight;
            AppearanceInfo.BorderThicknessBottom = source.AppearanceInfo?.BorderThicknessBottom ?? CalendarAppearanceInfo.Default.BorderThicknessBottom;

            CommonInfo.HoverCursor = source.CommonInfo?.HoverCursor ?? CalendarCommonInfo.Default.HoverCursor;
            CommonInfo.Enabled = source.CommonInfo?.Enabled ?? CalendarCommonInfo.Default.Enabled;
            CommonInfo.ShowButtonPanel = source.CommonInfo?.ShowButtonPanel ?? CalendarCommonInfo.Default.ShowButtonPanel;

            Width = source.Width;
            Height = source.Height;
            LayoutInfo.HorizontalAlignment = source.LayoutInfo?.HorizontalAlignment ?? CalendarLayoutInfo.Default.HorizontalAlignment;
            LayoutInfo.VerticalAlignment = source.LayoutInfo?.VerticalAlignment ?? CalendarLayoutInfo.Default.VerticalAlignment;
            LayoutInfo.MarginLeft = source.LayoutInfo?.MarginLeft ?? CalendarLayoutInfo.Default.MarginLeft;
            LayoutInfo.MarginTop = source.LayoutInfo?.MarginTop ?? CalendarLayoutInfo.Default.MarginTop;
            LayoutInfo.MarginRight = source.LayoutInfo?.MarginRight ?? CalendarLayoutInfo.Default.MarginRight;
            LayoutInfo.MarginBottom = source.LayoutInfo?.MarginBottom ?? CalendarLayoutInfo.Default.MarginBottom;
            LayoutInfo.MinWidth = source.LayoutInfo?.MinWidth ?? CalendarLayoutInfo.Default.MinWidth;
            LayoutInfo.MaxWidth = source.LayoutInfo?.MaxWidth ?? CalendarLayoutInfo.Default.MaxWidth;
            LayoutInfo.MinHeight = source.LayoutInfo?.MinHeight ?? CalendarLayoutInfo.Default.MinHeight;
            LayoutInfo.MaxHeight = source.LayoutInfo?.MaxHeight ?? CalendarLayoutInfo.Default.MaxHeight;

            TextInfo.FontFamily = source.TextInfo?.FontFamily ?? CalendarTextInfo.Default.FontFamily;
            TextInfo.FontColor = source.TextInfo?.FontColor ?? CalendarTextInfo.Default.FontColor;
            TextInfo.FontSize = source.TextInfo?.FontSize ?? CalendarTextInfo.Default.FontSize;
            TextInfo.IsItalic = source.TextInfo?.IsItalic ?? CalendarTextInfo.Default.IsItalic;
            TextInfo.IsBold = source.TextInfo?.IsBold ?? CalendarTextInfo.Default.IsBold;

            MiscInfo.SelectedDate = source.MiscInfo?.SelectedDate ?? CalendarMiscInfo.Default.SelectedDate;
            MiscInfo.SelectedDates = source.MiscInfo?.SelectedDates ?? CalendarMiscInfo.Default.SelectedDates;
            MiscInfo.DisplayDate = source.MiscInfo?.DisplayDate ?? CalendarMiscInfo.Default.DisplayDate;
            MiscInfo.DisplayDateStart = source.MiscInfo?.DisplayDateStart ?? CalendarMiscInfo.Default.DisplayDateStart;
            MiscInfo.DisplayDateEnd = source.MiscInfo?.DisplayDateEnd ?? CalendarMiscInfo.Default.DisplayDateEnd;
            MiscInfo.FirstDayOfWeek = source.MiscInfo?.FirstDayOfWeek ?? CalendarMiscInfo.Default.FirstDayOfWeek;
            MiscInfo.SelectionMode = source.MiscInfo?.SelectionMode ?? CalendarMiscInfo.Default.SelectionMode;
            MiscInfo.IsTodayHighlighted = source.MiscInfo?.IsTodayHighlighted ?? CalendarMiscInfo.Default.IsTodayHighlighted;
            MiscInfo.DisplayMode = source.MiscInfo?.DisplayMode ?? CalendarMiscInfo.Default.DisplayMode;
            MiscInfo.BlackoutDates = source.MiscInfo?.BlackoutDates ?? CalendarMiscInfo.Default.BlackoutDates;
        }

        private static T DeserializeObject<T>(MapCellPropValue val) where T : IMPPropObjectValue, new()
        {
            ObjectValue_Json objectValueJson = val.ToObjectValue_Json();
            GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValueJson);
            IMPPropObjectValue obj = new T();
            obj.PopulateFromBaseValue(griffinsBaseValue);
            return (T)obj;
        }
    }
}
