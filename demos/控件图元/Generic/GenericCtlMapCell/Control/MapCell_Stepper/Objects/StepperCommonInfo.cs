using System;
using System.ComponentModel;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using GKG.Map.MapCell.Generic.Control.Lable;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.Stepper.Objects
{
    /// <summary>
    /// 步进器公共信息对象
    /// 包含数值、范围、步长、小数位、是否启用、提示文字等
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class StepperCommonInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        #region 静态字段

        public static readonly StepperCommonInfo Default = new StepperCommonInfo("标签", 0, 0, 100, 1, 0, true, "");
        public static readonly Guid Object_ID = new Guid("{C1B2C3D4-7777-4AC8-BF66-281412CDE205}");

        #endregion

        #region 私有字段

        private string _labelName;
        private decimal _value;
        private decimal _minimum;
        private decimal _maximum;
        private decimal _increment;
        private int _decimalPlaces;
        private bool _isEnabled;
        private string _toolTip;

        #endregion

        #region 构造函数

        public StepperCommonInfo()
            : this("标签", 0, 0, 100, 1, 0, true, "")
        {
        }

        public StepperCommonInfo(string labelName, decimal value, decimal minimum, decimal maximum, decimal increment, int decimalPlaces, bool isEnabled, string toolTip)
        {
            LabelName = labelName;
            Value = value;
            Minimum = minimum;
            Maximum = maximum;
            Increment = increment;
            DecimalPlaces = decimalPlaces;
            IsEnabled = isEnabled;
            ToolTip = toolTip;
        }

        #endregion

        #region 属性

        [DisplayName("标签名称")]
        [PropertySortOrder(1)]
        public string LabelName
        {
            get { return _labelName; }
            set { SetProperty(ref _labelName, value, nameof(LabelName)); }
        }

        [DisplayName("当前值")]
        [PropertySortOrder(2)]
        public decimal Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value, nameof(Value)); }
        }

        [DisplayName("最小值")]
        [PropertySortOrder(3)]
        public decimal Minimum
        {
            get { return _minimum; }
            set { SetProperty(ref _minimum, value, nameof(Minimum)); }
        }

        [DisplayName("最大值")]
        [PropertySortOrder(4)]
        public decimal Maximum
        {
            get { return _maximum; }
            set { SetProperty(ref _maximum, value, nameof(Maximum)); }
        }

        [DisplayName("步长")]
        [PropertySortOrder(5)]
        public decimal Increment
        {
            get { return _increment; }
            set { SetProperty(ref _increment, value, nameof(Increment)); }
        }

        [DisplayName("小数位数")]
        [PropertySortOrder(6)]
        public int DecimalPlaces
        {
            get { return _decimalPlaces; }
            set { SetProperty(ref _decimalPlaces, value, nameof(DecimalPlaces)); }
        }

        [DisplayName("是否启用")]
        [PropertySortOrder(7)]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetProperty(ref _isEnabled, value, nameof(IsEnabled)); }
        }

        [DisplayName("提示文字")]
        [PropertySortOrder(8)]
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
                    throw new Exception("对象值不是StepperCommonInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是StepperCommonInfo转换的");
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
            LabelName = rootElement.TryGetProperty("LabelName", out value) ? value.GetString() : "标签";
            Value = rootElement.TryGetProperty("Value", out value) ? value.GetDecimal() : 0;
            Minimum = rootElement.TryGetProperty("Minimum", out value) ? value.GetDecimal() : 0;
            Maximum = rootElement.TryGetProperty("Maximum", out value) ? value.GetDecimal() : 100;
            Increment = rootElement.TryGetProperty("Increment", out value) ? value.GetDecimal() : 1;
            DecimalPlaces = rootElement.TryGetProperty("DecimalPlaces", out value) ? value.GetInt32() : 0;
            IsEnabled = rootElement.TryGetProperty("IsEnabled", out value) ? value.GetBoolean() : true;
            ToolTip = rootElement.TryGetProperty("ToolTip", out value) ? value.GetString() : "";
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                LabelName = LabelName ?? "",
                Value,
                Minimum,
                Maximum,
                Increment,
                DecimalPlaces,
                IsEnabled,
                ToolTip = ToolTip ?? ""
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        #endregion

        public override string ToString()
        {
            return "步进器设置";
        }
    }
}
