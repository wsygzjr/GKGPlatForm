using Avalonia.Media;
using Griffins;
using PropertyModels.ComponentModel;
using System;
using System.ComponentModel;
using GF_Gereric; // 引入平台原生的 BitmapData
using GKG.Map.MapCell.Generic.Control.Lable;

namespace GKG.Map.MapCell.Generic.IconButton
{
    // 给属性面板用的字体枚举
    public enum ButtonFontType
    {
        [Description("微软雅黑")] Microsoft_YaHei,
        [Description("宋体")] SimSun,
        [Description("黑体")] SimHei,
        [Description("楷体")] KaiTi,
        [Description("Arial")] Arial,
        [Description("Consolas")] Consolas
    }

    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("基础数据", 1)]
    [CategoryPriority("样式外观", 2)]
    [CategoryPriority("字体排版", 3)]
    [CategoryPriority("布局", 4)]
    public class IconButtonPropertyModelEdit : ControlCellPropertyModelEdit
    {
        #region 1. 基础数据 (Data)
        private string _buttonText = "通用按钮";
        private bool _showIcon = false;
        private string _iconBase64 = string.Empty;
        private BitmapData _iconSource;
        private bool _isUpdatingIconSource;
        private CursorType _cursorType = CursorType.Hand;
        private bool _isEnabled = true;
        private string _toolTip = string.Empty;
        private bool _showText = true;

        [DisplayName("显示文本")]
        [Category("基础数据")]
        [PropertySortOrder(1)]
        public bool ShowText { get => _showText; set => SetProperty(ref _showText, value); }

        [DisplayName("按钮文本")]
        [Category("基础数据")]
        [PropertySortOrder(1)]
        public string ButtonText { get => _buttonText; set => SetProperty(ref _buttonText, value ?? string.Empty); }

        [DisplayName("显示图标")]
        [Category("基础数据")]
        [PropertySortOrder(2)]
        public bool ShowIcon { get => _showIcon; set => SetProperty(ref _showIcon, value); }

        // ====================================================================
        // 【核心魔法】：向平台暴露 BitmapData 唤醒文件选择器，同时自动转为 Base64
        // ====================================================================
        [DisplayName("图标资源")]
        [Category("基础数据")]
        [PropertySortOrder(3)]
        [System.Text.Json.Serialization.JsonIgnore] // 绝对不能序列化这个胖对象
        public BitmapData IconSource
        {
            get
            {
                if ((_iconSource == null || _iconSource.Bitmap == null) && !string.IsNullOrWhiteSpace(_iconBase64))
                {
                    try
                    {
                        var bytes = System.Convert.FromBase64String(_iconBase64);
                        _iconSource = new BitmapData();
                        _iconSource.FromBytes(bytes);
                    }
                    catch { _iconSource = null; }
                }
                return _iconSource;
            }
            set
            {
                if (_isUpdatingIconSource) return;
                _isUpdatingIconSource = true;
                try
                {
                    SetProperty(ref _iconSource, value);

                    if (value != null && value.Bitmap != null)
                    {
                        var bytes = value.ToBytes();
                        IconBase64 = bytes != null && bytes.Length > 0 ? System.Convert.ToBase64String(bytes) : "";
                        // 极其贴心的小细节：用户选了图片，自动帮他把“显示图标”开关打开！
                        ShowIcon = !string.IsNullOrEmpty(IconBase64);
                    }
                    else
                    {
                        IconBase64 = "";
                    }
                }
                finally
                {
                    _isUpdatingIconSource = false;
                }
            }
        }

        // 把真实的纯字符串数据藏起来，留给底层序列化和跨电脑传输用
        [Browsable(false)]
        public string IconBase64 { get => _iconBase64; set => SetProperty(ref _iconBase64, value ?? string.Empty); }
        // ====================================================================

        [DisplayName("鼠标样式")]
        [Category("基础数据")]
        [PropertySortOrder(4)]
        public CursorType CursorType { get => _cursorType; set => SetProperty(ref _cursorType, value); }

        [DisplayName("是否启用")]
        [Category("基础数据")]
        [PropertySortOrder(5)]
        public bool IsEnabled { get => _isEnabled; set => SetProperty(ref _isEnabled, value); }

        [DisplayName("提示文案")]
        [Category("基础数据")]
        [PropertySortOrder(6)]
        public string ToolTip { get => _toolTip; set => SetProperty(ref _toolTip, value ?? string.Empty); }
        #endregion

        #region 2. 样式外观 (Style)
        private Color _backgroundColor = Color.Parse("#1890FF");
        private Color _foregroundColor = Colors.White;
        private Color _borderBrush = Colors.Transparent;
        private string _bgColorStr;
        private string _fgColorStr;
        private string _bdColorStr;

        [Browsable(false)] public string BgColorStr { get => _bgColorStr ?? _backgroundColor.ToString(); set => _bgColorStr = value; }
        [Browsable(false)] public string FgColorStr { get => _fgColorStr ?? _foregroundColor.ToString(); set => _fgColorStr = value; }
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

        private string _cornerRadius = "4";
        [DisplayName("圆角大小")]
        [Category("样式外观")]
        [PropertySortOrder(5)]
        public string CornerRadius { get => _cornerRadius; set => SetProperty(ref _cornerRadius, value ?? "0"); }

        private double _opacity = 1.0;
        [DisplayName("透明度(0-1)")]
        [Category("样式外观")]
        [PropertySortOrder(6)]
        public double Opacity { get => _opacity; set => SetProperty(ref _opacity, Math.Max(0.0, Math.Min(1.0, value))); }
        #endregion

        #region 3. 字体排版 (Font)
        private int _fontSize = 14;
        private string _fontFamily = "Microsoft YaHei";
        private bool _isBold;
        private bool _isItalic;
        private bool _isUnderline;

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
        public ButtonFontType FontFamilyUI
        {
            get => _fontFamily switch { "SimSun" => ButtonFontType.SimSun, "SimHei" => ButtonFontType.SimHei, "Arial" => ButtonFontType.Arial, "KaiTi" => ButtonFontType.KaiTi, "Consolas" => ButtonFontType.Consolas, _ => ButtonFontType.Microsoft_YaHei };
            set => FontFamily = value switch { ButtonFontType.SimSun => "SimSun", ButtonFontType.SimHei => "SimHei", ButtonFontType.Arial => "Arial", ButtonFontType.KaiTi => "KaiTi", ButtonFontType.Consolas => "Consolas", _ => "Microsoft YaHei" };
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
        #endregion

        #region 4. 布局 (Layout)
        private IconPlacementType _iconPlacement = IconPlacementType.Left;
        private double _iconSize = 20;
        private double _iconSpacing = 8;
        private string _margin = "0";
        private string _padding = "8,4";
        private HorizontalAlignType _horizontalAlign = HorizontalAlignType.Center;

        [DisplayName("图标位置")]
        [Category("布局")]
        [PropertySortOrder(1)]
        public IconPlacementType IconPlacement { get => _iconPlacement; set => SetProperty(ref _iconPlacement, value); }

        [DisplayName("图标尺寸(宽/高)")]
        [Category("布局")]
        [PropertySortOrder(2)]
        public double IconSize { get => _iconSize; set => SetProperty(ref _iconSize, Math.Max(0, value)); }

        [DisplayName("图文间距")]
        [Category("布局")]
        [PropertySortOrder(3)]
        public double IconSpacing { get => _iconSpacing; set => SetProperty(ref _iconSpacing, Math.Max(0, value)); }

        [DisplayName("外边距(Margin)")]
        [Category("布局")]
        [PropertySortOrder(4)]
        public string Margin { get => _margin; set => SetProperty(ref _margin, value ?? "0"); }

        [DisplayName("内边距(Padding)")]
        [Category("布局")]
        [PropertySortOrder(5)]
        public string Padding { get => _padding; set => SetProperty(ref _padding, value ?? "8,4"); }

        [DisplayName("内容对齐")]
        [Category("布局")]
        [PropertySortOrder(6)]
        public HorizontalAlignType HorizontalAlign { get => _horizontalAlign; set => SetProperty(ref _horizontalAlign, value); }
        #endregion

        private Color SafeParseColor(string hex, Color fallback)
        {
            if (!string.IsNullOrEmpty(hex) && Color.TryParse(hex, out var c)) return c;
            return fallback;
        }

        public void CopyFrom(IconButtonPropertyModelEdit source)
        {
            if (source == null) return;
            base.CopyFrom(source);

            ShowText = source.ShowText;
            ButtonText = source.ButtonText;
            ShowIcon = source.ShowIcon;
            IconBase64 = source.IconBase64; // 只复制字符串即可，BitmapData 会自动根据字符串生成
            CursorType = source.CursorType;
            IsEnabled = source.IsEnabled;
            ToolTip = source.ToolTip;

            BackgroundColor = SafeParseColor(source.BgColorStr, source.BackgroundColor);
            ForegroundColor = SafeParseColor(source.FgColorStr, source.ForegroundColor);
            BorderBrush = SafeParseColor(source.BdColorStr, source.BorderBrush);
            BorderThickness = source.BorderThickness;
            CornerRadius = source.CornerRadius;
            Opacity = source.Opacity;

            FontSize = source.FontSize;
            FontFamily = source.FontFamily;
            IsBold = source.IsBold;
            IsItalic = source.IsItalic;
            IsUnderline = source.IsUnderline;

            IconPlacement = source.IconPlacement;
            IconSize = source.IconSize;
            IconSpacing = source.IconSpacing;
            Margin = source.Margin;
            Padding = source.Padding;
            HorizontalAlign = source.HorizontalAlign;
        }
    }
}