using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using GKG.Map.MapCell.Generic.Control.MapCell_IconButton.Converts;
using GKG.Map.MapCell.Generic.Control.MapCell_TextBox;
using GKG.Map.MapCell.Generic.Control.MapCell_TextBox.Converts;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace GKG.Map.MapCell.Generic.IconButton
{
    public partial class IconButtonView : ReactiveUserControl<IconButtonViewModel>
    {
        private static readonly BitmapDataToImageSourceConverter BitmapToImageSourceConverter = new();
        private static readonly BitmapDataAndColorToTintedImageSourceConverter TintedImageSourceConverter = new();
        private bool _isPointerOver;
        private bool _isPointerPressed;
        private bool _isMutualSelected;
        private bool _hasPendingInitialLayoutStabilizer;
        private DispatcherTimer _initialStabilizeTimer;
        private int _initialStabilizeTickCount;

        public IconButtonView()
        {
            InitializeComponent();
            DataContextChanged += (_, _) => ApplyInitialStateFromViewModel();
            AttachedToVisualTree += (_, _) =>
            {
                ApplyInitialStateFromViewModel();
                RegisterInitialLayoutStabilizer();
                RegisterDeferredInitialApply();
                StartInitialStabilizeTimer();
            };
            DetachedFromVisualTree += (_, _) => StopInitialStabilizeTimer();

            this.WhenActivated(disposables =>
            {
                if (ViewModel == null)
                {
                    return;
                }

                Point GetScreenPoint()
                {
                    PixelPoint screenPixel = CustomButton.PointToScreen(new Point(0, 0));
                    return new Point(screenPixel.X, screenPixel.Y);
                }

                Observable.FromEventPattern<PointerPressedEventArgs>(
                        addHandler: h => CustomButton.PointerPressed += h,
                        removeHandler: h => CustomButton.PointerPressed -= h
                    )
                    .Where(x => x.EventArgs.GetCurrentPoint(CustomButton).Properties.IsLeftButtonPressed)
                    .Do(_ => SetPointerPressed(true))
                    .Select(_ => GetScreenPoint())
                    .InvokeCommand(ViewModel, vm => vm.MouseDownCommand)
                    .DisposeWith(disposables);

                Observable.FromEventPattern<PointerReleasedEventArgs>(
                        addHandler: h => CustomButton.PointerReleased += h,
                        removeHandler: h => CustomButton.PointerReleased -= h
                    )
                    .Where(x => x.EventArgs.InitialPressMouseButton == MouseButton.Left)
                    .Do(_ => SetPointerPressed(false))
                    .Select(_ => GetScreenPoint())
                    .InvokeCommand(ViewModel, vm => vm.MouseUpCommand)
                    .DisposeWith(disposables);

                Observable.FromEventPattern<PointerEventArgs>(
                        addHandler: h => CustomButton.PointerEntered += h,
                        removeHandler: h => CustomButton.PointerEntered -= h
                    )
                    .Do(_ => SetPointerOver(true))
                    .Subscribe(_ => { })
                    .DisposeWith(disposables);

                Observable.FromEventPattern<PointerEventArgs>(
                        addHandler: h => CustomButton.PointerExited += h,
                        removeHandler: h => CustomButton.PointerExited -= h
                    )
                    .Do(_ =>
                    {
                        SetPointerOver(false);
                        SetPointerPressed(false);
                    })
                    .Select(_ => GetScreenPoint())
                    .InvokeCommand(ViewModel, vm => vm.MouseLeaveCommand)
                    .DisposeWith(disposables);

                Observable.FromEventPattern<TappedEventArgs>(
                        addHandler: h => CustomButton.DoubleTapped += h,
                        removeHandler: h => CustomButton.DoubleTapped -= h
                    )
                    .Select(_ => GetScreenPoint())
                    .InvokeCommand(ViewModel, vm => vm.MouseDoubleClickCommand)
                    .DisposeWith(disposables);

                Observable.FromEventPattern<PointerReleasedEventArgs>(
                        addHandler: h => CustomButton.PointerReleased += h,
                        removeHandler: h => CustomButton.PointerReleased -= h
                    )
                    .Where(x => x.EventArgs.InitialPressMouseButton == MouseButton.Right)
                    .Do(_ => SetPointerPressed(false))
                    .Select(_ => GetScreenPoint())
                    .InvokeCommand(ViewModel, vm => vm.MouseRightClickCommand)
                    .DisposeWith(disposables);

                var clickPoints = Observable.FromEventPattern<RoutedEventArgs>(
                        addHandler: h => CustomButton.Click += h,
                        removeHandler: h => CustomButton.Click -= h
                    )
                    .Select(_ => GetScreenPoint());

                this.BindCommand<IconButtonView, IconButtonViewModel, ReactiveCommand<Point, Unit>, Avalonia.Controls.Button, Point>(
                    viewModel: ViewModel,
                    propertyName: vm => vm.ButtonClickCommand,
                    controlName: v => v.CustomButton,
                    withParameter: clickPoints)
                    .DisposeWith(disposables);

                ViewModel.WhenAnyValue(vm => vm.CommonInfo.TooltipText)
                    .Subscribe(tooltipText =>
                    {
                        if (!string.IsNullOrWhiteSpace(tooltipText))
                            ToolTip.SetTip(CustomButton, tooltipText);
                        else
                            ToolTip.SetTip(CustomButton, null);
                    })
                    .DisposeWith(disposables);

                ViewModel.WhenAnyValue(vm => vm.CommonInfo.HoverCursor)
                    .Subscribe(ApplyHoverCursor)
                    .DisposeWith(disposables);

                // 图标按钮试点按图片图元的尺寸承接方式收口：外层尺寸由宿主统一承接，这里不再重复直赋 Width/Height。

                Observable.CombineLatest(
                        ViewModel.WhenAnyValue(vm => vm.LayoutInfo.MarginLeft),
                        ViewModel.WhenAnyValue(vm => vm.LayoutInfo.MarginTop),
                        ViewModel.WhenAnyValue(vm => vm.LayoutInfo.MarginRight),
                        ViewModel.WhenAnyValue(vm => vm.LayoutInfo.MarginBottom),
                        (left, top, right, bottom) => new Thickness(left, top, right, bottom)
                    )
                    .Subscribe(margin => Margin = margin)
                    .DisposeWith(disposables);

                Observable.CombineLatest(
                        ViewModel.WhenAnyValue(vm => vm.AppearanceInfo.BorderThicknessLeft),
                        ViewModel.WhenAnyValue(vm => vm.AppearanceInfo.BorderThicknessTop),
                        ViewModel.WhenAnyValue(vm => vm.AppearanceInfo.BorderThicknessRight),
                        ViewModel.WhenAnyValue(vm => vm.AppearanceInfo.BorderThicknessBottom),
                        (left, top, right, bottom) => new Thickness(
                            Math.Max(0, left),
                            Math.Max(0, top),
                            Math.Max(0, right),
                            Math.Max(0, bottom)))
                    .Subscribe(_ => ApplyButtonChromeState())
                    .DisposeWith(disposables);

                ViewModel.WhenAnyValue(vm => vm.MiscInfo.IconPosition)
                    .Subscribe(iconPosition =>
                    {
                        UpdateContentLayout(iconPosition);
                        ApplyContentSlotLayout(iconPosition);
                        InvalidateInitialLayoutTargets();
                    })
                    .DisposeWith(disposables);

                Observable.Merge(
                        ViewModel.WhenAnyValue(vm => vm.MiscInfo.IconSource).Select(_ => Unit.Default),
                        ViewModel.WhenAnyValue(vm => vm.MiscInfo.IconWidth).Select(_ => Unit.Default),
                        ViewModel.WhenAnyValue(vm => vm.MiscInfo.IconHeight).Select(_ => Unit.Default),
                        ViewModel.WhenAnyValue(vm => vm.BrushInfo.HoverForegroundColor).Select(_ => Unit.Default),
                        ViewModel.WhenAnyValue(vm => vm.BrushInfo.ClickForegroundColor).Select(_ => Unit.Default))
                    .Subscribe(_ =>
                    {
                        ApplyIconVisualState();
                        InvalidateInitialLayoutTargets();
                    })
                    .DisposeWith(disposables);

                Observable.CombineLatest(
                        ViewModel.WhenAnyValue(vm => vm.ParagraphInfo.ParagraphSpacingBefore),
                        ViewModel.WhenAnyValue(vm => vm.ParagraphInfo.ParagraphSpacingAfter),
                        (before, after) => new Thickness(0, Math.Max(0, before), 0, Math.Max(0, after)))
                    .Subscribe(margin => TextParagraphHost.Margin = margin)
                    .DisposeWith(disposables);

                // 这里统一监听会被 ApplyButtonChromeState 直接写入到按钮本地值的外观属性。
                // 否则属性面板只改这些值时，XAML 绑定会被本地值覆盖，看起来就像“改了没反应”。
                Observable.Merge(
                        ViewModel.WhenAnyValue(vm => vm.BrushInfo.BackgroundColor).Select(_ => Unit.Default),
                        ViewModel.WhenAnyValue(vm => vm.BrushInfo.HoverBackgroundColor).Select(_ => Unit.Default),
                        ViewModel.WhenAnyValue(vm => vm.BrushInfo.ClickBackgroundColor).Select(_ => Unit.Default),
                        ViewModel.WhenAnyValue(vm => vm.BrushInfo.BorderColor).Select(_ => Unit.Default),
                        ViewModel.WhenAnyValue(vm => vm.BrushInfo.ForegroundColor).Select(_ => Unit.Default),
                        ViewModel.WhenAnyValue(vm => vm.BrushInfo.HoverForegroundColor).Select(_ => Unit.Default),
                        ViewModel.WhenAnyValue(vm => vm.BrushInfo.ClickForegroundColor).Select(_ => Unit.Default),
                        ViewModel.WhenAnyValue(vm => vm.BrushInfo.ClickBorderColorLeft).Select(_ => Unit.Default),
                        ViewModel.WhenAnyValue(vm => vm.BrushInfo.ClickBorderColorTop).Select(_ => Unit.Default),
                        ViewModel.WhenAnyValue(vm => vm.BrushInfo.ClickBorderColorRight).Select(_ => Unit.Default),
                        ViewModel.WhenAnyValue(vm => vm.BrushInfo.ClickBorderColorBottom).Select(_ => Unit.Default),
                        ViewModel.WhenAnyValue(vm => vm.AppearanceInfo.ClickBorderThicknessLeft).Select(_ => Unit.Default),
                        ViewModel.WhenAnyValue(vm => vm.AppearanceInfo.ClickBorderThicknessTop).Select(_ => Unit.Default),
                        ViewModel.WhenAnyValue(vm => vm.AppearanceInfo.ClickBorderThicknessRight).Select(_ => Unit.Default),
                        ViewModel.WhenAnyValue(vm => vm.AppearanceInfo.ClickBorderThicknessBottom).Select(_ => Unit.Default),
                        ViewModel.WhenAnyValue(vm => vm.MiscInfo.BackgroundImage).Select(_ => Unit.Default),
                        ViewModel.WhenAnyValue(vm => vm.MiscInfo.ImageSizeMode).Select(_ => Unit.Default))
                    .Subscribe(_ => ApplyButtonChromeState())
                    .DisposeWith(disposables);

                ViewModel.WhenAnyValue(vm => vm.LayoutInfo.HorizontalAlignment)
                    .Subscribe(alignment =>
                    {
                        HorizontalAlignment = alignment switch
                        {
                            IconButtonLayoutInfo.HorizontalAlignmentEnum.Left => Avalonia.Layout.HorizontalAlignment.Left,
                            IconButtonLayoutInfo.HorizontalAlignmentEnum.Center => Avalonia.Layout.HorizontalAlignment.Center,
                            IconButtonLayoutInfo.HorizontalAlignmentEnum.Right => Avalonia.Layout.HorizontalAlignment.Right,
                            _ => Avalonia.Layout.HorizontalAlignment.Stretch
                        };
                    })
                    .DisposeWith(disposables);

                ViewModel.WhenAnyValue(vm => vm.LayoutInfo.VerticalAlignment)
                    .Subscribe(alignment =>
                    {
                        VerticalAlignment = alignment switch
                        {
                            IconButtonLayoutInfo.VerticalAlignmentEnum.Top => Avalonia.Layout.VerticalAlignment.Top,
                            IconButtonLayoutInfo.VerticalAlignmentEnum.Center => Avalonia.Layout.VerticalAlignment.Center,
                            IconButtonLayoutInfo.VerticalAlignmentEnum.Bottom => Avalonia.Layout.VerticalAlignment.Bottom,
                            _ => Avalonia.Layout.VerticalAlignment.Stretch
                        };
                    })
                    .DisposeWith(disposables);

                EventHandler<VisualTreeAttachmentEventArgs> detachedHandler = (_, _) =>
                {
                    SetPointerOver(false);
                    SetPointerPressed(false);
                    ApplyMutualSelectionState(false);
                };

                DetachedFromVisualTree += detachedHandler;
                Disposable.Create(() =>
                {
                    DetachedFromVisualTree -= detachedHandler;
                    SetPointerOver(false);
                    SetPointerPressed(false);
                    ApplyMutualSelectionState(false);
                }).DisposeWith(disposables);

                ApplyInitialStateFromViewModel();
            });
        }

        /// <summary>
        /// 运行时切页首帧先按当前 ViewModel 的真实值同步落位，避免等响应式订阅分多拍收敛时出现图标和文字瞬间叠在一起。
        /// </summary>
        public void ApplyInitialStateFromViewModel()
        {
            if (ViewModel == null)
            {
                return;
            }

            Margin = new Thickness(
                ViewModel.LayoutInfo.MarginLeft,
                ViewModel.LayoutInfo.MarginTop,
                ViewModel.LayoutInfo.MarginRight,
                ViewModel.LayoutInfo.MarginBottom);

            HorizontalAlignment = ViewModel.LayoutInfo.HorizontalAlignment switch
            {
                IconButtonLayoutInfo.HorizontalAlignmentEnum.Left => Avalonia.Layout.HorizontalAlignment.Left,
                IconButtonLayoutInfo.HorizontalAlignmentEnum.Center => Avalonia.Layout.HorizontalAlignment.Center,
                IconButtonLayoutInfo.HorizontalAlignmentEnum.Right => Avalonia.Layout.HorizontalAlignment.Right,
                _ => Avalonia.Layout.HorizontalAlignment.Stretch
            };

            VerticalAlignment = ViewModel.LayoutInfo.VerticalAlignment switch
            {
                IconButtonLayoutInfo.VerticalAlignmentEnum.Top => Avalonia.Layout.VerticalAlignment.Top,
                IconButtonLayoutInfo.VerticalAlignmentEnum.Center => Avalonia.Layout.VerticalAlignment.Center,
                IconButtonLayoutInfo.VerticalAlignmentEnum.Bottom => Avalonia.Layout.VerticalAlignment.Bottom,
                _ => Avalonia.Layout.VerticalAlignment.Stretch
            };

            UpdateContentLayout(ViewModel.MiscInfo.IconPosition);
            ApplyContentSlotLayout(ViewModel.MiscInfo.IconPosition);
            TextParagraphHost.Margin = new Thickness(
                0,
                Math.Max(0, ViewModel.ParagraphInfo.ParagraphSpacingBefore),
                0,
                Math.Max(0, ViewModel.ParagraphInfo.ParagraphSpacingAfter));
            ApplyIconVisualState();

            ApplyHoverCursor(ViewModel.CommonInfo.HoverCursor);

            if (!string.IsNullOrWhiteSpace(ViewModel.CommonInfo.TooltipText))
                ToolTip.SetTip(CustomButton, ViewModel.CommonInfo.TooltipText);
            else
                ToolTip.SetTip(CustomButton, null);

            ApplyButtonChromeState();
            PrimeInitialIconChromeState();
            InvalidateInitialLayoutTargets();
        }

        /// <summary>
        /// 首帧直接把图标宿主和文本宿主放到最终单元格，避免第一次进入子页面时仍先按 XAML 默认格子叠在一起。
        /// </summary>
        private void ApplyContentSlotLayout(IconButtonMiscInfo.IconPositionEnum iconPosition)
        {
            switch (iconPosition)
            {
                case IconButtonMiscInfo.IconPositionEnum.Left:
                    Grid.SetColumn(IconVisualHost, 0);
                    Grid.SetRow(IconVisualHost, 0);
                    Grid.SetColumn(TextParagraphHost, 1);
                    Grid.SetRow(TextParagraphHost, 0);
                    break;
                case IconButtonMiscInfo.IconPositionEnum.Right:
                    Grid.SetColumn(IconVisualHost, 1);
                    Grid.SetRow(IconVisualHost, 0);
                    Grid.SetColumn(TextParagraphHost, 0);
                    Grid.SetRow(TextParagraphHost, 0);
                    break;
                case IconButtonMiscInfo.IconPositionEnum.Top:
                    Grid.SetColumn(IconVisualHost, 0);
                    Grid.SetRow(IconVisualHost, 0);
                    Grid.SetColumn(TextParagraphHost, 0);
                    Grid.SetRow(TextParagraphHost, 1);
                    break;
                case IconButtonMiscInfo.IconPositionEnum.Bottom:
                    Grid.SetColumn(IconVisualHost, 0);
                    Grid.SetRow(IconVisualHost, 1);
                    Grid.SetColumn(TextParagraphHost, 0);
                    Grid.SetRow(TextParagraphHost, 0);
                    break;
                default:
                    Grid.SetColumn(IconVisualHost, 0);
                    Grid.SetRow(IconVisualHost, 0);
                    Grid.SetColumn(TextParagraphHost, 1);
                    Grid.SetRow(TextParagraphHost, 0);
                    break;
            }
        }

        /// <summary>
        /// 图标源、尺寸和可见性也在首帧同步应用一次，避免第一次打开子页面时仍沿用默认值参与测量。
        /// </summary>
        private void ApplyIconVisualState()
        {
            var iconSource = ViewModel?.MiscInfo?.IconSource;
            var hasIcon = iconSource?.Bitmap != null;
            IconVisualHost.IsVisible = hasIcon;
            var width = Math.Max(0, ViewModel?.MiscInfo?.IconWidth ?? 0);
            var height = Math.Max(0, ViewModel?.MiscInfo?.IconHeight ?? 0);
            CustomIcon.Width = width;
            CustomIcon.Height = height;
            HoverTintedIcon.Width = width;
            HoverTintedIcon.Height = height;
            ClickTintedIcon.Width = width;
            ClickTintedIcon.Height = height;

            // 第一次在主预览宿主里打开页面时，图标源绑定可能晚一拍到位。
            // 这里只针对“常态图标”赋予正确的染色图（依据ForegroundColor），避免首帧错位或显示黑块。
            // 删除了对 HoverTintedIcon 和 ClickTintedIcon 的代码赋值，保留它们的 XAML 绑定。
            if (hasIcon && ViewModel?.BrushInfo != null)
            {
                CustomIcon.Source = TintedImageSourceConverter.Convert(
                    new object?[] { iconSource, ViewModel.BrushInfo.ForegroundColor },
                    typeof(object),
                    null,
                    System.Globalization.CultureInfo.CurrentCulture) as IImage;
            }
            else
            {
                CustomIcon.Source = null;
            }
        }

        /// <summary>
        /// 首次附着到可视树时再补一轮布局收敛，避免 MapTool 第一次打开页面时第一拍仍沿用默认网格布局。
        /// </summary>
        private void RegisterInitialLayoutStabilizer()
        {
            if (_hasPendingInitialLayoutStabilizer)
            {
                return;
            }

            _hasPendingInitialLayoutStabilizer = true;

            EventHandler? handler = null;
            handler = (_, _) =>
            {
                LayoutUpdated -= handler;
                _hasPendingInitialLayoutStabilizer = false;
                ApplyInitialStateFromViewModel();
            };

            LayoutUpdated += handler;
        }

        private void RegisterDeferredInitialApply()
        {
            Dispatcher.UIThread.Post(
                () =>
                {
                    if (VisualRoot != null)
                    {
                        ApplyInitialStateFromViewModel();
                    }
                },
                DispatcherPriority.Loaded);

            Dispatcher.UIThread.Post(
                () =>
                {
                    if (VisualRoot != null)
                    {
                        ApplyInitialStateFromViewModel();
                    }
                },
                DispatcherPriority.Render);
        }

        private void StartInitialStabilizeTimer()
        {
            StopInitialStabilizeTimer();
            _initialStabilizeTickCount = 0;
            _initialStabilizeTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(120)
            };
            _initialStabilizeTimer.Tick += OnInitialStabilizeTick;
            _initialStabilizeTimer.Start();
        }

        private void StopInitialStabilizeTimer()
        {
            if (_initialStabilizeTimer == null)
            {
                return;
            }

            _initialStabilizeTimer.Stop();
            _initialStabilizeTimer.Tick -= OnInitialStabilizeTick;
            _initialStabilizeTimer = null;
            _initialStabilizeTickCount = 0;
        }

        private void OnInitialStabilizeTick(object sender, EventArgs e)
        {
            _initialStabilizeTickCount++;
            ApplyInitialStateFromViewModel();

            // 主预览宿主在首次打开页面后的前几拍还会继续改布局。
            // 这里给首屏 1 秒左右的短时稳定窗口，把图标和文字关系持续收稳，之后立刻停止，不常驻。
            if (_initialStabilizeTickCount >= 8)
            {
                StopInitialStabilizeTimer();
            }
        }

        private void PrimeInitialIconChromeState()
        {
            if (ViewModel == null)
            {
                return;
            }

            var originalPointerOver = _isPointerOver;
            var originalPointerPressed = _isPointerPressed;
            var originalMutualSelected = _isMutualSelected;

            // 首次进入 MapTool 主预览宿主时，鼠标移上去会立刻恢复正常，说明悬停态那条图标刷新链能把图层真正激活。
            // 这里在首帧同步模拟一次“进入悬停再回到原状态”，把这条刷新链提前跑掉，避免必须靠用户把鼠标移上去才正常。
            _isPointerOver = true;
            _isPointerPressed = false;
            _isMutualSelected = false;
            ApplyButtonChromeState();

            _isPointerOver = originalPointerOver;
            _isPointerPressed = originalPointerPressed;
            _isMutualSelected = originalMutualSelected;
            ApplyButtonChromeState();
        }

        /// <summary>
        /// 图标和文本相对位置依赖网格行列、按钮模板和段落容器共同参与布局，首帧需要显式触发一次重新测量。
        /// </summary>
        private void InvalidateInitialLayoutTargets()
        {
            InvalidateMeasure();
            InvalidateArrange();
            CustomButton.InvalidateMeasure();
            CustomButton.InvalidateArrange();
            ContentLayoutGrid.InvalidateMeasure();
            ContentLayoutGrid.InvalidateArrange();
            IconVisualHost.InvalidateMeasure();
            IconVisualHost.InvalidateArrange();
            TextParagraphHost.InvalidateMeasure();
            TextParagraphHost.InvalidateArrange();
        }

        /// <summary>
        /// 悬停光标属于控件视图态，需要同时作用到外层控件和内部按钮模板。
        /// </summary>
        private void ApplyHoverCursor(CommonCursorType hoverCursor)
        {
            Cursor cursor = new(hoverCursor.ToStandard());
            Cursor = cursor;
            CustomButton.Cursor = cursor;
        }

        /// <summary>
        /// 根据图标上下左右位置动态调整文本区域的可用空间，让文本对齐针对整块文本区生效。
        /// </summary>
        private void UpdateContentLayout(IconButtonMiscInfo.IconPositionEnum iconPosition)
        {
            switch (iconPosition)
            {
                case IconButtonMiscInfo.IconPositionEnum.Left:
                    ContentLayoutGrid.ColumnDefinitions = new ColumnDefinitions("Auto,*");
                    ContentLayoutGrid.RowDefinitions = new RowDefinitions("*");
                    break;
                case IconButtonMiscInfo.IconPositionEnum.Right:
                    ContentLayoutGrid.ColumnDefinitions = new ColumnDefinitions("*,Auto");
                    ContentLayoutGrid.RowDefinitions = new RowDefinitions("*");
                    break;
                case IconButtonMiscInfo.IconPositionEnum.Top:
                    ContentLayoutGrid.ColumnDefinitions = new ColumnDefinitions("*");
                    // 上下排版时改成两个 Auto 行，避免文本落在 * 行里因宿主高度差异而产生设计时/运行时间距不一致。
                    ContentLayoutGrid.RowDefinitions = new RowDefinitions("Auto,Auto");
                    break;
                case IconButtonMiscInfo.IconPositionEnum.Bottom:
                    ContentLayoutGrid.ColumnDefinitions = new ColumnDefinitions("*");
                    // 下方图标同样使用稳定的 Auto/Auto 行定义，让图标偏移只改变视觉位置，不受剩余高度重新分配影响。
                    ContentLayoutGrid.RowDefinitions = new RowDefinitions("Auto,Auto");
                    break;
                default:
                    ContentLayoutGrid.ColumnDefinitions = new ColumnDefinitions("Auto,*");
                    ContentLayoutGrid.RowDefinitions = new RowDefinitions("*");
                    break;
            }
        }

        private void SetPointerPressed(bool isPressed)
        {
            if (_isPointerPressed == isPressed)
            {
                return;
            }

            _isPointerPressed = isPressed;
            ApplyButtonChromeState();
        }

        /// <summary>
        /// 悬停态也是代码直接写按钮本地外观的一部分，需要单独维护鼠标进入/离开的界面态。
        /// </summary>
        private void SetPointerOver(bool isPointerOver)
        {
            if (_isPointerOver == isPointerOver)
            {
                return;
            }

            _isPointerOver = isPointerOver;
            ApplyButtonChromeState();
        }

        private void ApplyButtonChromeState()
        {
            if (ViewModel == null)
            {
                return;
            }

            var normalThickness = new Thickness(
                Math.Max(0, ViewModel.AppearanceInfo.BorderThicknessLeft),
                Math.Max(0, ViewModel.AppearanceInfo.BorderThicknessTop),
                Math.Max(0, ViewModel.AppearanceInfo.BorderThicknessRight),
                Math.Max(0, ViewModel.AppearanceInfo.BorderThicknessBottom));

            var isClickChromeActive = _isPointerPressed || _isMutualSelected;
            var isHoverChromeActive = _isPointerOver && !isClickChromeActive;
            if (!isClickChromeActive)
            {
                CustomButton.Margin = new Thickness(0);
                CustomButton.BorderThickness = normalThickness;
                CustomButton.BorderBrush = new SolidColorBrush(ViewModel.BrushInfo.BorderColor);
                CustomButton.Foreground = new SolidColorBrush(
                    isHoverChromeActive
                        ? ViewModel.BrushInfo.HoverForegroundColor
                        : ViewModel.BrushInfo.ForegroundColor);
                CustomButton.Background = ViewModel.MiscInfo.BackgroundImage != null
                    ? BackgroundMultiConverter.GetBackgroundBrush(
                        ViewModel.MiscInfo.BackgroundImage,
                        isHoverChromeActive
                            ? ViewModel.BrushInfo.HoverBackgroundColor
                            : ViewModel.BrushInfo.BackgroundColor,
                        ViewModel.MiscInfo.ImageSizeMode)
                    : new SolidColorBrush(
                        isHoverChromeActive
                            ? ViewModel.BrushInfo.HoverBackgroundColor
                            : ViewModel.BrushInfo.BackgroundColor);
                // 普通态显示原图；悬停态切到悬停前景色染色图；点击态由下方点击链单独覆盖。
                CustomIcon.IsVisible = !isHoverChromeActive;
                HoverTintedIcon.IsVisible = isHoverChromeActive;
                ClickTintedIcon.IsVisible = false;
                ApplyClickBorderVisual(0, 0, 0, 0, Brushes.Transparent, Brushes.Transparent, Brushes.Transparent, Brushes.Transparent);
                return;
            }

            // 鼠标按下态和平台组选中态共用一套四边独立边框，同时把图标切到点击前景色染色图，形成“图标线条变色”的效果。
            // 这里把按钮主体往内缩，让四边点击边框占用最外层区域，看起来就是外描边而不是内部压一条色块。
            CustomButton.Margin = new Thickness(
                Math.Max(0, ViewModel.AppearanceInfo.ClickBorderThicknessLeft),
                Math.Max(0, ViewModel.AppearanceInfo.ClickBorderThicknessTop),
                Math.Max(0, ViewModel.AppearanceInfo.ClickBorderThicknessRight),
                Math.Max(0, ViewModel.AppearanceInfo.ClickBorderThicknessBottom));
            CustomButton.BorderThickness = new Thickness(0);
            CustomButton.BorderBrush = Brushes.Transparent;
            CustomButton.Foreground = new SolidColorBrush(ViewModel.BrushInfo.ClickForegroundColor);
            CustomButton.Background = ViewModel.MiscInfo.BackgroundImage != null
                ? BackgroundMultiConverter.GetBackgroundBrush(
                    ViewModel.MiscInfo.BackgroundImage,
                    ViewModel.BrushInfo.ClickBackgroundColor,
                    ViewModel.MiscInfo.ImageSizeMode)
                : new SolidColorBrush(ViewModel.BrushInfo.ClickBackgroundColor);
            CustomIcon.IsVisible = false;
            HoverTintedIcon.IsVisible = false;
            ClickTintedIcon.IsVisible = true;
            ApplyClickBorderVisual(
                Math.Max(0, ViewModel.AppearanceInfo.ClickBorderThicknessLeft),
                Math.Max(0, ViewModel.AppearanceInfo.ClickBorderThicknessTop),
                Math.Max(0, ViewModel.AppearanceInfo.ClickBorderThicknessRight),
                Math.Max(0, ViewModel.AppearanceInfo.ClickBorderThicknessBottom),
                new SolidColorBrush(ViewModel.BrushInfo.ClickBorderColorLeft),
                new SolidColorBrush(ViewModel.BrushInfo.ClickBorderColorTop),
                new SolidColorBrush(ViewModel.BrushInfo.ClickBorderColorRight),
                new SolidColorBrush(ViewModel.BrushInfo.ClickBorderColorBottom));
        }

        private void ApplyClickBorderVisual(int leftThickness, int topThickness, int rightThickness, int bottomThickness, IBrush leftBrush, IBrush topBrush, IBrush rightBrush, IBrush bottomBrush)
        {
            LeftClickBorder.Width = leftThickness;
            LeftClickBorder.Background = leftBrush;

            TopClickBorder.Height = topThickness;
            TopClickBorder.Background = topBrush;

            RightClickBorder.Width = rightThickness;
            RightClickBorder.Background = rightBrush;

            BottomClickBorder.Height = bottomThickness;
            BottomClickBorder.Background = bottomBrush;
        }

        /// <summary>
        /// 平台互斥消息只改变当前控件的纯界面态，不去写回图元属性模型。
        /// </summary>
        public void ApplyMutualSelectionState(bool isSelected)
        {
            if (_isMutualSelected == isSelected)
            {
                return;
            }

            _isMutualSelected = isSelected;
            ApplyButtonChromeState();
        }
    }
}
