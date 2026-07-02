using GF_Gereric;
using GKG.LoadUnloadMachineModules.Properties;
using GKG.SubMM;
using Griffins;
using Griffins.ImeIOT;
using Griffins.PF;
using Griffins.PF.RichClient;
using Griffins.PF.Server;
using MaterialBoxSubMachineModules.FeedPort;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GKG
{
    namespace MM
    {
        /// <summary>
        /// 上下料总控执行器：组装各子模组，并将上料/下料流程编排成统一对外命令。
        /// </summary>
        /// <remarks>整体说明见 loadmaterial/EXECUTOR_IMPLEMENTATION.md §3。</remarks>
        public class LoadUnloadMMCmdExecutor : IMMCmdExecutor, IMMManualModeCmdExecutor, IMMAutoModeCmdExecutor
        {
            /// <summary>
            /// 单次上下料流程的主状态机：先判料，再推料，再切换到下一槽位。
            /// </summary>
            private enum FeedState
            {
                Start,
                SenseHasMaterial,
                CheckHasMaterialResult,
                CallPushRodExtend,
                CheckPushRodExtendResult,
                CallPushRodRetract,
                CheckPushRodRetractResult,
                CallMoveToNextSlot,
                CheckMoveToNextSlotResult,
                CheckMaterialState,
                TriggerSuccess,
                TriggerFail,
                End
            }

            #region 字段

            private readonly LoadUnloadMachineModulesFactoryCfg factoryCfg;
            private readonly LoadUnloadMachineModulesInitCfg initCfg;
            private readonly LoadUnloadMachineModulesPPCfg ppCfg;
            private readonly MMAlias alias;
            private const string UpfeedInformType = "上料";
            private const string DownfeedInformType = "下料";

            private ImeGenNormalEventHandler imeGenNormalEventHandler;
            private ImeCabilityEventHandler imeCabilityEventHandler;
            private ImeAlarmEventHandler imeAlarmEventHandler;

            private int upfeedContinuousEmptyPushCounter;
            private int downfeedContinuousEmptyPushCounter;
            private ICompUIDataObjPropValRW _compUIDataObjPropValRW;
            IMMCmdExecutorCallBack _callBack;
            private bool initSucceeded = false;
            private Task RefreshMetrialStatus;
            private bool refreshMaterialStatusRunning = false;
            #endregion

            #region 构造函数

            public LoadUnloadMMCmdExecutor(MMAlias alias, byte[] factoryCfgInfo)
            {
                this.alias = alias;
                factoryCfg = new LoadUnloadMachineModulesFactoryCfg();
                initCfg = new LoadUnloadMachineModulesInitCfg();
                ppCfg = new LoadUnloadMachineModulesPPCfg();

                if (factoryCfgInfo != null && factoryCfgInfo.Length > 0)
                {
                    factoryCfg.FromBytes(factoryCfgInfo);
                }
            }

            #endregion

            #region IMMAutoModeCmdExecutor（事件）

            event ImeGenNormalEventHandler IMMAutoModeCmdExecutor.GenNormalEvent
            {
                add { imeGenNormalEventHandler += value; }
                remove { imeGenNormalEventHandler -= value; }
            }

            event ImeCabilityEventHandler IMMAutoModeCmdExecutor.CabilityEvent
            {
                add { imeCabilityEventHandler += value; }
                remove { imeCabilityEventHandler -= value; }
            }

            event ImeAlarmEventHandler IMMAutoModeCmdExecutor.AlarmEvent
            {
                add { imeAlarmEventHandler += value; }
                remove { imeAlarmEventHandler -= value; }
            }

            #endregion

            #region IMMCmdExecutor

            /// <summary>总控预初始化钩子；当前无额外逻辑。</summary>
            void IMMCmdExecutor.BeforeInit()
            {
            }

            /// <summary>初始化总模组，并在内部创建料盒、电机推杆、气缸推杆三个子模组实例。</summary>
            void IMMCmdExecutor.Init(byte[] initCfgInfo, byte[] calibrationCfgInfo, IMMCmdExecutorCallBack callBack)
            {
                try
                {
                    _callBack = callBack;
                    initCfg.FromBytes(initCfgInfo);
                    //RegisterInformInfoProcessDelegate();
                    if (initCfgInfo != null && initCfgInfo.Length > 0)
                    {
                        initCfg.FromBytes(initCfgInfo);
                    }
                    if (_compUIDataObjPropValRW == null)
                        _compUIDataObjPropValRW = new LoadUnloadMMCompUIDataObjPropValRW(ExecRuntimeCtlCmdCore);
                   
                    initSucceeded = true;
                }
                catch (Exception ex)
                {

                }
            }

            void IMMCmdExecutor.AfterInit()
            {
                //PublishMaterialContainerStatusInfo(true);
                //PublishMaterialContainerStatusInfo(false);
                if (RefreshMetrialStatus == null)
                {
                    refreshMaterialStatusRunning = true;
                    RefreshMetrialStatus = Task.Run(() =>
                    {
                        while (true)
                        {
                            if (!refreshMaterialStatusRunning)
                                break;
                            GFBaseTypeParamValueList result = new GFBaseTypeParamValueList();
                            AttachMaterialContainerStatus(result);
                            Thread.Sleep(100);
                        }
                    });
                }
            }

            IMMManualModeCmdExecutor IMMCmdExecutor.GetMMManualModeCmdExecutor()
            {
                return this;
            }

            IMMAutoModeCmdExecutor IMMCmdExecutor.GetMMAutoModeCmdExecutor()
            {
                return this;
            }

            /// <summary>注销状态通知委托，停止料盒状态后台刷新任务。</summary>
            void IMMCmdExecutor.UnInit()
            {
                //UnRegisterInformInfoProcessDelegate();
                if (RefreshMetrialStatus != null)
                {
                    refreshMaterialStatusRunning = false;
                    RefreshMetrialStatus.Wait(2000);
                }
            }
            #endregion

            #region IMMAutoModeCmdExecutor

            bool IMMAutoModeCmdExecutor.CanStartWork(out string reasonMsg)
            {
                if(!initSucceeded)
                {
                    reasonMsg = Resources.InitFailed;

                }
                reasonMsg = string.Empty;
                return true;
            }

            void IMMAutoModeCmdExecutor.SetRunMode(ImeRunMode imeRunMode)
            {
            }
            /// <summary>下发配方后，继续向各子模组同步各自的配方片段。</summary>
            void IMMAutoModeCmdExecutor.SetPFCfgInfo(byte[] pfCfgInfo)
            {
                ppCfg.FromBytes(pfCfgInfo);
            }

            /// <summary>进入自动运行：解除全局暂停标记。</summary>
            void IMMAutoModeCmdExecutor.StartWork() => PauseObj.Status = 2;

            /// <summary>停止自动运行：置暂停标记。</summary>
            void IMMAutoModeCmdExecutor.StopWork() => PauseObj.Status = 1;

            /// <summary>暂停自动运行。</summary>
            void IMMAutoModeCmdExecutor.Pause() => PauseObj.Status = 1;

            /// <summary>恢复自动运行。</summary>
            void IMMAutoModeCmdExecutor.Resume() => PauseObj.Status = 2;

            void IMMAutoModeCmdExecutor.BeforeSwitchPF() { }

            void IMMAutoModeCmdExecutor.AfterStopWork() { }

            /// <summary>普通方法入口：Upfeed/Downfeed 进入上下料主状态机。</summary>
            GFBaseTypeParamValueList IMMAutoModeCmdExecutor.ExecMethod(string methodID, GFBaseTypeParamValueList param)
            {
                switch (methodID)
                {
                    case LoadUnloadMachineModulesConst.UpfeedMethodID:
                        return ExecuteFeedProcess(true, param);
                    case LoadUnloadMachineModulesConst.DownfeedMethodID:
                        return ExecuteFeedProcess(false, param);
                    default:
                        return CreateErrorResult($"未识别的方法: {methodID ?? "<null>"}");
                }
            }

            GFParamValueList IMMAutoModeCmdExecutor.ExecMethod(string methodID, GFParamValueList param)
            {
                return new GFParamValueList();
            }

            /// <summary>能力方法入口：与 ExecMethod 相同，走上下料流程。</summary>
            GFBaseTypeParamValueList IMMAutoModeCmdExecutor.ExecCabilityMethod(string methodID, GFBaseTypeParamValueList param)
            {
                switch (methodID)
                {
                    case LoadUnloadMachineModulesConst.UpfeedMethodID:
                        return ExecuteFeedProcess(true, param);
                    case LoadUnloadMachineModulesConst.DownfeedMethodID:
                        return ExecuteFeedProcess(false, param);
                    default:
                        return CreateErrorResult($"未识别的方法: {methodID ?? "<null>"}");
                }
            }

            /// <summary>运行时控制命令路由表：总模块只负责转发，具体动作由对应子模组执行。</summary>
            GFBaseTypeParamValueList IMMAutoModeCmdExecutor.ExecRuntimeCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                return ExecRuntimeCtlCmdCore(cmdID, cmdParam);
            }

            /// <summary>
            /// 获取界面数据对象读写接口实例供子模组调用，以便子模组能够在执行方法或上卷事件时更新界面显示；总控和子模组共用一个界面数据对象实例，避免数据不同步问题。
            /// </summary>
            /// <returns></returns>
            ICompUIDataObjPropValRW IMMAutoModeCmdExecutor.GetUIDataObjPropValRW()
            {
                if (_compUIDataObjPropValRW == null)
                    _compUIDataObjPropValRW = new LoadUnloadMMCompUIDataObjPropValRW(ExecRuntimeCtlCmdCore);
                return _compUIDataObjPropValRW;
            }

            /// <summary>预留子模组普通事件上卷入口；当前总控以主动调用并同步收敛结果为主。</summary>
            void IMMAutoModeCmdExecutor.ExecSubMMEvent(InnerAlias innerAlias, int eventID, GFBaseTypeParamValueList eventParam)
            {
                // 当前上下料机械模组主要以主动调用子机械模组为主，暂不做子机械模组普通事件上卷处理。
            }

            /// <summary>子模组能力事件上卷入口：料盒子模组状态变化后，触发一次容器状态推送。</summary>
            void IMMAutoModeCmdExecutor.ExecSubMMCabilityEvent(InnerAlias innerAlias, string eventID, GFBaseTypeParamValueList eventParam)
            {
                if (!string.Equals(innerAlias.ToString(), LoadUnloadMachineModulesConst.InnerAliasMaterialBox.ToString(), StringComparison.Ordinal))
                    return;

                if (!TryResolveInformTypeFromSubMMCabilityEvent(eventID, eventParam, out bool isUpfeed))
                    return;

                PublishMaterialContainerStatusInfo(isUpfeed);
            }

            /// <summary>回原点/复位阶段的最小动作：通知所有子模组先停机，避免残留动作继续执行。</summary>
            void IMMAutoModeCmdExecutor.ReturnToOriginal()
            {

            }

            #endregion

            #region IMMManualModeCmdExecutor

            GFBaseTypeParamValueList IMMManualModeCmdExecutor.ExecCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                return ExecRuntimeCtlCmdCore(cmdID, cmdParam);
            }

            #endregion

            #region 私有方法

            /// <summary>
            /// 命令执行路由核心方法：上料/下料流程相关命令直接路由到流程骨架，其他命令如开合料箱、查询状态等路由到对应子模组；总控不直接执行具体动作，避免与子模组职责冲突。
            /// </summary>
            /// <param name="cmdID"></param>
            /// <param name="cmdParam"></param>
            /// <returns></returns>
            private GFBaseTypeParamValueList ExecRuntimeCtlCmdCore(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                switch (cmdID)
                {
                    case LoadUnloadMachineModulesConst.StorageOpenMethodID:
                        return ExecuteStorageDeviceMethod(MaterialBoxSubMachineModulesConst.StorageOpenMethodID, cmdParam);
                    case LoadUnloadMachineModulesConst.StorageCloseMethodID:
                        return ExecuteStorageDeviceMethod(MaterialBoxSubMachineModulesConst.StorageCloseMethodID, cmdParam);
                    case LoadUnloadMachineModulesConst.GetMaterialStatus:
                        GFBaseTypeParamValueList result = new GFBaseTypeParamValueList();
                        AttachMaterialContainerStatus(result);
                        return result;
                    case LoadUnloadMachineModulesConst.RtCmdLoadOnce:
                        ExecuteFeedProcess(true, new GFBaseTypeParamValueList());
                        return new GFBaseTypeParamValueList();
                    case LoadUnloadMachineModulesConst.RtCmdUnloadOnce:
                        ExecuteFeedProcess(false, new GFBaseTypeParamValueList());
                        return new GFBaseTypeParamValueList();
                    case LoadUnloadMachineModulesConst.UpdateMaterialBoxStateMethodID:
                        return ExecuteStorageDeviceMethod(MaterialBoxSubMachineModulesConst.UpdateMaterialBoxStateMethodID, cmdParam);
                    default:
                        return CreateErrorResult(string.Format(Resources.UnrecognizedRuntimeCommandFormat, cmdID ?? "<null>"));
                }
            }

            /// <summary>转发料盒开合到子模组 StorageOpen/Close，附带 MaterialRack 与 CylinderIndex。</summary>
            private GFBaseTypeParamValueList ExecuteStorageDeviceMethod(string methodID, GFBaseTypeParamValueList cmdParam)
            {
                if (_callBack == null)
                    return CreateErrorResult(Resources.LoadUnloadNotInitializedForMaterialBoxForwarding);

                if (!TryResolveMaterialRack(cmdParam, out int rack, out string errorMsg))
                    return CreateErrorResult(errorMsg);

                GFBaseTypeParamValueList subCmdParam = new GFBaseTypeParamValueList();
                subCmdParam.Add(new GFBaseTypeParamValue("MaterialRack", new GriffinsBaseValue(rack.ToString())));
                if (int.TryParse(TryGetResultString(cmdParam, "CylinderIndex"), NumberStyles.Integer, CultureInfo.InvariantCulture, out int cylinderIndex)
                    && cylinderIndex >= 0)
                {
                    subCmdParam.Add(new GFBaseTypeParamValue(
                        "CylinderIndex",
                        new GriffinsBaseValue(cylinderIndex.ToString(CultureInfo.InvariantCulture))));
                }

                GFBaseTypeParamValueList result = _callBack.ExecSubMMMethod(
                    LoadUnloadMachineModulesConst.InnerAliasMaterialBox,
                    methodID,
                    subCmdParam);

                result ??= CreateErrorResult(Resources.MaterialBoxSubModuleReturnedNullResult);
                AttachMaterialContainerStatus(result);
                return result;
            }

            /// <summary>解析 MaterialRack(0~3)，或按 ContainerName/MagName 推断 rack。</summary>
            private static bool TryResolveMaterialRack(GFBaseTypeParamValueList cmdParam, out int rack, out string errorMsg)
            {
                rack = -1;
                errorMsg = null;

                if (int.TryParse(TryGetResultString(cmdParam, "MaterialRack"), out int parsedRack)
                    && parsedRack >= 0
                    && parsedRack <= 3)
                {
                    rack = parsedRack;
                    return true;
                }

                string storageDeviceName = TryGetResultString(cmdParam, "ContainerName");
                string storageBoxName = TryGetResultString(cmdParam, "MagName");

                if (!TryResolveStorageSide(storageDeviceName, storageBoxName, out bool isLoadSide))
                {
                    errorMsg = string.Format(
                        Resources.InvalidStorageDeviceOrBoxNameFormat,
                        storageDeviceName ?? "<null>",
                        storageBoxName ?? "<null>");
                    return false;
                }

                if (!TryResolveStorageIndex(storageBoxName, out int storageIndex))
                {
                    errorMsg = string.Format(Resources.InvalidStorageBoxNameFormat, storageBoxName ?? "<null>");
                    return false;
                }

                rack = isLoadSide
                    ? (storageIndex == 0 ? 0 : 1)
                    : (storageIndex == 0 ? 2 : 3);
                return true;
            }

            /// <summary>由储料装置名/料盒名判断上料侧或下料侧。</summary>
            private static bool TryResolveStorageSide(string storageDeviceName, string storageBoxName, out bool isLoadSide)
            {
                if (string.Equals(storageDeviceName, "LoadStorageDevice", StringComparison.OrdinalIgnoreCase))
                {
                    isLoadSide = true;
                    return true;
                }

                if (string.Equals(storageDeviceName, "UnloadStorageDevice", StringComparison.OrdinalIgnoreCase))
                {
                    isLoadSide = false;
                    return true;
                }

                if (!string.IsNullOrWhiteSpace(storageBoxName))
                {
                    if (storageBoxName.StartsWith("LoadStorageDevice", StringComparison.OrdinalIgnoreCase))
                    {
                        isLoadSide = true;
                        return true;
                    }

                    if (storageBoxName.StartsWith("UnloadStorageDevice", StringComparison.OrdinalIgnoreCase))
                    {
                        isLoadSide = false;
                        return true;
                    }
                }

                isLoadSide = false;
                return false;
            }

            /// <summary>由料盒名 Upper/Lower 后缀解析储料位索引 0/1。</summary>
            private static bool TryResolveStorageIndex(string storageBoxName, out int storageIndex)
            {
                storageIndex = -1;
                if (string.IsNullOrWhiteSpace(storageBoxName))
                    return false;

                if (storageBoxName.EndsWith("UpperRack", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(storageBoxName, "UpperRack", StringComparison.OrdinalIgnoreCase))
                {
                    storageIndex = 0;
                    return true;
                }

                if (storageBoxName.EndsWith("LowerRack", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(storageBoxName, "LowerRack", StringComparison.OrdinalIgnoreCase))
                {
                    storageIndex = 1;
                    return true;
                }

                return false;
            }

            /// <summary>统一的上下料流程骨架；上料/下料共用同一套状态机，只在命令路由上区分方向。</summary>
            private GFBaseTypeParamValueList ExecuteFeedProcess(bool isUpfeed, GFBaseTypeParamValueList param)
            {
                FeedState state = FeedState.Start;
                GFBaseTypeParamValueList latestResult = CreateResult();
                GFBaseTypeParamValueList cmdParam = param ?? new GFBaseTypeParamValueList();
                string returnValueOverride = null;
                bool hasMaterial = true;
                int curContinuousEmptyPushThreshold = 0;
                while (state != FeedState.End)
                {
                    switch (state)
                    {
                        case FeedState.Start:
                            // 移动到当前料槽
                            if (isUpfeed)
                                latestResult = _callBack.ExecSubMMMethod(LoadUnloadMachineModulesConst.InnerAliasMaterialBox, MaterialBoxSubMachineModulesConst.UpfeedMoveCurrentSlotMethodID, cmdParam);
                            else
                                latestResult = _callBack.ExecSubMMMethod(LoadUnloadMachineModulesConst.InnerAliasMaterialBox, MaterialBoxSubMachineModulesConst.DownfeedMoveCurrentSlotMethodID, cmdParam);

                            // 状态机入口：先进入判料阶段。
                            state = FeedState.SenseHasMaterial;
                            break;
                        case FeedState.SenseHasMaterial:
                            // 读取当前方向的有料信号；这里可能带持续时间判稳。
                            hasMaterial = ObserveHasMaterialSignal(isUpfeed, cmdParam);
                            state = FeedState.CheckHasMaterialResult;
                            break;
                        case FeedState.CheckHasMaterialResult:
                            if (hasMaterial)
                            {
                                // 检测到有料，清空连续空推计数，继续执行推料。
                                state = FeedState.CallPushRodExtend;
                            }
                            else
                            {
                                state = FeedState.CallPushRodExtend;
                            }
                            break;
                        case FeedState.CallPushRodExtend:
                            // 调用当前方向对应的推杆执行“伸出/前推”动作。
                            latestResult = ExecutePushRodMove(isUpfeed, true, cmdParam);
                            state = FeedState.CheckPushRodExtendResult;
                            break;
                        case FeedState.CheckPushRodExtendResult:
                            if (IsSuccess(latestResult))
                            {
                                // 前推动作成功后，继续执行推杆回退。
                                state = FeedState.CallPushRodRetract;
                            }
                            else
                            {
                                // 前推失败时，优先保留/映射底层失败事件，随后统一走失败出口。
                                returnValueOverride = ResolvePushRodExtendReturnValue(latestResult);
                                state = FeedState.TriggerFail;
                            }
                            break;
                        case FeedState.CallPushRodRetract:
                            // 调用当前方向对应的推杆执行“回退”动作。
                            latestResult = ExecutePushRodMove(isUpfeed, false, cmdParam);
                            state = FeedState.CheckPushRodRetractResult;
                            break;
                        case FeedState.CheckPushRodRetractResult:
                            if (IsSuccess(latestResult))
                            {
                                cmdParam.Add(new GFBaseTypeParamValue("IsFeeding", new GriffinsBaseValue(isUpfeed)));
                                cmdParam.Add(new GFBaseTypeParamValue("HasMaterial", new GriffinsBaseValue(!isUpfeed)));
                                _callBack.ExecSubMMMethod(LoadUnloadMachineModulesConst.InnerAliasMaterialBox, MaterialBoxSubMachineModulesConst.SetCurrentSlotStatusMethodID, cmdParam);
                                // 推杆完全回退后，再通知料盒切换到下一槽位。
                                state = FeedState.CallMoveToNextSlot;
                            }
                            else
                            {
                                // 回退失败同样统一映射返回值，然后走失败出口。
                                returnValueOverride = ResolvePushRodRetractReturnValue(latestResult);
                                state = FeedState.TriggerFail;
                            }
                            break;
                        case FeedState.CallMoveToNextSlot:
                            // 料盒子模组负责真正的“切下一槽”动作与边界判断。
                            if (isUpfeed)
                                latestResult = _callBack.ExecSubMMMethod(LoadUnloadMachineModulesConst.InnerAliasMaterialBox, MaterialBoxSubMachineModulesConst.UpfeedMoveNextSlotMethodID, cmdParam);
                            else
                                latestResult = _callBack.ExecSubMMMethod(LoadUnloadMachineModulesConst.InnerAliasMaterialBox, MaterialBoxSubMachineModulesConst.DownfeedMoveNextSlotMethodID, cmdParam);
                            state = FeedState.CheckMoveToNextSlotResult;
                            break;
                        case FeedState.CheckMoveToNextSlotResult:
                            // 根据切槽结果进入统一成功出口或失败出口。
                            state = IsSuccess(latestResult) ? FeedState.TriggerSuccess : FeedState.TriggerFail;
                            break;
                        case FeedState.CheckMaterialState:
                            {
                                cmdParam.Add(new GFBaseTypeParamValue("Index", new GriffinsBaseValue(0)));
                                cmdParam.Add(new GFBaseTypeParamValue("Type", new GriffinsBaseValue((isUpfeed ? FeedPortRole.Feed : FeedPortRole.Receive).ToString())));
                                latestResult = _callBack.ExecSubMMMethod(LoadUnloadMachineModulesConst.InnerAliasMaterialBox, MaterialBoxSubMachineModulesConst.GetMaterialStateMethodID, cmdParam);
                                bool hasMaterialAfterPush = IsSuccess(latestResult) && TryGetResultBool(latestResult, "Result");
                                if (isUpfeed)
                                {
                                    if (curContinuousEmptyPushThreshold > initCfg.ContinuousEmptyPushThreshold)
                                    {
                                        // 连续空推超过阈值时直接判定为失败，避免反复推料导致设备损伤。
                                        state = FeedState.TriggerFail;
                                    }
                                    else
                                    {
                                        state = hasMaterialAfterPush ? FeedState.TriggerSuccess : FeedState.SenseHasMaterial;
                                        curContinuousEmptyPushThreshold++;
                                    }
                                }
                                else
                                {
                                    state = hasMaterialAfterPush ? FeedState.TriggerSuccess : FeedState.TriggerFail;
                                }
                            }
                            break;
                        case FeedState.TriggerSuccess:
                            // 汇总本次流程结果，并打上总控成功事件。
                            latestResult = BuildFeedResult(isUpfeed, true, latestResult, null);
                            state = FeedState.End;
                            break;
                        case FeedState.TriggerFail:
                            // 汇总本次流程结果，并打上总控失败事件与最终 ReturnValue。
                            latestResult = BuildFeedResult(isUpfeed, false, latestResult, returnValueOverride);
                            state = FeedState.End;
                            break;
                        default:
                            // 兜底保护：出现未知状态时直接按失败返回。
                            latestResult = CreateErrorResult("未知上下料状态");
                            state = FeedState.End;
                            break;
                    }
                }
                PublishMaterialContainerStatusInfo(isUpfeed);
                AttachMaterialContainerStatus(latestResult);
                return latestResult;
            }

            /// <summary>按配置选择电机推杆或气缸推杆，并将伸出/回退动作路由到对应子模组。</summary>
            private GFBaseTypeParamValueList ExecutePushRodMove(bool isUpfeed, bool extend, GFBaseTypeParamValueList param)
            {
                var rtn = new GFBaseTypeParamValueList();
                if (isUpfeed)
                {
                    // 电机推杆：通过运行时控制命令转发给电机推杆子模组。
                    if(extend)
                        rtn = _callBack.ExecSubMMMethod(LoadUnloadMachineModulesConst.InnerAliasLoadPushRod, PushRodSubMachineModulesConst.ExtendMethodID, new GFBaseTypeParamValueList());
                    else
                        rtn = _callBack.ExecSubMMMethod(LoadUnloadMachineModulesConst.InnerAliasLoadPushRod, PushRodSubMachineModulesConst.RetractMethodID, new GFBaseTypeParamValueList());
                }
                else
                {
                    if(extend)
                        rtn = _callBack.ExecSubMMMethod(LoadUnloadMachineModulesConst.InnerAliasUnLoadPushRod, PushRodSubMachineModulesConst.ExtendMethodID, new GFBaseTypeParamValueList());
                    else
                        rtn = _callBack.ExecSubMMMethod(LoadUnloadMachineModulesConst.InnerAliasUnLoadPushRod, PushRodSubMachineModulesConst.RetractMethodID, new GFBaseTypeParamValueList());
                }
                return rtn;

            }

            /// <summary>汇总子模组结果为总控统一返回值，并补齐总流程事件号与 ReturnValue。</summary>
            private GFBaseTypeParamValueList BuildFeedResult(bool isUpfeed, bool success, GFBaseTypeParamValueList source, string returnValueOverride)
            {
                GFBaseTypeParamValueList result = CreateResult();
                if (source != null)
                {
                    CopyResultValue(source, result, "Result");
                    CopyResultValue(source, result, "errorMsg");
                    CopyResultValue(source, result, "data");
                    CopyResultValue(source, result, "EventID");
                    CopyResultValue(source, result, "WarningEventID");
                }

                string sourceEventID = TryGetResultString(source, "EventID");
                result["SourceEventID"] = new GriffinsBaseValue(sourceEventID ?? "");

                string flowEventID = success
                    ? (isUpfeed ? LoadUnloadMachineModulesConst.UpfeedSuccessEventID : LoadUnloadMachineModulesConst.DownfeedSuccessEventID)
                    : (isUpfeed ? LoadUnloadMachineModulesConst.UpfeedFailEventID : LoadUnloadMachineModulesConst.DownfeedFailEventID);
                result["EventID"] = new GriffinsBaseValue(flowEventID);

                string returnValue = returnValueOverride;
                if (string.IsNullOrWhiteSpace(returnValue))
                    returnValue = sourceEventID;
                if (string.IsNullOrWhiteSpace(returnValue))
                    returnValue = TryGetResultString(source, "EventID");
                if (string.IsNullOrWhiteSpace(returnValue))
                    returnValue = TryGetResultString(source, "WarningEventID");
                if (string.IsNullOrWhiteSpace(returnValue))
                    returnValue = TryGetResultString(source, "data");
                if (string.IsNullOrWhiteSpace(returnValue))
                    returnValue = flowEventID;
                result["ReturnValue"] = new GriffinsBaseValue(returnValue);
                return result;
            }

            /// <summary>按配置的感应持续时间观察“有料”信号；持续时间内任一时刻掉料都判定为无料。</summary>
            private bool ObserveHasMaterialSignal(bool isUpfeed, GFBaseTypeParamValueList cmdParam)
            {
                double senseDuration = ppCfg.HasMaterialSignalSenseDuration;
                if (senseDuration <= 0)
                    return QueryHasMaterial(isUpfeed, cmdParam);

                DateTime deadline = DateTime.UtcNow.AddMilliseconds(senseDuration);
                while (DateTime.UtcNow < deadline)
                {
                    if (!QueryHasMaterial(isUpfeed, cmdParam))
                        return false;

                    Thread.Sleep(10);
                }

                return QueryHasMaterial(isUpfeed, cmdParam);
            }

            /// <summary>向当前方向对应的推杆子模组发起判料查询，不直接关心底层是 IO 还是位置传感。</summary>
            private bool QueryHasMaterial(bool isUpfeed, GFBaseTypeParamValueList cmdParam)
            {
                InnerAlias pushRodAlias = GetPushRodExecutor(isUpfeed);
                string methodID = PushRodSubMachineModulesConst.CheckHasMaterialMethodID;

                GFBaseTypeParamValueList result = _callBack.ExecSubMMMethod(pushRodAlias, methodID, cmdParam ?? new GFBaseTypeParamValueList());
                return IsSuccess(result) && TryGetResultBool(result, "HasMaterial");
            }

            /// <summary>根据上料/下料方向返回当前生效的推杆执行器实例。</summary>
            private InnerAlias GetPushRodExecutor(bool isUpfeed)
            {
                return isUpfeed ? LoadUnloadMachineModulesConst.InnerAliasLoadPushRod : LoadUnloadMachineModulesConst.InnerAliasUnLoadPushRod;
            }

            /// <summary>累计连续空推次数；上下料方向分别独立计数。</summary>
            private int IncreaseContinuousEmptyPushCount(bool isUpfeed)
            {
                if (isUpfeed)
                {
                    upfeedContinuousEmptyPushCounter++;
                    return upfeedContinuousEmptyPushCounter;
                }

                downfeedContinuousEmptyPushCounter++;
                return downfeedContinuousEmptyPushCounter;
            }

            /// <summary>检测到有料后清零当前方向的连续空推计数。</summary>
            private void ResetContinuousEmptyPushCount(bool isUpfeed)
            {
                if (isUpfeed)
                    upfeedContinuousEmptyPushCounter = 0;
                else
                    downfeedContinuousEmptyPushCounter = 0;
            }

            /// <summary>推杆伸出失败时优先沿用子模组事件号；缺省时统一映射为卡料事件。</summary>
            private static string ResolvePushRodExtendReturnValue(GFBaseTypeParamValueList source)
            {
                string sourceEventID = TryGetResultString(source, "EventID");
                if (string.Equals(sourceEventID, PushRodSubMachineModulesConst.EventPushJam, StringComparison.Ordinal) ||
                    string.Equals(sourceEventID, PushRodSubMachineModulesConst.EventPushJam, StringComparison.Ordinal))
                {
                    return sourceEventID;
                }

                return PushRodSubMachineModulesConst.EventPushJam;
            }

            /// <summary>推杆回退失败时尽量保留底层事件号，缺省时返回统一的回退失败事件。</summary>
            private static string ResolvePushRodRetractReturnValue(GFBaseTypeParamValueList source)
            {
                string sourceEventID = TryGetResultString(source, "EventID");
                if (!string.IsNullOrWhiteSpace(sourceEventID))
                    return sourceEventID;

                return PushRodSubMachineModulesConst.EventPusherRetractFailed;
            }

            /// <summary>将料盒容器状态 JSON 写入返回包并同步 UIDataObj。</summary>
            private void AttachMaterialContainerStatus(GFBaseTypeParamValueList result)
            {
                try
                {
                    if (result == null)
                        return;

                    GriffinsBaseValue materialContainerStatusJson = TryGetMaterialContainerStatusJson();
                    if (materialContainerStatusJson == null)
                        return;

                    result[LoadUnloadMachineModulesConst.MaterialContainerStatusPropertyID] = materialContainerStatusJson;
                    UpdateUIDataObjMaterialContainerStatus(materialContainerStatusJson);
                }
                catch (Exception ex)
                {
                }
            }

            /// <summary>主动推送上/下料侧料盒容器状态（Inform + UIDataObj）。</summary>
            private void PublishMaterialContainerStatusInfo(bool isUpfeed)
            {
                var materialContainerStatusJson = TryGetMaterialContainerStatusJson();
                if (materialContainerStatusJson == null)
                    return;

                if (_callBack == null)
                    return;

                string informType = GetInformType(isUpfeed);
                UpdateUIDataObjMaterialContainerStatus(materialContainerStatusJson);
                var notify = new LoadUnloadMaterialContainerStatusNotify
                {
                    Type = informType,
                    Param = materialContainerStatusJson.ToObjectValue_Json().JsonVal
                };
                string paramJson = JsonObjConvert.ToJSon(notify);

                _callBack.SendToMapTmlStateChanged(paramJson);
            }

            /// <summary>调料盒子模组 GetMaterialContainerStatus 获取容器状态 JSON。</summary>
            private GriffinsBaseValue TryGetMaterialContainerStatusJson()
            {
                try
                {
                    GFBaseTypeParamValueList result = _callBack?.ExecSubMMMethod(
                        LoadUnloadMachineModulesConst.InnerAliasMaterialBox,
                        MaterialBoxSubMachineModulesConst.GetMaterialContainerStatusMethodID,
                        new GFBaseTypeParamValueList());
                    return result["data"];
                }
                catch
                {
                    return null;
                }
            }

            /// <summary>更新总控界面数据对象中的料盒容器状态属性。</summary>
            private void UpdateUIDataObjMaterialContainerStatus(GriffinsBaseValue materialContainerStatusJson)
            {
                if (_compUIDataObjPropValRW is LoadUnloadMMCompUIDataObjPropValRW compUIDataObjPropValRW)
                {
                    compUIDataObjPropValRW.UpdateMaterialContainerStatus(materialContainerStatusJson);
                }
            }

            /// <summary>上/下料 Inform 类型字符串。</summary>
            private static string GetInformType(bool isUpfeed) => isUpfeed ? UpfeedInformType : DownfeedInformType;

            /// <summary>解析 Inform 中的 Type 字段为上/下料方向。</summary>
            private static bool TryParseInformType(string type, out bool isUpfeed)
            {
                if (string.Equals(type, UpfeedInformType, StringComparison.Ordinal))
                {
                    isUpfeed = true;
                    return true;
                }

                if (string.Equals(type, DownfeedInformType, StringComparison.Ordinal))
                {
                    isUpfeed = false;
                    return true;
                }

                isUpfeed = false;
                return false;
            }

            /// <summary>从子模组能力事件的 Side 或 EventID 前缀推断上/下料。</summary>
            private static bool TryResolveInformTypeFromSubMMCabilityEvent(string eventID, GFBaseTypeParamValueList eventParam, out bool isUpfeed)
            {
                string side = TryGetResultString(eventParam, "Side");
                if (string.Equals(side, "Load", StringComparison.OrdinalIgnoreCase))
                {
                    isUpfeed = true;
                    return true;
                }

                if (string.Equals(side, "Unload", StringComparison.OrdinalIgnoreCase))
                {
                    isUpfeed = false;
                    return true;
                }

                if (!string.IsNullOrWhiteSpace(eventID))
                {
                    if (eventID.StartsWith("Upfeed", StringComparison.Ordinal)
                        || eventID.StartsWith("Load", StringComparison.Ordinal))
                    {
                        isUpfeed = true;
                        return true;
                    }

                    if (eventID.StartsWith("Downfeed", StringComparison.Ordinal)
                        || eventID.StartsWith("Unload", StringComparison.Ordinal))
                    {
                        isUpfeed = false;
                        return true;
                    }
                }

                isUpfeed = false;
                return false;
            }

            /// <summary>安全复制返回字典中的单个字段。</summary>
            private static void CopyResultValue(GFBaseTypeParamValueList source, GFBaseTypeParamValueList target, string key)
            {
                try
                {
                    GriffinsBaseValue value = source[key];
                    if (value != null)
                    {
                        target[key] = value;
                    }
                }
                catch
                {
                }
            }

            /// <summary>判断 Result 字段是否为 "0"。</summary>
            private static bool IsSuccess(GFBaseTypeParamValueList result)
            {
                try
                {
                    GriffinsBaseValue value = result?["Result"];
                    return value != null && string.Equals(value.ToString(), "0", StringComparison.Ordinal);
                }
                catch
                {
                    return false;
                }
            }

            /// <summary>安全读取返回字典中的字符串字段。</summary>
            private static string TryGetResultString(GFBaseTypeParamValueList result, string key)
            {
                try
                {
                    GriffinsBaseValue value = result?[key];
                    return value?.ToString();
                }
                catch
                {
                    return null;
                }
            }

            /// <summary>安全读取返回字典中的布尔字段。</summary>
            private static bool TryGetResultBool(GFBaseTypeParamValueList result, string key)
            {
                try
                {
                    GriffinsBaseValue value = result?[key];
                    return value != null && value.ToBool();
                }
                catch
                {
                    return false;
                }
            }

            /// <summary>总控统一返回结构；普通方法、运行时命令和流程状态机都复用这套字段。</summary>
            private static GFBaseTypeParamValueList CreateResult()
            {
                GFBaseTypeParamValueList result = new GFBaseTypeParamValueList();
                result.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue("0")));
                result.Add(new GFBaseTypeParamValue("errorMsg", new GriffinsBaseValue("")));
                result.Add(new GFBaseTypeParamValue("data", new GriffinsBaseValue("")));
                result.Add(new GFBaseTypeParamValue("EventID", new GriffinsBaseValue("")));
                result.Add(new GFBaseTypeParamValue("SourceEventID", new GriffinsBaseValue("")));
                result.Add(new GFBaseTypeParamValue("WarningEventID", new GriffinsBaseValue("")));
                result.Add(new GFBaseTypeParamValue("ReturnValue", new GriffinsBaseValue("")));
                result.Add(new GFBaseTypeParamValue(LoadUnloadMachineModulesConst.MaterialContainerStatusPropertyID, new GriffinsBaseValue("")));
                return result;
            }

            /// <summary>构造标准错误结果，只覆盖 Result 和 errorMsg，其余字段保持统一结构。</summary>
            private static GFBaseTypeParamValueList CreateErrorResult(string errorMsg)
            {
                GFBaseTypeParamValueList result = CreateResult();
                result["Result"] = new GriffinsBaseValue("-1");
                result["errorMsg"] = new GriffinsBaseValue(errorMsg ?? "");
                return result;
            }
            /// <summary>服务端状态 Inform 回调：解析后刷新对应侧料盒容器显示。</summary>
            //private void ProcessInformInfo_StatusChanged(GriffinsInfoKindID infoKind, string infoNo, InformInfoBase info)
            //{
            //    if (info is not InformInfo_StatusChanged informInfo)
            //        return;

            //    string aliasText = alias.ToString();
            //    if (!string.Equals(informInfo.Alias, aliasText, StringComparison.Ordinal))
            //        return;

            //    if (string.IsNullOrWhiteSpace(informInfo.Param))
            //    {
            //        IGriffinsBaseValue value = new MaterialContainerStatusList();
            //        //UpdateUIDataObjMaterialContainerStatus(value.ToBaseValue());
            //        return;
            //    }

            //    LoadUnloadMaterialContainerStatusNotify notify;
            //    try
            //    {
            //        notify = JsonObjConvert.FromJSon<LoadUnloadMaterialContainerStatusNotify>(informInfo.Param);
            //    }
            //    catch
            //    {
            //        return;
            //    }

            //    if (notify == null || !TryParseInformType(notify.Type, out bool isUpfeed))
            //        return;

            //    PublishMaterialContainerStatusInfo(isUpfeed);
            //}
            ///// <summary>注册服务端 InformInfo_StatusChanged 处理（总控进程内）。</summary>
            //private void RegisterInformInfoProcessDelegate()
            //{
            //    ServerInfoProcessRegister.RegisterInformInfoProcessDelegate(
            //        InformInfo_StatusChanged.InfoKindID,
            //        ProcessInformInfo_StatusChanged);
            //}

            ///// <summary>注销 Inform 处理委托。</summary>
            //private void UnRegisterInformInfoProcessDelegate()
            //{
            //    ServerInfoProcessRegister.UnRegisterInformInfoProcessDelegate(
            //        InformInfo_StatusChanged.InfoKindID,
            //        ProcessInformInfo_StatusChanged);
            //}

            #endregion
        }
    }
}
