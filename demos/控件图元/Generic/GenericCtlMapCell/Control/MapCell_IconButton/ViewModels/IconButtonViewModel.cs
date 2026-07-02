using System;
using System.Reactive;
using Avalonia;
using Avalonia.Media;
using ReactiveUI;

namespace GKG.Map.MapCell.Generic.IconButton
{
    /// <summary>
    /// 图标按钮视图模型
    /// </summary>
    public class IconButtonViewModel : PropertyModels.ComponentModel.ReactiveObject, IDisposable
    {
        private readonly IconButtonPropertyModelEdit _propertyModel;
        private readonly Action<Point> _clickAction;
        private readonly Action<Point> _mouseDownAction;
        private readonly Action<Point> _mouseUpAction;
        private readonly Action<Point> _mouseLeaveAction;
        private readonly Action<Point> _mouseDoubleClickAction;
        private readonly Action<Point> _mouseRightClickAction;
        private bool _disposed;
        public IconButtonBrushInfo BrushInfo => _propertyModel.BrushInfo;

        public IconButtonAppearanceInfo AppearanceInfo => _propertyModel.AppearanceInfo;

        public IconButtonCommonInfo CommonInfo => _propertyModel.CommonInfo;

        public IconButtonLayoutInfo LayoutInfo => _propertyModel.LayoutInfo;
        public IconButtonPropertyModelEdit Model => _propertyModel;

        public IconButtonFontInfo FontInfo => _propertyModel.FontInfo;

        public IconButtonParagraphInfo ParagraphInfo => _propertyModel.ParagraphInfo;

        public IconButtonMiscInfo MiscInfo => _propertyModel.MiscInfo;

        public ReactiveCommand<Point, Unit> ButtonClickCommand { get; }

        public ReactiveCommand<Point, Unit> MouseDownCommand { get; }

        public ReactiveCommand<Point, Unit> MouseUpCommand { get; }

        public ReactiveCommand<Point, Unit> MouseLeaveCommand { get; }

        public ReactiveCommand<Point, Unit> MouseDoubleClickCommand { get; }

        public ReactiveCommand<Point, Unit> MouseRightClickCommand { get; }

        public double OpacityValue => 1.0 - AppearanceInfo.Opacity / 100.0;

        public bool IsTooltipEnabled => !string.IsNullOrEmpty(CommonInfo.TooltipText);

        public IconButtonViewModel(
            IconButtonPropertyModelEdit propertyModel,
            Action<Point> clickAction,
            Action<Point> mouseDownAction,
            Action<Point> mouseUpAction,
            Action<Point> mouseLeaveAction,
            Action<Point> mouseDoubleClickAction,
            Action<Point> mouseRightClickAction)
        {
            _propertyModel = propertyModel;
            _clickAction = clickAction;
            _mouseDownAction = mouseDownAction;
            _mouseUpAction = mouseUpAction;
            _mouseLeaveAction = mouseLeaveAction;
            _mouseDoubleClickAction = mouseDoubleClickAction;
            _mouseRightClickAction = mouseRightClickAction;
            ButtonClickCommand = ReactiveCommand.Create<Point>(OnButtonClicked);
            MouseDownCommand = ReactiveCommand.Create<Point>(OnMouseDown);
            MouseUpCommand = ReactiveCommand.Create<Point>(OnMouseUp);
            MouseLeaveCommand = ReactiveCommand.Create<Point>(OnMouseLeave);
            MouseDoubleClickCommand = ReactiveCommand.Create<Point>(OnMouseDoubleClick);
            MouseRightClickCommand = ReactiveCommand.Create<Point>(OnMouseRightClick);

            _propertyModel.PropertyChanged += PropertyModel_PropertyChanged;
            _propertyModel.BrushInfo.PropertyChanged += ChildPropertyChanged;
            _propertyModel.AppearanceInfo.PropertyChanged += ChildPropertyChanged;
            _propertyModel.LayoutInfo.PropertyChanged += ChildPropertyChanged;
            _propertyModel.CommonInfo.PropertyChanged += ChildPropertyChanged;
            _propertyModel.FontInfo.PropertyChanged += ChildPropertyChanged;
            _propertyModel.ParagraphInfo.PropertyChanged += ChildPropertyChanged;
            _propertyModel.MiscInfo.PropertyChanged += ChildPropertyChanged;
        }

        public void ReloadFromModel()
        {
            RaisePropertyChanged(nameof(BrushInfo));
            RaisePropertyChanged(nameof(AppearanceInfo));
            RaisePropertyChanged(nameof(CommonInfo));
            RaisePropertyChanged(nameof(LayoutInfo));
            RaisePropertyChanged(nameof(FontInfo));
            RaisePropertyChanged(nameof(ParagraphInfo));
            RaisePropertyChanged(nameof(MiscInfo));
            RaisePropertyChanged(nameof(OpacityValue));
            RaisePropertyChanged(nameof(IsTooltipEnabled));
        }

        private void PropertyModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        private void ChildPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is IconButtonFontInfo && e.PropertyName == nameof(IconButtonFontInfo.FontColor))
            {
                _propertyModel.BrushInfo.ForegroundColor = _propertyModel.FontInfo.FontColor;
            }
            else if (sender is IconButtonBrushInfo && e.PropertyName == nameof(IconButtonBrushInfo.ForegroundColor))
            {
                _propertyModel.FontInfo.FontColor = _propertyModel.BrushInfo.ForegroundColor;
            }
            else if (sender is IconButtonAppearanceInfo && e.PropertyName == nameof(IconButtonAppearanceInfo.Opacity))
            {
                RaisePropertyChanged(nameof(OpacityValue));
            }

            string propertyName = string.Empty;
            if (sender is IconButtonBrushInfo)
            {
                propertyName = nameof(BrushInfo);
            }
            else if (sender is IconButtonAppearanceInfo)
            {
                propertyName = nameof(AppearanceInfo);
            }
            else if (sender is IconButtonLayoutInfo)
            {
                propertyName = nameof(LayoutInfo);
            }
            else if (sender is IconButtonCommonInfo)
            {
                propertyName = nameof(CommonInfo);
            }
            else if (sender is IconButtonFontInfo)
            {
                propertyName = nameof(FontInfo);
            }
            else if (sender is IconButtonParagraphInfo)
            {
                propertyName = nameof(ParagraphInfo);
            }
            else if (sender is IconButtonMiscInfo)
            {
                propertyName = nameof(MiscInfo);
            }

            if (!string.IsNullOrEmpty(propertyName))
            {
                RaisePropertyChanged(propertyName);
                if (sender is IconButtonLayoutInfo
                    && (e.PropertyName == nameof(IconButtonLayoutInfo.HorizontalAlignment)
                    || e.PropertyName == nameof(IconButtonLayoutInfo.VerticalAlignment)))
                {
                    RaisePropertyChanged(nameof(LayoutInfo));
                }

            }
        }

        private void OnButtonClicked(Point screenP)
        {
            _clickAction?.Invoke(screenP);
        }

        private void OnMouseDown(Point screenP)
        {
            _mouseDownAction?.Invoke(screenP);
        }

        private void OnMouseUp(Point screenP)
        {
            _mouseUpAction?.Invoke(screenP);
        }

        private void OnMouseLeave(Point screenP)
        {
            _mouseLeaveAction?.Invoke(screenP);
        }

        private void OnMouseDoubleClick(Point screenP)
        {
            _mouseDoubleClickAction?.Invoke(screenP);
        }

        private void OnMouseRightClick(Point screenP)
        {
            _mouseRightClickAction?.Invoke(screenP);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _propertyModel.PropertyChanged -= PropertyModel_PropertyChanged;
                    _propertyModel.BrushInfo.PropertyChanged -= ChildPropertyChanged;
                    _propertyModel.AppearanceInfo.PropertyChanged -= ChildPropertyChanged;
                    _propertyModel.LayoutInfo.PropertyChanged -= ChildPropertyChanged;
                    _propertyModel.CommonInfo.PropertyChanged -= ChildPropertyChanged;
                    _propertyModel.FontInfo.PropertyChanged -= ChildPropertyChanged;
                    _propertyModel.ParagraphInfo.PropertyChanged -= ChildPropertyChanged;
                    _propertyModel.MiscInfo.PropertyChanged -= ChildPropertyChanged;
                }

                _disposed = true;
            }
        }
    }
}
