using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using GKG.Map.MapCell.Generic.Control.Lable;
using GKG.Map.MapCell.Generic.RadioButton.ViewModel;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace GKG.Map.MapCell.Generic.RadioButton.View;

/// <summary>
/// 单选框图元视图
/// 负责将 ViewModel 的属性绑定到 UI 控件
/// </summary>
public partial class RadioButtonView : ReactiveUserControl<RadioButtonViewModel>
{
    private bool _syncing;

    #region 构造函数

    public RadioButtonView()
    {
        InitializeComponent();

        // 处理按下事件，实现点击已选中的RadioButton可以取消选中
        RadioBtn.AddHandler(InputElement.PointerPressedEvent, OnRadioButtonPointerPressed, RoutingStrategies.Tunnel);

        // 默认 RadioButton 行为不会回写到 ViewModel（我们用了 OneWayBind），这里补齐同步
        RadioBtn.Checked += OnRadioBtnChecked;
        RadioBtn.Unchecked += OnRadioBtnUnchecked;
        
        this.WhenActivated(disposables =>
        {
            if (ViewModel == null) return;

            #region 文本绑定

            this.Bind(ViewModel,
                vm => vm.Text,
                view => view.RadioBtn.Content)
                .DisposeWith(disposables);

            // 使用单向绑定从ViewModel到View，点击事件单独处理
            this.OneWayBind(ViewModel,
                vm => vm.IsChecked,
                view => view.RadioBtn.IsChecked)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                vm => vm.GroupName,
                view => view.RadioBtn.GroupName)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                vm => vm.IsThreeState,
                view => view.RadioBtn.IsThreeState)
                .DisposeWith(disposables);

            #endregion

            #region 颜色绑定

            this.OneWayBind(ViewModel,
                vm => vm.BackColor,
                view => view.BackBorder.Background,
                color => new SolidColorBrush(color))
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                vm => vm.BorderColor,
                view => view.RadioBtn.BorderBrush,
                color => new SolidColorBrush(color))
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                vm => vm.ForeColor,
                view => view.RadioBtn.Foreground,
                color => new SolidColorBrush(color))
                .DisposeWith(disposables);

            #endregion

            #region 边框绑定

            Observable.CombineLatest(
                ViewModel.WhenAnyValue(vm => vm.BorderThicknessLeft),
                ViewModel.WhenAnyValue(vm => vm.BorderThicknessTop),
                ViewModel.WhenAnyValue(vm => vm.BorderThicknessRight),
                ViewModel.WhenAnyValue(vm => vm.BorderThicknessBottom),
                (left, top, right, bottom) => new Thickness(left, top, right, bottom)
            ).Subscribe(thickness => RadioBtn.BorderThickness = thickness).DisposeWith(disposables);

            #endregion

            #region 字体绑定

            this.OneWayBind(ViewModel,
                vm => vm.FontFamilyObj,
                view => view.RadioBtn.FontFamily,
                fontFamily => fontFamily)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                vm => vm.FontColor,
                view => view.RadioBtn.Foreground,
                color => new SolidColorBrush(color))
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                vm => vm.FontSize,
                view => view.RadioBtn.FontSize)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                vm => vm.IsBold,
                view => view.RadioBtn.FontWeight,
                isBold => isBold ? FontWeight.Bold : FontWeight.Normal)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                vm => vm.IsItalic,
                view => view.RadioBtn.FontStyle,
                isItalic => isItalic ? FontStyle.Italic : FontStyle.Normal)
                .DisposeWith(disposables);

            #endregion

            #region 布局绑定

            // 单选框外层尺寸改由宿主统一承接，这里不再重复把父类宽高直赋到 View。

            this.OneWayBind(ViewModel, vm => vm.HorizontalAlign, v => v.RadioBtn.HorizontalAlignment,
               align => (Avalonia.Layout.HorizontalAlignment)align)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.VerticalAlign, v => v.RadioBtn.VerticalAlignment,
                align => (Avalonia.Layout.VerticalAlignment)align)
                .DisposeWith(disposables);

            Observable.CombineLatest(
                ViewModel.WhenAnyValue(vm => vm.MarginLeft),
                ViewModel.WhenAnyValue(vm => vm.MarginTop),
                ViewModel.WhenAnyValue(vm => vm.MarginRight),
                ViewModel.WhenAnyValue(vm => vm.MarginBottom),
                (left, top, right, bottom) => new Thickness(left, top, right, bottom)
            ).Subscribe(margin => this.Margin = margin).DisposeWith(disposables);

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

            #endregion

            #region 外观绑定

            this.OneWayBind(ViewModel,
                vm => vm.Opacity,
                view => view.Opacity)
                .DisposeWith(disposables);

            #endregion

            #region 交互绑定

            this.OneWayBind(ViewModel,
                vm => vm.IsEnabled,
                view => view.RadioBtn.IsEnabled)
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

            #endregion
        });
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 处理RadioButton按下事件，实现点击已选中的可以取消选中
    /// </summary>
    private void OnRadioButtonPointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if (ViewModel == null) return;

        if (ViewModel.IsThreeState)
        {
            var cur = ViewModel.IsChecked;

            // 方案2：三态循环 null -> true -> false -> null
            // 互斥只在进入 true 时生效：这里放行“null -> true”，让 Avalonia 默认 RadioButton 处理组互斥。
            if (cur == null)
                return;

            if (cur == true)
            {
                SetChecked(false);
                e.Handled = true;
                return;
            }

            // cur == false
            SetChecked(null);
            e.Handled = true;
            return;
        }

        // 只处理“已选中 -> 取消选中”。
        // 未选中时放行默认行为（包括组内互斥）。
        if (ViewModel.IsChecked != true)
            return;

        // 以 ViewModel 为“旧值”来源，手动 toggle，避免 RadioButton 默认行为强制选中后无法取消
        SetChecked(false);

        // 阻止默认 RadioButton 处理，避免它在组内把自己再次置为 true
        e.Handled = true;
    }

    private void OnRadioBtnChecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (RadioBtn.IsChecked == true)
            SetChecked(true);
    }

    private void OnRadioBtnUnchecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (RadioBtn.IsChecked == false)
            SetChecked(false);
    }

    private void SetChecked(bool? value)
    {
        if (_syncing)
            return;

        try
        {
            _syncing = true;
            if (RadioBtn != null)
                RadioBtn.IsChecked = value;
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

    #endregion
}
