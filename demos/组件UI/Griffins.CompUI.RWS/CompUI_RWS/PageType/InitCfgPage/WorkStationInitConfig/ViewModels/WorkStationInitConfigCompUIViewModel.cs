#nullable disable
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins.CompUI.RWS.CompUI_RWS;
using Griffins.CompUI.RWS.CompUI_RWS.PageType.InitCfgPage.WorkStationInitConfig.Views;
using Griffins.Map.UI;
using GKG;
using GKG.SubMM;
using GKG.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Reactive;
using System.Reactive.Disposables;
using BackendWorkStationEleInitParams = GKG.SubMM.WorkStationEleInitParams;
using BackendWorkStationTransSpeedGear = GKG.SubMM.WorkStationTransSpeedGear;
using BackendWorkStationTransSpeedGearList = GKG.SubMM.WorkStationTransSpeedGearList;

namespace Griffins.CompUI.RWS.CompUI_RWS.PageType.InitCfgPage.WorkStationInitConfig.ViewModels
{
    internal class WorkStationInitConfigCompUIViewModel : ReactiveObject, IDisposable
    {
        private const string RunTimeCtlGetIOStateInfos = "GetIOInfos";

        private readonly bool isDesign;
        private readonly ICompUIRunTimeCallBack callBack;
        private readonly CompositeDisposable disposables = new();
        private readonly List<IOStateInformation> ioStateInformations = new();

        private Control viewReference;
        private readonly FrontendWorkStationEleInitParams eleInitData = new();
        private RailWorkStationSubMachineModulesInitCfg loadedData = new();
        private bool isApplyingData;
        private bool isInitializingView;
        private bool readOnly;
        private StationPreviewViewModel stationPreview = new();
        private RailWorkStationSubMachineModulesFactoryCfg factoryParams = new RailWorkStationSubMachineModulesFactoryCfg();
        public ObservableCollection<TransSpeedGearItemViewModel> TransSpeedGearItems { get; } = new();

        public bool ReadOnly
        {
            get => readOnly;
            set
            {
                this.RaiseAndSetIfChanged(ref readOnly, value);
                this.RaisePropertyChanged(nameof(CanEditInitConfig));
                this.RaisePropertyChanged(nameof(CanEditTransSpeedGearList));
                foreach (var item in TransSpeedGearItems)
                {
                    item.IsReadOnly = readOnly;
                }
            }
        }

        public bool CanEditInitConfig => !ReadOnly;
        public bool CanEditTransSpeedGearList => !ReadOnly;

        public bool ShowProximitySensorConfig => isDesign || factoryParams.WorkStationEleConfigParams.HasProximitySensor;

        public bool ShowLeftSensorConfig => isDesign || factoryParams.WorkStationEleConfigParams.HasLeftSensor;

        public bool ShowRightSensorConfig => isDesign || factoryParams.WorkStationEleConfigParams.HasRightSensor;

        public bool ShowEleConfigSection
            => ShowProximitySensorConfig || ShowLeftSensorConfig || ShowRightSensorConfig;

        public bool ShowLeftBlockCylinderConfig => isDesign || factoryParams.WorkStationEleConfigParams.HasLeftBlock;

        public bool ShowRightBlockCylinderConfig => isDesign || factoryParams.WorkStationEleConfigParams.HasRightBlock;

        public bool ShowBlockCylinderSection
            => ShowLeftBlockCylinderConfig || ShowRightBlockCylinderConfig;

        public GKG.UI.General.CylinderConfigViewModel LeftCylinderConfigViewModel { get; } = new();
        public GKG.UI.General.CylinderConfigViewModel RightCylinderConfigViewModel { get; } = new();
        public SensorConfigViewModel LeftSensorConfigViewModel { get; } = new();
        public SensorConfigViewModel RightSensorConfigViewModel { get; } = new();
        public SensorConfigViewModel ProximitySensorConfigViewModel { get; } = new();

        public bool HasProximitySensor
            => eleInitData.ProximitySensorID != Guid.Empty;

        public bool HasLeftSensor
            => eleInitData.LeftSensorID != Guid.Empty;

        public bool HasRightSensor
            => eleInitData.RightSensorID != Guid.Empty;

        public StationPreviewViewModel StationPreview
        {
            get => stationPreview;
            private set => this.RaiseAndSetIfChanged(ref stationPreview, value);
        }

        public bool HasLeftCylinder
            => HasCylinderConfig(eleInitData.LeftBlockCylinderParams);

        public bool HasRightCylinder
            => HasCylinderConfig(eleInitData.RightBlockCylinderParams);

        public string ProximitySensorSummary => BuildSensorSummary(eleInitData.ProximitySensorID);

        public string LeftSensorSummary => BuildSensorSummary(eleInitData.LeftSensorID);

        public string RightSensorSummary => BuildSensorSummary(eleInitData.RightSensorID);

        public string LeftCylinderSummary => BuildCylinderSummary(eleInitData.LeftBlockCylinderParams);

        public string RightCylinderSummary => BuildCylinderSummary(eleInitData.RightBlockCylinderParams);

        public ReactiveCommand<Unit, Unit> ConfigProximitySensorCommand { get; }
        public ReactiveCommand<Unit, Unit> ConfigLeftSensorCommand { get; }
        public ReactiveCommand<Unit, Unit> ConfigRightSensorCommand { get; }
        public ReactiveCommand<Unit, Unit> ConfigLeftCylinderCommand { get; }
        public ReactiveCommand<Unit, Unit> ConfigRightCylinderCommand { get; }
        public ReactiveCommand<Unit, Unit> AddTransSpeedGearCommand { get; }

        public event EventHandler AfterModified;

        public WorkStationInitConfigCompUIViewModel()
        {
            isDesign = true;
            InitStationPreviewBinding();
            InitEmbeddedCylinderConfig();
            InitEmbeddedSensorConfig();

            ConfigProximitySensorCommand = ReactiveCommand.Create(() => { });
            ConfigLeftSensorCommand = ReactiveCommand.Create(() => { });
            ConfigRightSensorCommand = ReactiveCommand.Create(() => { });
            ConfigLeftCylinderCommand = ReactiveCommand.Create(() => { });
            ConfigRightCylinderCommand = ReactiveCommand.Create(() => { });
            AddTransSpeedGearCommand = ReactiveCommand.Create(AddTransSpeedGearItem);
        }

        public WorkStationInitConfigCompUIViewModel(bool isDesign, ICompUIRunTimeCallBack callBack)
            : this()
        {
            this.isDesign = isDesign;
            this.callBack = callBack;

            LoadFactoryParams();
            RaiseFactoryVisibilityChanged();

            ConfigProximitySensorCommand = ReactiveCommand.Create(OnConfigProximitySensor);
            ConfigLeftSensorCommand = ReactiveCommand.Create(OnConfigLeftSensor);
            ConfigRightSensorCommand = ReactiveCommand.Create(OnConfigRightSensor);
            ConfigLeftCylinderCommand = ReactiveCommand.Create(OnConfigLeftCylinder);
            ConfigRightCylinderCommand = ReactiveCommand.Create(OnConfigRightCylinder);

            LoadIOStateInfos();
            ApplyWithoutModified(RefreshSensorIoOptions);
        }

        private void InitStationPreviewBinding()
        {
            this.WhenAnyValue(
                    x => x.ShowProximitySensorConfig,
                    x => x.ShowLeftSensorConfig,
                    x => x.ShowRightSensorConfig)
                .Subscribe(v =>
                {
                    StationPreview.ShowSensors = v.Item1 || v.Item2 || v.Item3;
                    StationPreview.ShowLeftSensorImage = v.Item2;
                    StationPreview.ShowRightSensorImage = v.Item3;
                })
                .DisposeWith(disposables);
        }

        private void InitEmbeddedCylinderConfig()
        {
            LeftCylinderConfigViewModel.AfterModified += (_, __) =>
            {
                if (isApplyingData || isInitializingView)
                {
                    return;
                }

                eleInitData.LeftBlockCylinderParams ??= new GKG.UI.General.CylinderConfigCfgInfo();
                LeftCylinderConfigViewModel.CopyTo(eleInitData.LeftBlockCylinderParams);
                RaiseEleInitStateChanged();
            };

            RightCylinderConfigViewModel.AfterModified += (_, __) =>
            {
                if (isApplyingData || isInitializingView)
                {
                    return;
                }

                eleInitData.RightBlockCylinderParams ??= new GKG.UI.General.CylinderConfigCfgInfo();
                RightCylinderConfigViewModel.CopyTo(eleInitData.RightBlockCylinderParams);
                RaiseEleInitStateChanged();
            };
        }

        private void InitEmbeddedSensorConfig()
        {
            void BindSensor(SensorConfigViewModel sensorViewModel, Action<Guid> assignGuid)
            {
                void SyncFromView()
                {
                    if (isApplyingData || isInitializingView)
                    {
                        return;
                    }

                    assignGuid(sensorViewModel.GetSelectedGuid());
                    RaiseEleInitStateChanged();
                }

                sensorViewModel.IOChannelViewModel.ValueChanged += (_, __) => SyncFromView();
                sensorViewModel.IOChannelViewModel.WhenAnyValue(x => x.SelectedItem)
                    .Subscribe(_ => SyncFromView())
                    .DisposeWith(disposables);
            }

            BindSensor(LeftSensorConfigViewModel, guid => eleInitData.LeftSensorID = guid);
            BindSensor(RightSensorConfigViewModel, guid => eleInitData.RightSensorID = guid);
            BindSensor(ProximitySensorConfigViewModel, guid => eleInitData.ProximitySensorID = guid);
        }

        private void LoadIOStateInfos()
        {
            ioStateInformations.Clear();
            if (callBack == null)
            {
                return;
            }

            try
            {
                var result = callBack.ExecConfigSvrCtlCmd(RunTimeCtlGetIOStateInfos, new GFBaseTypeParamValueList());
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
                    ioStateInformations.AddRange(ioInfos.Where(item => item != null && item.IOGuid != Guid.Empty));
                }
            }
            catch
            {
            }
        }

        public void SetData(RailWorkStationSubMachineModulesInitCfg model)
        {
            RaiseFactoryVisibilityChanged();
            ApplyWithoutModified(() =>
            {
                loadedData = CloneData(model);
                var eleInitParams = loadedData?.WorkStationEleInitParams ?? new BackendWorkStationEleInitParams();

                eleInitData.ProximitySensorID = eleInitParams.ProximitySensorID;
                eleInitData.LeftSensorID = eleInitParams.LeftSensorID;
                eleInitData.RightSensorID = eleInitParams.RightSensorID;
                eleInitData.LeftBlockCylinderParams = BuildCylinderCfgFromInitParams(eleInitParams.LeftBlockCylinderParams);
                eleInitData.RightBlockCylinderParams = BuildCylinderCfgFromInitParams(eleInitParams.RightBlockCylinderParams);
                RefreshSensorIoOptions();
                LeftCylinderConfigViewModel.CopyFrom(eleInitData.LeftBlockCylinderParams ?? new GKG.UI.General.CylinderConfigCfgInfo());
                RightCylinderConfigViewModel.CopyFrom(eleInitData.RightBlockCylinderParams ?? new GKG.UI.General.CylinderConfigCfgInfo());
                RaiseEleInitStateChanged(notifyModified: false);
                SetTransSpeedGearItems(loadedData?.WorkStationTransSpeedGearList);
            });
        }

        public RailWorkStationSubMachineModulesInitCfg GetData()
        {
            var result = CloneData(loadedData) ?? new RailWorkStationSubMachineModulesInitCfg();
            result.WorkStationEleInitParams = GetEleInitData();
            result.WorkStationTransSpeedGearList = GetTransSpeedGearList();
            loadedData = CloneData(result);
            return result;
        }

        public BackendWorkStationEleInitParams GetEleInitData()
        {
            return new BackendWorkStationEleInitParams
            {
                // Sensor IDs are maintained by SensorConfigViewModel change events.
                // Do not re-resolve from current UI state here, otherwise unrelated edits
                // (e.g. cylinder options initialization) may overwrite cached values.
                LeftSensorID = eleInitData.LeftSensorID,
                RightSensorID = eleInitData.RightSensorID,
                ProximitySensorID = eleInitData.ProximitySensorID,
                LeftBlockCylinderParams = BuildCylinderInitParams(eleInitData.LeftBlockCylinderParams ?? new GKG.UI.General.CylinderConfigCfgInfo()),
                RightBlockCylinderParams = BuildCylinderInitParams(eleInitData.RightBlockCylinderParams ?? new GKG.UI.General.CylinderConfigCfgInfo()),
            };
        }

        private static Guid ResolveSensorGuid(SensorConfigViewModel sensorViewModel, Guid cachedGuid)
        {
            var selectedGuid = sensorViewModel.GetSelectedGuid();
            return selectedGuid != Guid.Empty ? selectedGuid : cachedGuid;
        }

        private void RefreshSensorIoOptions()
        {
            LeftSensorConfigViewModel.SetOptions(ioStateInformations, eleInitData.LeftSensorID);
            RightSensorConfigViewModel.SetOptions(ioStateInformations, eleInitData.RightSensorID);
            ProximitySensorConfigViewModel.SetOptions(ioStateInformations, eleInitData.ProximitySensorID);
        }

        private static GKG.UI.General.CylinderConfigCfgInfo CopyCylinderCfgFromView(GKG.UI.General.CylinderConfigViewModel viewModel)
        {
            var cfg = new GKG.UI.General.CylinderConfigCfgInfo();
            viewModel.CopyTo(cfg);
            return cfg;
        }

        private void ApplyWithoutModified(Action action)
        {
            isApplyingData = true;
            try
            {
                action();
            }
            finally
            {
                isApplyingData = false;
            }
        }

        private static RailWorkStationSubMachineModulesInitCfg CloneData(RailWorkStationSubMachineModulesInitCfg data)
        {
            if (data == null)
            {
                return new RailWorkStationSubMachineModulesInitCfg();
            }

            return JsonObjConvert.FromJSon<RailWorkStationSubMachineModulesInitCfg>(
                JsonObjConvert.ToJSon(data)) ?? new RailWorkStationSubMachineModulesInitCfg();
        }

        private static GKG.UI.General.CylinderConfigCfgInfo BuildCylinderCfgFromInitParams(CylinderInitParameters cylinderParams)
        {
            cylinderParams ??= new CylinderInitParameters();
            var ioGuids = cylinderParams.IOStateGuidList ?? new List<Guid>();

            GKG.UI.General.HorizontalControlCardStateInitCfgInfo BuildModel(int index)
            {
                var channelId = string.Empty;
                if (index < ioGuids.Count && ioGuids[index] != Guid.Empty)
                {
                    channelId = ioGuids[index].ToString();
                }

                return new GKG.UI.General.HorizontalControlCardStateInitCfgInfo
                {
                    CardType = GKG.UI.General.ControlCardType.GC800,
                    channelID = channelId,
                };
            }

            var cfg = new GKG.UI.General.CylinderConfigCfgInfo
            {
                ConfigType = (GKG.UI.General.ECylinderType)cylinderParams.eCylinderType,
            };

            switch (cylinderParams.eCylinderType)
            {
                case GKG.ECylinderType.SingleControlSingleLimit:
                    cfg.SingleActSingleLimit = new GKG.UI.General.SingleControlSingleLimitCfgInfo
                    {
                        ControlModel = BuildModel(0),
                        LimitModel = BuildModel(1),
                        CylinderDelayModel = new GKG.UI.General.CylinderDelayCfgInfo
                        {
                            DelayNumeric = cylinderParams.CylinderDelay,
                        },
                    };
                    break;
                case GKG.ECylinderType.SingleControlDoubleLimit:
                    cfg.SingleActDoubleLimit = new GKG.UI.General.SingleControlDoubleLimitCfgInfo
                    {
                        ControlModel = BuildModel(0),
                        FirstLimitModel = BuildModel(1),
                        SecondLimitModel = BuildModel(2),
                        CylinderDelayModel = new GKG.UI.General.CylinderDelayCfgInfo
                        {
                            DelayNumeric = cylinderParams.CylinderDelay,
                        },
                    };
                    break;
                case GKG.ECylinderType.DoubleControlSingleLimit:
                    cfg.DoubleActSingleLimit = new GKG.UI.General.DoubleControlSingleLimitCfgInfo
                    {
                        FirstControlModel = BuildModel(0),
                        SecondControlModel = BuildModel(1),
                        LimitModel = BuildModel(2),
                        CylinderDelayModel = new GKG.UI.General.CylinderDelayCfgInfo
                        {
                            DelayNumeric = cylinderParams.CylinderDelay,
                        },
                    };
                    break;
                case GKG.ECylinderType.DoubleControlDoubleLimit:
                    cfg.DoubleActDoubleLimit = new GKG.UI.General.DoubleControlDoubleLimitCfgInfo
                    {
                        FirstControlModel = BuildModel(0),
                        SecondControlModel = BuildModel(1),
                        FirstLimitModel = BuildModel(2),
                        SecondLimitModel = BuildModel(3),
                        CylinderDelayModel = new GKG.UI.General.CylinderDelayCfgInfo
                        {
                            DelayNumeric = cylinderParams.CylinderDelay,
                        },
                    };
                    break;
            }

            return cfg;
        }

        private static CylinderInitParameters BuildCylinderInitParams(GKG.UI.General.CylinderConfigCfgInfo cfg)
        {
            var result = new CylinderInitParameters();
            if (cfg == null)
            {
                return result;
            }

            result.eCylinderType = (GKG.ECylinderType)cfg.ConfigType;
            result.CylinderDelay = ExtractCylinderDelay(cfg);
            result.IOStateGuidList = new List<Guid>();

            switch (cfg.ConfigType)
            {
                case GKG.UI.General.ECylinderType.SingleActSingleLimit:
                    AppendIoGuidSlot(result.IOStateGuidList, cfg.SingleActSingleLimit?.ControlModel);
                    AppendIoGuidSlot(result.IOStateGuidList, cfg.SingleActSingleLimit?.LimitModel);
                    break;
                case GKG.UI.General.ECylinderType.SingleActDoubleLimit:
                    AppendIoGuidSlot(result.IOStateGuidList, cfg.SingleActDoubleLimit?.ControlModel);
                    AppendIoGuidSlot(result.IOStateGuidList, cfg.SingleActDoubleLimit?.FirstLimitModel);
                    AppendIoGuidSlot(result.IOStateGuidList, cfg.SingleActDoubleLimit?.SecondLimitModel);
                    break;
                case GKG.UI.General.ECylinderType.DoubleActSingleLimit:
                    AppendIoGuidSlot(result.IOStateGuidList, cfg.DoubleActSingleLimit?.FirstControlModel);
                    AppendIoGuidSlot(result.IOStateGuidList, cfg.DoubleActSingleLimit?.SecondControlModel);
                    AppendIoGuidSlot(result.IOStateGuidList, cfg.DoubleActSingleLimit?.LimitModel);
                    break;
                case GKG.UI.General.ECylinderType.DoubleActDoubleLimit:
                    AppendIoGuidSlot(result.IOStateGuidList, cfg.DoubleActDoubleLimit?.FirstControlModel);
                    AppendIoGuidSlot(result.IOStateGuidList, cfg.DoubleActDoubleLimit?.SecondControlModel);
                    AppendIoGuidSlot(result.IOStateGuidList, cfg.DoubleActDoubleLimit?.FirstLimitModel);
                    AppendIoGuidSlot(result.IOStateGuidList, cfg.DoubleActDoubleLimit?.SecondLimitModel);
                    break;
            }

            return result;
        }

        public BackendWorkStationTransSpeedGearList GetTransSpeedGearList()
        {
            var list = new BackendWorkStationTransSpeedGearList();
            foreach (var item in TransSpeedGearItems)
            {
                if (!item.TryBuild(out var gear))
                {
                    continue;
                }

                list.Add(gear);
            }

            return list;
        }

        private static List<Guid> ExtractCylinderGuidList(GKG.UI.General.CylinderConfigCfgInfo cfg)
        {
            var values = new List<string>();

            static void AddValue(List<string> targets, GKG.UI.General.HorizontalControlCardStateInitCfgInfo model)
            {
                if (!string.IsNullOrWhiteSpace(model?.channelID))
                {
                    targets.Add(model.channelID);
                }
            }

            switch (cfg.ConfigType)
            {
                case GKG.UI.General.ECylinderType.SingleActSingleLimit:
                    AddValue(values, cfg.SingleActSingleLimit?.ControlModel);
                    AddValue(values, cfg.SingleActSingleLimit?.LimitModel);
                    break;
                case GKG.UI.General.ECylinderType.SingleActDoubleLimit:
                    AddValue(values, cfg.SingleActDoubleLimit?.ControlModel);
                    AddValue(values, cfg.SingleActDoubleLimit?.FirstLimitModel);
                    AddValue(values, cfg.SingleActDoubleLimit?.SecondLimitModel);
                    break;
                case GKG.UI.General.ECylinderType.DoubleActSingleLimit:
                    AddValue(values, cfg.DoubleActSingleLimit?.FirstControlModel);
                    AddValue(values, cfg.DoubleActSingleLimit?.SecondControlModel);
                    AddValue(values, cfg.DoubleActSingleLimit?.LimitModel);
                    break;
                case GKG.UI.General.ECylinderType.DoubleActDoubleLimit:
                    AddValue(values, cfg.DoubleActDoubleLimit?.FirstControlModel);
                    AddValue(values, cfg.DoubleActDoubleLimit?.SecondControlModel);
                    AddValue(values, cfg.DoubleActDoubleLimit?.FirstLimitModel);
                    AddValue(values, cfg.DoubleActDoubleLimit?.SecondLimitModel);
                    break;
            }

            return values
                .Select(ParseGuid)
                .Where(guid => guid != Guid.Empty)
                .ToList();
        }

        private static Guid ParseGuid(string value)
        {
            return Guid.TryParse(value, out var guid) ? guid : Guid.Empty;
        }

        private static void AppendIoGuid(List<Guid> ioGuids, GKG.UI.General.HorizontalControlCardStateInitCfgInfo model)
        {
            if (!Guid.TryParse(model?.channelID, out var ioGuid) || ioGuid == Guid.Empty)
            {
                return;
            }

            ioGuids.Add(ioGuid);
        }

        private static void AppendIoGuidSlot(List<Guid> ioGuids, GKG.UI.General.HorizontalControlCardStateInitCfgInfo model)
        {
            if (!Guid.TryParse(model?.channelID, out var ioGuid))
            {
                ioGuid = Guid.Empty;
            }

            ioGuids.Add(ioGuid);
        }

        private static int ParseIntOrDefault(string value, int defaultValue)
        {
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed) ? parsed : defaultValue;
        }

        private static string BuildSensorSummary(Guid sensorId)
        {
            return sensorId == Guid.Empty ? "未绑定ID" : $"已绑定: {sensorId}";
        }

        private static string BuildCylinderSummary(GKG.UI.General.CylinderConfigCfgInfo cfg)
        {
            if (cfg == null)
            {
                return "未配置";
            }

            var ioCount = ExtractCylinderGuidList(cfg).Count;
            return $"类型: {cfg.ConfigType}，IO数量: {ioCount}";
        }

        private static bool HasCylinderConfig(GKG.UI.General.CylinderConfigCfgInfo cfg)
        {
            return cfg != null && ExtractCylinderGuidList(cfg).Count > 0;
        }

        private void RaiseEleInitStateChanged(bool notifyModified = true)
        {
            this.RaisePropertyChanged(nameof(HasProximitySensor));
            this.RaisePropertyChanged(nameof(HasLeftSensor));
            this.RaisePropertyChanged(nameof(HasRightSensor));
            this.RaisePropertyChanged(nameof(HasLeftCylinder));
            this.RaisePropertyChanged(nameof(HasRightCylinder));
            this.RaisePropertyChanged(nameof(ProximitySensorSummary));
            this.RaisePropertyChanged(nameof(LeftSensorSummary));
            this.RaisePropertyChanged(nameof(RightSensorSummary));
            this.RaisePropertyChanged(nameof(LeftCylinderSummary));
            this.RaisePropertyChanged(nameof(RightCylinderSummary));

            if (notifyModified)
            {
                NotifyDataModified();
            }
        }

        private void NotifyDataModified()
        {
            if (isApplyingData)
            {
                return;
            }

            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        private void SetTransSpeedGearItems(BackendWorkStationTransSpeedGearList list)
        {
            foreach (var item in TransSpeedGearItems.ToList())
            {
                UnsubscribeTransSpeedGearItem(item);
            }

            TransSpeedGearItems.Clear();

            if (list != null)
            {
                foreach (var item in list)
                {
                    AddTransSpeedGearItem(item);
                }
            }

            if (TransSpeedGearItems.Count == 0)
            {
                AddTransSpeedGearItem();
            }
            RefreshTransSpeedGearItemIndexes();
            RaiseTransSpeedGearListChanged();
        }

        private static GKG.UI.General.ECylinderType ToUiCylinderType(GKG.ECylinderType cylinderType)
        {
            return cylinderType switch
            {
                GKG.ECylinderType.SingleControlSingleLimit => GKG.UI.General.ECylinderType.SingleActSingleLimit,
                GKG.ECylinderType.SingleControlDoubleLimit => GKG.UI.General.ECylinderType.SingleActDoubleLimit,
                GKG.ECylinderType.DoubleControlSingleLimit => GKG.UI.General.ECylinderType.DoubleActSingleLimit,
                GKG.ECylinderType.DoubleControlDoubleLimit => GKG.UI.General.ECylinderType.DoubleActDoubleLimit,
                _ => GKG.UI.General.ECylinderType.SingleActSingleLimit,
            };
        }

        private static GKG.ECylinderType ToBackendCylinderType(GKG.UI.General.ECylinderType cylinderType)
        {
            return cylinderType switch
            {
                GKG.UI.General.ECylinderType.SingleActSingleLimit => GKG.ECylinderType.SingleControlSingleLimit,
                GKG.UI.General.ECylinderType.SingleActDoubleLimit => GKG.ECylinderType.SingleControlDoubleLimit,
                GKG.UI.General.ECylinderType.DoubleActSingleLimit => GKG.ECylinderType.DoubleControlSingleLimit,
                GKG.UI.General.ECylinderType.DoubleActDoubleLimit => GKG.ECylinderType.DoubleControlDoubleLimit,
                _ => GKG.ECylinderType.SingleControlSingleLimit,
            };
        }

        public void SetViewReference(Control view)
        {
            viewReference = view;
            isInitializingView = true;
            try
            {
                ApplyWithoutModified(() =>
                {
                    LeftCylinderConfigViewModel.SetViewReference(view);
                    RightCylinderConfigViewModel.SetViewReference(view);

                    // Loading IO options will trigger internal ValueChanged events.
                    // Re-apply current data snapshot to avoid overwriting already loaded init parameters.
                    LeftCylinderConfigViewModel.SetIOChannelOptions(ioStateInformations);
                    RightCylinderConfigViewModel.SetIOChannelOptions(ioStateInformations);

                    LeftCylinderConfigViewModel.CopyFrom(eleInitData.LeftBlockCylinderParams ?? new GKG.UI.General.CylinderConfigCfgInfo());
                    RightCylinderConfigViewModel.CopyFrom(eleInitData.RightBlockCylinderParams ?? new GKG.UI.General.CylinderConfigCfgInfo());
                });
            }
            finally
            {
                isInitializingView = false;
            }
        }

        public void Dispose()
        {
            foreach (var item in TransSpeedGearItems.ToList())
            {
                UnsubscribeTransSpeedGearItem(item);
            }
            disposables.Dispose();
        }

        private void LoadFactoryParams()
        {
            if (callBack == null)
            {
                return;
            }

            try
            {
                var result = callBack.ExecConfigSvrCtlCmd(
                    RailWorkStationSubMachineModulesConst.GetFactoryParamsCmdID,
                    new GFBaseTypeParamValueList());

                var raw = result?["Result"]?.ToStringVal();
                if (string.IsNullOrWhiteSpace(raw))
                {
                    return;
                }

                factoryParams = JsonObjConvert.FromJSon<RailWorkStationSubMachineModulesFactoryCfg>(raw)
                    ?? new RailWorkStationSubMachineModulesFactoryCfg();
            }
            catch
            {
                factoryParams = new RailWorkStationSubMachineModulesFactoryCfg();
            }
        }

        private void RaiseFactoryVisibilityChanged()
        {
            this.RaisePropertyChanged(nameof(ShowProximitySensorConfig));
            this.RaisePropertyChanged(nameof(ShowLeftSensorConfig));
            this.RaisePropertyChanged(nameof(ShowRightSensorConfig));
            this.RaisePropertyChanged(nameof(ShowEleConfigSection));
            this.RaisePropertyChanged(nameof(ShowLeftBlockCylinderConfig));
            this.RaisePropertyChanged(nameof(ShowRightBlockCylinderConfig));
            this.RaisePropertyChanged(nameof(ShowBlockCylinderSection));
        }

        private void AddTransSpeedGearItem()
        {
            AddTransSpeedGearItem(new BackendWorkStationTransSpeedGear());
        }

        private void AddTransSpeedGearItem(BackendWorkStationTransSpeedGear gear)
        {
            var item = new TransSpeedGearItemViewModel(
                gear?.TransSpeed.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
                ReadOnly,
                RemoveTransSpeedGearItem);

            item.PropertyChanged += TransSpeedGearItem_PropertyChanged;
            TransSpeedGearItems.Add(item);
            RefreshTransSpeedGearItemIndexes();
            RaiseTransSpeedGearListChanged();
        }

        private void RemoveTransSpeedGearItem(TransSpeedGearItemViewModel item)
        {
            if (item == null || !TransSpeedGearItems.Contains(item))
            {
                return;
            }

            UnsubscribeTransSpeedGearItem(item);
            TransSpeedGearItems.Remove(item);
            RefreshTransSpeedGearItemIndexes();
            if (TransSpeedGearItems.Count == 0)
            {
                AddTransSpeedGearItem();
                return;
            }

            RaiseTransSpeedGearListChanged();
        }

        private void UnsubscribeTransSpeedGearItem(TransSpeedGearItemViewModel item)
        {
            item.PropertyChanged -= TransSpeedGearItem_PropertyChanged;
        }

        private void TransSpeedGearItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TransSpeedGearItemViewModel.SpeedText))
            {
                RaiseTransSpeedGearListChanged();
            }
        }

        private void RaiseTransSpeedGearListChanged()
        {
            this.RaisePropertyChanged(nameof(TransSpeedGearItems));
            if (!isApplyingData)
            {
                NotifyDataModified();
            }
        }

        private void RefreshTransSpeedGearItemIndexes()
        {
            for (int i = 0; i < TransSpeedGearItems.Count; i++)
            {
                TransSpeedGearItems[i].RowNumber = i + 1;
            }
        }

        internal class StationPreviewViewModel : ReactiveObject
        {
            private const string AssetsBaseUri = "avares://Griffins.CompUI.RWS/CompUI_RWS/PageType/InitCfgPage/WorkStationInitConfig/Images/";

            private static Bitmap LoadBitmap(string fileName)
            {
                try
                {
                    var uri = new Uri(AssetsBaseUri + fileName);
                    using Stream s = AssetLoader.Open(uri);
                    return new Bitmap(s);
                }
                catch
                {
                    return null;
                }
            }

            private static readonly Bitmap stationBmp = LoadBitmap("ctr-Station.png");
            private static readonly Bitmap stationActiveBmp = LoadBitmap("ctr-Station-active.png");
            private static readonly Bitmap noBmp = LoadBitmap("ctr-No.png");
            private static readonly Bitmap yesBmp = LoadBitmap("ctr-Yes.png");

            private bool hasBoard;
            public bool HasBoard
            {
                get => hasBoard;
                set
                {
                    this.RaiseAndSetIfChanged(ref hasBoard, value);
                    UpdateStationImage();
                }
            }

            private bool leftHasBoard;
            public bool LeftHasBoard
            {
                get => leftHasBoard;
                set
                {
                    this.RaiseAndSetIfChanged(ref leftHasBoard, value);
                    UpdateLeftSensorImage();
                }
            }

            private bool rightHasBoard;
            public bool RightHasBoard
            {
                get => rightHasBoard;
                set
                {
                    this.RaiseAndSetIfChanged(ref rightHasBoard, value);
                    UpdateRightSensorImage();
                }
            }

            private bool showSensors = true;
            public bool ShowSensors
            {
                get => showSensors;
                set
                {
                    this.RaiseAndSetIfChanged(ref showSensors, value);
                    this.RaisePropertyChanged(nameof(LeftSensorVisible));
                    this.RaisePropertyChanged(nameof(RightSensorVisible));
                }
            }

            private bool showLeftSensorImage = true;
            public bool ShowLeftSensorImage
            {
                get => showLeftSensorImage;
                set
                {
                    this.RaiseAndSetIfChanged(ref showLeftSensorImage, value);
                    this.RaisePropertyChanged(nameof(LeftSensorVisible));
                }
            }

            private bool showRightSensorImage = true;
            public bool ShowRightSensorImage
            {
                get => showRightSensorImage;
                set
                {
                    this.RaiseAndSetIfChanged(ref showRightSensorImage, value);
                    this.RaisePropertyChanged(nameof(RightSensorVisible));
                }
            }

            public bool LeftSensorVisible => ShowSensors && ShowLeftSensorImage;
            public bool RightSensorVisible => ShowSensors && ShowRightSensorImage;

            private Bitmap leftSensorImage;
            public Bitmap LeftSensorImage
            {
                get => leftSensorImage;
                private set => this.RaiseAndSetIfChanged(ref leftSensorImage, value);
            }

            private Bitmap stationImage;
            public Bitmap StationImage
            {
                get => stationImage;
                private set => this.RaiseAndSetIfChanged(ref stationImage, value);
            }

            private Bitmap rightSensorImage;
            public Bitmap RightSensorImage
            {
                get => rightSensorImage;
                private set => this.RaiseAndSetIfChanged(ref rightSensorImage, value);
            }

            private void UpdateLeftSensorImage()
            {
                LeftSensorImage = LeftHasBoard ? yesBmp : noBmp;
            }

            private void UpdateRightSensorImage()
            {
                RightSensorImage = RightHasBoard ? yesBmp : noBmp;
            }

            private void UpdateStationImage()
            {
                StationImage = HasBoard ? (stationActiveBmp ?? stationBmp) : stationBmp;
            }

            public StationPreviewViewModel()
            {
                StationImage = stationBmp;
                LeftSensorImage = noBmp;
                RightSensorImage = noBmp;
            }
        }

        internal class TransSpeedGearItemViewModel : ReactiveObject
        {
            private readonly Action<TransSpeedGearItemViewModel> removeAction;
            private string speedText;
            private bool isReadOnly;
            private int rowNumber;

            public int RowNumber
            {
                get => rowNumber;
                set => this.RaiseAndSetIfChanged(ref rowNumber, value);
            }

            public string SpeedText
            {
                get => speedText;
                set
                {
                    this.RaiseAndSetIfChanged(ref speedText, value);
                    this.RaisePropertyChanged(nameof(SpeedNumeric));
                }
            }

            public bool IsReadOnly
            {
                get => isReadOnly;
                set
                {
                    this.RaiseAndSetIfChanged(ref isReadOnly, value);
                    this.RaisePropertyChanged(nameof(CanEdit));
                }
            }

            public bool CanEdit => !IsReadOnly;

            public decimal SpeedNumeric
            {
                get => double.TryParse(SpeedText, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var value) ? (decimal)value : 0m;
                set
                {
                    var normalized = value.ToString("0.###", CultureInfo.InvariantCulture);
                    if (SpeedText != normalized)
                    {
                        SpeedText = normalized;
                    }
                }
            }

            public ReactiveCommand<Unit, Unit> RemoveCommand { get; }

            public TransSpeedGearItemViewModel(string speedText, bool isReadOnly, Action<TransSpeedGearItemViewModel> removeAction)
            {
                this.speedText = speedText;
                this.isReadOnly = isReadOnly;
                this.removeAction = removeAction;
                RemoveCommand = ReactiveCommand.Create(() => this.removeAction?.Invoke(this));
            }

            public bool TryBuild(out BackendWorkStationTransSpeedGear gear)
            {
                gear = null;

                if (RowNumber <= 0)
                {
                    return false;
                }

                if (!double.TryParse(SpeedText, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var speed))
                {
                    return false;
                }

                gear = new BackendWorkStationTransSpeedGear
                {
                    TransSpeedGear = RowNumber,
                    TransSpeed = speed,
                };
                return true;
            }
        }

        private void OnConfigLeftSensor()
        {
            if (!isDesign)
            {
                ShowSensorConfigDialog(true);
            }
        }

        private void OnConfigProximitySensor()
        {
            if (!isDesign)
            {
                ShowSensorConfigDialog(null);
            }
        }

        private void OnConfigRightSensor()
        {
            if (!isDesign)
            {
                ShowSensorConfigDialog(false);
            }
        }

        private void OnConfigLeftCylinder()
        {
            if (!isDesign)
            {
                ShowCylinderConfigDialog(true);
            }
        }

        private void OnConfigRightCylinder()
        {
            if (!isDesign)
            {
                ShowCylinderConfigDialog(false);
            }
        }

        private void ShowSensorConfigDialog(bool? isLeft)
        {
            if (viewReference == null)
            {
                return;
            }

            var selectedGuid = isLeft switch
            {
                true => eleInitData.LeftSensorID,
                false => eleInitData.RightSensorID,
                null => eleInitData.ProximitySensorID,
            };

            var cfgVm = new SensorConfigViewModel(ioStateInformations, selectedGuid);
            var cfgView = new SensorConfigView
            {
                DataContext = cfgVm,
            };

            var ownerWindow = TopLevel.GetTopLevel(viewReference) as Window;
            var window = new Window
            {
                Title = isLeft switch
                {
                    true => "左感应器配置",
                    false => "右感应器配置",
                    null => "接近感应器配置",
                },
                Width = 650,
                Height = 220,
                Padding = new Thickness(10),
                Content = cfgView,
                WindowStartupLocation = ownerWindow != null ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen,
            };

            window.Closed += (_, __) =>
            {
                var ioGuid = cfgVm.GetSelectedGuid();
                switch (isLeft)
                {
                    case true:
                        eleInitData.LeftSensorID = ioGuid;
                        break;
                    case false:
                        eleInitData.RightSensorID = ioGuid;
                        break;
                    default:
                        eleInitData.ProximitySensorID = ioGuid;
                        break;
                }
                RaiseEleInitStateChanged();
            };

            if (ownerWindow != null)
            {
                _ = window.ShowDialog(ownerWindow);
            }
            else
            {
                window.Show();
            }
        }

        private void ShowCylinderConfigDialog(bool isLeft)
        {
            if (viewReference == null)
            {
                return;
            }

            var cfgModel = isLeft ? eleInitData.LeftBlockCylinderParams : eleInitData.RightBlockCylinderParams;
            cfgModel ??= new GKG.UI.General.CylinderConfigCfgInfo();

            var cfgVm = new GKG.UI.General.CylinderConfigViewModel();
            cfgVm.SetViewReference(viewReference);
            cfgVm.SetIOChannelOptions(ioStateInformations);
            cfgVm.CopyFrom(cfgModel);

            var cfgView = new GKG.UI.General.CylinderConfigView
            {
                DataContext = cfgVm,
            };

            var ownerWindow = TopLevel.GetTopLevel(viewReference) as Window;
            var window = new Window
            {
                Title = "气缸配置",
                Width = 900,
                Height = 650,
                Padding = new Thickness(10),
                Content = cfgView,
                WindowStartupLocation = ownerWindow != null ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen,
            };

            window.Closed += (_, __) =>
            {
                cfgVm.CopyTo(cfgModel);
                if (isLeft)
                {
                    eleInitData.LeftBlockCylinderParams = cfgModel;
                }
                else
                {
                    eleInitData.RightBlockCylinderParams = cfgModel;
                }
                RaiseEleInitStateChanged();
            };

            if (ownerWindow != null)
            {
                _ = window.ShowDialog(ownerWindow);
            }
            else
            {
                window.Show();
            }
        }

        private static int ExtractCylinderDelay(GKG.UI.General.CylinderConfigCfgInfo cfg)
        {
            if (cfg == null)
            {
                return 100;
            }

            decimal? delay = cfg.ConfigType switch
            {
                GKG.UI.General.ECylinderType.SingleActSingleLimit => cfg.SingleActSingleLimit?.CylinderDelayModel?.DelayNumeric,
                GKG.UI.General.ECylinderType.SingleActDoubleLimit => cfg.SingleActDoubleLimit?.CylinderDelayModel?.DelayNumeric,
                GKG.UI.General.ECylinderType.DoubleActSingleLimit => cfg.DoubleActSingleLimit?.CylinderDelayModel?.DelayNumeric,
                GKG.UI.General.ECylinderType.DoubleActDoubleLimit => cfg.DoubleActDoubleLimit?.CylinderDelayModel?.DelayNumeric,
                _ => null,
            };

            return decimal.ToInt32(delay ?? 100m);
        }

        private static void ApplyCylinderDelay(GKG.UI.General.CylinderConfigCfgInfo cfg, int cylinderDelay)
        {
            if (cfg == null)
            {
                return;
            }

            switch (cfg.ConfigType)
            {
                case GKG.UI.General.ECylinderType.SingleActSingleLimit:
                    cfg.SingleActSingleLimit ??= new GKG.UI.General.SingleControlSingleLimitCfgInfo();
                    cfg.SingleActSingleLimit.CylinderDelayModel ??= new GKG.UI.General.CylinderDelayCfgInfo();
                    cfg.SingleActSingleLimit.CylinderDelayModel.DelayNumeric = cylinderDelay;
                    break;
                case GKG.UI.General.ECylinderType.SingleActDoubleLimit:
                    cfg.SingleActDoubleLimit ??= new GKG.UI.General.SingleControlDoubleLimitCfgInfo();
                    cfg.SingleActDoubleLimit.CylinderDelayModel ??= new GKG.UI.General.CylinderDelayCfgInfo();
                    cfg.SingleActDoubleLimit.CylinderDelayModel.DelayNumeric = cylinderDelay;
                    break;
                case GKG.UI.General.ECylinderType.DoubleActSingleLimit:
                    cfg.DoubleActSingleLimit ??= new GKG.UI.General.DoubleControlSingleLimitCfgInfo();
                    cfg.DoubleActSingleLimit.CylinderDelayModel ??= new GKG.UI.General.CylinderDelayCfgInfo();
                    cfg.DoubleActSingleLimit.CylinderDelayModel.DelayNumeric = cylinderDelay;
                    break;
                case GKG.UI.General.ECylinderType.DoubleActDoubleLimit:
                    cfg.DoubleActDoubleLimit ??= new GKG.UI.General.DoubleControlDoubleLimitCfgInfo();
                    cfg.DoubleActDoubleLimit.CylinderDelayModel ??= new GKG.UI.General.CylinderDelayCfgInfo();
                    cfg.DoubleActDoubleLimit.CylinderDelayModel.DelayNumeric = cylinderDelay;
                    break;
            }
        }

        private sealed class FrontendWorkStationEleInitParams
        {
            public Guid LeftSensorID { get; set; } = Guid.Empty;

            public Guid RightSensorID { get; set; } = Guid.Empty;

            public Guid ProximitySensorID { get; set; } = Guid.Empty;

            public GKG.UI.General.CylinderConfigCfgInfo LeftBlockCylinderParams { get; set; }

            public GKG.UI.General.CylinderConfigCfgInfo RightBlockCylinderParams { get; set; }
        }
    }
}
