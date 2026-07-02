using System;
using System.ComponentModel;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using PropertyModels.ComponentModel;
using GKG.Map.MapCell.Generic.Control.Lable;

namespace GKG.Map.MapCell.Generic.CheckBox
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class CheckBoxCommonInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        public static readonly CheckBoxCommonInfo Default = new CheckBoxCommonInfo("复选框", (bool?)false, false, CursorType.Arrow, true, "");
        public static readonly Guid Object_ID = new Guid("{D2B2C3D4-6666-4AC8-BF66-281412CDE304}");

        private string _text;
        private bool? _isChecked;
        private bool _isThreeState;
        private CursorType _cursorType;
        private bool _isEnabled;
        private string _toolTip;

        public CheckBoxCommonInfo()
            : this("复选框", false, false, CursorType.Arrow, true, "")
        {
        }

        public CheckBoxCommonInfo(string text, bool isChecked, bool isThreeState, CursorType cursorType, bool isEnabled, string toolTip)
            : this(text, (bool?)isChecked, isThreeState, cursorType, isEnabled, toolTip)
        {
        }

        public CheckBoxCommonInfo(string text, bool? isChecked, bool isThreeState, CursorType cursorType, bool isEnabled, string toolTip)
        {
            Text = text;
            IsChecked = isChecked;
            IsThreeState = isThreeState;
            CursorType = cursorType;
            IsEnabled = isEnabled;
            ToolTip = toolTip;
        }

        [DisplayName("文本")]
        [PropertySortOrder(1)]
        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value, nameof(Text)); }
        }

        [DisplayName("是否选中")]
        [PropertySortOrder(2)]
        public bool? IsChecked
        {
            get { return _isChecked; }
            set { SetProperty(ref _isChecked, value, nameof(IsChecked)); }
        }

        [DisplayName("支持三态")]
        [Description("是否支持不确定状态")]
        [PropertySortOrder(3)]
        public bool IsThreeState
        {
            get { return _isThreeState; }
            set { SetProperty(ref _isThreeState, value, nameof(IsThreeState)); }
        }

        [DisplayName("鼠标样式")]
        [PropertySortOrder(4)]
        public CursorType CursorType
        {
            get { return _cursorType; }
            set { SetProperty(ref _cursorType, value, nameof(CursorType)); }
        }

        [DisplayName("是否启用")]
        [PropertySortOrder(5)]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetProperty(ref _isEnabled, value, nameof(IsEnabled)); }
        }

        [DisplayName("提示文字")]
        [PropertySortOrder(6)]
        public string ToolTip
        {
            get { return _toolTip; }
            set { SetProperty(ref _toolTip, value, nameof(ToolTip)); }
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
                    throw new Exception("对象值不是CheckBoxCommonInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是CheckBoxCommonInfo转换的");
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
            Text = rootElement.TryGetProperty("Text", out value) ? value.GetString() : "复选框";
            // 旧页面里可能仍然保留 GroupName 字段，这里按兼容口径静默忽略。
            if (rootElement.TryGetProperty("IsChecked", out value))
            {
                if (value.ValueKind == JsonValueKind.True || value.ValueKind == JsonValueKind.False)
                    IsChecked = value.GetBoolean();
                else if (value.ValueKind == JsonValueKind.Null)
                    IsChecked = null;
                else
                    IsChecked = false;
            }
            else
            {
                IsChecked = false;
            }
            IsThreeState = rootElement.TryGetProperty("IsThreeState", out value) ? value.GetBoolean() : false;

            string cursorStr = rootElement.TryGetProperty("CursorType", out value) ? value.GetString() : "Arrow";
            CursorType = Enum.TryParse<CursorType>(cursorStr, out var result) ? result : CursorType.Arrow;

            IsEnabled = rootElement.TryGetProperty("IsEnabled", out value) ? value.GetBoolean() : true;
            ToolTip = rootElement.TryGetProperty("ToolTip", out value) ? value.GetString() : "";
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                Text = Text ?? "",
                IsChecked,
                IsThreeState,
                CursorType = CursorType.ToString(),
                IsEnabled,
                ToolTip = ToolTip ?? ""
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        public override string ToString()
        {
            string text = string.IsNullOrEmpty(Text) ? "(空)" : (Text.Length > 10 ? Text.Substring(0, 10) + "..." : Text);
            return text;
        }
    }
}
