using GKG.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.ControlCard.ViewModels
{
    public class AxisConfigViewModel : ReactiveObject
    {
        private readonly Func<IEnumerable<ControlCardViewModel>> _cardsProvider;
        private bool _isUpdatingCheckState;
        private bool _isResolvingDuplicate;
        private readonly Dictionary<AxisConfigItemViewModel, string> _lastValidAxisNames = new();
        private readonly MotionControlFactoryParameterViewModel _fallbackAxisParameter = new();
        public event EventHandler? AfterModified;
        public ObservableCollection<AxisConfigItemViewModel> AxisItems { get; } = new();

        private AxisConfigItemViewModel? _selectedAxisItem;
        public AxisConfigItemViewModel? SelectedAxisItem
        {
            get => _selectedAxisItem;
            set
            {
                if (ReferenceEquals(_selectedAxisItem, value))
                    return;

                if (_selectedAxisItem != null)
                    _selectedAxisItem.PropertyChanged -= SelectedAxisItem_PropertyChanged;

                this.RaiseAndSetIfChanged(ref _selectedAxisItem, value);

                if (_selectedAxisItem != null)
                    _selectedAxisItem.PropertyChanged += SelectedAxisItem_PropertyChanged;

                RaiseSelectionProperties();
            }
        }

        public MotionControlFactoryParameterViewModel SelectedAxis =>
            SelectedAxisItem?.AxisParameter ?? _fallbackAxisParameter;
        public bool HasSelectedAxis => SelectedAxisItem?.HasSelectedAxis == true;
        public bool HasNoSelectedAxis => !HasSelectedAxis;
        public bool CanOpenAdvancedParameter =>
            SelectedAxisItem?.HasSelectedControlCard == true &&
            SelectedAxisItem?.HasSelectedAxis == true;
        public string SelectedItemDisplayName =>
            SelectedAxisItem == null
                ? string.Empty
                : string.IsNullOrWhiteSpace(SelectedAxisItem.AxisNameEditor)
                    ? SelectedAxisItem.AxisDisplayName
                    : SelectedAxisItem.AxisNameEditor;

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

        public IEnumerable<int> PulseOutputModeSource =>
        [
            GKG.MotionControlPulseOutputModeConstants.Pulse,
            GKG.MotionControlPulseOutputModeConstants.Level,
        ];

        public ReactiveCommand<Unit, Unit> AddCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCheckedCommand { get; }
        public ReactiveCommand<AxisConfigItemViewModel, Unit> DeleteCommand { get; }

        public AxisConfigViewModel(Func<IEnumerable<ControlCardViewModel>> cardsProvider)
        {
            _cardsProvider = cardsProvider;

            AddCommand = ReactiveCommand.Create(AddAxisItem);
            DeleteCheckedCommand = ReactiveCommand.Create(DeleteCheckedItems);
            DeleteCommand = ReactiveCommand.Create<AxisConfigItemViewModel>(DeleteAxisItem);
            AxisItems.CollectionChanged += AxisItems_CollectionChanged;
        }

        public void SyncFromCards()
        {
            foreach (var item in AxisItems)
            {
                item.RefreshFromCards();
            }

            UpdateSequenceNumbers();

            if (SelectedAxisItem != null && !AxisItems.Contains(SelectedAxisItem))
                SelectedAxisItem = AxisItems.FirstOrDefault();
        }

        public void Load(
            IEnumerable<GKG.AxisInformation>? items,
            Func<GKG.AxisInformation, GKG.MotionControlFactoryParameter?>? parameterResolver = null)
        {
            AxisItems.Clear();
            _lastValidAxisNames.Clear();

            if (items != null)
            {
                foreach (var dto in items)
                {
                    var item = new AxisConfigItemViewModel(_cardsProvider);
                    item.Load(dto, parameterResolver?.Invoke(dto));
                    AxisItems.Add(item);
                    _lastValidAxisNames[item] = item.GetNormalizedAxisName();
                }
            }

            UpdateSequenceNumbers();
            SelectedAxisItem = AxisItems.FirstOrDefault();
            UpdateIsAllChecked();
        }

        public List<GKG.AxisInformation> ToBackendAxisInformations()
        {
            return AxisItems
                .Select(x => x.ToBackendAxisInformation())
                .Where(x => x != null)
                .Cast<GKG.AxisInformation>()
                .ToList();
        }

        public List<(Guid MotionCardGuid, int AxisNo, GKG.MotionControlFactoryParameter Parameter)> ToBackendAxisParameterMappings()
        {
            return AxisItems
                .Select(item =>
                {
                    var axisInfo = item.ToBackendAxisInformation();
                    var parameter = item.ToBackendMotionControlFactoryParameter();
                    return axisInfo == null || parameter == null
                        ? ((Guid MotionCardGuid, int AxisNo, GKG.MotionControlFactoryParameter Parameter)?)null
                        : (axisInfo.MotionCardGuid, axisInfo.AxisNo, parameter);
                })
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .ToList();
        }

        public List<(Guid MotionCardGuid, int AxisNo, GKG.MotionControlFactoryParameter Parameter, long Revision)> ToBackendAxisParameterSnapshots()
        {
            return AxisItems
                .Select(item =>
                {
                    var axisInfo = item.ToBackendAxisInformation();
                    var parameter = item.ToBackendMotionControlFactoryParameter();
                    return axisInfo == null || parameter == null
                        ? ((Guid MotionCardGuid, int AxisNo, GKG.MotionControlFactoryParameter Parameter, long Revision)?)null
                        : (axisInfo.MotionCardGuid, axisInfo.AxisNo, parameter, item.AxisParameter.Revision);
                })
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .ToList();
        }

        private void AddAxisItem()
        {
            var item = new AxisConfigItemViewModel(_cardsProvider);
            item.InitializeEmpty();
            item.AxisNameEditor = GetNextDefaultAxisName();

            AxisItems.Add(item);
            _lastValidAxisNames[item] = item.GetNormalizedAxisName();
            UpdateSequenceNumbers();
            SelectedAxisItem = item;
            UpdateIsAllChecked();
            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        private void DeleteAxisItem(AxisConfigItemViewModel? item)
        {
            if (item == null)
                return;

            var wasSelected = ReferenceEquals(SelectedAxisItem, item);
            AxisItems.Remove(item);
            UpdateSequenceNumbers();

            if (wasSelected)
                SelectedAxisItem = AxisItems.FirstOrDefault();

            UpdateIsAllChecked();
            RemoveLastValidSnapshots(item);
            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        private void DeleteCheckedItems()
        {
            var checkedItems = AxisItems.Where(x => x.IsChecked).ToList();
            if (checkedItems.Count == 0)
                return;

            var selectedDeleted = checkedItems.Contains(SelectedAxisItem);
            foreach (var item in checkedItems)
            {
                AxisItems.Remove(item);
                RemoveLastValidSnapshots(item);
            }

            UpdateSequenceNumbers();

            if (selectedDeleted)
                SelectedAxisItem = AxisItems.FirstOrDefault();

            UpdateIsAllChecked();
            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        private void SelectedAxisItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AxisConfigItemViewModel.AxisParameter) ||
                e.PropertyName == nameof(AxisConfigItemViewModel.SelectedControlCardOption) ||
                e.PropertyName == nameof(AxisConfigItemViewModel.SelectedAxisOption) ||
                e.PropertyName == nameof(AxisConfigItemViewModel.AxisNameEditor) ||
                e.PropertyName == nameof(AxisConfigItemViewModel.HasSelectedControlCard))
            {
                RaiseSelectionProperties();
            }
        }

        private void RaiseSelectionProperties()
        {
            this.RaisePropertyChanged(nameof(SelectedAxis));
            this.RaisePropertyChanged(nameof(HasSelectedAxis));
            this.RaisePropertyChanged(nameof(HasNoSelectedAxis));
            this.RaisePropertyChanged(nameof(PulseOutputModeSource));
            this.RaisePropertyChanged(nameof(SelectedItemDisplayName));
            this.RaisePropertyChanged(nameof(CanOpenAdvancedParameter));
        }

        private void UpdateSequenceNumbers()
        {
            for (var i = 0; i < AxisItems.Count; i++)
            {
                AxisItems[i].SequenceNo = i + 1;
            }
        }

        private string GetNextDefaultAxisName()
        {
            var index = 1;
            var existingNames = AxisItems
                .Select(x => x.AxisNameEditor?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            while (existingNames.Contains($"逻辑轴{index}"))
            {
                index++;
            }

            return $"逻辑轴{index}";
        }

        private void AxisItems_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems.OfType<AxisConfigItemViewModel>())
                {
                    item.PropertyChanged -= AxisItem_PropertyChanged;
                    RemoveLastValidSnapshots(item);
                }
            }

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems.OfType<AxisConfigItemViewModel>())
                {
                    item.PropertyChanged += AxisItem_PropertyChanged;
                    _lastValidAxisNames[item] = item.GetNormalizedAxisName();
                }
            }

            UpdateIsAllChecked();
        }

        private void AxisItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isResolvingDuplicate || sender is not AxisConfigItemViewModel item)
                return;

            if (e.PropertyName == nameof(AxisConfigItemViewModel.IsChecked))
            {
                UpdateIsAllChecked();
                return;
            }

            if (e.PropertyName == nameof(AxisConfigItemViewModel.AxisNameEditor))
            {
                var previousName = _lastValidAxisNames.TryGetValue(item, out var savedName) ? savedName : string.Empty;
                ValidateAxisName(item);
                if (!string.Equals(previousName, item.GetNormalizedAxisName(), StringComparison.Ordinal))
                    AfterModified?.Invoke(this, EventArgs.Empty);
                return;
            }

            if (e.PropertyName == nameof(AxisConfigItemViewModel.SelectedControlCardOption) ||
                e.PropertyName == nameof(AxisConfigItemViewModel.SelectedAxisOption))
            {
                AfterModified?.Invoke(this, EventArgs.Empty);
                return;
            }

            if (e.PropertyName == nameof(AxisConfigItemViewModel.AxisParameter) ||
                e.PropertyName == nameof(AxisConfigItemViewModel.BindingModule))
            {
                AfterModified?.Invoke(this, EventArgs.Empty);
            }
        }

        private void ValidateAxisName(AxisConfigItemViewModel item)
        {
            var axisName = item.GetNormalizedAxisName();
            if (string.IsNullOrWhiteSpace(axisName))
            {
                _lastValidAxisNames[item] = axisName;
                return;
            }

            var isDuplicate = AxisItems
                .Where(x => !ReferenceEquals(x, item))
                .Any(x => string.Equals(x.GetNormalizedAxisName(), axisName, StringComparison.OrdinalIgnoreCase));

            if (!isDuplicate)
            {
                _lastValidAxisNames[item] = axisName;
                return;
            }

            _isResolvingDuplicate = true;
            try
            {
                item.RestoreAxisName(_lastValidAxisNames.TryGetValue(item, out var previousName) ? previousName : string.Empty);
            }
            finally
            {
                _isResolvingDuplicate = false;
            }
        }

        private void RemoveLastValidSnapshots(AxisConfigItemViewModel item)
        {
            _lastValidAxisNames.Remove(item);
        }

        private void SetAllChecked(bool isChecked)
        {
            _isUpdatingCheckState = true;
            try
            {
                foreach (var item in AxisItems)
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
            var isAllChecked = AxisItems.Count > 0 && AxisItems.All(x => x.IsChecked);

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
