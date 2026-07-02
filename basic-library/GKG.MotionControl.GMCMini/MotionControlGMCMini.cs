using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG.ElectronicControl
{
    namespace GMCMini
    {
        public class MotionControlGMCMini : MotionControlBase
        {
            public override int IniMotionCard(int CardNo)
            {
                base.IniMotionCard(CardNo);

                //debug
                _supportAxisNum = 24;
                _supportIoStateNum = 255;
                _supportAnalogNum = 0;

                int rtn = 0;                  // 指令返回值
                rtn = GMC_MINI.DLL_InitCard(); // 控制卡初始化

                if (rtn <= 0)//卡数量不大于0
                {
                    //debug
                    //return rtn;
                }
                //AxisLock = new Mutex[SupportAxisNum];
                //// 初始化轴锁定互斥量
                //for (int i = 0; i < SupportAxisNum; i++)
                //{
                //    AxisLock[i] = new Mutex();
                //}

                // 初始化IO锁定互斥量
                //IoLock = new Mutex[SupportIoStateNum];
                //for (int i = 0; i < SupportIoStateNum; i++)
                //{
                //    IoLock[i] = new Mutex();
                //}

                //for(int i = 0; i < SupportAxisNum; i++)
                //{
                //    AxisEnabled(i, true);
                //    Thread.Sleep(100);
                //}

                return rtn;
            }

            /// <summary>
            /// 反初始化运控卡
            /// </summary>
            public override int CloseMotionCard(int CardNo)
            {
                int rtn = GMC_MINI.DLL_CloseCard();
                return rtn;
            }

            /// <summary>
            /// 获取版本号
            /// </summary>
            public override int GetCardSoftVersion()
            {
                return 0;
            }

            /// <summary>
            /// 读取单轴状态 是否报警,是否使能
            /// </summary>
            public override bool GetAxisState(int axis, MotionControlAxisStatus motionStatus)
            {
                Thread.Sleep(10);
                bool isSignal = false;
                switch(motionStatus)
                {
                    case MotionControlAxisStatus.Origin:
                        isSignal = GMC_MINI.DLL_GetAxisStatus(axis, (int)GMC_MINI.EnumSensorType.SensorType_Home, 1) > 0;
                        break;
                    case MotionControlAxisStatus.ServoEnable:
                        isSignal = GMC_MINI.DLL_CheckServoOn(axis) > 0;
                        break;
                    case MotionControlAxisStatus.Alarm:
                        isSignal = GMC_MINI.DLL_GetAxisStatus(axis, (int)GMC_MINI.EnumSensorType.SensorType_Alarm, 1) > 0;
                        break;
                    case MotionControlAxisStatus.PositiveLimit:
                        isSignal = GMC_MINI.DLL_GetAxisStatus(axis, (int)GMC_MINI.EnumSensorType.SensorType_PositiveLimit, 1) > 0;
                        break;
                    case MotionControlAxisStatus.NegativeLimit:
                        isSignal = GMC_MINI.DLL_GetAxisStatus(axis, (int)GMC_MINI.EnumSensorType.SensorType_NegativeLimit, 1) > 0;
                        break;
                }
                return isSignal;
            }

            /// <summary>
            /// 读取单轴状态 是否报警,是否使能
            /// </summary>
            public override bool GetAxisState(Guid guid, MotionControlAxisStatus motionStatus)
            {
                Thread.Sleep(10);
                bool isSignal = false;
                int axisNo = GetAxisByGuid(guid);
                switch (motionStatus)
                {
                    case MotionControlAxisStatus.Origin:
                        isSignal = GMC_MINI.DLL_GetAxisStatus(axisNo, (int)GMC_MINI.EnumSensorType.SensorType_Home, 1) > 0;
                        break;
                    case MotionControlAxisStatus.ServoEnable:
                        isSignal = GMC_MINI.DLL_CheckServoOn(axisNo) > 0;
                        break;
                    case MotionControlAxisStatus.Alarm:
                        isSignal = GMC_MINI.DLL_GetAxisStatus(axisNo, (int)GMC_MINI.EnumSensorType.SensorType_Alarm, 1) > 0;
                        break;
                    case MotionControlAxisStatus.PositiveLimit:
                        isSignal = GMC_MINI.DLL_GetAxisStatus(axisNo, (int)GMC_MINI.EnumSensorType.SensorType_PositiveLimit, 1) > 0;
                        break;
                    case MotionControlAxisStatus.NegativeLimit:
                        isSignal = GMC_MINI.DLL_GetAxisStatus(axisNo, (int)GMC_MINI.EnumSensorType.SensorType_NegativeLimit, 1) > 0;
                        break;
                }
                return isSignal;
            }

            /// <summary>
            /// 绝对运动
            /// </summary>
            public override int AbsoluteMove(Guid guid, int motionType, double pos, double startSpeed,
                double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS)
            {
                Thread.Sleep(10);
                int axisNo = GetAxisByGuid(guid);
                int targetPosPulse = LengthToPulse(axisNo, pos);
                int targetSpeedPulsePerMs = SpeedMmPerSecToPulsePerS(axisNo, Math.Abs(maxSpeed));
                return GMC_MINI.DLL_AbsMove(axisNo, targetSpeedPulsePerMs, targetPosPulse);
            }

            /// <summary>
            /// 相对运动
            /// </summary>
            public override int RelativeMove(Guid guid, int motionType, double pos, double startSpeed,
                double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS)
            {
                Thread.Sleep(10);
                int axisNo = GetAxisByGuid(guid);
                int distancePulse = LengthToPulse(axisNo, pos);
                int targetSpeedPulsePerMs = SpeedMmPerSecToPulsePerS(axisNo, Math.Abs(maxSpeed));
                return GMC_MINI.DLL_RelativeMove(axisNo, targetSpeedPulsePerMs, distancePulse);
            }

            /// <summary>
            /// 连续运动
            /// </summary>
            public override int VelocityMove(Guid guid, int motionType, double startSpeed,
                double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS)
            {
                Thread.Sleep(10);
                int axisNo = GetAxisByGuid(guid);
                int targetSpeedPulsePerMs = SpeedMmPerSecToPulsePerS(axisNo, maxSpeed);
                return GMC_MINI.DLL_VelocityMove(axisNo, targetSpeedPulsePerMs);
            }

            /// <summary>
            /// 轴停止
            /// </summary>
            public override int AxisStop(Guid guid, MotionControlAxisStopTypeConstants motionStopType)
            {
                Thread.Sleep(10);
                int rtn = 0;
                int axisNo = GetAxisByGuid(guid);
                rtn = GMC_MINI.DLL_Stop(axisNo);
                return rtn;
            }

            /// <summary>
            /// 单轴回零
            /// </summary>
            public override int AxisHome(Guid guid)
            {
                Thread.Sleep(10);
                int rtn = 0;
                int axisNo = GetAxisByGuid(guid);
                int speed = SpeedMmPerSecToPulsePerS(axisNo, MotionControlFactoryParameters.Parameters[axisNo].AxisHomingParameters.HomingMaximumSpeed);
                rtn = GMC_MINI.DLL_HomeMove(axisNo, speed);

                return rtn;
            }

            /// <summary>
            /// 使能开关
            /// </summary>
            public override int AxisEnabled(int axis, bool isEnabled)
            {
                Thread.Sleep(10);
                int rtn = GMC_MINI.DLL_SetServo(axis, isEnabled ? 1 : 0);
                return rtn;
            }

            /// <summary>
            /// 获取轴当前位置
            /// </summary>
            public override double GetAxisPos(int axis, MotionControlAxisPositionType motionAxisType)
            {
                Thread.Sleep(10);
                double posPulse = GMC_MINI.DLL_GetPos(axis);
                return PulseToLength(axis, (int)Math.Round(posPulse));
            }

            /// <summary>
            /// 获取轴当前位置
            /// </summary>
            public override double GetAxisPos(Guid guid, MotionControlAxisPositionType motionAxisType)
            {
                Thread.Sleep(10);
                int axisNo = GetAxisByGuid(guid);
                double posPulse = GMC_MINI.DLL_GetPos(axisNo);
                return PulseToLength(axisNo, (int)Math.Round(posPulse));
            }

            /// <summary>
            /// 设置轴当前位置
            /// </summary>
            public override double SetAxisPos(int axis, double axisPos)
            {
                Thread.Sleep(10);
                int axisPosPulse = LengthToPulse(axis, axisPos);
                int rtn = GMC_MINI.DLL_SetPos(axis, axisPosPulse);
                return rtn;
            }

            /// <summary>
            /// 等待轴停止运动
            /// </summary>
            public override int WaitAxisStop(Guid guid, int timeOut)
            {
                Thread.Sleep(10);
                int rtn = 0;
                int axisNo = GetAxisByGuid(guid);
                rtn = GMC_MINI.DLL_WaitDone(axisNo, timeOut);
                return rtn;
            }

            /// <summary>
            /// 清除轴报警
            /// </summary>
            public override int ClearAxisAlarm(Guid guid)
            {
                //无
                return 0;
            }

            /// <summary>
            /// 轴软限位设定
            /// </summary>
            public override int SetAxisSoftLimit(Guid guid, MotionControlAxisSoftLimit motionAxisSoft)
            {
                return 0;
            }

            /// <summary>
            /// 轴软限位读取
            /// </summary>
            public override MotionControlAxisSoftLimit GetAxisSoftLimit(int axis)
            {
                return new MotionControlAxisSoftLimit();
            }

            /// <summary>
            /// 轴软限位读取
            /// </summary>
            public override MotionControlAxisSoftLimit GetAxisSoftLimit(Guid guid)
            {
                return new MotionControlAxisSoftLimit();
            }

            /// <summary>
            /// 获取单轴规划速度
            /// </summary>
            public override double GetAxisPlanSpeed(int axis)
            {
                return 0;
            }

            /// <summary>
            /// 获取单轴规划速度
            /// </summary>
            public override double GetAxisPlanSpeed(Guid guid)
            {
                int axisNo = GetAxisByGuid(guid);
                return 0;
            }

            /// <summary>
            /// 获取电机实际速度
            /// </summary>
            public override double GetAxisActualSpeed(Guid guid)
            {
                Thread.Sleep(10);
                int axisNo = GetAxisByGuid(guid);
                int pulsePerMs = GMC_MINI.DLL_GetAxisVel(axisNo);
                return pulsePerMs * GetPulseEquivalent(axisNo) * 1000.0;
            }

            /// <summary>
            /// 获取电机实际速度
            /// </summary>
            public override int GetAxisActualSpeed(int axis)
            {
                Thread.Sleep(10);
                int pulsePerMs = GMC_MINI.DLL_GetAxisVel(axis);
                return (int)Math.Round(pulsePerMs * GetPulseEquivalent(axis) * 1000.0);
            }

            /// <summary>
            /// 二轴直线插补
            /// </summary>
            public override int TwoAxisLinearInterpolation(Guid[] guid, int coordinateSystemNo, double[] targetPos, int motionType, double startSpeed,
                double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS)
            {
                short rtn = 0;
                return rtn;
            }

            /// <summary>
            /// 三轴直线插补
            /// </summary>
            public override int ThreeAxisLinearInterpolation(Guid[] guid, int coordinateSystemNo, double[] targetPos, int motionType, double startSpeed,
                double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS)
            {
                short rtn = 0;
                return rtn;
            }

            /// <summary>
            /// 二轴圆弧插补
            /// </summary>
            public override int TwoAxisCircularInterpolation(Guid[] guid, int coordinateSystemNo, double[] targetPos, double[] centerCircle, int arcDirection, int motionType,
                double startSpeed, double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS)
            {
                short rtn = 0;
                return rtn;
            }

            /// <summary>
            /// 三轴圆弧插补
            /// </summary>
            public override int ThreeAxisCircularInterpolation(Guid[] guid, int coordinateSystemNo, double[] targetPos, double[] centerCircle, int arcDirection, int motionType,
                double startSpeed, double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS)
            {
                short rtn = 0;
                return rtn;
            }

            /// <summary>
            /// 设置运动参数
            /// </summary>
            public override int SetAxisMoveParameter(Guid guid, MotionControlArcFeedForwardParameters motionForwardParamter, MotionControlPositionComparison2DParameters motion2DParamter)
            {
                return 0;
            }

            /// <summary>
            /// 读输入状态量
            /// </summary>
            public override bool GetInputState(int ioChannel)
            {
                Thread.Sleep(10);
                int inputNum = GMC_MINI.DLL_ReadSingleBit(ioChannel);
                return (inputNum > 0);
            }

            /// <summary>
            /// 读输出状态量
            /// </summary>
            public override bool GetOutputState(int ioChannel)
            {
                Thread.Sleep(10);
                int outputNum = GMC_MINI.DLL_ReadOutputBit(ioChannel);
                return (outputNum > 0);
            }

            /// <summary>
            /// 读输入输出状态量
            /// </summary>
            public override bool GetInOutputState(Guid guid)
            {
                return false;
            }

            /// <summary>
            /// 写输出状态量
            /// </summary>
            public override int SetOutputState(Guid guid, bool isHave)
            {
                Thread.Sleep(10);
                int ioChannel = GetIoByGuid(guid);
                int ioNum = isHave ? 1 : 0;
                int rtn = GMC_MINI.DLL_WriteOutputBit(ioChannel, ioNum);
                return rtn;
            }

            /// <summary>
            /// 读输入模拟量
            /// </summary>
            public override double GetInputNum(int ioChannel)
            {
                return 0;
            }

            /// <summary>
            /// 读输出模拟量
            /// </summary>
            public override double GetOutputNum(int ioChannel)
            {
                return 0;
            }

            /// <summary>
            /// 读输入输出模拟量
            /// </summary>
            public override double GetInOutputNum(Guid guid)
            {
                return 0;
            }

            /// <summary>
            /// 写输入输出模拟量
            /// </summary>
            public override double SetInOutputNum(Guid guid, double analogValue)
            {
                return 0;
            }

            #region IO接口定义方法
            /// <summary>
            /// 创建设备的通道名称
            /// </summary>
            /// <param name="eReadWriteMode">读写模式</param>
            /// <param name="bitno">端口号</param>
            /// <returns></returns>
            /// <exception cref="Exception"></exception>
            private static string createChannelID(EReadWriteMode eReadWriteMode, ushort bitno)
            {
                switch (eReadWriteMode)
                {
                    case EReadWriteMode.ReadWrite:
                        return ReadWriteModeConstStr.ReadWriteStr + bitno.ToString("000");
                    case EReadWriteMode.ReadOnly:
                        return ReadWriteModeConstStr.ReadOnlyStr + bitno.ToString("000");
                    case EReadWriteMode.WriteOnly:
                        return ReadWriteModeConstStr.WriteOnlyStr + bitno.ToString("000");
                    default:
                        throw new Exception();
                }
            }

            /// <summary>
            /// 解析通道名称获取通道号
            /// </summary>
            /// <param name="channelID"></param>
            /// <param name="eReadWriteMode">读写模式</param>
            /// <param name="bitno">端口号</param>
            /// <exception cref="Exception"></exception>
            private static void splitChannelID(string channelID, out EReadWriteMode eReadWriteMode, out ushort bitno)
            {
                string readWriteModeStr = channelID.Substring(0, 2);
                string channelIDStr = "";
                if(channelID.Length == 5)
                    channelIDStr = channelID.Substring(2, 3);
                else
                    channelIDStr = channelID.Substring(2, 2);

                switch (readWriteModeStr)
                    {
                        case ReadWriteModeConstStr.ReadOnlyStr:
                            eReadWriteMode = EReadWriteMode.ReadOnly;
                            break;
                        case ReadWriteModeConstStr.WriteOnlyStr:
                            eReadWriteMode = EReadWriteMode.WriteOnly;
                            break;
                        case ReadWriteModeConstStr.ReadWriteStr:
                            eReadWriteMode = EReadWriteMode.ReadWrite;
                            break;
                        default:
                            throw new Exception();
                    }
                bitno = Convert.ToUInt16(channelIDStr);
            }

            static MotionControlGMCMini()
            {
                analogChannelList = new ChannelParametersList();
                stateChannelList = new ChannelParametersList();
                for (ushort i = 0; i < 255; i++)
                {
                    stateChannelList.Add(new ChannelParameters() { channelID = createChannelID(EReadWriteMode.ReadOnly, i), channelMode = EReadWriteMode.ReadOnly });
                    stateChannelList.Add(new ChannelParameters() { channelID = createChannelID(EReadWriteMode.ReadWrite, i), channelMode = EReadWriteMode.ReadWrite });
                }
            }
            private static readonly ChannelParametersList analogChannelList;
            private static ChannelParametersList stateChannelList;

            public override void InitIOCard(string initCfg)
            {
                throw new NotSupportedException($"{nameof(MotionControlGMCMini)} 不支持通过 InitIOCard 进行独立 IO 初始化。");
            }

            public override void UnInitIOCard()
            {
                throw new NotSupportedException($"{nameof(MotionControlGMCMini)} 不支持通过 UnInitIOCard 进行独立 IO 反初始化。");
            }

            public override decimal AnalogRead(string channelID)
            {
                throw new NotSupportedException($"{nameof(MotionControlGMCMini)} 暂不支持模拟量读取。");
            }

            public override void AnalogWrite(string channelID, decimal analog)
            {
                throw new NotSupportedException($"{nameof(MotionControlGMCMini)} 暂不支持模拟量写入。");
            }

            public override ChannelParametersList AnalogChannelList => analogChannelList;
            public override ChannelParametersList StateChannelList => stateChannelList;
            public override EReadWriteMode ReadWriteMode => EReadWriteMode.ReadWrite;

            public override bool StateRead(string channelID)
            {
                splitChannelID(channelID, out EReadWriteMode eReadWriteMode, out ushort bitno);
                bool ret = false;
                if (eReadWriteMode == EReadWriteMode.ReadOnly)
                {
                    ret = Convert.ToBoolean(GetInputState(bitno));
                }
                else if (eReadWriteMode == EReadWriteMode.ReadWrite)
                {
                    ret = Convert.ToBoolean(GetOutputState(bitno));
                }
                return ret;
            }

            public override void StateWrite(string channelID, bool state)
            {
                splitChannelID(channelID, out EReadWriteMode eReadWriteMode, out ushort bitno);
                if (eReadWriteMode != EReadWriteMode.ReadWrite)
                    throw new Exception();
                var guid = LockIO(bitno);
                SetOutputState(guid, state);
                UnLockIO(guid);
            }
            #endregion
        }
    }
}
