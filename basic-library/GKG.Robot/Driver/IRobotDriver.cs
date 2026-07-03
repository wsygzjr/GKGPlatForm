using GKG.ElectronicControl;

namespace GKG
{
    namespace MotionControl
    {
        /// <summary>
        /// 机器人驱动接口。
        /// 当前阶段以兼容现有 BasicRobot/CategoryARobot 的执行模型为主。
        /// </summary>
        public interface IRobotDriver : IRobotContextAware
        {
            /// <summary>
            /// 驱动名称。
            /// </summary>
            string DriverName { get; }

            /// <summary>
            /// 驱动初始化。
            /// 用一个接口同时接收驱动初始化参数与底层运控卡对象。
            /// </summary>
            void Init(RobotInitParameters initParameters, IMotionControlBase? motionControlBase);

            /// <summary>
            /// 获取能力描述。
            /// </summary>
            RobotCapabilities GetCapabilities();

            /// <summary>
            /// 位置变化事件。
            /// </summary>
            event EventHandler<PositionChangedEventArgs>? PositionChanged;

            /// <summary>
            /// 运动完成事件。
            /// </summary>
            event EventHandler? MoveFinished;

            /// <summary>
            /// 运动失败事件。
            /// </summary>
            event EventHandler? MoveFailed;

            /// <summary>
            /// 对中立运控指令序列做 Robot 层适配。
            /// 默认可直接原样返回，也可由具体 Robot 做功能头/机构/安全动作改写。
            /// </summary>
            MotionInstructionBase[] AdaptMotionInstructions(
                MotionInstructionBase[] instructionSequence,
                RobotExecutionContext context);

            /// <summary>
            /// 按显式序列类型执行运控指令序列。
            /// </summary>
            void Execute(
                MotionInstructionSequence sequence,
                RobotExecutionContext context);

            /// <summary>
            /// 所有轴回零。
            /// </summary>
            void AxisHome();

            /// <summary>
            /// 指定逻辑轴回零。
            /// </summary>
            /// <param name="logicAxis">逻辑轴号</param>
            void AxisHome(int logicAxis);

            /// <summary>
            /// 配置位置锁存。
            /// </summary>
            void SetPositionLatch(int logicAxis, MotionControlPositionLatchCaptureLogic positionLatchCaptureLogic, int channel, MotionControlPositionLatchSignalTriggerMode positionLatchSignalTriggerMode, short level, int triggerCount);

            /// <summary>
            /// 启用/禁用位置锁存。
            /// </summary>
            void SetPositionLatchEnabled(int logicAxis, bool isEnabled);

            /// <summary>
            /// 获取位置锁存结果。
            /// </summary>
            double[] GetPositionLatchResult(int logicAxis);
        }
    }
}
