using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PropertyModels.ComponentModel;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GKG.Map.MapCell.Generic.IconButton
{
    /// <summary>
    /// 图标按钮外观信息
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class IconButtonAppearanceInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        /// <summary>
        /// 对象ID
        /// </summary>
        public static readonly Guid Object_ID = new Guid("{2A2B3C4D-5E6F-7A8B-9C0D-E1F2A3B4C5D7}");

        /// <summary>
        /// 默认值
        /// </summary>
        public static readonly IconButtonAppearanceInfo Default = new IconButtonAppearanceInfo();

        private int _opacity = 0;
        private int _borderThickness = 0;
        private int _borderThicknessLeft = 0;
        private int _borderThicknessTop = 0;
        private int _borderThicknessRight = 0;
        private int _borderThicknessBottom = 0;
        private int _clickBorderThicknessLeft = 0;
        private int _clickBorderThicknessTop = 0;
        private int _clickBorderThicknessRight = 0;
        private int _clickBorderThicknessBottom = 0;

        /// <summary>
        /// 透明度
        /// </summary>
        [DisplayName("透明度")]
        [Category("外观")]
        [Range(0, 100)]
        public int Opacity
        {
            get => _opacity;
            set => SetProperty(ref _opacity, Math.Max(0, Math.Min(100, value)));
        }

        /// <summary>
        /// 边框粗细
        /// </summary>
        [DisplayName("边框粗细")]
        [Category("外观")]
        [Range(0, 100)]
        public int BorderThickness
        {
            get => _borderThickness;
            set 
            {
                if (SetProperty(ref _borderThickness, value))
                {
                    BorderThicknessLeft = value;
                    BorderThicknessTop = value;
                    BorderThicknessRight = value;
                    BorderThicknessBottom = value;
                }
            }
        }

        /// <summary>
        /// 左边框粗细
        /// </summary>
        [DisplayName("左")]
        [Category("外观")]
        [Range(0, 100)]
        public int BorderThicknessLeft
        {
            get => _borderThicknessLeft;
            set => SetProperty(ref _borderThicknessLeft, value);
        }

        /// <summary>
        /// 上边框粗细
        /// </summary>
        [DisplayName("上")]
        [Category("外观")]
        [Range(0, 100)]
        public int BorderThicknessTop
        {
            get => _borderThicknessTop;
            set => SetProperty(ref _borderThicknessTop, value);
        }

        /// <summary>
        /// 右边框粗细
        /// </summary>
        [DisplayName("右")]
        [Category("外观")]
        [Range(0, 100)]
        public int BorderThicknessRight
        {
            get => _borderThicknessRight;
            set => SetProperty(ref _borderThicknessRight, value);
        }

        /// <summary>
        /// 下边框粗细
        /// </summary>
        [DisplayName("下")]
        [Category("外观")]
        [Range(0, 100)]
        public int BorderThicknessBottom
        {
            get => _borderThicknessBottom;
            set => SetProperty(ref _borderThicknessBottom, value);
        }

        /// <summary>
        /// 按钮点击左边框粗细
        /// </summary>
        [DisplayName("点击左")]
        [Category("外观")]
        [Range(0, 100)]
        public int ClickBorderThicknessLeft
        {
            get => _clickBorderThicknessLeft;
            set => SetProperty(ref _clickBorderThicknessLeft, value);
        }

        /// <summary>
        /// 按钮点击上边框粗细
        /// </summary>
        [DisplayName("点击上")]
        [Category("外观")]
        [Range(0, 100)]
        public int ClickBorderThicknessTop
        {
            get => _clickBorderThicknessTop;
            set => SetProperty(ref _clickBorderThicknessTop, value);
        }

        /// <summary>
        /// 按钮点击右边框粗细
        /// </summary>
        [DisplayName("点击右")]
        [Category("外观")]
        [Range(0, 100)]
        public int ClickBorderThicknessRight
        {
            get => _clickBorderThicknessRight;
            set => SetProperty(ref _clickBorderThicknessRight, value);
        }

        /// <summary>
        /// 按钮点击下边框粗细
        /// </summary>
        [DisplayName("点击下")]
        [Category("外观")]
        [Range(0, 100)]
        public int ClickBorderThicknessBottom
        {
            get => _clickBorderThicknessBottom;
            set => SetProperty(ref _clickBorderThicknessBottom, value);
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
                    throw new Exception("对象值不是IconButtonAppearanceInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是IconButtonAppearanceInfo转换的");
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
            if (rootElement.TryGetProperty("Opacity", out value))
                _opacity = value.GetInt32();
            if (rootElement.TryGetProperty("BorderThickness", out value))
                _borderThickness = value.GetInt32();
            if (rootElement.TryGetProperty("BorderThicknessLeft", out value))
                _borderThicknessLeft = value.GetInt32();
            if (rootElement.TryGetProperty("BorderThicknessTop", out value))
                _borderThicknessTop = value.GetInt32();
            if (rootElement.TryGetProperty("BorderThicknessRight", out value))
                _borderThicknessRight = value.GetInt32();
            if (rootElement.TryGetProperty("BorderThicknessBottom", out value))
                _borderThicknessBottom = value.GetInt32();
            // 旧数据没有点击态边框粗细时，自动沿用普通边框粗细，避免升级后视觉突变。
            if (rootElement.TryGetProperty("ClickBorderThicknessLeft", out value))
                _clickBorderThicknessLeft = value.GetInt32();
            else
                _clickBorderThicknessLeft = _borderThicknessLeft;
            if (rootElement.TryGetProperty("ClickBorderThicknessTop", out value))
                _clickBorderThicknessTop = value.GetInt32();
            else
                _clickBorderThicknessTop = _borderThicknessTop;
            if (rootElement.TryGetProperty("ClickBorderThicknessRight", out value))
                _clickBorderThicknessRight = value.GetInt32();
            else
                _clickBorderThicknessRight = _borderThicknessRight;
            if (rootElement.TryGetProperty("ClickBorderThicknessBottom", out value))
                _clickBorderThicknessBottom = value.GetInt32();
            else
                _clickBorderThicknessBottom = _borderThicknessBottom;
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                Opacity = _opacity,
                BorderThickness = _borderThickness,
                BorderThicknessLeft = _borderThicknessLeft,
                BorderThicknessTop = _borderThicknessTop,
                BorderThicknessRight = _borderThicknessRight,
                BorderThicknessBottom = _borderThicknessBottom,
                ClickBorderThicknessLeft = _clickBorderThicknessLeft,
                ClickBorderThicknessTop = _clickBorderThicknessTop,
                ClickBorderThicknessRight = _clickBorderThicknessRight,
                ClickBorderThicknessBottom = _clickBorderThicknessBottom
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        #endregion
    }
}
