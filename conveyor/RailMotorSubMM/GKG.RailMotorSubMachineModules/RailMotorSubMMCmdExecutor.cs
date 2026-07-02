using GF_Gereric;
using GKG.ElectronicControl;
using GKG.ElectronicControl.General;
using GKG.MotionControl;
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
        public class RailMotorSubMMCmdExecutor : ISubMMCmdExecutor, ISubMMManualModeCmdExecutor, ISubMMAutoModeCmdExecutor
        {
            private RailMotorSubMachineModulesFactoryCfg railMotorSubMachineModulesFactoryCfg;

            private RailMotorSubMachineModulesInitCfg railMotorSubMachineModulesInitCfg;

            private RailMotorSubMachineModulesPPCfg railMotorSubMachineModulesPPCfg;

            private ISubMMCmdExecutorCallBack iSubMMCmdExecutorCallBack;

            private readonly SubMMAlias alias;

            private ImeGenNormalEventHandler imeGenNormalEventHandler;

            private ImeCabilityEventHandler imeCabilityEventHandler;

            private ImeAlarmEventHandler imeAlarmEventHandler;
            private IRobotDriver basicRobot;

            public RailMotorSubMMCmdExecutor(SubMMAlias alias, byte[] factoryCfgInfo)
            {
                this.alias = alias;
                railMotorSubMachineModulesFactoryCfg = new RailMotorSubMachineModulesFactoryCfg();
                railMotorSubMachineModulesFactoryCfg.FromBytes(factoryCfgInfo);
                railMotorSubMachineModulesInitCfg = new RailMotorSubMachineModulesInitCfg();
                railMotorSubMachineModulesPPCfg = new RailMotorSubMachineModulesPPCfg();
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


            void ISubMMCmdExecutor.BeforeInit(GFBaseTypePropValueList devicePropValues)
            {
            }

            void ISubMMCmdExecutor.Init(byte[] initCfgInfo, byte[] calibrationCfgInfo, ISubMMCmdExecutorCallBack callBack)
            {
                railMotorSubMachineModulesFactoryCfg ??= new RailMotorSubMachineModulesFactoryCfg();
                railMotorSubMachineModulesInitCfg ??= new RailMotorSubMachineModulesInitCfg();
                railMotorSubMachineModulesPPCfg ??= new RailMotorSubMachineModulesPPCfg();
                railMotorSubMachineModulesInitCfg.AxisBindingObjID = Guid.Parse("025ac219-2a5d-4208-90c8-484d9fb527a8");
                iSubMMCmdExecutorCallBack = callBack;
                if (initCfgInfo != null && initCfgInfo.Length > 0)
                {
                    railMotorSubMachineModulesInitCfg = new RailMotorSubMachineModulesInitCfg();
                    railMotorSubMachineModulesInitCfg.FromBytes(initCfgInfo);
                    railMotorSubMachineModulesInitCfg.AxisBindingObjID = Guid.Parse("025ac219-2a5d-4208-90c8-484d9fb527a8");
                }

            }

            void ISubMMCmdExecutor.AfterInit()
            {
                EnsureBasicRobotInitialized();
            }

            void ISubMMCmdExecutor.UnInit()
            {
                basicRobot = null;
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
                    railMotorSubMachineModulesPPCfg.FromBytes(pfCfgInfo);
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
                    case RailMotorSubMachineModulesConst.ContinueMoveMethodID:
                        {
                            double acc = (double)param["Acceleration"].ToDecimal();
                            double speed = (double)param["Speed"].ToDecimal();
                            bool direction = param["Direction"].ToBool();
                            ContinueMove(new ContinueMoveParameters
                            {
                                Acceleration = acc,
                                Speed = speed,
                                Direction = direction
                            });
                            break;
                        }
                    case RailMotorSubMachineModulesConst.RelativeMoveMethodID:
                        {
                            double relativeDistance = (double)param["RelativeDistance"].ToDecimal();
                            double acc = (double)param["Acceleration"].ToDecimal();
                            double speed = (double)param["Speed"].ToDecimal();
                            bool direction = param["Direction"].ToBool();
                            RelativeMove(new RelativeMoveParameters
                            {
                                RelativeDistance = relativeDistance,
                                Speed = speed,
                                Acceleration = acc,
                                Direction = direction
                            });
                            break;
                        }
                    case RailMotorSubMachineModulesConst.StopMoveMethodID:
                        {
                            StopMove();
                            break;
                        }
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

            bool ISubMMAutoModeCmdExecutor.CanStartWork(out string reasonMsg)
            {
                reasonMsg = string.Empty;
                return true;
            }
            ICompUIDataObjPropValRW ISubMMAutoModeCmdExecutor.GetUIDataObjPropValRW()
            {
                return null;
            }
            private GFBaseTypeParamValueList ExecCtlCmdCore(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                GFBaseTypeParamValueList rst = new GFBaseTypeParamValueList();
                switch (cmdID)
                {
                    case RailMotorSubMachineModulesConst.RtCmdGetAxisOptions:
                        {
                            List<AxisInformation> axisInformation;
                            Dictionary<Guid, AxisInformation> axisInfoDict = new Dictionary<Guid, AxisInformation>();
                            var axisInfosResponse = ServerInnerInfoSender.SendMutualInfo(
                                AxisInfosRequest.InfoKindID,
                                new AxisInfosRequest(MotionControlCardType.Normal));

                            if (axisInfosResponse == null || axisInfosResponse.Count == 0)
                                axisInformation = new List<AxisInformation>();

                            AxisInfosResponse response = axisInfosResponse[0].Response as AxisInfosResponse;
                            if (response?.AxisInformations == null)
                                axisInformation = new List<AxisInformation>();

                            foreach (AxisInformation axisInfo in response.AxisInformations)
                            {
                                if (axisInfo != null && axisInfo.AxisGuid != Guid.Empty)
                                    axisInfoDict[axisInfo.AxisGuid] = axisInfo;
                            }
                            axisInformation = axisInfoDict.Values.ToList();
                            rst.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue(JsonObjConvert.ToJSon(axisInformation))));
                        }
                        break;
                    default:
                        break;
                }
                return rst;
            }

            #region 子机械模组普通方法

            private int GetLogicalAxis()
            {
                return 0;
            }

            private void EnsureBasicRobotInitialized()
            {
                if (basicRobot != null)
                    return;

                if (railMotorSubMachineModulesInitCfg.AxisBindingObjID == Guid.Empty)
                    throw new InvalidOperationException(Resources.RailMotorAxisBindingMissing);

                RobotDriverByAxisIdsRequest request = new RobotDriverByAxisIdsRequest(new List<Guid> { railMotorSubMachineModulesInitCfg.AxisBindingObjID })
                {
                    MotionCardType = MotionControlCardType.Normal
                };

                var robotDriverResponses = ServerInnerInfoSender.SendMutualInfo(RobotDriverByAxisIdsRequest.InfoKindID, request);
                if (robotDriverResponses == null || robotDriverResponses.Count == 0)
                    throw new InvalidOperationException(Resources.RailMotorRobotDriverResponseMissing);

                RobotDriverByAxisIdsResponse robotDriverResponse = robotDriverResponses[0].Response as RobotDriverByAxisIdsResponse;
                if (robotDriverResponse?.RobotDriver == null)
                    throw new InvalidOperationException(Resources.RailMotorRobotDriverUnavailable);

                basicRobot = robotDriverResponse.RobotDriver;
            }

            private void ContinueMove(ContinueMoveParameters moveParameters)
            {
                EnsureBasicRobotInitialized();

                basicRobot.Execute(new MotionInstructionSequence()
                {
                    SequenceType = MotionInstructionSequenceType.StepByStep,
                    Instructions = new MotionInstructionBase[]
                    {
                        new ContinueMoveInstruction
                        {
                            Speed = moveParameters.Speed,
                            Acceleration = moveParameters.Acceleration,
                            LogicAxis = GetLogicalAxis()
                        }
                    },
                    ExtendedParameters = null
                }, new RobotExecutionContext());
            }

            private void RelativeMove(RelativeMoveParameters moveParameters)
            {
                EnsureBasicRobotInitialized();

                double distance = moveParameters.Direction ? moveParameters.RelativeDistance : -moveParameters.RelativeDistance;

                basicRobot.Execute(new MotionInstructionSequence()
                {
                    SequenceType = MotionInstructionSequenceType.StepByStep,
                    Instructions = new MotionInstructionBase[]
                    {
                        new RelativeMoveInstruction() 
                        { 
                            Speed = moveParameters.Speed,
                            Acceleration = moveParameters.Acceleration,
                            Distance = new AxisConstantValues
                            {
                                Axis = GetLogicalAxis(),
                                PositionValue = distance
                            }
                        }
                    },
                    ExtendedParameters = null
                }, new RobotExecutionContext());
            }

            private void StopMove()
            {
                if (basicRobot == null)
                    return;

                basicRobot.Execute(new MotionInstructionSequence()
                {
                    SequenceType = MotionInstructionSequenceType.StepByStep,
                    Instructions = new MotionInstructionBase[]
                    {
                        new StopMoveInstruction
                        {
                            LogicAxis = GetLogicalAxis()
                        }
                    },
                    ExtendedParameters = null
                }, new RobotExecutionContext());
            }

            #endregion
        }
    }
}
