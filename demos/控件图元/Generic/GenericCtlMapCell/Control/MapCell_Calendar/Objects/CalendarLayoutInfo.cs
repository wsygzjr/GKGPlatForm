using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Calendar.Objects
{
    [Serializable]
    public enum CalendarHorizontalAlignmentType
    {
        [Description("左对齐")] Left = 0,
        [Description("居中")] Center = 1,
        [Description("右对齐")] Right = 2,
        [Description("拉伸")] Stretch = 3
    }

    [Serializable]
    public enum CalendarVerticalAlignmentType
    {
        [Description("顶部")] Top = 0,
        [Description("居中")] Center = 1,
        [Description("底部")] Bottom = 2,
        [Description("拉伸")] Stretch = 3
    }

    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class CalendarLayoutInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        public static readonly Guid Object_ID = new("{57182caf-8573-43c5-bfae-14e7b5352483}");
        public static readonly CalendarLayoutInfo Default = new();

        private CalendarHorizontalAlignmentType _horizontalAlignment = CalendarHorizontalAlignmentType.Stretch;
        private CalendarVerticalAlignmentType _verticalAlignment = CalendarVerticalAlignmentType.Stretch;
        private int _marginLeft;
        private int _marginTop;
        private int _marginRight;
        private int _marginBottom;
        private int _minWidth;
        private int _maxWidth = 10000;
        private int _minHeight;
        private int _maxHeight = 10000;

        [DisplayName("水平对齐特征")]
        [Category("布局")]
        public CalendarHorizontalAlignmentType HorizontalAlignment
        {
            get => _horizontalAlignment;
            set => SetProperty(ref _horizontalAlignment, value);
        }

        [DisplayName("垂直对齐特征")]
        [Category("布局")]
        public CalendarVerticalAlignmentType VerticalAlignment
        {
            get => _verticalAlignment;
            set => SetProperty(ref _verticalAlignment, value);
        }

        [DisplayName("外边距")]
        [Category("布局")]
        public int Margin
        {
            get => _marginLeft;
            set
            {
                int normalized = Math.Max(0, value);
                bool changed = false;
                changed |= SetProperty(ref _marginLeft, normalized, nameof(MarginLeft));
                changed |= SetProperty(ref _marginTop, normalized, nameof(MarginTop));
                changed |= SetProperty(ref _marginRight, normalized, nameof(MarginRight));
                changed |= SetProperty(ref _marginBottom, normalized, nameof(MarginBottom));
                if (changed)
                    RaisePropertyChanged(nameof(Margin));
            }
        }

        [DisplayName("左外边距")]
        [Category("布局")]
        public int MarginLeft
        {
            get => _marginLeft;
            set => SetProperty(ref _marginLeft, Math.Max(0, value));
        }

        [DisplayName("上外边距")]
        [Category("布局")]
        public int MarginTop
        {
            get => _marginTop;
            set => SetProperty(ref _marginTop, Math.Max(0, value));
        }

        [DisplayName("右外边距")]
        [Category("布局")]
        public int MarginRight
        {
            get => _marginRight;
            set => SetProperty(ref _marginRight, Math.Max(0, value));
        }

        [DisplayName("下外边距")]
        [Category("布局")]
        public int MarginBottom
        {
            get => _marginBottom;
            set => SetProperty(ref _marginBottom, Math.Max(0, value));
        }

        [DisplayName("最小宽度约束")]
        [Category("布局")]
        [Range(0, 10000)]
        public int MinWidth
        {
            get => _minWidth;
            set
            {
                int normalized = Math.Max(0, value);
                SetProperty(ref _minWidth, normalized);
            }
        }

        [DisplayName("最大宽度约束")]
        [Category("布局")]
        [Range(0, 10000)]
        public int MaxWidth
        {
            get => _maxWidth;
            set
            {
                int normalized = value <= 0 ? int.MaxValue : Math.Max(0, value);
                SetProperty(ref _maxWidth, normalized);
            }
        }

        [DisplayName("最小高度约束")]
        [Category("布局")]
        [Range(0, 10000)]
        public int MinHeight
        {
            get => _minHeight;
            set
            {
                int normalized = Math.Max(0, value);
                SetProperty(ref _minHeight, normalized);
            }
        }

        [DisplayName("最大高度约束")]
        [Category("布局")]
        [Range(0, 10000)]
        public int MaxHeight
        {
            get => _maxHeight;
            set
            {
                int normalized = value <= 0 ? int.MaxValue : Math.Max(0, value);
                SetProperty(ref _maxHeight, normalized);
            }
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
                throw new Exception("对象值不是 CalendarLayoutInfo 转换的");

            ((IJsonValueConvert)this).FromJsonDataObject(objectValueJson.JsonVal);
        }

        void IJsonValueConvert.FromJsonDataObject(string jsonDataObject)
        {
            using JsonDocument jsonDocument = JsonDocument.Parse(jsonDataObject);
            JsonElement root = jsonDocument.RootElement;

            if (root.TryGetProperty(nameof(HorizontalAlignment), out JsonElement horizontalAlignment))
            {
                if (horizontalAlignment.ValueKind == JsonValueKind.String && Enum.TryParse(horizontalAlignment.GetString(), out CalendarHorizontalAlignmentType horizontal))
                    _horizontalAlignment = horizontal;
                else if (horizontalAlignment.ValueKind == JsonValueKind.Number && horizontalAlignment.TryGetInt32(out int horizontalValue) && Enum.IsDefined(typeof(CalendarHorizontalAlignmentType), horizontalValue))
                    _horizontalAlignment = (CalendarHorizontalAlignmentType)horizontalValue;
            }
            if (root.TryGetProperty(nameof(VerticalAlignment), out JsonElement verticalAlignment))
            {
                if (verticalAlignment.ValueKind == JsonValueKind.String && Enum.TryParse(verticalAlignment.GetString(), out CalendarVerticalAlignmentType vertical))
                    _verticalAlignment = vertical;
                else if (verticalAlignment.ValueKind == JsonValueKind.Number && verticalAlignment.TryGetInt32(out int verticalValue) && Enum.IsDefined(typeof(CalendarVerticalAlignmentType), verticalValue))
                    _verticalAlignment = (CalendarVerticalAlignmentType)verticalValue;
            }
            if (root.TryGetProperty(nameof(MarginLeft), out JsonElement marginLeft) && marginLeft.ValueKind == JsonValueKind.Number)
                _marginLeft = marginLeft.GetInt32();
            if (root.TryGetProperty(nameof(MarginTop), out JsonElement marginTop) && marginTop.ValueKind == JsonValueKind.Number)
                _marginTop = marginTop.GetInt32();
            if (root.TryGetProperty(nameof(MarginRight), out JsonElement marginRight) && marginRight.ValueKind == JsonValueKind.Number)
                _marginRight = marginRight.GetInt32();
            if (root.TryGetProperty(nameof(MarginBottom), out JsonElement marginBottom) && marginBottom.ValueKind == JsonValueKind.Number)
                _marginBottom = marginBottom.GetInt32();
            if (root.TryGetProperty(nameof(MinWidth), out JsonElement minWidth) && minWidth.ValueKind == JsonValueKind.Number)
                _minWidth = Math.Max(0, minWidth.GetInt32());
            if (root.TryGetProperty(nameof(MaxWidth), out JsonElement maxWidth) && maxWidth.ValueKind == JsonValueKind.Number)
                _maxWidth = maxWidth.GetInt32();
            if (root.TryGetProperty(nameof(MinHeight), out JsonElement minHeight) && minHeight.ValueKind == JsonValueKind.Number)
                _minHeight = Math.Max(0, minHeight.GetInt32());
            if (root.TryGetProperty(nameof(MaxHeight), out JsonElement maxHeight) && maxHeight.ValueKind == JsonValueKind.Number)
                _maxHeight = maxHeight.GetInt32();
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
                // 宽高主数据已迁到父类 Width/Height，这里不再由 LayoutInfo 自身承载尺寸。
            return JsonSerializer.Serialize(new
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
            });
        }
    }
}
