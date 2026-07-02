using System;
using System.ComponentModel;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Calendar.Objects
{
    public enum CalendarFirstDayOfWeekType
    {
        [Description("周日")] Sunday = 0,
        [Description("周一")] Monday = 1,
        [Description("周二")] Tuesday = 2,
        [Description("周三")] Wednesday = 3,
        [Description("周四")] Thursday = 4,
        [Description("周五")] Friday = 5,
        [Description("周六")] Saturday = 6
    }

    public enum CalendarSelectionModeType
    {
        [Description("单个日期")] SingleDate = 0,
        [Description("连续日期范围")] SingleRange = 1,
        [Description("多个不连续范围")] MultipleRange = 2,
        [Description("禁止选择")] None = 3
    }

    public enum CalendarDisplayModeType
    {
        [Description("Day")] Day = 0,
        [Description("Month")] Month = 1,
        [Description("Year")] Year = 2
    }

    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class CalendarMiscInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        public static readonly Guid Object_ID = new("{9c003424-ff28-497b-b3c4-9fbd43cffa9e}");
        public static readonly CalendarMiscInfo Default = new();

        private string _selectedDate = string.Empty;
        private string _selectedDates = "[]";
        private string _displayDate = CalendarValueHelpers.FormatDate(DateTime.Today);
        private string _displayDateStart = string.Empty;
        private string _displayDateEnd = string.Empty;
        private CalendarFirstDayOfWeekType _firstDayOfWeek = CalendarFirstDayOfWeekType.Sunday;
        private CalendarSelectionModeType _selectionMode = CalendarSelectionModeType.SingleDate;
        private bool _isTodayHighlighted = true;
        private CalendarDisplayModeType _displayMode = CalendarDisplayModeType.Day;
        private string _blackoutDates = "[]";

        [DisplayName("SelectedDate")]
        [Category("杂项")]
        public string SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, value ?? string.Empty);
        }

        [DisplayName("SelectedDates")]
        [Category("杂项")]
        public string SelectedDates
        {
            get => _selectedDates;
            set => SetProperty(ref _selectedDates, string.IsNullOrWhiteSpace(value) ? "[]" : value);
        }

        [DisplayName("DisplayDate")]
        [Category("杂项")]
        public string DisplayDate
        {
            get => _displayDate;
            set => SetProperty(ref _displayDate, value ?? string.Empty);
        }

        [DisplayName("显示开始日期")]
        [Category("杂项")]
        public string DisplayDateStart
        {
            get => _displayDateStart;
            set => SetProperty(ref _displayDateStart, value ?? string.Empty);
        }

        [DisplayName("显示结束日期")]
        [Category("杂项")]
        public string DisplayDateEnd
        {
            get => _displayDateEnd;
            set => SetProperty(ref _displayDateEnd, value ?? string.Empty);
        }

        [DisplayName("FirstDayOfWeek")]
        [Category("杂项")]
        public CalendarFirstDayOfWeekType FirstDayOfWeek
        {
            get => _firstDayOfWeek;
            set => SetProperty(ref _firstDayOfWeek, value);
        }

        [DisplayName("SelectionMode")]
        [Category("杂项")]
        public CalendarSelectionModeType SelectionMode
        {
            get => _selectionMode;
            set => SetProperty(ref _selectionMode, value);
        }

        [DisplayName("IsTodayHighlighted")]
        [Category("杂项")]
        public bool IsTodayHighlighted
        {
            get => _isTodayHighlighted;
            set => SetProperty(ref _isTodayHighlighted, value);
        }

        [DisplayName("DisplayMode")]
        [Category("杂项")]
        public CalendarDisplayModeType DisplayMode
        {
            get => _displayMode;
            set => SetProperty(ref _displayMode, value);
        }

        [DisplayName("BlackoutDates")]
        [Category("杂项")]
        public string BlackoutDates
        {
            get => _blackoutDates;
            set => SetProperty(ref _blackoutDates, string.IsNullOrWhiteSpace(value) ? "[]" : value);
        }

        bool IMPPropObjectValue.IsObject_Byte => false;

        Guid IMPPropObjectValue.GetObject_ID() => Object_ID;

        GriffinsBaseValue IMPPropObjectValue.ToBaseValue()
        {
            ObjectValue_Json objectValueJson = new(Object_ID) { JsonVal = ((IJsonValueConvert)this).ToJsonDataObject() };
            return GriffinsBaseValue.Create(objectValueJson);
        }

        void IMPPropObjectValue.PopulateFromBaseValue(GriffinsBaseValue baseValue)
        {
            if (baseValue?.Val is not ObjectValue_Json objectValueJson || objectValueJson.Object_ID != Object_ID)
                throw new Exception("对象值不是 CalendarMiscInfo 转换的");

            ((IJsonValueConvert)this).FromJsonDataObject(objectValueJson.JsonVal);
        }

        void IJsonValueConvert.FromJsonDataObject(string jsonDataObject)
        {
            using JsonDocument jsonDocument = JsonDocument.Parse(jsonDataObject);
            JsonElement root = jsonDocument.RootElement;

            if (root.TryGetProperty(nameof(SelectedDate), out JsonElement selectedDate) && selectedDate.ValueKind == JsonValueKind.String)
                _selectedDate = selectedDate.GetString() ?? string.Empty;
            if (root.TryGetProperty(nameof(SelectedDates), out JsonElement selectedDates) && selectedDates.ValueKind == JsonValueKind.String)
                _selectedDates = selectedDates.GetString() ?? "[]";
            if (root.TryGetProperty(nameof(DisplayDate), out JsonElement displayDate) && displayDate.ValueKind == JsonValueKind.String)
                _displayDate = displayDate.GetString() ?? string.Empty;
            if (root.TryGetProperty(nameof(DisplayDateStart), out JsonElement displayDateStart) && displayDateStart.ValueKind == JsonValueKind.String)
                _displayDateStart = displayDateStart.GetString() ?? string.Empty;
            if (root.TryGetProperty(nameof(DisplayDateEnd), out JsonElement displayDateEnd) && displayDateEnd.ValueKind == JsonValueKind.String)
                _displayDateEnd = displayDateEnd.GetString() ?? string.Empty;
            if (root.TryGetProperty(nameof(FirstDayOfWeek), out JsonElement firstDayOfWeek))
            {
                if (firstDayOfWeek.ValueKind == JsonValueKind.String && Enum.TryParse(firstDayOfWeek.GetString(), out CalendarFirstDayOfWeekType firstDay))
                    _firstDayOfWeek = firstDay;
                else if (firstDayOfWeek.ValueKind == JsonValueKind.Number && firstDayOfWeek.TryGetInt32(out int firstDayValue) && Enum.IsDefined(typeof(CalendarFirstDayOfWeekType), firstDayValue))
                    _firstDayOfWeek = (CalendarFirstDayOfWeekType)firstDayValue;
            }
            if (root.TryGetProperty(nameof(SelectionMode), out JsonElement selectionMode))
            {
                if (selectionMode.ValueKind == JsonValueKind.String && Enum.TryParse(selectionMode.GetString(), out CalendarSelectionModeType mode))
                    _selectionMode = mode;
                else if (selectionMode.ValueKind == JsonValueKind.Number && selectionMode.TryGetInt32(out int modeValue) && Enum.IsDefined(typeof(CalendarSelectionModeType), modeValue))
                    _selectionMode = (CalendarSelectionModeType)modeValue;
            }
            if (root.TryGetProperty(nameof(IsTodayHighlighted), out JsonElement isTodayHighlighted) && (isTodayHighlighted.ValueKind == JsonValueKind.True || isTodayHighlighted.ValueKind == JsonValueKind.False))
                _isTodayHighlighted = isTodayHighlighted.GetBoolean();
            if (root.TryGetProperty(nameof(DisplayMode), out JsonElement displayMode))
            {
                if (displayMode.ValueKind == JsonValueKind.String && Enum.TryParse(displayMode.GetString(), out CalendarDisplayModeType mode))
                    _displayMode = mode;
                else if (displayMode.ValueKind == JsonValueKind.Number && displayMode.TryGetInt32(out int modeValue) && Enum.IsDefined(typeof(CalendarDisplayModeType), modeValue))
                    _displayMode = (CalendarDisplayModeType)modeValue;
            }
            if (root.TryGetProperty(nameof(BlackoutDates), out JsonElement blackoutDates) && blackoutDates.ValueKind == JsonValueKind.String)
                _blackoutDates = blackoutDates.GetString() ?? "[]";
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            return JsonSerializer.Serialize(new
            {
                SelectedDate = _selectedDate,
                SelectedDates = _selectedDates,
                DisplayDate = _displayDate,
                DisplayDateStart = _displayDateStart,
                DisplayDateEnd = _displayDateEnd,
                FirstDayOfWeek = _firstDayOfWeek.ToString(),
                SelectionMode = _selectionMode.ToString(),
                IsTodayHighlighted = _isTodayHighlighted,
                DisplayMode = _displayMode.ToString(),
                BlackoutDates = _blackoutDates
            });
        }
    }
}
