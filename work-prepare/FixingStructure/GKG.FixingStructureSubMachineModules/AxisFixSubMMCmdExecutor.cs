using FixingStructureSubMachineModules;
using GF_Gereric;
using GKG.ElectronicControl;
using GKG.ElectronicControl;
using GKG.ElectronicControl.General;
using GKG.MotionControl;
using Griffins;
using Griffins.ImeIOT;
using Griffins.PF.Server;
using System.Transactions;

namespace GKG.SubMM
{
    public class AxisFixSubMMCmdExecutor : ISubMMCmdExecutor, ISubMMManualModeCmdExecutor, ISubMMAutoModeCmdExecutor
    {
        private AxisFixSubMachineModulesFactoryCfg factoryCfg;
        private AxisFixSubMachineModulesInitCfg initCfg;
        private AxisFixSubMachineModulesPPCfg pPCfg;
        private ISubMMCmdExecutorCallBack? iSubMMCmdExecutorCallBack;
        private readonly SubMMAlias alias;
        private ImeGenNormalEventHandler? imeGenNormalEventHandler;
        private ImeCabilityEventHandler? imeCabilityEventHandler;
        private ImeAlarmEventHandler? imeAlarmEventHandler;
        private IRobotDriver? robotDriver;
        private bool isInitialized;
        private bool isFixed;
        private double currentPosition;

        public AxisFixSubMMCmdExecutor(SubMMAlias alias, byte[] factoryCfgInfo)
        {
            this.alias = alias;
            factoryCfg = new AxisFixSubMachineModulesFactoryCfg();
            factoryCfg.FromBytes(factoryCfgInfo);
            initCfg = new AxisFixSubMachineModulesInitCfg();
            pPCfg = new AxisFixSubMachineModulesPPCfg();
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
            initCfg ??= new AxisFixSubMachineModulesInitCfg();
            iSubMMCmdExecutorCallBack = callBack;

            if (initCfgInfo != null && initCfgInfo.Length > 0)
                initCfg.FromBytes(initCfgInfo);

        }
        void ISubMMCmdExecutor.AfterInit()
        {
            EnsureFixingCylinderInitialized();
        }

        void ISubMMCmdExecutor.UnInit()
        {
            isInitialized = false;
            if(robotDriver!=null)
            {
                robotDriver.PositionChanged -= RobotDriver_PositionChanged;
                robotDriver.MoveFinished -= RobotDriver_MoveFinished;
                robotDriver.MoveFailed -= RobotDriver_MoveFailed;
                robotDriver = null;
            }
            isFixed = false;
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
                pPCfg.FromBytes(pfCfgInfo);
            //pPCfg.FixingPosition = 5;
            //pPCfg.ReleaseFixingPosition = 0;
            //pPCfg.trajectoryParameters = new NonProcessingTrajectoryParameters
            //{
            //    Acceleration = 10,
            //    Deceleration = 10,
            //    MaxSpeed = 5,
            //    StartSpeed = 0
            //};
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
            return ExecMethodCore(methodID, param);
        }

        Task<GFBaseTypeParamValueList> ISubMMAutoModeCmdExecutor.AsynExecMethod(string methodID, GFBaseTypeParamValueList param)
        {
            return Task.Run(() => ExecMethodCore(methodID, param));
        }

        GFParamValueList ISubMMAutoModeCmdExecutor.ExecMethod(string methodID, GFParamValueList param)
        {
            return new GFParamValueList();
        }

        Task<GFParamValueList> ISubMMAutoModeCmdExecutor.AsynExecMethod(string methodID, GFParamValueList param)
        {
            return Task.Run(() => new GFParamValueList());
        }

        GFBaseTypeParamValueList ISubMMAutoModeCmdExecutor.ExecCabilityMethod(string methodID, GFBaseTypeParamValueList param)
        {
            return ExecMethodCore(methodID, param);
        }

        Task<GFBaseTypeParamValueList> ISubMMAutoModeCmdExecutor.AsynExecCabilityMethod(string methodID, GFBaseTypeParamValueList param)
        {
            return Task.Run(() => ExecMethodCore(methodID, param));
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
            return null!;
        }

        bool ISubMMAutoModeCmdExecutor.CanStartWork(out string reasonMsg)
        {
            reasonMsg = string.Empty;
            return true;
        }

        private GFBaseTypeParamValueList ExecMethodCore(string methodID, GFBaseTypeParamValueList param)
        {
            GFBaseTypeParamValueList rst = new GFBaseTypeParamValueList();
            switch (methodID)
            {
                case FixingStructureSubMachineModulesConst.FixingMethodID:
                    Fixing();
                    break;
                case FixingStructureSubMachineModulesConst.ReleaseFixingMethodID:
                    ReleaseFixing();
                    break;
                default:
                    break;
            }

            return rst;
        }

        private GFBaseTypeParamValueList ExecCtlCmdCore(string cmdID, GFBaseTypeParamValueList cmdParam)
        {
            GFBaseTypeParamValueList rst = new GFBaseTypeParamValueList();
            switch (cmdID)
            {
                case FixingStructureSubMachineModulesConst.RtCmdGetFixingState:
                    {
                        rst.Add(new GFBaseTypeParamValue(FixingStructureSubMachineModulesConst.FixingState, new GriffinsBaseValue(isFixed)));
                    }
                    break;
                case FixingStructureSubMachineModulesConst.RtCmdGetIOOptions:
                    {
                        var ioStateInfosResponse = ServerInnerInfoSender.SendMutualInfo(
                            IOStateInfosRequest.InfoKindID,
                            new IOStateInfosRequest());

                        if (ioStateInfosResponse == null || ioStateInfosResponse.Count == 0)
                        {
                            rst.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue(JsonObjConvert.ToJSon(new List<IOStateInformation>()))));
                            break;
                        }

                        IOStateInfosResponse? response = ioStateInfosResponse[0].Response as IOStateInfosResponse;
                        if (response?.IOStateInformations == null)
                        {
                            rst.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue(JsonObjConvert.ToJSon(new List<IOStateInformation>()))));
                            break;
                        }

                        rst.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue(JsonObjConvert.ToJSon(response?.IOStateInformations))));
                    }
                    break;
                case FixingStructureSubMachineModulesConst.RtCmdGetAxisOptions:
                    {
                        var axisInfosResponse = ServerInnerInfoSender.SendMutualInfo(
                            AxisInfosRequest.InfoKindID,
                            new AxisInfosRequest());

                        if (axisInfosResponse == null || axisInfosResponse.Count == 0)
                        {
                            rst.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue(JsonObjConvert.ToJSon(new List<IOStateInformation>()))));
                            break;
                        }

                        AxisInfosResponse? response = axisInfosResponse[0].Response as AxisInfosResponse;
                        if (response?.AxisInformations == null)
                        {
                            rst.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue(JsonObjConvert.ToJSon(new List<IOStateInformation>()))));
                            break;
                        }

                        rst.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue(JsonObjConvert.ToJSon(response?.AxisInformations))));
                    }
                    break;
                case FixingStructureSubMachineModulesConst.FixingMethodID:
                    Fixing();
                    break;
                case FixingStructureSubMachineModulesConst.ReleaseFixingMethodID:
                    ReleaseFixing();
                    break;
                case FixingStructureSubMachineModulesConst.RtCmdMoveTo:
                    {
                        double speed = (double)cmdParam["Speed"].ToDecimal();
                        double acc = (double)cmdParam["Acc"].ToDecimal();
                        double position = (double)cmdParam["Position"].ToDecimal();
                        RobotMoveTo(speed, acc, position);
                    }
                    break;
                case FixingStructureSubMachineModulesConst.RtCmdContinueMove:
                    {
                        double speed = (double)cmdParam["Speed"].ToDecimal();
                        double acc = (double)cmdParam["Acc"].ToDecimal();
                        bool direction = cmdParam["Direction"].ToBool();
                        RobotContinueMove(speed, acc, direction);
                    }
                    break;
                case FixingStructureSubMachineModulesConst.RtCmdStop:
                    {
                        RobotStop();
                    }
                    break;
                default:
                    break;
            }

            return rst;
        }

        private void EnsureFixingCylinderInitialized()
        {
            if (isInitialized)
                return;
            robotDriver = ElectronicFunc.CreateRobotDriver(
                /*Guid.Parse("{b1aefc53-5bb5-4659-bf2b-0d54bd532977}"),*/initCfg.AxisBindingObjID,
                "固定气缸未绑定轴对象。",
                "获取固定气缸驱动失败。",
                "固定气缸驱动不可用。");
            robotDriver.PositionChanged += RobotDriver_PositionChanged;
            robotDriver.MoveFinished += RobotDriver_MoveFinished;
            robotDriver.MoveFailed += RobotDriver_MoveFailed;
            isInitialized = true;
        }

        private void RobotDriver_MoveFailed(object? sender, EventArgs e)
        {
            if (isFixed)
            {
                imeCabilityEventHandler.Invoke(this, new ImeCabilityEventArgs(FixingStructureSubMachineModulesConst.FixingFailedEventID, new GFBaseTypeParamValueList()));
            }
            else
            {
                imeCabilityEventHandler.Invoke(this, new ImeCabilityEventArgs(FixingStructureSubMachineModulesConst.ReleaseFixingFailedEventID, new GFBaseTypeParamValueList()));
            }
        }

        private void RobotDriver_MoveFinished(object? sender, EventArgs e)
        {
            if(isFixed)
            {
                imeCabilityEventHandler.Invoke(this,new ImeCabilityEventArgs(FixingStructureSubMachineModulesConst.FixingFinishedEventID,new GFBaseTypeParamValueList()));
            }
            else
            {
                imeCabilityEventHandler.Invoke(this,new ImeCabilityEventArgs(FixingStructureSubMachineModulesConst.ReleaseFixingFinishedEventID, new GFBaseTypeParamValueList()));
            }
        }

        private void RobotDriver_PositionChanged(object? sender, PositionChangedEventArgs e)
        {
            if (e.NewPosition.Length > 0)
            {
                currentPosition = e.NewPosition[0].PositionValue;
                iSubMMCmdExecutorCallBack.SendToMapTmlStateChanged(JsonObjConvert.ToJSon(new InformInfo_StatusChanged(alias.ToString(),"PositionChanged", currentPosition.ToString())));
            }
        }

        private void Fixing()
        {
            EnsureFixingCylinderInitialized();
            isFixed = true;
            RobotMoveTo(pPCfg.trajectoryParameters.MaxSpeed, pPCfg.trajectoryParameters.Acceleration, pPCfg.FixingPosition);
        }

        private void ReleaseFixing()
        {
            EnsureFixingCylinderInitialized();
            isFixed = false;
            RobotMoveTo(pPCfg.trajectoryParameters.MaxSpeed, pPCfg.trajectoryParameters.Acceleration, pPCfg.ReleaseFixingPosition);
        }

        /// <summary>
        /// 移动到指定位置
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="acc"></param>
        /// <param name="position"></param>
        private void RobotMoveTo(double speed, double acc, double position)
        {
            robotDriver?.Execute(
                new MotionInstructionSequence
                {
                    SequenceType = MotionInstructionSequenceType.StepByStep,
                    Instructions = new MotionInstructionBase[]
                    {
                         new Point
                         {
                              Acceleration = acc,
                               Speed = speed,
                               TargetPosition = new AxisConstantValues[]
                               {
                                      new AxisConstantValues
                                      {
                                         Axis = 0,
                                          PositionValue = position
                                      }
                               }
                         }
                    }
                },
                new RobotExecutionContext());
        }
        /// <summary>
        /// 连续运动
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="acc"></param>
        /// <param name="direction"></param>
        private void RobotContinueMove(double speed, double acc, bool direction)
        {
            robotDriver?.Execute(
                new MotionInstructionSequence
                {
                    SequenceType = MotionInstructionSequenceType.StepByStep,
                    Instructions = new MotionInstructionBase[]
                    {
                         new ContinueMoveInstruction
                         {
                             Speed = speed*(direction?1:-1),
                             Acceleration = acc,
                             LogicAxis = 0
                         }
                    }
                },
                new RobotExecutionContext());
        }
        /// <summary>
        /// 停止运动
        /// </summary>
        private void RobotStop()
        {
            robotDriver?.Execute(
                new MotionInstructionSequence
                {
                    SequenceType = MotionInstructionSequenceType.StepByStep,
                    Instructions = new MotionInstructionBase[]
                    {
                         new StopMoveInstruction
                         {
                              LogicAxis = 0
                         }
                    }
                },
                new RobotExecutionContext());
        }

    }
}
