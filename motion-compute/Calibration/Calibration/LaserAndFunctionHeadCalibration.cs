using GF_Gereric;
using GKG.ElectronicControl;
using GKG.ElectronicControl.General;

namespace GKG
{
    namespace MotionControl
    {
        public class LaserAndFunctionHeadCalibrationInitParams
        {
            /// <summary>
            /// 功能头ID
            /// </summary>
            public string FunctionHead = "";

            /// <summary>
            /// 是否启用位置锁存
            /// </summary>
            public bool IsUsePositionLatch;

            /// <summary>
            /// 相机与激光的偏移标定结果
            /// </summary>
            public OffsetCalibrationResult CCDVSLaser;

            /// <summary>
            /// 相机与阀的偏移标定结果
            /// </summary>
            public OffsetCalibrationResult CCDVSValve;

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
            /// 位置锁存通道
            /// </summary>
            public int Channel;

            /// <summary>
            /// 蘑菇头限制高度
            /// </summary>
            public double MushroomPLimitHeight;

            /// <summary>
            /// 蘑菇头高度补偿
            /// </summary>
            public double ZheightCalibrationCompen;
        }

        public class LaserAndFunctionHeadCalibration : CalibrationBase
        {
            /// <summary>
            /// 标定类型
            /// </summary>
            public override CalibrationType Type => CalibrationType.LaserHeight;

            /// <summary>
            /// 测高
            /// </summary>
            private IMeasureHeightBase? measureHeight;

            /// <summary>
            /// 蘑菇头IO
            /// </summary>
            private IBaseStateIO? measureHeightIO;

            /// <summary>
            /// 测高标定参数
            /// </summary>
            private LaserAndFunctionHeadCalibrationParameters calibrationParameter = new LaserAndFunctionHeadCalibrationParameters();

            /// <summary>
            /// 测高标定结果
            /// </summary>
            private LaserAndFunctionHeadCalibrationResult calibrationResult = new LaserAndFunctionHeadCalibrationResult();

            /// <summary>
            /// 测高标定初始化参数
            /// </summary>
            private LaserAndFunctionHeadCalibrationInitParams laserAndFunctionHeadCalibrationInitParams = new LaserAndFunctionHeadCalibrationInitParams();

            private Point3D axisPosition = new Point3D();

            private bool stop = false;

            ~LaserAndFunctionHeadCalibration()
            {
                if (runtimeContext.RobotDriver != null)
                {
                    runtimeContext.RobotDriver.PositionChanged -= RobotDriver_PositionChanged;
                }
            }

            public override void Init(byte[] initParams)
            {
                IOStateInitParameters iOStateInitParameters = new IOStateInitParameters();
                iOStateInitParameters.channelID = "RO04";
                iOStateInitParameters.deviceID = "0";
                //measureHeightIO = IOStateCallBack.GetIOStateControl(JsonObjConvert.ToJSon(iOStateInitParameters), 4);
                //measureHeight = TestCallBack.GetMeasureHeight();
                _ = RobotDriver;
                calibrationResult.MeasureHeightPositiveDir = measureHeight?.PositiveDir ?? 0;
                RobotDriver.PositionChanged -= RobotDriver_PositionChanged;
                RobotDriver.PositionChanged += RobotDriver_PositionChanged;
                laserAndFunctionHeadCalibrationInitParams = JsonObjConvert.FromJSonBytes<LaserAndFunctionHeadCalibrationInitParams>(initParams);
                functionHead = laserAndFunctionHeadCalibrationInitParams.FunctionHead;
            }

            // 事件处理方法
            private void RobotDriver_PositionChanged(object? sender, PositionChangedEventArgs e)
            {
                axisPosition.X = e.NewPosition[0].PositionValue;
                axisPosition.Y = e.NewPosition[1].PositionValue;
                axisPosition.Z = e.NewPosition[2].PositionValue;
            }

            /// <summary>
            /// 设置标定参数
            /// </summary>
            /// <param name="calibrationParameters"></param>
            public override void SetCalibrationParams(CalibrationParameters calibrationParameters)
            {
                calibrationParameter = (LaserAndFunctionHeadCalibrationParameters)calibrationParameters;
            }

            /// <summary>
            /// 功能头移动
            /// </summary>
            /// <param name="ccdPosition"></param>
            /// <param name="offsetCalibrationResult"></param>
            /// <exception cref="GKGException"></exception>
            private void MoveToFunctionHeadPosition(Point3D ccdPosition, OffsetCalibrationResult offsetCalibrationResult)
            {
                // 计算激光测高位置
                Point3D functionHeadPos = (Point3D)offsetCalibrationResult.Calculate(ccdPosition);
                // Z轴抬高到安全高度
                Task task = ExecutePointMoveAsync(
                    new AxisConstantValues[]
                    {
                        new AxisConstantValues() { Axis = AxisConstants.Z, PositionValue = 0 }
                    });
                bool rtn = task.Wait(60000);
                if (rtn != true)
                {
                    //Z 轴移动错误
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_ROBOT_MOVE_TIMEOUT, MotionErr.RobotMoveTimeout, MotionErr.RobotMoveTimeout);
                }
                task = ExecutePointMoveAsync(
                    new AxisConstantValues[]
                    {
                        new AxisConstantValues() { Axis = AxisConstants.X, PositionValue = functionHeadPos.X },
                        new AxisConstantValues() { Axis = AxisConstants.Y, PositionValue = functionHeadPos.Y }
                    });
                rtn = task.Wait(60000);
                if (rtn != true)
                {
                    //Z 轴移动错误
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_ROBOT_MOVE_TIMEOUT, MotionErr.RobotMoveTimeout, MotionErr.RobotMoveTimeout);
                }
            }

            /// <summary>
            /// 激光功能头移动
            /// </summary>
            /// <param name="ccdPosition"></param>
            /// <exception cref="GKGException"></exception>
            private void MoveToLaserPosition(Point3D ccdPosition)
            {
                MoveToFunctionHeadPosition(ccdPosition, laserAndFunctionHeadCalibrationInitParams.CCDVSLaser);
            }

            /// <summary>
            /// 阀移动
            /// </summary>
            /// <param name="ccdPosition"></param>
            private void MoveToValvePosition(Point3D ccdPosition)
            {
                MoveToFunctionHeadPosition(ccdPosition, laserAndFunctionHeadCalibrationInitParams.CCDVSValve);
            }

            /// <summary>
            /// 标定z轴压蘑菇头流程
            /// </summary>
            /// <param name="speed"></param>
            /// <returns></returns>
            private double Calibrate(double speed)
            {
                bool bLaserSignal = false;
                if (laserAndFunctionHeadCalibrationInitParams.IsUsePositionLatch)//开启了位置锁存测试胶阀高度
                {
                    InvokeRobotMethod("SetPositionLatch", AxisConstants.Z, MotionControlPositionLatchCaptureLogic.IO, laserAndFunctionHeadCalibrationInitParams.Channel, MotionControlPositionLatchSignalTriggerMode.FallingEdge, (short)0, 1);
                    InvokeRobotMethod("SetPositionLatchEnabled", AxisConstants.Z, true);
                }

                // 慢速去找平台测高信号
                MoveParam moveParam = new MoveParam();
                moveParam.speed = speed;
                moveParam.AxisCount = 1;
                moveParam.logicAxis = new int[] { AxisConstants.Z };
                moveParam.acc = laserAndFunctionHeadCalibrationInitParams.AccelerationZ;
                InvokeRobotMethod("ContinueMove", moveParam);

                do
                {
                    bLaserSignal = measureHeightIO?.Read() ?? false;

                    if (bLaserSignal)
                    {
                        InvokeRobotMethod("StopContinueMove", AxisConstants.Z);
                        break;
                    }
                    double dCurPos_z = axisPosition.Z;
                    if (dCurPos_z > laserAndFunctionHeadCalibrationInitParams.MushroomPLimitHeight)
                    {
                        InvokeRobotMethod("StopContinueMove", AxisConstants.Z);
                    }

                    if (stop)
                    {
                        InvokeRobotMethod("StopContinueMove", AxisConstants.Z);
                        return 0;
                    }
                } while (true);

                double dZHeight = 0;
                if (laserAndFunctionHeadCalibrationInitParams.IsUsePositionLatch)//开启了位置锁存测试胶阀高度
                {
                    double[] pos = (double[])(InvokeRobotMethod("GetPositionLatchResult", AxisConstants.Z) ?? Array.Empty<double>());
                    InvokeRobotMethod("SetPositionLatchEnabled", AxisConstants.Z, false);
                    if (pos.Length > 0 && pos[0] > 0.0001f)
                    {
                        dZHeight = pos[0];
                    }
                    else
                    {
                        dZHeight = axisPosition.Z;
                    }
                }
                else
                {
                    dZHeight = axisPosition.Z;
                }
                return dZHeight;
            }

            /// <summary>
            /// 标定
            /// </summary>
            /// <returns>偏移标定结果</returns>
            public override LaserAndFunctionHeadCalibrationResult Calibrate()
            {
                stop = false;
                Point3D ccdPoint = calibrationParameter.LaserCoordinates;

                // 移动到激光测高位置(蘑菇头)
                MoveToLaserPosition(ccdPoint);
                int rtn = 0;
                double height = 0;
                //获取激光测高值
                for (int iGetIndex = 0; iGetIndex < MeasureHeightConsts.MAX_GetLaserHErrorCnt; iGetIndex++)
                {
                    try
                    {
                        if (measureHeight == null)
                        {
                            throw new InvalidOperationException("measureHeight 未初始化");
                        }
                        height = measureHeight.ReadHeight();
                        if (height > MeasureHeightConsts.MIN_TestLaserValve && height < MeasureHeightConsts.MAX_TestLaserValve)
                        {
                            // 成功获取高度值
                            rtn = 0;
                            break;
                        }
                        else
                        {
                            rtn = -1;
                        }
                    }
                    catch (GKGException)
                    {
                        throw;
                    }
                }
                if (rtn != 0)
                {
                    throw new GKGException(MeasureHeightErrCodeConsts.ERR_MEASURE_HEIGHT_READ_FAIL, MeasureHeightErr.MeasureHeightReadFail, MeasureHeightErr.MeasureHeightReadFail);
                }

                // 移动到阀位置
                MoveToValvePosition(ccdPoint);

                double dZHeight = 0;
                // 第一次测试
                dZHeight = Calibrate(MotionConstants.Teach_ZAxisTestFirstLaserVel);
                // 用户主动终止
                if (stop)
                    return new LaserAndFunctionHeadCalibrationResult();
                //回抬2毫米高度
                Task task = ExecutePointMoveAsync(
                    new AxisConstantValues[]
                    {
                        new AxisConstantValues() { Axis = AxisConstants.Z, PositionValue = dZHeight - 2 }
                    });
                task.Wait(60000);

                ////------------------------------------------------------------
                ////第二次测试
                dZHeight = Calibrate(MotionConstants.Teach_ZAxisTestSecondLaserVel);
                // 用户主动终止
                if (stop)
                    return new LaserAndFunctionHeadCalibrationResult();
                //将标定标定后的蘑菇头高度差值补偿进激光测高校正数值内
                dZHeight -= laserAndFunctionHeadCalibrationInitParams.ZheightCalibrationCompen;

                // z抬到安全高度
                task = ExecutePointMoveAsync(
                    new AxisConstantValues[]
                    {
                        new AxisConstantValues() { Axis = AxisConstants.Z, PositionValue = 0 }
                    });
                task.Wait(60000);

                // 保存结果
                calibrationResult.FunctionHeadId = calibrationParameter.FunctionHeadId;
                calibrationResult.LaserToPlaneValue = height;
                calibrationResult.ValveZAxisPosition = dZHeight;

                return calibrationResult;
            }

            /// <summary>
            /// 获取标定结果
            /// </summary>
            /// <returns>偏移标定结果</returns>
            public override LaserAndFunctionHeadCalibrationResult GetCalibrationResult()
            {
                return calibrationResult;
            }

            public void Terminate()
            {
                stop = true;
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

            private Task ExecutePointMoveAsync(AxisConstantValues[] targetPosition)
            {
                _ = MotionCalculatorDriver;

                TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
                EventHandler? moveFinishedHandler = null;
                EventHandler? moveFailedHandler = null;

                moveFinishedHandler = (sender, args) =>
                {
                    RobotDriver.MoveFinished -= moveFinishedHandler;
                    RobotDriver.MoveFailed -= moveFailedHandler;
                    taskCompletionSource.TrySetResult(true);
                };

                moveFailedHandler = (sender, args) =>
                {
                    RobotDriver.MoveFinished -= moveFinishedHandler;
                    RobotDriver.MoveFailed -= moveFailedHandler;
                    taskCompletionSource.TrySetException(new GKGException(MotionErrCodeConsts.ERR_MOTION_ROBOT_MOVE_TIMEOUT, MotionErr.RobotMoveFailed, MotionErr.RobotMoveFailed));
                };

                RobotDriver.MoveFinished += moveFinishedHandler;
                RobotDriver.MoveFailed += moveFailedHandler;

                try
                {
                    double speed = targetPosition.Length == 1 ? laserAndFunctionHeadCalibrationInitParams.SpeedZ : laserAndFunctionHeadCalibrationInitParams.SpeedXY;
                    double acceleration = targetPosition.Length == 1 ? laserAndFunctionHeadCalibrationInitParams.AccelerationZ : laserAndFunctionHeadCalibrationInitParams.AccelerationXY;
                    MotionTrajectory motionTrajectory = MotionCalculatorDriver.Calculate(BuildPointMoveCalculationParameters(targetPosition, speed, acceleration));
                    RobotDriver.Execute(
                        new MotionInstructionSequence()
                        {
                            SequenceType = MotionInstructionSequenceType.StepByStep,
                            Instructions = motionTrajectory.MotionInstructions ?? Array.Empty<MotionInstructionBase>()
                        },
                        RobotExecutionContext);
                }
                catch
                {
                    RobotDriver.MoveFinished -= moveFinishedHandler;
                    RobotDriver.MoveFailed -= moveFailedHandler;
                    throw;
                }

                return taskCompletionSource.Task;
            }

            private MotionCalculationParameters BuildPointMoveCalculationParameters(AxisConstantValues[] targetPosition, double speed, double acceleration)
            {
                return new MotionCalculationParameters
                {
                    ProductProcessingTrajectory = new ProductProcessingTrajectoryItem[]
                    {
                        new NonProcessingTrajectory
                        {
                            MotionTrajectory = new DotMotionTrajectory
                            {
                                TargetPoint = new MotionTrajectoryPoint
                                {
                                    Position = targetPosition
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
        }
    }
}