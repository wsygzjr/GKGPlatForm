using System;
using System.ComponentModel;
using System.Text.Json;
using Avalonia.Media;
using GF_Gereric;
using Griffins;
using Newtonsoft.JsonG;
using PropertyModels.ComponentModel;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GKG.Map.MapCell.Generic.CheckBox
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class CheckBoxBrushInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        public static readonly CheckBoxBrushInfo Default = new CheckBoxBrushInfo(
            Color.FromArgb(0, 255, 255, 255),
            Color.FromArgb(255, 100, 100, 100),
            Colors.Black
        );

        public static readonly Guid Object_ID = new Guid("{D2B2C3D4-1111-4AC8-BF66-281412CDE301}");

        private string _backColorStr = "#00FFFFFF";
        private string _borderColorStr = "#FF646464";
        private string _foreColorStr = "#FF000000";

        public CheckBoxBrushInfo()
            : this(Color.FromArgb(0, 255, 255, 255), Color.FromArgb(255, 100, 100, 100), Colors.Black)
        {
        }

        public CheckBoxBrushInfo(Color backColor, Color borderColor, Color foreColor)
        {
            _backColorStr = backColor.ToColorString();
            _borderColorStr = borderColor.ToColorString();
            _foreColorStr = foreColor.ToColorString();
        }

        [DisplayName("背景色")]
        [JsonIgnore]
        public Color BackColor
        {
            get { return Color.Parse(_backColorStr ?? "#00FFFFFF"); }
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

        [DisplayName("边框色")]
        [JsonIgnore]
        public Color BorderColor
        {
            get { return Color.Parse(_borderColorStr ?? "#FF646464"); }
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
            get { return Color.Parse(_foreColorStr ?? "#FF000000"); }
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

        [Browsable(false)]
        public string BackColorStr
        {
            get { return _backColorStr; }
            set
            {
                if (_backColorStr != value)
                {
                    _backColorStr = value ?? "#00FFFFFF";
                    RaisePropertyChanged(nameof(BackColorStr));
                    RaisePropertyChanged(nameof(BackColor));
                }
            }
        }

        [Browsable(false)]
        public string BorderColorStr
        {
            get { return _borderColorStr; }
            set
            {
                if (_borderColorStr != value)
                {
                    _borderColorStr = value ?? "#FF646464";
                    RaisePropertyChanged(nameof(BorderColorStr));
                    RaisePropertyChanged(nameof(BorderColor));
                }
            }
        }

        [Browsable(false)]
        public string ForeColorStr
        {
            get { return _foreColorStr; }
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
                    throw new Exception("对象值不是CheckBoxBrushInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是CheckBoxBrushInfo转换的");
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
            BackColorStr = rootElement.TryGetProperty("BackColor", out value) ? value.GetString() : "#00FFFFFF";
            BorderColorStr = rootElement.TryGetProperty("BorderColor", out value) ? value.GetString() : "#FF646464";
            ForeColorStr = rootElement.TryGetProperty("ForeColor", out value) ? value.GetString() : "#FF000000";
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                BackColor = BackColorStr,
                BorderColor = BorderColorStr,
                ForeColor = ForeColorStr
            };
            return JsonSerializer.Serialize(value);
        }

        public override string ToString() => $"背景:{BackColor}, 边框:{BorderColor}, 前景:{ForeColor}";
    }
}
