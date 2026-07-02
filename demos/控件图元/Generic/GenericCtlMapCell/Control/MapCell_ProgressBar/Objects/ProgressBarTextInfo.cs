using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Avalonia.Media;
using GF_Gereric;
using Griffins;
using GKG.Map.MapCell.Generic.GroupPanel;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.ProgressBar
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class ProgressBarTextInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        public static readonly ProgressBarTextInfo Default = new ProgressBarTextInfo(FontFamilyType.MicrosoftYaHei, Color.FromArgb(255, 0, 0, 0), 12, false, false);
        public static readonly Guid Object_ID = new Guid("{58F8A3E8-41D3-4B34-9F32-9C2B3F1C8A15}");

        private FontFamilyType _fontFamilyType;
        private string _fontColorStr = "#FF000000";
        private double _fontSize;
        private bool _isItalic;
        private bool _isBold;

        public ProgressBarTextInfo()
            : this(FontFamilyType.MicrosoftYaHei, Color.FromArgb(255, 0, 0, 0), 12, false, false)
        {
        }

        public ProgressBarTextInfo(FontFamilyType fontFamilyType, Color fontColor, double fontSize, bool isItalic, bool isBold)
        {
            FontFamilyType = fontFamilyType;
            _fontColorStr = fontColor.ToColorString();
            FontSize = fontSize;
            IsItalic = isItalic;
            IsBold = isBold;
        }

        [DisplayName("字体")]
        [PropertySortOrder(1)]
        public FontFamilyType FontFamilyType
        {
            get => _fontFamilyType;
            set
            {
                if (SetProperty(ref _fontFamilyType, value, nameof(FontFamilyType)))
                {
                    RaisePropertyChanged(nameof(FontFamily));
                }
            }
        }

        [Browsable(false)]
        public string FontFamily => GroupPanelTextInfo.GetFontFamilyName(_fontFamilyType);

        [DisplayName("字体颜色")]
        [PropertySortOrder(2)]
        public Color FontColor
        {
            get { return Color.Parse(_fontColorStr ?? "#FF000000"); }
            set
            {
                var newStr = value.ToColorString();
                if (_fontColorStr != newStr)
                {
                    _fontColorStr = newStr;
                    RaisePropertyChanged(nameof(FontColor));
                    RaisePropertyChanged(nameof(FontColorStr));
                }
            }
        }

        [Browsable(false)]
        public string FontColorStr
        {
            get { return _fontColorStr; }
            set
            {
                if (_fontColorStr != value)
                {
                    _fontColorStr = value ?? "#FF000000";
                    RaisePropertyChanged(nameof(FontColorStr));
                    RaisePropertyChanged(nameof(FontColor));
                }
            }
        }

        [DisplayName("字体大小")]
        [PropertySortOrder(3)]
        [Range(6, 72)]
        [FloatPrecision(0)]
        public double FontSize
        {
            get => _fontSize;
            set => SetProperty(ref _fontSize, value, nameof(FontSize));
        }

        [DisplayName("是否斜体")]
        [PropertySortOrder(4)]
        public bool IsItalic
        {
            get => _isItalic;
            set => SetProperty(ref _isItalic, value, nameof(IsItalic));
        }

        [DisplayName("是否加粗")]
        [PropertySortOrder(5)]
        public bool IsBold
        {
            get => _isBold;
            set => SetProperty(ref _isBold, value, nameof(IsBold));
        }

        bool IMPPropObjectValue.IsObject_Byte => false;

        Guid IMPPropObjectValue.GetObject_ID() => Object_ID;

        GriffinsBaseValue IMPPropObjectValue.ToBaseValue()
        {
            ObjectValue_Json objectValue_Json = new ObjectValue_Json(Object_ID);
            objectValue_Json.JsonVal = ((IJsonValueConvert)this).ToJsonDataObject();
            return GriffinsBaseValue.Create(objectValue_Json);
        }

        void IMPPropObjectValue.PopulateFromBaseValue(GriffinsBaseValue baseValue)
        {
            if (baseValue != null && baseValue.Val != null)
            {
                if (!(baseValue.Val is ObjectValue_Json))
                    throw new Exception("对象值不是ProgressBarTextInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是ProgressBarTextInfo转换的");
                ((IJsonValueConvert)this).FromJsonDataObject((baseValue.Val as ObjectValue_Json).JsonVal);
            }
        }

        void IJsonValueConvert.FromJsonDataObject(string jsonDataObject)
        {
            if (string.IsNullOrEmpty(jsonDataObject))
                throw new ArgumentNullException(nameof(jsonDataObject));

            using JsonDocument jsonDocument = JsonDocument.Parse(jsonDataObject);
            JsonElement rootElement = jsonDocument.RootElement;

            JsonElement value;
            string fontFamilyStr = rootElement.TryGetProperty("FontFamilyType", out value) ? value.GetString() : FontFamilyType.MicrosoftYaHei.ToString();
            FontFamilyType = Enum.TryParse<FontFamilyType>(fontFamilyStr, out var family) ? family : FontFamilyType.MicrosoftYaHei;
            FontColorStr = rootElement.TryGetProperty("FontColor", out value) ? value.GetString() : "#FF000000";
            FontSize = rootElement.TryGetProperty("FontSize", out value) ? value.GetDouble() : 12;
            IsItalic = rootElement.TryGetProperty("IsItalic", out value) ? value.GetBoolean() : false;
            IsBold = rootElement.TryGetProperty("IsBold", out value) ? value.GetBoolean() : false;
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                FontFamilyType = FontFamilyType.ToString(),
                FontColor = FontColorStr,
                FontSize,
                IsItalic,
                IsBold
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        public override string ToString() => $"{FontFamily}, {FontSize}";
    }
}
