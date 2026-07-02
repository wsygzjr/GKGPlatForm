using System.Globalization;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml.Converters;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using GKG.Map.CommunicationFuncCtlMapCell.ViewModel;
using ReactiveUI;

namespace GKG.Map.CommunicationFuncCtlMapCell.View;

public partial class CommunicationView : ReactiveUserControl<CommunicationViewModel>
{
    public CommunicationView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            // Background
            this.OneWayBind(
                viewModel: ViewModel,
                vmProperty: vm => vm.BackColor,
                viewProperty: v => v.MainBorder.Background,
                selector: color =>
                {
                    var brushConverter = new ColorToBrushConverter();
                    return brushConverter.Convert(color, typeof(IBrush), null, CultureInfo.CurrentCulture) as IBrush ?? new SolidColorBrush(Colors.Transparent);
                }
            ).DisposeWith(disposables);

            // TextBlock
            BindTextBlock(tbRaiseTime, disposables);
            BindTextBlock(tbDispenseTime, disposables);
            BindTextBlock(tbImpactTime, disposables);
            BindTextBlock(tbIntermittentTime, disposables);
            BindTextBlock(tbVoltageRatio, disposables);
            BindTextBlock(tbDotCount, disposables);
            BindTextBlock(tbDispenseMode, disposables);
            BindTextBlock(tbAfterStop, disposables);
            BindTextBlock(tbTotalCount, disposables);

            // TextBox
            BindTextBox(txtRaiseTime, disposables);
            BindTextBox(txtDispenseTime, disposables);
            BindTextBox(txtImpactTime, disposables);
            BindTextBox(txtIntermittentTime, disposables);
            BindTextBox(txtVoltageRatio, disposables);
            BindTextBox(txtDotCount, disposables);
            BindTextBox(txtDispenseMode, disposables);
            BindTextBox(txtTotalCount, disposables);

            // RadioButton / ToggleSwitch
            BindRadioButton(rbModeDot, disposables);
            BindRadioButton(rbModeLine, disposables);
            BindToggleSwitch(tsAfterStop, disposables);
        });
    }


    private void BindFont(TemplatedControl target, CompositeDisposable disposables)
    {
        this.WhenAnyValue(v => v.ViewModel!.TextFont.FontFamily)
            .BindTo(target, t => t.FontFamily)
            .DisposeWith(disposables);

        this.WhenAnyValue(v => v.ViewModel!.TextFont.FontSize)
            .BindTo(target, t => t.FontSize)
            .DisposeWith(disposables);

        this.WhenAnyValue(v => v.ViewModel!.TextFont.FontWeight)
            .BindTo(target, t => t.FontWeight)
            .DisposeWith(disposables);
    }

    private void BindTextBlock(TextBlock target, CompositeDisposable disposables)
    {
        this.WhenAnyValue(v => v.ViewModel!.TextColor)
            .Select(ConvertColorToBrush)
            .BindTo(target, t => t.Foreground)
            .DisposeWith(disposables);

        this.WhenAnyValue(v => v.ViewModel!.TextFont.FontFamily)
            .BindTo(target, t => t.FontFamily)
            .DisposeWith(disposables);

        this.WhenAnyValue(v => v.ViewModel!.TextFont.FontSize)
            .BindTo(target, t => t.FontSize)
            .DisposeWith(disposables);

        this.WhenAnyValue(v => v.ViewModel!.TextFont.FontWeight)
            .BindTo(target, t => t.FontWeight)
            .DisposeWith(disposables);
    }

    private void BindTextBox(TextBox target, CompositeDisposable disposables)
    {
        this.WhenAnyValue(v => v.ViewModel!.TextColor)
            .Select(ConvertColorToBrush)
            .BindTo(target, t => t.Foreground)
            .DisposeWith(disposables);

        BindFont(target, disposables);
    }

    private void BindRadioButton(RadioButton target, CompositeDisposable disposables)
    {
        this.WhenAnyValue(v => v.ViewModel!.TextColor)
            .Select(ConvertColorToBrush)
            .BindTo(target, t => t.Foreground)
            .DisposeWith(disposables);

        BindFont(target, disposables);
    }

    private void BindToggleSwitch(ToggleSwitch target, CompositeDisposable disposables)
    {
        this.WhenAnyValue(v => v.ViewModel!.TextColor)
            .Select(ConvertColorToBrush)
            .BindTo(target, t => t.Foreground)
            .DisposeWith(disposables);

        BindFont(target, disposables);
    }

    private static IBrush ConvertColorToBrush(Color color)
    {
        var converter = new ColorToBrushConverter();
        return converter.Convert(color, typeof(IBrush), null, CultureInfo.CurrentCulture) as IBrush
               ?? new SolidColorBrush(Colors.Transparent);
    }
}