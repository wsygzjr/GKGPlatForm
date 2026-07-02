using System;
using System.ComponentModel;
using Avalonia.Media;
using PropertyModels.ComponentModel;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Slider
{
    /// <summary>
    /// 滑块文本信息
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class SliderTextInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        /// <summary>
        /// 对象ID
        /// </summary>
        public static readonly Guid Object_ID = new("{5A2B3C4D-5E6F-7A8B-9C0D-E1F2A3B4C5D8}");

        /// <summary>
        /// 默认值
        /// </summary>
        public static readonly SliderTextInfo Default = new();

        private FontFamily _fontFamily = new("Arial");
        private string _fontColorStr = Colors.Black.ToColorString();
        private int _fontSize = 12;
        private bool _isItalic = false;
        private bool _isBold = false;

        /// <summary>
        /// 字体族
        /// </summary>
        [DisplayName("字体族")]
        [Category("文本")]
        public FontFamily FontFamily
        {
            get => _fontFamily;
            set => SetProperty(ref _fontFamily, value);
        }

        /// <summary>
        /// 字体颜色
        /// </summary>
        [DisplayName("字体颜色")]
        [Category("文本")]
        public Color FontColor
        {
            get => Color.Parse(_fontColorStr);
            set => SetProperty(ref _fontColorStr, value.ToColorString(), nameof(FontColor));
        }

        /// <summary>
        /// 字体大小
        /// </summary>
        [DisplayName("字体大小")]
        [Category("文本")]
        public int FontSize
        {
            get => _fontSize;
            set => SetProperty(ref _fontSize, value);
        }

        /// <summary>
        /// 是否斜体
        /// </summary>
        [DisplayName("是否斜体")]
        [Category("文本")]
        public bool IsItalic
        {
            get => _isItalic;
            set => SetProperty(ref _isItalic, value);
        }

        /// <summary>
        /// 是否加粗
        /// </summary>
        [DisplayName("是否加粗")]
        [Category("文本")]
        public bool IsBold
        {
            get => _isBold;
            set => SetProperty(ref _isBold, value);
        }

        #region IMPPropObjectValue 实现

        bool IMPPropObjectValue.IsObject_Byte => false;

        Guid IMPPropObjectValue.GetObject_ID() => Object_ID;

        GriffinsBaseValue IMPPropObjectValue.ToBaseValue()
        {
            ObjectValue_Json objectValue_Json = new(Object_ID)
            {
                JsonVal = ((IJsonValueConvert)this).ToJsonDataObject()
            };
            return GriffinsBaseValue.Create(objectValue_Json);
        }

        void IMPPropObjectValue.PopulateFromBaseValue(GriffinsBaseValue baseValue)
        {
            if (baseValue != null && baseValue.Val != null)
            {
                if (baseValue.Val is not ObjectValue_Json)
                    throw new Exception("对象值不是SliderTextInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是SliderTextInfo转换的");
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

            if (rootElement.TryGetProperty(nameof(FontFamily), out JsonElement value))
                _fontFamily = new FontFamily(value.GetString() ?? "Arial");
            if (rootElement.TryGetProperty(nameof(FontColor), out value))
                _fontColorStr = value.GetString() ?? Colors.Black.ToColorString();
            if (rootElement.TryGetProperty(nameof(FontSize), out value))
                _fontSize = (int)value.GetDouble();
            if (rootElement.TryGetProperty(nameof(IsItalic), out value))
                _isItalic = value.GetBoolean();
            if (rootElement.TryGetProperty(nameof(IsBold), out value))
                _isBold = value.GetBoolean();
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                FontFamily = _fontFamily?.Name ?? "Arial",
                FontColor = _fontColorStr,
                FontSize = _fontSize,
                IsItalic = _isItalic,
                IsBold = _isBold
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        #endregion
    }
}