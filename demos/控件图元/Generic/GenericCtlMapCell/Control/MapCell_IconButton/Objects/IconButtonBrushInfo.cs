using System;
using System.ComponentModel;
using Avalonia.Media;
using PropertyModels.ComponentModel;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GKG.Map.MapCell.Generic.IconButton
{
    /// <summary>
    /// 图标按钮画笔信息
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class IconButtonBrushInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        /// <summary>
        /// 对象ID
        /// </summary>
        public static readonly Guid Object_ID = new Guid("{1A2B3C4D-5E6F-7A8B-9C0D-E1F2A3B4C5D6}");

        /// <summary>
        /// 默认值
        /// </summary>
        public static readonly IconButtonBrushInfo Default = new IconButtonBrushInfo();

        private string _backgroundColorStr = Colors.Gray.ToColorString();
        private string _borderColorStr = Colors.Green.ToColorString();
        private string _foregroundColorStr = Colors.Black.ToColorString();
        private string _hoverBackgroundColorStr = Colors.Transparent.ToColorString();
        private string _hoverForegroundColorStr = Colors.Transparent.ToColorString();
        private string _clickForegroundColorStr = Colors.Transparent.ToColorString();
        private string _clickBackgroundColorStr = Colors.Transparent.ToColorString();
        private string _clickBorderColorLeftStr = Colors.Green.ToColorString();
        private string _clickBorderColorTopStr = Colors.Green.ToColorString();
        private string _clickBorderColorRightStr = Colors.Green.ToColorString();
        private string _clickBorderColorBottomStr = Colors.Green.ToColorString();

        /// <summary>
        /// 背景色
        /// </summary>
        [DisplayName("背景色")]
        [Category("画笔")]
        public Color BackgroundColor
        {
            get => Color.Parse(_backgroundColorStr);
            set => SetProperty(ref _backgroundColorStr, value.ToColorString(), nameof(BackgroundColor));
        }

        /// <summary>
        /// 边框颜色
        /// </summary>
        [DisplayName("边框颜色")]
        [Category("画笔")]
        public Color BorderColor
        {
            get => Color.Parse(_borderColorStr);
            set => SetProperty(ref _borderColorStr, value.ToColorString(), nameof(BorderColor));
        }

        /// <summary>
        /// 前景色
        /// </summary>
        [DisplayName("前景色")]
        [Category("画笔")]
        public Color ForegroundColor
        {
            get => Color.Parse(_foregroundColorStr);
            set => SetProperty(ref _foregroundColorStr, value.ToColorString(), nameof(ForegroundColor));
        }

        /// <summary>
        /// 鼠标悬停时背景色
        /// </summary>
        [DisplayName("鼠标悬停时背景色")]
        [Category("画笔")]
        public Color HoverBackgroundColor
        {
            get => Color.Parse(_hoverBackgroundColorStr);
            set => SetProperty(ref _hoverBackgroundColorStr, value.ToColorString(), nameof(HoverBackgroundColor));
        }

        /// <summary>
        /// 鼠标悬停时前景色
        /// </summary>
        [DisplayName("鼠标悬停时前景色")]
        [Category("画笔")]
        public Color HoverForegroundColor
        {
            get => Color.Parse(_hoverForegroundColorStr);
            set => SetProperty(ref _hoverForegroundColorStr, value.ToColorString(), nameof(HoverForegroundColor));
        }

        /// <summary>
        /// 按钮点击前景色
        /// </summary>
        [DisplayName("按钮点击前景色")]
        [Category("画笔")]
        public Color ClickForegroundColor
        {
            get => Color.Parse(_clickForegroundColorStr);
            set => SetProperty(ref _clickForegroundColorStr, value.ToColorString(), nameof(ClickForegroundColor));
        }

        /// <summary>
        /// 按钮点击背景色
        /// </summary>
        [DisplayName("按钮点击背景色")]
        [Category("画笔")]
        public Color ClickBackgroundColor
        {
            get => Color.Parse(_clickBackgroundColorStr);
            set => SetProperty(ref _clickBackgroundColorStr, value.ToColorString(), nameof(ClickBackgroundColor));
        }

        /// <summary>
        /// 按钮点击左边框颜色
        /// </summary>
        [DisplayName("按钮点击左边框颜色")]
        [Category("画笔")]
        public Color ClickBorderColorLeft
        {
            get => Color.Parse(_clickBorderColorLeftStr);
            set => SetProperty(ref _clickBorderColorLeftStr, value.ToColorString(), nameof(ClickBorderColorLeft));
        }

        /// <summary>
        /// 按钮点击上边框颜色
        /// </summary>
        [DisplayName("按钮点击上边框颜色")]
        [Category("画笔")]
        public Color ClickBorderColorTop
        {
            get => Color.Parse(_clickBorderColorTopStr);
            set => SetProperty(ref _clickBorderColorTopStr, value.ToColorString(), nameof(ClickBorderColorTop));
        }

        /// <summary>
        /// 按钮点击右边框颜色
        /// </summary>
        [DisplayName("按钮点击右边框颜色")]
        [Category("画笔")]
        public Color ClickBorderColorRight
        {
            get => Color.Parse(_clickBorderColorRightStr);
            set => SetProperty(ref _clickBorderColorRightStr, value.ToColorString(), nameof(ClickBorderColorRight));
        }

        /// <summary>
        /// 按钮点击下边框颜色
        /// </summary>
        [DisplayName("按钮点击下边框颜色")]
        [Category("画笔")]
        public Color ClickBorderColorBottom
        {
            get => Color.Parse(_clickBorderColorBottomStr);
            set => SetProperty(ref _clickBorderColorBottomStr, value.ToColorString(), nameof(ClickBorderColorBottom));
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
                    throw new Exception("对象值不是IconButtonBrushInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是IconButtonBrushInfo转换的");
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
            if (rootElement.TryGetProperty("BackgroundColor", out value))
                _backgroundColorStr = value.GetString() ?? Colors.Transparent.ToColorString();
            if (rootElement.TryGetProperty("BorderColor", out value))
                _borderColorStr = value.GetString() ?? Colors.Transparent.ToColorString();
            if (rootElement.TryGetProperty("ForegroundColor", out value))
                _foregroundColorStr = value.GetString() ?? Colors.Black.ToColorString();
            if (rootElement.TryGetProperty("HoverBackgroundColor", out value))
                _hoverBackgroundColorStr = value.GetString() ?? Colors.Transparent.ToColorString();
            if (rootElement.TryGetProperty("HoverForegroundColor", out value))
                _hoverForegroundColorStr = value.GetString() ?? Colors.Transparent.ToColorString();
            if (rootElement.TryGetProperty("ClickForegroundColor", out value))
                _clickForegroundColorStr = value.GetString() ?? Colors.Transparent.ToColorString();
            if (rootElement.TryGetProperty("ClickBackgroundColor", out value))
                _clickBackgroundColorStr = value.GetString() ?? Colors.Transparent.ToColorString();
            // 旧数据没有点击态边框颜色时，自动沿用普通边框颜色，避免升级后视觉突变。
            if (rootElement.TryGetProperty("ClickBorderColorLeft", out value))
                _clickBorderColorLeftStr = value.GetString() ?? _borderColorStr;
            else
                _clickBorderColorLeftStr = _borderColorStr;
            if (rootElement.TryGetProperty("ClickBorderColorTop", out value))
                _clickBorderColorTopStr = value.GetString() ?? _borderColorStr;
            else
                _clickBorderColorTopStr = _borderColorStr;
            if (rootElement.TryGetProperty("ClickBorderColorRight", out value))
                _clickBorderColorRightStr = value.GetString() ?? _borderColorStr;
            else
                _clickBorderColorRightStr = _borderColorStr;
            if (rootElement.TryGetProperty("ClickBorderColorBottom", out value))
                _clickBorderColorBottomStr = value.GetString() ?? _borderColorStr;
            else
                _clickBorderColorBottomStr = _borderColorStr;
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                BackgroundColor = _backgroundColorStr,
                BorderColor = _borderColorStr,
                ForegroundColor = _foregroundColorStr,
                HoverBackgroundColor = _hoverBackgroundColorStr,
                HoverForegroundColor = _hoverForegroundColorStr,
                ClickForegroundColor = _clickForegroundColorStr,
                ClickBackgroundColor = _clickBackgroundColorStr,
                ClickBorderColorLeft = _clickBorderColorLeftStr,
                ClickBorderColorTop = _clickBorderColorTopStr,
                ClickBorderColorRight = _clickBorderColorRightStr,
                ClickBorderColorBottom = _clickBorderColorBottomStr
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        #endregion
    }
}
