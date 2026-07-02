using GKG.Map.ProductionInformationFuncCtlMapCell.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace GKG.Map.ProductionInformationFuncCtlMapCell.ViewModel
{
    /// <summary>
    /// 生产详情视图模型
    /// 负责处理日期选择及对应生产详情数据的加载逻辑。
    /// </summary>
    public class ProductionDetailsViewModel : ReactiveObject, IDisposable
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        // 外部注入的底层拉取数据委托，参数为：目标查询日期
        private readonly Func<DateTime, Task<List<ProductionDetails>>> _fetchDetailsFunc;

        #region 数据绑定属性

        /// <summary>
        /// 当前选中的日期 (UI 日历控件绑定)
        /// </summary>
        [Reactive]
        public DateTime? SelectedDate { get; set; }

        /// <summary>
        /// 供 DataGrid 绑定的生产详情集合
        /// </summary>
        public ObservableCollection<ProductionDetails> ProductionItems { get; } = new();

        #endregion

        #region 交互管线与命令

        /// <summary>
        /// 全局通用交互管线，用于拉起异常提示等独立弹窗
        /// </summary>
        public Interaction<ReactiveObject, bool> CommonInteraction { get; } = new();

        /// <summary>
        /// 核心查询指令 (响应式管控执行状态)
        /// </summary>
        public ReactiveCommand<DateTime, Unit> QueryCommand { get; }

        #endregion

        /// <summary>
        /// 实例化生产详情视图模型
        /// </summary>
        /// <param name="fetchDetailsFunc">获取生产详情的异步委托</param>
        public ProductionDetailsViewModel(Func<DateTime, Task<List<ProductionDetails>>> fetchDetailsFunc)
        {
            _fetchDetailsFunc = fetchDetailsFunc ?? throw new ArgumentNullException(nameof(fetchDetailsFunc));

            // 初始化查询命令并绑定到底层异步方法
            QueryCommand = ReactiveCommand.CreateFromTask<DateTime>(QueryDataAsync);

            // 核心业务流：监听日期变更 -> 防抖 -> 触发查询命令
            this.WhenAnyValue(x => x.SelectedDate)
                .Where(date => date.HasValue)
                .Select(date => date.Value)
                .DistinctUntilChanged() // 过滤重复日期，防止无意义查询
                .InvokeCommand(QueryCommand)
                .DisposeWith(_disposables);

            // 异常拦截管线：捕获查询过程中发生的任何异常，拉起全局弹窗
            QueryCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(async ex =>
                {
                    System.Diagnostics.Debug.WriteLine($"[生产详情查询异常] {ex.Message}");

                    var errorDialog = new MessageDialogViewModel
                    {
                        Title = "查询失败",
                        IconType = MessageDialogIconType.Alarm,
                        Message = $"获取生产详情失败：\n{ex.Message}",
                        ButtonContentOk = "确定",
                        IsOkVisible = true,
                        IsYesVisible = false,
                        IsNoVisible = false
                    };

                    await CommonInteraction.Handle(errorDialog);
                }).DisposeWith(_disposables);

            // 赋值初始日期，自动触发第一次监听流执行首屏查询
            SelectedDate = DateTime.Today;
        }

        #region 内部业务逻辑

        /// <summary>
        /// 执行核心数据查询与 UI 刷新
        /// </summary>
        private async Task QueryDataAsync(DateTime targetDate)
        {
            // 每次发起查询前，强制在 UI 线程清空旧数据，防止因后续异常导致界面残留历史数据
            RxApp.MainThreadScheduler.Schedule(() => ProductionItems.Clear());

            // 调用纯净的委托获取数据，所有的 JSON 解析与脱机测试逻辑已在 图元对象 被接管
            var list = await _fetchDetailsFunc(targetDate);

            if (list != null && list.Count > 0)
            {
                // 将数据灌入 ObservableCollection 时，必须封送至主线程以防跨线程 UI 崩溃
                RxApp.MainThreadScheduler.Schedule(() =>
                {
                    foreach (var item in list)
                    {
                        ProductionItems.Add(item);
                    }
                });
            }

            // 物理级防抖锁定：让 Command 保持 500ms 的 "IsExecuting" 状态
            // 保护系统免受日历控件长按/快切引发的高频网络 IO 风暴
            await Task.Delay(500);
        }

        #endregion

        /// <summary>
        /// 释放当前视图模型持有的非托管资源和静态订阅
        /// </summary>
        public void Dispose() => _disposables.Dispose();
    }
}