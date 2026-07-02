using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using GKG.Map.MapCell.Generic.Control.Lable;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.Stepper.Objects
{
    /// <summary>
    /// 步进器布局信息对象
    /// 包含对齐方式、外边距、尺寸约束
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class StepperLayoutInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        #region 静态字段

        public static readonly StepperLayoutInfo Default = new StepperLayoutInfo();
        public static readonly Guid Object_ID = new Guid("{C1B2C3D4-7777-4AC8-BF66-281412CDE207}");

        #endregion

        #region 私有字段

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

        #endregion

        #region 构造函数

        public StepperLayoutInfo()
            : this(HorizontalAlignType.Stretch, VerticalAlignType.Stretch, 0, 0, 0, 0, 0, 10000, 0, 10000)
        {
        }

        public StepperLayoutInfo(HorizontalAlignType horizontalAlign, VerticalAlignType verticalAlign,
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

        #endregion

        #region 属性 - 对齐

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

        #endregion

        #region 属性 - 外边距

        [DisplayName("上外边距")]
        [PropertySortOrder(3)]
        [FloatPrecision(0)]
        public double MarginTop
        {
            get => _marginTop;
            set => SetProperty(ref _marginTop, value, nameof(MarginTop));
        }

        [DisplayName("下外边距")]
        [PropertySortOrder(4)]
        [FloatPrecision(0)]
        public double MarginBottom
        {
            get => _marginBottom;
            set => SetProperty(ref _marginBottom, value, nameof(MarginBottom));
        }

        [DisplayName("左外边距")]
        [PropertySortOrder(5)]
        [FloatPrecision(0)]
        public double MarginLeft
        {
            get => _marginLeft;
            set => SetProperty(ref _marginLeft, value, nameof(MarginLeft));
        }

        [DisplayName("右外边距")]
        [PropertySortOrder(6)]
        [FloatPrecision(0)]
        public double MarginRight
        {
            get => _marginRight;
            set => SetProperty(ref _marginRight, value, nameof(MarginRight));
        }

        #endregion

        #region 属性 - 尺寸约束

        [DisplayName("最小宽度")]
        [PropertySortOrder(7)]
        [FloatPrecision(0)]
        [Range(0, 10000)]
        public double MinWidth
        {
            get => _minWidth;
            set
            {
                var newVal = value;
                if (double.IsNaN(newVal) || double.IsInfinity(newVal) || newVal < 0)
                    newVal = 0;

                if (!_isNormalizing)
                {
                    _isNormalizing = true;
                    SetProperty(ref _minWidth, newVal, nameof(MinWidth));
                    if (_maxWidth > 0 && _minWidth > _maxWidth)
                        SetProperty(ref _maxWidth, _minWidth, nameof(MaxWidth));
                    _isNormalizing = false;
                    return;
                }
                SetProperty(ref _minWidth, newVal, nameof(MinWidth));
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

                if (!_isNormalizing)
                {
                    _isNormalizing = true;
                    SetProperty(ref _maxWidth, newVal, nameof(MaxWidth));
                    if (_maxWidth > 0)
                    {
                        if (_minWidth > _maxWidth)
                            SetProperty(ref _minWidth, _maxWidth, nameof(MinWidth));
                    }
                    _isNormalizing = false;
                    return;
                }
                SetProperty(ref _maxWidth, newVal, nameof(MaxWidth));
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
                var newVal = value;
                if (double.IsNaN(newVal) || double.IsInfinity(newVal) || newVal < 0)
                    newVal = 0;

                if (!_isNormalizing)
                {
                    _isNormalizing = true;
                    SetProperty(ref _minHeight, newVal, nameof(MinHeight));
                    if (_maxHeight > 0 && _minHeight > _maxHeight)
                        SetProperty(ref _maxHeight, _minHeight, nameof(MaxHeight));
                    _isNormalizing = false;
                    return;
                }
                SetProperty(ref _minHeight, newVal, nameof(MinHeight));
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

                if (!_isNormalizing)
                {
                    _isNormalizing = true;
                    SetProperty(ref _maxHeight, newVal, nameof(MaxHeight));
                    if (_maxHeight > 0)
                    {
                        if (_minHeight > _maxHeight)
                            SetProperty(ref _minHeight, _maxHeight, nameof(MinHeight));
                    }
                    _isNormalizing = false;
                    return;
                }
                SetProperty(ref _maxHeight, newVal, nameof(MaxHeight));
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
                    throw new Exception("对象值不是StepperLayoutInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是StepperLayoutInfo转换的");
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

            string hAlignStr = rootElement.TryGetProperty("HorizontalAlign", out val) ? val.GetString() : "Stretch";
            HorizontalAlign = Enum.TryParse<HorizontalAlignType>(hAlignStr, out var hResult) ? hResult : HorizontalAlignType.Stretch;

            string vAlignStr = rootElement.TryGetProperty("VerticalAlign", out val) ? val.GetString() : "Stretch";
            VerticalAlign = Enum.TryParse<VerticalAlignType>(vAlignStr, out var vResult) ? vResult : VerticalAlignType.Stretch;

            MarginTop = rootElement.TryGetProperty("MarginTop", out val) ? val.GetDouble() : 0;
            MarginBottom = rootElement.TryGetProperty("MarginBottom", out val) ? val.GetDouble() : 0;
            MarginLeft = rootElement.TryGetProperty("MarginLeft", out val) ? val.GetDouble() : 0;
            MarginRight = rootElement.TryGetProperty("MarginRight", out val) ? val.GetDouble() : 0;

            MinWidth = rootElement.TryGetProperty("MinWidth", out val) ? val.GetDouble() : 0;
            MaxWidth = rootElement.TryGetProperty("MaxWidth", out val) ? val.GetDouble() : 0;
            MinHeight = rootElement.TryGetProperty("MinHeight", out val) ? val.GetDouble() : 0;
            MaxHeight = rootElement.TryGetProperty("MaxHeight", out val) ? val.GetDouble() : 0;
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            // 宽高主数据已迁到父类 Width/Height，这里不再由 LayoutInfo 自身承载尺寸。
            var obj = new
            {
                HorizontalAlign = HorizontalAlign.ToString(),
                VerticalAlign = VerticalAlign.ToString(),
                MarginTop,
                MarginBottom,
                MarginLeft,
                MarginRight,
                MinWidth,
                MaxWidth,
                MinHeight,
                MaxHeight
            };
            return System.Text.Json.JsonSerializer.Serialize(obj);
        }

        #endregion

        public override string ToString()
        {
            return $"{HorizontalAlign} / {VerticalAlign}";
        }
    }
}
