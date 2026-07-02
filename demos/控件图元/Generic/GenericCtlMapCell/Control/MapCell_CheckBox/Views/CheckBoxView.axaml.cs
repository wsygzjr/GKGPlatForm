using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using GKG.Map.MapCell.Generic.CheckBox.ViewModel;
using GKG.Map.MapCell.Generic.Control.Lable;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace GKG.Map.MapCell.Generic.CheckBox.View;

public partial class CheckBoxView : ReactiveUserControl<CheckBoxViewModel>
{
    private bool _syncing;

    public CheckBoxView()
    {
        InitializeComponent();

        CheckBoxCtl.Checked += OnCheckBoxChecked;
        CheckBoxCtl.Unchecked += OnCheckBoxUnchecked;
        CheckBoxCtl.Indeterminate += OnCheckBoxIndeterminate;

        this.WhenActivated(disposables =>
        {
            if (ViewModel == null) return;

            this.Bind(ViewModel, vm => vm.Text, view => view.CheckBoxCtl.Content)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.IsChecked, view => view.CheckBoxCtl.IsChecked)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.IsThreeState, view => view.CheckBoxCtl.IsThreeState)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.BackColor, view => view.BackBorder.Background, color => new SolidColorBrush(color))
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.BorderColor, view => view.CheckBoxCtl.BorderBrush, color => new SolidColorBrush(color))
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.ForeColor, view => view.CheckBoxCtl.Foreground, color => new SolidColorBrush(color))
                .DisposeWith(disposables);

            Observable.CombineLatest(
                ViewModel.WhenAnyValue(vm => vm.BorderThicknessLeft),
                ViewModel.WhenAnyValue(vm => vm.BorderThicknessTop),
                ViewModel.WhenAnyValue(vm => vm.BorderThicknessRight),
                ViewModel.WhenAnyValue(vm => vm.BorderThicknessBottom),
                (left, top, right, bottom) => new Thickness(left, top, right, bottom)
            ).Subscribe(thickness => CheckBoxCtl.BorderThickness = thickness).DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.FontFamilyObj, view => view.CheckBoxCtl.FontFamily, fontFamily => fontFamily)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.FontColor, view => view.CheckBoxCtl.Foreground, color => new SolidColorBrush(color))
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.FontSize, view => view.CheckBoxCtl.FontSize)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.IsBold, view => view.CheckBoxCtl.FontWeight, isBold => isBold ? FontWeight.Bold : FontWeight.Normal)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.IsItalic, view => view.CheckBoxCtl.FontStyle, isItalic => isItalic ? FontStyle.Italic : FontStyle.Normal)
                .DisposeWith(disposables);

            // 复选框外层尺寸交由宿主统一承接，这里不再重复把父类宽高直赋到 View。

            // 外层 CheckBoxView 是宿主和绿色外框管理的图元本体，布局边距只作用到内部内容。
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

            this.OneWayBind(ViewModel, vm => vm.Opacity, view => view.Opacity)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.IsEnabled, view => view.CheckBoxCtl.IsEnabled)
                .DisposeWith(disposables);

            ViewModel.WhenAnyValue(vm => vm.CursorType)
                .Subscribe(cursorType => this.Cursor = ConvertToCursor(cursorType))
                .DisposeWith(disposables);

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

    private void OnCheckBoxChecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (CheckBoxCtl.IsChecked == true)
            SetChecked(true);
    }

    private void OnCheckBoxUnchecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (CheckBoxCtl.IsChecked == false)
            SetChecked(false);
    }

    private void OnCheckBoxIndeterminate(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (CheckBoxCtl.IsChecked == null)
            SetChecked(null);
    }

    private void SetChecked(bool? value)
    {
        if (_syncing)
            return;

        try
        {
            _syncing = true;
            if (CheckBoxCtl != null)
                CheckBoxCtl.IsChecked = value;
            if (ViewModel != null)
                ViewModel.IsChecked = value;
            if (ViewModel?.Model != null)
                ViewModel.Model.CommonInfo.IsChecked = value;
        }
        finally
        {
            _syncing = false;
        }
    }

    private static Avalonia.Input.Cursor ConvertToCursor(CursorType cursorType)
    {
        return cursorType switch
        {
            CursorType.Arrow => new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Arrow),
            CursorType.Ibeam => new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Ibeam),
            CursorType.Wait => new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Wait),
            CursorType.Cross => new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Cross),
            CursorType.UpArrow => new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.UpArrow),
            CursorType.SizeWestEast => new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.SizeWestEast),
            CursorType.SizeNorthSouth => new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.SizeNorthSouth),
            CursorType.SizeAll => new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.SizeAll),
            CursorType.No => new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.No),
            CursorType.Hand => new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand),
            CursorType.AppStarting => new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.AppStarting),
            CursorType.Help => new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Help),
            _ => new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Arrow)
        };
    }
}
