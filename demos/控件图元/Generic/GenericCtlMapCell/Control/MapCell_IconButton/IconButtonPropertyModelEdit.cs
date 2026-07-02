using System;
using System.ComponentModel;

using PropertyModels.ComponentModel;
using GKG.Map.MapCell.Generic.Control.MapCell_TextBox;
using Griffins;
using Avalonia.Media;

namespace GKG.Map.MapCell.Generic.IconButton
{
    /// <summary>
    /// 图标按钮属性编辑模型
    /// </summary>
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("图元信息", 1)]
    [CategoryPriority("画笔", 2)]
    [CategoryPriority("外观", 3)]
    [CategoryPriority("公共", 4)]
    [CategoryPriority("布局", 5)]
    [CategoryPriority("文本", 6)]
    [CategoryPriority("杂项", 7)]
    public class IconButtonPropertyModelEdit : ControlCellPropertyModelEdit
    {
        private IconButtonBrushInfo _brushInfo = new IconButtonBrushInfo();
        private IconButtonAppearanceInfo _appearanceInfo = new IconButtonAppearanceInfo();
        private IconButtonCommonInfo _commonInfo = new IconButtonCommonInfo();
        private IconButtonLayoutInfo _layoutInfo = new IconButtonLayoutInfo();
        private IconButtonFontInfo _fontInfo = new IconButtonFontInfo();
        private IconButtonParagraphInfo _paragraphInfo = new IconButtonParagraphInfo();
        private IconButtonMiscInfo _miscInfo = new IconButtonMiscInfo();

        /// <summary>
        /// 画笔设置
        /// </summary>
        [DisplayName("画笔设置")]
        [Category("画笔")]
        [PropertySortOrder(1)]
        public IconButtonBrushInfo BrushInfo
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
        public IconButtonAppearanceInfo AppearanceInfo
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
        public IconButtonCommonInfo CommonInfo
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
        public IconButtonLayoutInfo LayoutInfo
        {
            get => _layoutInfo;
            set => SetProperty(ref _layoutInfo, value);
        }

        /// <summary>
        /// 字体设置
        /// </summary>
        [DisplayName("字体设置")]
        [Category("文本")]
        [PropertySortOrder(1)]
        public IconButtonFontInfo FontInfo
        {
            get => _fontInfo;
            set => SetProperty(ref _fontInfo, value);
        }

        /// <summary>
        /// 段落设置
        /// </summary>
        [DisplayName("段落设置")]
        [Category("文本")]
        [PropertySortOrder(2)]
        public IconButtonParagraphInfo ParagraphInfo
        {
            get => _paragraphInfo;
            set => SetProperty(ref _paragraphInfo, value);
        }

        /// <summary>
        /// 杂项设置
        /// </summary>
        [DisplayName("杂项设置")]
        [Category("杂项")]
        [PropertySortOrder(1)]
        public IconButtonMiscInfo MiscInfo
        {
            get => _miscInfo;
            set => SetProperty(ref _miscInfo, value);
        }

        /// <summary>
        /// 是否可见
        /// </summary>
        #region SetPropertyValue 方法

        public override bool SetPropertyValue(string propertyID, MapCellPropValue propertyVal)
        {
            if (string.Compare(propertyID, nameof(IconButtonBrushInfo.BackgroundColor)) == 0)
            {
                _brushInfo ??= new IconButtonBrushInfo();
                _brushInfo.BackgroundColor = propertyVal != null
                    ? Color.Parse(propertyVal.ToPrimitiveValue<string>())
                    : IconButtonBrushInfo.Default.BackgroundColor;
                RaisePropertyChanged(nameof(BrushInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(IconButtonBrushInfo.BorderColor)) == 0)
            {
                _brushInfo ??= new IconButtonBrushInfo();
                _brushInfo.BorderColor = propertyVal != null
                    ? Color.Parse(propertyVal.ToPrimitiveValue<string>())
                    : IconButtonBrushInfo.Default.BorderColor;
                RaisePropertyChanged(nameof(BrushInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(IconButtonBrushInfo.ForegroundColor)) == 0)
            {
                _brushInfo ??= new IconButtonBrushInfo();
                _brushInfo.ForegroundColor = propertyVal != null
                    ? Color.Parse(propertyVal.ToPrimitiveValue<string>())
                    : IconButtonBrushInfo.Default.ForegroundColor;
                RaisePropertyChanged(nameof(BrushInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(IconButtonAppearanceInfo.Opacity)) == 0)
            {
                _appearanceInfo ??= new IconButtonAppearanceInfo();
                _appearanceInfo.Opacity = propertyVal != null
                    ? propertyVal.ToPrimitiveValue<int>()
                    : IconButtonAppearanceInfo.Default.Opacity;
                RaisePropertyChanged(nameof(AppearanceInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(IconButtonCommonInfo.ButtonText)) == 0)
            {
                _commonInfo ??= new IconButtonCommonInfo();
                _commonInfo.ButtonText = propertyVal != null
                    ? propertyVal.ToPrimitiveValue<string>()
                    : IconButtonCommonInfo.Default.ButtonText;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(IconButtonCommonInfo.GroupId)) == 0)
            {
                _commonInfo ??= new IconButtonCommonInfo();
                _commonInfo.GroupId = propertyVal != null
                    ? propertyVal.ToPrimitiveValue<string>()
                    : IconButtonCommonInfo.Default.GroupId;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(BrushInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<IconButtonBrushInfo>(propertyVal) : new IconButtonBrushInfo();
                _brushInfo ??= new IconButtonBrushInfo();
                _brushInfo.BackgroundColor = src.BackgroundColor;
                _brushInfo.BorderColor = src.BorderColor;
                _brushInfo.ForegroundColor = src.ForegroundColor;
                _brushInfo.HoverBackgroundColor = src.HoverBackgroundColor;
                _brushInfo.HoverForegroundColor = src.HoverForegroundColor;
                _brushInfo.ClickForegroundColor = src.ClickForegroundColor;
                _brushInfo.ClickBackgroundColor = src.ClickBackgroundColor;
                // 点击态支持四边独立边框颜色，这里需要和整组画笔一起同步。
                _brushInfo.ClickBorderColorLeft = src.ClickBorderColorLeft;
                _brushInfo.ClickBorderColorTop = src.ClickBorderColorTop;
                _brushInfo.ClickBorderColorRight = src.ClickBorderColorRight;
                _brushInfo.ClickBorderColorBottom = src.ClickBorderColorBottom;
                RaisePropertyChanged(nameof(BrushInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(AppearanceInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<IconButtonAppearanceInfo>(propertyVal) : new IconButtonAppearanceInfo();
                _appearanceInfo ??= new IconButtonAppearanceInfo();
                _appearanceInfo.Opacity = src.Opacity;
                _appearanceInfo.BorderThickness = src.BorderThickness;
                _appearanceInfo.BorderThicknessLeft = src.BorderThicknessLeft;
                _appearanceInfo.BorderThicknessTop = src.BorderThicknessTop;
                _appearanceInfo.BorderThicknessRight = src.BorderThicknessRight;
                _appearanceInfo.BorderThicknessBottom = src.BorderThicknessBottom;
                // 点击态支持四边独立边框粗细，这里需要和整组外观一起同步。
                _appearanceInfo.ClickBorderThicknessLeft = src.ClickBorderThicknessLeft;
                _appearanceInfo.ClickBorderThicknessTop = src.ClickBorderThicknessTop;
                _appearanceInfo.ClickBorderThicknessRight = src.ClickBorderThicknessRight;
                _appearanceInfo.ClickBorderThicknessBottom = src.ClickBorderThicknessBottom;
                RaisePropertyChanged(nameof(AppearanceInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(LayoutInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<IconButtonLayoutInfo>(propertyVal) : new IconButtonLayoutInfo();
                _layoutInfo ??= new IconButtonLayoutInfo();
                // 宽高主数据统一迁到父类 Width/Height，LayoutInfo 不再承载宽高。
                _layoutInfo.HorizontalAlignment = src.HorizontalAlignment;
                _layoutInfo.VerticalAlignment = src.VerticalAlignment;
                _layoutInfo.Margin = src.Margin;
                _layoutInfo.MarginLeft = src.MarginLeft;
                _layoutInfo.MarginTop = src.MarginTop;
                _layoutInfo.MarginRight = src.MarginRight;
                _layoutInfo.MarginBottom = src.MarginBottom;
                RaisePropertyChanged(nameof(LayoutInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(CommonInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<IconButtonCommonInfo>(propertyVal) : new IconButtonCommonInfo();
                _commonInfo ??= new IconButtonCommonInfo();
                _commonInfo.ButtonText = src.ButtonText;
                _commonInfo.HoverCursor = src.HoverCursor;
                _commonInfo.Enabled = src.Enabled;
                _commonInfo.TooltipText = src.TooltipText;
                _commonInfo.GroupId = src.GroupId;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(FontInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<IconButtonFontInfo>(propertyVal) : new IconButtonFontInfo();
                _fontInfo ??= new IconButtonFontInfo();
                _fontInfo.FontColor = src.FontColor;
                _fontInfo.FontSize = src.FontSize;
                _fontInfo.IsBold = src.IsBold;
                _fontInfo.IsItalic = src.IsItalic;
                _fontInfo.IsUnderline = src.IsUnderline;
                RaisePropertyChanged(nameof(FontInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(ParagraphInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<IconButtonParagraphInfo>(propertyVal) : new IconButtonParagraphInfo();
                _paragraphInfo ??= new IconButtonParagraphInfo();
                _paragraphInfo.LineHeight = src.LineHeight;
                _paragraphInfo.ParagraphSpacingBefore = src.ParagraphSpacingBefore;
                _paragraphInfo.ParagraphSpacingAfter = src.ParagraphSpacingAfter;
                _paragraphInfo.TextAlignment = src.TextAlignment;
                RaisePropertyChanged(nameof(ParagraphInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(MiscInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<IconButtonMiscInfo>(propertyVal) : new IconButtonMiscInfo();
                _miscInfo ??= new IconButtonMiscInfo();
                // 确保IconSource不会被设置为null，避免ControlObjectRunTimeHost.Refresh()方法中的NullReferenceException
                if (src.IconSource != null)
                {
                    _miscInfo.IconSource = src.IconSource;
                }
                _miscInfo.IconWidth = src.IconWidth;
                _miscInfo.IconHeight = src.IconHeight;
                _miscInfo.IconMargin = src.IconMargin;
                _miscInfo.IconMarginLeft = src.IconMarginLeft;
                _miscInfo.IconMarginTop = src.IconMarginTop;
                _miscInfo.IconMarginRight = src.IconMarginRight;
                _miscInfo.IconMarginBottom = src.IconMarginBottom;
                _miscInfo.IconPosition = src.IconPosition;
                RaisePropertyChanged(nameof(MiscInfo));
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

        public void CopyFrom(IconButtonPropertyModelEdit source)
        {
            if (source == null) return;

            // 复制画笔信息
            _brushInfo ??= new IconButtonBrushInfo();
            _brushInfo.BackgroundColor = source.BrushInfo?.BackgroundColor ?? _brushInfo.BackgroundColor;
            _brushInfo.BorderColor = source.BrushInfo?.BorderColor ?? _brushInfo.BorderColor;
            _brushInfo.ForegroundColor = source.BrushInfo?.ForegroundColor ?? _brushInfo.ForegroundColor;
            _brushInfo.HoverBackgroundColor = source.BrushInfo?.HoverBackgroundColor ?? _brushInfo.HoverBackgroundColor;
            _brushInfo.HoverForegroundColor = source.BrushInfo?.HoverForegroundColor ?? _brushInfo.HoverForegroundColor;
            _brushInfo.ClickForegroundColor = source.BrushInfo?.ClickForegroundColor ?? _brushInfo.ClickForegroundColor;
            _brushInfo.ClickBackgroundColor = source.BrushInfo?.ClickBackgroundColor ?? _brushInfo.ClickBackgroundColor;
            _brushInfo.ClickBorderColorLeft = source.BrushInfo?.ClickBorderColorLeft ?? _brushInfo.ClickBorderColorLeft;
            _brushInfo.ClickBorderColorTop = source.BrushInfo?.ClickBorderColorTop ?? _brushInfo.ClickBorderColorTop;
            _brushInfo.ClickBorderColorRight = source.BrushInfo?.ClickBorderColorRight ?? _brushInfo.ClickBorderColorRight;
            _brushInfo.ClickBorderColorBottom = source.BrushInfo?.ClickBorderColorBottom ?? _brushInfo.ClickBorderColorBottom;
            RaisePropertyChanged(nameof(BrushInfo));

            // 复制外观信息
            _appearanceInfo ??= new IconButtonAppearanceInfo();
            _appearanceInfo.Opacity = source.AppearanceInfo?.Opacity ?? 100;
            _appearanceInfo.BorderThickness = source.AppearanceInfo?.BorderThickness ?? 0;
            _appearanceInfo.BorderThicknessLeft = source.AppearanceInfo?.BorderThicknessLeft ?? 0;
            _appearanceInfo.BorderThicknessTop = source.AppearanceInfo?.BorderThicknessTop ?? 0;
            _appearanceInfo.BorderThicknessRight = source.AppearanceInfo?.BorderThicknessRight ?? 0;
            _appearanceInfo.BorderThicknessBottom = source.AppearanceInfo?.BorderThicknessBottom ?? 0;
            _appearanceInfo.ClickBorderThicknessLeft = source.AppearanceInfo?.ClickBorderThicknessLeft ?? 0;
            _appearanceInfo.ClickBorderThicknessTop = source.AppearanceInfo?.ClickBorderThicknessTop ?? 0;
            _appearanceInfo.ClickBorderThicknessRight = source.AppearanceInfo?.ClickBorderThicknessRight ?? 0;
            _appearanceInfo.ClickBorderThicknessBottom = source.AppearanceInfo?.ClickBorderThicknessBottom ?? 0;
            RaisePropertyChanged(nameof(AppearanceInfo));

            // 复制布局信息
            _layoutInfo ??= new IconButtonLayoutInfo();
            Width = source.Width;
            Height = source.Height;
            _layoutInfo.HorizontalAlignment = source.LayoutInfo?.HorizontalAlignment ?? IconButtonLayoutInfo.HorizontalAlignmentEnum.Stretch;
            _layoutInfo.VerticalAlignment = source.LayoutInfo?.VerticalAlignment ?? IconButtonLayoutInfo.VerticalAlignmentEnum.Stretch;
            _layoutInfo.Margin = source.LayoutInfo?.Margin ?? 0;
            _layoutInfo.MarginLeft = source.LayoutInfo?.MarginLeft ?? 0;
            _layoutInfo.MarginTop = source.LayoutInfo?.MarginTop ?? 0;
            _layoutInfo.MarginRight = source.LayoutInfo?.MarginRight ?? 0;
            _layoutInfo.MarginBottom = source.LayoutInfo?.MarginBottom ?? 0;
            RaisePropertyChanged(nameof(LayoutInfo));

            // 复制公共信息
            _commonInfo ??= new IconButtonCommonInfo();
            _commonInfo.ButtonText = source.CommonInfo?.ButtonText ?? "";
            _commonInfo.HoverCursor = source.CommonInfo?.HoverCursor ?? CommonCursorType.手型;
            _commonInfo.Enabled = source.CommonInfo?.Enabled ?? true;
            _commonInfo.TooltipText = source.CommonInfo?.TooltipText ?? "";
            _commonInfo.GroupId = source.CommonInfo?.GroupId ?? "";
            RaisePropertyChanged(nameof(CommonInfo));

            // 复制字体信息
            _fontInfo ??= new IconButtonFontInfo();
            _fontInfo.FontColor = source.FontInfo?.FontColor ?? _fontInfo.FontColor;
            _fontInfo.FontSize = source.FontInfo?.FontSize ?? 16;
            _fontInfo.IsBold = source.FontInfo?.IsBold ?? false;
            _fontInfo.IsItalic = source.FontInfo?.IsItalic ?? false;
            _fontInfo.IsUnderline = source.FontInfo?.IsUnderline ?? false;
            RaisePropertyChanged(nameof(FontInfo));

            // 复制段落信息
            _paragraphInfo ??= new IconButtonParagraphInfo();
            _paragraphInfo.LineHeight = source.ParagraphInfo?.LineHeight ?? 0;
            _paragraphInfo.ParagraphSpacingBefore = source.ParagraphInfo?.ParagraphSpacingBefore ?? 0;
            _paragraphInfo.ParagraphSpacingAfter = source.ParagraphInfo?.ParagraphSpacingAfter ?? 0;
            _paragraphInfo.TextAlignment = source.ParagraphInfo?.TextAlignment ?? IconButtonParagraphInfo.TextAlignmentEnum.Center;
            RaisePropertyChanged(nameof(ParagraphInfo));

            // 复制杂项信息
            _miscInfo ??= new IconButtonMiscInfo();
            // 确保IconSource不会被设置为null，避免ControlObjectRunTimeHost.Refresh()方法中的NullReferenceException
            if (source.MiscInfo?.IconSource != null)
            {
                _miscInfo.IconSource = source.MiscInfo.IconSource;
            }
            _miscInfo.IconWidth = source.MiscInfo?.IconWidth ?? 24;
            _miscInfo.IconHeight = source.MiscInfo?.IconHeight ?? 24;
            _miscInfo.IconMargin = source.MiscInfo?.IconMargin ?? 0;
            _miscInfo.IconMarginLeft = source.MiscInfo?.IconMarginLeft ?? 0;
            _miscInfo.IconMarginTop = source.MiscInfo?.IconMarginTop ?? 0;
            _miscInfo.IconMarginRight = source.MiscInfo?.IconMarginRight ?? 0;
            _miscInfo.IconMarginBottom = source.MiscInfo?.IconMarginBottom ?? 0;
            _miscInfo.IconPosition = source.MiscInfo?.IconPosition ?? IconButtonMiscInfo.IconPositionEnum.Left;
            RaisePropertyChanged(nameof(MiscInfo));
        }

        #endregion
    }
}

