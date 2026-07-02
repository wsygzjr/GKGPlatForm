using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using GKG.Map.MapCell.Generic.Control.MapCell_ImageGroup.ViewModels;
using PropertyModels.Extensions;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace GKG.Map.MapCell.Generic.Control.MapCell_ImageGroup.Views
{
    public partial class ImageGroupView : ReactiveUserControl<ImageGroupViewModel>
    {
        public ImageGroupView()
        {
            InitializeComponent();
            DataContextChanged += (_, _) => ApplyInitialStateFromViewModel();
            AttachedToVisualTree += (_, _) => ApplyInitialStateFromViewModel();

            this.WhenActivated(disposables =>
            {
                if (ViewModel == null) return;

                Observable.FromEventPattern<PointerReleasedEventArgs>(
                        addHandler: h => OuterBorder.PointerReleased += h,
                        removeHandler: h => OuterBorder.PointerReleased -= h
                    )
                    .Where(x => x.EventArgs.InitialPressMouseButton == MouseButton.Left)
                    .Select(_ =>
                    {
                        PixelPoint screenPixel = OuterBorder.PointToScreen(new Point(0, 0));
                        return new Point(screenPixel.X, screenPixel.Y);
                    })
                    .InvokeCommand(ViewModel, vm => vm.MouseClickCommand)
                    .DisposeWith(disposables);

                // 图片组试点按单张图片图元的尺寸承接方式收口：外层尺寸由宿主统一承接，这里不再重复直赋 Width/Height。

                ViewModel.WhenAnyValue(vm => vm.MinWidth)
                    .Subscribe(minWidth => this.MinWidth = minWidth > 0 ? minWidth : 0)
                    .DisposeWith(disposables);

                ViewModel.WhenAnyValue(vm => vm.MaxWidth)
                    .Subscribe(maxWidth => this.MaxWidth = maxWidth > 0 ? maxWidth : double.PositiveInfinity)
                    .DisposeWith(disposables);

                ViewModel.WhenAnyValue(vm => vm.MinHeight)
                    .Subscribe(minHeight => this.MinHeight = minHeight > 0 ? minHeight : 0)
                    .DisposeWith(disposables);

                ViewModel.WhenAnyValue(vm => vm.MaxHeight)
                    .Subscribe(maxHeight => this.MaxHeight = maxHeight > 0 ? maxHeight : double.PositiveInfinity)
                    .DisposeWith(disposables);

                Observable.CombineLatest(
                        ViewModel.WhenAnyValue(vm => vm.MarginLeft),
                        ViewModel.WhenAnyValue(vm => vm.MarginTop),
                        ViewModel.WhenAnyValue(vm => vm.MarginRight),
                        ViewModel.WhenAnyValue(vm => vm.MarginBottom),
                        (left, top, right, bottom) => new Thickness(left, top, right, bottom)
                    )
                    .Subscribe(margin => this.Margin = margin)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.HorizontalAlign, v => v.ImageControl.HorizontalAlignment,
                        align => (Avalonia.Layout.HorizontalAlignment)align)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.VerticalAlign, v => v.ImageControl.VerticalAlignment,
                        align => (Avalonia.Layout.VerticalAlignment)align)
                    .DisposeWith(disposables);

                ApplyInitialStateFromViewModel();
            });
        }

        /// <summary>
        /// 图片组切页首帧先按当前 ViewModel 真实值同步落位，避免等响应式订阅分多拍生效时先出现空白或布局跳变。
        /// </summary>
        public void ApplyInitialStateFromViewModel()
        {
            if (ViewModel == null)
            {
                return;
            }

            MinWidth = ViewModel.MinWidth > 0 ? ViewModel.MinWidth : 0;
            MaxWidth = ViewModel.MaxWidth > 0 ? ViewModel.MaxWidth : double.PositiveInfinity;
            MinHeight = ViewModel.MinHeight > 0 ? ViewModel.MinHeight : 0;
            MaxHeight = ViewModel.MaxHeight > 0 ? ViewModel.MaxHeight : double.PositiveInfinity;

            Margin = new Thickness(
                ViewModel.MarginLeft,
                ViewModel.MarginTop,
                ViewModel.MarginRight,
                ViewModel.MarginBottom);

            ImageControl.HorizontalAlignment = (Avalonia.Layout.HorizontalAlignment)ViewModel.HorizontalAlign;
            ImageControl.VerticalAlignment = (Avalonia.Layout.VerticalAlignment)ViewModel.VerticalAlign;
        }
    }
}
