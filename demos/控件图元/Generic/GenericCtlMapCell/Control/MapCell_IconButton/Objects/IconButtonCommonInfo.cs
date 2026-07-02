using System;
using System.ComponentModel;
using PropertyModels.ComponentModel;
using System.Text.Json;
using GKG.Map.MapCell.Generic.Control.MapCell_TextBox;
using GF_Gereric;
using Griffins;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GKG.Map.MapCell.Generic.IconButton
{
    /// <summary>
    /// 图标按钮公共信息
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class IconButtonCommonInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        /// <summary>
        /// 对象ID
        /// </summary>
        public static readonly Guid Object_ID = new Guid("{3A2B3C4D-5E6F-7A8B-9C0D-E1F2A3B4C5D8}");

        /// <summary>
        /// 默认值
        /// </summary>
        public static readonly IconButtonCommonInfo Default = new IconButtonCommonInfo();

        private string _buttonText = "";
        private CommonCursorType _hoverCursor = CommonCursorType.手型;
        private bool _enabled = true;
        private string _tooltipText = "";
        private string _groupId = "";

        /// <summary>
        /// 按钮文本
        /// </summary>
        [DisplayName("按钮文本")]
        [Category("公共")]
        public string ButtonText
        {
            get => _buttonText;
            set => SetProperty(ref _buttonText, value);
        }

        [DisplayName("鼠标悬停的光标")]
        [Category("公共")]
        public CommonCursorType HoverCursor
        {
            get => _hoverCursor;
            set => SetProperty(ref _hoverCursor, value);
        }

        /// <summary>
        /// 是否启用
        /// </summary>
        [DisplayName("是否启用")]
        [Category("公共")]
        public bool Enabled
        {
            get => _enabled;
            set => SetProperty(ref _enabled, value);
        }

        /// <summary>
        /// 提示文本
        /// </summary>
        [DisplayName("提示文本")]
        [Category("公共")]
        public string TooltipText
        {
            get => _tooltipText;
            set => SetProperty(ref _tooltipText, value);
        }

        /// <summary>
        /// 组ID
        /// </summary>
        [DisplayName("组ID")]
        [Category("公共")]
        public string GroupId
        {
            get => _groupId;
            set => SetProperty(ref _groupId, value);
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
                    throw new Exception("对象值不是IconButtonCommonInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是IconButtonCommonInfo转换的");
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
            if (rootElement.TryGetProperty("ButtonText", out value))
                _buttonText = value.GetString() ?? string.Empty;
            if (rootElement.TryGetProperty("HoverCursor", out value))
            {
                if (value.ValueKind == JsonValueKind.String && Enum.TryParse<CommonCursorType>(value.GetString(), out var ce))
                    _hoverCursor = ce;
                else if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var i) && Enum.IsDefined(typeof(CommonCursorType), i))
                    _hoverCursor = (CommonCursorType)i;
            }
            if (rootElement.TryGetProperty("Enabled", out value))
                _enabled = value.GetBoolean();
            if (rootElement.TryGetProperty("TooltipText", out value))
                _tooltipText = value.GetString() ?? string.Empty;
            if (rootElement.TryGetProperty("GroupId", out value))
                _groupId = value.GetString() ?? string.Empty;
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                ButtonText = _buttonText,
                HoverCursor = _hoverCursor.ToString(),
                Enabled = _enabled,
                TooltipText = _tooltipText,
                GroupId = _groupId
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        #endregion
    }
}
