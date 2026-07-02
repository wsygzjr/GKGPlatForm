using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using GKG.Map.HeatingControlFuncCtlMapCell.Convert;
using GKG.Map.HeatingControlFuncCtlMapCell.ViewModel;
using Griffins.UI2;
using ReactiveUI;
using System;
using System.Globalization;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace GKG.Map.HeatingControlFuncCtlMapCell.View;

public partial class HeatingControlView : ReactiveUserControl<HeatingControlViewModel>
{
    public HeatingControlView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            if (ViewModel == null) return;

            this.OneWayBind(
                viewModel: ViewModel,
                vmProperty: vm => vm.BackColor,
                viewProperty: v => v.MainBorder.Background,
                selector: color =>
                {
                    var brushConverter = new ColorToBrushConverter();
                    return brushConverter.Convert(color, typeof(IBrush), null, CultureInfo.CurrentCulture) as IBrush
                           ?? new SolidColorBrush(Colors.Transparent);
                }
            ).DisposeWith(disposables);

            ViewModel.WhenAnyValue(vm => vm.TextColor)
                .Select(c => (IBrush)new SolidColorBrush(c))
                .Subscribe(ApplyTextColor)
                .DisposeWith(disposables);

            ViewModel.WhenAnyValue(vm => vm.TextFont)
                .Subscribe(ApplyTextFont)
                .DisposeWith(disposables);

            Observable.FromEventPattern<PointerPressedEventArgs>(
                    addHandler: h => MainBorder.PointerPressed += h,
                    removeHandler: h => MainBorder.PointerPressed -= h
                )
                .Select(_ =>
                {
                    PixelPoint screenPixel = MainBorder.PointToScreen(new Point(0, 0));
                    return new Point(screenPixel.X, screenPixel.Y);
                })
                .InvokeCommand(ViewModel, vm => vm.PointerPressedCommand)
                .DisposeWith(disposables);
        });
    }

    private void ApplyTextColor(IBrush brush)
    {
        if (brush == null) return;
        foreach (var tb in MainBorder.GetVisualDescendants().OfType<TextBlock>())
            tb.Foreground = brush;
        foreach (var txt in MainBorder.GetVisualDescendants().OfType<TextBox>())
            txt.Foreground = brush;
        foreach (var chk in MainBorder.GetVisualDescendants().OfType<CheckBox>())
            chk.Foreground = brush;
    }

    private void ApplyTextFont(FontInfo font)
    {
        if (font == null) return;

        foreach (var tb in MainBorder.GetVisualDescendants().OfType<TextBlock>())
        {
            tb.FontFamily = font.FontFamily;
            tb.FontSize = font.FontSize;
            tb.FontWeight = font.FontWeight;
            tb.FontStyle = font.FontStyle;
        }

        foreach (var txt in MainBorder.GetVisualDescendants().OfType<TextBox>())
        {
            txt.FontFamily = font.FontFamily;
            txt.FontSize = font.FontSize;
            txt.FontWeight = font.FontWeight;
            txt.FontStyle = font.FontStyle;
        }

        foreach (var chk in MainBorder.GetVisualDescendants().OfType<CheckBox>())
        {
            chk.FontFamily = font.FontFamily;
            chk.FontSize = font.FontSize;
            chk.FontWeight = font.FontWeight;
            chk.FontStyle = font.FontStyle;
        }
    }
}
