namespace GKG
{
    namespace MotionControl
    {
        /// <summary>
        /// 运动计算驱动接口。
        /// 用于对接不同运动计算后端，实现与业务层解耦。
        /// </summary>
        public interface IMotionCalculatorDriver
        {
            /// <summary>
            /// 驱动名称。
            /// </summary>
            string DriverName { get; }

            void Init(byte[] initParam);

            MotionTrajectory Calculate(MotionCalculationParameters motionCalculationParameters);

            MotionTrajectory Calculate(Dictionary<string, MotionCalculationParameters> motionCalculationParameters);
        }
    }
}
