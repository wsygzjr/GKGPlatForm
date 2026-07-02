using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace GKG.Map.MapCell.Generic.Link
{
    public partial class LinkView : ReactiveUserControl<LinkViewModel>
    {
        public LinkView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                if (ViewModel == null)
                    return;

                Point GetScreenPoint()
                {
                    PixelPoint screenPixel = LinkButton.PointToScreen(new Point(0, 0));
                    return new Point(screenPixel.X, screenPixel.Y);
                }

                bool isHovering = false;

                void ApplyForeground()
                {
                    if (ViewModel == null)
                        return;

                    var color = isHovering ? ViewModel.BrushInfo.HoverTextColor : ViewModel.BrushInfo.TextColor;
                    LinkTextBlock.Foreground = new SolidColorBrush(color);
                }

                this.BindCommand<LinkView, LinkViewModel, ReactiveCommand<Point, Unit>, Button, Point>(
                    viewModel: ViewModel,
                    propertyName: vm => vm.ClickCommand,
                    controlName: v => v.LinkButton,
                    withParameter: Observable.FromEventPattern<RoutedEventArgs>(
                            addHandler: h => LinkButton.Click += h,
                            removeHandler: h => LinkButton.Click -= h)
                        .Select(_ => GetScreenPoint()))
                    .DisposeWith(disposables);

                Observable.Merge(
                        ViewModel.WhenAnyValue(vm => vm.BrushInfo.TextColor).Select(_ => Unit.Default),
                        ViewModel.WhenAnyValue(vm => vm.BrushInfo.HoverTextColor).Select(_ => Unit.Default),
                        Observable.FromEventPattern<PointerEventArgs>(
                                addHandler: h => LinkButton.PointerEntered += h,
                                removeHandler: h => LinkButton.PointerEntered -= h)
                            .Do(_ => isHovering = true)
                            .Select(_ => Unit.Default),
                        Observable.FromEventPattern<PointerEventArgs>(
                                addHandler: h => LinkButton.PointerExited += h,
                                removeHandler: h => LinkButton.PointerExited -= h)
                            .Do(_ => isHovering = false)
                            .Select(_ => Unit.Default))
                    .Subscribe(_ => ApplyForeground())
                    .DisposeWith(disposables);

                ViewModel.WhenAnyValue(vm => vm.CommonInfo.LinkText)
                    .BindTo(this, v => v.LinkTextBlock.Text)
                    .DisposeWith(disposables);

                ViewModel.WhenAnyValue(vm => vm.BrushInfo.TextColor)
                    .Select(color => (IBrush)new SolidColorBrush(color))
                    .BindTo(this, v => v.RootBorder.BorderBrush)
                    .DisposeWith(disposables);
            });
        }
    }
}
