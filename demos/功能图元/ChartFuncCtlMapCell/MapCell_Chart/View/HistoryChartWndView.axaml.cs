using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using GKG.Map.ChartFuncCtlMapCell.Models;
using GKG.Map.ChartFuncCtlMapCell.ViewModel;
using ReactiveUI;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.TickGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace GKG.Map.ChartFuncCtlMapCell.View;

public partial class HistoryChartWndView : ReactiveWindow<HistoryChartWndViewModel>
{
    #region 私有字段 (图表图层)

    private Scatter _myScatter;       // 数据散点主图层
    private Marker _highlightMarker;  // 鼠标悬停时的高亮红圈
    private Text _tooltipText;        // 鼠标悬停时的信息提示框
    private bool _isUpdatingFromChart = false; // 互斥锁标志位，防止双向联动死循环

    #endregion

    public HistoryChartWndView()
    {
        InitializeComponent();
        InitializeChart();

        this.WhenActivated(disposables =>
        {
            if (ViewModel == null) return;

            #region 1. 绑定基础查询条件 (时间与下拉框)

            // 日期选择
            this.Bind(ViewModel, vm => vm.StartDate, v => v.StartDatePicker.SelectedDate).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.EndDate, v => v.EndDatePicker.SelectedDate).DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.StartDateText, v => v.StartDateTextBox.Text).DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.EndDateText, v => v.EndDateTextBox.Text).DisposeWith(disposables);

            // 时分选择
            this.Bind(ViewModel, vm => vm.StartHours, v => v.StartHourNumeric.Value).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.StartMinutes, v => v.StartMinuteNumeric.Value).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.EndHours, v => v.EndHourNumeric.Value).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.EndMinutes, v => v.EndMinuteNumeric.Value).DisposeWith(disposables);

            // 图表类型选择
            this.OneWayBind(ViewModel, vm => vm.TypeList, v => v.ChartChooseBox.ItemsSource).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.SelectionType, v => v.ChartChooseBox.SelectedItem).DisposeWith(disposables);

            #endregion

            #region 2. 绑定统计数据与命令

            // 统计数据
            this.OneWayBind(ViewModel, vm => vm.ChartCPK, v => v.ChartCpkTextBlock.Text).DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.ChartCPU, v => v.ChartCPUTextBlock.Text).DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.ChartMax, v => v.ChartMaxTextBlock.Text).DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.ChartDataMean, v => v.ChartDataMeanTextBlock.Text).DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.ChartCPL, v => v.ChartCPLTextBlock.Text).DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.ChartMin, v => v.ChartMinTextBlock.Text).DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.ChartUpperLimit, v => v.ChartUpLimitValueTextBlock.Text).DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.ChartLowerLimit, v => v.ChartDownLimitValueTextBlock.Text).DisposeWith(disposables);

            // 操作按钮
            this.BindCommand(ViewModel, vm => vm.RefreshCommand, v => v.RefreshChartButton).DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm.CleanCommand, v => v.CleanChartDataButton).DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm.ExportCommand, v => v.ExportChartFileButton).DisposeWith(disposables);

            #endregion

            #region 3. 注册交互请求 (Interactions)

            // 注册：通用提示弹窗
            this.BindInteraction(ViewModel, vm => vm.ShowTipInteraction, async context =>
            {
                var tipDialogView = new TipDialogView { DataContext = context.Input };

                // 安全获取父窗口
                if (this.GetVisualRoot() is Window parentWindow)
                {
                    var result = await tipDialogView.ShowDialog<DialogResultType>(parentWindow);
                    context.SetOutput(result);
                }
                else
                {
                    context.SetOutput(DialogResultType.Cancel);
                }
            }).DisposeWith(disposables);

            // 注册：导出文件保存弹窗
            this.BindInteraction(ViewModel, vm => vm.SaveFileInteraction, async context =>
            {
                var topLevel = TopLevel.GetTopLevel(this);
                if (topLevel == null)
                {
                    context.SetOutput(string.Empty);
                    return;
                }

                // 获取历史常用保存目录
                var customFolder = await topLevel.StorageProvider.TryGetFolderFromPathAsync("D:\\DispenserData\\WeightDataExport");

                var file = await topLevel.StorageProvider.SaveFilePickerAsync(new Avalonia.Platform.Storage.FilePickerSaveOptions
                {
                    Title = "导出历史数据",
                    SuggestedFileName = context.Input,
                    SuggestedStartLocation = customFolder,
                    DefaultExtension = "csv",
                    FileTypeChoices = new[]
                    {
                        new Avalonia.Platform.Storage.FilePickerFileType("CSV 数据文件") { Patterns = new[] { "*.csv" } }
                    }
                });

                context.SetOutput(file?.Path.LocalPath ?? string.Empty);
            }).DisposeWith(disposables);

            #endregion

            #region 4. 数据订阅与 UI 更新流

            // 监听：图表数据变化，触发 UI 重绘
            this.WhenAnyValue(x => x.ViewModel.HistoryDatas)
                .WhereNotNull() // 完美洗白 null，确保只有有效列表进入下游
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(data =>
                {
                    RenderHistoryChart(data);
                    UpdateListBox(data);
                })
                .DisposeWith(disposables);

            // 监听：鼠标移动实现顺滑的十字光标 (60FPS 采样)
            Observable.FromEventPattern<EventHandler<PointerEventArgs>, PointerEventArgs>(
                        h => this.HistoryPlot.PointerMoved += h,
                        h => this.HistoryPlot.PointerMoved -= h)
                      .Select(x => x.EventArgs)
                      .Sample(TimeSpan.FromMilliseconds(16))
                      .ObserveOn(RxApp.MainThreadScheduler)
                      .Subscribe(e => HandleHistoryPlotPointerMoved(e))
                      .DisposeWith(disposables);

            // 监听：ListBox 选中项改变时，联动图表高亮与自动追踪
            Observable.FromEventPattern<SelectionChangedEventArgs>(
                        h => this.MyListBox.SelectionChanged += h,
                        h => this.MyListBox.SelectionChanged -= h)
                      .ObserveOn(RxApp.MainThreadScheduler)
                      .Subscribe(e =>
                      {
                          if (this.MyListBox.SelectedIndex == -1 || _myScatter == null || ViewModel?.HistoryDatas == null)
                              return;

                          // 互斥锁检测：如果是图表滑动引起的改变，忽略
                          if (_isUpdatingFromChart)
                              return;

                          int targetIndex = this.MyListBox.SelectedIndex;

                          // 越界保护
                          if (targetIndex >= 0 && targetIndex < ViewModel.HistoryDatas.Count)
                          {
                              var plot = this.HistoryPlot.Plot;
                              var targetData = ViewModel.HistoryDatas[targetIndex];

                              double xPos = targetData.Time.ToOADate();
                              double yPos = targetData.Value;
                              var targetCoordinates = new Coordinates(xPos, yPos);

                              // 1. 移动高亮圈
                              _highlightMarker.Location = targetCoordinates;
                              _highlightMarker.IsVisible = true;

                              // 2. 移动提示框并赋值
                              _tooltipText.LabelText = $"{targetData.Time:yyyy-MM-dd HH:mm:ss}, {yPos:F2}";
                              _tooltipText.Location = targetCoordinates;

                              // ==========================================
                              // 第一步：象限智能避让 
                              // ==========================================
                              var limits = plot.Axes.GetLimits();
                              double xCenter = (limits.Left + limits.Right) / 2;
                              double yCenter = (limits.Top + limits.Bottom) / 2;

                              bool isRightSide = xPos > xCenter;
                              bool isTopSide = yPos > yCenter;

                              if (isRightSide && isTopSide)
                              {
                                  _tooltipText.LabelStyle.Alignment = ScottPlot.Alignment.UpperRight;
                              }
                              else if (isRightSide && !isTopSide)
                              {
                                  _tooltipText.LabelStyle.Alignment = ScottPlot.Alignment.LowerRight;
                              }
                              else if (!isRightSide && isTopSide)
                              {
                                  _tooltipText.LabelStyle.Alignment = ScottPlot.Alignment.UpperLeft;
                              }
                              else
                              {
                                  _tooltipText.LabelStyle.Alignment = ScottPlot.Alignment.LowerLeft;
                              }

                              _tooltipText.IsVisible = true;

                              // ==========================================
                              // 第二步：摄像机智能追踪（仅出界时平移） 
                              // ==========================================
                              // 判断点是否【真正】跑出了当前的屏幕可视范围
                              bool isOutX = xPos <= limits.Left || xPos >= limits.Right;
                              bool isOutY = yPos <= limits.Bottom || yPos >= limits.Top;

                              if (isOutX || isOutY)
                              {
                                  double xRange = limits.Right - limits.Left;
                                  double yRange = limits.Top - limits.Bottom;

                                  // 给予 5% 的呼吸空间，避免拉回来后点紧贴着边框
                                  double xPadding = xRange * 0.05;
                                  double yPadding = yRange * 0.05;

                                  double panX = 0;
                                  double panY = 0;

                                  if (xPos <= limits.Left)
                                      panX = xPos - limits.Left - xPadding;
                                  else if (xPos >= limits.Right)
                                      panX = xPos - limits.Right + xPadding;

                                  if (yPos <= limits.Bottom)
                                      panY = yPos - limits.Bottom - yPadding;
                                  else if (yPos >= limits.Top)
                                      panY = yPos - limits.Top + yPadding;

                                  plot.Axes.SetLimits(
                                      left: limits.Left + panX,
                                      right: limits.Right + panX,
                                      bottom: limits.Bottom + panY,
                                      top: limits.Top + panY
                                  );
                              }

                              // 刷新图表
                              this.HistoryPlot.Refresh();
                          }
                      })
                      .DisposeWith(disposables);

            #endregion
        });
    }

    private void OnBackgroundPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // 获取当前窗口的焦点管理器
        var focusManager = TopLevel.GetTopLevel(this)?.FocusManager;

        // 强制清空当前所有焦点
        focusManager?.ClearFocus();
    }

    private async void StartDatePicker_PointerExited(object? sender, PointerEventArgs e)
    {
        // 给用户 250 毫秒的容错时间
        await Task.Delay(250);

        // 250毫秒后，再次确认鼠标是不是真的不在日历上了
        if (StartDatePicker != null && !StartDatePicker.IsPointerOver)
        {
            StartCalendarBtn.Flyout?.Hide();
        }
    }

    private async void EndDatePicker_PointerExited(object? sender, PointerEventArgs e)
    {
        // 给用户 250 毫秒的容错时间
        await Task.Delay(250);

        // 250毫秒后，再次确认鼠标是不是真的不在日历上了
        if (StartDatePicker != null && !StartDatePicker.IsPointerOver)
        {
            EndCalendarBtn.Flyout?.Hide();
        }
    }

    #region 核心制图与渲染逻辑

    /// <summary>
    /// 初始化图表基础样式与交互图层
    /// </summary>
    private void InitializeChart()
    {
        var plot = this.HistoryPlot.Plot;

        // X轴：时间格式化
        plot.Axes.Bottom.TickGenerator = new DateTimeAutomatic();
        if (plot.Axes.Bottom.TickGenerator is DateTimeAutomatic dtGen)
        {
            dtGen.LabelFormatter = (date) => date.ToString("HH:mm:ss");
        }
        plot.Axes.Bottom.Label.Text = "时间";
        plot.Axes.Bottom.Label.FontName = "Microsoft YaHei UI";

        // Y轴
        plot.Axes.Left.Label.Text = "重量";
        plot.Axes.Left.Label.FontName = "Microsoft YaHei UI";

        // 初始化高亮圆圈 (隐藏备用)
        _highlightMarker = plot.Add.Marker(0, 0);
        _highlightMarker.Shape = MarkerShape.OpenCircle;
        _highlightMarker.Color = Colors.Red;
        _highlightMarker.Size = 15;
        _highlightMarker.LineWidth = 2;
        _highlightMarker.IsVisible = false;

        // 初始化提示文本框 (隐藏备用)
        _tooltipText = plot.Add.Text(string.Empty, 0, 0);
        _tooltipText.LabelStyle.FontName = "Microsoft YaHei UI";
        _tooltipText.LabelStyle.FontSize = 12;
        _tooltipText.LabelStyle.ForeColor = Colors.Red;
        _tooltipText.LabelStyle.BackgroundColor = Colors.White.WithAlpha(0.9);
        _tooltipText.LabelStyle.BorderColor = Colors.Red;
        _tooltipText.LabelStyle.BorderWidth = 1;
        _tooltipText.LabelStyle.Padding = 5;
        _tooltipText.IsVisible = false;
    }

    /// <summary>
    /// 渲染历史散点图与防坍缩处理
    /// </summary>
    private void RenderHistoryChart(List<ChartData> dataList)
    {
        var plot = this.HistoryPlot.Plot;

        // 无数据时，彻底移除图层并置空
        if (dataList == null || dataList.Count == 0)
        {
            if (_myScatter != null)
            {
                plot.Remove(_myScatter); // 从画布中物理剥离
                _myScatter = null;       // 置空，彻底切断鼠标寻址的数据源
            }

            _highlightMarker.IsVisible = false;
            _tooltipText.IsVisible = false;
            this.HistoryPlot.Refresh();
            return;
        }

        // 提取坐标系数据
        double[] xs = dataList.Select(d => d.Time.ToOADate()).ToArray();
        double[] ys = dataList.Select(d => d.Value).ToArray();

        // 覆盖旧图层
        if (_myScatter != null)
        {
            plot.Remove(_myScatter);
        }

        _myScatter = plot.Add.Scatter(xs, ys);
        _myScatter.LineWidth = 0;
        _myScatter.MarkerSize = 10;
        _myScatter.Color = Colors.Blue;

        // 强行将交互图层置于最上层
        plot.Remove(_highlightMarker);
        plot.Add.Plottable(_highlightMarker);
        plot.Remove(_tooltipText);
        plot.Add.Plottable(_tooltipText);

        // 设置动态标题
        plot.Axes.Title.Label.Text = $"{this.ViewModel?.SelectionType ?? "查询结果"}";
        plot.Axes.Title.Label.FontName = "Microsoft YaHei UI";

        // 防坍缩自适应缩放 (AutoScale)
        if (xs.Length > 0 && ys.Length > 0)
        {
            double xMin = xs.Min();
            double xMax = xs.Max();
            double yMin = ys.Min();
            double yMax = ys.Max();

            // X 轴时间防坍缩处理
            if (Math.Abs(xMax - xMin) < 0.000001)
            {
                double oneHour = 1.0 / 24.0; // OADate 单位为天
                plot.Axes.SetLimitsX(xMin - oneHour, xMax + oneHour);
            }
            else
            {
                double xPadding = (xMax - xMin) * 0.05;
                plot.Axes.SetLimitsX(xMin - xPadding, xMax + xPadding);
            }

            // Y 轴重量防坍缩处理
            if (Math.Abs(yMax - yMin) < 0.000001)
            {
                double yPadding = (yMax == 0) ? 1.0 : Math.Abs(yMax) * 0.1;
                plot.Axes.SetLimitsY(yMin - yPadding, yMax + yPadding);
            }
            else
            {
                double yPadding = (yMax - yMin) * 0.1;
                plot.Axes.SetLimitsY(yMin - yPadding, yMax + yPadding);
            }
        }

        this.HistoryPlot.Refresh();
    }

    /// <summary>
    /// 更新右侧历史数据文本列表
    /// </summary>
    private void UpdateListBox(List<ChartData> dataList)
    {
        List<string> items = new List<string>(dataList.Count);

        foreach (ChartData data in dataList)
        {
            items.Add($"{data.Time:yyyy-MM-dd HH:mm:ss} : {data.Value:F2}");
        }

        this.MyListBox.ItemsSource = items;
    }

    /// <summary>
    /// 处理鼠标悬停十字光标及象限智能避让
    /// </summary>
    private void HandleHistoryPlotPointerMoved(PointerEventArgs e)
    {
        // 如果主图层是空的，或者数据已被清空，绝对不允许继续往下执行！
        if (_myScatter == null || ViewModel?.HistoryDatas == null || ViewModel.HistoryDatas.Count == 0)
            return;

        var plot = this.HistoryPlot.Plot;
        var point = e.GetPosition(this.HistoryPlot);
        var mousePixel = new Pixel((float)point.X, (float)point.Y);
        var mouseLocation = plot.GetCoordinates(mousePixel);

        // 寻找距离鼠标最近的渲染点
        var dataPoint = _myScatter.Data.GetNearest(mouseLocation, plot.LastRender);

        if (dataPoint.IsReal)
        {
            // 更新高亮圈
            _highlightMarker.Location = dataPoint.Coordinates;
            _highlightMarker.IsVisible = true;

            // 格式化提示文本
            DateTime date = DateTime.FromOADate(dataPoint.Coordinates.X);
            double value = dataPoint.Coordinates.Y;
            _tooltipText.LabelText = $"{date:yyyy-MM-dd HH:mm:ss}, {value:F2}";
            _tooltipText.Location = dataPoint.Coordinates;

            // 获取当前视图边界计算屏幕象限
            var limits = plot.Axes.GetLimits();
            double xCenter = (limits.Left + limits.Right) / 2;
            double yCenter = (limits.Top + limits.Bottom) / 2;

            bool isRightSide = dataPoint.Coordinates.X > xCenter;
            bool isTopSide = dataPoint.Coordinates.Y > yCenter;

            // 智能避让锚点设置：点在哪边，锚点就设在哪边，使得文本往屏幕中央方向显示
            if (isRightSide && isTopSide)
            {
                _tooltipText.LabelStyle.Alignment = ScottPlot.Alignment.UpperRight;
            }
            else if (isRightSide && !isTopSide)
            {
                _tooltipText.LabelStyle.Alignment = ScottPlot.Alignment.LowerRight;
            }
            else if (!isRightSide && isTopSide)
            {
                _tooltipText.LabelStyle.Alignment = ScottPlot.Alignment.UpperLeft;
            }
            else
            {
                _tooltipText.LabelStyle.Alignment = ScottPlot.Alignment.LowerLeft;
            }

            // 判断当前 ListBox 选中的是不是这个点，防止重复触发滚动导致画面抖动
            if (this.MyListBox.SelectedIndex != dataPoint.Index)
            {
                _isUpdatingFromChart = true;

                // 设置 ListBox 的选中项
                this.MyListBox.SelectedIndex = dataPoint.Index;

                // 强制 ListBox 滚动到刚才选中的那一项，确保它出现在用户视野中
                if (this.MyListBox.SelectedItem != null)
                {
                    this.MyListBox.ScrollIntoView(this.MyListBox.SelectedItem);
                }

                _isUpdatingFromChart = false;
            }

            _tooltipText.IsVisible = true;
            this.HistoryPlot.Refresh();
        }
        else
        {
            // 无有效点时隐藏交互层
            if (_highlightMarker.IsVisible)
            {
                _highlightMarker.IsVisible = false;
                _tooltipText.IsVisible = false;
                this.HistoryPlot.Refresh();

                // 鼠标移开图表时，取消 ListBox 的高亮选中
                this.MyListBox.SelectedIndex = -1;
            }
        }
    }

    #endregion
}