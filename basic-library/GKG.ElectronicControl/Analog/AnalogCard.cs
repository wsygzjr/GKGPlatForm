using GF_Gereric;
using GKG.ElectronicControl.General;
/// <summary>
/// 模拟量卡相关类
/// 设计思路
/// </summary>
/// <summary>
/// 1. 模拟量卡通过串口通讯，每个串口可以连接多张模拟量卡，每张模拟量卡有唯一的站号
/// 2. 通过串口配置和站号唯一标识一张模拟量卡
/// 3. 使用静态类AnalogCardInstance管理模拟量卡实例的创建和获取，确保每张模拟量卡只有一个实例
/// 4. 模拟量卡类AnalogCard封装了模拟量卡的读写操作，包括初始化、设置值和获取值的方法
/// 5. 模拟量卡的读写命令和返回数据解析位置通过常量和数组进行定义，方便维护和修改
/// 6. 模拟量值和真实值的转换通过专门的方法进行封装，确保代码的清晰和可维护性
/// 7. 异常处理通过自定义异常类GKGException进行，确保在读写过程中出现问题时能够及时反馈错误信息
/// </summary>

namespace GKG
{
    namespace ElectronicControl
    {
        /// <summary>
        /// 模拟量卡
        /// </summary>
        public class AnalogCard : IAnalogQuantity
        {
            /// <summary>
            /// 通讯对象
            /// </summary>
            private IBaseCommunicate communicator;

            // 模拟量卡读写命令
            public const string AnalogCommandRead = "0400000004";

            public const string AnalogCommandWrite = "100000000408";

            /// <summary>
            /// 返回的数据解析位置
            /// </summary>
            private static readonly int[] returnDataPos = { 6, 10, 14, 18 };

            /// <summary>
            /// 存储的通道值
            /// </summary>
            private double[] values;

            /// <summary>
            /// 站号
            /// </summary>
            private int stationNo = 0;

            /// <summary>
            /// 通道ID参数列表
            /// </summary>
            private static ChannelParametersList channelParameters = new ChannelParametersList();

            /// <summary>
            /// 通道ID属性列表
            /// </summary>
            public ChannelParametersList ChannelParameters { get => channelParameters; }

            /// <summary>
            /// 通道ID属性列表是否初始化标志
            /// </summary>

            private static bool bIsChannelParametersInit = false;

            /// <summary>
            /// 通道ID属性列表初始化方法
            /// </summary>
            private static void InitChannelParameters()
            {
                if (bIsChannelParametersInit)
                    return;
                for (int i = 0; i < returnDataPos.Length; i++)
                {
                    channelParameters.Add(new ChannelParameters { channelID = i.ToString(), channelMode = EReadWriteMode.ReadWrite });
                }
                bIsChannelParametersInit = true;
            }

            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="serialConfig">串口配置</param>
            /// <param name="stationId">站号</param>
            public void Init(byte[] serialConfig, int stationId)
            {
                communicator = new SerialPortCommunicate();//TestCallBack.GetCommunicator(serialConfig);
                communicator.Init(serialConfig);
                communicator.Open(1000);
                stationNo = stationId;
                values = new double[returnDataPos.Length];
                InitChannelParameters();
            }

            /// <summary>
            /// 设置模拟量值
            /// </summary>
            /// <param name="channelId">通道号</param>
            /// <param name="value">模拟量值</param>
            public void SetValue(string channelId, double value)
            {
                int iChannelID = int.Parse(channelId);
                if (iChannelID > returnDataPos.Length)
                {
                    return;
                }
                values[iChannelID] = value;
                string command = stationNo.ToString("00") + AnalogCommandWrite;
                for (int i = 0; i < values.Length; i++)
                {
                    command += ((int)AnalogCard_In_MpTomV(values[i])).ToString($"X{4}");
                }
                communicator.Write(command);
                Thread.Sleep(50);
                communicator.ReadTimeout(50, out byte[] returnBytes);
            }

            /// <summary>
            /// 获取模拟量值
            /// </summary>
            /// <param name="channelId">通道号</param>
            /// <returns>模拟量值</returns>
            /// <exception cref="GKGException">异常（错误码，错误描述）</exception>
            public double GetValue(string channelId)
            {
                int iChannelID = int.Parse(channelId);
                if (iChannelID > returnDataPos.Length)
                {
                    throw new GKGException(PressureControlErrCodeConsts.PressureControlInitFail, PressureControlErr.PressureControlInitFail, PressureControlErr.PressureControlInitFail);
                }

                // 站号+读命令
                string command = stationNo.ToString("00") + AnalogCommandRead;

                bool rtn = false;
                rtn = communicator.Write(command);
                if (rtn == false)
                {
                    throw new GKGException(PressureControlErrCodeConsts.PressureControlSerialWriteFail, PressureControlErr.PressureControlSerialWriteFail, PressureControlErr.PressureControlSerialWriteFail);
                }
                Thread.Sleep(500);
                rtn = communicator.ReadTimeout(50, out byte[] returnBytes);
                if (rtn == false)
                {
                    throw new GKGException(PressureControlErrCodeConsts.PressureControlSerialReadFail, PressureControlErr.PressureControlSerialReadFail, PressureControlErr.PressureControlSerialReadFail);
                }
                string hexString = StringConverter.BytesToHexString(returnBytes);

                if (hexString.Length <= returnDataPos[returnDataPos.Length - 1] + 4)
                {
                    throw new GKGException(PressureControlErrCodeConsts.PressureControlSerialReadFail, PressureControlErr.PressureControlSerialReadFail, PressureControlErr.PressureControlSerialReadFail);
                }
                for (int i = 0; i < returnDataPos.Length; i++)
                {
                    // 每个数据都是四位
                    string singleValue = hexString.Substring(returnDataPos[i], 4);
                    values[i] = AnalogCard_Out_mVToMp(StringConverter.HexToInt32Int(singleValue));
                }
                return values[iChannelID];
            }

            /// <summary>
            /// 真实值转换为模拟量值
            /// </summary>
            /// <param name="dMp"></param>
            /// <returns></returns>
            private static double AnalogCard_In_MpTomV(double dMp)
            {
                //y = kx + b   k = 1107.17  , b = 14.6807
                double dmV = (1107.17 * dMp) + 14.6807;
                if (dMp <= 0) dmV = 0;
                return dmV;
            }

            /// <summary>
            /// 模拟量值转换为真实值
            /// </summary>
            /// <param name="dmV"></param>
            /// <returns></returns>
            private static double AnalogCard_Out_mVToMp(double dmV)
            {
                double dMp = (0.0023041475 * dmV) + (-2.20506912);
                if (dmV <= 0) dMp = 0;
                return dMp;
            }
        }

        public static class AnalogCardInstance
        {
            /// <summary>
            /// 需要PortName和StationId来唯一标识一个模拟量卡
            /// </summary>
            private class AnalogCardKey
            {
                public string PortName = "";
                public int StationId = 0;

                public override bool Equals(object? obj)
                {
                    if (obj is AnalogCardKey other)
                    {
                        return PortName == other.PortName && StationId == other.StationId;
                    }
                    return false;
                }

                public override int GetHashCode()
                {
                    return HashCode.Combine(PortName, StationId);
                }

                public static bool operator ==(AnalogCardKey? left, AnalogCardKey? right)
                {
                    if (ReferenceEquals(left, right)) return true;
                    if (left is null || right is null) return false;
                    return left.Equals(right);
                }

                public static bool operator !=(AnalogCardKey? left, AnalogCardKey? right)
                {
                    return !(left == right);
                }
            }

            /// <summary>
            /// 模拟量卡的静态实例集合
            /// </summary>
            private static Dictionary<AnalogCardKey, AnalogCard> instances = new Dictionary<AnalogCardKey, AnalogCard>();

            /// <summary>
            /// 用于获取模拟量卡实例，如果不存在则创建且初始化
            /// </summary>
            /// <param name="serialConfig">串口配置</param>
            /// <param name="stationId">站号</param>
            /// <returns></returns>
            public static AnalogCard GetInstance(byte[] serialConfig, int stationId)
            {
                SerialConfig serial = JsonObjConvert.FromJSonBytes<SerialConfig>(serialConfig);
                AnalogCardKey analogCardKey = new AnalogCardKey()
                {
                    PortName = serial.PortName,
                    StationId = stationId
                };
                if (!instances.ContainsKey(analogCardKey))
                {
                    AnalogCard analogCard = new AnalogCard();
                    analogCard.Init(serialConfig, stationId);
                    instances.Add(analogCardKey, analogCard);
                }
                return instances[analogCardKey];
            }
        }
    }
}