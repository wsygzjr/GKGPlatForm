using GF_Gereric;

namespace GKG
{
    namespace ElectronicControl
    {
        namespace General
        {
            public class WeighingBalanceApwInitParams
            {
                public byte[] SerialConfig = new byte[0];
            }

            public class WeighingBalanceApw : WeighingBalanceBase, IWeighingBalanceBase
            {
                #region 常量

                private const string CommandReadsteady = "S\r\n";
                private const string CommandReadRealTime = "SI\r\n";
                private const string CommandZero = "ZI\r\n";
                private const string returnDataStart = "SS";
                private const string returnDataStartRealTimt = "SD";
                private const string returnDataEnd = "g";
                private const string returnDataErr = "ES";

                #endregion

                #region 私有字段

                private readonly int[] WeighingBalanceApwDataPos = { 5, 9, 8 };

                #endregion

                #region 公有属性

                public override double Weight => GetWeightRealTime();

                #endregion

                #region 公有方法

                public override void Init(byte[] initParams)
                {
                    WeighingBalanceApwInitParams weighingBalanceApwInitParams = JsonObjConvert.FromJSonBytes<WeighingBalanceApwInitParams>(initParams);
                    if (communicator == null)
                    {
                        communicator = new SerialPortCommunicate();
                        communicator.Init(weighingBalanceApwInitParams.SerialConfig);
                        communicator.Open(1000);
                    }
                }

                public override double GetWeight()
                {
                    return GetWeight(CommandReadsteady);
                }

                public override void Zero()
                {
                    bool rtn = communicator.Write(CommandZero);
                    if (!rtn)
                    {
                        throw new Exception();
                    }
                }

                #endregion

                #region 私有方法

                private double GetWeight(string command)
                {
                    bool rtn = communicator.Write(command);
                    if (!rtn)
                    {
                        throw new GKGException(WeighingBalanceErrCodeConsts.WeighingBalanceWriteCommandFail, WeighingBalanceErr.WeighingBalanceWriteCommandFail, WeighingBalanceErr.WeighingBalanceWriteCommandFail);
                    }

                    rtn = communicator.ReadTimeout(100, out byte[] returnBytes);
                    if (!rtn)
                    {
                        throw new GKGException(WeighingBalanceErrCodeConsts.WeighingBalanceReadCommandFail, WeighingBalanceErr.WeighingBalanceReadCommandFail, WeighingBalanceErr.WeighingBalanceReadCommandFail);
                    }

                    string cString = StringConverter.BytesToString(returnBytes);
                    cString = StringConverter.RemoveSpace(cString);
                    cString = StringConverter.RemoveEnter(cString);
                    if (cString.Contains(returnDataErr))
                    {
                        throw new Exception();
                    }

                    int iSplitPos_s = 0;
                    int iSplitPos_g = 0;
                    string sComMiddleData = "";
                    double weight = 0;
                    if (cString.Length >= WeighingBalanceApwDataPos[1])
                    {
                        if (cString.Substring(0, 2).Equals(returnDataStart) && cString.Substring(cString.Length - 1, 1).Equals(returnDataEnd))
                        {
                            iSplitPos_s = cString.LastIndexOf(returnDataStart) + returnDataStart.Length - 1;
                            if (iSplitPos_s != -1 && iSplitPos_s + 2 <= cString.Length)
                            {
                                sComMiddleData = cString.Substring(iSplitPos_s + 1, cString.Length - iSplitPos_s - 1);
                                iSplitPos_g = sComMiddleData.IndexOf(returnDataEnd);
                                if (iSplitPos_g > 0)
                                {
                                    weight = double.Parse(sComMiddleData.Substring(0, iSplitPos_g));
                                }
                                else
                                {
                                    throw new GKGException(WeighingBalanceErrCodeConsts.WeighingBalanceParseDataFail, WeighingBalanceErr.WeighingBalanceParseDataFail, WeighingBalanceErr.WeighingBalanceParseDataFail);
                                }
                            }
                            else
                            {
                                throw new GKGException(WeighingBalanceErrCodeConsts.WeighingBalanceParseDataFail, WeighingBalanceErr.WeighingBalanceParseDataFail, WeighingBalanceErr.WeighingBalanceParseDataFail);
                            }
                        }
                        else if (cString.Substring(0, 2).Equals(returnDataStartRealTimt) && cString.Substring(cString.Length - 1, 1).Equals(returnDataEnd))
                        {
                            iSplitPos_s = cString.LastIndexOf(returnDataStartRealTimt) + returnDataStartRealTimt.Length - 1;
                            if (iSplitPos_s != -1 && iSplitPos_s + 2 <= cString.Length)
                            {
                                sComMiddleData = cString.Substring(iSplitPos_s + 2, cString.Length - iSplitPos_s - 2);
                                iSplitPos_g = sComMiddleData.IndexOf(returnDataEnd);
                                if (iSplitPos_g > 0)
                                {
                                    weight = double.Parse(sComMiddleData.Substring(0, iSplitPos_g));
                                }
                                else
                                {
                                    throw new GKGException(WeighingBalanceErrCodeConsts.WeighingBalanceParseDataFail, WeighingBalanceErr.WeighingBalanceParseDataFail, WeighingBalanceErr.WeighingBalanceParseDataFail);
                                }
                            }
                            else
                            {
                                throw new GKGException(WeighingBalanceErrCodeConsts.WeighingBalanceParseDataFail, WeighingBalanceErr.WeighingBalanceParseDataFail, WeighingBalanceErr.WeighingBalanceParseDataFail);
                            }
                        }
                        else
                        {
                            throw new GKGException(WeighingBalanceErrCodeConsts.WeighingBalanceParseDataFail, WeighingBalanceErr.WeighingBalanceParseDataFail, WeighingBalanceErr.WeighingBalanceParseDataFail);
                        }
                    }
                    else
                    {
                        throw new GKGException(WeighingBalanceErrCodeConsts.WeighingBalanceParseDataFail, WeighingBalanceErr.WeighingBalanceParseDataFail, WeighingBalanceErr.WeighingBalanceParseDataFail);
                    }
                    return weight;
                }

                private double GetWeightRealTime()
                {
                    return GetWeight(CommandReadRealTime);
                }

                #endregion
            }
        }
    }
}
