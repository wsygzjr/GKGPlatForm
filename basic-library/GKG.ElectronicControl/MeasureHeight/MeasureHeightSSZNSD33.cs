using GF_Gereric;

namespace GKG
{
    namespace ElectronicControl
    {
        namespace General
        {
            public enum MeasureRange
            {
                Range30,
                Range50,
            }

            public class InitParamSSZNSD33
            {
                public MeasureRange MeasureRangeInitParam;
                public byte[] CommunicatorInitParam = new byte[0];
            };

            /// <summary>
            /// 深视智能SD33测高
            /// </summary>
            public class MeasureHeightSSZNSD33 : MeasureHeightBase, IMeasureHeightBase
            {
                #region 常量

                public const int SSZNSD33ValueIndex1 = 3;
                public const int SSZNSD33ValueIndex2 = 4;

                #endregion

                #region 私有字段

                private MeasureRange measureRange;

                #endregion

                #region 公有属性

                /// <summary>
                /// 当前测高值
                /// </summary>
                public override double CurrentHeight => ReadHeight();

                /// <summary>
                /// 激光测高正方向
                /// </summary>
                public override MeasureHeightPositiveDir PositiveDir => MeasureHeightPositiveDir.Down;

                #endregion

                #region 公有方法

                /// <summary>
                /// 初始化
                /// </summary>
                /// <param name="initParameters">初始化参数</param>
                public override void Init(byte[] initParameters)
                {
                    if (communicator == null)
                    {
                        communicator = new SerialPortCommunicate();
                    }

                    InitParamSSZNSD33 initParamSSZNSD33 = JsonObjConvert.FromJSonBytes<InitParamSSZNSD33>(initParameters);
                    communicator.Init(initParamSSZNSD33.CommunicatorInitParam);
                    measureRange = initParamSSZNSD33.MeasureRangeInitParam;
                    if (!communicator.IsOpen)
                    {
                        communicator.Open(1000);
                    }
                }

                /// <summary>
                /// 读取测高值
                /// </summary>
                /// <returns>测高值</returns>
                public override double ReadHeight()
                {
                    return ReadCore();
                }

                #endregion

                #region 私有方法

                /// <summary>
                /// 读取测高值（私有方法）
                /// </summary>
                /// <returns>测高值</returns>
                private double ReadCore()
                {
                    if (communicator == null)
                    {
                        throw new GKGException(MeasureHeightErrCodeConsts.MeasureHeightNotInit, MeasureHeightErr.UnInit, MeasureHeightErr.UnInit);
                    }
                    if (!communicator.IsOpen)
                    {
                        throw new GKGException(MeasureHeightErrCodeConsts.MeasureHeightComNotOpen, MeasureHeightErr.ComNotOpen, MeasureHeightErr.ComNotOpen);
                    }

                    bool rtn = communicator.Write(MeasureHeightConsts.CommendReadSSZNSD33);
                    if (!rtn)
                    {
                        throw new GKGException(MeasureHeightErrCodeConsts.MeasureHeightWriteFail, MeasureHeightErr.WriteFail, MeasureHeightErr.WriteFail);
                    }

                    rtn = communicator.ReadTimeout(50, out byte[] returnBytes);
                    if (!rtn)
                    {
                        throw new GKGException(MeasureHeightErrCodeConsts.MeasureHeightReadFail, MeasureHeightErr.ReadFail, MeasureHeightErr.ReadFail);
                    }

                    string hexReturnStr = StringConverter.BytesToHexString(returnBytes);
                    if (hexReturnStr[0] != '0' || hexReturnStr[1] != '1'
                        || hexReturnStr[2] != '0' || hexReturnStr[3] != '4')
                    {
                        throw new GKGException(MeasureHeightErrCodeConsts.MeasureHeightReturnsFail, MeasureHeightErr.ReturnsFail, MeasureHeightErr.ReturnsFail);
                    }

                    return SSZNSD33GetValue(new byte[2] { returnBytes[SSZNSD33ValueIndex1], returnBytes[SSZNSD33ValueIndex2] });
                }

                /// <summary>
                /// 深视智能SD33测高数据协议
                /// </summary>
                /// <param name="bytes">数据字节数组</param>
                /// <returns>高度值</returns>
                private double SSZNSD33GetValue(byte[] bytes)
                {
                    int value = 0;
                    int high = (bytes[0] + 0x0) & 0xff;
                    int low = (bytes[1] + 0x0) & 0xff;
                    int tmp = (high << 8) + low;
                    if (tmp < 0x8fff)
                    {
                        value += (tmp & 0x7000);
                        value += (tmp & 0x0f00);
                        value += (tmp & 0x00f0);
                        value += (tmp & 0x000f);
                    }
                    else
                    {
                        tmp ^= 0x7fff;
                        value += (tmp & 0x7000);
                        value += (tmp & 0x0f00);
                        value += (tmp & 0x00f0);
                        value += (tmp & 0x000f);
                        value = -value;
                    }

                    if (value > 9999 || value < -9999)
                    {
                        value = 9999;
                    }
                    if (value < 0)
                    {
                        value -= 1;
                    }
                    return value / 100.00;
                }

                #endregion
            }
        }
    }
}
