namespace GKG
{
    namespace MotionControl
    {
        /// <summary>
        /// 运动计算驱动基类。
        /// 提供统一的初始化与默认未实现行为，方便扩展不同运动计算后端。
        /// </summary>
        public abstract class MotionCalculatorDriverBase : IMotionCalculatorDriver
        {
            #region 受保护属性

            protected MotionCalculatorInitParameters InitParameters { get; private set; } = new MotionCalculatorInitParameters();
            protected byte[] RawInitParam { get; private set; } = Array.Empty<byte>();

            #endregion

            #region 公有属性

            public abstract string DriverName { get; }

            #endregion

            #region 公有方法

            public virtual void Init(byte[] initParam)
            {
                RawInitParam = initParam ?? Array.Empty<byte>();
                InitParameters = MotionCalculatorInitParameters.TryFromBytes(initParam) ?? new MotionCalculatorInitParameters();
                OnInit(InitParameters);
            }

            public abstract MotionTrajectory Calculate(MotionCalculationParameters motionCalculationParameters);

            public abstract MotionTrajectory Calculate(Dictionary<string, MotionCalculationParameters> motionCalculationParameters);

            #endregion

            #region 受保护方法

            protected byte[] ResolveDriverInitParam()
            {
                if (InitParameters.DriverInitParameters != null && InitParameters.DriverInitParameters.Length > 0)
                {
                    return InitParameters.DriverInitParameters;
                }

                return RawInitParam;
            }

            protected abstract void OnInit(MotionCalculatorInitParameters initParameters);

            #endregion
        }
    }
}
