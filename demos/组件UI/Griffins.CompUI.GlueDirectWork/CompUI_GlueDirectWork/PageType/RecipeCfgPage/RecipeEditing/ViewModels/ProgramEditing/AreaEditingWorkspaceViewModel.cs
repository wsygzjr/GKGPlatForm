using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    internal sealed class AreaEditingWorkspaceViewModel : ReactiveObject
    {
        /// <summary>当前列表选中行。</summary>
        private AreaEditingRowViewModel? _selectedItem;
        /// <summary>全选勾选状态。</summary>
        private bool _isAllSelected;
        /// <summary>模板集合引用（用于新建区域行的模板下拉源）。</summary>
        private readonly ObservableCollection<ProgramTemplateViewModel> _templates;

        /// <summary>初始化区域编辑工作区与默认首行数据。</summary>
        public AreaEditingWorkspaceViewModel(ObservableCollection<ProgramTemplateViewModel> templates)
        {
            _templates = templates;
            Items = new ObservableCollection<AreaEditingRowViewModel>();
            var first = new AreaEditingRowViewModel(_templates);
            first.AreaNameViewModel.Text = "区域1";
            Items.Add(first);
            SelectedItem = first;
            RefreshSerialNumbers();

            AddCommand = ReactiveCommand.Create(AddRow);
            BatchDeleteCommand = ReactiveCommand.Create(BatchDelete);
            PreviewCommand = ReactiveCommand.Create(() => { });
            DeleteRowCommand = ReactiveCommand.Create<AreaEditingRowViewModel>(DeleteRow);
        }

        /// <summary>区域编辑行集合。</summary>
        public ObservableCollection<AreaEditingRowViewModel> Items { get; }

        /// <summary>当前选中的区域行。</summary>
        public AreaEditingRowViewModel? SelectedItem
        {
            get => _selectedItem;
            set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
        }

        /// <summary>全选状态（同步控制每一行 IsChecked）。</summary>
        public bool IsAllSelected
        {
            get => _isAllSelected;
            set
            {
                if (this.RaiseAndSetIfChanged(ref _isAllSelected, value))
                {
                    foreach (var it in Items)
                        it.IsChecked = value;
                }
            }
        }

        /// <summary>新增区域行命令。</summary>
        public ReactiveCommand<Unit, Unit> AddCommand { get; }
        /// <summary>批量删除勾选区域命令。</summary>
        public ReactiveCommand<Unit, Unit> BatchDeleteCommand { get; }
        /// <summary>预览命令（预留）。</summary>
        public ReactiveCommand<Unit, Unit> PreviewCommand { get; }
        /// <summary>删除单行区域命令。</summary>
        public ReactiveCommand<AreaEditingRowViewModel, Unit> DeleteRowCommand { get; }

        /// <summary>
        /// 从“区域批量生成”追加一个新区域行，并默认选中该行。
        /// </summary>
        public void AppendRowFromBatch(AreaEditingRowViewModel row)
        {
            if (row == null)
                return;

            Items.Add(row);
            RefreshSerialNumbers();
            SelectedItem = row;
            IsAllSelected = false;
        }

        /// <summary>统一设置所有区域行的勾选状态。</summary>
        public void ApplySelectAll(bool isChecked)
        {
            foreach (var it in Items)
                it.IsChecked = isChecked;
            IsAllSelected = isChecked;
        }

        /// <summary>新增一个默认区域行并选中。</summary>
        private void AddRow()
        {
            var row = new AreaEditingRowViewModel(_templates);
            row.AreaNameViewModel.Text = $"区域{Items.Count + 1}";
            Items.Add(row);
            RefreshSerialNumbers();
            SelectedItem = row;
            IsAllSelected = false;
        }

        /// <summary>删除所有已勾选的区域行。</summary>
        private void BatchDelete()
        {
            var targets = Items.Where(x => x.IsChecked).ToList();
            foreach (var t in targets)
                Items.Remove(t);
            RefreshSerialNumbers();
            foreach (var r in Items)
                r.IsChecked = false;
            IsAllSelected = false;
            SelectedItem = Items.FirstOrDefault();
        }

        /// <summary>删除指定区域行并维护选中状态。</summary>
        private void DeleteRow(AreaEditingRowViewModel? row)
        {
            if (row == null || !Items.Contains(row))
                return;
            Items.Remove(row);
            RefreshSerialNumbers();
            IsAllSelected = Items.Count > 0 && Items.All(x => x.IsChecked);
            if (SelectedItem == row)
                SelectedItem = Items.FirstOrDefault();
        }

        /// <summary>按当前行顺序重排序号。</summary>
        private void RefreshSerialNumbers()
        {
            var i = 1;
            foreach (var it in Items)
                it.SerialNumber = i++;
        }
    }
}
