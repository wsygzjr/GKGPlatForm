using System;
using System.ComponentModel;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using GKG.Map.MapCell.Generic.Control.Lable;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.PasswordBox
{
    /// <summary>
    /// 密码输入框公共信息
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class PasswordBoxCommonInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        public static readonly Guid Object_ID = new Guid("{B1C2D3E4-4001-4AC8-BF66-281412CDE104}");
        public static readonly PasswordBoxCommonInfo Default = new PasswordBoxCommonInfo();

        private string _passwordValue = "";
        private CursorType _cursorType = CursorType.Ibeam;
        private bool _enabled = true;
        private string _placeholderText = "";
        private bool _passwordVisible = false;

        [DisplayName("密码值")]
        [PasswordPropertyText(true)]
        public string PasswordValue
        {
            get => _passwordValue;
            set => SetProperty(ref _passwordValue, value ?? "", nameof(PasswordValue));
        }

        [DisplayName("鼠标光标")]
        public CursorType CursorType
        {
            get => _cursorType;
            set => SetProperty(ref _cursorType, value, nameof(CursorType));
        }

        [DisplayName("是否启用")]
        public bool Enabled
        {
            get => _enabled;
            set => SetProperty(ref _enabled, value, nameof(Enabled));
        }

        [DisplayName("提示文本")]
        public string PlaceholderText
        {
            get => _placeholderText;
            set => SetProperty(ref _placeholderText, value ?? "", nameof(PlaceholderText));
        }

        [DisplayName("密码可见")]
        public bool PasswordVisible
        {
            get => _passwordVisible;
            set => SetProperty(ref _passwordVisible, value, nameof(PasswordVisible));
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
                    throw new Exception("对象值不是PasswordBoxCommonInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是PasswordBoxCommonInfo转换的");
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
            if (rootElement.TryGetProperty("PasswordValue", out value))
                PasswordValue = value.GetString() ?? "";
            if (rootElement.TryGetProperty("CursorType", out value))
            {
                var str = value.GetString();
                if (Enum.TryParse<CursorType>(str, out var c))
                    CursorType = c;
            }
            if (rootElement.TryGetProperty("Enabled", out value))
                Enabled = value.GetBoolean();
            if (rootElement.TryGetProperty("PlaceholderText", out value))
                PlaceholderText = value.GetString() ?? "";
            if (rootElement.TryGetProperty("PasswordVisible", out value))
                PasswordVisible = value.GetBoolean();
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                PasswordValue = _passwordValue,
                CursorType = _cursorType.ToString(),
                Enabled = _enabled,
                PlaceholderText = _placeholderText,
                PasswordVisible = _passwordVisible
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        #endregion
    }
}
