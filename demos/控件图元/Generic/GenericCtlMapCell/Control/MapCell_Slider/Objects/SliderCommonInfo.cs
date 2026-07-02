using System;
using System.ComponentModel;
using PropertyModels.ComponentModel;
using System.Text.Json;
using GKG.Map.MapCell.Generic.Control.MapCell_TextBox;
using GF_Gereric;
using Griffins;
using Avalonia.Controls;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Slider
{
    /// <summary>
    /// 滑块公共信息
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class SliderCommonInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        /// <summary>
        /// 对象ID
        /// </summary>
        public static readonly Guid Object_ID = new Guid("{3A2B3C4D-5E6F-7A8B-9C0D-E1F2A3B4C5D8}");

        /// <summary>
        /// 默认值
        /// </summary>
        public static readonly SliderCommonInfo Default = new SliderCommonInfo();

        private int _maximum = 100;
        private int _minimum = 0;
        private DirectionEnum _direction = DirectionEnum.水平;
        private int _smallChange = 1;
        private int _stepOffsetFromMinimum = 0;
        private int _value = 50;
        private int _tickFrequency = 10;
        private TickPlacement _tickPlacement = TickPlacement.None;
        private CommonCursorType _hoverCursor = CommonCursorType.手型;
        private bool _enabled = true;
        private string _tooltipText = "";

        private static int PositiveMod(int value, int modulus)
        {
            if (modulus <= 0)
                return 0;
            int result = value % modulus;
            return result < 0 ? result + modulus : result;
        }

        private int GetStepSize() => _smallChange <= 0 ? 1 : _smallChange;

        private int SnapToStep(int value)
        {
            int step = GetStepSize();
            
            // 步长为1时，直接返回值
            if (step <= 1)
            {
                return value;
            }
            
            // 计算从最小值到目标值的差值
            int diff = value - _minimum;
            
            // 计算完整的步数和余数
            int fullSteps = diff / step;
            int remainder = diff % step;
            
            int snapped;
            
            // 当目标值等于当前值时，直接返回当前值
            if (value == _value)
            {
                return _value;
            }
            // 根据当前值和目标值的关系来决定是向上还是向下对齐
            else if (value > _value)
            {
                // 当目标值大于当前值时（用户在增加值），计算下一个步长位置
                snapped = _value + step;
            }
            else
            {
                // 当目标值小于当前值时（用户在减少值），计算上一个步长位置
                snapped = _value - step;
            }

            // 确保对齐后的值在最小值和最大值之间
            if (snapped < _minimum)
                snapped = _minimum;
            else if (snapped > _maximum)
                snapped = _maximum;
            return snapped;
        }

        /// <summary>
        /// 最大值
        /// </summary>
        [DisplayName("最大值")]
        [Category("公共")]
        public int Maximum
        {
            get => _maximum;
            set 
            {
                // 最大值不能小于等于0且不能小于最小值
                if (value <= 0)
                    value = 1;
                if (value <= _minimum)
                    value = _minimum + 1;
                if (SetProperty(ref _maximum, value))
                {
                    if (_value > _maximum)
                    {
                        int newValue = SnapToStep(_maximum);
                        SetProperty(ref _value, newValue, nameof(Value));
                    }
                    _stepOffsetFromMinimum = PositiveMod(_value - _minimum, GetStepSize());
                }
            }
        }

        /// <summary>
        /// 最小值
        /// </summary>
        [DisplayName("最小值")]
        [Category("公共")]
        public int Minimum
        {
            get => _minimum;
            set 
            {
                // 最小值不能小于0且不能大于最大值
                if (value < 0)
                    value = 0;
                if (value >= _maximum)
                    value = _maximum - 1;
                if (SetProperty(ref _minimum, value))
                {
                    if (_value < _minimum)
                    {
                        int newValue = SnapToStep(_minimum);
                        SetProperty(ref _value, newValue, nameof(Value));
                    }
                    _stepOffsetFromMinimum = PositiveMod(_value - _minimum, GetStepSize());
                }
            }
        }

        /// <summary>
        /// 方向
        /// </summary>
        [DisplayName("方向")]
        [Category("公共")]
        public DirectionEnum Direction
        {
            get => _direction;
            set => SetProperty(ref _direction, value);
        }

        /// <summary>
        /// 方向枚举
        /// </summary>
        public enum DirectionEnum
        {
            /// <summary>
            /// 水平方向
            /// </summary>
            水平,
            /// <summary>
            /// 垂直方向
            /// </summary>
            垂直
        }

        /// <summary>
        /// 最小变化量
        /// </summary>
        [DisplayName("最小变化量")]
        [Category("公共")]
        public int SmallChange
        {
            get => _smallChange;
            set
            {
                if (value <= 0)
                    value = 1;
                if (SetProperty(ref _smallChange, value))
                {
                    _stepOffsetFromMinimum = PositiveMod(_value - _minimum, GetStepSize());
                }
            }
        }

        /// <summary>
        /// 值
        /// </summary>
        [DisplayName("值")]
        [Category("公共")]
        public int Value
        {
            get => _value;
            set 
            {
                // 确保值在最小值和最大值之间
                if (value < _minimum)
                    value = _minimum;
                else if (value > _maximum)
                    value = _maximum;
                value = SnapToStep(value);
                if (SetProperty(ref _value, value))
                {
                    // 当值改变时，重新计算stepOffset，确保基于当前值对齐
                    _stepOffsetFromMinimum = PositiveMod(_value - _minimum, GetStepSize());
                }
            }
        }

        /// <summary>
        /// 刻度线间隔
        /// </summary>
        [DisplayName("刻度线间隔")]
        [Category("公共")]
        public int TickFrequency
        {
            get => _tickFrequency;
            set
            {
                // 刻度线间隔小于等于 0 时没有意义，这里统一收口到 1。
                if (value <= 0)
                    value = 1;
                SetProperty(ref _tickFrequency, value);
            }
        }

        /// <summary>
        /// 刻度线相对于滑块的位置
        /// </summary>
        [DisplayName("刻度线相对位置")]
        [Category("公共")]
        public TickPlacement TickPlacement
        {
            get => _tickPlacement;
            set => SetProperty(ref _tickPlacement, value);
        }

        /// <summary>
        /// 鼠标悬停的光标
        /// </summary>
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
                    throw new Exception("对象值不是SliderCommonInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是SliderCommonInfo转换的");
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
            if (rootElement.TryGetProperty("Maximum", out value))
                _maximum = (int)value.GetDouble();
            if (rootElement.TryGetProperty("Minimum", out value))
                _minimum = (int)value.GetDouble();
            if (rootElement.TryGetProperty("Direction", out value))
            {
                string directionStr = value.GetString() ?? "水平";
                if (directionStr == "水平")
                    _direction = DirectionEnum.水平;
                else if (directionStr == "垂直")
                    _direction = DirectionEnum.垂直;
                else
                    _direction = DirectionEnum.水平;
            }
            if (rootElement.TryGetProperty("SmallChange", out value))
                _smallChange = (int)value.GetDouble();
            if (rootElement.TryGetProperty("Value", out value))
                _value = (int)value.GetDouble();
            if (rootElement.TryGetProperty("TickFrequency", out value))
                _tickFrequency = (int)value.GetDouble();
            if (rootElement.TryGetProperty("TickPlacement", out value))
            {
                string tickPlacementStr = value.GetString() ?? nameof(TickPlacement.None);
                if (!Enum.TryParse(tickPlacementStr, true, out _tickPlacement))
                    _tickPlacement = TickPlacement.None;
            }
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
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                Maximum = _maximum,
                Minimum = _minimum,
                Direction = _direction.ToString(),
                SmallChange = _smallChange,
                Value = _value,
                TickFrequency = _tickFrequency,
                TickPlacement = _tickPlacement.ToString(),
                HoverCursor = _hoverCursor.ToString(),
                Enabled = _enabled,
                TooltipText = _tooltipText
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        #endregion
    }
}
