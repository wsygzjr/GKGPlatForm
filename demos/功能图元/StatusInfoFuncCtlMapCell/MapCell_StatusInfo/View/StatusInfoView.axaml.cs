using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using GKG.Map.StatusInfoFuncCtlMapCell.ViewModel;
using Griffins.UI2;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace GKG.Map.StatusInfoFuncCtlMapCell.View;

/// <summary>
/// 状态信息视图
/// </summary>
public partial class StatusInfoView : ReactiveUserControl<StatusInfoViewModel>
{
    #region 构造函数
    /// <summary>
    /// 初始化状态信息视图
    /// </summary>
    public StatusInfoView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            if (ViewModel == null) return;

            #region 文本/图片绑定

            this.OneWayBind(ViewModel, vm => vm.AWaitingAddGlueTime, v => v.TxtATime.Text)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.BWaitingAddGlueTime, v => v.TxtBTime.Text)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.StatusOkImage)
                .Subscribe(img =>
                {
                    ImgLeftValveOk.Source = img;
                    ImgLeftValveQuantitativeOk.Source = img;
                    ImgLeftValveRemainOk.Source = img;
                    ImgLeftValveAlarmOk.Source = img;
                    ImgRightValveOk.Source = img;
                    ImgRightValveQuantitativeOk.Source = img;
                    ImgRightValveRemainOk.Source = img;
                    ImgRightValveAlarmOk.Source = img;
                })
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.StatusNgImage)
                .Subscribe(img =>
                {
                    ImgLeftValveNg.Source = img;
                    ImgLeftValveQuantitativeNg.Source = img;
                    ImgLeftValveRemainNg.Source = img;
                    ImgLeftValveAlarmNg.Source = img;
                    ImgRightValveNg.Source = img;
                    ImgRightValveQuantitativeNg.Source = img;
                    ImgRightValveRemainNg.Source = img;
                    ImgRightValveAlarmNg.Source = img;
                })
                .DisposeWith(disposables);

            #endregion

            #region 状态切换

            this.WhenAnyValue(v => v.ViewModel.LeftValveGlueMonitorState)
                .Subscribe(v =>
                {
                    ImgLeftValveOk.IsVisible = v;
                    ImgLeftValveNg.IsVisible = !v;
                })
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.LeftValveQuantitativeGlueMonitorState)
                .Subscribe(v =>
                {
                    ImgLeftValveQuantitativeOk.IsVisible = v;
                    ImgLeftValveQuantitativeNg.IsVisible = !v;
                })
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.LeftValveRemainingMonitorState)
                .Subscribe(v =>
                {
                    ImgLeftValveRemainOk.IsVisible = v;
                    ImgLeftValveRemainNg.IsVisible = !v;
                })
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.LeftPressureCyclesAlarmState)
                .Subscribe(v =>
                {
                    ImgLeftValveAlarmOk.IsVisible = v;
                    ImgLeftValveAlarmNg.IsVisible = !v;
                })
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.RightValveGlueMonitorState)
                .Subscribe(v =>
                {
                    ImgRightValveOk.IsVisible = v;
                    ImgRightValveNg.IsVisible = !v;
                })
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.RightValveQuantitativeGlueMonitorState)
                .Subscribe(v =>
                {
                    ImgRightValveQuantitativeOk.IsVisible = v;
                    ImgRightValveQuantitativeNg.IsVisible = !v;
                })
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.RightValveRemainingMonitorState)
                .Subscribe(v =>
                {
                    ImgRightValveRemainOk.IsVisible = v;
                    ImgRightValveRemainNg.IsVisible = !v;
                })
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.RightPressureCyclesAlarmState)
                .Subscribe(v =>
                {
                    ImgRightValveAlarmOk.IsVisible = v;
                    ImgRightValveAlarmNg.IsVisible = !v;
                })
                .DisposeWith(disposables);

            #endregion

            #region 字体/颜色绑定

            this.WhenAnyValue(v => v.ViewModel!.TextFont)
                .Subscribe(f => ApplyTextFont(f))
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.TextFont.FontFamily)
                .Subscribe(_ => ApplyTextFont(ViewModel.TextFont))
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.TextFont.FontSize)
                .Subscribe(_ => ApplyTextFont(ViewModel.TextFont))
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.TextFont.FontWeight)
                .Subscribe(_ => ApplyTextFont(ViewModel.TextFont))
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.TextFont.FontStyle)
                .Subscribe(_ => ApplyTextFont(ViewModel.TextFont))
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.TextColor, v => v.LblLeftValve.Foreground, color => (IBrush)new SolidColorBrush(color)).DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.TextColor, v => v.LblLeftValveQuantitative.Foreground, color => (IBrush)new SolidColorBrush(color)).DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.TextColor, v => v.LblLeftValveRemain.Foreground, color => (IBrush)new SolidColorBrush(color)).DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.TextColor, v => v.LblLeftValveAlarm.Foreground, color => (IBrush)new SolidColorBrush(color)).DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.TextColor, v => v.LblRightValve.Foreground, color => (IBrush)new SolidColorBrush(color)).DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.TextColor, v => v.LblRightValveQuantitative.Foreground, color => (IBrush)new SolidColorBrush(color)).DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.TextColor, v => v.LblRightValveRemain.Foreground, color => (IBrush)new SolidColorBrush(color)).DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.TextColor, v => v.LblRightValveAlarm.Foreground, color => (IBrush)new SolidColorBrush(color)).DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.TextColor, v => v.LblATime.Foreground, color => (IBrush)new SolidColorBrush(color)).DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.TextColor, v => v.LblBTime.Foreground, color => (IBrush)new SolidColorBrush(color)).DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.TextColor, v => v.TxtATime.Foreground, color => (IBrush)new SolidColorBrush(color)).DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.TextColor, v => v.TxtBTime.Foreground, color => (IBrush)new SolidColorBrush(color)).DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.BackColor, v => v.MainBorder.Background, color => (IBrush)new SolidColorBrush(color)).DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.TextFont.FontFamily, v => v.LblLeftValve.FontFamily).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontSize, v => v.LblLeftValve.FontSize).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontWeight, v => v.LblLeftValve.FontWeight).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontFamily, v => v.LblLeftValveQuantitative.FontFamily).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontSize, v => v.LblLeftValveQuantitative.FontSize).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontWeight, v => v.LblLeftValveQuantitative.FontWeight).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontFamily, v => v.LblLeftValveRemain.FontFamily).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontSize, v => v.LblLeftValveRemain.FontSize).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontWeight, v => v.LblLeftValveRemain.FontWeight).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontFamily, v => v.LblLeftValveAlarm.FontFamily).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontSize, v => v.LblLeftValveAlarm.FontSize).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontWeight, v => v.LblLeftValveAlarm.FontWeight).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontFamily, v => v.LblRightValve.FontFamily).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontSize, v => v.LblRightValve.FontSize).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontWeight, v => v.LblRightValve.FontWeight).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontFamily, v => v.LblRightValveQuantitative.FontFamily).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontSize, v => v.LblRightValveQuantitative.FontSize).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontWeight, v => v.LblRightValveQuantitative.FontWeight).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontFamily, v => v.LblRightValveRemain.FontFamily).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontSize, v => v.LblRightValveRemain.FontSize).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontWeight, v => v.LblRightValveRemain.FontWeight).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontFamily, v => v.LblRightValveAlarm.FontFamily).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontSize, v => v.LblRightValveAlarm.FontSize).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontWeight, v => v.LblRightValveAlarm.FontWeight).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontFamily, v => v.LblATime.FontFamily).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontSize, v => v.LblATime.FontSize).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontWeight, v => v.LblATime.FontWeight).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontFamily, v => v.TxtATime.FontFamily).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontSize, v => v.TxtATime.FontSize).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontWeight, v => v.TxtATime.FontWeight).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontFamily, v => v.LblBTime.FontFamily).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontSize, v => v.LblBTime.FontSize).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontWeight, v => v.LblBTime.FontWeight).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontFamily, v => v.TxtBTime.FontFamily).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontSize, v => v.TxtBTime.FontSize).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.TextFont.FontWeight, v => v.TxtBTime.FontWeight).DisposeWith(disposables);

            ApplyTextFont(ViewModel.TextFont);

            #endregion

            #region 鼠标事件

            Observable.FromEventPattern<PointerPressedEventArgs>(
                    addHandler: h => MainBorder.PointerPressed += h,
                    removeHandler: h => MainBorder.PointerPressed -= h)
                .Select(_ =>
                {
                    PixelPoint screenPixel = MainBorder.PointToScreen(new Point(0, 0));
                    return new Point(screenPixel.X, screenPixel.Y);
                })
                .InvokeCommand(ViewModel, vm => vm.PointerPressedCommand)
                .DisposeWith(disposables);

            #endregion
        });
    }

    #endregion

    /// <summary>
    /// 应用文本字体到所有文本控件
    /// </summary>
    /// <param name="font">字体信息</param>
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
    }
}
