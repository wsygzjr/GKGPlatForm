using GF_Gereric;
using GKG;
using GKG.SubMM;
using GKG.SubMM.StorageDeviceModule;
using GKG.SubMM.TransportMechanismModule;
using GKG.UI;
using GKG.UI.General;
using Griffins.CompUI.MaterialBox.CompUI_MaterialBox;
using Griffins.Map.UI;
using MaterialBoxSubMachineModules.FeedPort;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using BackendMaterialBoxInitCfg = GKG.SubMM.MaterialBoxSubMachineModulesInitCfg;
using UICylinderType = GKG.UI.General.ECylinderType;

namespace Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.InitCfgPage.MaterialBoxInitConfig.ViewModels
{
    internal class MaterialBoxInitConfigCompUIViewModel : ReactiveObject
    {
        private const string RunTimeCtlGetAxisInfos = MaterialBoxSubMachineModulesConst.RtCmdGetAxisInfos;
        private const string RunTimeCtlGetIOStateInfos = MaterialBoxSubMachineModulesConst.RtCmdGetIOInfos;

        private readonly List<ComBoxItem> _loadAxisItems = new();
        private readonly List<ComBoxItem> _unloadAxisItems = new();
        private readonly List<AxisInformation> _axisInformations = new();
        private readonly List<IOStateInformation> _ioStateInformations = new();
        private readonly List<ComBoxItem> _loadUpperSensorItems = new();
        private readonly List<ComBoxItem> _loadLowerSensorItems = new();
        private readonly List<ComBoxItem> _unloadUpperSensorItems = new();
        private readonly List<ComBoxItem> _unloadLowerSensorItems = new();

        private BackendMaterialBoxInitCfg _loadedData = new();
        private bool _isApplyingData;
        private bool _readOnly;
        private object _viewTag;
        public ComboxViewModel LoadAxisViewModel { get; }
        public ComboxViewModel UnloadAxisViewModel { get; }
        public ComboxViewModel LoadUpperSensorViewModel { get; }
        public ComboxViewModel LoadLowerSensorViewModel { get; }
        public ComboxViewModel UnloadUpperSensorViewModel { get; }
        public ComboxViewModel UnloadLowerSensorViewModel { get; }

        public ObservableCollection<MaterialBoxPortBindingRowViewModel> FeedingPortRows { get; } = new();
        public ObservableCollection<MaterialBoxPortBindingRowViewModel> ReceivePortRows { get; } = new();

        public TextInputViewModel LoadStartSpeedViewModel { get; }
        public TextInputViewModel LoadMaxSpeedViewModel { get; }
        public TextInputViewModel LoadAccelerationViewModel { get; }
        public TextInputViewModel LoadDecelerationViewModel { get; }

        public TextInputViewModel UnloadStartSpeedViewModel { get; }
        public TextInputViewModel UnloadMaxSpeedViewModel { get; }
        public TextInputViewModel UnloadAccelerationViewModel { get; }
        public TextInputViewModel UnloadDecelerationViewModel { get; }

        public CylinderConfigViewModel LoadUpperCylinderViewModel { get; }
        public CylinderConfigViewModel LoadLowerCylinderViewModel { get; }
        public CylinderConfigViewModel UnloadUpperCylinderViewModel { get; }
        public CylinderConfigViewModel UnloadLowerCylinderViewModel { get; }

        public event EventHandler AfterModified;

        public object ViewTag
        {
            get => _viewTag;
            set => this.RaiseAndSetIfChanged(ref _viewTag, value);
        }

        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                this.RaiseAndSetIfChanged(ref _readOnly, value);
                ApplyReadOnly();
            }
        }

        public bool CanEditCylinderConfig => !_readOnly;

        public MaterialBoxInitConfigCompUIViewModel(ICompUIRunTimeCallBack callBack)
        {
            LoadAxisViewModel = CreateComboxViewModel(_loadAxisItems);
            UnloadAxisViewModel = CreateComboxViewModel(_unloadAxisItems);
            LoadUpperSensorViewModel = CreateComboxViewModel(_loadUpperSensorItems);
            LoadLowerSensorViewModel = CreateComboxViewModel(_loadLowerSensorItems);
            UnloadUpperSensorViewModel = CreateComboxViewModel(_unloadUpperSensorItems);
            UnloadLowerSensorViewModel = CreateComboxViewModel(_unloadLowerSensorItems);

            LoadStartSpeedViewModel = CreateTextInput();
            LoadMaxSpeedViewModel = CreateTextInput();
            LoadAccelerationViewModel = CreateTextInput();
            LoadDecelerationViewModel = CreateTextInput();
            UnloadStartSpeedViewModel = CreateTextInput();
            UnloadMaxSpeedViewModel = CreateTextInput();
            UnloadAccelerationViewModel = CreateTextInput();
            UnloadDecelerationViewModel = CreateTextInput();

            LoadUpperCylinderViewModel = CreateCylinderViewModel();
            LoadLowerCylinderViewModel = CreateCylinderViewModel();
            UnloadUpperCylinderViewModel = CreateCylinderViewModel();
            UnloadLowerCylinderViewModel = CreateCylinderViewModel();

            SubscribeModified(LoadAxisViewModel);
            SubscribeModified(UnloadAxisViewModel);
            SubscribeModified(LoadUpperSensorViewModel);
            SubscribeModified(LoadLowerSensorViewModel);
            SubscribeModified(UnloadUpperSensorViewModel);
            SubscribeModified(UnloadLowerSensorViewModel);

            MaterialBoxSharedState.FactoryCfgChanged += OnFactoryCfgChanged;

            LoadAxisInfos(callBack);
            LoadIOStateInfos(callBack);
            
            ReadOnly = false;
        }

        public void SetAxisOptions(IEnumerable<AxisInformation> loadAxisOptions, IEnumerable<AxisInformation> unloadAxisOptions)
        {
            ApplyAxisOptions(_loadAxisItems, LoadAxisViewModel, loadAxisOptions, GetTransportBindingAxisId(_loadedData?.LoadTransportMechanism, true));
            ApplyAxisOptions(_unloadAxisItems, UnloadAxisViewModel, unloadAxisOptions, GetTransportBindingAxisId(_loadedData?.UnloadTransportMechanism, false));
        }

        public void SetCylinderIOChannelOptions(IEnumerable<IOStateInformation> channelOptions)
        {
            var ioStates = (channelOptions ?? Enumerable.Empty<IOStateInformation>())
                .Where(x => x != null && x.IOGuid != Guid.Empty)
                .GroupBy(x => x.IOGuid)
                .Select(x => x.First())
                .ToList();

            LoadUpperCylinderViewModel.SetIOChannelOptions(ioStates);
            LoadLowerCylinderViewModel.SetIOChannelOptions(ioStates);
            UnloadUpperCylinderViewModel.SetIOChannelOptions(ioStates);
            UnloadLowerCylinderViewModel.SetIOChannelOptions(ioStates);
        }

        public void SetSensorIOOptions(IEnumerable<IOStateInformation> ioStates)
        {
            ApplySensorOptions(_loadUpperSensorItems, LoadUpperSensorViewModel, ioStates, GetSensorGuid(_loadedData?.LoadStorageDevice?.StorageMechanism[0]?.SenserIOGuids, 0));
            ApplySensorOptions(_loadLowerSensorItems, LoadLowerSensorViewModel, ioStates, GetSensorGuid(_loadedData?.LoadStorageDevice?.StorageMechanism[1]?.SenserIOGuids, 0));
            ApplySensorOptions(_unloadUpperSensorItems, UnloadUpperSensorViewModel, ioStates, GetSensorGuid(_loadedData?.UnloadStorageDevice?.StorageMechanism[0]?.SenserIOGuids, 0));
            ApplySensorOptions(_unloadLowerSensorItems, UnloadLowerSensorViewModel, ioStates, GetSensorGuid(_loadedData?.UnloadStorageDevice?.StorageMechanism[1]?.SenserIOGuids, 0));
            RefreshPortBindingRows();
        }

        public void SetData(BackendMaterialBoxInitCfg data)
        {
            ApplyWithoutModified(() =>
            {
                _loadedData.CopyFrom(CloneData(data));
                EnsureDataShape(_loadedData);

                SetAxisOptions(
                    BuildCurrentAxisOptions(GetTransportBindingAxisId(_loadedData.LoadTransportMechanism, true), null),
                    BuildCurrentAxisOptions(GetTransportBindingAxisId(_loadedData.UnloadTransportMechanism, false), null));
                SetSensorIOOptions(BuildCurrentIOStateOptions());
                SetCylinderIOChannelOptions(BuildCurrentIOStateOptions());

                SetSelectedAxis(LoadAxisViewModel, _loadAxisItems, GetTransportBindingAxisId(_loadedData.LoadTransportMechanism, true));
                SetSelectedAxis(UnloadAxisViewModel, _unloadAxisItems, GetTransportBindingAxisId(_loadedData.UnloadTransportMechanism, false));

                FillTrajectory(_loadedData.LoadTransportMechanism.AxisMotionParameters, LoadStartSpeedViewModel, LoadMaxSpeedViewModel, LoadAccelerationViewModel, LoadDecelerationViewModel);
                FillTrajectory(_loadedData.UnloadTransportMechanism.AxisMotionParameters, UnloadStartSpeedViewModel, UnloadMaxSpeedViewModel, UnloadAccelerationViewModel, UnloadDecelerationViewModel);

                LoadUpperCylinderViewModel.CopyFrom(ToCylinderConfig(GetCylinder(_loadedData.LoadStorageDevice.StorageMechanism[0].CylinderInitParameters, 0)));
                LoadLowerCylinderViewModel.CopyFrom(ToCylinderConfig(GetCylinder(_loadedData.LoadStorageDevice.StorageMechanism[1].CylinderInitParameters, 0)));
                UnloadUpperCylinderViewModel.CopyFrom(ToCylinderConfig(GetCylinder(_loadedData.UnloadStorageDevice.StorageMechanism[0].CylinderInitParameters, 0)));
                UnloadLowerCylinderViewModel.CopyFrom(ToCylinderConfig(GetCylinder(_loadedData.UnloadStorageDevice.StorageMechanism[1].CylinderInitParameters, 0)));
                RefreshPortBindingRows();
            });
        }

        public BackendMaterialBoxInitCfg GetData()
        {
            var result = new BackendMaterialBoxInitCfg();
            result.CopyFrom(CloneData(_loadedData));
            EnsureDataShape(result);

            SetTransportBindingAxisId(result.LoadTransportMechanism, GetSelectedAxisGuid(LoadAxisViewModel), true);
            SetTransportBindingAxisId(result.UnloadTransportMechanism, GetSelectedAxisGuid(UnloadAxisViewModel), false);

            result.LoadTransportMechanism.AxisMotionParameters = BuildTrajectory(LoadStartSpeedViewModel, LoadMaxSpeedViewModel, LoadAccelerationViewModel, LoadDecelerationViewModel, result.LoadTransportMechanism.AxisMotionParameters);
            result.UnloadTransportMechanism.AxisMotionParameters = BuildTrajectory(UnloadStartSpeedViewModel, UnloadMaxSpeedViewModel, UnloadAccelerationViewModel, UnloadDecelerationViewModel, result.UnloadTransportMechanism.AxisMotionParameters);

            EnsureGuidListSize(result.LoadStorageDevice.StorageMechanism[0].SenserIOGuids, 1);
            EnsureGuidListSize(result.LoadStorageDevice.StorageMechanism[1].SenserIOGuids, 1);
            EnsureGuidListSize(result.UnloadStorageDevice.StorageMechanism[0].SenserIOGuids, 1);
            EnsureGuidListSize(result.UnloadStorageDevice.StorageMechanism[1].SenserIOGuids, 1);
            EnsureCylinderListSize(result.LoadStorageDevice.StorageMechanism[0].CylinderInitParameters, 1);
            EnsureCylinderListSize(result.LoadStorageDevice.StorageMechanism[1].CylinderInitParameters, 1);
            EnsureCylinderListSize(result.UnloadStorageDevice.StorageMechanism[0].CylinderInitParameters, 1);
            EnsureCylinderListSize(result.UnloadStorageDevice.StorageMechanism[1].CylinderInitParameters, 1);

            result.LoadStorageDevice.StorageMechanism[0].SenserIOGuids[0] = GetSelectedSensorGuid(LoadUpperSensorViewModel);
            result.LoadStorageDevice.StorageMechanism[1].SenserIOGuids[0] = GetSelectedSensorGuid(LoadLowerSensorViewModel);
            result.UnloadStorageDevice.StorageMechanism[0].SenserIOGuids[0] = GetSelectedSensorGuid(UnloadUpperSensorViewModel);
            result.UnloadStorageDevice.StorageMechanism[1].SenserIOGuids[0] = GetSelectedSensorGuid(UnloadLowerSensorViewModel);

            result.LoadStorageDevice.StorageMechanism[0].CylinderInitParameters[0] = ToCylinderModel(LoadUpperCylinderViewModel);
            result.LoadStorageDevice.StorageMechanism[1].CylinderInitParameters[0] = ToCylinderModel(LoadLowerCylinderViewModel);
            result.UnloadStorageDevice.StorageMechanism[0].CylinderInitParameters[0] = ToCylinderModel(UnloadUpperCylinderViewModel);
            result.UnloadStorageDevice.StorageMechanism[1].CylinderInitParameters[0] = ToCylinderModel(UnloadLowerCylinderViewModel);

            result.FeedingPortInitCfg.SensorIOGuids = CapturePortBindingGuids(FeedingPortRows);
            result.ReceivePortInitCfg.SensorIOGuids = CapturePortBindingGuids(ReceivePortRows);

            _loadedData.CopyFrom(CloneData(result));
            return result;
        }

        private void ApplyReadOnly()
        {
            var isEnabled = !_readOnly;
            LoadAxisViewModel.IsEnabled = isEnabled;
            UnloadAxisViewModel.IsEnabled = isEnabled;
            LoadUpperSensorViewModel.IsEnabled = isEnabled;
            LoadLowerSensorViewModel.IsEnabled = isEnabled;
            UnloadUpperSensorViewModel.IsEnabled = isEnabled;
            UnloadLowerSensorViewModel.IsEnabled = isEnabled;

            foreach (var viewModel in new[]
            {
                LoadStartSpeedViewModel, LoadMaxSpeedViewModel, LoadAccelerationViewModel, LoadDecelerationViewModel,
                UnloadStartSpeedViewModel, UnloadMaxSpeedViewModel, UnloadAccelerationViewModel, UnloadDecelerationViewModel,
            })
            {
                viewModel.IsEnabled = isEnabled;
            }

            foreach (var row in FeedingPortRows.Concat(ReceivePortRows))
            {
                row.SetEnabled(isEnabled);
            }

            this.RaisePropertyChanged(nameof(CanEditCylinderConfig));
        }

        private static ComboxViewModel CreateComboxViewModel(List<ComBoxItem> items)
        {
            return new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                ItemsSource = items,
            };
        }

        private TextInputViewModel CreateTextInput()
        {
            var viewModel = new TextInputViewModel();
            viewModel.ValueChanged += (_, __) => RaiseAfterModified();
            return viewModel;
        }

        private CylinderConfigViewModel CreateCylinderViewModel()
        {
            var viewModel = new CylinderConfigViewModel();
            viewModel.AfterModified += (_, __) => RaiseAfterModified();
            return viewModel;
        }

        private void SubscribeModified(ComboxViewModel viewModel)
        {
            viewModel.ValueChanged += (_, __) => RaiseAfterModified();
        }

        private void LoadAxisInfos(ICompUIRunTimeCallBack callBack)
        {
            _axisInformations.Clear();
            try
            {
                var result = callBack?.ExecConfigSvrCtlCmd(RunTimeCtlGetAxisInfos, new GFBaseTypeParamValueList());
                var raw = result?["data"]?.ToStringVal();
                if (string.IsNullOrWhiteSpace(raw))
                {
                    raw = result?["Result"]?.ToStringVal();
                }

                var axisInfos = string.IsNullOrWhiteSpace(raw)
                    ? null
                    : JsonObjConvert.FromJSon<List<AxisInformation>>(raw);

                if (axisInfos != null)
                {
                    _axisInformations.AddRange(axisInfos.Where(item => item != null && item.AxisGuid != Guid.Empty));
                }
            }
            catch
            {
            }
        }

        private void LoadIOStateInfos(ICompUIRunTimeCallBack callBack)
        {
            _ioStateInformations.Clear();
            try
            {
                var result = callBack?.ExecConfigSvrCtlCmd(RunTimeCtlGetIOStateInfos, new GFBaseTypeParamValueList());
                var raw = result?["data"]?.ToStringVal();
                if (string.IsNullOrWhiteSpace(raw))
                {
                    raw = result?["Result"]?.ToStringVal();
                }

                var ioInfos = string.IsNullOrWhiteSpace(raw)
                    ? null
                    : JsonObjConvert.FromJSon<List<IOStateInformation>>(raw);

                if (ioInfos != null)
                {
                    _ioStateInformations.AddRange(ioInfos.Where(item => item != null && item.IOGuid != Guid.Empty));
                }
            }
            catch
            {
            }
        }

        private void ApplyAxisOptions(List<ComBoxItem> items, ComboxViewModel viewModel, IEnumerable<AxisInformation> axisOptions, Guid selectedAxisGuid)
        {
            items.Clear();

            foreach (var axisInfo in (axisOptions ?? Enumerable.Empty<AxisInformation>())
                .Where(item => item != null && item.AxisGuid != Guid.Empty)
                .GroupBy(item => item.AxisGuid)
                .Select(group => group.First()))
            {
                items.Add(new ComBoxItem
                {
                    Value = axisInfo,
                    DisplayName = string.IsNullOrWhiteSpace(axisInfo.AxisName) ? axisInfo.AxisGuid.ToString() : axisInfo.AxisName,
                });
            }

            if (selectedAxisGuid != Guid.Empty &&
                items.All(item => (item.Value as AxisInformation)?.AxisGuid != selectedAxisGuid))
            {
                items.Add(new ComBoxItem
                {
                    Value = new AxisInformation
                    {
                        AxisGuid = selectedAxisGuid,
                        AxisName = selectedAxisGuid.ToString(),
                    },
                    DisplayName = selectedAxisGuid.ToString(),
                });
            }

            if (items.Count == 0)
            {
                items.Add(new ComBoxItem
                {
                    Value = null,
                    DisplayName = "等待后端轴列表",
                });
            }

            viewModel.SelectedItem = items[0];
            SetSelectedAxis(viewModel, items, selectedAxisGuid);
        }

        private void ApplySensorOptions(List<ComBoxItem> items, ComboxViewModel viewModel, IEnumerable<IOStateInformation> ioStates, Guid selectedIOGuid)
        {
            items.Clear();

            foreach (var ioState in (ioStates ?? Enumerable.Empty<IOStateInformation>())
                .Where(item => item != null && item.IOGuid != Guid.Empty)
                .GroupBy(item => item.IOGuid)
                .Select(group => group.First()))
            {
                items.Add(new ComBoxItem
                {
                    Value = ioState,
                    DisplayName = BuildIOStateDisplayName(ioState),
                });
            }

            if (selectedIOGuid != Guid.Empty &&
                items.All(item => (item.Value as IOStateInformation)?.IOGuid != selectedIOGuid))
            {
                items.Add(new ComBoxItem
                {
                    Value = new IOStateInformation
                    {
                        IOGuid = selectedIOGuid,
                        IOName = selectedIOGuid.ToString(),
                    },
                    DisplayName = selectedIOGuid.ToString(),
                });
            }

            if (items.Count == 0)
            {
                items.Add(new ComBoxItem
                {
                    Value = null,
                    DisplayName = "等待后端IO列表",
                });
            }

            viewModel.SelectedItem = items[0];
            SetSelectedSensor(viewModel, items, selectedIOGuid);
        }

        private void RefreshPortBindingRows()
        {
            var factoryCfg = MaterialBoxSharedState.FactoryCfg;
            var feedingCount = factoryCfg?.FeedingPortFactoryCfg?.PortCount ?? 0;
            var receiveCount = factoryCfg?.ReceivePortFactoryCfg?.PortCount ?? 0;

            RebuildPortBindingRows(FeedingPortRows, _loadedData?.FeedingPortInitCfg?.SensorIOGuids, feedingCount);
            RebuildPortBindingRows(ReceivePortRows, _loadedData?.ReceivePortInitCfg?.SensorIOGuids, receiveCount);
        }

        private List<Guid> RebuildPortBindingRows(
            ObservableCollection<MaterialBoxPortBindingRowViewModel> rows,
            List<Guid> savedBindings,
            int count)
        {
            var rowBindings = rows.Count > 0 ? CapturePortBindingGuids(rows) : new List<Guid>();
            var preservedBindings = rowBindings.Any(x => x != Guid.Empty)
                ? rowBindings
                : (savedBindings ?? new List<Guid>());
            var targetCount = count > 0 ? count : preservedBindings.Count;
            var ioOptions = BuildCurrentIOStateOptions();

            rows.Clear();
            for (var i = 0; i < targetCount; i++)
            {
                var portName = $"料口{i + 1}";
                var row = new MaterialBoxPortBindingRowViewModel(portName, !_readOnly);
                row.SetOptions(ioOptions, i < preservedBindings.Count ? preservedBindings[i] : Guid.Empty);
                row.AfterModified += (_, __) => RaiseAfterModified();
                rows.Add(row);
            }

            return CapturePortBindingGuids(rows);
        }

        private void OnFactoryCfgChanged(object sender, EventArgs e)
        {
            RefreshPortBindingRows();
            RaiseAfterModified();
        }

        private void ApplyWithoutModified(Action action)
        {
            _isApplyingData = true;
            try
            {
                action?.Invoke();
            }
            finally
            {
                _isApplyingData = false;
            }
        }

        private void RaiseAfterModified()
        {
            if (_isApplyingData)
            {
                return;
            }

            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        private static BackendMaterialBoxInitCfg CloneData(BackendMaterialBoxInitCfg data)
        {
            if (data == null)
            {
                var empty = new BackendMaterialBoxInitCfg();
                EnsureDataShape(empty);
                return empty;
            }

            var cloned = new BackendMaterialBoxInitCfg();
            cloned.CopyFrom(data);
            EnsureDataShape(cloned);
            return cloned;
        }

        private static void EnsureDataShape(BackendMaterialBoxInitCfg data)
        {
            if (data == null)
            {
                return;
            }

            data.LoadStorageDevice ??= new StorageDeviceInitCfg();
            data.UnloadStorageDevice ??= new StorageDeviceInitCfg();
            data.LoadStorageDevice.StorageMechanism ??= new List<StorageMechanismInitCfg>();
            if(data.LoadStorageDevice.StorageMechanism.Count == 0)
            {
                data.LoadStorageDevice.StorageMechanism.Add(new StorageMechanismInitCfg());
                data.LoadStorageDevice.StorageMechanism.Add(new StorageMechanismInitCfg());
            }
            data.UnloadStorageDevice.StorageMechanism ??= new List<StorageMechanismInitCfg>();
            if (data.UnloadStorageDevice.StorageMechanism.Count == 0)
            {
                data.UnloadStorageDevice.StorageMechanism.Add(new StorageMechanismInitCfg());
                data.UnloadStorageDevice.StorageMechanism.Add(new StorageMechanismInitCfg());
            }
            data.LoadTransportMechanism ??= new TransportMechanismInitCfg();
            data.UnloadTransportMechanism ??= new TransportMechanismInitCfg();
            data.FeedingPortInitCfg ??= new FeedPortInitCfg();
            data.ReceivePortInitCfg ??= new FeedPortInitCfg();
            data.LoadStorageDevice.StorageMechanism[0].SenserIOGuids ??= new List<Guid>();
            data.LoadStorageDevice.StorageMechanism[1].SenserIOGuids ??= new List<Guid>();
            data.UnloadStorageDevice.StorageMechanism[0].SenserIOGuids ??= new List<Guid>();
            data.UnloadStorageDevice.StorageMechanism[1].SenserIOGuids ??= new List<Guid>();
            data.LoadStorageDevice.StorageMechanism[0].CylinderInitParameters ??= new List<CylinderInitParameters>();
            data.LoadStorageDevice.StorageMechanism[1].CylinderInitParameters ??= new List<CylinderInitParameters>();
            data.UnloadStorageDevice.StorageMechanism[0].CylinderInitParameters ??= new List<CylinderInitParameters>();
            data.UnloadStorageDevice.StorageMechanism[1].CylinderInitParameters ??= new List<CylinderInitParameters>();
            data.FeedingPortInitCfg.SensorIOGuids ??= new List<Guid>();
            data.ReceivePortInitCfg.SensorIOGuids ??= new List<Guid>();
        }

        private static List<Guid> CapturePortBindingGuids(IEnumerable<MaterialBoxPortBindingRowViewModel> rows)
        {
            return rows.Select(row => row.ToGuid()).ToList();
        }

        private static void SetSelectedAxis(ComboxViewModel viewModel, List<ComBoxItem> items, Guid axisGuid)
        {
            var target = items.FirstOrDefault(item => (item.Value as AxisInformation)?.AxisGuid == axisGuid);
            if (target != null)
            {
                viewModel.SelectedItem = target;
            }
        }

        private static void SetSelectedSensor(ComboxViewModel viewModel, List<ComBoxItem> items, Guid ioGuid)
        {
            var target = items.FirstOrDefault(item => (item.Value as IOStateInformation)?.IOGuid == ioGuid);
            if (target != null)
            {
                viewModel.SelectedItem = target;
            }
        }

        private static Guid GetSelectedAxisGuid(ComboxViewModel viewModel)
        {
            return ((viewModel.SelectedItem as ComBoxItem)?.Value as AxisInformation)?.AxisGuid ?? Guid.Empty;
        }

        private static Guid GetSelectedSensorGuid(ComboxViewModel viewModel)
            => ((viewModel.SelectedItem as ComBoxItem)?.Value as IOStateInformation)?.IOGuid ?? Guid.Empty;

        private IEnumerable<AxisInformation> BuildCurrentAxisOptions(Guid axisGuid, string axisName)
        {
            if (_axisInformations.Count > 0)
            {
                return _axisInformations;
            }

            if (axisGuid == Guid.Empty)
            {
                return Array.Empty<AxisInformation>();
            }

            return new[]
            {
                new AxisInformation
                {
                    AxisGuid = axisGuid,
                    AxisName = string.IsNullOrWhiteSpace(axisName) ? axisGuid.ToString() : axisName,
                }
            };
        }

        private List<IOStateInformation> BuildCurrentIOStateOptions()
        {
            if (_ioStateInformations.Count > 0)
            {
                return _ioStateInformations;
            }

            var result = new List<IOStateInformation>();
            AppendIOStateOption(result, GetSensorGuid(_loadedData.LoadStorageDevice?.StorageMechanism[0]?.SenserIOGuids, 0));
            AppendIOStateOption(result, GetSensorGuid(_loadedData.LoadStorageDevice?.StorageMechanism[1]?.SenserIOGuids, 0));
            AppendIOStateOption(result, GetSensorGuid(_loadedData.UnloadStorageDevice?.StorageMechanism[0]?.SenserIOGuids, 0));
            AppendIOStateOption(result, GetSensorGuid(_loadedData.UnloadStorageDevice?.StorageMechanism[1]?.SenserIOGuids, 0));
            AppendCylinderIOStateOptions(result, GetCylinder(_loadedData.LoadStorageDevice?.StorageMechanism[0]?.CylinderInitParameters, 0));
            AppendCylinderIOStateOptions(result, GetCylinder(_loadedData.LoadStorageDevice?.StorageMechanism[1]?.CylinderInitParameters, 0));
            AppendCylinderIOStateOptions(result, GetCylinder(_loadedData.UnloadStorageDevice?.StorageMechanism[0]?.CylinderInitParameters, 0));
            AppendCylinderIOStateOptions(result, GetCylinder(_loadedData.UnloadStorageDevice?.StorageMechanism[1]?.CylinderInitParameters, 0));
            AppendPortIOStateOptions(result, _loadedData.FeedingPortInitCfg?.SensorIOGuids);
            AppendPortIOStateOptions(result, _loadedData.ReceivePortInitCfg?.SensorIOGuids);
            AppendPortIOStateOptions(result, CapturePortBindingGuids(FeedingPortRows));
            AppendPortIOStateOptions(result, CapturePortBindingGuids(ReceivePortRows));
            return result;
        }

        private static void AppendIOStateOption(List<IOStateInformation> result, Guid ioGuid)
        {
            if (ioGuid == Guid.Empty || result.Any(item => item.IOGuid == ioGuid))
            {
                return;
            }

            result.Add(new IOStateInformation
            {
                IOGuid = ioGuid,
                IOName = ioGuid.ToString(),
            });
        }

        private static void AppendCylinderIOStateOptions(List<IOStateInformation> result, CylinderInitParameters cylinder)
        {
            foreach (var ioGuid in GetCylinderIoGuids(cylinder))
            {
                if (result.Any(item => item.IOGuid == ioGuid))
                {
                    continue;
                }

                result.Add(new IOStateInformation
                {
                    IOGuid = ioGuid,
                    IOName = ioGuid.ToString(),
                });
            }
        }

        private static void AppendPortIOStateOptions(List<IOStateInformation> result, IEnumerable<Guid> bindings)
        {
            if (bindings == null)
            {
                return;
            }

            foreach (var binding in bindings)
            {
                if (binding == Guid.Empty || result.Any(item => item.IOGuid == binding))
                {
                    continue;
                }

                result.Add(new IOStateInformation
                {
                    IOGuid = binding,
                    IOName = binding.ToString(),
                });
            }
        }

        private static void FillTrajectory(NonProcessingTrajectoryParameters axis, TextInputViewModel startSpeed, TextInputViewModel maxSpeed, TextInputViewModel acceleration, TextInputViewModel deceleration)
        {
            startSpeed.Text = axis.StartSpeed.ToString("0.###", CultureInfo.InvariantCulture);
            maxSpeed.Text = axis.MaxSpeed.ToString("0.###", CultureInfo.InvariantCulture);
            acceleration.Text = axis.Acceleration.ToString("0.###", CultureInfo.InvariantCulture);
            deceleration.Text = axis.Deceleration.ToString("0.###", CultureInfo.InvariantCulture);
        }

        private static NonProcessingTrajectoryParameters BuildTrajectory(TextInputViewModel startSpeed, TextInputViewModel maxSpeed, TextInputViewModel acceleration, TextInputViewModel deceleration, NonProcessingTrajectoryParameters current)
        {
            current.StartSpeed = ParseDouble(startSpeed.Text);
            current.MaxSpeed = ParseDouble(maxSpeed.Text);
            current.Acceleration = ParseDouble(acceleration.Text);
            current.Deceleration = ParseDouble(deceleration.Text);
            return current;
        }

        private static double ParseDouble(string text)
        {
            return double.TryParse(text, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var value) ? value : 0;
        }

        private static string BuildIOStateDisplayName(IOStateInformation ioState)
        {
            if (ioState == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(ioState.IOName))
            {
                return ioState.IOName;
            }

            if (!string.IsNullOrWhiteSpace(ioState.ChannelId))
            {
                return ioState.ChannelId;
            }

            return ioState.IOGuid.ToString();
        }

        private static CylinderConfigCfgInfo ToCylinderConfig(CylinderInitParameters data)
        {
            data ??= new CylinderInitParameters();
            var ioGuids = GetCylinderIoGuids(data);

            HorizontalControlCardStateInitCfgInfo BuildModel(int index)
            {
                return new HorizontalControlCardStateInitCfgInfo
                {
                    CardType = ControlCardType.GC800,
                    channelID = index < ioGuids.Count ? ioGuids[index].ToString() : string.Empty,
                };
            }

            var result = new CylinderConfigCfgInfo
            {
                ConfigType = (UICylinderType)GetCylinderType(data),
            };

            switch ((GKG.ECylinderType)GetCylinderType(data))
            {
                case GKG.ECylinderType.SingleControlSingleLimit:
                    if (ioGuids.Count >= 2)
                    {
                        result.SingleActSingleLimit = new SingleControlSingleLimitCfgInfo
                        {
                            ControlModel = BuildModel(0),
                            LimitModel = BuildModel(1),
                            CylinderDelayModel = new CylinderDelayCfgInfo
                            {
                                DelayNumeric = GetCylinderDelay(data),
                            },
                        };
                    }
                    break;
                case GKG.ECylinderType.SingleControlDoubleLimit:
                    if (ioGuids.Count >= 3)
                    {
                        result.SingleActDoubleLimit = new SingleControlDoubleLimitCfgInfo
                        {
                            ControlModel = BuildModel(0),
                            FirstLimitModel = BuildModel(1),
                            SecondLimitModel = BuildModel(2),
                            CylinderDelayModel = new CylinderDelayCfgInfo
                            {
                                DelayNumeric = GetCylinderDelay(data),
                            },
                        };
                    }
                    break;
                case GKG.ECylinderType.DoubleControlSingleLimit:
                    if (ioGuids.Count >= 3)
                    {
                        result.DoubleActSingleLimit = new DoubleControlSingleLimitCfgInfo
                        {
                            FirstControlModel = BuildModel(0),
                            SecondControlModel = BuildModel(1),
                            LimitModel = BuildModel(2),
                            CylinderDelayModel = new CylinderDelayCfgInfo
                            {
                                DelayNumeric = GetCylinderDelay(data),
                            },
                        };
                    }
                    break;
                case GKG.ECylinderType.DoubleControlDoubleLimit:
                    if (ioGuids.Count >= 4)
                    {
                        result.DoubleActDoubleLimit = new DoubleControlDoubleLimitCfgInfo
                        {
                            FirstControlModel = BuildModel(0),
                            SecondControlModel = BuildModel(1),
                            FirstLimitModel = BuildModel(2),
                            SecondLimitModel = BuildModel(3),
                            CylinderDelayModel = new CylinderDelayCfgInfo
                            {
                                DelayNumeric = GetCylinderDelay(data),
                            },
                        };
                    }
                    break;
            }

            return result;
        }

        private static CylinderInitParameters ToCylinderModel(CylinderConfigViewModel viewModel)
        {
            var cfg = new CylinderConfigCfgInfo();
            viewModel.CopyTo(cfg);
            var result = new CylinderInitParameters();
            SetCylinderType(result, (int)cfg.ConfigType);
            SetCylinderDelay(result, GetCylinderDelay(cfg));
            var ioGuids = new List<Guid>();

            switch (cfg.ConfigType)
            {
                case UICylinderType.SingleActSingleLimit:
                    AppendIoGuid(ioGuids, cfg.SingleActSingleLimit?.ControlModel);
                    AppendIoGuid(ioGuids, cfg.SingleActSingleLimit?.LimitModel);
                    break;
                case UICylinderType.SingleActDoubleLimit:
                    AppendIoGuid(ioGuids, cfg.SingleActDoubleLimit?.ControlModel);
                    AppendIoGuid(ioGuids, cfg.SingleActDoubleLimit?.FirstLimitModel);
                    AppendIoGuid(ioGuids, cfg.SingleActDoubleLimit?.SecondLimitModel);
                    break;
                case UICylinderType.DoubleActSingleLimit:
                    AppendIoGuid(ioGuids, cfg.DoubleActSingleLimit?.FirstControlModel);
                    AppendIoGuid(ioGuids, cfg.DoubleActSingleLimit?.SecondControlModel);
                    AppendIoGuid(ioGuids, cfg.DoubleActSingleLimit?.LimitModel);
                    break;
                case UICylinderType.DoubleActDoubleLimit:
                    AppendIoGuid(ioGuids, cfg.DoubleActDoubleLimit?.FirstControlModel);
                    AppendIoGuid(ioGuids, cfg.DoubleActDoubleLimit?.SecondControlModel);
                    AppendIoGuid(ioGuids, cfg.DoubleActDoubleLimit?.FirstLimitModel);
                    AppendIoGuid(ioGuids, cfg.DoubleActDoubleLimit?.SecondLimitModel);
                    break;
            }

            SetCylinderIoGuids(result, ioGuids);
            return result;
        }

        private static void AppendIoGuid(List<Guid> ioGuids, HorizontalControlCardStateInitCfgInfo model)
        {
            if (!Guid.TryParse(model?.channelID, out var ioGuid) || ioGuid == Guid.Empty)
            {
                return;
            }

            ioGuids.Add(ioGuid);
        }

        private static int GetCylinderDelay(CylinderConfigCfgInfo cfg)
        {
            return cfg.ConfigType switch
            {
                UICylinderType.SingleActSingleLimit => (int)(cfg.SingleActSingleLimit?.CylinderDelayModel?.DelayNumeric ?? 100),
                UICylinderType.SingleActDoubleLimit => (int)(cfg.SingleActDoubleLimit?.CylinderDelayModel?.DelayNumeric ?? 100),
                UICylinderType.DoubleActSingleLimit => (int)(cfg.DoubleActSingleLimit?.CylinderDelayModel?.DelayNumeric ?? 100),
                UICylinderType.DoubleActDoubleLimit => (int)(cfg.DoubleActDoubleLimit?.CylinderDelayModel?.DelayNumeric ?? 100),
                _ => 100,
            };
        }

        private static Guid GetSensorGuid(List<Guid> sensorGuids, int index)
        {
            return sensorGuids != null && index >= 0 && index < sensorGuids.Count
                ? sensorGuids[index]
                : Guid.Empty;
        }

        private static CylinderInitParameters GetCylinder(List<CylinderInitParameters> cylinders, int index)
        {
            return cylinders != null && index >= 0 && index < cylinders.Count && cylinders[index] != null
                ? cylinders[index]
                : new CylinderInitParameters();
        }

        private static void EnsureGuidListSize(List<Guid> list, int size)
        {
            while (list.Count < size)
            {
                list.Add(Guid.Empty);
            }
        }

        private static void EnsureCylinderListSize(List<CylinderInitParameters> list, int size)
        {
            while (list.Count < size)
            {
                list.Add(new CylinderInitParameters());
            }
        }

        private static Guid GetTransportBindingAxisId(TransportMechanismInitCfg transport, bool isLoadSide)
        {
            if (transport == null)
                return Guid.Empty;

            var type = transport.GetType();
            var singleBindingProperty = type.GetProperty("BindingAxisId", BindingFlags.Public | BindingFlags.Instance);
            if (singleBindingProperty?.PropertyType == typeof(Guid))
                return (Guid)(singleBindingProperty.GetValue(transport) ?? Guid.Empty);

            var legacyPropertyName = isLoadSide ? "BindingLoadAxisId" : "BindingUnloadAxisId";
            var legacyBindingProperty = type.GetProperty(legacyPropertyName, BindingFlags.Public | BindingFlags.Instance);
            if (legacyBindingProperty?.PropertyType == typeof(Guid))
                return (Guid)(legacyBindingProperty.GetValue(transport) ?? Guid.Empty);

            return Guid.Empty;
        }

        private static void SetTransportBindingAxisId(TransportMechanismInitCfg transport, Guid axisId, bool isLoadSide)
        {
            if (transport == null)
                return;

            var type = transport.GetType();
            var singleBindingProperty = type.GetProperty("BindingAxisId", BindingFlags.Public | BindingFlags.Instance);
            if (singleBindingProperty?.CanWrite == true && singleBindingProperty.PropertyType == typeof(Guid))
            {
                singleBindingProperty.SetValue(transport, axisId);
                return;
            }

            var legacyPropertyName = isLoadSide ? "BindingLoadAxisId" : "BindingUnloadAxisId";
            var legacyBindingProperty = type.GetProperty(legacyPropertyName, BindingFlags.Public | BindingFlags.Instance);
            if (legacyBindingProperty?.CanWrite == true && legacyBindingProperty.PropertyType == typeof(Guid))
                legacyBindingProperty.SetValue(transport, axisId);
        }

        private static int GetCylinderType(CylinderInitParameters cylinder)
        {
            var property = cylinder?.GetType().GetProperty("eCylinderType", BindingFlags.Public | BindingFlags.Instance);
            var value = property?.GetValue(cylinder);
            return value == null ? 0 : (int)value;
        }

        private static void SetCylinderType(CylinderInitParameters cylinder, int value)
        {
            var property = cylinder?.GetType().GetProperty("eCylinderType", BindingFlags.Public | BindingFlags.Instance);
            if (property == null || !property.CanWrite)
            {
                return;
            }

            var enumValue = Enum.ToObject(property.PropertyType, value);
            property.SetValue(cylinder, enumValue);
        }

        private static List<Guid> GetCylinderIoGuids(CylinderInitParameters cylinder)
        {
            var property = cylinder?.GetType().GetProperty("IOStateGuidList", BindingFlags.Public | BindingFlags.Instance);
            return property?.GetValue(cylinder) as List<Guid> ?? new List<Guid>();
        }

        private static void SetCylinderIoGuids(CylinderInitParameters cylinder, List<Guid> value)
        {
            var property = cylinder?.GetType().GetProperty("IOStateGuidList", BindingFlags.Public | BindingFlags.Instance);
            if (property?.CanWrite == true)
            {
                property.SetValue(cylinder, value ?? new List<Guid>());
            }
        }

        private static int GetCylinderDelay(CylinderInitParameters cylinder)
        {
            foreach (var name in new[] { "CylinderDelay", "Delay", "DelayMs" })
            {
                var property = cylinder?.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                if (property == null)
                {
                    continue;
                }

                var value = property.GetValue(cylinder);
                if (value != null)
                {
                    return Convert.ToInt32(value, CultureInfo.InvariantCulture);
                }
            }

            return 100;
        }

        private static void SetCylinderDelay(CylinderInitParameters cylinder, int value)
        {
            foreach (var name in new[] { "CylinderDelay", "Delay", "DelayMs" })
            {
                var property = cylinder?.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                if (property?.CanWrite != true)
                {
                    continue;
                }

                var targetValue = Convert.ChangeType(value, property.PropertyType, CultureInfo.InvariantCulture);
                property.SetValue(cylinder, targetValue);
                return;
            }
        }
    }

    public class MaterialBoxPortBindingRowViewModel : ReactiveObject
    {
        private readonly List<ComBoxItem> _items = new();

        public string PortName { get; }

        public ComboxViewModel IOViewModel { get; }

        public event EventHandler AfterModified;

        public MaterialBoxPortBindingRowViewModel(string portName, bool isEnabled)
        {
            PortName = portName;
            IOViewModel = new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                ItemsSource = _items,
                IsEnabled = isEnabled,
            };
            IOViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
        }

        public void SetEnabled(bool isEnabled)
        {
            IOViewModel.IsEnabled = isEnabled;
        }

        public void SetOptions(IEnumerable<IOStateInformation> ioStates, Guid selectedGuid)
        {
            _items.Clear();

            foreach (var ioState in (ioStates ?? Enumerable.Empty<IOStateInformation>())
                .Where(item => item != null && item.IOGuid != Guid.Empty)
                .GroupBy(item => item.IOGuid)
                .Select(group => group.First()))
            {
                _items.Add(new ComBoxItem
                {
                    Value = ioState,
                    DisplayName = BuildDisplayName(ioState),
                });
            }

            if (selectedGuid != Guid.Empty &&
                _items.All(item => (item.Value as IOStateInformation)?.IOGuid != selectedGuid))
            {
                _items.Add(new ComBoxItem
                {
                    Value = new IOStateInformation
                    {
                        IOGuid = selectedGuid,
                        IOName = selectedGuid.ToString(),
                    },
                    DisplayName = selectedGuid.ToString(),
                });
            }

            if (_items.Count == 0)
            {
                _items.Add(new ComBoxItem
                {
                    Value = null,
                    DisplayName = "等待后端IO列表",
                });
            }

            IOViewModel.SelectedItem = _items[0];
            var target = _items.FirstOrDefault(item => (item.Value as IOStateInformation)?.IOGuid == selectedGuid);
            if (target != null)
            {
                IOViewModel.SelectedItem = target;
            }
        }

        public Guid ToGuid() => ((IOViewModel.SelectedItem as ComBoxItem)?.Value as IOStateInformation)?.IOGuid ?? Guid.Empty;

        private static string BuildDisplayName(IOStateInformation ioState)
        {
            if (ioState == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(ioState.IOName))
            {
                return ioState.IOName;
            }

            if (!string.IsNullOrWhiteSpace(ioState.ChannelId))
            {
                return ioState.ChannelId;
            }

            return ioState.IOGuid.ToString();
        }
    }
}
