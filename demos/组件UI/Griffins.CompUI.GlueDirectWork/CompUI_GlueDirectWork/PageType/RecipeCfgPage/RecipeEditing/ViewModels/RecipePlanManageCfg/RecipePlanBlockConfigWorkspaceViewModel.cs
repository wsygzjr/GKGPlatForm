using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    /// <summary>
    /// 方案配置 → 区块配置：数据来源于程序编辑的模板区块（指定模板的 Blocks）。
    /// </summary>
    internal sealed class RecipePlanBlockConfigWorkspaceViewModel : ReactiveObject, IDisposable
    {
        #region 字段
        private readonly ProgramTemplateViewModel _template;
        private readonly CompositeDisposable _d = new();
        private readonly Dictionary<ProgramBlockViewModel, bool> _blockEnabledBySource = new();
        private RecipePlanBlockConfigRowViewModel? _selectedRow;
        #endregion

        #region 构造
        /// <summary>初始化区块配置工作区（绑定模板 Blocks 作为数据源）。</summary>
        public RecipePlanBlockConfigWorkspaceViewModel(RecipePlanManageCfgViewModel owner, ProgramTemplateViewModel template)
        {
            _template = template;

            Rows = new ObservableCollection<RecipePlanBlockConfigRowViewModel>();
            DisplayedRows = new ObservableCollection<RecipePlanBlockConfigRowViewModel>();

            PreviewCommand = ReactiveCommand.Create(() => { });

            ToggleRowEnableCommand = ReactiveCommand.Create<RecipePlanBlockConfigRowViewModel>(ToggleRowEnable);
            MoveRowUpCommand = ReactiveCommand.Create<RecipePlanBlockConfigRowViewModel>(MoveRowUp);
            MoveRowDownCommand = ReactiveCommand.Create<RecipePlanBlockConfigRowViewModel>(MoveRowDown);

            _template.Blocks.CollectionChanged += OnBlocksChanged;
            _d.Add(Disposable.Create(() => _template.Blocks.CollectionChanged -= OnBlocksChanged));

            RebuildBlockRows();
        }
        #endregion

        #region 属性
        /// <summary>列表行集合（全部）。</summary>
        public ObservableCollection<RecipePlanBlockConfigRowViewModel> Rows { get; }

        /// <summary>当前展示行集合（当前版本与 Rows 一致，预留筛选能力）。</summary>
        public ObservableCollection<RecipePlanBlockConfigRowViewModel> DisplayedRows { get; }

        /// <summary>当前选中行。</summary>
        public RecipePlanBlockConfigRowViewModel? SelectedRow
        {
            get => _selectedRow;
            set => this.RaiseAndSetIfChanged(ref _selectedRow, value);
        }
        #endregion

        #region 命令
        public ReactiveCommand<Unit, Unit> PreviewCommand { get; }

        public ReactiveCommand<RecipePlanBlockConfigRowViewModel, Unit> ToggleRowEnableCommand { get; }
        public ReactiveCommand<RecipePlanBlockConfigRowViewModel, Unit> MoveRowUpCommand { get; }
        public ReactiveCommand<RecipePlanBlockConfigRowViewModel, Unit> MoveRowDownCommand { get; }
        #endregion

        #region 刷新/重建
        /// <summary>Blocks 变化时重建列表。</summary>
        private void OnBlocksChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            RebuildBlockRows();
        }

        /// <summary>重建区块行，保持启用状态缓存。</summary>
        private void RebuildBlockRows()
        {
            SyncEnabledStateMapWithCurrentItems();

            Rows.Clear();
            for (var i = 0; i < _template.Blocks.Count; i++)
            {
                var b = _template.Blocks[i];
                var enabled = _blockEnabledBySource.TryGetValue(b, out var v) ? v : true;
                _blockEnabledBySource[b] = enabled;

                var row = new RecipePlanBlockConfigRowViewModel(b, b.Name, enabled)
                {
                    SerialNumber = i + 1,
                    CanMoveUp = i > 0,
                    CanMoveDown = i < _template.Blocks.Count - 1,
                };

                Rows.Add(row);
            }

            RefreshDisplayedRows();
        }

        /// <summary>当前版本不做筛选，DisplayedRows 与 Rows 同步。</summary>
        private void RefreshDisplayedRows()
        {
            DisplayedRows.Clear();
            foreach (var r in Rows)
                DisplayedRows.Add(r);
        }

        /// <summary>清理启用状态缓存字典中已不存在的区块。</summary>
        private void SyncEnabledStateMapWithCurrentItems()
        {
            var set = new HashSet<ProgramBlockViewModel>(_template.Blocks);
            var stale = _blockEnabledBySource.Keys.Where(k => !set.Contains(k)).ToList();
            foreach (var k in stale)
                _blockEnabledBySource.Remove(k);
        }
        #endregion

        #region 启用/禁用
        /// <summary>切换单行启用状态。</summary>
        private void ToggleRowEnable(RecipePlanBlockConfigRowViewModel row)
        {
            if (row == null)
                return;
            row.BlockEnabledInPlan = !row.BlockEnabledInPlan;
            _blockEnabledBySource[row.SourceBlock] = row.BlockEnabledInPlan;
            ForceRefreshStatusTexts();
        }

        #endregion

        #region 移动
        /// <summary>单行上移。</summary>
        private void MoveRowUp(RecipePlanBlockConfigRowViewModel row)
        {
            if (row == null)
                return;
            var idx = _template.Blocks.IndexOf(row.SourceBlock);
            if (idx <= 0)
                return;
            MoveObservableItem(_template.Blocks, idx, idx - 1);
            RebuildBlockRows();
            SelectedRow = Rows.FirstOrDefault(x => ReferenceEquals(x.SourceBlock, row.SourceBlock));
        }

        /// <summary>单行下移。</summary>
        private void MoveRowDown(RecipePlanBlockConfigRowViewModel row)
        {
            if (row == null)
                return;
            var idx = _template.Blocks.IndexOf(row.SourceBlock);
            if (idx < 0 || idx >= _template.Blocks.Count - 1)
                return;
            MoveObservableItem(_template.Blocks, idx, idx + 1);
            RebuildBlockRows();
            SelectedRow = Rows.FirstOrDefault(x => ReferenceEquals(x.SourceBlock, row.SourceBlock));
        }

        /// <summary>移动集合中的单个元素。</summary>
        private static void MoveObservableItem<T>(ObservableCollection<T> list, int oldIndex, int newIndex)
        {
            if (oldIndex == newIndex)
                return;
            if (oldIndex < 0 || oldIndex >= list.Count)
                return;
            if (newIndex < 0 || newIndex >= list.Count)
                return;
            list.Move(oldIndex, newIndex);
        }
        #endregion

        #region 其它
        /// <summary>强制刷新所有行的启用状态文本（规避 DataGrid 虚拟化导致的文本不更新）。</summary>
        private void ForceRefreshStatusTexts()
        {
            foreach (var r in Rows)
                r.RaisePropertyChanged(nameof(RecipePlanBlockConfigRowViewModel.EnableStatusText));
        }

        public void Dispose() => _d.Dispose();
        #endregion
    }
}

