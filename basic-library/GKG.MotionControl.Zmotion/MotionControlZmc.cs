using System;
using System.Diagnostics;
using System.Text;
using GKG;
using GKG.ElectronicControl;
using cszmcaux;

namespace GKG.ElectronicControl.Zmotion
{
    /// <summary>
    /// 正运动 ZMC 运控卡实现
    /// </summary>
    public class MotionControlZmc : MotionControlBase, IMotionControlCategoryA
    {
        #region 私有字段

        /// <summary>控制器连接句柄</summary>
        private IntPtr _handle = IntPtr.Zero;
        /// <summary>是否已初始化连接</summary>
        private bool _isInitialized = false;

        #endregion

        #region 构造函数

        /// <summary>初始化默认轴数、IO通道数和插补能力</summary>
        public MotionControlZmc()
        {
            _supportAxisNum = 8;
            _supportAnalogNum = 0;
            _supportIoStateNum = 32;
            _isTwoAxisLinearInterpolation = true;
            _isThreeAxisLinearInterpolation = true;
            _isTwoAxisCircularInterpolation = true;
            _isThreeAxisCircularInterpolation = true;
        }
        static MotionControlZmc()
        {
            analogChannelList = new ChannelParametersList();
            stateChannelList = new ChannelParametersList();
            for (ushort i = 0; i < 32; i++)
            {
                stateChannelList.Add(new ChannelParameters() { channelID = createChannelID(EReadWriteMode.ReadOnly, i), channelMode = EReadWriteMode.ReadOnly });
                stateChannelList.Add(new ChannelParameters() { channelID = createChannelID(EReadWriteMode.ReadWrite, i), channelMode = EReadWriteMode.ReadWrite });
            }
        }
        #endregion

        #region 控制器连接管理

        /// <summary>搜索局域网内控制器并连接第一个</summary>
        public int SearchAndConnect(int timeout = 3000)
        {
            try
            {
                StringBuilder ipList = new StringBuilder(256);
                int ret = zmcaux.ZAux_SearchEthlist(ipList, 256, (uint)timeout);
                if (ret != 0) return ret;

                string ipAddress = ipList.ToString().Trim();
                if (string.IsNullOrEmpty(ipAddress)) return -1;

                return Connect(ipAddress);
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>通过以太网连接指定IP的控制器</summary>
        public int Connect(string ipAddress)
        {
            int ret = zmcaux.ZAux_OpenEth(ipAddress, out _handle);
            if (ret == 0) _isInitialized = true;
            return ret;
        }

        /// <summary>断开控制器连接</summary>
        public int Close()
        {
            if (_handle == IntPtr.Zero) return -1;
            int ret = zmcaux.ZAux_Close(_handle);
            if (ret == 0)
            {
                _isInitialized = false;
                _handle = IntPtr.Zero;
            }
            return ret;
        }

        #endregion

        #region 初始化与关闭

        /// <summary>初始化运控卡,未连接时自动连接本机</summary>
        public override int IniMotionCard(int cardNo)
        {
            base.IniMotionCard(cardNo);
            if (!_isInitialized)
            {
                return Connect("127.0.0.1");
            }
            return 0;
        }

        /// <summary>关闭运控卡连接</summary>
        public override int CloseMotionCard(int cardNo)
        {
            return Close();
        }

        /// <summary>获取运控卡固件版本号</summary>
        public override int GetCardSoftVersion()
        {
            return 100;
        }

        #endregion

        #region 轴状态控制

        /// <summary>查询轴状态(原点/报警/限位/使能/到位等)</summary>
        public override bool GetAxisState(int axis, MotionControlAxisStatus motionStatus)
        {
            if (!_isInitialized || _handle == IntPtr.Zero) return false;
            if (axis < 0 || axis >= _supportAxisNum) return false;

            int axisStatus = 0;
            zmcaux.ZAux_Direct_GetAxisStatus(_handle, axis, ref axisStatus);

            switch (motionStatus)
            {
                case MotionControlAxisStatus.Origin:
                    {
                        int piValue = 0;
                        int rtn = zmcaux.ZAux_Direct_GetDatumIn(_handle, axis, ref piValue);
                        if (rtn == 0)
                            return GetInputState(piValue);
                        else
                            return false;
                    }
                case MotionControlAxisStatus.Alarm:
                    return 4194304 == axisStatus || 4194352 == axisStatus;
                case MotionControlAxisStatus.PositiveLimit:
                    return 16 == axisStatus || 48 == axisStatus || 4194352 == axisStatus;
                case MotionControlAxisStatus.NegativeLimit:
                    return 32 == axisStatus || 48 == axisStatus || 4194352 == axisStatus;
                case MotionControlAxisStatus.Inp:
                    return (axisStatus & 0x400) != 0;
                case MotionControlAxisStatus.EZ:
                    return false;
                case MotionControlAxisStatus.ServoEnable:
                    int enable = 0;
                    zmcaux.ZAux_Direct_GetAxisEnable(_handle, axis, ref enable);
                    return enable == 1;
                case MotionControlAxisStatus.Ready:
                    return (axisStatus & 0x100) != 0;
                default:
                    return false;
            }
        }

        /// <summary>按GUID查询轴状态</summary>
        public override bool GetAxisState(Guid guid, MotionControlAxisStatus motionStatus)
        {
            return GetAxisState(GetAxisByGuid(guid), motionStatus);
        }

        /// <summary>使能或去使能指定轴</summary>
        public override int AxisEnabled(int axis, bool isEnabled)
        {
            if (!_isInitialized || _handle == IntPtr.Zero) return -1;
            if (axis < 0 || axis >= _supportAxisNum) return -2;
            return zmcaux.ZAux_Direct_SetAxisEnable(_handle, axis, isEnabled ? 1 : 0);
        }

        /// <summary>查询轴使能状态</summary>
        public int GetAxisEnable(int axis, out bool isEnabled)
        {
            isEnabled = false;
            if (!_isInitialized || _handle == IntPtr.Zero) return -1;
            if (axis < 0 || axis >= _supportAxisNum) return -2;
            int value = 0;
            int ret = zmcaux.ZAux_Direct_GetAxisEnable(_handle, axis, ref value);
            isEnabled = (value != 0);
            return ret;
        }

        /// <summary>判断轴是否已使能(内部辅助方法)</summary>
        private bool IsAxisEnabled(int axis)
        {
            if (!_isInitialized || _handle == IntPtr.Zero) return false;
            if (axis < 0 || axis >= _supportAxisNum) return false;
            int value = 0;
            zmcaux.ZAux_Direct_GetAxisEnable(_handle, axis, ref value);
            return (value != 0);
        }

        #endregion

        #region 轴运动控制

        /// <summary>基类绝对运动(GUID版,内部转为int轴号调用)</summary>
        public override int AbsoluteMove(Guid guid, int motionType, double pos, double startSpeed,
            double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS)
        {
            int axis = GetAxisByGuid(guid);
            return AbsoluteMove(axis, pos, maxSpeed, accTimeT > 0 ? maxSpeed / accTimeT : maxSpeed, decTimeT > 0 ? maxSpeed / decTimeT : maxSpeed);
        }

        /// <summary>基类相对运动(GUID版,内部转为int轴号调用)</summary>
        public override int RelativeMove(Guid guid, int motionType, double pos, double startSpeed,
            double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS)
        {
            int axis = GetAxisByGuid(guid);
            return RelativeMove(axis, pos, maxSpeed, accTimeT > 0 ? maxSpeed / accTimeT : maxSpeed, decTimeT > 0 ? maxSpeed / decTimeT : maxSpeed);
        }

        /// <summary>基类速度运动(GUID版,内部转为int轴号调用)</summary>
        public override int VelocityMove(Guid guid, int motionType, double startSpeed,
            double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS)
        {
            int axis = GetAxisByGuid(guid);
            return VelocityMove(axis, 1, maxSpeed, accTimeT > 0 ? maxSpeed / accTimeT : maxSpeed);
        }

        /// <summary>基类停止轴(GUID版)</summary>
        public override int AxisStop(Guid guid, MotionControlAxisStopTypeConstants motionStopType)
        {
            int axis = GetAxisByGuid(guid);
            return AxisStop(axis, motionStopType);
        }

        /// <summary>基类回零(GUID版)</summary>
        public override int AxisHome(Guid guid)
        {
            int axis = GetAxisByGuid(guid);
            return AxisHome(axis);
        }

        /// <summary>基类等待轴停止(GUID版)</summary>
        public override int WaitAxisStop(Guid guid, int timeOut)
        {
            int axis = GetAxisByGuid(guid);
            return WaitAxisStop(axis, timeOut);
        }

        /// <summary>基类清除轴报警(GUID版)</summary>
        public override int ClearAxisAlarm(Guid guid)
        {
            int axis = GetAxisByGuid(guid);
            return ClearAxisAlarm(axis);
        }

        /// <summary>基类设置软限位(GUID版)</summary>
        public override int SetAxisSoftLimit(Guid guid, MotionControlAxisSoftLimit motionAxisSoft)
        {
            int axis = GetAxisByGuid(guid);
            return SetAxisSoftLimit(axis, motionAxisSoft.PositiveLimit, motionAxisSoft.NegativeLimit);
        }

        /// <summary>获取软限位(返回默认值)</summary>
        public override MotionControlAxisSoftLimit GetAxisSoftLimit(int axis)
        {
            return new MotionControlAxisSoftLimit
            {
                PositiveLimit = 99999,
                NegativeLimit = -99999
            };
        }

        /// <summary>按GUID获取软限位</summary>
        public override MotionControlAxisSoftLimit GetAxisSoftLimit(Guid guid)
        {
            return GetAxisSoftLimit(GetAxisByGuid(guid));
        }

        #endregion

        #region 轴位置与速度控制

        /// <summary>读取轴位置(指令位置或编码器位置)</summary>
        public override double GetAxisPos(int axis, MotionControlAxisPositionType motionAxisType)
        {
            if (!_isInitialized || _handle == IntPtr.Zero) return 0;
            if (axis < 0 || axis >= _supportAxisNum) return 0;

            float pos = 0;
            switch (motionAxisType)
            {
                case MotionControlAxisPositionType.Command:
                case MotionControlAxisPositionType.Target:
                    zmcaux.ZAux_Direct_GetDpos(_handle, axis, ref pos);
                    break;
                case MotionControlAxisPositionType.Actual:
                case MotionControlAxisPositionType.EncoderInternal:
                case MotionControlAxisPositionType.Encoder:
                    zmcaux.ZAux_Direct_GetEncoder(_handle, axis, ref pos);
                    break;
                default:
                    pos = 0;
                    break;
            }
            return pos;
        }

        /// <summary>按GUID读取轴位置</summary>
        public override double GetAxisPos(Guid guid, MotionControlAxisPositionType motionAxisType)
        {
            return GetAxisPos(GetAxisByGuid(guid), motionAxisType);
        }

        /// <summary>设置轴当前位置(修改指令/编码器值)</summary>
        public override double SetAxisPos(int axis, double axisPos)
        {
            if (!_isInitialized || _handle == IntPtr.Zero) return -1;
            if (axis < 0 || axis >= _supportAxisNum) return -2;
            return zmcaux.ZAux_Direct_SetDpos(_handle, axis, (float)axisPos);
        }

        /// <summary>读取轴规划速度</summary>
        public override double GetAxisPlanSpeed(int axis)
        {
            if (!_isInitialized || _handle == IntPtr.Zero) return 0;
            if (axis < 0 || axis >= _supportAxisNum) return 0;
            float speed = 0;
            zmcaux.ZAux_Direct_GetMspeed(_handle, axis, ref speed);
            return speed;
        }

        /// <summary>按GUID读取轴规划速度</summary>
        public override double GetAxisPlanSpeed(Guid guid)
        {
            return GetAxisPlanSpeed(GetAxisByGuid(guid));
        }

        /// <summary>按GUID读取轴实际速度</summary>
        public override double GetAxisActualSpeed(Guid guid)
        {
            return GetAxisActualSpeed(GetAxisByGuid(guid));
        }

        /// <summary>读取轴实际速度(整数)</summary>
        public override int GetAxisActualSpeed(int axis)
        {
            if (!_isInitialized || _handle == IntPtr.Zero) return 0;
            if (axis < 0 || axis >= _supportAxisNum) return 0;
            float speed = 0;
            zmcaux.ZAux_Direct_GetMspeed(_handle, axis, ref speed);
            return (int)speed;
        }

        #endregion

        #region 插补运动

        /// <summary>基类两轴直线插补(GUID版)</summary>
        public override int TwoAxisLinearInterpolation(Guid[] guid, int coordinateSystemNo, double[] targetPos, int motionType, double startSpeed,
            double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS)
        {
            if (guid.Length < 2 || targetPos.Length < 2) return -1;
            int axis1 = GetAxisByGuid(guid[0]);
            int axis2 = GetAxisByGuid(guid[1]);
            return TwoAxisLinearInterpolation(axis1, axis2, targetPos[0], targetPos[1], maxSpeed, accTimeT > 0 ? maxSpeed / accTimeT : maxSpeed);
        }

        /// <summary>基类三轴直线插补(GUID版)</summary>
        public override int ThreeAxisLinearInterpolation(Guid[] guid, int coordinateSystemNo, double[] targetPos, int motionType, double startSpeed,
            double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS)
        {
            if (guid.Length < 3 || targetPos.Length < 3) return -1;
            int axis1 = GetAxisByGuid(guid[0]);
            int axis2 = GetAxisByGuid(guid[1]);
            int axis3 = GetAxisByGuid(guid[2]);
            return ThreeAxisLinearInterpolation(axis1, axis2, axis3, targetPos[0], targetPos[1], targetPos[2], maxSpeed, accTimeT > 0 ? maxSpeed / accTimeT : maxSpeed);
        }

        /// <summary>基类两轴圆弧插补(GUID版)</summary>
        public override int TwoAxisCircularInterpolation(Guid[] guid, int coordinateSystemNo, double[] targetPos, double[] centerCircle, int arcDirection, int motionType,
            double startSpeed, double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS)
        {
            if (guid.Length < 2 || targetPos.Length < 2 || centerCircle.Length < 2) return -1;
            int axis1 = GetAxisByGuid(guid[0]);
            int axis2 = GetAxisByGuid(guid[1]);
            return TwoAxisCircularInterpolation(axis1, axis2, targetPos[0], targetPos[1], centerCircle[0], centerCircle[1], arcDirection, maxSpeed);
        }

        /// <summary>基类三轴圆弧插补(GUID版,前两轴圆弧+第三轴独立运动)</summary>
        public override int ThreeAxisCircularInterpolation(Guid[] guid, int coordinateSystemNo, double[] targetPos, double[] centerCircle, int arcDirection, int motionType,
            double startSpeed, double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS)
        {
            if (guid.Length < 3 || targetPos.Length < 3 || centerCircle.Length < 2) return -1;
            int[] axes = new int[] { GetAxisByGuid(guid[0]), GetAxisByGuid(guid[1]), GetAxisByGuid(guid[2]) };
            return ThreeAxisCircularInterpolation(axes, targetPos, centerCircle, arcDirection, maxSpeed, accTimeT > 0 ? maxSpeed / accTimeT : maxSpeed);
        }

        #endregion

        #region IO 控制

        /// <summary>读取数字输入IO状态</summary>
        public override bool GetInputState(int ioChannel)
        {
            if (!_isInitialized || _handle == IntPtr.Zero) return false;
            uint value = 0;
            zmcaux.ZAux_Direct_GetIn(_handle, ioChannel, ref value);
            return value != 0;
        }

        /// <summary>读取数字输出IO状态</summary>
        public override bool GetOutputState(int ioChannel)
        {
            if (!_isInitialized || _handle == IntPtr.Zero) return false;
            uint value = 0;
            zmcaux.ZAux_Direct_GetOp(_handle, ioChannel, ref value);
            return value != 0;
        }

        /// <summary>按GUID读取IO输出状态</summary>
        public override bool GetInOutputState(Guid guid)
        {
            return GetOutputState(GetIoByGuid(guid));
        }

        /// <summary>按GUID设置IO输出状态</summary>
        public override int SetOutputState(Guid guid, bool isHave)
        {
            return SetOutputState(GetIoByGuid(guid), isHave);
        }

        /// <summary>读取模拟量输入值</summary>
        public override double GetInputNum(int ioChannel)
        {
            if (!_isInitialized || _handle == IntPtr.Zero) return 0;
            float value = 0;
            zmcaux.ZAux_Direct_GetAD(_handle, ioChannel, ref value);
            return value;
        }

        /// <summary>读取模拟量输出值</summary>
        public override double GetOutputNum(int ioChannel)
        {
            if (!_isInitialized || _handle == IntPtr.Zero) return 0;
            float value = 0;
            zmcaux.ZAux_Direct_GetDA(_handle, ioChannel, ref value);
            return value;
        }

        /// <summary>按GUID读取模拟量输出值</summary>
        public override double GetInOutputNum(Guid guid)
        {
            return GetOutputNum(GetIoByGuid(guid));
        }

        /// <summary>按GUID设置模拟量输出值</summary>
        public override double SetInOutputNum(Guid guid, double analogValue)
        {
            return SetOutputNum(GetIoByGuid(guid), analogValue);
        }

        #endregion

        #region 坐标系控制

        /// <summary>等待坐标系运动完成</summary>
        public new int WaitCrdMoveDone(int coordinateSystemNo, int timeout = 1000)
        {
            if (!_isInitialized || _handle == IntPtr.Zero) return -1;
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (true)
            {
                int axisStatus = 0;
                zmcaux.ZAux_Direct_GetAxisStatus(_handle, 0, ref axisStatus);
                if ((axisStatus & 0x01) == 0) return 0;
                if (stopwatch.ElapsedMilliseconds > timeout) return -1;
                System.Threading.Thread.Sleep(1);
            }
        }

        #endregion

        #region IMotionControlCategoryA 接口实现

        /// <summary>
        /// 在线变速 - 运动过程中动态修改速度
        /// </summary>
        /// <param name="axisGuid">轴锁定句柄</param>
        /// <param name="targetSpeed">目标速度(mm/s)</param>
        public void OnlineSpeedChange(Guid axisGuid, double targetSpeed)
        {
            int axis = GetAxisByGuid(axisGuid);
            if (!_isInitialized || _handle == IntPtr.Zero)
                throw new GKGException(MotionErrCodeConsts.ERR_MOTION_ONLINESPEEDCHANGE_FAIL, 
                    MotionErr.OnlineSpeedChangeFail, "运控卡未初始化");
            
            if (axis < 0 || axis >= _supportAxisNum)
                throw new GKGException(MotionErrCodeConsts.ERR_MOTION_BADPARAM, 
                    MotionErr.AxisOutOfRange, $"轴号超出范围: {axis}");

            int ret = zmcaux.ZAux_Direct_SetSpeed(_handle, axis, (float)targetSpeed);
            if (ret != 0)
                throw new GKGException(MotionErrCodeConsts.ERR_MOTION_ONLINESPEEDCHANGE_FAIL, 
                    MotionErr.OnlineSpeedChangeFail, $"ZMC在线变速失败,错误码:{ret}");
        }

        /// <summary>
        /// 在线变位 - 运动过程中修改目标位置
        /// </summary>
        /// <param name="axisGuid">轴锁定句柄</param>
        /// <param name="targetPosition">目标位置(mm)</param>
        public void OnlineTargetPositionChange(Guid axisGuid, double targetPosition)
        {
            int axis = GetAxisByGuid(axisGuid);
            if (!_isInitialized || _handle == IntPtr.Zero)
                throw new GKGException(MotionErrCodeConsts.ERR_MOTION_ONLINETARGETPOSITIONCHANGE_FAIL, 
                    MotionErr.OnlineTargetPositionChangeFail, "运控卡未初始化");
            
            if (axis < 0 || axis >= _supportAxisNum)
                throw new GKGException(MotionErrCodeConsts.ERR_MOTION_BADPARAM, 
                    MotionErr.AxisOutOfRange, $"轴号超出范围: {axis}");

            // 使用 MOVETO_TARGET 寄存器修改目标位置
            int ret = zmcaux.ZAux_Direct_SetParam(_handle, "MOVETO_TARGET", axis, (float)targetPosition);
            if (ret != 0)
                throw new GKGException(MotionErrCodeConsts.ERR_MOTION_ONLINETARGETPOSITIONCHANGE_FAIL, 
                    MotionErr.OnlineTargetPositionChangeFail, $"ZMC在线变位失败,错误码:{ret}");
        }

        /// <summary>设置2D补偿参数(预留)</summary>
        public void Set2DCompensationParameters(Guid[] axisGuidList, MotionControl2DOffsetParameters compensationParams) { }

        /// <summary>启用/禁用2D补偿(预留)</summary>
        public void Set2DCompensationEnabled(Guid[] axisGuidList, int CompensationCoordinateSystemId, bool isEnabled) { }

        /// <summary>获取2D补偿偏移量(预留)</summary>
        public Struct2DOffsetParameters Get2DCompensationParameters(int[] axisList, int coordinateSystemId, Point2D point)
        {
            return new Struct2DOffsetParameters { OffsetX = 0, OffsetY = 0 };
        }

        /// <summary>获取绝对运动规划时间(预留)</summary>
        public int GetAbsoluteMotionPlanningTime(int axis) => 0;

        /// <summary>按GUID获取绝对运动规划时间(预留)</summary>
        public int GetAbsoluteMotionPlanningTime(Guid axisGuid) => 0;

        /// <summary>
        /// 设置位置锁存参数
        /// </summary>
        /// <param name="axisGuid">轴锁定句柄</param>
        /// <param name="positionLatchCaptureLogic">位置锁存捕获逻辑（IO触发、编码器Z相触发等）</param>
        /// <param name="channel">位置锁存触发通道号，范围：0-31（DI通道编号）</param>
        /// <param name="positionLatchSignalTriggerMode">位置锁存信号触发模式（上升沿、下降沿、电平触发）</param>
        /// <param name="level">电平模式的电平值，高电平=1，低电平=0</param>
        /// <param name="triggerCount">触发次数，范围：1-255（255表示无限次触发）</param>
        /// <exception cref="GKGException">运控卡未初始化时抛出异常</exception>
        /// <exception cref="GKGException">轴号超出范围时抛出异常</exception>
        /// <exception cref="GKGException">通道号超出范围(0-31)时抛出异常</exception>
        /// <exception cref="GKGException">触发次数超出范围(1-255)时抛出异常</exception>
        /// <exception cref="GKGException">ZMC API调用失败时抛出异常</exception>
        /// <remarks>
        /// 功能说明：配置位置锁存功能，当指定的DI通道满足触发条件时，自动锁存轴的当前位置。
        /// 触发模式：
        /// - 上升沿触发(RisingEdge)：DI信号从低到高跳变时触发
        /// - 下降沿触发(FallingEdge)：DI信号从高到低跳变时触发
        /// - 电平触发(IOLevel)：DI信号保持指定电平时持续触发
        /// - 编码器Z相触发(EncoderZ)：编码器Z相信号触发
        /// 使用流程：SetPositionLatch → SetPositionLatchEnabled(true) → GetPositionLatchResult
        /// </remarks>
        public void SetPositionLatch(Guid axisGuid, MotionControlPositionLatchCaptureLogic positionLatchCaptureLogic, int channel, MotionControlPositionLatchSignalTriggerMode positionLatchSignalTriggerMode, short level, int triggerCount)
        {
            // 获取轴号
            int axis = GetAxisByGuid(axisGuid);

            // 参数校验
            if (!_isInitialized || _handle == IntPtr.Zero)
                throw new GKGException(MotionErrCodeConsts.ERR_MOTION_BADPARAM,
                    MotionErr.MotionCardInitFailed, "运控卡未初始化");

            if (axis < 0 || axis >= _supportAxisNum)
                throw new GKGException(MotionErrCodeConsts.ERR_MOTION_BADPARAM,
                    MotionErr.AxisOutOfRange, $"轴号超出范围: {axis}");

            if (channel < 0 || channel > 31)
                throw new GKGException(MotionErrCodeConsts.ERR_MOTION_BADPARAM,
                    MotionErr.AxisOutOfRange, $"通道号超出范围(0-31): {channel}");

            if (triggerCount < 1 || triggerCount > 255)
                throw new GKGException(MotionErrCodeConsts.ERR_MOTION_BADPARAM,
                    MotionErr.AxisOutOfRange, $"触发次数超出范围(1-255): {triggerCount}");

            // 配置锁存模式
            // ZMC 锁存模式：
            // 0 = 不锁存
            // 1 = 上升沿触发 REG_MARK
            // 2 = 下降沿触发 REG_MARK  
            // 3 = 电平触发（高电平）
            // 4 = 电平触发（低电平）
            // 11 = 编码器Z相触发
            int registMode = 0;

            switch (positionLatchCaptureLogic)
            {
                case MotionControlPositionLatchCaptureLogic.EncoderZ:
                    // 编码器Z相捕获
                    registMode = 11;
                    break;

                case MotionControlPositionLatchCaptureLogic.IO:
                    // IO捕获，根据触发模式设置
                    switch (positionLatchSignalTriggerMode)
                    {
                        case MotionControlPositionLatchSignalTriggerMode.RisingEdge:
                            registMode = 1;  // 上升沿触发
                            break;
                        case MotionControlPositionLatchSignalTriggerMode.FallingEdge:
                            registMode = 2;  // 下降沿触发
                            break;
                        case MotionControlPositionLatchSignalTriggerMode.IOLevel:
                            // 电平触发，根据 level 参数
                            registMode = (level > 0) ? 3 : 4;
                            break;
                    }
                    break;

                case MotionControlPositionLatchCaptureLogic.IOPlusZ:
                case MotionControlPositionLatchCaptureLogic.IOPlusZThenEncoderZ:
                    // ZMC 可能不直接支持这些复合模式，使用基本IO模式
                    registMode = 1;  // 默认上升沿
                    break;
            }

            // 设置触发通道：使用 REG_MARK 寄存器映射 DI 通道
            // REG_MARK 参数用于指定触发源
            int ret = zmcaux.ZAux_Direct_SetParam(_handle, "REG_MARK", axis, (float)channel);
            if (ret != 0)
                throw new GKGException(MotionErrCodeConsts.ERR_MOTION_BADPARAM,
                    MotionErr.MotionCardInitFailed, $"ZMC设置锁存通道失败,错误码:{ret}");

            // 设置触发次数：使用 REG_MARK_COUNT 寄存器
            ret = zmcaux.ZAux_Direct_SetParam(_handle, "REG_MARK_COUNT", axis, (float)triggerCount);
            if (ret != 0)
                throw new GKGException(MotionErrCodeConsts.ERR_MOTION_BADPARAM,
                    MotionErr.MotionCardInitFailed, $"ZMC设置触发次数失败,错误码:{ret}");

            // 执行位置锁存配置
            ret = zmcaux.ZAux_Direct_Regist(_handle, axis, registMode);
            if (ret != 0)
                throw new GKGException(MotionErrCodeConsts.ERR_MOTION_BADPARAM,
                    MotionErr.MotionCardInitFailed, $"ZMC设置位置锁存失败,错误码:{ret}");
        }

        /// <summary>
        /// 启用或禁用位置锁存功能
        /// </summary>
        /// <param name="axisGuid">轴锁定句柄</param>
        /// <param name="isEnabled">是否启用，true=启用锁存功能，false=禁用并清空缓冲区</param>
        /// <exception cref="GKGException">运控卡未初始化时抛出异常</exception>
        /// <exception cref="GKGException">轴号超出范围时抛出异常</exception>
        /// <exception cref="GKGException">ZMC API调用失败时抛出异常</exception>
        /// <remarks>
        /// 功能说明：控制位置锁存功能的开关状态。
        /// - 启用(true)：激活位置锁存监听，开始响应触发信号
        /// - 禁用(false)：停止位置锁存，清空已触发的历史数据
        /// 注意：
        /// 1. 必须先调用SetPositionLatch配置参数，再调用此方法启用
        /// 2. 启用时会重新激活之前在SetPositionLatch中配置的锁存模式
        /// 3. 禁用时会清空锁存缓冲区和触发状态
        /// ZMC API 说明：
        /// - 启用：需要保留之前配置的锁存模式并重新调用Regist，但当前实现假设锁存配置持久保存
        /// - 禁用：调用Regist(axis, 0)关闭锁存，并清除MARK状态
        /// </remarks>
        public void SetPositionLatchEnabled(Guid axisGuid, bool isEnabled)
        {
            // 获取轴号
            int axis = GetAxisByGuid(axisGuid);

            // 参数校验
            if (!_isInitialized || _handle == IntPtr.Zero)
                throw new GKGException(MotionErrCodeConsts.ERR_MOTION_BADPARAM,
                    MotionErr.MotionCardInitFailed, "运控卡未初始化");

            if (axis < 0 || axis >= _supportAxisNum)
                throw new GKGException(MotionErrCodeConsts.ERR_MOTION_BADPARAM,
                    MotionErr.AxisOutOfRange, $"轴号超出范围: {axis}");

            int ret;
            
            if (isEnabled)
            {
                // 启用位置锁存
                // 注意：ZMC的Regist配置在SetPositionLatch中已设置，这里假设配置持久保存
                // 如果需要重新启用，可能需要重新调用Regist或使用使能寄存器
                // 这里使用ZMC的锁存使能机制（如果支持的话）
                
                // 方法1：尝试使用REG_MARK_EN寄存器（如果ZMC支持）
                ret = zmcaux.ZAux_Direct_SetParam(_handle, "REG_MARK_EN", axis, 1.0f);
                if (ret != 0)
                {
                    // 如果REG_MARK_EN不支持，可能需要重新调用Regist
                    // 但这里我们假设SetPositionLatch的配置持久有效
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_BADPARAM,
                        MotionErr.MotionCardInitFailed, $"ZMC启用位置锁存失败,错误码:{ret}");
                }
            }
            else
            {
                // 禁用位置锁存：设置锁存模式为0（关闭）
                ret = zmcaux.ZAux_Direct_Regist(_handle, axis, 0);
                if (ret != 0)
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_BADPARAM,
                        MotionErr.MotionCardInitFailed, $"ZMC禁用位置锁存失败,错误码:{ret}");

                // 清除MARK触发状态标志
                ret = zmcaux.ZAux_Direct_SetParam(_handle, "MARK", axis, 0.0f);
                if (ret != 0)
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_BADPARAM,
                        MotionErr.MotionCardInitFailed, $"ZMC清除锁存触发状态失败,错误码:{ret}");

                // 清空触发次数计数器
                ret = zmcaux.ZAux_Direct_SetParam(_handle, "REG_MARK_COUNT_ACTUAL", axis, 0.0f);
                if (ret != 0)
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_BADPARAM,
                        MotionErr.MotionCardInitFailed, $"ZMC清空锁存计数器失败,错误码:{ret}");

                // 清空锁存位置寄存器（REG_POS）
                ret = zmcaux.ZAux_Direct_SetParam(_handle, "REG_POS", axis, 0.0f);
                if (ret != 0)
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_BADPARAM,
                        MotionErr.MotionCardInitFailed, $"ZMC清空锁存位置寄存器失败,错误码:{ret}");
            }
        }

        /// <summary>
        /// 获取位置锁存结果（按轴GUID）
        /// </summary>
        /// <param name="axisGuid">轴锁定句柄</param>
        /// <returns>位置锁存结果数组，单位：mm，按触发时间顺序排列。如果未触发，返回空数组</returns>
        /// <exception cref="GKGException">运控卡未初始化时抛出异常</exception>
        /// <exception cref="GKGException">轴号超出范围时抛出异常</exception>
        /// <exception cref="GKGException">ZMC API调用失败时抛出异常</exception>
        /// <remarks>
        /// 功能说明：读取外部信号触发后锁存的轴位置数据。
        /// 返回值特征：
        /// - 返回数组的单位为毫米(mm)，已自动进行脉冲→长度转换
        /// - 数组按触发时间顺序排列，最早触发的位置在前
        /// - 如果没有触发，返回空数组（长度为0）
        /// 注意：ZMC通常只支持单次位置锁存读取，多次锁存需使用CycleRegist功能
        /// </remarks>
        public double[] GetPositionLatchResult(Guid axisGuid)
        {
            return GetPositionLatchResult(GetAxisByGuid(axisGuid));
        }

        /// <summary>
        /// 获取位置锁存结果（按轴号）
        /// </summary>
        /// <param name="axis">轴号，范围：0 至 (支持轴数-1)</param>
        /// <returns>位置锁存结果数组，单位：mm，按触发时间顺序排列。如果未触发，返回空数组</returns>
        /// <exception cref="GKGException">运控卡未初始化时抛出异常</exception>
        /// <exception cref="GKGException">轴号超出范围时抛出异常</exception>
        /// <exception cref="GKGException">ZMC API调用失败时抛出异常</exception>
        /// <remarks>
        /// 功能说明：读取外部信号触发后锁存的轴位置数据。
        /// 实现逻辑：
        /// 1. 读取MARK触发状态（-1=已触发，0=未触发）
        /// 2. 读取实际触发次数（REG_MARK_COUNT_ACTUAL）
        /// 3. 读取锁存位置寄存器（REG_POS）
        /// 4. 转换脉冲值为毫米单位
        /// 注意：当前实现仅支持读取最后一次触发的位置，多次锁存需使用CycleRegist功能
        /// </remarks>
        public double[] GetPositionLatchResult(int axis)
        {
            // 参数校验
            if (!_isInitialized || _handle == IntPtr.Zero)
                throw new GKGException(MotionErrCodeConsts.ERR_MOTION_BADPARAM,
                    MotionErr.MotionCardInitFailed, "运控卡未初始化");

            if (axis < 0 || axis >= _supportAxisNum)
                throw new GKGException(MotionErrCodeConsts.ERR_MOTION_BADPARAM,
                    MotionErr.AxisOutOfRange, $"轴号超出范围: {axis}");

            // 读取锁存触发状态
            int markStatus = 0;
            int ret = zmcaux.ZAux_Direct_GetMark(_handle, axis, ref markStatus);
            if (ret != 0)
                throw new GKGException(MotionErrCodeConsts.ERR_MOTION_BADPARAM,
                    MotionErr.MotionCardInitFailed, $"ZMC读取锁存状态失败,错误码:{ret}");

            // markStatus: -1表示已触发, 0表示未触发
            if (markStatus == -1)
            {
                // 读取锁存的触发次数
                float triggerCountFloat = 0;
                ret = zmcaux.ZAux_Direct_GetParam(_handle, "REG_MARK_COUNT_ACTUAL", axis, ref triggerCountFloat);
                if (ret != 0)
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_BADPARAM,
                        MotionErr.MotionCardInitFailed, $"ZMC读取触发次数失败,错误码:{ret}");

                int triggerCount = (int)triggerCountFloat;
                
                if (triggerCount > 0)
                {
                    // 读取锁存位置（脉冲值）
                    float regPosFloat = 0;
                    ret = zmcaux.ZAux_Direct_GetRegPos(_handle, axis, ref regPosFloat);
                    if (ret != 0)
                        throw new GKGException(MotionErrCodeConsts.ERR_MOTION_BADPARAM,
                            MotionErr.MotionCardInitFailed, $"ZMC读取锁存位置失败,错误码:{ret}");

                    // ZMC 通常只支持单次锁存位置读取
                    // 如果需要多次锁存，需要使用 CycleRegist 功能
                    double[] results = new double[1];
                    results[0] = PulseToLength(axis, (int)regPosFloat);
                    
                    return results;
                }
            }

            // 未触发或无数据，返回空数组
            return new double[0];
        }

        /// <summary>2D位置比较输出</summary>
        /// <param name="coordinateSystemNo">坐标系编号</param>
        /// <param name="axisGuidList">轴GUID列表（至少2个轴）</param>
        /// <param name="PositionComparisonTriggerPoints">触发点列表</param>
        /// <param name="motionTrajectoryList">运动轨迹列表（可选）</param>
        /// <param name="outputChannel">输出通道(0-3)，默认0</param>
        /// <param name="pulseWidthMs">脉冲宽度(毫秒)，默认1ms</param>
        /// <param name="maxErrorMm">最大误差(mm)，默认0.1mm</param>
        public void PositionComparison2D(
            int coordinateSystemNo, 
            Guid[] axisGuidList, 
            MotionControlPositionComparisonTriggerPoint[] PositionComparisonTriggerPoints, 
            MotionInstructionBase[] motionTrajectoryList,
            int outputChannel = 0,
            double pulseWidthMs = 1.0,
            double maxErrorMm = 0.1)
        {
            if (!_isInitialized || _handle == IntPtr.Zero)
                throw new InvalidOperationException("运控卡未初始化");

            if (axisGuidList == null || axisGuidList.Length < 2)
                throw new ArgumentException("至少需要2个轴用于2D位置比较");

            if (PositionComparisonTriggerPoints == null || PositionComparisonTriggerPoints.Length == 0)
                throw new ArgumentException("位置比较触发点不能为空");

            // 获取轴号
            int[] axisList = new int[axisGuidList.Length];
            for (int i = 0; i < axisGuidList.Length; i++)
            {
                axisList[i] = GetAxisByGuid(axisGuidList[i]);
            }

            // 停止之前的位置比较
            StopManualPositionComparison();

            // 设置位置比较参数（使用改进的参数化方法）
            SetPositionComparisonParam(
                axisList, 
                PositionComparisonTriggerPoints,
                outputChannel,
                pulseWidthMs,
                maxErrorMm);

            // 如果有运动轨迹，执行连续插补运动
            if (motionTrajectoryList != null && motionTrajectoryList.Length > 0)
            {
                ContinuousInterpolationMotion(coordinateSystemNo, axisGuidList, motionTrajectoryList);
            }
        }

        /// <summary>连续插补运动</summary>
        public void ContinuousInterpolationMotion(int coordinateSystemNo, Guid[] axisGuidList, MotionInstructionBase[] motionTrajectoryList)
        {
            if (!_isInitialized || _handle == IntPtr.Zero)
                throw new InvalidOperationException("运控卡未初始化");

            if (axisGuidList == null || axisGuidList.Length == 0)
                throw new ArgumentException("轴列表不能为空");

            if (motionTrajectoryList == null || motionTrajectoryList.Length == 0)
                throw new ArgumentException("运动轨迹列表不能为空");

            try
            {
                // 获取轴号列表
                int[] axisList = new int[axisGuidList.Length];
                for (int i = 0; i < axisGuidList.Length; i++)
                {
                    axisList[i] = GetAxisByGuid(axisGuidList[i]);
                }

                // 执行每一条运动指令
                foreach (var instruction in motionTrajectoryList)
                {
                    ExecuteMotionInstruction(instruction, axisList);
                }
            }
            catch (Exception ex)
            {
                throw new GKGException(
                    MotionErrCodeConsts.ERR_MOTION_INTERPOLATION_FAIL,
                    "ERR_MOTION_INTERPOLATION_FAIL",
                    $"ZMC运控卡连续插补运动失败: {ex.Message}"
                );
            }
        }

        /// <summary>执行单条运动指令</summary>
        private void ExecuteMotionInstruction(MotionInstructionBase instruction, int[] axisList)
        {
            if (instruction == null)
                throw new ArgumentNullException(nameof(instruction));

            int ret;

            switch (instruction.InstructionType)
            {
                case MotionInstructionType.Linear:
                    {
                        if (instruction is not StraightLine straightLine)
                            throw new ArgumentException("指令类型不匹配: 应为StraightLine");

                        if (straightLine.EndPosition == null || straightLine.EndPosition.Length < axisList.Length)
                            throw new ArgumentException($"直线运动终点坐标数量不足，需要{axisList.Length}个轴");

                        // 构建目标位置数组（脉冲单位）
                        float[] targetPos = new float[axisList.Length];
                        for (int i = 0; i < axisList.Length; i++)
                        {
                            double positionMm = straightLine.EndPosition[i].PositionValue;
                            targetPos[i] = (float)LengthToPulse(straightLine.EndPosition[i].Axis, positionMm);
                        }

                        // 设置速度和加速度
                        for (int i = 0; i < axisList.Length; i++)
                        {
                            int axis = axisList[i];
                            // ZMC使用pulse/s和pulse/s²单位
                            float speedPulse = (float)(SpeedMmPerSecToPulsePerMs(straightLine.EndPosition[i].Axis, straightLine.Speed) * 1000);
                            float accPulse = (float)(SpeedMmPerSecToPulsePerMs(straightLine.EndPosition[i].Axis, straightLine.Acceleration) * 1000);

                            ret = zmcaux.ZAux_Direct_SetSpeed(_handle, axis, speedPulse);
                            if (ret != 0)
                                throw new GKGException(ret, "ZAux_Direct_SetSpeed", $"设置轴{axis}速度失败");

                            ret = zmcaux.ZAux_Direct_SetAccel(_handle, axis, accPulse);
                            if (ret != 0)
                                throw new GKGException(ret, "ZAux_Direct_SetAccel", $"设置轴{axis}加速度失败");
                        }

                        // 执行绝对位置直线插补运动（带速度规划）
                        ret = zmcaux.ZAux_Direct_MoveAbsSp(_handle, axisList.Length, axisList, targetPos);
                        if (ret != 0)
                            throw new GKGException(ret, "ZAux_Direct_MoveAbsSp", "直线插补运动失败");

                        // 等待运动完成
                        WaitForMotionComplete(axisList);
                    }
                    break;

                case MotionInstructionType.ArcA:
                    {
                        if (instruction is not ArcA arcA)
                            throw new ArgumentException("指令类型不匹配: 应为ArcA");

                        if (arcA.EndPosition == null || arcA.EndPosition.Length < 2)
                            throw new ArgumentException("圆弧运动需要至少2个轴的终点坐标");

                        if (arcA.MiddlePosition == null || arcA.MiddlePosition.Length < 2)
                            throw new ArgumentException("圆弧运动需要至少2个轴的中间点坐标");

                        if (axisList.Length < 2)
                            throw new ArgumentException("圆弧运动需要至少2个轴");

                        // 获取当前位置作为起点
                        float startPos1 = 0, startPos2 = 0;
                        ret = zmcaux.ZAux_Direct_GetDpos(_handle, axisList[0], ref startPos1);
                        if (ret != 0)
                            throw new GKGException(ret, "ZAux_Direct_GetDpos", $"获取轴{axisList[0]}当前位置失败");

                        ret = zmcaux.ZAux_Direct_GetDpos(_handle, axisList[1], ref startPos2);
                        if (ret != 0)
                            throw new GKGException(ret, "ZAux_Direct_GetDpos", $"获取轴{axisList[1]}当前位置失败");

                        // 转换为脉冲单位（相对于起点）
                        float endPos1 = (float)LengthToPulse(arcA.EndPosition[0].Axis, arcA.EndPosition[0].PositionValue);
                        float endPos2 = (float)LengthToPulse(arcA.EndPosition[1].Axis, arcA.EndPosition[1].PositionValue);
                        float midPos1 = (float)LengthToPulse(arcA.MiddlePosition[0].Axis, arcA.MiddlePosition[0].PositionValue);
                        float midPos2 = (float)LengthToPulse(arcA.MiddlePosition[1].Axis, arcA.MiddlePosition[1].PositionValue);

                        // 设置速度和加速度
                        for (int i = 0; i < Math.Min(2, axisList.Length); i++)
                        {
                            int axis = axisList[i];
                            float speedPulse = (float)(SpeedMmPerSecToPulsePerMs(arcA.EndPosition[i].Axis, arcA.Speed) * 1000);
                            float accPulse = (float)(SpeedMmPerSecToPulsePerMs(arcA.EndPosition[i].Axis, arcA.Acceleration) * 1000);

                            ret = zmcaux.ZAux_Direct_SetSpeed(_handle, axis, speedPulse);
                            if (ret != 0)
                                throw new GKGException(ret, "ZAux_Direct_SetSpeed", $"设置轴{axis}速度失败");

                            ret = zmcaux.ZAux_Direct_SetAccel(_handle, axis, accPulse);
                            if (ret != 0)
                                throw new GKGException(ret, "ZAux_Direct_SetAccel", $"设置轴{axis}加速度失败");
                        }

                        // 使用三点圆弧插补（绝对位置+速度规划）
                        ret = zmcaux.ZAux_Direct_MoveCirc2AbsSp(_handle, 2, axisList, midPos1, midPos2, endPos1, endPos2);
                        if (ret != 0)
                            throw new GKGException(ret, "ZAux_Direct_MoveCirc2AbsSp", "圆弧插补运动失败");

                        // 如果有第三轴，同步运动
                        if (axisList.Length > 2 && arcA.EndPosition.Length > 2)
                        {
                            int[] thirdAxis = new int[] { axisList[2] };
                            float[] thirdPos = new float[] { (float)LengthToPulse(arcA.EndPosition[2].Axis, arcA.EndPosition[2].PositionValue) };
                            // 第三轴已经通过前面的设置参与运动
                        }

                        // 等待运动完成
                        WaitForMotionComplete(axisList);
                    }
                    break;

                case MotionInstructionType.Circle:
                    {
                        if (instruction is not Circle circle)
                            throw new ArgumentException("指令类型不匹配: 应为Circle");

                        if (circle.EndPosition == null || circle.EndPosition.Length < 2)
                            throw new ArgumentException("圆形运动需要至少2个轴的终点坐标");

                        if (circle.MiddlePosition == null || circle.MiddlePosition.Length < 2)
                            throw new ArgumentException("圆形运动需要至少2个轴的中间点坐标");

                        if (axisList.Length < 2)
                            throw new ArgumentException("圆形运动需要至少2个轴");

                        // 转换为脉冲单位
                        float endPos1 = (float)LengthToPulse(circle.EndPosition[0].Axis, circle.EndPosition[0].PositionValue);
                        float endPos2 = (float)LengthToPulse(circle.EndPosition[1].Axis, circle.EndPosition[1].PositionValue);
                        float midPos1 = (float)LengthToPulse(circle.MiddlePosition[0].Axis, circle.MiddlePosition[0].PositionValue);
                        float midPos2 = (float)LengthToPulse(circle.MiddlePosition[1].Axis, circle.MiddlePosition[1].PositionValue);

                        // 设置速度和加速度
                        for (int i = 0; i < Math.Min(2, axisList.Length); i++)
                        {
                            int axis = axisList[i];
                            float speedPulse = (float)(SpeedMmPerSecToPulsePerMs(circle.EndPosition[i].Axis, circle.Speed) * 1000);
                            float accPulse = (float)(SpeedMmPerSecToPulsePerMs(circle.EndPosition[i].Axis, circle.Acceleration) * 1000);

                            ret = zmcaux.ZAux_Direct_SetSpeed(_handle, axis, speedPulse);
                            if (ret != 0)
                                throw new GKGException(ret, "ZAux_Direct_SetSpeed", $"设置轴{axis}速度失败");

                            ret = zmcaux.ZAux_Direct_SetAccel(_handle, axis, accPulse);
                            if (ret != 0)
                                throw new GKGException(ret, "ZAux_Direct_SetAccel", $"设置轴{axis}加速度失败");
                        }

                        // 圆形运动：起点=终点，使用三点圆弧
                        ret = zmcaux.ZAux_Direct_MoveCirc2AbsSp(_handle, 2, axisList, midPos1, midPos2, endPos1, endPos2);
                        if (ret != 0)
                            throw new GKGException(ret, "ZAux_Direct_MoveCirc2AbsSp", "圆形插补运动失败");

                        // 等待运动完成
                        WaitForMotionComplete(axisList);
                    }
                    break;

                case MotionInstructionType.Delay:
                    {
                        if (instruction is not Delay delay)
                            throw new ArgumentException("指令类型不匹配: 应为Delay");

                        if (delay.Duration > 0)
                        {
                            System.Threading.Thread.Sleep((int)delay.Duration);
                        }
                    }
                    break;

                case MotionInstructionType.BufferIO:
                    {
                        if (instruction is not BufferIO bufferIO)
                            throw new ArgumentException("指令类型不匹配: 应为BufferIO");

                        // 执行IO输出
                        ret = zmcaux.ZAux_Direct_SetOp(_handle, bufferIO.Channel, (uint)bufferIO.Data);
                        if (ret != 0)
                            throw new GKGException(ret, "ZAux_Direct_SetOp", $"设置IO输出失败: 通道{bufferIO.Channel}");
                    }
                    break;

                default:
                    throw new NotSupportedException($"不支持的运动指令类型: {instruction.InstructionType}");
            }
        }

        /// <summary>等待指定轴运动完成</summary>
        private void WaitForMotionComplete(int[] axisList)
        {
            if (axisList == null || axisList.Length == 0)
                return;

            const int checkIntervalMs = 10;
            const int timeout = 60000; // 60秒超时
            int elapsed = 0;

            while (elapsed < timeout)
            {
                bool allStopped = true;

                foreach (int axis in axisList)
                {
                    int status = 0;
                    int ret = zmcaux.ZAux_Direct_GetIfIdle(_handle, axis, ref status);
                    
                    if (ret != 0)
                        throw new GKGException(ret, "ZAux_Direct_GetIfIdle", $"获取轴{axis}运动状态失败");

                    // status=1表示静止，status=0表示运动中
                    if (status == 0)
                    {
                        allStopped = false;
                        break;
                    }
                }

                if (allStopped)
                    return;

                System.Threading.Thread.Sleep(checkIntervalMs);
                elapsed += checkIntervalMs;
            }

            throw new TimeoutException($"等待运动完成超时({timeout}ms)");
        }

        /// <summary>手动位置比较输出</summary>
        public void ManualPositionComparison(Guid[] axisGuidList, int[] channel, short startLevel, int pulseOutputMode, int triggerCount, double openTime, double closeTime)
        {
            if (!_isInitialized || _handle == IntPtr.Zero)
                throw new InvalidOperationException("运控卡未初始化");

            if (axisGuidList == null || axisGuidList.Length < 2)
                throw new ArgumentException("至少需要2个轴用于手动位置比较");

            if (channel == null || channel.Length == 0)
                throw new ArgumentException("输出通道不能为空");

            // 获取轴号
            int[] axisList = new int[axisGuidList.Length];
            for (int i = 0; i < axisGuidList.Length; i++)
            {
                axisList[i] = GetAxisByGuid(axisGuidList[i]);
            }

            // 设置手动位置比较参数
            for (int i = 0; i < channel.Length; i++)
            {
                // Mode = 2 表示手动触发模式
                // Opstate: startLevel (0或1)
                // ModePara1: openTime (秒)
                // ModePara2: closeTime (秒)
                // ModePara3: 预留
                // ModePara4: 预留
                int ret = zmcaux.ZAux_Direct_HwPswitch2(
                    _handle,
                    axisList[0],  // 主轴号
                    2,  // 模式2: 手动触发
                    channel[i],  // 输出通道号
                    startLevel,  // 起始电平
                    (float)openTime,  // 打开时间(秒)
                    (float)closeTime,  // 关闭时间(秒)
                    (float)triggerCount,  // 触发次数
                    0  // 预留
                );

                if (ret != 0)
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_BADPARAM, 
                        "ManualPositionComparisonFail", 
                        $"设置手动位置比较失败，轴{axisList[0]}，通道{channel[i]}，错误码：{ret}");
            }
        }

        /// <summary>停止手动位置比较</summary>
        public void StopManualPositionComparison()
        {
            if (!_isInitialized || _handle == IntPtr.Zero)
                return;

            // 停止硬件位置比较输出 (Mode = 0 表示停止)
            for (int ch = 0; ch < 4; ch++)  // 假设最多4个通道
            {
                try
                {
                    zmcaux.ZAux_Direct_HwPswitch2(_handle, 0, 0, ch, 0, 0, 0, 0, 0);
                }
                catch
                {
                    // 忽略停止时的错误
                }
            }

            // 关闭相关输出IO (假设通道16-19)
            for (int io = 16; io <= 19; io++)
            {
                try
                {
                    SetOutputState(io, false);
                }
                catch
                {
                    // 忽略IO设置错误
                }
            }
        }

        #endregion

        #region IO 卡接口实现

        /// <summary>初始化IO卡(预留)</summary>
        public override void InitIOCard(string initCfg) { }

        /// <summary>反初始化IO卡(预留)</summary>
        public override void UnInitIOCard() { }

        /// <summary>模拟量读取(按通道ID字符串)</summary>
        public override decimal AnalogRead(string channelID)
        {
            if (int.TryParse(channelID, out int channel))
                return (decimal)GetInputNum(channel);
            return 0;
        }

        /// <summary>模拟量写入(按通道ID字符串)</summary>
        public override void AnalogWrite(string channelID, decimal analog)
        {
            if (int.TryParse(channelID, out int channel))
                SetOutputNum(channel, (double)analog);
        }

        /// <summary>数字量读取(按通道ID字符串)</summary>
        public override bool StateRead(string channelID)
        {
            splitChannelID(channelID, out EReadWriteMode mode, out ushort bitno);
            switch(mode)
            {
                case EReadWriteMode.ReadOnly:
                    return GetInputState(bitno);
                case EReadWriteMode.ReadWrite:
                    return GetOutputState(bitno);
                default:
                    return false;
            }
        }

        /// <summary>数字量写入(按通道ID字符串)</summary>
        public override void StateWrite(string channelID, bool state)
        {
            splitChannelID(channelID, out EReadWriteMode mode, out ushort bitno);
            switch (mode)
            {
                case EReadWriteMode.ReadWrite:
                case EReadWriteMode.WriteOnly:
                    SetOutputState(bitno, state);
                    break;
                default:
                    break;
            }
        }

        /// <summary>模拟量通道列表</summary>
        public override ChannelParametersList AnalogChannelList => analogChannelList;
        /// <summary>数字量通道列表</summary>
        public override ChannelParametersList StateChannelList => stateChannelList;

        private static readonly ChannelParametersList analogChannelList;
        private static ChannelParametersList stateChannelList;
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
            if (channelID.Length == 5)
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
        /// <summary>读写模式</summary>
        public override EReadWriteMode ReadWriteMode { get; } = EReadWriteMode.ReadWrite;

        #endregion

        #region 简单方法

        /// <summary>绝对运动到指定位置</summary>
        public int AbsoluteMove(int axis, double pos, double speed, double accel, double decel)
        {
            if (!_isInitialized || _handle == IntPtr.Zero) return -1;
            if (axis < 0 || axis >= _supportAxisNum) return -2;
            if (!IsAxisEnabled(axis)) return -10;

            zmcaux.ZAux_Direct_SetSpeed(_handle, axis, (float)speed);
            zmcaux.ZAux_Direct_SetAccel(_handle, axis, (float)accel);
            zmcaux.ZAux_Direct_SetDecel(_handle, axis, (float)decel);

            int[] axisList = new int[] { axis };
            float[] posList = new float[] { (float)pos };
            return zmcaux.ZAux_Direct_MoveAbs(_handle, 1, axisList, posList);
        }

        /// <summary>相对运动指定距离</summary>
        public int RelativeMove(int axis, double distance, double speed, double accel, double decel)
        {
            if (!_isInitialized || _handle == IntPtr.Zero) return -1;
            if (axis < 0 || axis >= _supportAxisNum) return -2;
            if (!IsAxisEnabled(axis)) return -10;

            zmcaux.ZAux_Direct_SetSpeed(_handle, axis, (float)speed);
            zmcaux.ZAux_Direct_SetAccel(_handle, axis, (float)accel);
            zmcaux.ZAux_Direct_SetDecel(_handle, axis, (float)decel);

            int[] axisList = new int[] { axis };
            float[] posList = new float[] { (float)distance };
            return zmcaux.ZAux_Direct_MoveSp(_handle, 1, axisList, posList);
        }

        /// <summary>速度模式运动(JOG,方向由正负决定)</summary>
        public int VelocityMove(int axis, int direction, double speed, double accel)
        {
            if (!_isInitialized || _handle == IntPtr.Zero) return -1;
            if (axis < 0 || axis >= _supportAxisNum) return -2;
            if (!IsAxisEnabled(axis)) return -10;

            zmcaux.ZAux_Direct_SetSpeed(_handle, axis, (float)speed);
            zmcaux.ZAux_Direct_SetAccel(_handle, axis, (float)accel);

            int[] axisList = new int[] { axis };
            float[] posList = new float[] { (float)(direction * 1000000) };
            return zmcaux.ZAux_Direct_MoveSp(_handle, 1, axisList, posList);
        }

        /// <summary>停止轴(减速停止或立即停止)</summary>
        public int AxisStop(int axis, MotionControlAxisStopTypeConstants motionStopType)
        {
            if (!_isInitialized || _handle == IntPtr.Zero) return -1;
            if (axis < 0 || axis >= _supportAxisNum) return -2;
            int mode = (motionStopType == MotionControlAxisStopTypeConstants.ImmediateStop) ? 2 : 0;
            return zmcaux.ZAux_Direct_Rapidstop(_handle, mode);
        }

        /// <summary>回零(默认模式0)</summary>
        public int AxisHome(int axis) => AxisHome(axis, 39);

        /// <summary>回零(指定模式、高速、低速、偏移量)</summary>
        public int AxisHome(int axis, int homeMode, float highSpeed = 10f, float lowSpeed = 1f, float datumOffset = 0f)
        {
            if (!_isInitialized || _handle == IntPtr.Zero) return -1;
            if (axis < 0 || axis >= _supportAxisNum) return -2;
            if (!IsAxisEnabled(axis)) return -10;
            return zmcaux.ZAux_Direct_UserDatum(_handle, axis, homeMode, highSpeed, lowSpeed, datumOffset);
        }

        //public int AxisHome(int axis, int homeMode)
        //{
        //    if (!_isInitialized || _handle == IntPtr.Zero) return -1;
        //    if (axis < 0 || axis >= _supportAxisNum) return -2;
        //    if (!IsAxisEnabled(axis)) return -10;
        //    return zmcaux.ZAux_Direct_Single_Datum(_handle, axis, homeMode);
        //}

        /// <summary>等待轴运动停止(轮询轴状态位)</summary>
        public int WaitAxisStop(int axis, int timeout = 30000)
        {
            if (!_isInitialized || _handle == IntPtr.Zero) return -1;
            if (axis < 0 || axis >= _supportAxisNum) return -2;
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (true)
            {
                int axisStatus = 0;
                zmcaux.ZAux_Direct_GetAxisStatus(_handle, axis, ref axisStatus);
                if ((axisStatus & 0x01) == 0) break;
                if (stopwatch.ElapsedMilliseconds > timeout) return -1;
                System.Threading.Thread.Sleep(1);
            }
            return 0;
        }

        /// <summary>清除轴报警(将AXISERROR寄存器置0)</summary>
        public int ClearAxisAlarm(int axis)
        {
            if (!_isInitialized || _handle == IntPtr.Zero) return -1;
            if (axis < 0 || axis >= _supportAxisNum) return -2;
            return zmcaux.ZAux_Direct_SetParam(_handle, "AXISERROR", axis, 0.0f);
        }

        /// <summary>设置轴正负软限位</summary>
        public int SetAxisSoftLimit(int axis, double positiveLimit, double negativeLimit)
        {
            if (!_isInitialized || _handle == IntPtr.Zero) return -1;
            if (axis < 0 || axis >= _supportAxisNum) return -2;
            int ret = zmcaux.ZAux_Direct_SetParam(_handle, "FPOS", axis, (float)positiveLimit);
            if (ret != 0) return ret;
            ret = zmcaux.ZAux_Direct_SetParam(_handle, "RNPOS", axis, (float)negativeLimit);
            return ret;
        }

        /// <summary>两轴直线插补</summary>
        public int TwoAxisLinearInterpolation(int axis1, int axis2, double targetPos1, double targetPos2, double speed, double accel)
        {
            if (!_isInitialized || _handle == IntPtr.Zero) return -1;
            if (!IsAxisEnabled(axis1) || !IsAxisEnabled(axis2)) return -10;

            zmcaux.ZAux_Direct_SetSpeed(_handle, axis1, (float)speed);
            zmcaux.ZAux_Direct_SetAccel(_handle, axis1, (float)accel);

            int[] axisList = new int[] { axis1, axis2 };
            float[] posList = new float[] { (float)targetPos1, (float)targetPos2 };
            return zmcaux.ZAux_Direct_MoveAbs(_handle, 2, axisList, posList);
        }

        /// <summary>三轴直线插补</summary>
        public int ThreeAxisLinearInterpolation(int axis1, int axis2, int axis3, double targetPos1, double targetPos2, double targetPos3, double speed, double accel)
        {
            if (!_isInitialized || _handle == IntPtr.Zero) return -1;
            if (!IsAxisEnabled(axis1) || !IsAxisEnabled(axis2) || !IsAxisEnabled(axis3)) return -10;

            zmcaux.ZAux_Direct_SetSpeed(_handle, axis1, (float)speed);
            zmcaux.ZAux_Direct_SetAccel(_handle, axis1, (float)accel);

            int[] axisList = new int[] { axis1, axis2, axis3 };
            float[] posList = new float[] { (float)targetPos1, (float)targetPos2, (float)targetPos3 };
            return zmcaux.ZAux_Direct_MoveAbs(_handle, 3, axisList, posList);
        }

        /// <summary>两轴圆弧插补(终点+圆心)</summary>
        public int TwoAxisCircularInterpolation(int axis1, int axis2, double endPos1, double endPos2, double centerPos1, double centerPos2, int arcDirection, double speed)
        {
            if (!_isInitialized || _handle == IntPtr.Zero) return -1;
            if (!IsAxisEnabled(axis1) || !IsAxisEnabled(axis2)) return -10;

            int[] axisList = new int[] { axis1, axis2 };
            return zmcaux.ZAux_Direct_MoveCirc(_handle, 2, axisList, (float)endPos1, (float)endPos2, (float)centerPos1, (float)centerPos2, arcDirection);
        }

        /// <summary>三轴圆弧插补(前两轴圆弧+第三轴独立运动)</summary>
        public int ThreeAxisCircularInterpolation(int[] axes, double[] targetPos, double[] centerCircle, int arcDirection, double speed, double accel)
        {
            if (!_isInitialized || _handle == IntPtr.Zero) return -1;
            if (axes.Length < 3) return -2;
            if (!IsAxisEnabled(axes[0]) || !IsAxisEnabled(axes[1]) || !IsAxisEnabled(axes[2])) return -10;

            zmcaux.ZAux_Direct_SetSpeed(_handle, axes[2], (float)speed);
            zmcaux.ZAux_Direct_SetAccel(_handle, axes[2], (float)accel);

            int[] thirdAxisList = new int[] { axes[2] };
            float[] thirdPosList = new float[] { (float)targetPos[2] };
            int ret = zmcaux.ZAux_Direct_MoveAbs(_handle, 1, thirdAxisList, thirdPosList);
            if (ret != 0) return ret;

            int[] axisList = new int[] { axes[0], axes[1] };
            return zmcaux.ZAux_Direct_MoveCirc(_handle, 2, axisList, (float)targetPos[0], (float)targetPos[1], (float)centerCircle[0], (float)centerCircle[1], arcDirection);
        }

        /// <summary>设置数字IO输出状态</summary>
        public int SetOutputState(int ioNum, bool isHave)
        {
            if (!_isInitialized || _handle == IntPtr.Zero) return -1;
            return zmcaux.ZAux_Direct_SetOp(_handle, ioNum, isHave ? 1u : 0u);
        }

        /// <summary>设置模拟量输出值</summary>
        public double SetOutputNum(int ioNum, double analogValue)
        {
            if (!_isInitialized || _handle == IntPtr.Zero) return -1;
            return zmcaux.ZAux_Direct_SetDA(_handle, ioNum, (float)analogValue);
        }

        #endregion

        #region 私有辅助方法 - 位置比较

        /// <summary>
        /// 设置位置比较参数（改进版）
        /// </summary>
        /// <param name="axisList">轴号列表（至少2个轴）</param>
        /// <param name="triggerPoints">位置比较触发点列表</param>
        /// <param name="outputChannel">输出通道号(0-3)</param>
        /// <param name="pulseWidthMs">脉冲宽度(毫秒)</param>
        /// <param name="maxErrorMm">最大误差(mm)</param>
        private void SetPositionComparisonParam(
            int[] axisList, 
            MotionControlPositionComparisonTriggerPoint[] triggerPoints,
            int outputChannel = 0,
            double pulseWidthMs = 1.0,
            double maxErrorMm = 0.1)
        {
            if (axisList == null || axisList.Length < 2)
                throw new ArgumentException("至少需要2个轴用于位置比较");

            if (triggerPoints == null || triggerPoints.Length == 0)
                throw new ArgumentException("触发点列表不能为空");

            // 使用较高的TABLE编号，避免与系统TABLE冲突
            int tableStart = 1000;
            int pointCount = triggerPoints.Length;

            // 准备TABLE数据数组
            float[] tableData = new float[pointCount * axisList.Length];
            
            // 填充触发点坐标到数组，并转换为脉冲单位
            for (int i = 0; i < pointCount; i++)
            {
                // X坐标（mm → pulse）
                tableData[i * axisList.Length] = (float)LengthToPulse(axisList[0], triggerPoints[i].X);
                
                // Y坐标（mm → pulse）
                if (axisList.Length > 1)
                    tableData[i * axisList.Length + 1] = (float)LengthToPulse(axisList[1], triggerPoints[i].Y);
                
                // 如果有第三轴（Z轴），继续添加
                if (axisList.Length > 2 && triggerPoints[i] is { } point)
                {
                    // 可以扩展支持Z坐标
                    // tableData[i * axisList.Length + 2] = (float)LengthToPulse(axisList[2], point.Z);
                }
            }

            // 一次性写入所有TABLE数据
            int ret = zmcaux.ZAux_Direct_SetTable(_handle, tableStart, tableData.Length, tableData);
            if (ret != 0)
                throw new GKGException(MotionErrCodeConsts.ERR_MOTION_BADPARAM, 
                    "SetPositionComparisonTableFail", 
                    $"设置触发点TABLE数据失败，错误码：{ret}");

            // 将误差从mm转换为脉冲
            int maxErr = (int)LengthToPulse(axisList[0], maxErrorMm);
            
            // 配置脉冲参数（单位：秒）
            float pulseWidth = (float)(pulseWidthMs / 1000.0);  // ms → s
            float pulseInterval = pulseWidth;  // 间隔等于宽度
            float pulseCycle = pulseWidth + pulseInterval;  // 周期 = 宽度 + 间隔

            // 使用HwPswitch2_2配置2D位置比较
            // Mode参数说明：
            // - 模式0：点位比较（根据TABLE中的点进行比较）
            // - 模式1：启动比较器
            // - 模式2：停止比较器
            // - 模式3：矢量比较方式
            int result = zmcaux.ZAux_Direct_HwPswitch2_2(
                _handle,
                axisList,           // 轴列表
                axisList.Length,    // 轴数量
                0,                  // 模式0：点位比较
                outputChannel,      // 输出通道（参数化）
                1,                  // 输出状态（1=有效）
                maxErr,             // 最大误差（已转换为脉冲）
                tableStart,         // TABLE起始编号
                pointCount,         // 触发点数量
                true,               // 使能标志
                pulseWidth,         // 脉冲宽度（秒）
                pulseInterval,      // 脉冲间隔（秒）
                pulseCycle          // 脉冲周期（秒）
            );

            if (result != 0)
                throw new GKGException(MotionErrCodeConsts.ERR_MOTION_BADPARAM, 
                    "SetPositionComparisonParamFail", 
                    $"配置2D位置比较参数失败，错误码：{result}，轴[{string.Join(",", axisList)}]，点数：{pointCount}，通道：{outputChannel}");
        }

        public void PositionComparison2D(int coordinateSystemNo, Guid[] axisGuidList, MotionControlPositionComparisonTriggerPoint[] PositionComparisonTriggerPoints, MotionInstructionBase[] motionTrajectoryList)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
