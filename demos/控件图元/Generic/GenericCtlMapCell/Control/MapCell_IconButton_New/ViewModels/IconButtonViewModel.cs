using Avalonia.Media;
using ReactiveUI;
using System;
using GKG.Map.MapCell.Generic.Control.Lable; // 使用公用枚举

namespace GKG.Map.MapCell.Generic.IconButton.ViewModels
{
    public class IconButtonViewModel : ReactiveObject, IDisposable
    {
        private readonly IconButtonPropertyModelEdit _model;
        private readonly Action _clickAction;

        // 1. 数据组
        // 智能文本开关
        public bool ShowText => _model.ShowText;
        public string ButtonText => _model.ButtonText;
        public bool ShowIcon => _model.ShowIcon;
        public string IconBase64 => _model.IconBase64;
        public CursorType CursorType => _model.CursorType;
        public bool IsEnabled => _model.IsEnabled;
        public string ToolTip => string.IsNullOrWhiteSpace(_model.ToolTip) ? null : _model.ToolTip;

        // 2. 样式组
        public Color BackgroundColor => _model.BackgroundColor;
        public Color ForegroundColor => _model.ForegroundColor;
        public Color BorderBrush => _model.BorderBrush;
        public string BorderThickness => _model.BorderThickness;
        public string CornerRadius => _model.CornerRadius;
        public double Opacity => _model.Opacity;

        // 3. 字体组
        public int FontSize => _model.FontSize;
        public string FontFamily => _model.FontFamily;
        public bool IsBold => _model.IsBold;
        public bool IsItalic => _model.IsItalic;
        public bool IsUnderline => _model.IsUnderline;

        // 4. 布局组
        public IconPlacementType IconPlacement => _model.IconPlacement;
        public double IconSize => _model.IconSize;
        public double IconSpacing => _model.IconSpacing;
        // 智能计算间距：如果隐藏了文字，间距直接归零，保证图标绝对居中！
        public string IconMarginLeftStr => ShowText ? $"0,0,{_model.IconSpacing},0" : "0";
        public string IconMarginRightStr => ShowText ? $"{_model.IconSpacing},0,0,0" : "0";
        public string IconMarginTopStr => ShowText ? $"0,0,0,{_model.IconSpacing}" : "0";
        public string IconMarginBottomStr => ShowText ? $"0,{_model.IconSpacing},0,0" : "0";
        public string Margin => _model.Margin;
        public string Padding => _model.Padding;
        public HorizontalAlignType HorizontalAlign => _model.HorizontalAlign;

        public Action OnClickAction { get; set; }
        public Action OnMouseDoubleClickAction { get; set; }
        public Action OnMouseEnterAction { get; set; }
        public Action OnMouseLeaveAction { get; set; }
        public Action OnMouseDownAction { get; set; }
        public Action OnMouseUpAction { get; set; }

        public IconButtonViewModel(IconButtonPropertyModelEdit model)
        {
            _model = model;
        }

        public void NotifyClicked() => OnClickAction?.Invoke();
        public void NotifyMouseDoubleClick() => OnMouseDoubleClickAction?.Invoke();
        public void NotifyMouseEnter() => OnMouseEnterAction?.Invoke();
        public void NotifyMouseLeave() => OnMouseLeaveAction?.Invoke();
        public void NotifyMouseDown() => OnMouseDownAction?.Invoke();
        public void NotifyMouseUp() => OnMouseUpAction?.Invoke();

        public void ReloadFromModel()
        {
            RefreshData();
            RefreshStyle();
            RefreshFont();
            RefreshLayout();
        }

        public void RefreshData()
        {
            this.RaisePropertyChanged(nameof(ShowText));
            this.RaisePropertyChanged(nameof(ButtonText));
            this.RaisePropertyChanged(nameof(ShowIcon));
            this.RaisePropertyChanged(nameof(IconBase64));
            this.RaisePropertyChanged(nameof(CursorType));
            this.RaisePropertyChanged(nameof(IsEnabled));
            this.RaisePropertyChanged(nameof(ToolTip));
        }

        public void RefreshStyle()
        {
            this.RaisePropertyChanged(nameof(BackgroundColor));
            this.RaisePropertyChanged(nameof(ForegroundColor));
            this.RaisePropertyChanged(nameof(BorderBrush));
            this.RaisePropertyChanged(nameof(BorderThickness));
            this.RaisePropertyChanged(nameof(CornerRadius));
            this.RaisePropertyChanged(nameof(Opacity));
        }

        public void RefreshFont()
        {
            this.RaisePropertyChanged(nameof(FontSize));
            this.RaisePropertyChanged(nameof(FontFamily));
            this.RaisePropertyChanged(nameof(IsBold));
            this.RaisePropertyChanged(nameof(IsItalic));
            this.RaisePropertyChanged(nameof(IsUnderline));
        }

        public void RefreshLayout()
        {
            this.RaisePropertyChanged(nameof(IconPlacement));
            this.RaisePropertyChanged(nameof(IconSize));
            this.RaisePropertyChanged(nameof(IconMarginLeftStr));
            this.RaisePropertyChanged(nameof(IconMarginRightStr));
            this.RaisePropertyChanged(nameof(IconMarginTopStr));
            this.RaisePropertyChanged(nameof(IconMarginBottomStr));
            this.RaisePropertyChanged(nameof(Margin));
            this.RaisePropertyChanged(nameof(Padding));
            this.RaisePropertyChanged(nameof(HorizontalAlign));
        }

        public void Dispose() { }
    }
}