namespace GKG
{
    namespace MotionControl
    {
        /// <summary>
        /// Plane 杩愬姩璁＄畻椹卞姩銆?        /// </summary>
        public class PlaneMotionCalculatorDriver : MotionCalculatorDriverBase
        {
            #region 私有字段

            private readonly PlaneCalculator calculator = new PlaneCalculator();

            #endregion

            #region 公有属性

            public override string DriverName => MotionCalculatorDriverNames.Plane;

            #endregion

            #region 公有方法

            public override void Init(byte[] initParam)
            {
                base.Init(initParam);
                calculator.Init(ResolveDriverInitParam());
            }

            public override MotionTrajectory Calculate(MotionCalculationParameters motionCalculationParameters)
            {
                return calculator.Calculate(motionCalculationParameters);
            }

            public override MotionTrajectory Calculate(Dictionary<string, MotionCalculationParameters> motionCalculationParameters)
            {
                return calculator.Calculate(motionCalculationParameters);
            }

            #endregion

            #region 受保护方法

            protected override void OnInit(MotionCalculatorInitParameters initParameters)
            {
            }

            #endregion
        }
    }
}
