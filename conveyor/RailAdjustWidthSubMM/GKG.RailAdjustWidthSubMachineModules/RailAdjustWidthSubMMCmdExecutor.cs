using GF_Gereric;
using GKG.MotionControl;
using Griffins;
using Griffins.ImeIOT;
using Griffins.Map;
using Griffins.PF.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GKG
{
    namespace SubMM
    {
        public class RailAdjustWidthSubMMCmdExecutor : ISubMMCmdExecutor, ISubMMManualModeCmdExecutor, ISubMMAutoModeCmdExecutor
        {
            private RailAdjustWidthSubMachineModulesFactoryCfg railAdjustWidthSubMachineModulesFactoryCfg;
            private RailAdjustWidthSubMachineModulesInitCfg railAdjustWidthSubMachineModulesInitCfg;
            private RailAdjustWidthSubMachineModulesPPCfg railAdjustWidthSubMachineModulesPPCfg;
            private ISubMMCmdExecutorCallBack iSubMMCmdExecutorCallBack;
            private readonly SubMMAlias alias;
            private ImeGenNormalEventHandler imeGenNormalEventHandler;
            private ImeCabilityEventHandler imeCabilityEventHandler;
            private ImeAlarmEventHandler imeAlarmEventHandler;
            private IRobotDriver frontRailRobot;
            private IRobotDriver backRailRobot;
            private double frontRailPosition;
            private double backRailPosition;
            private bool isInitialized = false;
            public RailAdjustWidthSubMMCmdExecutor(SubMMAlias alias, byte[] factoryCfgInfo)
            {
                this.alias = alias;
                railAdjustWidthSubMachineModulesFactoryCfg = new RailAdjustWidthSubMachineModulesFactoryCfg();
                railAdjustWidthSubMachineModulesFactoryCfg.FromBytes(factoryCfgInfo);
                railAdjustWidthSubMachineModulesInitCfg = new RailAdjustWidthSubMachineModulesInitCfg();
                railAdjustWidthSubMachineModulesPPCfg = new RailAdjustWidthSubMachineModulesPPCfg();
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
                railAdjustWidthSubMachineModulesFactoryCfg ??= new RailAdjustWidthSubMachineModulesFactoryCfg();
                railAdjustWidthSubMachineModulesInitCfg ??= new RailAdjustWidthSubMachineModulesInitCfg();
                railAdjustWidthSubMachineModulesPPCfg ??= new RailAdjustWidthSubMachineModulesPPCfg();
                iSubMMCmdExecutorCallBack = callBack;

                if (initCfgInfo != null && initCfgInfo.Length > 0)
                {
                    railAdjustWidthSubMachineModulesInitCfg = new RailAdjustWidthSubMachineModulesInitCfg();
                    railAdjustWidthSubMachineModulesInitCfg.FromBytes(initCfgInfo);
                }

            }
            void ISubMMCmdExecutor.AfterInit()
            {
                EnsureRailRobotsInitialized();
            }
            void ISubMMCmdExecutor.UnInit()
            {
                if (frontRailRobot != null)
                {
                    frontRailRobot.PositionChanged -= FrontRailRobot_PositionChanged;
                    frontRailRobot = null;
                }
                if (backRailRobot != null)
                {
                    backRailRobot.PositionChanged -= BackRailRobot_PositionChanged;
                    backRailRobot = null;
                }
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
                    railAdjustWidthSubMachineModulesPPCfg.FromBytes(pfCfgInfo);
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
                    case RailAdjustWidthSubMachineModulesConst.RailAdjustWidthMethodID:
                        {
                            AdjustWidth();
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
                GFBaseTypeParamValueList rst = new GFBaseTypeParamValueList();
                switch (cmdID)
                {
                    case RailAdjustWidthSubMachineModulesConst.RailContinueMoveMethodID:
                        {
                            RailAdjustWidthAxis railID = Enum.Parse<RailAdjustWidthAxis>(cmdParam["RailID"].ToStringVal());
                            MoveDirection direction = Enum.Parse<MoveDirection>(cmdParam["Direction"].ToStringVal());
                            double speed = (double)cmdParam["Speed"].ToDecimal();
                            double acc = (double)cmdParam["Acceleration"].ToDecimal();
                            ContinueMove(new TranslationParameters
                            {
                                RailID = railID,
                                Direction = direction,
                                Speed = speed,
                                Acceleration = acc
                            });
                        }
                        break;
                    case RailAdjustWidthSubMachineModulesConst.RailAdjustWidthMethodID:
                        {
                            AdjustWidth();
                        }
                        break;
                    case RailAdjustWidthSubMachineModulesConst.RtCmdGetAxisOptions:
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
                    case RailAdjustWidthSubMachineModulesConst.RtCmdGetFactoryParams:
                        {
                            rst.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue(JsonObjConvert.ToJSon(railAdjustWidthSubMachineModulesFactoryCfg))));
                        }
                        break;
                    case RailAdjustWidthSubMachineModulesConst.RtCmdMoveTo:
                        {
                            RailAdjustWidthAxis railID = Enum.Parse<RailAdjustWidthAxis>(cmdParam["RailID"].ToStringVal());
                            double speed = (double)cmdParam["Speed"].ToDecimal();
                            double acc = (double)cmdParam["Acceleration"].ToDecimal();
                            double position = (double)cmdParam["Position"].ToDecimal();
                            IRobotDriver robot = GetRobotByRailID(railID);
                            ExecuteAbsoluteMove(robot, position, speed, acc);
                        }
                        break;
                    case RailAdjustWidthSubMachineModulesConst.RtCmdStop:
                        {
                            RailAdjustWidthAxis railID = Enum.Parse<RailAdjustWidthAxis>(cmdParam["RailID"].ToStringVal());
                            IRobotDriver robot = GetRobotByRailID(railID);
                            ExecuteStop(robot);
                        }
                        break;
                    case RailAdjustWidthSubMachineModulesConst.RtGoHome:
                        {
                            RailAdjustWidthAxis railID = Enum.Parse<RailAdjustWidthAxis>(cmdParam["RailID"].ToStringVal());
                            IRobotDriver robot = GetRobotByRailID(railID);
                            ExecuteHome(robot, 0);
                        }
                        break;
                    default:
                        return rst;
                }
                return rst;
            }


            /// <summary>
            /// 初始化调宽轨机械手
            /// </summary>
            private void EnsureRailRobotsInitialized()
            {
                if (isInitialized)
                    return;
                if (railAdjustWidthSubMachineModulesFactoryCfg.FrontRailIsMovable)
                {
                    frontRailRobot ??= CreateRobotDriver(
                        railAdjustWidthSubMachineModulesInitCfg.FrontRailAxisBindingObjID,
                        Resources.RailAdjustWidthFrontRailAxisBindingMissing,
                        Resources.RailAdjustWidthFrontRailRobotDriverResponseMissing,
                        Resources.RailAdjustWidthFrontRailRobotDriverUnavailable);
                    frontRailRobot.PositionChanged += FrontRailRobot_PositionChanged;
                }

                if (railAdjustWidthSubMachineModulesFactoryCfg.BackRailIsMovable)
                {
                    backRailRobot ??= CreateRobotDriver(
                        railAdjustWidthSubMachineModulesInitCfg.BackRailAxisBindingObjID,
                        Resources.RailAdjustWidthBackRailAxisBindingMissing,
                        Resources.RailAdjustWidthBackRailRobotDriverResponseMissing,
                        Resources.RailAdjustWidthBackRailRobotDriverUnavailable);
                    backRailRobot.PositionChanged += BackRailRobot_PositionChanged;
                }
                    
                isInitialized = true;
            }
            private void FrontRailRobot_PositionChanged(object sender, PositionChangedEventArgs e)
            {
                if (e.NewPosition.Length > 0)
                {
                    frontRailPosition = e.NewPosition[0].PositionValue;
                    iSubMMCmdExecutorCallBack.SendToMapTmlStateChanged(JsonObjConvert.ToJSon(new InformInfo_StatusChanged(alias.ToString(), RailAdjustWidthSubMachineModulesConst.FrontRailPosition, frontRailPosition.ToString())));
                }
            }
            private void BackRailRobot_PositionChanged(object sender, PositionChangedEventArgs e)
            {
                if (e.NewPosition.Length > 0)
                {
                    backRailPosition = e.NewPosition[0].PositionValue;
                    iSubMMCmdExecutorCallBack.SendToMapTmlStateChanged(JsonObjConvert.ToJSon(new InformInfo_StatusChanged(alias.ToString(), RailAdjustWidthSubMachineModulesConst.BackRailPosition, backRailPosition.ToString())));
                }
            }

            private static IRobotDriver CreateRobotDriver(Guid axisBindingObjID, string axisMissingMsg, string responseMissingMsg, string unavailableMsg)
            {
                if (axisBindingObjID == Guid.Empty)
                    throw new InvalidOperationException(axisMissingMsg);

                RobotDriverByAxisIdsRequest request = new RobotDriverByAxisIdsRequest(new List<Guid> { axisBindingObjID })
                {
                    MotionCardType = MotionControlCardType.Normal
                };

                var robotDriverResponses = ServerInnerInfoSender.SendMutualInfo(RobotDriverByAxisIdsRequest.InfoKindID, request);
                if (robotDriverResponses == null || robotDriverResponses.Count == 0)
                    throw new InvalidOperationException(responseMissingMsg);

                RobotDriverByAxisIdsResponse robotDriverResponse = robotDriverResponses[0].Response as RobotDriverByAxisIdsResponse;
                if (robotDriverResponse?.RobotDriver == null)
                    throw new InvalidOperationException(unavailableMsg);

                return robotDriverResponse.RobotDriver;
            }

            private void AdjustWidth()
            {
                EnsureRailRobotsInitialized();

                double targetWidth = railAdjustWidthSubMachineModulesPPCfg.Width;
                double frontRailAxisPos = 0;
                double backRailAxisPos = 0;
                CalWidthPos(targetWidth, ref frontRailAxisPos, ref backRailAxisPos);

                if (railAdjustWidthSubMachineModulesFactoryCfg.FrontRailIsMovable && frontRailRobot != null)
                    ExecuteAbsoluteMove(frontRailRobot, frontRailAxisPos, GetAdjustSpeed(), GetAdjustAcceleration());

                if (railAdjustWidthSubMachineModulesFactoryCfg.BackRailIsMovable && backRailRobot != null)
                    ExecuteAbsoluteMove(backRailRobot, backRailAxisPos, GetAdjustSpeed(), GetAdjustAcceleration());
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="translationParameters"></param>
            /// <exception cref="InvalidOperationException"></exception>
            private void ContinueMove(TranslationParameters translationParameters)
            {
                EnsureRailRobotsInitialized();

                double speed = translationParameters.Speed > 0 ? translationParameters.Speed : GetAdjustSpeed();
                double acc = translationParameters.Acceleration > 0 ? translationParameters.Acceleration : 1;

                // 根据方向调整速度正负
                if (translationParameters.Direction == MoveDirection.Negative)
                {
                    speed *= -1;
                }
                IRobotDriver robot = GetRobotByRailID(translationParameters.RailID);
                ExecuteContinueMove(robot, acc, speed);
            }
            private IRobotDriver GetRobotByRailID(RailAdjustWidthAxis railID)
            {
                switch (railID)
                {
                    case RailAdjustWidthAxis.FrontRail:
                        return frontRailRobot;
                    case RailAdjustWidthAxis.BackRail:
                        return backRailRobot;
                    default:
                        throw new InvalidOperationException(Resources.RailAdjustWidthInvalidRailID);
                }
                 
            }
            /// <summary>
            /// 回零方法 待定
            /// </summary>
            private void Original()
            {
                EnsureRailRobotsInitialized();

                if (railAdjustWidthSubMachineModulesFactoryCfg.FrontRailIsMovable && frontRailRobot != null)
                    ExecuteHome(frontRailRobot, 0);

                if (railAdjustWidthSubMachineModulesFactoryCfg.BackRailIsMovable && backRailRobot != null)
                    ExecuteHome(backRailRobot, 0);
            }

            /// <summary>
            /// 计算调宽位置
            /// </summary>
            /// <param name="adjustWidth"></param>
            /// <param name="frontRailAxisPos"></param>
            /// <param name="backRailAxisPos"></param>
            /// <exception cref="InvalidOperationException"></exception>
            private void CalWidthPos(double adjustWidth, ref double frontRailAxisPos, ref double backRailAxisPos)
            {
                adjustWidth += 1;
                double maxWidth = railAdjustWidthSubMachineModulesFactoryCfg.MaxWidth;
                double minWidth = railAdjustWidthSubMachineModulesFactoryCfg.MinWidth;

                if (adjustWidth > (maxWidth + minWidth))
                    throw new InvalidOperationException(Resources.RailAdjustWidthTargetWidthOutOfRange);

                RailAdjustWidthAxis fixRailID = railAdjustWidthSubMachineModulesPPCfg.FixRailID ?? railAdjustWidthSubMachineModulesFactoryCfg.FixRailID;
                double fixAxisPos = railAdjustWidthSubMachineModulesPPCfg.FixRailPosition ?? railAdjustWidthSubMachineModulesFactoryCfg.FixRailPosition;
                bool isEnableMaxDistanceAdjustWidth = railAdjustWidthSubMachineModulesInitCfg.IsEnableMaxDistanceAdjustWidth;

                if (isEnableMaxDistanceAdjustWidth)
                {
                    double slaveAxisPos = maxWidth + minWidth - adjustWidth;
                    if (fixRailID == RailAdjustWidthAxis.BackRail)
                    {
                        frontRailAxisPos = fixAxisPos;
                        backRailAxisPos = slaveAxisPos;
                    }
                    else
                    {
                        frontRailAxisPos = slaveAxisPos;
                        backRailAxisPos = fixAxisPos;
                    }
                }
                else
                {
                    double slaveAxisPos = fixAxisPos + adjustWidth - minWidth;
                    if (fixRailID == RailAdjustWidthAxis.BackRail)
                    {
                        frontRailAxisPos = fixAxisPos;
                        backRailAxisPos = slaveAxisPos;
                    }
                    else
                    {
                        frontRailAxisPos = slaveAxisPos;
                        backRailAxisPos = fixAxisPos;
                    }
                }
            }
            /// <summary>
            /// 获得当前调宽速度，优先使用初始化配置中的速度参数，如果未设置或设置为非正数，则使用默认速度20
            /// </summary>
            /// <returns></returns>

            private int GetAdjustSpeed()
            {
                return railAdjustWidthSubMachineModulesInitCfg.AdjustWidthSpeed > 0
                    ? railAdjustWidthSubMachineModulesInitCfg.AdjustWidthSpeed
                    : 20;
            }
            private int GetAdjustAcceleration()
            {
                return railAdjustWidthSubMachineModulesInitCfg.AdjustWidthAcceleration > 0
                    ? railAdjustWidthSubMachineModulesInitCfg.AdjustWidthAcceleration
                    : 200;
            }

            /// <summary>
            /// 绝对运动到目标位置
            /// </summary>
            /// <param name="robot"></param>
            /// <param name="targetPos"></param>
            /// <param name="speed"></param>
            private static void ExecuteAbsoluteMove(IRobotDriver robot, double targetPos, double speed, double acc)
            {
                robot.Execute(new MotionInstructionSequence()
                {
                    SequenceType = MotionInstructionSequenceType.StepByStep,
                    Instructions = new MotionInstructionBase[]
                    {
                        new Point()
                        {
                            Acceleration = acc,
                            Speed = speed,
                            TargetPosition = new AxisConstantValues[]
                            {
                                new AxisConstantValues
                                {
                                    Axis = 0,
                                    PositionValue = targetPos
                                }
                            }
                        }
                    },
                    ExtendedParameters = null
                }, new RobotExecutionContext());
            }
            /// <summary>
            /// 连续运动
            /// </summary>
            /// <param name="robot"></param>
            /// <param name="acc"></param>
            /// <param name="speed"></param>
            private static void ExecuteContinueMove(IRobotDriver robot, double acc, double speed)
            {
                robot.Execute(new MotionInstructionSequence()
                {
                    SequenceType = MotionInstructionSequenceType.StepByStep,
                    Instructions = new MotionInstructionBase[]
                    {
                        new ContinueMoveInstruction()
                        {
                            Speed = speed,
                            Acceleration = acc,
                            LogicAxis = 0,
                        }
                    },
                    ExtendedParameters = null
                }, new RobotExecutionContext());
            }
            /// <summary>
            /// 回零
            /// </summary>
            /// <param name="robot"></param>
            /// <param name="logicalAxis"></param>
            private static void ExecuteHome(IRobotDriver robot, int logicalAxis)
            {
                robot.AxisHome(logicalAxis);
            }

            private static void ExecuteStop(IRobotDriver robot)
            {
                robot.Execute(new MotionInstructionSequence
                {
                    SequenceType = MotionInstructionSequenceType.StepByStep,
                    Instructions = new MotionInstructionBase[]
                    {
                        new StopMoveInstruction
                        {
                            LogicAxis = 0,
                        }
                    },
                    ExtendedParameters = null
                }, new RobotExecutionContext());
            }
        }
    }
}
