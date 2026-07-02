using System;
using System.ComponentModel;
using PropertyModels.ComponentModel;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Slider
{
    /// <summary>
    /// 滑块布局信息
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class SliderLayoutInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        /// <summary>
        /// 对象ID
        /// </summary>
        public static readonly Guid Object_ID = new Guid("{4A2B3C4D-5E6F-7A8B-9C0D-E1F2A3B4C5D8}");

        /// <summary>
        /// 默认值
        /// </summary>
        public static readonly SliderLayoutInfo Default = new SliderLayoutInfo();

        private HorizontalAlignmentEnum _horizontalAlignment = HorizontalAlignmentEnum.Stretch;
        private VerticalAlignmentEnum _verticalAlignment = VerticalAlignmentEnum.Stretch;
        private int _margin = 0;
        private int _marginLeft = 0;
        private int _marginTop = 0;
        private int _marginRight = 0;
        private int _marginBottom = 0;
        private int _minWidth = 50;
        private int _maxWidth = int.MaxValue;
        private int _minHeight = 20;
        private int _maxHeight = int.MaxValue;

        /// <summary>
        /// 水平对齐特征
        /// </summary>
        [DisplayName("水平对齐特征")]
        [Category("布局")]
        public HorizontalAlignmentEnum HorizontalAlignment
        {
            get => _horizontalAlignment;
            set => SetProperty(ref _horizontalAlignment, value);
        }

        /// <summary>
        /// 垂直对齐特征
        /// </summary>
        [DisplayName("垂直对齐特征")]
        [Category("布局")]
        public VerticalAlignmentEnum VerticalAlignment
        {
            get => _verticalAlignment;
            set => SetProperty(ref _verticalAlignment, value);
        }

        /// <summary>
        /// 元素外边距
        /// </summary>
        [DisplayName("元素外边距")]
        [Category("布局")]
        public int Margin
        {
            get => _margin;
            set 
            {
                if (SetProperty(ref _margin, value))
                {
                    MarginLeft = value;
                    MarginTop = value;
                    MarginRight = value;
                    MarginBottom = value;
                }
            }
        }

        /// <summary>
        /// 左
        /// </summary>
        [DisplayName("左")]
        [Category("布局")]
        public int MarginLeft
        {
            get => _marginLeft;
            set => SetProperty(ref _marginLeft, value);
        }

        /// <summary>
        /// 上
        /// </summary>
        [DisplayName("上")]
        [Category("布局")]
        public int MarginTop
        {
            get => _marginTop;
            set => SetProperty(ref _marginTop, value);
        }

        /// <summary>
        /// 右
        /// </summary>
        [DisplayName("右")]
        [Category("布局")]
        public int MarginRight
        {
            get => _marginRight;
            set => SetProperty(ref _marginRight, value);
        }

        /// <summary>
        /// 下
        /// </summary>
        [DisplayName("下")]
        [Category("布局")]
        public int MarginBottom
        {
            get => _marginBottom;
            set => SetProperty(ref _marginBottom, value);
        }

        /// <summary>
        /// 水平对齐枚举
        /// </summary>
        public enum HorizontalAlignmentEnum
        {
            /// <summary>
            /// 左对齐
            /// </summary>
            Left,
            /// <summary>
            /// 居中
            /// </summary>
            Center,
            /// <summary>
            /// 右对齐
            /// </summary>
            Right,
            /// <summary>
            /// 拉伸
            /// </summary>
            Stretch
        }

        /// <summary>
        /// 垂直对齐枚举
        /// </summary>
        public enum VerticalAlignmentEnum
        {
            /// <summary>
            /// 顶部对齐
            /// </summary>
            Top,
            /// <summary>
            /// 居中
            /// </summary>
            Center,
            /// <summary>
            /// 底部对齐
            /// </summary>
            Bottom,
            /// <summary>
            /// 拉伸
            /// </summary>
            Stretch
        }

        /// <summary>
        /// 最小宽度约束
        /// </summary>
        [DisplayName("最小宽度约束")]
        [Category("布局")]
        public int MinWidth
        {
            get => _minWidth;
            set 
            {
                // 最小宽度约束需要大于等于0且不能大于最大宽度约束
                if (value < 0)
                    value = 0;
                if (value > _maxWidth)
                    value = _maxWidth;
                SetProperty(ref _minWidth, value);
            }
        }

        /// <summary>
        /// 最大宽度约束
        /// </summary>
        [DisplayName("最大宽度约束")]
        [Category("布局")]
        public int MaxWidth
        {
            get => _maxWidth;
            set 
            {
                // 最大宽度约束需要大于等于0且不能小于最小宽度约束
                if (value < 0)
                    value = 0;
                if (value < _minWidth)
                    value = _minWidth;
                SetProperty(ref _maxWidth, value);
            }
        }

        /// <summary>
        /// 最小高度约束
        /// </summary>
        [DisplayName("最小高度约束")]
        [Category("布局")]
        public int MinHeight
        {
            get => _minHeight;
            set 
            {
                // 最小高度约束需要大于等于0且不能大于最大高度约束
                if (value < 0)
                    value = 0;
                if (value > _maxHeight)
                    value = _maxHeight;
                SetProperty(ref _minHeight, value);
            }
        }

        /// <summary>
        /// 最大高度约束
        /// </summary>
        [DisplayName("最大高度约束")]
        [Category("布局")]
        public int MaxHeight
        {
            get => _maxHeight;
            set 
            {
                // 最大高度约束需要大于等于0且不能小于最小高度约束
                if (value < 0)
                    value = 0;
                if (value < _minHeight)
                    value = _minHeight;
                SetProperty(ref _maxHeight, value);
            }
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
                    throw new Exception("对象值不是SliderLayoutInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是SliderLayoutInfo转换的");
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
                string alignmentStr = value.GetString() ?? "Stretch";
                _horizontalAlignment = Enum.Parse<HorizontalAlignmentEnum>(alignmentStr, true);
            }
            if (rootElement.TryGetProperty("VerticalAlignment", out value))
            {
                string alignmentStr = value.GetString() ?? "Stretch";
                _verticalAlignment = Enum.Parse<VerticalAlignmentEnum>(alignmentStr, true);
            }
            if (rootElement.TryGetProperty("Margin", out value))
                _margin = (int)value.GetDouble();
            if (rootElement.TryGetProperty("MarginLeft", out value))
                _marginLeft = (int)value.GetDouble();
            if (rootElement.TryGetProperty("MarginTop", out value))
                _marginTop = (int)value.GetDouble();
            if (rootElement.TryGetProperty("MarginRight", out value))
                _marginRight = (int)value.GetDouble();
            if (rootElement.TryGetProperty("MarginBottom", out value))
                _marginBottom = (int)value.GetDouble();
            if (rootElement.TryGetProperty("MinWidth", out value))
                _minWidth = (int)value.GetDouble();
            if (rootElement.TryGetProperty("MaxWidth", out value))
                _maxWidth = (int)value.GetDouble();
            if (rootElement.TryGetProperty("MinHeight", out value))
                _minHeight = (int)value.GetDouble();
            if (rootElement.TryGetProperty("MaxHeight", out value))
                _maxHeight = (int)value.GetDouble();
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            // 宽高主数据已迁到父类 Width/Height，这里不再由 LayoutInfo 自身承载尺寸。
            var value = new
            {
                HorizontalAlignment = _horizontalAlignment.ToString(),
                VerticalAlignment = _verticalAlignment.ToString(),
                Margin = _margin,
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
