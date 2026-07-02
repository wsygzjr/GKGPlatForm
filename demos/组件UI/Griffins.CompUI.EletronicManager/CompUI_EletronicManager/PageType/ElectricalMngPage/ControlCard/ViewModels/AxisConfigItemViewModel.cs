using GKG.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.ControlCard.ViewModels
{
    public class AxisConfigItemViewModel : ReactiveObject
    {
        private const string EmptyDisplayName = "[无]";
        private const string MissingCardDisplayFormat = "[已删除卡]{0}";
        private const string MissingAxisDisplayFormat = "[已失配轴]轴{0}";

        private readonly Func<IEnumerable<ControlCardViewModel>> _cardsProvider;
        private Guid? _selectedControlCardId;
        private Guid? _selectedControlCardCadId;
        private Guid _axisGuid;
        private int? _selectedAxisNo;
        private bool _isChecked;
        private int _sequenceNo;
        private string _bindingModule = string.Empty;

        public ObservableCollection<ComBoxItem> ControlCardOptions { get; } = new();
        public ObservableCollection<ComBoxItem> AxisOptions { get; } = new();
        public ComboxViewModel ControlCardComboxViewModel { get; } = new();
        public ComboxViewModel AxisComboxViewModel { get; } = new();

        public MotionControlFactoryParameterViewModel AxisParameter { get; } = CreateAxisParameter();

        public Guid AxisGuid
        {
            get => _axisGuid;
            set => this.RaiseAndSetIfChanged(ref _axisGuid, value);
        }

        public bool IsChecked
        {
            get => _isChecked;
            set => this.RaiseAndSetIfChanged(ref _isChecked, value);
        }

        public int SequenceNo
        {
            get => _sequenceNo;
            set => this.RaiseAndSetIfChanged(ref _sequenceNo, value);
        }

        public string BindingModule
        {
            get => _bindingModule;
            set
            {
                var normalized = value ?? string.Empty;
                this.RaiseAndSetIfChanged(ref _bindingModule, normalized);
                this.RaisePropertyChanged(nameof(BindingModuleDisplay));
            }
        }

        public string BindingModuleDisplay => string.IsNullOrWhiteSpace(BindingModule) ? "暂无" : BindingModule;

        private object? _selectedControlCardOption;
        public object? SelectedControlCardOption
        {
            get => _selectedControlCardOption;
            set
            {
                if (Equals(_selectedControlCardOption, value))
                    return;

                var previousControlCardId = _selectedControlCardId;
                var previousControlCardCadId = _selectedControlCardCadId;
                _selectedControlCardOption = value;
                var selectedOption = value as ComBoxItem;
                var selectedCard = ResolveCardByOption(selectedOption);
                if (selectedCard != null)
                {
                    _selectedControlCardId = selectedCard.ObjectId;
                    _selectedControlCardCadId = selectedCard.CadID;
                }
                else if (TryResolveMissingCardOption(selectedOption, out var missingCardSelection))
                {
                    _selectedControlCardId = missingCardSelection.ObjectId;
                    _selectedControlCardCadId = missingCardSelection.CadId;
                }
                else
                {
                    _selectedControlCardId = null;
                    _selectedControlCardCadId = null;
                }

                var keepCurrentAxis =
                    (selectedCard != null &&
                    ((previousControlCardId.HasValue && selectedCard.ObjectId == previousControlCardId.Value) ||
                     (previousControlCardCadId.HasValue && selectedCard.CadID == previousControlCardCadId.Value))) ||
                    TryResolveMissingCardOption(selectedOption, out _);
                RefreshAxisOptions(keepCurrentAxis: keepCurrentAxis);
                RefreshAxisMetadata();
                this.RaisePropertyChanged(nameof(SelectedControlCardOption));
            }
        }

        private object? _selectedAxisOption;
        public object? SelectedAxisOption
        {
            get => _selectedAxisOption;
            set
            {
                if (Equals(_selectedAxisOption, value))
                    return;

                _selectedAxisOption = value;
                _selectedAxisNo = (value as ComBoxItem)?.Value is int axisNo ? axisNo : null;
                RefreshAxisMetadata();
                this.RaisePropertyChanged(nameof(SelectedAxisOption));
            }
        }

        public Guid? SelectedControlCardId => _selectedControlCardId;
        public int? SelectedAxisNo => _selectedAxisNo;

        public string AxisNameEditor
        {
            get => AxisParameter.AxisNameEditor;
            set
            {
                AxisParameter.AxisNameEditor = value ?? string.Empty;
                RaiseAxisDisplayProperties();
            }
        }

        public string CardDisplayName =>
            (SelectedControlCardOption as ComBoxItem)?.DisplayName ?? EmptyDisplayName;

        public string AxisDisplayName =>
            (SelectedAxisOption as ComBoxItem)?.DisplayName ?? EmptyDisplayName;

        public bool HasSelectedControlCard => _selectedControlCardId.HasValue;
        public bool HasSelectedAxis => _selectedAxisNo.HasValue;

        public AxisConfigItemViewModel(Func<IEnumerable<ControlCardViewModel>> cardsProvider)
        {
            _cardsProvider = cardsProvider;
            InitializeComboxViewModels();

            AxisParameter.PropertyChanged += (_, __) => RaiseAxisDisplayProperties();
            RefreshControlCardOptions(null);
            RefreshAxisOptions(null, keepCurrentAxis: true);
            RefreshAxisMetadata();
        }

        public void InitializeEmpty()
        {
            AxisGuid = Guid.NewGuid();
            _selectedControlCardId = null;
            _selectedControlCardCadId = null;
            _selectedAxisNo = null;
            BindingModule = string.Empty;
            RefreshControlCardOptions(null);
            RefreshAxisOptions(null, keepCurrentAxis: true);
            SelectNoControlCardAndNoAxis();
            RefreshAxisMetadata();
        }

        public void Initialize(ControlCardViewModel cardVm, int axisNo)
        {
            if (AxisGuid == Guid.Empty)
                AxisGuid = Guid.NewGuid();
            RefreshControlCardOptions(cardVm.ObjectId);
            RefreshAxisOptions(axisNo, keepCurrentAxis: true);
            RefreshAxisMetadata();
        }

        public void RefreshFromCards()
        {
            var currentControlCardId = _selectedControlCardId;
            var currentAxisNo = _selectedAxisNo;
            RefreshControlCardOptions(currentControlCardId);
            RefreshAxisOptions(currentAxisNo, keepCurrentAxis: true);
            RefreshAxisMetadata();
        }

        public void Load(GKG.AxisInformation axisInformation, GKG.MotionControlFactoryParameter? parameter = null)
        {
            AxisGuid = axisInformation.AxisGuid == Guid.Empty ? Guid.NewGuid() : axisInformation.AxisGuid;
            _selectedControlCardCadId = axisInformation.MotionCardGuid;
            var preferredCard = _cardsProvider().FirstOrDefault(x => x.CadID == axisInformation.MotionCardGuid);
            RefreshControlCardOptions(preferredCard?.ObjectId);
            RefreshAxisOptions(axisInformation.AxisNo, keepCurrentAxis: false);

            if (parameter != null)
                AxisParameter.CopyFrom(parameter);

            AxisParameter.AxisName = axisInformation.AxisName ?? string.Empty;
            AxisParameter.AxisNameViewModel.Text = AxisParameter.AxisName;
            BindingModule = axisInformation.BindingModule ?? string.Empty;
            RefreshAxisMetadata();
        }

        public GKG.AxisInformation? ToBackendAxisInformation()
        {
            if (AxisGuid == Guid.Empty)
                AxisGuid = Guid.NewGuid();

            var selectedCard = GetSelectedCard();
            var selectedCardGuid = selectedCard?.CadID ?? _selectedControlCardCadId;
            if (!selectedCardGuid.HasValue || !_selectedAxisNo.HasValue)
                return null;

            return new GKG.AxisInformation
            {
                AxisGuid = AxisGuid,
                MotionCardGuid = selectedCardGuid.Value,
                AxisNo = _selectedAxisNo.Value,
                AxisName = AxisParameter.AxisName,
                BindingModule = BindingModule,
            };
        }

        public GKG.MotionControlFactoryParameter? ToBackendMotionControlFactoryParameter()
        {
            return _selectedAxisNo.HasValue
                ? AxisParameter.ToModel()
                : null;
        }

        public string GetNormalizedAxisName()
        {
            return AxisNameEditor?.Trim() ?? string.Empty;
        }

        public void RestoreAxisName(string axisName)
        {
            AxisNameEditor = axisName ?? string.Empty;
        }

        public void RestoreCardAndAxis(Guid? controlCardId, int? axisNo)
        {
            RefreshControlCardOptions(controlCardId);
            RefreshAxisOptions(axisNo, keepCurrentAxis: false);
            RefreshAxisMetadata();
        }

        public void SelectNoControlCardAndNoAxis()
        {
            static bool IsNoneItem(ComBoxItem? x) =>
                x != null && (x.Value == null || x.Value is string s && s.Length == 0);

            if (ControlCardComboxViewModel.ItemsSource is IEnumerable<ComBoxItem> cardList)
            {
                var noneCard = cardList.FirstOrDefault(IsNoneItem) ?? cardList.FirstOrDefault();
                if (!ReferenceEquals(ControlCardComboxViewModel.SelectedItem, noneCard))
                    ControlCardComboxViewModel.SelectedItem = noneCard;
            }

            if (AxisComboxViewModel.ItemsSource is IEnumerable<ComBoxItem> axisList)
            {
                var noneAxis = axisList.FirstOrDefault(IsNoneItem) ?? axisList.FirstOrDefault();
                if (!ReferenceEquals(AxisComboxViewModel.SelectedItem, noneAxis))
                    AxisComboxViewModel.SelectedItem = noneAxis;
            }
        }

        private void RefreshControlCardOptions(Guid? preferredCardId)
        {
            ControlCardOptions.Clear();
            ControlCardOptions.Add(new ComBoxItem { Value = string.Empty, DisplayName = EmptyDisplayName });

            foreach (var card in _cardsProvider())
            {
                ControlCardOptions.Add(new ComBoxItem
                {
                    Value = card.ObjectId,
                    DisplayName = string.IsNullOrWhiteSpace(card.MotionCardName) ? card.MotionCardCustomID : card.MotionCardName,
                });
            }

            var resolvedPreferredCardId = ResolvePreferredControlCardId(preferredCardId);
            if (!resolvedPreferredCardId.HasValue &&
                _selectedControlCardCadId.HasValue &&
                _selectedControlCardId.HasValue)
            {
                ControlCardOptions.Add(new ComBoxItem
                {
                    Value = new MissingControlCardSelection(_selectedControlCardId.Value, _selectedControlCardCadId.Value),
                    DisplayName = string.Format(MissingCardDisplayFormat, _selectedControlCardCadId.Value.ToString("N")[..8]),
                });
            }

            SelectedControlCardOption = ControlCardOptions.FirstOrDefault(x => Equals(x.Value, resolvedPreferredCardId))
                ?? ControlCardOptions.FirstOrDefault(x => IsMissingCardOptionForCurrentSelection(x))
                ?? ControlCardOptions.FirstOrDefault(x => x.Value is string s && s.Length == 0)
                ?? ControlCardOptions.FirstOrDefault();
        }

        private void RefreshAxisOptions(int? preferredAxisNo = null, bool keepCurrentAxis = true)
        {
            AxisOptions.Clear();
            AxisOptions.Add(new ComBoxItem { Value = string.Empty, DisplayName = EmptyDisplayName });

            var selectedCard = GetSelectedCard();
            var axisCount = selectedCard?.AxisCount ?? 0;
            for (var i = 0; i < axisCount; i++)
            {
                AxisOptions.Add(new ComBoxItem
                {
                    Value = i,
                    DisplayName = $"轴{i}",
                });
            }

            var targetAxisNo = keepCurrentAxis ? (_selectedAxisNo ?? preferredAxisNo) : preferredAxisNo;
            if (axisCount == 0 && targetAxisNo.HasValue)
            {
                AxisOptions.Add(new ComBoxItem
                {
                    Value = targetAxisNo.Value,
                    DisplayName = string.Format(MissingAxisDisplayFormat, targetAxisNo.Value),
                });
            }

            SelectedAxisOption = AxisOptions.FirstOrDefault(x => Equals(x.Value, targetAxisNo))
                ?? AxisOptions.FirstOrDefault(x => x.Value is string s && s.Length == 0)
                ?? AxisOptions.FirstOrDefault();
        }

        private void InitializeComboxViewModels()
        {
            ControlCardComboxViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            ControlCardComboxViewModel.ItemsSource = ControlCardOptions;
            ControlCardComboxViewModel.ValueChanged += (_, __) =>
            {
                var selectedItem = ControlCardComboxViewModel.SelectedItem;
                if (!Equals(SelectedControlCardOption, selectedItem))
                    SelectedControlCardOption = selectedItem;
            };

            AxisComboxViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            AxisComboxViewModel.ItemsSource = AxisOptions;
            AxisComboxViewModel.ValueChanged += (_, __) =>
            {
                var selectedItem = AxisComboxViewModel.SelectedItem;
                if (!Equals(SelectedAxisOption, selectedItem))
                    SelectedAxisOption = selectedItem;
            };
        }

        private void RefreshAxisMetadata()
        {
            AxisParameter.CardDisplayName = CardDisplayName;
            AxisParameter.AxisNo = _selectedAxisNo ?? 0;
            if (!_selectedControlCardCadId.HasValue || !_selectedAxisNo.HasValue)
            {
                BindingModule = string.Empty;
            }

            RaiseAxisDisplayProperties();
        }

        private ControlCardViewModel? GetSelectedCard()
        {
            var cards = _cardsProvider().ToList();
            var selectedCard = cards.FirstOrDefault(x => x.ObjectId == _selectedControlCardId);
            if (selectedCard != null)
                return selectedCard;

            if (_selectedControlCardCadId.HasValue)
                selectedCard = cards.FirstOrDefault(x => x.CadID == _selectedControlCardCadId.Value);

            if (selectedCard != null)
            {
                _selectedControlCardId = selectedCard.ObjectId;
                _selectedControlCardCadId = selectedCard.CadID;
            }

            return selectedCard;
        }

        private Guid? ResolvePreferredControlCardId(Guid? preferredCardId)
        {
            if (preferredCardId.HasValue && _cardsProvider().Any(x => x.ObjectId == preferredCardId.Value))
                return preferredCardId.Value;

            if (_selectedControlCardId.HasValue && _cardsProvider().Any(x => x.ObjectId == _selectedControlCardId.Value))
                return _selectedControlCardId.Value;

            if (_selectedControlCardCadId.HasValue)
            {
                var selectedCard = _cardsProvider().FirstOrDefault(x => x.CadID == _selectedControlCardCadId.Value);
                if (selectedCard != null)
                    return selectedCard.ObjectId;
            }

            return null;
        }

        private ControlCardViewModel? ResolveCardByOption(ComBoxItem? option)
        {
            if (option?.Value is not Guid selectedObjectId)
                return null;

            return _cardsProvider().FirstOrDefault(x => x.ObjectId == selectedObjectId);
        }

        private bool IsMissingCardOptionForCurrentSelection(ComBoxItem item)
        {
            return item.Value is MissingControlCardSelection missing &&
                   _selectedControlCardId.HasValue &&
                   _selectedControlCardCadId.HasValue &&
                   missing.ObjectId == _selectedControlCardId.Value &&
                   missing.CadId == _selectedControlCardCadId.Value;
        }

        private static bool TryResolveMissingCardOption(ComBoxItem? option, out MissingControlCardSelection missing)
        {
            if (option?.Value is MissingControlCardSelection selection)
            {
                missing = selection;
                return true;
            }

            missing = default;
            return false;
        }

        private void RaiseAxisDisplayProperties()
        {
            if (!ReferenceEquals(ControlCardComboxViewModel.SelectedItem, _selectedControlCardOption) && _selectedControlCardOption != null)
                ControlCardComboxViewModel.SelectedItem = _selectedControlCardOption;

            if (!ReferenceEquals(AxisComboxViewModel.SelectedItem, _selectedAxisOption) && _selectedAxisOption != null)
                AxisComboxViewModel.SelectedItem = _selectedAxisOption;

            this.RaisePropertyChanged(nameof(AxisNameEditor));
            this.RaisePropertyChanged(nameof(CardDisplayName));
            this.RaisePropertyChanged(nameof(AxisDisplayName));
            this.RaisePropertyChanged(nameof(BindingModuleDisplay));
            this.RaisePropertyChanged(nameof(HasSelectedControlCard));
            this.RaisePropertyChanged(nameof(HasSelectedAxis));
            this.RaisePropertyChanged(nameof(AxisParameter));
        }

        private static MotionControlFactoryParameterViewModel CreateAxisParameter()
        {
            var parameter = new MotionControlFactoryParameterViewModel
            {
                AxisNo = 0,
                CardDisplayName = EmptyDisplayName,
                HomingDirection = GKG.MotionControlAxisHomingDirectionConstants.NegativeDirection,
                PulseOutputMode = GKG.MotionControlPulseOutputModeConstants.Pulse,
                PulseInputMode = GKG.MotionControlPulseInputMode.PulseDirection,
                EncoderType = GKG.EncoderType.None,
            };

            parameter.AxisNameEditor = string.Empty;
            return parameter;
        }

        private readonly record struct MissingControlCardSelection(Guid ObjectId, Guid CadId);
    }
}
