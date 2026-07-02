using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.CheckBox
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class CheckBoxAppearanceInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        public static readonly CheckBoxAppearanceInfo Default = new CheckBoxAppearanceInfo(1.0, 1, 1, 1, 1);
        public static readonly Guid Object_ID = new Guid("{D2B2C3D4-2222-4AC8-BF66-281412CDE302}");

        private double _opacity;
        private double _borderThicknessLeft;
        private double _borderThicknessTop;
        private double _borderThicknessRight;
        private double _borderThicknessBottom;

        public CheckBoxAppearanceInfo()
            : this(1.0, 1, 1, 1, 1)
        {
        }

        public CheckBoxAppearanceInfo(double opacity, double borderLeft, double borderTop, double borderRight, double borderBottom)
        {
            Opacity = opacity;
            BorderThicknessLeft = borderLeft;
            BorderThicknessTop = borderTop;
            BorderThicknessRight = borderRight;
            BorderThicknessBottom = borderBottom;
        }

        [DisplayName("透明度")]
        [FloatPrecision(2)]
        [Range(0.0, 1.0)]
        [PropertySortOrder(1)]
        public double Opacity
        {
            get { return _opacity; }
            set { SetProperty(ref _opacity, value, nameof(Opacity)); }
        }

        [DisplayName("左边框线宽")]
        [FloatPrecision(0)]
        [Range(0, 100)]
        [PropertySortOrder(3)]
        public double BorderThicknessLeft
        {
            get => _borderThicknessLeft;
            set => SetProperty(ref _borderThicknessLeft, Math.Max(0, value), nameof(BorderThicknessLeft));
        }

        [DisplayName("上边框线宽")]
        [FloatPrecision(0)]
        [Range(0, 100)]
        [PropertySortOrder(4)]
        public double BorderThicknessTop
        {
            get => _borderThicknessTop;
            set => SetProperty(ref _borderThicknessTop, Math.Max(0, value), nameof(BorderThicknessTop));
        }

        [DisplayName("右边框线宽")]
        [FloatPrecision(0)]
        [Range(0, 100)]
        [PropertySortOrder(5)]
        public double BorderThicknessRight
        {
            get => _borderThicknessRight;
            set => SetProperty(ref _borderThicknessRight, Math.Max(0, value), nameof(BorderThicknessRight));
        }

        [DisplayName("下边框线宽")]
        [FloatPrecision(0)]
        [Range(0, 100)]
        [PropertySortOrder(6)]
        public double BorderThicknessBottom
        {
            get => _borderThicknessBottom;
            set => SetProperty(ref _borderThicknessBottom, Math.Max(0, value), nameof(BorderThicknessBottom));
        }

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
                    throw new Exception("对象值不是CheckBoxAppearanceInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是CheckBoxAppearanceInfo转换的");
                ((IJsonValueConvert)this).FromJsonDataObject((baseValue.Val as ObjectValue_Json).JsonVal);
            }
        }

        void IJsonValueConvert.FromJsonDataObject(string jsonDataObject)
        {
            if (string.IsNullOrEmpty(jsonDataObject))
                throw new ArgumentNullException(nameof(jsonDataObject));

            using JsonDocument jsonDocument = JsonDocument.Parse(jsonDataObject);
            JsonElement rootElement = jsonDocument.RootElement;

            JsonElement value;
            Opacity = rootElement.TryGetProperty("Opacity", out value) ? value.GetDouble() : 1.0;
            BorderThicknessLeft = rootElement.TryGetProperty("BorderThicknessLeft", out value) ? value.GetDouble() : 1;
            BorderThicknessTop = rootElement.TryGetProperty("BorderThicknessTop", out value) ? value.GetDouble() : 1;
            BorderThicknessRight = rootElement.TryGetProperty("BorderThicknessRight", out value) ? value.GetDouble() : 1;
            BorderThicknessBottom = rootElement.TryGetProperty("BorderThicknessBottom", out value) ? value.GetDouble() : 1;
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                Opacity,
                BorderThicknessLeft,
                BorderThicknessTop,
                BorderThicknessRight,
                BorderThicknessBottom
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        public override string ToString() => $"透明度:{Opacity:F2}";
    }
}
