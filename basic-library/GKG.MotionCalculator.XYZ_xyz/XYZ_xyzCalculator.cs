using GF_Gereric;
using GKG.Maths;
namespace GKG.MotionControl
{
    public class XYZ_xyzCalculatorInitCfg
    {
        public MultiValveSpindle multiValveSpindle { get; set; }
        public double NonProcessingSpeed { get; set; }

        public double ProcessingAcceleration { get; set; }

        public double Acceleration { get; set; }

        public double ViceSpeed { get; set; }

        public double ViceAcceleration { get; set; }

        public Dictionary<string, Point2D> FunctionHeadOffset = new Dictionary<string, Point2D>();

        public int[] Channels { get; set; }

        public XYZ_xyzCalculatorInitCfg()
        {
            multiValveSpindle = MultiValveSpindle.X;
            NonProcessingSpeed = 100;
            ProcessingAcceleration = 1000;
            Acceleration= 1000;
            ViceSpeed = 100;
            ViceAcceleration = 1000;
        }
    }
    public class XYZ_xyzCalculator : IBaseMotionCalculator
    {
        /// <summary>
        /// 运动计算器初始化配置
        /// </summary>
        private XYZ_xyzCalculatorInitCfg initParameters { get; set; } = new XYZ_xyzCalculatorInitCfg();

        public void Init(byte[] initCfg)
        {
            initParameters = JsonObjConvert.FromJSonBytes<XYZ_xyzCalculatorInitCfg>(initCfg);
        }

        /// <summary>
        /// 运动计算器计算接口，暂时不使用
        /// </summary>
        /// <param name="motionCalculationParameters"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public MotionTrajectory Calculate(MotionCalculationParameters motionCalculationParameters)
        {
            // 计算副阀xy的的目标运动位置
            MotionTrajectory motionTrajectory = new MotionTrajectory();
            List<MotionInstructionBase> motionInstructionList = new List<MotionInstructionBase>();
            // 主阀轨迹
            for (int i = 0; i < motionCalculationParameters.ProductProcessingTrajectory.Length; i++)
            {
                // 加工轨迹
                if (motionCalculationParameters.ProductProcessingTrajectory[i].IsProcessing)
                {
                    // 通道需要添加配置，当前先默认用主阀通道
                    int[] channels; 
                    if (initParameters.Channels.Length>0)
                    {
                        channels = new int[1] { initParameters.Channels[0] };
                    }
                    else
                    {
                        channels = new int[1] { 16 };
                    }

                    ProcessingTrajectory processingTrajectory = motionCalculationParameters.ProductProcessingTrajectory[i] as ProcessingTrajectory;
                    motionInstructionList.AddRange(CalculateProcessingTrajectory(processingTrajectory, channels));
                }
                // 非加工轨迹
                else
                {
                    NonProcessingTrajectory nonProcessingTrajectory = motionCalculationParameters.ProductProcessingTrajectory[i] as NonProcessingTrajectory;
                    motionInstructionList.AddRange(CalculateNoProcessingTrajectory(nonProcessingTrajectory));
                }
            }
            motionTrajectory.MotionInstructions = motionInstructionList.ToArray();
            motionTrajectory.SequenceType = InferSequenceType(motionTrajectory.MotionInstructions);
            // 暂时为空
            motionTrajectory.ControlParameters = null;
            return motionTrajectory;
        }

        /// <summary>
        /// 计算
        /// </summary>
        /// <param name="motionCalculationParameters">Key:功能头id Value运动轨迹参数</param>
        /// <returns></returns>
        /// <exception cref="GKGException"></exception>
        public MotionTrajectory Calculate(Dictionary<string, MotionCalculationParameters> motionCalculationParameters)
        {
            string[] functionHeadIDs = motionCalculationParameters.Keys.ToArray();
            MotionCalculationParameters[] parameters = motionCalculationParameters.Values.ToArray();
            // 双阀间距和副阀目标位置
            Point2D viceDistance, viceTargetPosition;
            SortAndPairValves(functionHeadIDs, ref parameters, out viceDistance, out viceTargetPosition);

            // 构造副阀移动轨迹
            MotionInstructionBase viceTrajectory = new Point
            {
                TargetPosition = new AxisConstantValues[2]
                {
                   new AxisConstantValues{ Axis=AxisConstants.X1,PositionValue=viceTargetPosition.X},
                   new AxisConstantValues{ Axis=AxisConstants.Y1,PositionValue=viceTargetPosition.Y}
                },
                Speed = initParameters.ViceSpeed,
                Acceleration = initParameters.ViceAcceleration,
            };

            // 计算副阀xy的的目标运动位置
            MotionTrajectory motionTrajectory = new MotionTrajectory();
            List<MotionInstructionBase> motionInstructionList = new List<MotionInstructionBase>();
            motionInstructionList.Add(viceTrajectory);
            // 主阀轨迹
            for (int i = 0; i < parameters[0].ProductProcessingTrajectory.Length; i++)
            {
                // 加工轨迹
                if (parameters[0].ProductProcessingTrajectory[i].IsProcessing)
                {
                    // 通道需要添加配置，当前先默认用主阀通道
                    int[] channels = new int[functionHeadIDs.Length];
                    for (int j = 0; j < functionHeadIDs.Length; j++)
                    {
                        channels[j] = initParameters.Channels[j];
                    }
                    ProcessingTrajectory processingTrajectory = parameters[0].ProductProcessingTrajectory[i] as ProcessingTrajectory;
                    motionInstructionList.AddRange(CalculateProcessingTrajectory(processingTrajectory, channels));
                }
                // 非加工轨迹
                else
                {
                    NonProcessingTrajectory nonProcessingTrajectory = parameters[0].ProductProcessingTrajectory[i] as NonProcessingTrajectory;
                    motionInstructionList.AddRange(CalculateNoProcessingTrajectory(nonProcessingTrajectory));
                }
            }
            motionTrajectory.MotionInstructions = motionInstructionList.ToArray();
            motionTrajectory.SequenceType = InferSequenceType(motionTrajectory.MotionInstructions);
            // 暂时为空
            motionTrajectory.ControlParameters = null;
            return motionTrajectory;
        }

        private static int InferSequenceType(MotionInstructionBase[]? instructions)
        {
            if (instructions == null || instructions.Length == 0)
            {
                return MotionTrajectorySequenceTypeConstants.StepByStep;
            }

            bool hasPulseCompare = instructions.Any(i =>
                i.InstructionType == MotionInstructionType.Buf2DComparePulseExElemData ||
                i.InstructionType == MotionInstructionType.PositionComparison2D);
            if (hasPulseCompare)
            {
                return MotionTrajectorySequenceTypeConstants.PositionComparison2D;
            }

            bool hasContinuousGeometry = instructions.Any(i =>
                i.InstructionType == MotionInstructionType.Linear ||
                i.InstructionType == MotionInstructionType.ArcA ||
                i.InstructionType == MotionInstructionType.ArcB ||
                i.InstructionType == MotionInstructionType.Circle);
            if (hasContinuousGeometry)
            {
                return MotionTrajectorySequenceTypeConstants.ContinuousInterpolation;
            }

            return MotionTrajectorySequenceTypeConstants.StepByStep;
        }

        /// <summary>
        /// 将轨迹数据进行排序，方便后续计算
        /// </summary>
        /// <param name="parameters">轨迹数据</param>
        /// <exception cref="GKGException"></exception>
        private void MotionCalculationParametersSort(ref MotionCalculationParameters[] parameters, out Point2D[] valvesSpacing)
        {
            valvesSpacing = new Point2D[parameters.Length - 1];
            // 如果长度小于2则不需要排序
            if (parameters.Length < 2)
                return;
            // 长度大于2只考虑双阀，其他功能头不考虑
            MotionCalculationParameters[] parametersSort = new MotionCalculationParameters[parameters.Length];
            KeyValuePair<int, AxisConstantValues[]>[] axisPosPairs = new KeyValuePair<int, AxisConstantValues[]>[parameters.Length];
            // 先取出所有点位，只取首点即可，因为这里是相对于功能头位置的排序
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                if (parameter.ProductProcessingTrajectory == null || parameter.ProductProcessingTrajectory.Length == 0 || parameter.ProductProcessingTrajectory[0] == null)
                {
                    throw new GKGException(MotionCalculateErrCodeConsts.MotionCalculateErrParamsIsNull, MotionCalculateErr.MotionCalculateErrParamsIsNull, MotionCalculateErr.MotionCalculateErrParamsIsNull);
                }
                if (parameter.ProductProcessingTrajectory[0].IsProcessing == true)
                {
                    ProcessingTrajectory processingTrajectory = parameter.ProductProcessingTrajectory[0] as ProcessingTrajectory;
                    switch (processingTrajectory.MotionTrajectory.TrajectoryType)
                    {
                        case MotionTrajectoryType.Linear:
                            {
                                axisPosPairs[i] = new KeyValuePair<int, AxisConstantValues[]>(i, (processingTrajectory.MotionTrajectory as LinearMotionTrajectory).StartPoint);
                                //axisPosArray[i] = (processingTrajectory.MotionTrajectory as LinearMotionTrajectory).StartPoint;
                            }
                            break;

                        case MotionTrajectoryType.Dot:
                            {
                                axisPosPairs[i] = new KeyValuePair<int, AxisConstantValues[]>(i, (processingTrajectory.MotionTrajectory as DotMotionTrajectory).TargetPoint.Position);
                                //axisPosArray[i] = (processingTrajectory.MotionTrajectory as DotMotionTrajectory).TargetPoint;
                            }
                            break;
                    }
                }
            }

            // 排序，从轴的负方向到轴的正方向
            switch (initParameters.multiValveSpindle)
            {
                // 按X方向排序
                case MultiValveSpindle.X:
                    {
                        Array.Sort(axisPosPairs, (p1, p2) =>
                        {
                            if (p1.Value[0].PositionValue < p2.Value[0].PositionValue)
                                return -1;
                            else if (p1.Value[0].PositionValue == p2.Value[0].PositionValue)
                                return 0;
                            else
                                return -1;
                        });
                    }
                    break;
                // 按Y方向排序
                case MultiValveSpindle.Y:
                    {
                        Array.Sort(axisPosPairs, (p1, p2) =>
                        {
                            if (p1.Value[1].PositionValue < p2.Value[1].PositionValue)
                                return -1;
                            else if (p1.Value[1].PositionValue == p2.Value[1].PositionValue)
                                return 0;
                            else
                                return -1;
                        });
                    }
                    break;
            }

            // 排序之后重组数据
            for (int i = 0; i < axisPosPairs.Length; i++)
            {
                parametersSort[i] = parameters[axisPosPairs[i].Key];
            }
            // 将排序的数据赋值给源数据
            parameters = parametersSort;

            // 计算双阀间距
            for (int i = 1; i < axisPosPairs.Length; i++)
            {
                valvesSpacing[i].X = axisPosPairs[i].Value[0].PositionValue - axisPosPairs[i - 1].Value[0].PositionValue;
                valvesSpacing[i].Y = axisPosPairs[i].Value[1].PositionValue - axisPosPairs[i - 1].Value[1].PositionValue;
            }
        }

        /// <summary>
        /// 计算加工轨迹预处理轨迹
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="linearMotionTrajectory"></param>
        /// <param name="prePostProcessingParameters"></param>
        /// <param name="channels"></param>
        /// <returns></returns>
        /// <exception cref="GKGException"></exception>
        private List<MotionInstructionBase> CalculatePreProcessingTrajectory(ref Point3D startPoint, LinearMotionTrajectoryItem linearMotionTrajectory, DispensePrePostProcessingParameters prePostProcessingParameters, int[] channels)
        {
            List<MotionInstructionBase> motionInstructionList = new List<MotionInstructionBase>();

            // 计算加工前轨迹
            switch (linearMotionTrajectory.ItemType)
            {
                // 处理直线轨迹
                case LinearMotionTrajectoryItemType.StraightLine:
                    {
                        LinearMotionTrajectoryItemStraightLine straightLineItem = linearMotionTrajectory.ItemBase as LinearMotionTrajectoryItemStraightLine;
                        if (straightLineItem?.EndPoint?.Position?.Length < 3)
                        {
                            throw new GKGException(MotionCalculateErrCodeConsts.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect);
                        }
                        // 计算起始点和终点
                        Point3D endPoint = new Point3D(straightLineItem.EndPoint.Position[0].PositionValue, straightLineItem.EndPoint.Position[1].PositionValue, straightLineItem.EndPoint.Position[2].PositionValue);

                        // 计算方向向量
                        Point3D direction = new Point3D(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y, endPoint.Z - startPoint.Z);

                        // 计算长度
                        double length = Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y + direction.Z * direction.Z);

                        // 归一化方向向量
                        direction.X /= length;
                        direction.Y /= length;
                        direction.Z /= length;

                        // 计算预处理起始点
                        Point3D preProcessingStartPoint = new Point3D(
                            startPoint.X - direction.X * prePostProcessingParameters.AdvanceDistance,
                            startPoint.Y - direction.Y * prePostProcessingParameters.AdvanceDistance,
                            startPoint.Z - direction.Z * prePostProcessingParameters.AdvanceDistance
                            );

                        // 组直线轨迹

                        // 首先移动到预处理起始点（提前距离点）
                        motionInstructionList.Add(new StraightLine
                        {
                            EndPosition = GKGMath.point3DToAxisConstantValues(preProcessingStartPoint),
                            Speed = initParameters.NonProcessingSpeed,
                            Acceleration = initParameters.ProcessingAcceleration
                        });

                        // 计算开阀点
                        if (prePostProcessingParameters.AdvanceDistance > prePostProcessingParameters.AdvanceOpenDistance)
                        {
                            // 提前距离大于提前开阀距离 ，计算开阀点
                            Point3D openPoint = new Point3D(
                                startPoint.X - direction.X * prePostProcessingParameters.AdvanceOpenDistance,
                                startPoint.Y - direction.Y * prePostProcessingParameters.AdvanceOpenDistance,
                                startPoint.Z - direction.Z * prePostProcessingParameters.AdvanceOpenDistance
                                );

                            // 移动到开阀点
                            motionInstructionList.Add(new StraightLine
                            {
                                StartPosition = GKGMath.point3DToAxisConstantValues(preProcessingStartPoint),
                                EndPosition = GKGMath.point3DToAxisConstantValues(openPoint),
                                Speed = straightLineItem.InProcessingParameters.Value.ProcessingSpeed,
                                Acceleration = initParameters.ProcessingAcceleration
                            });
                            // 更新预处理起始点为开阀点，供下一个轨迹使用
                            preProcessingStartPoint = openPoint;
                        }
                        // 开阀
                        foreach (var channel in channels)
                        {
                            motionInstructionList.Add(new Buffer2DComparePulse { Channel = (short)channel, StartLevel = 1 });
                        }
                        // 运动到到结束点
                        motionInstructionList.Add(new StraightLine
                        {
                            StartPosition = GKGMath.point3DToAxisConstantValues(preProcessingStartPoint),
                            EndPosition = GKGMath.point3DToAxisConstantValues(endPoint),
                            Speed = straightLineItem.InProcessingParameters.Value.ProcessingSpeed,
                            Acceleration = initParameters.ProcessingAcceleration
                        });

                        // 更新起始点为结束点，供下一个轨迹使用
                        startPoint = endPoint;
                    }
                    break;

                //处理A类圆弧轨迹
                case LinearMotionTrajectoryItemType.ArcA:
                    {
                        LinearMotionTrajectoryItemArcA arcALineItem = linearMotionTrajectory.ItemBase as LinearMotionTrajectoryItemArcA;
                        if (arcALineItem?.MiddlePoint?.Position?.Length < 3 ||
                            arcALineItem?.EndPoint?.Position?.Length < 3)
                        {
                            throw new GKGException(MotionCalculateErrCodeConsts.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect);
                        }

                        // 获取圆弧中点和终点
                        Point3D middlePoint = new Point3D(arcALineItem.MiddlePoint.Position[0].PositionValue, arcALineItem.MiddlePoint.Position[1].PositionValue, arcALineItem.MiddlePoint.Position[2].PositionValue);
                        Point3D endPoint = new Point3D(arcALineItem.EndPoint.Position[0].PositionValue, arcALineItem.EndPoint.Position[1].PositionValue, arcALineItem.EndPoint.Position[2].PositionValue);

                        // 计算预处理起始点 根据圆弧方向，圆弧半径，提前距离计算
                        Point3D preProcessingStartPoint = GKGMath.CalculatePrePoint(startPoint, middlePoint, endPoint, prePostProcessingParameters.AdvanceDistance);

                        // 移动到提前点
                        motionInstructionList.Add(new StraightLine
                        {
                            EndPosition = GKGMath.point3DToAxisConstantValues(preProcessingStartPoint),
                            Speed = initParameters.NonProcessingSpeed,
                            Acceleration = initParameters.ProcessingAcceleration
                        });

                        // 计算开阀点
                        if (prePostProcessingParameters.AdvanceDistance > prePostProcessingParameters.AdvanceOpenDistance)
                        {
                            // 提前距离大于提前开阀距离 ，计算开阀点
                            Point3D openPoint = GKGMath.CalculatePrePoint(startPoint, middlePoint, endPoint, prePostProcessingParameters.AdvanceOpenDistance);

                            // 计算提前点和开阀点中间的一点，方便组圆弧轨迹
                            Point3D preOpenPoint = GKGMath.CalculatePrePoint(startPoint, middlePoint, endPoint, (prePostProcessingParameters.AdvanceDistance + prePostProcessingParameters.AdvanceOpenDistance) / 2);

                            // 提前点到开阀点之间的圆弧轨迹
                            motionInstructionList.Add(new ArcA
                            {
                                StartPosition = GKGMath.point3DToAxisConstantValues(preProcessingStartPoint),
                                MiddlePosition = GKGMath.point3DToAxisConstantValues(preOpenPoint),
                                EndPosition = GKGMath.point3DToAxisConstantValues(openPoint),
                                Speed = arcALineItem.InProcessingParameters.Value.ProcessingSpeed,
                                Acceleration = initParameters.ProcessingAcceleration,
                            });

                            // 更新预处理起始点为开阀点，供下一个轨迹使用
                            preProcessingStartPoint = openPoint;
                        }
                        // 开阀
                        foreach (var channel in channels)
                        {
                            motionInstructionList.Add(new Buffer2DComparePulse { Channel = (short)channel, StartLevel = 1 });
                        }
                        // 原本的圆弧轨迹，从开阀点到结束点
                        motionInstructionList.Add(new ArcA
                        {
                            StartPosition = GKGMath.point3DToAxisConstantValues(preProcessingStartPoint),
                            MiddlePosition = GKGMath.point3DToAxisConstantValues(middlePoint),
                            EndPosition = GKGMath.point3DToAxisConstantValues(endPoint),
                            Speed = arcALineItem.InProcessingParameters.Value.ProcessingSpeed,
                            Acceleration = initParameters.ProcessingAcceleration,
                        });
                        startPoint = endPoint;
                    }
                    break;

                // 处理A类圆轨迹
                case LinearMotionTrajectoryItemType.CircleA:
                    {
                        LinearMotionTrajectoryItemCircleA circleALineItem = linearMotionTrajectory.ItemBase as LinearMotionTrajectoryItemCircleA;
                        if (circleALineItem?.MiddlePoint?.Position?.Length < 3 ||
                            circleALineItem?.EndPoint?.Position?.Length < 3)
                        {
                            throw new GKGException(MotionCalculateErrCodeConsts.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect);
                        }
                        // 获取圆弧中点和终点
                        Point3D middlePoint = new Point3D(circleALineItem.MiddlePoint.Position[0].PositionValue, circleALineItem.MiddlePoint.Position[1].PositionValue, circleALineItem.MiddlePoint.Position[2].PositionValue);
                        Point3D endPoint = new Point3D(circleALineItem.EndPoint.Position[0].PositionValue, circleALineItem.EndPoint.Position[1].PositionValue, circleALineItem.EndPoint.Position[2].PositionValue);
                        // 计算预处理起始点 根据圆弧方向，圆弧半径，提前距离计算
                        Point3D preProcessingStartPoint = GKGMath.CalculatePrePoint(startPoint, middlePoint, endPoint, prePostProcessingParameters.AdvanceDistance);
                        // 移动到提前点
                        motionInstructionList.Add(new StraightLine
                        {
                            EndPosition = GKGMath.point3DToAxisConstantValues(preProcessingStartPoint),
                            Speed = initParameters.NonProcessingSpeed,
                            Acceleration = initParameters.ProcessingAcceleration
                        });

                        // 计算开阀点
                        if (prePostProcessingParameters.AdvanceDistance > prePostProcessingParameters.AdvanceOpenDistance)
                        {
                            // 提前距离大于提前开阀距离 ，计算开阀点
                            Point3D openPoint = GKGMath.CalculatePrePoint(startPoint, middlePoint, endPoint, prePostProcessingParameters.AdvanceOpenDistance);

                            // 计算提前点和开阀点中间的一点，方便组圆弧轨迹
                            Point3D preOpenPoint = GKGMath.CalculatePrePoint(startPoint, middlePoint, endPoint, (prePostProcessingParameters.AdvanceDistance + prePostProcessingParameters.AdvanceOpenDistance) / 2);

                            // 提前点到开阀点之间的圆弧轨迹
                            motionInstructionList.Add(new ArcA
                            {
                                StartPosition = GKGMath.point3DToAxisConstantValues(preProcessingStartPoint),
                                MiddlePosition = GKGMath.point3DToAxisConstantValues(preOpenPoint),
                                EndPosition = GKGMath.point3DToAxisConstantValues(openPoint),
                                Speed = circleALineItem.InProcessingParameters.Value.ProcessingSpeed,
                                Acceleration = initParameters.ProcessingAcceleration,
                            });

                            // 更新预处理起始点为开阀点，供下一个轨迹使用
                            preProcessingStartPoint = openPoint;
                        }
                        // 开阀
                        foreach (var channel in channels)
                        {
                            motionInstructionList.Add(new Buffer2DComparePulse { Channel = (short)channel, StartLevel = 1 });
                        }
                        // 原本的圆轨迹，从开阀点到结束点
                        motionInstructionList.Add(new Circle
                        {
                            // 圆的起始点坐标就是结束点坐标
                            EndPosition = GKGMath.point3DToAxisConstantValues(startPoint),
                            MiddlePosition = GKGMath.point3DToAxisConstantValues(middlePoint),
                            Speed = circleALineItem.InProcessingParameters.Value.ProcessingSpeed,
                            Acceleration = initParameters.ProcessingAcceleration,
                        });
                    }
                    break;
            }

            return motionInstructionList;
        }

        /// <summary>
        /// 计算加工轨迹后处理轨迹
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="linearMotionTrajectory"></param>
        /// <param name="prePostProcessingParameters"></param>
        /// <param name="channels"></param>
        /// <returns></returns>
        /// <exception cref="GKGException"></exception>
        private List<MotionInstructionBase> CalculatePostProcessingTrajectory(Point3D startPoint, LinearMotionTrajectoryItem linearMotionTrajectory, DispensePrePostProcessingParameters prePostProcessingParameters, int[] channels)
        {
            // 计算加工后轨迹
            List<MotionInstructionBase> motionInstructionList = new List<MotionInstructionBase>();

            // 计算加工前轨迹
            switch (linearMotionTrajectory.ItemType)
            {
                // 处理直线轨迹
                case LinearMotionTrajectoryItemType.StraightLine:
                    {
                        LinearMotionTrajectoryItemStraightLine straightLineItem = linearMotionTrajectory.ItemBase as LinearMotionTrajectoryItemStraightLine;
                        if (straightLineItem?.EndPoint?.Position?.Length < 3)
                        {
                            throw new GKGException(MotionCalculateErrCodeConsts.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect);
                        }
                        // 计算起始点和终点
                        Point3D endPoint = new Point3D(straightLineItem.EndPoint.Position[0].PositionValue, straightLineItem.EndPoint.Position[1].PositionValue, straightLineItem.EndPoint.Position[2].PositionValue);

                        // 计算方向向量
                        Point3D direction = new Point3D(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y, endPoint.Z - startPoint.Z);

                        // 计算长度
                        double length = Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y + direction.Z * direction.Z);

                        // 归一化方向向量
                        direction.X /= length;
                        direction.Y /= length;
                        direction.Z /= length;

                        // 计算预处理结束点
                        Point3D postProcessingEndPoint = new Point3D(
                            endPoint.X + direction.X * prePostProcessingParameters.AdvanceDistance,
                            endPoint.Y + direction.Y * prePostProcessingParameters.AdvanceDistance,
                            endPoint.Z + direction.Z * prePostProcessingParameters.AdvanceDistance
                            );

                        // 组直线轨迹

                        // 正常情况，关阀点就是结束点
                        Point3D closePoint = endPoint;
                        // 不同于提前开阀，延后关阀可以是负距离，表示提前关阀，所以这里分两种情况计算关阀点
                        // 延后距离不能（不会）小于0
                        // 计算关阀点
                        if (prePostProcessingParameters.DelayCloseDistance > 0)
                        {
                            // 延后关阀距离大于0，属于正常延后关阀，计算关阀点

                            if (prePostProcessingParameters.DelayCloseDistance < prePostProcessingParameters.AdvanceDistance)
                            {
                                // 延后关阀距离小于延后距离，说明关阀点在结束点和预处理结束点之间
                                closePoint = new Point3D(
                                endPoint.X + direction.X * prePostProcessingParameters.DelayCloseDistance,
                                endPoint.Y + direction.Y * prePostProcessingParameters.DelayCloseDistance,
                                endPoint.Z + direction.Z * prePostProcessingParameters.DelayCloseDistance);
                            }
                            else
                            {
                                // 延后关阀距离大于等于延后距离，说明关阀点就是预处理结束点
                                closePoint = postProcessingEndPoint;
                            }
                        }
                        else
                        {
                            // 延后关阀距离小于等于0，属于提前关阀，计算关阀点
                            if (GKGMath.CalculateDistance(startPoint, endPoint) > Math.Abs(prePostProcessingParameters.DelayCloseDistance))
                            {
                                // 起始点到结束点距离大于提前关阀距离，说明关阀点在起始点和结束点之间
                                closePoint = new Point3D(
                                endPoint.X + direction.X * prePostProcessingParameters.DelayCloseDistance,
                                endPoint.Y + direction.Y * prePostProcessingParameters.DelayCloseDistance,
                                endPoint.Z + direction.Z * prePostProcessingParameters.DelayCloseDistance);
                            }
                            else
                            {
                                // 需要报错，提前关阀距离过大，无法实现
                                throw new GKGException(MotionCalculateErrCodeConsts.MotionCalculateErrDelayCloseTooMin, MotionCalculateErr.MotionCalculateErrDelayCloseTooMin, MotionCalculateErr.MotionCalculateErrDelayCloseTooMin);
                            }
                        }
                        // 组关阀轨迹
                        motionInstructionList.Add(new StraightLine
                        {
                            StartPosition = GKGMath.point3DToAxisConstantValues(startPoint),
                            EndPosition = GKGMath.point3DToAxisConstantValues(closePoint),
                            Speed = straightLineItem.InProcessingParameters.Value.ProcessingSpeed,
                            Acceleration = initParameters.ProcessingAcceleration
                        });
                        // 关阀
                        foreach (var channel in channels)
                        {
                            motionInstructionList.Add(new Buffer2DComparePulse { Channel = (short)channel, StartLevel = 0 });
                        }

                        // 如果关阀点不等于预处理结束点，则需要继续运动到预处理结束点
                        if (closePoint != postProcessingEndPoint)
                        {
                            motionInstructionList.Add(new StraightLine
                            {
                                StartPosition = GKGMath.point3DToAxisConstantValues(closePoint),
                                EndPosition = GKGMath.point3DToAxisConstantValues(postProcessingEndPoint),
                                Speed = straightLineItem.InProcessingParameters.Value.ProcessingSpeed,
                                Acceleration = initParameters.ProcessingAcceleration
                            });
                        }
                    }
                    break;

                //处理A类圆弧轨迹
                case LinearMotionTrajectoryItemType.ArcA:
                    {
                        LinearMotionTrajectoryItemArcA arcALineItem = linearMotionTrajectory.ItemBase as LinearMotionTrajectoryItemArcA;
                        if (arcALineItem?.MiddlePoint?.Position?.Length < 3 ||
                            arcALineItem?.EndPoint?.Position?.Length < 3)
                        {
                            throw new GKGException(MotionCalculateErrCodeConsts.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect);
                        }

                        // 获取圆弧中点和终点
                        Point3D middlePoint = new Point3D(arcALineItem.MiddlePoint.Position[0].PositionValue, arcALineItem.MiddlePoint.Position[1].PositionValue, arcALineItem.MiddlePoint.Position[2].PositionValue);
                        Point3D endPoint = new Point3D(arcALineItem.EndPoint.Position[0].PositionValue, arcALineItem.EndPoint.Position[1].PositionValue, arcALineItem.EndPoint.Position[2].PositionValue);

                        // 计算预处理起始点 根据圆弧方向，圆弧半径，提前距离计算
                        Point3D postProcessingEndPoint = GKGMath.CalculatePostPoint(startPoint, middlePoint, endPoint, prePostProcessingParameters.DelayDistance);

                        // 正常情况，关阀点就是结束点
                        Point3D closePoint = endPoint;

                        // 延后点和关阀点中间的一个点
                        Point3D postClosePoint = postProcessingEndPoint;

                        // 不同于提前开阀，延后关阀可以是负距离，表示提前关阀，所以这里分两种情况计算关阀点
                        // 延后距离不能（不会）小于0
                        // 计算开阀点
                        if (prePostProcessingParameters.DelayCloseDistance > 0)
                        {
                            // 延后关阀距离大于0，属于正常延后关阀，计算关阀点

                            if (prePostProcessingParameters.DelayCloseDistance < prePostProcessingParameters.AdvanceDistance)
                            {
                                // 延后关阀距离小于延后距离，说明关阀点在结束点和预处理结束点之间
                                closePoint = GKGMath.CalculatePostPoint(startPoint, middlePoint, endPoint, prePostProcessingParameters.DelayCloseDistance);
                                postClosePoint = GKGMath.CalculatePostPoint(startPoint, middlePoint, endPoint, (prePostProcessingParameters.DelayCloseDistance + prePostProcessingParameters.DelayDistance) / 2);
                            }
                            else
                            {
                                // 延后关阀距离大于等于延后距离，说明关阀点就是预处理结束点
                                closePoint = postProcessingEndPoint;
                            }
                        }
                        else
                        {
                            double arcLength = GKGMath.CalculateArcLength(startPoint, middlePoint, endPoint);
                            // 延后关阀距离小于等于0，属于提前关阀，计算关阀点
                            if (arcLength > Math.Abs(prePostProcessingParameters.DelayCloseDistance))
                            {
                                // 起始点到结束点距离大于提前关阀距离，说明关阀点在起始点和结束点之间
                                closePoint = GKGMath.CalculatePostPoint(startPoint, middlePoint, endPoint, prePostProcessingParameters.DelayCloseDistance);
                                postClosePoint = GKGMath.CalculatePostPoint(startPoint, middlePoint, endPoint, (prePostProcessingParameters.DelayCloseDistance + prePostProcessingParameters.DelayDistance) / 2);
                                middlePoint = GKGMath.CalculatePostPoint(startPoint, middlePoint, endPoint, (prePostProcessingParameters.DelayCloseDistance - arcLength) / 2);
                                postProcessingEndPoint = endPoint;
                            }
                            else
                            {
                                // 需要报错，提前关阀距离过大，无法实现
                                throw new GKGException(MotionCalculateErrCodeConsts.MotionCalculateErrDelayCloseTooMin, MotionCalculateErr.MotionCalculateErrDelayCloseTooMin, MotionCalculateErr.MotionCalculateErrDelayCloseTooMin);
                            }
                        }
                        motionInstructionList.Add(new ArcA
                        {
                            StartPosition = GKGMath.point3DToAxisConstantValues(startPoint),
                            MiddlePosition = GKGMath.point3DToAxisConstantValues(middlePoint),
                            EndPosition = GKGMath.point3DToAxisConstantValues(closePoint),
                            Speed = arcALineItem.InProcessingParameters.Value.ProcessingSpeed,
                            Acceleration = initParameters.ProcessingAcceleration,
                        });
                        // 关阀
                        foreach (var channel in channels)
                        {
                            motionInstructionList.Add(new Buffer2DComparePulse { Channel = (short)channel, StartLevel = 0 });
                        }

                        if (closePoint != postProcessingEndPoint)
                        {
                            // 原本的圆弧轨迹，从关阀点到结束点
                            motionInstructionList.Add(new ArcA
                            {
                                StartPosition = GKGMath.point3DToAxisConstantValues(closePoint),
                                MiddlePosition = GKGMath.point3DToAxisConstantValues(postClosePoint),
                                EndPosition = GKGMath.point3DToAxisConstantValues(postProcessingEndPoint),
                                Speed = arcALineItem.InProcessingParameters.Value.ProcessingSpeed,
                                Acceleration = initParameters.ProcessingAcceleration,
                            });
                        }
                    }
                    break;

                // 处理A类圆轨迹
                case LinearMotionTrajectoryItemType.CircleA:
                    {
                        LinearMotionTrajectoryItemCircleA circleALineItem = linearMotionTrajectory.ItemBase as LinearMotionTrajectoryItemCircleA;
                        if (circleALineItem?.MiddlePoint?.Position?.Length < 3 ||
                            circleALineItem?.EndPoint?.Position?.Length < 3)
                        {
                            throw new GKGException(MotionCalculateErrCodeConsts.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect);
                        }
                        // 获取圆中点和终点
                        Point3D middlePoint = new Point3D(circleALineItem.MiddlePoint.Position[0].PositionValue, circleALineItem.MiddlePoint.Position[1].PositionValue, circleALineItem.MiddlePoint.Position[2].PositionValue);
                        Point3D endPoint = new Point3D(circleALineItem.EndPoint.Position[0].PositionValue, circleALineItem.EndPoint.Position[1].PositionValue, circleALineItem.EndPoint.Position[2].PositionValue);
                        // 计算预处理起始点 根据圆弧方向，圆弧半径，提前距离计算
                        Point3D postProcessingEndPoint = GKGMath.CalculatePostPoint(startPoint, middlePoint, endPoint, prePostProcessingParameters.DelayDistance);

                        // 正常情况，关阀点就是结束点
                        Point3D closePoint = endPoint;

                        // 延后点和关阀点中间的一个点
                        Point3D postClosePoint = postProcessingEndPoint;

                        // 不同于提前开阀，延后关阀可以是负距离，表示提前关阀，所以这里分两种情况计算关阀点
                        // 延后距离不能（不会）小于0
                        // 计算开阀点
                        if (prePostProcessingParameters.DelayCloseDistance > 0)
                        {
                            // 延后关阀距离大于0，属于正常延后关阀，计算关阀点

                            if (prePostProcessingParameters.DelayCloseDistance < prePostProcessingParameters.AdvanceDistance)
                            {
                                // 延后关阀距离小于延后距离，说明关阀点在结束点和预处理结束点之间
                                closePoint = GKGMath.CalculatePostPoint(startPoint, middlePoint, endPoint, prePostProcessingParameters.DelayCloseDistance);
                                postClosePoint = GKGMath.CalculatePostPoint(startPoint, middlePoint, endPoint, (prePostProcessingParameters.DelayCloseDistance + prePostProcessingParameters.DelayDistance) / 2);
                            }
                            else
                            {
                                // 延后关阀距离大于等于延后距离，说明关阀点就是预处理结束点
                                closePoint = postProcessingEndPoint;
                            }
                        }
                        else
                        {
                            double arcLength = GKGMath.CalculateCircleLength(startPoint, middlePoint, endPoint);
                            // 延后关阀距离小于等于0，属于提前关阀，计算关阀点
                            if (arcLength > Math.Abs(prePostProcessingParameters.DelayCloseDistance))
                            {
                                // 起始点到结束点距离大于提前关阀距离，说明关阀点在起始点和结束点之间
                                closePoint = GKGMath.CalculatePostPoint(startPoint, middlePoint, endPoint, prePostProcessingParameters.DelayCloseDistance);
                                postClosePoint = GKGMath.CalculatePostPoint(startPoint, middlePoint, endPoint, (prePostProcessingParameters.DelayCloseDistance + prePostProcessingParameters.DelayDistance) / 2);
                                middlePoint = GKGMath.CalculatePostPoint(startPoint, middlePoint, endPoint, (prePostProcessingParameters.DelayCloseDistance - arcLength) / 2);
                                postProcessingEndPoint = endPoint;
                            }
                            else
                            {
                                // 需要报错，提前关阀距离过大，无法实现
                                throw new GKGException(MotionCalculateErrCodeConsts.MotionCalculateErrDelayCloseTooMin, MotionCalculateErr.MotionCalculateErrDelayCloseTooMin, MotionCalculateErr.MotionCalculateErrDelayCloseTooMin);
                            }
                        }
                        motionInstructionList.Add(new ArcA
                        {
                            StartPosition = GKGMath.point3DToAxisConstantValues(startPoint),
                            MiddlePosition = GKGMath.point3DToAxisConstantValues(middlePoint),
                            EndPosition = GKGMath.point3DToAxisConstantValues(closePoint),
                            Speed = circleALineItem.InProcessingParameters.Value.ProcessingSpeed,
                            Acceleration = initParameters.ProcessingAcceleration,
                        });
                        // 关阀
                        foreach (var channel in channels)
                        {
                            motionInstructionList.Add(new Buffer2DComparePulse { Channel = (short)channel, StartLevel = 0 });
                        }

                        if (closePoint != postProcessingEndPoint)
                        {
                            // 原本的圆弧轨迹，从关阀点到结束点
                            motionInstructionList.Add(new ArcA
                            {
                                StartPosition = GKGMath.point3DToAxisConstantValues(closePoint),
                                MiddlePosition = GKGMath.point3DToAxisConstantValues(postClosePoint),
                                EndPosition = GKGMath.point3DToAxisConstantValues(postProcessingEndPoint),
                                Speed = circleALineItem.InProcessingParameters.Value.ProcessingSpeed,
                                Acceleration = initParameters.ProcessingAcceleration,
                            });
                        }
                    }
                    break;
            }

            return motionInstructionList;
        }

        /// <summary>
        /// 计算回走回拉轨迹
        /// </summary>
        /// <param name="motionInstructions"></param>
        /// <param name="prePostProcessingParameters"></param>
        /// <exception cref="GKGException"></exception>
        private void CalculateGoBackProcessingTrajectory(ref List<MotionInstructionBase> motionInstructions, DispensePrePostProcessingParameters prePostProcessingParameters)
        {
            List<MotionInstructionBase> goBackMotionInstructions = new List<MotionInstructionBase>();
            double gobackDistance = prePostProcessingParameters.GoBackDistance;
            Point3D lastPoint = new Point3D();
            for (int i = motionInstructions.Count - 1; i >= 0; i--)
            {
                if (gobackDistance <= 0)
                    break;
                switch (motionInstructions[i].InstructionType)
                {
                    case MotionInstructionType.Linear:
                        {
                            StraightLine straightLine = motionInstructions[i] as StraightLine;
                            if (straightLine != null && straightLine.StartPosition != null)
                            {
                                // 计算回退点 // 倒着取点
                                Point3D startPoint = new Point3D(straightLine.EndPosition[0].PositionValue, straightLine.EndPosition[1].PositionValue, straightLine.EndPosition[2].PositionValue);
                                Point3D endPoint = new Point3D(straightLine.StartPosition[0].PositionValue, straightLine.StartPosition[1].PositionValue, straightLine.StartPosition[2].PositionValue);
                                // 计算长度
                                double length = GKGMath.CalculateDistance(startPoint, endPoint);

                                // 计算方向向量
                                Point3D direction = new Point3D(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y, endPoint.Z - startPoint.Z);
                                // 计算长度
                                double directionLength = Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y + direction.Z * direction.Z);
                                // 归一化方向向量
                                direction.X /= directionLength;
                                direction.Y /= directionLength;
                                direction.Z /= directionLength;
                                Point3D goBackPoint = new Point3D();
                                if (gobackDistance < length)
                                {
                                    // 计算回退点
                                    goBackPoint = new Point3D(
                                        endPoint.X - direction.X * gobackDistance,
                                        endPoint.Y - direction.Y * gobackDistance,
                                        endPoint.Z - direction.Z * gobackDistance
                                    );
                                }
                                else
                                {
                                    goBackPoint = endPoint;
                                }

                                // 提高高度（回走高度）
                                startPoint.Z -= prePostProcessingParameters.GoBackHeight;
                                goBackPoint.Z = goBackPoint.Z - prePostProcessingParameters.GoBackHeight;

                                lastPoint = goBackPoint;
                                // 更新剩余回退距离
                                gobackDistance -= length;
                                // 插入回退轨迹
                                goBackMotionInstructions.Add(new StraightLine
                                {
                                    StartPosition = GKGMath.point3DToAxisConstantValues(startPoint),
                                    EndPosition = GKGMath.point3DToAxisConstantValues(goBackPoint),
                                    Speed = prePostProcessingParameters.GoBackSpeed,
                                    Acceleration = initParameters.Acceleration
                                });
                            }
                        }
                        break;

                    case MotionInstructionType.ArcA:
                        {
                            ArcA arcA = motionInstructions[i] as ArcA;
                            if (arcA != null)
                            {
                                // 计算回退点 // 倒着取点
                                Point3D startPoint = new Point3D(arcA.EndPosition[0].PositionValue, arcA.EndPosition[1].PositionValue, arcA.EndPosition[2].PositionValue);
                                Point3D middlePoint = new Point3D(arcA.MiddlePosition[0].PositionValue, arcA.MiddlePosition[1].PositionValue, arcA.MiddlePosition[2].PositionValue);
                                Point3D endPoint = new Point3D(arcA.StartPosition[0].PositionValue, arcA.StartPosition[1].PositionValue, arcA.StartPosition[2].PositionValue);
                                double arcLength = GKGMath.CalculateArcLength(startPoint, middlePoint, endPoint);
                                Point3D goBackPoint = new Point3D();
                                // 更新剩余回退距离
                                if (gobackDistance < arcLength)
                                {
                                    // 计算回退点
                                    goBackPoint = GKGMath.CalculatePostPoint(startPoint, middlePoint, endPoint, -gobackDistance);
                                }
                                else
                                {
                                    // 回退整个圆弧
                                    goBackPoint = endPoint;
                                }

                                // 更新剩余回退距离
                                gobackDistance -= arcLength;

                                // 提高高度（回走高度）
                                startPoint.Z -= prePostProcessingParameters.GoBackHeight;
                                middlePoint.Z -= prePostProcessingParameters.GoBackHeight;
                                goBackPoint.Z -= prePostProcessingParameters.GoBackHeight;

                                lastPoint = goBackPoint;

                                // 插入回退轨迹
                                goBackMotionInstructions.Add(new ArcA
                                {
                                    StartPosition = GKGMath.point3DToAxisConstantValues(startPoint),
                                    MiddlePosition = GKGMath.point3DToAxisConstantValues(middlePoint),
                                    EndPosition = GKGMath.point3DToAxisConstantValues(goBackPoint),
                                    Speed = prePostProcessingParameters.GoBackSpeed,
                                    Acceleration = initParameters.Acceleration
                                });
                            }
                        }
                        break;

                    case MotionInstructionType.Circle:
                        {
                            Circle circle = motionInstructions[i] as Circle;
                            if (circle != null)
                            {
                                // 计算回退点 // 倒着取点
                                Point3D startPoint = new Point3D(circle.EndPosition[0].PositionValue, circle.EndPosition[1].PositionValue, circle.EndPosition[2].PositionValue);
                                Point3D middlePoint = new Point3D(circle.MiddlePosition[0].PositionValue, circle.MiddlePosition[1].PositionValue, circle.MiddlePosition[2].PositionValue);
                                Point3D endPoint = new Point3D();

                                // 需要向前寻找圆弧的起点
                                int j = i - 1;
                                bool findEndPoint = false;
                                while (j >= 0)
                                {
                                    switch (motionInstructions[i].InstructionType)
                                    {
                                        case MotionInstructionType.Linear:
                                            {
                                                endPoint = new Point3D((motionInstructions[j] as StraightLine).EndPosition[0].PositionValue,
                                                    (motionInstructions[j] as StraightLine).EndPosition[1].PositionValue,
                                                    (motionInstructions[j] as StraightLine).EndPosition[2].PositionValue);
                                                findEndPoint = true;
                                            }
                                            break;

                                        case MotionInstructionType.ArcA:
                                            {
                                                endPoint = new Point3D((motionInstructions[j] as ArcA).EndPosition[0].PositionValue,
                                                    (motionInstructions[j] as ArcA).EndPosition[1].PositionValue,
                                                    (motionInstructions[j] as ArcA).EndPosition[2].PositionValue);
                                                findEndPoint = true;
                                            }
                                            break;

                                        case MotionInstructionType.Circle:
                                            {
                                                endPoint = new Point3D((motionInstructions[j] as Circle).EndPosition[0].PositionValue,
                                                    (motionInstructions[j] as Circle).EndPosition[1].PositionValue,
                                                    (motionInstructions[j] as Circle).EndPosition[2].PositionValue);
                                                findEndPoint = true;
                                            }
                                            break;

                                        default:
                                            break;
                                    }
                                    if (findEndPoint)
                                        break;
                                    j--;
                                }
                                if (!findEndPoint)
                                {
                                    throw new GKGException(MotionCalculateErrCodeConsts.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect);
                                }
                                double arcLength = GKGMath.CalculateCircleLength(startPoint, middlePoint, endPoint);
                                Point3D goBackPoint = new Point3D();
                                // 更新剩余回退距离
                                if (gobackDistance < arcLength)
                                {
                                    // 计算回退点
                                    goBackPoint = GKGMath.CalculatePostPoint(startPoint, middlePoint, endPoint, -gobackDistance);
                                }
                                else
                                {
                                    // 回退整个圆弧
                                    goBackPoint = endPoint;
                                }
                                // 更新剩余回退距离
                                gobackDistance -= arcLength;
                                // 提高高度（回走高度）
                                startPoint.Z -= prePostProcessingParameters.GoBackHeight;
                                middlePoint.Z -= prePostProcessingParameters.GoBackHeight;
                                goBackPoint.Z -= prePostProcessingParameters.GoBackHeight;

                                lastPoint = goBackPoint;

                                // 插入回退轨迹
                                goBackMotionInstructions.Add(new Circle
                                {
                                    EndPosition = GKGMath.point3DToAxisConstantValues(startPoint),
                                    MiddlePosition = GKGMath.point3DToAxisConstantValues(middlePoint),
                                    Speed = prePostProcessingParameters.GoBackSpeed,
                                    Acceleration = initParameters.Acceleration
                                });
                            }
                        }
                        break;

                    default:
                        break;
                }
            }

            // 回抬
            if (prePostProcessingParameters.GoBackHeight > 0)
            {
                lastPoint.Z -= prePostProcessingParameters.GoLiftHeight;
                goBackMotionInstructions.Add(new StraightLine
                {
                    EndPosition = GKGMath.point3DToAxisConstantValues(lastPoint),
                    Speed = prePostProcessingParameters.GoLiftSpeed,
                    Acceleration = initParameters.Acceleration
                });
            }
            motionInstructions.AddRange(goBackMotionInstructions);
        }

        /// <summary>
        /// 计算单个加工轨迹
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="linearMotionTrajectory"></param>
        /// <param name="channels"></param>
        /// <returns></returns>
        /// <exception cref="GKGException"></exception>
        private List<MotionInstructionBase> CalculateProcessingTrajectory(ref Point3D startPoint, LinearMotionTrajectoryItem linearMotionTrajectory, int[] channels)
        {
            // 计算加工轨迹
            List<MotionInstructionBase> motionInstructionList = new List<MotionInstructionBase>();
            // 计算加工前轨迹
            switch (linearMotionTrajectory.ItemType)
            {
                // 处理直线轨迹
                case LinearMotionTrajectoryItemType.StraightLine:
                    {
                        LinearMotionTrajectoryItemStraightLine straightLineItem = linearMotionTrajectory.ItemBase as LinearMotionTrajectoryItemStraightLine;
                        if (straightLineItem?.EndPoint?.Position?.Length < 3 || straightLineItem?.InProcessingParameters == null)
                        {
                            throw new GKGException(MotionCalculateErrCodeConsts.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect);
                        }
                        // 计算起始点和终点
                        Point3D endPoint = new Point3D(straightLineItem.EndPoint.Position[0].PositionValue, straightLineItem.EndPoint.Position[1].PositionValue, straightLineItem.EndPoint.Position[2].PositionValue);
                        // 组直线轨迹
                        motionInstructionList.Add(new StraightLine
                        {
                            StartPosition = GKGMath.point3DToAxisConstantValues(startPoint),
                            EndPosition = straightLineItem.EndPoint.Position,
                            Speed = straightLineItem.InProcessingParameters.Value.ProcessingSpeed,
                            Acceleration = initParameters.ProcessingAcceleration
                        });
                        startPoint = endPoint;
                    }
                    break;
                //处理A类圆弧轨迹
                case LinearMotionTrajectoryItemType.ArcA:
                    {
                        LinearMotionTrajectoryItemArcA arcALineItem = linearMotionTrajectory.ItemBase as LinearMotionTrajectoryItemArcA;
                        if (arcALineItem?.MiddlePoint?.Position?.Length < 3 ||
                            arcALineItem?.EndPoint?.Position?.Length < 3 ||
                            arcALineItem?.InProcessingParameters == null)
                        {
                            throw new GKGException(MotionCalculateErrCodeConsts.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect);
                        }
                        // 获取圆弧中点和终点
                        Point3D endPoint = new Point3D(arcALineItem.EndPoint.Position[0].PositionValue, arcALineItem.EndPoint.Position[1].PositionValue, arcALineItem.EndPoint.Position[2].PositionValue);
                        // 组圆弧轨迹
                        motionInstructionList.Add(new ArcA
                        {
                            StartPosition = GKGMath.point3DToAxisConstantValues(startPoint),
                            MiddlePosition = arcALineItem.MiddlePoint.Position,
                            EndPosition = arcALineItem.EndPoint.Position,
                            Speed = arcALineItem.InProcessingParameters.Value.ProcessingSpeed,
                            Acceleration = initParameters.ProcessingAcceleration,
                        });
                        startPoint = endPoint;
                    }
                    break;
                // 处理A类圆轨迹
                case LinearMotionTrajectoryItemType.CircleA:
                    {
                        LinearMotionTrajectoryItemCircleA circleALineItem = linearMotionTrajectory.ItemBase as LinearMotionTrajectoryItemCircleA;
                        if (circleALineItem?.MiddlePoint?.Position?.Length < 3 ||
                            circleALineItem?.EndPoint?.Position?.Length < 3 ||
                            circleALineItem?.InProcessingParameters == null)
                        {
                            throw new GKGException(MotionCalculateErrCodeConsts.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect);
                        }
                        // 组圆轨迹
                        motionInstructionList.Add(new Circle
                        {
                            // 圆的起始点坐标就是结束点坐标
                            EndPosition = GKGMath.point3DToAxisConstantValues(startPoint),
                            MiddlePosition = circleALineItem.MiddlePoint.Position,
                            Speed = circleALineItem.InProcessingParameters.Value.ProcessingSpeed,
                            Acceleration = initParameters.ProcessingAcceleration,
                        });
                    }
                    break;
            }
            return motionInstructionList;
        }

        /// <summary>
        /// 计算连续加工轨迹
        /// </summary>
        /// <param name="processingTrajectory"></param>
        /// <param name="channels"></param>
        /// <returns></returns>
        /// <exception cref="GKGException"></exception>
        private List<MotionInstructionBase> CalculateProcessingTrajectory(ProcessingTrajectory processingTrajectory, int[] channels)
        {
            if (processingTrajectory.MotionTrajectory == null)
            {
                throw new GKGException(MotionCalculateErrCodeConsts.MotionCalculateErrParamsIsNull, MotionCalculateErr.MotionCalculateErrParamsIsNull, MotionCalculateErr.MotionCalculateErrParamsIsNull);
            }
            // 如果是点的话就不会是加工轨迹
            if (processingTrajectory.MotionTrajectory.TrajectoryType == MotionTrajectoryType.Dot)
            {
                throw new GKGException(MotionCalculateErrCodeConsts.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect);
            }
            else
            {
                // 线计算轨迹
                LinearMotionTrajectory linearMotionTrajectory = processingTrajectory.MotionTrajectory as LinearMotionTrajectory;
                // 加工前后参数解析
                DispensePrePostProcessingParameters prePostProcessingParameters = JsonObjConvert.FromJSonBytes<DispensePrePostProcessingParameters>(processingTrajectory.PrePostProcessingParameters.Value.ExtendedParameters);
                if (linearMotionTrajectory.LinearMotionTrajectoryItems == null)
                {
                    throw new GKGException(MotionCalculateErrCodeConsts.MotionCalculateErrParamsIsNull, MotionCalculateErr.MotionCalculateErrParamsIsNull, MotionCalculateErr.MotionCalculateErrParamsIsNull);
                }

                List<MotionInstructionBase> motionInstructionList = new List<MotionInstructionBase>();

                // 获取起始点
                Point3D StartPoint = new Point3D(linearMotionTrajectory.StartPoint[0].PositionValue, linearMotionTrajectory.StartPoint[1].PositionValue, linearMotionTrajectory.StartPoint[2].PositionValue);

                for (int i = 0; i < linearMotionTrajectory.LinearMotionTrajectoryItems.Length; i++)
                {
                    if (i == 0)
                    {
                        // 第一个点，起始点为轨迹起始点
                        motionInstructionList.AddRange(CalculatePreProcessingTrajectory(ref StartPoint, linearMotionTrajectory.LinearMotionTrajectoryItems[i], prePostProcessingParameters, channels));
                    }
                    else if (i == linearMotionTrajectory.LinearMotionTrajectoryItems.Length - 1)
                    {
                        // 最后一个点，起始点为上一个轨迹终点
                        motionInstructionList.AddRange(CalculatePostProcessingTrajectory(StartPoint, linearMotionTrajectory.LinearMotionTrajectoryItems[i], prePostProcessingParameters, channels));
                    }
                    else
                    {
                        // 其他点，起始点为上一个轨迹终点
                        motionInstructionList.AddRange(CalculateProcessingTrajectory(ref StartPoint, linearMotionTrajectory.LinearMotionTrajectoryItems[i], channels));
                    }
                }

                // 计算回走回拉轨迹
                CalculateGoBackProcessingTrajectory(ref motionInstructionList, prePostProcessingParameters);
                return motionInstructionList;
            }
        }

        /// <summary>
        /// 计算单个非加工轨迹
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="linearMotionTrajectoryItem"></param>
        /// <param name="nonProcessingTrajectoryParameters"></param>
        /// <returns></returns>
        /// <exception cref="GKGException"></exception>
        private List<MotionInstructionBase> CalculateNoProcessingTrajectory(ref Point3D startPoint, LinearMotionTrajectoryItem linearMotionTrajectoryItem, NonProcessingTrajectoryParameters nonProcessingTrajectoryParameters)
        {
            List<MotionInstructionBase> motionInstructionList = new List<MotionInstructionBase>();

            // 计算不加工轨迹
            switch (linearMotionTrajectoryItem.ItemType)
            {
                // 非加工直线轨迹
                case LinearMotionTrajectoryItemType.StraightLine:
                    {
                        LinearMotionTrajectoryItemStraightLine straightLineItem = linearMotionTrajectoryItem.ItemBase as LinearMotionTrajectoryItemStraightLine;
                        if (straightLineItem?.EndPoint?.Position?.Length < 3 ||
                            straightLineItem.InProcessingParameters == null)
                        {
                            throw new GKGException(MotionCalculateErrCodeConsts.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect);
                        }
                        // 计算终点
                        Point3D endPoint = new Point3D(straightLineItem.EndPoint.Position[0].PositionValue, straightLineItem.EndPoint.Position[1].PositionValue, straightLineItem.EndPoint.Position[2].PositionValue);
                        // 组直线轨迹
                        motionInstructionList.Add(new StraightLine
                        {
                            StartPosition = GKGMath.point3DToAxisConstantValues(startPoint),
                            EndPosition = straightLineItem.EndPoint.Position,
                            Speed = nonProcessingTrajectoryParameters.MaxSpeed,
                            Acceleration = nonProcessingTrajectoryParameters.Acceleration
                        });
                        startPoint = endPoint;
                    }
                    break;
                // 非加工A类圆弧轨迹
                case LinearMotionTrajectoryItemType.ArcA:
                    {
                        LinearMotionTrajectoryItemArcA arcALineItem = linearMotionTrajectoryItem.ItemBase as LinearMotionTrajectoryItemArcA;
                        if (arcALineItem?.MiddlePoint?.Position?.Length < 3 ||
                            arcALineItem?.EndPoint?.Position?.Length < 3 ||
                            arcALineItem?.InProcessingParameters == null)
                        {
                            throw new GKGException(MotionCalculateErrCodeConsts.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect);
                        }
                        // 获取圆弧中点和终点
                        Point3D middlePoint = new Point3D(arcALineItem.MiddlePoint.Position[0].PositionValue, arcALineItem.MiddlePoint.Position[1].PositionValue, arcALineItem.MiddlePoint.Position[2].PositionValue);
                        Point3D endPoint = new Point3D(arcALineItem.EndPoint.Position[0].PositionValue, arcALineItem.EndPoint.Position[1].PositionValue, arcALineItem.EndPoint.Position[2].PositionValue);
                        // 组圆弧轨迹
                        motionInstructionList.Add(new ArcA
                        {
                            StartPosition = GKGMath.point3DToAxisConstantValues(startPoint),
                            MiddlePosition = arcALineItem.MiddlePoint.Position,
                            EndPosition = arcALineItem.EndPoint.Position,
                            Speed = nonProcessingTrajectoryParameters.MaxSpeed,
                            Acceleration = nonProcessingTrajectoryParameters.Acceleration,
                        });
                        startPoint = endPoint;
                    }
                    break;
                // 非加工A类圆轨迹
                case LinearMotionTrajectoryItemType.CircleA:
                    {
                        LinearMotionTrajectoryItemCircleA circleALineItem = linearMotionTrajectoryItem.ItemBase as LinearMotionTrajectoryItemCircleA;
                        if (circleALineItem?.MiddlePoint?.Position?.Length < 3 ||
                            circleALineItem?.EndPoint?.Position?.Length < 3 ||
                            circleALineItem?.InProcessingParameters == null)
                        {
                            throw new GKGException(MotionCalculateErrCodeConsts.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect);
                        }
                        // 获取圆中点和终点
                        Point3D middlePoint = new Point3D(circleALineItem.MiddlePoint.Position[0].PositionValue, circleALineItem.MiddlePoint.Position[1].PositionValue, circleALineItem.MiddlePoint.Position[2].PositionValue);
                        Point3D endPoint = new Point3D(circleALineItem.EndPoint.Position[0].PositionValue, circleALineItem.EndPoint.Position[1].PositionValue, circleALineItem.EndPoint.Position[2].PositionValue);
                        // 组圆轨迹
                        motionInstructionList.Add(new Circle
                        {
                            // 圆的起始点坐标就是结束点坐标
                            EndPosition = GKGMath.point3DToAxisConstantValues(startPoint),
                            MiddlePosition = circleALineItem.MiddlePoint.Position,
                            Speed = nonProcessingTrajectoryParameters.MaxSpeed,
                            Acceleration = nonProcessingTrajectoryParameters.Acceleration,
                        });
                    }
                    break;
            }

            return motionInstructionList;
        }

        /// <summary>
        /// 计算连续非加工轨迹
        /// </summary>
        /// <param name="noProcessingTrajectory"></param>
        /// <returns></returns>
        /// <exception cref="GKGException"></exception>
        private List<MotionInstructionBase> CalculateNoProcessingTrajectory(NonProcessingTrajectory noProcessingTrajectory)
        {
            List<MotionInstructionBase> motionInstructionList = new List<MotionInstructionBase>();

            switch (noProcessingTrajectory.MotionTrajectory.TrajectoryType)
            {
                case MotionTrajectoryType.Dot:
                    {
                        DotMotionTrajectory dotMotionTrajectory = noProcessingTrajectory.MotionTrajectory as DotMotionTrajectory;
                        if (dotMotionTrajectory == null)
                        {
                            throw new GKGException(MotionCalculateErrCodeConsts.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect);
                        }
                        motionInstructionList.Add(new Point
                        {
                            TargetPosition = dotMotionTrajectory.TargetPoint.Position,
                            Speed = initParameters.NonProcessingSpeed,
                            Acceleration = initParameters.Acceleration,
                        });
                    }
                    break;

                case MotionTrajectoryType.Linear:
                    {
                        LinearMotionTrajectory linearMotionTrajectory = noProcessingTrajectory.MotionTrajectory as LinearMotionTrajectory;
                        if (linearMotionTrajectory == null)
                        {
                            throw new GKGException(MotionCalculateErrCodeConsts.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect, MotionCalculateErr.MotionCalculateErrParamsTypeIncorrect);
                        }
                        Point3D startPoint = new Point3D(linearMotionTrajectory.StartPoint[0].PositionValue, linearMotionTrajectory.StartPoint[1].PositionValue, linearMotionTrajectory.StartPoint[2].PositionValue);
                        for (int i = 0; i < linearMotionTrajectory.LinearMotionTrajectoryItems.Length; i++)
                        {
                            motionInstructionList.AddRange(CalculateNoProcessingTrajectory(ref startPoint, linearMotionTrajectory.LinearMotionTrajectoryItems[i], noProcessingTrajectory.NonProcessingParameters.Value));
                        }
                    }
                    break;
            }
            return motionInstructionList;
        }

        /// <summary>
        /// 对参数排序并计算双阀间距和副阀目标位置
        /// </summary>
        /// <param name="functionHeadIDs">功能头ID数组</param>
        /// <param name="parameters">运动计算参数数组</param>
        /// <param name="viceDistance">副阀间距</param>
        /// <param name="viceTargetPosition">副阀目标位置</param>
        private void SortAndPairValves(
            string[] functionHeadIDs,
            ref MotionCalculationParameters[] parameters,
            out Point2D viceDistance,
            out Point2D viceTargetPosition)
        {
            MotionCalculationParametersSort(ref parameters, out Point2D[] valvesSpacing);
            CalculateValvePairing(functionHeadIDs, valvesSpacing, out viceDistance, out viceTargetPosition);
        }

        /// <summary>
        /// 根据双阀与相机偏移量进行配对，返回副阀间距和副阀目标位置
        /// </summary>
        /// <param name="functionHeadIDs">功能头ID数组</param>
        /// <param name="valvesSpacing">阀间距数组</param>
        /// <param name="viceDistance">副阀间距</param>
        /// <param name="viceTargetPosition">副阀目标位置</param>
        private void CalculateValvePairing(
            string[] functionHeadIDs,
            Point2D[] valvesSpacing,
            out Point2D viceDistance,
            out Point2D viceTargetPosition)
        {
            viceDistance = new Point2D();
            viceTargetPosition = new Point2D(0, 0);

            if (functionHeadIDs.Length > 1)
            {
                viceDistance.X = Math.Abs(initParameters.FunctionHeadOffset[functionHeadIDs[0]].X - initParameters.FunctionHeadOffset[functionHeadIDs[1]].X);
                viceDistance.Y = Math.Abs(initParameters.FunctionHeadOffset[functionHeadIDs[0]].Y - initParameters.FunctionHeadOffset[functionHeadIDs[1]].Y);
                viceTargetPosition.X = valvesSpacing[0].X - viceDistance.X;
                viceTargetPosition.Y = valvesSpacing[0].Y - viceDistance.Y;
            }
        }
    }
}
