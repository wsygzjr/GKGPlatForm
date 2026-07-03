using GKG.ElectronicControl;
using GKG.PokaYoke;

namespace GKG
{
    namespace MotionControl
    {
        /// <summary>
        /// Robot 驱动基类。
        /// 这一层只抽公共驱动骨架：初始化、上下文持有、轴绑定、防呆列表、通用事件。
        /// 现有动作执行逻辑仍可保留在子类中逐步迁移。
        /// </summary>
        public abstract class RobotDriverBase : IRobotDriver
        {
            #region 受保护属性

            protected IMotionControlBase? motionControl { get; private set; }
            protected RobotInitParameters InitParameters { get; private set; } = new RobotInitParameters();
            protected RobotPlanContext PlanContext { get; private set; } = new RobotPlanContext();
            protected List<IPokaYokeBase> PokaYokeList { get; } = new List<IPokaYokeBase>();
            protected int AxisCount { get; set; } = 1;
            protected int CoordinateSystemId
            {
                get => PlanContext.CoordinateSystemId;
                set => PlanContext.CoordinateSystemId = value;
            }

            protected Dictionary<int, AxisBinding> AxisBindingPairs => PlanContext.AxisBindingPairs;
            protected AxisBinding[] AxisBindings { get; set; } = Array.Empty<AxisBinding>();

            #endregion

            #region 受保护字段

            protected Dictionary<int, Guid> axisLockGuids = new Dictionary<int, Guid>();
            protected bool StopMove = false;

            #endregion

            #region 公有属性

            public abstract string DriverName { get; }

            #endregion

            #region 公有事件

            public event EventHandler<PositionChangedEventArgs>? PositionChanged;
            public event EventHandler? MoveFinished;
            public event EventHandler? MoveFailed;

            #endregion

            #region 公有方法

            public virtual void Init(RobotInitParameters initParameters, IMotionControlBase? motionControlBase)
            {
                InitParameters = initParameters ?? new RobotInitParameters();
                if (string.IsNullOrWhiteSpace(InitParameters.DriverName))
                {
                    InitParameters.DriverName = RobotDriverNames.BasicRobot;
                }

                motionControl = motionControlBase;
                PlanContext.MotionControl = motionControlBase;
                //OnInit(InitParameters);
            }

            public abstract RobotCapabilities GetCapabilities();

            public virtual MotionInstructionBase[] AdaptMotionInstructions(
                MotionInstructionBase[] instructionSequence,
                RobotExecutionContext context)
            {
                return instructionSequence ?? Array.Empty<MotionInstructionBase>();
            }

            public abstract void Execute(
                MotionInstructionSequence sequence,
                RobotExecutionContext context);

            public abstract void AxisHome();

            public abstract void AxisHome(int logicAxis);

            public virtual void SetPositionLatch(int logicAxis, MotionControlPositionLatchCaptureLogic positionLatchCaptureLogic, int channel, MotionControlPositionLatchSignalTriggerMode positionLatchSignalTriggerMode, short level, int triggerCount)
            {
                throw new NotSupportedException($"Robot driver '{DriverName}' does not support position latch.");
            }

            public virtual void SetPositionLatchEnabled(int logicAxis, bool isEnabled)
            {
                throw new NotSupportedException($"Robot driver '{DriverName}' does not support position latch.");
            }

            public virtual double[] GetPositionLatchResult(int logicAxis)
            {
                throw new NotSupportedException($"Robot driver '{DriverName}' does not support position latch.");
            }

            public virtual void SetPlanContext(RobotPlanContext context)
            {
                PlanContext = context ?? new RobotPlanContext();
                if (PlanContext.MotionControl != null)
                {
                    motionControl = PlanContext.MotionControl;
                }
                CoordinateSystemId = PlanContext.CoordinateSystemId;
            }

            #endregion

            #region 受保护方法

            protected byte[] ResolveDriverInitParam()
            {
                if (InitParameters.DriverInitParameters != null && InitParameters.DriverInitParameters.Length > 0)
                {
                    return InitParameters.DriverInitParameters;
                }

                return Array.Empty<byte>();
            }

            protected abstract void OnInit(RobotInitParameters initParameters);

            protected virtual void OnPositionChanged(AxisConstantValues[] newPosition)
            {
                PositionChanged?.Invoke(this, new PositionChangedEventArgs(newPosition));
            }

            protected virtual void OnMoveFinished()
            {
                MoveFinished?.Invoke(this, EventArgs.Empty);
            }

            protected virtual void OnMoveFailed()
            {
                MoveFailed?.Invoke(this, EventArgs.Empty);
            }
            protected abstract void ExecuteInstruction(
                MotionInstructionBase instruction,
                RobotExecutionContext context);
            #endregion
        }
    }
}
