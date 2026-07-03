using GKG.ElectronicControl;
using System.Threading.Channels;

namespace GKG
{
    namespace MotionControl
    {
        // A类运控机械手
        public class CategoryARobot : BasicRobot
        {
            private sealed class CoordinateMotionMonitorState
            {
                public CancellationTokenSource TokenSource { get; } = new CancellationTokenSource();
                public bool StopRequested { get; set; }
                public bool ExecutionCompleted { get; set; }
                public Exception? ExecutionException { get; set; }
                public int[] LogicAxisList { get; set; } = Array.Empty<int>();
            }

            private readonly object coordinateMotionMonitorLock = new object();
            private CoordinateMotionMonitorState? coordinateMotionMonitor;

            protected IMotionControlCategoryA motionControlCategoryA;

            /// <summary>
            /// 构造函数（测试版）
            /// </summary>
            public CategoryARobot()
            {

            }

            public override string DriverName => RobotDriverNames.CategoryARobot;

            public override void Init(RobotInitParameters initParameters, IMotionControlBase? motionControlBase)
            {
                base.Init(initParameters, motionControlBase);
                if (motionControlBase != null)
                {
                    motionControlCategoryA = (IMotionControlCategoryA)motionControlBase;
                }
            }

            public override RobotCapabilities GetCapabilities()
            {
                return new RobotCapabilities
                {
                    SupportsSingleAxisMove = true,
                    SupportsTwoAxisLinearInterpolation = true,
                    SupportsThreeAxisLinearInterpolation = true,
                    SupportsTwoAxisCircularInterpolation = true,
                    SupportsContinuousInterpolationMotion = true,
                    SupportsPositionComparison2D = true,
                    SupportsManualPositionComparison = true,
                    SupportsPositionLatch = true
                };
            }

            public override MotionInstructionBase[] AdaptMotionInstructions(
                MotionInstructionBase[] instructionSequence,
                RobotExecutionContext context)
            {
                return base.AdaptMotionInstructions(instructionSequence, context);
            }

            public override void Execute(
                MotionInstructionSequence sequence,
                RobotExecutionContext context)
            {
                if (sequence == null || sequence.Instructions == null || sequence.Instructions.Length == 0)
                    return;

                RobotExecutionContext safeContext = context ?? new RobotExecutionContext();
                ApplyExecutionContext(safeContext);
                AxisBindings = AxisBindingPairs.Values.ToArray();

                MotionInstructionBase[] adaptedInstructions = AdaptMotionInstructions(sequence.Instructions, safeContext);
                if (adaptedInstructions.Length == 0)
                    return;

                switch (sequence.SequenceType)
                {
                    case MotionInstructionSequenceType.StepByStep:
                        foreach(var instruction in adaptedInstructions)
                        {
                            ExecuteInstruction(instruction, safeContext);
                        }
                        return;
                    case MotionInstructionSequenceType.ContinuousInterpolationMotion:
                        ValidateContinuousSequence(adaptedInstructions);
                        ContinuousInterpolationMotion(adaptedInstructions);
                        break;
                    default:
                        base.Execute(new MotionInstructionSequence
                        {
                            SequenceType = MotionInstructionSequenceType.StepByStep,
                            Instructions = adaptedInstructions,
                            ExtendedParameters = sequence.ExtendedParameters
                        }, safeContext);
                        return;
                }
            }

            protected override void ExecuteInstruction(
                MotionInstructionBase instruction,
                RobotExecutionContext context)
            {
                switch (instruction.InstructionType)
                {
                    case MotionInstructionType.Point:
                    case MotionInstructionType.Linear:
                    case MotionInstructionType.ArcA:
                    case MotionInstructionType.ArcB:
                    case MotionInstructionType.Circle:
                    case MotionInstructionType.Delay:
                    case MotionInstructionType.ContinueMove:
                    case MotionInstructionType.RelativeMove:
                    case MotionInstructionType.StopMove:
                    case MotionInstructionType.BufferIO:
                        base.ExecuteInstruction(instruction, context);
                        break;
                    case MotionInstructionType.Buf2DComparePulseExElemData:
                        ExecuteBuffer2DComparePulse((Buffer2DComparePulse)instruction);
                        break;
                    case MotionInstructionType.PositionComparison2D:
                        ExecutePositionComparison2D((PositionComparison2DInstruction)instruction);
                        break;
                    case MotionInstructionType.ManualPositionComparison:
                        {
                            ManualPositionComparisonInstruction manual = (ManualPositionComparisonInstruction)instruction;
                            ManualPositionComparison(manual.Channel, manual.StartLevel, manual.PulseOutputMode, manual.TriggerCount, manual.OpenTime, manual.CloseTime);
                        }
                        break;
                    case MotionInstructionType.StopManualPositionComparison:
                        {
                            StopManualPositionComparison();
                        }
                        break;
                    default:
                        throw new NotSupportedException($"BasicRobot 暂不支持指令类型: {instruction.InstructionType}");
                }
            }

            protected virtual void ValidateContinuousSequence(MotionInstructionBase[] instructions)
            {
                foreach (var instruction in instructions)
                {
                    if (!IsContinuousInstruction(instruction))
                    {
                        throw new NotSupportedException($"指令类型 {instruction.InstructionType} 不允许出现在连续序列中。");
                    }
                }
            }

            protected virtual bool IsContinuousInstruction(MotionInstructionBase instruction)
            {
                return instruction.InstructionType switch
                {
                    MotionInstructionType.Point => true,
                    MotionInstructionType.Linear => true,
                    MotionInstructionType.ArcA => true,
                    MotionInstructionType.ArcB => true,
                    MotionInstructionType.Circle => true,
                    MotionInstructionType.Delay => true,
                    MotionInstructionType.BufferIO => true,
                    MotionInstructionType.Buf2DComparePulseExElemData => true,
                    _ => false,
                };
            }

            protected override void ExecutePositionComparison2D(PositionComparison2DInstruction instruction)
            {
                if (instruction == null)
                    throw new ArgumentNullException(nameof(instruction));

                ValidateContinuousSequence(instruction.Instructions ?? Array.Empty<MotionInstructionBase>());
                PositionComparison2D(
                    instruction.PositionComparisonTriggerPoints ?? Array.Empty<MotionControlPositionComparisonTriggerPoint>(),
                    instruction.Instructions ?? Array.Empty<MotionInstructionBase>());
            }

            /// <summary>
            /// 连续插补运动
            /// </summary>
            ///
            public void ContinuousInterpolationMotion(MotionInstructionBase[] motionTrajectoryList)
            {
                RunCoordinateMotion(
                    "ContinuousInterpolationMotion",
                    axisGuids =>
                    {
                        motionControlCategoryA.ContinuousInterpolationMotion(CoordinateSystemId, axisGuids, motionTrajectoryList);
                    });
            }

            /// <summary>
            /// 位置比较输出
            /// </summary>
            /// <param name="PositionComparisonTriggerPoints">位置比较点位</param>
            /// <param name="motionTrajectoryList">运动轨迹</param>
            public void PositionComparison2D(MotionControlPositionComparisonTriggerPoint[] PositionComparisonTriggerPoints, MotionInstructionBase[] motionTrajectoryList)
            {
                RunCoordinateMotion(
                    "PositionComparison2D",
                    axisGuids =>
                    {
                        motionControlCategoryA.PositionComparison2D(CoordinateSystemId, axisGuids, PositionComparisonTriggerPoints, motionTrajectoryList);
                    });
            }

            private void RunCoordinateMotion(string operationName, Action<Guid[]> executeMotion)
            {
                if (string.IsNullOrWhiteSpace(operationName))
                    throw new ArgumentException("operationName 不能为空", nameof(operationName));

                if (executeMotion == null)
                    throw new ArgumentNullException(nameof(executeMotion));

                int[] logicAxisList = AxisBindings.Select(a => a.LogicalAxis).Distinct().ToArray();
                if (logicAxisList.Length == 0)
                    return;

                foreach (var logicAxis in logicAxisList)
                {
                    if (!CheckPokaYoke(new int[] { logicAxis }))
                    {
                        throw new GKGException(MotionErrCodeConsts.ERR_MOTION_POKAYOKE_FAIL, MotionErr.RobotMoveFailed, MotionErr.PokaYokeCheckFailed);
                    }
                }

                lock (coordinateMotionMonitorLock)
                {
                    if (coordinateMotionMonitor != null)
                    {
                        throw new InvalidOperationException($"当前已有连续插补/位置比较运动在执行，无法启动 {operationName}，请等待其完成。");
                    }
                }

                StopMove = false;
                var axisGuidList = new List<Guid>();
                foreach (var logicAxis in logicAxisList)
                {
                    axisGuidList.Add(LockAxis(logicAxis));
                }

                CoordinateMotionMonitorState monitorState = new CoordinateMotionMonitorState
                {
                    LogicAxisList = logicAxisList
                };

                lock (coordinateMotionMonitorLock)
                {
                    coordinateMotionMonitor = monitorState;
                }

                _ = Task.Run(() =>
                {
                    try
                    {
                        executeMotion(axisGuidList.ToArray());
                    }
                    catch (Exception ex)
                    {
                        monitorState.ExecutionException = new Exception($"{operationName} 执行失败: {ex.Message}", ex);
                    }
                    finally
                    {
                        monitorState.ExecutionCompleted = true;
                        monitorState.TokenSource.Cancel();
                    }
                });

                _ = Task.Run(() =>
                {
                    bool moveStopped = false;
                    try
                    {
                        while (!monitorState.TokenSource.Token.IsCancellationRequested)
                        {
                            int rtn = motionControlCategoryA.WaitCrdMoveDone(CoordinateSystemId, 100);
                            UpdateBoundAxisPosition(monitorState.LogicAxisList);
                            UploadPokaYoke(monitorState.LogicAxisList);

                            if (rtn == 0)
                            {
                                moveStopped = true;
                                break;
                            }
                        }

                        UpdateBoundAxisPosition(monitorState.LogicAxisList);
                        UploadPokaYoke(monitorState.LogicAxisList);

                        if (monitorState.ExecutionException != null)
                        {
                            OnMoveFailed();
                        }
                        else if (moveStopped || monitorState.StopRequested || monitorState.ExecutionCompleted)
                        {
                            OnMoveFinished();
                        }
                    }
                    catch
                    {
                        OnMoveFailed();
                    }
                    finally
                    {
                        lock (coordinateMotionMonitorLock)
                        {
                            if (coordinateMotionMonitor == monitorState)
                            {
                                coordinateMotionMonitor = null;
                            }
                        }
                        monitorState.TokenSource.Dispose();

                        foreach (var logicAxis in monitorState.LogicAxisList)
                        {
                            UnLockAxis(logicAxis);
                        }
                    }
                }, monitorState.TokenSource.Token);
            }

            private void UpdateBoundAxisPosition(int[] logicAxisList)
            {
                List<AxisConstantValues> positionList = new List<AxisConstantValues>();
                foreach (var logicAxis in logicAxisList)
                {
                    if (!AxisBindingPairs.ContainsKey(logicAxis))
                        continue;

                    int physicalAxis = AxisBindingPairs[logicAxis].PhysicalAxis;
                    double position = motionControlCategoryA.GetAxisPos(physicalAxis, MotionControlAxisPositionType.Command);
                    positionList.Add(new AxisConstantValues
                    {
                        Axis = logicAxis,
                        PositionValue = position
                    });
                }

                if (positionList.Count > 0)
                {
                    OnPositionChanged(positionList.ToArray());
                }
            }

            /// <summary>
            /// 手动位置比较输出
            /// </summary>
            /// <param name="channel">通道</param>
            /// <param name="startLevel">起始电平</param>
            /// <param name="pulseOutputMode">脉冲输出模式(脉冲/电平)</param>
            /// <param name="triggerCount">触发次数</param>
            /// <param name="openTime">输出时间</param>
            /// <param name="closeTime">关闭时间</param>
            public void ManualPositionComparison(int[] channel, short startLevel, int pulseOutputMode, int triggerCount, double openTime, double closeTime)
            {
                // 锁定
                List<Guid> axisLockGuidList = new List<Guid>();
                foreach (var axisbind in AxisBindings)
                {
                    // 应该先查看运动轨迹里面用到的轴
                    axisLockGuidList.Add(LockAxis(axisbind.LogicalAxis));
                }
                motionControlCategoryA.ManualPositionComparison(axisLockGuidList.ToArray(), channel, startLevel, pulseOutputMode, triggerCount, openTime, closeTime);
                // 解锁
                foreach (var axisbind in AxisBindings)
                {
                    UnLockAxis(axisbind.LogicalAxis);
                }
            }

            /// <summary>
            /// 停止位置比较输出
            /// </summary>
            public void StopManualPositionComparison()
            {
                motionControlCategoryA.StopManualPositionComparison();
            }

            public override void SetPositionLatch(int logicAxis, MotionControlPositionLatchCaptureLogic positionLatchCaptureLogic, int channel, MotionControlPositionLatchSignalTriggerMode positionLatchSignalTriggerMode, short level, int triggerCount)
            {
                Guid guid = LockAxis(logicAxis);
                motionControlCategoryA.SetPositionLatch(guid, positionLatchCaptureLogic, channel, positionLatchSignalTriggerMode, level, triggerCount);
                UnLockAxis(logicAxis);
            }

            public override double[] GetPositionLatchResult(int logicAxis)
            {
                int physicalAxis = AxisBindings.First(ab => ab.LogicalAxis == logicAxis).PhysicalAxis;
                double[] result = motionControlCategoryA.GetPositionLatchResult(physicalAxis);
                return result;
            }

            public override void SetPositionLatchEnabled(int logicAxis, bool isEnabled)
            {
                LockAxis(logicAxis);
                motionControlCategoryA.SetPositionLatchEnabled(axisLockGuids[logicAxis], isEnabled);
                UnLockAxis(logicAxis);
            }
        }
    }
}