using System;
using System.ComponentModel;
using System.Text.Json;
using Avalonia.Media;
using GF_Gereric;
using Griffins;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Calendar.Objects
{
    public enum CalendarFontFamilyType
    {
        [Description("默认")] Default = 0,
        [Description("微软雅黑")] YaHei = 1,
        [Description("宋体")] SimSun = 2,
        [Description("黑体")] SimHei = 3,
        [Description("Segoe UI")] SegoeUi = 4,
        [Description("Arial")] Arial = 5,
        [Description("Consolas")] Consolas = 6,
        [Description("Times New Roman")] TimesNewRoman = 7
    }

    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class CalendarTextInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        public static readonly Guid Object_ID = new("{23c925e7-42ac-4a1f-9288-7b22d8864ead}");
        public static readonly CalendarTextInfo Default = new();

        private CalendarFontFamilyType _fontFamily = CalendarFontFamilyType.YaHei;
        private string _fontColorStr = Colors.Black.ToColorString();
        private int _fontSize = 14;
        private bool _isItalic;
        private bool _isBold;

        [DisplayName("字体簇")]
        [Category("文本")]
        public CalendarFontFamilyType FontFamily
        {
            get => _fontFamily;
            set => SetProperty(ref _fontFamily, value);
        }

        [DisplayName("字体颜色")]
        [Category("文本")]
        public Color FontColor
        {
            get => Color.Parse(_fontColorStr);
            set => SetProperty(ref _fontColorStr, value.ToColorString(), nameof(FontColor));
        }

        [DisplayName("字体大小")]
        [Category("文本")]
        public int FontSize
        {
            get => _fontSize;
            set => SetProperty(ref _fontSize, Math.Max(1, value));
        }

        [DisplayName("是否斜体")]
        [Category("文本")]
        public bool IsItalic
        {
            get => _isItalic;
            set => SetProperty(ref _isItalic, value);
        }

        [DisplayName("是否加粗")]
        [Category("文本")]
        public bool IsBold
        {
            get => _isBold;
            set => SetProperty(ref _isBold, value);
        }

        internal static string ToFontFamilyName(CalendarFontFamilyType value)
        {
            return value switch
            {
                CalendarFontFamilyType.YaHei => "Microsoft YaHei",
                CalendarFontFamilyType.SimSun => "SimSun",
                CalendarFontFamilyType.SimHei => "SimHei",
                CalendarFontFamilyType.SegoeUi => "Segoe UI",
                CalendarFontFamilyType.Arial => "Arial",
                CalendarFontFamilyType.Consolas => "Consolas",
                CalendarFontFamilyType.TimesNewRoman => "Times New Roman",
                _ => string.Empty
            };
        }

        private static CalendarFontFamilyType FromFontFamilyName(string? value)
        {
            return (value ?? string.Empty).Trim() switch
            {
                "Microsoft YaHei" => CalendarFontFamilyType.YaHei,
                "SimSun" => CalendarFontFamilyType.SimSun,
                "SimHei" => CalendarFontFamilyType.SimHei,
                "Segoe UI" => CalendarFontFamilyType.SegoeUi,
                "Arial" => CalendarFontFamilyType.Arial,
                "Consolas" => CalendarFontFamilyType.Consolas,
                "Times New Roman" => CalendarFontFamilyType.TimesNewRoman,
                _ => CalendarFontFamilyType.Default
            };
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
                throw new Exception("对象值不是 CalendarTextInfo 转换的");

            ((IJsonValueConvert)this).FromJsonDataObject(objectValueJson.JsonVal);
        }

        void IJsonValueConvert.FromJsonDataObject(string jsonDataObject)
        {
            using JsonDocument jsonDocument = JsonDocument.Parse(jsonDataObject);
            JsonElement root = jsonDocument.RootElement;

            if (root.TryGetProperty(nameof(FontFamily), out JsonElement fontFamily))
            {
                if (fontFamily.ValueKind == JsonValueKind.String)
                    _fontFamily = FromFontFamilyName(fontFamily.GetString());
                else if (fontFamily.ValueKind == JsonValueKind.Number && fontFamily.TryGetInt32(out int fontFamilyValue) && Enum.IsDefined(typeof(CalendarFontFamilyType), fontFamilyValue))
                    _fontFamily = (CalendarFontFamilyType)fontFamilyValue;
            }
            if (root.TryGetProperty(nameof(FontColor), out JsonElement fontColor) && fontColor.ValueKind == JsonValueKind.String)
                _fontColorStr = fontColor.GetString() ?? _fontColorStr;
            if (root.TryGetProperty(nameof(FontSize), out JsonElement fontSize) && fontSize.ValueKind == JsonValueKind.Number)
                _fontSize = Math.Max(1, fontSize.TryGetInt32(out int intValue) ? intValue : (int)Math.Round(fontSize.GetDouble()));
            if (root.TryGetProperty(nameof(IsItalic), out JsonElement isItalic) && (isItalic.ValueKind == JsonValueKind.True || isItalic.ValueKind == JsonValueKind.False))
                _isItalic = isItalic.GetBoolean();
            if (root.TryGetProperty(nameof(IsBold), out JsonElement isBold) && (isBold.ValueKind == JsonValueKind.True || isBold.ValueKind == JsonValueKind.False))
                _isBold = isBold.GetBoolean();
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            return JsonSerializer.Serialize(new
            {
                FontFamily = ToFontFamilyName(_fontFamily),
                FontColor = _fontColorStr,
                FontSize = _fontSize,
                IsItalic = _isItalic,
                IsBold = _isBold
            });
        }
    }
}
