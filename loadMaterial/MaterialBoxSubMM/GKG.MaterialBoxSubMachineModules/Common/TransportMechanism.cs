using GKG.MotionControl;
using GKG.SubMM.TransportMechanismModule;
using System;

namespace GKG.MaterialBoxSubMachineModules.Common
{
    /// <summary>
    /// 运输机构运行时对象
    /// </summary>
    public class TransportMechanism
    {
        /// <summary>
        /// 出厂参数
        /// </summary>
        private readonly TransportMechanismFactoryCfg factoryCfg;

        /// <summary>
        /// 初始化参数
        /// </summary>
        private TransportMechanismInitCfg initCfg;

        /// <summary>
        /// 当前生效配方
        /// </summary>
        private TransportMechanismPPCfg ppCfg;

        /// <summary>
        /// 当前运输位置缓存
        /// </summary>
        private double currentTransportPosition;

        public event EventHandler<PositionChangedEventArgs> PositionChanged;

        public event EventHandler MoveFinished;

        public event EventHandler MoveFailed;

        private IRobotDriver robotDriver;

        /// <summary>
        /// 构造运输机构运行时对象
        /// </summary>
        public TransportMechanism(TransportMechanismFactoryCfg factoryCfg)
        {
            this.factoryCfg = factoryCfg ?? new TransportMechanismFactoryCfg();
            initCfg = new TransportMechanismInitCfg();
            ppCfg = new TransportMechanismPPCfg();
        }

        /// <summary>
        /// 应用初始化参数
        /// </summary>
        public void Init(TransportMechanismInitCfg initCfg, Func<Guid, IRobotDriver> getRobotByIDs)
        {
            this.initCfg = initCfg ?? new TransportMechanismInitCfg();
            robotDriver = getRobotByIDs?.Invoke(initCfg.BindingAxisId);
            if (robotDriver != null)
            {
                robotDriver.MoveFailed += InvokeMoveFailedEvent;
                robotDriver.MoveFinished += InvokeMoveFinishedEvent;
                robotDriver.PositionChanged += InvokePositionChangedEvent;
            }
            else
            {
                throw new InvalidOperationException($"无法获取ID为{initCfg.BindingAxisId}的机器人驱动");
            }
        }

        /// <summary>
        /// 应用配方参数
        /// </summary>
        public void ApplyRecipe(TransportMechanismPPCfg ppCfg)
        {
            this.ppCfg = ppCfg ?? new TransportMechanismPPCfg();
        }

        /// <summary>
        /// 获取当前生效的运输运动参数
        /// </summary>
        public TransportMechanismPPCfg GetCurrentMotionParameters()
        {
            return ppCfg ?? new TransportMechanismPPCfg();
        }

        /// <summary>
        /// 读取当前运输轴位置缓存
        /// </summary>
        public double GetCurrentAxisPosition()
        {
            return currentTransportPosition;
        }

        /// <summary>
        /// 运输机构标准移动接口，语义与界面方法保持一致
        /// </summary>
        public void TransportMove(double recPos)
        {
            robotDriver.Execute(new MotionInstructionSequence
            {
                 SequenceType = MotionInstructionSequenceType.StepByStep,
                 Instructions = new MotionInstructionBase[]
                 {
                     new Point
                     {
                         TargetPosition = new AxisConstantValues[]
                         {
                             new AxisConstantValues
                             {
                                 Axis = 0,
                                 PositionValue = recPos
                             }
                         },
                         Speed = ppCfg.AxisMotionParameters.MaxSpeed,
                         Acceleration = ppCfg.AxisMotionParameters.Acceleration
                     }
                 }
            }, 
            new RobotExecutionContext());
        }

        /// <summary>
        /// 运输机构标准移动接口，语义与界面方法保持一致
        /// </summary>
        public void TransportMove(double speed, double acc, double recPos)
        {
            robotDriver.Execute(new MotionInstructionSequence
            {
                SequenceType = MotionInstructionSequenceType.StepByStep,
                Instructions = new MotionInstructionBase[]
                 {
                     new Point
                     {
                         TargetPosition = new AxisConstantValues[]
                         {
                             new AxisConstantValues
                             {
                                 Axis = 0,
                                 PositionValue = recPos
                             }
                         },
                         Speed = speed,
                         Acceleration = acc
                     }
                 }
            },
            new RobotExecutionContext());
        }

        /// <summary>
        /// 持续移动
        /// </summary>
        /// <param name="direction"></param>
        public void ContinueMove(bool direction)
        {
            robotDriver.Execute(new MotionInstructionSequence
            {
                SequenceType = MotionInstructionSequenceType.StepByStep,
                Instructions = new MotionInstructionBase[]
                 {
                     new ContinueMoveInstruction
                     {
                         Speed = ppCfg.AxisMotionParameters.MaxSpeed * (direction ? 1 : -1),
                         Acceleration = ppCfg.AxisMotionParameters.Acceleration,
                         LogicAxis = 0
                     }
                 }
            },
            new RobotExecutionContext());
        }

        /// <summary>
        /// 持续移动
        /// </summary>
        /// <param name="direction"></param>
        public void ContinueMove(double speed, double acc, bool direction)
        {
            robotDriver.Execute(new MotionInstructionSequence
            {
                SequenceType = MotionInstructionSequenceType.StepByStep,
                Instructions = new MotionInstructionBase[]
                 {
                     new ContinueMoveInstruction
                     {
                         Speed = speed * (direction ? 1 : -1),
                         Acceleration = acc,
                         LogicAxis = 0
                     }
                 }
            },
            new RobotExecutionContext());
        }
        public void RelativeMove(double speed, double acc, double relativeDistance)
        {
            robotDriver.Execute(new MotionInstructionSequence()
            {
                SequenceType = MotionInstructionSequenceType.StepByStep,
                Instructions = new MotionInstructionBase[] 
                { 
                    new RelativeMoveInstruction() 
                    { 
                        Speed = speed,
                        Acceleration = acc,
                        Distance = new AxisConstantValues
                        {
                            Axis = 0,
                            PositionValue = relativeDistance
                        }
                    } 
                },
                ExtendedParameters = null
            }, new RobotExecutionContext());
        }

        /// <summary>
        /// 停止运动
        /// </summary>
        public void StopMove()
        {
            robotDriver.Execute(new MotionInstructionSequence
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

        /// <summary>
        /// 当前位置
        /// </summary>
        public double GetCurrentPosition()
        {
            return currentTransportPosition;
        }

        /// <summary>
        /// 触发移动完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InvokeMoveFinishedEvent(object sender, EventArgs e)
        {
            MoveFinished?.Invoke(this, e);
        }
        /// <summary>
        /// 触发移动失败事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InvokeMoveFailedEvent(object sender, EventArgs e)
        {
            MoveFailed?.Invoke(this, e);
        }
        /// <summary>
        /// 触发位置变化事件，并更新当前位置缓存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InvokePositionChangedEvent(object sender, PositionChangedEventArgs e)
        {
            if (e.NewPosition.Length > 0)
            {
                currentTransportPosition = e.NewPosition[0].PositionValue;
            }
            PositionChanged?.Invoke(this, e);
        }
    }
}
