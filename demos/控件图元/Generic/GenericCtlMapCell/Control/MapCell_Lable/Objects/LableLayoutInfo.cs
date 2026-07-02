using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.Lable
{
    /// <summary>
    /// 标签布局信息对象
        /// 包含对齐方式
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class LableLayoutInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        #region 静态字段

        /// <summary>
        /// 默认布局信息
        /// </summary>
        public static readonly LableLayoutInfo Default = new LableLayoutInfo();

        /// <summary>
        /// 对象唯一标识符
        /// </summary>
        public static readonly Guid Object_ID = new Guid("{A1B2C3D4-3333-4AC8-BF66-281412CDE003}");

        #endregion

        #region 私有字段

        private HorizontalAlignType _horizontalAlign;
        private VerticalAlignType _verticalAlign;

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public LableLayoutInfo()
            : this(HorizontalAlignType.Stretch, VerticalAlignType.Stretch)
        {
        }

        /// <summary>
        /// 带参数构造函数
        /// </summary>
        public LableLayoutInfo(HorizontalAlignType horizontalAlign, VerticalAlignType verticalAlign)
        {
            HorizontalAlign = horizontalAlign;
            VerticalAlign = verticalAlign;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 水平对齐
        /// </summary>
        [DisplayName("水平对齐")]
        public HorizontalAlignType HorizontalAlign
        {
            get { return _horizontalAlign; }
            set { SetProperty(ref _horizontalAlign, value, nameof(HorizontalAlign)); }
        }

        /// <summary>
        /// 垂直对齐
        /// </summary>
        [DisplayName("垂直对齐")]
        public VerticalAlignType VerticalAlign
        {
            get { return _verticalAlign; }
            set { SetProperty(ref _verticalAlign, value, nameof(VerticalAlign)); }
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
                    throw new Exception("对象值不是LableLayoutInfo转换的");
                }

                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                {
                    throw new Exception("对象值不是LableLayoutInfo转换的");
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
            string hAlignStr = rootElement.TryGetProperty("HorizontalAlign", out value) ? value.GetString() : "Stretch";
            HorizontalAlign = Enum.TryParse<HorizontalAlignType>(hAlignStr, out var hResult) ? hResult : HorizontalAlignType.Stretch;
            
            string vAlignStr = rootElement.TryGetProperty("VerticalAlign", out value) ? value.GetString() : "Stretch";
            VerticalAlign = Enum.TryParse<VerticalAlignType>(vAlignStr, out var vResult) ? vResult : VerticalAlignType.Stretch;
            
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            // 宽高主数据已迁到父类 Width/Height，这里不再由 LayoutInfo 自身承载尺寸。
            var value = new
            {
                HorizontalAlign = HorizontalAlign.ToString(),
                VerticalAlign = VerticalAlign.ToString()
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        #endregion

        #region 重写方法

        public override string ToString()
        {
            return $"{HorizontalAlign} / {VerticalAlign}";
        }

        #endregion
    }
}
