namespace GKG
{
    namespace ElectronicControl
    {
        namespace General
        {
            public interface IMeasureHeightBase
            {
                /// <summary>
                /// 当前测高值
                /// </summary>
                public double CurrentHeight
                {
                    get
                    {
                        return ReadHeight();
                    }
                }

                /// <summary>
                /// 激光测高正方向
                /// </summary>
                public MeasureHeightPositiveDir PositiveDir => MeasureHeightPositiveDir.Down;

                /// <summary>
                /// 初始化
                /// </summary>
                /// <param name="InitParameters">初始化参数</param>
                public void Init(byte[] InitParameters);

                /// <summary>
                /// 读取高度值
                /// </summary>
                /// <returns>高度值</returns>
                public double ReadHeight();
            }

            public abstract class MeasureHeightBase : IMeasureHeightBase
            {
                #region 受保护字段

                protected IBaseCommunicate communicator;

                #endregion

                #region 公有属性

                /// <summary>
                /// 当前测高值
                /// </summary>
                public abstract double CurrentHeight { get; }

                public virtual MeasureHeightPositiveDir PositiveDir => MeasureHeightPositiveDir.Down;

                #endregion

                #region 公有方法

                public virtual void Init(byte[] initParameters)
                {
                    communicator.Init(initParameters);
                }

                public abstract double ReadHeight();

                #endregion
            }

            public static class MeasureHeightFactory
            {
                public static IMeasureHeightBase CreateMeasureHeight(MeasureHeightType measureHeightType)
                {
                    switch(measureHeightType)
                    {
                        case MeasureHeightType.SSZNSD33:
                            {
                                return new MeasureHeightSSZNSD33();
                            }
                        default:
                            throw new ArgumentOutOfRangeException(nameof(measureHeightType), measureHeightType, null);
                    }
                }
            }
        }
    }
}
