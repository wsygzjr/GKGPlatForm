using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.Lable
{
    public enum TextVerticalAlignType
    {
        [Description("顶部")] Top = 0,
        [Description("居中")] Center = 1,
        [Description("底部")] Bottom = 2,
        [Description("拉伸")] Stretch = 3
    }

    /// <summary>
    /// 标签段落信息对象
    /// 包含行高、段落间距、文本对齐设置
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class LableParagraphInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        #region 静态字段

        /// <summary>
        /// 默认段落信息
        /// </summary>
        public static readonly LableParagraphInfo Default = new LableParagraphInfo(1.0, 0, 0, TextAlignType.Left, TextVerticalAlignType.Center);

        /// <summary>
        /// 对象唯一标识符
        /// </summary>
        public static readonly Guid Object_ID = new Guid("{A1B2C3D4-5555-4AC8-BF66-281412CDE005}");

        #endregion

        #region 私有字段

        private double _lineHeight;
        private double _paragraphSpacingBefore;
        private double _paragraphSpacingAfter;
        private TextAlignType _textAlignment;
        private TextVerticalAlignType _verticalTextAlignment;

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public LableParagraphInfo()
            : this(1.0, 0, 0, TextAlignType.Left, TextVerticalAlignType.Center)
        {
        }

        /// <summary>
        /// 带参数构造函数
        /// </summary>
        public LableParagraphInfo(double lineHeight, double paragraphSpacingBefore, 
            double paragraphSpacingAfter, TextAlignType textAlignment, TextVerticalAlignType verticalTextAlignment)
        {
            LineHeight = lineHeight;
            ParagraphSpacingBefore = paragraphSpacingBefore;
            ParagraphSpacingAfter = paragraphSpacingAfter;
            TextAlignment = textAlignment;
            VerticalTextAlignment = verticalTextAlignment;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 行高 (倍数)
        /// </summary>
        [DisplayName("行高")]
        [FloatPrecision(1)]
        [Range(0.5, 5.0)]
        public double LineHeight
        {
            get { return _lineHeight; }
            set { SetProperty(ref _lineHeight, value, nameof(LineHeight)); }
        }

        /// <summary>
        /// 段落前间距
        /// </summary>
        [DisplayName("段落前间距")]
        [FloatPrecision(0)]
        [Range(0, 100)]
        public double ParagraphSpacingBefore
        {
            get { return _paragraphSpacingBefore; }
            set { SetProperty(ref _paragraphSpacingBefore, value, nameof(ParagraphSpacingBefore)); }
        }

        /// <summary>
        /// 段落后间距
        /// </summary>
        [DisplayName("段落后间距")]
        [FloatPrecision(0)]
        [Range(0, 100)]
        public double ParagraphSpacingAfter
        {
            get { return _paragraphSpacingAfter; }
            set { SetProperty(ref _paragraphSpacingAfter, value, nameof(ParagraphSpacingAfter)); }
        }

        /// <summary>
        /// 文本对齐
        /// </summary>
        [DisplayName("文本水平对齐")]
        public TextAlignType TextAlignment
        {
            get { return _textAlignment; }
            set { SetProperty(ref _textAlignment, value, nameof(TextAlignment)); }
        }

        /// <summary>
        /// 文本垂直对齐
        /// </summary>
        [DisplayName("文本垂直对齐")]
        public TextVerticalAlignType VerticalTextAlignment
        {
            get { return _verticalTextAlignment; }
            set { SetProperty(ref _verticalTextAlignment, value, nameof(VerticalTextAlignment)); }
        }

        #endregion

        #region IMPPropObjectValue 实现

        bool IMPPropObjectValue.IsObject_Byte => false;

        Guid IMPPropObjectValue.GetObject_ID()
        {
            return Object_ID;
        }

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
                {
                    throw new Exception("对象值不是LableParagraphInfo转换的");
                }

                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                {
                    throw new Exception("对象值不是LableParagraphInfo转换的");
                }

                ((IJsonValueConvert)this).FromJsonDataObject((baseValue.Val as ObjectValue_Json).JsonVal);
            }
        }

        #endregion

        #region IJsonValueConvert 实现

        void IJsonValueConvert.FromJsonDataObject(string jsonDataObject)
        {
            if (string.IsNullOrEmpty(jsonDataObject))
            {
                throw new ArgumentNullException(nameof(jsonDataObject));
            }

            using JsonDocument jsonDocument = JsonDocument.Parse(jsonDataObject);
            JsonElement rootElement = jsonDocument.RootElement;

            JsonElement value;
            LineHeight = rootElement.TryGetProperty("LineHeight", out value) ? value.GetDouble() : 1.0;
            ParagraphSpacingBefore = rootElement.TryGetProperty("ParagraphSpacingBefore", out value) ? value.GetDouble() : 0;
            ParagraphSpacingAfter = rootElement.TryGetProperty("ParagraphSpacingAfter", out value) ? value.GetDouble() : 0;
            
            string alignStr = rootElement.TryGetProperty("TextAlignment", out value) ? value.GetString() : "Left";
            TextAlignment = Enum.TryParse<TextAlignType>(alignStr, out var result) ? result : TextAlignType.Left;

            string verticalAlignStr = rootElement.TryGetProperty("VerticalTextAlignment", out value) ? value.GetString() : "Center";
            VerticalTextAlignment = Enum.TryParse<TextVerticalAlignType>(verticalAlignStr, out var verticalResult) ? verticalResult : TextVerticalAlignType.Center;
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                LineHeight = LineHeight,
                ParagraphSpacingBefore = ParagraphSpacingBefore,
                ParagraphSpacingAfter = ParagraphSpacingAfter,
                TextAlignment = TextAlignment.ToString(),
                VerticalTextAlignment = VerticalTextAlignment.ToString()
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        #endregion

        #region 重写方法

        public override string ToString()
        {
            return $"行高:{LineHeight:F1}倍, {TextAlignment}, {VerticalTextAlignment}";
        }

        #endregion
    }
}
