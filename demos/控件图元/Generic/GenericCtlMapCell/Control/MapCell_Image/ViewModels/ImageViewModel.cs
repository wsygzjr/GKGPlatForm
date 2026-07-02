using Avalonia;
using Avalonia.Media;
using Griffins;
using GKG.Map.MapCell.Generic.Control.MapCell_Image.Objects;
using GKG.Map.MapCell.Generic.Control.Lable;
using ReactiveUI;
using System;
using System.Reactive;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Image.ViewModels
{
    /// <summary>
    /// 图片图元视图模型
    /// </summary>
    public class ImageViewModel : ReactiveObject, IDisposable
    {
        private bool _designTime;
        private ImagePropertyModelEdit _propertyModelEdit;
        private readonly Action _mouseClickAction;

        public ImagePropertyModelEdit Model => _propertyModelEdit;

        public ReactiveCommand<Point, Unit> MouseClickCommand { get; }

        public ImageViewModel()
        {
            MouseClickCommand = ReactiveCommand.Create<Point>(OnMouseClicked);
            _mouseClickAction = () => { };
        }

        public ImageViewModel(bool designTime, ImagePropertyModelEdit propertyModelEdit, Action mouseClickAction)
        {
            _designTime = designTime;
            _propertyModelEdit = propertyModelEdit;
            _mouseClickAction = mouseClickAction ?? (() => { });
            MouseClickCommand = ReactiveCommand.Create<Point>(OnMouseClicked);
            InitializeFromModel(propertyModelEdit);
        }

        private void InitializeFromModel(ImagePropertyModelEdit model)
        {
            if (model == null) return;

            if (model.AppearanceInfo != null)
            {
                _opacity = model.AppearanceInfo.Opacity;
            }

            if (model.CommonInfo != null)
            {
                _imageSource = model.CommonInfo.ImageSource;
                _stretchMode = model.CommonInfo.StretchMode;
                _isEnabled = model.CommonInfo.IsEnabled;
                _toolTip = model.CommonInfo.ToolTip;
            }

            if (model.LayoutInfo != null)
            {
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
        }

        private double _opacity = 1.0;
        public double Opacity
        {
            get => _opacity;
            set => this.RaiseAndSetIfChanged(ref _opacity, value);
        }

        private BitmapData _imageSource = new BitmapData();
        public BitmapData ImageSource
        {
            get => _imageSource;
            set => this.RaiseAndSetIfChanged(ref _imageSource, value ?? new BitmapData());
        }

        private ImageStretchMode _stretchMode = ImageStretchMode.Uniform;
        public ImageStretchMode StretchMode
        {
            get => _stretchMode;
            set
            {
                this.RaiseAndSetIfChanged(ref _stretchMode, value);
                this.RaisePropertyChanged(nameof(Stretch));
            }
        }

        public Stretch Stretch => StretchMode switch
        {
            ImageStretchMode.None => Stretch.None,
            ImageStretchMode.Fill => Stretch.Fill,
            ImageStretchMode.Uniform => Stretch.Uniform,
            ImageStretchMode.UniformToFill => Stretch.UniformToFill,
            _ => Stretch.Uniform
        };

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;
            set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
        }

        private string _toolTip = "";
        public string ToolTip
        {
            get => _toolTip;
            set
            {
                this.RaiseAndSetIfChanged(ref _toolTip, value);
                this.RaisePropertyChanged(nameof(ToolTipTip));
            }
        }

        public object ToolTipTip => string.IsNullOrWhiteSpace(_toolTip) ? null : _toolTip;

        private HorizontalAlignType _horizontalAlign = HorizontalAlignType.Stretch;
        public HorizontalAlignType HorizontalAlign
        {
            get => _horizontalAlign;
            set => this.RaiseAndSetIfChanged(ref _horizontalAlign, value);
        }

        private VerticalAlignType _verticalAlign = VerticalAlignType.Stretch;
        public VerticalAlignType VerticalAlign
        {
            get => _verticalAlign;
            set => this.RaiseAndSetIfChanged(ref _verticalAlign, value);
        }

        private double _marginTop;
        public double MarginTop
        {
            get => _marginTop;
            set => this.RaiseAndSetIfChanged(ref _marginTop, value);
        }

        private double _marginLeft;
        public double MarginLeft
        {
            get => _marginLeft;
            set => this.RaiseAndSetIfChanged(ref _marginLeft, value);
        }

        private double _marginBottom;
        public double MarginBottom
        {
            get => _marginBottom;
            set => this.RaiseAndSetIfChanged(ref _marginBottom, value);
        }

        private double _marginRight;
        public double MarginRight
        {
            get => _marginRight;
            set => this.RaiseAndSetIfChanged(ref _marginRight, value);
        }

        private double _minWidth;
        public double MinWidth
        {
            get => _minWidth;
            set => this.RaiseAndSetIfChanged(ref _minWidth, value);
        }

        private double _maxWidth;
        public double MaxWidth
        {
            get => _maxWidth;
            set => this.RaiseAndSetIfChanged(ref _maxWidth, value);
        }

        private double _minHeight;
        public double MinHeight
        {
            get => _minHeight;
            set => this.RaiseAndSetIfChanged(ref _minHeight, value);
        }

        private double _maxHeight;
        public double MaxHeight
        {
            get => _maxHeight;
            set => this.RaiseAndSetIfChanged(ref _maxHeight, value);
        }

        private void OnMouseClicked(Point screenPoint)
        {
            _mouseClickAction?.Invoke();
        }

        public void Dispose()
        {
        }
    }
}
