using GF_Gereric;

namespace GKG
{
    namespace MotionControl
    {
        public class PlaneCalculator : IBaseMotionCalculator
        {
            public void Init(byte[] initCfg)
            {
                offsetCalibrationResult = new OffsetCalibrationResult();
                JsonObjConvert.PopulateObject(initCfg, offsetCalibrationResult);
            }

            public MotionTrajectory Calculate(MotionCalculationParameters motionCalculationParameters)
            {
                if (motionCalculationParameters == null)
                    throw new Exception();
                MotionTrajectory retMotionTraj = new MotionTrajectory();
                int size = motionCalculationParameters.ProductProcessingTrajectory.Length;
                if (size <= 0)
                    throw new Exception();
                List<MotionInstructionBase> motionInstructions = new List<MotionInstructionBase>();
                int idx = 0;
                retMotionTraj.SequenceType = MotionTrajectorySequenceTypeConstants.StepByStep;
                foreach (var motionItem in motionCalculationParameters.ProductProcessingTrajectory)
                {
                    if (motionItem.IsProcessing == false)
                    {
                        NonProcessingTrajectory nonProcessingTrajectory = motionItem as NonProcessingTrajectory;
                        if (nonProcessingTrajectory == null)
                            throw new Exception();
                        retMotionTraj.ControlParameters = nonProcessingTrajectory.NonProcessingParameters;
                        if (nonProcessingTrajectory.MotionTrajectory.TrajectoryType == MotionTrajectoryType.Dot)
                        {
                            DotMotionTrajectory? dotMotionTrajectory = nonProcessingTrajectory.MotionTrajectory as DotMotionTrajectory;
                            if (dotMotionTrajectory == null)
                                throw new Exception();
                            Point pointMotionInstruction = new Point();
                            NonProcessingTrajectoryParameters? parameters = nonProcessingTrajectory.NonProcessingParameters;
                            if (!parameters.HasValue)
                                throw new Exception();
                            pointMotionInstruction.Acceleration = parameters.Value.Acceleration;
                            pointMotionInstruction.Speed = parameters.Value.MaxSpeed;
                            pointMotionInstruction.TargetPosition = dotMotionTrajectory.TargetPoint.Position;
                            if (offsetCalibrationResult.FunctionHeadId != string.Empty)
                            {
                                pointMotionInstruction.TargetPosition[0].PositionValue += offsetCalibrationResult.OffsetValue.X;
                                pointMotionInstruction.TargetPosition[1].PositionValue += offsetCalibrationResult.OffsetValue.Y;
                            }
                            motionInstructions.Add(pointMotionInstruction);
                        }
                        else
                        {
                            LinearMotionTrajectory? linearMotionTrajectory = nonProcessingTrajectory.MotionTrajectory as LinearMotionTrajectory;
                            AxisConstantValues[] startPoint = linearMotionTrajectory.StartPoint;
                            NonProcessingTrajectoryParameters? parameters = nonProcessingTrajectory.NonProcessingParameters;
                            if (!parameters.HasValue)
                                throw new Exception();
                            foreach (var lineItem in linearMotionTrajectory.LinearMotionTrajectoryItems)
                            {
                                if (lineItem.ItemType == LinearMotionTrajectoryItemType.StraightLine)
                                {
                                    LinearMotionTrajectoryItemStraightLine? line = lineItem.ItemBase as LinearMotionTrajectoryItemStraightLine;
                                    if (line == null)
                                        throw new Exception();
                                    StraightLine straightLine = new StraightLine();
                                    straightLine.StartPosition = startPoint;
                                    straightLine.EndPosition = line.EndPoint.Position;
                                    if (offsetCalibrationResult.FunctionHeadId != string.Empty)
                                    {
                                        straightLine.StartPosition[0].PositionValue += offsetCalibrationResult.OffsetValue.X;
                                        straightLine.StartPosition[1].PositionValue += offsetCalibrationResult.OffsetValue.Y;
                                        straightLine.EndPosition[0].PositionValue += offsetCalibrationResult.OffsetValue.X;
                                        straightLine.EndPosition[1].PositionValue += offsetCalibrationResult.OffsetValue.Y;
                                    }
                                    straightLine.Speed = parameters.Value.MaxSpeed;
                                    straightLine.Acceleration = parameters.Value.Acceleration;
                                    motionInstructions.Add(straightLine);
                                }
                                else
                                {
                                    throw new Exception();
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                retMotionTraj.MotionInstructions = motionInstructions.ToArray();
                return retMotionTraj;
            }

            public MotionTrajectory Calculate(Dictionary<string, MotionCalculationParameters> motionCalculationParameters)
            {
                throw new NotImplementedException();
            }

            private OffsetCalibrationResult? offsetCalibrationResult;
        }
    }
}
