using Avalonia.Media;
using Griffins;
using PropertyModels.ComponentModel;
using System;
using System.ComponentModel;

namespace GKG.Map.MapCell.Generic.Control.Lable
{
    #region 通用枚举定义

    public enum CursorType
    {
        [Description("默认箭头")] Arrow = 0,
        [Description("文本输入")] Ibeam = 1,
        [Description("等待")] Wait = 2,
        [Description("十字")] Cross = 3,
        [Description("上箭头")] UpArrow = 4,
        [Description("左右调整")] SizeWestEast = 5,
        [Description("上下调整")] SizeNorthSouth = 6,
        [Description("移动(四向)")] SizeAll = 7,
        [Description("禁止")] No = 8,
        [Description("手型")] Hand = 9,
        [Description("后台运行")] AppStarting = 10,
        [Description("帮助")] Help = 11
    }

    public enum HorizontalAlignType 
    { 
        [Description("拉伸")] Stretch = 0, 
        [Description("左对齐")] Left = 1, 
        [Description("居中")] Center = 2, 
        [Description("右对齐")] Right = 3 
    }
    public enum VerticalAlignType 
    { 
        [Description("拉伸")] Stretch = 0, 
        [Description("顶部")] Top = 1, 
        [Description("居中")] Center = 2, 
        [Description("底部")] Bottom = 3 
    }
    public enum TextAlignType 
    { 
        [Description("左对齐")] Left = 0, 
        [Description("居中")] Center = 1, 
        [Description("右对齐")] Right = 2, 
        [Description("两端对齐")] Justify = 3 
    }

    // 给属性面板用的字体枚举
    public enum LableFontType
    {
        [Description("微软雅黑")] Microsoft_YaHei, 
        [Description("宋体")] SimSun, 
        [Description("黑体")] SimHei,
        [Description("楷体")] KaiTi, 
        [Description("Arial")] Arial, 
        [Description("Consolas")] Consolas
    }

    #endregion

    /// <summary>
    /// 标签图元属性编辑模型对象
    /// </summary>
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("基础数据", 1)]
    [CategoryPriority("样式外观", 2)]
    [CategoryPriority("字体排版", 3)]
    [CategoryPriority("布局", 4)]
    public class LablePropertyModelEdit : ControlCellPropertyModelEdit
    {
        #region 1. 基础数据 (Data)

        private string _lableText = "标签文本";
        private CursorType _cursorType = CursorType.Arrow;
        private bool _isEnabled = true;
        private string _toolTip = string.Empty;

        [DisplayName("标签文本")]
        [Category("基础数据")]
        [PropertySortOrder(1)]
        public string LableText { get => _lableText; set => SetProperty(ref _lableText, value ?? string.Empty); }

        [DisplayName("鼠标样式")]
        [Category("基础数据")]
        [PropertySortOrder(2)]
        public CursorType CursorType { get => _cursorType; set => SetProperty(ref _cursorType, value); }

        [DisplayName("是否启用")]
        [Category("基础数据")]
        [PropertySortOrder(3)]
        public bool IsEnabled { get => _isEnabled; set => SetProperty(ref _isEnabled, value); }

        [DisplayName("提示文案")]
        [Category("基础数据")]
        [PropertySortOrder(4)]
        public string ToolTip { get => _toolTip; set => SetProperty(ref _toolTip, value ?? string.Empty); }

        #endregion

        #region 2. 样式外观 (Style)

        private Color _backgroundColor = Colors.Transparent;
        private Color _foregroundColor = Colors.Black;
        private Color _borderBrush = Colors.Transparent;

        private string _bgColorStr;
        [Browsable(false)] public string BgColorStr { get => _bgColorStr ?? _backgroundColor.ToString(); set => _bgColorStr = value; }

        private string _fgColorStr;
        [Browsable(false)] public string FgColorStr { get => _fgColorStr ?? _foregroundColor.ToString(); set => _fgColorStr = value; }

        private string _bdColorStr;
        [Browsable(false)] public string BdColorStr { get => _bdColorStr ?? _borderBrush.ToString(); set => _bdColorStr = value; }

        [DisplayName("背景色")]
        [Category("样式外观")]
        [PropertySortOrder(1)]
        [System.Text.Json.Serialization.JsonIgnore]
        public Color BackgroundColor { get => _backgroundColor; set => SetProperty(ref _backgroundColor, value); }

        [DisplayName("文字颜色")]
        [Category("样式外观")]
        [PropertySortOrder(2)]
        [System.Text.Json.Serialization.JsonIgnore]
        public Color ForegroundColor { get => _foregroundColor; set => SetProperty(ref _foregroundColor, value); }

        [DisplayName("边框颜色")]
        [Category("样式外观")]
        [PropertySortOrder(3)]
        [System.Text.Json.Serialization.JsonIgnore]
        public Color BorderBrush { get => _borderBrush; set => SetProperty(ref _borderBrush, value); }

        private string _borderThickness = "0";
        [DisplayName("边框厚度")]
        [Category("样式外观")]
        [PropertySortOrder(4)]
        public string BorderThickness { get => _borderThickness; set => SetProperty(ref _borderThickness, value ?? "0"); }

        private double _opacity = 1.0;
        [DisplayName("透明度(0-1)")]
        [Category("样式外观")]
        [PropertySortOrder(5)]
        public double Opacity { get => _opacity; set => SetProperty(ref _opacity, Math.Max(0.0, Math.Min(1.0, value))); }

        #endregion

        #region 3. 字体排版 (Font)

        private int _fontSize = 14;
        private string _fontFamily = "Microsoft YaHei";
        private bool _isBold;
        private bool _isItalic;
        private bool _isUnderline;
        private double _lineHeight = 1.0;
        private TextAlignType _textAlignment = TextAlignType.Left;

        [DisplayName("字号")]
        [Category("字体排版")]
        [PropertySortOrder(1)]
        public int FontSize { get => _fontSize; set => SetProperty(ref _fontSize, Math.Max(1, value)); }

        [Browsable(false)]
        public string FontFamily { get => _fontFamily; set { if (SetProperty(ref _fontFamily, value ?? "Microsoft YaHei")) RaisePropertyChanged(nameof(FontFamilyUI)); } }

        [DisplayName("字体族")]
        [Category("字体排版")]
        [PropertySortOrder(2)]
        [System.Text.Json.Serialization.JsonIgnore]
        public LableFontType FontFamilyUI
        {
            get => _fontFamily switch { "SimSun" => LableFontType.SimSun, "SimHei" => LableFontType.SimHei, "Arial" => LableFontType.Arial, _ => LableFontType.Microsoft_YaHei };
            set => FontFamily = value switch { LableFontType.SimSun => "SimSun", LableFontType.SimHei => "SimHei", LableFontType.Arial => "Arial", _ => "Microsoft YaHei" };
        }

        [DisplayName("加粗")]
        [Category("字体排版")]
        [PropertySortOrder(3)]
        public bool IsBold { get => _isBold; set => SetProperty(ref _isBold, value); }

        [DisplayName("斜体")]
        [Category("字体排版")]
        [PropertySortOrder(4)]
        public bool IsItalic { get => _isItalic; set => SetProperty(ref _isItalic, value); }

        [DisplayName("下划线")]
        [Category("字体排版")]
        [PropertySortOrder(5)]
        public bool IsUnderline { get => _isUnderline; set => SetProperty(ref _isUnderline, value); }

        [DisplayName("行高(倍数)")]
        [Category("字体排版")]
        [PropertySortOrder(6)]
        public double LineHeight { get => _lineHeight; set => SetProperty(ref _lineHeight, Math.Max(0.1, value)); }

        [DisplayName("文本对齐")]
        [Category("字体排版")]
        [PropertySortOrder(7)]
        public TextAlignType TextAlignment { get => _textAlignment; set => SetProperty(ref _textAlignment, value); }

        #endregion

        #region 4. 布局 (Layout)

        private string _margin = "0";
        private string _padding = "0"; // 替代了以前极其复杂的段前段后间距
        private HorizontalAlignType _horizontalAlign = HorizontalAlignType.Stretch;
        private VerticalAlignType _verticalAlign = VerticalAlignType.Stretch;

        [DisplayName("外边距(Margin)")]
        [Category("布局")]
        [PropertySortOrder(1)]
        public string Margin { get => _margin; set => SetProperty(ref _margin, value ?? "0"); }

        [DisplayName("内边距(Padding)")]
        [Category("布局")]
        [PropertySortOrder(2)]
        public string Padding { get => _padding; set => SetProperty(ref _padding, value ?? "0"); }

        [DisplayName("水平对齐")]
        [Category("布局")]
        [PropertySortOrder(3)]
        public HorizontalAlignType HorizontalAlign { get => _horizontalAlign; set => SetProperty(ref _horizontalAlign, value); }

        [DisplayName("垂直对齐")]
        [Category("布局")]
        [PropertySortOrder(4)]
        public VerticalAlignType VerticalAlign { get => _verticalAlign; set => SetProperty(ref _verticalAlign, value); }

        #endregion

        #region 拷贝与赋值

        private Color SafeParseColor(string hex, Color fallback)
        {
            if (!string.IsNullOrEmpty(hex) && Color.TryParse(hex, out var c)) return c;
            return fallback;
        }

        public void CopyFrom(LablePropertyModelEdit source)
        {
            if (source == null) return;
            base.CopyFrom(source);

            LableText = source.LableText;
            CursorType = source.CursorType;
            IsEnabled = source.IsEnabled;
            ToolTip = source.ToolTip;

            BackgroundColor = SafeParseColor(source.BgColorStr, source.BackgroundColor);
            ForegroundColor = SafeParseColor(source.FgColorStr, source.ForegroundColor);
            BorderBrush = SafeParseColor(source.BdColorStr, source.BorderBrush);
            BorderThickness = source.BorderThickness;
            Opacity = source.Opacity;

            FontSize = source.FontSize;
            FontFamily = source.FontFamily;
            IsBold = source.IsBold;
            IsItalic = source.IsItalic;
            IsUnderline = source.IsUnderline;
            LineHeight = source.LineHeight;
            TextAlignment = source.TextAlignment;

            Margin = source.Margin;
            Padding = source.Padding;
            HorizontalAlign = source.HorizontalAlign;
            VerticalAlign = source.VerticalAlign;
        }
        #endregion
    }

}