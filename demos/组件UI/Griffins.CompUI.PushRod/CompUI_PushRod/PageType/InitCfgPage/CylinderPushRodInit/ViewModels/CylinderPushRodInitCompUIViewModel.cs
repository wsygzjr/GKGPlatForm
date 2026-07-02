using Avalonia.Controls;
using GF_Gereric;
using GKG;
using GKG.SubMM;
using GKG.UI;
using GKG.UI.General;
using Griffins.Map.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Griffins.CompUI.PushRod.CompUI_PushRod.PageType.InitCfgPage.CylinderPushRodInit.ViewModels
{
    internal class CylinderPushRodInitCompUIViewModel : ReactiveObject
    {
        private readonly List<IOStateInformation> _ioStateInformations = new();
        private CylinderPushRodSubMachineModulesInitCfg _data = new();
        private object _viewTag;
        private bool _readOnly;

        public CylinderConfigViewModel CylinderConfigViewModel { get; }

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
                this.RaisePropertyChanged(nameof(CanEditCylinderConfig));
            }
        }

        public bool CanEditCylinderConfig => !_readOnly;

        public CylinderPushRodInitCompUIViewModel(ICompUIRunTimeCallBack callBack)
        {
            CylinderConfigViewModel = CreateCylinderViewModel();
            LoadIOStateInfos(callBack);
            ApplyCylinderIOChannelOptions(BuildCurrentIOStateOptions());
            ReadOnly = false;
        }

        public void SetViewReference(Control view)
        {
            CylinderConfigViewModel.SetViewReference(view);
        }

        public void SetData(CylinderPushRodSubMachineModulesInitCfg data)
        {
            _data = CloneData(data);
            EnsureDataShape(_data);
            ApplyCylinderIOChannelOptions(BuildCurrentIOStateOptions());
            CylinderConfigViewModel.CopyFrom(ToCylinderConfig(_data.CylinderInitParameters));
        }

        public CylinderPushRodSubMachineModulesInitCfg GetData()
        {
            EnsureDataShape(_data);
            _data.CylinderInitParameters = ToCylinderModel(CylinderConfigViewModel);
            return CloneData(_data);
        }

        private CylinderConfigViewModel CreateCylinderViewModel()
        {
            var viewModel = new CylinderConfigViewModel();
            viewModel.AfterModified += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            return viewModel;
        }

        private void LoadIOStateInfos(ICompUIRunTimeCallBack callBack)
        {
            _ioStateInformations.Clear();
            try
            {
                var result = callBack?.ExecConfigSvrCtlCmd(PushRodSubMachineModulesConst.RtCmdGetCylinderIOChannelOptions, new GFBaseTypeParamValueList());
                var raw = result?["data"]?.ToStringVal();
                if (string.IsNullOrWhiteSpace(raw))
                    raw = result?["Result"]?.ToStringVal();

                var ioInfos = string.IsNullOrWhiteSpace(raw)
                    ? null
                    : JsonObjConvert.FromJSon<List<IOStateInformation>>(raw);

                if (ioInfos != null)
                    _ioStateInformations.AddRange(ioInfos.Where(item => item != null && item.IOGuid != Guid.Empty));
            }
            catch
            {
            }
        }

        private void ApplyCylinderIOChannelOptions(IEnumerable<IOStateInformation> ioStates)
        {
            var options = (ioStates ?? Enumerable.Empty<IOStateInformation>())
                .Where(item => item != null && item.IOGuid != Guid.Empty)
                .GroupBy(item => item.IOGuid)
                .Select(group => group.First())
                .ToList();

            CylinderConfigViewModel.SetIOChannelOptions(options);
        }

        private List<IOStateInformation> BuildCurrentIOStateOptions()
        {
            var result = _ioStateInformations
                .Where(item => item != null && item.IOGuid != Guid.Empty)
                .GroupBy(item => item.IOGuid)
                .Select(group => group.First())
                .ToList();

            foreach (var ioGuid in _data?.CylinderInitParameters?.IOStateGuidList ?? Enumerable.Empty<Guid>())
            {
                if (ioGuid == Guid.Empty || result.Any(item => item.IOGuid == ioGuid))
                    continue;

                result.Add(new IOStateInformation
                {
                    IOGuid = ioGuid,
                    IOName = ioGuid.ToString(),
                });
            }

            return result;
        }

        private static CylinderPushRodSubMachineModulesInitCfg CloneData(CylinderPushRodSubMachineModulesInitCfg data)
        {
            var cloned = data == null
                ? new CylinderPushRodSubMachineModulesInitCfg()
                : JsonObjConvert.FromJSonBytes<CylinderPushRodSubMachineModulesInitCfg>(JsonObjConvert.ToJSonBytes(data))
                    ?? new CylinderPushRodSubMachineModulesInitCfg();
            EnsureDataShape(cloned);
            return cloned;
        }

        private static void EnsureDataShape(CylinderPushRodSubMachineModulesInitCfg data)
        {
            data.CylinderInitParameters ??= new CylinderInitParameters();
            data.CylinderInitParameters.IOStateGuidList ??= new List<Guid>();
        }

        private static CylinderConfigCfgInfo ToCylinderConfig(CylinderInitParameters data)
        {
            data ??= new CylinderInitParameters();
            var ioGuids = data.IOStateGuidList ?? new List<Guid>();

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
                ConfigType = (GKG.UI.General.ECylinderType)data.eCylinderType,
            };

            switch (data.eCylinderType)
            {
                case GKG.ECylinderType.SingleControlSingleLimit:
                    result.SingleActSingleLimit = new SingleControlSingleLimitCfgInfo
                    {
                        ControlModel = BuildModel(0),
                        LimitModel = BuildModel(1),
                        CylinderDelayModel = new CylinderDelayCfgInfo
                        {
                            DelayNumeric = data.CylinderDelay,
                        },
                    };
                    break;
                case GKG.ECylinderType.SingleControlDoubleLimit:
                    result.SingleActDoubleLimit = new SingleControlDoubleLimitCfgInfo
                    {
                        ControlModel = BuildModel(0),
                        FirstLimitModel = BuildModel(1),
                        SecondLimitModel = BuildModel(2),
                        CylinderDelayModel = new CylinderDelayCfgInfo
                        {
                            DelayNumeric = data.CylinderDelay,
                        },
                    };
                    break;
                case GKG.ECylinderType.DoubleControlSingleLimit:
                    result.DoubleActSingleLimit = new DoubleControlSingleLimitCfgInfo
                    {
                        FirstControlModel = BuildModel(0),
                        SecondControlModel = BuildModel(1),
                        LimitModel = BuildModel(2),
                        CylinderDelayModel = new CylinderDelayCfgInfo
                        {
                            DelayNumeric = data.CylinderDelay,
                        },
                    };
                    break;
                case GKG.ECylinderType.DoubleControlDoubleLimit:
                    result.DoubleActDoubleLimit = new DoubleControlDoubleLimitCfgInfo
                    {
                        FirstControlModel = BuildModel(0),
                        SecondControlModel = BuildModel(1),
                        FirstLimitModel = BuildModel(2),
                        SecondLimitModel = BuildModel(3),
                        CylinderDelayModel = new CylinderDelayCfgInfo
                        {
                            DelayNumeric = data.CylinderDelay,
                        },
                    };
                    break;
            }

            return result;
        }

        private static CylinderInitParameters ToCylinderModel(CylinderConfigViewModel viewModel)
        {
            var cfg = new CylinderConfigCfgInfo();
            viewModel.CopyTo(cfg);

            var result = new CylinderInitParameters
            {
                eCylinderType = (GKG.ECylinderType)cfg.ConfigType,
                CylinderDelay = GetCylinderDelay(cfg),
                IOStateGuidList = new List<Guid>(),
            };

            switch (cfg.ConfigType)
            {
                case GKG.UI.General.ECylinderType.SingleActSingleLimit:
                    AppendIoGuid(result.IOStateGuidList, cfg.SingleActSingleLimit?.ControlModel);
                    AppendIoGuid(result.IOStateGuidList, cfg.SingleActSingleLimit?.LimitModel);
                    break;
                case GKG.UI.General.ECylinderType.SingleActDoubleLimit:
                    AppendIoGuid(result.IOStateGuidList, cfg.SingleActDoubleLimit?.ControlModel);
                    AppendIoGuid(result.IOStateGuidList, cfg.SingleActDoubleLimit?.FirstLimitModel);
                    AppendIoGuid(result.IOStateGuidList, cfg.SingleActDoubleLimit?.SecondLimitModel);
                    break;
                case GKG.UI.General.ECylinderType.DoubleActSingleLimit:
                    AppendIoGuid(result.IOStateGuidList, cfg.DoubleActSingleLimit?.FirstControlModel);
                    AppendIoGuid(result.IOStateGuidList, cfg.DoubleActSingleLimit?.SecondControlModel);
                    AppendIoGuid(result.IOStateGuidList, cfg.DoubleActSingleLimit?.LimitModel);
                    break;
                case GKG.UI.General.ECylinderType.DoubleActDoubleLimit:
                    AppendIoGuid(result.IOStateGuidList, cfg.DoubleActDoubleLimit?.FirstControlModel);
                    AppendIoGuid(result.IOStateGuidList, cfg.DoubleActDoubleLimit?.SecondControlModel);
                    AppendIoGuid(result.IOStateGuidList, cfg.DoubleActDoubleLimit?.FirstLimitModel);
                    AppendIoGuid(result.IOStateGuidList, cfg.DoubleActDoubleLimit?.SecondLimitModel);
                    break;
            }

            return result;
        }

        private static void AppendIoGuid(List<Guid> ioGuids, HorizontalControlCardStateInitCfgInfo model)
        {
            if (!Guid.TryParse(model?.channelID, out var ioGuid) || ioGuid == Guid.Empty)
                return;

            ioGuids.Add(ioGuid);
        }

        private static int GetCylinderDelay(CylinderConfigCfgInfo cfg)
        {
            return cfg.ConfigType switch
            {
                GKG.UI.General.ECylinderType.SingleActSingleLimit => (int)(cfg.SingleActSingleLimit?.CylinderDelayModel?.DelayNumeric ?? 100),
                GKG.UI.General.ECylinderType.SingleActDoubleLimit => (int)(cfg.SingleActDoubleLimit?.CylinderDelayModel?.DelayNumeric ?? 100),
                GKG.UI.General.ECylinderType.DoubleActSingleLimit => (int)(cfg.DoubleActSingleLimit?.CylinderDelayModel?.DelayNumeric ?? 100),
                GKG.UI.General.ECylinderType.DoubleActDoubleLimit => (int)(cfg.DoubleActDoubleLimit?.CylinderDelayModel?.DelayNumeric ?? 100),
                _ => 100,
            };
        }
    }
}
