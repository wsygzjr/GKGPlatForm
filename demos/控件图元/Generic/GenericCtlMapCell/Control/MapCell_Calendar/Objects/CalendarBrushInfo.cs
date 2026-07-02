using System;
using System.ComponentModel;
using System.Text.Json;
using Avalonia.Media;
using GF_Gereric;
using Griffins;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Calendar.Objects
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class CalendarBrushInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        public static readonly Guid Object_ID = new("{6eb76241-42b6-4860-9cda-0e7db8a52f15}");
        public static readonly CalendarBrushInfo Default = new();

        private string _backgroundColorStr = Colors.White.ToColorString();
        private string _borderColorStr = Colors.Gray.ToColorString();
        private string _foregroundColorStr = Colors.Black.ToColorString();

        [DisplayName("背景色")]
        [Category("画笔")]
        public Color BackgroundColor
        {
            get => Color.Parse(_backgroundColorStr);
            set => SetProperty(ref _backgroundColorStr, value.ToColorString(), nameof(BackgroundColor));
        }

        [DisplayName("边框颜色")]
        [Category("画笔")]
        public Color BorderColor
        {
            get => Color.Parse(_borderColorStr);
            set => SetProperty(ref _borderColorStr, value.ToColorString(), nameof(BorderColor));
        }

        [DisplayName("前景色")]
        [Category("画笔")]
        public Color ForegroundColor
        {
            get => Color.Parse(_foregroundColorStr);
            set => SetProperty(ref _foregroundColorStr, value.ToColorString(), nameof(ForegroundColor));
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
                throw new Exception("对象值不是 CalendarBrushInfo 转换的");

            ((IJsonValueConvert)this).FromJsonDataObject(objectValueJson.JsonVal);
        }

        void IJsonValueConvert.FromJsonDataObject(string jsonDataObject)
        {
            using JsonDocument jsonDocument = JsonDocument.Parse(jsonDataObject);
            JsonElement root = jsonDocument.RootElement;

            if (root.TryGetProperty(nameof(BackgroundColor), out JsonElement backgroundColor) && backgroundColor.ValueKind == JsonValueKind.String)
                _backgroundColorStr = backgroundColor.GetString() ?? _backgroundColorStr;
            if (root.TryGetProperty(nameof(BorderColor), out JsonElement borderColor) && borderColor.ValueKind == JsonValueKind.String)
                _borderColorStr = borderColor.GetString() ?? _borderColorStr;
            if (root.TryGetProperty(nameof(ForegroundColor), out JsonElement foregroundColor) && foregroundColor.ValueKind == JsonValueKind.String)
                _foregroundColorStr = foregroundColor.GetString() ?? _foregroundColorStr;
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            return JsonSerializer.Serialize(new
            {
                BackgroundColor = _backgroundColorStr,
                BorderColor = _borderColorStr,
                ForegroundColor = _foregroundColorStr
            });
        }
    }
}
