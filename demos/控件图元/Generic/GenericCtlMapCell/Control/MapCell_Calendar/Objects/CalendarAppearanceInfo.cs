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
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class CalendarAppearanceInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        public static readonly Guid Object_ID = new("{bd5830c1-1062-4c87-bbb9-cc12669995d4}");
        public static readonly CalendarAppearanceInfo Default = new();

        private int _opacity;
        private int _borderThicknessLeft = 1;
        private int _borderThicknessTop = 1;
        private int _borderThicknessRight = 1;
        private int _borderThicknessBottom = 1;

        [DisplayName("透明度")]
        [Category("外观")]
        [Range(0, 100)]
        public int Opacity
        {
            get => _opacity;
            set => SetProperty(ref _opacity, Math.Clamp(value, 0, 100));
        }

        [DisplayName("边框线宽")]
        [Category("外观")]
        public int BorderThickness
        {
            get => _borderThicknessLeft;
            set
            {
                int normalized = Math.Max(0, value);
                bool changed = false;
                changed |= SetProperty(ref _borderThicknessLeft, normalized, nameof(BorderThicknessLeft));
                changed |= SetProperty(ref _borderThicknessTop, normalized, nameof(BorderThicknessTop));
                changed |= SetProperty(ref _borderThicknessRight, normalized, nameof(BorderThicknessRight));
                changed |= SetProperty(ref _borderThicknessBottom, normalized, nameof(BorderThicknessBottom));
                if (changed)
                    RaisePropertyChanged(nameof(BorderThickness));
            }
        }

        [DisplayName("左边框")]
        [Category("外观")]
        public int BorderThicknessLeft
        {
            get => _borderThicknessLeft;
            set => SetProperty(ref _borderThicknessLeft, Math.Max(0, value));
        }

        [DisplayName("上边框")]
        [Category("外观")]
        public int BorderThicknessTop
        {
            get => _borderThicknessTop;
            set => SetProperty(ref _borderThicknessTop, Math.Max(0, value));
        }

        [DisplayName("右边框")]
        [Category("外观")]
        public int BorderThicknessRight
        {
            get => _borderThicknessRight;
            set => SetProperty(ref _borderThicknessRight, Math.Max(0, value));
        }

        [DisplayName("下边框")]
        [Category("外观")]
        public int BorderThicknessBottom
        {
            get => _borderThicknessBottom;
            set => SetProperty(ref _borderThicknessBottom, Math.Max(0, value));
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
                throw new Exception("对象值不是 CalendarAppearanceInfo 转换的");

            ((IJsonValueConvert)this).FromJsonDataObject(objectValueJson.JsonVal);
        }

        void IJsonValueConvert.FromJsonDataObject(string jsonDataObject)
        {
            using JsonDocument jsonDocument = JsonDocument.Parse(jsonDataObject);
            JsonElement root = jsonDocument.RootElement;

            if (root.TryGetProperty(nameof(Opacity), out JsonElement opacity) && opacity.ValueKind == JsonValueKind.Number)
                _opacity = opacity.GetInt32();
            if (root.TryGetProperty(nameof(BorderThicknessLeft), out JsonElement left) && left.ValueKind == JsonValueKind.Number)
                _borderThicknessLeft = Math.Max(0, left.GetInt32());
            if (root.TryGetProperty(nameof(BorderThicknessTop), out JsonElement top) && top.ValueKind == JsonValueKind.Number)
                _borderThicknessTop = Math.Max(0, top.GetInt32());
            if (root.TryGetProperty(nameof(BorderThicknessRight), out JsonElement right) && right.ValueKind == JsonValueKind.Number)
                _borderThicknessRight = Math.Max(0, right.GetInt32());
            if (root.TryGetProperty(nameof(BorderThicknessBottom), out JsonElement bottom) && bottom.ValueKind == JsonValueKind.Number)
                _borderThicknessBottom = Math.Max(0, bottom.GetInt32());
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            return JsonSerializer.Serialize(new
            {
                Opacity = _opacity,
                BorderThicknessLeft = _borderThicknessLeft,
                BorderThicknessTop = _borderThicknessTop,
                BorderThicknessRight = _borderThicknessRight,
                BorderThicknessBottom = _borderThicknessBottom
            });
        }
    }
}
