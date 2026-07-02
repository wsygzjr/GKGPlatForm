using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using GKG.Map.MapCell.Generic.Control.MapCell_TextBox.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace GKG.Map.MapCell.Generic.Control.MapCell_TextBox.Views
{
    /// <summary>
    /// 文本输入框视图
    /// </summary>
    public partial class TextBoxView : ReactiveUserControl<TextBoxViewModel>
    {
        public TextBoxView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                if (ViewModel == null)
                    return;

                // 订阅方式绑定：用于把 LayoutInfo 的变更实时同步到实际控件属性
                // （某些属性使用订阅处理 NaN/枚举映射等逻辑，避免 XAML 绑定中写复杂转换）

                // 文本框试点按图片图元的尺寸承接方式收口：外层尺寸由宿主统一承接，这里不再重复把父类宽高直赋到内部控件。

                // 对齐
                ViewModel.WhenAnyValue(vm => vm.LayoutInfo.HorizontalAlignment)
                    .Subscribe(alignment =>
                    {
                        // 属性面板枚举 -> Avalonia 对齐枚举
                        InputTextBox.HorizontalAlignment = alignment switch
                        {
                            TextBoxLayoutInfo.HorizontalAlignmentEnum.Left => Avalonia.Layout.HorizontalAlignment.Left,
                            TextBoxLayoutInfo.HorizontalAlignmentEnum.Center => Avalonia.Layout.HorizontalAlignment.Center,
                            TextBoxLayoutInfo.HorizontalAlignmentEnum.Right => Avalonia.Layout.HorizontalAlignment.Right,
                            TextBoxLayoutInfo.HorizontalAlignmentEnum.Stretch => Avalonia.Layout.HorizontalAlignment.Stretch,
                            _ => Avalonia.Layout.HorizontalAlignment.Stretch
                        };
                    })
                    .DisposeWith(disposables);

                ViewModel.WhenAnyValue(vm => vm.LayoutInfo.VerticalAlignment)
                    .Subscribe(alignment =>
                    {
                        // 属性面板枚举 -> Avalonia 对齐枚举
                        InputTextBox.VerticalAlignment = alignment switch
                        {
                            TextBoxLayoutInfo.VerticalAlignmentEnum.Top => Avalonia.Layout.VerticalAlignment.Top,
                            TextBoxLayoutInfo.VerticalAlignmentEnum.Center => Avalonia.Layout.VerticalAlignment.Center,
                            TextBoxLayoutInfo.VerticalAlignmentEnum.Bottom => Avalonia.Layout.VerticalAlignment.Bottom,
                            TextBoxLayoutInfo.VerticalAlignmentEnum.Stretch => Avalonia.Layout.VerticalAlignment.Stretch,
                            _ => Avalonia.Layout.VerticalAlignment.Stretch
                        };
                    })
                    .DisposeWith(disposables);

                ViewModel.WhenAnyValue(vm => vm.TextInfo.TextAlignment)
                    .Subscribe(alignment =>
                    {
                        InputTextBox.TextAlignment = alignment switch
                        {
                            TextBoxTextAlignmentType.Left => TextAlignment.Left,
                            TextBoxTextAlignmentType.Center => TextAlignment.Center,
                            TextBoxTextAlignmentType.Right => TextAlignment.Right,
                            TextBoxTextAlignmentType.Justify => TextAlignment.Justify,
                            _ => TextAlignment.Left
                        };
                    })
                    .DisposeWith(disposables);

                ViewModel.WhenAnyValue(vm => vm.TextInfo.VerticalTextAlignment)
                    .Subscribe(alignment =>
                    {
                        InputTextBox.VerticalContentAlignment = alignment switch
                        {
                            TextBoxVerticalTextAlignmentType.Top => Avalonia.Layout.VerticalAlignment.Top,
                            TextBoxVerticalTextAlignmentType.Center => Avalonia.Layout.VerticalAlignment.Center,
                            TextBoxVerticalTextAlignmentType.Bottom => Avalonia.Layout.VerticalAlignment.Bottom,
                            TextBoxVerticalTextAlignmentType.Stretch => Avalonia.Layout.VerticalAlignment.Stretch,
                            _ => Avalonia.Layout.VerticalAlignment.Center
                        };
                    })
                    .DisposeWith(disposables);

                // 在设计器/宿主中点击图元时，可能只触发“图元选中”，但不会让 TextBox 获得键盘焦点。
                // 为了让“选中时边框颜色”能够生效，这里在鼠标按下时主动给予焦点，触发 :focus-within。
                void OnPointerPressed(object sender, PointerPressedEventArgs e)
                {
                    if (e.GetCurrentPoint(InputTextBox).Properties.IsLeftButtonPressed)
                        InputTextBox.Focus();
                }

                InputTextBox.PointerPressed += OnPointerPressed;
                Disposable.Create(() => InputTextBox.PointerPressed -= OnPointerPressed)
                    .DisposeWith(disposables);

                // BorderThickness（四边）
                ViewModel.WhenAnyValue(
                        vm => vm.AppearanceInfo.BorderThicknessLeft,
                        vm => vm.AppearanceInfo.BorderThicknessTop,
                        vm => vm.AppearanceInfo.BorderThicknessRight,
                        vm => vm.AppearanceInfo.BorderThicknessBottom)
                    .Subscribe(v =>
                    {
                        var left = Math.Max(0, v.Item1);
                        var top = Math.Max(0, v.Item2);
                        var right = Math.Max(0, v.Item3);
                        var bottom = Math.Max(0, v.Item4);
                        InputTextBox.BorderThickness = new Thickness(left, top, right, bottom);
                    })
                    .DisposeWith(disposables);

                // Margin（四边）
                ViewModel.WhenAnyValue(
                        vm => vm.LayoutInfo.MarginLeft,
                        vm => vm.LayoutInfo.MarginTop,
                        vm => vm.LayoutInfo.MarginRight,
                        vm => vm.LayoutInfo.MarginBottom)
                    .Subscribe(v =>
                    {
                        var left = v.Item1;
                        var top = v.Item2;
                        var right = v.Item3;
                        var bottom = v.Item4;
                        InputTextBox.Margin = new Thickness(left, top, right, bottom);
                    })
                    .DisposeWith(disposables);

                // 选中文本的背景色：与文本框背景色保持一致
                ViewModel.WhenAnyValue(vm => vm.BrushInfo.BackgroundColor)
                    .Subscribe(bgColor =>
                    {
                        InputTextBox.SelectionBrush = new SolidColorBrush(bgColor);
                    })
                    .DisposeWith(disposables);

                // 所选文本透明度（0-100）-> 选中文字前景色 Alpha
                ViewModel.WhenAnyValue(vm => vm.CommonInfo.SelectedTextOpacity, vm => vm.TextInfo.FontColor)
                    .Subscribe(v =>
                    {
                        var opacity = Math.Max(0, Math.Min(100, v.Item1));
                        var fontColor = v.Item2;
                        // 属性面板语义与 AppearanceInfo.Opacity 一致：0=不透明，100=全透明
                        var a = (byte)Math.Round(255 * (1.0 - (opacity / 100.0)));
                        var selectedTextColor = Color.FromArgb(a, fontColor.R, fontColor.G, fontColor.B);
                        InputTextBox.SelectionForegroundBrush = new SolidColorBrush(selectedTextColor);
                    })
                    .DisposeWith(disposables);

                // ToolTip
                ViewModel.WhenAnyValue(vm => vm.CommonInfo.TooltipText)
                    .Subscribe(tooltipText =>
                    {
                        // 仅在有内容时设置提示，避免空字符串占位
                        if (!string.IsNullOrEmpty(tooltipText))
                            ToolTip.SetTip(InputTextBox, tooltipText);
                        else
                            ToolTip.SetTip(InputTextBox, null);
                    })
                    .DisposeWith(disposables);
            });
        }
    }
}
