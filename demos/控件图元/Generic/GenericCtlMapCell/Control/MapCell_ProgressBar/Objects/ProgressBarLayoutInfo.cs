using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using GKG.Map.MapCell.Generic.Control.Lable;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.ProgressBar
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class ProgressBarLayoutInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        public static readonly ProgressBarLayoutInfo Default = new ProgressBarLayoutInfo();
        public static readonly Guid Object_ID = new Guid("{58F8A3E8-41D3-4B34-9F32-9C2B3F1C8A13}");

        private HorizontalAlignType _horizontalAlign;
        private VerticalAlignType _verticalAlign;
        private double _marginTop;
        private double _marginLeft;
        private double _marginBottom;
        private double _marginRight;
        private double _minWidth;
        private double _maxWidth;
        private double _minHeight;
        private double _maxHeight;

        private bool _isNormalizing;

        public ProgressBarLayoutInfo()
            : this(HorizontalAlignType.Stretch, VerticalAlignType.Stretch, 0, 0, 0, 0, 0, 0, 0, 0)
        {
        }

        public ProgressBarLayoutInfo(HorizontalAlignType horizontalAlign, VerticalAlignType verticalAlign,
            double marginTop, double marginLeft, double marginBottom, double marginRight,
            double minWidth, double maxWidth, double minHeight, double maxHeight)
        {
            HorizontalAlign = horizontalAlign;
            VerticalAlign = verticalAlign;
            MarginTop = marginTop;
            MarginLeft = marginLeft;
            MarginBottom = marginBottom;
            MarginRight = marginRight;
            MinWidth = minWidth;
            MaxWidth = maxWidth;
            MinHeight = minHeight;
            MaxHeight = maxHeight;
        }

        [DisplayName("水平对齐")]
        [PropertySortOrder(1)]
        public HorizontalAlignType HorizontalAlign
        {
            get => _horizontalAlign;
            set => SetProperty(ref _horizontalAlign, value, nameof(HorizontalAlign));
        }

        [DisplayName("垂直对齐")]
        [PropertySortOrder(2)]
        public VerticalAlignType VerticalAlign
        {
            get => _verticalAlign;
            set => SetProperty(ref _verticalAlign, value, nameof(VerticalAlign));
        }

        [DisplayName("上边距")]
        [PropertySortOrder(3)]
        [FloatPrecision(0)]
        public double MarginTop { get => _marginTop; set => SetProperty(ref _marginTop, value, nameof(MarginTop)); }

        [DisplayName("左边距")]
        [PropertySortOrder(4)]
        [FloatPrecision(0)]
        public double MarginLeft { get => _marginLeft; set => SetProperty(ref _marginLeft, value, nameof(MarginLeft)); }

        [DisplayName("下边距")]
        [PropertySortOrder(5)]
        [FloatPrecision(0)]
        public double MarginBottom { get => _marginBottom; set => SetProperty(ref _marginBottom, value, nameof(MarginBottom)); }

        [DisplayName("右边距")]
        [PropertySortOrder(6)]
        [FloatPrecision(0)]
        public double MarginRight { get => _marginRight; set => SetProperty(ref _marginRight, value, nameof(MarginRight)); }

        [DisplayName("最小宽度")]
        [PropertySortOrder(7)]
        [FloatPrecision(0)]
        [Range(0, 10000)]
        public double MinWidth
        {
            get => _minWidth;
            set
            {
                var newVal = Math.Max(0, value);
                if (SetProperty(ref _minWidth, newVal, nameof(MinWidth)))
                {
                    if (!_isNormalizing)
                    {
                        _isNormalizing = true;
                        if (_maxWidth > 0 && _minWidth > _maxWidth) MaxWidth = _minWidth;
                        _isNormalizing = false;
                    }
                }
            }
        }

        [DisplayName("最大宽度")]
        [PropertySortOrder(8)]
        [FloatPrecision(0)]
        [Range(0, 10000)]
        public double MaxWidth
        {
            get => _maxWidth;
            set
            {
                var newVal = value;
                if (double.IsNaN(newVal) || double.IsInfinity(newVal) || newVal <= 0)
                    newVal = 0;

                if (SetProperty(ref _maxWidth, newVal, nameof(MaxWidth)))
                {
                    if (!_isNormalizing)
                    {
                        _isNormalizing = true;
                        if (_maxWidth > 0 && _minWidth > _maxWidth) MinWidth = _maxWidth;
                        _isNormalizing = false;
                    }
                }
            }
        }

        [DisplayName("最小高度")]
        [PropertySortOrder(9)]
        [FloatPrecision(0)]
        [Range(0, 10000)]
        public double MinHeight
        {
            get => _minHeight;
            set
            {
                var newVal = Math.Max(0, value);
                if (SetProperty(ref _minHeight, newVal, nameof(MinHeight)))
                {
                    if (!_isNormalizing)
                    {
                        _isNormalizing = true;
                        if (_maxHeight > 0 && _minHeight > _maxHeight) MaxHeight = _minHeight;
                        _isNormalizing = false;
                    }
                }
            }
        }

        [DisplayName("最大高度")]
        [PropertySortOrder(10)]
        [FloatPrecision(0)]
        [Range(0, 10000)]
        public double MaxHeight
        {
            get => _maxHeight;
            set
            {
                var newVal = value;
                if (double.IsNaN(newVal) || double.IsInfinity(newVal) || newVal <= 0)
                    newVal = 0;

                if (SetProperty(ref _maxHeight, newVal, nameof(MaxHeight)))
                {
                    if (!_isNormalizing)
                    {
                        _isNormalizing = true;
                        if (_maxHeight > 0 && _minHeight > _maxHeight) MinHeight = _maxHeight;
                        _isNormalizing = false;
                    }
                }
            }
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
                    throw new Exception("对象值不是ProgressBarLayoutInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是ProgressBarLayoutInfo转换的");
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
            string haStr = rootElement.TryGetProperty("HorizontalAlign", out value) ? value.GetString() : "Stretch";
            HorizontalAlign = Enum.TryParse<HorizontalAlignType>(haStr, out var ha) ? ha : HorizontalAlignType.Stretch;

            string vaStr = rootElement.TryGetProperty("VerticalAlign", out value) ? value.GetString() : "Stretch";
            VerticalAlign = Enum.TryParse<VerticalAlignType>(vaStr, out var va) ? va : VerticalAlignType.Stretch;

            MarginTop = rootElement.TryGetProperty("MarginTop", out value) ? value.GetDouble() : 0;
            MarginLeft = rootElement.TryGetProperty("MarginLeft", out value) ? value.GetDouble() : 0;
            MarginBottom = rootElement.TryGetProperty("MarginBottom", out value) ? value.GetDouble() : 0;
            MarginRight = rootElement.TryGetProperty("MarginRight", out value) ? value.GetDouble() : 0;

            MinWidth = rootElement.TryGetProperty("MinWidth", out value) ? value.GetDouble() : 0;
            MaxWidth = rootElement.TryGetProperty("MaxWidth", out value) ? value.GetDouble() : 0;
            MinHeight = rootElement.TryGetProperty("MinHeight", out value) ? value.GetDouble() : 0;
            MaxHeight = rootElement.TryGetProperty("MaxHeight", out value) ? value.GetDouble() : 0;
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            // 宽高主数据已迁到父类 Width/Height，这里不再由 LayoutInfo 自身承载尺寸。
            var value = new
            {
                HorizontalAlign = HorizontalAlign.ToString(),
                VerticalAlign = VerticalAlign.ToString(),
                MarginTop,
                MarginLeft,
                MarginBottom,
                MarginRight,
                MinWidth,
                MaxWidth,
                MinHeight,
                MaxHeight
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        public override string ToString() => $"{HorizontalAlign} / {VerticalAlign}";
    }
}
