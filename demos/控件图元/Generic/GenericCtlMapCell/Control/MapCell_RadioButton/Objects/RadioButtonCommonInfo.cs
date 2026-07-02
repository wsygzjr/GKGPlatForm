using System;
using System.ComponentModel;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using GKG.Map.MapCell.Generic.Control.Lable;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.RadioButton
{
    /// <summary>
    /// 单选框公共信息对象
    /// 包含文本、组名、是否选中、是否三态、鼠标样式、是否启用、提示文字
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class RadioButtonCommonInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        #region 静态字段

        public static readonly RadioButtonCommonInfo Default = new RadioButtonCommonInfo("单选框", "", (bool?)false, false, CursorType.Arrow, true, "");
        public static readonly Guid Object_ID = new Guid("{C1B2C3D4-6666-4AC8-BF66-281412CDE204}");

        #endregion

        #region 私有字段

        private string _text;
        private string _groupName;
        private bool? _isChecked;
        private bool _isThreeState;
        private CursorType _cursorType;
        private bool _isEnabled;
        private string _toolTip;

        #endregion

        #region 构造函数

        public RadioButtonCommonInfo()
            : this("单选框", "", false, false, CursorType.Arrow, true, "")
        {
        }

        public RadioButtonCommonInfo(string text, string groupName, bool isChecked, bool isThreeState, CursorType cursorType, bool isEnabled, string toolTip)
            : this(text, groupName, (bool?)isChecked, isThreeState, cursorType, isEnabled, toolTip)
        {
        }

        public RadioButtonCommonInfo(string text, string groupName, bool? isChecked, bool isThreeState, CursorType cursorType, bool isEnabled, string toolTip)
        {
            Text = text;
            GroupName = groupName;
            IsChecked = isChecked;
            IsThreeState = isThreeState;
            CursorType = cursorType;
            IsEnabled = isEnabled;
            ToolTip = toolTip;
        }

        #endregion

        #region 属性

        [DisplayName("文本")]
        [PropertySortOrder(1)]
        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value, nameof(Text)); }
        }

        [DisplayName("组名")]
        [Description("同一组名的单选框互斥")]
        [PropertySortOrder(2)]
        public string GroupName
        {
            get { return _groupName; }
            set { SetProperty(ref _groupName, value, nameof(GroupName)); }
        }

        [DisplayName("是否选中")]
        [PropertySortOrder(3)]
        public bool? IsChecked
        {
            get { return _isChecked; }
            set { SetProperty(ref _isChecked, value, nameof(IsChecked)); }
        }

        [DisplayName("支持三态")]
        [Description("是否支持不确定状态")]
        [PropertySortOrder(4)]
        public bool IsThreeState
        {
            get { return _isThreeState; }
            set { SetProperty(ref _isThreeState, value, nameof(IsThreeState)); }
        }

        [DisplayName("鼠标样式")]
        [PropertySortOrder(5)]
        public CursorType CursorType
        {
            get { return _cursorType; }
            set { SetProperty(ref _cursorType, value, nameof(CursorType)); }
        }

        [DisplayName("是否启用")]
        [PropertySortOrder(6)]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetProperty(ref _isEnabled, value, nameof(IsEnabled)); }
        }

        [DisplayName("提示文字")]
        [PropertySortOrder(7)]
        public string ToolTip
        {
            get { return _toolTip; }
            set { SetProperty(ref _toolTip, value, nameof(ToolTip)); }
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
                    throw new Exception("对象值不是RadioButtonCommonInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是RadioButtonCommonInfo转换的");
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
            Text = rootElement.TryGetProperty("Text", out value) ? value.GetString() : "单选框";
            GroupName = rootElement.TryGetProperty("GroupName", out value) ? value.GetString() : "";
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
                GroupName = GroupName ?? "",
                IsChecked,
                IsThreeState,
                CursorType = CursorType.ToString(),
                IsEnabled,
                ToolTip = ToolTip ?? ""
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        #endregion

        public override string ToString()
        {
            string text = string.IsNullOrEmpty(Text) ? "(空)" :
                (Text.Length > 10 ? Text.Substring(0, 10) + "..." : Text);
            return text;
        }
    }
}
