using System;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using GF_Gereric;
using Griffins;
using GKG.Map.MapCell.Generic.Control.Lable;
using PropertyModels.ComponentModel;
using Avalonia.Media;

namespace GKG.Map.MapCell.Generic.Stepper.Objects
{
    /// <summary>
    /// 步进器画笔信息对象
    /// 包含背景色、边框色、前景色
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class StepperBrushInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        #region 静态字段

        public static readonly StepperBrushInfo Default = new StepperBrushInfo();
        public static readonly Guid Object_ID = new Guid("{C1B2C3D4-7777-4AC8-BF66-281412CDE210}");

        #endregion

        #region 私有字段

        private string _backColorStr;
        private string _borderColorStr;
        private string _foreColorStr;

        #endregion

        #region 构造函数

        public StepperBrushInfo()
            : this("#FFFFFFFF", "#FF000000", "#FF000000")
        {
        }

        public StepperBrushInfo(string backColorStr, string borderColorStr, string foreColorStr)
        {
            BackColorStr = backColorStr;
            BorderColorStr = borderColorStr;
            ForeColorStr = foreColorStr;
        }

        #endregion

        #region 属性

        [Browsable(false)]
        public string BackColorStr
        {
            get => _backColorStr;
            set
            {
                if (_backColorStr != value)
                {
                    _backColorStr = value;
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
                    _borderColorStr = value;
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
                    _foreColorStr = value;
                    RaisePropertyChanged(nameof(ForeColorStr));
                    RaisePropertyChanged(nameof(ForeColor));
                }
            }
        }

        [DisplayName("背景色")]
        [PropertySortOrder(1)]
        [JsonIgnore]
        public Color BackColor
        {
            get => string.IsNullOrEmpty(_backColorStr) ? Colors.Transparent : Color.Parse(_backColorStr);
            set
            {
                var str = value.ToString();
                if (_backColorStr != str)
                {
                    _backColorStr = str;
                    RaisePropertyChanged(nameof(BackColorStr));
                    RaisePropertyChanged(nameof(BackColor));
                }
            }
        }

        [DisplayName("边框色")]
        [PropertySortOrder(2)]
        [JsonIgnore]
        public Color BorderColor
        {
            get => string.IsNullOrEmpty(_borderColorStr) ? Colors.Transparent : Color.Parse(_borderColorStr);
            set
            {
                var str = value.ToString();
                if (_borderColorStr != str)
                {
                    _borderColorStr = str;
                    RaisePropertyChanged(nameof(BorderColorStr));
                    RaisePropertyChanged(nameof(BorderColor));
                }
            }
        }

        [DisplayName("前景色")]
        [PropertySortOrder(3)]
        [JsonIgnore]
        public Color ForeColor
        {
            get => string.IsNullOrEmpty(_foreColorStr) ? Colors.Transparent : Color.Parse(_foreColorStr);
            set
            {
                var str = value.ToString();
                if (_foreColorStr != str)
                {
                    _foreColorStr = str;
                    RaisePropertyChanged(nameof(ForeColorStr));
                    RaisePropertyChanged(nameof(ForeColor));
                }
            }
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
                    throw new Exception("对象值不是StepperBrushInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是StepperBrushInfo转换的");
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
            JsonElement val;

            BackColorStr = rootElement.TryGetProperty("BackColorStr", out val) ? val.GetString() : "#FFFFFFFF";
            BorderColorStr = rootElement.TryGetProperty("BorderColorStr", out val) ? val.GetString() : "#FF000000";
            ForeColorStr = rootElement.TryGetProperty("ForeColorStr", out val) ? val.GetString() : "#FF000000";
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var obj = new
            {
                BackColorStr,
                BorderColorStr,
                ForeColorStr
            };
            return System.Text.Json.JsonSerializer.Serialize(obj);
        }

        #endregion

        public override string ToString()
        {
            return "画笔设置";
        }
    }
}
