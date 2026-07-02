using System;
using System.ComponentModel;
using System.Text.Json;
using Avalonia.Media;
using GF_Gereric;
using Griffins;
using Newtonsoft.JsonG;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.PasswordBox
{
    /// <summary>
    /// 密码输入框画笔信息
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class PasswordBoxBrushInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        public static readonly Guid Object_ID = new Guid("{B1C2D3E4-1001-4AC8-BF66-281412CDE101}");
        public static readonly PasswordBoxBrushInfo Default = new PasswordBoxBrushInfo();

        private string _backColorStr = "#FFFFFFFF";
        private string _borderColorStr = "#FF808080";
        private string _foreColorStr = "#FF000000";
        private string _focusBorderColorStr = "#FF0000FF";

        [DisplayName("背景色")]
        [JsonIgnore]
        public Color BackColor
        {
            get => Color.Parse(_backColorStr ?? "#FFFFFFFF");
            set
            {
                var newStr = value.ToColorString();
                if (_backColorStr != newStr)
                {
                    _backColorStr = newStr;
                    RaisePropertyChanged(nameof(BackColor));
                    RaisePropertyChanged(nameof(BackColorStr));
                }
            }
        }

        [DisplayName("边框颜色")]
        [JsonIgnore]
        public Color BorderColor
        {
            get => Color.Parse(_borderColorStr ?? "#FF808080");
            set
            {
                var newStr = value.ToColorString();
                if (_borderColorStr != newStr)
                {
                    _borderColorStr = newStr;
                    RaisePropertyChanged(nameof(BorderColor));
                    RaisePropertyChanged(nameof(BorderColorStr));
                }
            }
        }

        [DisplayName("前景色")]
        [JsonIgnore]
        public Color ForeColor
        {
            get => Color.Parse(_foreColorStr ?? "#FF000000");
            set
            {
                var newStr = value.ToColorString();
                if (_foreColorStr != newStr)
                {
                    _foreColorStr = newStr;
                    RaisePropertyChanged(nameof(ForeColor));
                    RaisePropertyChanged(nameof(ForeColorStr));
                }
            }
        }

        [DisplayName("选中时边框颜色")]
        [JsonIgnore]
        public Color FocusBorderColor
        {
            get => Color.Parse(_focusBorderColorStr ?? "#FF0000FF");
            set
            {
                var newStr = value.ToColorString();
                if (_focusBorderColorStr != newStr)
                {
                    _focusBorderColorStr = newStr;
                    RaisePropertyChanged(nameof(FocusBorderColor));
                    RaisePropertyChanged(nameof(FocusBorderColorStr));
                }
            }
        }

        // 用于 JSON 序列化的字符串属性
        [Browsable(false)]
        public string BackColorStr
        {
            get => _backColorStr;
            set
            {
                if (_backColorStr != value)
                {
                    _backColorStr = value ?? "#FFFFFFFF";
                    RaisePropertyChanged(nameof(BackColorStr));
                    RaisePropertyChanged(nameof(BackColor));
                }
            }
        }

        [Browsable(false)]
        public string BorderColorStr
        {
            get => _borderColorStr;
            set
            {
                if (_borderColorStr != value)
                {
                    _borderColorStr = value ?? "#FF808080";
                    RaisePropertyChanged(nameof(BorderColorStr));
                    RaisePropertyChanged(nameof(BorderColor));
                }
            }
        }

        [Browsable(false)]
        public string ForeColorStr
        {
            get => _foreColorStr;
            set
            {
                if (_foreColorStr != value)
                {
                    _foreColorStr = value ?? "#FF000000";
                    RaisePropertyChanged(nameof(ForeColorStr));
                    RaisePropertyChanged(nameof(ForeColor));
                }
            }
        }

        [Browsable(false)]
        public string FocusBorderColorStr
        {
            get => _focusBorderColorStr;
            set
            {
                if (_focusBorderColorStr != value)
                {
                    _focusBorderColorStr = value ?? "#FF0000FF";
                    RaisePropertyChanged(nameof(FocusBorderColorStr));
                    RaisePropertyChanged(nameof(FocusBorderColor));
                }
            }
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
                    throw new Exception("对象值不是PasswordBoxBrushInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是PasswordBoxBrushInfo转换的");
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
            if (rootElement.TryGetProperty("BackColor", out value))
                BackColorStr = value.GetString() ?? "#FFFFFFFF";
            if (rootElement.TryGetProperty("BorderColor", out value))
                BorderColorStr = value.GetString() ?? "#FF808080";
            if (rootElement.TryGetProperty("ForeColor", out value))
                ForeColorStr = value.GetString() ?? "#FF000000";
            if (rootElement.TryGetProperty("FocusBorderColor", out value))
                FocusBorderColorStr = value.GetString() ?? "#FF0000FF";
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                BackColor = BackColorStr,
                BorderColor = BorderColorStr,
                ForeColor = ForeColorStr,
                FocusBorderColor = FocusBorderColorStr
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        #endregion
    }
}
