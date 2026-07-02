using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Avalonia.Media;
using PropertyModels.ComponentModel;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GKG.Map.MapCell.Generic.IconButton
{
    /// <summary>
    /// 图标按钮字体信息
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class IconButtonFontInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        /// <summary>
        /// 对象ID
        /// </summary>
        public static readonly Guid Object_ID = new Guid("{5A2B3C4D-5E6F-7A8B-9C0D-E1F2A3B4C5EA}");

        /// <summary>
        /// 默认值
        /// </summary>
        public static readonly IconButtonFontInfo Default = new IconButtonFontInfo();

        private string _fontColorStr = Colors.Black.ToColorString();
        private int _fontSize = 16;
        private FontWeight _fontWeight = FontWeight.Normal;
        private FontStyle _fontStyle = FontStyle.Normal;
        private bool _isUnderline = false;

        /// <summary>
        /// 字体颜色
        /// </summary>
        [DisplayName("字体颜色")]
        [Category("文本.字体")]
        public Color FontColor
        {
            get => Color.Parse(_fontColorStr);
            set => SetProperty(ref _fontColorStr, value.ToColorString(), nameof(FontColor));
        }

        /// <summary>
        /// 字体大小
        /// </summary>
        [DisplayName("字体大小")]
        [Category("文本.字体")]
        [Range(1, 200)]
        public int FontSize
        {
            get => _fontSize;
            set => SetProperty(ref _fontSize, value, nameof(FontSize));
        }

        /// <summary>
        /// 是否加粗
        /// </summary>
        [DisplayName("是否加粗")]
        [Category("文本.字体")]
        public bool IsBold
        {
            get => _fontWeight == FontWeight.Bold;
            set 
            {
                if (SetProperty(ref _fontWeight, value ? FontWeight.Bold : FontWeight.Normal, nameof(IsBold)))
                {
                    // 触发FontWeight属性的PropertyChanged事件，因为XAML中绑定了这个属性
                    RaisePropertyChanged(nameof(FontWeight));
                }
            }
        }

        /// <summary>
        /// 是否斜体
        /// </summary>
        [DisplayName("是否斜体")]
        [Category("文本.字体")]
        public bool IsItalic
        {
            get => _fontStyle == FontStyle.Italic;
            set 
            {
                if (SetProperty(ref _fontStyle, value ? FontStyle.Italic : FontStyle.Normal, nameof(IsItalic)))
                {
                    // 触发FontStyle属性的PropertyChanged事件，因为XAML中绑定了这个属性
                    RaisePropertyChanged(nameof(FontStyle));
                }
            }
        }

        /// <summary>
        /// 是否下划线
        /// </summary>
        [DisplayName("是否下划线")]
        [Category("文本.字体")]
        public bool IsUnderline
        {
            get => _isUnderline;
            set => SetProperty(ref _isUnderline, value, nameof(IsUnderline));
        }

        /// <summary>
        /// 字体粗细（内部使用）
        /// </summary>
        [Browsable(false)]
        public FontWeight FontWeight
        {
            get => _fontWeight;
            set => SetProperty(ref _fontWeight, value, nameof(FontWeight));
        }

        /// <summary>
        /// 字体样式（内部使用）
        /// </summary>
        [Browsable(false)]
        public FontStyle FontStyle
        {
            get => _fontStyle;
            set => SetProperty(ref _fontStyle, value, nameof(FontStyle));
        }

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
                    throw new Exception("对象值不是IconButtonFontInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是IconButtonFontInfo转换的");
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
            if (rootElement.TryGetProperty("FontColor", out value))
                _fontColorStr = value.GetString() ?? Colors.Black.ToColorString();
            if (rootElement.TryGetProperty("FontSize", out value))
                _fontSize = value.GetInt32();
            if (rootElement.TryGetProperty("IsBold", out value))
                IsBold = value.GetBoolean();
            if (rootElement.TryGetProperty("IsItalic", out value))
                IsItalic = value.GetBoolean();
            if (rootElement.TryGetProperty("IsUnderline", out value))
                IsUnderline = value.GetBoolean();
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                FontColor = _fontColorStr,
                FontSize = _fontSize,
                IsBold = IsBold,
                IsItalic = IsItalic,
                IsUnderline = IsUnderline
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        #endregion
    }
}