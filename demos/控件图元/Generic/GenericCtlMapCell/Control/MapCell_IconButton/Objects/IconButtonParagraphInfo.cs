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
    /// 图标按钮段落信息
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class IconButtonParagraphInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        /// <summary>
        /// 对象ID
        /// </summary>
        public static readonly Guid Object_ID = new Guid("{6A2B3C4D-5E6F-7A8B-9C0D-E1F2A3B4C5EB}");

        /// <summary>
        /// 默认值
        /// </summary>
        public static readonly IconButtonParagraphInfo Default = new IconButtonParagraphInfo();

        private int _lineHeight = 23;
        private int _paragraphSpacingBefore = 0;
        private int _paragraphSpacingAfter = 0;
        private TextAlignmentEnum _textAlignment = TextAlignmentEnum.Center;

        /// <summary>
        /// 行高
        /// </summary>
        [DisplayName("行高")]
        [Category("文本.段落")]
        [Range(0, 100)]
        public int LineHeight
        {
            get => _lineHeight;
            set => SetProperty(ref _lineHeight, value);
        }

        /// <summary>
        /// 段落前间距
        /// </summary>
        [DisplayName("段落前间距")]
        [Category("文本.段落")]
        public int ParagraphSpacingBefore
        {
            get => _paragraphSpacingBefore;
            set => SetProperty(ref _paragraphSpacingBefore, value);
        }

        /// <summary>
        /// 段落后间距
        /// </summary>
        [DisplayName("段落后间距")]
        [Category("文本.段落")]
        public int ParagraphSpacingAfter
        {
            get => _paragraphSpacingAfter;
            set => SetProperty(ref _paragraphSpacingAfter, value);
        }

        /// <summary>
        /// 文本对齐
        /// </summary>
        [DisplayName("文本对齐")]
        [Category("文本.段落")]
        public TextAlignmentEnum TextAlignment
        {
            get => _textAlignment;
            set => SetProperty(ref _textAlignment, value);
        }

        /// <summary>
        /// 文本对齐枚举
        /// </summary>
        public enum TextAlignmentEnum
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
            Right
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
                    throw new Exception("对象值不是IconButtonParagraphInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是IconButtonParagraphInfo转换的");
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
            if (rootElement.TryGetProperty("LineHeight", out value))
                _lineHeight = value.GetInt32();
            if (rootElement.TryGetProperty("ParagraphSpacingBefore", out value))
                _paragraphSpacingBefore = value.GetInt32();
            if (rootElement.TryGetProperty("ParagraphSpacingAfter", out value))
                _paragraphSpacingAfter = value.GetInt32();
            if (rootElement.TryGetProperty("TextAlignment", out value))
                _textAlignment = (TextAlignmentEnum)value.GetInt32();
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                LineHeight = _lineHeight,
                ParagraphSpacingBefore = _paragraphSpacingBefore,
                ParagraphSpacingAfter = _paragraphSpacingAfter,
                TextAlignment = (int)_textAlignment
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        #endregion
    }
}