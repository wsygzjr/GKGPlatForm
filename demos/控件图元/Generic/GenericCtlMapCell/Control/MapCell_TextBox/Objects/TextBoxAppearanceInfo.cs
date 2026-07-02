using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PropertyModels.ComponentModel;
using System.Text.Json;
using GF_Gereric;
using Griffins;

namespace GKG.Map.MapCell.Generic.Control.MapCell_TextBox
{
    /// <summary>
    /// 文本输入框外观信息
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class TextBoxAppearanceInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        public static readonly Guid Object_ID = new Guid("{C7A47B7C-9C4A-46A1-A45F-68E0712B6A11}");
        public static readonly TextBoxAppearanceInfo Default = new TextBoxAppearanceInfo();

        private int _opacity = 0;

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
            // 属性面板使用 0-100，ViewModel 会换算为 Avalonia 的 0-1
            set => SetProperty(ref _opacity, Math.Max(0, Math.Min(100, value)));
        }

        [DisplayName("边框线宽")]
        [Category("外观")]
        public int BorderThickness
        {
            get => _borderThicknessLeft;
            set
            {
                value = Math.Max(0, value);

                // “统一线宽”修改时同步到四边，并确保四边属性都能收到变更通知
                var changed = false;
                changed |= SetProperty(ref _borderThicknessLeft, value, nameof(BorderThicknessLeft));
                changed |= SetProperty(ref _borderThicknessTop, value, nameof(BorderThicknessTop));
                changed |= SetProperty(ref _borderThicknessRight, value, nameof(BorderThicknessRight));
                changed |= SetProperty(ref _borderThicknessBottom, value, nameof(BorderThicknessBottom));

                if (changed)
                    RaisePropertyChanged(nameof(BorderThickness));
            }
        }

        [DisplayName("左")]
        [Category("外观")]
        public int BorderThicknessLeft
        {
            get => _borderThicknessLeft;
            set => SetProperty(ref _borderThicknessLeft, Math.Max(0, value));
        }

        [DisplayName("上")]
        [Category("外观")]
        public int BorderThicknessTop
        {
            get => _borderThicknessTop;
            set => SetProperty(ref _borderThicknessTop, Math.Max(0, value));
        }

        [DisplayName("右")]
        [Category("外观")]
        public int BorderThicknessRight
        {
            get => _borderThicknessRight;
            set => SetProperty(ref _borderThicknessRight, Math.Max(0, value));
        }

        [DisplayName("下")]
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
                    throw new Exception("对象值不是TextBoxAppearanceInfo转换的");
                if (ov.Object_ID != Object_ID)
                    throw new Exception("对象值不是TextBoxAppearanceInfo转换的");
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

            if (root.TryGetProperty("Opacity", out var v2) && v2.ValueKind == JsonValueKind.Number)
                _opacity = v2.GetInt32();

            if (root.TryGetProperty("BorderThicknessLeft", out var l) && l.ValueKind == JsonValueKind.Number)
                _borderThicknessLeft = Math.Max(0, l.GetInt32());
            if (root.TryGetProperty("BorderThicknessTop", out var t) && t.ValueKind == JsonValueKind.Number)
                _borderThicknessTop = Math.Max(0, t.GetInt32());
            if (root.TryGetProperty("BorderThicknessRight", out var r) && r.ValueKind == JsonValueKind.Number)
                _borderThicknessRight = Math.Max(0, r.GetInt32());
            if (root.TryGetProperty("BorderThicknessBottom", out var b) && b.ValueKind == JsonValueKind.Number)
                _borderThicknessBottom = Math.Max(0, b.GetInt32());
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
    }
}
