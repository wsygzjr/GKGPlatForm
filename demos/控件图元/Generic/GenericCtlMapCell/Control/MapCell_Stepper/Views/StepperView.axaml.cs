using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using Avalonia.VisualTree;
using GKG.Map.MapCell.Generic.Control.Lable;
using GKG.Map.MapCell.Generic.Stepper.ViewModels;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace GKG.Map.MapCell.Generic.Stepper.Views
{
    /// <summary>
    /// 步进器图元视图
    /// </summary>
    public partial class StepperView : ReactiveUserControl<StepperViewModel>
    {
        public StepperView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                if (ViewModel == null) return;

                #region 公共属性绑定

                // 标签文本
                this.OneWayBind(ViewModel,
                    vm => vm.LabelName,
                    view => view.LabelText.Text)
                    .DisposeWith(disposables);

                // 数值相关 (双向绑定)
                this.Bind(ViewModel,
                    vm => vm.Value,
                    view => view.Stepper.Value)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                    vm => vm.Minimum,
                    view => view.Stepper.Minimum)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                    vm => vm.Maximum,
                    view => view.Stepper.Maximum)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                    vm => vm.Increment,
                    view => view.Stepper.Increment)
                    .DisposeWith(disposables);

                // 小数位数 -> FormatString
                ViewModel.WhenAnyValue(x => x.DecimalPlaces)
                    .Subscribe(places =>
                    {
                        if (Stepper != null && places >= 0)
                        {
                            Stepper.FormatString = $"F{places}";
                        }
                    })
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                    vm => vm.IsEnabled,
                    view => view.Stepper.IsEnabled)
                    .DisposeWith(disposables);

                // 移除重复的 OneWayBind，因为下面的 WhenAnyValue 订阅已经处理了 ToolTip 的设置
                /*
                this.OneWayBind(ViewModel,
                    vm => vm.ToolTip,
                    view => Avalonia.Controls.ToolTip.TipProperty) 
                    .DisposeWith(disposables);
                */
                    
                ViewModel.WhenAnyValue(vm => vm.ToolTip)
                   .Subscribe(toolTip =>
                   {
                       if (!string.IsNullOrEmpty(toolTip))
                           Avalonia.Controls.ToolTip.SetTip(Stepper, toolTip);
                       else
                           Avalonia.Controls.ToolTip.SetTip(Stepper, null);
                   }).DisposeWith(disposables);

                #endregion

                #region 颜色绑定

                this.OneWayBind(ViewModel,
                    vm => vm.BackColor,
                    view => view.BackBorder.Background,
                    color => new SolidColorBrush(color))
                    .DisposeWith(disposables);

                // 边框色应用到 BackBorder
                this.OneWayBind(ViewModel,
                    vm => vm.BorderColor,
                    view => view.BackBorder.BorderBrush,
                    color => new SolidColorBrush(color))
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                    vm => vm.ForeColor,
                    view => view.Stepper.Foreground,
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
                ).Subscribe(thickness => 
                {
                    // 如果 BackBorder 用于背景，那么它应该有边框
                    BackBorder.BorderThickness = thickness;
                    BackBorder.BorderBrush = new SolidColorBrush(ViewModel.BorderColor);
                }).DisposeWith(disposables);

                #endregion

                #region 字体绑定

                // 绑定到 LabelText 和 Stepper
                this.OneWayBind(ViewModel, vm => vm.FontFamilyObj, v => v.LabelText.FontFamily).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.FontFamilyObj, v => v.Stepper.FontFamily).DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.FontColor, v => v.LabelText.Foreground, c => new SolidColorBrush(c)).DisposeWith(disposables);
                // Stepper Foreground already bound above

                this.OneWayBind(ViewModel, vm => vm.FontSize, v => v.LabelText.FontSize).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.FontSize, v => v.Stepper.FontSize).DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.IsBold, v => v.LabelText.FontWeight, b => b ? FontWeight.Bold : FontWeight.Normal).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.IsBold, v => v.Stepper.FontWeight, b => b ? FontWeight.Bold : FontWeight.Normal).DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.IsItalic, v => v.LabelText.FontStyle, i => i ? FontStyle.Italic : FontStyle.Normal).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.IsItalic, v => v.Stepper.FontStyle, i => i ? FontStyle.Italic : FontStyle.Normal).DisposeWith(disposables);

                #endregion

                #region 布局绑定

                // 外层 StepperView 是宿主和绿色外框管理的图元本体，布局边距只作用到内部内容。

                this.OneWayBind(ViewModel, vm => vm.HorizontalAlign, v => v.BackBorder.HorizontalAlignment,
                   align => (Avalonia.Layout.HorizontalAlignment)align)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.VerticalAlign, v => v.BackBorder.VerticalAlignment,
                    align => (Avalonia.Layout.VerticalAlignment)align)
                    .DisposeWith(disposables);

                Observable.CombineLatest(
                    ViewModel.WhenAnyValue(vm => vm.MarginLeft),
                    ViewModel.WhenAnyValue(vm => vm.MarginTop),
                    ViewModel.WhenAnyValue(vm => vm.MarginRight),
                    ViewModel.WhenAnyValue(vm => vm.MarginBottom),
                    (left, top, right, bottom) => new Thickness(left, top, right, bottom)
                ).Subscribe(margin => BackBorder.Margin = margin).DisposeWith(disposables);

                ViewModel.WhenAnyValue(vm => vm.MinWidth)
                    .Subscribe(minWidth => MinWidth = minWidth > 0 ? minWidth : 0)
                    .DisposeWith(disposables);

                ViewModel.WhenAnyValue(vm => vm.MaxWidth)
                    .Subscribe(maxWidth => MaxWidth = maxWidth > 0 ? maxWidth : double.PositiveInfinity)
                    .DisposeWith(disposables);

                ViewModel.WhenAnyValue(vm => vm.MinHeight)
                    .Subscribe(minHeight => MinHeight = minHeight > 0 ? minHeight : 0)
                    .DisposeWith(disposables);

                ViewModel.WhenAnyValue(vm => vm.MaxHeight)
                    .Subscribe(maxHeight => MaxHeight = maxHeight > 0 ? maxHeight : double.PositiveInfinity)
                    .DisposeWith(disposables);

                #endregion

                #region 外观绑定

                this.OneWayBind(ViewModel,
                    vm => vm.Opacity,
                    view => view.Opacity)
                    .DisposeWith(disposables);

                #endregion
            });
        }
    }
}
