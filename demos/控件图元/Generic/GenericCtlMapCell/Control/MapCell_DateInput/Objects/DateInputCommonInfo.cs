using System;
using System.ComponentModel;
using System.Text.Json;
using GF_Gereric;
using GKG.Map.MapCell.Generic.Control.MapCell_Calendar;
using Griffins;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.Control.MapCell_DateInput.Objects
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class DateInputCommonInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        public static readonly Guid Object_ID = new("{366E3B57-CAAF-4802-A245-92995A2D9101}");
        public static readonly DateInputCommonInfo Default = new();

        private string _selectedDate = string.Empty;

        [DisplayName("选中日期")]
        [Category("公共")]
        public string SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, NormalizeDate(value));
        }

        bool IMPPropObjectValue.IsObject_Byte => false;

        Guid IMPPropObjectValue.GetObject_ID() => Object_ID;

        GriffinsBaseValue IMPPropObjectValue.ToBaseValue()
        {
            return GriffinsBaseValue.Create(new ObjectValue_Json(Object_ID)
            {
                JsonVal = ((IJsonValueConvert)this).ToJsonDataObject()
            });
        }

        void IMPPropObjectValue.PopulateFromBaseValue(GriffinsBaseValue baseValue)
        {
            if (baseValue?.Val is not ObjectValue_Json objectValueJson || objectValueJson.Object_ID != Object_ID)
                throw new Exception("对象值不是 DateInputCommonInfo 转换的");

            ((IJsonValueConvert)this).FromJsonDataObject(objectValueJson.JsonVal);
        }

        void IJsonValueConvert.FromJsonDataObject(string jsonDataObject)
        {
            using JsonDocument jsonDocument = JsonDocument.Parse(jsonDataObject);
            JsonElement root = jsonDocument.RootElement;

            if (root.TryGetProperty(nameof(SelectedDate), out JsonElement selectedDate) && selectedDate.ValueKind == JsonValueKind.String)
                _selectedDate = NormalizeDate(selectedDate.GetString());
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            return JsonSerializer.Serialize(new
            {
                SelectedDate = _selectedDate
            });
        }

        private static string NormalizeDate(string value)
        {
            return CalendarValueHelpers.TryParseDate(value, out DateTime date)
                ? CalendarValueHelpers.FormatDate(date)
                : string.Empty;
        }
    }
}
