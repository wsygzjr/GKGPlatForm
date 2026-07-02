using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using DynamicData;
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

namespace GKG.Map.ChartFuncCtlMapCell.View
{
    /// <summary>
    /// 实时图表主界面 View
    /// 负责图表的渲染、防坍缩控制、鼠标丝滑交互以及与 ViewModel 的数据同步
    /// </summary>
    public partial class ChartView : ReactiveUserControl<ChartViewModel>
    {
        #region 常量配置 (配置提取区)

        // UI 交互相关常量
        private const int TooltipHitTolerancePx = 15;         // 鼠标吸附数据点的判定范围(像素)
        private const int XAxisViewWindowSeconds = 60;        // 图表横坐标默认保持的滚动视野(秒)
        private const double OADateValidThreshold = 40000.0;  // 有效时间阈值(约2009年), 低于此值视为无效数据剔除，防止坐标轴坍缩到1899年

        #endregion

        #region 私有字段 (图表交互图层)

        private Marker _highlightMarker; // 十字光标中心高亮红点
        private Text _tooltipText;       // 智能避让的悬浮提示文本
        private DataLogger _dataLogger;  // ScottPlot 高性能实时数据引擎

        // 游标：记录最后一次成功渲染的数据时间，防止数据乱序或回拨
        private double _lastLoggedTime = double.MinValue;

        #endregion

        #region 构造函数

        public ChartView()
        {
            InitializeComponent();
            InitializeChartStyles();

            // 严格管理 UI 生命周期，防止 Rx 订阅导致的内存泄漏
            this.WhenActivated(disposables =>
            {
                if (ViewModel == null) return;

                SetupBindings(disposables);
                SetupSubscriptions(disposables);
                SetupInteractions(disposables);
                SetupEventHandlers(disposables);
            });
        }

        #endregion

        #region 初始化配置 (UI Initialization)

        /// <summary>
        /// 初始化图表基础样式、DataLogger 与交互组件
        /// </summary>
        private void InitializeChartStyles()
        {
            var plot = this.TodayPlot.Plot;

            // 1. 初始化核心记录器 (DataLogger)
            _dataLogger = plot.Add.DataLogger();
            _dataLogger.Color = Colors.Blue;
            _dataLogger.LineWidth = 0;
            _dataLogger.MarkerSize = 10;
            _dataLogger.ManageAxisLimits = true; // 让记录器接管滚动视口

            // 设定心电图式的平滑滚动模式
            _dataLogger.ViewSlide(TimeSpan.FromSeconds(XAxisViewWindowSeconds).TotalDays);

            // 2. 配置坐标轴刻度与字体
            plot.Axes.Bottom.TickGenerator = new DateTimeAutomatic
            {
                LabelFormatter = (date) => date.ToString("HH:mm:ss")
            };
            plot.Axes.Bottom.Label.Text = "时间";
            plot.Axes.Bottom.Label.FontName = "Microsoft YaHei UI";

            plot.Axes.Left.Label.Text = "重量";
            plot.Axes.Left.Label.FontName = "Microsoft YaHei UI";
            plot.Axes.Title.Label.FontName = "Microsoft YaHei UI";

            // 3. 初始化独立交互图层 (初始置为不可见)
            _highlightMarker = plot.Add.Marker(0, 0);
            _highlightMarker.Shape = MarkerShape.OpenCircle;
            _highlightMarker.Color = Colors.Red;
            _highlightMarker.Size = 15;
            _highlightMarker.LineWidth = 2;
            _highlightMarker.IsVisible = false;

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

        #endregion

        #region 属性与命令绑定 (Bindings)

        private void SetupBindings(CompositeDisposable disposables)
        {
            // 下拉框数据源与双向绑定
            this.OneWayBind(ViewModel, vm => vm.AvailableChartTypes, v => v.ChartTypeComboBox.ItemsSource).DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.SelectedChartType, v => v.ChartTypeComboBox.SelectedItem).DisposeWith(disposables);

            // 加载状态互斥 (防连点机制)
            var isNotLoading = ViewModel.LoadDataCommand.IsExecuting.Select(isExecuting => !isExecuting).ObserveOn(RxApp.MainThreadScheduler);
            isNotLoading.BindTo(this, v => v.ChartTypeComboBox.IsEnabled).DisposeWith(disposables);
            isNotLoading.BindTo(this, v => v.RefreshChartButton.IsEnabled).DisposeWith(disposables);

            // 统计仪表盘数据绑定
            this.OneWayBind(ViewModel, vm => vm.Cpk, v => v.ChartCpkTextBlock.Text).DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.UpperLimit, v => v.ChartUpperLimitValueTextBlock.Text).DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.LowerLimit, v => v.ChartLowerLimitValueTextBlock.Text).DisposeWith(disposables);

            // 核心命令绑定
            this.BindCommand(ViewModel, vm => vm.RefreshChartCommand, v => v.RefreshChartButton).DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm.OpenFileCommand, v => v.OpenFileButton).DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm.CleanCommand, v => v.CleanButton).DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm.HistoryChartCommand, v => v.HistoryChartButton).DisposeWith(disposables);
        }

        #endregion

        #region 核心流订阅 (Subscriptions)

        private void SetupSubscriptions(CompositeDisposable disposables)
        {
            // 1. 图表高频数据源订阅 (加入 200ms 缓冲池降低渲染压力)
            ViewModel.DataStream
                .Buffer(TimeSpan.FromMilliseconds(200))
                .Where(buffer => buffer.Count > 0)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(ProcessDataChanges)
                .DisposeWith(disposables);

            // 2. 动态更新图表标题
            this.WhenAnyValue(x => x.ViewModel.SelectedChartType)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(type =>
                {
                    TodayPlot.Plot.Axes.Title.Label.Text = type ?? string.Empty;
                    TodayPlot.Refresh();
                })
                .DisposeWith(disposables);
        }

        #endregion

        #region 交互接口 (Interactions)

        private void SetupInteractions(CompositeDisposable disposables)
        {
            this.BindInteraction(ViewModel, vm => vm.ShowTipInteraction, async context =>
                await ShowTipDialogAsync(context)).DisposeWith(disposables);

            this.BindInteraction(ViewModel, vm => vm.HistoryChartInteraction, async context =>
                await ShowHistoryChartDialogAsync(context)).DisposeWith(disposables);
        }

        #endregion

        #region 事件处理 (Event Handlers)

        private void SetupEventHandlers(CompositeDisposable disposables)
        {
            // 图表鼠标移动流：使用 Sample 实现 60FPS 丝滑追踪，替代原本的 Throttle (避免卡顿滞后)
            Observable.FromEventPattern<EventHandler<PointerEventArgs>, PointerEventArgs>(
                    h => TodayPlot.PointerMoved += h,
                    h => TodayPlot.PointerMoved -= h)
                .Select(x => x.EventArgs.GetPosition(this.TodayPlot))
                .Sample(TimeSpan.FromMilliseconds(16)) // 约 60Hz 采样率
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(HandlePlotPointerMoved)
                .DisposeWith(disposables);
        }

        #endregion

        #region 图表渲染核心逻辑 (Core Plot Logic)

        /// <summary>
        /// 解析 Rx 数据变化集，维护图表点阵 (极简完美版，自带毒药过滤)
        /// </summary>
        private void ProcessDataChanges(IList<IChangeSet<ChartData>> changesList)
        {
            bool needsRefresh = false;

            foreach (var changeSet in changesList)
            {
                foreach (var change in changeSet)
                {
                    switch (change.Reason)
                    {
                        case ListChangeReason.Clear:
                            // 物理清空，DataLogger 将静静待在原位，绝不坍缩
                            _dataLogger.Clear();
                            _lastLoggedTime = double.MinValue;
                            needsRefresh = true;
                            break;

                        case ListChangeReason.Add:
                            var addedItem = change.Item.Current;
                            double newTime = addedItem.Time.ToOADate();

                            // 防御性过滤：拦截 OADate=0 (1899年) 的无效数据
                            if (newTime > OADateValidThreshold && newTime >= _lastLoggedTime)
                            {
                                _dataLogger.Add(newTime, addedItem.Value);
                                _lastLoggedTime = newTime;
                                needsRefresh = true;
                            }
                            break;

                        case ListChangeReason.AddRange:
                            var sortedRange = change.Range.OrderBy(x => x.Time.ToOADate());
                            foreach (var rangeItem in sortedRange)
                            {
                                double rangeTime = rangeItem.Time.ToOADate();

                                if (rangeTime > OADateValidThreshold && rangeTime >= _lastLoggedTime)
                                {
                                    _dataLogger.Add(rangeTime, rangeItem.Value);
                                    _lastLoggedTime = rangeTime;
                                }
                            }
                            needsRefresh = true;
                            break;
                    }
                }
            }

            if (needsRefresh)
            {
                TodayPlot.Refresh();
            }
        }

        /// <summary>
        /// 处理十字光标与 Tooltip 悬浮逻辑
        /// </summary>
        private void HandlePlotPointerMoved(Point point)
        {
            if (_dataLogger == null || _dataLogger.Data.Coordinates.Count == 0)
            {
                HideTooltip();
                return;
            }

            var plot = this.TodayPlot.Plot;
            var mousePixel = new Pixel((float)point.X, (float)point.Y);
            var mouseLocation = plot.GetCoordinates(mousePixel);

            // 二分查找最优解
            var dataList = _dataLogger.Data.Coordinates;
            int nearestIndex = GetNearestIndex(dataList, mouseLocation.X);
            Coordinates nearestPoint = dataList[nearestIndex];

            // 屏幕像素距离计算
            var dataRect = plot.LastRender.DataRect;
            float xPx = _dataLogger.Axes.XAxis.GetPixel(nearestPoint.X, dataRect);
            float yPx = _dataLogger.Axes.YAxis.GetPixel(nearestPoint.Y, dataRect);
            double distance = Math.Sqrt(Math.Pow(xPx - mousePixel.X, 2) + Math.Pow(yPx - mousePixel.Y, 2));

            if (distance < TooltipHitTolerancePx)
            {
                ShowTooltip(nearestPoint, plot);
            }
            else
            {
                HideTooltip();
            }
        }

        /// <summary>
        /// 智能避让式 Tooltip 定位渲染
        /// </summary>
        private void ShowTooltip(Coordinates point, Plot plot)
        {
            _highlightMarker.Location = point;
            _highlightMarker.IsVisible = true;

            DateTime date = DateTime.FromOADate(point.X);
            _tooltipText.LabelText = $"{date:yyyy-MM-dd HH:mm:ss}, {point.Y:F3}"; // 格式化为3位小数更清爽
            _tooltipText.Location = point;

            var limits = plot.Axes.GetLimits();
            double xCenter = (limits.Left + limits.Right) / 2;
            double yCenter = (limits.Top + limits.Bottom) / 2;

            bool isRightSide = point.X > xCenter;
            bool isTopSide = point.Y > yCenter;

            // 象限自适应锚点
            if (isRightSide)
            {
                _tooltipText.LabelStyle.Alignment = isTopSide ? Alignment.UpperRight : Alignment.LowerRight;
            }
            else
            {
                _tooltipText.LabelStyle.Alignment = isTopSide ? Alignment.UpperLeft : Alignment.LowerLeft;
            }

            _tooltipText.IsVisible = true;
            TodayPlot.Refresh();
        }

        private void HideTooltip()
        {
            if (_highlightMarker.IsVisible || _tooltipText.IsVisible)
            {
                _highlightMarker.IsVisible = false;
                _tooltipText.IsVisible = false;
                TodayPlot.Refresh();
            }
        }

        #endregion

        #region 辅助算法 (Algorithms)

        /// <summary>
        /// 在有序列表中二分查找最近 X 坐标的索引 (O(logN) 性能极佳)
        /// </summary>
        private int GetNearestIndex(IReadOnlyList<Coordinates> data, double targetX)
        {
            if (data.Count == 0) return 0;

            int left = 0;
            int right = data.Count - 1;

            if (targetX <= data[0].X) return 0;
            if (targetX >= data[right].X) return right;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;

                if (data[mid].X == targetX) return mid;

                if (data[mid].X < targetX) left = mid + 1;
                else right = mid - 1;
            }

            double diffLeft = Math.Abs(data[left].X - targetX);
            double diffRight = Math.Abs(data[right].X - targetX);

            return diffLeft < diffRight ? left : right;
        }

        #endregion

        #region 弹窗管理 (Dialog Helpers)

        private async Task ShowTipDialogAsync(IInteractionContext<TipDialogViewModel, DialogResultType> context)
        {
            var tipDialogView = new TipDialogView { DataContext = context.Input };

            // 安全的模式匹配获取根窗口
            if (this.GetVisualRoot() is Window parentWindow)
            {
                var result = await tipDialogView.ShowDialog<DialogResultType>(parentWindow);
                context.SetOutput(result);
            }
            else
            {
                context.SetOutput(DialogResultType.Cancel);
            }
        }

        private async Task ShowHistoryChartDialogAsync(IInteractionContext<HistoryChartWndViewModel, bool> context)
        {
            var historyView = new HistoryChartWndView { DataContext = context.Input };

            if (TopLevel.GetTopLevel(this) is Window parentWindow)
            {
                var result = await historyView.ShowDialog<bool>(parentWindow);
                context.SetOutput(result);
            }
            else
            {
                context.SetOutput(default);
            }
        }

        #endregion
    }
}