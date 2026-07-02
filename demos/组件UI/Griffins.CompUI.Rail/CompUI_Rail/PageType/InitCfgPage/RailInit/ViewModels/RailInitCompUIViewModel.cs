using GF_Gereric;
using GKG;
using GKG.MM;
using GKG.UI;
using Griffins.CompUI.Rail.CompUI_Rail.Interop;
using Griffins.Map.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Griffins.CompUI.Rail.CompUI_Rail.PageType.InitCfgPage.RailInit.ViewModels
{
    internal class RailInitCompUIViewModel : ReactiveObject
    {
        private readonly ICompUIRunTimeCallBack _callBack;
        private readonly List<IOStateInformation> _ioStateInformations = new();
        private RailMachineModulesInitCfg _data = new();
        private bool _isApplyingData;

        public ComboxViewModel InputSensorViewModel { get; }
        public ComboxViewModel OutputSensorViewModel { get; }

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
                InputSensorViewModel.IsEnabled = enabled;
                OutputSensorViewModel.IsEnabled = enabled;
            }
        }

        public RailInitCompUIViewModel(ICompUIRunTimeCallBack callBack)
        {
            _callBack = callBack;
            InputSensorViewModel = CreateComboViewModel();
            OutputSensorViewModel = CreateComboViewModel();

            InputSensorViewModel.ValueChanged += (_, __) => OnSensorSelectionChanged(isInput: true);
            OutputSensorViewModel.ValueChanged += (_, __) => OnSensorSelectionChanged(isInput: false);

            LoadIOStateInfos();
            ApplyWithoutModified(() =>
            {
                ApplySensorOptions(InputSensorViewModel, _data.InputSensorID);
                ApplySensorOptions(OutputSensorViewModel, _data.OutputSensorID);
            });

            ReadOnly = false;
        }

        public void SetData(RailMachineModulesInitCfg data)
        {
            ApplyWithoutModified(() =>
            {
                _data = CloneData(data);
                if (_ioStateInformations.Count == 0)
                    LoadIOStateInfos();

                ApplySensorOptions(InputSensorViewModel, _data.InputSensorID);
                ApplySensorOptions(OutputSensorViewModel, _data.OutputSensorID);
            });
        }

        public RailMachineModulesInitCfg GetData()
        {
            return CloneData(_data);
        }

        private void OnSensorSelectionChanged(bool isInput)
        {
            if (_isApplyingData)
                return;

            var selectedGuid = GetSelectedSensorGuid(isInput ? InputSensorViewModel : OutputSensorViewModel);
            if (isInput)
            {
                if (_data.InputSensorID == selectedGuid)
                    return;

                _data.InputSensorID = selectedGuid;
            }
            else
            {
                if (_data.OutputSensorID == selectedGuid)
                    return;

                _data.OutputSensorID = selectedGuid;
            }

            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        private void LoadIOStateInfos()
        {
            _ioStateInformations.Clear();
            if (_callBack == null)
                return;

            try
            {
                var result = ExecGetIOInfosCommand();
                var raw = result?["Result"]?.ToStringVal();
                if (string.IsNullOrWhiteSpace(raw))
                    raw = result?["data"]?.ToStringVal();

                var ioStates = string.IsNullOrWhiteSpace(raw)
                    ? null
                    : JsonObjConvert.FromJSon<List<IOStateInformation>>(raw);

                if (ioStates == null)
                    return;

                foreach (var ioState in ioStates.Where(item => item != null && item.IOGuid != Guid.Empty))
                {
                    var existingIndex = _ioStateInformations.FindIndex(item => item.IOGuid == ioState.IOGuid);
                    if (existingIndex >= 0)
                        _ioStateInformations[existingIndex] = ioState;
                    else
                        _ioStateInformations.Add(ioState);
                }
            }
            catch
            {
            }
        }

        private GFBaseTypeParamValueList ExecGetIOInfosCommand()
        {
            var cmdParam = new GFBaseTypeParamValueList();
            var cmdId = RailInteropConst.CmdGetIOInfos;

            try
            {
                return _callBack.ExecConfigSvrCtlCmd(cmdId, cmdParam)
                    ?? new GFBaseTypeParamValueList();
            }
            catch
            {
                try
                {
                    return _callBack.ExecNormalCtlCmd(cmdId, cmdParam)
                        ?? new GFBaseTypeParamValueList();
                }
                catch
                {
                    return new GFBaseTypeParamValueList();
                }
            }
        }

        private void ApplySensorOptions(ComboxViewModel viewModel, Guid selectedGuid)
        {
            var items = viewModel.ItemsSource as ObservableCollection<ComBoxItem>;
            items?.Clear();
            if (items == null)
                return;

            foreach (var ioState in _ioStateInformations
                .Where(item => item != null && item.IOGuid != Guid.Empty)
                .GroupBy(item => item.IOGuid)
                .Select(group => group.First()))
            {
                items.Add(new ComBoxItem
                {
                    Value = ioState,
                    DisplayName = BuildDisplayName(ioState),
                });
            }

            if (items.Count == 0)
            {
                items.Add(new ComBoxItem
                {
                    Value = null,
                    DisplayName = "等待后端 IO 列表",
                });
            }

            if (selectedGuid != Guid.Empty &&
                items.All(item => (item.Value as IOStateInformation)?.IOGuid != selectedGuid))
            {
                items.Add(new ComBoxItem
                {
                    Value = new IOStateInformation
                    {
                        IOGuid = selectedGuid,
                        IOName = selectedGuid.ToString(),
                        ChannelId = selectedGuid.ToString(),
                    },
                    DisplayName = selectedGuid.ToString(),
                });
            }

            SetSelectedSensor(viewModel, items, selectedGuid);
        }

        private static void SetSelectedSensor(
            ComboxViewModel viewModel,
            IEnumerable<ComBoxItem> items,
            Guid selectedGuid)
        {
            if (selectedGuid == Guid.Empty)
            {
                viewModel.SelectedItem = null;
                return;
            }

            var target = items.FirstOrDefault(item => (item.Value as IOStateInformation)?.IOGuid == selectedGuid);
            if (target != null)
                viewModel.SelectedItem = target;
        }

        private static Guid GetSelectedSensorGuid(ComboxViewModel viewModel)
            => ((viewModel.SelectedItem as ComBoxItem)?.Value as IOStateInformation)?.IOGuid ?? Guid.Empty;

        private static string BuildDisplayName(IOStateInformation ioState)
        {
            if (ioState == null)
                return string.Empty;

            if (!string.IsNullOrWhiteSpace(ioState.IOName))
                return ioState.IOName;

            if (!string.IsNullOrWhiteSpace(ioState.ChannelId))
                return ioState.ChannelId;

            return ioState.IOGuid.ToString();
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

        private static RailMachineModulesInitCfg CloneData(RailMachineModulesInitCfg data)
        {
            if (data == null)
                return new RailMachineModulesInitCfg();

            return JsonObjConvert.FromJSonBytes<RailMachineModulesInitCfg>(JsonObjConvert.ToJSonBytes(data))
                ?? new RailMachineModulesInitCfg();
        }
    }
}
