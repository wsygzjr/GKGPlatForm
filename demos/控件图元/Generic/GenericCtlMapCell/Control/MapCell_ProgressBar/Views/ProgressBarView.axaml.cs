using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using GKG.Map.MapCell.Generic.Control.Lable;
using GKG.Map.MapCell.Generic.ProgressBar.ViewModel;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace GKG.Map.MapCell.Generic.ProgressBar.View;

public partial class ProgressBarView : ReactiveUserControl<ProgressBarViewModel>
{
    public ProgressBarView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            if (ViewModel == null) return;

            this.OneWayBind(ViewModel, vm => vm.BackColor, v => v.BackBorder.Background, c => new SolidColorBrush(c))
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.BorderColor, v => v.BackBorder.BorderBrush, c => new SolidColorBrush(c))
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.ForeColor, v => v.ProgressBarCtl.Foreground, c => new SolidColorBrush(c))
                .DisposeWith(disposables);

            Observable.CombineLatest(
                ViewModel.WhenAnyValue(vm => vm.BorderThicknessLeft),
                ViewModel.WhenAnyValue(vm => vm.BorderThicknessTop),
                ViewModel.WhenAnyValue(vm => vm.BorderThicknessRight),
                ViewModel.WhenAnyValue(vm => vm.BorderThicknessBottom),
                (left, top, right, bottom) => new Thickness(left, top, right, bottom)
            ).Subscribe(thickness => BackBorder.BorderThickness = thickness).DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.Opacity, v => v.Opacity)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.IsEnabled, v => v.ProgressBarCtl.IsEnabled)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.Orientation, v => v.ProgressBarCtl.Orientation)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.Minimum, v => v.ProgressBarCtl.Minimum)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.Maximum, v => v.ProgressBarCtl.Maximum)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.Value, v => v.ProgressBarCtl.Value)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.IsIndeterminate, v => v.ProgressBarCtl.IsIndeterminate)
                .DisposeWith(disposables);

            // 进度条外层尺寸交由宿主统一承接，这里不再重复把父类宽高直赋到 View。

            // 外层 ProgressBarView 是宿主和绿色外框管理的图元本体，布局边距只作用到内部内容。
            this.OneWayBind(ViewModel, vm => vm.HorizontalAlign, v => v.ContentHost.HorizontalAlignment, align => (Avalonia.Layout.HorizontalAlignment)align)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.VerticalAlign, v => v.ContentHost.VerticalAlignment, align => (Avalonia.Layout.VerticalAlignment)align)
                .DisposeWith(disposables);

            Observable.CombineLatest(
                ViewModel.WhenAnyValue(vm => vm.MarginLeft),
                ViewModel.WhenAnyValue(vm => vm.MarginTop),
                ViewModel.WhenAnyValue(vm => vm.MarginRight),
                ViewModel.WhenAnyValue(vm => vm.MarginBottom),
                (left, top, right, bottom) => new Thickness(left, top, right, bottom)
            ).Subscribe(margin => ContentHost.Margin = margin).DisposeWith(disposables);

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

            this.OneWayBind(ViewModel, vm => vm.FontFamilyObj, v => v.TextCtl.FontFamily, f => f)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.FontColor, v => v.TextCtl.Foreground, c => new SolidColorBrush(c))
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.FontSize, v => v.TextCtl.FontSize)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.IsBold, v => v.TextCtl.FontWeight, isBold => isBold ? FontWeight.Bold : FontWeight.Normal)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.IsItalic, v => v.TextCtl.FontStyle, isItalic => isItalic ? FontStyle.Italic : FontStyle.Normal)
                .DisposeWith(disposables);

            Observable.CombineLatest(
                ViewModel.WhenAnyValue(vm => vm.Value),
                ViewModel.WhenAnyValue(vm => vm.Minimum),
                ViewModel.WhenAnyValue(vm => vm.Maximum),
                ViewModel.WhenAnyValue(vm => vm.IsIndeterminate),
                (val, min, max, indeterminate) =>
                {
                    if (indeterminate) return "";
                    var range = max - min;
                    if (Math.Abs(range) < 0.000001) return "";
                    var percent = (val - min) / range * 100.0;
                    if (double.IsNaN(percent) || double.IsInfinity(percent)) return "";
                    return $"{percent:0}%";
                }
            ).Subscribe(text => TextCtl.Text = text).DisposeWith(disposables);

            ViewModel.WhenAnyValue(vm => vm.ToolTip)
                .Subscribe(toolTip =>
                {
                    if (!string.IsNullOrEmpty(toolTip))
                        Avalonia.Controls.ToolTip.SetTip(this, toolTip);
                    else
                        Avalonia.Controls.ToolTip.SetTip(this, null);
                }).DisposeWith(disposables);
        });
    }
}
