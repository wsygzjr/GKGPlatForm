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
    /// 配方选择视图模型
    /// 负责配方列表的拉取、展示以及用户选择意图的转发。
    /// 采用委托注入设计，完全隔离底层通信组件。
    /// </summary>
    public class RecipeSelectionViewModel : ReactiveObject, IDisposable
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        // 外部注入的底层交互行为委托
        private readonly Func<Task<List<string>>> _fetchRecipesFunc;
        private readonly Func<string, Task> _applyRecipeFunc;

        #region 数据绑定属性

        /// <summary>
        /// 当前正在使用的配方名称
        /// </summary>
        public string CurrentRecipeName { get; }

        /// <summary>
        /// 供用户选择的配方列表集合
        /// </summary>
        public ObservableCollection<string> RecipeList { get; } = new();

        /// <summary>
        /// 用户当前选中的配方名称
        /// </summary>
        [Reactive]
        public string? SelectedRecipe { get; set; }

        #endregion

        #region 交互管线与命令

        /// <summary>
        /// 全局通用交互管线，用于拉起异常提示等独立弹窗
        /// </summary>
        public Interaction<ReactiveObject, bool> CommonInteraction { get; } = new();

        /// <summary>
        /// 视图关闭交互管线，用于通知绑定的 View 关闭自身
        /// </summary>
        public Interaction<bool, Unit> CloseDialog { get; } = new();

        public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }
        public ReactiveCommand<Unit, Unit> ConfirmCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        #endregion

        /// <summary>
        /// 实例化配方选择视图模型
        /// </summary>
        /// <param name="fetchRecipesFunc">拉取配方列表的异步委托</param>
        /// <param name="applyRecipeFunc">下发选中配方的异步委托</param>
        /// <param name="currentRecipeName">当前正在运行的配方名称</param>
        /// <exception cref="ArgumentNullException">必要委托为空时抛出异常</exception>
        public RecipeSelectionViewModel(
            Func<Task<List<string>>> fetchRecipesFunc,
            Func<string, Task> applyRecipeFunc,
            string currentRecipeName)
        {
            _fetchRecipesFunc = fetchRecipesFunc ?? throw new ArgumentNullException(nameof(fetchRecipesFunc));
            _applyRecipeFunc = applyRecipeFunc ?? throw new ArgumentNullException(nameof(applyRecipeFunc));
            CurrentRecipeName = currentRecipeName;

            LoadDataCommand = ReactiveCommand.CreateFromTask(QueryRecipeListAsync);

            // 业务防呆：仅当用户实际选中了某一项，且该项与当前运行配方不相同时，才允许触发确认下发
            var canConfirm = this.WhenAnyValue(x => x.SelectedRecipe,
                selected => !string.IsNullOrWhiteSpace(selected) && selected != CurrentRecipeName);

            ConfirmCommand = ReactiveCommand.CreateFromTask(ApplyRecipeAsync, canConfirm);

            // 取消操作直接通过管线通知视图关闭，并返回 false 标识未进行修改
            CancelCommand = ReactiveCommand.CreateFromTask(async () => await CloseDialog.Handle(false));

            // 全局异常拦截管线
            // 集中处理拉取列表或下发配方过程中抛出的任何未捕获异常
            Observable.Merge(LoadDataCommand.ThrownExceptions, ConfirmCommand.ThrownExceptions)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(async ex =>
                {
                    System.Diagnostics.Debug.WriteLine($"[配方模块执行异常] {ex.Message}");

                    var errorDialog = new MessageDialogViewModel
                    {
                        Title = "配方操作失败",
                        IconType = MessageDialogIconType.Alarm,
                        Message = $"发生异常：\n{ex.Message}",
                        ButtonContentOk = "确定",
                        IsOkVisible = true,
                        IsYesVisible = false,
                        IsNoVisible = false
                    };

                    await CommonInteraction.Handle(errorDialog);
                }).DisposeWith(_disposables);

            // 实例化后自动触发列表加载
            LoadDataCommand.Execute().Subscribe().DisposeWith(_disposables);
        }

        #region 内部业务逻辑

        /// <summary>
        /// 异步查询配方列表并刷新前端集合
        /// </summary>
        private async Task QueryRecipeListAsync()
        {
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => RecipeList.Clear());

            var list = await _fetchRecipesFunc();

            if (list != null && list.Count > 0)
            {
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    foreach (var item in list)
                    {
                        RecipeList.Add(item);
                    }
                });
            }
        }

        /// <summary>
        /// 异步应用选中的配方
        /// </summary>
        private async Task ApplyRecipeAsync()
        {
            if (string.IsNullOrEmpty(SelectedRecipe)) return;

            await _applyRecipeFunc(SelectedRecipe);

            // 指令下发无异常完成，通知视图安全关闭，并返回 true 标识切换成功
            await CloseDialog.Handle(true);
        }

        #endregion

        /// <summary>
        /// 释放当前视图模型持有的非托管资源和静态订阅
        /// </summary>
        public void Dispose() => _disposables.Dispose();
    }
}