using GF_Gereric;
using GKG;
using GKG.MM;
using GKG.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Griffins.CompUI.Rail.CompUI_Rail.PageType.RecipeCfgPage.RailRecipe.ViewModels
{
    internal class RailRecipeCompUIViewModel : ReactiveObject
    {
        private static readonly (ERailWorkMode Mode, string DisplayName)[] TransferDirectionOptions =
        {
            (ERailWorkMode.LeftInRightOut, "左进右出"),
            (ERailWorkMode.RightInLeftOut, "右进左出"),
            (ERailWorkMode.LeftInLeftOut, "左进左出"),
            (ERailWorkMode.RightInRightOut, "右进右出"),
        };

        private static readonly (EInletPanelWorkStation Station, string DisplayName)[] InletWorkStationOptions =
        {
            (EInletPanelWorkStation.StandBy, "进板到待料位"),
            (EInletPanelWorkStation.Working, "进板到工作位"),
        };

        private static readonly (EOutletPanelWorkStation Station, string DisplayName)[] OutletWorkStationOptions =
        {
            (EOutletPanelWorkStation.Out, "出板位出板"),
            (EOutletPanelWorkStation.Working, "工作位直接出板"),
        };

        private RailMachineModulesPPCfg _data = new();
        private bool _isApplyingData;

        public ComboxViewModel TransferDirectionViewModel { get; }
        public ComboxViewModel InletPanelWorkStationViewModel { get; }
        public ComboxViewModel OutletPanelWorkStationViewModel { get; }
        public TextInputViewModel OutletPanelDelayTimeViewModel { get; }

        public event EventHandler AfterModified;

        private object _viewTag;
        public object ViewTag
        {
            get => _viewTag;
            set => this.RaiseAndSetIfChanged(ref _viewTag, value);
        }

        private bool _readOnly;
        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                this.RaiseAndSetIfChanged(ref _readOnly, value);
                var enabled = !_readOnly;
                TransferDirectionViewModel.IsEnabled = enabled;
                InletPanelWorkStationViewModel.IsEnabled = enabled;
                OutletPanelWorkStationViewModel.IsEnabled = enabled;
                OutletPanelDelayTimeViewModel.IsEnabled = enabled;
            }
        }

        public RailRecipeCompUIViewModel()
        {
            TransferDirectionViewModel = CreateComboViewModel();
            InletPanelWorkStationViewModel = CreateComboViewModel();
            OutletPanelWorkStationViewModel = CreateComboViewModel();
            OutletPanelDelayTimeViewModel = new TextInputViewModel();

            TransferDirectionViewModel.ValueChanged += (_, __) => OnEnumSelectionChanged(UpdateTransferDirection);
            InletPanelWorkStationViewModel.ValueChanged += (_, __) => OnEnumSelectionChanged(UpdateInletWorkStation);
            OutletPanelWorkStationViewModel.ValueChanged += (_, __) => OnEnumSelectionChanged(UpdateOutletWorkStation);
            OutletPanelDelayTimeViewModel.ValueChanged += (_, __) => OnDelayTimeChanged();

            ApplyWithoutModified(RefreshAllOptions);
            ReadOnly = false;
        }

        public void SetData(RailMachineModulesPPCfg data)
        {
            ApplyWithoutModified(() =>
            {
                _data = CloneData(data);
                RefreshAllOptions();
                OutletPanelDelayTimeViewModel.Text = _data.OutletPanelDelayTime.ToString(CultureInfo.InvariantCulture);
            });
        }

        public RailMachineModulesPPCfg GetData()
        {
            _data.OutletPanelDelayTime = ParseInt(OutletPanelDelayTimeViewModel.Text, _data.OutletPanelDelayTime);
            return CloneData(_data);
        }

        private void RefreshAllOptions()
        {
            ApplyEnumOptions(TransferDirectionViewModel, TransferDirectionOptions, _data.TransferDirection);
            ApplyEnumOptions(InletPanelWorkStationViewModel, InletWorkStationOptions, _data.InletPanelWorkStation);
            ApplyEnumOptions(OutletPanelWorkStationViewModel, OutletWorkStationOptions, _data.OutletPanelWorkStation);
        }

        private void OnEnumSelectionChanged(Action updateAction)
        {
            if (_isApplyingData)
                return;

            updateAction();
            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        private void OnDelayTimeChanged()
        {
            if (_isApplyingData)
                return;

            var newValue = ParseInt(OutletPanelDelayTimeViewModel.Text, _data.OutletPanelDelayTime);
            if (_data.OutletPanelDelayTime == newValue)
                return;

            _data.OutletPanelDelayTime = newValue;
            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateTransferDirection()
        {
            var selected = GetSelectedEnum<ERailWorkMode>(TransferDirectionViewModel);
            if (_data.TransferDirection == selected)
                return;

            _data.TransferDirection = selected;
        }

        private void UpdateInletWorkStation()
        {
            var selected = GetSelectedEnum<EInletPanelWorkStation>(InletPanelWorkStationViewModel);
            if (_data.InletPanelWorkStation == selected)
                return;

            _data.InletPanelWorkStation = selected;
        }

        private void UpdateOutletWorkStation()
        {
            var selected = GetSelectedEnum<EOutletPanelWorkStation>(OutletPanelWorkStationViewModel);
            if (_data.OutletPanelWorkStation == selected)
                return;

            _data.OutletPanelWorkStation = selected;
        }

        private static void ApplyEnumOptions<TEnum>(
            ComboxViewModel viewModel,
            IEnumerable<(TEnum Value, string DisplayName)> options,
            TEnum selectedValue)
            where TEnum : struct, Enum
        {
            var items = viewModel.ItemsSource as ObservableCollection<ComBoxItem>;
            items?.Clear();
            if (items == null)
                return;

            foreach (var option in options)
            {
                items.Add(new ComBoxItem
                {
                    Value = option.Value,
                    DisplayName = option.DisplayName,
                });
            }

            var target = items.FirstOrDefault(item => item.Value is TEnum value && EqualityComparer<TEnum>.Default.Equals(value, selectedValue));
            viewModel.SelectedItem = target ?? items.FirstOrDefault();
        }

        private static TEnum GetSelectedEnum<TEnum>(ComboxViewModel viewModel)
            where TEnum : struct, Enum
        {
            if ((viewModel.SelectedItem as ComBoxItem)?.Value is TEnum value)
                return value;

            return default;
        }

        private static ComboxViewModel CreateComboViewModel()
        {
            return new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                ItemsSource = new ObservableCollection<ComBoxItem>(),
            };
        }

        private void ApplyWithoutModified(Action action)
        {
            _isApplyingData = true;
            try
            {
                action();
            }
            finally
            {
                _isApplyingData = false;
            }
        }

        private static int ParseInt(string text, int defaultValue)
        {
            return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
                ? value
                : defaultValue;
        }

        private static RailMachineModulesPPCfg CloneData(RailMachineModulesPPCfg data)
        {
            if (data == null)
                return new RailMachineModulesPPCfg();

            return JsonObjConvert.FromJSonBytes<RailMachineModulesPPCfg>(JsonObjConvert.ToJSonBytes(data))
                ?? new RailMachineModulesPPCfg();
        }
    }
}
