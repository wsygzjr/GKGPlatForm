using System;
using System.ComponentModel;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Calendar.Objects
{
    public enum CalendarCursorType
    {
        [Description("默认箭头")] Arrow = 0,
        [Description("文本输入")] IBeam = 1,
        [Description("等待")] Wait = 2,
        [Description("十字")] Cross = 3,
        [Description("向上箭头")] UpArrow = 4,
        [Description("左右调整")] SizeWestEast = 5,
        [Description("上下调整")] SizeNorthSouth = 6,
        [Description("移动")] SizeAll = 7,
        [Description("禁止")] No = 8,
        [Description("手型")] Hand = 9,
        [Description("启动中")] AppStarting = 10,
        [Description("帮助")] Help = 11
    }

    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class CalendarCommonInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        public static readonly Guid Object_ID = new("{9e96e90d-0cc1-4c16-99ad-6c38564c8192}");
        public static readonly CalendarCommonInfo Default = new();

        private CalendarCursorType _hoverCursor = CalendarCursorType.Arrow;
        private bool _enabled = true;
        private bool _showButtonPanel = true;

        [DisplayName("鼠标悬停时光标")]
        [Category("公共")]
        public CalendarCursorType HoverCursor
        {
            get => _hoverCursor;
            set => SetProperty(ref _hoverCursor, value);
        }

        [DisplayName("是否启用")]
        [Category("公共")]
        public bool Enabled
        {
            get => _enabled;
            set => SetProperty(ref _enabled, value);
        }

        [DisplayName("ShowButtonPanel")]
        [Category("公共")]
        public bool ShowButtonPanel
        {
            get => _showButtonPanel;
            set => SetProperty(ref _showButtonPanel, value);
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
                throw new Exception("对象值不是 CalendarCommonInfo 转换的");

            ((IJsonValueConvert)this).FromJsonDataObject(objectValueJson.JsonVal);
        }

        void IJsonValueConvert.FromJsonDataObject(string jsonDataObject)
        {
            using JsonDocument jsonDocument = JsonDocument.Parse(jsonDataObject);
            JsonElement root = jsonDocument.RootElement;

            if (root.TryGetProperty(nameof(HoverCursor), out JsonElement hoverCursor))
            {
                if (hoverCursor.ValueKind == JsonValueKind.String && Enum.TryParse(hoverCursor.GetString(), out CalendarCursorType cursor))
                    _hoverCursor = cursor;
                else if (hoverCursor.ValueKind == JsonValueKind.Number && hoverCursor.TryGetInt32(out int cursorValue) && Enum.IsDefined(typeof(CalendarCursorType), cursorValue))
                    _hoverCursor = (CalendarCursorType)cursorValue;
            }
            if (root.TryGetProperty(nameof(Enabled), out JsonElement enabled) && (enabled.ValueKind == JsonValueKind.True || enabled.ValueKind == JsonValueKind.False))
                _enabled = enabled.GetBoolean();
            if (root.TryGetProperty(nameof(ShowButtonPanel), out JsonElement showButtonPanel) && (showButtonPanel.ValueKind == JsonValueKind.True || showButtonPanel.ValueKind == JsonValueKind.False))
                _showButtonPanel = showButtonPanel.GetBoolean();
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            return JsonSerializer.Serialize(new
            {
                HoverCursor = _hoverCursor.ToString(),
                Enabled = _enabled,
                ShowButtonPanel = _showButtonPanel
            });
        }
    }
}
