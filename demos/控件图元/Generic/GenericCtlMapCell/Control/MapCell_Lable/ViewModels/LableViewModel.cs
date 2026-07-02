using Avalonia;
using Avalonia.Media;
using ReactiveUI;
using System;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace GKG.Map.MapCell.Generic.Lable.ViewModel
{
    /// <summary>
    /// 标签图元视图模型
    /// </summary>
    public class LableViewModel : ReactiveObject, IDisposable
    {
        #region 私有字段

        private LablePropertyModelEdit _model;
        private bool _suppressModelToViewModelSync;

        // 交互设置
        private CursorType _cursorType = CursorType.Arrow;
        private bool _isEnabled = true;
        private string _toolTip = "";

        // 外观设置
        private double _opacity = 1.0;
        private double _borderThicknessLeft = 0;
        private double _borderThicknessTop = 0;
        private double _borderThicknessRight = 0;
        private double _borderThicknessBottom = 0;

        // 文本内容
        private string _lableText;

        // 颜色设置
        private Color _lableColor;
        private Color _backColor;

        // 字体设置
        private double _fontSize = 14.0;
        private bool _isBold = false;
        private bool _isItalic = false;
        private bool _isUnderline = false;

        // 段落设置
        private double _lineHeight = 1.0;
        private double _paragraphSpacingBefore = 0;
        private double _paragraphSpacingAfter = 0;
        private TextAlignType _textAlignment = TextAlignType.Left;
        private TextVerticalAlignType _verticalTextAlignment = TextVerticalAlignType.Center;

        // 布局设置
        private HorizontalAlignType _horizontalAlign = HorizontalAlignType.Center;
        private VerticalAlignType _verticalAlign = VerticalAlignType.Center;

        #endregion


        #region 公共属性

        public LablePropertyModelEdit Model => _model;
        public CursorType CursorType { get => _cursorType; set => this.RaiseAndSetIfChanged(ref _cursorType, value); }
        public bool IsEnabled { get => _isEnabled; set => this.RaiseAndSetIfChanged(ref _isEnabled, value); }
        public string ToolTip { get => _toolTip; set => this.RaiseAndSetIfChanged(ref _toolTip, value); }
        public double Opacity { get => _opacity; set => this.RaiseAndSetIfChanged(ref _opacity, value); }
        public double BorderThicknessLeft
        {
            get => _borderThicknessLeft;
            set
            {
                this.RaiseAndSetIfChanged(ref _borderThicknessLeft, value);
                this.RaisePropertyChanged(nameof(BorderThickness));
            }
        }
        public double BorderThicknessTop
        {
            get => _borderThicknessTop;
            set
            {
                this.RaiseAndSetIfChanged(ref _borderThicknessTop, value);
                this.RaisePropertyChanged(nameof(BorderThickness));
            }
        }
        public double BorderThicknessRight
        {
            get => _borderThicknessRight;
            set
            {
                this.RaiseAndSetIfChanged(ref _borderThicknessRight, value);
                this.RaisePropertyChanged(nameof(BorderThickness));
            }
        }
        public double BorderThicknessBottom
        {
            get => _borderThicknessBottom;
            set
            {
                this.RaiseAndSetIfChanged(ref _borderThicknessBottom, value);
                this.RaisePropertyChanged(nameof(BorderThickness));
            }
        }
        public Thickness BorderThickness => new Thickness(BorderThicknessLeft, BorderThicknessTop, BorderThicknessRight, BorderThicknessBottom);
        public string LableText { get => _lableText; set => this.RaiseAndSetIfChanged(ref _lableText, value); }
        public Color LableColor { get => _lableColor; set => this.RaiseAndSetIfChanged(ref _lableColor, value); }
        public Color BackColor { get => _backColor; set => this.RaiseAndSetIfChanged(ref _backColor, value); }
        public double FontSize { get => _fontSize; set => this.RaiseAndSetIfChanged(ref _fontSize, value); }
        public bool IsBold { get => _isBold; set => this.RaiseAndSetIfChanged(ref _isBold, value); }
        public bool IsItalic { get => _isItalic; set => this.RaiseAndSetIfChanged(ref _isItalic, value); }
        public bool IsUnderline { get => _isUnderline; set => this.RaiseAndSetIfChanged(ref _isUnderline, value); }
        public double LineHeight { get => _lineHeight; set => this.RaiseAndSetIfChanged(ref _lineHeight, value); }
        public double ParagraphSpacingBefore { get => _paragraphSpacingBefore; set => this.RaiseAndSetIfChanged(ref _paragraphSpacingBefore, value); }
        public double ParagraphSpacingAfter { get => _paragraphSpacingAfter; set => this.RaiseAndSetIfChanged(ref _paragraphSpacingAfter, value); }
        public TextAlignType TextAlignment { get => _textAlignment; set => this.RaiseAndSetIfChanged(ref _textAlignment, value); }
        public TextVerticalAlignType VerticalTextAlignment { get => _verticalTextAlignment; set => this.RaiseAndSetIfChanged(ref _verticalTextAlignment, value); }
        public HorizontalAlignType HorizontalAlign { get => _horizontalAlign; set => this.RaiseAndSetIfChanged(ref _horizontalAlign, value); }
        public VerticalAlignType VerticalAlign { get => _verticalAlign; set => this.RaiseAndSetIfChanged(ref _verticalAlign, value); }

        #endregion

        #region 构造函数

        public LableViewModel(bool designTime, LablePropertyModelEdit model)
        {
            _model = model;
            _suppressModelToViewModelSync = true;
            InitializeFromModel(model);
            // 订阅嵌套对象的属性变化
            model.BrushInfo.PropertyChanged += OnBrushInfoChanged;
            model.AppearanceInfo.PropertyChanged += OnAppearanceInfoChanged;
            model.LayoutInfo.PropertyChanged += OnLayoutInfoChanged;
            model.CommonInfo.PropertyChanged += OnCommonInfoChanged;
            model.FontInfo.PropertyChanged += OnFontInfoChanged;
            model.ParagraphInfo.PropertyChanged += OnParagraphInfoChanged;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 运行时读档完成后，直接用模型真实值同步刷新一遍 ViewModel，
        /// 避免首帧先显示默认文本/默认布局，下一拍才慢慢回放到位。
        /// </summary>
        public void ReloadFromModel()
        {
            _suppressModelToViewModelSync = true;

            BackColor = _model.BrushInfo.BackColor;
            LableColor = _model.BrushInfo.ForeColor;

            Opacity = _model.AppearanceInfo.Opacity;
            BorderThicknessLeft = _model.AppearanceInfo.BorderThicknessLeft;
            BorderThicknessTop = _model.AppearanceInfo.BorderThicknessTop;
            BorderThicknessRight = _model.AppearanceInfo.BorderThicknessRight;
            BorderThicknessBottom = _model.AppearanceInfo.BorderThicknessBottom;

            HorizontalAlign = _model.LayoutInfo.HorizontalAlign;
            VerticalAlign = _model.LayoutInfo.VerticalAlign;

            LableText = _model.CommonInfo.LableValue;
            CursorType = _model.CommonInfo.CursorType;
            IsEnabled = _model.CommonInfo.IsEnabled;
            ToolTip = _model.CommonInfo.ToolTip;

            FontSize = _model.FontInfo.FontSize;
            IsBold = _model.FontInfo.IsBold;
            IsItalic = _model.FontInfo.IsItalic;
            IsUnderline = _model.FontInfo.IsUnderline;

            LineHeight = _model.ParagraphInfo.LineHeight;
            ParagraphSpacingBefore = _model.ParagraphInfo.ParagraphSpacingBefore;
            ParagraphSpacingAfter = _model.ParagraphInfo.ParagraphSpacingAfter;
            TextAlignment = _model.ParagraphInfo.TextAlignment;
            VerticalTextAlignment = _model.ParagraphInfo.VerticalTextAlignment;

            _suppressModelToViewModelSync = false;
        }

        private void InitializeFromModel(LablePropertyModelEdit model)
        {
            // 画笔设置
            _backColor = model.BrushInfo.BackColor;
            _lableColor = model.BrushInfo.ForeColor;

            // 外观设置
            _opacity = model.AppearanceInfo.Opacity;
            _borderThicknessLeft = model.AppearanceInfo.BorderThicknessLeft;
            _borderThicknessTop = model.AppearanceInfo.BorderThicknessTop;
            _borderThicknessRight = model.AppearanceInfo.BorderThicknessRight;
            _borderThicknessBottom = model.AppearanceInfo.BorderThicknessBottom;

            // 布局设置
            // 宽高主数据统一走父类 Width/Height，LayoutInfo 仅保留旧页面兼容镜像。
            _horizontalAlign = model.LayoutInfo.HorizontalAlign;
            _verticalAlign = model.LayoutInfo.VerticalAlign;

            // 公共设置
            _lableText = model.CommonInfo.LableValue;
            _cursorType = model.CommonInfo.CursorType;
            _isEnabled = model.CommonInfo.IsEnabled;
            _toolTip = model.CommonInfo.ToolTip;

            // 字体设置
            _fontSize = model.FontInfo.FontSize;
            _isBold = model.FontInfo.IsBold;
            _isItalic = model.FontInfo.IsItalic;
            _isUnderline = model.FontInfo.IsUnderline;

            // 段落设置
            _lineHeight = model.ParagraphInfo.LineHeight;
            _paragraphSpacingBefore = model.ParagraphInfo.ParagraphSpacingBefore;
            _paragraphSpacingAfter = model.ParagraphInfo.ParagraphSpacingAfter;
            _textAlignment = model.ParagraphInfo.TextAlignment;
            _verticalTextAlignment = model.ParagraphInfo.VerticalTextAlignment;
        }

        private void OnBrushInfoChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_suppressModelToViewModelSync)
            {
                return;
            }
            BackColor = _model.BrushInfo.BackColor;
            LableColor = _model.BrushInfo.ForeColor;
        }

        private void OnAppearanceInfoChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_suppressModelToViewModelSync)
            {
                return;
            }
            Opacity = _model.AppearanceInfo.Opacity;
            BorderThicknessLeft = _model.AppearanceInfo.BorderThicknessLeft;
            BorderThicknessTop = _model.AppearanceInfo.BorderThicknessTop;
            BorderThicknessRight = _model.AppearanceInfo.BorderThicknessRight;
            BorderThicknessBottom = _model.AppearanceInfo.BorderThicknessBottom;
        }

        private void OnLayoutInfoChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_suppressModelToViewModelSync)
            {
                return;
            }
            HorizontalAlign = _model.LayoutInfo.HorizontalAlign;
            VerticalAlign = _model.LayoutInfo.VerticalAlign;
        }

        private void OnCommonInfoChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_suppressModelToViewModelSync)
            {
                return;
            }
            LableText = _model.CommonInfo.LableValue;
            CursorType = _model.CommonInfo.CursorType;
            IsEnabled = _model.CommonInfo.IsEnabled;
            ToolTip = _model.CommonInfo.ToolTip;
        }

        private void OnFontInfoChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_suppressModelToViewModelSync)
            {
                return;
            }
            LableColor = _model.FontInfo.FontColor;
            FontSize = _model.FontInfo.FontSize;
            IsBold = _model.FontInfo.IsBold;
            IsItalic = _model.FontInfo.IsItalic;
            IsUnderline = _model.FontInfo.IsUnderline;
        }

        private void OnParagraphInfoChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_suppressModelToViewModelSync)
            {
                return;
            }
            LineHeight = _model.ParagraphInfo.LineHeight;
            ParagraphSpacingBefore = _model.ParagraphInfo.ParagraphSpacingBefore;
            ParagraphSpacingAfter = _model.ParagraphInfo.ParagraphSpacingAfter;
            TextAlignment = _model.ParagraphInfo.TextAlignment;
            VerticalTextAlignment = _model.ParagraphInfo.VerticalTextAlignment;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (_model != null)
            {
                _model.BrushInfo.PropertyChanged -= OnBrushInfoChanged;
                _model.AppearanceInfo.PropertyChanged -= OnAppearanceInfoChanged;
                _model.LayoutInfo.PropertyChanged -= OnLayoutInfoChanged;
                _model.CommonInfo.PropertyChanged -= OnCommonInfoChanged;
                _model.FontInfo.PropertyChanged -= OnFontInfoChanged;
                _model.ParagraphInfo.PropertyChanged -= OnParagraphInfoChanged;
            }
        }

        #endregion
    }
}
