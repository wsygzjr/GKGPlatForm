using System;
using System.ComponentModel;
using PropertyModels.ComponentModel;
using System.Text.Json;
using GF_Gereric;
using Griffins;

namespace GKG.Map.MapCell.Generic.Control.MapCell_TextBox
{
    /// <summary>
    /// 文本输入框布局信息
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class TextBoxLayoutInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        public static readonly Guid Object_ID = new Guid("{C7A47B7C-9C4A-46A1-A45F-68E0712B6A12}");
        public static readonly TextBoxLayoutInfo Default = new TextBoxLayoutInfo();

        private HorizontalAlignmentEnum _horizontalAlignment = HorizontalAlignmentEnum.Stretch;
        private VerticalAlignmentEnum _verticalAlignment = VerticalAlignmentEnum.Stretch;

        private int _marginLeft;
        private int _marginTop;
        private int _marginRight;
        private int _marginBottom;

        private int _minWidth = 0;
        private int _maxWidth = int.MaxValue;
        private int _minHeight = 0;
        private int _maxHeight = int.MaxValue;

        [DisplayName("水平对齐特征")]
        [Category("布局")]
        public HorizontalAlignmentEnum HorizontalAlignment
        {
            get => _horizontalAlignment;
            set => SetProperty(ref _horizontalAlignment, value);
        }

        [DisplayName("垂直对齐特征")]
        [Category("布局")]
        public VerticalAlignmentEnum VerticalAlignment
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
                value = Math.Max(0, value);

                // “统一外边距”修改时同步到四边，并确保四边属性都能收到变更通知
                var changed = false;
                changed |= SetProperty(ref _marginLeft, value, nameof(MarginLeft));
                changed |= SetProperty(ref _marginTop, value, nameof(MarginTop));
                changed |= SetProperty(ref _marginRight, value, nameof(MarginRight));
                changed |= SetProperty(ref _marginBottom, value, nameof(MarginBottom));

                if (changed)
                    RaisePropertyChanged(nameof(Margin));
            }
        }

        [DisplayName("左")]
        [Category("布局")]
        public int MarginLeft
        {
            get => _marginLeft;
            set => SetProperty(ref _marginLeft, value);
        }

        [DisplayName("上")]
        [Category("布局")]
        public int MarginTop
        {
            get => _marginTop;
            set => SetProperty(ref _marginTop, value);
        }

        [DisplayName("右")]
        [Category("布局")]
        public int MarginRight
        {
            get => _marginRight;
            set => SetProperty(ref _marginRight, value);
        }

        [DisplayName("下")]
        [Category("布局")]
        public int MarginBottom
        {
            get => _marginBottom;
            set => SetProperty(ref _marginBottom, value);
        }

        [DisplayName("最小宽度约束")]
        [Category("布局")]
        public int MinWidth
        {
            get => _minWidth;
            set
            {
                if (value < 0) value = 0;
                SetProperty(ref _minWidth, value);
            }
        }

        [DisplayName("最大宽度约束")]
        [Category("布局")]
        public int MaxWidth
        {
            get => _maxWidth;
            set
            {
                if (value <= 0) value = int.MaxValue;
                SetProperty(ref _maxWidth, value);
            }
        }

        [DisplayName("最小高度约束")]
        [Category("布局")]
        public int MinHeight
        {
            get => _minHeight;
            set
            {
                if (value < 0) value = 0;
                SetProperty(ref _minHeight, value);
            }
        }

        [DisplayName("最大高度约束")]
        [Category("布局")]
        public int MaxHeight
        {
            get => _maxHeight;
            set
            {
                if (value <= 0) value = int.MaxValue;
                SetProperty(ref _maxHeight, value);
            }
        }

        // 对齐枚举用于属性面板显示，View 侧会映射到 Avalonia 对齐枚举
        public enum HorizontalAlignmentEnum { Left, Center, Right, Stretch }
        public enum VerticalAlignmentEnum { Top, Center, Bottom, Stretch }

        bool IMPPropObjectValue.IsObject_Byte => false;
        Guid IMPPropObjectValue.GetObject_ID() => Object_ID;

        GriffinsBaseValue IMPPropObjectValue.ToBaseValue()
        {
            // 将对象序列化为 GriffinsBaseValue（ObjectValue_Json）供属性系统存储
            ObjectValue_Json objectValue_Json = new ObjectValue_Json(Object_ID);
            objectValue_Json.JsonVal = ((IJsonValueConvert)this).ToJsonDataObject();
            return GriffinsBaseValue.Create(objectValue_Json);
        }

        void IMPPropObjectValue.PopulateFromBaseValue(GriffinsBaseValue baseValue)
        {
            if (baseValue != null && baseValue.Val != null)
            {
                if (baseValue.Val is not ObjectValue_Json ov)
                    throw new Exception("对象值不是TextBoxLayoutInfo转换的");
                if (ov.Object_ID != Object_ID)
                    throw new Exception("对象值不是TextBoxLayoutInfo转换的");
                // 由 Json 反填充内部字段
                ((IJsonValueConvert)this).FromJsonDataObject(ov.JsonVal);
            }
        }

        void IJsonValueConvert.FromJsonDataObject(string jsonDataObject)
        {
            if (string.IsNullOrEmpty(jsonDataObject))
                throw new ArgumentNullException(nameof(jsonDataObject));

            using JsonDocument jsonDocument = JsonDocument.Parse(jsonDataObject);
            JsonElement root = jsonDocument.RootElement;

            if (root.TryGetProperty("HorizontalAlignment", out var ha) && ha.ValueKind == JsonValueKind.String && Enum.TryParse<HorizontalAlignmentEnum>(ha.GetString(), out var hae))
                _horizontalAlignment = hae;
            if (root.TryGetProperty("VerticalAlignment", out var va) && va.ValueKind == JsonValueKind.String && Enum.TryParse<VerticalAlignmentEnum>(va.GetString(), out var vae))
                _verticalAlignment = vae;

            if (root.TryGetProperty("MarginLeft", out var ml) && ml.ValueKind == JsonValueKind.Number)
                _marginLeft = ml.GetInt32();
            if (root.TryGetProperty("MarginTop", out var mt) && mt.ValueKind == JsonValueKind.Number)
                _marginTop = mt.GetInt32();
            if (root.TryGetProperty("MarginRight", out var mr) && mr.ValueKind == JsonValueKind.Number)
                _marginRight = mr.GetInt32();
            if (root.TryGetProperty("MarginBottom", out var mb) && mb.ValueKind == JsonValueKind.Number)
                _marginBottom = mb.GetInt32();

            if (root.TryGetProperty("MinWidth", out var minw) && minw.ValueKind == JsonValueKind.Number)
                _minWidth = minw.GetInt32();
            if (root.TryGetProperty("MaxWidth", out var maxw) && maxw.ValueKind == JsonValueKind.Number)
                _maxWidth = maxw.GetInt32();
            if (root.TryGetProperty("MinHeight", out var minh) && minh.ValueKind == JsonValueKind.Number)
                _minHeight = minh.GetInt32();
            if (root.TryGetProperty("MaxHeight", out var maxh) && maxh.ValueKind == JsonValueKind.Number)
                _maxHeight = maxh.GetInt32();
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
    }
}
