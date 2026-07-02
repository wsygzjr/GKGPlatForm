using System;

using System.ComponentModel;

using Avalonia.Media;

using PropertyModels.ComponentModel;



namespace GKG.Map.MapCell.Generic.Control.MapCell_TextBox

{

    /// <summary>

    /// 文本输入框属性编辑模型

    /// </summary>

    [Serializable]

    [MapPropertyOrder]

    [CategoryPriority("图元信息", 1)]

    [CategoryPriority("画笔", 2)]

    [CategoryPriority("外观", 3)]

    [CategoryPriority("公共", 4)]

    [CategoryPriority("布局", 5)]

    [CategoryPriority("文本", 6)]

    public class TextBoxPropertyModelEdit : ControlCellPropertyModelEdit

    {

        private TextBoxBrushInfo _brushInfo = new TextBoxBrushInfo();

        private TextBoxAppearanceInfo _appearanceInfo = new TextBoxAppearanceInfo();

        private TextBoxCommonInfo _commonInfo = new TextBoxCommonInfo();

        private TextBoxLayoutInfo _layoutInfo = new TextBoxLayoutInfo();

        private TextBoxTextInfo _textInfo = new TextBoxTextInfo();



        /// <summary>

        /// 画笔设置

        /// </summary>

        [DisplayName("画笔设置")]

        [Category("画笔")]

        [PropertySortOrder(1)]

        public TextBoxBrushInfo BrushInfo

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

        public TextBoxAppearanceInfo AppearanceInfo

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

        public TextBoxCommonInfo CommonInfo

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

        public TextBoxLayoutInfo LayoutInfo

        {

            get => _layoutInfo;

            set => SetProperty(ref _layoutInfo, value);

        }



        /// <summary>

        /// 文本设置

        /// </summary>

        [DisplayName("文本设置")]

        [Category("文本")]

        [PropertySortOrder(1)]

        public TextBoxTextInfo TextInfo

        {

            get => _textInfo;

            set => SetProperty(ref _textInfo, value);

        }



        #region SetPropertyValue 方法



        public override bool SetPropertyValue(string propertyID, MapCellPropValue propertyVal)
        {
            // ??????????????????????????????????????
            if (string.Compare(propertyID, nameof(TextBoxBrushInfo.BackgroundColor)) == 0)
            {
                _brushInfo ??= new TextBoxBrushInfo();
                _brushInfo.BackgroundColor = propertyVal != null
                    ? Color.Parse(propertyVal.ToPrimitiveValue<string>())
                    : TextBoxBrushInfo.Default.BackgroundColor;
                RaisePropertyChanged(nameof(BrushInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(TextBoxBrushInfo.BorderColor)) == 0)
            {
                _brushInfo ??= new TextBoxBrushInfo();
                _brushInfo.BorderColor = propertyVal != null
                    ? Color.Parse(propertyVal.ToPrimitiveValue<string>())
                    : TextBoxBrushInfo.Default.BorderColor;
                RaisePropertyChanged(nameof(BrushInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(TextBoxBrushInfo.ForegroundColor)) == 0)
            {
                _brushInfo ??= new TextBoxBrushInfo();
                _brushInfo.ForegroundColor = propertyVal != null
                    ? Color.Parse(propertyVal.ToPrimitiveValue<string>())
                    : TextBoxBrushInfo.Default.ForegroundColor;
                RaisePropertyChanged(nameof(BrushInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(TextBoxAppearanceInfo.Opacity)) == 0)
            {
                _appearanceInfo ??= new TextBoxAppearanceInfo();
                _appearanceInfo.Opacity = propertyVal != null
                    ? propertyVal.ToPrimitiveValue<int>()
                    : TextBoxAppearanceInfo.Default.Opacity;
                RaisePropertyChanged(nameof(AppearanceInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(TextBoxCommonInfo.Text)) == 0)
            {
                _commonInfo ??= new TextBoxCommonInfo();
                _commonInfo.Text = propertyVal != null
                    ? propertyVal.ToPrimitiveValue<string>() ?? string.Empty
                    : TextBoxCommonInfo.Default.Text;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }

            if (string.Compare(propertyID, nameof(BrushInfo)) == 0)

            {

                // 属性面板/操作原子下发的是对象Json，这里做反序列化并把值“灌入”现有对象，

                // 以确保已绑定的引用对象不被替换（保持 UI/VM 对象引用稳定）。

                var src = propertyVal != null ? DeserializeObject<TextBoxBrushInfo>(propertyVal) : new TextBoxBrushInfo();

                _brushInfo ??= new TextBoxBrushInfo();

                _brushInfo.BackgroundColor = src.BackgroundColor;

                _brushInfo.BorderColor = src.BorderColor;

                _brushInfo.ForegroundColor = src.ForegroundColor;

                _brushInfo.SelectedBorderColor = src.SelectedBorderColor;

                RaisePropertyChanged(nameof(BrushInfo));

                return true;

            }

            if (string.Compare(propertyID, nameof(AppearanceInfo)) == 0)

            {

                // 外观设置：透明度/边框线宽等

                var src = propertyVal != null ? DeserializeObject<TextBoxAppearanceInfo>(propertyVal) : new TextBoxAppearanceInfo();

                _appearanceInfo ??= new TextBoxAppearanceInfo();

                _appearanceInfo.Opacity = src.Opacity;

                _appearanceInfo.BorderThicknessLeft = src.BorderThicknessLeft;

                _appearanceInfo.BorderThicknessTop = src.BorderThicknessTop;

                _appearanceInfo.BorderThicknessRight = src.BorderThicknessRight;

                _appearanceInfo.BorderThicknessBottom = src.BorderThicknessBottom;

                RaisePropertyChanged(nameof(AppearanceInfo));

                return true;

            }

            if (string.Compare(propertyID, nameof(CommonInfo)) == 0)

            {

                // 公共设置：文本值/只读/启用/提示/光标等

                var src = propertyVal != null ? DeserializeObject<TextBoxCommonInfo>(propertyVal) : new TextBoxCommonInfo();

                _commonInfo ??= new TextBoxCommonInfo();

                _commonInfo.Text = src.Text;

                _commonInfo.TooltipText = src.TooltipText;

                _commonInfo.Enabled = src.Enabled;

                _commonInfo.IsReadOnly = src.IsReadOnly;

                _commonInfo.HoverCursor = src.HoverCursor;

                _commonInfo.SelectedTextOpacity = src.SelectedTextOpacity;

                _commonInfo.EnableSpellCheck = src.EnableSpellCheck;

                RaisePropertyChanged(nameof(CommonInfo));

                return true;

            }

            if (string.Compare(propertyID, nameof(LayoutInfo)) == 0)

            {

                // 布局设置：尺寸/对齐/边距/最小最大约束

                var src = propertyVal != null ? DeserializeObject<TextBoxLayoutInfo>(propertyVal) : new TextBoxLayoutInfo();

                _layoutInfo ??= new TextBoxLayoutInfo();

                // 宽高主数据统一走父类 Width/Height，LayoutInfo 不再承载宽高。

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

            if (string.Compare(propertyID, nameof(TextInfo)) == 0)

            {

                // 文本设置：字体族/颜色/字号/粗细/斜体

                var src = propertyVal != null ? DeserializeObject<TextBoxTextInfo>(propertyVal) : new TextBoxTextInfo();

                _textInfo ??= new TextBoxTextInfo();

                _textInfo.FontFamily = src.FontFamily;

                _textInfo.FontColor = src.FontColor;

                _textInfo.FontSize = src.FontSize;

                _textInfo.IsItalic = src.IsItalic;

                _textInfo.IsBold = src.IsBold;

                _textInfo.TextAlignment = src.TextAlignment;

                _textInfo.VerticalTextAlignment = src.VerticalTextAlignment;

                RaisePropertyChanged(nameof(TextInfo));

                return true;

            }

            return base.SetPropertyValue(propertyID, propertyVal);

        }



        /// <summary>

        /// 反序列化对象（MapCellPropValue -> PropObjectValue）

        /// </summary>

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



        public void CopyFrom(TextBoxPropertyModelEdit source)

        {

            if (source == null) return;



            // 复制画笔信息

            _brushInfo ??= new TextBoxBrushInfo();

            _brushInfo.BackgroundColor = source.BrushInfo?.BackgroundColor ?? _brushInfo.BackgroundColor;

            _brushInfo.BorderColor = source.BrushInfo?.BorderColor ?? _brushInfo.BorderColor;

            _brushInfo.ForegroundColor = source.BrushInfo?.ForegroundColor ?? _brushInfo.ForegroundColor;

            _brushInfo.SelectedBorderColor = source.BrushInfo?.SelectedBorderColor ?? _brushInfo.SelectedBorderColor;

            RaisePropertyChanged(nameof(BrushInfo));



            // 复制外观信息

            _appearanceInfo ??= new TextBoxAppearanceInfo();

            _appearanceInfo.Opacity = source.AppearanceInfo?.Opacity ?? 0;

            _appearanceInfo.BorderThicknessLeft = source.AppearanceInfo?.BorderThicknessLeft ?? 1;

            _appearanceInfo.BorderThicknessTop = source.AppearanceInfo?.BorderThicknessTop ?? 1;

            _appearanceInfo.BorderThicknessRight = source.AppearanceInfo?.BorderThicknessRight ?? 1;

            _appearanceInfo.BorderThicknessBottom = source.AppearanceInfo?.BorderThicknessBottom ?? 1;

            RaisePropertyChanged(nameof(AppearanceInfo));



            // 复制公共信息

            _commonInfo ??= new TextBoxCommonInfo();

            _commonInfo.Text = source.CommonInfo?.Text ?? string.Empty;

            _commonInfo.TooltipText = source.CommonInfo?.TooltipText ?? string.Empty;

            _commonInfo.Enabled = source.CommonInfo?.Enabled ?? true;

            _commonInfo.IsReadOnly = source.CommonInfo?.IsReadOnly ?? false;

            _commonInfo.HoverCursor = source.CommonInfo?.HoverCursor ?? CommonCursorType.文本输入;

            _commonInfo.SelectedTextOpacity = source.CommonInfo?.SelectedTextOpacity ?? 100;

            _commonInfo.EnableSpellCheck = source.CommonInfo?.EnableSpellCheck ?? false;

            RaisePropertyChanged(nameof(CommonInfo));



            // 复制布局信息

            _layoutInfo ??= new TextBoxLayoutInfo();

            Width = source.Width;

            Height = source.Height;

            _layoutInfo.HorizontalAlignment = source.LayoutInfo?.HorizontalAlignment ?? TextBoxLayoutInfo.HorizontalAlignmentEnum.Stretch;

            _layoutInfo.VerticalAlignment = source.LayoutInfo?.VerticalAlignment ?? TextBoxLayoutInfo.VerticalAlignmentEnum.Stretch;

            _layoutInfo.MarginLeft = source.LayoutInfo?.MarginLeft ?? 0;

            _layoutInfo.MarginTop = source.LayoutInfo?.MarginTop ?? 0;

            _layoutInfo.MarginRight = source.LayoutInfo?.MarginRight ?? 0;

            _layoutInfo.MarginBottom = source.LayoutInfo?.MarginBottom ?? 0;

            _layoutInfo.MinWidth = source.LayoutInfo?.MinWidth ?? 0;

            _layoutInfo.MaxWidth = source.LayoutInfo?.MaxWidth ?? int.MaxValue;

            _layoutInfo.MinHeight = source.LayoutInfo?.MinHeight ?? 0;

            _layoutInfo.MaxHeight = source.LayoutInfo?.MaxHeight ?? int.MaxValue;

            RaisePropertyChanged(nameof(LayoutInfo));



            // 复制文本信息

            _textInfo ??= new TextBoxTextInfo();

            _textInfo.FontFamily = source.TextInfo?.FontFamily ?? TextBoxFontFamilyType.微软雅黑;

            _textInfo.FontColor = source.TextInfo?.FontColor ?? _textInfo.FontColor;

            _textInfo.FontSize = source.TextInfo?.FontSize ?? 14;

            _textInfo.IsItalic = source.TextInfo?.IsItalic ?? false;

            _textInfo.IsBold = source.TextInfo?.IsBold ?? false;

            _textInfo.TextAlignment = source.TextInfo?.TextAlignment ?? TextBoxTextAlignmentType.Left;

            _textInfo.VerticalTextAlignment = source.TextInfo?.VerticalTextAlignment ?? TextBoxVerticalTextAlignmentType.Center;

            RaisePropertyChanged(nameof(TextInfo));

        }



        #endregion

    }

}

