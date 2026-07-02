using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using GKG.Map.ProductionInformationFuncCtlMapCell.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace GKG.Map.ProductionInformationFuncCtlMapCell.ViewModel
{
    #region 动态轨道计数数据模型 (子级 ViewModel)

    /// <summary>
    /// 轨道生产数据模型
    /// 负责承载单轨道的实时统计数据，并提供轨道级的物理防抖交互命令。
    /// </summary>
    public class LaneProductionModel : ReactiveObject
    {
        public string LaneId { get; }
        public string LaneName { get; }

        [Reactive] public int TotalBigBoardCount { get; set; }
        [Reactive] public int TotalSmallBoardCount { get; set; }
        [Reactive] public int CurrentBigBoardCount { get; set; }
        [Reactive] public int CurrentSmallBoardCount { get; set; }

        public ReactiveCommand<Unit, Unit> ClearTotalCommand { get; }
        public ReactiveCommand<Unit, Unit> ClearCurrentCommand { get; }

        public LaneProductionModel(string laneId, string laneName, Func<string, bool, Task> onClearAction)
        {
            LaneId = laneId;
            LaneName = laneName;

            // 物理级防抖：即使是清零确认弹窗，也必须加入防抖冷却锁，避免触摸屏环境下的极速连点穿透
            ClearTotalCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await onClearAction(LaneId, true);
                await Task.Delay(500);
            });

            ClearCurrentCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await onClearAction(LaneId, false);
                await Task.Delay(500);
            });
        }
    }

    #endregion

    /// <summary>
    /// 生产信息核心视图模型 (ViewModel)
    /// 纯净 MVVM 设计：彻底剥离底层控制逻辑，依靠响应式流 (Reactive Extensions) 实现状态的高效分发与同步。
    /// </summary>
    public class ProductionInformationViewModel : ReactiveObject, IDisposable, IActivatableViewModel
    {
        private static bool _isResourcesLoaded = false;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        // 绝对单一的数据源入口
        private readonly ProductionInfoPropertyModelEdit _model;

        #region UI 绑定属性 (平铺属性映射)

        [Reactive] public string CurrentRecipeName { get; set; } = string.Empty;
        [Reactive] public string MachineRunTime { get; set; } = string.Empty;
        [Reactive] public double UtilizationRate { get; set; }
        [Reactive] public double Uph { get; set; }

        [Reactive] public double CurrentCycleTime { get; set; }
        [Reactive] public double CurrentBoardProcessTime { get; set; }
        [Reactive] public double CurrentLoadTime { get; set; }
        [Reactive] public double CurrentUnloadTime { get; set; }
        [Reactive] public double CurrentMarkTime { get; set; }
        [Reactive] public double CurrentDispenseTime { get; set; }

        [Reactive] public double AverageCycleTime { get; set; }
        [Reactive] public double AverageBoardProcessTime { get; set; }
        [Reactive] public double AverageLoadTime { get; set; }
        [Reactive] public double AverageUnloadTime { get; set; }
        [Reactive] public double AverageMarkTime { get; set; }
        [Reactive] public double AverageDispenseTime { get; set; }

        /// <summary>
        /// 动态轨道数据集合
        /// </summary>
        public ObservableCollection<LaneProductionModel> LaneProductionList { get; } = new();

        #endregion

        #region 交互管线与事件委托 (Event Bus)

        public ViewModelActivator Activator { get; } = new();

        /// <summary>
        /// 提供给本界面的弹窗通讯管线
        /// ViewModel 通过该管线将纯净的子级弹窗 VM 抛给 View 层进行渲染展示。
        /// </summary>
        public Interaction<ReactiveObject, bool> CommonInteraction { get; } = new();

        // ViewModel 不做任何底层业务通信，仅向外抛出事件，由图元对象订阅并接管！
        public event Func<Interaction<ReactiveObject, bool>, Task>? OnSwitchRecipeRequested;
        public event Func<Interaction<ReactiveObject, bool>, Task>? OnZipChangeProgramRequested;
        public event Func<Interaction<ReactiveObject, bool>, Task>? OnQueryProductionInfoRequested;
        public event Func<Interaction<ReactiveObject, bool>, Task>? OnShowMachineRunningStatisticsRequested;
        public event Func<string, bool, Task>? OnClearLaneDataRequested;

        #endregion

        #region 物理防抖命令

        public ReactiveCommand<Unit, Unit> SwitchRecipeCommand { get; }
        public ReactiveCommand<Unit, Unit> ZipChangeProgramCommand { get; }
        public ReactiveCommand<Unit, Unit> QueryProductionInfoCommand { get; }
        public ReactiveCommand<Unit, Unit> ShowMachineRunningStatisticsCommand { get; }

        #endregion

        /// <summary>
        /// 实例化生产信息视图模型
        /// </summary>
        /// <param name="propertyModelEdit">底层图元数据编辑模型</param>
        public ProductionInformationViewModel(ProductionInfoPropertyModelEdit propertyModelEdit)
        {
            _model = propertyModelEdit ?? throw new ArgumentNullException(nameof(propertyModelEdit));

            LoadModuleResources();

            // 物理级防抖控制：将 UI 点击意图异步派发给外部，并强制锁定按钮 500ms
            SwitchRecipeCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (OnSwitchRecipeRequested != null) await OnSwitchRecipeRequested.Invoke(CommonInteraction);
                await Task.Delay(500);
            });

            ZipChangeProgramCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (OnZipChangeProgramRequested != null) await OnZipChangeProgramRequested.Invoke(CommonInteraction);
                await Task.Delay(500);
            });

            QueryProductionInfoCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (OnQueryProductionInfoRequested != null) await OnQueryProductionInfoRequested.Invoke(CommonInteraction);
                await Task.Delay(500);
            });

            ShowMachineRunningStatisticsCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (OnShowMachineRunningStatisticsRequested != null) await OnShowMachineRunningStatisticsRequested.Invoke(CommonInteraction);
                await Task.Delay(500);
            });

            // 响应式生命周期托管
            this.WhenActivated(disposables =>
            {
                // 当 Model.Datas 发生替换或更新时，自动同步到 ViewModel 的平铺属性上
                _model.WhenAnyValue(x => x.Datas)
                      .ObserveOn(RxApp.MainThreadScheduler) // 确保属性变更在 UI 线程执行
                      .Subscribe(data =>
                      {
                          if (data == null) return;

                          // 基础统计映射
                          CurrentRecipeName = data.CurrentRecipeName;
                          MachineRunTime = data.MachineRunTime;
                          UtilizationRate = data.UtilizationRate;
                          Uph = data.Uph;

                          // 当前周期映射
                          CurrentCycleTime = data.CurrentCycleTime;
                          CurrentBoardProcessTime = data.CurrentBoardProcessTime;
                          CurrentLoadTime = data.CurrentLoadTime;
                          CurrentUnloadTime = data.CurrentUnloadTime;
                          CurrentMarkTime = data.CurrentMarkTime;
                          CurrentDispenseTime = data.CurrentDispenseTime;

                          // 平均周期映射
                          AverageCycleTime = data.AverageCycleTime;
                          AverageBoardProcessTime = data.AverageBoardProcessTime;
                          AverageLoadTime = data.AverageLoadTime;
                          AverageUnloadTime = data.AverageUnloadTime;
                          AverageMarkTime = data.AverageMarkTime;
                          AverageDispenseTime = data.AverageDispenseTime;

                          // 轨道数据聚合映射
                          SyncLaneData(data.Lanes);
                      })
                      .DisposeWith(disposables);

                // 全局异常拦截管线：捕获上述所有防抖命令中产生的未处理异常
                Observable.Merge(
                    SwitchRecipeCommand.ThrownExceptions,
                    ZipChangeProgramCommand.ThrownExceptions,
                    QueryProductionInfoCommand.ThrownExceptions,
                    ShowMachineRunningStatisticsCommand.ThrownExceptions
                )
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(async ex =>
                {
                    System.Diagnostics.Debug.WriteLine($"[生产信息 UI 操作异常] {ex.Message}");

                    var errorDialog = new MessageDialogViewModel
                    {
                        Title = "操作失败",
                        IconType = MessageDialogIconType.Alarm,
                        Message = $"发生系统级异常：\n{ex.Message}",
                        ButtonContentOk = "确定",
                        IsOkVisible = true,
                        IsYesVisible = false,
                        IsNoVisible = false
                    };

                    await CommonInteraction.Handle(errorDialog);
                })
                .DisposeWith(disposables);
            });
        }

        #region 内部业务逻辑 (UI 交互防呆与集合同步)

        /// <summary>
        /// 拦截清零交互，向用户发起高危操作弹窗确认
        /// </summary>
        private async Task ExeClearLaneDataAsync(string laneId, bool isTotal)
        {
            var messageDialog = new MessageDialogViewModel
            {
                Title = "安全操作确认",
                IconType = MessageDialogIconType.Question,
                Message = $"警告：即将不可逆地清零轨道 [{laneId}] 的 {(isTotal ? "总生产数量" : "当前生产数量")}。\n您确定要继续吗？",
                ButtonContentYes = "确定清零",
                ButtonContentNo = "取消",
                IsYesVisible = true,
                IsNoVisible = true,
                IsOkVisible = false
            };

            bool isConfirmed = await CommonInteraction.Handle(messageDialog);

            // 确认后，将纯净的业务意图抛给外层图元对象执行真实的硬件指令
            if (isConfirmed && OnClearLaneDataRequested != null)
            {
                await OnClearLaneDataRequested.Invoke(laneId, isTotal);
            }
        }

        /// <summary>
        /// 无损同步动态轨道数据集合
        /// </summary>
        /// <param name="lanes">后端推送的最新轨道字典</param>
        private void SyncLaneData(Dictionary<string, LaneData>? lanes)
        {
            if (lanes == null || lanes.Count == 0)
            {
                LaneProductionList.Clear();
                return;
            }

            // 1. 反向遍历，安全剔除已经消失的轨道，避免集合越界
            for (int i = LaneProductionList.Count - 1; i >= 0; i--)
            {
                if (!lanes.ContainsKey(LaneProductionList[i].LaneId))
                {
                    LaneProductionList.RemoveAt(i);
                }
            }

            // 2. 遍历后端新数据，执行 Upsert (更新或插入) 操作
            foreach (var kvp in lanes)
            {
                var data = kvp.Value;
                if (data == null) continue;

                // 寻找已有匹配轨道
                LaneProductionModel? existingLane = null;
                for (int i = 0; i < LaneProductionList.Count; i++)
                {
                    if (LaneProductionList[i].LaneId == data.LaneId)
                    {
                        existingLane = LaneProductionList[i];
                        break;
                    }
                }

                if (existingLane != null)
                {
                    // 轨道已存在：复用内存，无损更新其计数值，触发 Reactive 通知
                    existingLane.TotalBigBoardCount = data.TotalBigBoardCount;
                    existingLane.TotalSmallBoardCount = data.TotalSmallBoardCount;
                    existingLane.CurrentBigBoardCount = data.CurrentBigBoardCount;
                    existingLane.CurrentSmallBoardCount = data.CurrentSmallBoardCount;
                }
                else
                {
                    // 轨道不存在：实例化全新轨道并装载到 UI 绑定集合中
                    LaneProductionList.Add(new LaneProductionModel(data.LaneId, data.LaneName, ExeClearLaneDataAsync)
                    {
                        TotalBigBoardCount = data.TotalBigBoardCount,
                        TotalSmallBoardCount = data.TotalSmallBoardCount,
                        CurrentBigBoardCount = data.CurrentBigBoardCount,
                        CurrentSmallBoardCount = data.CurrentSmallBoardCount
                    });
                }
            }
        }

        #endregion

        #region 资源加载与回收

        /// <summary>
        /// 预加载当前模块专属的 Avalonia 样式与资源字典
        /// </summary>
        private void LoadModuleResources()
        {
            if (_isResourcesLoaded) return;

            var app = Application.Current;
            if (app != null)
            {
                var baseUri = new Uri("avares://GKG.Map.ProductionInformationFuncCtlMapCell/");
                app.Resources.MergedDictionaries.Add(new ResourceInclude(baseUri) { Source = new Uri("avares://GKG.Map.ProductionInformationFuncCtlMapCell/Assets/MyControlResources.axaml") });
                app.Styles.Add(new StyleInclude(baseUri) { Source = new Uri("avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml") });
                app.Styles.Add(new StyleInclude(baseUri) { Source = new Uri("avares://GKG.Map.ProductionInformationFuncCtlMapCell/Styles/ControlStyles.axaml") });

                _isResourcesLoaded = true;
            }
        }

        /// <summary>
        /// 回收视图模型产生的非托管资源和静态订阅
        /// </summary>
        public void Dispose() => _disposables.Dispose();

        #endregion
    }
}