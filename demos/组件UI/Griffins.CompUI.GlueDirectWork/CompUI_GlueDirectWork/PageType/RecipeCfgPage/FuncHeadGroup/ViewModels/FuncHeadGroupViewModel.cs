using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.FuncHeadGroup.Models;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reactive;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.FuncHeadGroup.ViewModels
{
    internal sealed class FuncHeadGroupViewModel : ReactiveObject
    {
        private bool _isLoading;
        private bool _readOnly;
        private FuncHeadGroupRowViewModel? _selectedItem;

        /// <summary>界面数据发生变更时通知外部（加载阶段会抑制）。</summary>
        public event EventHandler? AfterModified;

        public FuncHeadGroupViewModel()
        {
            Items = new ObservableCollection<FuncHeadGroupRowViewModel>();
            AddCommand = ReactiveCommand.Create(AddRow, this.WhenAnyValue(x => x.IsEditable));
            DeleteRowCommand = ReactiveCommand.Create<FuncHeadGroupRowViewModel>(DeleteRow, this.WhenAnyValue(x => x.IsEditable));
        }

        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                if (this.RaiseAndSetIfChanged(ref _readOnly, value))
                {
                    this.RaisePropertyChanged(nameof(IsEditable));
                }
            }
        }

        /// <summary>用于行内控件在只读模式下禁用编辑。</summary>
        public bool IsEditable => !ReadOnly;

        public ObservableCollection<FuncHeadGroupRowViewModel> Items { get; }
        public FuncHeadGroupRowViewModel? SelectedItem
        {
            get => _selectedItem;
            set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
        }
        public ReactiveCommand<Unit, Unit> AddCommand { get; }
        public ReactiveCommand<FuncHeadGroupRowViewModel, Unit> DeleteRowCommand { get; }

        /// <summary>
        /// 加载功能头组配置到界面，仅使用当前配方已保存的数据。
        /// </summary>
        public void SetData(FuncHeadGroupModel? model)
        {
            model ??= new FuncHeadGroupModel();

            _isLoading = true;

            Items.Clear();
            var serial = 1;
            if (model.Rows != null)
            {
                foreach (var saved in model.Rows)
                {
                    var persistedName = saved.FunctionHeadDisplayName?.Trim() ?? string.Empty;
                    var displayName = !string.IsNullOrEmpty(persistedName)
                        ? persistedName
                        : $"功能头组{serial}";

                    var row = new FuncHeadGroupRowViewModel
                    {
                        SerialNumber = serial++,
                        FunctionHeadDisplayName = displayName,
                    };
                    row.AfterModified += (_, __) => { if (!_isLoading) AfterModified?.Invoke(this, EventArgs.Empty); };
                    Items.Add(row);
                }
            }

            RefreshSerialNumbers();
            SelectedItem = Items.FirstOrDefault();

            _isLoading = false;
        }

        /// <summary>
        /// 从当前界面提取可持久化的配置数据（用于保存配方）。
        /// </summary>
        public FuncHeadGroupModel GetData()
        {
            var model = new FuncHeadGroupModel();
            foreach (var vm in Items)
            {
                model.Rows.Add(new FuncHeadGroupRowModel
                {
                    FunctionHeadDisplayName = vm.FunctionHeadDisplayName ?? string.Empty,
                });
            }

            return model;
        }

        private void AddRow()
        {
            var nextIndex = GetNextDefaultGroupIndex();
            var row = new FuncHeadGroupRowViewModel
            {
                FunctionHeadDisplayName = $"功能头组{nextIndex}",
                SerialNumber = Items.Count + 1,
            };
            row.AfterModified += (_, __) => { if (!_isLoading) AfterModified?.Invoke(this, EventArgs.Empty); };
            Items.Add(row);
            RefreshSerialNumbers();
            SelectedItem = row;
            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        private int GetNextDefaultGroupIndex()
        {
            var max = 0;
            foreach (var it in Items)
            {
                if (TryParseDefaultGroupIndex(it.FunctionHeadDisplayName, out var n))
                {
                    max = Math.Max(max, n);
                }
            }

            return max + 1;
        }

        private static bool TryParseDefaultGroupIndex(string? name, out int index)
        {
            index = 0;
            if (string.IsNullOrWhiteSpace(name))
                return false;

            var s = name.Trim();
            const string prefix = "功能头组";
            if (!s.StartsWith(prefix, StringComparison.Ordinal))
                return false;

            var suffix = s.Substring(prefix.Length).Trim();
            return int.TryParse(suffix, NumberStyles.Integer, CultureInfo.InvariantCulture, out index) && index > 0;
        }

        private void DeleteRow(FuncHeadGroupRowViewModel? row)
        {
            if (row == null || !Items.Contains(row))
                return;

            Items.Remove(row);
            RefreshSerialNumbers();
            SelectedItem = Items.FirstOrDefault();
            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        private void RefreshSerialNumbers()
        {
            var i = 1;
            foreach (var row in Items)
                row.SerialNumber = i++;
        }
    }
}
