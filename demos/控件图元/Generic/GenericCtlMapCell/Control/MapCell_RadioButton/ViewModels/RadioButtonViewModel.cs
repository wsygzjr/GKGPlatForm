using Avalonia.Media;
using GKG.Map.MapCell.Generic.GroupPanel;
using GKG.Map.MapCell.Generic.Control.Lable;
using ReactiveUI;
using System;

namespace GKG.Map.MapCell.Generic.RadioButton.ViewModel
{
    /// <summary>
    /// 单选框图元视图模型
    /// 只持有属性，由操作原子执行器更新
    /// </summary>
    public class RadioButtonViewModel : ReactiveObject, IDisposable
    {
        #region 私有字段

        private bool _designTime;
        private RadioButtonPropertyModelEdit _model;
        private Action<string> _eventAction;

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
        private string _text = "单选框";
        private string _groupName = "";
        private bool? _isChecked = false;
        private bool _isThreeState = false;
        private CursorType _cursorType = CursorType.Arrow;
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
        public string Text { get => _text; set => this.RaiseAndSetIfChanged(ref _text, value); }
        public string GroupName { get => _groupName; set => this.RaiseAndSetIfChanged(ref _groupName, value); }
        public bool? IsChecked 
        { 
            get => _isChecked; 
            set 
            {
                var oldValue = _isChecked;
                this.RaiseAndSetIfChanged(ref _isChecked, value);
                if (oldValue != value)
                {
                    // 触发 CheckedChanged 事件
                    _eventAction?.Invoke("CheckedChanged");
                    // 触发 Checked 或 Unchecked 事件
                    if (value == true)
                        _eventAction?.Invoke("Checked");
                    else if (value == false)
                        _eventAction?.Invoke("Unchecked");
                }
            }
        }
        public bool IsThreeState { get => _isThreeState; set => this.RaiseAndSetIfChanged(ref _isThreeState, value); }
        public CursorType CursorType { get => _cursorType; set => this.RaiseAndSetIfChanged(ref _cursorType, value); }
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

        public RadioButtonPropertyModelEdit Model => _model;

        #endregion

        #region 构造函数

        public RadioButtonViewModel(bool designTime, RadioButtonPropertyModelEdit model, Action<string> eventAction = null)
        {
            _designTime = designTime;
            _model = model;
            _eventAction = eventAction;
            InitializeFromModel(model);
        }

        #endregion

        #region 私有方法

        private void InitializeFromModel(RadioButtonPropertyModelEdit model)
        {
            // 画笔设置
            _backColor = model.BrushInfo.BackColor;
            _borderColor = model.BrushInfo.BorderColor;
            _foreColor = model.BrushInfo.ForeColor;

            // 外观设置
            _opacity = model.AppearanceInfo.Opacity;
            _borderThicknessLeft = model.AppearanceInfo.BorderThicknessLeft;
            _borderThicknessTop = model.AppearanceInfo.BorderThicknessTop;
            _borderThicknessRight = model.AppearanceInfo.BorderThicknessRight;
            _borderThicknessBottom = model.AppearanceInfo.BorderThicknessBottom;

            // 公共设置
            _text = model.CommonInfo.Text;
            _groupName = model.CommonInfo.GroupName;
            _isChecked = model.CommonInfo.IsChecked;
            _isThreeState = model.CommonInfo.IsThreeState;
            _cursorType = model.CommonInfo.CursorType;
            _isEnabled = model.CommonInfo.IsEnabled;
            _toolTip = model.CommonInfo.ToolTip;

            // 布局设置
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

            // 文本设置
            _fontFamilyType = model.TextInfo.FontFamilyType;
            _fontColor = model.TextInfo.FontColor;
            _fontSize = model.TextInfo.FontSize;
            _isItalic = model.TextInfo.IsItalic;
            _isBold = model.TextInfo.IsBold;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
        }

        #endregion
    }
}
