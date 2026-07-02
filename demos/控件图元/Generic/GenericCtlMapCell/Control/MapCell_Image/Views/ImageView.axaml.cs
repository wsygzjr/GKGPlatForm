using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using GKG.Map.MapCell.Generic.Control.MapCell_Image.ViewModels;
using PropertyModels.Extensions;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Image.Views
{
    public partial class ImageView : ReactiveUserControl<ImageViewModel>
    {
        public ImageView()
        {
            InitializeComponent();

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

                // 图片图元试点按示例 Button 的尺寸承接方式收口：
                // 外层 Width/Height 由宿主统一承接，这里只保留最小最大尺寸、边距和内部图片对齐逻辑。
            });
        }
    }
}
