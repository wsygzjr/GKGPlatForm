using System;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using GF_Gereric;
using Griffins;
using GKG.Map.MapCell.Generic.GroupPanel;
using GKG.Map.MapCell.Generic.Control.Lable;
using PropertyModels.ComponentModel;
using Avalonia.Media;

namespace GKG.Map.MapCell.Generic.Stepper.Objects
{
    /// <summary>
    /// 步进器文本信息对象
    /// 包含字体、字号、颜色、粗体、斜体
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class StepperTextInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        #region 静态字段

        public static readonly StepperTextInfo Default = new StepperTextInfo();
        public static readonly Guid Object_ID = new Guid("{C1B2C3D4-7777-4AC8-BF66-281412CDE209}");

        #endregion

        #region 私有字段

        private FontFamilyType _fontFamilyType;
        private string _fontColorStr;
        private double _fontSize;
        private bool _isItalic;
        private bool _isBold;

        #endregion

        #region 构造函数

        public StepperTextInfo()
            : this(FontFamilyType.MicrosoftYaHei, "#FF000000", 14, false, false)
        {
        }

        public StepperTextInfo(FontFamilyType fontFamilyType, string fontColorStr, double fontSize, bool isItalic, bool isBold)
        {
            FontFamilyType = fontFamilyType;
            FontColorStr = fontColorStr;
            FontSize = fontSize;
            IsItalic = isItalic;
            IsBold = isBold;
        }

        #endregion

        #region 属性

        [DisplayName("字体")]
        [PropertySortOrder(1)]
        public FontFamilyType FontFamilyType
        {
            get => _fontFamilyType;
            set => SetProperty(ref _fontFamilyType, value, nameof(FontFamilyType));
        }

        [Browsable(false)]
        public string FontColorStr
        {
            get => _fontColorStr;
            set
            {
                if (_fontColorStr != value)
                {
                    _fontColorStr = value;
                    RaisePropertyChanged(nameof(FontColorStr));
                    RaisePropertyChanged(nameof(FontColor));
                }
            }
        }

        [DisplayName("字号")]
        [PropertySortOrder(3)]
        [FloatPrecision(0)]
        public double FontSize
        {
            get => _fontSize;
            set => SetProperty(ref _fontSize, value, nameof(FontSize));
        }

        [DisplayName("斜体")]
        [PropertySortOrder(4)]
        public bool IsItalic
        {
            get => _isItalic;
            set => SetProperty(ref _isItalic, value, nameof(IsItalic));
        }

        [DisplayName("粗体")]
        [PropertySortOrder(5)]
        public bool IsBold
        {
            get => _isBold;
            set => SetProperty(ref _isBold, value, nameof(IsBold));
        }

        [DisplayName("字体颜色")]
        [PropertySortOrder(2)]
        [JsonIgnore]
        public Color FontColor
        {
            get => string.IsNullOrEmpty(_fontColorStr) ? Colors.Black : Color.Parse(_fontColorStr);
            set
            {
                var str = value.ToString();
                if (_fontColorStr != str)
                {
                    _fontColorStr = str;
                    RaisePropertyChanged(nameof(FontColorStr));
                    RaisePropertyChanged(nameof(FontColor));
                }
            }
        }

        #endregion

        #region IMPPropObjectValue 实现

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
                    throw new Exception("对象值不是StepperTextInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是StepperTextInfo转换的");
                ((IJsonValueConvert)this).FromJsonDataObject((baseValue.Val as ObjectValue_Json).JsonVal);
            }
        }

        #endregion

        #region IJsonValueConvert 实现

        void IJsonValueConvert.FromJsonDataObject(string jsonDataObject)
        {
            if (string.IsNullOrEmpty(jsonDataObject))
                throw new ArgumentNullException(nameof(jsonDataObject));

            using JsonDocument jsonDocument = JsonDocument.Parse(jsonDataObject);
            JsonElement rootElement = jsonDocument.RootElement;
            JsonElement val;

            string fontStr = rootElement.TryGetProperty("FontFamilyType", out val) ? val.GetString() : "MicrosoftYaHei";
            FontFamilyType = Enum.TryParse<FontFamilyType>(fontStr, out var fResult) ? fResult : FontFamilyType.MicrosoftYaHei;

            FontColorStr = rootElement.TryGetProperty("FontColorStr", out val) ? val.GetString() : "#FF000000";
            FontSize = rootElement.TryGetProperty("FontSize", out val) ? val.GetDouble() : 14;
            IsItalic = rootElement.TryGetProperty("IsItalic", out val) ? val.GetBoolean() : false;
            IsBold = rootElement.TryGetProperty("IsBold", out val) ? val.GetBoolean() : false;
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var obj = new
            {
                FontFamilyType = FontFamilyType.ToString(),
                FontColorStr,
                FontSize,
                IsItalic,
                IsBold
            };
            return System.Text.Json.JsonSerializer.Serialize(obj);
        }

        #endregion

        public override string ToString()
        {
            return "文本设置";
        }
    }
}
