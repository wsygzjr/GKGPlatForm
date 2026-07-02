using System.Collections.Concurrent;

namespace GKG
{
    namespace ElectronicControl
    {
        /// <summary>
        /// 基础接口
        /// </summary>
        public interface IMotionControlBase : IBaseIO
        {
            /// <summary>
            /// 支持的轴数量
            /// </summary>
            int SupportAxisNum { get; }

            /// <summary>
            /// 支持的模拟量数量
            /// </summary>
            int SupportAnalogNum { get; }

            /// <summary>
            /// 支持的状态量数量
            /// </summary>
            int SupportIoStateNum { get; }

            /// <summary>
            /// 是否支持二轴直线插补
            /// </summary>
            bool IsTwoAxisLinearInterpolation { get; }

            /// <summary>
            /// 是否支持三轴直线插补
            /// </summary>
            bool IsThreeAxisLinearInterpolation { get; }

            /// <summary>
            /// 是否支持二轴圆弧插补
            /// </summary>
            bool IsTwoAxisCircularInterpolation { get; }

            /// <summary>
            /// 是否支持三轴圆弧插补
            /// </summary>
            bool IsThreeAxisCircularInterpolation { get; }

            /// <summary>
            /// 设置出厂参数
            /// </summary>
            void SetFactoryParameters(MotionControlFactoryParameters motionControlFactoryParameters);

            /// <summary>
            /// 设置初始化参数
            /// </summary>
            void SetInitializationParameters(MotionControlInitializationParameters initializationParameters);

            /// <summary>
            /// 初始化运控卡
            /// </summary>
            int IniMotionCard(int cardNo);

            /// <summary>
            /// 反初始化运控卡
            /// </summary>
            int CloseMotionCard(int cardNo);

            /// <summary>
            /// 获取版本号
            /// </summary>
            int GetCardSoftVersion();

            /// <summary>
            /// 轴锁定 返回轴锁定句柄Guid
            /// </summary>
            Guid LockAxis(int axis, int timeOut);

            /// <summary>
            /// 轴解锁
            /// </summary>
            void UnLockAxis(Guid guid);

            /// <summary>
            /// 获取轴锁定状态
            /// </summary>
            bool GetAxisLockState(int axis);

            /// <summary>
            /// 读取单轴状态 是否报警,是否使能
            /// </summary>
            bool GetAxisState(int axis, MotionControlAxisStatus motionStatus);

            /// <summary>
            /// 读取单轴状态 是否报警,是否使能
            /// </summary>
            bool GetAxisState(Guid guid, MotionControlAxisStatus motionStatus);

            /// <summary>
            /// 绝对运动
            /// </summary>
            int AbsoluteMove(Guid guid, int motionType, double pos, double startSpeed,
                double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS);

            /// <summary>
            /// 相对运动
            /// </summary>
            int RelativeMove(Guid guid, int motionType, double pos, double startSpeed,
                double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS);

            /// <summary>
            /// 连续运动
            /// </summary>
            int VelocityMove(Guid guid, int motionType, double startSpeed,
                double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS);

            /// <summary>
            /// 轴停止
            /// </summary>
            int AxisStop(Guid guid, MotionControlAxisStopTypeConstants motionStopType);

            /// <summary>
            /// 单轴回零
            /// </summary>
            int AxisHome(Guid guid);

            /// <summary>
            /// 使能开关
            /// </summary>
            int AxisEnabled(int axis, bool isEnabled);

            /// <summary>
            /// 获取轴当前位置
            /// </summary>
            double GetAxisPos(int axis, MotionControlAxisPositionType motionAxisType);

            /// <summary>
            /// 获取轴当前位置
            /// </summary>
            double GetAxisPos(Guid guid, MotionControlAxisPositionType motionAxisType);

            /// <summary>
            /// 设置轴当前位置
            /// </summary>
            double SetAxisPos(int axis, double axisPos);

            /// <summary>
            /// 等待轴停止运动
            /// </summary>
            int WaitAxisStop(Guid guid, int timeOut);

            /// <summary>
            /// 清除轴报警
            /// </summary>
            int ClearAxisAlarm(Guid guid);

            /// <summary>
            /// 轴软限位设定
            /// </summary>
            int SetAxisSoftLimit(Guid guid, MotionControlAxisSoftLimit motionAxisSoft);

            /// <summary>
            /// 轴软限位读取
            /// </summary>
            MotionControlAxisSoftLimit GetAxisSoftLimit(int axis);

            /// <summary>
            /// 轴软限位读取
            /// </summary>
            MotionControlAxisSoftLimit GetAxisSoftLimit(Guid guid);

            /// <summary>
            /// 获取单轴规划速度
            /// </summary>
            double GetAxisPlanSpeed(int axis);

            /// <summary>
            /// 获取单轴规划速度
            /// </summary>
            double GetAxisPlanSpeed(Guid guid);

            /// <summary>
            /// 获取电机实际速度
            /// </summary>
            double GetAxisActualSpeed(Guid guid);

            /// <summary>
            /// 获取电机实际速度
            /// </summary>
            int GetAxisActualSpeed(int axis);

            /// <summary>
            /// 二轴直线插补
            /// </summary>
            int TwoAxisLinearInterpolation(Guid[] guid, int coordinateSystemNo, double[] targetPos, int motionType, double startSpeed,
                double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS);

            /// <summary>
            /// 三轴直线插补
            /// </summary>
            int ThreeAxisLinearInterpolation(Guid[] guid, int coordinateSystemNo, double[] targetPos, int motionType, double startSpeed,
                double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS);

            /// <summary>
            /// 二轴圆弧插补
            /// </summary>
            int TwoAxisCircularInterpolation(Guid[] guid, int coordinateSystemNo, double[] targetPos, double[] centerCircle, int arcDirection, int motionType,
                double startSpeed, double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS);

            /// <summary>
            /// 三轴圆弧插补
            /// </summary>
            int ThreeAxisCircularInterpolation(Guid[] guid, int coordinateSystemNo, double[] targetPos, double[] centerCircle, int arcDirection, int motionType,
                double startSpeed, double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS);

            /// <summary>
            /// 设置运动参数
            /// </summary>
            int SetAxisMoveParameter(Guid guid, MotionControlArcFeedForwardParameters motionForwardParamter, MotionControlPositionComparison2DParameters motion2DParamter);

            /// <summary>
            /// IO锁定
            /// </summary>
            Guid LockIO(int ioChannel);

            /// <summary>
            /// IO解锁
            /// </summary>
            int UnLockIO(Guid guid);

            /// <summary>
            /// 获取IO锁定状态
            /// </summary>
            bool GetIOState(int ioChannel);

            /// <summary>
            /// 读输入状态量
            /// </summary>
            bool GetInputState(int ioChannel);

            /// <summary>
            /// 读输出状态量
            /// </summary>
            bool GetOutputState(int ioChannel);

            /// <summary>
            /// 读输入输出状态量
            /// </summary>
            bool GetInOutputState(Guid guid);

            /// <summary>
            /// 写输出状态量
            /// </summary>
            int SetOutputState(Guid guid, bool isHave);

            /// <summary>
            /// 读输入模拟量
            /// </summary>
            double GetInputNum(int ioChannel);

            /// <summary>
            /// 读输出模拟量
            /// </summary>
            double GetOutputNum(int ioChannel);

            /// <summary>
            /// 读输入输出模拟量
            /// </summary>
            double GetInOutputNum(Guid guid);

            /// <summary>
            /// 写输入输出模拟量
            /// </summary>
            double SetInOutputNum(Guid guid, double analogValue);

            /// <summary>
            /// 等待坐标系运动完成
            /// </summary>
            /// <param name="coordinateSystemNo"></param>
            /// <param name="timeout"></param>
            /// <returns></returns>
            int WaitCrdMoveDone(int coordinateSystemNo, int timeout = 1000);
        }

        public abstract class MotionControlBase : IMotionControlBase
        {
            #region 受保护字段与属性

            /// <summary>
            /// 轴锁定互斥量
            /// </summary>
            protected ConcurrentDictionary<int, Mutex> AxisLock { get; set; } = new ConcurrentDictionary<int, Mutex>();
            /// <summary>
            /// 轴锁定句柄字典
            /// key:轴锁定句柄Guid
            /// value:轴号
            /// </summary>
            protected Dictionary<Guid, int> axisLockDict = new Dictionary<Guid, int>();

            /// <summary>
            /// IO锁定互斥量
            /// </summary>
            protected ConcurrentDictionary<int, Mutex> IoLock { get; set; } = new ConcurrentDictionary<int, Mutex>();

            /// <summary>
            /// Io锁定句柄字典
            /// key:Io锁定句柄Guid
            /// value:Io号
            /// </summary>
            protected Dictionary<Guid, int> IoLockDict = new Dictionary<Guid, int>();

            /// <summary>
            /// 退出循环标志
            /// </summary>
            protected bool ExitLoop = false;

            #endregion

            #region 受保护方法

            /// <summary>
            /// 通过句柄获取轴号
            /// </summary>
            /// <param name="axisGuid">轴锁定句柄</param>
            /// <returns>轴号</returns>
            protected int GetAxisByGuid(Guid axisGuid)
            {
                if (!axisLockDict.ContainsKey(axisGuid))
                {
                    throw new ArgumentException(nameof(axisGuid), $"未找到对应的轴锁定句柄");
                }
                return axisLockDict[axisGuid];
            }

            /// <summary>
            /// 通过句柄获取IO号
            /// </summary>
            /// <param name="axisGuid">Io锁定句柄</param>
            /// <returns>IO号</returns>
            protected int GetIoByGuid(Guid ioGuid)
            {
                if (!IoLockDict.ContainsKey(ioGuid))
                {
                    throw new ArgumentException(nameof(ioGuid), $"未找到对应的IO锁定句柄");
                }
                return IoLockDict[ioGuid];
            }

            /// <summary>
            /// 获取轴的脉冲当量（mm/pulse）
            /// </summary>
            /// <param name="axis"></param>
            /// <returns></returns>
            protected virtual double GetPulseEquivalent(int axis)
            {
                if (MotionControlFactoryParameters.Parameters == null)
                {
                    throw new InvalidOperationException("MotionControlFactoryParameters.Parameters未初始化");
                }
                if (axis < 0 || axis >= MotionControlFactoryParameters.Parameters.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(axis), "轴号超出范围");
                }
                if (MotionControlFactoryParameters.Parameters[axis].PulseRatioParameters.PulseRatio == 0)
                {
                    throw new Exception($"轴{axis}脉冲比参数为0");
                }
                else
                {
                    //脉冲比(mm/pulse)*1000取倒数
                    return (MotionControlFactoryParameters.Parameters[axis].PulseRatioParameters.PulseRatio / 1000.0);
                }
            }

            /// <summary>
            /// 脉冲比转换，将物理量长度转换为脉冲数，mm->pulse
            /// </summary>
            /// <param name="axis">轴号</param>
            /// <param name="length">长度(mm)</param>
            /// <returns>长度对应的脉冲数(pulse)</returns>
            protected virtual int LengthToPulse(int axis, double length)
            {
                return (int)(length / GetPulseEquivalent(axis));
            }

            /// <summary>
            /// 脉冲比转换，将脉冲数转换为物理量长度，pulse->mm
            /// </summary>
            /// <param name="axis">轴号</param>
            /// <param name="pulse">脉冲数(pulse)</param>
            /// <returns>脉冲数对应的长度(mm)</returns>
            protected virtual double PulseToLength(int axis, int pulse)
            {
                return pulse * GetPulseEquivalent(axis);
            }

            /// <summary>
            /// 将速度(mm/s)转换为脉冲/ms
            /// </summary>
            /// <param name="axis">轴号</param>
            /// <param name="speedMmPerSec">速度(mm/s)</param>
            /// <returns>脉冲速度(pulse/ms)</returns>
            protected virtual int SpeedMmPerSecToPulsePerMs(int axis, double speedMmPerSec)
            {
                double pulseEquivalent = GetPulseEquivalent(axis); // mm/脉冲
                if (pulseEquivalent <= 0) return 0;
                // 速度(mm/s) / 脉冲当量(mm/脉冲) = 脉冲/s
                // 脉冲/s / 1000 = 脉冲/ms
                return (int)(speedMmPerSec / pulseEquivalent / 1000.0);
            }
            /// <summary>
            /// 将速度(mm/s)转换为脉冲/s
            /// </summary>
            /// <param name="axis">轴号</param>
            /// <param name="speedMmPerSec">速度(mm/s)</param>
            /// <returns>脉冲速度(pulse/s)</returns>
            protected virtual int SpeedMmPerSecToPulsePerS(int axis, double speedMmPerSec)
            {
                double pulseEquivalent = GetPulseEquivalent(axis); // mm/脉冲
                if (pulseEquivalent <= 0) return 0;
                // 速度(mm/s) / 脉冲当量(mm/脉冲) = 脉冲/s
                // 脉冲/s / 1000 = 脉冲/ms
                return (int)(speedMmPerSec / pulseEquivalent);
            }
            protected int _supportAxisNum { get; set; }

            /// <summary>
            /// 支持的轴数量
            /// </summary>
            public int SupportAxisNum
            {
                get => _supportAxisNum;
            }

            protected int _supportAnalogNum { get; set; }

            /// <summary>
            /// 支持的模拟量数量
            /// </summary>
            public int SupportAnalogNum => _supportAnalogNum;


            protected int _supportIoStateNum { get; set; }

            /// <summary>
            /// 支持的状态量数量
            /// </summary>
            public int SupportIoStateNum
            {
                get => _supportIoStateNum;
            }

            protected bool _isTwoAxisLinearInterpolation { get; set; }

            /// <summary>
            /// 是否支持二轴直线插补
            /// </summary>
            public bool IsTwoAxisLinearInterpolation
            {
                get => _isTwoAxisLinearInterpolation;
            }

            protected bool _isThreeAxisLinearInterpolation { get; set; }

            /// <summary>
            /// 是否支持三轴直线插补
            /// </summary>
            public bool IsThreeAxisLinearInterpolation
            {
                get => _isThreeAxisLinearInterpolation;
            }

            protected bool _isTwoAxisCircularInterpolation { get; set; }

            /// <summary>
            /// 是否支持二轴圆弧插补
            /// </summary>
            public bool IsTwoAxisCircularInterpolation
            {
                get => _isTwoAxisCircularInterpolation;
            }

            public bool _isThreeAxisCircularInterpolation { get; set; }

            /// <summary>
            /// 是否支持三轴圆弧插补
            /// </summary>
            public bool IsThreeAxisCircularInterpolation
            {
                get => _isThreeAxisCircularInterpolation;
            }

            /// <summary>
            /// 出厂参数
            /// </summary>
            protected MotionControlFactoryParameters MotionControlFactoryParameters { get; set; } = new MotionControlFactoryParameters();

            /// <summary>
            /// 初始化参数
            /// </summary>
            protected MotionControlInitializationParameters MotionControlInitializationParameters { get; set; } = new MotionControlInitializationParameters();

            /// <summary>
            /// 设置出厂参数
            /// </summary>
            public virtual void SetFactoryParameters(MotionControlFactoryParameters motionControlFactoryParameters)
            {
                if (motionControlFactoryParameters == null)
                    throw new ArgumentException("无效的出厂参数");

                // 设置出厂参数
                MotionControlFactoryParameters = motionControlFactoryParameters;
            }

            /// <summary>
            /// 设置初始化参数
            /// </summary>
            public virtual void SetInitializationParameters(MotionControlInitializationParameters initializationParameters)
            {
                if (initializationParameters == null)
                    throw new ArgumentException("无效的初始化参数");

                MotionControlInitializationParameters = initializationParameters;
            }

            /// <summary>
            /// 初始化运控卡
            /// </summary>
            public virtual int IniMotionCard(int cardNo)
            {
                // 初始化轴锁定互斥量
                for (int i = 0; i < SupportAxisNum; i++)
                {
                    AxisLock[i] = new Mutex();
                }
                return 0;
            }

            /// <summary>
            /// 反初始化运控卡
            /// </summary>
            public virtual int CloseMotionCard(int cardNo)
            {
                return 0;
            }

            /// <summary>
            /// 获取版本号
            /// </summary>
            public virtual int GetCardSoftVersion()
            {
                return 0;
            }

            /// <summary>
            /// 轴锁定 返回轴锁定句柄Guid
            /// </summary>
            public virtual Guid LockAxis(int axis, int timeOut)
            {
                if (AxisLock == null)
                    throw new InvalidOperationException("AxisLock未初始化");
                if (axis < 0 || axis >= SupportAxisNum)
                    throw new ArgumentOutOfRangeException(nameof(axis), "轴号超出范围");
                if (!TryLockAxis(axis, timeOut))
                {
                    throw new Exception($"轴{axis}超时未获取到轴锁定句柄");
                }
                if (axisLockDict.ContainsValue(axis))
                {
                    AxisLock[axis].ReleaseMutex();
                    throw new Exception($"轴{axis}已被锁定");
                }
                else
                {
                    Guid guid = Guid.NewGuid();
                    axisLockDict[guid] = axis;
                    AxisLock[axis].ReleaseMutex();
                    return guid;
                }
            }
                /// <summary>
                /// 轴解锁
                /// </summary>
                public virtual void UnLockAxis(Guid guid)
                {
                    if (axisLockDict.ContainsKey(guid))
                    {
                        axisLockDict.Remove(guid);
                    }
                    else
                    {
                        throw new Exception($"找不到轴锁定句柄{guid.ToString()}解锁");
                    }
                }

                private bool TryLockAxis(int axis, int timeout)
                {
                    bool acquired = false;
                    if (AxisLock.ContainsKey(axis))
                    {
                        acquired = AxisLock[axis].WaitOne(timeout);
                    }
                    else
                    {
                        AxisLock.TryAdd(axis, new Mutex());
                        acquired = AxisLock[axis].WaitOne(timeout);
                    }
                    return acquired;
                }

                /// <summary>
                /// 获取轴锁定状态
                /// </summary>
                public virtual bool GetAxisLockState(int axis)
                {
                    return axisLockDict.ContainsValue(axis);
                }

            /// <summary>
            /// 读取单轴状态 是否报警,是否使能
            /// </summary>
            public virtual bool GetAxisState(int axis, MotionControlAxisStatus motionStatus)
            {
                return true;
            }

            /// <summary>
            /// 读取单轴状态 是否报警,是否使能
            /// </summary>
            public virtual bool GetAxisState(Guid guid, MotionControlAxisStatus motionStatus)
            {
                return true;
            }

            /// <summary>
            /// 绝对运动
            /// </summary>
            public virtual int AbsoluteMove(Guid guid, int motionType, double pos, double startSpeed,
                double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS)
            {
                return 0;
            }

            /// <summary>
            /// 相对运动
            /// </summary>
            public virtual int RelativeMove(Guid guid, int motionType, double pos, double startSpeed,
                double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS)
            {
                return 0;
            }

            /// <summary>
            /// 连续运动
            /// </summary>
            public virtual int VelocityMove(Guid guid, int motionType, double startSpeed,
                double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS)
            {
                return 0;
            }

            /// <summary>
            /// 轴停止
            /// </summary>
            public virtual int AxisStop(Guid guid, MotionControlAxisStopTypeConstants motionStopType)
            {
                return 0;
            }

            /// <summary>
            /// 单轴回零
            /// </summary>
            public virtual int AxisHome(Guid guid)
            {
                return 0;
            }

            /// <summary>
            /// 使能开关
            /// </summary>
            public virtual int AxisEnabled(int axis, bool isEnabled)
            {
                return 0;
            }

            /// <summary>
            /// 获取轴当前位置
            /// </summary>
            public virtual double GetAxisPos(int axis, MotionControlAxisPositionType motionAxisType)
            {
                return 0;
            }

            /// <summary>
            /// 获取轴当前位置
            /// </summary>
            public virtual double GetAxisPos(Guid guid, MotionControlAxisPositionType motionAxisType)
            {
                return 0;
            }

            /// <summary>
            /// 设置轴当前位置
            /// </summary>
            public virtual double SetAxisPos(int axis, double axisPos)
            {
                return 0;
            }

            /// <summary>
            /// 等待轴停止运动
            /// </summary>
            public virtual int WaitAxisStop(Guid guid, int timeOut)
            {
                return 0;
            }

            /// <summary>
            /// 清除轴报警
            /// </summary>
            public virtual int ClearAxisAlarm(Guid guid)
            {
                return 0;
            }

            /// <summary>
            /// 轴软限位设定
            /// </summary>
            public virtual int SetAxisSoftLimit(Guid guid, MotionControlAxisSoftLimit motionAxisSoft)
            {
                return 0;
            }

            /// <summary>
            /// 轴软限位读取
            /// </summary>
            public virtual MotionControlAxisSoftLimit GetAxisSoftLimit(int axis)
            {
                return new MotionControlAxisSoftLimit();
            }

            /// <summary>
            /// 轴软限位读取
            /// </summary>
            public virtual MotionControlAxisSoftLimit GetAxisSoftLimit(Guid guid)
            {
                return new MotionControlAxisSoftLimit();
            }

            /// <summary>
            /// 获取单轴规划速度
            /// </summary>
            public virtual double GetAxisPlanSpeed(int axis)
            {
                return 0;
            }

            /// <summary>
            /// 获取单轴规划速度
            /// </summary>
            public virtual double GetAxisPlanSpeed(Guid guid)
            {
                return 0;
            }

            /// <summary>
            /// 获取电机实际速度
            /// </summary>
            public virtual double GetAxisActualSpeed(Guid guid)
            {
                return 0;
            }

            /// <summary>
            /// 获取电机实际速度
            /// </summary>
            public virtual int GetAxisActualSpeed(int axis)
            {
                return 0;
            }

            /// <summary>
            /// 二轴直线插补
            /// </summary>
            public virtual int TwoAxisLinearInterpolation(Guid[] guid, int coordinateSystemNo, double[] targetPos, int motionType, double startSpeed,
                double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS)
            {
                return 0;
            }

            /// <summary>
            /// 三轴直线插补
            /// </summary>
            public virtual int ThreeAxisLinearInterpolation(Guid[] guid, int coordinateSystemNo, double[] targetPos, int motionType, double startSpeed,
                double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS)
            {
                return 0;
            }

            /// <summary>
            /// 二轴圆弧插补
            /// </summary>
            public virtual int TwoAxisCircularInterpolation(Guid[] guid, int coordinateSystemNo, double[] targetPos, double[] centerCircle, int arcDirection, int motionType,
                double startSpeed, double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS)
            {
                return 0;
            }

            /// <summary>
            /// 三轴圆弧插补
            /// </summary>
            public virtual int ThreeAxisCircularInterpolation(Guid[] guid, int coordinateSystemNo, double[] targetPos, double[] centerCircle, int arcDirection, int motionType,
                double startSpeed, double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS)
            {
                return 0;
            }

            /// <summary>
            /// 设置运动参数
            /// </summary>
            public virtual int SetAxisMoveParameter(Guid guid, MotionControlArcFeedForwardParameters motionForwardParamter, MotionControlPositionComparison2DParameters motion2DParamter)
            {
                return 0;
            }

            /// <summary>
            /// IO锁定
            /// </summary>
            public virtual Guid LockIO(int ioChannel)
            {
                if (IoLock == null)
                    throw new InvalidOperationException("IoLock未初始化");
                if (ioChannel < 0 || ioChannel >= SupportIoStateNum)
                    throw new ArgumentOutOfRangeException(nameof(ioChannel), "Io端口号超出范围");
                //bool acquired = IoLock[ioChannel].WaitOne();
                if (!TryLockIO(ioChannel))
                {
                    throw new Exception($"IO端口{ioChannel}未获取到IO锁定句柄");
                }
                if (IoLockDict.ContainsValue(ioChannel))
                {
                    IoLock[ioChannel].ReleaseMutex();
                    throw new Exception($"IO端口{ioChannel}已被锁定");
                }
                else
                {
                    Guid guid = Guid.NewGuid();
                    IoLockDict[guid] = ioChannel;
                    return guid;
                }
            }

                /// <summary>
                /// IO解锁
                /// </summary>
                public virtual int UnLockIO(Guid guid)
                {
                    if (!IoLockDict.ContainsKey(guid))
                    {
                        throw new Exception($"找不到IO锁定句柄{guid.ToString()}解锁");
                    }
                    else
                    {
                        IoLock[IoLockDict[guid]].ReleaseMutex();
                        IoLockDict.Remove(guid);
                    }
                    return 0;
                }

                private bool TryLockIO(int ioChannel)
                {
                    bool acquired = false;
                    if (IoLock.ContainsKey(ioChannel))
                    {
                        acquired = IoLock[ioChannel].WaitOne(1000);
                    }
                    else
                    {
                        IoLock.TryAdd(ioChannel, new Mutex());
                        acquired = IoLock[ioChannel].WaitOne(1000);
                    }
                    return acquired;
                }

                /// <summary>
                /// 获取IO锁定状态
                /// </summary>
                public virtual bool GetIOState(int ioChannel)
                {
                    return IoLockDict.ContainsValue(ioChannel);
                }

            /// <summary>
            /// 读输入状态量
            /// </summary>
            public virtual bool GetInputState(int ioChannel)
            {
                return true;
            }

            /// <summary>
            /// 读输出状态量
            /// </summary>
            public virtual bool GetOutputState(int ioChannel)
            {
                return true;
            }

            /// <summary>
            /// 读输入输出状态量
            /// </summary>
            public virtual bool GetInOutputState(Guid guid)
            {
                return true;
            }

            /// <summary>
            /// 写输出状态量
            /// </summary>
            public virtual int SetOutputState(Guid guid, bool isHave)
            {
                return 0;
            }

            /// <summary>
            /// 读输入模拟量
            /// </summary>
            public virtual double GetInputNum(int ioChannel)
            {
                return 0;
            }

            /// <summary>
            /// 读输出模拟量
            /// </summary>
            public virtual double GetOutputNum(int ioChannel)
            {
                return 0;
            }

            /// <summary>
            /// 读输入输出模拟量
            /// </summary>
            public virtual double GetInOutputNum(Guid guid)
            {
                return 0;
            }

            /// <summary>
            /// 写输入输出模拟量
            /// </summary>
            public virtual double SetInOutputNum(Guid guid, double analogValue)
            {
                return 0;
            }

            /// <summary>
            /// 等待坐标系运动完成
            /// </summary>
            /// <param name="coordinateSystemNo"></param>
            /// <param name="timeout"></param>
            /// <returns></returns>
            public int WaitCrdMoveDone(int coordinateSystemNo, int timeout = 1000)
            {
                return 0;
            }

            #endregion

            #region 公有方法

            #region IO接口的实现

            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="initCfg"></param>
            public abstract void InitIOCard(string initCfg);

            /// <summary>
            /// 反初始化
            /// </summary>
            public abstract void UnInitIOCard();

            /// <summary>
            /// 模拟量读取
            /// </summary>
            /// <param name="channelID"></param>
            /// <returns></returns>
            public abstract Decimal AnalogRead(string channelID);

            /// <summary>
            /// 写模拟量
            /// </summary>
            /// <param name="channelID">通道号</param>
            /// <param name="analog">模拟量值</param>
            public abstract void AnalogWrite(string channelID, decimal analog);

            /// <summary>
            /// 读状态量
            /// </summary>
            /// <param name="channelID">通道号</param>
            /// <returns></returns>
            public abstract bool StateRead(string channelID);

            /// <summary>
            /// 写状态量
            /// </summary>
            /// <param name="channelID"></param>
            /// <param name="state"></param>
            public abstract void StateWrite(string channelID, bool state);

            public abstract ChannelParametersList AnalogChannelList { get; }

            public abstract ChannelParametersList StateChannelList { get; }

            public abstract EReadWriteMode ReadWriteMode { get; }

            #endregion IO接口的实现

            #endregion
        }

        /*public static class MotionControlBaseFactory
        {
            private static IMotionControlBase CreateMotionControl(MotionCardType motionCardType)
            {
                switch (motionCardType)
                {
                    case MotionCardType.DMC1000B:
                        return new MotionControlDmcAuto();

                    case MotionCardType.GC800:
                        return new MotionControlGaoChAuto();

                    default:
                        throw new NotImplementedException();
                }
            }
        }*/
    }
}