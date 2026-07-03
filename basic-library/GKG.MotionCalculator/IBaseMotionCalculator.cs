namespace GKG
{
    namespace MotionControl
    {
        public interface IBaseMotionCalculator
        {
            void Init(byte[] initCfg);

            MotionTrajectory Calculate(MotionCalculationParameters motionCalculationParameters);
            MotionTrajectory Calculate(Dictionary<string, MotionCalculationParameters> motionCalculationParameters);
        }
    }
}
