using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.FuncHeadGroup.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    /// <summary>方案配置 → 区域配置：区域名来自程序编辑「区域编辑」，功能头组来自功能头组配置。</summary>
    internal sealed class RecipePlanAreaConfigWorkspaceViewModel : ReactiveObject
    {
        #region 字段
        //指向父级的引用
        private readonly RecipePlanManageCfgViewModel _owner;  
        /// <summary>
        /// 统一管理当前ViewModel里创建的订阅/资源，避免内存泄漏和重复事件绑定
        /// </summary>
        private readonly CompositeDisposable _rootDispose = new(); 
        /// <summary>
        /// 区域启用状态缓存字典，管理每个区域的启用禁用状态
        /// </summary>
        private readonly Dictionary<AreaEditingRowViewModel, bool> _regionEnabledByAreaRow = new();
        /// <summary>功能头组参数区项的订阅集合（用于强制单选）。</summary>
        private readonly List<IDisposable> _funcHeadPickSubscriptions = new();
        private bool _isAllSelected;   //是否全选
        private bool _hasCheckedRows;  //当前是否存在被勾选项
        private int _filterMode;  //列表筛选模式:0表示全部，1表示只看启用，2表示只看禁用
        private RecipePlanAreaConfigRowViewModel? _selectedRow;
        #endregion

        #region 构造与事件绑定

        /// <summary>初始化区域配置工作区，绑定命令与外部数据源。</summary>
        public RecipePlanAreaConfigWorkspaceViewModel(RecipePlanManageCfgViewModel owner)
        {
            _owner = owner;
            Rows = new ObservableCollection<RecipePlanAreaConfigRowViewModel>();
            DisplayedRows = new ObservableCollection<RecipePlanAreaConfigRowViewModel>();
            FuncHeadPickItems = new ObservableCollection<RecipePlanFuncHeadPickItemViewModel>();

            PreviewCommand = ReactiveCommand.Create(() => { });
            BatchEnableCommand = ReactiveCommand.Create(BatchEnable, this.WhenAnyValue(x => x.HasCheckedRows));
            BatchDisableCommand = ReactiveCommand.Create(BatchDisable, this.WhenAnyValue(x => x.HasCheckedRows));
            BatchMoveUpCommand = ReactiveCommand.Create(BatchMoveUp, this.WhenAnyValue(x => x.HasCheckedRows));
            BatchMoveDownCommand = ReactiveCommand.Create(BatchMoveDown, this.WhenAnyValue(x => x.HasCheckedRows));
            MoveUpCommand = ReactiveCommand.Create(
                MoveUp,
                this.WhenAnyValue(x => x.SelectedRow).Select(row => row?.CanMoveUp == true));
            MoveDownCommand = ReactiveCommand.Create(
                MoveDown,
                this.WhenAnyValue(x => x.SelectedRow).Select(row => row?.CanMoveDown == true));
            ToggleRowEnableCommand = ReactiveCommand.Create<RecipePlanAreaConfigRowViewModel>(ToggleRowEnable);
            MoveRowUpCommand = ReactiveCommand.Create<RecipePlanAreaConfigRowViewModel>(MoveRowUp);
            MoveRowDownCommand = ReactiveCommand.Create<RecipePlanAreaConfigRowViewModel>(MoveRowDown);

            var area = _owner.AreaEditingWorkspaceViewModel;
            area.Items.CollectionChanged += OnAreaItemsChanged;
            _rootDispose.Add(Disposable.Create(() => area.Items.CollectionChanged -= OnAreaItemsChanged));

            RebuildAreaRows();
            RefreshFuncHeadPicks();

            _rootDispose.Add(
                _owner.WhenAnyValue(x => x.FuncHeadGroupData)
                    .Subscribe(_ => RefreshFuncHeadPicks()));
        }
        #endregion

        #region 集合属性
        public ObservableCollection<RecipePlanAreaConfigRowViewModel> Rows { get; }
        public ObservableCollection<RecipePlanAreaConfigRowViewModel> DisplayedRows { get; }
        public ObservableCollection<RecipePlanFuncHeadPickItemViewModel> FuncHeadPickItems { get; }
        #endregion

        #region 筛选与选择属性
        /// <summary>0 全部 / 1 启用 / 2 禁用</summary>
        public int FilterMode
        {
            get => _filterMode;
            set
            {
                if (_filterMode == value)
                    return;
                this.RaiseAndSetIfChanged(ref _filterMode, value);
                RefreshDisplayedRows();
                ForceRefreshStatusTexts();
            }
        }

        public bool IsAllSelected
        {
            get => _isAllSelected;
            set
            {
                if (!this.RaiseAndSetIfChanged(ref _isAllSelected, value))
                    return;
                foreach (var r in Rows)
                    r.IsChecked = value;
                UpdateHasCheckedRows();
            }
        }

        public bool HasCheckedRows
        {
            get => _hasCheckedRows;
            private set => this.RaiseAndSetIfChanged(ref _hasCheckedRows, value);
        }

        public RecipePlanAreaConfigRowViewModel? SelectedRow
        {
            get => _selectedRow;
            set => this.RaiseAndSetIfChanged(ref _selectedRow, value);
        }
        #endregion

        #region 命令
        public ReactiveCommand<Unit, Unit> PreviewCommand { get; }
        public ReactiveCommand<Unit, Unit> BatchEnableCommand { get; }
        public ReactiveCommand<Unit, Unit> BatchDisableCommand { get; }
        public ReactiveCommand<Unit, Unit> BatchMoveUpCommand { get; }
        public ReactiveCommand<Unit, Unit> BatchMoveDownCommand { get; }
        public ReactiveCommand<Unit, Unit> MoveUpCommand { get; }
        public ReactiveCommand<Unit, Unit> MoveDownCommand { get; }
        public ReactiveCommand<RecipePlanAreaConfigRowViewModel, Unit> ToggleRowEnableCommand { get; }
        public ReactiveCommand<RecipePlanAreaConfigRowViewModel, Unit> MoveRowUpCommand { get; }
        public ReactiveCommand<RecipePlanAreaConfigRowViewModel, Unit> MoveRowDownCommand { get; }
        #endregion

        #region 公共方法
        /// <summary>应用表头全选/全不选到当前全部行。</summary>
        public void ApplySelectAll(bool isChecked)
        {
            foreach (var r in Rows)
                r.IsChecked = isChecked;
            IsAllSelected = isChecked;
        }

        /// <summary>从功能头组模型刷新参数区勾选项。</summary>
        public void RefreshFuncHeadPicks()
        {
            foreach (var d in _funcHeadPickSubscriptions)
                d.Dispose();
            _funcHeadPickSubscriptions.Clear();

            FuncHeadPickItems.Clear();
            var model = _owner.FuncHeadGroupData;
            if (model?.Rows == null)
                return;

            foreach (var row in model.Rows)
            {
                var name = row.FunctionHeadDisplayName?.Trim();
                if (string.IsNullOrEmpty(name))
                    name = "功能头组";
                var item = new RecipePlanFuncHeadPickItemViewModel
                {
                    DisplayName = name,
                    IsSelected = false
                };
                _funcHeadPickSubscriptions.Add(
                    item.WhenAnyValue(x => x.IsSelected)
                        .Where(v => v)
                        .Subscribe(_ => EnsureSingleFuncHeadSelection(item)));
                FuncHeadPickItems.Add(item);
            }
        }
        #endregion

        #region 私有方法
        /// <summary>功能头组列表强制单选：选中当前项时取消其它项。</summary>
        private void EnsureSingleFuncHeadSelection(RecipePlanFuncHeadPickItemViewModel selected)
        {
            foreach (var it in FuncHeadPickItems)
            {
                if (!ReferenceEquals(it, selected) && it.IsSelected)
                    it.IsSelected = false;
            }
        }

        /// <summary>程序编辑区域列表变化时，重建区域配置行。</summary>
        private void OnAreaItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            RebuildAreaRows();
        }

        /// <summary>响应行属性变化，维护全选状态与启用状态映射。</summary>
        private void OnRowPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not RecipePlanAreaConfigRowViewModel row)
                return;

            if (e.PropertyName == nameof(RecipePlanAreaConfigRowViewModel.IsChecked))
            {
                UpdateHasCheckedRows();
                SyncAllSelected();
            }
            else if (e.PropertyName == nameof(RecipePlanAreaConfigRowViewModel.RegionEnabledInPlan))
            {
                _regionEnabledByAreaRow[row.SourceAreaRow] = row.RegionEnabledInPlan;
                RefreshDisplayedRows();
            }
        }

        /// <summary>根据程序编辑中的区域行重新构建当前工作区行。</summary>
        private void RebuildAreaRows()
        {
            // 保留已编辑的启用状态，避免重建行后丢失。
            foreach (var r in Rows)
            {
                _regionEnabledByAreaRow[r.SourceAreaRow] = r.RegionEnabledInPlan;
                r.PropertyChanged -= OnRowPropertyChanged;
                r.DisposeSubscriptions();
            }

            Rows.Clear();
            var items = _owner.AreaEditingWorkspaceViewModel.Items;
            SyncEnabledStateMapWithCurrentItems(items);
            var seq = 1;
            foreach (var areaRow in items)
            {
                var enabled = _regionEnabledByAreaRow.TryGetValue(areaRow, out var v) ? v : true;
                var vm = new RecipePlanAreaConfigRowViewModel(areaRow, seq++, enabled);
                vm.PropertyChanged += OnRowPropertyChanged;
                Rows.Add(vm);
            }
            RefreshMoveStates();

            RefreshDisplayedRows();
            UpdateHasCheckedRows();
            SyncAllSelected();
        }

        /// <summary>按当前筛选条件刷新展示列表。</summary>
        private void RefreshDisplayedRows()
        {
            DisplayedRows.Clear();
            foreach (var r in Rows)
            {
                if (PassesFilter(r))
                    DisplayedRows.Add(r);
            }
            ForceRefreshStatusTexts();
        }

        /// <summary>判断一行是否满足当前筛选条件。</summary>
        private bool PassesFilter(RecipePlanAreaConfigRowViewModel r)
        {
            return FilterMode switch
            {
                1 => r.RegionEnabledInPlan,
                2 => !r.RegionEnabledInPlan,
                _ => true
            };
        }

        /// <summary>批量启用所有已勾选区域。</summary>
        private void BatchEnable()
        {
            foreach (var r in Rows.Where(x => x.IsChecked))
                r.RegionEnabledInPlan = true;
            RefreshDisplayedRows();
            ForceRefreshStatusTexts();
        }

        /// <summary>批量禁用所有已勾选区域。</summary>
        private void BatchDisable()
        {
            foreach (var r in Rows.Where(x => x.IsChecked))
                r.RegionEnabledInPlan = false;
            RefreshDisplayedRows();
            ForceRefreshStatusTexts();
        }

        /// <summary>切换单行区域的启用/禁用状态。</summary>
        private void ToggleRowEnable(RecipePlanAreaConfigRowViewModel? row)
        {
            if (row == null)
                return;
            row.RegionEnabledInPlan = !row.RegionEnabledInPlan;
            RefreshDisplayedRows();
            ForceRefreshStatusTexts();
        }

        /// <summary>按当前选中行执行单行上移。</summary>
        private void MoveUp()
        {
            MoveRowUp(SelectedRow);
        }

        /// <summary>按当前选中行执行单行下移。</summary>
        private void MoveDown()
        {
            MoveRowDown(SelectedRow);
        }

        /// <summary>将指定行上移一位。</summary>
        private void MoveRowUp(RecipePlanAreaConfigRowViewModel? row)
        {
            if (row?.SourceAreaRow == null || !row.CanMoveUp)
                return;
            var list = _owner.AreaEditingWorkspaceViewModel.Items;
            var idx = list.IndexOf(row.SourceAreaRow);
            if (idx <= 0)
                return;
            MoveObservableItem(list, idx, idx - 1);
            RebuildAreaRows();
            SelectedRow = Rows.FirstOrDefault(x => x.SourceAreaRow == row.SourceAreaRow);
        }

        /// <summary>将指定行下移一位。</summary>
        private void MoveRowDown(RecipePlanAreaConfigRowViewModel? row)
        {
            if (row?.SourceAreaRow == null || !row.CanMoveDown)
                return;
            var list = _owner.AreaEditingWorkspaceViewModel.Items;
            var idx = list.IndexOf(row.SourceAreaRow);
            if (idx < 0 || idx >= list.Count - 1)
                return;
            MoveObservableItem(list, idx, idx + 1);
            RebuildAreaRows();
            SelectedRow = Rows.FirstOrDefault(x => x.SourceAreaRow == row.SourceAreaRow);
        }

        /// <summary>批量上移：连续勾选整段移动，不连续勾选逐项移动一格。</summary>
        private void BatchMoveUp()
        {
            var list = _owner.AreaEditingWorkspaceViewModel.Items;
            if (list.Count <= 1)
                return;

            if (!TryBuildCheckedSnapshot(list, out var checkedSources, out var checkedIndices))
                return;
            if (checkedIndices[0] == 0)
                return;

            if (IsContiguousIndices(checkedIndices))
            {
                var start = checkedIndices[0];
                var end = checkedIndices[^1];
                MoveContiguousBlockUp(list, start, end);
            }
            else
            {
                BubbleMoveCheckedUp(list, checkedSources);
            }

            var selectedSource = SelectedRow?.SourceAreaRow;
            RebuildAreaRows();
            if (selectedSource != null)
                SelectedRow = Rows.FirstOrDefault(x => x.SourceAreaRow == selectedSource);
        }

        /// <summary>批量下移：连续勾选整段移动，不连续勾选逐项移动一格。</summary>
        private void BatchMoveDown()
        {
            var list = _owner.AreaEditingWorkspaceViewModel.Items;
            if (list.Count <= 1)
                return;

            if (!TryBuildCheckedSnapshot(list, out var checkedSources, out var checkedIndices))
                return;
            if (checkedIndices[^1] >= list.Count - 1)
                return;

            if (IsContiguousIndices(checkedIndices))
            {
                var start = checkedIndices[0];
                var end = checkedIndices[^1];
                MoveContiguousBlockDown(list, start, end);
            }
            else
            {
                BubbleMoveCheckedDown(list, checkedSources);
            }

            var selectedSource = SelectedRow?.SourceAreaRow;
            RebuildAreaRows();
            if (selectedSource != null)
                SelectedRow = Rows.FirstOrDefault(x => x.SourceAreaRow == selectedSource);
        }

        /// <summary>
        /// 获取到被勾选项集合，以及对应的每一项在列表中的下标，为了给批量上下移使用
        /// </summary>
        private bool TryBuildCheckedSnapshot(ObservableCollection<AreaEditingRowViewModel> list,out HashSet<AreaEditingRowViewModel> checkedSources,out List<int> checkedIndices)
        {
            checkedSources = new HashSet<AreaEditingRowViewModel>();
            foreach (var r in Rows)
            {
                if (r.IsChecked)
                    checkedSources.Add(r.SourceAreaRow);
            }

            checkedIndices = new List<int>();
            for (var i = 0; i < list.Count; i++)
            {
                if (checkedSources.Contains(list[i]))
                    checkedIndices.Add(i);
            }
            return checkedIndices.Count > 0;
        }

        /// <summary>判断勾选下标是否连续。</summary>
        private static bool IsContiguousIndices(List<int> sortedIndices)
        {
            if (sortedIndices.Count <= 1)
                return true;
            for (var k = 1; k < sortedIndices.Count; k++)
            {
                if (sortedIndices[k] != sortedIndices[k - 1] + 1)
                    return false;
            }

            return true;
        }

        /// <summary>将连续区间 [start..end] 整体上移一格（与上方一行交换位置）。</summary>
        private static void MoveContiguousBlockUp(ObservableCollection<AreaEditingRowViewModel> list, int start, int end)
        {
            if (start <= 0)
                return;
            var slice = new List<AreaEditingRowViewModel>();
            // 把勾选的项都放到slice中
            for (var i = start; i <= end; i++)
                slice.Add(list[i]);
            //在全量集合list中移除所有被勾选的项
            for (var i = end; i >= start; i--)
                list.RemoveAt(i);
            // 将被勾选项一个一个添加到列表中
            for (var i = 0; i < slice.Count; i++)
                list.Insert(start - 1 + i, slice[i]);
        }

        /// <summary>将连续区间 [start..end] 整体下移一格（与下方一行交换位置）。</summary>
        private static void MoveContiguousBlockDown(ObservableCollection<AreaEditingRowViewModel> list, int start, int end)
        {
            if (end >= list.Count - 1)
                return;
            var slice = new List<AreaEditingRowViewModel>();
            for (var i = start; i <= end; i++)
                slice.Add(list[i]);
            for (var i = end; i >= start; i--)
                list.RemoveAt(i);
            for (var i = 0; i < slice.Count; i++)
                list.Insert(start + 1 + i, slice[i]);
        }

        /// <summary>非连续勾选：反复将勾选行与上方未勾选行交换，直到无法再上移。</summary>
        private static void BubbleMoveCheckedUp(ObservableCollection<AreaEditingRowViewModel> list, HashSet<AreaEditingRowViewModel> checkedSources)
        {
            // 仅移动一格：从上到下扫描，遇到“勾选且上一格未勾选”就交换一次
            for (var i = 1; i < list.Count; i++)
            {
                // 该项不是被勾选项直接跳过，进入下一个循环
                if (!checkedSources.Contains(list[i]))
                    continue;
                // 上一项是勾选项也跳过，进入下一个循环
                if (checkedSources.Contains(list[i - 1]))
                    continue;
                // 该项是勾选项且上一项不是勾选项则上移
                MoveObservableItem(list, i, i - 1);
            }
        }

        /// <summary>非连续勾选：反复将勾选行与下方未勾选行交换，直到无法再下移。</summary>
        private static void BubbleMoveCheckedDown(ObservableCollection<AreaEditingRowViewModel> list, HashSet<AreaEditingRowViewModel> checkedSources)
        {
            // 仅移动一格：从下到上扫描，遇到“勾选且下一格未勾选”就交换一次
            for (var i = list.Count - 2; i >= 0; i--)
            {
                // 该项不是被勾选项直接跳过，进入下一个循环
                if (!checkedSources.Contains(list[i]))
                    continue;
                // 下一项是勾选项也跳过，进入下一个循环
                if (checkedSources.Contains(list[i + 1]))
                    continue;
                // 该项是勾选项且下一项不是勾选项则下移
                MoveObservableItem(list, i, i + 1);
            }
        }

        /// <summary>在可观察集合中执行单项位置交换。</summary>
        private static void MoveObservableItem<T>(ObservableCollection<T> list, int oldIndex, int newIndex)
        {
            var item = list[oldIndex];
            list.RemoveAt(oldIndex);
            list.Insert(newIndex, item);
        }
        #endregion

        #region 辅助方法
        /// <summary>刷新每行是否允许上移/下移的状态。</summary>
        private void RefreshMoveStates()
        {
            for (var i = 0; i < Rows.Count; i++)
            {
                Rows[i].CanMoveUp = i > 0;
                Rows[i].CanMoveDown = i < Rows.Count - 1;
            }
        }

        /// <summary>强制刷新状态文本，避免表格单元格复用导致显示滞后。</summary>
        private void ForceRefreshStatusTexts()
        {
            // DataGrid 可能复用单元格导致计算属性显示不刷新，这里强制通知一次。
            foreach (var r in Rows)
                r.RaisePropertyChanged(nameof(RecipePlanAreaConfigRowViewModel.EnableStatusText));
        }

        /// <summary>清理已移除区域的启用状态缓存，保持映射与当前数据一致。</summary>
        private void SyncEnabledStateMapWithCurrentItems(ObservableCollection<AreaEditingRowViewModel> items)
        {
            var current = new HashSet<AreaEditingRowViewModel>(items);
            var staleKeys = _regionEnabledByAreaRow.Keys.Where(k => !current.Contains(k)).ToList();
            foreach (var k in staleKeys)
                _regionEnabledByAreaRow.Remove(k);
        }

        /// <summary>更新“是否存在勾选行”标记。</summary>
        private void UpdateHasCheckedRows()
        {
            HasCheckedRows = Rows.Any(x => x.IsChecked);
        }

        /// <summary>同步表头全选状态与当前勾选结果。</summary>
        private void SyncAllSelected()
        {
            var all = Rows.Count > 0 && Rows.All(x => x.IsChecked);
            if (_isAllSelected != all)
                this.RaiseAndSetIfChanged(ref _isAllSelected, all);
        }
        #endregion
    }

}
