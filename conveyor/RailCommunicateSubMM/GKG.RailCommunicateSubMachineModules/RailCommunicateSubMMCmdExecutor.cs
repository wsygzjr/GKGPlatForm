using GF_Gereric;
using GKG.ElectronicControl;
using Griffins;
using Griffins.ImeIOT;
using Griffins.PF.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace GKG
{
    namespace SubMM
    {
        public class RailCommunicateSubMMCmdExecutor : ISubMMCmdExecutor, ISubMMManualModeCmdExecutor, ISubMMAutoModeCmdExecutor
        {
            private RailCommunicateSubMachineModulesFactoryCfg railCommunicateSubMachineModulesFactoryCfg;
            private RailCommunicateSubMachineModulesInitCfg railCommunicateSubMachineModulesInitCfg;
            private RailCommunicateSubMachineModulesPPCfg railCommunicateSubMachineModulesPPCfg;
            private ISubMMCmdExecutorCallBack iSubMMCmdExecutorCallBack;
            private readonly SubMMAlias alias;
            private ImeGenNormalEventHandler imeGenNormalEventHandler;
            private ImeCabilityEventHandler imeCabilityEventHandler;
            private ImeAlarmEventHandler imeAlarmEventHandler;

            private IBaseStateIO upperMachineState;
            private IBaseStateIO lowerMachineState;
            private IBaseStateIO machineHasPanel;
            private IBaseStateIO machineNeedPanel;
            private bool isInited = false;
            public RailCommunicateSubMMCmdExecutor(SubMMAlias alias, byte[] factoryCfgInfo)
            {
                this.alias = alias;
                railCommunicateSubMachineModulesFactoryCfg = new RailCommunicateSubMachineModulesFactoryCfg();
                railCommunicateSubMachineModulesFactoryCfg.FromBytes(factoryCfgInfo);
                railCommunicateSubMachineModulesInitCfg = new RailCommunicateSubMachineModulesInitCfg();
                railCommunicateSubMachineModulesPPCfg = new RailCommunicateSubMachineModulesPPCfg();
            }

            event ImeGenNormalEventHandler ISubMMAutoModeCmdExecutor.GenNormalEvent
            {
                add { imeGenNormalEventHandler += value; }
                remove { imeGenNormalEventHandler -= value; }
            }

            event ImeCabilityEventHandler ISubMMAutoModeCmdExecutor.CabilityEvent
            {
                add { imeCabilityEventHandler += value; }
                remove { imeCabilityEventHandler -= value; }
            }

            event ImeAlarmEventHandler ISubMMAutoModeCmdExecutor.AlarmEvent
            {
                add { imeAlarmEventHandler += value; }
                remove { imeAlarmEventHandler -= value; }
            }

            void ISubMMCmdExecutor.BeforeInit(GFBaseTypePropValueList devicePropValues)
            {
            }

            void ISubMMCmdExecutor.Init(byte[] initCfgInfo, byte[] calibrationCfgInfo, ISubMMCmdExecutorCallBack callBack)
            {
                railCommunicateSubMachineModulesFactoryCfg ??= new RailCommunicateSubMachineModulesFactoryCfg();
                railCommunicateSubMachineModulesInitCfg ??= new RailCommunicateSubMachineModulesInitCfg();
                railCommunicateSubMachineModulesPPCfg ??= new RailCommunicateSubMachineModulesPPCfg();
                iSubMMCmdExecutorCallBack = callBack;

                if (initCfgInfo != null && initCfgInfo.Length > 0)
                {
                    railCommunicateSubMachineModulesInitCfg = new RailCommunicateSubMachineModulesInitCfg();
                    railCommunicateSubMachineModulesInitCfg.FromBytes(initCfgInfo);
                }

            }

            void ISubMMCmdExecutor.AfterInit()
            {
                BindStateIOInstancesFromInitCfg();
            }

            void ISubMMCmdExecutor.UnInit()
            {
                upperMachineState = null;
                lowerMachineState = null;
                machineHasPanel = null;
                machineNeedPanel = null;
            }

            ISubMMManualModeCmdExecutor ISubMMCmdExecutor.GetSubMMManualModeCmdExecutor()
            {
                return this;
            }

            ISubMMAutoModeCmdExecutor ISubMMCmdExecutor.GetSubMMAutoModeCmdExecutor()
            {
                return this;
            }

            void ISubMMAutoModeCmdExecutor.SetPFCfgInfo(byte[] pfCfgInfo)
            {
                if (pfCfgInfo != null && pfCfgInfo.Length > 0)
                    railCommunicateSubMachineModulesPPCfg.FromBytes(pfCfgInfo);
            }

            void ISubMMAutoModeCmdExecutor.SetRunMode(ImeRunMode imeRunMode)
            {
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
                GFBaseTypeParamValueList retParams = new GFBaseTypeParamValueList();
                switch (methodID)
                {
                    case RailCommunicateSubMachineModulesConst.InputPanelMethodID:
                        InputPanel();
                        break;
                    case RailCommunicateSubMachineModulesConst.InputPanelSucceededMethodID:
                        InputPanelSucceed();
                        break;
                    case RailCommunicateSubMachineModulesConst.OutputPanelMethodID:
                        OutputPanel();
                        break;
                    case RailCommunicateSubMachineModulesConst.OutputPanelSucceededMethodID:
                        OutputPanelSucceed();
                        break;
                    default:
                        break;
                }
                return retParams;
            }

            Task<GFBaseTypeParamValueList> ISubMMAutoModeCmdExecutor.AsynExecMethod(string methodID, GFBaseTypeParamValueList param)
            {
                return Task.Run(() => new GFBaseTypeParamValueList());
            }

            GFParamValueList ISubMMAutoModeCmdExecutor.ExecMethod(string methodID, GFParamValueList param)
            {
                return new GFParamValueList();
            }

            Task<GFParamValueList> ISubMMAutoModeCmdExecutor.AsynExecMethod(string methodID, GFParamValueList param)
            {
                return Task.Run(() =>
                {
                    Thread.Sleep(10);
                    return new GFParamValueList();
                });
            }

            GFBaseTypeParamValueList ISubMMAutoModeCmdExecutor.ExecCabilityMethod(string methodID, GFBaseTypeParamValueList param)
            {
                return new GFBaseTypeParamValueList();
            }

            Task<GFBaseTypeParamValueList> ISubMMAutoModeCmdExecutor.AsynExecCabilityMethod(string methodID, GFBaseTypeParamValueList param)
            {
                return Task.Run(() => new GFBaseTypeParamValueList());
            }

            GFBaseTypeParamValueList ISubMMAutoModeCmdExecutor.ExecRuntimeCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                return ExecCtlCmdCore(cmdID, cmdParam);
            }

            GFBaseTypeParamValueList ISubMMManualModeCmdExecutor.ExecCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                return ExecCtlCmdCore(cmdID, cmdParam);
            }

            ICompUIDataObjPropValRW ISubMMAutoModeCmdExecutor.GetUIDataObjPropValRW()
            {
                return null;
            }

            bool ISubMMAutoModeCmdExecutor.CanStartWork(out string reasonMsg)
            {
                reasonMsg = string.Empty;
                return true;
            }

            private GFBaseTypeParamValueList ExecCtlCmdCore(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                GFBaseTypeParamValueList retParams = new GFBaseTypeParamValueList();
                switch (cmdID)
                {
                    case RailCommunicateSubMachineModulesConst.SetMachineNeedPanelMethodID:
                        SetMachineNeedPanel(true);
                        break;
                    case RailCommunicateSubMachineModulesConst.SetMachineHasPanelMethodID:
                        SetMachineHasPanel(true);
                        break;
                    case RailCommunicateSubMachineModulesConst.GetUpperMachineStateMethodID:
                        retParams.Add(new GFBaseTypeParamValue("UpperMachineState", new GriffinsBaseValue(GetUpperMachineState())));
                        break;
                    case RailCommunicateSubMachineModulesConst.GetLowerMachineStateMethodID:
                        retParams.Add(new GFBaseTypeParamValue("LowerMachineState", new GriffinsBaseValue(GetLowerMachineState())));
                        break;
                    case RailCommunicateSubMachineModulesConst.GetIOInfosCtlCmdID:
                        {
                            List<IOStateInformation> IOInfos;
                            Dictionary<Guid, IOStateInformation> ioStateInfoDict = new Dictionary<Guid, IOStateInformation>();
                            var ioStateInfosResponse = ServerInnerInfoSender.SendMutualInfo(
                                IOStateInfosRequest.InfoKindID,
                                new IOStateInfosRequest());

                            if (ioStateInfosResponse == null || ioStateInfosResponse.Count == 0)
                                IOInfos = new List<IOStateInformation>();

                            IOStateInfosResponse response = ioStateInfosResponse[0].Response as IOStateInfosResponse;
                            if (response?.IOStateInformations == null)
                                IOInfos = new List<IOStateInformation>();

                            foreach (IOStateInformation ioStateInfo in response.IOStateInformations)
                            {
                                if (ioStateInfo != null && ioStateInfo.IOGuid != Guid.Empty)
                                    ioStateInfoDict[ioStateInfo.IOGuid] = ioStateInfo;
                            }
                            IOInfos = ioStateInfoDict.Values.ToList();
                            retParams["Result"] = new GriffinsBaseValue(JsonObjConvert.ToJSon(IOInfos));
                        }
                        break;
                    default:
                        break;
                }
                return retParams;
            }

            /// <summary>
            /// 绑定IO
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            private void BindStateIOInstancesFromInitCfg()
            {
                if (isInited)
                    return;
                upperMachineState = null;
                lowerMachineState = null;
                machineHasPanel = null;
                machineNeedPanel = null;


                List<IBaseStateIO> ioInstances = GetStateIOInstancesByIds(new List<Guid>
                {
                    railCommunicateSubMachineModulesInitCfg.UpperMachineStateId,
                    railCommunicateSubMachineModulesInitCfg.LowerMachineStateId,
                    railCommunicateSubMachineModulesInitCfg.MachineHasPanelId,
                    railCommunicateSubMachineModulesInitCfg.MachineNeedPanelId,
                });

                if (ioInstances.Count != 4)
                    throw new InvalidOperationException(string.Format(Resources.RailCommunicateIoInstanceCountMismatchFormat, 4, ioInstances.Count));

                upperMachineState = ioInstances[0];
                lowerMachineState = ioInstances[1];
                machineHasPanel = ioInstances[2];
                machineNeedPanel = ioInstances[3];
                isInited = true;
            }

            /// <summary>
            /// 获取IO实例
            /// </summary>
            /// <param name="ioGuids"></param>
            /// <returns></returns>
            /// <exception cref="InvalidOperationException"></exception>
            private static List<IBaseStateIO> GetStateIOInstancesByIds(List<Guid> ioGuids)
            {
                if (ioGuids == null || ioGuids.Count == 0)
                    return new List<IBaseStateIO>();

                var response = ServerInnerInfoSender.SendMutualInfo(
                    StateIOInstancesByIdsRequest.InfoKindID,
                    new StateIOInstancesByIdsRequest(ioGuids));
                if (response == null || response.Count == 0)
                    throw new InvalidOperationException(Resources.RailCommunicateIoRequestFailed);

                StateIOInstancesByIdsResponse ioResponse = response[0].Response as StateIOInstancesByIdsResponse;
                List<IBaseStateIO> ioInstances = ioResponse?.StateIOInstances ?? new List<IBaseStateIO>();
                if (ioInstances.Count != ioGuids.Count)
                    throw new InvalidOperationException(string.Format(Resources.RailCommunicateIoInstanceCountMismatchFormat, ioGuids.Count, ioInstances.Count));

                return ioInstances;
            }

            /// <summary>
            /// 获取上位机有板信号
            /// </summary>
            /// <returns></returns>
            /// <exception cref="InvalidOperationException"></exception>
            private bool GetUpperMachineState()
            {
                if (upperMachineState == null)
                    throw new InvalidOperationException(Resources.RailCommunicateUpperMachineStateIoMissing);
                return upperMachineState.Read();
            }

            /// <summary>
            /// 获取下位机要板信号
            /// </summary>
            /// <returns></returns>
            /// <exception cref="InvalidOperationException"></exception>
            private bool GetLowerMachineState()
            {
                if (lowerMachineState == null)
                    throw new InvalidOperationException(Resources.RailCommunicateLowerMachineStateIoMissing);
                return lowerMachineState.Read();
            }

            /// <summary>
            /// 设置本机要板信号
            /// </summary>
            /// <param name="needPanel"></param>
            /// <exception cref="InvalidOperationException"></exception>
            private void SetMachineNeedPanel(bool needPanel)
            {
                if (machineNeedPanel == null)
                    throw new InvalidOperationException(Resources.RailCommunicateMachineNeedPanelIoMissing);
                machineNeedPanel.Write(needPanel);
            }

            /// <summary>
            /// 设置本机有板信号
            /// </summary>
            /// <param name="hasPanel"></param>
            /// <exception cref="InvalidOperationException"></exception>
            private void SetMachineHasPanel(bool hasPanel)
            {
                if (machineHasPanel == null)
                    throw new InvalidOperationException(Resources.RailCommunicateMachineHasPanelIoMissing);
                machineHasPanel.Write(hasPanel);
            }

            /// <summary>
            /// 进板要板流程
            /// </summary>
            private void InputPanel()
            {
                switch (railCommunicateSubMachineModulesPPCfg.RequestPanelMode)
                {
                    case RequestPanelMode.RequestOnly:
                        SetMachineNeedPanel(true);
                        break;
                    case RequestPanelMode.RequestBeforeHave:
                        {
                            SetMachineNeedPanel(true);
                            while (true)
                            {
                                if (upperMachineState.Read())
                                    break;
                                if (PauseObj.Status == 1)
                                {
                                    SetMachineNeedPanel(false);
                                    return;
                                }

                                Thread.Sleep(10);
                            }
                            RequestPanelSucceed();
                        }
                        break;
                    case RequestPanelMode.HaveBeforeRequest:
                        {
                            while (true)
                            {
                                if (upperMachineState.Read())
                                    break;
                                if (PauseObj.Status == 1)
                                {
                                    return;
                                }
                                Thread.Sleep(10);
                            }
                            SetMachineNeedPanel(true);
                        }
                        break;
                }
            }
            /// <summary>
            /// 进板完成
            /// </summary>
            private void InputPanelSucceed()
            {
                SetMachineNeedPanel(false);
            }

            /// <summary>
            /// 要板成功
            /// </summary>
            private void RequestPanelSucceed()
            {
                // 触发要板成功事件，通知机械模组
                imeGenNormalEventHandler.Invoke(this,
                    new ImeGenNormalEventArgs
                    (
                        RailCommunicateSubMachineModulesConst.RequestPanelEventKind,
                        new GFBaseTypeParamValueList
                         {
                             new GFBaseTypeParamValue
                             {
                                  ID = RailCommunicateSubMachineModulesConst.RequestPanelEventID,
                                  Value = new GriffinsBaseValue(true)
                             }
                         }
                    ));
            }

            /// <summary>
            /// 出板流程
            /// </summary>
            private void OutputPanel()
            {
                SetMachineHasPanel(true);
                while (true)
                {
                    if (lowerMachineState.Read())
                        break;
                    if (PauseObj.Status == 1)
                    {
                        SetMachineHasPanel(true);
                        return;
                    }
                    Thread.Sleep(10);
                }
                ResponsePanelSucceed();
            }

            /// <summary>
            /// 出板成功
            /// </summary>
            private void OutputPanelSucceed()
            {
                SetMachineHasPanel(false);
            }

            /// <summary>
            /// 出板反馈
            /// </summary>
            private void ResponsePanelSucceed()
            {
                // 触发有板成功事件，通知上位机
                imeGenNormalEventHandler.Invoke(this,
                    new ImeGenNormalEventArgs
                    (
                        RailCommunicateSubMachineModulesConst.ResponsePanelEventKind,
                        new GFBaseTypeParamValueList
                         {
                             new GFBaseTypeParamValue
                             {
                                  ID = RailCommunicateSubMachineModulesConst.ResponsePanelEventID,
                                  Value = new GriffinsBaseValue(true)
                             }
                         }
                    ));
            }
        }
    }
}
