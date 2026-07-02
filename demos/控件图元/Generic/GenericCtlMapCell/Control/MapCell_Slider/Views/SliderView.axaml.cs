using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using GKG.Map.MapCell.Generic.Control.MapCell_Slider.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Slider.Views;

public partial class SliderView : ReactiveUserControl<SliderViewModel>
{
    public SliderView()
    {
        InitializeComponent();

        // 统一处理键盘/滚轮的步进（确保跟随SmallChange）
        CustomSlider.KeyDown += CustomSlider_KeyDown;
        CustomSlider.PointerWheelChanged += CustomSlider_PointerWheelChanged;
        CustomSlider.PointerPressed += CustomSlider_PointerPressed;

        this.WhenActivated(disposables =>
        {
            if (ViewModel == null)
            {
                return;
            }

            // 绑定水平对齐
            ViewModel.WhenAnyValue(vm => vm.LayoutInfo.HorizontalAlignment)
                .Subscribe(alignment =>
                {
                    // 直接映射水平对齐，不再反转
                    var ha = alignment switch
                    {
                        GKG.Map.MapCell.Generic.Control.MapCell_Slider.SliderLayoutInfo.HorizontalAlignmentEnum.Left => Avalonia.Layout.HorizontalAlignment.Left,
                        GKG.Map.MapCell.Generic.Control.MapCell_Slider.SliderLayoutInfo.HorizontalAlignmentEnum.Center => Avalonia.Layout.HorizontalAlignment.Center,
                        GKG.Map.MapCell.Generic.Control.MapCell_Slider.SliderLayoutInfo.HorizontalAlignmentEnum.Right => Avalonia.Layout.HorizontalAlignment.Right,
                        GKG.Map.MapCell.Generic.Control.MapCell_Slider.SliderLayoutInfo.HorizontalAlignmentEnum.Stretch => Avalonia.Layout.HorizontalAlignment.Stretch,
                        _ => Avalonia.Layout.HorizontalAlignment.Stretch
                    };

                    this.HorizontalAlignment = ha;
                })
                .DisposeWith(disposables);

            // 绑定垂直对齐
            ViewModel.WhenAnyValue(vm => vm.LayoutInfo.VerticalAlignment)
                .Subscribe(alignment =>
                {
                    // 直接映射垂直对齐，不再反转
                    var va = alignment switch
                    {
                        GKG.Map.MapCell.Generic.Control.MapCell_Slider.SliderLayoutInfo.VerticalAlignmentEnum.Top => Avalonia.Layout.VerticalAlignment.Top,
                        GKG.Map.MapCell.Generic.Control.MapCell_Slider.SliderLayoutInfo.VerticalAlignmentEnum.Center => Avalonia.Layout.VerticalAlignment.Center,
                        GKG.Map.MapCell.Generic.Control.MapCell_Slider.SliderLayoutInfo.VerticalAlignmentEnum.Bottom => Avalonia.Layout.VerticalAlignment.Bottom,
                        GKG.Map.MapCell.Generic.Control.MapCell_Slider.SliderLayoutInfo.VerticalAlignmentEnum.Stretch => Avalonia.Layout.VerticalAlignment.Stretch,
                        _ => Avalonia.Layout.VerticalAlignment.Stretch
                    };

                    this.VerticalAlignment = va;
                })
                .DisposeWith(disposables);

            // 绑定Margin相关属性
            ViewModel.WhenAnyValue(
                vm => vm.LayoutInfo.MarginLeft,
                vm => vm.LayoutInfo.MarginTop,
                vm => vm.LayoutInfo.MarginRight,
                vm => vm.LayoutInfo.MarginBottom)
                .Subscribe(_ =>
                {
                    // 更新Slider的Margin
                    var margin = new Thickness(
                        ViewModel.LayoutInfo.MarginLeft,
                        ViewModel.LayoutInfo.MarginTop,
                        ViewModel.LayoutInfo.MarginRight,
                        ViewModel.LayoutInfo.MarginBottom);
                    this.Margin = margin;
                    RootBorder.Margin = new Thickness(0);
                })
                .DisposeWith(disposables);

            // 存储之前的方向，用于检测方向变化
            var previousDirection = ViewModel.CommonInfo.Direction;

            // 绑定方向变化
            ViewModel.WhenAnyValue(vm => vm.CommonInfo.Direction)
                .Subscribe(newDirection =>
                {
                    if (previousDirection != newDirection)
                    {
                        previousDirection = newDirection;

                        // 强制重新布局
                        this.InvalidateMeasure();
                        this.InvalidateArrange();
                        CustomSlider.InvalidateMeasure();
                        CustomSlider.InvalidateArrange();
                    }
                })
                .DisposeWith(disposables);

            // 绑定最小变化量
            ViewModel.WhenAnyValue(vm => vm.CommonInfo.SmallChange)
                .Subscribe(smallChange =>
                {
                    CustomSlider.SmallChange = smallChange;
                })
                .DisposeWith(disposables);

            // 绑定值变化
            ViewModel.WhenAnyValue(vm => vm.CommonInfo.Value)
                .Subscribe(value =>
                {
                })
                .DisposeWith(disposables);

            // 绑定背景色变化
            ViewModel.WhenAnyValue(vm => vm.BrushInfo.BackgroundColor)
                .Subscribe(backgroundColor =>
                {
                    // 强制刷新整个控件树，确保背景色变化能立即反映到UI上
                    CustomSlider.InvalidateVisual();
                    CustomSlider.InvalidateMeasure();
                    CustomSlider.InvalidateArrange();
                })
                .DisposeWith(disposables);

            // 绑定透明度变化
            ViewModel.WhenAnyValue(vm => vm.OpacityValue)
                .Subscribe(opacityValue =>
                {
                    CustomSlider.Opacity = opacityValue;
                })
                .DisposeWith(disposables);

            // 绑定启用状态变化
            ViewModel.WhenAnyValue(vm => vm.CommonInfo.Enabled)
                .Subscribe(enabled =>
                {
                    CustomSlider.IsEnabled = enabled;
                })
                .DisposeWith(disposables);
            
            // 绑定工具提示
            ViewModel.WhenAnyValue(vm => vm.CommonInfo.TooltipText)
                .Subscribe(tooltipText =>
                {
                    if (!string.IsNullOrEmpty(tooltipText))
                        Avalonia.Controls.ToolTip.SetTip(CustomSlider, tooltipText);
                    else
                        Avalonia.Controls.ToolTip.SetTip(CustomSlider, null);
                })
                .DisposeWith(disposables);
        });
    }

    private void CustomSlider_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        // 确保能接收上下方向键
        CustomSlider.Focus();
    }

    private void CustomSlider_KeyDown(object sender, KeyEventArgs e)
    {
        if (DataContext is not SliderViewModel viewModel)
            return;
        if (!viewModel.CommonInfo.Enabled)
            return;

        int step = viewModel.CommonInfo.SmallChange <= 0 ? 1 : viewModel.CommonInfo.SmallChange;

        if (e.Key == Key.Up)
        {
            viewModel.CommonInfo.Value = viewModel.CommonInfo.Value + step;
            e.Handled = true;
        }
        else if (e.Key == Key.Down)
        {
            viewModel.CommonInfo.Value = viewModel.CommonInfo.Value - step;
            e.Handled = true;
        }
    }

    private void CustomSlider_PointerWheelChanged(object sender, PointerWheelEventArgs e)
    {
        if (DataContext is not SliderViewModel viewModel)
            return;
        if (!viewModel.CommonInfo.Enabled)
            return;

        // 约定：向上滚轮=增加，向下滚轮=减少
        int step = viewModel.CommonInfo.SmallChange <= 0 ? 1 : viewModel.CommonInfo.SmallChange;
        if (e.Delta.Y > 0)
        {
            viewModel.CommonInfo.Value = viewModel.CommonInfo.Value + step;
            e.Handled = true;
        }
        else if (e.Delta.Y < 0)
        {
            viewModel.CommonInfo.Value = viewModel.CommonInfo.Value - step;
            e.Handled = true;
        }
    }
}
