using Avalonia.Media;
using ReactiveUI;
using System;

namespace GKG.Map.MapCell.Generic.Control.Lable.ViewModel
{
    /// <summary>
    /// 标签图元视图模型
    /// </summary>
    public class LableViewModel : ReactiveObject, IDisposable
    {
        private readonly LablePropertyModelEdit _model;

        // =========================================================
        // 映射的被动 UI 属性
        // =========================================================

        // 1. 数据组
        public string LableText => _model.LableText;
        public CursorType CursorType => _model.CursorType;
        public bool IsEnabled => _model.IsEnabled;
        public string ToolTip => string.IsNullOrWhiteSpace(_model.ToolTip) ? null : _model.ToolTip;

        // 2. 样式组
        public Color BackgroundColor => _model.BackgroundColor;
        public Color ForegroundColor => _model.ForegroundColor;
        public Color BorderBrush => _model.BorderBrush;
        public string BorderThickness => _model.BorderThickness;
        public double Opacity => _model.Opacity;

        // 3. 字体组
        public int FontSize => _model.FontSize;
        public string FontFamily => _model.FontFamily;
        public bool IsBold => _model.IsBold;
        public bool IsItalic => _model.IsItalic;
        public bool IsUnderline => _model.IsUnderline;
        public double LineHeight => _model.LineHeight;
        public TextAlignType TextAlignment => _model.TextAlignment;

        // 4. 布局组
        public string Margin => _model.Margin;
        public string Padding => _model.Padding;
        public HorizontalAlignType HorizontalAlign => _model.HorizontalAlign;
        public VerticalAlignType VerticalAlign => _model.VerticalAlign;

        public LableViewModel(LablePropertyModelEdit model)
        {
            _model = model;
        }

        // =========================================================
        // 执行器 (Exector) 的四大专用刷新通道
        // =========================================================

        public void ReloadFromModel()
        {
            RefreshData();
            RefreshStyle();
            RefreshFont();
            RefreshLayout();
        }

        public void RefreshData()
        {
            this.RaisePropertyChanged(nameof(LableText));
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
            this.RaisePropertyChanged(nameof(Opacity));
        }

        public void RefreshFont()
        {
            this.RaisePropertyChanged(nameof(FontSize));
            this.RaisePropertyChanged(nameof(FontFamily));
            this.RaisePropertyChanged(nameof(IsBold));
            this.RaisePropertyChanged(nameof(IsItalic));
            this.RaisePropertyChanged(nameof(IsUnderline));
            this.RaisePropertyChanged(nameof(LineHeight));
            this.RaisePropertyChanged(nameof(TextAlignment));
        }

        public void RefreshLayout()
        {
            this.RaisePropertyChanged(nameof(Margin));
            this.RaisePropertyChanged(nameof(Padding));
            this.RaisePropertyChanged(nameof(HorizontalAlign));
            this.RaisePropertyChanged(nameof(VerticalAlign));
        }

        public void Dispose()
        {
            // 无需清理事件监听
        }
    }
}