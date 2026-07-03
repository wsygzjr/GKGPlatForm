namespace GKG
{
    namespace MotionControl
    {
        /// <summary>
        /// 运动计算服务接口。
        /// </summary>
        public interface IMotionCalculatorService
        {
            void Init(byte[] initParam);

            MotionTrajectory Calculate(MotionCalculationParameters motionCalculationParameters);

            MotionTrajectory Calculate(Dictionary<string, MotionCalculationParameters> motionCalculationParameters);
        }
    }
}
