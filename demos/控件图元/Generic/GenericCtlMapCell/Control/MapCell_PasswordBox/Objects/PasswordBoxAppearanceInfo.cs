using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.PasswordBox
{
    /// <summary>
    /// 密码输入框外观信息
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class PasswordBoxAppearanceInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        public static readonly Guid Object_ID = new Guid("{B1C2D3E4-2001-4AC8-BF66-281412CDE102}");
        public static readonly PasswordBoxAppearanceInfo Default = new PasswordBoxAppearanceInfo();

        private double _opacity = 1.0;
        private double _borderThicknessLeft = 1;
        private double _borderThicknessTop = 1;
        private double _borderThicknessRight = 1;
        private double _borderThicknessBottom = 1;

        [DisplayName("透明度")]
        [FloatPrecision(2)]
        [Range(0.0, 1.0)]
        public double Opacity
        {
            get => _opacity;
            set => SetProperty(ref _opacity, value, nameof(Opacity));
        }

        [DisplayName("左边框粗细")]
        [FloatPrecision(0)]
        [Range(0, 100)]
        public double BorderThicknessLeft
        {
            get => _borderThicknessLeft;
            set => SetProperty(ref _borderThicknessLeft, value, nameof(BorderThicknessLeft));
        }

        [DisplayName("上边框粗细")]
        [FloatPrecision(0)]
        [Range(0, 100)]
        public double BorderThicknessTop
        {
            get => _borderThicknessTop;
            set => SetProperty(ref _borderThicknessTop, value, nameof(BorderThicknessTop));
        }

        [DisplayName("右边框粗细")]
        [FloatPrecision(0)]
        [Range(0, 100)]
        public double BorderThicknessRight
        {
            get => _borderThicknessRight;
            set => SetProperty(ref _borderThicknessRight, value, nameof(BorderThicknessRight));
        }

        [DisplayName("下边框粗细")]
        [FloatPrecision(0)]
        [Range(0, 100)]
        public double BorderThicknessBottom
        {
            get => _borderThicknessBottom;
            set => SetProperty(ref _borderThicknessBottom, value, nameof(BorderThicknessBottom));
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
                    throw new Exception("对象值不是PasswordBoxAppearanceInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是PasswordBoxAppearanceInfo转换的");
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
            if (rootElement.TryGetProperty("Opacity", out value))
                Opacity = value.GetDouble();
            if (rootElement.TryGetProperty("BorderThicknessLeft", out value))
                BorderThicknessLeft = value.GetDouble();
            if (rootElement.TryGetProperty("BorderThicknessTop", out value))
                BorderThicknessTop = value.GetDouble();
            if (rootElement.TryGetProperty("BorderThicknessRight", out value))
                BorderThicknessRight = value.GetDouble();
            if (rootElement.TryGetProperty("BorderThicknessBottom", out value))
                BorderThicknessBottom = value.GetDouble();
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                Opacity = _opacity,
                BorderThicknessLeft = _borderThicknessLeft,
                BorderThicknessTop = _borderThicknessTop,
                BorderThicknessRight = _borderThicknessRight,
                BorderThicknessBottom = _borderThicknessBottom
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        #endregion
    }
}
