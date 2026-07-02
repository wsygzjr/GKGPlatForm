using DynamicData;
using GKG.Map.ChartFuncCtlMapCell.Models;
using Griffins;
using Griffins.Map.Cmd;
using Griffins.UI2;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace GKG.Map.ChartFuncCtlMapCell.ViewModel
{
    /// <summary>
    /// 图表显示 ViewModel
    /// 负责处理实时图表数据的加载、交互逻辑、后台轮询以及与视图的状态同步
    /// </summary>
    public class ChartViewModel : ReactiveObject, IDisposable, IActivatableViewModel
    {
        #region 私有字段与依赖注入

        // 统一管理 Rx 订阅的销毁，防止内存泄漏
        private readonly CompositeDisposable _disposables = new();

        // 并发锁：保证底层图表数据源在多线程读写下的绝对安全
        private readonly object _syncLock = new object();

        // 内部动态数据源
        private readonly SourceList<ChartData> _todayDataSource = new();

        // 底层指令执行器与设备编号
        private readonly Func<MapCmdExector> _getCmdExector;
        private readonly string _mapNo;

        // 模拟器状态标志
        private bool _isSimulating = false;

        #endregion

        #region 生命周期与数据流输出

        /// <summary>
        /// 用于控制 ViewModel 的激活与销毁生命周期
        /// </summary>
        public ViewModelActivator Activator { get; } = new();

        /// <summary>
        /// 图表数据变更流 (View 层订阅此流来安全地更新图表 UI)
        /// </summary>
        public IObservable<IChangeSet<ChartData>> DataStream { get; }

        #endregion

        #region 绑定的属性 (UI Bindings)

        /// <summary>
        /// 支持的图表类型下拉列表
        /// </summary>
        public ReadOnlyCollection<string> AvailableChartTypes { get; } = new(new[]
        {
            "右阀单点称重", "左阀单点称重", "右阀定量称重", "左阀定量称重"
        });

        /// <summary>
        /// 当前选中的图表类型
        /// </summary>
        [Reactive] public string SelectedChartType { get; set; } = "右阀单点称重";

        /// <summary>
        /// 过程能力指数 (CPK)
        /// </summary>
        [Reactive] public double Cpk { get; set; }

        /// <summary>
        /// 数据统计上限 (Max)
        /// </summary>
        [Reactive] public double UpperLimit { get; set; }

        /// <summary>
        /// 数据统计下限 (Min)
        /// </summary>
        [Reactive] public double LowerLimit { get; set; }

        #endregion

        #region 命令与交互请求 (Commands & Interactions)

        /// <summary>
        /// 刷新图表命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> RefreshChartCommand { get; private set; }

        /// <summary>
        /// 打开本地日志文件夹命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> OpenFileCommand { get; private set; }

        /// <summary>
        /// 清除当前所有图表数据命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> CleanCommand { get; private set; }

        /// <summary>
        /// 打开历史图表弹窗命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> HistoryChartCommand { get; private set; }

        /// <summary>
        /// 底层加载数据命令 (参数: 类型, 开始时间, 结束时间)
        /// </summary>
        public ReactiveCommand<Tuple<string, DateTime, DateTime>, ChartDataDto> LoadDataCommand { get; private set; }

        /// <summary>
        /// 消息弹窗交互请求 (View 层接管并显示提示框)
        /// </summary>
        public Interaction<TipDialogViewModel, DialogResultType> ShowTipInteraction { get; } = new();

        /// <summary>
        /// 历史图表窗口交互请求 (View 层接管并弹出新窗口)
        /// </summary>
        public Interaction<HistoryChartWndViewModel, bool> HistoryChartInteraction { get; } = new();

        #endregion

        #region 构造函数

        public ChartViewModel(ChartPropertyModelEdit chartPropertyModelEdit, Func<MapCmdExector> getCmdExector, string mapNo)
        {
            _getCmdExector = getCmdExector;
            _mapNo = mapNo;

            // 1. 初始化核心数据流 (开启广播)
            DataStream = _todayDataSource.Connect()
                .Publish()
                .RefCount();

            // 2. 初始化命令绑定
            InitializeCommands();

            // 3. 监听下拉框类型变化：防抖并自动触发重载
            this.WhenAnyValue(x => x.SelectedChartType)
                .Where(x => !string.IsNullOrEmpty(x))
                .Skip(1)
                .DistinctUntilChanged()
                .Subscribe(async type => await HandleTypeChangeAsync(type))
                .DisposeWith(_disposables);

            // 4. 开启实时数据模拟 (如需对接真实底层，可注释此行)
            StartSimulation();
        }

        #endregion

        #region 核心业务逻辑 (Private Methods)

        /// <summary>
        /// 初始化所有 ReactiveCommand 及其订阅逻辑
        /// </summary>
        private void InitializeCommands()
        {
            RefreshChartCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await HandleTypeChangeAsync(SelectedChartType);
            });

            OpenFileCommand = ReactiveCommand.Create(() =>
            {
                Process.Start("explorer.exe", AppDomain.CurrentDomain.BaseDirectory);
            });

            CleanCommand = ReactiveCommand.CreateFromTask(ExeCleanAsync);

            HistoryChartCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                // 使用 using 保证历史窗口 ViewModel 在弹窗关闭后正确释放，防止内存泄漏
                using var historyVm = new HistoryChartWndViewModel(_getCmdExector(), _mapNo);
                await HistoryChartInteraction.Handle(historyVm);
            });

            LoadDataCommand = ReactiveCommand.CreateFromTask<Tuple<string, DateTime, DateTime>, ChartDataDto>(ExeLoadDataAsync);

            // 订阅加载数据结果
            LoadDataCommand.Subscribe(dto =>
            {
                if (dto != null)
                {
                    // 更新 UI 统计属性
                    Cpk = dto.CPK;
                    UpperLimit = dto.UpperLimit;
                    LowerLimit = dto.LowerLimit;

                    // 线程安全地覆盖底层数据源
                    lock (_syncLock)
                    {
                        _todayDataSource.Edit(innerList =>
                        {
                            innerList.Clear();
                            if (dto.Data != null)
                            {
                                innerList.AddRange(dto.Data);
                            }
                        });
                    }
                }
            }).DisposeWith(_disposables);
        }

        /// <summary>
        /// 处理图表类型的切换：清空旧数据并触发新一轮加载
        /// </summary>
        private async Task HandleTypeChangeAsync(string type)
        {
            lock (_syncLock)
            {
                _todayDataSource.Clear();
            }

            var queryParams = new Tuple<string, DateTime, DateTime>(
                type,
                DateTime.Today,
                DateTime.Today.AddDays(1));

            await LoadDataCommand.Execute(queryParams);
        }

        /// <summary>
        /// 从底层设备获取图表数据
        /// </summary>
        private async Task<ChartDataDto> ExeLoadDataAsync(Tuple<string, DateTime, DateTime> t)
        {
            try
            {
                return await Task.Run(() =>
                {
                    GFBaseTypeParamValueList result = _getCmdExector().ExecUIDataObjCommand(_mapNo, "LoadChartData", new GFBaseTypeParamValueList
                    {
                        new GFBaseTypeParamValue("Type", new GriffinsBaseValue(t.Item1)),
                        new GFBaseTypeParamValue("StartTime", new GriffinsBaseValue(t.Item2)),
                        new GFBaseTypeParamValue("EndTime", new GriffinsBaseValue(t.Item3))
                    });

                    var resultValue = result?.FirstOrDefault(p => p.ID == "ChartDataDto")?.Value;

                    if (resultValue != null)
                    {
                        var charDataDto = new ChartDataDto();
                        ((IGriffinsBaseValue)charDataDto).PopulateFromBaseValue(resultValue);
                        return charDataDto;
                    }
                    return null;
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"执行 LoadChartData 失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 执行清除数据逻辑，并下发清除指令给底层
        /// </summary>
        private async Task ExeCleanAsync()
        {
            var tipDialogVm = new TipDialogViewModel();
            tipDialogVm.Setup(
                message: "是否确定清除当前图表数据？",
                title: "严重警告",
                iconType: DialogIconType.Warning,
                comboType: TipButtonCombo.YesNo);

            DialogResultType result = await ShowTipInteraction.Handle(tipDialogVm);
            if (result != DialogResultType.Yes) return;

            // 1. 通知后台删文件
            try
            {
                await Task.Run(() =>
                {
                    _getCmdExector().ExecUIDataObjCommand(_mapNo, "ClearChartData", new GFBaseTypeParamValueList
                    {
                        new GFBaseTypeParamValue("Type", new GriffinsBaseValue(SelectedChartType)),
                        new GFBaseTypeParamValue("Time", new GriffinsBaseValue(DateTime.Today)),
                    });
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"执行 ClearChartData 失败: {ex.Message}");
            }

            // 2. 线程安全地清空前端数据流
            lock (_syncLock)
            {
                _todayDataSource.Clear();
            }

            // 3. 重置统计属性
            Cpk = 0;
            UpperLimit = 0;
            LowerLimit = 0;
        }

        #endregion

        #region 外部与模拟数据接口 (Public Methods)

        /// <summary>
        /// 添加新的独立数据点 (供外部 Service 实时推送，或模拟器调用)
        /// </summary>
        public void AddNewData(DateTime time, double value)
        {
            lock (_syncLock)
            {
                _todayDataSource.Add(new ChartData() { Time = time, Value = value });
            }
        }

        /// <summary>
        /// 开启实时数据模拟轮询 (用于 UI 样式展示与测试)
        /// </summary>
        public void StartSimulation()
        {
            if (_isSimulating) return;
            _isSimulating = true;

            Task.Run(async () =>
            {
                var random = new Random();
                double baseWeight = 50.0; // 设定基准重量
                double timeAngle = 0;

                while (_isSimulating)
                {
                    DateTime currentTime = DateTime.Now;

                    // 模拟漂移与噪点，形成真实的波形波动
                    double slowDrift = Math.Sin(timeAngle) * 1.5;
                    double sensorNoise = (random.NextDouble() - 0.5) * 0.6;
                    double currentValue = baseWeight + slowDrift + sensorNoise;

                    AddNewData(currentTime, currentValue);

                    timeAngle += 0.05;
                    await Task.Delay(500);
                }
            });
        }

        /// <summary>
        /// 停止实时数据模拟
        /// </summary>
        public void StopSimulation()
        {
            _isSimulating = false;
        }

        #endregion

        #region 资源清理 (IDisposable)

        public void Dispose()
        {
            // 停止可能在后台运行的模拟器
            StopSimulation();

            _disposables.Dispose();
            _todayDataSource.Dispose();
        }

        #endregion
    }
}