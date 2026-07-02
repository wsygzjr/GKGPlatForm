using GF_Gereric;
using GKG.Vision;
using GKG.ElectronicControl;

namespace GKG.MotionControl
{
    public class FlyingCalibrationInitPatams
    {
        /// <summary>
        /// 功能头ID
        /// </summary>
        public string FunctionHead = "";
        /// <summary>
        /// xy移动加速度
        /// </summary>
        public double AccelerationXY;
        /// <summary>
        /// xy移动速度
        /// </summary>
        public double SpeedXY;
        /// <summary>
        /// z移动加速度
        /// </summary>
        public double AccelerationZ;
        /// <summary>
        /// z移动速度
        /// </summary>
        public double SpeedZ;
        /// <summary>
        /// 相机拍照高度
        /// </summary>
        public double CameraHeight;
        /// <summary>
        /// 标定模板路径
        /// </summary>
        public string CalibrationModelPath = "";
    }
    public class FlyingCalibration : CalibrationBase
    {
        /// <summary>
        /// 标定类型
        /// </summary>
        public override CalibrationType Type => CalibrationType.FlyingCCD;

        private IBaseStateIO ChangeValveOrCCDIO;

        private Point3D axisPosition = new Point3D();

        FlyingCalibrationParameters flyingCalibrationParameters = new FlyingCalibrationParameters();

        FlyingCalibrationResult flyingCalibrationResult = new FlyingCalibrationResult();

        FlyingCalibrationInitPatams flyingCalibrationInitPatams = new FlyingCalibrationInitPatams();

        // 事件处理方法
        private void RobotDriver_PositionChanged(object? sender, PositionChangedEventArgs e)
        {
            for (int i = 0; i < e.NewPosition.Length; i++)
            {
                axisPosition.X = e.NewPosition[0].PositionValue;
                axisPosition.Y = e.NewPosition[1].PositionValue;
                axisPosition.Z = e.NewPosition[2].PositionValue;
            }
        }
        ~FlyingCalibration()
        {
            if(runtimeContext.RobotDriver!=null)
            {
                runtimeContext.RobotDriver.PositionChanged -= RobotDriver_PositionChanged;
            }
        }
        public override void Init(byte[] initParams)
        {
            flyingCalibrationInitPatams = JsonObjConvert.FromJSonBytes<FlyingCalibrationInitPatams>(initParams);
            IOStateInitParameters iOStateInitParameters = new IOStateInitParameters();
            iOStateInitParameters.channelID = "RW25";
            iOStateInitParameters.deviceID = "0";
            ChangeValveOrCCDIO = new StateControlMotionCard();
            ChangeValveOrCCDIO.Init(JsonObjConvert.ToJSonBytes(iOStateInitParameters));
            if (runtimeContext.MotionControl != null)
            {
                ChangeValveOrCCDIO.SetDeviceInstance(runtimeContext.MotionControl);
            }

            RobotDriver.PositionChanged -= RobotDriver_PositionChanged;
            RobotDriver.PositionChanged += RobotDriver_PositionChanged;
            functionHead = flyingCalibrationInitPatams.FunctionHead;
        }

        public void SetCalibrationParams(FlyingCalibrationParameters flyingCalibrationParameter)
        {
            flyingCalibrationParameters = flyingCalibrationParameter;
        }

        public override FlyingCalibrationResult Calibrate()
        {
            VisionDriver.CalibForFlying(new SearchMarkParams() { ModelPath = flyingCalibrationInitPatams.CalibrationModelPath });
            Point3D centerPt = flyingCalibrationParameters.FlyingCoordinates;
            Point3D firstPt;
            Point3D lastPt;
            if (flyingCalibrationParameters.FlyingDirection == FlyingDirection.X)
            {
                firstPt = new Point3D()
                {
                    X = centerPt.X - flyingCalibrationParameters.PreAccelerationDistance,
                    Y = centerPt.Y,
                    Z = centerPt.Z
                };
                lastPt = new Point3D()
                {
                    X = centerPt.X + flyingCalibrationParameters.PreAccelerationDistance,
                    Y = centerPt.Y,
                    Z = centerPt.Z
                };
            }
            else
            {
                firstPt = new Point3D()
                {
                    X = centerPt.X,
                    Y = centerPt.Y - flyingCalibrationParameters.PreAccelerationDistance,
                    Z = centerPt.Z
                };
                lastPt = new Point3D()
                {
                    X = centerPt.X,
                    Y = centerPt.Y + flyingCalibrationParameters.PreAccelerationDistance,
                    Z = centerPt.Z
                };
            }

            ChangeValveOrCCDIO.Write(true);
            try
            {
                MotionControlPositionComparisonTriggerPoint[] triggerPoints = new MotionControlPositionComparisonTriggerPoint[1]
                {
                    new MotionControlPositionComparisonTriggerPoint()
                    {
                        X = centerPt.X,
                        Y = centerPt.Y
                    }
                };

                MotionTrajectory motionTrajectory = MotionCalculatorDriver.Calculate(
                    BuildFlyingMotionCalculationParameters(firstPt, centerPt, lastPt, flyingCalibrationParameters.FlyingSpeed, flyingCalibrationInitPatams.AccelerationXY));

                InvokeRobotMethod(
                    "PositionComparison2D",
                    triggerPoints,
                    motionTrajectory.MotionInstructions ?? Array.Empty<MotionInstructionBase>());
            }
            finally
            {
                ChangeValveOrCCDIO.Write(false);
            }

            Point2D[] result = VisionDriver.GetFlyingRst();
            // 计算标定结果
            return flyingCalibrationResult;
        }

        public override FlyingCalibrationResult GetCalibrationResult()
        {
            return flyingCalibrationResult;
        }

        private MotionCalculationParameters BuildFlyingMotionCalculationParameters(Point3D firstPt, Point3D centerPt, Point3D lastPt, double speed, double acceleration)
        {
            return new MotionCalculationParameters
            {
                ProductProcessingTrajectory = new ProductProcessingTrajectoryItem[]
                {
                    new NonProcessingTrajectory
                    {
                        MotionTrajectory = new LinearMotionTrajectory
                        {
                            StartPoint = firstPt.ToToAxisConstantValues(),
                            LinearMotionTrajectoryItems = new LinearMotionTrajectoryItem[]
                            {
                                new LinearMotionTrajectoryItem
                                {
                                    ItemType = LinearMotionTrajectoryItemType.StraightLine,
                                    ItemBase = new LinearMotionTrajectoryItemStraightLine
                                    {
                                        EndPoint = new MotionTrajectoryPoint
                                        {
                                            Position = centerPt.ToToAxisConstantValues()
                                        }
                                    }
                                },
                                new LinearMotionTrajectoryItem
                                {
                                    ItemType = LinearMotionTrajectoryItemType.StraightLine,
                                    ItemBase = new LinearMotionTrajectoryItemStraightLine
                                    {
                                        EndPoint = new MotionTrajectoryPoint
                                        {
                                            Position = lastPt.ToToAxisConstantValues()
                                        }
                                    }
                                }
                            }
                        },
                        NonProcessingParameters = new NonProcessingTrajectoryParameters
                        {
                            StartSpeed = 0,
                            MaxSpeed = speed,
                            Acceleration = acceleration,
                            Deceleration = acceleration
                        }
                    }
                }
            };
        }

        private object? InvokeRobotMethod(string methodName, params object[] args)
        {
            var method = RobotDriver.GetType().GetMethod(methodName, args.Select(a => a.GetType()).ToArray());
            if (method == null)
            {
                throw new InvalidOperationException($"当前 RobotDriver 不支持方法: {methodName}");
            }

            return method.Invoke(RobotDriver, args);
        }
    }
}
