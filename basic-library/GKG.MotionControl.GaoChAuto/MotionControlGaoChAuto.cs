using System.Diagnostics;
using static GC.Frame.Motion.Privt.CNMCLib20;
using HAND = System.UInt16;

namespace GKG.ElectronicControl
{
    namespace GaoChAuto
    {
        /// <summary>
        /// A类运控接口实现
        /// </summary>
        /// 高川卡实现

        public class MotionControlGaoChAuto : MotionControlBase, IMotionControlCategoryA
        {
            //坐标系最大加速度
            public const Int32 GC_MAXCRDACC = 4500;

            //坐标系最大速度
            public const Int32 GC_MAXCRDVEL = 4500;

            //坐标系最大平滑减速度
            public const Int32 GC_MAXSMOOTHDEC = 10;

            public static readonly short[] NormalChannel = new short[4] { 16, 17, 18, 19 };

            /// <summary>
            /// 支持的坐标系数量
            /// </summary>
            public int SupportCoordinateSystemNum { get; } = 2;

            //板卡控制器操作句柄
            private HAND DevHand;

            //轴操作句柄列表
            private HAND[]? AxisHand;

            //坐标系操作句柄列表
            private HAND[]? CoordinateSystemHand;

            //插补运动前瞻参数
            private Dictionary<int, MotionControlArcFeedForwardParameters>? ArcFeedForwardParameters;

            //二维位置比较参数
            private Dictionary<int, MotionControlPositionComparison2DParameters>? PositionComparison2DParameters;

            //中断2D位置比较输出
            private bool StopCompare2DInterruptOutput = false;

            public MotionControlGaoChAuto()
            {
                SupportCoordinateSystemNum = 2;
                _supportAxisNum = 8;
                _supportAnalogNum = 32;
                _supportIoStateNum = 32;
                _isTwoAxisLinearInterpolation = true;
                _isThreeAxisLinearInterpolation = true;
                _isTwoAxisCircularInterpolation = true;
                _isThreeAxisCircularInterpolation = true;
            }

            /// <summary>
            /// 通过轴号获取高川轴句柄
            /// </summary>
            private HAND GetAxisHANDByAxis(int axis)
            {
                if (AxisHand == null)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_INIT_FAIL, MotionErr.MotionCardInitFailed, MotionErr.MotionCardInitFailed);
                }
                if (axis < 0 || axis >= AxisHand.Length)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_BADPARAM, MotionErr.AxisOutOfRange, MotionErr.AxisOutOfRange);
                }
                return AxisHand[axis];
            }

            /// <summary>
            /// 初始化运控卡
            /// </summary>
            /// <param name="CardNo">卡号</param>
            public override int IniMotionCard(int CardNo)
            {
                base.IniMotionCard(CardNo);
                NMC_SetCmdDebug(3, "");
                CoordinateSystemHand = new HAND[2];
                short rtn = 0;                  // 指令返回值
                short devNum = 0;               // 设备序号，从0开始
                byte[] pInfoList = new byte[256]; // 设备信息缓冲区

                rtn = NMC_DevSearch(TSearchMode.Ethernet, ref devNum, pInfoList); // 搜索控制器
                if (rtn != 0)
                {
                    return rtn;
                }

                //AxisLock = new Mutex[SupportAxisNum];
                //// 初始化轴锁定互斥量
                //for (int i = 0; i < SupportAxisNum; i++)
                //{
                //    AxisLock[i] = new Mutex();
                //}

                //// 初始化IO锁定互斥量
                //IoLock = new Mutex[SupportIoStateNum];
                //for (int i = 0; i < SupportIoStateNum; i++)
                //{
                //    IoLock[i] = new Mutex();
                //}

                AxisHand = new HAND[SupportAxisNum];
                for (ushort axisNo = 0; axisNo < SupportAxisNum; axisNo++)
                {
                    AxisHand[axisNo] = axisNo;
                }

                if (CardNo < devNum)
                {
                    rtn = NMC_DevOpen((short)CardNo, ref DevHand);  // 打开控制器
                    if (rtn != 0)
                    {
                        return rtn;
                    }
                    rtn = NMC_DevReset(DevHand);  // 复位控制器：控制器复位到初始状态
                    if (rtn != 0)
                    {
                        return rtn;
                    }

                    // 由初始化传入的参数
                    // 下载控制器配置文件
                    string configPath = $"GCN800{CardNo}.cfg";
                    byte[] configPathBytes = System.Text.Encoding.Default.GetBytes(configPath);

                    //加载控制器配置
                    rtn = NMC_LoadConfigFromFile(DevHand, configPathBytes);

                    for (short axisNo = 0; axisNo < SupportAxisNum; axisNo++)
                    {
                        rtn = NMC_MtOpen(DevHand, axisNo, ref AxisHand[axisNo]);  // 打开轴,获得轴句柄
                        if (rtn != 0)
                        {
                            return rtn;
                        }
                        rtn = NMC_MtZeroPos(AxisHand[axisNo]);  // 轴位置清零,命令位置和编码器位置强制清零
                        if (rtn != 0)
                        {
                            return rtn;
                        }
                        rtn = NMC_MtSetAxisArrivalPara(AxisHand[axisNo], 10, 3); // 设置轴到位误差带
                        if (rtn != 0)
                        {
                            return rtn;
                        }
                    }
                }
                else
                {
                    return -1;
                }
                return rtn;
            }

            /// <summary>
            /// 反初始化运控卡
            /// </summary>
            public override int CloseMotionCard(int CardNo)
            {
                return NMC_DevClose(ref DevHand);
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
                HAND axisHand = GetAxisHANDByAxis(axis);

                short sres = 0;
                int level = 0;
                bool isSignal = false;
                NMC_MtGetSts(axisHand, ref sres);

                switch (motionStatus)
                {
                    case MotionControlAxisStatus.Origin:
                        NMC_MtGetMotionIO(axisHand, ref level);
                        level = 1 & (level >> 2);
                        if (level == 1) isSignal = true;
                        else isSignal = false;
                        break;

                    case MotionControlAxisStatus.Alarm:
                        isSignal = (1 & (sres >> 10)) == 1;
                        break;

                    case MotionControlAxisStatus.PositiveLimit:
                        isSignal = (1 & (sres >> 6)) == 1;
                        break;

                    case MotionControlAxisStatus.NegativeLimit:
                        isSignal = (1 & (sres >> 7)) == 1;
                        break;

                    case MotionControlAxisStatus.Inp:
                        isSignal = false;
                        break;

                    case MotionControlAxisStatus.EZ:
                        isSignal = false;
                        break;

                    case MotionControlAxisStatus.ServoEnable:
                        isSignal = (1 & (sres >> 3)) == 1;
                        break;

                    case MotionControlAxisStatus.Ready:
                        isSignal = false;
                        break;
                }
                return isSignal;
            }

            /// <summary>
            /// 读取单轴状态 是否报警,是否使能
            /// </summary>
            public override bool GetAxisState(Guid guid, MotionControlAxisStatus motionStatus)
            {
                return GetAxisState(GetAxisByGuid(guid), motionStatus);
            }

            /// <summary>
            /// 绝对运动
            /// </summary>
            /// <param name="guid">轴锁定句柄</param>
            /// <param name="motionType">运动类型</param>
            /// <param name="pos">目标位置，单位mm</param>
            /// <param name="startSpeed">起跳速度，单位mm/s</param>
            /// <param name="maxSpeed">最大速度，单位mm/s</param>
            /// <param name="accTimeT">T形曲线加速时间，单位s</param>
            /// <param name="decTimeT">T形曲线减速时间，单位s</param>
            /// <param name="accTimeS">S形曲线加速时间，单位s</param>
            /// <param name="decTimeS">S形曲线减速时间，单位s</param>
            public override int AbsoluteMove(Guid guid, int motionType, double pos, double startSpeed,
                double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS)
            {
                int rtn = 0;
                int axisNo = GetAxisByGuid(guid);
                HAND axisHand = GetAxisHANDByAxis(axisNo);

                int targetPos = LengthToPulse(axisNo, pos);

                //配置成点动模式
                rtn = NMC_MtSetPrfMode(axisHand, MT_PTP_PRF_MODE);
                if (rtn != RTN_CMD_SUCCESS)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_ABSULOTEMOVE_FAIL, MotionErr.AbsuloteMoveFail, MotionErr.AbsuloteMoveFail);
                }

                //运动参数配置
                TPtpPara ptpPrm = new TPtpPara();

                rtn = NMC_MtGetPtpPara(axisHand, ref ptpPrm);
                if (rtn != RTN_CMD_SUCCESS)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_BADPARAM, MotionErr.AbsuloteMoveFail, MotionErr.AbsuloteMoveFail);
                }
                //加速度 pulse/us²
                ptpPrm.acc = SpeedMmPerSecToPulsePerMs(axisNo, (Math.Abs(maxSpeed) / decTimeT));

                //减速度 pulse/us²
                ptpPrm.dec = SpeedMmPerSecToPulsePerMs(axisNo, (Math.Abs(maxSpeed) / decTimeT));

                //结束速度 pulse/us
                ptpPrm.endVel = 0;

                //起跳速度 pulse/us
                ptpPrm.startVel = SpeedMmPerSecToPulsePerMs(axisNo, Math.Abs(startSpeed));

                if (ptpPrm.acc <= 0)
                {
                    ptpPrm.acc = 0.1;
                }

                if (ptpPrm.dec <= 0)
                {
                    ptpPrm.dec = 0.1;
                }

                if (ptpPrm.startVel <= 0)
                {
                    ptpPrm.startVel = 1;
                }
                rtn = NMC_MtSetPtpPara(axisHand, ref ptpPrm);
                if (rtn != RTN_CMD_SUCCESS)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_BADPARAM, MotionErr.AbsoluteMoveStartSpeed, MotionErr.AbsoluteMoveStartSpeed);
                }

                //设置最高速度
                rtn = NMC_MtSetVel(axisHand, maxSpeed);
                if (rtn != RTN_CMD_SUCCESS)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_BADPARAM, MotionErr.AbsoluteMoveMaxSpeed, MotionErr.AbsoluteMoveMaxSpeed);
                }

                //设置目标位置
                rtn = NMC_MtSetPtpTgtPos(axisHand, (int)targetPos);
                if (rtn != RTN_CMD_SUCCESS)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_BADPARAM, MotionErr.AbsoluteMoveTargetPosition, MotionErr.AbsoluteMoveTargetPosition);
                }

                //启动运动
                rtn = NMC_MtUpdate(axisHand);
                if (rtn != RTN_CMD_SUCCESS)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_ABSULOTEMOVE_FAIL, MotionErr.AbsoluteMoveStart, MotionErr.AbsoluteMoveStart);
                }
                return rtn;
            }

            /// <summary>
            /// 相对运动
            /// </summary>
            /// <param name="guid">轴锁定句柄</param>
            /// <param name="motionType">运动类型</param>
            /// <param name="pos">移动距离，单位mm</param>
            /// <param name="startSpeed">起跳速度，单位mm/s</param>
            /// <param name="maxSpeed">最大速度，单位mm/s</param>
            /// <param name="accTimeT">T形曲线加速时间，单位s</param>
            /// <param name="decTimeT">T形曲线减速时间，单位s</param>
            /// <param name="accTimeS">S形曲线加速时间，单位s</param>
            /// <param name="decTimeS">S形曲线减速时间，单位s</param>
            public override int RelativeMove(Guid guid, int motionType, double pos, double startSpeed,
                double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS)
            {
                int rtn = 0;
                int axisNo = GetAxisByGuid(guid);
                HAND axisHand = GetAxisHANDByAxis(axisNo);
                int distance = LengthToPulse(axisNo, pos);

                //配置成点动模式
                rtn = NMC_MtSetPrfMode(axisHand, MT_PTP_PRF_MODE);
                if (rtn != RTN_CMD_SUCCESS)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_RELATIVEMOVE_FAIL, MotionErr.RelativeMoveFail, MotionErr.RelativeMoveFail);
                }
                //运动参数配置
                TPtpPara ptpPrm = new();

                rtn = NMC_MtGetPtpPara(axisHand, ref ptpPrm);
                if (rtn != RTN_CMD_SUCCESS)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_RELATIVEMOVE_FAIL, MotionErr.RelativeMoveFail, MotionErr.RelativeMoveFail);
                }
                //加速度 pulse/us²
                ptpPrm.acc = SpeedMmPerSecToPulsePerMs(axisNo, (Math.Abs(maxSpeed) / decTimeT));

                //减速度 pulse/us²
                ptpPrm.dec = SpeedMmPerSecToPulsePerMs(axisNo, (Math.Abs(maxSpeed) / decTimeT));

                //结束速度 pulse/us
                ptpPrm.endVel = 0;

                //起跳速度 pulse/us
                ptpPrm.startVel = SpeedMmPerSecToPulsePerMs(axisNo, Math.Abs(startSpeed));

                if (ptpPrm.acc <= 0)
                {
                    ptpPrm.acc = 0.1;
                }

                if (ptpPrm.dec <= 0)
                {
                    ptpPrm.dec = 0.1;
                }

                if (ptpPrm.startVel <= 0)
                {
                    ptpPrm.startVel = 1;
                }

                rtn = NMC_MtSetPtpPara(axisHand, ref ptpPrm);
                if (rtn != RTN_CMD_SUCCESS)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_RELATIVEMOVE_FAIL, MotionErr.RelativeMoveFail, MotionErr.RelativeMoveFail);
                }

                //设置最高速度
                rtn = NMC_MtSetVel(axisHand, maxSpeed);
                if (rtn != RTN_CMD_SUCCESS)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_RELATIVEMOVE_FAIL, MotionErr.RelativeMoveFail, MotionErr.RelativeMoveFail);
                }

                //获取目标位置
                double currentPos = GetAxisPos(axisNo, MotionControlAxisPositionType.Command);

                //设置目标位置 pulse
                rtn = NMC_MtSetPtpTgtPos(axisHand, (int)(distance + LengthToPulse(axisNo, currentPos)));
                if (rtn != RTN_CMD_SUCCESS)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_RELATIVEMOVE_FAIL, MotionErr.RelativeMoveFail, MotionErr.RelativeMoveFail);
                }

                //启动运动
                rtn = NMC_MtUpdate(axisHand);
                if (rtn != RTN_CMD_SUCCESS)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_RELATIVEMOVE_FAIL, MotionErr.RelativeMoveFail, MotionErr.RelativeMoveFail);
                }
                return rtn;
            }

            /// <summary>
            /// 连续运动
            /// </summary>
            /// <param name="guid">轴锁定句柄</param>
            /// <param name="motionType">运动类型</param>
            /// <param name="startSpeed">起跳速度，单位mm/s</param>
            /// <param name="maxSpeed">最大速度，单位mm/s</param>
            /// <param name="accTimeT">T形曲线加速时间，单位s</param>
            /// <param name="decTimeT">T形曲线减速时间，单位s</param>
            /// <param name="accTimeS">S形曲线加速时间，单位s</param>
            /// <param name="decTimeS">S形曲线减速时间，单位s</param>
            public override int VelocityMove(Guid guid, int motionType, double startSpeed,
                double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS)
            {
                int rtn = 0;
                int axisNo = GetAxisByGuid(guid);
                HAND axisHand = GetAxisHANDByAxis(axisNo);

                //配置成JOG模式
                rtn = NMC_MtSetPrfMode(axisHand, MT_JOG_PRF_MODE);
                if (rtn != RTN_CMD_SUCCESS)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_VELOCITYMOVE_FAIL, MotionErr.VelocityMoveFail, MotionErr.VelocityMoveFail);
                }

                double acc = SpeedMmPerSecToPulsePerMs(axisNo, Math.Abs(maxSpeed) / accTimeT);             //加速度
                double dec = SpeedMmPerSecToPulsePerMs(axisNo, Math.Abs(maxSpeed) / accTimeT);             //减速度

                TJogPara jogPrm;
                jogPrm.acc = acc; //加速度
                jogPrm.dec = dec; //减速度
                jogPrm.smoothCoef = 1;  //平滑系数

                rtn = NMC_MtSetJogPara(axisHand, ref jogPrm);
                if (rtn != RTN_CMD_SUCCESS)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_VELOCITYMOVE_FAIL, MotionErr.VelocityMoveFail, MotionErr.VelocityMoveFail);
                }

                //设置规划最高速度
                rtn = NMC_MtSetVel(axisHand, maxSpeed);
                if (rtn != RTN_CMD_SUCCESS)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_VELOCITYMOVE_FAIL, MotionErr.VelocityMoveFail, MotionErr.VelocityMoveFail);
                }

                //启动运动
                rtn = NMC_MtUpdate(axisHand);
                if (rtn != RTN_CMD_SUCCESS)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_VELOCITYMOVE_FAIL, MotionErr.VelocityMoveFail, MotionErr.VelocityMoveFail);
                }
                return rtn;
            }

            /// <summary>
            /// 轴停止
            /// </summary>
            public override int AxisStop(Guid guid, MotionControlAxisStopTypeConstants motionStopType)
            {
                int rtn = 0;
                int axisNo = GetAxisByGuid(guid);
                HAND axisHand = GetAxisHANDByAxis(axisNo);
                short sres = 0;

                //判断轴是否在坐标系模式下
                rtn = NMC_MtGetPrfMode(axisHand, ref sres);
                if (sres == MT_CRD_PRF_MODE || sres == MT_MULTI_LINE_MODE)
                {
                    switch (motionStopType)
                    {
                        case MotionControlAxisStopTypeConstants.ImmediateStop:
                            //PositionComparison2DParameters[0].AxisList

                            //rtn = NMC_CrdStopMtn(CoordinateSystemHand[0]);
                            break;

                        case MotionControlAxisStopTypeConstants.DecelerationStop:
                            //rtn = NMC_CrdEstopMtn();
                            break;
                    }
                }
                else
                {
                    switch (motionStopType)
                    {
                        case MotionControlAxisStopTypeConstants.ImmediateStop:
                            rtn = NMC_MtStop(axisHand);
                            break;

                        case MotionControlAxisStopTypeConstants.DecelerationStop:
                            rtn = NMC_MtStop(axisHand);
                            break;
                    }
                }
                return 0;
            }

            /// <summary>
            /// 单轴回零
            /// </summary>
            public override int AxisHome(Guid guid)
            {
                int rtn = 0;
                int axisNo = GetAxisByGuid(guid);
                HAND axisHand = GetAxisHANDByAxis(axisNo);
                if (MotionControlFactoryParameters == null || MotionControlFactoryParameters.Parameters == null)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_AXISHOME_FAIL, MotionErr.AxisHomeFail, MotionErr.AxisHomeFail);
                }
                if (axisNo >= MotionControlFactoryParameters.Parameters.Length)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_AXISHOME_FAIL, MotionErr.AxisHomeFail, MotionErr.AxisHomeFail);
                }
                //强制清零
                rtn = NMC_MtZeroPos(axisHand);
                if (rtn != RTN_CMD_SUCCESS)
                {
                    // 参数配置出错
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_AXISHOME_FAIL, MotionErr.AxisHomeFail, MotionErr.AxisHomeFail);
                }

                THomeSetting homeSetup = new THomeSetting(true);
                rtn = NMC_MtGetHomePara(axisHand, ref homeSetup);//获取卡当前配置的回原参数
                if (rtn != RTN_CMD_SUCCESS)
                {
                    // 参数配置出错
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_AXISHOME_FAIL, MotionErr.AxisHomeFail, MotionErr.AxisHomeFail);
                }

                //只修改回原方式，速度等

                MotionControlAxisHomingMode motionHomingMode = MotionControlFactoryParameters.Parameters[axisNo].AxisHomingParameters.HomingMode;
                switch (motionHomingMode)
                {
                    case MotionControlAxisHomingMode.OnceOriginGoHome:
                        homeSetup.mode = (short)THomeMode.HM_MODE1;
                        break;

                    case MotionControlAxisHomingMode.TwiceOriginGoHome:
                        homeSetup.mode = (short)THomeMode.HM_MODE1;
                        homeSetup.reScanEn = 1;
                        break;

                    case MotionControlAxisHomingMode.NegativeGoHome:
                        homeSetup.mode = (short)THomeMode.HM_MODE2; // HOME_MODE_LIMIT_INDEX;
                        break;

                    case MotionControlAxisHomingMode.EZGoHome:
                        homeSetup.mode = (short)THomeMode.HM_MODE3;
                        break;

                    case MotionControlAxisHomingMode.FindEZStop:
                        homeSetup.mode = (short)THomeMode.HM_MODE3;
                        break;

                    case MotionControlAxisHomingMode.FindEZLatchBack:
                        homeSetup.mode = (short)THomeMode.HM_MODE3;
                        homeSetup.reScanEn = 1;
                        break;

                    default:
                        return -1;
                }

                /*
                homeSetup.mode = HM_MODE1;      // 回零模式(HM_MODE1单原点回零)
                homeSetup.dir = 0;              // 搜寻零点方向（必须）, 0:负向,1：正向,其它值无意义
                homeSetup.offset = 0;           // 原点偏移
                homeSetup.scan1stVel = 5;       // 基本搜寻速度
                homeSetup.scan2ndVel = 0;       // 二次回零时使用，低速(建议小于10p/ms)，与参数reScanEn一起使用
                homeSetup.acc = 0.5;            // 加速度
                homeSetup.reScanEn = 0;         // 二次搜寻零点，与参数scan2ndVel一起使用
                homeSetup.homeEdge = 0;         // 原点，触发沿,下降沿
                homeSetup.lmtEdge = 0;          // 限位,触发沿(默认下降沿)
                homeSetup.zEdge = 0;            // Z相位,触发沿(默认下降沿)
                homeSetup.iniRetPos = 0;        // 起始反向运动距离（可选,不用时设为0）
                homeSetup.retSwOffset = 0;      // 反向运动时离开开关距离（可选,不用时设为0）
                homeSetup.safeLen = 0;          // 安全距离,回零时最远搜寻距离（可选,不用时设为0,不限制距离）
                homeSetup.usePreSetPtpPara = 0; //当usePreSetPtpPara=0时，回零运动的
                                                //减加速度默认等于acc,起跳速度、终点速度、平滑系数默认为0
                */
                // 搜寻零点方向（必须）, 0:负向,1：正向,其它值无意义
                homeSetup.dir = (short)MotionControlFactoryParameters.Parameters[axisNo].AxisHomingParameters.HomingDirection;

                //设置捕获沿，搜索原点时的运动方向 配置文件配好
                //设置回原速度，加速度
                homeSetup.scan1stVel = SpeedMmPerSecToPulsePerMs(axisNo, MotionControlFactoryParameters.Parameters[axisNo].AxisHomingParameters.HomingMaximumSpeed);
                homeSetup.scan2ndVel = SpeedMmPerSecToPulsePerMs(axisNo, MotionControlFactoryParameters.Parameters[axisNo].AxisHomingParameters.HomingMinimumSpeed);
                homeSetup.acc = SpeedMmPerSecToPulsePerMs(axisNo, MotionControlFactoryParameters.Parameters[axisNo].AxisHomingParameters.HomingAccelerationTime);

                //最终停止的位置相对于原点的偏移量,
                homeSetup.offset = LengthToPulse(axisNo, (int)MotionControlFactoryParameters.Parameters[axisNo].AxisHomingParameters.HomingRetractDistance);
                //搜索到原点信号，反向移动距离
                homeSetup.retSwOffset = LengthToPulse(axisNo, (int)MotionControlFactoryParameters.Parameters[axisNo].AxisHomingParameters.HomingRetractDistance);
                rtn = NMC_MtSetHomePara(axisHand, ref homeSetup);
                if (rtn != RTN_CMD_SUCCESS)
                {
                    // 参数配置出错
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_AXISHOME_FAIL, MotionErr.AxisHomeFail, MotionErr.AxisHomeFail);
                }
                // 启动回零
                rtn = NMC_MtHome(axisHand);
                if (rtn != RTN_CMD_SUCCESS)
                {
                    // 参数配置出错
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_AXISHOME_FAIL, MotionErr.AxisHomeFail, MotionErr.AxisHomeFail);
                }
                // 多轴同步回零的情况下，此处是否不需要等待，而是外部统一等待？？？
                short homeState = BIT_AXHOME_BUSY;
                Stopwatch stopwatch = Stopwatch.StartNew();
                while (true)
                {
                    rtn = NMC_MtGetHomeSts(axisHand, ref homeState);   //读取回零状态
                    if (rtn != RTN_CMD_SUCCESS)
                    {
                        // 参数配置出错
                        throw new GKGException(MotionErrCodeConsts.ERR_MOTION_AXISHOME_FAIL, MotionErr.AxisHomeFail, MotionErr.AxisHomeFail);
                    }
                    if ((homeState & BIT_AXHOME_OK) != 0)
                    {
                        break;                      // 对应轴原点信号被触发，回零完成
                    }
                    if (((homeState & BIT_AXHOME_FAIL) != 0)
                        || ((homeState & BIT_AXHOME_ERR_MV) != 0)
                        || ((homeState & BIT_AXHOME_ERR_SWT) != 0))
                    {
                        throw new GKGException(MotionErrCodeConsts.ERR_MOTION_AXISHOME_FAIL, MotionErr.AxisHomeFail, MotionErr.AxisHomeFail);
                    }
                    if (stopwatch.ElapsedMilliseconds > 60000)
                    {
                        //60s超时
                        throw new GKGException(MotionErrCodeConsts.ERR_MOTION_AXISHOME_FAIL, MotionErr.AxisHomeFail, MotionErr.AxisHomeFail);
                    }
                    Thread.Sleep(1);
                }

                return rtn;
            }

            /// <summary>
            /// 使能开关
            /// </summary>
            public override int AxisEnabled(int axis, bool isEnabled)
            {
                HAND axisHand = GetAxisHANDByAxis(axis);
                if (isEnabled)
                {
                    NMC_MtSetSvOn(axisHand);
                }
                else
                {
                    NMC_MtSetSvOff(axisHand);
                }
                return 0;
            }

            /// <summary>
            /// 获取轴当前位置
            /// </summary>
            /// <param name="axis">轴号</param>
            /// <param name="motionAxisType">位置类型</param>
            /// <returns>轴当前位置(mm)</returns>
            public override double GetAxisPos(int axis, MotionControlAxisPositionType motionAxisType)
            {
                int pos = 0; //单位：pulse
                HAND axisHand = GetAxisHANDByAxis(axis);
                int rtn = 0;

                switch (motionAxisType)
                {
                    case MotionControlAxisPositionType.Command:
                        rtn = NMC_MtGetPrfPos(axisHand, ref pos);   //读取规划坐标系位置
                        break;

                    case MotionControlAxisPositionType.Actual:
                        break;

                    case MotionControlAxisPositionType.Target:
                        rtn = NMC_MtGetCmdPos(axisHand, ref pos);//读取发送到执行器的目标位置
                        break;

                    case MotionControlAxisPositionType.EncoderInternal:
                        rtn = NMC_MtGetEncPos(axisHand, ref pos);//编码器位置
                        break;

                    default:
                        break;
                }

                return PulseToLength(axis, pos);
            }

            /// <summary>
            /// 获取轴当前位置
            /// </summary>
            /// <param name="guid">轴锁定句柄</param>
            /// <param name="motionAxisType">位置类型</param>
            /// <returns>轴当前位置(mm)</returns>
            public override double GetAxisPos(Guid guid, MotionControlAxisPositionType motionAxisType)
            {
                return GetAxisPos(GetAxisByGuid(guid), motionAxisType);
            }

            /// <summary>
            /// 设置轴当前位置
            /// </summary>
            public override double SetAxisPos(int axis, double axisPos)
            {
                int rtn = 0;
                HAND axisHand = GetAxisHANDByAxis(axis);
                rtn = NMC_MtClrError(axisHand);

                // 设置规划位置只能在轴静止时使用
                rtn = NMC_MtSetAxisPos(axisHand, (int)axisPos);
                if (rtn != RTN_CMD_SUCCESS) return rtn;
                // 设置编码器位置
                rtn = NMC_MtSetEncPos(axisHand, (int)axisPos);
                if (rtn != RTN_CMD_SUCCESS) return rtn;
                // 轴静止时执行, 如果后面是 update 指令, 需要延时一个周期
                Thread.Sleep(50);
                return rtn;
            }

            /// <summary>
            /// 等待轴停止运动
            /// </summary>
            public override int WaitAxisStop(Guid guid, int timeOut)
            {
                short axisState = 0;
                int rtn = 0;
                int axisNo = GetAxisByGuid(guid);
                HAND axisHand = GetAxisHANDByAxis(axisNo);
                Stopwatch stopWatch = new();
                stopWatch.Start();
                while (true)
                {
                    NMC_MtGetSts(axisHand, ref axisState);
                    if ((axisState & BIT_AXIS_BUSY) == 0)
                        break;
                    if (timeOut > 0)
                    {
                        if (stopWatch.ElapsedMilliseconds > timeOut)
                        {
                            rtn = -1;
                            //throw new Exception($"等待轴运动停止超时，接口：WaitAxisStop");
                            break;
                        }
                    }
                    Thread.Sleep(1);
                }
                return rtn;
            }

            /// <summary>
            /// 清除轴报警
            /// </summary>
            public override int ClearAxisAlarm(Guid guid)
            {
                int axisNo = GetAxisByGuid(guid);
                HAND axisHand = GetAxisHANDByAxis(axisNo);
                /// <summary>
                /// 清除轴错误状态
                /// </summary>
                /// <param name="axisHandle">轴句柄</param>
                /// <returns></returns>
                NMC_MtClrError(axisHand);
                return 0;
            }

            /// <summary>
            /// 轴软限位设定
            /// </summary>
            public override int SetAxisSoftLimit(Guid guid, MotionControlAxisSoftLimit motionAxisSoft)
            {
                int rtn = 0;
                int axisNo = GetAxisByGuid(guid);
                HAND axisHand = GetAxisHANDByAxis(axisNo);
                rtn = NMC_MtSwLmtValue(axisHand, LengthToPulse(axisNo, motionAxisSoft.PositiveLimit), LengthToPulse(axisNo, motionAxisSoft.NegativeLimit));  // 设置软件限位数值
                if (rtn != 0)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_SETSOFTLIMIT_FAIL, MotionErr.SetSoftLimitFail, MotionErr.SetSoftLimitFail);
                }
                rtn = NMC_MtSwLmtOnOff(axisHand, 1);    // 软件限位检查开启
                if (rtn != 0)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_SETSOFTLIMIT_FAIL, MotionErr.SetSoftLimitFail, MotionErr.SetSoftLimitFail);
                }
                return rtn;
            }

            /// <summary>
            /// 轴软限位读取
            /// </summary>
            public override MotionControlAxisSoftLimit GetAxisSoftLimit(int axis)
            {
                HAND axisHand = GetAxisHANDByAxis(axis);
                int posLmt = 0;
                int negLmt = 0;
                NMC_MtGetSwLmtValue(axisHand, ref posLmt, ref negLmt);
                return new MotionControlAxisSoftLimit()
                {
                    NegativeLimit = PulseToLength(axis, negLmt),
                    PositiveLimit = PulseToLength(axis, posLmt)
                };
            }

            /// <summary>
            /// 轴软限位读取
            /// </summary>
            public override MotionControlAxisSoftLimit GetAxisSoftLimit(Guid guid)
            {
                return GetAxisSoftLimit(GetAxisByGuid(guid));
            }

            /// <summary>
            /// 获取单轴规划速度
            /// </summary>
            public override double GetAxisPlanSpeed(int axis)
            {
                double planSpeed = 0;
                HAND axisHand = GetAxisHANDByAxis(axis);
                NMC_MtGetPrfVel(axisHand, ref planSpeed);//规划速度
                return planSpeed;
            }

            /// <summary>
            /// 获取单轴规划速度
            /// </summary>
            public override double GetAxisPlanSpeed(Guid guid)
            {
                return GetAxisPlanSpeed(GetAxisByGuid(guid));
            }

            /// <summary>
            /// 获取电机实际速度
            /// </summary>
            public override double GetAxisActualSpeed(Guid guid)
            {
                return GetAxisActualSpeed(GetAxisByGuid(guid));
            }

            /// <summary>
            /// 获取电机实际速度
            /// </summary>
            public override int GetAxisActualSpeed(int axis)
            {
                int actualSpeed = 0;
                HAND axisHand = GetAxisHANDByAxis(axis);
                NMC_MtGetEncPos(axisHand, ref actualSpeed);//规划速度
                return actualSpeed;
            }

            /// <summary>
            /// 二轴直线插补
            /// </summary>
            public override int TwoAxisLinearInterpolation(Guid[] guid, int coordinateSystemNo, double[] targetPos, int motionType, double startSpeed,
                double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS)
            {
                short rtn = 0;
                if (ArcFeedForwardParameters == null)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_INTERPOLATION_FAIL, MotionErr.InterpolationFail, MotionErr.InterpolationFail);
                }
                if (!ArcFeedForwardParameters.ContainsKey(0))
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_INTERPOLATION_FAIL, MotionErr.InterpolationFail, MotionErr.InterpolationFail);
                }
                if (CoordinateSystemHand == null || CoordinateSystemHand.Length <= coordinateSystemNo)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_INTERPOLATION_FAIL, MotionErr.InterpolationFail, MotionErr.InterpolationFail);
                }

                MotionControlCoordinateSystemParameters crdparam = new MotionControlCoordinateSystemParameters();

                crdparam.Dimension = 2;
                for (int i = 0; i < guid.Length; i++)
                {
                    crdparam.AxisList[i] = (short)GetAxisByGuid(guid[i]);
                }

                crdparam.MaxCompositeSpeed = GC_MAXCRDVEL;
                crdparam.MaxCompositeAcceleration = (motionType == MotionCurveTypeConstants.TCurve ? accTimeT : accTimeS);//(dSynVelMax / 1000) / (dSynAccMax * 1000);//1000; (//由1000改到20 改善停止时异响问题)
                crdparam.OriginFlag = 1;
                crdparam.UserDefinedOrigin[0] = 0;
                crdparam.UserDefinedOrigin[1] = 0;
                try
                {
                    InitialCoordinateSystem(coordinateSystemNo, crdparam);

                    SetArcAngleParam(coordinateSystemNo);

                    ClearBufferData(coordinateSystemNo);

                    int[] targetPosForCard = new int[3];
                    //目标位置信息
                    targetPosForCard[0] = LengthToPulse(crdparam.AxisList[0], targetPos[0]);
                    targetPosForCard[1] = LengthToPulse(crdparam.AxisList[1], targetPos[1]);
                    targetPosForCard[2] = 0;
                    //运动速度(pulse/us)、加速度(pulse/us2)
                    float vel = SpeedMmPerSecToPulsePerMs(crdparam.AxisList[0], maxSpeed);
                    float acc = SpeedMmPerSecToPulsePerMs(crdparam.AxisList[0], maxSpeed / crdparam.MaxCompositeAcceleration);

                    //添加直线运动缓冲区数据
                    rtn = NMC_CrdLineXYZEx(CoordinateSystemHand[coordinateSystemNo], 0, 0x07, targetPosForCard, 0, vel, acc, 0);

                    //启动插补运动
                    StartInterpolationMotion(coordinateSystemNo);

                    //等待插补运动完成
                    WaitContinuousInterpolationMotionFinished(coordinateSystemNo);
                }
                catch (Exception e)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_INTERPOLATION_FAIL, MotionErr.InterpolationFail, MotionErr.InterpolationFail);
                }

                return rtn;
            }

            /// <summary>
            /// 三轴直线插补
            /// </summary>
            public override int ThreeAxisLinearInterpolation(Guid[] guid, int coordinateSystemNo, double[] targetPos, int motionType, double startSpeed,
                double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS)
            {
                short rtn = 0;
                if (ArcFeedForwardParameters == null)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_INTERPOLATION_FAIL, MotionErr.InterpolationFail, MotionErr.InterpolationFail);
                }
                if (!ArcFeedForwardParameters.ContainsKey(0))
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_INTERPOLATION_FAIL, MotionErr.InterpolationFail, MotionErr.InterpolationFail);
                }
                if (CoordinateSystemHand == null || CoordinateSystemHand.Length <= coordinateSystemNo)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_INTERPOLATION_FAIL, MotionErr.InterpolationFail, MotionErr.InterpolationFail);
                }

                MotionControlCoordinateSystemParameters crdparam = new MotionControlCoordinateSystemParameters();

                crdparam.Dimension = 3;

                for (int i = 0; i < guid.Length; i++)
                {
                    crdparam.AxisList[i] = (short)GetAxisByGuid(guid[i]);
                }

                crdparam.MaxCompositeSpeed = GC_MAXCRDVEL;
                crdparam.MaxCompositeAcceleration = (motionType == MotionCurveTypeConstants.TCurve ? accTimeT : accTimeS);//(dSynVelMax / 1000) / (dSynAccMax * 1000);//1000; (//由1000改到20 改善停止时异响问题)
                crdparam.OriginFlag = 1;
                crdparam.UserDefinedOrigin[0] = 0;
                crdparam.UserDefinedOrigin[1] = 0;
                try
                {
                    InitialCoordinateSystem(coordinateSystemNo, crdparam);

                    SetArcAngleParam(coordinateSystemNo);

                    ClearBufferData(coordinateSystemNo);

                    int[] targetPosForCard = new int[3];
                    //目标位置信息
                    targetPosForCard[0] = LengthToPulse(crdparam.AxisList[0], targetPos[0]);
                    targetPosForCard[1] = LengthToPulse(crdparam.AxisList[1], targetPos[1]);
                    targetPosForCard[2] = LengthToPulse(crdparam.AxisList[2], targetPos[2]);
                    //运动速度(pulse/us)、加速度(pulse/us2)
                    float vel = SpeedMmPerSecToPulsePerMs(crdparam.AxisList[0], maxSpeed);
                    float acc = SpeedMmPerSecToPulsePerMs(crdparam.AxisList[0], maxSpeed / crdparam.MaxCompositeAcceleration);

                    //添加直线运动缓冲区数据
                    rtn = NMC_CrdLineXYZEx(CoordinateSystemHand[coordinateSystemNo], 0, 0x07, targetPosForCard, 0, vel, acc, 0);

                    //启动插补运动
                    StartInterpolationMotion(coordinateSystemNo);

                    //等待插补运动完成
                    WaitContinuousInterpolationMotionFinished(coordinateSystemNo);
                }
                catch (Exception e)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_INTERPOLATION_FAIL, MotionErr.InterpolationFail, MotionErr.InterpolationFail);
                }
                return rtn;
            }

            /// <summary>
            /// 二轴圆弧插补
            /// </summary>
            public override int TwoAxisCircularInterpolation(Guid[] guid, int coordinateSystemNo, double[] targetPos, double[] centerCircle, int arcDirection, int motionType,
                double startSpeed, double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS)
            {
                short rtn = 0;
                if (ArcFeedForwardParameters == null)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_INTERPOLATION_FAIL, MotionErr.InterpolationFail, MotionErr.InterpolationFail);
                }
                if (!ArcFeedForwardParameters.ContainsKey(0))
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_INTERPOLATION_FAIL, MotionErr.InterpolationFail, MotionErr.InterpolationFail);
                }
                if (CoordinateSystemHand == null || CoordinateSystemHand.Length <= coordinateSystemNo)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_INTERPOLATION_FAIL, MotionErr.InterpolationFail, MotionErr.InterpolationFail);
                }

                MotionControlCoordinateSystemParameters crdparam = new MotionControlCoordinateSystemParameters();

                crdparam.Dimension = 2;

                for (int i = 0; i < guid.Length; i++)
                {
                    crdparam.AxisList[i] = (short)GetAxisByGuid(guid[i]);
                }

                crdparam.MaxCompositeSpeed = GC_MAXCRDVEL;
                crdparam.MaxCompositeAcceleration = (motionType == MotionCurveTypeConstants.TCurve ? accTimeT : accTimeS);//(dSynVelMax / 1000) / (dSynAccMax * 1000);//1000; (//由1000改到20 改善停止时异响问题)
                crdparam.OriginFlag = 1;
                crdparam.UserDefinedOrigin[0] = 0;
                crdparam.UserDefinedOrigin[1] = 0;
                try
                {
                    InitialCoordinateSystem(coordinateSystemNo, crdparam);

                    SetArcAngleParam(coordinateSystemNo);

                    ClearBufferData(coordinateSystemNo);

                    int[] targetPosForCard = new int[3];
                    int[] centerCircleForCard = new int[3];
                    //目标位置信息
                    targetPosForCard[0] = LengthToPulse(crdparam.AxisList[0], targetPos[0]);
                    targetPosForCard[1] = LengthToPulse(crdparam.AxisList[1], targetPos[1]);
                    targetPosForCard[2] = 0;
                    //圆心位置信息
                    centerCircleForCard[0] = LengthToPulse(crdparam.AxisList[0], centerCircle[0]);
                    centerCircleForCard[1] = LengthToPulse(crdparam.AxisList[1], centerCircle[1]);
                    centerCircleForCard[2] = 0;
                    //运动速度(pulse/us)、加速度(pulse/us2)
                    float vel = SpeedMmPerSecToPulsePerMs(crdparam.AxisList[0], maxSpeed);
                    float acc = SpeedMmPerSecToPulsePerMs(crdparam.AxisList[0], maxSpeed / crdparam.MaxCompositeAcceleration);

                    //添加直线运动缓冲区数据
                    rtn = NMC_CrdArcCenterEx(CoordinateSystemHand[coordinateSystemNo], 0, targetPosForCard, centerCircleForCard, (short)arcDirection, 0, vel, acc, 0);

                    //启动插补运动
                    StartInterpolationMotion(coordinateSystemNo);

                    //等待插补运动完成
                    WaitContinuousInterpolationMotionFinished(coordinateSystemNo);
                }
                catch (Exception e)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_INTERPOLATION_FAIL, MotionErr.InterpolationFail, MotionErr.InterpolationFail);
                }
                return rtn;
            }

            /// <summary>
            /// 三轴圆弧插补
            /// </summary>
            public override int ThreeAxisCircularInterpolation(Guid[] guid, int coordinateSystemNo, double[] targetPos, double[] centerCircle, int arcDirection, int motionType,
                double startSpeed, double maxSpeed, double accTimeT, double decTimeT, double accTimeS, double decTimeS)
            {
                short rtn = 0;
                if (ArcFeedForwardParameters == null)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_INTERPOLATION_FAIL, MotionErr.InterpolationFail, MotionErr.InterpolationFail);
                }
                if (!ArcFeedForwardParameters.ContainsKey(0))
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_INTERPOLATION_FAIL, MotionErr.InterpolationFail, MotionErr.InterpolationFail);
                }
                if (CoordinateSystemHand == null || CoordinateSystemHand.Length <= coordinateSystemNo)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_INTERPOLATION_FAIL, MotionErr.InterpolationFail, MotionErr.InterpolationFail);
                }

                MotionControlCoordinateSystemParameters crdparam = new MotionControlCoordinateSystemParameters();

                crdparam.Dimension = 3;

                for (int i = 0; i < guid.Length; i++)
                {
                    crdparam.AxisList[i] = (short)GetAxisByGuid(guid[i]);
                }

                crdparam.MaxCompositeSpeed = GC_MAXCRDVEL;
                crdparam.MaxCompositeAcceleration = (motionType == MotionCurveTypeConstants.TCurve ? accTimeT : accTimeS);//(dSynVelMax / 1000) / (dSynAccMax * 1000);//1000; (//由1000改到20 改善停止时异响问题)
                crdparam.OriginFlag = 1;
                crdparam.UserDefinedOrigin[0] = 0;
                crdparam.UserDefinedOrigin[1] = 0;
                try
                {
                    InitialCoordinateSystem(coordinateSystemNo, crdparam);

                    SetArcAngleParam(coordinateSystemNo);

                    ClearBufferData(coordinateSystemNo);

                    int[] targetPosForCard = new int[3];
                    int[] centerCircleForCard = new int[3];
                    //目标位置信息
                    targetPosForCard[0] = LengthToPulse(crdparam.AxisList[0], targetPos[0]);
                    targetPosForCard[1] = LengthToPulse(crdparam.AxisList[1], targetPos[1]);
                    targetPosForCard[2] = LengthToPulse(crdparam.AxisList[2], targetPos[2]);
                    //圆心位置信息
                    centerCircleForCard[0] = LengthToPulse(crdparam.AxisList[0], centerCircle[0]);
                    centerCircleForCard[1] = LengthToPulse(crdparam.AxisList[1], centerCircle[1]);
                    centerCircleForCard[2] = LengthToPulse(crdparam.AxisList[2], centerCircle[2]);
                    //运动速度(pulse/us)、加速度(pulse/us2)
                    float vel = SpeedMmPerSecToPulsePerMs(crdparam.AxisList[0], maxSpeed);
                    float acc = SpeedMmPerSecToPulsePerMs(crdparam.AxisList[0], maxSpeed / crdparam.MaxCompositeAcceleration);

                    //添加直线运动缓冲区数据
                    rtn = NMC_CrdArcCenterEx(CoordinateSystemHand[coordinateSystemNo], 0, targetPosForCard, centerCircleForCard, (short)arcDirection, 0, vel, acc, 0);

                    //启动插补运动
                    StartInterpolationMotion(coordinateSystemNo);

                    //等待插补运动完成
                    WaitContinuousInterpolationMotionFinished(coordinateSystemNo);
                }
                catch (Exception e)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_INTERPOLATION_FAIL, MotionErr.InterpolationFail, MotionErr.InterpolationFail);
                }
                return rtn;
            }

            /// <summary>
            /// 设置运动参数
            /// </summary>
            public override int SetAxisMoveParameter(Guid guid, MotionControlArcFeedForwardParameters motionForwardParamter, MotionControlPositionComparison2DParameters motion2DParamter)
            {
                if (PositionComparison2DParameters == null)
                {
                    PositionComparison2DParameters = new Dictionary<int, MotionControlPositionComparison2DParameters>();
                }
                if (ArcFeedForwardParameters == null)
                {
                    ArcFeedForwardParameters = new Dictionary<int, MotionControlArcFeedForwardParameters>();
                }
                if (motion2DParamter.CoordinateSystemId != null)
                {
                    if (PositionComparison2DParameters.ContainsKey(motion2DParamter.CoordinateSystemId.Value))
                    {
                        PositionComparison2DParameters[motion2DParamter.CoordinateSystemId.Value] = motion2DParamter;
                    }
                    else
                    {
                        PositionComparison2DParameters.Add(motion2DParamter.CoordinateSystemId.Value, motion2DParamter);
                    }
                    if (ArcFeedForwardParameters.ContainsKey(motion2DParamter.CoordinateSystemId.Value))
                    {
                        ArcFeedForwardParameters[motion2DParamter.CoordinateSystemId.Value] = motionForwardParamter;
                    }
                    else
                    {
                        ArcFeedForwardParameters.Add(motion2DParamter.CoordinateSystemId.Value, motionForwardParamter);
                    }
                }
                return 0;
            }

            /// <summary>
            /// 读输入状态量
            /// </summary>
            public override bool GetInputState(int ioChannel)
            {
                short divaule = 0;
                short rtn = NMC_GetDIBit(DevHand, (short)ioChannel, ref divaule);
                return (divaule == 0);
            }

            /// <summary>
            /// 读输出状态量
            /// </summary>
            public override bool GetOutputState(int ioChannel)
            {
                short divaule = 0;
                short rtn = NMC_GetDOBit(DevHand, (short)ioChannel, ref divaule);
                return (divaule == 0);
            }

            /// <summary>
            /// 读输入输出状态量
            /// </summary>
            public override bool GetInOutputState(Guid guid)
            {
                return GetOutputState(GetIoByGuid(guid));
            }

            /// <summary>
            /// 写输出状态量
            /// </summary>
            public override int SetOutputState(Guid guid, bool isHave)
            {
                int ioChannel = GetIoByGuid(guid);
                if (isHave)//true 高电平
                {
                    return NMC_SetDOBit(DevHand, (short)ioChannel, 0);
                }
                else //false 低电平
                {
                    //高电平无效
                    return NMC_SetDOBit(DevHand, (short)ioChannel, 1);
                }
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

            /// <summary>
            /// 在线变速
            /// </summary>
            /// <param name="axisGuid">轴锁定句柄</param>
            /// <param name="targetSpeed">目标速度</param>
            void IMotionControlCategoryA.OnlineSpeedChange(Guid axisGuid, double targetSpeed)
            {
                //获取轴号
                int axis = GetAxisByGuid(axisGuid);

                //获取高川轴句柄
                HAND axisHand = GetAxisHANDByAxis(axis);

                //targetSpeed(mm/s)转换成脉冲单位
                int vel = SpeedMmPerSecToPulsePerMs(axis, targetSpeed);

                //NMC_MtSetVel(轴号,速度pulse/ms);
                short rtn = NMC_MtSetVel(axisHand, vel);

                if (rtn != 0)
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_ONLINESPEEDCHANGE_FAIL, MotionErr.OnlineSpeedChangeFail, MotionErr.OnlineSpeedChangeFail);
            }

            /// <summary>
            /// 在线变位
            /// </summary>
            /// <param name="axisGuid">轴锁定句柄</param>
            /// <param name="targetPosition">目标位置</param>
            void IMotionControlCategoryA.OnlineTargetPositionChange(Guid axisGuid, double targetPosition)
            {
                //获取轴号
                int axis = GetAxisByGuid(axisGuid);
                //获取高川轴句柄
                HAND axisHand = GetAxisHANDByAxis(axis);
                //targetPosition转换成脉冲单位
                int pos = LengthToPulse(axis, targetPosition);
                //在线变位
                short rtn = NMC_MtSetPtpTgtPos(axisHand, pos);
                if (rtn != 0)
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_ONLINETARGETPOSITIONCHANGE_FAIL, MotionErr.OnlineTargetPositionChangeFail, MotionErr.OnlineTargetPositionChangeFail);
            }

            /// <summary>
            /// 设置平面2D补偿参数
            /// </summary>
            /// <param name="axisGuidList">轴锁定句柄列表</param>
            /// <param name="compensationParams">平面2D补偿参数</param>
            void IMotionControlCategoryA.Set2DCompensationParameters(Guid[] axisGuidList, MotionControl2DOffsetParameters compensationParams)
            {
                if (compensationParams.OffsetList == null || compensationParams.OffsetList.Length < compensationParams.DataPointsX * compensationParams.DataPointsY)
                {
                    throw new ArgumentException(nameof(compensationParams), "补偿数据点数量不足");
                }
                int axis1 = GetAxisByGuid(axisGuidList[0]);
                int axis2 = GetAxisByGuid(axisGuidList[1]);

                //坐标系索引
                short tableIndex = (short)compensationParams.CompensationCoordinateSystemId;

                //2D补偿Table数据
                T2DCompensationTable t2DCompensationTable = new T2DCompensationTable(true);
                t2DCompensationTable.angle = compensationParams.CalibrationAngle;
                t2DCompensationTable.count[0] = (short)compensationParams.DataPointsX;//x方向数据点个数
                t2DCompensationTable.count[1] = (short)compensationParams.DataPointsY;//y方向数据点个数
                t2DCompensationTable.posBegin[0] = LengthToPulse(axis1, compensationParams.StartOffsetX);//x方向起始补偿位置
                t2DCompensationTable.posBegin[1] = LengthToPulse(axis1, compensationParams.StartOffsetY);//y方向起始补偿位置
                t2DCompensationTable.step[0] = LengthToPulse(axis1, compensationParams.StepOffsetX);//x方向补偿步长
                t2DCompensationTable.step[1] = LengthToPulse(axis1, compensationParams.StepOffsetY);//y方向补偿步长

                //2D补偿实测数据
                TCompData[] compData = new TCompData[compensationParams.DataPointsX * compensationParams.DataPointsY];
                for (int i = 0; i < compensationParams.DataPointsX * compensationParams.DataPointsY; i++)
                {
                    compData[i].xDirComp = LengthToPulse(axis1, compensationParams.OffsetList[i].OffsetX);
                    compData[i].yDirComp = LengthToPulse(axis2, compensationParams.OffsetList[i].OffsetX);
                    compData[i].zDirComp = 0;
                }

                //设置2D补偿参数
                short rtn = NMC_Set2DCompensationTable(DevHand, tableIndex, ref t2DCompensationTable, ref compData[0]);
                if (rtn != 0)
                    throw new Exception($"高川卡，设置2D补偿参数失败,接口：NMC_Set2DCompensationTable，返回值：{rtn}");
            }

            /// <summary>
            /// 平面2D补偿开关
            /// </summary>
            /// <param name="axisGuidList">轴锁定句柄列表</param>
            /// <param name="CompensationCoordinateSystemId">补偿坐标系ID</param>
            /// <param name="isEnabled">是否启用</param>
            void IMotionControlCategoryA.Set2DCompensationEnabled(Guid[] axisGuidList, int CompensationCoordinateSystemId, bool isEnabled)
            {
                if (axisGuidList.Length < 2)
                {
                    throw new ArgumentException(nameof(axisGuidList), "接口Set2DCompensationEnabled参数，轴锁定句柄列表数量不足");
                }
                T2DCompensation t2DCompensation = new T2DCompensation(true);

                int axis1 = GetAxisByGuid(axisGuidList[0]);
                int axis2 = GetAxisByGuid(axisGuidList[1]);

                t2DCompensation.enable = isEnabled ? (short)1 : (short)0;  // 启用/禁用 补偿
                t2DCompensation.tableIndex = (short)CompensationCoordinateSystemId;  //补偿表号
                t2DCompensation.axisType[0] = 0;   //X轴根据规划位置补偿
                t2DCompensation.axisType[1] = 0;   //Y轴根据规划位置补偿
                t2DCompensation.axisIndex[0] = (short)axis1;  //X轴轴号0
                t2DCompensation.axisIndex[1] = (short)axis2;  //Y轴轴号0
                t2DCompensation.axisIndex[2] = -1;//2D不使用Z轴
                short rtn = NMC_Set2DCompensation(DevHand, ref t2DCompensation);
                if (rtn != 0)
                {
                    throw new Exception($"高川卡，设置2D补偿开关失败,接口：NMC_Set2DCompensation，返回值：{rtn}");
                }
            }

            /// <summary>
            /// 获取单点平面2D补偿数据
            /// </summary>
            /// <param name="coordinateSystemId">补偿坐标系ID</param>
            /// <param name="point">补偿点位坐标</param>
            /// <returns>2D补偿量</returns>
            Struct2DOffsetParameters IMotionControlCategoryA.Get2DCompensationParameters(int[] axisList, int coordinateSystemId, Point2D point)
            {
                if (axisList.Length < 2)
                {
                    throw new ArgumentException(nameof(axisList), "接口Get2DCompensationParameters参数，轴列表数量不足");
                }
                Struct2DOffsetParameters OffsetParameters = new Struct2DOffsetParameters();
                int OffsetX = 0;//返回的x方向补偿量(pulse)
                int OffsetY = 0;//返回的y方向补偿量(pulse)
                int OffsetZ = 0;
                OEM_Get2DCompensationPos(DevHand, (short)coordinateSystemId, LengthToPulse(axisList[0], point.X), LengthToPulse(axisList[1], point.Y), ref OffsetX, ref OffsetY, ref OffsetZ);
                OffsetParameters.OffsetX = PulseToLength(axisList[0], OffsetX);
                OffsetParameters.OffsetY = PulseToLength(axisList[1], OffsetY);
                return OffsetParameters;
            }

            /// <summary>
            /// 获取绝对运动移动指令规划时间
            /// </summary>
            /// <param name="axis">轴号</param>
            /// <returns>int:规划时间(ms)</returns>
            private int GetAbsoluteMotionPlanningTime(int axis)
            {
                HAND axisHand = GetAxisHANDByAxis(axis);
                TTrapTime trapTime = new TTrapTime();
                short rtn = NMC_MtGetTrapTime(axisHand, ref trapTime);
                if (rtn != 0)
                {
                    throw new Exception($"高川卡，获取绝对运动移动指令规划时间失败,接口：NMC_MtGetTrapTime，返回值：{rtn}");
                }
                else
                {
                    return (int)trapTime.totalTime;
                }
            }

            /// <summary>
            /// 获取绝对运动移动指令规划时间
            /// </summary>
            /// <param name="axis">轴号</param>
            /// <returns>int:规划时间(ms)</returns>
            int IMotionControlCategoryA.GetAbsoluteMotionPlanningTime(int axis)
            {
                return GetAbsoluteMotionPlanningTime(axis);
            }

            /// <summary>
            /// 获取绝对运动移动指令规划时间
            /// </summary>
            /// <param name="axisGuid">轴锁定句柄</param>
            /// <returns>int:规划时间(ms)</returns>
            int IMotionControlCategoryA.GetAbsoluteMotionPlanningTime(Guid axisGuid)
            {
                return GetAbsoluteMotionPlanningTime(GetAxisByGuid(axisGuid));
            }

            /// <summary>
            /// 设置位置锁存
            /// </summary>
            /// <param name="axisGuid">轴锁定句柄</param>
            /// <param name="positionLatchCaptureLogic">位置锁存捕获逻辑</param>
            /// <param name="channel">位置锁存触发通道号</param>
            /// <param name="positionLatchSignalTriggerMode">位置锁存信号触发模式</param>
            /// <param name="triggerCount">触发次数</param>
            void IMotionControlCategoryA.SetPositionLatch(Guid axisGuid, MotionControlPositionLatchCaptureLogic positionLatchCaptureLogic, int channel, MotionControlPositionLatchSignalTriggerMode positionLatchSignalTriggerMode, short level, int triggerCount)
            {
                //获取轴号
                int axis = GetAxisByGuid(axisGuid);

                //获取高川轴句柄
                HAND axisHand = GetAxisHANDByAxis(axis);

                switch (positionLatchSignalTriggerMode)
                {
                    case MotionControlPositionLatchSignalTriggerMode.RisingEdge:
                        {
                            level = 1;
                            break;
                        }
                    case MotionControlPositionLatchSignalTriggerMode.FallingEdge:
                        {
                            level = 0;
                            break;
                        }
                    case MotionControlPositionLatchSignalTriggerMode.IOLevel:
                        {
                            if (level > 0)
                            {
                                level = 0x00000002;
                            }
                            else
                            {
                                level = 0;
                            }
                            break;
                        }
                    default:
                        break;
                }
                //设置位置锁存重复触发检测次数
                short rtn = NMC_MtSetCaptRepeat(axisHand, (short)triggerCount);
                if (rtn != 0)
                {
                    throw new Exception($"高川卡，设置位置锁存重复触发检测次数,接口：NMC_MtSetCaptRepeat，返回值：{rtn}");
                }
                //设置位置锁存参数
                rtn = NMC_MtSetCaptSns(axisHand, (short)positionLatchCaptureLogic, (short)(CAPT_IO_SRC_DI0 + channel), level);
                if (rtn != 0)
                {
                    throw new Exception($"高川卡，设置位置锁存失败,接口：NMC_MtSetCaptSns，返回值：{rtn}");
                }
            }

            /// <summary>
            /// 位置锁存开关
            /// </summary>
            /// <param name="axisGuid">轴锁定句柄</param>
            /// <param name="isEnabled">是否启用</param>
            void IMotionControlCategoryA.SetPositionLatchEnabled(Guid axisGuid, bool isEnabled)
            {
                //获取轴号
                int axis = GetAxisByGuid(axisGuid);
                //获取高川轴句柄
                HAND axisHand = GetAxisHANDByAxis(axis);
                short rtn = 0;
                if (isEnabled)
                {
                    rtn = NMC_MtSetCapt(axisHand);
                    if (rtn != 0)
                    {
                        throw new Exception($"高川卡，设置位置锁存启用失败,接口：NMC_MtSetCapt，返回值：{rtn}");
                    }
                }
                else
                {
                    rtn = NMC_MtClrCaptSts(axisHand);
                    if (rtn != 0)
                    {
                        throw new Exception($"高川卡，设置位置锁存禁用失败,接口：NMC_MtClrCaptSts，返回值：{rtn}");
                    }
                }
            }

            /// <summary>
            /// 获取位置锁存
            /// </summary>
            /// <param name="axis">轴号</param>
            /// <returns>位置锁存结果</returns>
            ///
            private double[] GetPositionLatchResult(int axis)
            {
                //获取高川轴句柄
                HAND axisHand = GetAxisHANDByAxis(axis);
                short count = 0;
                //获取轴触发位置锁存次数
                short rtn = NMC_MtGetCaptRepeatStatus(axisHand, ref count);
                if (rtn != 0)
                {
                    throw new Exception($"高川卡，获取位置锁存触发次数失败,接口：NMC_MtGetCaptRepeatStatus，返回值：{rtn}");
                }
                //触发次数大于0时获取触发位置
                if (count > 0)
                {
                    int[] positions = new int[count];
                    rtn = NMC_MtGetCaptRepeatPosMulti(axisHand, ref positions[0], 0, count);
                    if (rtn != 0)
                    {
                        throw new Exception($"高川卡，获取位置锁存结果,接口：NMC_MtGetCaptRepeatPosMulti，返回值：{rtn}");
                    }
                    double[] PositionLatchResult = new double[count];
                    for (int i = 0; i < positions.Length; i++)
                    {
                        PositionLatchResult[i] = PulseToLength(axis, positions[i]);
                    }
                    return PositionLatchResult;
                }
                else
                {
                    //未触发返回空
                    return [];
                }
            }

            /// <summary>
            /// 获取位置锁存
            /// </summary>
            /// <param name="axisGuid">轴锁定句柄</param>
            /// <returns>位置锁存结果</returns>
            double[] IMotionControlCategoryA.GetPositionLatchResult(Guid axisGuid)
            {
                return GetPositionLatchResult(GetAxisByGuid(axisGuid));
            }

            /// <summary>
            /// 获取位置锁存
            /// </summary>
            /// <param name="axis">轴号</param>
            /// <returns>位置锁存结果</returns>
            double[] IMotionControlCategoryA.GetPositionLatchResult(int axis)
            {
                return GetPositionLatchResult(axis);
            }

            /// <summary>
            /// 初始化坐标系
            /// </summary>
            /// <param name="coordinateSystemNo">坐标系编号</param>
            /// <param name="coordinateSystemParameters">坐标系参数</param>
            private void InitialCoordinateSystem(int coordinateSystemNo, MotionControlCoordinateSystemParameters coordinateSystemParameters)
            {
                if (CoordinateSystemHand == null)
                {
                    throw new ArgumentNullException(nameof(CoordinateSystemHand), "坐标系个数不能为空");
                }
                if (coordinateSystemNo < 0 || coordinateSystemNo >= SupportCoordinateSystemNum)
                {
                    throw new ArgumentOutOfRangeException(nameof(coordinateSystemNo), "坐标系编号超出范围");
                }
                if (coordinateSystemParameters == null)
                {
                    throw new ArgumentNullException(nameof(coordinateSystemParameters), "坐标系参数不能为空");
                }
                if (coordinateSystemParameters.AxisList == null || coordinateSystemParameters.AxisList.Length < 2)
                {
                    throw new ArgumentException(nameof(coordinateSystemParameters), "坐标系参数轴列表数量不足");
                }
                if (ArcFeedForwardParameters == null)
                {
                    throw new Exception($"高川卡，设置运动参数，ArcFeedForwardParameters为空");
                }
                //两种接口均可打开坐标系
                //short rtn = NMC_CrdOpenEx(DevHand, (short)CoordinateSystemNo, ref CoordinateSystemHand[CoordinateSystemNo]);
                short rtn = NMC_CrdOpen(DevHand, ref CoordinateSystemHand[coordinateSystemNo]);
                if (rtn != 0)
                {
                    throw new Exception($"高川卡，打开坐标系失败,接口：NMC_CrdOpen，返回值：{rtn}");
                }
                //配置坐标系轴
                TCrdConfig crdConfig = new TCrdConfig(true);
                crdConfig.axCnts = (short)coordinateSystemParameters.Dimension;
                for (int i = 0; i < coordinateSystemParameters.Dimension; i++)
                {
                    crdConfig.pAxArray[i] = coordinateSystemParameters.AxisList[i];
                }
                rtn = NMC_CrdConfig(CoordinateSystemHand[coordinateSystemNo], ref crdConfig);
                if (rtn != 0)
                {
                    throw new Exception($"高川卡，配置坐标系轴失败,接口：NMC_CrdConfig，返回值：{rtn}");
                }

                //坐标系参数
                TCrdPara crdPara = new TCrdPara(true);
                crdPara.orgFlag = 1;
                crdPara.synAccMax = coordinateSystemParameters.MaxCompositeAcceleration;
                crdPara.synVelMax = coordinateSystemParameters.MaxCompositeSpeed;
                for (int i = 0; i < coordinateSystemParameters.Dimension; i++)
                {
                    crdPara.offset[i] = coordinateSystemParameters.UserDefinedOrigin[i];
                }
                //设置坐标系参数
                rtn = NMC_CrdSetPara(CoordinateSystemHand[coordinateSystemNo], ref crdPara);
                if (rtn != 0)
                {
                    throw new Exception($"高川卡，设置坐标系参数失败,接口：NMC_CrdSetPara,speed:{crdPara.synVelMax},acc{crdPara.synAccMax},返回值：{rtn}");
                }

                //前瞻参数
                TExtCrdPara extCrdPara = new TExtCrdPara();
                extCrdPara.eventTime = coordinateSystemParameters.EvenTime;
                extCrdPara.T = ArcFeedForwardParameters[coordinateSystemNo].FeedForwardTimeConstant;
                extCrdPara.lookAheadSwitch = 100;
                extCrdPara.smoothDec = GC_MAXSMOOTHDEC;
                extCrdPara.abruptDec = GC_MAXSMOOTHDEC;
                extCrdPara.startVel = 0;
                //设置坐标系前瞻参数
                rtn = NMC_CrdSetExtPara(CoordinateSystemHand[coordinateSystemNo], ref extCrdPara);
                if (rtn != 0)
                {
                    throw new Exception($"高川卡，设置坐标系前瞻参数失败,接口：NMC_CrdSetExtPara，返回值：{rtn}");
                }
                //原本代码中写死了向心加速度10
                //rtn = NMC_CrdSetLookAheadCentriAcc(CoordinateSystemHand[CoordinateSystemNo], 1, 10);
                rtn = NMC_CrdSetLookAheadCentriAcc(CoordinateSystemHand[coordinateSystemNo], 1, ArcFeedForwardParameters[coordinateSystemNo].CentripetalAcceleration);
                if (rtn != 0)
                {
                    throw new Exception($"高川卡，设置坐标系向心加速度失败,接口：NMC_CrdSetLookAheadCentriAcc，返回值：{rtn}");
                }
            }

            /// <summary>
            /// 设置拐角平滑参数
            /// </summary>
            /// <param name="coordinateSystemNo">坐标系编号</param>
            private void SetArcAngleParam(int coordinateSystemNo)
            {
                if (CoordinateSystemHand == null)
                {
                    throw new ArgumentNullException(nameof(CoordinateSystemHand), "坐标系个数不能为空");
                }
                if (CoordinateSystemHand.Length < coordinateSystemNo || coordinateSystemNo < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(coordinateSystemNo), "坐标系编号超出范围");
                }
                short rtn = 0;
                TCrdSPrfPara para_s = new TCrdSPrfPara();
                if (ArcFeedForwardParameters == null)
                {
                    throw new Exception("高川卡，SetArcAngleParam，ArcFeedForwardParameters为空");
                }
                TLookaheadPara lookaheadParam = new TLookaheadPara(true);
                rtn = NMC_CrdGetLookaheadPara(CoordinateSystemHand[coordinateSystemNo], ref lookaheadParam, 0);
                if (rtn != 0)
                {
                    throw new Exception($"高川卡，获取坐标系前瞻参数失败,接口：NMC_CrdGetLookaheadPara，返回值：{rtn}");
                }

                if (ArcFeedForwardParameters[coordinateSystemNo].TrajectoryPreprocessingSwitch == true)
                {
                    //打开轨迹预处理
                    lookaheadParam.preHandleEn = 1;
                    lookaheadParam.preHandleDisAngMin = ArcFeedForwardParameters[coordinateSystemNo].TrajectoryPreprocessingMinAngle;
                    lookaheadParam.preHandleDisAngMax = ArcFeedForwardParameters[coordinateSystemNo].TrajectoryPreprocessingMaxAngle;
                    lookaheadParam.preHandleTol = ArcFeedForwardParameters[coordinateSystemNo].TrajectoryPreprocessingError;
                    lookaheadParam.centAccEn = 1;
                    lookaheadParam.centAcc = ArcFeedForwardParameters[coordinateSystemNo].CentripetalAcceleration;
                }
                else
                {
                    //关闭轨迹预处理
                    lookaheadParam.preHandleEn = 0;
                }
                rtn = NMC_CrdSetLookaheadPara(CoordinateSystemHand[coordinateSystemNo], ref lookaheadParam);
                if (rtn != 0)
                {
                    throw new Exception($"高川卡，设置坐标系前瞻参数失败,接口：NMC_CrdSetLookaheadPara，返回值：{rtn}");
                }
                short pOnOff = 0;
                TCrdSPrfPara spfPara = new TCrdSPrfPara();
                rtn = OEM_CrdGetSPrfPara(CoordinateSystemHand[coordinateSystemNo], ref spfPara, ref pOnOff);
                if (rtn != 0)
                {
                    throw new Exception($"高川卡，获取坐标系S曲线规划参数失败,接口：OEM_CrdGetSPrfPara，返回值：{rtn}");
                }

                //配置拐角平滑参数
                if (ArcFeedForwardParameters[coordinateSystemNo].SmoothingSegmentSwitch == true)
                {
                    para_s.maxJerk = ArcFeedForwardParameters[coordinateSystemNo].SmoothingSegmentAcceleration;
                    para_s.minCrvLen = ArcFeedForwardParameters[coordinateSystemNo].SmoothingSegmentMinLength;//可以设置50，大于50的长度则平滑
                    para_s.ratio = ArcFeedForwardParameters[coordinateSystemNo].SmoothingSegmentRatio;  //0 - 1的范围
                    para_s.usePtpTimeFlag = 0;

                    pOnOff = 1;
                }
                else
                {
                    pOnOff = 0;
                }
                //设置拐角平滑参数
                rtn = OEM_CrdSetSPrfPara(CoordinateSystemHand[coordinateSystemNo], ref para_s, pOnOff);//GC兼容旧版本固件,忽略错误码
                if (rtn != 0)
                {
                    throw new Exception($"高川卡，设置拐角平滑参数失败,接口：OEM_CrdGetSPrfPara，返回值：{rtn}");
                }
            }

            /// <summary>
            /// 停止位置比较输出
            /// </summary>
            /// <param name="coordinateSystemNo">坐标系编号</param>
            private void StopPositionComparisonOutput()
            {
                int rtn = 0;
                rtn = NMC_Comp2DimensOnoff(DevHand, 0, 0, 0/*保留*/);//onOff： 0 停止, 1 输出 2 手动

                rtn = NMC_Comp2DimensOnoff(DevHand, 1, 0, 0/*保留*/);//onOff： 0 停止, 1 输出 2 手动

                StopCompare2DInterruptOutput = true;

                short nDOStatus = 1; //關閉普通DO通道(16/17/18/19)
                rtn = NMC_SetDOBit(DevHand, 16, nDOStatus);

                rtn = NMC_SetDOBit(DevHand, 16 + 1, nDOStatus);

                rtn = NMC_SetDOBit(DevHand, 16 + 2, nDOStatus);

                rtn = NMC_SetDOBit(DevHand, 16 + 3, nDOStatus);
            }

            /// <summary>
            /// 等待编码器到位
            /// </summary>
            /// <param name="axis">轴号</param>
            /// <param name="timeOut">超时时间(默认值2000)(ms)</param>
            private void WaitAxisPosRec(int axis, int timeOut = 2000)
            {
                short rtn = 0;
                HAND axisHand = GetAxisHANDByAxis(axis);
                short axisSts = 0;
                //计时器
                Stopwatch sw = Stopwatch.StartNew();
                while (true)
                {
                    if (ExitLoop)
                    {
                        break;
                    }

                    rtn = NMC_MtGetSts(axisHand, ref axisSts);
                    if (rtn != 0)
                    {
                        throw new Exception($"高川卡，轴{axis}等待编码器到位时获取轴状态失败，接口：NMC_MtGetSts,返回值：{rtn}");
                    }
                    if ((axisSts & BIT_AXIS_POSREC) == BIT_AXIS_POSREC) // bit 1 , 伺服位置到达,步进模式时位置到达,伺服模式时实际位置到达误差限
                        break;
                    if (timeOut > 0)
                    {
                        if (sw.ElapsedMilliseconds > timeOut)
                        {
                            throw new Exception($"高川卡，轴{axis}等待编码器到位超时");
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            /// <summary>
            /// 设置位置比较参数
            /// </summary>
            /// <param name="coordinateSystemNo"></param>
            /// <param name="channel"></param>
            /// <exception cref="ArgumentException"></exception>
            /// <exception cref="ArgumentOutOfRangeException"></exception>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="Exception"></exception>
            private void SetPositionComparisonParam(int coordinateSystemNo, int[] channel)
            {
                if (channel == null)
                {
                    throw new ArgumentException(nameof(channel), "位置比较通道列表数量不足");
                }
                foreach (int i in channel)
                {
                    if (i < 0 || i >= 2)
                    {
                        throw new ArgumentOutOfRangeException(nameof(channel), "位置比较通道编号超出范围");
                    }
                }

                //安全判断
                if (PositionComparison2DParameters == null)
                {
                    throw new InvalidOperationException(nameof(PositionComparison2DParameters) + " 未初始化");
                }
                var param = PositionComparison2DParameters[coordinateSystemNo];
                if (param == null)
                {
                    throw new InvalidOperationException($"PositionComparison2DParameters[{coordinateSystemNo}] 未初始化");
                }
                if (param.AxisList == null || param.AxisList.Length < 1)
                {
                    throw new InvalidOperationException($"PositionComparison2DParameters[{coordinateSystemNo}].AxisList 未初始化或数量不足");
                }

                short rtn = 0;
                for (int i = 0; i < channel.Length; i++)
                {
                    TComp2DimensParamEx compara_2dEx = new TComp2DimensParamEx(true);

                    //设置配置参数
                    compara_2dEx.dir1No = (short)param.AxisList[0];// 方向1 的位置源轴号（0~11）
                    if (param.ComparisonMode == MotionControlPositionComparisonMode.OneDimensional)
                        compara_2dEx.dir2No = -1;// iAxisNo2;										//  方向2 的位置源轴号（0~11）
                    else
                        compara_2dEx.dir2No = (short)param.AxisList[1];                          //  方向2 的位置源轴号（0~11）

                    // 修复 CS8629: 可为 null 的值类型可为 null。
                    // 在使用 param.OutputMode 前先判断是否为 null，否则使用默认值（如 0）
                    compara_2dEx.outputType = (short)param.OutputMode; // 输出方式0：脉冲1：电平
                    if (param.ComparisonSource == 0)
                    {
                        //：0规划
                        compara_2dEx.chnType = 0;                                                   // 通道类型：0 GPO, 1  GATE通道
                        compara_2dEx.outputChn = (short)(16 + channel[i]);                                 //比较输出的通道号，取值[0,n] 不用专用IO输出，用HD0~3 GPO输出 对应IO号16~19
                        compara_2dEx.posSrc = (short)param.ComparisonSource;                           // 轴位置类型 ：0规划1：编码器
                    }
                    else
                    {
                        //1：编码器
                        compara_2dEx.chnType = 1;//0;												// 通道类型：0 GPO, 1  GATE通道
                        compara_2dEx.outputChn = 0;// 16 + iChannel;								//比较输出的通道号，取值[0,n] 不用专用IO输出，用HD0~3 GPO输出 对应IO号16~19
                        compara_2dEx.posSrc = (short)(param.ComparisonSource);                             // 轴位置类型 ：0规划1：编码器
                    }
                    compara_2dEx.gateTime = (short)param.PulseWidth * 1000;                         // 脉冲方式脉冲时间,单位us
                    compara_2dEx.stLevel = (short)param.StartLevel;                           //起始电平低电平  电平模式下的起始电平（0 或 1）

                    compara_2dEx.errZone = (short)param.MaxPositionError;                                                  // 进入比较点容差半径范围（pulse）
                                                                                                                           //compara_2dEx.directOutZone = param.iDirectOutZone; //30;//								// 近距离直接触发范围
                                                                                                                           //compara_2dEx.vibrateRange = param.iVibrateRange;// 20;									// 抖动滤波范围
                    compara_2dEx.minIntervalTime = (short)((param.PulseWidth + param.PulseOffTime) * 1000);   // 最小触发时间间隔,单位us

                    rtn = NMC_Comp2DimensSetParamEx(DevHand, (short)channel[i], ref compara_2dEx, 0);
                    if (rtn != 0)
                    {
                        throw new Exception($"高川卡，设置位置比较参数失败,接口：NMC_Comp2DimensSetParamEx,返回值：{rtn}");
                    }
                }
            }

            /// <summary>
            /// 设置2D位置比较触发点
            /// </summary>
            /// <param name="channel"></param>
            /// <param name="PositionComparisonTriggerPoints"></param>
            private void SetPositionComparisonPoints(int[] axisList, int[] channel, MotionControlPositionComparisonTriggerPoint[] positionComparisonTriggerPoints)
            {
                short rtn = 0;
                int buffSize = positionComparisonTriggerPoints.Length;
                // 每次设置100组数据，X有100组，Y有100组，一共200组
                int[] data = new int[200];

                for (int i = 0; i < channel.Length; i++)
                {
                    //先清空
                    rtn = NMC_Comp2DimensSetData(DevHand, (short)channel[i], data, 0, 0);
                    if (rtn != 0)
                    {
                        throw new Exception($"高川卡，清空位置比较点缓冲区失败,接口：NMC_Comp2DimensClearBuf,返回值：{rtn}");
                    }
                    int RepetCnt = buffSize / 100;
                    int iSub = buffSize % 100;

                    //mm=>pulse
                    //NMC_Comp2DimensSetData 一次只能压入100组数据
                    int j = 0;
                    for (; j < buffSize; j++)
                    {
                        if (j % 100 == 0 && j != 0)
                        {
                            rtn = NMC_Comp2DimensSetData(DevHand, (short)channel[i], data, 100, 0);
                            if (rtn != 0)
                            {
                                throw new Exception($"高川卡，设置位置比较点数据失败,接口：NMC_Comp2DimensSetData,返回值：{rtn}");
                            }
                        }
                        data[(j % 100) * 2] = LengthToPulse(axisList[0], positionComparisonTriggerPoints[j].X);
                        data[(j % 100) * 2 + 1] = LengthToPulse(axisList[1], positionComparisonTriggerPoints[j].Y);
                    }
                    if (j % 100 != 0)
                    {
                        int startIndex = RepetCnt * 200;//计算剩余数组数据索引
                        rtn = NMC_Comp2DimensSetData(DevHand, (short)channel[i], data, (short)(j % 100), 0);
                        if (rtn != 0)
                        {
                            throw new Exception($"高川卡，设置位置比较点数据失败,接口：NMC_Comp2DimensSetData,返回值：{rtn}");
                        }
                    }
                }
            }

            /// <summary>
            /// 启动2D位置比较输出
            /// </summary>
            /// <param name="channel"></param>
            /// <exception cref="ArgumentException"></exception>
            /// <exception cref="ArgumentOutOfRangeException"></exception>
            /// <exception cref="Exception"></exception>
            private void StartPositionComparisonOutput(int[] channel)
            {
                if (channel == null)
                {
                    throw new ArgumentException(nameof(channel), "位置比较通道列表数量不足");
                }
                foreach (int i in channel)
                {
                    if (i < 0 || i >= 2)
                    {
                        throw new ArgumentOutOfRangeException(nameof(channel), "位置比较通道编号超出范围");
                    }
                }
                /*
                iChn 通道号 0：HSIO0，通道号 1：HSIO1
                */
                short rtn = 0;
                for (int i = 0; i < channel.Length; i++)
                {
                    //设置2D位置比较输出开关
                    rtn = NMC_Comp2DimensOnoff(DevHand, (short)channel[i], 1, 0);//onOff： 0 停止, 1 输出 2 手动
                    if (rtn != 0)
                    {
                        throw new Exception($"高川卡，启动位置比较输出失败,接口：NMC_Comp2DimensOnoff,返回值：{rtn}");
                    }
                }
            }

            /// <summary>
            /// 清除缓存区的数据
            /// </summary>
            /// <param name="coordinateSystemNo"></param>
            /// <exception cref="ArgumentNullException"></exception>
            /// <exception cref="Exception"></exception>
            private void ClearBufferData(int coordinateSystemNo)
            {
                if (CoordinateSystemHand == null)
                {
                    throw new ArgumentNullException(nameof(CoordinateSystemHand), "坐标系个数不能为空");
                }
                short rtn;
                //清除坐标系错误状态
                rtn = NMC_CrdClrError(CoordinateSystemHand[coordinateSystemNo]);
                if (rtn != 0)
                {
                    throw new Exception($"高川卡,清除坐标系错误状态,接口：NMC_CrdClrError,返回值：{rtn}");
                }
                //压入指令之前需要清空缓存区
                rtn = NMC_CrdBufClr(CoordinateSystemHand[coordinateSystemNo]);
                if (rtn != 0)
                {
                    throw new Exception($"高川卡,清空缓存区失败,接口：NMC_CrdBufClr,返回值：{rtn}");
                }
            }

            /// <summary>
            /// 添加连续插补缓冲区指令
            /// </summary>
            /// <param name="coordinateSystemNo"></param>
            /// <param name="motionTrajectoryList"></param>
            /// <exception cref="ArgumentNullException"></exception>
            /// <exception cref="ArgumentOutOfRangeException"></exception>
            /// <exception cref="Exception"></exception>
            private void AddBufferData(int coordinateSystemNo, MotionInstructionBase[] motionTrajectoryList)
            {
                if (ArcFeedForwardParameters == null)
                {
                    throw new Exception($"高川卡，添加连续插补缓冲区指令，ArcFeedForwardParameters为空");
                }
                if (CoordinateSystemHand == null)
                {
                    throw new ArgumentNullException(nameof(CoordinateSystemHand), "坐标系个数不能为空");
                }
                if (CoordinateSystemHand.Length < coordinateSystemNo || coordinateSystemNo < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(coordinateSystemNo), "坐标系编号超出范围");
                }

                // 指令返回值变量
                short rtn;
                int space = 0;// 坐标系的缓存区剩余空间查询变量
                int[] targetPos = new int[3];
                int[] midPos = new int[3];
                int[] centerpos = new int[2];
                int segno = 0;
                double acc = 0.0, vel = 0.0;
                for (int i = 0; i < motionTrajectoryList.Length; i++)
                {
                    switch (motionTrajectoryList[i].InstructionType)
                    {
                        case MotionInstructionType.WaitEncInPosition:
                            {
                                //编码器到位等待指令，100ms超时
                                rtn = NMC_CrdBufWaitEncInPosition(CoordinateSystemHand[coordinateSystemNo], segno++, 3, 100);
                                if (rtn != 0)
                                {
                                    throw new Exception($"高川卡，添加等待编码器到位指令失败,接口：NMC_CrdBufWaitEncInPosition,返回值：{rtn}");
                                }
                            }
                            break;

                        case MotionInstructionType.Linear:
                            {
                                if (motionTrajectoryList[i] is not StraightLine straightLine)
                                {
                                    throw new Exception($"高川卡，连续插补运动轨迹{nameof(motionTrajectoryList)}的 InstructionBase 不是 StraightLine 或为 null");
                                }
                                else
                                {
                                    if (straightLine.EndPosition == null || straightLine.EndPosition.Length < 3)
                                    {
                                        throw new Exception($"高川卡，连续插补运动轨迹{nameof(motionTrajectoryList)}的 InstructionBase 的 EndPosition 为空或数量不足");
                                    }
                                    //目标位置信息
                                    targetPos[0] = LengthToPulse(straightLine.EndPosition[0].Axis, straightLine.EndPosition[0].PositionValue);
                                    targetPos[1] = LengthToPulse(straightLine.EndPosition[1].Axis, straightLine.EndPosition[1].PositionValue);
                                    targetPos[2] = LengthToPulse(straightLine.EndPosition[2].Axis, straightLine.EndPosition[2].PositionValue);

                                    //运动速度(pulse/us)、加速度(pulse/us2)
                                    vel = SpeedMmPerSecToPulsePerMs(straightLine.EndPosition[0].Axis, straightLine.Speed);
                                    acc = SpeedMmPerSecToPulsePerMs(straightLine.EndPosition[0].Axis, straightLine.Acceleration);

                                    //添加直线运动缓冲区数据
                                    rtn = NMC_CrdLineXYZEx(CoordinateSystemHand[coordinateSystemNo], segno++, 0x07, targetPos, 0, vel, acc, 0);
                                    if (rtn != 0)
                                    {
                                        throw new Exception($"添加直线运动缓冲区数据失败,接口：NMC_CrdLineXYZEx,返回值：{rtn}");
                                    }
                                }
                            }
                            break;

                        case MotionInstructionType.ArcA:
                            {
                                if (motionTrajectoryList[i] is not ArcA arcA)
                                {
                                    throw new Exception($"高川卡，连续插补运动轨迹{nameof(motionTrajectoryList)}的 InstructionBase 不是 ArcA 或为 null");
                                }
                                else
                                {
                                    if (arcA.EndPosition == null || arcA.EndPosition.Length < 3)
                                    {
                                        throw new Exception($"高川卡，连续插补运动轨迹{nameof(motionTrajectoryList)}的 InstructionBase 的 EndPosition 为空或数量不足");
                                    }
                                    if (arcA.MiddlePosition == null || arcA.MiddlePosition.Length < 3)
                                    {
                                        throw new Exception($"高川卡，连续插补运动轨迹{nameof(motionTrajectoryList)}的 InstructionBase 的 MiddlePosition 为空或数量不足");
                                    }
                                    //目标位置信息
                                    targetPos[0] = LengthToPulse(arcA.EndPosition[0].Axis, arcA.EndPosition[0].PositionValue);
                                    targetPos[1] = LengthToPulse(arcA.EndPosition[0].Axis, arcA.EndPosition[1].PositionValue);
                                    targetPos[2] = LengthToPulse(arcA.EndPosition[0].Axis, arcA.EndPosition[2].PositionValue);
                                    midPos[0] = LengthToPulse(arcA.EndPosition[0].Axis, arcA.MiddlePosition[0].PositionValue);
                                    midPos[1] = LengthToPulse(arcA.EndPosition[0].Axis, arcA.MiddlePosition[1].PositionValue);
                                    midPos[2] = LengthToPulse(arcA.EndPosition[0].Axis, arcA.MiddlePosition[2].PositionValue);
                                    //运动速度(pulse/us)、加速度(pulse/us2)
                                    vel = SpeedMmPerSecToPulsePerMs(arcA.EndPosition[0].Axis, arcA.Speed);
                                    acc = SpeedMmPerSecToPulsePerMs(arcA.EndPosition[0].Axis, arcA.Acceleration);
                                    //添加圆弧运动缓冲区数据
                                    rtn = NMC_CrdArc3DEx(CoordinateSystemHand[coordinateSystemNo], segno++, midPos, targetPos, 0, vel, acc, 0);
                                    if (rtn != 0)
                                    {
                                        throw new Exception($"添加圆弧运动缓冲区数据失败,,接口：NMC_CrdArc3DEx,返回值：{rtn}");
                                    }
                                }
                            }
                            break;

                        case MotionInstructionType.Circle:
                            {
                                if (motionTrajectoryList[i] is not Circle circle)
                                {
                                    throw new Exception($"高川卡，连续插补运动轨迹{nameof(motionTrajectoryList)}的 InstructionBase 不是 Circle 或为 null");
                                }
                                else
                                {
                                    //圆的起点就是结束点
                                    if (circle.EndPosition == null || circle.EndPosition.Length < 3)
                                    {
                                        throw new Exception($"高川卡，连续插补运动轨迹{nameof(motionTrajectoryList)}的 InstructionBase 的 StartPosition 为空或数量不足");
                                    }
                                    if (circle.MiddlePosition == null || circle.MiddlePosition.Length < 3)
                                    {
                                        throw new Exception($"高川卡，连续插补运动轨迹{nameof(motionTrajectoryList)}的 InstructionBase 的 MiddlePosition 为空或数量不足");
                                    }
                                    //目标位置信息
                                    targetPos[0] = LengthToPulse(circle.EndPosition[0].Axis, circle.EndPosition[0].PositionValue);
                                    targetPos[1] = LengthToPulse(circle.EndPosition[0].Axis, circle.EndPosition[1].PositionValue);
                                    targetPos[2] = LengthToPulse(circle.EndPosition[0].Axis, circle.EndPosition[2].PositionValue);
                                    midPos[0] = LengthToPulse(circle.EndPosition[0].Axis, circle.MiddlePosition[0].PositionValue);
                                    midPos[1] = LengthToPulse(circle.EndPosition[0].Axis, circle.MiddlePosition[1].PositionValue);
                                    midPos[2] = LengthToPulse(circle.EndPosition[0].Axis, circle.MiddlePosition[2].PositionValue);
                                    //运动速度(pulse/us)、加速度(pulse/us2)
                                    vel = SpeedMmPerSecToPulsePerMs(circle.EndPosition[0].Axis, circle.Speed);
                                    acc = SpeedMmPerSecToPulsePerMs(circle.EndPosition[0].Axis, circle.Acceleration);
                                    //添加圆弧运动缓冲区数据
                                    rtn = NMC_CrdCircle3DEx(CoordinateSystemHand[coordinateSystemNo], (short)segno, midPos, targetPos, 0, vel, acc, 0);
                                    segno++;
                                    if (rtn != 0)
                                    {
                                        throw new Exception($"添加圆运动缓冲区数据失败,,接口：NMC_CrdCircle3DEx,返回值：{rtn}");
                                    }
                                }
                            }
                            break;

                        case MotionInstructionType.BufferIO:
                            {
                                if (motionTrajectoryList[i] is not BufferIO bufferIO)
                                {
                                    throw new Exception($"高川卡，连续插补运动轨迹{nameof(motionTrajectoryList)}的 InstructionBase 不是 BufferIO 或为 null");
                                }
                                else
                                {
                                    // 缓冲区输出 DO 组定义
                                    //#define CRD_BUFF_DO_MOTOR_ENABLE 1 // 电机使能
                                    //#define CRD_BUFF_DO_MOTOR_CLEAR 2 // 电机报警清除
                                    //#define CRD_BUFF_DO_GPDO1 3 // 通用输出 1
                                    //#define CRD_BUFF_DO_GPDO2 4 // 通用输出 2
                                    //#define CRD_BUFF_DO_EXTDO1 5 // 扩展模块 1
                                    //#define CRD_BUFF_DO_EXTDO2 6 // 扩展模块 2
                                    //#define CRD_BUFF_DO_EXTDO3 7 // 扩展模块 3
                                    //#define CRD_BUFF_DO_EXTDO4 8 // 扩展模块 4
                                    //#define CRD_BUFF_DO_EXTDO5 9 // 扩展模块 5
                                    //#define CRD_BUFF_DO_EXTDO6 10 // 扩展模块 6

                                    rtn = NMC_CrdBufDoEx(CoordinateSystemHand[coordinateSystemNo], segno++, /*motionTrajectoryList[i].bufIOElem.doType*/CRD_BUFF_DO_GPDO1, bufferIO.Channel, (int)bufferIO.Data);
                                    if (rtn != 0)
                                    {
                                        throw new Exception($"高川卡，添加缓冲区IO指令失败,接口：NMC_CrdBufDoEx,返回值：{rtn}");
                                    }
                                }
                            }
                            break;

                        case MotionInstructionType.Delay:
                            {
                                if (motionTrajectoryList[i] is not Delay delay)
                                {
                                    throw new Exception($"高川卡，连续插补运动轨迹{nameof(motionTrajectoryList)}的 InstructionBase 不是 Delay 或为 null");
                                }
                                else
                                {
                                    rtn = NMC_CrdBufDelay(CoordinateSystemHand[coordinateSystemNo], segno++, 0, delay.Duration);
                                    if (rtn != 0)
                                    {
                                        throw new Exception($"高川卡，添加缓冲区IO指令失败,接口：NMC_CrdBufDelay,返回值：{rtn}");
                                    }
                                }
                            }
                            break;

                        case MotionInstructionType.Buf2DComparePulseExElemData:
                            {
                                if (motionTrajectoryList[i] is not Buffer2DComparePulse buffer2DComparePulse)
                                {
                                    throw new Exception($"高川卡，连续插补运动轨迹{nameof(motionTrajectoryList)}的 InstructionBase 不是 Buffer2DComparePulse 或为 null");
                                }
                                else
                                {
                                    //bufferIO点胶硬件配置 点胶信号输出 HD0 即IO口16 motionTrajectoryList[i].buf2DPulse.chn + 16,硬件导致！
                                    rtn = NMC_CrdBufDo(CoordinateSystemHand[coordinateSystemNo], segno++, CRD_BUFF_DO_GPDO1, buffer2DComparePulse.Channel + 16, 1 - buffer2DComparePulse.StartLevel);
                                    if (rtn != 0)
                                    {
                                        throw new Exception($"高川卡，1添加缓冲区IO指令失败,接口：NMC_CrdBufDo,返回值：{rtn}");
                                    }
                                    rtn = NMC_CrdBufDo(CoordinateSystemHand[coordinateSystemNo], segno++, 20, buffer2DComparePulse.Channel, buffer2DComparePulse.StartLevel);
                                    if (rtn != 0)
                                    {
                                        throw new Exception($"高川卡，2添加缓冲区IO指令失败,接口：NMC_CrdBufDo,返回值：{rtn}");
                                    }
                                }
                            }
                            break;

                        default:
                            break;
                    }
                    // 查询坐标系的FIFO0所剩余的空间
                    rtn = NMC_CrdBufGetFree(CoordinateSystemHand[coordinateSystemNo], ref space);
                    if (rtn != 0)
                    {
                        throw new Exception($"查询坐标系的FIFO0所剩余的空间失败，接口：NMC_CrdBufGetFree，返回值{rtn}");
                    }
                    if (space == 0)
                    {
                        throw new Exception($"坐标系的FIFO0所剩余的空间不足！！！");
                    }
                }
            }

            /// <summary>
            /// 启动插补运动
            /// </summary>
            /// <param name="coordinateSystemNo"></param>
            /// <exception cref="ArgumentNullException"></exception>
            /// <exception cref="ArgumentOutOfRangeException"></exception>
            /// <exception cref="Exception"></exception>
            private void StartInterpolationMotion(int coordinateSystemNo)
            {
                if (CoordinateSystemHand == null)
                {
                    throw new ArgumentNullException(nameof(CoordinateSystemHand), "坐标系个数不能为空");
                }
                if (CoordinateSystemHand.Length < coordinateSystemNo || coordinateSystemNo < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(coordinateSystemNo), "坐标系编号超出范围");
                }
                short rtn = 0;

                // 结束数据压入
                rtn = NMC_CrdEndMtn(CoordinateSystemHand[coordinateSystemNo]);
                if (rtn != 0)
                {
                    throw new Exception($"高川卡,启动插补运动，结束数据压入失败，接口：NMC_CrdEndMtn，返回值：{rtn}");
                }
                // 启动缓冲区运动
                rtn = NMC_CrdStartMtn(CoordinateSystemHand[coordinateSystemNo]);
                if (rtn != 0)
                {
                    throw new Exception($"高川卡,启动插补运动，启动缓冲区运动失败，接口：NMC_CrdStartMtn，返回值：{rtn}");
                }
            }

            /// <summary>
            /// 连续插补运动 内部函数
            /// </summary>
            /// <param name="axisGuidList">轴锁定句柄列表</param>
            /// <param name="coordinateSystemNo">坐标系编号</param>
            /// <param name="motionTrajectoryList">运动轨迹列表</param>
            private void ContinuousInterpolationMotion(int coordinateSystemNo, Guid[] axisGuidList, MotionInstructionBase[] motionTrajectoryList)
            {
                try
                {
                    //清除FIFO缓存数据
                    ClearBufferData(coordinateSystemNo);

                    //缓存添加数据
                    AddBufferData(coordinateSystemNo, motionTrajectoryList);

                    //启动插补运动
                    StartInterpolationMotion(coordinateSystemNo);
                }
                catch (Exception e)
                {
                    throw (new Exception($"高川卡，连续插补运动失败，坐标系：{coordinateSystemNo}，错误：{e.Message}"));
                }
            }

            /// <summary>
            /// 等待连续插补完成
            /// </summary>
            /// <param name="coordinateSystemNo"></param>
            /// <exception cref="Exception"></exception>
            private void WaitContinuousInterpolationMotionFinished(int coordinateSystemNo)
            {
                if (CoordinateSystemHand == null)
                {
                    throw new Exception($"高川卡，等待插补运动完成，坐标系句柄为空");
                }
                if (coordinateSystemNo < 0 || CoordinateSystemHand.Length < coordinateSystemNo)
                {
                    throw new Exception($"高川卡，等待插补运动完成，坐标系编号超出范围");
                }
                short crdSts = 0;
                int rtn = 0;
                do
                {
                    if (ExitLoop)
                        break;

                    //获取坐标系运动状态
                    rtn = NMC_CrdGetSts(CoordinateSystemHand[coordinateSystemNo], ref crdSts);

                    //判断坐标系是否运动完成
                    if ((crdSts & BIT_CORD_BUSY) == 0)
                    {
                        // 运动完成
                        break;
                    }
                    Thread.Sleep(1);
                } while (true);
            }

            /// <summary>
            /// 1维位置比较输出
            /// </summary>
            /// <param name="axisGuidList">轴锁定句柄列表</param>
            /// <param name="PositionComparisonTriggerPoints">位置比较触发点列表</param>
            /// <param name="motionTrajectoryList">运动轨迹列表</param>
            private void PositionComparison1D(Guid[] axisGuidList, MotionControlPositionComparisonTriggerPoint[] PositionComparisonTriggerPoints, MotionInstruction[] motionTrajectoryList)
            {
                // 位置比较1D接口未实现
            }

            /// <summary>
            /// 连续插补，电平模式点胶
            /// </summary>
            /// <param name="coordinateSystemNo">坐标系编号</param>
            /// <param name="axisGuidList"></param>
            /// <param name="motionTrajectoryList"></param>
            void IMotionControlCategoryA.ContinuousInterpolationMotion(int coordinateSystemNo, Guid[] axisGuidList, MotionInstructionBase[] motionTrajectoryList)
            {
                if (CoordinateSystemHand == null)
                {
                    throw new Exception($"高川卡，等待插补运动完成，坐标系句柄为空");
                }
                if (coordinateSystemNo < 0 || CoordinateSystemHand.Length < coordinateSystemNo)
                {
                    throw new Exception($"高川卡，等待插补运动完成，坐标系编号超出范围");
                }
                short rtn = 0;

                if (PositionComparison2DParameters == null)
                {
                    throw new Exception($"高川卡，ContinuousInterpolationMotion，PositionComparison2DParameters为空");
                }
                MotionControlPositionComparison2DParameters? param = null;
                if (PositionComparison2DParameters.ContainsKey(coordinateSystemNo))
                {
                    param = PositionComparison2DParameters[coordinateSystemNo];
                }
                if (param == null)
                {
                    throw new Exception($"高川卡，ContinuousInterpolationMotion，PositionComparison2DParameters[{coordinateSystemNo}]为空");
                }
                // 增加非空检查
                if (param.CoordinateSystem == null)
                {
                    throw new Exception($"高川卡，ContinuousInterpolationMotion，PositionComparison2DParameters[{coordinateSystemNo}].CoordinateSystem为空");
                }
                // 增加 ChannelList 非空检查
                if (param.ChannelList == null)
                {
                    throw new Exception($"高川卡，ContinuousInterpolationMotion，PositionComparison2DParameters[{coordinateSystemNo}].ChannelList为空");
                }

                // 建立坐标系
                InitialCoordinateSystem(coordinateSystemNo, param.CoordinateSystem);

                // 设置拐角平滑参数
                SetArcAngleParam(coordinateSystemNo);

                // 打开BuffIO
                short valveIOStatus = 0;
                for (int i = 0; i < param.ChannelList.Length; i++)
                {
                    rtn = NMC_SetPWMPort(CoordinateSystemHand[coordinateSystemNo], 0, (short)param.ChannelList[i], (short)param.ChannelList[i]);
                    rtn = NMC_GetDOBit(CoordinateSystemHand[coordinateSystemNo], (short)(16 + param.ChannelList[i]), ref valveIOStatus);
                }

                // 连续插补运动
                ContinuousInterpolationMotion(coordinateSystemNo, axisGuidList, motionTrajectoryList);

                // 等待连续插补完成
                WaitContinuousInterpolationMotionFinished(coordinateSystemNo);

                // 关闭BuffIO
                valveIOStatus = 1;
                for (int i = 0; i < param.ChannelList.Length; i++)
                {
                    rtn = NMC_SetPWMPort(CoordinateSystemHand[coordinateSystemNo], 0, (short)param.ChannelList[i], (short)param.ChannelList[i]);
                    rtn = NMC_GetDOBit(CoordinateSystemHand[coordinateSystemNo], (short)(16 + param.ChannelList[i]), ref valveIOStatus);
                }
            }

            /// <summary>
            /// 2D位置比较输出(位置比较模式，电平模式手动出胶)
            /// </summary>
            /// <param name="coordinateSystemNo">坐标系编号</param>
            /// <param name="axisGuidList">轴锁定句柄列表</param>
            /// <param name="PositionComparisonTriggerPoints">位置比较触发点列表</param>
            /// <param name="motionTrajectoryList">运动轨迹列表</param>
            /// <remarks>位置比较其他参数是通过SetAxisMoveParameter接口设置的</remarks>
            void IMotionControlCategoryA.PositionComparison2D(int coordinateSystemNo, Guid[] axisGuidList, MotionControlPositionComparisonTriggerPoint[] PositionComparisonTriggerPoints, MotionInstructionBase[] motionTrajectoryList)
            {
                if (PositionComparison2DParameters == null)
                {
                    throw new Exception($"2D位置比较输出接口PositionComparison2D：PositionComparison2DParameters空");
                }
                try
                {
                    MotionControlPositionComparison2DParameters param = PositionComparison2DParameters[coordinateSystemNo];

                    // 增加 ChannelList 非空检查
                    if (param.ChannelList == null)
                    {
                        throw new Exception($"高川卡，PositionComparison2D，PositionComparison2DParameters[{coordinateSystemNo}].ChannelList为空");
                    }

                    // 增加param.CoordinateSystem 非空检查
                    if (param.CoordinateSystem == null)
                    {
                        throw new Exception($"高川卡，PositionComparison2D，PositionComparison2DParameters[{coordinateSystemNo}].CoordinateSystem为空");
                    }
                    // 建立坐标系
                    InitialCoordinateSystem(coordinateSystemNo, param.CoordinateSystem);
                    // 设置拐角平滑参数
                    SetArcAngleParam(coordinateSystemNo);

                    //停止位置比较输出
                    StopPositionComparisonOutput();

                    if (param.ComparisonSource == 1)
                    {
                        // 等待编码器到位
                        WaitAxisPosRec(coordinateSystemNo);
                    }

                    // 设置位置比较参数
                    SetPositionComparisonParam(coordinateSystemNo, param.ChannelList);

                    // 清除空闲的buffer数据
                    ClearBufferData(coordinateSystemNo);

                    // 设置比较点位（绝对位置）
                    int[] axisList = new int[axisGuidList.Length];

                    for (int i = 0; i < axisGuidList.Length; i++)
                    {
                        axisList[i] = GetAxisByGuid(axisGuidList[i]);
                    }
                    SetPositionComparisonPoints(axisList, param.ChannelList, PositionComparisonTriggerPoints);

                    // 启动2D位置比较输出
                    StartPositionComparisonOutput(param.ChannelList);

                    // 启动连续插补运动
                    ContinuousInterpolationMotion(coordinateSystemNo, axisGuidList, motionTrajectoryList);

                    // 等待连续插补完成
                    WaitContinuousInterpolationMotionFinished(coordinateSystemNo);

                    // 停止位置比较输出
                    StopPositionComparisonOutput();
                }
                catch (Exception e)
                {
                    throw new Exception($"2D位置比较输出失败，错误：{e.Message}");
                }
            }

            /// <summary>
            /// 手动位置比较输出
            /// </summary>
            /// <param name="axisGuidList">轴锁定句柄列表</param>
            /// <param name="channel">比较通道号</param>
            /// <param name="startLevel">起始电平信号</param>
            /// <param name="pulseOutputMode">脉冲输出模式</param>
            /// <param name="triggerCount">触发点数</param>
            /// <param name="openTime">开阀时间(ms)</param>
            /// <param name="closeTime">关阀时间</param>
            /// <remarks>channel参数可以传入多个通道，触发时同时触发，适用于双阀，如果是单阀，则channel只包含一个通道</remarks>
            void IMotionControlCategoryA.ManualPositionComparison(Guid[] axisGuidList, int[] channel, short startLevel, int pulseOutputMode, int triggerCount, double openTime, double closeTime)
            {
                short rtn = 0;
                int outIntervalTime = (int)(openTime + closeTime) * 1000;
                StopCompare2DInterruptOutput = false;
                TComp2DimensParamEx compara_2dEx = new TComp2DimensParamEx();
                //设置配置参数
                compara_2dEx.dir1No = 0;
                compara_2dEx.dir2No = 1;//0,1轴比较轴
                compara_2dEx.outputType = 0;
                compara_2dEx.chnType = 0; //GATE 通道   通道类型：0 GPO, 1 GATE 通道
                compara_2dEx.posSrc = 1; //规划位置  轴位置类型 ：0 规划 1：编码器
                compara_2dEx.gateTime = (int)(pulseOutputMode == 0 ? openTime * 1000 : outIntervalTime);
                compara_2dEx.stLevel = 0; //起始电平低电平  电平模式下的起始电平（0 或 1）

                compara_2dEx.errZone = 100;//容差半径
                compara_2dEx.directOutZone = 20;//
                compara_2dEx.vibrateRange = 30;
                compara_2dEx.minIntervalTime = 0;

                //触发次数
                for (int i = 0; i < channel.Length; i++)
                {
                    //设置通道
                    compara_2dEx.outputChn = (short)(16 + channel[i]);

                    //设置比较参数
                    rtn = NMC_Comp2DimensSetParamEx(DevHand, (short)channel[i], ref compara_2dEx, 0);
                    if (rtn != 0)
                    {
                        throw new Exception($"高川卡，手动位置比较输出失败，接口：NMC_Comp2DimensSetParamEx，返回值：{rtn}");
                    }
                    //开始比较
                    rtn = OEM_Comp2DimensOutPulse(DevHand, (short)channel[i], 1, (int)((openTime + closeTime) * 1000), triggerCount, 0);
                    if (rtn != 0)
                    {
                        throw new Exception($"高川卡，手动位置比较输出失败，接口：OEM_Comp2DimensOutPulse，返回值：{rtn}");
                    }
                }
                //等待位置比较输出完成
                for (int i = 0; i < channel.Length; i++)
                {
                    TComp2DimensSts status = new TComp2DimensSts();
                    while (true)
                    {
                        rtn = NMC_Comp2DimensStatusEx(DevHand, (short)channel[i], ref status, 0); //NMC_Comp2DimensStatus
                        if (rtn != 0)
                        {
                            throw new Exception($"高川卡，手动位置比较输出失败，接口：NMC_Comp2DimensStatusEx，返回值：{rtn}");
                        }
                        if (status.manualCount == triggerCount)
                        {
                            break;
                        }
                        if (StopCompare2DInterruptOutput)
                        {
                            rtn = NMC_Comp2DimensOnoff(DevHand, (short)channel[i], 0, 0);
                            if (rtn != 0)
                            {
                                throw new Exception($"高川卡，手动位置比较输出失败，接口：NMC_Comp2DimensOnoff，返回值：{rtn}");
                            }
                            break;
                        }
                    }
                }
            }

            /// <summary>
            /// 停止手动位置比较输出(位置比较模式，电平模式停止手动出胶)
            /// </summary>
            void IMotionControlCategoryA.StopManualPositionComparison()
            {
                //停止位置比较输出
                StopPositionComparisonOutput();
            }

            /// <summary>
            /// 等待坐标系运动完成
            /// </summary>
            /// <param name="coordinateSystemNo"></param>
            /// <param name="timeout"></param>
            /// <returns></returns>
            int IMotionControlBase.WaitCrdMoveDone(int coordinateSystemNo, int timeout)
            {
                if (CoordinateSystemHand == null)
                {
                    throw new Exception($"高川卡，等待插补运动完成，坐标系句柄为空");
                }
                if (coordinateSystemNo < 0 || CoordinateSystemHand.Length < coordinateSystemNo)
                {
                    throw new Exception($"高川卡，等待插补运动完成，坐标系编号超出范围");
                }
                short crdSts = 0;
                int rtn = 0;
                Stopwatch stopWatch = new();
                stopWatch.Start();
                do
                {
                    if (ExitLoop)
                        break;

                    //获取坐标系运动状态
                    rtn = NMC_CrdGetSts(CoordinateSystemHand[coordinateSystemNo], ref crdSts);
                    if (stopWatch.ElapsedMilliseconds > timeout)
                    {
                        return -1;
                    }
                    //判断坐标系是否运动完成
                    if ((crdSts & BIT_CORD_BUSY) == 0)
                    {
                        // 运动完成
                        break;
                    }
                    Thread.Sleep(1);
                } while (true);
                return 0;
            }

            #region IO接口方法定义

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

            static MotionControlGaoChAuto()
            {
                analogChannelList = new ChannelParametersList();
                stateChannelList = new ChannelParametersList();
                for (ushort i = 0; i < 32; i++)
                {
                    stateChannelList.Add(new ChannelParameters() { channelID = createChannelID(EReadWriteMode.ReadOnly, i), channelMode = EReadWriteMode.ReadOnly });
                    stateChannelList.Add(new ChannelParameters() { channelID = createChannelID(EReadWriteMode.ReadWrite, i), channelMode = EReadWriteMode.ReadWrite });
                }
            }
            private static readonly ChannelParametersList analogChannelList;
            private static ChannelParametersList stateChannelList;

            public override void InitIOCard(string initCfg)
            {
                throw new NotSupportedException($"{nameof(MotionControlGaoChAuto)} 不支持通过 InitIOCard 进行独立 IO 初始化。");
            }

            public override void UnInitIOCard()
            {
                throw new NotSupportedException($"{nameof(MotionControlGaoChAuto)} 不支持通过 UnInitIOCard 进行独立 IO 反初始化。");
            }

            public override decimal AnalogRead(string channelID)
            {
                throw new NotSupportedException($"{nameof(MotionControlGaoChAuto)} 暂不支持模拟量读取。");
            }

            public override void AnalogWrite(string channelID, decimal analog)
            {
                throw new NotSupportedException($"{nameof(MotionControlGaoChAuto)} 暂不支持模拟量写入。");
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
                try
                {
                    var guid = LockIO(bitno);
                    SetOutputState(guid, state);
                    UnLockIO(guid);
                }
                catch (Exception ex)
                {
                    using (StreamWriter writer = new StreamWriter("log.txt", true))
                    {
                        writer.WriteLine("StateWrite " + ex.Message);
                    }
                }
            }

            #endregion IO接口方法定义
        }
    }
}