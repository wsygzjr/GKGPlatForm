
namespace GKG
{
    namespace MotionControl
    {
        public class StraightAxisCalculator : IBaseMotionCalculator
        {
            public static int calculationTypeConstant = CalculationTypeConstants.General;

            public void Init(byte[] initCfg)
            {
                throw new NotImplementedException();
            } 

            public MotionTrajectory Calculate(MotionCalculationParameters motionCalculationParameters)
            {
                if (motionCalculationParameters == null)
                    throw new Exception();
                MotionTrajectory retMotionTraj = new MotionTrajectory();
                int size = motionCalculationParameters.ProductProcessingTrajectory.Length;
                if(size <= 0)
                    throw new Exception();
                retMotionTraj.MotionInstructions = new MotionInstructionBase[size];
                int idx = 0;
                retMotionTraj.SequenceType = MotionTrajectorySequenceTypeConstants.StepByStep;
                foreach (var motionItem in motionCalculationParameters.ProductProcessingTrajectory)
                {
                    if(motionItem.IsProcessing == false)
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
                            MotionInstruction motionInstruction = new MotionInstruction();
                            retMotionTraj.MotionInstructions[idx++] = pointMotionInstruction;
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                    else
                    {
                        throw new Exception(); 
                    }
                }
                return retMotionTraj;
            }

            public MotionTrajectory Calculate(Dictionary<string, MotionCalculationParameters> motionCalculationParameters)
            {
                throw new NotImplementedException();
            }
        }
    }
}
