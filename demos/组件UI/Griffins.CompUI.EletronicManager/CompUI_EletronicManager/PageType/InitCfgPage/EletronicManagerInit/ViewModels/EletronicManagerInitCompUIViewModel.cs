using GF_Gereric;
using GKG;
using GKG.SubMM;
using GKG.UI;
using Griffins.Map.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.InitCfgPage.EletronicManagerInit.ViewModels
{
    internal class EletronicManagerInitCompUIViewModel : ReactiveObject
    {
        private const string ParamKey_CardGuid = "CardGuid";

        private readonly ICompUIRunTimeCallBack _callBack;
        private readonly List<IOStateInformation> _ioStateInformations = new();
        private EletronicManagerSubMachineModulesInitCfg _data = new();
        private bool _isApplyingData;

        public ComboxViewModel PowerIOViewModel { get; }
        public TextInputViewModel PowerTimeViewModel { get; }

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
                PowerIOViewModel.IsEnabled = enabled;
                PowerTimeViewModel.IsEnabled = enabled;
            }
        }

        public EletronicManagerInitCompUIViewModel(ICompUIRunTimeCallBack callBack)
        {
            _callBack = callBack;
            PowerIOViewModel = CreateComboViewModel();
            PowerTimeViewModel = new TextInputViewModel();

            PowerIOViewModel.ValueChanged += (_, __) => OnPowerIoSelectionChanged();
            PowerTimeViewModel.ValueChanged += (_, __) => OnPowerTimeChanged();

            LoadIoConfigList();
            ApplyWithoutModified(() =>
            {
                ApplyPowerIoOptions(_data.PowerIOStateGuid);
                PowerTimeViewModel.Text = _data.PowerTime.ToString(CultureInfo.InvariantCulture);
            });

            ReadOnly = false;
        }

        public void SetData(EletronicManagerSubMachineModulesInitCfg data)
        {
            ApplyWithoutModified(() =>
            {
                _data = CloneData(data);
                if (_ioStateInformations.Count == 0)
                    LoadIoConfigList();

                ApplyPowerIoOptions(_data.PowerIOStateGuid);
                PowerTimeViewModel.Text = _data.PowerTime.ToString(CultureInfo.InvariantCulture);
            });
        }

        public EletronicManagerSubMachineModulesInitCfg GetData()
        {
            _data.PowerTime = ParseInt(PowerTimeViewModel.Text, _data.PowerTime);
            return CloneData(_data);
        }

        private void OnPowerIoSelectionChanged()
        {
            if (_isApplyingData)
                return;

            var selectedGuid = GetSelectedIoGuid();
            if (_data.PowerIOStateGuid == selectedGuid)
                return;

            _data.PowerIOStateGuid = selectedGuid;
            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        private void OnPowerTimeChanged()
        {
            if (_isApplyingData)
                return;

            var newValue = ParseInt(PowerTimeViewModel.Text, _data.PowerTime);
            if (_data.PowerTime == newValue)
                return;

            _data.PowerTime = newValue;
            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        private void LoadIoConfigList()
        {
            _ioStateInformations.Clear();
            if (_callBack == null)
                return;

            try
            {
                var cardGuids = ResolveMotionCardGuids();
                if (cardGuids.Count > 0)
                {
                    foreach (var cardGuid in cardGuids)
                    {
                        if (TryLoadIoConfigListFromRuntime(cardGuid, out var runtimeIoList) && runtimeIoList.Count > 0)
                        {
                            MergeIoStateInformations(runtimeIoList);
                            return;
                        }
                    }
                }

                if (TryLoadIoConfigListFromFactoryCfg(out var factoryIoList))
                    MergeIoStateInformations(factoryIoList);
            }
            catch
            {
            }
        }

        /// <summary>
        /// GetIoConfigList 走 ExecRuntimeCtlCmdCore，必须先通过 TryGetCard 校验 CardGuid。
        /// </summary>
        private bool TryLoadIoConfigListFromRuntime(Guid cardGuid, out List<IOStateInformation> ioList)
        {
            ioList = new List<IOStateInformation>();
            if (cardGuid == Guid.Empty)
                return false;

            var cmdParam = new GFBaseTypeParamValueList();
            cmdParam[ParamKey_CardGuid] = new GriffinsBaseValue(cardGuid.ToString());

            var result = _callBack.ExecConfigSvrCtlCmd(
                EletronicManagerSubMachineModulesConst.RtCmdGetIoConfigList,
                cmdParam);

            if (!IsCommandSucceeded(result))
                return false;

            var json = TryGetResultData(result);
            if (string.IsNullOrWhiteSpace(json))
                return false;

            ioList = ParseIoStateInformations(json);
            return ioList.Count > 0;
        }

        private bool TryLoadIoConfigListFromFactoryCfg(out List<IOStateInformation> ioList)
        {
            ioList = new List<IOStateInformation>();

            var result = _callBack.ExecConfigSvrCtlCmd(
                EletronicManagerSubMachineModulesConst.RtCmdGetElectricalFactoryCfg,
                new GFBaseTypeParamValueList());

            if (!IsCommandSucceeded(result))
                return false;

            var factoryCfgJson = TryGetResultData(result);
            if (string.IsNullOrWhiteSpace(factoryCfgJson))
                return false;

            var factoryCfg = JsonObjConvert.FromJSon<EletronicManagerSubMachineModulesFactoryCfg>(factoryCfgJson);
            if (factoryCfg?.IOStateInformations == null)
                return false;

            ioList = factoryCfg.IOStateInformations
                .Where(item => item != null && item.IOGuid != Guid.Empty)
                .ToList();

            return ioList.Count > 0;
        }

        private List<Guid> ResolveMotionCardGuids()
        {
            var result = _callBack.ExecConfigSvrCtlCmd(
                EletronicManagerSubMachineModulesConst.RtCmdGetElectricalFactoryCfg,
                new GFBaseTypeParamValueList());

            if (!IsCommandSucceeded(result))
                return new List<Guid>();

            var factoryCfgJson = TryGetResultData(result);
            if (string.IsNullOrWhiteSpace(factoryCfgJson))
                return new List<Guid>();

            var factoryCfg = JsonObjConvert.FromJSon<EletronicManagerSubMachineModulesFactoryCfg>(factoryCfgJson);
            return (factoryCfg?.EletronicFactoryParameters?.MotionControlCardInformations ?? new List<MotionControlCardInformations>())
                .Where(card => card != null && card.MotionCardID != Guid.Empty)
                .Select(card => card.MotionCardID)
                .Distinct()
                .ToList();
        }

        private static List<IOStateInformation> ParseIoStateInformations(string json)
        {
            var groupedIoInfos = JsonObjConvert.FromJSon<List<List<IOStateInformation>>>(json);
            if (groupedIoInfos != null)
            {
                return groupedIoInfos
                    .SelectMany(group => group ?? Enumerable.Empty<IOStateInformation>())
                    .Where(item => item != null && item.IOGuid != Guid.Empty)
                    .ToList();
            }

            var flatIoInfos = JsonObjConvert.FromJSon<List<IOStateInformation>>(json);
            return flatIoInfos?
                .Where(item => item != null && item.IOGuid != Guid.Empty)
                .ToList()
                ?? new List<IOStateInformation>();
        }

        private void MergeIoStateInformations(IEnumerable<IOStateInformation> ioStates)
        {
            foreach (var ioState in ioStates.Where(item => item != null && item.IOGuid != Guid.Empty))
            {
                var existingIndex = _ioStateInformations.FindIndex(item => item.IOGuid == ioState.IOGuid);
                if (existingIndex >= 0)
                    _ioStateInformations[existingIndex] = ioState;
                else
                    _ioStateInformations.Add(ioState);
            }
        }

        private void ApplyPowerIoOptions(Guid selectedGuid)
        {
            var items = PowerIOViewModel.ItemsSource as ObservableCollection<ComBoxItem>;
            items?.Clear();
            if (items == null)
                return;

            foreach (var ioState in _ioStateInformations
                .Where(item => item != null && item.IOGuid != Guid.Empty)
                .GroupBy(item => item.IOGuid)
                .Select(group => group.First())
                .OrderBy(BuildDisplayName))
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
                    DisplayName = "等待后端 IO 配置列表",
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

            SetSelectedPowerIo(items, selectedGuid);
        }

        private void SetSelectedPowerIo(IEnumerable<ComBoxItem> items, Guid selectedGuid)
        {
            if (selectedGuid == Guid.Empty)
            {
                PowerIOViewModel.SelectedItem = null;
                return;
            }

            var target = items.FirstOrDefault(item => (item.Value as IOStateInformation)?.IOGuid == selectedGuid);
            if (target != null)
                PowerIOViewModel.SelectedItem = target;
        }

        private Guid GetSelectedIoGuid()
            => ((PowerIOViewModel.SelectedItem as ComBoxItem)?.Value as IOStateInformation)?.IOGuid ?? Guid.Empty;

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

        private static bool IsCommandSucceeded(GFBaseTypeParamValueList result)
        {
            if (result == null)
                return false;

            var code = result["Result"]?.ToString();
            return string.IsNullOrWhiteSpace(code) || code == "0";
        }

        private static string TryGetResultData(GFBaseTypeParamValueList result)
        {
            if (result == null)
                return null;

            var data = result["data"]?.ToString();
            if (!string.IsNullOrWhiteSpace(data))
                return data;

            return result["Result"]?.ToStringVal();
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

        private static EletronicManagerSubMachineModulesInitCfg CloneData(EletronicManagerSubMachineModulesInitCfg data)
        {
            if (data == null)
                return new EletronicManagerSubMachineModulesInitCfg();

            return JsonObjConvert.FromJSonBytes<EletronicManagerSubMachineModulesInitCfg>(JsonObjConvert.ToJSonBytes(data))
                ?? new EletronicManagerSubMachineModulesInitCfg();
        }
    }
}
