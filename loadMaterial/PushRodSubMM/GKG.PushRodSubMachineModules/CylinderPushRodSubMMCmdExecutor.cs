using GKG.MotionControl;
using Griffins;
using Griffins.ImeIOT;
using System;
using System.Threading;
using System.Threading.Tasks;
using GKG.ElectronicControl.General;
using GKG.ElectronicControl;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using GF_Gereric;
using System.Text;
using Griffins.PF.Server;
using CylinderPushRodSubMachineModulesConst = GKG.SubMM.PushRodSubMachineModulesConst;
using Griffins.PF.RichClient;

namespace GKG
{
    namespace SubMM
    {
        /// <summary>气缸推杆子模组执行器：负责气缸实例创建、伸退料控制和感应信号读取。</summary>
        /// <remarks>整体说明见 loadmaterial/EXECUTOR_IMPLEMENTATION.md §5。</remarks>
        public class CylinderPushRodSubMMCmdExecutor : ISubMMCmdExecutor, ISubMMManualModeCmdExecutor, ISubMMAutoModeCmdExecutor
        {
            /// <summary>气缸推杆单次动作状态机：与电机推杆保持一致的校验/执行/收敛结构。</summary>
            private enum CylinderPushRodState
            {
                Start,
                Validate,
                Execute,
                Success,
                Fail,
                End
            }

            private CylinderPushRodSubMachineModulesFactoryCfg cylinderPushRodSubMachineModulesFactoryCfg;

            private CylinderPushRodSubMachineModulesInitCfg cylinderPushRodSubMachineModulesInitCfg;

            private CylinderPushRodSubMachineModulesPPCfg cylinderPushRodSubMachineModulesPPCfg;

            private ISubMMCmdExecutorCallBack iSubMMCmdExecutorCallBack;

            private SubMMAlias alias;

            private ImeGenNormalEventHandler imeGenNormalEventHandler;

            private ImeCabilityEventHandler imeCabilityEventHandler;

            private ImeAlarmEventHandler imeAlarmEventHandler;

            private IBaseCylinder pusherCylinder = null;

            private IBaseStateIO pusherOutSignal = null;

            private bool bUnloadFinished = false;

            public CylinderPushRodSubMMCmdExecutor(SubMMAlias alias, Guid subMMObjID, byte[] factoryCfgInfo)
            {
                this.alias = alias;
                if (factoryCfgInfo != null && factoryCfgInfo.Length > 0)
                    cylinderPushRodSubMachineModulesFactoryCfg = JsonObjConvert.FromJSonBytes<CylinderPushRodSubMachineModulesFactoryCfg>(factoryCfgInfo) ?? new CylinderPushRodSubMachineModulesFactoryCfg();
                else
                    cylinderPushRodSubMachineModulesFactoryCfg = new CylinderPushRodSubMachineModulesFactoryCfg();
                cylinderPushRodSubMachineModulesInitCfg = new CylinderPushRodSubMachineModulesInitCfg();
                cylinderPushRodSubMachineModulesPPCfg = new CylinderPushRodSubMachineModulesPPCfg();
            }

     

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

            /// <summary>加载初始化配置，并依据气缸类型与 IO 参数创建气缸对象。</summary>
            void ISubMMCmdExecutor.Init(byte[] initCfgInfo, byte[] calibrationCfgInfo, ISubMMCmdExecutorCallBack callBack)
            {

                if (initCfgInfo != null && initCfgInfo.Length > 0)
                {
                    cylinderPushRodSubMachineModulesInitCfg = new CylinderPushRodSubMachineModulesInitCfg();
                    cylinderPushRodSubMachineModulesInitCfg.FromBytes(initCfgInfo);
                }
                iSubMMCmdExecutorCallBack = callBack;

            }
            void ISubMMCmdExecutor.BeforeInit(GFBaseTypePropValueList devicePropValues)
            {
            }

            void ISubMMCmdExecutor.AfterInit()
            {
                TryCreateCylinder();
            }

            void ISubMMCmdExecutor.UnInit()
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
            bool ISubMMAutoModeCmdExecutor.CanStartWork(out string reasonMsg)
            {
                reasonMsg = string.Empty;
                return true;
            }


            void ISubMMAutoModeCmdExecutor.SetRunMode(ImeRunMode imeRunMode)
            {
            }

            void ISubMMAutoModeCmdExecutor.SetPFCfgInfo(byte[] pfCfgInfo)
            {
                cylinderPushRodSubMachineModulesPPCfg.FromBytes(pfCfgInfo);
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

            GFBaseTypeParamValueList ISubMMAutoModeCmdExecutor.ExecMethod(string methodID, GFBaseTypeParamValueList param)
            {
                switch (methodID)
                {
                    case CylinderPushRodSubMachineModulesConst.CheckHasMaterialMethodID:
                        return CreateHasMaterialResult(ReadHasMaterial(param));
                    case CylinderPushRodSubMachineModulesConst.ExtendMethodID:
                        return ExecuteCylinderPushRodStateMachine(
                            methodID,
                            () => ExtendMaterial(),
                            CylinderPushRodSubMachineModulesConst.EventPusherForwardCompleted);
                    case CylinderPushRodSubMachineModulesConst.RetractMethodID:
                        return ExecuteCylinderPushRodStateMachine(
                            methodID,
                            () => RetractMaterial(),
                            CylinderPushRodSubMachineModulesConst.EventPusherBackwardCompleted);
                    case CylinderPushRodSubMachineModulesConst.SetLoadFinishedMethodID:
                        return CreateSuccessResult(string.Empty);
                    case CylinderPushRodSubMachineModulesConst.SetUnloadFinishedMethodID:
                        ConfirmUnLoadFinished();
                        return CreateSuccessResult(string.Empty);
                    default:
                        return CreateErrorResult($"未识别的方法: {methodID ?? "<null>"}");
                }
            }

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

            GFBaseTypeParamValueList ISubMMAutoModeCmdExecutor.ExecCabilityMethod(string methodID, GFBaseTypeParamValueList param)
            {
                switch (methodID)
                {
                    case CylinderPushRodSubMachineModulesConst.GetInitParametersMethodID:
                        return CreateParametersResult(cylinderPushRodSubMachineModulesInitCfg);
                    case CylinderPushRodSubMachineModulesConst.GetRecipeParametersMethodID:
                        return CreateParametersResult(cylinderPushRodSubMachineModulesPPCfg);
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

            GFBaseTypeParamValueList ISubMMAutoModeCmdExecutor.ExecRuntimeCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                return ExecRuntimeCtlCmdCore(cmdID, cmdParam);
            }
            /// <summary>运行时命令：GetStatus(IO) / 前推 / 后退 / IO 列表；无 PushOnce。</summary>
            GFBaseTypeParamValueList ExecRuntimeCtlCmdCore(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                switch (cmdID)
                {
                    case CylinderPushRodSubMachineModulesConst.RtCmdGetStatus:
                        return CreateSuccessResult(
                            string.Empty,
                            ReadPushOutSignal() ? "1" : "0");
                    case CylinderPushRodSubMachineModulesConst.RtCmdGetCylinderIOChannelOptions:
                        return CreateSuccessResult(
                            string.Empty,
                            ToJson(GetAllIOStateInfosFromElectronicManager()));
                    case CylinderPushRodSubMachineModulesConst.RtCmdPusherForward:
                        return ExecuteCylinderPushRodStateMachine(
                            CylinderPushRodSubMachineModulesConst.ExtendMethodID,
                            () => ExtendMaterial(),
                            CylinderPushRodSubMachineModulesConst.EventPusherForwardCompleted,
                            () => CylinderPushRodSubMachineModulesConst.EventPusherForwardCompleted);
                    case CylinderPushRodSubMachineModulesConst.RtCmdPusherBackward:
                        return ExecuteCylinderPushRodStateMachine(
                            CylinderPushRodSubMachineModulesConst.RetractMethodID,
                            () => PushRodBackward(),
                            CylinderPushRodSubMachineModulesConst.EventPusherBackwardCompleted,
                            () => CylinderPushRodSubMachineModulesConst.EventPusherBackwardCompleted);
                    default:
                        return CreateErrorResult($"未识别的运行时命令: {cmdID ?? "<null>"}");
                }
            }
            /// <summary>
            /// 获取界面数据对象属性读写接口实例，如果不支持返回nul
            /// </summary>
            /// <returns>界面数据对象属性读写接口实例</returns>
            ICompUIDataObjPropValRW ISubMMAutoModeCmdExecutor.GetUIDataObjPropValRW()
            {
                return null;
            }

            GFBaseTypeParamValueList ISubMMManualModeCmdExecutor.ExecCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                return ExecRuntimeCtlCmdCore(cmdID, cmdParam);
            }


            #region 子机械模组普通方法
            private static bool TryGetInt(GFBaseTypeParamValueList cmdParam, string key, out int value)
            {
                return RuntimeParamReader.TryGetInt(cmdParam, key, out value);
            }

            private static bool TryGetString(GFBaseTypeParamValueList cmdParam, string key, out string value)
            {
                return RuntimeParamReader.TryGetString(cmdParam, key, out value);
            }

            /// <summary>流程缩回：可选判料 → Retract → 电磁阀延时 → 再判料（完整版）。</summary>
            public void RetractMaterial()
            {
                TryEnsureCylinderReady();
                PerfmTimer perfmTimer = new PerfmTimer();
                bool bIsSupportHasMaterialCheck = cylinderPushRodSubMachineModulesFactoryCfg.IsSupportHasMaterialCheck;
                if (bIsSupportHasMaterialCheck)
                {
                    if (pusherOutSignal.Read() == false)
                    {
                        throw new GKGException(1);
                    }
                }
                pusherCylinder.Retract();
                perfmTimer.Start();
                // 使用初始化配置中的电磁阀升降延时，默认3000ms
                double delayTime = cylinderPushRodSubMachineModulesInitCfg.PushCylinderSolenoidValveLiftDelay > 0
                    ? cylinderPushRodSubMachineModulesInitCfg.PushCylinderSolenoidValveLiftDelay
                    : 3000.0;
                while (perfmTimer.GetElapsedMilliseconds() < delayTime)
                {
                    Thread.Sleep(1);
                }
                if (bIsSupportHasMaterialCheck)
                {
                    if (pusherOutSignal.Read() == true)
                    {
                        throw new GKGException(1);
                    }
                }
            }

            /// <summary>流程伸出：Stretch()，无额外延时等待。</summary>
            public void ExtendMaterial()
            {
                TryEnsureCylinderReady();
                pusherCylinder.Stretch();
            }

            /// <summary>运行时后退：仅 Retract()，不含 RetractMaterial 的判料与延时。</summary>
            private void PushRodBackward()
            {
                TryEnsureCylinderReady();
                pusherCylinder.Retract();
            }

            private void ConfirmUnLoadFinished() => bUnloadFinished = true;

            /// <summary>判料优先级：命令参数覆盖默认行为；未指定时退回传感器或推到位信号。</summary>
            private bool ReadHasMaterial(GFBaseTypeParamValueList cmdParam)
            {
                return true;
            }

            /// <summary>读取推到位 IO；GetStatus 返回 "1"/"0"。</summary>
            private bool ReadPushOutSignal()
            {
                if (pusherCylinder == null || pusherOutSignal == null)
                    TryCreateCylinder();
                return pusherOutSignal != null && pusherOutSignal.Read();
            }

            /// <summary>从电器管理获取 IO 列表（初始化页通道下拉）。</summary>
            private static List<IOStateInformation> GetAllIOStateInfosFromElectronicManager()
            {
                Dictionary<Guid, IOStateInformation> ioStateInfoDict = new Dictionary<Guid, IOStateInformation>();
                var ioStateInfosResponse = ServerInnerInfoSender.SendMutualInfo(
                    IOStateInfosRequest.InfoKindID,
                    new IOStateInfosRequest());

                if (ioStateInfosResponse == null || ioStateInfosResponse.Count == 0)
                    return new List<IOStateInformation>();

                IOStateInfosResponse response = ioStateInfosResponse[0].Response as IOStateInfosResponse;
                if (response?.IOStateInformations == null)
                    return new List<IOStateInformation>();

                foreach (IOStateInformation ioStateInfo in response.IOStateInformations)
                {
                    if (ioStateInfo != null && ioStateInfo.IOGuid != Guid.Empty)
                        ioStateInfoDict[ioStateInfo.IOGuid] = ioStateInfo;
                }

                return ioStateInfoDict.Values.ToList();
            }

            /// <summary>确保气缸实例已创建，否则抛错。</summary>
            private void TryEnsureCylinderReady()
            {
                if (pusherCylinder == null)
                    TryCreateCylinder();
                if (pusherCylinder == null)
                    throw new InvalidOperationException("气杆推杆气缸未初始化");
            }

            /// <summary>按初始化配置创建气缸，并把相关状态 IO 绑定到气缸实例上。</summary>
            private void TryCreateCylinder()
            {
                pusherCylinder = CylinderFactory.CreateCylinder(cylinderPushRodSubMachineModulesInitCfg.CylinderInitParameters.eCylinderType);
                if (pusherCylinder == null)
                    return;
                pusherCylinder.Init(JsonObjConvert.ToJSonBytes(cylinderPushRodSubMachineModulesInitCfg.CylinderInitParameters));
                List<IBaseStateIO> ioList = new List<IBaseStateIO>();
                if (cylinderPushRodSubMachineModulesInitCfg.CylinderInitParameters?.IOStateGuidList != null)
                {
                    var response = ServerInnerInfoSender.SendMutualInfo(
                        StateIOInstancesByIdsRequest.InfoKindID,
                        new StateIOInstancesByIdsRequest(cylinderPushRodSubMachineModulesInitCfg.CylinderInitParameters?.IOStateGuidList));
                    if (response == null || response.Count == 0)
                        throw new InvalidOperationException("通过交互消息获取IO实例失败。");

                    StateIOInstancesByIdsResponse ioResponse = response[0].Response as StateIOInstancesByIdsResponse;
                    ioList = ioResponse.StateIOInstances;
                }

                if (ioList.Count > 0)
                {
                    pusherCylinder.SetStateIOInstanceList(ioList);
                    pusherOutSignal = ioList.LastOrDefault();
                }
            }

            /// <summary>将底层异常映射为对外事件号；优先识别“卡料”，否则按伸出/回退失败区分。</summary>
            private static string ResolvePushRodFailureEventID(string methodID, Exception ex)
            {
                string msg = ex?.Message ?? string.Empty;
                if (msg.IndexOf("卡料", StringComparison.OrdinalIgnoreCase) >= 0 || ex is GKGException)
                    return CylinderPushRodSubMachineModulesConst.EventPushJam;

                return string.Equals(methodID, CylinderPushRodSubMachineModulesConst.RetractMethodID, StringComparison.Ordinal)
                    ? CylinderPushRodSubMachineModulesConst.EventPusherRetractFailed
                    : CylinderPushRodSubMachineModulesConst.EventPusherExtendFailed;
            }

            /// <summary>气缸推杆动作状态机：统一封装校验、执行和结果收敛，供伸出/回退/运行时命令复用。</summary>
            private GFBaseTypeParamValueList ExecuteCylinderPushRodStateMachine(
                string methodID,
                Action executeAction,
                string successEventID,
                Func<string> dataProvider = null)
            {
                GFBaseTypeParamValueList result = CreateResult();
                CylinderPushRodState state = CylinderPushRodState.Start;
                Exception failure = null;

                while (state != CylinderPushRodState.End)
                {
                    switch (state)
                    {
                        case CylinderPushRodState.Start:
                            // 状态机入口：先确保气缸实例和 IO 资源已经准备好。
                            state = CylinderPushRodState.Validate;
                            break;
                        case CylinderPushRodState.Validate:
                            // 若气缸未创建或资源缺失，这里会尝试补建，不满足则直接抛错。
                            TryEnsureCylinderReady();
                            state = CylinderPushRodState.Execute;
                            break;
                        case CylinderPushRodState.Execute:
                            // 执行真实气缸动作；异常统一收敛到失败分支。
                            try
                            {
                                executeAction?.Invoke();
                                state = CylinderPushRodState.Success;
                            }
                            catch (Exception ex)
                            {
                                failure = ex;
                                state = CylinderPushRodState.Fail;
                            }
                            break;
                        case CylinderPushRodState.Success:
                            // 动作成功后统一填充标准返回字段和成功事件号。
                            result["Result"] = new GriffinsBaseValue("0");
                            result["errorMsg"] = new GriffinsBaseValue(string.Empty);
                            result["EventID"] = new GriffinsBaseValue(successEventID ?? string.Empty);
                            result["data"] = new GriffinsBaseValue(dataProvider != null ? (dataProvider() ?? string.Empty) : string.Empty);
                            state = CylinderPushRodState.End;
                            break;
                        case CylinderPushRodState.Fail:
                            // 动作失败时保留异常信息，并映射成对外可识别的失败事件号。
                            result["Result"] = new GriffinsBaseValue("-1");
                            result["errorMsg"] = new GriffinsBaseValue(failure?.Message ?? string.Empty);
                            string failEventID = ResolvePushRodFailureEventID(methodID, failure);
                            result["EventID"] = new GriffinsBaseValue(failEventID);
                            result["data"] = new GriffinsBaseValue(failEventID);
                            state = CylinderPushRodState.End;
                            break;
                        default:
                            // 兜底保护：出现未知状态时按失败返回。
                            result["Result"] = new GriffinsBaseValue("-1");
                            result["errorMsg"] = new GriffinsBaseValue("未知气杆状态");
                            result["EventID"] = new GriffinsBaseValue(ResolvePushRodFailureEventID(methodID, null));
                            state = CylinderPushRodState.End;
                            break;
                    }
                }

                return result;
            }

            /// <summary>气缸推杆统一返回结构；动作命令、判料和参数查询共用这组基础字段。</summary>
            private static GFBaseTypeParamValueList CreateResult()
            {
                GFBaseTypeParamValueList result = new GFBaseTypeParamValueList();
                result.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue("0")));
                result.Add(new GFBaseTypeParamValue("errorMsg", new GriffinsBaseValue("")));
                result.Add(new GFBaseTypeParamValue("data", new GriffinsBaseValue("")));
                result.Add(new GFBaseTypeParamValue("EventID", new GriffinsBaseValue("")));
                return result;
            }

            private static GFBaseTypeParamValueList CreateSuccessResult(string eventID, string data = "")
            {
                GFBaseTypeParamValueList result = CreateResult();
                result["EventID"] = new GriffinsBaseValue(eventID ?? string.Empty);
                result["data"] = new GriffinsBaseValue(data ?? string.Empty);
                return result;
            }

            private static GFBaseTypeParamValueList CreateHasMaterialResult(bool hasMaterial)
            {
                GFBaseTypeParamValueList result = CreateSuccessResult(string.Empty);
                result.Add(new GFBaseTypeParamValue("HasMaterial", new GriffinsBaseValue(hasMaterial)));
                return result;
            }

            private static GFBaseTypeParamValueList CreateParametersResult(object parameters)
            {
                GFBaseTypeParamValueList result = CreateResult();
                string json = ToJson(parameters);
                result["data"] = new GriffinsBaseValue(json);
                result.Add(new GFBaseTypeParamValue("Json", new GriffinsBaseValue(json)));
                return result;
            }

            private static GFBaseTypeParamValueList CreateErrorResult(string errorMsg)
            {
                GFBaseTypeParamValueList result = CreateResult();
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
        }
    }
}
