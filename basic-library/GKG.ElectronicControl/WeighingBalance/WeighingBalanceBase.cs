namespace GKG
{
    namespace ElectronicControl
    {
        namespace General
        {
            public interface IWeighingBalanceBase
            {
                /// <summary>
                /// 实时重量值
                /// </summary>
                public double Weight { get; }

                /// <summary>
                /// 初始化
                /// </summary>
                /// <param name="initParams"></param>
                public void Init(byte[] initParams);

                /// <summary>
                /// 获取重量
                /// </summary>
                /// <returns></returns>
                public double GetWeight();

                /// <summary>
                /// 重量清零
                /// </summary>
                public void Zero();
            }

            public abstract class WeighingBalanceBase : IWeighingBalanceBase
            {
                #region 受保护字段

                protected IBaseCommunicate communicator;

                #endregion

                #region 公有属性

                public abstract double Weight { get; }

                #endregion

                #region 公有方法

                public abstract void Init(byte[] initParams);

                public abstract double GetWeight();

                public abstract void Zero();

                public static IWeighingBalanceBase CreateWeighingBalance(WeighingBalanceType weighingBalanceType)
                {
                    switch (weighingBalanceType)
                    {
                        case WeighingBalanceType.APW:
                            return new WeighingBalanceApw();

                        default:
                            return new WeighingBalanceApw();
                    }
                }

                #endregion
            }
        }
    }
}
