using GKG.PokaYoke;
using System;
using GKG.ElectronicControl;
using System.Threading.Tasks;

namespace GKG
{
    namespace MotionControl
    {
        /// <summary>
        /// 基础机械手。
        /// 当前同时作为最基础的 RobotDriver 实现，后续可逐步下沉更多公共能力到独立基类。
        /// </summary>
        public class BasicRobot : RobotDriverBase
        {
            private sealed class ContinueMoveMonitorState
            {
                public CancellationTokenSource TokenSource { get; } = new CancellationTokenSource();
                public bool StopRequested { get; set; }
            }

            private readonly object continueMoveMonitorLock = new object();
            private readonly Dictionary<int, ContinueMoveMonitorState> continueMoveMonitors = new Dictionary<int, ContinueMoveMonitorState>();
            private Task continueMoveTask;
            /// <summary>
            /// 构造函数（测试版）
            /// </summary>
            public BasicRobot()
            {

            }

            public BasicRobot(int axisCount)
            {
                // 初始化机械手轴数量
                AxisCount = axisCount;
                // 运控卡实例
                //motionControl = new MotionControlGaoChAuto();
            }

            /// <summary>
            /// 驱动名称。
            /// </summary>
            public override string DriverName => RobotDriverNames.BasicRobot;

            /// <summary>
            /// 初始化。
            /// 当前基础机器人暂无额外参数，先保留扩展点。
            /// </summary>
            public override void Init(RobotInitParameters initParameters, IMotionControlBase? motionControlBase)
            {
                base.Init(initParameters, motionControlBase);
                OnInit(initParameters);
            }

            protected override void OnInit(RobotInitParameters initParameters)
            {
                BindAxesFromInit(initParameters);
            }

            private void BindAxesFromInit(RobotInitParameters initParameters)
            {
                if (initParameters?.AxisBindings == null || initParameters.AxisBindings.Length == 0)
                {
                    return;
                }
                AxisBindings = initParameters.AxisBindings;
                foreach (var axisBinding in initParameters.AxisBindings)
                {
                    Bind(axisBinding.MotionControlCardId, axisBinding.PhysicalAxis, axisBinding.LogicalAxis);
                }
            }

            /// <summary>
            /// 获取能力描述。
            /// </summary>
            public override RobotCapabilities GetCapabilities()
            {
                return new RobotCapabilities
                {
                    SupportsSingleAxisMove = true,
                    SupportsTwoAxisLinearInterpolation = true,
                    SupportsThreeAxisLinearInterpolation = true,
                    SupportsTwoAxisCircularInterpolation = true
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

                MotionInstructionBase[] adaptedInstructions = AdaptMotionInstructions(sequence.Instructions, safeContext);
                switch (sequence.SequenceType)
                {
                    case MotionInstructionSequenceType.StepByStep:
                    default:
                        foreach (var instruction in adaptedInstructions)
                        {
                            ExecuteInstruction(instruction, safeContext);
                        }
                        break;
                }
            }

            protected virtual void ApplyExecutionContext(RobotExecutionContext context)
            {
                CoordinateSystemId = context.CoordinateSystemId;

                if (context.AxisBindingPairs != null && context.AxisBindingPairs.Count > 0)
                {
                    foreach (var pair in context.AxisBindingPairs)
                    {
                        if (!AxisBindingPairs.ContainsKey(pair.Key))
                        {
                            AxisBindingPairs.Add(pair.Key, pair.Value);
                        }
                        else
                        {
                            AxisBindingPairs[pair.Key] = pair.Value;
                        }

                        if (!axisLockGuids.ContainsKey(pair.Key))
                        {
                            axisLockGuids.Add(pair.Key, Guid.Empty);
                        }
                    }
                }
            }

            protected override void ExecuteInstruction(
                MotionInstructionBase instruction,
                RobotExecutionContext context)
            {
                switch (instruction.InstructionType)
                {
                    case MotionInstructionType.Point:
                        ExecutePoint((Point)instruction);
                        break;

                    case MotionInstructionType.Linear:
                        ExecuteStraightLine((StraightLine)instruction);
                        break;

                    case MotionInstructionType.ArcA:
                        ExecuteArcA((ArcA)instruction);
                        break;

                    case MotionInstructionType.ArcB:
                        ExecuteArcB((ArcB)instruction);
                        break;

                    case MotionInstructionType.Circle:
                        ExecuteCircle((Circle)instruction);
                        break;

                    case MotionInstructionType.Delay:
                        ExecuteDelay((Delay)instruction);
                        break;

                    case MotionInstructionType.BufferIO:
                        ExecuteBufferIO((BufferIO)instruction);
                        break;

                    case MotionInstructionType.Buf2DComparePulseExElemData:
                        ExecuteBuffer2DComparePulse((Buffer2DComparePulse)instruction);
                        break;

                    case MotionInstructionType.ContinueMove:
                        ExecuteContinueMove((ContinueMoveInstruction)instruction);
                        break;

                    case MotionInstructionType.RelativeMove:
                        ExecuteRelativeMove((RelativeMoveInstruction)instruction);
                        break;

                    case MotionInstructionType.StopMove:
                        ExecuteStopContinueMove((StopMoveInstruction)instruction);
                        break;

                    case MotionInstructionType.PositionComparison2D:
                        ExecutePositionComparison2D((PositionComparison2DInstruction)instruction);
                        break;

                    default:
                        throw new NotSupportedException($"BasicRobot 暂不支持指令类型: {instruction.InstructionType}");
                }
            }

            protected virtual void ExecutePoint(Point instruction)
            {
                if (instruction.TargetPosition == null || instruction.TargetPosition.Length == 0)
                    throw new ArgumentException("Point.TargetPosition 不能为空");

                int[] logicAxis = instruction.TargetPosition.Select(p => p.Axis).ToArray();
                if (logicAxis.Length == 1)
                {
                    Move(logicAxis[0], instruction.TargetPosition[0].PositionValue, instruction.Speed, instruction.Acceleration);
                }
                else if (logicAxis.Length == 2)
                {
                    Move(logicAxis, new Point2D(
                        instruction.TargetPosition[0].PositionValue,
                        instruction.TargetPosition[1].PositionValue), instruction.Speed, instruction.Acceleration);
                }
                else if (logicAxis.Length == 3)
                {
                    Move(logicAxis, new Point3D(
                        instruction.TargetPosition[0].PositionValue,
                        instruction.TargetPosition[1].PositionValue,
                        instruction.TargetPosition[2].PositionValue), instruction.Speed, instruction.Acceleration);
                }
                else
                {
                    throw new NotSupportedException($"BasicRobot 暂不支持 {logicAxis.Length} 轴点位执行");
                }
            }

            protected virtual void ExecuteStraightLine(StraightLine instruction)
            {
                if (instruction.EndPosition == null || instruction.EndPosition.Length == 0)
                    throw new ArgumentException("StraightLine.EndPosition 不能为空");

                int[] logicAxis = instruction.EndPosition.Select(p => p.Axis).ToArray();
                if (logicAxis.Length == 2)
                {
                    Move(logicAxis, new Point2D(
                        instruction.EndPosition[0].PositionValue,
                        instruction.EndPosition[1].PositionValue), instruction.Speed, instruction.Acceleration);
                }
                else if (logicAxis.Length == 3)
                {
                    Move(logicAxis, new Point3D(
                        instruction.EndPosition[0].PositionValue,
                        instruction.EndPosition[1].PositionValue,
                        instruction.EndPosition[2].PositionValue), instruction.Speed, instruction.Acceleration);
                }
                else
                {
                    throw new NotSupportedException($"BasicRobot 仅支持 2/3 轴直线执行，当前: {logicAxis.Length}");
                }
            }

            protected virtual void ExecuteArcA(ArcA instruction)
            {
                if (instruction.StartPosition == null || instruction.MiddlePosition == null || instruction.EndPosition == null)
                    throw new ArgumentException("ArcA.StartPosition / MiddlePosition / EndPosition 不能为空");

                if (instruction.StartPosition.Length < 2 || instruction.MiddlePosition.Length < 2 || instruction.EndPosition.Length < 2)
                    throw new NotSupportedException("BasicRobot 仅支持二维 ArcA 执行");

                int[] logicAxis = new int[]
                {
                    instruction.EndPosition[0].Axis,
                    instruction.EndPosition[1].Axis
                };

                var start = new Point2D(
                    instruction.StartPosition[0].PositionValue,
                    instruction.StartPosition[1].PositionValue);
                var middle = new Point2D(
                    instruction.MiddlePosition[0].PositionValue,
                    instruction.MiddlePosition[1].PositionValue);
                var end = new Point2D(
                    instruction.EndPosition[0].PositionValue,
                    instruction.EndPosition[1].PositionValue);

                Point2D center = CalculateCircleCenter(start, middle, end);
                int direction = ResolveArcDirection(start, middle, end);
                ExecuteCircularInterpolation(logicAxis, start, center, end, direction, instruction.Speed, instruction.Acceleration);
            }

            protected virtual void ExecuteArcB(ArcB instruction)
            {
                if (instruction.StartPosition == null || instruction.CenterPosition == null || instruction.EndPosition == null)
                    throw new ArgumentException("ArcB.StartPosition / CenterPosition / EndPosition 不能为空");

                if (instruction.StartPosition.Length < 2 || instruction.CenterPosition.Length < 2 || instruction.EndPosition.Length < 2)
                    throw new NotSupportedException("BasicRobot 仅支持二维 ArcB 执行");

                int[] logicAxis = new int[]
                {
                    instruction.EndPosition[0].Axis,
                    instruction.EndPosition[1].Axis
                };

                var start = new Point2D(
                    instruction.StartPosition[0].PositionValue,
                    instruction.StartPosition[1].PositionValue);
                var center = new Point2D(
                    instruction.CenterPosition[0].PositionValue,
                    instruction.CenterPosition[1].PositionValue);
                var end = new Point2D(
                    instruction.EndPosition[0].PositionValue,
                    instruction.EndPosition[1].PositionValue);

                int direction = ResolveArcDirectionByCenter(start, center, end);
                ExecuteCircularInterpolation(logicAxis, start, center, end, direction, instruction.Speed, instruction.Acceleration);
            }

            protected virtual void ExecuteCircle(Circle instruction)
            {
                if (instruction.MiddlePosition == null || instruction.EndPosition == null)
                    throw new ArgumentException("Circle.MiddlePosition / EndPosition 不能为空");

                if (instruction.MiddlePosition.Length < 2 || instruction.EndPosition.Length < 2)
                    throw new NotSupportedException("BasicRobot 仅支持二维 Circle 执行");

                int[] logicAxis = new int[]
                {
                    instruction.EndPosition[0].Axis,
                    instruction.EndPosition[1].Axis
                };

                double centerX = instruction.MiddlePosition[0].PositionValue;
                double centerY = instruction.MiddlePosition[1].PositionValue;
                double endX = instruction.EndPosition[0].PositionValue;
                double endY = instruction.EndPosition[1].PositionValue;
                double radius = Math.Sqrt(Math.Pow(endX - centerX, 2) + Math.Pow(endY - centerY, 2));
                MoveCircle(logicAxis, new Point2D(centerX, centerY), radius, instruction.Speed, instruction.Acceleration);
            }

            protected virtual void ExecuteCircularInterpolation(
                int[] logicAxis,
                Point2D start,
                Point2D center,
                Point2D end,
                int direction,
                double speed,
                double acceleration)
            {
                if (logicAxis.Length != 2)
                    throw new ArgumentException("二维圆弧插补要求 logicAxis 长度为 2");

                if (!CheckPokaYoke(logicAxis))
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_POKAYOKE_FAIL, MotionErr.RobotMoveFailed, MotionErr.PokaYokeCheckFailed);
                }

                StopMove = false;
                Guid guidX = LockAxis(logicAxis[0]);
                Guid guidY = LockAxis(logicAxis[1]);
                try
                {
                    double accTime = speed / acceleration;
                    double[] endPos = new double[] { end.X, end.Y };
                    double[] centerOffset = new double[] { center.X - start.X, center.Y - start.Y };
                    int rtn = motionControl.TwoAxisCircularInterpolation(
                        new Guid[] { guidX, guidY },
                        CoordinateSystemId,
                        endPos,
                        centerOffset,
                        direction,
                        0,
                        0,
                        speed,
                        accTime,
                        accTime,
                        accTime,
                        accTime);
                    if (rtn != 0)
                    {
                        throw new Exception($"二维圆弧插补执行失败，返回码:{rtn}");
                    }
                }
                finally
                {
                    UnLockAxis(logicAxis[0]);
                    UnLockAxis(logicAxis[1]);
                }

                UpdateAllAxisPosition();
                OnMoveFinished();
            }

            protected virtual Point2D CalculateCircleCenter(Point2D start, Point2D middle, Point2D end)
            {
                double x1 = start.X, y1 = start.Y;
                double x2 = middle.X, y2 = middle.Y;
                double x3 = end.X, y3 = end.Y;

                double d = 2 * (x1 * (y2 - y3) + x2 * (y3 - y1) + x3 * (y1 - y2));
                if (Math.Abs(d) < 1e-9)
                    throw new ArgumentException("ArcA 三点共线，无法确定圆心");

                double ux = ((x1 * x1 + y1 * y1) * (y2 - y3) +
                             (x2 * x2 + y2 * y2) * (y3 - y1) +
                             (x3 * x3 + y3 * y3) * (y1 - y2)) / d;

                double uy = ((x1 * x1 + y1 * y1) * (x3 - x2) +
                             (x2 * x2 + y2 * y2) * (x1 - x3) +
                             (x3 * x3 + y3 * y3) * (x2 - x1)) / d;

                return new Point2D(ux, uy);
            }

            protected virtual int ResolveArcDirection(Point2D start, Point2D middle, Point2D end)
            {
                double cross = (middle.X - start.X) * (end.Y - middle.Y) - (middle.Y - start.Y) * (end.X - middle.X);
                return cross < 0
                    ? MotionControlArcDirectionConstants.Clockwise
                    : MotionControlArcDirectionConstants.CounterClockwise;
            }

            protected virtual int ResolveArcDirectionByCenter(Point2D start, Point2D center, Point2D end)
            {
                double sx = start.X - center.X;
                double sy = start.Y - center.Y;
                double ex = end.X - center.X;
                double ey = end.Y - center.Y;
                double cross = sx * ey - sy * ex;
                return cross < 0
                    ? MotionControlArcDirectionConstants.Clockwise
                    : MotionControlArcDirectionConstants.CounterClockwise;
            }

            protected virtual void ExecuteDelay(Delay instruction)
            {
                if (instruction.Duration > 0)
                {
                    Thread.Sleep(instruction.Duration);
                }
            }

            protected virtual void ExecuteBufferIO(BufferIO instruction)
            {
                // 当前基础 Robot 暂不直接下发 BufferIO 到底层卡接口。
                // 后续可在 Robot->MotionControl 编译/执行层补成明确的数字量输出协议。
            }

            protected virtual void ExecuteBuffer2DComparePulse(Buffer2DComparePulse instruction)
            {
                throw new NotSupportedException("BasicRobot 不支持 2D 位置比较脉冲输出，请使用 CategoryARobot 或其他高级 Robot 驱动。");
            }

            protected virtual void ExecuteContinueMove(ContinueMoveInstruction instruction)
            {
                if (instruction == null)
                    throw new ArgumentException("ContinueMoveInstruction.MoveParam 不能为空");

                ContinueMove(instruction);
            }

            protected virtual void ExecuteRelativeMove(RelativeMoveInstruction instruction)
            {
                if (instruction == null)
                    throw new ArgumentException("RelativeMoveInstruction.MoveParam 不能为空");

                RelativeMove(instruction);
            }

            protected virtual void ExecuteStopContinueMove(StopMoveInstruction instruction)
            {
                StopContinueMove(instruction.LogicAxis);
            }

            protected virtual void ExecutePositionComparison2D(PositionComparison2DInstruction instruction)
            {
                throw new NotSupportedException("BasicRobot 不支持 PositionComparison2DInstruction，请使用 CategoryARobot 或其他高级 Robot 驱动。");
            }

            /// <summary>
            /// 检查机械手是否可以移动
            /// </summary>
            /// <returns>是否可移动</returns>
            public bool CanCallMove()
            {
                foreach (var binding in AxisBindingPairs.Values)
                {
                    // 检查防呆
                    foreach (var pokaYoke in PokaYokeList)
                    {
                        if (!pokaYoke.CheckCanMove(binding.LogicalAxis))
                        {
                            return false;
                        }
                    }
                    var axisStatus = motionControl?.GetAxisState(binding.PhysicalAxis, MotionControlAxisStatus.PositiveLimit);
                    axisStatus = motionControl?.GetAxisState(binding.PhysicalAxis, MotionControlAxisStatus.Alarm);
                    if (axisStatus == true)
                    {
                        // 轴报警
                        return false;
                    }
                    axisStatus = motionControl?.GetAxisState(binding.PhysicalAxis, MotionControlAxisStatus.ServoEnable);
                    if (axisStatus == false)
                    {
                        // 轴未使能
                        return false;
                    }
                }
                return true;
            }
       
            /// <summary>
            /// 检查防呆
            /// </summary>
            /// <param name="logicAxis">逻辑轴号</param>
            /// <returns></returns>
            protected bool CheckPokaYoke(int[] logicAxis)
            {
                // 逻辑轴转换为物理轴
                List<int> axisList = new List<int>();
                foreach(var axis in logicAxis)
                {
                    if(AxisBindingPairs.ContainsKey(axis))
                    {
                        axisList.Add(AxisBindingPairs[axis].PhysicalAxis);
                    }
                    else
                    {
                        throw new GKGException(MotionErrCodeConsts.ERR_MOTION_AXISBINDING_NOTEXIST, MotionErr.AxisBindingPairsNotExist, MotionErr.AxisBindingPairsNotExist);
                    }
                }
                // 检查防呆
                foreach (var axis in axisList)
                {
                    foreach (var pokaYoke in PokaYokeList)
                    {
                        if (!pokaYoke.CheckCanMove(axis))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            protected void UploadPokaYoke(int[] logicAxis)
            {
                // 逻辑轴转换为物理轴
                List<int> axisList = new List<int>();
                foreach (var axis in logicAxis)
                {
                    if (AxisBindingPairs.ContainsKey(axis))
                    {
                        axisList.Add(AxisBindingPairs[axis].PhysicalAxis);
                    }
                    else
                    {
                        throw new GKGException(MotionErrCodeConsts.ERR_MOTION_AXISBINDING_NOTEXIST, MotionErr.AxisBindingPairsNotExist, MotionErr.AxisBindingPairsNotExist);
                    }
                }

                // 位置上报防呆
                foreach (var physicalAxis in axisList)
                {
                    foreach (var pokaYoke in PokaYokeList)
                    {
                        double position = motionControl.GetAxisPos(physicalAxis, MotionControlAxisPositionType.Command);
                        pokaYoke.UpLoadAxisPosition(physicalAxis, position);
                    }
                }
            }

            /// <summary>
            /// 绑定物理轴和逻辑轴
            /// </summary>
            /// <param name="motionCardGuid"></param>
            /// <param name="physicalAxis"></param>
            /// <param name="logicalAxis"></param>
            public void Bind(Guid motionCardGuid, int physicalAxis, int logicalAxis)
            {
                if (!AxisBindingPairs.ContainsKey(logicalAxis))
                {
                    AxisBindingPairs.Add(logicalAxis, new AxisBinding(motionCardGuid, logicalAxis, physicalAxis));
                }
                else
                {
                    AxisBindingPairs[logicalAxis] = new AxisBinding(motionCardGuid, logicalAxis, physicalAxis);
                }

                if (!axisLockGuids.ContainsKey(logicalAxis))
                {
                    axisLockGuids.Add(logicalAxis, Guid.Empty);
                }
                else
                {
                    axisLockGuids[logicalAxis] = Guid.Empty;
                }
            }

            /// <summary>
            /// 机械手所有轴回零
            /// </summary>
            public override void AxisHome()
            {
                List<Guid> axisGuidList = new List<Guid>();
                foreach (var axisBinding in AxisBindingPairs.Values)
                {
                    Guid guidAxis = LockAxis(axisBinding.LogicalAxis);
                    axisGuidList.Add(guidAxis);
                    motionControl.AxisHome(guidAxis);
                }
                foreach (var axisGuid in axisGuidList)
                {
                    motionControl.WaitAxisStop(axisGuid, 60000);
                    motionControl.UnLockAxis(axisGuid);
                }
            }

            /// <summary>
            /// 机械手单轴回零
            /// </summary>
            /// <param name="logicAxis">逻辑轴号</param>
            public override void AxisHome(int logicAxis)
            {
                Guid guidAxis = LockAxis(logicAxis);
                motionControl.AxisHome(guidAxis);
                motionControl.WaitAxisStop(guidAxis, 60000);
                motionControl.UnLockAxis(guidAxis);
            }

            private Task WaitOneAxisMoveDoneAsync(Guid guid, int logicAxis, double targetPosition)
            {
                return WaitAxisMoveDoneAsync(
                    guid,
                    new int[] { logicAxis },
                    () => motionControl.GetAxisPos(guid, MotionControlAxisPositionType.Command) == targetPosition);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="logicAxis">逻辑轴</param>
            /// <param name="targetPosition">目标位置</param>
            /// <param name="speed">运动速度</param>
            /// <param name="acc">运动加速度</param>
            public Task BeginMove(int logicAxis, double targetPosition, double speed, double acc)
            {
                if (!CheckPokaYoke(new int[] { logicAxis }))
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_POKAYOKE_FAIL, MotionErr.RobotMoveFailed, MotionErr.PokaYokeCheckFailed);
                }

                StopMove = false;
                double accTime = speed / acc;
                Guid guid = LockAxis(logicAxis);
                try
                {
                    motionControl.AbsoluteMove(guid, 0, targetPosition, 0, speed, accTime, accTime, accTime, accTime);
                }
                catch
                {
                    OnMoveFailed();
                    StopMove = true;
                    UnLockAxis(logicAxis);
                    throw;
                }

                return WaitOneAxisMoveDoneAsync(guid, logicAxis, targetPosition);
            }

            private Task WaitTwoAxisMoveDoneAsync(Guid[] guids, int[] logicAxis, Point2D targetPosition)
            {
                return WaitCoordinateMoveDoneAsync(
                    guids,
                    logicAxis,
                    () => motionControl.GetAxisPos(guids[0], MotionControlAxisPositionType.Command) == targetPosition.X
                       && motionControl.GetAxisPos(guids[1], MotionControlAxisPositionType.Command) == targetPosition.Y);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="logicAxis">逻辑轴列表</param>
            /// <param name="targetPosition">目标位置</param>
            /// <param name="speed">运动速度</param>
            /// <param name="acc">运动加速度</param>
            public Task BeginMove(int[] logicAxis, Point2D targetPosition, double speed, double acc)
            {
                if (logicAxis.Length != 2)
                {
                    throw new GKGException(-10001, MotionErr.RobotMoveFailed, MotionErr.AxisCountErr);
                }
                if (!CheckPokaYoke(logicAxis))
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_POKAYOKE_FAIL, MotionErr.RobotMoveFailed, MotionErr.PokaYokeCheckFailed);
                }

                StopMove = false;
                Guid guidX = LockAxis(logicAxis[0]);
                Guid guidY = LockAxis(logicAxis[1]);
                double accTime = speed / acc;
                try
                {
                    motionControl.TwoAxisLinearInterpolation(new Guid[] { guidX, guidY }, CoordinateSystemId, new double[] { targetPosition.X, targetPosition.Y }, 0, 0, speed, accTime, accTime, accTime, accTime);
                }
                catch
                {
                    OnMoveFailed();
                    StopMove = true;
                    UnLockAxis(logicAxis[0]);
                    UnLockAxis(logicAxis[1]);
                    throw;
                }

                return WaitTwoAxisMoveDoneAsync(new Guid[] { guidX, guidY }, logicAxis, targetPosition);
            }

            private Task WaitThreeAxisMoveDoneAsync(Guid[] guids, int[] logicAxis, Point3D targetPosition)
            {
                return WaitCoordinateMoveDoneAsync(
                    guids,
                    logicAxis,
                    () => motionControl.GetAxisPos(guids[0], MotionControlAxisPositionType.Command) == targetPosition.X
                       && motionControl.GetAxisPos(guids[1], MotionControlAxisPositionType.Command) == targetPosition.Y
                       && motionControl.GetAxisPos(guids[2], MotionControlAxisPositionType.Command) == targetPosition.Z);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="logicAxis">逻辑轴列表</param>
            /// <param name="targetPosition">目标位置</param>
            /// <param name="speed">运动速度</param>
            /// <param name="acc">运动加速度</param>
            public Task BeginMove(int[] logicAxis, Point3D targetPosition, double speed, double acc)
            {
                if (logicAxis.Length != 3)
                {
                    throw new GKGException(-10001, MotionErr.RobotMoveFailed, MotionErr.AxisCountErr);
                }
                if (!CheckPokaYoke(logicAxis))
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_POKAYOKE_FAIL, MotionErr.RobotMoveFailed, MotionErr.PokaYokeCheckFailed);
                }
                StopMove = false;
                Guid guidX = LockAxis(logicAxis[0]);
                Guid guidY = LockAxis(logicAxis[1]);
                Guid guidZ = LockAxis(logicAxis[2]);
                double accTime = speed / acc;

                try
                {
                    motionControl.ThreeAxisLinearInterpolation(new Guid[] { guidX, guidY, guidZ }, CoordinateSystemId, new double[] { targetPosition.X, targetPosition.Y, targetPosition.Z }, 0, 0, speed, accTime, accTime, accTime, accTime);
                }
                catch
                {
                    OnMoveFailed();
                    StopMove = true;
                    UnLockAxis(logicAxis[0]);
                    UnLockAxis(logicAxis[1]);
                    UnLockAxis(logicAxis[2]);
                    throw;
                }
                return WaitThreeAxisMoveDoneAsync(new Guid[] { guidX, guidY, guidZ }, logicAxis, targetPosition);
            }

            public void Move(int logicAxis, double targetPosition, double speed, double acc)
            {
                BeginMove(logicAxis, targetPosition, speed, acc).GetAwaiter().GetResult();
            }

            public void Move(int[] logicAxis, Point2D targetPosition, double speed, double acc)
            {
                BeginMove(logicAxis, targetPosition, speed, acc).GetAwaiter().GetResult();
            }

            public void Move(int[] logicAxis, Point3D targetPosition, double speed, double acc)
            {
                BeginMove(logicAxis, targetPosition, speed, acc).GetAwaiter().GetResult();
            }

            public void MoveCircle(int[] logicAxis, Point2D center, double radius, double speed, double acc)
            {
                double startX = center.X;
                double startY = center.Y - radius;
                Point2D start = new Point2D(startX, startY);
                Point2D end = start;
                ExecuteCircularInterpolation(logicAxis, start, center, end, MotionControlArcDirectionConstants.Clockwise, speed, acc);
            }

            /// <summary>
            /// 运动
            /// </summary>
            /// <param name="trajectory">运动轨迹</param>
            public void Move(MoveParam moveParam)
            {
                switch (moveParam.AxisCount)
                {
                    case 1:
                        {
                            BeginMove(moveParam.logicAxis[0], moveParam.targetPosition.X, moveParam.speed, moveParam.acc);
                        }
                        break;
                    case 2:
                        {
                            BeginMove(moveParam.logicAxis, new Point2D(moveParam.targetPosition.X, moveParam.targetPosition.Y), moveParam.speed, moveParam.acc);
                        }
                        break;
                    case 3:
                        {
                            BeginMove(moveParam.logicAxis, moveParam.targetPosition, moveParam.speed, moveParam.acc);
                        }
                        break;
                }
            }

            private Task WaitAxisMoveDoneAsync(Guid guid, int[] logicAxis, Func<bool> successPredicate)
            {
                return Task.Run(() =>
                {
                    try
                    {
                        int rtn = 0;
                        while (true)
                        {
                            if (StopMove)
                            {
                                break;
                            }

                            rtn = motionControl.WaitAxisStop(guid, 100);
                            UpdateAllAxisPosition();
                            UploadPokaYoke(logicAxis);

                            if (rtn == 0)
                            {
                                break;
                            }
                        }

                        if (!StopMove && !successPredicate())
                        {
                            OnMoveFailed();
                        }
                        else
                        {
                            OnMoveFinished();
                        }

                        UpdateAllAxisPosition();
                    }
                    finally
                    {
                        foreach (var axis in logicAxis)
                        {
                            UnLockAxis(axis);
                        }
                    }
                });
            }

            private Task WaitCoordinateMoveDoneAsync(Guid[] guids, int[] logicAxis, Func<bool> successPredicate)
            {
                return Task.Run(() =>
                {
                    try
                    {
                        int rtn = 0;
                        while (true)
                        {
                            if (StopMove)
                            {
                                break;
                            }

                            rtn = motionControl.WaitCrdMoveDone(CoordinateSystemId, 100);
                            UpdateAllAxisPosition();
                            UploadPokaYoke(logicAxis);

                            if (rtn == 0)
                            {
                                break;
                            }
                        }

                        if (!StopMove && !successPredicate())
                        {
                            OnMoveFailed();
                        }
                        else
                        {
                            OnMoveFinished();
                        }

                        UpdateAllAxisPosition();
                    }
                    finally
                    {
                        foreach (var axis in logicAxis)
                        {
                            UnLockAxis(axis);
                        }
                    }
                });
            }

            /// <summary>
            /// 轴锁定
            /// </summary>
            /// <param name="logicAxis">逻辑轴号</param>
            /// <returns>轴锁定句柄</returns>
            protected Guid LockAxis(int logicAxis)
            {
                if (axisLockGuids == null)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_AXISLOCK_FAIL, MotionErr.AxisLockGuidNull, MotionErr.AxisLockGuidNull);
                }
                if (AxisBindingPairs == null)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_AXISLOCK_FAIL, MotionErr.AxisBindingPairsNull, MotionErr.AxisBindingPairsNull);
                }
                if(!axisLockGuids.ContainsKey(logicAxis))
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_AXISLOCK_FAIL, MotionErr.AxisLockGuidNotExist, MotionErr.AxisLockGuidNotExist);
                }
                if (!AxisBindingPairs.ContainsKey(logicAxis))
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_AXISLOCK_FAIL, MotionErr.AxisBindingPairsNotExist, MotionErr.AxisBindingPairsNotExist);
                }
                // 机械手运动
                int physicalAxis = AxisBindingPairs[logicAxis].PhysicalAxis;
                // 调用运控接口运动
                axisLockGuids[logicAxis] = motionControl.LockAxis(physicalAxis, 1000);
                return axisLockGuids[logicAxis];
            }

            /// <summary>
            /// 轴解锁
            /// </summary>
            /// <param name="logicAxis">逻辑轴号</param>
            protected void UnLockAxis(int logicAxis)
            {
                if (axisLockGuids == null)
                {
                    //throw
                }
                if (!axisLockGuids.ContainsKey(logicAxis))
                {
                    return;
                }
                if(axisLockGuids[logicAxis] != Guid.Empty)
                {
                    motionControl.UnLockAxis(axisLockGuids[logicAxis]);
                    axisLockGuids[logicAxis] = Guid.Empty;
                }
            }

            /// <summary>
            /// 终止运动
            /// </summary>
            public void Terminate()
            {
                StopMove = true;
                // 终止机械手
                foreach (var axisGuid in axisLockGuids.Values)
                {
                    motionControl?.AxisStop(axisGuid, MotionControlAxisStopTypeConstants.ImmediateStop);
                }
            }

            /// <summary>
            /// 连续运动
            /// </summary>
            /// <param name="moveParam"></param>
            /// <exception cref="Exception"></exception>
            public void ContinueMove(ContinueMoveInstruction continueMoveInstruction)
            {
                if (continueMoveInstruction == null)
                    throw new Exception();

                StopMove = false;
                double speed = continueMoveInstruction.Speed;
                double accTime = Math.Abs(continueMoveInstruction.Speed / continueMoveInstruction.Acceleration);
                int logicAxis = continueMoveInstruction.LogicAxis;

                lock (continueMoveMonitorLock)
                {
                    if (continueMoveMonitors.ContainsKey(logicAxis))
                    {
                        throw new InvalidOperationException($"逻辑轴 {logicAxis} 已处于连续运动中，请先调用 StopContinueMove。");
                    }
                }

                Guid guid = LockAxis(logicAxis);
                ContinueMoveMonitorState monitorState = new ContinueMoveMonitorState();

                lock (continueMoveMonitorLock)
                {
                    continueMoveMonitors[logicAxis] = monitorState;
                }

                try
                {
                    motionControl.VelocityMove(guid, 0, 1, speed, accTime, accTime, accTime, accTime);
                    axisLockGuids[logicAxis] = guid;
                }
                catch
                {
                    lock (continueMoveMonitorLock)
                    {
                        if (continueMoveMonitors.TryGetValue(logicAxis, out var existingMonitor) && existingMonitor == monitorState)
                        {
                            continueMoveMonitors.Remove(logicAxis);
                        }
                    }
                    monitorState.TokenSource.Dispose();
                    OnMoveFailed();
                    StopMove = true;
                    UnLockAxis(logicAxis);
                    throw;
                }

                continueMoveTask = Task.Run(() =>
                {
                    bool moveStopped = false;
                    try
                    {
                        while (!monitorState.TokenSource.Token.IsCancellationRequested)
                        {
                            int rtn = motionControl.WaitAxisStop(guid, 100);
                            UpdateAllAxisPosition();
                            UploadPokaYoke(new int[] { logicAxis });

                            if (rtn == 0)
                            {
                                moveStopped = true;
                                break;
                            }
                        }

                        UpdateAllAxisPosition();
                        UploadPokaYoke(new int[] { logicAxis });

                        if (moveStopped || monitorState.StopRequested)
                        {
                            OnMoveFinished();
                        }
                    }
                    catch
                    {
                        OnMoveFailed();
                        throw;
                    }
                    finally
                    {
                        int rtn = motionControl.AxisStop(guid, MotionControlAxisStopTypeConstants.ImmediateStop);
                        lock (continueMoveMonitorLock)
                        {
                            if (continueMoveMonitors.TryGetValue(logicAxis, out var existingMonitor) && existingMonitor == monitorState)
                            {
                                continueMoveMonitors.Remove(logicAxis);
                            }
                        }
                        monitorState.TokenSource.Dispose();
                        UnLockAxis(logicAxis);
                    }
                }, monitorState.TokenSource.Token);
            }
            
            /// <summary>
            /// 停止连续运动
            /// </summary>
            /// <param name="logicAxis"></param>
            public void StopContinueMove(int logicAxis)
            {
                if (!axisLockGuids.ContainsKey(logicAxis))
                    return;

                ContinueMoveMonitorState? monitorState = null;
                lock (continueMoveMonitorLock)
                {
                    continueMoveMonitors.TryGetValue(logicAxis, out monitorState);
                    if (monitorState != null)
                    {
                        monitorState.StopRequested = true;
                    }
                }

                Guid guid = axisLockGuids[logicAxis];
                try
                {
                    int rtn = motionControl.AxisStop(guid, MotionControlAxisStopTypeConstants.ImmediateStop);
                    if (rtn != 0)
                    {
                        OnMoveFailed();
                        return;
                    }

                    motionControl.WaitAxisStop(guid, 1000);

                    if (monitorState != null)
                    {
                        lock (continueMoveMonitorLock)
                        {
                            if (continueMoveMonitors.TryGetValue(logicAxis, out var existingMonitor) && existingMonitor == monitorState)
                            {
                                continueMoveMonitors.Remove(logicAxis);
                            }
                        }
                        monitorState.TokenSource.Cancel();
                    }
                    else
                    {
                        UpdateAllAxisPosition();
                        OnMoveFinished();
                        UnLockAxis(logicAxis);
                    }
                    if (continueMoveTask != null)
                    {
                        continueMoveTask.Wait(10000);
                    }
                }
                catch
                {
                    OnMoveFailed();
                    throw;
                }
                finally
                {
                    continueMoveTask = null;
                }
            }

            /// <summary>
            /// 相对运动
            /// </summary>
            /// <param name="moveParam"></param>
            /// <exception cref="Exception"></exception>
            public void RelativeMove(RelativeMoveInstruction relativeMoveInstruction)
            {
                if (relativeMoveInstruction == null)
                    throw new Exception();

                double speed = relativeMoveInstruction.Speed;
                double accTime = Math.Abs(relativeMoveInstruction.Speed / relativeMoveInstruction.Acceleration);
                int logicAxis = relativeMoveInstruction.Distance.Axis;
                double pos = relativeMoveInstruction.Distance.PositionValue;
                Guid guid = LockAxis(logicAxis);
                double curPos = motionControl.GetAxisPos(guid, MotionControlAxisPositionType.Command);

                try
                {
                    motionControl.RelativeMove(guid, 0, pos, 0, speed, accTime, accTime, accTime, accTime);
                }
                catch
                {
                    OnMoveFailed();
                    StopMove = true;
                    UnLockAxis(logicAxis);
                    throw;
                }

                _ = WaitAxisMoveDoneAsync(
                    guid,
                    new int[] { logicAxis },
                    () => Math.Abs(motionControl.GetAxisPos(guid, MotionControlAxisPositionType.Command) - curPos) == pos);
            }

            /// <summary>
            /// 获取绑定信息内所有轴的位置信息
            /// </summary>
            private void UpdateAllAxisPosition()
            {
                List<AxisConstantValues> positionList = new List<AxisConstantValues>();
                foreach (var axisBinding in AxisBindingPairs.Values)
                {
                    double position = motionControl.GetAxisPos(axisBinding.PhysicalAxis, MotionControlAxisPositionType.Command);
                    positionList.Add(new AxisConstantValues
                    {
                        Axis = axisBinding.LogicalAxis,
                        PositionValue = position
                    });
                }
                OnPositionChanged(positionList.ToArray());
            }
        }
    }
}
