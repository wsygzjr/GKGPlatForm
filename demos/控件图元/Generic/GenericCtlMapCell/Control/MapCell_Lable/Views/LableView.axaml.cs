using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using GKG.Map.MapCell.Generic.Lable.Convert;
using GKG.Map.MapCell.Generic.Lable.ViewModel;
using PropertyModels.Extensions;
using ReactiveUI;
using System;
using System.Globalization;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Layout;

namespace GKG.Map.MapCell.Generic.Lable.View;

/// <summary>
/// 标签图元视图
/// 负责将 ViewModel 的属性绑定到 UI 控件
/// </summary>
public partial class LableView : ReactiveUserControl<LableViewModel>
{
    #region 构造函数

    public LableView()
    {
        InitializeComponent();
        DataContextChanged += (_, _) => ApplyInitialStateFromViewModel();
        AttachedToVisualTree += (_, _) => ApplyInitialStateFromViewModel();
        this.WhenActivated(disposables =>
        {
            if (ViewModel == null) return;

            #region 文本绑定

            // 绑定标签文本
            this.Bind(ViewModel,
                vm => vm.LableText,
                view => view.Lable.Text)
                .DisposeWith(disposables);

            #endregion

            #region 颜色绑定

            // 绑定标签前景色
            this.OneWayBind(
                viewModel: ViewModel,
                vmProperty: vm => vm.LableColor,
                viewProperty: v => v.Lable.Foreground,
                selector: color =>
                {
                    var converter = new ColorToBrushConverter();
                    return converter.Convert(color, typeof(IBrush), null, CultureInfo.CurrentCulture) as IBrush
                           ?? new SolidColorBrush(Colors.Transparent);
                }
            ).DisposeWith(disposables);

            // 标签没有单独边框颜色属性，边框默认跟随前景色
            this.OneWayBind(
                viewModel: ViewModel,
                vmProperty: vm => vm.LableColor,
                viewProperty: v => v.RootBorder.BorderBrush,
                selector: color =>
                {
                    var converter = new ColorToBrushConverter();
                    return converter.Convert(color, typeof(IBrush), null, CultureInfo.CurrentCulture) as IBrush
                           ?? new SolidColorBrush(Colors.Transparent);
                }
            ).DisposeWith(disposables);

            // 绑定背景颜色
            this.OneWayBind(
                viewModel: ViewModel,
                vmProperty: vm => vm.BackColor,
                viewProperty: v => v.RootBorder.Background,
                selector: color =>
                {
                    var converter = new ColorToBrushConverter();
                    return converter.Convert(color, typeof(IBrush), null, CultureInfo.CurrentCulture) as IBrush
                           ?? new SolidColorBrush(Colors.Transparent);
                }
            ).DisposeWith(disposables);

            #endregion

            #region 字体绑定

            // 绑定字体大小
            this.OneWayBind(
                viewModel: ViewModel,
                vmProperty: vm => vm.FontSize,
                viewProperty: v => v.Lable.FontSize
            ).DisposeWith(disposables);

            // 绑定是否加粗
            this.OneWayBind(
                viewModel: ViewModel,
                vmProperty: vm => vm.IsBold,
                viewProperty: v => v.Lable.FontWeight,
                selector: isBold => isBold ? FontWeight.Bold : FontWeight.Normal
            ).DisposeWith(disposables);

            // 绑定是否斜体
            this.OneWayBind(
                viewModel: ViewModel,
                vmProperty: vm => vm.IsItalic,
                viewProperty: v => v.Lable.FontStyle,
                selector: isItalic => isItalic ? FontStyle.Italic : FontStyle.Normal
            ).DisposeWith(disposables);

            // 绑定是否下划线
            this.OneWayBind(
                viewModel: ViewModel,
                vmProperty: vm => vm.IsUnderline,
                viewProperty: v => v.Lable.TextDecorations,
                selector: isUnderline => isUnderline ? TextDecorations.Underline : null
            ).DisposeWith(disposables);

            #endregion

            #region 段落绑定

            // 绑定行高 (LineHeight 是倍数值，如1.2表示1.2倍行高)
            ViewModel.WhenAnyValue(vm => vm.LineHeight, vm => vm.FontSize)
                .Subscribe(tuple =>
                {
                    var (lineHeight, fontSize) = tuple;
                    Lable.LineHeight = lineHeight * fontSize;
                }).DisposeWith(disposables);

            // 绑定段落前后间距 (通过 Padding 实现)
            Observable.CombineLatest(
                ViewModel.WhenAnyValue(vm => vm.ParagraphSpacingBefore),
                ViewModel.WhenAnyValue(vm => vm.ParagraphSpacingAfter),
                ViewModel.WhenAnyValue(vm => vm.IsUnderline),
                ViewModel.WhenAnyValue(vm => vm.FontSize),
                (before, after, isUnderline, fontSize) =>
                {
                    var extraBottom = isUnderline ? Math.Max(1.0, Math.Ceiling(fontSize * 0.15)) : 0.0;
                    return new Thickness(0, before, 0, after + extraBottom);
                }
            ).Subscribe(padding => Lable.Padding = padding).DisposeWith(disposables);

            // 绑定文本对齐方式
            this.OneWayBind(ViewModel, vm => vm.TextAlignment, v => v.Lable.TextAlignment,
                align => (TextAlignment)align
            ).DisposeWith(disposables);

            // 绑定文本垂直对齐方式
            this.OneWayBind(ViewModel, vm => vm.VerticalTextAlignment, v => v.Lable.VerticalAlignment,
                align => align switch
                {
                    TextVerticalAlignType.Top => Avalonia.Layout.VerticalAlignment.Top,
                    TextVerticalAlignType.Center => Avalonia.Layout.VerticalAlignment.Center,
                    TextVerticalAlignType.Bottom => Avalonia.Layout.VerticalAlignment.Bottom,
                    TextVerticalAlignType.Stretch => Avalonia.Layout.VerticalAlignment.Stretch,
                    _ => Avalonia.Layout.VerticalAlignment.Center
                }
            ).DisposeWith(disposables);

            #endregion

            #region 布局绑定

            // 标签外层尺寸交由宿主统一承接，这里不再重复把父类宽高直赋到 View。

            // 绑定水平对齐
            this.OneWayBind(ViewModel, vm => vm.HorizontalAlign, v => v.HorizontalAlignment,
               align => (Avalonia.Layout.HorizontalAlignment)align
            ).DisposeWith(disposables);

            // 绑定垂直对齐
            this.OneWayBind(ViewModel, vm => vm.VerticalAlign, v => v.VerticalAlignment,
                align => (Avalonia.Layout.VerticalAlignment)align
            ).DisposeWith(disposables);

            #endregion

            #region 外观绑定

            // 绑定透明度
            this.OneWayBind(
                viewModel: ViewModel,
                vmProperty: vm => vm.Opacity,
                viewProperty: v => v.Opacity
            ).DisposeWith(disposables);

            this.OneWayBind(
                viewModel: ViewModel,
                vmProperty: vm => vm.BorderThickness,
                viewProperty: v => v.RootBorder.BorderThickness
            ).DisposeWith(disposables);

            #endregion

            #region 交互绑定

            // 绑定鼠标光标样式
            ViewModel.WhenAnyValue(vm => vm.CursorType)
                .Subscribe(cursorType =>
                {
                    this.Cursor = ConvertToCursor(cursorType);
                }).DisposeWith(disposables);

            // 绑定提示文字
            ViewModel.WhenAnyValue(vm => vm.ToolTip)
                .Subscribe(toolTip =>
                {
                    if (!string.IsNullOrEmpty(toolTip))
                        Avalonia.Controls.ToolTip.SetTip(this, toolTip);
                    else
                        Avalonia.Controls.ToolTip.SetTip(this, null);
                }).DisposeWith(disposables);

            #endregion

            #region 视觉效果

            // 背景颜色变化时闪烁效果
            ViewModel.WhenAnyValue(vm => vm.BackColor)
                .Throttle(TimeSpan.FromMilliseconds(200))
                .SelectMany(_ =>
                {
                    SetLableOpacity(0.8);
                    return Observable.Timer(TimeSpan.FromMilliseconds(200));
                })
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => SetLableOpacity(1))
                .DisposeWith(disposables);

            ApplyInitialStateFromViewModel();

            #endregion
        });
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 首帧直接把当前 ViewModel 的真实值同步到控件，避免切页时先显示默认布局或空文本。
    /// </summary>
    public void ApplyInitialStateFromViewModel()
    {
        if (ViewModel == null)
            return;

        var brushConverter = new ColorToBrushConverter();

        Lable.Text = ViewModel.LableText;
        Lable.Foreground = brushConverter.Convert(ViewModel.LableColor, typeof(IBrush), null, CultureInfo.CurrentCulture) as IBrush
                           ?? new SolidColorBrush(Colors.Transparent);
        RootBorder.BorderBrush = brushConverter.Convert(ViewModel.LableColor, typeof(IBrush), null, CultureInfo.CurrentCulture) as IBrush
                                 ?? new SolidColorBrush(Colors.Transparent);
        RootBorder.Background = brushConverter.Convert(ViewModel.BackColor, typeof(IBrush), null, CultureInfo.CurrentCulture) as IBrush
                                ?? new SolidColorBrush(Colors.Transparent);

        Lable.FontSize = ViewModel.FontSize;
        Lable.FontWeight = ViewModel.IsBold ? FontWeight.Bold : FontWeight.Normal;
        Lable.FontStyle = ViewModel.IsItalic ? FontStyle.Italic : FontStyle.Normal;
        Lable.TextDecorations = ViewModel.IsUnderline ? TextDecorations.Underline : null;
        Lable.LineHeight = ViewModel.LineHeight * ViewModel.FontSize;
        Lable.Padding = new Thickness(
            0,
            ViewModel.ParagraphSpacingBefore,
            0,
            ViewModel.ParagraphSpacingAfter + (ViewModel.IsUnderline ? Math.Max(1.0, Math.Ceiling(ViewModel.FontSize * 0.15)) : 0.0));
        Lable.TextAlignment = (TextAlignment)ViewModel.TextAlignment;
        Lable.VerticalAlignment = ViewModel.VerticalTextAlignment switch
        {
            TextVerticalAlignType.Top => VerticalAlignment.Top,
            TextVerticalAlignType.Center => VerticalAlignment.Center,
            TextVerticalAlignType.Bottom => VerticalAlignment.Bottom,
            TextVerticalAlignType.Stretch => VerticalAlignment.Stretch,
            _ => VerticalAlignment.Center
        };

        RootBorder.BorderThickness = ViewModel.BorderThickness;
        Opacity = ViewModel.Opacity;
        HorizontalAlignment = (HorizontalAlignment)ViewModel.HorizontalAlign;
        VerticalAlignment = (VerticalAlignment)ViewModel.VerticalAlign;
        Cursor = ConvertToCursor(ViewModel.CursorType);

        if (!string.IsNullOrEmpty(ViewModel.ToolTip))
            Avalonia.Controls.ToolTip.SetTip(this, ViewModel.ToolTip);
        else
            Avalonia.Controls.ToolTip.SetTip(this, null);
    }

    /// <summary>
    /// 设置标签控件的透明度
    /// </summary>
    /// <param name="opacity">透明度值 (0-1)</param>
    private void SetLableOpacity(double opacity)
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            Lable.Opacity = opacity;
        }
        else
        {
            Dispatcher.UIThread.Post(() =>
            {
                Lable.Opacity = opacity;
            });
        }
    }

    /// <summary>
    /// 将 CursorType 枚举转换为 Avalonia Cursor
    /// </summary>
    /// <param name="cursorType">光标类型枚举</param>
    /// <returns>Avalonia 光标对象</returns>
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
