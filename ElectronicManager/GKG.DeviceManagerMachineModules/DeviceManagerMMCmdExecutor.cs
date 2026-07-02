using GF_Gereric;
using GKG.SubMM;
using Griffins;
using Griffins.ImeIOT;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GKG
{
    namespace MM
    {
        public class DeviceManagerMMCmdExecutor : IMMCmdExecutor, IMMManualModeCmdExecutor, IMMAutoModeCmdExecutor
        {
            private readonly DeviceManagerMachineModulesFactoryCfg factoryCfg;
            private readonly DeviceManagerMachineModulesInitCfg initCfg;
            private readonly DeviceManagerMachineModulesPPCfg ppCfg;
            private readonly MMAlias alias;

            private ImeGenNormalEventHandler imeGenNormalEventHandler;
            private ImeCabilityEventHandler imeCabilityEventHandler;
            private ImeAlarmEventHandler imeAlarmEventHandler;

            private int upfeedContinuousEmptyPushCounter;
            private int downfeedContinuousEmptyPushCounter;

            internal IMMCmdExecutorCallBack _callBack;
            private DeviceManagerMMUIDataObj deviceManagerMMUIDataObj;
            public DeviceManagerMMCmdExecutor(MMAlias alias, byte[] factoryCfgInfo)
            {
                this.alias = alias;
                factoryCfg = new DeviceManagerMachineModulesFactoryCfg();
                initCfg = new DeviceManagerMachineModulesInitCfg();
                ppCfg = new DeviceManagerMachineModulesPPCfg();

                if (factoryCfgInfo != null && factoryCfgInfo.Length > 0)
                {
                    factoryCfg.FromBytes(factoryCfgInfo);
                }
            }

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

            void IMMCmdExecutor.BeforeInit()
            {
            }
            private Task refreshTask;
            private bool exit = false;
            /// <summary>初始化总模组，并在内部创建料盒、电机推杆、气缸推杆三个子模组实例。</summary>
            void IMMCmdExecutor.Init(byte[] initCfgInfo, byte[] calibrationCfgInfo, IMMCmdExecutorCallBack callBack)
            {
                _callBack = callBack;
                if (deviceManagerMMUIDataObj == null)
                    deviceManagerMMUIDataObj = new DeviceManagerMMUIDataObj(this);
                //if (refreshTask == null)
                //{
                //    refreshTask = Task.Run(() =>
                //    {
                //        while (true)
                //        {
                //            if (exit)
                //                break;
                //            deviceManagerMMUIDataObj.UpdateUIDataObjProps();
                //            Thread.Sleep(100);
                //        }
                //    });
                //}
                if (initCfgInfo != null && initCfgInfo.Length > 0)
                {
                    initCfg.FromBytes(initCfgInfo);
                }
            }

            void IMMCmdExecutor.AfterInit()
            {
                
            }

            void IMMCmdExecutor.UnInit()
            {
                exit = true;
                refreshTask.Wait(2000);
            }

            IMMManualModeCmdExecutor IMMCmdExecutor.GetMMManualModeCmdExecutor()
            {
                return this;
            }

            IMMAutoModeCmdExecutor IMMCmdExecutor.GetMMAutoModeCmdExecutor()
            {
                return this;
            }

            bool IMMAutoModeCmdExecutor.CanStartWork(out string reasonMsg)
            {
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

            void IMMAutoModeCmdExecutor.StartWork()
            {
                PauseObj.Status = 2;
            }

            void IMMAutoModeCmdExecutor.StopWork()
            {
                PauseObj.Status = 1;
            }

            void IMMAutoModeCmdExecutor.Pause()
            {
                PauseObj.Status = 1;
            }

            void IMMAutoModeCmdExecutor.Resume()
            {
                PauseObj.Status = 2;
            }

            void IMMAutoModeCmdExecutor.BeforeSwitchPF()
            {
            }

            void IMMAutoModeCmdExecutor.AfterStopWork()
            {
            }

            GFBaseTypeParamValueList IMMAutoModeCmdExecutor.ExecMethod(string methodID, GFBaseTypeParamValueList param)
            {
                switch (methodID)
                {
                    default:
                        return CreateErrorResult($"未识别的方法: {methodID ?? "<null>"}");
                }
            }

            GFParamValueList IMMAutoModeCmdExecutor.ExecMethod(string methodID, GFParamValueList param)
            {
                return new GFParamValueList();
            }

            GFBaseTypeParamValueList IMMAutoModeCmdExecutor.ExecCabilityMethod(string methodID, GFBaseTypeParamValueList param)
            {
                switch (methodID)
                {
                    default:
                        return CreateErrorResult($"未识别的方法: {methodID ?? "<null>"}");
                }
            }

            /// <summary>运行时控制命令路由表：总模块只负责转发，具体动作由对应子模组执行。</summary>
            GFBaseTypeParamValueList IMMAutoModeCmdExecutor.ExecRuntimeCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                return ExecRuntimeCtlCmdCore(cmdID, cmdParam);
            }
            internal GFBaseTypeParamValueList ExecRuntimeCtlCmdCore(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                switch (cmdID)
                {
                    case DeviceManagerMachineModulesConst.SetExecMode:
                    case SvrForMMProcessCmd.CmdID_StartWork:
                    case SvrForMMProcessCmd.CmdID_StopWork:
                    case DeviceManagerMachineModulesConst.NextStep:
                        return ((ICompUIDataObjPropValRW)deviceManagerMMUIDataObj).ExecUIDataObjCommand(cmdID, cmdParam);
                    default:
                        return CreateErrorResult($"未识别的运行时控制命令: {cmdID ?? "<null>"}");
                }
            }

            ICompUIDataObjPropValRW IMMAutoModeCmdExecutor.GetUIDataObjPropValRW()
            {
                if (deviceManagerMMUIDataObj == null)
                    deviceManagerMMUIDataObj = new DeviceManagerMMUIDataObj(this);
                return deviceManagerMMUIDataObj;
            }

            void IMMAutoModeCmdExecutor.ExecSubMMEvent(InnerAlias innerAlias, int eventID, GFBaseTypeParamValueList eventParam)
            {
            }

            void IMMAutoModeCmdExecutor.ExecSubMMCabilityEvent(InnerAlias innerAlias, string eventID, GFBaseTypeParamValueList eventParam)
            {
            }

            void IMMAutoModeCmdExecutor.ReturnToOriginal()
            {

            }

            GFBaseTypeParamValueList IMMManualModeCmdExecutor.ExecCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                return ExecRuntimeCtlCmdCore(cmdID, cmdParam);
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
        }
    }
}
