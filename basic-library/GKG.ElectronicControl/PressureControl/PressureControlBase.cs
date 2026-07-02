namespace GKG
{
    namespace ElectronicControl
    {
        namespace General
        {
            public interface IPressureControlBase
            {
                /// <summary>
                /// 初始化
                /// </summary>
                /// <param name="initParams">初始化参数</param>
                void Init(byte[] initParams);

                /// <summary>
                /// 设定气压值
                /// </summary>
                /// <param name="pressure">气压值</param>
                void SetPressure(double pressure);

                /// <summary>
                /// 获取气压值
                /// </summary>
                /// <returns>气压值</returns>
                double GetPressure();
            }

            public abstract class PressureControlBase : IPressureControlBase
            {
                #region 公有方法

                /// <summary>
                /// 初始化
                /// </summary>
                /// <param name="initParams">初始化参数</param>
                public abstract void Init(byte[] initParams);

                /// <summary>
                /// 设定气压值
                /// </summary>
                /// <param name="pressure">气压值</param>
                public abstract void SetPressure(double pressure);

                /// <summary>
                /// 获取气压值
                /// </summary>
                /// <returns>气压值</returns>
                public abstract double GetPressure();

                #endregion
            }
        }
    }
}
