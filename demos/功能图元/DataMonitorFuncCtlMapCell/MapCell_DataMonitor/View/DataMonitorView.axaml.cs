using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using GKG.Map.DataMonitorFuncCtlMapCell.ViewModel;
using GKG.Map.DeviceStatusFuncCtlMapCell.MapCell_DeviceStatus.Objects;
using GKG.Map.DeviceStatusFuncCtlMapCell.MapCell_DeviceStatus.ViewModels;
using Griffins.Map;
using GKG.Map.DataMonitorFuncCtlMapCell.Convert;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Griffins.UI2;

namespace GKG.Map.DataMonitorFuncCtlMapCell.View;

/// <summary>
/// 数据监控功能图元视图类
/// 负责显示数据监控图元的用户界面，包括背景颜色、文本颜色等属性的绑定
/// </summary>
public partial class DataMonitorView : ReactiveUserControl<DataMonitorViewModel>
{
    #region 构造函数
    /// <summary>
    /// 初始化数据监控视图
    /// 设置组件初始化和数据绑定
    /// </summary>
    public DataMonitorView()
    {
        InitializeComponent();

        #region 视图激活时的数据绑定
        this.WhenActivated(disposables =>
        {
            if (ViewModel == null) return;

            // WhenActivated：只有在View真正被加入视觉树并激活时才建立订阅，
            // 并由CompositeDisposable统一管理释放，避免内存泄漏。

            #region 背景颜色绑定
            // Bind background color
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
            #endregion

            #region 文本颜色绑定
            // 颜色/字体变化后，遍历可视树给TextBlock/TextBox/Button统一设置前景和字体。
            this.WhenAnyValue(x => x.ViewModel!.TextColor)
                .Subscribe(c => ApplyTextColor(new SolidColorBrush(c)))
                .DisposeWith(disposables);

            this.WhenAnyValue(x => x.ViewModel!.TextFont)
                .Subscribe(f => ApplyTextFont(f))
                .DisposeWith(disposables);

            this.WhenAnyValue(x => x.ViewModel!.TextFont.FontFamily)
                .Subscribe(_ => ApplyTextFont(ViewModel.TextFont))
                .DisposeWith(disposables);

            this.WhenAnyValue(x => x.ViewModel!.TextFont.FontSize)
                .Subscribe(_ => ApplyTextFont(ViewModel.TextFont))
                .DisposeWith(disposables);

            this.WhenAnyValue(x => x.ViewModel!.TextFont.FontWeight)
                .Subscribe(_ => ApplyTextFont(ViewModel.TextFont))
                .DisposeWith(disposables);

            this.WhenAnyValue(x => x.ViewModel!.TextFont.FontStyle)
                .Subscribe(_ => ApplyTextFont(ViewModel.TextFont))
                .DisposeWith(disposables);
            #endregion

            #region 状态图片绑定
            // 绑定状态图片
            this.WhenAnyValue(v => v.ViewModel.StatusOkImage)
                .Subscribe(img =>
                {
                    ImgSafetyDoorOk.Source = img;
                    ImgTotalPressureOk.Source = img;
                    ImgCleaningClothOk.Source = img;
                })
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.StatusNgImage)
                .Subscribe(img =>
                {
                    ImgSafetyDoorNg.Source = img;
                    ImgTotalPressureNg.Source = img;
                    ImgCleaningClothNg.Source = img;
                })
                .DisposeWith(disposables);

            // 绑定监控信息栏扩展属性（根据图片重构）
            // 这些Subscribe主要用于触发UI刷新（InvalidateVisual/Measure/Arrange），
            // 以确保底部信息栏的内容变化能及时重绘。
            this.WhenAnyValue(v => v.ViewModel.GlueRemaining)
                .Subscribe(v =>
                {
                    // 强制更新UI - 使用调度器确保在UI线程执行
                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        UpdateGlueRemainingFill(
                            containerWidth: RightGlueRemainingBarContainer?.Bounds.Width ?? 0,
                            glueRemaining: ViewModel.GlueRemaining,
                            fillBorder: RightGlueRemainingFillBorder,
                            percentText: RightGlueRemainingPercentText);

                        this.InvalidateVisual();
                        this.InvalidateMeasure();
                        this.InvalidateArrange();
                    });
                })
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.RightGlueRemainingBarContainer.Bounds.Width)
                .Subscribe(_ =>
                {
                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        UpdateGlueRemainingFill(
                            containerWidth: RightGlueRemainingBarContainer?.Bounds.Width ?? 0,
                            glueRemaining: ViewModel.GlueRemaining,
                            fillBorder: RightGlueRemainingFillBorder,
                            percentText: RightGlueRemainingPercentText);
                    });
                })
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.LeftGlueRemaining)
                .Subscribe(_ =>
                {
                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        UpdateGlueRemainingFill(
                            containerWidth: LeftGlueRemainingBarContainer?.Bounds.Width ?? 0,
                            glueRemaining: ViewModel.LeftGlueRemaining,
                            fillBorder: LeftGlueRemainingFillBorder,
                            percentText: LeftGlueRemainingPercentText);
                    });
                })
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.LeftGlueRemainingBarContainer.Bounds.Width)
                .Subscribe(_ =>
                {
                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        UpdateGlueRemainingFill(
                            containerWidth: LeftGlueRemainingBarContainer?.Bounds.Width ?? 0,
                            glueRemaining: ViewModel.LeftGlueRemaining,
                            fillBorder: LeftGlueRemainingFillBorder,
                            percentText: LeftGlueRemainingPercentText);
                    });
                })
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.Calibration)
                .Subscribe(v =>
                {
                    // 强制更新UI
                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        this.InvalidateVisual();
                        this.InvalidateMeasure();
                        this.InvalidateArrange();
                    });
                })
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.ValveBodyValue)
                .Subscribe(v =>
                {
                    // 强制更新UI
                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        this.InvalidateVisual();
                        this.InvalidateMeasure();
                        this.InvalidateArrange();
                    });
                })
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.SealingRingValue)
                .Subscribe(v =>
                {
                    // 强制更新UI
                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        this.InvalidateVisual();
                        this.InvalidateMeasure();
                        this.InvalidateArrange();
                    });
                })
                .DisposeWith(disposables);

            // 绑定状态切换
            this.WhenAnyValue(v => v.ViewModel.SafetyDoorStatus)
                .Subscribe(v =>
                {
                    // 直接更新图片可见性
                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        ImgSafetyDoorOk.IsVisible = v;
                        ImgSafetyDoorNg.IsVisible = !v;
                        ImgSafetyDoorOk.InvalidateVisual();
                        ImgSafetyDoorNg.InvalidateVisual();
                    });
                })
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.TotalPressureStatus)
                .Subscribe(v =>
                {
                    // 直接更新图片可见性
                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        ImgTotalPressureOk.IsVisible = v;
                        ImgTotalPressureNg.IsVisible = !v;
                        ImgTotalPressureOk.InvalidateVisual();
                        ImgTotalPressureNg.InvalidateVisual();
                    });
                })
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.CleaningClothStatus)
                .Subscribe(v =>
                {
                    // 直接更新图片可见性
                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        ImgCleaningClothOk.IsVisible = v;
                        ImgCleaningClothNg.IsVisible = !v;
                        ImgCleaningClothOk.InvalidateVisual();
                        ImgCleaningClothNg.InvalidateVisual();
                    });
                })
                .DisposeWith(disposables);
            #endregion

            #region 创建DeviceStatusCtlMapCell实例
            // 顶部三个设备状态栏改为“控件级复用(DeviceStatusView)”：XAML直接引用DeviceStatusView并绑定到ViewModel中的DeviceStatusViewModel。
            // 这里仅保留背景色联动：根据CurrentIndex切换左侧图片区域背景（Water->绿，Close->灰）。
            this.WhenAnyValue(v => v.ViewModel!.SupplyValvePressureDeviceStatusVm.CurrentIndex)
                .Subscribe(i => UpdateDeviceStatusBackground(SupplyValvePressureControl, i))
                .DisposeWith(disposables);
            this.WhenAnyValue(v => v.ViewModel!.SupplyGluePressureDeviceStatusVm.CurrentIndex)
                .Subscribe(i => UpdateDeviceStatusBackground(SupplyGluePressureControl, i))
                .DisposeWith(disposables);
            this.WhenAnyValue(v => v.ViewModel!.NozzleHeatingDeviceStatusVm.CurrentIndex)
                .Subscribe(i => UpdateDeviceStatusBackground(NozzleHeatingControl, i))
                .DisposeWith(disposables);

            // 初始化背景色
            UpdateDeviceStatusBackground(SupplyValvePressureControl, ViewModel.SupplyValvePressureDeviceStatusVm.CurrentIndex);
            UpdateDeviceStatusBackground(SupplyGluePressureControl, ViewModel.SupplyGluePressureDeviceStatusVm.CurrentIndex);
            UpdateDeviceStatusBackground(NozzleHeatingControl, ViewModel.NozzleHeatingDeviceStatusVm.CurrentIndex);

            ApplyTextColor(new SolidColorBrush(ViewModel.TextColor));
            ApplyTextFont(ViewModel.TextFont);
            #endregion
        });
        #endregion
    }

    private static void UpdateGlueRemainingFill(double containerWidth, string glueRemaining, Border fillBorder, TextBlock percentText)
    {
        if (fillBorder == null)
            return;

        var percent = ParseGlueRemainingPercent(glueRemaining);
        var safeWidth = Math.Max(0, containerWidth);
        fillBorder.Width = safeWidth * percent;

        if (percentText != null)
            percentText.Text = $"{Math.Round(percent * 100)}%";
    }

    private static double ParseGlueRemainingPercent(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return 0;

        var s = input.Trim();

        // 常见格式："50%" / "50 %" / "0.5" / "50"。
        var hasPercentSign = s.Contains('%');
        s = s.Replace("%", string.Empty).Trim();

        // 尝试从字符串中提取第一个数字片段。
        var numberChars = new List<char>(s.Length);
        var started = false;
        foreach (var ch in s)
        {
            if ((ch >= '0' && ch <= '9') || ch == '.' || ch == ',')
            {
                started = true;
                numberChars.Add(ch == ',' ? '.' : ch);
            }
            else if (started)
            {
                break;
            }
        }

        var numStr = new string(numberChars.ToArray());
        if (!double.TryParse(numStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
            return 0;

        // 解析规则：
        // - 带%号：按 0-100 处理（"1%" => 0.01）。
        // - 不带%号：
        //   - 形如 "0.5"（含小数点且 <= 1）：按 0-1 小数处理（"0.5" => 0.5）。
        //   - 形如 "1" / "50"：按 0-100 处理（"1" => 0.01，"50" => 0.5）。
        var hasDecimalPoint = s.Contains('.') || s.Contains(',');
        var percent = hasPercentSign
            ? value / 100.0
            : (hasDecimalPoint && value <= 1 ? value : value / 100.0);
        if (double.IsNaN(percent) || double.IsInfinity(percent))
            return 0;
        return Math.Clamp(percent, 0, 1);
    }

    private void ApplyTextColor(IBrush brush)
    {
        if (brush == null) return;

        // 统一设置可视树内常用文本控件的前景色。
        foreach (var tb in MainBorder.GetVisualDescendants().OfType<TextBlock>())
            tb.Foreground = brush;
        foreach (var txt in MainBorder.GetVisualDescendants().OfType<TextBox>())
            txt.Foreground = brush;
        foreach (var btn in MainBorder.GetVisualDescendants().OfType<Button>())
            btn.Foreground = brush;
    }

    private void ApplyTextFont(FontInfo font)
    {
        if (font == null) return;

        // 统一设置可视树内常用文本控件的字体属性。

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

        foreach (var btn in MainBorder.GetVisualDescendants().OfType<Button>())
        {
            btn.FontFamily = font.FontFamily;
            btn.FontSize = font.FontSize;
            btn.FontWeight = font.FontWeight;
            btn.FontStyle = font.FontStyle;
        }
    }

    /// <summary>
    /// 更新设备状态栏的背景颜色
    /// </summary>
    /// <param name="control">DeviceStatusCtlMapCell视图</param>
    /// <param name="currentIndex">当前图片索引</param>
    private void UpdateDeviceStatusBackground(UserControl control, int currentIndex)
    {
        if (control == null) return;

        // 根据CurrentIndex决定背景颜色
        // 0 (Water图片) -> 绿色
        // 1 (Close图片) -> 灰色
        var backgroundColor = currentIndex == 0 ?
            new SolidColorBrush(Colors.LimeGreen) :   // Water状态 - 绿色
            new SolidColorBrush(Colors.Gray);        // Close状态 - 灰色

        // 精确设置左边图片区域的背景颜色
        TryDirectBackgroundSet(control, backgroundColor);
    }

    /// <summary>
    /// 设置左边图片区域的背景颜色
    /// </summary>
    /// <param name="control">控件</param>
    /// <param name="backgroundColor">背景颜色</param>
    private void TryDirectBackgroundSet(Control control, SolidColorBrush backgroundColor)
    {
        try
        {
            // 根据DeviceStatusView的结构精确查找
            // 结构：UserControl -> Border(OuterBorder) -> Grid -> Border(左边图片) + TextBlocks

            if (control is UserControl userControl)
            {
                // 查找OuterBorder
                if (userControl.Content is Border outerBorder && outerBorder.Child is Grid grid)
                {
                    // 在Grid中查找左边的Border (包含Image的那个)
                    foreach (var child in grid.Children)
                    {
                        if (child is Border imageBorder)
                        {
                            // 检查是否是左边的Border（包含Image）
                            if (imageBorder.Child is Image)
                            {
                                imageBorder.Background = backgroundColor;
                                return;
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // 静默处理异常，不影响主要功能
        }
    }
    #endregion
}
