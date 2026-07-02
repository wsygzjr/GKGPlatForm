using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Griffins;
using GKG.Map.MapCell.Generic.Control.MapCell_ImageGroup.Objects;
using GKG.Map.MapCell.Generic.Control.Lable;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;

namespace GKG.Map.MapCell.Generic.Control.MapCell_ImageGroup.ViewModels
{
    /// <summary>
    /// 图片组图元视图模型
    /// </summary>
    public class ImageGroupViewModel : ReactiveObject, IDisposable
    {
        private bool _designTime;
        private ImageGroupPropertyModelEdit _propertyModelEdit;
        private readonly Action _mouseClickAction;
        private readonly object _imageLock = new object();
        private CancellationTokenSource _imageCts;

        public ReactiveCommand<Point, Unit> MouseClickCommand { get; }
        public ImageGroupPropertyModelEdit Model => _propertyModelEdit;

        public ImageGroupViewModel()
        {
            MouseClickCommand = ReactiveCommand.Create<Point>(OnMouseClicked);
            _mouseClickAction = () => { };
        }

        public ImageGroupViewModel(bool designTime, ImageGroupPropertyModelEdit propertyModelEdit, Action mouseClickAction)
        {
            _designTime = designTime;
            _propertyModelEdit = propertyModelEdit;
            _mouseClickAction = mouseClickAction ?? (() => { });
            MouseClickCommand = ReactiveCommand.Create<Point>(OnMouseClicked);
            InitializeFromModel(propertyModelEdit);
            UpdateCurrentImage(immediate: true);
        }

        private void InitializeFromModel(ImageGroupPropertyModelEdit model)
        {
            if (model == null) return;

            if (model.AppearanceInfo != null)
            {
                _opacity = model.AppearanceInfo.Opacity;
            }

            if (model.CommonInfo != null)
            {
                _imageSources = model.CommonInfo.ImageSources ?? new List<BitmapData>();
                _currentIndex = model.CommonInfo.CurrentIndex;
                _stretchMode = model.CommonInfo.StretchMode;
                _isEnabled = model.CommonInfo.IsEnabled;
                _toolTip = model.CommonInfo.ToolTip;
            }

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
        }

        private double _opacity = 1.0;
        public double Opacity
        {
            get => _opacity;
            set => this.RaiseAndSetIfChanged(ref _opacity, value);
        }

        private List<BitmapData> _imageSources = new List<BitmapData>();
        public List<BitmapData> ImageSources
        {
            get => _imageSources;
            set
            {
                if (ReferenceEquals(_imageSources, value)) return;
                _imageSources = value ?? new List<BitmapData>();
                this.RaisePropertyChanged(nameof(ImageSources));
                this.RaisePropertyChanged(nameof(CurrentImageSource));
                this.RaisePropertyChanged(nameof(ImageCount));
                UpdateCurrentImage(immediate: false);
            }
        }

        private int _currentIndex;
        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                if (_currentIndex == value) return;
                _currentIndex = value;
                this.RaisePropertyChanged(nameof(CurrentIndex));
                this.RaisePropertyChanged(nameof(CurrentImageSource));
                UpdateCurrentImage(immediate: false);
            }
        }

        private IImage _currentImage;
        public IImage CurrentImage
        {
            get => _currentImage;
            private set => this.RaiseAndSetIfChanged(ref _currentImage, value);
        }

        public BitmapData CurrentImageSource
        {
            get
            {
                if (_imageSources != null && _imageSources.Count > 0)
                {
                    int validIndex = Math.Max(0, Math.Min(_currentIndex, _imageSources.Count - 1));
                    return _imageSources[validIndex];
                }

                return null;
            }
        }

        /// <summary>
        /// 图片组首帧需要立即拿到当前图像，避免切页时先空一帧再补图。
        /// 后续普通切图仍允许走后台解码，但不再先把当前图清空。
        /// </summary>
        private void UpdateCurrentImage(bool immediate)
        {
            CancellationTokenSource newCts;
            lock (_imageLock)
            {
                _imageCts?.Cancel();
                _imageCts?.Dispose();
                _imageCts = new CancellationTokenSource();
                newCts = _imageCts;
            }

            var token = newCts.Token;
            var src = CurrentImageSource;

            if (src == null)
            {
                CurrentImage = null;
                return;
            }

            if (immediate)
            {
                var bmp = TryGetBitmapFromBitmapData(src);
                if (bmp == null || token.IsCancellationRequested)
                {
                    return;
                }

                if (Dispatcher.UIThread.CheckAccess())
                {
                    if (!token.IsCancellationRequested)
                        CurrentImage = bmp;
                }
                else
                {
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        if (!token.IsCancellationRequested)
                            CurrentImage = bmp;
                    }, DispatcherPriority.Send).GetAwaiter().GetResult();
                }

                return;
            }

            Task.Run(() =>
            {
                var bmp = TryGetBitmapFromBitmapData(src);
                if (bmp == null || token.IsCancellationRequested) return;
                Dispatcher.UIThread.Post(() =>
                {
                    if (!token.IsCancellationRequested) CurrentImage = bmp;
                }, DispatcherPriority.Background);
            }, token);
        }

        private static Bitmap TryGetBitmapFromBitmapData(BitmapData bitmapData)
        {
            try
            {
                if (bitmapData == null) return null;
                var bytes = bitmapData.ToBytes();
                if (bytes == null || bytes.Length == 0) return null;
                using var ms = new MemoryStream(bytes);
                return new Bitmap(ms);
            }
            catch { return null; }
        }

        public int ImageCount => _imageSources?.Count ?? 0;

        private ImageGroupStretchMode _stretchMode = ImageGroupStretchMode.Uniform;
        public ImageGroupStretchMode StretchMode
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
            ImageGroupStretchMode.None => Stretch.None,
            ImageGroupStretchMode.Fill => Stretch.Fill,
            ImageGroupStretchMode.Uniform => Stretch.Uniform,
            ImageGroupStretchMode.UniformToFill => Stretch.UniformToFill,
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

        /// <summary>
        /// 读档完成后统一按真实模型值同步刷新，避免图片、可见性、布局分多拍收敛导致首帧闪烁。
        /// </summary>
        public void ReloadFromModel(bool immediateCurrentImage = true)
        {
            var model = _propertyModelEdit;
            if (model == null)
            {
                return;
            }

            Opacity = model.AppearanceInfo?.Opacity ?? 1.0;

            _imageSources = model.CommonInfo?.ImageSources ?? new List<BitmapData>();
            _currentIndex = model.CommonInfo?.CurrentIndex ?? 0;
            _stretchMode = model.CommonInfo?.StretchMode ?? ImageGroupStretchMode.Uniform;
            _isEnabled = model.CommonInfo?.IsEnabled ?? true;
            _toolTip = model.CommonInfo?.ToolTip ?? string.Empty;

            _horizontalAlign = model.LayoutInfo?.HorizontalAlign ?? HorizontalAlignType.Stretch;
            _verticalAlign = model.LayoutInfo?.VerticalAlign ?? VerticalAlignType.Stretch;
            _marginTop = model.LayoutInfo?.MarginTop ?? 0;
            _marginLeft = model.LayoutInfo?.MarginLeft ?? 0;
            _marginBottom = model.LayoutInfo?.MarginBottom ?? 0;
            _marginRight = model.LayoutInfo?.MarginRight ?? 0;
            _minWidth = model.LayoutInfo?.MinWidth ?? 0;
            _maxWidth = model.LayoutInfo?.MaxWidth ?? 0;
            _minHeight = model.LayoutInfo?.MinHeight ?? 0;
            _maxHeight = model.LayoutInfo?.MaxHeight ?? 0;

            this.RaisePropertyChanged(nameof(Opacity));
            this.RaisePropertyChanged(nameof(ImageSources));
            this.RaisePropertyChanged(nameof(CurrentIndex));
            this.RaisePropertyChanged(nameof(CurrentImageSource));
            this.RaisePropertyChanged(nameof(ImageCount));
            this.RaisePropertyChanged(nameof(StretchMode));
            this.RaisePropertyChanged(nameof(Stretch));
            this.RaisePropertyChanged(nameof(IsEnabled));
            this.RaisePropertyChanged(nameof(ToolTip));
            this.RaisePropertyChanged(nameof(ToolTipTip));
            this.RaisePropertyChanged(nameof(HorizontalAlign));
            this.RaisePropertyChanged(nameof(VerticalAlign));
            this.RaisePropertyChanged(nameof(MarginTop));
            this.RaisePropertyChanged(nameof(MarginLeft));
            this.RaisePropertyChanged(nameof(MarginBottom));
            this.RaisePropertyChanged(nameof(MarginRight));
            this.RaisePropertyChanged(nameof(MinWidth));
            this.RaisePropertyChanged(nameof(MaxWidth));
            this.RaisePropertyChanged(nameof(MinHeight));
            this.RaisePropertyChanged(nameof(MaxHeight));

            UpdateCurrentImage(immediateCurrentImage);
        }

        public void Dispose()
        {
            lock (_imageLock)
            {
                _imageCts?.Cancel();
                _imageCts?.Dispose();
                _imageCts = null;
            }
        }
    }
}
