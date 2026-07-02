using GKG.Map.ChartFuncCtlMapCell.Models;
using Griffins;
using Griffins.Map.Cmd;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace GKG.Map.ChartFuncCtlMapCell.ViewModel
{
    /// <summary>
    /// 历史图表视图模型 - 负责处理历史数据的查询、显示与导出
    /// </summary>
    public class HistoryChartWndViewModel : ReactiveObject, IDisposable, IActivatableViewModel
    {
        #region 私有字段与依赖

        // 集中存储 Rx 订阅，统一释放，防止内存泄漏
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        // 底层服务执行器与地图编号
        private readonly MapCmdExector _cmdExector;
        private readonly string _mapNo;

        // 防止时间自动纠正时触发死循环的锁
        private bool _isAdjustingTime = false;
        // 记忆上一次合法的时间状态，用来判断是“谁撞了谁”
        private DateTime? _lastValidStartDate = null;
        private DateTime? _lastValidEndDate = null;
        private int _lastValidStartMinutes = -1;
        private int _lastValidEndMinutes = -1;

        public ViewModelActivator Activator { get; } = new();

        #endregion

        #region 交互请求 (Interactions)

        /// <summary>
        /// 消息弹窗交互请求 (View 层负责弹窗并返回用户选择)
        /// </summary>
        public Interaction<TipDialogViewModel, DialogResultType> ShowTipInteraction { get; } = new();

        /// <summary>
        /// 另存为对话框交互请求 (View 层负责弹出系统文件框并返回绝对路径)
        /// </summary>
        public Interaction<string, string> SaveFileInteraction { get; } = new();

        #endregion

        #region 命令 (Commands)

        /// <summary>
        /// 刷新图表命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> RefreshCommand { get; }

        /// <summary>
        /// 清除图表历史数据命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> CleanCommand { get; }

        /// <summary>
        /// 导出历史数据为 CSV 文件命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> ExportCommand { get; }

        /// <summary>
        /// 异步加载后台底层图表数据命令
        /// </summary>
        public ReactiveCommand<Tuple<string, DateTime, DateTime>, ChartDataDto> LoadDataCommand { get; }

        #endregion

        #region 绑定的属性 (UI Bindings)

        // ================= 数据源 =================
        /// <summary>
        /// 核心图表数据集合
        /// </summary>
        [Reactive] public List<ChartData> HistoryDatas { get; set; }

        // ================= 搜索条件：日期与时间 =================
        [Reactive] public DateTime? StartDate { get; set; }
        [Reactive] public DateTime? EndDate { get; set; }
        [Reactive] public string StartDateText { get; set; } = string.Empty;
        [Reactive] public string EndDateText { get; set; } = string.Empty;

        [Reactive] public int StartHours { get; set; }
        [Reactive] public int StartMinutes { get; set; }
        [Reactive] public int EndHours { get; set; }
        [Reactive] public int EndMinutes { get; set; }

        // ================= 搜索条件：阀门类型 =================
        public List<string> TypeList { get; } = new List<string>
        {
            "右阀单点称重",
            "左阀单点称重",
            "右阀定量称重",
            "左阀定量称重"
        };
        [Reactive] public string SelectionType { get; set; } = string.Empty;

        // ================= 统计面板数据 =================
        [Reactive] public string ChartCPK { get; set; } = "0.000";
        [Reactive] public string ChartCPU { get; set; } = "0.000";
        [Reactive] public string ChartMax { get; set; } = "0.000";
        [Reactive] public string ChartDataMean { get; set; } = "0.000";
        [Reactive] public string ChartCPL { get; set; } = "0.000";
        [Reactive] public string ChartMin { get; set; } = "0.000";
        [Reactive] public string ChartUpperLimit { get; set; } = "0.000";
        [Reactive] public string ChartLowerLimit { get; set; } = "0.000";

        #endregion

        #region 构造函数与初始化

        public HistoryChartWndViewModel(MapCmdExector cmdExector, string mapNo)
        {
            _cmdExector = cmdExector;
            _mapNo = mapNo;

            // 初始化默认属性
            SelectionType = TypeList[0];
            StartDate = DateTime.Today;
            EndDate = DateTime.Today;

            // 初始化命令
            RefreshCommand = ReactiveCommand.Create(ExeRefresh);
            CleanCommand = ReactiveCommand.CreateFromTask(ExeCleanAsync);
            ExportCommand = ReactiveCommand.CreateFromTask(ExeExportAsync);
            LoadDataCommand = ReactiveCommand.CreateFromTask<Tuple<string, DateTime, DateTime>, ChartDataDto>(ExeLoadDataAsync);

            // 监听：底层加载数据的返回结果
            LoadDataCommand
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(dto =>
                {
                    if (dto != null)
                    {
                        HistoryDatas = dto.Data ?? new List<ChartData>();
                        ChartCPK = dto.CPK.ToString("F3");
                        ChartCPU = dto.CPU.ToString("F3");
                        ChartCPL = dto.CPL.ToString("F3");
                        ChartMax = dto.Max.ToString("F3");
                        ChartMin = dto.Min.ToString("F3");
                        ChartDataMean = dto.Mean.ToString("F3");
                        ChartUpperLimit = dto.UpperLimit.ToString("F3");
                        ChartLowerLimit = dto.LowerLimit.ToString("F3");
                    }
                    else
                    {
                        HistoryDatas = new List<ChartData>();
                    }
                })
                .DisposeWith(_disposables);

            // 监听：日期与时间的 6 维联动，智能防错与防穿越
            this.WhenAnyValue(
                    x => x.StartDate, x => x.EndDate,
                    x => x.StartHours, x => x.StartMinutes,
                    x => x.EndHours, x => x.EndMinutes)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => EnsureTimeLogicValid())
                .DisposeWith(_disposables);

            // 监听：选中类型变化，自动触发刷新
            this.WhenAnyValue(x => x.SelectionType)
                .Skip(1) // 跳过初始化时的第一次触发
                .Subscribe(_ => ExeRefresh())
                .DisposeWith(_disposables);

            // 注入测试数据
            LoadMockData(); 
        }

        #endregion

        #region 私有方法 (业务实现)

        /// <summary>
        /// 确保时间跨度逻辑绝对合法 (智能双向推平)
        /// </summary>
        private void EnsureTimeLogicValid()
        {
            // 如果正在自动纠正中，或者日期还没选好，直接返回打破死循环
            if (_isAdjustingTime) return;
            if (!StartDate.HasValue || !EndDate.HasValue) return;

            _isAdjustingTime = true; // 上锁

            try
            {
                // 1. 初始化记忆状态 (首次打开界面时)
                if (_lastValidStartMinutes == -1)
                {
                    _lastValidStartDate = StartDate;
                    _lastValidEndDate = EndDate;
                    _lastValidStartMinutes = StartHours * 60 + StartMinutes;
                    _lastValidEndMinutes = EndHours * 60 + EndMinutes;

                    this.StartDateText = StartDate.Value.ToString("yyyy/MM/dd");
                    this.EndDateText = EndDate.Value.ToString("yyyy/MM/dd");
                    return;
                }

                // 计算到底是哪个框被用户动了
                bool isStartDateChanged = StartDate != _lastValidStartDate;
                bool isEndDateChanged = EndDate != _lastValidEndDate;

                int currentStartTotal = StartHours * 60 + StartMinutes;
                int currentEndTotal = EndHours * 60 + EndMinutes;

                bool isStartTimeChanged = currentStartTotal != _lastValidStartMinutes;
                bool isEndTimeChanged = currentEndTotal != _lastValidEndMinutes;

                // 2. 【日期级防撞】
                if (StartDate.Value.Date > EndDate.Value.Date)
                {
                    if (isStartDateChanged)
                    {
                        // 用户把“开始日期”往后推，撞到了“结束日期” -> 把结束日期推平
                        var targetDate = StartDate.Value;
                        RxApp.MainThreadScheduler.Schedule(() =>
                        {
                            this.EndDate = null;
                            RxApp.MainThreadScheduler.Schedule(() => this.EndDate = targetDate);
                        });
                        _lastValidEndDate = targetDate;
                    }
                    else if (isEndDateChanged)
                    {
                        // 用户把“结束日期”往前推，撞到了“开始日期” -> 把开始日期推平
                        var targetDate = EndDate.Value;
                        RxApp.MainThreadScheduler.Schedule(() =>
                        {
                            this.StartDate = null;
                            RxApp.MainThreadScheduler.Schedule(() => this.StartDate = targetDate);
                        });
                        _lastValidStartDate = targetDate;
                    }
                }
                // 3. 【时间级防撞】(在同一天内)
                else if (StartDate.Value.Date == EndDate.Value.Date)
                {
                    if (currentStartTotal > currentEndTotal)
                    {
                        if (isStartTimeChanged)
                        {
                            // 用户把“开始时间”加大了，推平结束时间
                            EndHours = StartHours;
                            EndMinutes = StartMinutes;
                            currentEndTotal = currentStartTotal;
                        }
                        else if (isEndTimeChanged)
                        {
                            // 用户把“结束时间”减小了，推平开始时间！
                            StartHours = EndHours;
                            StartMinutes = EndMinutes;
                            currentStartTotal = currentEndTotal;
                        }
                    }
                }

                // 4. 同步更新显示在文本框上的日期文本
                this.StartDateText = StartDate.Value.ToString("yyyy/MM/dd");
                this.EndDateText = EndDate.Value.ToString("yyyy/MM/dd");

                // 5. 将当前的安全状态存档，作为下一次判断的依据
                _lastValidStartDate = StartDate;
                _lastValidEndDate = EndDate;
                _lastValidStartMinutes = currentStartTotal;
                _lastValidEndMinutes = currentEndTotal;
            }
            finally
            {
                _isAdjustingTime = false; // 纠正完毕，解锁
            }
        }

        /// <summary>
        /// 执行：组装查询条件并触发刷新
        /// </summary>
        private void ExeRefresh()
        {
            if (!StartDate.HasValue || !EndDate.HasValue) return;

            // 将日期与数值框里的时、分精准拼装成 DateTime
            DateTime start = StartDate.Value.Date.AddHours(StartHours).AddMinutes(StartMinutes);
            DateTime end = EndDate.Value.Date.AddHours(EndHours).AddMinutes(EndMinutes);

            // 构建参数并触发加载命令
            var queryParams = new Tuple<string, DateTime, DateTime>(SelectionType, start, end);
            LoadDataCommand.Execute(queryParams).Subscribe();
        }

        /// <summary>
        /// 执行：从底层后台获取历史图表数据
        /// </summary>
        private async Task<ChartDataDto> ExeLoadDataAsync(Tuple<string, DateTime, DateTime> t)
        {
            try
            {
                return await Task.Run(() =>
                {
                    GFBaseTypeParamValueList result = _cmdExector.ExecUIDataObjCommand(_mapNo, "LoadChartData", new GFBaseTypeParamValueList
                    {
                        new GFBaseTypeParamValue("Type", new GriffinsBaseValue(t.Item1)),
                        new GFBaseTypeParamValue("StartTime", new GriffinsBaseValue(t.Item2)),
                        new GFBaseTypeParamValue("EndTime", new GriffinsBaseValue(t.Item3))
                    });

                    var resultValue = result?.FirstOrDefault(p => p.ID == "ChartDataDto")?.Value;
                    if (resultValue != null)
                    {
                        var chartDataDto = new ChartDataDto();
                        ((IGriffinsBaseValue)chartDataDto).PopulateFromBaseValue(resultValue);
                        return chartDataDto;
                    }
                    return null;
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadChartData 执行失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 执行：清除当前时间段的历史数据
        /// </summary>
        private async Task ExeCleanAsync()
        {
            // 1. 弹出防误删警告框
            var tipDialogVm = new TipDialogViewModel();
            tipDialogVm.Setup(
                message: "是否确定清除该时间段的历史数据？",
                title: "严重警告",
                iconType: DialogIconType.Warning,
                comboType: TipButtonCombo.YesNo);

            DialogResultType result = await ShowTipInteraction.Handle(tipDialogVm);
            if (result != DialogResultType.Yes) return;

            // 2. 传递清除指令给底层服务
            try
            {
                await Task.Run(() =>
                {
                    DateTime start = StartDate.Value.Date.AddHours(StartHours).AddMinutes(StartMinutes);
                    DateTime end = EndDate.Value.Date.AddHours(EndHours).AddMinutes(EndMinutes);

                    _cmdExector.ExecUIDataObjCommand(_mapNo, "ClearHistoryChartData", new GFBaseTypeParamValueList
                    {
                        new GFBaseTypeParamValue("Type", new GriffinsBaseValue(SelectionType)),
                        new GFBaseTypeParamValue("StartTime", new GriffinsBaseValue(start)),
                        new GFBaseTypeParamValue("EndTime", new GriffinsBaseValue(end))
                    });
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ClearHistoryChartData 执行失败: {ex.Message}");
            }

            // 3. 清空前端界面显示
            HistoryDatas = new List<ChartData>();
            ChartCPK = "0.000"; ChartCPU = "0.000"; ChartMax = "0.000";
            ChartDataMean = "0.000"; ChartCPL = "0.000"; ChartMin = "0.000";
        }

        /// <summary>
        /// 执行：将内存中的历史数据导出为 CSV 报表
        /// </summary>
        private async Task ExeExportAsync()
        {
            // 拦截：无数据可导出的情况
            if (HistoryDatas == null || HistoryDatas.Count == 0)
            {
                var tipDialogVM = new TipDialogViewModel();
                tipDialogVM.Setup(
                    message: "当前没有可导出的数据，请先查询历史数据！",
                    title: "提示",
                    iconType: DialogIconType.Tip,
                    comboType: TipButtonCombo.Ok);

                await ShowTipInteraction.Handle(tipDialogVM);
                return;
            }

            // 获取用户指定的保存路径
            string defaultFileName = $"{SelectionType}_{StartDate.Value:yyyy_MM_dd}.csv";
            string filePath = await SaveFileInteraction.Handle(defaultFileName);

            if (string.IsNullOrEmpty(filePath)) return;

            // 后台线程异步写入文件，防止 UI 假死
            bool isSuccess = false;
            await Task.Run(() =>
            {
                try
                {
                    // 使用 UTF8 with BOM 编码，防止 Excel 预览乱码
                    using (var writer = new System.IO.StreamWriter(filePath, false, new System.Text.UTF8Encoding(true)))
                    {
                        writer.WriteLine("时间(Time),数值(Value)");
                        foreach (var data in HistoryDatas)
                        {
                            writer.WriteLine($"{data.Time:yyyy-MM-dd HH:mm:ss},{data.Value}");
                        }
                    }
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"文件导出失败: {ex.Message}");
                }
            });

            // 导出成功反馈
            if (isSuccess)
            {
                var successDialogVM = new TipDialogViewModel();
                successDialogVM.Setup(
                    message: "文件导出成功！\n\n文件路径：" + filePath,
                    title: "导出完成",
                    iconType: DialogIconType.Tip,
                    comboType: TipButtonCombo.Ok);

                await ShowTipInteraction.Handle(successDialogVM);
            }
        }

        #endregion

        #region 测试与模拟数据

        /// <summary>
        /// 生成高质量的模拟测试数据 (用于 UI 样式调试)
        /// </summary>
        private void LoadMockData()
        {
            var mockData = new List<ChartData>();
            var random = new Random();
            DateTime currentTime = DateTime.Today.AddHours(8); // 早上 8 点开机

            double baseWeight = 50.0; // 基准重量

            // 模拟 1000 个点的数据，带有自然的正弦波动和随机噪点
            for (int i = 0; i < 1000; i++)
            {
                // 每隔 2~5 秒产出一个产品
                currentTime = currentTime.AddSeconds(random.Next(2, 6));

                // 模拟工厂真实环境：设备温度随时间变化引起的基准漂移 (正弦波) + 偶然误差 (随机数)
                double drift = Math.Sin(i / 50.0) * 1.5;
                double noise = (random.NextDouble() - 0.5) * 2.0;

                double finalValue = baseWeight + drift + noise;

                mockData.Add(new ChartData
                {
                    Time = currentTime,
                    Value = finalValue
                });
            }

            // 更新图表与统计值
            HistoryDatas = mockData;
            ChartDataMean = mockData.Average(d => d.Value).ToString("F3");
            ChartMax = mockData.Max(d => d.Value).ToString("F3");
            ChartMin = mockData.Min(d => d.Value).ToString("F3");
            ChartCPK = "1.334"; // 假想的良率
            ChartCPU = "1.452";
            ChartCPL = "1.216";
            ChartUpperLimit = "55.000";
            ChartLowerLimit = "45.000";
        }

        #endregion

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}