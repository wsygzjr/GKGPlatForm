using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PropertyModels.ComponentModel;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GKG.Map.MapCell.Generic.IconButton
{
    /// <summary>
    /// 图标按钮布局信息
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class IconButtonLayoutInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        /// <summary>
        /// 对象ID
        /// </summary>
        public static readonly Guid Object_ID = new Guid("{4A2B3C4D-5E6F-7A8B-9C0D-E1F2A3B4C5D9}");

        /// <summary>
        /// 默认值
        /// </summary>
        public static readonly IconButtonLayoutInfo Default = new IconButtonLayoutInfo();

        private HorizontalAlignmentEnum _horizontalAlignment = HorizontalAlignmentEnum.Stretch;
        private VerticalAlignmentEnum _verticalAlignment = VerticalAlignmentEnum.Stretch;
        private int _margin = 0;
        private int _marginLeft = 0;
        private int _marginTop = 0;
        private int _marginRight = 0;
        private int _marginBottom = 0;

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
        /// 左外边距
        /// </summary>
        [DisplayName("左")]
        [Category("布局")]
        public int MarginLeft
        {
            get => _marginLeft;
            set => SetProperty(ref _marginLeft, value);
        }

        /// <summary>
        /// 上外边距
        /// </summary>
        [DisplayName("上")]
        [Category("布局")]
        public int MarginTop
        {
            get => _marginTop;
            set => SetProperty(ref _marginTop, value);
        }

        /// <summary>
        /// 右外边距
        /// </summary>
        [DisplayName("右")]
        [Category("布局")]
        public int MarginRight
        {
            get => _marginRight;
            set => SetProperty(ref _marginRight, value);
        }

        /// <summary>
        /// 下外边距
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
                    throw new Exception("对象值不是IconButtonLayoutInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是IconButtonLayoutInfo转换的");
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
                _horizontalAlignment = (HorizontalAlignmentEnum)value.GetInt32();
            if (rootElement.TryGetProperty("VerticalAlignment", out value))
                _verticalAlignment = (VerticalAlignmentEnum)value.GetInt32();
            if (rootElement.TryGetProperty("Margin", out value))
                _margin = value.GetInt32();
            if (rootElement.TryGetProperty("MarginLeft", out value))
                _marginLeft = value.GetInt32();
            if (rootElement.TryGetProperty("MarginTop", out value))
                _marginTop = value.GetInt32();
            if (rootElement.TryGetProperty("MarginRight", out value))
                _marginRight = value.GetInt32();
            if (rootElement.TryGetProperty("MarginBottom", out value))
                _marginBottom = value.GetInt32();
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            // 宽高主数据已迁到父类 Width/Height，这里不再由 LayoutInfo 自身承载尺寸。
            var value = new
            {
                HorizontalAlignment = (int)_horizontalAlignment,
                VerticalAlignment = (int)_verticalAlignment,
                Margin = _margin,
                MarginLeft = _marginLeft,
                MarginTop = _marginTop,
                MarginRight = _marginRight,
                MarginBottom = _marginBottom
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        #endregion
    }
}
