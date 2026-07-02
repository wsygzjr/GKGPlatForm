using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Avalonia.Media;
using GF_Gereric;
using Griffins;
using Newtonsoft.JsonG;
using PropertyModels.ComponentModel;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GKG.Map.MapCell.Generic.Lable
{
    /// <summary>
    /// 标签字体信息对象
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class LableFontInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        #region 静态字段

        public static readonly LableFontInfo Default = new LableFontInfo(Colors.Black, 14, false, false, false);
        public static readonly Guid Object_ID = new Guid("{A1B2C3D4-4444-4AC8-BF66-281412CDE004}");

        #endregion

        #region 私有字段

        // 存储颜色字符串用于序列化
        private string _fontColorStr = "#FF000000";
        private double _fontSize = 14;
        private bool _isBold;
        private bool _isItalic;
        private bool _isUnderline;

        #endregion

        #region 构造函数

        public LableFontInfo()
            : this(Colors.Black, 14, false, false, false)
        {
        }

        public LableFontInfo(Color fontColor, double fontSize, bool isBold, bool isItalic, bool isUnderline)
        {
            _fontColorStr = fontColor.ToColorString();
            _fontSize = fontSize;
            _isBold = isBold;
            _isItalic = isItalic;
            _isUnderline = isUnderline;
        }

        #endregion

        #region 属性 - 用于 PropertyGrid 显示

        [DisplayName("字体颜色")]
        [JsonIgnore]
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

        [DisplayName("字体大小")]
        [FloatPrecision(0)]
        [Range(1, 200)]
        public double FontSize
        {
            get { return _fontSize; }
            set { SetProperty(ref _fontSize, value, nameof(FontSize)); }
        }

        [DisplayName("是否加粗")]
        public bool IsBold
        {
            get { return _isBold; }
            set { SetProperty(ref _isBold, value, nameof(IsBold)); }
        }

        [DisplayName("是否斜体")]
        public bool IsItalic
        {
            get { return _isItalic; }
            set { SetProperty(ref _isItalic, value, nameof(IsItalic)); }
        }

        [DisplayName("是否下划线")]
        public bool IsUnderline
        {
            get { return _isUnderline; }
            set { SetProperty(ref _isUnderline, value, nameof(IsUnderline)); }
        }

        #endregion

        #region 属性 - 用于 JSON 序列化

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
                    throw new Exception("对象值不是LableFontInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是LableFontInfo转换的");
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

            JsonElement value;
            FontColorStr = rootElement.TryGetProperty("FontColor", out value) ? value.GetString() : "#FF000000";
            FontSize = rootElement.TryGetProperty("FontSize", out value) ? value.GetDouble() : 14;
            IsBold = rootElement.TryGetProperty("IsBold", out value) ? value.GetBoolean() : false;
            IsItalic = rootElement.TryGetProperty("IsItalic", out value) ? value.GetBoolean() : false;
            IsUnderline = rootElement.TryGetProperty("IsUnderline", out value) ? value.GetBoolean() : false;
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                FontColor = FontColorStr,
                FontSize = FontSize,
                IsBold = IsBold,
                IsItalic = IsItalic,
                IsUnderline = IsUnderline
            };
            return JsonSerializer.Serialize(value);
        }

        #endregion

        public override string ToString()
        {
            string style = "";
            if (IsBold) style += "粗";
            if (IsItalic) style += "斜";
            if (IsUnderline) style += "下划线";
            if (string.IsNullOrEmpty(style)) style = "常规";
            return $"{FontSize}pt, {style}";
        }
    }
}
