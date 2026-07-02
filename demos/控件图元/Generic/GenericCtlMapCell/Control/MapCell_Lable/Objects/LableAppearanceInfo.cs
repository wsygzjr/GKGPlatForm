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
    /// 标签外观信息对象
    /// 包含透明度设置
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class LableAppearanceInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        #region 静态字段

        /// <summary>
        /// 默认外观信息
        /// </summary>
        public static readonly LableAppearanceInfo Default = new LableAppearanceInfo(1.0, 0, 0, 0, 0);

        /// <summary>
        /// 对象唯一标识符
        /// </summary>
        public static readonly Guid Object_ID = new Guid("{A1B2C3D4-2222-4AC8-BF66-281412CDE002}");

        #endregion

        #region 私有字段

        private double _opacity;
        private double _borderThicknessLeft;
        private double _borderThicknessTop;
        private double _borderThicknessRight;
        private double _borderThicknessBottom;

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public LableAppearanceInfo()
            : this(1.0, 0, 0, 0, 0)
        {
        }

        /// <summary>
        /// 带参数构造函数
        /// </summary>
        /// <param name="opacity">透明度</param>
        /// <param name="borderLeft">左边框线宽</param>
        /// <param name="borderTop">上边框线宽</param>
        /// <param name="borderRight">右边框线宽</param>
        /// <param name="borderBottom">下边框线宽</param>
        public LableAppearanceInfo(double opacity, double borderLeft, double borderTop, double borderRight, double borderBottom)
        {
            Opacity = opacity;
            BorderThicknessLeft = borderLeft;
            BorderThicknessTop = borderTop;
            BorderThicknessRight = borderRight;
            BorderThicknessBottom = borderBottom;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 透明度 (0.0 - 1.0)
        /// </summary>
        [DisplayName("透明度")]
        [FloatPrecision(2)]
        [Range(0.0, 1.0)]
        public double Opacity
        {
            get { return _opacity; }
            set { SetProperty(ref _opacity, value, nameof(Opacity)); }
        }

        [DisplayName("左边框线宽")]
        [FloatPrecision(0)]
        [Range(0, 100)]
        public double BorderThicknessLeft
        {
            get { return _borderThicknessLeft; }
            set { SetProperty(ref _borderThicknessLeft, Math.Max(0, value), nameof(BorderThicknessLeft)); }
        }

        [DisplayName("上边框线宽")]
        [FloatPrecision(0)]
        [Range(0, 100)]
        public double BorderThicknessTop
        {
            get { return _borderThicknessTop; }
            set { SetProperty(ref _borderThicknessTop, Math.Max(0, value), nameof(BorderThicknessTop)); }
        }

        [DisplayName("右边框线宽")]
        [FloatPrecision(0)]
        [Range(0, 100)]
        public double BorderThicknessRight
        {
            get { return _borderThicknessRight; }
            set { SetProperty(ref _borderThicknessRight, Math.Max(0, value), nameof(BorderThicknessRight)); }
        }

        [DisplayName("下边框线宽")]
        [FloatPrecision(0)]
        [Range(0, 100)]
        public double BorderThicknessBottom
        {
            get { return _borderThicknessBottom; }
            set { SetProperty(ref _borderThicknessBottom, Math.Max(0, value), nameof(BorderThicknessBottom)); }
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
                    throw new Exception("对象值不是LableAppearanceInfo转换的");
                }

                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                {
                    throw new Exception("对象值不是LableAppearanceInfo转换的");
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
            double opacity = rootElement.TryGetProperty("Opacity", out value) ? value.GetDouble() : 1.0;
            double borderLeft = rootElement.TryGetProperty("BorderThicknessLeft", out value) ? value.GetDouble() : 0;
            double borderTop = rootElement.TryGetProperty("BorderThicknessTop", out value) ? value.GetDouble() : 0;
            double borderRight = rootElement.TryGetProperty("BorderThicknessRight", out value) ? value.GetDouble() : 0;
            double borderBottom = rootElement.TryGetProperty("BorderThicknessBottom", out value) ? value.GetDouble() : 0;

            Opacity = opacity;
            BorderThicknessLeft = borderLeft;
            BorderThicknessTop = borderTop;
            BorderThicknessRight = borderRight;
            BorderThicknessBottom = borderBottom;
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                Opacity = Opacity,
                BorderThicknessLeft = BorderThicknessLeft,
                BorderThicknessTop = BorderThicknessTop,
                BorderThicknessRight = BorderThicknessRight,
                BorderThicknessBottom = BorderThicknessBottom
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        #endregion

        #region 重写方法

        public override string ToString()
        {
            return $"透明度:{Opacity:F2}, 边框:{BorderThicknessLeft}/{BorderThicknessTop}/{BorderThicknessRight}/{BorderThicknessBottom}";
        }

        #endregion
    }
}
