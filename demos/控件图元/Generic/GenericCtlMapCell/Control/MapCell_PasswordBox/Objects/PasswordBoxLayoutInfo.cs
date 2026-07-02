using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using GKG.Map.MapCell.Generic.Control.Lable;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.PasswordBox
{
    /// <summary>
    /// 密码输入框布局信息
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class PasswordBoxLayoutInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        public static readonly Guid Object_ID = new Guid("{B1C2D3E4-3001-4AC8-BF66-281412CDE103}");
        public static readonly PasswordBoxLayoutInfo Default = new PasswordBoxLayoutInfo();

        private HorizontalAlignType _horizontalAlignment = HorizontalAlignType.Stretch;
        private VerticalAlignType _verticalAlignment = VerticalAlignType.Stretch;
        private double _marginLeft = 0;
        private double _marginTop = 0;
        private double _marginRight = 0;
        private double _marginBottom = 0;
        private double _minWidth = 0;
        private double _maxWidth = 10000;
        private double _minHeight = 0;
        private double _maxHeight = 10000;

        [DisplayName("水平对齐")]
        public HorizontalAlignType HorizontalAlignment
        {
            get => _horizontalAlignment;
            set => SetProperty(ref _horizontalAlignment, value, nameof(HorizontalAlignment));
        }

        [DisplayName("垂直对齐")]
        public VerticalAlignType VerticalAlignment
        {
            get => _verticalAlignment;
            set => SetProperty(ref _verticalAlignment, value, nameof(VerticalAlignment));
        }

        [DisplayName("外边距-左")]
        [FloatPrecision(0)]
        public double MarginLeft
        {
            get => _marginLeft;
            set => SetProperty(ref _marginLeft, value, nameof(MarginLeft));
        }

        [DisplayName("外边距-上")]
        [FloatPrecision(0)]
        public double MarginTop
        {
            get => _marginTop;
            set => SetProperty(ref _marginTop, value, nameof(MarginTop));
        }

        [DisplayName("外边距-右")]
        [FloatPrecision(0)]
        public double MarginRight
        {
            get => _marginRight;
            set => SetProperty(ref _marginRight, value, nameof(MarginRight));
        }

        [DisplayName("外边距-下")]
        [FloatPrecision(0)]
        public double MarginBottom
        {
            get => _marginBottom;
            set => SetProperty(ref _marginBottom, value, nameof(MarginBottom));
        }

        [DisplayName("最小宽度")]
        [FloatPrecision(0)]
        [Range(0, 10000)]
        public double MinWidth
        {
            get => _minWidth;
            set => SetProperty(ref _minWidth, value, nameof(MinWidth));
        }

        [DisplayName("最大宽度")]
        [FloatPrecision(0)]
        public double MaxWidth
        {
            get => _maxWidth;
            set => SetProperty(ref _maxWidth, value, nameof(MaxWidth));
        }

        [DisplayName("最小高度")]
        [FloatPrecision(0)]
        [Range(0, 10000)]
        public double MinHeight
        {
            get => _minHeight;
            set => SetProperty(ref _minHeight, value, nameof(MinHeight));
        }

        [DisplayName("最大高度")]
        [FloatPrecision(0)]
        public double MaxHeight
        {
            get => _maxHeight;
            set => SetProperty(ref _maxHeight, value, nameof(MaxHeight));
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
                    throw new Exception("对象值不是PasswordBoxLayoutInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是PasswordBoxLayoutInfo转换的");
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
            if (rootElement.TryGetProperty("HorizontalAlignment", out value))
            {
                var str = value.GetString();
                if (Enum.TryParse<HorizontalAlignType>(str, out var h))
                    HorizontalAlignment = h;
            }
            if (rootElement.TryGetProperty("VerticalAlignment", out value))
            {
                var str = value.GetString();
                if (Enum.TryParse<VerticalAlignType>(str, out var v))
                    VerticalAlignment = v;
            }
            if (rootElement.TryGetProperty("MarginLeft", out value))
                MarginLeft = value.GetDouble();
            if (rootElement.TryGetProperty("MarginTop", out value))
                MarginTop = value.GetDouble();
            if (rootElement.TryGetProperty("MarginRight", out value))
                MarginRight = value.GetDouble();
            if (rootElement.TryGetProperty("MarginBottom", out value))
                MarginBottom = value.GetDouble();
            if (rootElement.TryGetProperty("MinWidth", out value))
                MinWidth = value.GetDouble();
            if (rootElement.TryGetProperty("MaxWidth", out value))
                MaxWidth = value.GetDouble();
            if (rootElement.TryGetProperty("MinHeight", out value))
                MinHeight = value.GetDouble();
            if (rootElement.TryGetProperty("MaxHeight", out value))
                MaxHeight = value.GetDouble();
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            // 宽高主数据已迁到父类 Width/Height，这里不再由 LayoutInfo 自身承载尺寸。
            var value = new
            {
                HorizontalAlignment = _horizontalAlignment.ToString(),
                VerticalAlignment = _verticalAlignment.ToString(),
                MarginLeft = _marginLeft,
                MarginTop = _marginTop,
                MarginRight = _marginRight,
                MarginBottom = _marginBottom,
                MinWidth = _minWidth,
                MaxWidth = _maxWidth,
                MinHeight = _minHeight,
                MaxHeight = _maxHeight
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        #endregion
    }
}
