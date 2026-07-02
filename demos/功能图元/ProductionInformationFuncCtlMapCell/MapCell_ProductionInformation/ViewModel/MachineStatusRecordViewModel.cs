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
    /// <summary>
    /// 机台状态统计视图模型
    /// 负责展示特定时间段内的机台运行状态统计信息。
    /// 采用委托注入设计，完全隔离底层通信组件。
    /// </summary>
    public class MachineStatusRecordViewModel : ReactiveObject, IDisposable
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        // 外部注入的底层拉取数据委托，参数依次为：起始时间、结束时间
        private readonly Func<DateTime, DateTime, Task<List<MachineStatusStatistics>>> _fetchStatusFunc;

        #region 数据绑定属性

        /// <summary>
        /// 查询起始日期
        /// </summary>
        [Reactive]
        public DateTime OffsetStartDate { get; set; } = DateTime.Today;

        /// <summary>
        /// 查询结束日期
        /// </summary>
        [Reactive]
        public DateTime OffsetEndDate { get; set; } = DateTime.Today;

        /// <summary>
        /// 供 DataGrid 绑定的机台状态统计数据列表
        /// </summary>
        public ObservableCollection<MachineStatusStatistics> MachineStatusList { get; } = new();

        #endregion

        #region 交互管线与命令

        /// <summary>
        /// 全局通用交互管线，用于拉起异常提示等独立弹窗
        /// </summary>
        public Interaction<ReactiveObject, bool> CommonInteraction { get; } = new();

        /// <summary>
        /// 根据当前选择的日期范围刷新数据
        /// </summary>
        public ReactiveCommand<Unit, Unit> RefreshCommand { get; }

        /// <summary>
        /// 快速刷新今日数据
        /// </summary>
        public ReactiveCommand<Unit, Unit> RefreshTodayCommand { get; }

        #endregion

        /// <summary>
        /// 实例化机台状态统计视图模型
        /// </summary>
        /// <param name="fetchStatusFunc">获取统计数据的异步委托</param>
        public MachineStatusRecordViewModel(Func<DateTime, DateTime, Task<List<MachineStatusStatistics>>> fetchStatusFunc)
        {
            _fetchStatusFunc = fetchStatusFunc ?? throw new ArgumentNullException(nameof(fetchStatusFunc));

            RefreshCommand = ReactiveCommand.CreateFromTask(ExeRefreshAsync);
            RefreshTodayCommand = ReactiveCommand.CreateFromTask(ExeRefreshTodayAsync);

            // 全局异常拦截管线
            // 集中处理查询过程中抛出的任何未捕获异常，阻断崩溃并弹窗提示
            Observable.Merge(RefreshCommand.ThrownExceptions, RefreshTodayCommand.ThrownExceptions)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(async ex =>
                {
                    System.Diagnostics.Debug.WriteLine($"[机台状态统计模块异常] {ex.Message}");

                    var errorDialog = new MessageDialogViewModel
                    {
                        Title = "查询失败",
                        IconType = MessageDialogIconType.Alarm,
                        Message = $"获取运行数据失败：\n{ex.Message}",
                        ButtonContentOk = "确定",
                        IsOkVisible = true,
                        IsYesVisible = false,
                        IsNoVisible = false
                    };

                    await CommonInteraction.Handle(errorDialog);
                }).DisposeWith(_disposables);

            // 实例化后自动触发今日数据加载
            RefreshTodayCommand.Execute().Subscribe().DisposeWith(_disposables);
        }

        #region 内部业务逻辑

        /// <summary>
        /// 执行自定义日期范围查询
        /// </summary>
        private async Task ExeRefreshAsync()
        {
            await QueryDataAsync(OffsetStartDate, OffsetEndDate);
            await Task.Delay(500); // 物理级防抖锁定，防止用户快速连点发送过多网络请求
        }

        /// <summary>
        /// 执行今日快速查询
        /// </summary>
        private async Task ExeRefreshTodayAsync()
        {
            OffsetStartDate = DateTime.Today;
            OffsetEndDate = DateTime.Today;

            await QueryDataAsync(DateTime.Today, DateTime.Today);
            await Task.Delay(500); // 物理级防抖锁定
        }

        /// <summary>
        /// 核心数据查询与 UI 刷新逻辑
        /// </summary>
        private async Task QueryDataAsync(DateTime start, DateTime end)
        {
            // 每次发起查询前，强制在 UI 线程清空旧数据，防止因后续异常导致界面残留历史数据
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => MachineStatusList.Clear());

            // 直接调用委托拉取数据，底层细节由 Controller 接管
            var list = await _fetchStatusFunc(start, end);

            if (list != null && list.Count > 0)
            {
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    foreach (var item in list)
                    {
                        MachineStatusList.Add(item);
                    }
                });
            }
        }

        #endregion

        /// <summary>
        /// 释放当前视图模型持有的非托管资源和静态订阅
        /// </summary>
        public void Dispose() => _disposables.Dispose();
    }
}