using GF_Gereric;
using GKG.ElectronicControl;
using GKG.ElectronicControl.General;
using GKG.MotionControl;
using Griffins.PF.Server;

namespace GKG
{
    namespace ElectronicControl
    {
        namespace Dispenser
        {
            public class GKGPiezoValveInitParams
            {
                public Guid XAxisBindingObjID {  get; set; }
                /// <summary>
                /// 阀名称
                /// </summary>
                public string Name { get; set; }

                /// <summary>
                /// 阀ID
                /// </summary>
                public string ID { get; set; }

                /// <summary>
                /// 阀通讯对象参数
                /// </summary>
                public byte[] CommunicatorParams { get; set; }

                /// <summary>
                /// 气压控制参数
                /// </summary>
                public byte[] PressureControlParams { get; set; }

                /// <summary>
                /// 阀报警IO参数
                /// </summary>
                public byte[] ValveAlarmIOParams { get; set; }

                /// <summary>
                /// 阀报警IO初始化参数
                /// </summary>
                public byte[] ValveAlarmIOInitParams { get; set; }

                /// <summary>
                /// 触发通道
                /// </summary>
                public int Channel { get; set; } = 0;

                /// <summary>
                /// 开阀时间(ms)
                /// </summary>
                public double OpenValveTimeMs { get; set; } = 0;

                /// <summary>
                /// 关阀时间(ms)
                /// </summary>
                public double CloseValveTimeMs { get; set; } = 0;

                /// <summary>
                /// 报警次数检测
                /// </summary>
                public int AlarmCountDetect { get; set; }

                /// <summary>
                /// 是否开启报警次数检测
                /// </summary>
                public bool IsDetectAlarmCount { get; set; }

                /// <summary>
                /// 人工清洗喷嘴间隔时间
                /// </summary>
                public int ManualCleaningOfTheNozzleTimeS { get; set; }

                /// <summary>
                /// 是否开启人工清洗喷嘴提示功能
                /// </summary>
                public bool IsManualCleaning { get; set; }
            }

            public class GKGPiezoValveFormulaParams
            {
                /// <summary>
                /// 阀参数列表
                /// </summary>
                public GKGPiezoValveParams[] ValveParams;
            }

            public class GKGPiezoValveParams
            {
                /// <summary>
                /// 上升时间
                /// </summary>
                public double UpriseTimeMs { get; set; }

                /// <summary>
                /// 点胶时间
                /// </summary>
                public double DispenseTimeMs { get; set; }

                /// <summary>
                /// 撞击时间
                /// </summary>
                public double ImpactTimeMs { get; set; }

                /// <summary>
                /// 间歇时间
                /// </summary>
                public double IntervalTimeMs { get; set; }

                /// <summary>
                /// 电压比(%)
                /// </summary>
                public int PressureControlParam { get; set; }

                /// <summary>
                /// 打点数量
                /// </summary>
                public int PointCount { get; set; }

                /// <summary>
                /// 点胶模式
                /// </summary>
                public int DispenseMode { get; set; }

                /// <summary>
                /// 启停状态
                /// </summary>
                public int StartStopState { get; set; }
            }

            public class GKGPiezoValve : ValveBase, IValveBase
            {
                /// <summary>
                /// 机械手
                /// </summary>
                private IRobotDriver robot;

                /// <summary>
                /// 阀报警IO
                /// </summary>
                private IBaseStateIO valveAlarmIO;

                /// <summary>
                /// 初始化数据
                /// </summary>
                private GKGPiezoValveInitParams initParams;

                /// <summary>
                /// 气压控制参数
                /// </summary>
                private DetectPressureParams detectPressureParam;

                /// <summary>
                /// 阀控制参数
                /// </summary>
                private GKGPiezoValveFormulaParams GKGPiezoValveParam { get; set; }

                private const string writeCommand = "1010000000081000000001004000010000000000C800C8";
                private const string readCommand = "100300000008";
                private const string readAlarmCommand = "1003001A0001";

                /// <summary>
                /// 初始化
                /// </summary>
                /// <param name="InitParams"></param>
                public override void Init(byte[] InitParams)
                {
                    initParams = JsonObjConvert.FromJSonBytes<GKGPiezoValveInitParams>(InitParams);
                    if (initParams.XAxisBindingObjID == Guid.Empty)
                        throw new Exception();/*InvalidOperationException(axisMissingMsg);*/

                    RobotDriverByAxisIdsRequest request = new RobotDriverByAxisIdsRequest(new List<Guid> { initParams.XAxisBindingObjID })
                    {
                        MotionCardType = MotionControlCardType.Normal
                    };

                    var robotDriverResponses = ServerInnerInfoSender.SendMutualInfo(RobotDriverByAxisIdsRequest.InfoKindID, request);
                    if (robotDriverResponses == null || robotDriverResponses.Count == 0)
                        throw new Exception(); //new InvalidOperationException(responseMissingMsg);

                    RobotDriverByAxisIdsResponse robotDriverResponse = robotDriverResponses[0].Response as RobotDriverByAxisIdsResponse;
                    if (robotDriverResponse?.RobotDriver == null)
                        throw new Exception();//InvalidOperationException(unavailableMsg);

                    robot = robotDriverResponse.RobotDriver;
                    if(communicator == null)
                    {
                        communicator = new SerialPortCommunicate();
                        communicator.Init(initParams.CommunicatorParams);
                    }

                    if(pressureControl == null)
                    {
                        pressureControl = new PressureControlSerialPort();
                        pressureControl.Init(initParams.PressureControlParams);
                    }

                    IOStateInitParameters iOStateInitParameters = JsonObjConvert.FromJSonBytes<IOStateInitParameters>(initParams.ValveAlarmIOInitParams);
                    //valveAlarmIO = IOStateCallBack.GetIOStateControl(JsonObjConvert.ToJSon(iOStateInitParameters), 0);
                    GKGPiezoValveParam = new GKGPiezoValveFormulaParams();
                }

                public override void Dispose()
                {
                    communicator.Close();
                }

                public override void SetFormulaParams(byte[] formulaParams)
                {
                    GKGPiezoValveParam = JsonObjConvert.FromJSonBytes<GKGPiezoValveFormulaParams>(formulaParams);
                }

                /// <summary>
                /// 打开
                /// </summary>
                public override void Open()
                {
                    robot.Execute(new MotionInstructionSequence
                    {
                        SequenceType = MotionInstructionSequenceType.StepByStep,
                        Instructions = new MotionInstructionBase[]
                         {
                             new ManualPositionComparisonInstruction
                             {
                                 Channel = new int[]{(short)initParams.Channel },
                                 StartLevel = 1,
                                 PulseOutputMode = 0,
                                 TriggerCount = 10000,
                                 OpenTime = initParams.OpenValveTimeMs,
                                 CloseTime = initParams.CloseValveTimeMs,
                             }
                         }
                    }, new RobotExecutionContext());
                }

                /// <summary>
                /// 打开
                /// </summary>
                public override void Open(int pointCount)
                {
                    robot.Execute(new MotionInstructionSequence
                    {
                        SequenceType = MotionInstructionSequenceType.StepByStep,
                        Instructions = new MotionInstructionBase[]
     {
                             new ManualPositionComparisonInstruction
                             {
                                 Channel = new int[]{(short)initParams.Channel },
                                 StartLevel = 1,
                                 PulseOutputMode = 0,
                                 TriggerCount = pointCount,
                                 OpenTime = initParams.OpenValveTimeMs,
                                 CloseTime = initParams.CloseValveTimeMs,
                             }
     }
                    }, new RobotExecutionContext());
                }

                /// <summary>
                /// 关闭
                /// </summary>
                public override void Close()
                {
                    robot.Execute(new MotionInstructionSequence
                    {
                        SequenceType = MotionInstructionSequenceType.StepByStep,
                        Instructions = new MotionInstructionBase[]
                        {
                             new StopManualPositionComparison()
                        }
                    }, new RobotExecutionContext());
                }

                /// <summary>
                /// 设置阀气压
                /// </summary>
                /// <param name="pressure"></param>
                public override void SetValveAirPressure(DetectPressureParams param)
                {
                    detectPressureParam = param;
                }

                /// <summary>
                /// 获取阀气压
                /// </summary>
                /// <returns></returns>
                public override double GetValveAirPressure()
                {
                    return pressureControl.GetPressure();
                }

                public override void SetValveParam()
                {
                    string command = writeCommand.Substring(0, 14);
                    command += StringConverter.IntToHexString((int)(GKGPiezoValveParam.ValveParams[0].UpriseTimeMs * 1000), 4);
                    command += StringConverter.IntToHexString((int)(GKGPiezoValveParam.ValveParams[0].DispenseTimeMs * 1000), 4);
                    command += StringConverter.IntToHexString((int)(GKGPiezoValveParam.ValveParams[0].ImpactTimeMs * 1000), 4);
                    command += StringConverter.IntToHexString((int)(GKGPiezoValveParam.ValveParams[0].IntervalTimeMs * 1000), 4);
                    command += StringConverter.IntToHexString(GKGPiezoValveParam.ValveParams[0].PressureControlParam, 4);
                    command += StringConverter.IntToHexString(GKGPiezoValveParam.ValveParams[0].PointCount, 4);
                    command += StringConverter.IntToHexString(GKGPiezoValveParam.ValveParams[0].DispenseMode, 4);
                    command += StringConverter.IntToHexString(GKGPiezoValveParam.ValveParams[0].StartStopState, 4);
                    communicator.Write(command);
                }

                public override byte[] GetValveParam()
                {
                    communicator.Write(readCommand);
                    communicator.ReadTimeout(50, out byte[] response);
                    if (response == null || response.Length < 18)
                        throw new Exception();
                    int intdat1 = 0, intdat2 = 0;
                    intdat1 = ((int)response[4]) & 0X00FF; intdat2 = ((int)response[3]) << 8;
                    GKGPiezoValveParam.ValveParams[0].UpriseTimeMs = (double)(intdat1 | intdat2) / 1000;
                    intdat1 = ((int)response[6]) & 0X00FF; intdat2 = ((int)response[5]) << 8;
                    GKGPiezoValveParam.ValveParams[0].DispenseTimeMs = (double)(intdat1 | intdat2) / 1000;
                    intdat1 = ((int)response[8]) & 0X00FF; intdat2 = ((int)response[7]) << 8;
                    GKGPiezoValveParam.ValveParams[0].ImpactTimeMs = (double)(intdat1 | intdat2) / 1000;
                    intdat1 = ((int)response[10]) & 0X00FF; intdat2 = ((int)response[9]) << 8;
                    GKGPiezoValveParam.ValveParams[0].IntervalTimeMs = (double)(intdat1 | intdat2) / 1000;

                    intdat1 = ((int)response[12]) & 0X00FF; intdat2 = ((int)response[11]) << 8;
                    GKGPiezoValveParam.ValveParams[0].PressureControlParam = intdat1 | intdat2;
                    intdat1 = ((int)response[14]) & 0X00FF; intdat2 = ((int)response[13]) << 8;
                    GKGPiezoValveParam.ValveParams[0].PointCount = intdat1 | intdat2;
                    intdat1 = ((int)response[16]) & 0X00FF; intdat2 = ((int)response[15]) << 8;
                    GKGPiezoValveParam.ValveParams[0].DispenseMode = intdat1 | intdat2;
                    intdat1 = ((int)response[18]) & 0X00FF; intdat2 = ((int)response[17]) << 8;
                    GKGPiezoValveParam.ValveParams[0].StartStopState = intdat1 | intdat2;
                    return JsonObjConvert.ToJSonBytes(GKGPiezoValveParam.ValveParams[0]);
                }

                private static string HexStrToBinStr(string sInputHex)
                {
                    //string binStr;
                    //for (auto ch : sInputHex)
                    //{
                    //	switch (ch)
                    //	{
                    //	case '\x0': binStr += "0000"; break;
                    //	case '\x1': binStr += "0001"; break;
                    //	case '\x2': binStr += "0010"; break;
                    //	case '\x3': binStr += "0011"; break;
                    //	case '\x4': binStr += "0100"; break;
                    //	case '\x5': binStr += "0101"; break;
                    //	case '\x6': binStr += "0110"; break;
                    //	case '\x7': binStr += "0111"; break;
                    //	case '\x8': binStr += "1000"; break;
                    //	case '\x9': binStr += "1001"; break;
                    //	case 'a': binStr += "1010"; break;
                    //	case 'b': binStr += "1011"; break;
                    //	case 'c': binStr += "1100"; break;
                    //	case 'd': binStr += "1101"; break;
                    //	case 'e': binStr += "1110"; break;
                    //	case 'f': binStr += "1111"; break;
                    //	default:
                    //		break;
                    //	}
                    //}
                    string binaryDigit = "";
                    for (int i = 0; i < sInputHex.Length; ++i)
                    {
                        char e = sInputHex[i];
                        if (e >= 'a' && e <= 'f')
                        {
                            e = (char)((int)e - ((int)'a' - (int)'A'));
                        }
                        if (e >= 'A' && e <= 'F')
                        {
                            int a = (int)(e - 'A');
                            switch (a)
                            {
                                case 0: binaryDigit += "1010"; break;
                                case 1: binaryDigit += "1011"; break;
                                case 2: binaryDigit += "1100"; break;
                                case 3: binaryDigit += "1101"; break;
                                case 4: binaryDigit += "1110"; break;
                                case 5: binaryDigit += "1111"; break;
                                default:
                                    break;
                            }
                        }
                        else if (char.IsDigit(e))
                        {
                            int b = (int)(e - '0');

                            switch (b)
                            {
                                case 1: binaryDigit += "0001"; break;
                                case 2: binaryDigit += "0010"; break;
                                case 3: binaryDigit += "0011"; break;
                                case 4: binaryDigit += "0100"; break;
                                case 5: binaryDigit += "0101"; break;
                                case 6: binaryDigit += "0110"; break;
                                case 7: binaryDigit += "0111"; break;
                                case 8: binaryDigit += "1000"; break;
                                case 9: binaryDigit += "1001"; break;
                                default:
                                    break;
                            }
                        }
                    }

                    return binaryDigit;
                }

                // 检测阀报警次数
                private void CheckAlarmCount()
                {
                    if (initParams.IsDetectAlarmCount)
                    {
                        if (valveAlarmIO.Read())
                        {
                            communicator.Write(readAlarmCommand);
                            communicator.ReadTimeout(50, out byte[] response);
                            string cReturnHex = StringConverter.BytesToHexString(response);
                            string cReturnHexLog = cReturnHex;

                            string sDataStr = cReturnHexLog;
                            string sDataStr2 = sDataStr.Substring(6, 4); ;
                            sDataStr = HexStrToBinStr(sDataStr2);
                            sDataStr = new string(sDataStr.Reverse().ToArray());

                            for (int i = 0; i < sDataStr.Length; i++)
                            {
                                if (sDataStr[i] == '1')
                                {
                                    switch (i)
                                    {
                                        case 0://过流保护	E001
                                            {
                                                //if (eDeviceType == DeviceType_PiezoValve_GKG_Right)
                                                //    sComUserData.iErrorCode = -35010;

                                                //else if (eDeviceType == DeviceType_PiezoValve_GKG_Left)
                                                //    sComUserData.iErrorCode = -35016;
                                            }
                                            break;

                                        case 1://阀体过温	E002
                                            {
                                                //    if (eDeviceType == DeviceType_PiezoValve_GKG_Right)
                                                //        sComUserData.iErrorCode = -35011;

                                                //    else if (eDeviceType == DeviceType_PiezoValve_GKG_Left)
                                                //        sComUserData.iErrorCode = -35017;
                                            }
                                            break;

                                        case 2://控制器过温	E003

                                            {
                                                //if (eDeviceType == DeviceType_PiezoValve_GKG_Right)
                                                //    sComUserData.iErrorCode = -35012;

                                                //else if (eDeviceType == DeviceType_PiezoValve_GKG_Left)
                                                //    sComUserData.iErrorCode = -35018;
                                            }
                                            break;

                                        case 3://阀体存储器异常	E005
                                            {
                                                //if (eDeviceType == DeviceType_PiezoValve_GKG_Right)
                                                //    sComUserData.iErrorCode = -35013;

                                                //else if (eDeviceType == DeviceType_PiezoValve_GKG_Left)
                                                //    sComUserData.iErrorCode = -35019;
                                            }
                                            break;

                                        case 5://电源异常	E019
                                            {
                                                //if (eDeviceType == DeviceType_PiezoValve_GKG_Right)
                                                //    sComUserData.iErrorCode = -35014;

                                                //else if (eDeviceType == DeviceType_PiezoValve_GKG_Left)
                                                //    sComUserData.iErrorCode = -35020;
                                            }
                                            break;

                                        case 6://电源异常	E020
                                            {
                                                //if (eDeviceType == DeviceType_PiezoValve_GKG_Right)
                                                //    sComUserData.iErrorCode = -35015;

                                                //else if (eDeviceType == DeviceType_PiezoValve_GKG_Left)
                                                //    sComUserData.iErrorCode = -35021;
                                            }
                                            break;

                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                        //if (alarmCount >= initParams.AlarmCountDetect)
                        //{
                        //    throw new Exception($"Piezo Valve {initParams.Name} Alarm Count Exceeded Limit: {alarmCount}");
                        //}
                    }
                }
            }
        }
    }
}