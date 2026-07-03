using GF_Gereric;

namespace GKG
{
    namespace MotionControl
    {
        /// <summary>
        /// 运动计算服务。
        /// 作为业务层入口，对外保持统一接口，内部委托给具体运动计算驱动。
        /// </summary>
        public class MotionCalculatorService : IMotionCalculatorService
        {
            private IMotionCalculatorDriver? driver;
            private MotionCalculatorInitParameters initParameters = new MotionCalculatorInitParameters();

            public void Init(byte[] initParam)
            {
                MotionCalculatorPluginManager.Init();
                initParameters = MotionCalculatorInitParameters.TryFromBytes(initParam) ?? new MotionCalculatorInitParameters();
                driver = MotionCalculatorPluginManager.GetMotionCalculatorDriver(initParameters.DriverName);
                driver.Init(initParam);
            }

            public MotionTrajectory Calculate(MotionCalculationParameters motionCalculationParameters)
            {
                return EnsureDriver().Calculate(motionCalculationParameters);
            }

            public MotionTrajectory Calculate(Dictionary<string, MotionCalculationParameters> motionCalculationParameters)
            {
                return EnsureDriver().Calculate(motionCalculationParameters);
            }

            private IMotionCalculatorDriver EnsureDriver()
            {
                if (driver != null)
                {
                    return driver;
                }

                MotionCalculatorPluginManager.Init();
                driver = MotionCalculatorPluginManager.GetMotionCalculatorDriver(initParameters.DriverName);
                driver.Init(JsonObjConvert.ToJSonBytes(initParameters));
                return driver;
            }
        }
    }
}
