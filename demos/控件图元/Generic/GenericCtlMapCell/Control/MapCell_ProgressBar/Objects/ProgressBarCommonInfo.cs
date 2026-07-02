using System;
using System.ComponentModel;
using System.Text.Json;
using Avalonia.Layout;
using GF_Gereric;
using Griffins;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.ProgressBar
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class ProgressBarCommonInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        public static readonly ProgressBarCommonInfo Default = new ProgressBarCommonInfo(Orientation.Horizontal, 0, 0, 100, false, true, "");
        public static readonly Guid Object_ID = new Guid("{58F8A3E8-41D3-4B34-9F32-9C2B3F1C8A14}");

        private Orientation _orientation;
        private double _value;
        private double _minimum;
        private double _maximum;
        private bool _isIndeterminate;
        private bool _isEnabled;
        private string _toolTip;

        private bool _normalizing;

        public ProgressBarCommonInfo()
            : this(Orientation.Horizontal, 0, 0, 100, false, true, "")
        {
        }

        public ProgressBarCommonInfo(Orientation orientation, double value, double minimum, double maximum, bool isIndeterminate, bool isEnabled, string toolTip)
        {
            Orientation = orientation;
            Minimum = minimum;
            Maximum = maximum;
            Value = value;
            IsIndeterminate = isIndeterminate;
            IsEnabled = isEnabled;
            ToolTip = toolTip;
        }

        [DisplayName("方向")]
        [PropertySortOrder(1)]
        public Orientation Orientation
        {
            get => _orientation;
            set => SetProperty(ref _orientation, value, nameof(Orientation));
        }

        [DisplayName("最小值")]
        [PropertySortOrder(2)]
        [FloatPrecision(0)]
        public double Minimum
        {
            get => _minimum;
            set
            {
                var newVal = value;
                if (!_normalizing)
                {
                    _normalizing = true;
                    SetProperty(ref _minimum, newVal, nameof(Minimum));
                    if (_maximum < _minimum) Maximum = _minimum;
                    if (_value < _minimum) Value = _minimum;
                    _normalizing = false;
                    return;
                }
                SetProperty(ref _minimum, newVal, nameof(Minimum));
            }
        }

        [DisplayName("最大值")]
        [PropertySortOrder(3)]
        [FloatPrecision(0)]
        public double Maximum
        {
            get => _maximum;
            set
            {
                var newVal = value;
                if (!_normalizing)
                {
                    _normalizing = true;
                    SetProperty(ref _maximum, newVal, nameof(Maximum));
                    if (_maximum < _minimum) Minimum = _maximum;
                    if (_value > _maximum) Value = _maximum;
                    _normalizing = false;
                    return;
                }
                SetProperty(ref _maximum, newVal, nameof(Maximum));
            }
        }

        [DisplayName("值")]
        [PropertySortOrder(4)]
        [FloatPrecision(0)]
        public double Value
        {
            get => _value;
            set
            {
                var newVal = value;
                if (!_normalizing)
                {
                    _normalizing = true;
                    if (newVal < _minimum) newVal = _minimum;
                    if (newVal > _maximum) newVal = _maximum;
                    SetProperty(ref _value, newVal, nameof(Value));
                    _normalizing = false;
                    return;
                }
                SetProperty(ref _value, newVal, nameof(Value));
            }
        }

        [DisplayName("不确定模式")]
        [PropertySortOrder(5)]
        public bool IsIndeterminate
        {
            get => _isIndeterminate;
            set => SetProperty(ref _isIndeterminate, value, nameof(IsIndeterminate));
        }

        [DisplayName("是否启用")]
        [PropertySortOrder(6)]
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value, nameof(IsEnabled));
        }

        [DisplayName("提示文字")]
        [PropertySortOrder(7)]
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
                    throw new Exception("对象值不是ProgressBarCommonInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是ProgressBarCommonInfo转换的");
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
            string oriStr = rootElement.TryGetProperty("Orientation", out value) ? value.GetString() : "Horizontal";
            Orientation = Enum.TryParse<Orientation>(oriStr, out var o) ? o : Orientation.Horizontal;
            Minimum = rootElement.TryGetProperty("Minimum", out value) ? value.GetDouble() : 0;
            Maximum = rootElement.TryGetProperty("Maximum", out value) ? value.GetDouble() : 100;
            Value = rootElement.TryGetProperty("Value", out value) ? value.GetDouble() : 0;
            IsIndeterminate = rootElement.TryGetProperty("IsIndeterminate", out value) ? value.GetBoolean() : false;
            IsEnabled = rootElement.TryGetProperty("IsEnabled", out value) ? value.GetBoolean() : true;
            ToolTip = rootElement.TryGetProperty("ToolTip", out value) ? value.GetString() : "";
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                Orientation = Orientation.ToString(),
                Minimum,
                Maximum,
                Value,
                IsIndeterminate,
                IsEnabled,
                ToolTip = ToolTip ?? ""
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        public override string ToString() => $"{Value}/{Maximum}";
    }
}
