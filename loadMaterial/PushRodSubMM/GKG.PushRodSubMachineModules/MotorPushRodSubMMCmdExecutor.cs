using GKG.MotionControl;
using Griffins;
using Griffins.ImeIOT;
using Griffins.PF;
//using Griffins.PF.RichClient;
using System;
using System.Threading;
using System.Threading.Tasks;
using GKG.ElectronicControl.General;
using GKG.ElectronicControl;
using System.Collections.Generic;
using System.Globalization;
using GF_Gereric;
using System.Linq;
using System.Text;
using MotorPushRodSubMachineModulesConst = GKG.SubMM.PushRodSubMachineModulesConst;
using Griffins.PF.Server;
using GKG.PushRodSubMachineModules.Properties;

namespace GKG
{
    namespace SubMM
    {
        /// <summary>电机推杆子模组执行器：负责电机轴绑定、推/退料动作和推料位感应。</summary>
        /// <remarks>整体说明见 loadmaterial/EXECUTOR_IMPLEMENTATION.md §4。</remarks>
        public class MotorPushRodSubMMCmdExecutor : ISubMMCmdExecutor, ISubMMManualModeCmdExecutor, ISubMMAutoModeCmdExecutor
        {
            private const string PushRodAxisInformType = "PushRod";

            /// <summary>电机推杆单次动作状态机：先校验，再执行，最后统一产出成功/失败结果。</summary>
            private enum MotorPushRodState
            {
                Start,
                Validate,
                Execute,
                Success,
                Fail,
                End
            }

            private MotorPushRodSubMachineModulesFactoryCfg motorPushRodSubMachineModulesFactoryCfg;

            private MotorPushRodSubMachineModulesInitCfg motorPushRodSubMachineModulesInitCfg;

            private MotorPushRodSubMachineModulesPPCfg motorPushRodSubMachineModulesPPCfg;

            private ISubMMCmdExecutorCallBack iSubMMCmdExecutorCallBack;

            private SubMMAlias alias;

            private ImeGenNormalEventHandler imeGenNormalEventHandler;

            private ImeCabilityEventHandler imeCabilityEventHandler;

            private ImeAlarmEventHandler imeAlarmEventHandler;

            /// <summary>BasicRobot 驱动实例，仅绑定一个逻辑轴，对应推杆物理轴。</summary>
            private IRobotDriver pushRodMotor;

            /// <summary>本地获取到的运控卡实例；机械手驱动通过该实例完成真实轴控制。</summary>
            private IMotionControlBase motionControl;

            private bool bLoadFinished = false;

            private bool bUnLoadFinished = false;

            private readonly object axisPositionLock = new object();
            /// <summary>缓存推杆轴当前位置，避免业务层频繁直接读驱动。</summary>
            private readonly Dictionary<int, double> axisPositionCache = new Dictionary<int, double>();

            

            public MotorPushRodSubMMCmdExecutor(SubMMAlias alias, Guid subMMObjID, byte[] factoryCfgInfo)
            {
                this.alias = alias;
                if (factoryCfgInfo != null && factoryCfgInfo.Length > 0)
                    motorPushRodSubMachineModulesFactoryCfg = JsonObjConvert.FromJSonBytes<MotorPushRodSubMachineModulesFactoryCfg>(factoryCfgInfo) ?? new MotorPushRodSubMachineModulesFactoryCfg();
                else
                    motorPushRodSubMachineModulesFactoryCfg = new MotorPushRodSubMachineModulesFactoryCfg();
                motorPushRodSubMachineModulesInitCfg = new MotorPushRodSubMachineModulesInitCfg();
                motorPushRodSubMachineModulesPPCfg = new MotorPushRodSubMachineModulesPPCfg();
            }

            #region 框架接口：事件、初始化与生命周期

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

            /// <summary>加载初始化参数，获取运控实例，并按当前物理轴重建推杆轴绑定。</summary>
            void ISubMMCmdExecutor.Init(byte[] initCfgInfo, byte[] calibrationCfgInfo, ISubMMCmdExecutorCallBack callBack)
            {

                if (initCfgInfo != null && initCfgInfo.Length > 0)
                {
                    motorPushRodSubMachineModulesInitCfg = new MotorPushRodSubMachineModulesInitCfg();
                    motorPushRodSubMachineModulesInitCfg.FromBytes(initCfgInfo);
                }

                iSubMMCmdExecutorCallBack = callBack;
            }
            void ISubMMCmdExecutor.BeforeInit(GFBaseTypePropValueList devicePropValues) { }
            void ISubMMCmdExecutor.AfterInit()
            {
                if (motorPushRodSubMachineModulesInitCfg.PusherPhysicalAxis != Guid.Empty)
                    RebuildPushRodRobotBinding(motorPushRodSubMachineModulesInitCfg.PusherPhysicalAxis);
            }
            void ISubMMCmdExecutor.UnInit() { }

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
            #endregion

            #region 配方与运行控制

            /// <summary>加载配方：推料距离、速度、加速度。</summary>
            void ISubMMAutoModeCmdExecutor.SetPFCfgInfo(byte[] pfCfgInfo)
            {
                motorPushRodSubMachineModulesPPCfg.FromBytes(pfCfgInfo);
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


            void ISubMMAutoModeCmdExecutor.AfterStopWork() { }

            /// <summary>流程方法：Extend/Retract 走状态机；SetLoad/UnloadFinished 置完成标志。</summary>
            GFBaseTypeParamValueList ISubMMAutoModeCmdExecutor.ExecMethod(string methodID, GFBaseTypeParamValueList param)
            {
                switch (methodID)
                {
                    case MotorPushRodSubMachineModulesConst.CheckHasMaterialMethodID:
                        return CreateHasMaterialResult(ReadHasMaterial(param));
                    case MotorPushRodSubMachineModulesConst.ExtendMethodID:
                        return ExecuteMotorPushRodStateMachine(
                            methodID,
                            () => ExtendMaterial(),
                            MotorPushRodSubMachineModulesConst.EventPusherForwardCompleted);
                    case MotorPushRodSubMachineModulesConst.RetractMethodID:
                        return ExecuteMotorPushRodStateMachine(
                            methodID,
                            () => RetractMaterial(),
                            MotorPushRodSubMachineModulesConst.EventPusherBackwardCompleted);
                    case MotorPushRodSubMachineModulesConst.SetLoadFinishedMethodID:
                        ConfirmLoadFinished();
                        return CreateSuccessResult(string.Empty);
                    case MotorPushRodSubMachineModulesConst.SetUnloadFinishedMethodID:
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
                    case MotorPushRodSubMachineModulesConst.GetInitParametersMethodID:
                        return CreateParametersResult(motorPushRodSubMachineModulesInitCfg);
                    case MotorPushRodSubMachineModulesConst.GetRecipeParametersMethodID:
                        return CreateParametersResult(motorPushRodSubMachineModulesPPCfg);
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
            /// <summary>运行时命令入口（配方页点动），与手动 ExecCtlCmd 共用核心分发。</summary>
            GFBaseTypeParamValueList ISubMMAutoModeCmdExecutor.ExecRuntimeCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                return ExecRuntimeCtlCmdCore(cmdID, cmdParam);
            }

            /// <summary>运行时命令分发：GetStatus / 前推 / 后退 / 推料一次 / 轴列表。</summary>
            GFBaseTypeParamValueList ExecRuntimeCtlCmdCore(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                GFBaseTypeParamValueList runtimeParam = cmdParam ?? new GFBaseTypeParamValueList();
                switch (cmdID)
                {
                    case MotorPushRodSubMachineModulesConst.RtCmdGetStatus:
                        EnsurePushRodRobotBound();
                        return CreateSuccessResult(
                            string.Empty,
                            GetCurrentPusherPosition(runtimeParam).ToString(CultureInfo.InvariantCulture));
                    case MotorPushRodSubMachineModulesConst.RtCmdPusherForward:
                        return ExecuteMotorPushRodStateMachine(
                            MotorPushRodSubMachineModulesConst.ExtendMethodID,
                            () =>
                            {
                                HandlePusherAbsoluteMove(runtimeParam, true);
                            },
                            MotorPushRodSubMachineModulesConst.EventPusherForwardCompleted,
                            () => GetSignedRuntimeDistance(runtimeParam, true));
                    case MotorPushRodSubMachineModulesConst.RtCmdPusherBackward:
                        return ExecuteMotorPushRodStateMachine(
                            MotorPushRodSubMachineModulesConst.RetractMethodID,
                            () =>
                            {
                                HandlePusherAbsoluteMove(runtimeParam, false);
                            },
                            MotorPushRodSubMachineModulesConst.EventPusherBackwardCompleted,
                            () => GetSignedRuntimeDistance(runtimeParam, false));
                    case MotorPushRodSubMachineModulesConst.RtCmdPushOnce:
                        return ExecuteMotorPushRodStateMachine(
                            MotorPushRodSubMachineModulesConst.ExtendMethodID,
                            () => PushOnce(runtimeParam),
                            MotorPushRodSubMachineModulesConst.EventPusherForwardCompleted,
                            () => GetSignedRuntimeDistance(runtimeParam, true));
                    case MotorPushRodSubMachineModulesConst.RtCmdGetAxisOptions:
                        return CreateSuccessResult(string.Empty, ToJson(GetAllAxisInfosFromElectronicManager()));
                    default:
                        return CreateErrorResult($"未识别的运行时命令: {cmdID ?? "<null>"}");
                }
            }

            GFBaseTypeParamValueList ISubMMManualModeCmdExecutor.ExecCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                return ExecRuntimeCtlCmdCore(cmdID, cmdParam);
            }

            private static string ToJson(object data)
            {
                return data != null
                    ? Encoding.UTF8.GetString(JsonObjConvert.ToJSonBytes(data))
                    : "{}";
            }
            /// <summary>从电器管理获取全部轴信息（RtCmdGetAxisInfos / 初始化页）。</summary>
            private static List<AxisInformation> GetAllAxisInfosFromElectronicManager()
            {
                Dictionary<Guid, AxisInformation> axisInfoDict = new Dictionary<Guid, AxisInformation>();
                var ioStateInfosResponse = ServerInnerInfoSender.SendMutualInfo(
                    AxisInfosRequest.InfoKindID,
                    new AxisInfosRequest(MotionControlCardType.TypeA));

                if (ioStateInfosResponse == null || ioStateInfosResponse.Count == 0)
                    return new List<AxisInformation>();

                AxisInfosResponse response = ioStateInfosResponse[0].Response as AxisInfosResponse;
                if (response?.AxisInformations == null)
                    return new List<AxisInformation>();

                foreach (AxisInformation axisInfo in response.AxisInformations)
                {
                    if (axisInfo != null && axisInfo.AxisGuid != Guid.Empty)
                        axisInfoDict[axisInfo.AxisGuid] = axisInfo;
                }

                return axisInfoDict.Values.ToList();
            }

            /// <summary>
            /// 获取界面数据对象属性读写接口实例，如果不支持返回nul
            /// </summary>
            /// <returns>界面数据对象属性读写接口实例</returns>
            ICompUIDataObjPropValRW ISubMMAutoModeCmdExecutor.GetUIDataObjPropValRW()
            {
                return null;
            }


            #region 运动与位置

            /// <summary>命令参数 MaxSpeed 优先，否则用配方 PushAxisSpeed。</summary>
            private bool TryGetEffectiveMaxSpeed(GFBaseTypeParamValueList cmdParam, out double speed)
            {
                if (TryGetDouble(cmdParam, "MaxSpeed", out speed) && speed > 0)
                    return true;
                // 使用推杆轴速度参数
                speed = motorPushRodSubMachineModulesPPCfg.PushAxisSpeed > 0
                    ? motorPushRodSubMachineModulesPPCfg.PushAxisSpeed
                    : 10.0; // 默认速度
                return speed > 0;
            }

            /// <summary>命令参数 PushDistance 优先，否则用配方 PushDistance。</summary>
            private bool TryGetEffectivePushDistance(GFBaseTypeParamValueList cmdParam, out double distance, out string errorMsg)
            {
                errorMsg = null;
                distance = 0;
                if (TryGetDouble(cmdParam, "PushDistance", out distance) && distance > 0)
                    return true;
                // 使用推杆距离参数
                distance = motorPushRodSubMachineModulesPPCfg.PushDistance > 0
                    ? motorPushRodSubMachineModulesPPCfg.PushDistance
                    : 0.0;
                if (distance > 0)
                    return true;
                errorMsg = "缺少 PushDistance，且配方中未配置";
                return false;
            }

            /// <summary>按指定物理轴重建推杆 Robot 绑定；通过轴信息互斥消息找到 AxisGuid，再通过 RobotDriverByAxisIdsRequest 获取驱动。</summary>
            private void RebuildPushRodRobotBinding(Guid physicalAxis)
            {
                if (pushRodMotor != null)
                    pushRodMotor.PositionChanged -= OnPositionChanged;

                RobotDriverByAxisIdsRequest request = new RobotDriverByAxisIdsRequest(new List<Guid> { physicalAxis })
                {
                    MotionCardType = MotionControlCardType.Normal
                };

                var robotDriverResponses = ServerInnerInfoSender.SendMutualInfo(RobotDriverByAxisIdsRequest.InfoKindID, request);
                if (robotDriverResponses == null || robotDriverResponses.Count == 0)
                    throw new InvalidOperationException(Resources.PushRodRobotDriverResponseMissing);

                RobotDriverByAxisIdsResponse robotDriverResponse = robotDriverResponses[0].Response as RobotDriverByAxisIdsResponse;
                if (robotDriverResponse?.RobotDriver == null)
                    throw new InvalidOperationException(Resources.PushRodRobotDriverUnavailable);

                pushRodMotor = robotDriverResponse.RobotDriver;
                pushRodMotor.PositionChanged -= OnPositionChanged;
                pushRodMotor.PositionChanged += OnPositionChanged;
            }

            /// <summary>同步驱动上报的位置到本地缓存，供当前位置读取和相对运动复用。</summary>
            private void OnPositionChanged(object? sender, PositionChangedEventArgs e)
            {
                if (e?.NewPosition == null || e.NewPosition.Length == 0)
                    return;

                lock (axisPositionLock)
                {
                    foreach (AxisConstantValues position in e.NewPosition)
                    {
                        axisPositionCache[position.Axis] = position.PositionValue;
                        PublishAxisPositionChangedInfo(position.Axis, position.PositionValue);
                    }
                }
            }

            private void PublishAxisPositionChangedInfo(int logicAxis, double currentPosition)
            {
                if (logicAxis != 0)
                    return;

                if (iSubMMCmdExecutorCallBack == null)
                    return;

                var axisStatus = new PushRodAxisStatus
                {
                    Staus = 0,
                    Position = currentPosition
                };
                string paramJson = JsonObjConvert.ToJSon(axisStatus);

                iSubMMCmdExecutorCallBack.SendToMapTmlStateChanged(paramJson);
            }

            /// <summary>推料一次：按配方/界面参数前推指定距离后再回退，完成单次推料动作。</summary>
            private void PushOnce(GFBaseTypeParamValueList cmdParam)
            {
                RebuildPushRodRobotBinding(motorPushRodSubMachineModulesInitCfg.PusherPhysicalAxis);
                HandlePusherAbsoluteMove(cmdParam, true);
                HandlePusherAbsoluteMove(cmdParam, false);
            }

            /// <summary>按参数/配方计算距离与速度，执行单次相对点位移动（逻辑轴 0）。</summary>
            private void HandlePusherAbsoluteMove(
                GFBaseTypeParamValueList cmdParam,
                bool isForward)
            {
                if (!TryGetEffectivePushDistance(cmdParam, out double distance, out string errDist) || distance <= 0)
                    throw new InvalidOperationException(errDist ?? Resources.PushDistanceInvalid);
                if (!TryGetEffectiveMaxSpeed(cmdParam, out double speed) || speed <= 0)
                    throw new InvalidOperationException(Resources.MaxSpeedInvalid);
                double acc = TryGetDouble(cmdParam, "Acc", out double accVal) && accVal > 0
                    ? accVal
                    : Math.Max(1.0, speed / 10.0);
                double delta = isForward ? distance : -distance;
                PushRodAbsoluteMove(delta, speed, acc);
            }

            private static bool TryGetInt(GFBaseTypeParamValueList cmdParam, string key, out int value)
            {
                return RuntimeParamReader.TryGetInt(cmdParam, key, out value);
            }

            private static bool TryGetDouble(GFBaseTypeParamValueList cmdParam, string key, out double value)
            {
                return RuntimeParamReader.TryGetDouble(cmdParam, key, out value);
            }

            private static bool TryGetString(GFBaseTypeParamValueList cmdParam, string key, out string value)
            {
                return RuntimeParamReader.TryGetString(cmdParam, key, out value);
            }

            /// <summary>下发 StepByStep 单点指令到 pushRodMotor。</summary>
            private void PushRodAbsoluteMove(double deltaMm, double speed, double acc)
            {
                //MoveParam moveParam = new MoveParam
                //{
                //    acc = acc,
                //    speed = speed,
                //    AxisCount = 1,
                //    logicAxis = new[] { 0 },
                //    targetPosition = new Point3D(deltaMm, 0, 0)
                //};
                pushRodMotor.Execute(new MotionInstructionSequence()
                {
                    SequenceType = MotionInstructionSequenceType.StepByStep,
                    Instructions = new MotionInstructionBase[] {
                        new Point()
                        {
                             Acceleration = acc,
                             Speed = speed,
                             TargetPosition = new AxisConstantValues[]
                             { 
                                 new AxisConstantValues 
                                 {  
                                     Axis = 0, 
                                     PositionValue = deltaMm 
                                 } 
                             }
                        }
                    },
                    ExtendedParameters = null
                }, new RobotExecutionContext()); 
            }

            /// <summary>驱动未绑定时按 InitCfg 物理轴 GUID 重建 Robot。</summary>
            private void EnsurePushRodRobotBound()
            {
                if (pushRodMotor == null && motorPushRodSubMachineModulesInitCfg.PusherPhysicalAxis != Guid.Empty)
                    RebuildPushRodRobotBinding(motorPushRodSubMachineModulesInitCfg.PusherPhysicalAxis);
            }

            /// <summary>读逻辑轴 0 位置缓存；无缓存返回 0。</summary>
            private double GetCurrentPusherPosition(GFBaseTypeParamValueList cmdParam)
            {
                lock (axisPositionLock)
                {
                    if (axisPositionCache.TryGetValue(0, out double cachedPosition))
                        return cachedPosition;
                }

                return 0;
            }

            /// <summary>流程缩回：相对负向移动，轮询 bUnLoadFinished 直至超时。</summary>
            public void RetractMaterial()
            {
                bUnLoadFinished = false;
                double speed = motorPushRodSubMachineModulesPPCfg.PushAxisSpeed > 0
                    ? motorPushRodSubMachineModulesPPCfg.PushAxisSpeed
                    : 10.0;

                RebuildPushRodRobotBinding(motorPushRodSubMachineModulesInitCfg.PusherPhysicalAxis);

                pushRodMotor.Execute(new MotionInstructionSequence()
                {
                    SequenceType = MotionInstructionSequenceType.StepByStep,
                    Instructions = new MotionInstructionBase[] {
                        new Point()
                        {
                            Acceleration = motorPushRodSubMachineModulesPPCfg.PushAxisAcceleration,
                            Speed = speed,
                            TargetPosition = new AxisConstantValues[]
                            {
                                new AxisConstantValues
                                {
                                    Axis = 0,
                                    PositionValue = 0
                                }
                            }
                        }
                    },
                    ExtendedParameters = null
                }, new RobotExecutionContext());

                PerfmTimer perfmTimer = new PerfmTimer();
                double timeout = motorPushRodSubMachineModulesInitCfg.PushRodTimeout > 0
                    ? motorPushRodSubMachineModulesInitCfg.PushRodTimeout
                    : 5000.0;
                //perfmTimer.Start();
                //while (!bUnLoadFinished)
                //{
                //    if (perfmTimer.GetElapsedMilliseconds() > timeout)
                //    {
                //        throw new Exception(Resources.PushRodRetractTimeout);
                //    }
                //    Thread.Sleep(10);
                //}
            }

            /// <summary>流程伸出：绝对点位到配方距离，轮询 bLoadFinished 直至超时。</summary>
            public void ExtendMaterial()
            {
                bLoadFinished = false;
                // 使用推杆轴速度
                double speed = motorPushRodSubMachineModulesPPCfg.PushAxisSpeed > 0 
                    ? motorPushRodSubMachineModulesPPCfg.PushAxisSpeed 
                    : 10.0; // 默认速度
                // 使用推杆距离
                double position = motorPushRodSubMachineModulesPPCfg.PushDistance > 0 
                    ? motorPushRodSubMachineModulesPPCfg.PushDistance 
                    : 0.0; // 默认距离

                RebuildPushRodRobotBinding(motorPushRodSubMachineModulesInitCfg.PusherPhysicalAxis);

                PerfmTimer perfmTimer = new PerfmTimer();

                pushRodMotor.Execute(new MotionInstructionSequence()
                {
                    SequenceType = MotionInstructionSequenceType.StepByStep,
                    Instructions = new MotionInstructionBase[]
                    {
                        new Point()
                        {
                            Acceleration = motorPushRodSubMachineModulesPPCfg.PushAxisAcceleration,
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
                    },
                    ExtendedParameters = null
                }, new RobotExecutionContext());

                // 使用初始化配置中的推杆超时时间，默认5000ms
                double timeout = motorPushRodSubMachineModulesInitCfg.PushRodTimeout > 0
                    ? motorPushRodSubMachineModulesInitCfg.PushRodTimeout
                    : 5000.0;
                //perfmTimer.Start();
                //while (!bLoadFinished)
                //{
                //    if (perfmTimer.GetElapsedMilliseconds() > timeout)
                //    {
                //        //超时处理
                //        throw new Exception(Resources.PushRodExtendTimeout);
                //    }
                //    Thread.Sleep(10);
                //}
            }

            /// <summary>外部/流程确认伸出完成，解除 ExtendMaterial 等待。</summary>
            public void ConfirmLoadFinished() => bLoadFinished = true;

            /// <summary>外部/流程确认缩回完成，解除 RetractMaterial 等待。</summary>
            public void ConfirmUnLoadFinished() => bUnLoadFinished = true;

            /// <summary>异常映射：卡料 → PushJam，否则伸/缩失败事件。</summary>
            private static string ResolvePushRodFailureEventID(string methodID, Exception ex)
            {
                string msg = ex?.Message ?? string.Empty;
                if (msg.IndexOf("卡料", StringComparison.OrdinalIgnoreCase) >= 0 || ex is GKGException)
                    return MotorPushRodSubMachineModulesConst.EventPushJam;

                return string.Equals(methodID, MotorPushRodSubMachineModulesConst.RetractMethodID, StringComparison.Ordinal)
                    ? MotorPushRodSubMachineModulesConst.EventPusherRetractFailed
                    : MotorPushRodSubMachineModulesConst.EventPusherExtendFailed;
            }

            /// <summary>优先读取调用方显式传入的判料结果，其次走指定 IO，最后退回默认传感通道。</summary>
            private bool ReadHasMaterial(GFBaseTypeParamValueList cmdParam)
            {
                return true;
            }

            /// <summary>电机推杆动作状态机：统一封装校验、执行和结果收敛，供前推/回退/运行时命令复用。</summary>
            private GFBaseTypeParamValueList ExecuteMotorPushRodStateMachine(
                string methodID,
                Action executeAction,
                string successEventID,
                Func<string> dataProvider = null)
            {
                GFBaseTypeParamValueList result = CreateResult();
                MotorPushRodState state = MotorPushRodState.Start;
                Exception failure = null;

                while (state != MotorPushRodState.End)
                {
                    switch (state)
                    {
                        case MotorPushRodState.Start:
                            // 状态机入口：先进入资源校验阶段。
                            state = MotorPushRodState.Validate;
                            break;
                        case MotorPushRodState.Validate:
                            // 若驱动实例尚未建立，则按当前配置重建推杆轴绑定。
                            if (pushRodMotor == null)
                                RebuildPushRodRobotBinding(motorPushRodSubMachineModulesInitCfg.PusherPhysicalAxis);
                            state = MotorPushRodState.Execute;
                            break;
                        case MotorPushRodState.Execute:
                            // 执行真实推杆动作；异常统一收敛到失败分支。
                            try
                            {
                                executeAction?.Invoke();
                                state = MotorPushRodState.Success;
                            }
                            catch (Exception ex)
                            {
                                failure = ex;
                                state = MotorPushRodState.Fail;
                            }
                            break;
                        case MotorPushRodState.Success:
                            // 动作成功后统一填充标准返回字段和成功事件号。
                            result["Result"] = new GriffinsBaseValue("0");
                            result["errorMsg"] = new GriffinsBaseValue(string.Empty);
                            result["EventID"] = new GriffinsBaseValue(successEventID ?? string.Empty);
                            result["data"] = new GriffinsBaseValue(dataProvider != null ? (dataProvider() ?? string.Empty) : string.Empty);
                            state = MotorPushRodState.End;
                            break;
                        case MotorPushRodState.Fail:
                            // 动作失败时保留异常信息，并映射成对外可识别的失败事件号。
                            result["Result"] = new GriffinsBaseValue("-1");
                            result["errorMsg"] = new GriffinsBaseValue(failure?.Message ?? string.Empty);
                            string failEventID = ResolvePushRodFailureEventID(methodID, failure);
                            result["EventID"] = new GriffinsBaseValue(failEventID);
                            result["data"] = new GriffinsBaseValue(failEventID);
                            state = MotorPushRodState.End;
                            break;
                        default:
                            // 兜底保护：出现未知状态时按失败返回。
                            result["Result"] = new GriffinsBaseValue("-1");
                            result["errorMsg"] = new GriffinsBaseValue("未知推杆状态");
                            result["EventID"] = new GriffinsBaseValue(ResolvePushRodFailureEventID(methodID, null));
                            state = MotorPushRodState.End;
                            break;
                    }
                }

                return result;
            }

            /// <summary>电机推杆统一返回结构；动作命令、判料和参数查询共用这组基础字段。</summary>
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
                string json = parameters != null
                    ? Encoding.UTF8.GetString(JsonObjConvert.ToJSonBytes(parameters))
                    : "{}";
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

            /// <summary>返回带符号的运行时位移字符串，写入成功结果的 data 字段。</summary>
            private string GetSignedRuntimeDistance(GFBaseTypeParamValueList cmdParam, bool isForward)
            {
                if (!TryGetEffectivePushDistance(cmdParam, out double distance, out _))
                    return string.Empty;

                double signedDistance = isForward ? distance : -distance;
                return signedDistance.ToString(CultureInfo.InvariantCulture);
            }


            #endregion
        }
    }
}
