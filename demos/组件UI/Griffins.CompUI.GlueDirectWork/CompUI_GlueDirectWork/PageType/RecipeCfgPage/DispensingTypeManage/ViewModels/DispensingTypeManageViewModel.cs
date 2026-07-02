using GKG.UI;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.DispensingTypeManage.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.ComponentModel;
using System.Linq;
using System.Reactive;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.DispensingTypeManage.ViewModels
{
    internal sealed class DispensingTypeManageViewModel : ReactiveObject
    {
        private bool _isLoading;
        private bool _readOnly;
        private DispensingTypeItemViewModel? _selectedItem;

        public event EventHandler? AfterModified;

        public DispensingTypeManageViewModel()
        {
            MachineDispensingTypeNameViewModel = new TextInputViewModel();
            MachineDispensingTypeNameViewModel.ValueChanged += (_, __) =>
            {
                if (!_isLoading)
                {
                    AfterModified?.Invoke(this, EventArgs.Empty);
                }
            };

            Items = new ObservableCollection<DispensingTypeItemViewModel>();

            AddCommand = ReactiveCommand.Create(() => AddItemWithName(string.Empty), this.WhenAnyValue(x => x.ReadOnly, ro => !ro));
            RemoveItemCommand = ReactiveCommand.Create<DispensingTypeItemViewModel>(RemoveItem, this.WhenAnyValue(x => x.ReadOnly, ro => !ro));
        }

        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                this.RaiseAndSetIfChanged(ref _readOnly, value);
                MachineDispensingTypeNameViewModel.IsEnabled = !value;
            }
        }

        public TextInputViewModel MachineDispensingTypeNameViewModel { get; }

        public string MachineDispensingTypeName
        {
            get => MachineDispensingTypeNameViewModel.Text;
            set
            {
                if (MachineDispensingTypeNameViewModel.Text == value) return;
                MachineDispensingTypeNameViewModel.Text = value ?? string.Empty;
                this.RaisePropertyChanged(nameof(MachineDispensingTypeName));
            }
        }

        public ObservableCollection<DispensingTypeItemViewModel> Items { get; }

        public DispensingTypeItemViewModel? SelectedItem
        {
            get => _selectedItem;
            set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
        }

        public ReactiveCommand<Unit, Unit> AddCommand { get; }

        public ReactiveCommand<DispensingTypeItemViewModel, Unit> RemoveItemCommand { get; }

        /// <summary>
        /// 将模型数据加载到界面（用于进入页面或切换配方时刷新显示）。
        /// </summary>
        public void SetData(DispensingTypeManageModel model)
        {
            model ??= new DispensingTypeManageModel();

            _isLoading = true;

            MachineDispensingTypeName = model.MachineDispensingTypeName ?? string.Empty;

            foreach (var old in Items)
            {
                old.PropertyChanged -= OnItemPropertyChanged;
            }
            Items.Clear();
            if (model.DispensingTypes != null)
            {
                foreach (var it in model.DispensingTypes)
                {
                    var vm = new DispensingTypeItemViewModel
                    {
                        SerialNumber = it.SerialNumber,
                        DispensingName = it.DispensingName ?? string.Empty,
                    };
                    vm.PropertyChanged += OnItemPropertyChanged;
                    vm.AfterModified += (_, __) => { if (!_isLoading) AfterModified?.Invoke(this, EventArgs.Empty); };
                    Items.Add(vm);
                }
            }

            RefreshSerialNumbers();
            SelectedItem = Items.FirstOrDefault();

            _isLoading = false;
        }

        /// <summary>
        /// 从当前界面提取数据并生成模型（用于保存配方/持久化）。
        /// </summary>
        public DispensingTypeManageModel GetData()
        {
            var model = new DispensingTypeManageModel
            {
                MachineDispensingTypeName = MachineDispensingTypeName ?? string.Empty,
                DispensingTypes = new List<DispensingTypeItemModel>(),
            };

            foreach (var vm in Items)
            {
                model.DispensingTypes.Add(new DispensingTypeItemModel
                {
                    SerialNumber = vm.SerialNumber,
                    DispensingName = vm.DispensingName ?? string.Empty,
                });
            }

            return model;
        }

        /// <summary>
        /// 新增一条胶水类型：优先使用输入框内容，否则使用默认名称。
        /// </summary>
        public void AddItemWithName(string dispensingName)
        {
            if (ReadOnly)
            {
                return;
            }

            var inputName = (dispensingName ?? string.Empty).Trim();
            var finalName = string.IsNullOrEmpty(inputName)
                ? $"胶水{GetNextDefaultDispensingIndex()}"
                : inputName;

            var vm = new DispensingTypeItemViewModel
            {
                DispensingName = finalName,
            };
            vm.PropertyChanged += OnItemPropertyChanged;
            vm.AfterModified += (_, __) => { if (!_isLoading) AfterModified?.Invoke(this, EventArgs.Empty); };

            Items.Add(vm);
            RefreshSerialNumbers();
            SelectedItem = vm;

            if (!_isLoading)
            {
                AfterModified?.Invoke(this, EventArgs.Empty);
            }
        }

        private int GetNextDefaultDispensingIndex()
        {
            var max = 0;
            foreach (var it in Items)
            {
                if (TryParseDefaultDispensingIndex(it.DispensingName, out var n))
                {
                    max = Math.Max(max, n);
                }
            }

            return max + 1;
        }

        private static bool TryParseDefaultDispensingIndex(string? name, out int index)
        {
            index = 0;
            if (string.IsNullOrWhiteSpace(name))
                return false;

            var s = name.Trim();
            const string prefix = "胶水";
            if (!s.StartsWith(prefix, StringComparison.Ordinal))
                return false;

            var suffix = s.Substring(prefix.Length).Trim();
            return int.TryParse(suffix, NumberStyles.Integer, CultureInfo.InvariantCulture, out index) && index > 0;
        }

        private void RemoveItem(DispensingTypeItemViewModel? item)
        {
            if (ReadOnly || item == null)
            {
                return;
            }

            var idx = Items.IndexOf(item);
            if (idx < 0)
            {
                return;
            }

            Items.RemoveAt(idx);
            item.PropertyChanged -= OnItemPropertyChanged;
            RefreshSerialNumbers();
            SelectedItem = Items.FirstOrDefault();

            if (!_isLoading)
            {
                AfterModified?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 重新计算并刷新列表的序号列（从 1 开始连续编号）。
        /// </summary>
        private void RefreshSerialNumbers()
        {
            var i = 1;
            foreach (var it in Items)
            {
                it.SerialNumber = i++;
            }
        }

        private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e) { }
    }
}
