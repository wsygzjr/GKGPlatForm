using GF_Gereric;

namespace GKG
{
    namespace ElectronicControl
    {
        namespace General
        {
            public class PressureControlSerialPortInitParams
            {
                /// <summary>
                /// 串口初始化参数
                /// </summary>
                public byte[] SerialConfig = new byte[0];

                /// <summary>
                /// 站号
                /// </summary>
                public int StationId = 0;

                /// <summary>
                /// 通道号
                /// </summary>
                public int ChannelId = 0;
            }

            /// <summary>
            /// 串口型气压控制
            /// </summary>
            public class PressureControlSerialPort : PressureControlBase, IPressureControlBase
            {
                /// <summary>
                /// 通道号
                /// </summary>
                private string channelId = "";

                /// <summary>
                /// 模拟量卡对象
                /// </summary>
                private IAnalogQuantity analogCard;

                /// <summary>
                /// 初始化
                /// </summary>
                /// <param name="initParams">初始化参数</param>
                public override void Init(byte[] initParams)
                {
                    // 初始化解析参数
                    PressureControlSerialPortInitParams pressureControlSerialPortInitParams = JsonObjConvert.FromJSonBytes<PressureControlSerialPortInitParams>(initParams);
                    analogCard = AnalogCardInstance.GetInstance(pressureControlSerialPortInitParams.SerialConfig, pressureControlSerialPortInitParams.StationId);
                    if (pressureControlSerialPortInitParams.ChannelId < analogCard.ChannelParameters.Count)
                    {
                        channelId = analogCard.ChannelParameters[pressureControlSerialPortInitParams.ChannelId].channelID;
                    }
                    else
                    {
                        throw new GKGException(PressureControlErrCodeConsts.PressureControlSerialInitParamsBad, PressureControlErr.PressureControlSerialInitChannelParamsBad, PressureControlErr.PressureControlSerialInitChannelParamsBad);
                    }
                }

                /// <summary>
                /// 设定气压值
                /// </summary>
                /// <param name="pressure">气压值</param>
                public override void SetPressure(double pressure)
                {
                    analogCard.SetValue(channelId, pressure);
                }

                /// <summary>
                /// 获取气压值
                /// </summary>
                /// <returns>气压值</returns>
                public override double GetPressure()
                {
                    return analogCard.GetValue(channelId);
                }
            }
        }
    }
}