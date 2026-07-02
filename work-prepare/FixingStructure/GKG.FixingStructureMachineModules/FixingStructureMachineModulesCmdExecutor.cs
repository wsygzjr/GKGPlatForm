using GKG.MM;
using GKG.SubMM;
using Griffins;
using Griffins.ImeIOT;
using System;

namespace GKG
{
    namespace MM
    {
        public class FixingStructureMachineModulesCmdExecutor : IMMCmdExecutor, IMMManualModeCmdExecutor, IMMAutoModeCmdExecutor, ICompUIDataObjPropValRW
        {

            private FixingStructureMachineModulesFactoryCfg factoryCfg;
            private FixingStructureMachineModulesInitCfg initCfg;
            private FixingStructureMachineModulesPPCfg pPCfg;

            private IMMCmdExecutorCallBack _callBack;

            private MMAlias alias;

            private ImeGenNormalEventHandler imeGenNormalEventHandler;

            private ImeCabilityEventHandler imeCabilityEventHandler;

            private ImeAlarmEventHandler imeAlarmEventHandler;

            private ImePropValChangedEventHandler uIDataObjPropValChangedEvent;

            private bool isFixing = false;
            public FixingStructureMachineModulesCmdExecutor(MMAlias alias, byte[] factoryCfgInfo)
            {
                this.alias = alias;
                factoryCfg = new FixingStructureMachineModulesFactoryCfg();
                factoryCfg.FromBytes(factoryCfgInfo);
                initCfg = new FixingStructureMachineModulesInitCfg();
                pPCfg = new FixingStructureMachineModulesPPCfg();
            }

            event ImeGenNormalEventHandler IMMAutoModeCmdExecutor.GenNormalEvent
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

            event ImeCabilityEventHandler IMMAutoModeCmdExecutor.CabilityEvent
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

            event ImeAlarmEventHandler IMMAutoModeCmdExecutor.AlarmEvent
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

            event ImePropValChangedEventHandler ICompUIDataObjPropValRW.UIDataObjPropValChangedEvent
            {
                add
                {
                    uIDataObjPropValChangedEvent += value;
                }

                remove
                {
                    uIDataObjPropValChangedEvent -= value;
                }
            }
            /// <summary>
            /// 运行模式：ConfigMode-配置模式，AutoMode-自动模式，ManualMode-手动模式
            /// </summary>
            private ImeRunMode runMode = ImeRunMode.ConfigMode;
            void IMMCmdExecutor.BeforeInit()
            {

            }

            void IMMCmdExecutor.Init(byte[] initCfgInfo, byte[] calibrationCfgInfo, IMMCmdExecutorCallBack callBack)
            {
                _callBack = callBack;
                initCfg.FromBytes(initCfgInfo);
            }

            void IMMCmdExecutor.AfterInit()
            {

            }

            void IMMCmdExecutor.UnInit()
            {

            }

            IMMManualModeCmdExecutor IMMCmdExecutor.GetMMManualModeCmdExecutor()
            {
                return this;
            }

            IMMAutoModeCmdExecutor IMMCmdExecutor.GetMMAutoModeCmdExecutor()
            {
                return this;
            }

            void IMMAutoModeCmdExecutor.SetPFCfgInfo(byte[] pfCfgInfo)
            {

            }

            bool IMMAutoModeCmdExecutor.CanStartWork(out string reasonMsg)
            {
                reasonMsg = string.Empty;
                return true;
            }

            void IMMAutoModeCmdExecutor.SetRunMode(ImeRunMode imeRunMode)
            {
                runMode = imeRunMode;
            }

            void IMMAutoModeCmdExecutor.StartWork()
            {
                PauseObj.Status = 2;
            }

            void IMMAutoModeCmdExecutor.StopWork()
            {
                PauseObj.Status = 3;
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
                GFBaseTypeParamValueList retParams = new GFBaseTypeParamValueList();
                switch (methodID)
                {
                    case FixingStructureMachineModulesConst.FixingMethodID:
                        {
                            Fixing();
                        }
                        break;
                    case FixingStructureMachineModulesConst.ReleaseFixingMethodID:
                        {
                            ReleaseFixing();
                        }
                        break;
                    default:
                        break;
                }
                return retParams;
            }

            GFParamValueList IMMAutoModeCmdExecutor.ExecMethod(string methodID, GFParamValueList param)
            {
                return new GFParamValueList();
            }

            GFBaseTypeParamValueList IMMAutoModeCmdExecutor.ExecCabilityMethod(string methodID, GFBaseTypeParamValueList param)
            {
                GFBaseTypeParamValueList rst = new GFBaseTypeParamValueList();
                switch (methodID)
                {
                    case FixingStructureMachineModulesConst.FixingMethodID:
                        {
                            Fixing();
                        }
                        break;
                    case FixingStructureMachineModulesConst.ReleaseFixingMethodID:
                        {
                            ReleaseFixing();
                        }
                        break;
                    default:
                        break;
                }
                return rst;
            }

            GFBaseTypeParamValueList IMMAutoModeCmdExecutor.ExecRuntimeCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                return ExecCtlCmdCore(cmdID, cmdParam);
            }

            /// <summary>
            /// 获取界面数据对象属性读写接口实例，如果不支持返回nul
            /// </summary>
            /// <returns>界面数据对象属性读写接口实例</returns>
            #region 界面数据对象
            ICompUIDataObjPropValRW IMMAutoModeCmdExecutor.GetUIDataObjPropValRW()
            {
                return this;
            }

            void ICompUIDataObjPropValRW.SetUIDataObjPropPathValue(ObjInstPropPath objInstPropPath, GriffinsBaseValue value)
            {

            }

            void ICompUIDataObjPropValRW.SetUIDataObjPropPathValues(GFBaseTypeObjPropPathValueList values)
            {

            }

            GriffinsBaseValue ICompUIDataObjPropValRW.GetUIDataObjPropPathValue(ObjInstPropPath objInstPropPath)
            {
                return new GriffinsBaseValue();
            }

            GFBaseTypeObjPropPathValueList ICompUIDataObjPropValRW.GetUIDataObjPropPathValues(ObjInstPropPath[] objInstPropPaths)
            {
                return new GFBaseTypeObjPropPathValueList();
            }

            GFBaseTypeObjPropPathValueList ICompUIDataObjPropValRW.GetAllUIDataObjPropPathValues()
            {
                return new GFBaseTypeObjPropPathValueList();
            }

            GFBaseTypeParamValueList ICompUIDataObjPropValRW.ExecUIDataObjCommand(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                return ExecCtlCmdCore(cmdID, cmdParam);
            }
            #endregion
            void IMMAutoModeCmdExecutor.ExecSubMMEvent(InnerAlias innerAlias, int eventID, GFBaseTypeParamValueList eventParam)
            {

            }

            void IMMAutoModeCmdExecutor.ExecSubMMCabilityEvent(InnerAlias innerAlias, string eventID, GFBaseTypeParamValueList eventParam)
            {
                if(innerAlias == FixingStructureMachineModulesConst.InnerAliasFixing)
                {
                    switch (eventID)
                    {
                        case FixingStructureSubMachineModulesConst.FixingFinishedEventID:
                            {
                                isFixing = true;
                                //uIDataObjPropValChangedEvent.Invoke(this, new ImePropValChangedEventArgs(new MPPropertyID(FixingStructureMachineModulesConst.FixingStatePropertyID), new GriffinsBaseValue(isFixing), DateTime.Now));
                                imeCabilityEventHandler.Invoke(this, new ImeCabilityEventArgs(FixingStructureMachineModulesConst.FixingFinishedEventID, eventParam));
                            }
                            break;
                        case FixingStructureSubMachineModulesConst.FixingFailedEventID:
                            {
                                imeCabilityEventHandler?.Invoke(this, new ImeCabilityEventArgs(FixingStructureMachineModulesConst.FixingFailedEventID, eventParam));
                            }
                            break;
                        case FixingStructureSubMachineModulesConst.ReleaseFixingFinishedEventID:
                            {
                                isFixing = false;
                                //uIDataObjPropValChangedEvent.Invoke(this, new ImePropValChangedEventArgs(new MPPropertyID(FixingStructureMachineModulesConst.FixingStatePropertyID), new GriffinsBaseValue(isFixing), DateTime.Now));
                                imeCabilityEventHandler.Invoke(this, new ImeCabilityEventArgs(FixingStructureMachineModulesConst.ReleaseFixingFinishedEventID, eventParam));
                            }
                            break;
                        case FixingStructureSubMachineModulesConst.ReleaseFixingFailedEventID:
                            {
                                imeCabilityEventHandler?.Invoke(this, new ImeCabilityEventArgs(FixingStructureMachineModulesConst.ReleaseFixingFailedEventID, eventParam));
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            void IMMAutoModeCmdExecutor.ReturnToOriginal()
            {

            }

            GFBaseTypeParamValueList IMMManualModeCmdExecutor.ExecCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                return ExecCtlCmdCore(cmdID, cmdParam);
            }

            #region 机械模组普通方法
            GFBaseTypeParamValueList ExecCtlCmdCore(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                GFBaseTypeParamValueList rst = new GFBaseTypeParamValueList();
                switch (cmdID)
                {
                    case FixingStructureMachineModulesConst.FixingMethodID:
                        {
                            Fixing();
                        }
                        break;
                    case FixingStructureMachineModulesConst.ReleaseFixingMethodID:
                        {
                            ReleaseFixing();
                        }
                        break;
                    case FixingStructureMachineModulesConst.JackingCmdID:
                        {
                            if (isFixing)
                            {
                                ReleaseFixing();
                            }
                            else
                            {
                                Fixing();
                            }
                        }
                        break;
                    default:
                        break;
                }
                return rst;
            }


            private void Fixing()
            {
                _callBack.ExecSubMMCabilityMethod(FixingStructureMachineModulesConst.InnerAliasFixing, FixingStructureSubMachineModulesConst.FixingMethodID, new GFBaseTypeParamValueList());
            }

            private void ReleaseFixing()
            {
                _callBack.ExecSubMMCabilityMethod(FixingStructureMachineModulesConst.InnerAliasFixing, FixingStructureSubMachineModulesConst.ReleaseFixingMethodID, new GFBaseTypeParamValueList());

            }

            #endregion
        }
    }
}
