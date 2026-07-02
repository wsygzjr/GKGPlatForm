using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Griffins.Map.UI;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.ControlCard.Views;
using System.Collections.Generic;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.ControlCard.ViewModels
{
    public class ControlCardListViewModel : ReactiveObject
    {
        private ICompUIRunTimeCallBack? _callBack;
        private bool _isUpdatingCheckState;

        public ObservableCollection<ControlCardViewModel> ControlCardList { get; } = new ObservableCollection<ControlCardViewModel>();
        public Func<IReadOnlyList<ControlCardViewModel.MotionCardDriverDefinition>>? DriverDefinitionsProvider { get; set; }

        public Func<Window?>? OwnerWindowProvider { get; set; }

        private ControlCardViewModel? _selectedControlCard;
        private ControlCardViewModel? _lastNonNullSelectedControlCard;

        public ControlCardViewModel? LastNonNullSelectedControlCard
        {
            get => _lastNonNullSelectedControlCard;
            private set => this.RaiseAndSetIfChanged(ref _lastNonNullSelectedControlCard, value);
        }

        public ControlCardViewModel? SelectedControlCard
        {
            get => _selectedControlCard;
            set
            {
                // DataGrid 切换选中项时，可能会短暂经过 null -> newItem 的过渡。
                // 允许 null 写入，避免干扰控件自身的选择状态机。
                // 业务层可以通过 LastNonNullSelectedControlCard 兜底，避免右侧区域出现空白。
                if (value == null && ControlCardList.Count > 0)
                {
                    var fallback = LastNonNullSelectedControlCard ?? _selectedControlCard ?? ControlCardList[0];
                  
                    if (fallback != null && !ReferenceEquals(_selectedControlCard, fallback))
                        this.RaiseAndSetIfChanged(ref _selectedControlCard, fallback);

                    return;
                }

                if (value != null)
                    LastNonNullSelectedControlCard = value;

                this.RaiseAndSetIfChanged(ref _selectedControlCard, value);
            }
        }

        private bool _isAllChecked;
        public bool IsAllChecked
        {
            get => _isAllChecked;
            set
            {
                if (this.RaiseAndSetIfChanged(ref _isAllChecked, value) && !_isUpdatingCheckState)
                    SetAllChecked(value);
            }
        }

        public ReactiveCommand<Unit, Unit> AddCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCheckedCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteSelectedCommand { get; }
        public ReactiveCommand<ControlCardViewModel, Unit> EditCommand { get; }
        public ReactiveCommand<ControlCardViewModel, Unit> OpenConfigCommand { get; }
        public ReactiveCommand<ControlCardViewModel, Unit> DeleteCommand { get; }

        public ControlCardListViewModel()
        {
            AddCommand = ReactiveCommand.CreateFromTask(AddAsync);
            DeleteCheckedCommand = ReactiveCommand.Create(DeleteChecked);
            DeleteSelectedCommand = ReactiveCommand.Create(DeleteSelected);

            EditCommand = ReactiveCommand.CreateFromTask<ControlCardViewModel>(EditAsync);
            OpenConfigCommand = ReactiveCommand.CreateFromTask<ControlCardViewModel>(OpenConfigAsync);

            DeleteCommand = ReactiveCommand.Create<ControlCardViewModel>(vm =>
            {
                if (vm == null) return;
                ControlCardList.Remove(vm);
                if (SelectedControlCard == vm)
                {
                    SelectedControlCard = ControlCardList.Count > 0 ? ControlCardList[0] : null;
                }
                UpdateCanDeleteFlags();
                UpdateIsAllChecked();
            });

            ControlCardList.CollectionChanged += ControlCardList_CollectionChanged;
            UpdateCanDeleteFlags();
            UpdateIsAllChecked();
        }

        private void ControlCardList_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is ControlCardViewModel vm)
                    {
                        vm.PropertyChanged += ControlCard_PropertyChanged;
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is ControlCardViewModel vm)
                        vm.PropertyChanged -= ControlCard_PropertyChanged;
                }
            }

            UpdateCanDeleteFlags();
            UpdateIsAllChecked();
        }

        private void ControlCard_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ControlCardViewModel.IsDummy))
                UpdateCanDeleteFlags();

            if (e.PropertyName == nameof(ControlCardViewModel.IsChecked))
                UpdateIsAllChecked();
        }

        private void UpdateCanDeleteFlags()
        {
            var canDeleteByCount = ControlCardList.Count >= 1;
            foreach (var vm in ControlCardList)
            {
                vm.CanDelete = canDeleteByCount && !vm.IsDummy;
            }
        }

        public void SetCallBack(ICompUIRunTimeCallBack? callBack)
        {
            _callBack = callBack;
        }

        private async Task AddAsync()
        {
            var owner = OwnerWindowProvider?.Invoke();
            if (owner == null)
                return;

            var newItem = new ControlCardViewModel();
            var driverDefinitions = DriverDefinitionsProvider?.Invoke();
            if (driverDefinitions != null && driverDefinitions.Count > 0)
                newItem.ApplyDriverDefinitions(driverDefinitions);

            var nextObjectId = GetNextObjectId();
            newItem.MotionCardCustomID = nextObjectId;
            newItem.MotionCardCustomIDViewModel.Text = nextObjectId;
            var dialogVm = new ControlCardEditWindowViewModel(newItem);
            var win = new ControlCardEditWindow { DataContext = dialogVm };
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var ok = await win.ShowDialog<bool>(owner);
            if (!ok)
                return;

            ControlCardList.Add(newItem);
            SelectedControlCard = newItem;
            UpdateIsAllChecked();
        }

        public string GetNextObjectId()
        {
            var maxId = 0;

            foreach (var card in ControlCardList)
            {
                if (int.TryParse(card.MotionCardCustomID?.Trim(), out var currentId) && currentId > maxId)
                    maxId = currentId;
            }

            return (maxId + 1).ToString();
        }

        private async Task EditAsync(ControlCardViewModel vm)
        {
            if (vm == null)
                return;

            var owner = OwnerWindowProvider?.Invoke();
            if (owner == null)
                return;

            var dialogVm = new ControlCardEditWindowViewModel(vm);
            var win = new ControlCardEditWindow { DataContext = dialogVm };
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var ok = await win.ShowDialog<bool>(owner);
            if (!ok)
                return;

            SelectedControlCard = vm;
        }

        private async Task OpenConfigAsync(ControlCardViewModel vm)
        {
            if (vm == null)
                return;

            var owner = OwnerWindowProvider?.Invoke();
            if (owner == null)
                return;

            var win = new ControlCardBlankConfigWindow();
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            await win.ShowDialog(owner);
        }

        private void DeleteSelected()
        {
            if (SelectedControlCard == null)
                return;

            DeleteCommand.Execute(SelectedControlCard).Subscribe();
        }

        private void DeleteChecked()
        {
            var checkedItems = ControlCardList.Where(x => x.IsChecked).ToList();
            if (checkedItems.Count == 0)
                return;

            foreach (var item in checkedItems)
            {
                ControlCardList.Remove(item);
            }

            SelectedControlCard = ControlCardList.FirstOrDefault();
            UpdateCanDeleteFlags();
            UpdateIsAllChecked();
        }

        private void SetAllChecked(bool isChecked)
        {
            _isUpdatingCheckState = true;
            try
            {
                foreach (var item in ControlCardList)
                    item.IsChecked = isChecked;
            }
            finally
            {
                _isUpdatingCheckState = false;
            }

            UpdateIsAllChecked();
        }

        private void UpdateIsAllChecked()
        {
            var isAllChecked = ControlCardList.Count > 0 && ControlCardList.All(x => x.IsChecked);

            _isUpdatingCheckState = true;
            try
            {
                this.RaiseAndSetIfChanged(ref _isAllChecked, isAllChecked);
            }
            finally
            {
                _isUpdatingCheckState = false;
            }
        }
    }
}
