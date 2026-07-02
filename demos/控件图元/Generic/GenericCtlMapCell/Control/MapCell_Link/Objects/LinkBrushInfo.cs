using System;
using System.ComponentModel;
using System.Text.Json;
using Avalonia.Media;
using GF_Gereric;
using Griffins;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.Link
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class LinkBrushInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        public static readonly Guid Object_ID = new Guid("{6A0F85EC-6D38-498A-B43B-C20C65F78111}");
        public static readonly LinkBrushInfo Default = new LinkBrushInfo();

        private string _textColorStr = Colors.Blue.ToColorString();
        private string _hoverTextColorStr = Colors.Red.ToColorString();

        [DisplayName("文本颜色")]
        [Category("画笔")]
        public Color TextColor
        {
            get => Color.Parse(_textColorStr);
            set => SetProperty(ref _textColorStr, value.ToColorString(), nameof(TextColor));
        }

        [DisplayName("鼠标悬停颜色")]
        [Category("画笔")]
        public Color HoverTextColor
        {
            get => Color.Parse(_hoverTextColorStr);
            set => SetProperty(ref _hoverTextColorStr, value.ToColorString(), nameof(HoverTextColor));
        }

        bool IMPPropObjectValue.IsObject_Byte => false;

        Guid IMPPropObjectValue.GetObject_ID() => Object_ID;

        GriffinsBaseValue IMPPropObjectValue.ToBaseValue()
        {
            var objectValue = new ObjectValue_Json(Object_ID)
            {
                JsonVal = ((IJsonValueConvert)this).ToJsonDataObject()
            };
            return GriffinsBaseValue.Create(objectValue);
        }

        void IMPPropObjectValue.PopulateFromBaseValue(GriffinsBaseValue baseValue)
        {
            if (baseValue?.Val is not ObjectValue_Json objectValue || objectValue.Object_ID != Object_ID)
                throw new Exception("对象值不是LinkBrushInfo转换的");

            ((IJsonValueConvert)this).FromJsonDataObject(objectValue.JsonVal);
        }

        void IJsonValueConvert.FromJsonDataObject(string jsonDataObject)
        {
            if (string.IsNullOrWhiteSpace(jsonDataObject))
                throw new ArgumentNullException(nameof(jsonDataObject));

            using var jsonDocument = JsonDocument.Parse(jsonDataObject);
            var root = jsonDocument.RootElement;
            if (root.TryGetProperty(nameof(TextColor), out var textColor))
                _textColorStr = textColor.GetString() ?? Default._textColorStr;
            if (root.TryGetProperty(nameof(HoverTextColor), out var hoverColor))
                _hoverTextColorStr = hoverColor.GetString() ?? Default._hoverTextColorStr;
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            return JsonSerializer.Serialize(new
            {
                TextColor = _textColorStr,
                HoverTextColor = _hoverTextColorStr
            });
        }

        public override string ToString() => $"文本:{_textColorStr}, 悬停:{_hoverTextColorStr}";
    }
}
