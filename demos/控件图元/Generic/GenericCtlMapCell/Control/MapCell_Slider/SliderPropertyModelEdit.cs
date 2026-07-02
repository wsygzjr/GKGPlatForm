using System;
using System.ComponentModel;
using Avalonia.Media;
using PropertyModels.ComponentModel;
using GKG.Map.MapCell.Generic.Control.MapCell_TextBox;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Slider
{
    /// <summary>
    /// 滑块属性编辑模型
    /// </summary>
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("图元信息", 1)]
    [CategoryPriority("画笔", 2)]
    [CategoryPriority("外观", 3)]
    [CategoryPriority("公共", 4)]
    [CategoryPriority("布局", 5)]
    public class SliderPropertyModelEdit : ControlCellPropertyModelEdit
    {
        private SliderBrushInfo _brushInfo = new SliderBrushInfo();
        private SliderAppearanceInfo _appearanceInfo = new SliderAppearanceInfo();
        private SliderCommonInfo _commonInfo = new SliderCommonInfo();
        private SliderLayoutInfo _layoutInfo = new SliderLayoutInfo();

        /// <summary>
        /// 画笔设置
        /// </summary>
        [DisplayName("画笔设置")]
        [Category("画笔")]
        [PropertySortOrder(1)]
        public SliderBrushInfo BrushInfo
        {
            get => _brushInfo;
            set => SetProperty(ref _brushInfo, value);
        }

        /// <summary>
        /// 外观设置
        /// </summary>
        [DisplayName("外观设置")]
        [Category("外观")]
        [PropertySortOrder(1)]
        public SliderAppearanceInfo AppearanceInfo
        {
            get => _appearanceInfo;
            set => SetProperty(ref _appearanceInfo, value);
        }

        /// <summary>
        /// 公共设置
        /// </summary>
        [DisplayName("公共设置")]
        [Category("公共")]
        [PropertySortOrder(1)]
        public SliderCommonInfo CommonInfo
        {
            get => _commonInfo;
            set => SetProperty(ref _commonInfo, value);
        }

        /// <summary>
        /// 布局设置
        /// </summary>
        [DisplayName("布局设置")]
        [Category("布局")]
        [PropertySortOrder(1)]
        public SliderLayoutInfo LayoutInfo
        {
            get => _layoutInfo;
            set => SetProperty(ref _layoutInfo, value);
        }

        /// <summary>
        /// 是否可见
        /// </summary>
        #region SetPropertyValue 方法

        public override bool SetPropertyValue(string propertyID, MapCellPropValue propertyVal)
        {
            if (string.Compare(propertyID, nameof(SliderCommonInfo.Value)) == 0)
            {
                _commonInfo ??= new SliderCommonInfo();
                _commonInfo.Value = propertyVal != null ? (int)propertyVal.ToPrimitiveValue<decimal>() : SliderCommonInfo.Default.Value;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(SliderCommonInfo.TickFrequency)) == 0)
            {
                _commonInfo ??= new SliderCommonInfo();
                _commonInfo.TickFrequency = propertyVal != null ? (int)propertyVal.ToPrimitiveValue<decimal>() : SliderCommonInfo.Default.TickFrequency;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(SliderCommonInfo.TickPlacement)) == 0)
            {
                _commonInfo ??= new SliderCommonInfo();
                if (propertyVal != null)
                {
                    var placementStr = propertyVal.ToPrimitiveValue<string>();
                    _commonInfo.TickPlacement = Enum.TryParse<Avalonia.Controls.TickPlacement>(placementStr, out var placement)
                        ? placement
                        : SliderCommonInfo.Default.TickPlacement;
                }
                else
                {
                    _commonInfo.TickPlacement = SliderCommonInfo.Default.TickPlacement;
                }
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(SliderCommonInfo.Minimum)) == 0)
            {
                _commonInfo ??= new SliderCommonInfo();
                _commonInfo.Minimum = propertyVal != null ? (int)propertyVal.ToPrimitiveValue<decimal>() : SliderCommonInfo.Default.Minimum;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(SliderCommonInfo.Maximum)) == 0)
            {
                _commonInfo ??= new SliderCommonInfo();
                _commonInfo.Maximum = propertyVal != null ? (int)propertyVal.ToPrimitiveValue<decimal>() : SliderCommonInfo.Default.Maximum;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(SliderCommonInfo.SmallChange)) == 0)
            {
                _commonInfo ??= new SliderCommonInfo();
                _commonInfo.SmallChange = propertyVal != null ? (int)propertyVal.ToPrimitiveValue<decimal>() : SliderCommonInfo.Default.SmallChange;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(SliderCommonInfo.Enabled)) == 0)
            {
                _commonInfo ??= new SliderCommonInfo();
                _commonInfo.Enabled = propertyVal != null ? propertyVal.ToPrimitiveValue<bool>() : SliderCommonInfo.Default.Enabled;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(SliderCommonInfo.TooltipText)) == 0)
            {
                _commonInfo ??= new SliderCommonInfo();
                _commonInfo.TooltipText = propertyVal != null ? propertyVal.ToPrimitiveValue<string>() : SliderCommonInfo.Default.TooltipText;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(SliderCommonInfo.Direction)) == 0)
            {
                _commonInfo ??= new SliderCommonInfo();
                if (propertyVal != null)
                {
                    var dirStr = propertyVal.ToPrimitiveValue<string>();
                    _commonInfo.Direction = Enum.TryParse<SliderCommonInfo.DirectionEnum>(dirStr, out var d) ? d : SliderCommonInfo.Default.Direction;
                }
                else
                {
                    _commonInfo.Direction = SliderCommonInfo.Default.Direction;
                }
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(SliderCommonInfo.HoverCursor)) == 0)
            {
                _commonInfo ??= new SliderCommonInfo();
                if (propertyVal != null)
                {
                    var cursorStr = propertyVal.ToPrimitiveValue<string>();
                    _commonInfo.HoverCursor = Enum.TryParse<CommonCursorType>(cursorStr, out var c) ? c : SliderCommonInfo.Default.HoverCursor;
                }
                else
                {
                    _commonInfo.HoverCursor = SliderCommonInfo.Default.HoverCursor;
                }
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }

            if (string.Compare(propertyID, nameof(SliderBrushInfo.BackgroundColor)) == 0 || string.Compare(propertyID, "BackgroundColor") == 0)
            {
                _brushInfo ??= new SliderBrushInfo();
                if (propertyVal != null)
                {
                    var colorStr = propertyVal.ToPrimitiveValue<string>();
                    _brushInfo.BackgroundColor = Color.Parse(colorStr);
                }
                else
                {
                    _brushInfo.BackgroundColor = SliderBrushInfo.Default.BackgroundColor;
                }
                RaisePropertyChanged(nameof(BrushInfo));
                return true;
            }

            if (string.Compare(propertyID, nameof(SliderAppearanceInfo.Opacity)) == 0)
            {
                _appearanceInfo ??= new SliderAppearanceInfo();
                _appearanceInfo.Opacity = propertyVal != null ? (int)propertyVal.ToPrimitiveValue<decimal>() : SliderAppearanceInfo.Default.Opacity;
                RaisePropertyChanged(nameof(AppearanceInfo));
                return true;
            }

            if (string.Compare(propertyID, nameof(BrushInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<SliderBrushInfo>(propertyVal) : new SliderBrushInfo();
                _brushInfo ??= new SliderBrushInfo();
                _brushInfo.BackgroundColor = src.BackgroundColor;
                RaisePropertyChanged(nameof(BrushInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(AppearanceInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<SliderAppearanceInfo>(propertyVal) : new SliderAppearanceInfo();
                _appearanceInfo ??= new SliderAppearanceInfo();
                _appearanceInfo.Opacity = src.Opacity;
                RaisePropertyChanged(nameof(AppearanceInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(CommonInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<SliderCommonInfo>(propertyVal) : new SliderCommonInfo();
                _commonInfo ??= new SliderCommonInfo();
                _commonInfo.Maximum = src.Maximum;
                _commonInfo.Minimum = src.Minimum;
                _commonInfo.Direction = src.Direction;
                _commonInfo.SmallChange = src.SmallChange;
                _commonInfo.Value = src.Value;
                _commonInfo.TickFrequency = src.TickFrequency;
                _commonInfo.TickPlacement = src.TickPlacement;
                _commonInfo.HoverCursor = src.HoverCursor;
                _commonInfo.Enabled = src.Enabled;
                _commonInfo.TooltipText = src.TooltipText;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(LayoutInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<SliderLayoutInfo>(propertyVal) : new SliderLayoutInfo();
                _layoutInfo ??= new SliderLayoutInfo();
                _layoutInfo.HorizontalAlignment = src.HorizontalAlignment;
                _layoutInfo.VerticalAlignment = src.VerticalAlignment;
                _layoutInfo.MarginLeft = src.MarginLeft;
                _layoutInfo.MarginTop = src.MarginTop;
                _layoutInfo.MarginRight = src.MarginRight;
                _layoutInfo.MarginBottom = src.MarginBottom;
                _layoutInfo.MinWidth = src.MinWidth;
                _layoutInfo.MaxWidth = src.MaxWidth;
                _layoutInfo.MinHeight = src.MinHeight;
                _layoutInfo.MaxHeight = src.MaxHeight;
                RaisePropertyChanged(nameof(LayoutInfo));
                return true;
            }
            return base.SetPropertyValue(propertyID, propertyVal);
        }

        private static T DeserializeObject<T>(MapCellPropValue val) where T : IMPPropObjectValue, new()
        {
            ObjectValue_Json objectValue_Json = val.ToObjectValue_Json();
            GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
            IMPPropObjectValue obj = new T();
            obj.PopulateFromBaseValue(griffinsBaseValue);
            return (T)obj;
        }

        #endregion

        #region CopyFrom 方法

        public void CopyFrom(SliderPropertyModelEdit source)
        {
            if (source == null) return;

            // 复制画笔信息
            _brushInfo ??= new SliderBrushInfo();
            _brushInfo.BackgroundColor = source.BrushInfo?.BackgroundColor ?? _brushInfo.BackgroundColor;
            RaisePropertyChanged(nameof(BrushInfo));

            // 复制外观信息
            _appearanceInfo ??= new SliderAppearanceInfo();
            _appearanceInfo.Opacity = source.AppearanceInfo?.Opacity ?? 100;
            RaisePropertyChanged(nameof(AppearanceInfo));

            // 复制公共信息
            _commonInfo ??= new SliderCommonInfo();
            _commonInfo.Maximum = source.CommonInfo?.Maximum ?? 100;
            _commonInfo.Minimum = source.CommonInfo?.Minimum ?? 0;
            _commonInfo.Direction = source.CommonInfo?.Direction ?? SliderCommonInfo.DirectionEnum.水平;
            _commonInfo.SmallChange = source.CommonInfo?.SmallChange ?? 1;
            _commonInfo.Value = source.CommonInfo?.Value ?? 50;
            _commonInfo.TickFrequency = source.CommonInfo?.TickFrequency ?? 10;
            _commonInfo.TickPlacement = source.CommonInfo?.TickPlacement ?? Avalonia.Controls.TickPlacement.None;
            _commonInfo.HoverCursor = source.CommonInfo?.HoverCursor ?? CommonCursorType.手型;
            _commonInfo.Enabled = source.CommonInfo?.Enabled ?? true;
            _commonInfo.TooltipText = source.CommonInfo?.TooltipText ?? "";
            RaisePropertyChanged(nameof(CommonInfo));

            // 复制布局信息
            _layoutInfo ??= new SliderLayoutInfo();
            // 宽高主数据统一走父类 Width/Height，LayoutInfo 不再承载宽高。
            Width = source.Width;
            Height = source.Height;
            // 新建或缺省布局时，滑块默认使用居中对齐，避免一落到画布上就被 Stretch 撑满。
            _layoutInfo.HorizontalAlignment = source.LayoutInfo?.HorizontalAlignment ?? SliderLayoutInfo.HorizontalAlignmentEnum.Center;
            _layoutInfo.VerticalAlignment = source.LayoutInfo?.VerticalAlignment ?? SliderLayoutInfo.VerticalAlignmentEnum.Center;
            _layoutInfo.Margin = source.LayoutInfo?.Margin ?? 0;
            _layoutInfo.MarginLeft = source.LayoutInfo?.MarginLeft ?? 0;
            _layoutInfo.MarginTop = source.LayoutInfo?.MarginTop ?? 0;
            _layoutInfo.MarginRight = source.LayoutInfo?.MarginRight ?? 0;
            _layoutInfo.MarginBottom = source.LayoutInfo?.MarginBottom ?? 0;
            _layoutInfo.MinWidth = source.LayoutInfo?.MinWidth ?? 50;
            _layoutInfo.MaxWidth = source.LayoutInfo?.MaxWidth ?? int.MaxValue;
            _layoutInfo.MinHeight = source.LayoutInfo?.MinHeight ?? 20;
            _layoutInfo.MaxHeight = source.LayoutInfo?.MaxHeight ?? int.MaxValue;
            RaisePropertyChanged(nameof(LayoutInfo));
        }

        #endregion
    }
}
