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
    /// 步进器外观信息对象
    /// 包含不透明度和边框粗细
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class StepperAppearanceInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        #region 静态字段

        public static readonly StepperAppearanceInfo Default = new StepperAppearanceInfo();
        public static readonly Guid Object_ID = new Guid("{C1B2C3D4-7777-4AC8-BF66-281412CDE208}");

        #endregion

        #region 私有字段

        private double _opacity;
        private double _borderThicknessLeft;
        private double _borderThicknessTop;
        private double _borderThicknessRight;
        private double _borderThicknessBottom;

        #endregion

        #region 构造函数

        public StepperAppearanceInfo()
            : this(1.0, 1, 1, 1, 1)
        {
        }

        public StepperAppearanceInfo(double opacity,
            double borderThicknessLeft, double borderThicknessTop,
            double borderThicknessRight, double borderThicknessBottom)
        {
            Opacity = opacity;
            BorderThicknessLeft = borderThicknessLeft;
            BorderThicknessTop = borderThicknessTop;
            BorderThicknessRight = borderThicknessRight;
            BorderThicknessBottom = borderThicknessBottom;
        }

        #endregion

        #region 属性

        [DisplayName("不透明度")]
        [PropertySortOrder(1)]
        [Range(0, 1.0)]
        [FloatPrecision(2)]
        public double Opacity
        {
            get => _opacity;
            set => SetProperty(ref _opacity, value, nameof(Opacity));
        }

        [DisplayName("左边框")]
        [PropertySortOrder(3)]
        [FloatPrecision(0)]
        public double BorderThicknessLeft
        {
            get => _borderThicknessLeft;
            set => SetProperty(ref _borderThicknessLeft, value, nameof(BorderThicknessLeft));
        }

        [DisplayName("上边框")]
        [PropertySortOrder(4)]
        [FloatPrecision(0)]
        public double BorderThicknessTop
        {
            get => _borderThicknessTop;
            set => SetProperty(ref _borderThicknessTop, value, nameof(BorderThicknessTop));
        }

        [DisplayName("右边框")]
        [PropertySortOrder(5)]
        [FloatPrecision(0)]
        public double BorderThicknessRight
        {
            get => _borderThicknessRight;
            set => SetProperty(ref _borderThicknessRight, value, nameof(BorderThicknessRight));
        }

        [DisplayName("下边框")]
        [PropertySortOrder(6)]
        [FloatPrecision(0)]
        public double BorderThicknessBottom
        {
            get => _borderThicknessBottom;
            set => SetProperty(ref _borderThicknessBottom, value, nameof(BorderThicknessBottom));
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
                    throw new Exception("对象值不是StepperAppearanceInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是StepperAppearanceInfo转换的");
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

            Opacity = rootElement.TryGetProperty("Opacity", out val) ? val.GetDouble() : 1.0;
            BorderThicknessLeft = rootElement.TryGetProperty("BorderThicknessLeft", out val) ? val.GetDouble() : 1;
            BorderThicknessTop = rootElement.TryGetProperty("BorderThicknessTop", out val) ? val.GetDouble() : 1;
            BorderThicknessRight = rootElement.TryGetProperty("BorderThicknessRight", out val) ? val.GetDouble() : 1;
            BorderThicknessBottom = rootElement.TryGetProperty("BorderThicknessBottom", out val) ? val.GetDouble() : 1;
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var obj = new
            {
                Opacity,
                BorderThicknessLeft,
                BorderThicknessTop,
                BorderThicknessRight,
                BorderThicknessBottom
            };
            return System.Text.Json.JsonSerializer.Serialize(obj);
        }

        #endregion

        public override string ToString()
        {
            return "外观设置";
        }
    }
}
