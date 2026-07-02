using Avalonia.Media;
using GKG.Map.MapCell.Generic.GroupPanel;
using GKG.Map.MapCell.Generic.Control.Lable;
using GKG.Map.MapCell.Generic.Stepper.Objects;
using ReactiveUI;
using System;

namespace GKG.Map.MapCell.Generic.Stepper.ViewModels
{
    /// <summary>
    /// 步进器图元视图模型
    /// </summary>
    public class StepperViewModel : ReactiveObject, IDisposable
    {
        #region 私有字段

        private bool _designTime;
        private StepperPropertyModelEdit _model;

        // 画笔设置
        private Color _backColor;
        private Color _borderColor;
        private Color _foreColor;

        // 外观设置
        private double _opacity = 1.0;
        private double _borderThicknessLeft = 1;
        private double _borderThicknessTop = 1;
        private double _borderThicknessRight = 1;
        private double _borderThicknessBottom = 1;

        // 公共设置
        private string _labelName = "标签";
        private decimal _value = 0;
        private decimal _minimum = 0;
        private decimal _maximum = 100;
        private decimal _increment = 1;
        private int _decimalPlaces = 0;
        private bool _isEnabled = true;
        private string _toolTip = "";

        // 布局设置
        private HorizontalAlignType _horizontalAlign = HorizontalAlignType.Left;
        private VerticalAlignType _verticalAlign = VerticalAlignType.Center;
        private double _marginTop = 0;
        private double _marginLeft = 0;
        private double _marginBottom = 0;
        private double _marginRight = 0;
        private double _minWidth = 0;
        private double _maxWidth = 0;
        private double _minHeight = 0;
        private double _maxHeight = 0;

        // 文本设置
        private FontFamilyType _fontFamilyType = FontFamilyType.MicrosoftYaHei;
        private Color _fontColor = Colors.Black;
        private double _fontSize = 14;
        private bool _isItalic = false;
        private bool _isBold = false;

        #endregion

        #region 公共属性

        public bool IsDesignTime => _designTime;

        // 画笔
        public Color BackColor { get => _backColor; set => this.RaiseAndSetIfChanged(ref _backColor, value); }
        public Color BorderColor { get => _borderColor; set => this.RaiseAndSetIfChanged(ref _borderColor, value); }
        public Color ForeColor { get => _foreColor; set => this.RaiseAndSetIfChanged(ref _foreColor, value); }

        // 外观
        public double Opacity { get => _opacity; set => this.RaiseAndSetIfChanged(ref _opacity, value); }
        public double BorderThicknessLeft { get => _borderThicknessLeft; set => this.RaiseAndSetIfChanged(ref _borderThicknessLeft, value); }
        public double BorderThicknessTop { get => _borderThicknessTop; set => this.RaiseAndSetIfChanged(ref _borderThicknessTop, value); }
        public double BorderThicknessRight { get => _borderThicknessRight; set => this.RaiseAndSetIfChanged(ref _borderThicknessRight, value); }
        public double BorderThicknessBottom { get => _borderThicknessBottom; set => this.RaiseAndSetIfChanged(ref _borderThicknessBottom, value); }

        // 公共
        public string LabelName { get => _labelName; set => this.RaiseAndSetIfChanged(ref _labelName, value); }
        public decimal Value 
        { 
            get => _value; 
            set 
            {
                if (_value != value)
                {
                    this.RaiseAndSetIfChanged(ref _value, value);
                    // 可以在这里触发事件通知 Model 更新
                    if (_model != null && _model.CommonInfo != null && !_designTime)
                    {
                        // 通常由操作原子更新，但如果是界面交互导致的变更，需要回写？
                        // RadioButtonView 中使用了 OneWayBind 并在 Checked 事件中手动回写
                        // 这里如果是双向绑定，或者在View中处理 ValueChanged
                    }
                }
            } 
        }
        public decimal Minimum { get => _minimum; set => this.RaiseAndSetIfChanged(ref _minimum, value); }
        public decimal Maximum { get => _maximum; set => this.RaiseAndSetIfChanged(ref _maximum, value); }
        public decimal Increment { get => _increment; set => this.RaiseAndSetIfChanged(ref _increment, value); }
        public int DecimalPlaces { get => _decimalPlaces; set => this.RaiseAndSetIfChanged(ref _decimalPlaces, value); }
        public bool IsEnabled { get => _isEnabled; set => this.RaiseAndSetIfChanged(ref _isEnabled, value); }
        public string ToolTip { get => _toolTip; set => this.RaiseAndSetIfChanged(ref _toolTip, value); }

        // 布局
        public HorizontalAlignType HorizontalAlign { get => _horizontalAlign; set => this.RaiseAndSetIfChanged(ref _horizontalAlign, value); }
        public VerticalAlignType VerticalAlign { get => _verticalAlign; set => this.RaiseAndSetIfChanged(ref _verticalAlign, value); }
        public double MarginTop { get => _marginTop; set => this.RaiseAndSetIfChanged(ref _marginTop, value); }
        public double MarginLeft { get => _marginLeft; set => this.RaiseAndSetIfChanged(ref _marginLeft, value); }
        public double MarginBottom { get => _marginBottom; set => this.RaiseAndSetIfChanged(ref _marginBottom, value); }
        public double MarginRight { get => _marginRight; set => this.RaiseAndSetIfChanged(ref _marginRight, value); }
        public double MinWidth { get => _minWidth; set => this.RaiseAndSetIfChanged(ref _minWidth, value); }
        public double MaxWidth { get => _maxWidth; set => this.RaiseAndSetIfChanged(ref _maxWidth, value); }
        public double MinHeight { get => _minHeight; set => this.RaiseAndSetIfChanged(ref _minHeight, value); }
        public double MaxHeight { get => _maxHeight; set => this.RaiseAndSetIfChanged(ref _maxHeight, value); }

        // 文本
        public FontFamilyType FontFamilyType
        {
            get => _fontFamilyType;
            set
            {
                if (_fontFamilyType == value)
                    return;
                this.RaiseAndSetIfChanged(ref _fontFamilyType, value);
                this.RaisePropertyChanged(nameof(FontFamily));
                this.RaisePropertyChanged(nameof(FontFamilyObj));
            }
        }

        public string FontFamily => GroupPanelTextInfo.GetFontFamilyName(_fontFamilyType);

        public FontFamily FontFamilyObj => new FontFamily(FontFamily ?? "Microsoft YaHei");
        public Color FontColor { get => _fontColor; set => this.RaiseAndSetIfChanged(ref _fontColor, value); }
        public double FontSize { get => _fontSize; set => this.RaiseAndSetIfChanged(ref _fontSize, value); }
        public bool IsItalic { get => _isItalic; set => this.RaiseAndSetIfChanged(ref _isItalic, value); }
        public bool IsBold { get => _isBold; set => this.RaiseAndSetIfChanged(ref _isBold, value); }

        public StepperPropertyModelEdit Model => _model;

        #endregion

        #region 构造函数

        public StepperViewModel(bool designTime, StepperPropertyModelEdit model)
        {
            _designTime = designTime;
            _model = model;
            InitializeFromModel(model);

            if (_model != null)
            {
                if (_model.LayoutInfo != null)
                {
                    _model.LayoutInfo.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == nameof(StepperLayoutInfo.MinWidth)) MinWidth = _model.LayoutInfo.MinWidth;
                        if (e.PropertyName == nameof(StepperLayoutInfo.MaxWidth)) MaxWidth = _model.LayoutInfo.MaxWidth;
                        if (e.PropertyName == nameof(StepperLayoutInfo.MinHeight)) MinHeight = _model.LayoutInfo.MinHeight;
                        if (e.PropertyName == nameof(StepperLayoutInfo.MaxHeight)) MaxHeight = _model.LayoutInfo.MaxHeight;
                        
                        // Sync other layout properties if needed
                        if (e.PropertyName == nameof(StepperLayoutInfo.MarginLeft)) MarginLeft = _model.LayoutInfo.MarginLeft;
                        if (e.PropertyName == nameof(StepperLayoutInfo.MarginTop)) MarginTop = _model.LayoutInfo.MarginTop;
                        if (e.PropertyName == nameof(StepperLayoutInfo.MarginRight)) MarginRight = _model.LayoutInfo.MarginRight;
                        if (e.PropertyName == nameof(StepperLayoutInfo.MarginBottom)) MarginBottom = _model.LayoutInfo.MarginBottom;
                        if (e.PropertyName == nameof(StepperLayoutInfo.HorizontalAlign)) HorizontalAlign = _model.LayoutInfo.HorizontalAlign;
                        if (e.PropertyName == nameof(StepperLayoutInfo.VerticalAlign)) VerticalAlign = _model.LayoutInfo.VerticalAlign;
                    };
                }
            }
        }

        #endregion

        #region 私有方法

        private void InitializeFromModel(StepperPropertyModelEdit model)
        {
            if (model == null) return;

            // 画笔设置
            if (model.BrushInfo != null)
            {
                _backColor = model.BrushInfo.BackColor;
                _borderColor = model.BrushInfo.BorderColor;
                _foreColor = model.BrushInfo.ForeColor;
            }

            // 外观设置
            if (model.AppearanceInfo != null)
            {
                _opacity = model.AppearanceInfo.Opacity;
                _borderThicknessLeft = model.AppearanceInfo.BorderThicknessLeft;
                _borderThicknessTop = model.AppearanceInfo.BorderThicknessTop;
                _borderThicknessRight = model.AppearanceInfo.BorderThicknessRight;
                _borderThicknessBottom = model.AppearanceInfo.BorderThicknessBottom;
            }

            // 公共设置
            if (model.CommonInfo != null)
            {
                _labelName = model.CommonInfo.LabelName;
                _value = model.CommonInfo.Value;
                _minimum = model.CommonInfo.Minimum;
                _maximum = model.CommonInfo.Maximum;
                _increment = model.CommonInfo.Increment;
                _decimalPlaces = model.CommonInfo.DecimalPlaces;
                _isEnabled = model.CommonInfo.IsEnabled;
                _toolTip = model.CommonInfo.ToolTip;
            }

            // 布局设置
            if (model.LayoutInfo != null)
            {
                // 宽高主数据统一走父类 Width/Height，LayoutInfo 仅保留旧页面兼容镜像。
            _horizontalAlign = model.LayoutInfo.HorizontalAlign;
            _verticalAlign = model.LayoutInfo.VerticalAlign;
                _marginTop = model.LayoutInfo.MarginTop;
                _marginLeft = model.LayoutInfo.MarginLeft;
                _marginBottom = model.LayoutInfo.MarginBottom;
                _marginRight = model.LayoutInfo.MarginRight;
                _minWidth = model.LayoutInfo.MinWidth;
                _maxWidth = model.LayoutInfo.MaxWidth;
                _minHeight = model.LayoutInfo.MinHeight;
                _maxHeight = model.LayoutInfo.MaxHeight;
            }

            // 文本设置
            if (model.TextInfo != null)
            {
                _fontFamilyType = model.TextInfo.FontFamilyType;
                _fontColor = model.TextInfo.FontColor;
                _fontSize = model.TextInfo.FontSize;
                _isItalic = model.TextInfo.IsItalic;
                _isBold = model.TextInfo.IsBold;
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
        }

        #endregion
    }
}
