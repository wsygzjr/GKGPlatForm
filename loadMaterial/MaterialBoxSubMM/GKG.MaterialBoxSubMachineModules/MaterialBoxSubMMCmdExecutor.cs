using GF_Gereric;
using GKG.ElectronicControl;
using GKG.ElectronicControl.General;
using GKG.MaterialBoxSubMachineModules.Common;
using GKG.MM;
using GKG.MotionControl;
using GKG.SubMM.StorageDeviceModule;
using GKG.SubMM.TransportMechanismModule;
using Griffins;
using Griffins.ImeIOT;
using Griffins.PF;
//using Griffins.PF.RichClient;
using Griffins.PF.Server;
using MaterialBoxSubMachineModules.FeedPort;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GKG
{
    namespace SubMM
    {
        /// <summary>
        /// 料盒子机械模组执行器。
        /// 统一承载料盒、储料装置、运输机构三类对象的对外方法、运行时命令和事件上卷。
        /// </summary>
        /// <remarks>整体说明见 loadmaterial/EXECUTOR_IMPLEMENTATION.md §2。</remarks>
        public class MaterialBoxSubMMCmdExecutor : ISubMMCmdExecutor, ISubMMManualModeCmdExecutor, ISubMMAutoModeCmdExecutor
        {
            /// <summary>
            /// 储料装置执行上下文：
            /// 把 MaterialRack 解析成“具体储料对象 + 对象内索引”。
            /// </summary>
            private sealed class RunningContext
            {
                public int Rack { get; set; }

                public int StorageIndex { get; set; }

                public StorageDevice StorageDevice { get; set; }
                public TransportMechanism TransportMechanism { get; set; }
                public FeedPort feedingPort { get; set; }
            }

            private enum LayerMoveMode
            {
                FirstSlot,
                LastSlot,
                NextAvailableSlot,
                InitialPosition
            }

            private const int LoadUpperRack = 0;
            private const int LoadLowerRack = 1;
            private const int UnloadUpperRack = 2;
            private const int UnloadLowerRack = 3;
            private const string LoadAxisInformType = MaterialBoxSubMachineModulesConst.LoadAxisInformType;
            private const string UnloadAxisInformType = MaterialBoxSubMachineModulesConst.UnloadAxisInformType;
            private const string MaterialContainerName = MaterialBoxSubMachineModulesConst.MaterialContainerName;
            private const string LoadStorageDeviceName = MaterialBoxSubMachineModulesConst.LoadStorageDeviceName;
            private const string UnloadStorageDeviceName = MaterialBoxSubMachineModulesConst.UnloadStorageDeviceName;


            private MaterialBoxSubMachineModulesFactoryCfg materialBoxSubMachineModulesFactoryCfg;
            private MaterialBoxSubMachineModulesInitCfg materialBoxSubMachineModulesInitCfg;

            private MaterialBoxSubMachineModulesPPCfg materialBoxSubMachineModulesPPCfg;

            private ISubMMCmdExecutorCallBack iSubMMCmdExecutorCallBack;

            private SubMMAlias alias;

            private ImeGenNormalEventHandler imeGenNormalEventHandler;

            private ImeCabilityEventHandler imeCabilityEventHandler;

            private ImeAlarmEventHandler imeAlarmEventHandler;

            private RunningContext loadContext;

            private RunningContext unloadContext;

            /// <summary>
            /// 缓存双 Z 轴当前位置，供运行时命令直接返回给前端
            /// </summary>
            private readonly Dictionary<int, double> axisPositionCache = new Dictionary<int, double>();

            /// <summary>
            /// 保护位置缓存，避免驱动事件与前端读取并发冲突
            /// </summary>
            private readonly object axisPositionLock = new object();

            private AxisInformation loadZAxisInfo;

            private AxisInformation unloadZAxisInfo;

            private const int LoadZLogicAxis = 0;

            private const int UnloadZLogicAxis = 1;

            public MaterialBoxSubMMCmdExecutor(SubMMAlias alias, Guid subMMObjID, byte[] factoryCfg)
            {
                this.alias = alias;
                if (factoryCfg != null && factoryCfg.Length > 0)
                    materialBoxSubMachineModulesFactoryCfg = JsonObjConvert.FromJSonBytes<MaterialBoxSubMachineModulesFactoryCfg>(factoryCfg);
                materialBoxSubMachineModulesFactoryCfg ??= new MaterialBoxSubMachineModulesFactoryCfg();
                materialBoxSubMachineModulesInitCfg = new MaterialBoxSubMachineModulesInitCfg();

                loadContext = new RunningContext();
                unloadContext = new RunningContext();
                loadContext.StorageDevice = new StorageDevice(materialBoxSubMachineModulesFactoryCfg.LoadStorageDevice, LoadStorageDeviceName);
                unloadContext.StorageDevice = new StorageDevice(materialBoxSubMachineModulesFactoryCfg.UnloadStorageDevice, UnloadStorageDeviceName);
                loadContext.TransportMechanism = new TransportMechanism(materialBoxSubMachineModulesFactoryCfg.LoadTransportMechanism);
                unloadContext.TransportMechanism = new TransportMechanism(materialBoxSubMachineModulesFactoryCfg.UnloadTransportMechanism);
                loadContext.feedingPort = new FeedPort(materialBoxSubMachineModulesFactoryCfg.FeedingPortFactoryCfg);
                unloadContext.feedingPort = new FeedPort(materialBoxSubMachineModulesFactoryCfg.ReceivePortFactoryCfg);
            }

            #region Framework Events And Init
            event ImeGenNormalEventHandler ISubMMAutoModeCmdExecutor.GenNormalEvent
            {
                add
                {
                    imeGenNormalEventHandler += value;
                }

                remove
                {
                    imeGenNormalEventHandler -= value;
                }
            }

            event ImeCabilityEventHandler ISubMMAutoModeCmdExecutor.CabilityEvent
            {
                add
                {
                    imeCabilityEventHandler += value;
                }

                remove
                {
                    imeCabilityEventHandler -= value;
                }
            }

            event ImeAlarmEventHandler ISubMMAutoModeCmdExecutor.AlarmEvent
            {
                add
                {
                    imeAlarmEventHandler += value;
                }

                remove
                {
                    imeAlarmEventHandler -= value;
                }
            }
            /// <summary>Init：运输机构 Init → 绑定 Z 轴 → 绑定储料/料口 IO → 校验绑定。</summary>
            void ISubMMCmdExecutor.Init(byte[] initCfgInfo, byte[] calibrationCfgInfo, ISubMMCmdExecutorCallBack callBack)
            {

                if (initCfgInfo != null && initCfgInfo.Length > 0)
                {
                    materialBoxSubMachineModulesInitCfg = new MaterialBoxSubMachineModulesInitCfg();
                    materialBoxSubMachineModulesInitCfg.FromBytes(initCfgInfo);// = JsonObjConvert.FromJSonBytes<MaterialBoxSubMachineModulesInitCfg>(initCfgInfo);
                }

                iSubMMCmdExecutorCallBack = callBack;
            }
            void ISubMMCmdExecutor.BeforeInit(GFBaseTypePropValueList devicePropValues)
            {

            }
            void ISubMMCmdExecutor.AfterInit()
            {
                ////初始化私有对象
                InitPrivateObj();
                AttachObjectEvents();
            }
            void ISubMMCmdExecutor.UnInit()
            {
                DetachObjectEvents();
            }

            void ISubMMAutoModeCmdExecutor.SetRunMode(ImeRunMode imeRunMode)
            {

            }

            ISubMMManualModeCmdExecutor ISubMMCmdExecutor.GetSubMMManualModeCmdExecutor()
            {
                return this;
            }

            ISubMMAutoModeCmdExecutor ISubMMCmdExecutor.GetSubMMAutoModeCmdExecutor()
            {
                return this;
            }

            #endregion

            bool ISubMMAutoModeCmdExecutor.CanStartWork(out string reasonMsg)
            {
                reasonMsg = string.Empty;
                return true;
            }
            /// <summary>
            /// 统一订阅对象级事件。
            /// 对象内部只关心自身事件，这里负责汇总后上卷给框架。
            /// </summary>
            private void AttachObjectEvents()
            {
                loadContext.StorageDevice.MaterialWarning += OnLoadStorageMaterialWarning;
                loadContext.StorageDevice.MaterialEmpty += OnLoadStorageMaterialEmpty;
                loadContext.StorageDevice.StretchFinished += OnLoadStorageStretchFinished;
                loadContext.StorageDevice.RetractFinished += OnLoadStorageRetractFinished;

                unloadContext.StorageDevice.MaterialWarning += OnUnloadStorageMaterialWarning;
                unloadContext.StorageDevice.MaterialEmpty += OnUnloadStorageMaterialEmpty;
                unloadContext.StorageDevice.StretchFinished += OnUnloadStorageStretchFinished;
                unloadContext.StorageDevice.RetractFinished += OnUnloadStorageRetractFinished;

                loadContext.TransportMechanism.PositionChanged += OnLoadTransportPositionChanged;
                loadContext.TransportMechanism.MoveFinished += OnLoadTransportMoveFinished;

                unloadContext.TransportMechanism.PositionChanged += OnUnloadTransportPositionChanged;
                unloadContext.TransportMechanism.MoveFinished += OnUnloadTransportMoveFinished;
            }
            /// <summary>
            /// 取消订阅对象事件，避免执行器卸载时事件悬挂导致的异常。
            /// </summary>
            private void DetachObjectEvents()
            {
                loadContext.StorageDevice.MaterialWarning -= OnLoadStorageMaterialWarning;
                loadContext.StorageDevice.MaterialEmpty -= OnLoadStorageMaterialEmpty;
                loadContext.StorageDevice.StretchFinished -= OnLoadStorageStretchFinished;
                loadContext.StorageDevice.RetractFinished -= OnLoadStorageRetractFinished;
                unloadContext.StorageDevice.MaterialWarning -= OnUnloadStorageMaterialWarning;
                unloadContext.StorageDevice.MaterialEmpty -= OnUnloadStorageMaterialEmpty;
                unloadContext.StorageDevice.StretchFinished -= OnUnloadStorageStretchFinished;
                unloadContext.StorageDevice.RetractFinished -= OnUnloadStorageRetractFinished;
                loadContext.TransportMechanism.PositionChanged -= OnLoadTransportPositionChanged;
                loadContext.TransportMechanism.MoveFinished -= OnLoadTransportMoveFinished;
                unloadContext.TransportMechanism.PositionChanged -= OnUnloadTransportPositionChanged;
                unloadContext.TransportMechanism.MoveFinished -= OnUnloadTransportMoveFinished;
            }

            /// <summary>
            /// 料盒子模组的 IO 绑定入口：
            /// 从 materialBoxSubMachineModulesInitCfg 中读取界面配置好的 IO GUID，
            /// 再由 StorageDevice / FeedPort 内部通过 StateIOInstancesByIdsRequest
            /// 向电器管理换取对应的 IO 实例并完成绑定。
            /// </summary>
            private void InitPrivateObj()
            {
                foreach (var obj in materialBoxSubMachineModulesInitCfg.LoadStorageDevice.StorageMechanism)
                {
                    obj.IsFeeding = true;
                }
                foreach (var obj in materialBoxSubMachineModulesInitCfg.UnloadStorageDevice.StorageMechanism)
                {
                    obj.IsFeeding = false;
                }
                loadContext.StorageDevice.Init(materialBoxSubMachineModulesInitCfg.LoadStorageDevice, GetStateIOInstancesByIds);
                unloadContext.StorageDevice.Init(materialBoxSubMachineModulesInitCfg.UnloadStorageDevice, GetStateIOInstancesByIds);

                loadContext.TransportMechanism.Init(materialBoxSubMachineModulesInitCfg.LoadTransportMechanism, GetRobotDriverById);
                unloadContext.TransportMechanism.Init(materialBoxSubMachineModulesInitCfg.UnloadTransportMechanism, GetRobotDriverById);

                loadContext.TransportMechanism.PositionChanged += OnAxisPositionChanged;
                unloadContext.TransportMechanism.PositionChanged += OnAxisPositionChanged;

                loadContext.feedingPort.Init(materialBoxSubMachineModulesInitCfg.FeedingPortInitCfg, GetStateIOInstancesByIds);
                unloadContext.feedingPort.Init(materialBoxSubMachineModulesInitCfg.ReceivePortInitCfg, GetStateIOInstancesByIds);
            }

            /// <summary>通过电器管理互消息按 GUID 批量换取 IO 实例。</summary>
            private static List<IBaseStateIO> GetStateIOInstancesByIds(List<Guid> ioGuids)
            {
                if (ioGuids == null || ioGuids.Count == 0)
                    return new List<IBaseStateIO>();

                var response = ServerInnerInfoSender.SendMutualInfo(
                    StateIOInstancesByIdsRequest.InfoKindID,
                    new StateIOInstancesByIdsRequest(ioGuids));
                if (response == null || response.Count == 0)
                    throw new InvalidOperationException("通过交互消息获取IO实例失败。");

                StateIOInstancesByIdsResponse ioResponse = response[0].Response as StateIOInstancesByIdsResponse;
                return ioResponse?.StateIOInstances ?? new List<IBaseStateIO>();
            }

            /// <summary>单轴绑定：取驱动、挂 PositionChanged、写入运输机构。</summary>
            private IRobotDriver GetRobotDriverById(Guid axisId)
            {
                if (axisId == Guid.Empty)
                    return null;

                var robotDriverResponse = ServerInnerInfoSender.SendMutualInfo(
                    RobotDriverByAxisIdsRequest.InfoKindID,
                    new RobotDriverByAxisIdsRequest(new List<Guid> { axisId }));

                if (robotDriverResponse == null || robotDriverResponse.Count == 0)
                    throw new InvalidOperationException($"通过交互消息获取机械手实例失败。");

                RobotDriverByAxisIdsResponse robotResponse = robotDriverResponse[0].Response as RobotDriverByAxisIdsResponse;
                if (robotResponse?.RobotDriver == null)
                    throw new InvalidOperationException($"电器管理返回的机械手实例为空。");

                return robotResponse.RobotDriver;
            }

            private void OnAxisPositionChanged(object sender, PositionChangedEventArgs e)
            {
                if (e?.NewPosition == null || e.NewPosition.Length == 0)
                    return;

                // 更新到本地缓存
                int logicAxis = (sender == loadContext.TransportMechanism) ? LoadZLogicAxis : UnloadZLogicAxis;
                double currentPosition = e.NewPosition[0].PositionValue;
                lock (axisPositionLock)
                {
                    axisPositionCache[logicAxis] = currentPosition;
                }

                // 上传到UI
                string axisType = GetAxisInformType(logicAxis);
                if (string.IsNullOrWhiteSpace(axisType))
                    return;

                if (iSubMMCmdExecutorCallBack == null)
                    return;

                var axisStatus = new MaterialBoxAxisStatus
                {
                    Staus = 0,
                    Type = axisType,
                    Position = currentPosition
                };
                string paramJson = JsonObjConvert.ToJSon(axisStatus);

                iSubMMCmdExecutorCallBack.SendToMapTmlStateChanged(paramJson);
            }

            private static string GetAxisInformType(int logicAxis)
            {
                if (logicAxis == LoadZLogicAxis)
                    return LoadAxisInformType;

                if (logicAxis == UnloadZLogicAxis)
                    return UnloadAxisInformType;

                return string.Empty;
            }

            private static List<AxisInformation> GetAllAxisInfosFromElectronicManager()
            {
                Dictionary<Guid, AxisInformation> axisInfoDict = new Dictionary<Guid, AxisInformation>();
                foreach (MotionControlCardType cardType in Enum.GetValues(typeof(MotionControlCardType)))
                {
                    var axisInfosResponse = ServerInnerInfoSender.SendMutualInfo(
                        AxisInfosRequest.InfoKindID,
                        new AxisInfosRequest(cardType));
                    if (axisInfosResponse == null || axisInfosResponse.Count == 0)
                        continue;
                    AxisInfosResponse response = axisInfosResponse[0].Response as AxisInfosResponse;
                    if (response?.AxisInformations == null)
                        continue;

                    foreach (AxisInformation axisInfo in response.AxisInformations)
                    {
                        if (axisInfo != null && axisInfo.AxisGuid != Guid.Empty)
                            axisInfoDict[axisInfo.AxisGuid] = axisInfo;
                    }
                }

                return axisInfoDict.Values.ToList();
            }

            private void OnLoadStorageMaterialWarning(object sender, StorageDeviceEventArgs e)
            {
                HandleStorageEvent(true, "MaterialWarning", e?.materialBoxIndex ?? -1, -1);
            }

            private void OnLoadStorageMaterialEmpty(object sender, StorageDeviceEventArgs e)
            {
                HandleStorageEvent(true, "MaterialEmpty", e?.materialBoxIndex ?? -1, -1);
            }

            private void OnLoadStorageStretchFinished(object sender, StorageDeviceCylinderEventArgs e)
            {
                HandleStorageEvent(true, "StretchFinished", e?.materialBoxIndex ?? -1, e?.CylinderIndex ?? -1);
            }

            private void OnLoadStorageRetractFinished(object sender, StorageDeviceCylinderEventArgs e)
            {
                HandleStorageEvent(true, "RetractFinished", e?.materialBoxIndex ?? -1, e?.CylinderIndex ?? -1);
            }

            private void OnUnloadStorageMaterialWarning(object sender, StorageDeviceEventArgs e)
            {
                HandleStorageEvent(false, "MaterialWarning", e?.materialBoxIndex ?? -1, -1);
            }

            private void OnUnloadStorageMaterialEmpty(object sender, StorageDeviceEventArgs e)
            {
                HandleStorageEvent(false, "MaterialEmpty", e?.materialBoxIndex ?? -1, -1);
            }

            private void OnUnloadStorageStretchFinished(object sender, StorageDeviceCylinderEventArgs e)
            {
                HandleStorageEvent(false, "StretchFinished", e?.materialBoxIndex ?? -1, e?.CylinderIndex ?? -1);
            }

            private void OnUnloadStorageRetractFinished(object sender, StorageDeviceCylinderEventArgs e)
            {
                HandleStorageEvent(false, "RetractFinished", e?.materialBoxIndex ?? -1, e?.CylinderIndex ?? -1);
            }

            private void OnLoadTransportPositionChanged(object sender, PositionChangedEventArgs e)
            {
                HandleTransportEvent(true, "PositionChanged", e?.NewPosition[0].PositionValue ?? 0);
            }

            private void OnLoadTransportMoveFinished(object sender, EventArgs e)
            {
                HandleTransportEvent(true, "MoveFinished", 0);
            }

            private void OnUnloadTransportPositionChanged(object sender, PositionChangedEventArgs e)
            {
                HandleTransportEvent(false, "PositionChanged", e?.NewPosition[0].PositionValue ?? 0);
            }

            private void OnUnloadTransportMoveFinished(object sender, EventArgs e)
            {
                HandleTransportEvent(false, "MoveFinished", 0);
            }

            /// <summary>储料装置事件上卷：映射 EventID 并附带 rack/storage/cylinder 参数。</summary>
            private void HandleStorageEvent(bool isLoadSide, string eventName, int storageIndex, int cylinderIndex)
            {
                string eventID = GetStorageEventID(isLoadSide, eventName);
                if (string.IsNullOrWhiteSpace(eventID))
                    return;

                RaiseCabilityEvent(
                    eventID,
                    ("Side", isLoadSide ? "Load" : "Unload"),
                    ("StorageIndex", storageIndex),
                    ("MaterialRack", GetMaterialRackByStorageIndex(isLoadSide, storageIndex)),
                    ("CylinderIndex", cylinderIndex),
                    ("EventName", eventName ?? string.Empty));
            }

            /// <summary>运输机构事件上卷：位置变化/移动完成 → 能力事件。</summary>
            private void HandleTransportEvent(bool isLoadSide, string eventName, double position)
            {
                string eventID = GetTransportEventID(isLoadSide, eventName);
                if (string.IsNullOrWhiteSpace(eventID))
                    return;

                int logicAxis = isLoadSide ? LoadZLogicAxis : UnloadZLogicAxis;
                AxisInformation axisInfo = isLoadSide ? loadZAxisInfo : unloadZAxisInfo;

                RaiseCabilityEvent(
                    eventID,
                    ("Side", isLoadSide ? "Load" : "Unload"),
                    ("ZAxisSelect", logicAxis),
                    ("LogicAxis", logicAxis),
                    ("AxisGuid", axisInfo?.AxisGuid ?? Guid.Empty),
                    ("Position", position),
                    ("EventName", eventName ?? string.Empty));
            }

            /// <summary>触发框架能力事件回调（供总控/界面订阅）。</summary>
            private void RaiseCabilityEvent(string eventID, params (string Key, object Value)[] values)
            {
                if (imeCabilityEventHandler == null || string.IsNullOrWhiteSpace(eventID))
                    return;

                GFBaseTypeParamValueList eventParam = new GFBaseTypeParamValueList();
                foreach ((string Key, object Value) in values)
                {
                    eventParam.Add(new GFBaseTypeParamValue(Key, new GriffinsBaseValue(Value)));
                }

                imeCabilityEventHandler.Invoke(this, new ImeCabilityEventArgs(eventID, eventParam));
            }

            /// <summary>储料内部事件名 → 对外 EventID 常量。</summary>
            private static string GetStorageEventID(bool isLoadSide, string eventName)
            {
                if (isLoadSide)
                {
                    return eventName switch
                    {
                        "MaterialWarning" => MaterialBoxSubMachineModulesConst.EventLoadStorageMaterialWarning,
                        "MaterialEmpty" => MaterialBoxSubMachineModulesConst.EventLoadStorageMaterialEmpty,
                        "StretchFinished" => MaterialBoxSubMachineModulesConst.EventLoadStorageStretchFinished,
                        "RetractFinished" => MaterialBoxSubMachineModulesConst.EventLoadStorageRetractFinished,
                        _ => string.Empty
                    };
                }

                return eventName switch
                {
                    "MaterialWarning" => MaterialBoxSubMachineModulesConst.EventUnloadStorageMaterialWarning,
                    "MaterialEmpty" => MaterialBoxSubMachineModulesConst.EventUnloadStorageMaterialEmpty,
                    "StretchFinished" => MaterialBoxSubMachineModulesConst.EventUnloadStorageStretchFinished,
                    "RetractFinished" => MaterialBoxSubMachineModulesConst.EventUnloadStorageRetractFinished,
                    _ => string.Empty
                };
            }

            /// <summary>运输内部事件名 → 对外 EventID 常量。</summary>
            private static string GetTransportEventID(bool isLoadSide, string eventName)
            {
                if (isLoadSide)
                {
                    return eventName switch
                    {
                        "PositionChanged" => MaterialBoxSubMachineModulesConst.EventLoadTransportPositionChanged,
                        "MoveFinished" => MaterialBoxSubMachineModulesConst.EventLoadTransportMoveFinished,
                        _ => string.Empty
                    };
                }

                return eventName switch
                {
                    "PositionChanged" => MaterialBoxSubMachineModulesConst.EventUnloadTransportPositionChanged,
                    "MoveFinished" => MaterialBoxSubMachineModulesConst.EventUnloadTransportMoveFinished,
                    _ => string.Empty
                };
            }

            /// <summary>储料位索引 0/1 → MaterialRack 0~3。</summary>
            private static int GetMaterialRackByStorageIndex(bool isLoadSide, int storageIndex)
            {
                if (storageIndex < 0)
                    return -1;

                if (isLoadSide)
                    return storageIndex == 0 ? LoadUpperRack : LoadLowerRack;

                return storageIndex == 0 ? UnloadUpperRack : UnloadLowerRack;
            }

            #region Lifecycle

            /// <summary>配方下发：储料/运输/料口 ApplyRecipe，并初始化四个 MaterialRack 料盒槽位。</summary>
            void ISubMMAutoModeCmdExecutor.SetPFCfgInfo(byte[] pfCfgInfo)
            {
                if (pfCfgInfo != null && pfCfgInfo.Length > 0)
                    materialBoxSubMachineModulesPPCfg = JsonObjConvert.FromJSonBytes<MaterialBoxSubMachineModulesPPCfg>(pfCfgInfo);
                materialBoxSubMachineModulesPPCfg ??= new MaterialBoxSubMachineModulesPPCfg();
                loadContext.StorageDevice.ApplyRecipe(materialBoxSubMachineModulesPPCfg.LoadStorageDevice);
                unloadContext.StorageDevice.ApplyRecipe(materialBoxSubMachineModulesPPCfg.UnloadStorageDevice);
                loadContext.TransportMechanism.ApplyRecipe(materialBoxSubMachineModulesPPCfg.LoadTransportMechanism);
                unloadContext.TransportMechanism.ApplyRecipe(materialBoxSubMachineModulesPPCfg.UnloadTransportMechanism);
                loadContext.feedingPort.SetPPCfg(materialBoxSubMachineModulesPPCfg.FeedingPortPPCfg);
                loadContext.feedingPort.SetPPCfg(materialBoxSubMachineModulesPPCfg.ReceivePortPPCfg);
            }

            void ISubMMAutoModeCmdExecutor.StartWork()
            {
                PauseObj.Status = 2;
            }

            void ISubMMAutoModeCmdExecutor.StopWork()
            {
                PauseObj.Status = 1;
            }

            void ISubMMAutoModeCmdExecutor.Pause()
            {
                PauseObj.Status = 1;
            }

            void ISubMMAutoModeCmdExecutor.Resume()
            {
                PauseObj.Status = 2;
            }

            void ISubMMAutoModeCmdExecutor.BeforeSwitchPF()
            {
            }


            void ISubMMAutoModeCmdExecutor.AfterStopWork()
            {
            }
            #endregion
            /// <summary>
            /// 子机械模组普通方法入口。
            /// 直接按 MethodID 分发，异常统一收敛成错误结果，避免层层 Try 分支。
            /// </summary>
            GFBaseTypeParamValueList ISubMMAutoModeCmdExecutor.ExecMethod(string methodID, GFBaseTypeParamValueList param)
            {
                try
                {
                    return ExecuteMethodCore(methodID, param);
                }
                catch (Exception ex)
                {
                    return CreateErrorResult(ex.Message);
                }
            }

            #region Async Interface Stubs
            Task<GFBaseTypeParamValueList> ISubMMAutoModeCmdExecutor.AsynExecMethod(string methodID, GFBaseTypeParamValueList param)
            {
                return Task.Run(() =>
                {
                    GFBaseTypeParamValueList result = new GFBaseTypeParamValueList();
                    return result;
                });
            }

            GFParamValueList ISubMMAutoModeCmdExecutor.ExecMethod(string methodID, GFParamValueList param)
            {
                return new GFParamValueList();
            }

            Task<GFParamValueList> ISubMMAutoModeCmdExecutor.AsynExecMethod(string methodID, GFParamValueList param)
            {
                return Task.Run(() =>
                {
                    GFParamValueList result = new GFParamValueList();
                    Thread.Sleep(10);
                    return result;
                });
            }

            /// <summary>
            /// 子机械模组能力方法入口。
            /// 当前仅保留配置读取类能力接口，不承载储料装置对象和运输机构对象动作。
            /// </summary>
            GFBaseTypeParamValueList ISubMMAutoModeCmdExecutor.ExecCabilityMethod(string methodID, GFBaseTypeParamValueList param)
            {
                switch (methodID)
                {
                    case MaterialBoxSubMachineModulesConst.GetInitParametersMethodID:
                        return CreateParametersResult(materialBoxSubMachineModulesInitCfg);
                    case MaterialBoxSubMachineModulesConst.GetRecipeParametersMethodID:
                        return CreateParametersResult(materialBoxSubMachineModulesPPCfg);
                    default:
                        return CreateErrorResult($"未识别的能力方法: {methodID ?? "<null>"}");
                }
            }

            Task<GFBaseTypeParamValueList> ISubMMAutoModeCmdExecutor.AsynExecCabilityMethod(string methodID, GFBaseTypeParamValueList param)
            {
                return Task.Run(() =>
                {
                    GFBaseTypeParamValueList result = new GFBaseTypeParamValueList();
                    return result;
                });
            }
            #endregion

            /// <summary>
            /// 运行时界面按钮入口：cmdID 与前端下发的命令字符串一致（见 MaterialBoxSubMM_RuntimeCommands.txt）。
            /// 固定返回 Result、errorMsg、data（成功时 Result 为字符串 0），便于前端统一解析。
            /// </summary>
            GFBaseTypeParamValueList ExecRuntimeCtlCmdCore(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                GFBaseTypeParamValueList result = CreateRuntimeResult();

                try
                {
                    switch (cmdID)
                    {
                        case MaterialBoxSubMachineModulesConst.RtCmdMoveToFirstSlot:
                            {
                                int rack = RequireMaterialRack(cmdParam);
                                double speed = ResolveTransportSpeed(cmdParam);
                                double acc = ResolveTransportAcceleration(cmdParam);
                                RunningContext runningContext = GetRunningContext(rack);
                                int materialBoxIndex = GetMaterailBoxIndex(rack);
                                runningContext.TransportMechanism.TransportMove(speed, acc, runningContext.StorageDevice.GetFirstSlot(materialBoxIndex).Position);
                            }
                            break;
                        case MaterialBoxSubMachineModulesConst.RtCmdMoveToLastSlot:
                            {
                                int rack = RequireMaterialRack(cmdParam);
                                double speed = ResolveTransportSpeed(cmdParam);
                                double acc = ResolveTransportAcceleration(cmdParam);
                                RunningContext runningContext = GetRunningContext(rack);
                                int materialBoxIndex = GetMaterailBoxIndex(rack);
                                runningContext.TransportMechanism.TransportMove(speed, acc, runningContext.StorageDevice.GetLastSlot(materialBoxIndex).Position);
                            }
                            break;
                        case MaterialBoxSubMachineModulesConst.RtCmdMoveToNextSlot:
                        {
                            int rack = RequireMaterialRack(cmdParam);
                            double speed = ResolveTransportSpeed(cmdParam);
                            double acc = ResolveTransportAcceleration(cmdParam);
                            RunningContext runningContext = GetRunningContext(rack);
                            int materialBoxIndex = GetMaterailBoxIndex(rack);
                            bool hasMaterial = (rack == 2 || rack == 3);
                            runningContext.StorageDevice.TryMoveToNextOKSlot(hasMaterial, out Slot slot);
                            runningContext.TransportMechanism.TransportMove(speed, acc, slot.Position);
                            break;
                        }
                        case MaterialBoxSubMachineModulesConst.RtCmdResetMaterialBoxState:
                            {
                                int rack = RequireMaterialRack(cmdParam);
                                RunningContext runningContext = GetRunningContext(rack);
                                int materialBoxIndex = GetMaterailBoxIndex(rack);
                                bool hasMaterial = (rack == 0 || rack == 1);
                                runningContext.StorageDevice.SetSlotMaterialState(materialBoxIndex, hasMaterial);
                                SetRuntimeOk(result, "0");
                            }
                            break;
                        case MaterialBoxSubMachineModulesConst.RtCmdMoveToInitialPosition:
                            {
                                int rack = RequireMaterialRack(cmdParam);
                                double speed = ResolveTransportSpeed(cmdParam);
                                double acc = ResolveTransportAcceleration(cmdParam);
                                int materialBoxIndex = GetMaterailBoxIndex(rack);
                                double target = GetRunningContext(rack).StorageDevice.GetInitialPosition(materialBoxIndex);
                                GetRunningContext(rack).TransportMechanism.TransportMove(speed, acc, target);
                            }
                            break;
                        case MaterialBoxSubMachineModulesConst.RtCmdMoveTo:
                            {
                                int rack = RequireMaterialRack(cmdParam);
                                double speed = ResolveTransportSpeed(cmdParam);
                                double acc = ResolveTransportAcceleration(cmdParam);
                                double target = ResolveMoveToTarget(cmdParam, rack);
                                GetRunningContext(rack).TransportMechanism.TransportMove(speed, acc, target);
                            }
                            break;
                        case MaterialBoxSubMachineModulesConst.RtCmdMagazineMotion:
                            {
                                int logicAxis = ResolveLogicAxis(cmdParam);
                                double direction = ReadDoubleParam(cmdParam, "Direction", "缺少参数：Direction");
                                double speed = ResolveTransportSpeed(cmdParam);
                                double acc = ResolveTransportAcceleration(cmdParam);
                                RunningContext runningContext = logicAxis == 0 ? loadContext : unloadContext;
                                runningContext.TransportMechanism.ContinueMove(speed, acc, direction > 0);
                            }
                            break;
                        case MaterialBoxSubMachineModulesConst.RtCmdZAxisStop:
                            {
                                int logicAxis = ResolveLogicAxis(cmdParam);
                                RunningContext runningContext = logicAxis == 0 ? loadContext : unloadContext;
                                runningContext.TransportMechanism.StopMove();
                            }
                            break;
                        case MaterialBoxSubMachineModulesConst.RtCmdZMoveUp:
                            {
                                int logicAxis = ResolveLogicAxis(cmdParam);
                                double speed = ReadPositiveDouble(cmdParam, "MaxSpeed", "缺少参数或参数无效：MaxSpeed");
                                double acc = ResolveAccelerationOverride(cmdParam) ?? Math.Max(1.0, speed / 10.0);
                                double? step = TryGetDouble(cmdParam, "Step", out double stepValue) && stepValue > 0
                                    ? stepValue
                                    : null;
                                RunningContext runningContext = logicAxis == 0 ? loadContext : unloadContext;
                                if (!step.HasValue || step.Value <= 0)
                                {
                                    runningContext.TransportMechanism.ContinueMove(speed, acc, true);
                                    SetRuntimeOk(result);
                                }
                                else
                                {
                                    runningContext.TransportMechanism.RelativeMove(speed, acc, step.Value);
                                    SetRuntimeOk(result);
                                }
                                break;
                        }
                        case MaterialBoxSubMachineModulesConst.RtCmdZMoveDown:
                            {
                                int logicAxis = ResolveLogicAxis(cmdParam);
                                double speed = ReadPositiveDouble(cmdParam, "MaxSpeed", "缺少参数或参数无效：MaxSpeed");
                                double acc = ResolveAccelerationOverride(cmdParam) ?? Math.Max(1.0, speed / 10.0);
                                double? step = TryGetDouble(cmdParam, "Step", out double stepValue) && stepValue > 0
                                    ? stepValue
                                    : null;
                                RunningContext runningContext = logicAxis == 0 ? loadContext : unloadContext;
                                if (!step.HasValue || step.Value <= 0)
                                {
                                    runningContext.TransportMechanism.ContinueMove(speed, acc, false);
                                    SetRuntimeOk(result);
                                }
                                else
                                {
                                    runningContext.TransportMechanism.RelativeMove(speed, acc, -step.Value);
                                    SetRuntimeOk(result);
                                }
                            }
                            break;
                        case MaterialBoxSubMachineModulesConst.RtCmdGetAxisInfos:
                            {
                                List<AxisInformation> axisInformations = GetAllAxisInfosFromElectronicManager();
                                SetRuntimeOk(result, JsonObjConvert.ToJSon(axisInformations));
                            }
                            break;
                        case MaterialBoxSubMachineModulesConst.RtCmdGetAxisPos:
                            {
                                SetRuntimeOk(result, GetCurrentTransportAxisPosition(ResolveLogicAxis(cmdParam)).ToString(CultureInfo.InvariantCulture));
                            }
                            break;
                        case MaterialBoxSubMachineModulesConst.RtCmdMagazineClamp:
                            {
                                int rack = RequireMaterialRack(cmdParam);
                                int cylinderIndex = ResolveCylinderIndex(cmdParam);
                                int materialBoxIndex = GetMaterailBoxIndex(rack);
                                GetRunningContext(rack).StorageDevice.Stretch(materialBoxIndex, cylinderIndex);
                            }
                            break;
                        case MaterialBoxSubMachineModulesConst.RtCmdMagazineUnclamp:
                            {
                                int rack = RequireMaterialRack(cmdParam);
                                int cylinderIndex = ResolveCylinderIndex(cmdParam);
                                int materialBoxIndex = GetMaterailBoxIndex(rack);
                                GetRunningContext(rack).StorageDevice.Retract(materialBoxIndex, cylinderIndex);
                            }
                            break;
                        case MaterialBoxSubMachineModulesConst.RtCmdGetIOInfos:
                            {
                                List<IOStateInformation> ioInfos = GetAllIOStateInfosFromElectronicManager();
                                SetRuntimeOk(result, JsonObjConvert.ToJSon(ioInfos));
                            }
                            break;
                        default:
                            SetRuntimeError(result, $"未识别的运行时命令: {cmdID ?? "<null>"}");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    SetRuntimeError(result, ex.Message);
                }

                return result;
            }
            GFBaseTypeParamValueList ISubMMAutoModeCmdExecutor.ExecRuntimeCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                return ExecRuntimeCtlCmdCore(cmdID, cmdParam);
            }
            GFBaseTypeParamValueList ISubMMManualModeCmdExecutor.ExecCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                return ExecRuntimeCtlCmdCore(cmdID, cmdParam);
            }
            /// <summary>构造运行时统一返回结构 Result/errorMsg/data。</summary>
            private GFBaseTypeParamValueList CreateRuntimeResult()
            {
                GFBaseTypeParamValueList result = new GFBaseTypeParamValueList();
                result.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue("0")));
                result.Add(new GFBaseTypeParamValue("errorMsg", new GriffinsBaseValue("")));
                result.Add(new GFBaseTypeParamValue("data", new GriffinsBaseValue("")));
                return result;
            }

            /// <summary>标记运行时命令成功（Result=0）。</summary>
            private static void SetRuntimeOk(GFBaseTypeParamValueList result, string data = "")
            {
                result["Result"] = new GriffinsBaseValue("0");
                result["errorMsg"] = new GriffinsBaseValue("");
                result["data"] = new GriffinsBaseValue(data ?? "");
            }

            /// <summary>标记运行时命令失败并写入 errorMsg。</summary>
            private static void SetRuntimeError(GFBaseTypeParamValueList result, string msg, string resultCode = "-1")
            {
                result["Result"] = new GriffinsBaseValue(resultCode ?? "-1");
                result["errorMsg"] = new GriffinsBaseValue(msg ?? "");
            }

            #region 运行时命令处理（界面按钮）

            private List<IOStateInformation> GetAllIOStateInfosFromElectronicManager()
            {
                var response = ServerInnerInfoSender.SendMutualInfo(
                    IOStateInfosRequest.InfoKindID,
                    new IOStateInfosRequest());
                if (response == null || response.Count == 0)
                {
                    return null;
                }

                IOStateInfosResponse ioResponse = response[0].Response as IOStateInfosResponse;
                List<IOStateInformation> ioInfos = ioResponse?.IOStateInformations ?? new List<IOStateInformation>();
                return ioInfos;
            }

            /// <summary>
            /// 获取界面数据对象属性读写接口实例，如果不支持返回nul
            /// </summary>
            /// <returns>界面数据对象属性读写接口实例</returns>
            ICompUIDataObjPropValRW ISubMMAutoModeCmdExecutor.GetUIDataObjPropValRW()
            {
                return null;
            }

            // ...
            #region Object Methods And Shared Helpers

            /// <summary>解析 MoveTo 目标：SlotIndex 换算 mm，或直接读 Pos/Position。</summary>
            private double ResolveMoveToTarget(GFBaseTypeParamValueList cmdParam, int rack)
            {
                StorageDevice storageBox = GetRunningContext(rack).StorageDevice;
                int materialBoxIndex = GetMaterailBoxIndex(rack);
                if (TryGetInt(cmdParam, "SlotIndex", out int slotIndex))
                {
                    Slot slot = storageBox?.GetSlot(materialBoxIndex, slotIndex);
                    if (slot == null || slotIndex < 0)
                        throw new InvalidOperationException("参数无效：SlotIndex，或槽位列表尚未就绪");

                    if (!slot.IsEnabled)
                        throw new InvalidOperationException($"参数无效：SlotIndex={slotIndex}，该槽位已被禁用，无法移动到该槽位");

                    return slot.Position;
                }

                if (TryGetDouble(cmdParam, "Pos", out double pos))
                    return pos;
                if (TryGetDouble(cmdParam, "Position", out double position))
                    return position;

                throw new InvalidOperationException("缺少参数：SlotIndex 或 Pos/Position");
            }

            /// <summary>返回订阅缓存中的当前运输轴位置；缓存未建立时使用默认回退位置。</summary>
            private double GetCurrentTransportAxisPosition(int logicAxis)
            {
                return (logicAxis == 0?loadContext : unloadContext).TransportMechanism.GetCurrentAxisPosition();
            }

            private static bool TryGetInt(GFBaseTypeParamValueList cmdParam, string key, out int value)
            {
                return RuntimeParamReader.TryGetInt(cmdParam, key, out value);
            }

            private static bool TryGetDouble(GFBaseTypeParamValueList cmdParam, string key, out double value)
            {
                return RuntimeParamReader.TryGetDouble(cmdParam, key, out value);
            }

            /// <summary>运行时 MaxSpeed 优先，否则用上下文默认速度。</summary>
            private static double ResolveTransportSpeed(GFBaseTypeParamValueList cmdParam)
            {
                return TryGetDouble(cmdParam, "MaxSpeed", out double speedVal) && speedVal > 0
                    ? speedVal
                    : 10;
            }

            /// <summary>运行时 Acc 优先，否则用上下文默认加速度。</summary>
            private static double ResolveTransportAcceleration(GFBaseTypeParamValueList cmdParam)
            {
                return ResolveAccelerationOverride(cmdParam) ?? 10;
            }

            private RunningContext GetRunningContext(int rack)
            {
                switch(rack)
                {
                    case LoadUpperRack:
                    case LoadLowerRack:
                        return loadContext;
                    case UnloadUpperRack:
                    case UnloadLowerRack:
                        return unloadContext;
                    default:
                        throw new InvalidOperationException($"参数无效：MaterialRack={rack}");
                }
            }

            private int GetMaterailBoxIndex(int rack)
            {
                switch (rack)
                {
                    case LoadUpperRack:
                    case UnloadUpperRack:
                        return 0;
                    case LoadLowerRack:
                    case UnloadLowerRack:
                        return 1;
                    default:
                        throw new InvalidOperationException($"参数无效：MaterialRack={rack}");
                }
            }

            private int ResolveLogicAxis(GFBaseTypeParamValueList cmdParam)
            {
                if (TryGetInt(cmdParam, "ZAxisSelect", out int zSel))
                {
                    if (zSel == 0)
                        return LoadZLogicAxis;
                    if (zSel == 1)
                        return UnloadZLogicAxis;
                    throw new InvalidOperationException($"参数无效：ZAxisSelect={zSel}，应为 0(上料Z) 或 1(下料Z)");
                }

                if (TryGetInt(cmdParam, "MaterialRack", out _))
                    return IsLoadRack(RequireMaterialRack(cmdParam)) ? LoadZLogicAxis : UnloadZLogicAxis;

                throw new InvalidOperationException("请指定 Z 轴：ZAxisSelect(0=上料Z,1=下料Z)");
            }

            private int RequireMaterialRack(GFBaseTypeParamValueList cmdParam)
            {
                if (!TryGetInt(cmdParam, "MaterialRack", out int rack))
                    throw new InvalidOperationException("缺少参数：MaterialRack(0=上料上层,1=上料下层,2=下料上层,3=下料下层)");
                if (rack < LoadUpperRack || rack > UnloadLowerRack)
                    throw new InvalidOperationException($"参数无效：MaterialRack={rack}");
                return rack;
            }

            private int ResolveCylinderIndex(GFBaseTypeParamValueList cmdParam)
            {
                if (TryGetInt(cmdParam, "CylinderIndex", out int cylinderIndex) && cylinderIndex >= 0)
                    return cylinderIndex;
                return 0;
            }

            private static double ReadPositiveDouble(GFBaseTypeParamValueList cmdParam, string key, string errorMsg)
            {
                if (!TryGetDouble(cmdParam, key, out double value) || value <= 0)
                    throw new InvalidOperationException(errorMsg);
                return value;
            }

            private static double ReadDoubleParam(GFBaseTypeParamValueList cmdParam, string key, string errorMsg)
            {
                if (!TryGetDouble(cmdParam, key, out double value))
                    throw new InvalidOperationException(errorMsg);
                return value;
            }

            private static int ReadNonNegativeInt(GFBaseTypeParamValueList cmdParam, string key)
            {
                if (!TryGetInt(cmdParam, key, out int value) || value < 0)
                    throw new InvalidOperationException($"缺少参数或参数无效：{key}");
                return value;
            }


            private static double? ResolveAccelerationOverride(GFBaseTypeParamValueList cmdParam)
            {
                if (TryGetDouble(cmdParam, "Acc", out double acc) && acc > 0)
                    return acc;
                if (TryGetDouble(cmdParam, "acc", out double legacyAcc) && legacyAcc > 0)
                    return legacyAcc;
                return null;
            }

            private bool IsLoadRack(int rack)
            {
                return rack == LoadUpperRack || rack == LoadLowerRack;
            }


            /// <summary>
            /// 料盒对象能力方法：切到下一有效槽位。
            /// 内部串起料盒合法性检查、运行时轴移动和结果事件号封装。
            /// </summary>
            private void ExecuteMoveNextSlotMethod(bool isUpfeed)
            {
                RunningContext runningContext = isUpfeed ? loadContext : unloadContext;
                bool hasMaterial = isUpfeed;
                if(runningContext.StorageDevice.TryMoveToNextOKSlot(hasMaterial, out Slot slot))
                {
                    runningContext.TransportMechanism.TransportMove(slot.Position);
                }
            }

            private void ExecuteMoveCurrentSlotMethod(bool isUpfeed)
            {
                RunningContext runningContext = isUpfeed ? loadContext : unloadContext;
                bool hasMaterial = !isUpfeed;
                if (runningContext.StorageDevice.TryMoveToCurrentOKSlot(hasMaterial, out Slot slot))
                {
                    runningContext.TransportMechanism.TransportMove(slot.Position);
                }
            }

            private static GFBaseTypeParamValueList CreateMoveNextResult()
            {
                GFBaseTypeParamValueList result = new GFBaseTypeParamValueList();
                result.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue("0")));
                result.Add(new GFBaseTypeParamValue("EventID", new GriffinsBaseValue(string.Empty)));
                result.Add(new GFBaseTypeParamValue("WarningEventID", new GriffinsBaseValue(string.Empty)));
                result.Add(new GFBaseTypeParamValue("errorMsg", new GriffinsBaseValue(string.Empty)));
                result.Add(new GFBaseTypeParamValue("data", new GriffinsBaseValue(string.Empty)));
                return result;
            }

            private static GFBaseTypeParamValueList CreateParametersResult(object parameters)
            {
                GFBaseTypeParamValueList result = CreateMoveNextResult();
                string json = ToJson(parameters);
                result["data"] = new GriffinsBaseValue(json);
                result.Add(new GFBaseTypeParamValue("Json", new GriffinsBaseValue(json)));
                return result;
            }

            private static GFBaseTypeParamValueList CreateErrorResult(string errorMsg)
            {
                GFBaseTypeParamValueList result = CreateMoveNextResult();
                result["Result"] = new GriffinsBaseValue("-1");
                result["errorMsg"] = new GriffinsBaseValue(errorMsg ?? string.Empty);
                return result;
            }

            private static string ToJson(object data)
            {
                return data != null
                    ? Encoding.UTF8.GetString(JsonObjConvert.ToJSonBytes(data))
                    : "{}";
            }

            #endregion

            #region 普通方法分发（流程/总控调用）

            private GFBaseTypeParamValueList ExecuteMethodCore(string methodID, GFBaseTypeParamValueList param)
            {
                GFBaseTypeParamValueList rst = CreateOkResult();
                switch (methodID)
                {
                    case MaterialBoxSubMachineModulesConst.UpfeedMoveNextSlotMethodID:
                        ExecuteMoveNextSlotMethod(true);
                        break;
                    case MaterialBoxSubMachineModulesConst.DownfeedMoveNextSlotMethodID:
                        ExecuteMoveNextSlotMethod(false);
                        break;
                    case MaterialBoxSubMachineModulesConst.UpfeedMoveCurrentSlotMethodID:
                        ExecuteMoveCurrentSlotMethod(true);
                        break;
                    case MaterialBoxSubMachineModulesConst.DownfeedMoveCurrentSlotMethodID:
                        ExecuteMoveCurrentSlotMethod(false);
                        break;
                    case MaterialBoxSubMachineModulesConst.GetMaterialContainerStatusMethodID:
                        GFBaseTypeParamValueList containerResult = new GFBaseTypeParamValueList();
                        MaterialContainerStatusList containerStatus = new MaterialContainerStatusList();
                        // 添加上料和下料的料盒状态信息，分别对应 LoadAxisInformType 和 UnloadAxisInformType
                        containerStatus.MaterialContainers.Add(MaterialBoxSubMachineModulesConst.LoadAxisInformType + MaterialBoxSubMachineModulesConst.MaterialContainerName, BuildMaterialContainerStatus(true));
                        containerStatus.MaterialContainers.Add(MaterialBoxSubMachineModulesConst.UnloadAxisInformType + MaterialBoxSubMachineModulesConst.MaterialContainerName, BuildMaterialContainerStatus(false));
                        containerResult.Add(new GFBaseTypeParamValue("data", ((IGriffinsBaseValue)containerStatus).ToBaseValue()));
                        return containerResult;
                    case MaterialBoxSubMachineModulesConst.StorageOpenMethodID:
                    case MaterialBoxSubMachineModulesConst.StorageCloseMethodID:
                        return ExecuteStorageMethod(methodID, param);
                    case MaterialBoxSubMachineModulesConst.GetMaterialStateMethodID:
                        return ExecuteFeedPortMethod(param);
                    case MaterialBoxSubMachineModulesConst.SetCurrentSlotStatusMethodID:
                        {
                            bool isFeeding = param["IsFeeding"].ToBool();
                            bool hasMaterial = param["HasMaterial"].ToBool();
                            RunningContext runningContext = (isFeeding ? loadContext : unloadContext);
                            runningContext.StorageDevice.AdjustCurrentSlotMaterialState(hasMaterial);
                        }
                        break;
                    case MaterialBoxSubMachineModulesConst.UpdateMaterialBoxStateMethodID:
                        {
                            int rack = RequireMaterialRack(param);
                            bool isFeeding = IsLoadRack(rack);
                            int materialBoxIndex = GetMaterailBoxIndex(rack);
                            RunningContext runningContext = (isFeeding ? loadContext : unloadContext);
                            runningContext.StorageDevice.SetSlotMaterialState(materialBoxIndex, !isFeeding);
                        }
                        break;
                    default:
                        return CreateErrorResult($"未识别的方法: {methodID ?? "<null>"}");
                }
                return rst;
            }

            private GFBaseTypeParamValueList ExecuteStorageMethod(string methodID, GFBaseTypeParamValueList param)
            {
                int rack = RequireMaterialRack(param);
                RunningContext context = GetRunningContext(rack);
                int materialIndex = GetMaterailBoxIndex(rack);
                switch (methodID)
                {
                    case MaterialBoxSubMachineModulesConst.StorageOpenMethodID:
                        context.StorageDevice?.Retract(materialIndex, ResolveCylinderIndex(param));
                        return CreateOkResult();
                    case MaterialBoxSubMachineModulesConst.StorageCloseMethodID:
                        context.StorageDevice?.Stretch(materialIndex, ResolveCylinderIndex(param));
                        return CreateOkResult();
                    default:
                        return CreateErrorResult($"未识别的储料方法: {methodID ?? "<null>"}");
                }
            }

            private GFBaseTypeParamValueList ExecuteFeedPortMethod(GFBaseTypeParamValueList param)
            {
                FeedPort feedPort = ResolveFeedPort(param);
                int index = ReadNonNegativeInt(param, "Index");
                bool hasMaterial = feedPort.GetMaterialState(index);
                return CreateSuccessResult(("Result", hasMaterial));
            }

            private FeedPort ResolveFeedPort(GFBaseTypeParamValueList param)
            {
                string type = null;
                try
                {
                    type = param != null ? param["Type"]?.ToStringVal() : null;
                }
                catch
                {
                    type = null;
                }
                if (string.IsNullOrWhiteSpace(type))
                    throw new InvalidOperationException("缺少参数：Type");

                return Enum.Parse<FeedPortRole>(type) == FeedPortRole.Feed
                    ? loadContext.feedingPort
                    : unloadContext.feedingPort;
            }

            private GFBaseTypeParamValueList CreateSuccessResult(params (string Key, object Value)[] values)
            {
                GFBaseTypeParamValueList result = new GFBaseTypeParamValueList();
                foreach ((string Key, object Value) in values)
                {
                    result.Add(new GFBaseTypeParamValue(Key, new GriffinsBaseValue(Value)));
                }
                return result;
            }

            private GFBaseTypeParamValueList CreateOkResult(params (string Key, object Value)[] values)
            {
                List<(string Key, object Value)> items = new List<(string Key, object Value)>
                {
                    ("Result", "0")
                };
                if (values != null && values.Length > 0)
                    items.AddRange(values);

                return CreateSuccessResult(items.ToArray());
            }

            private MaterialContainerStatus BuildMaterialContainerStatus(bool isFeed)
            {
                MaterialContainerStatus result = new MaterialContainerStatus
                {
                    Name = (isFeed ? MaterialBoxSubMachineModulesConst.LoadStorageDeviceName : MaterialBoxSubMachineModulesConst.UnloadStorageDeviceName),
                    IsFeeding = isFeed
                };
                if (isFeed)
                {
                    result.MaterialBoxes.Add(
                        MaterialBoxSubMachineModulesConst.UpperRackName,
                        BuildMaterialBoxStatus(LoadUpperRack, isFeed));
                    result.MaterialBoxes.Add(
                        MaterialBoxSubMachineModulesConst.LowerRackName,
                        BuildMaterialBoxStatus(LoadLowerRack, isFeed));
                }
                else
                {
                    result.MaterialBoxes.Add(
                        MaterialBoxSubMachineModulesConst.UpperRackName,
                        BuildMaterialBoxStatus(UnloadUpperRack, isFeed));
                    result.MaterialBoxes.Add(
                        MaterialBoxSubMachineModulesConst.LowerRackName,
                        BuildMaterialBoxStatus(UnloadLowerRack, isFeed));
                }
                return result;
            }

            private MaterialBoxStatus BuildMaterialBoxStatus(int rack, bool isFeeding)
            {
                StorageDevice storageDevice = GetRunningContext(rack).StorageDevice;
                int materialBoxIndex = GetMaterailBoxIndex(rack);
                // 确保料盒已初始化，能正确返回 Name；否则 Name 可能一直是 null 导致界面显示异常
                MaterialBoxStatus result = new MaterialBoxStatus
                {
                    Name = storageDevice.GetMaterialBoxName(materialBoxIndex),
                    IsFeeding = isFeeding
                };
                if (storageDevice == null || materialBoxIndex < 0)
                    return result;
                List<Slot> allSlots = storageDevice.GetAllSlots(materialBoxIndex);
                SlotStatuses slotStatuses = new SlotStatuses();
                foreach (Slot slot in allSlots)
                {
                    slotStatuses.Add(new SlotStatus
                    {
                        ID = slot.Index.ToString(),
                        MaterialStatus = slot.IsEnabled
                            ? (!slot.IsEmpty ? MaterialStatus.Full : MaterialStatus.Empty)
                            : MaterialStatus.Disable
                    });
                }
                result.SlotStatusList.Add("SlotStatusList", slotStatuses);
                result.IsEmpty = !storageDevice.ReadStateIO(materialBoxIndex, 0);
                result.MaterialBoxCylinderStatus = (storageDevice.GetCylinderPosType(materialBoxIndex, 0) == ECylinderPosType.Stretch);
                return result;
            }
            #endregion
        }
    }
}
#endregion 