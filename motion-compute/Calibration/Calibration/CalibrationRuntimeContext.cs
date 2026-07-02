using GKG.ElectronicControl;
using GKG.Vision;

namespace GKG
{
    namespace MotionControl
    {
        public class CalibrationRuntimeContext
        {
            public IMotionControlBase? MotionControl { get; set; }

            public IRobotDriver? RobotDriver { get; set; }

            public IVisionDriver? VisionDriver { get; set; }

            public IMotionCalculatorDriver? MotionCalculatorDriver { get; set; }

            public RobotExecutionContext RobotExecutionContext { get; set; } = new RobotExecutionContext();
        }
    }
}
