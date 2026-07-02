using GF_Gereric;
using GKG.ElectronicControl;
using GKG.Vision;

namespace GKG
{
    namespace MotionControl
    {
        public class CameraScaleCalibrationInitParams
        {
            /// <summary>
            /// 功能头ID
            /// </summary>
            public string FunctionHead = "";

            public NonProcessingTrajectoryParameters XYMoveParameters = new NonProcessingTrajectoryParameters()
            {
                StartSpeed = 0,
                MaxSpeed = 100,
                Acceleration = 100,
                Deceleration = 100
            };

            public NonProcessingTrajectoryParameters ZMoveParameters = new NonProcessingTrajectoryParameters()
            {
                StartSpeed = 0,
                MaxSpeed = 50,
                Acceleration = 50,
                Deceleration = 50
            };

            /// <summary>
            /// 相机拍照高度
            /// </summary>
            public double CameraHeight;

            /// <summary>
            /// 标定模板路径
            /// </summary>
            public string CalibrationModelPath = "";

            public void FromJsonBytes(byte[] jsonBytes)
            {
                var obj = JsonObjConvert.FromJSonBytes<CameraScaleCalibrationInitParams>(jsonBytes);
                this.FunctionHead = obj.FunctionHead;
                this.XYMoveParameters = obj.XYMoveParameters;
                this.ZMoveParameters = obj.ZMoveParameters;
                this.CameraHeight = obj.CameraHeight;
                this.CalibrationModelPath = obj.CalibrationModelPath;
            }

            public byte[] ToJsonBytes()
            {
                return JsonObjConvert.ToJSonBytes(this);
            }
        }

        public class CameraScaleCalibration : CalibrationBase
        {
            /// <summary>
            /// 标定类型
            /// </summary>
            public override CalibrationType Type => CalibrationType.CameraScale;

            private Point3D axisPosition = new Point3D();

            private CameraScaleCalibrationParameters cameraScaleCalibrationParameter = new CameraScaleCalibrationParameters();
            private CameraScaleCalibrationResult cameraScaleCalibrationResult = new CameraScaleCalibrationResult();
            private CameraScaleCalibrationInitParams cameraScaleCalibrationInitParams = new CameraScaleCalibrationInitParams();
            private bool stop = false;
            private int camHeight;
            private int camWidth;

            ~CameraScaleCalibration()
            {
                if (runtimeContext.RobotDriver != null)
                {
                    runtimeContext.RobotDriver.PositionChanged -= RobotDriver_PositionChanged;
                }
            }

            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="initParamsBytes"></param>
            public override void Init(byte[] initParamsBytes)
            {
                cameraScaleCalibrationResult = new CameraScaleCalibrationResult();
                cameraScaleCalibrationInitParams = new CameraScaleCalibrationInitParams();
                cameraScaleCalibrationInitParams.FromJsonBytes(initParamsBytes);
                functionHead = cameraScaleCalibrationInitParams.FunctionHead;

                RobotDriver.PositionChanged -= RobotDriver_PositionChanged;
                RobotDriver.PositionChanged += RobotDriver_PositionChanged;
            }

            private void RobotDriver_PositionChanged(object? sender, PositionChangedEventArgs e)
            {
                if (e?.NewPosition == null)
                    return;

                foreach (var position in e.NewPosition)
                {
                    if (position.Axis == AxisConstants.X)
                    {
                        axisPosition.X = position.PositionValue;
                    }
                    else if (position.Axis == AxisConstants.Y)
                    {
                        axisPosition.Y = position.PositionValue;
                    }
                    else if (position.Axis == AxisConstants.Z)
                    {
                        axisPosition.Z = position.PositionValue;
                    }
                }
            }

            public override void SetCalibrationParams(CalibrationParameters cameraScaleCalibrationParameters)
            {
                cameraScaleCalibrationParameter = (CameraScaleCalibrationParameters)cameraScaleCalibrationParameters;
            }

            public override CameraScaleCalibrationResult Calibrate()
            {
                stop = false;
                EnsureReady();

                double motionStep = cameraScaleCalibrationParameter.MotionStep;
                if (motionStep < 0.1)
                {
                    motionStep = 0.1;
                }

                Point3D cameraCenterPoint = cameraScaleCalibrationParameter.CameraCoordinates;
                double[] destPosX = BuildNinePointX(cameraCenterPoint.X, motionStep);
                double[] destPosY = BuildNinePointY(cameraCenterPoint.Y, motionStep);
                double[] pixelX = new double[9];
                double[] pixelY = new double[9];

                MoveToCaptureHeight(cameraCenterPoint.Z);

                SearchMarkParams searchMarkParams = new SearchMarkParams()
                {
                    ModelPath = cameraScaleCalibrationInitParams.CalibrationModelPath
                };

                for (int i = 0; i < 9; i++)
                {
                    if (stop)
                    {
                        break;
                    }

                    MoveToCapturePoint(destPosX[i], destPosY[i]);
                    Thread.Sleep(300);

                    SearchMarkResult searchMarkResult = VisionDriver.SearchMark(searchMarkParams);
                    if (searchMarkResult == null || !searchMarkResult.IsOk)
                    {
                        throw new InvalidOperationException("视觉搜索 Mark 失败");
                    }

                    pixelX[i] = searchMarkResult.Offset.X;
                    pixelY[i] = searchMarkResult.Offset.Y;
                }

                if (stop)
                {
                    return new CameraScaleCalibrationResult();
                }

                double scaleX;
                double scaleY;
                CalculateNinePointCalibration(pixelX, pixelY, destPosX, destPosY, 9, 1280, 1024, out scaleX, out scaleY);

                cameraScaleCalibrationResult.FunctionHeadId = cameraScaleCalibrationParameter.FunctionHeadId;
                cameraScaleCalibrationResult.XPixelScale = Math.Abs(scaleX);
                cameraScaleCalibrationResult.YPixelScale = Math.Abs(scaleY);
                return cameraScaleCalibrationResult;
            }

            public override CameraScaleCalibrationResult GetCalibrationResult()
            {
                return cameraScaleCalibrationResult;
            }

            private void EnsureReady()
            {
                _ = RobotDriver;
                _ = VisionDriver;

                if (cameraScaleCalibrationParameter == null)
                {
                    throw new InvalidOperationException("相机比例标定参数未设置");
                }

                if (cameraScaleCalibrationInitParams == null)
                {
                    throw new InvalidOperationException("相机比例标定初始化参数未设置");
                }

                if (string.IsNullOrWhiteSpace(cameraScaleCalibrationInitParams.CalibrationModelPath))
                {
                    throw new InvalidOperationException("CalibrationModelPath 未配置");
                }
            }

            private void MoveToCaptureHeight(double currentZ)
            {
                ExecutePointMove(new AxisConstantValues[]
                {
                    new AxisConstantValues() { Axis = AxisConstants.Z, PositionValue = cameraScaleCalibrationInitParams.CameraHeight > 0 ? cameraScaleCalibrationInitParams.CameraHeight : currentZ }
                }, cameraScaleCalibrationInitParams.ZMoveParameters.MaxSpeed, cameraScaleCalibrationInitParams.ZMoveParameters.Acceleration);
            }

            private void MoveToCapturePoint(double x, double y)
            {
                ExecutePointMove(new AxisConstantValues[]
                {
                    new AxisConstantValues() { Axis = AxisConstants.X, PositionValue = x },
                    new AxisConstantValues() { Axis = AxisConstants.Y, PositionValue = y }
                }, cameraScaleCalibrationInitParams.XYMoveParameters.MaxSpeed, cameraScaleCalibrationInitParams.XYMoveParameters.Acceleration);
            }

            private void ExecutePointMove(AxisConstantValues[] targetPosition, double speed, double acceleration)
            {
                Task task = ExecutePointMoveAsync(targetPosition, speed, acceleration);
                bool completed = task.Wait(60000);
                if (!completed)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_ROBOT_MOVE_TIMEOUT, MotionErr.RobotMoveTimeout, MotionErr.RobotMoveTimeout);
                }
            }

            private Task ExecutePointMoveAsync(AxisConstantValues[] targetPosition, double speed, double acceleration)
            {
                _ = RobotDriver;
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
                    taskCompletionSource.TrySetException(new GKGException(MotionErrCodeConsts.ERR_MOTION_MOVE_FAIL, MotionErr.RobotMoveFailed, MotionErr.RobotMoveFailed));
                };

                RobotDriver.MoveFinished += moveFinishedHandler;
                RobotDriver.MoveFailed += moveFailedHandler;

                try
                {
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

            private static double[] BuildNinePointX(double centerX, double step)
            {
                return new double[9]
                {
                    centerX + 0,
                    centerX + step,
                    centerX + step,
                    centerX + 0,
                    centerX - step,
                    centerX - step,
                    centerX - step,
                    centerX + 0,
                    centerX + step
                };
            }

            private static double[] BuildNinePointY(double centerY, double step)
            {
                return new double[9]
                {
                    centerY + 0,
                    centerY + 0,
                    centerY + step,
                    centerY + step,
                    centerY + step,
                    centerY + 0,
                    centerY - step,
                    centerY - step,
                    centerY - step
                };
            }

            private void CalculateNinePointCalibration(double[] column, double[] row, double[] x, double[] y, int len, double width, double height, out double xsize, out double ysize)
            {
                double[] hvColumn = new double[9];
                double[] hvRow = new double[9];
                double[] hvX = new double[9];
                double[] hvY = new double[9];
                double hvQx2 = 0, hvQy2 = 0, hvQx = 0, hvQy = 0;
                camHeight = (int)height;
                camWidth = (int)width;

                for (int i = 0; i < len; i++)
                {
                    hvX[i] = x[i] - x[0];
                    hvY[i] = y[i] - y[0];
                    hvColumn[i] = width - column[i];
                    hvRow[i] = height - row[i];
                }

                GeneralMapMatrix(hvColumn, hvRow, hvX, hvY, len, out cameraScaleCalibrationResult.hvHomMat2D);
                GeneralMapMatrix(hvX, hvY, hvColumn, hvRow, len, out cameraScaleCalibrationResult.worldHomMat2D);

                MapPoint2d(width / 2, height / 2, ref hvQx, ref hvQy, cameraScaleCalibrationResult.hvHomMat2D);
                MapPoint2d(width / 2 + 1, height / 2 + 1, ref hvQx2, ref hvQy2, cameraScaleCalibrationResult.hvHomMat2D);

                xsize = hvQx - hvQx2;
                ysize = hvQy - hvQy2;
            }

            private void GeneralMapMatrix(double[] row, double[] col, double[] y, double[] x, int len, out double[] mapMatrix)
            {
                mapMatrix = new double[9];

                double er = Mean(row, len);
                double ec = Mean(col, len);
                double ey = Mean(y, len);
                double ex = Mean(x, len);

                double err = VarMean(row, row, len);
                double ecc = VarMean(col, col, len);
                double erc = VarMean(row, col, len);
                double ery = VarMean(row, y, len);
                double ecy = VarMean(col, y, len);
                double erx = VarMean(row, x, len);
                double ecx = VarMean(col, x, len);

                double varRR = Var(err, er, er);
                double varRC = Var(erc, er, ec);
                double varCC = Var(ecc, ec, ec);
                double varRY = Var(ery, er, ey);
                double varCY = Var(ecy, ec, ey);
                double varRX = Var(erx, er, ex);
                double varCX = Var(ecx, ec, ex);

                double denominator = varRR * varCC - varRC * varRC;
                if (Math.Abs(denominator) < 1e-12)
                {
                    throw new InvalidOperationException("9点标定矩阵计算失败，输入点退化。");
                }

                mapMatrix[0] = (varRY * varCC - varCY * varRC) / denominator;
                mapMatrix[1] = (varRR * varCY - varRY * varRC) / denominator;
                mapMatrix[2] = ey - er * mapMatrix[0] - ec * mapMatrix[1];
                mapMatrix[3] = (varRX * varCC - varCX * varRC) / denominator;
                mapMatrix[4] = (varRR * varCX - varRX * varRC) / denominator;
                mapMatrix[5] = ex - er * mapMatrix[3] - ec * mapMatrix[4];
                mapMatrix[6] = 0;
                mapMatrix[7] = 0;
                mapMatrix[8] = 1;
            }

            private void MapPoint2d(double row, double col, ref double y, ref double x, double[] mapMatrix)
            {
                x = mapMatrix[3] * row + mapMatrix[4] * col + mapMatrix[5];
                y = mapMatrix[0] * row + mapMatrix[1] * col + mapMatrix[2];
            }

            private double Sum(double[] data, int len)
            {
                double sum = 0;
                for (int i = 0; i < len; i++)
                {
                    sum += data[i];
                }
                return sum;
            }

            private double Mean(double[] data, int len)
            {
                return Sum(data, len) / len;
            }

            private double VarMean(double[] x, double[] y, int len)
            {
                double[] values = new double[len];
                for (int i = 0; i < len; i++)
                {
                    values[i] = x[i] * y[i];
                }
                return Mean(values, len);
            }

            private double Var(double exy, double ex, double ey)
            {
                return exy - ex * ey;
            }
        }
    }
}
