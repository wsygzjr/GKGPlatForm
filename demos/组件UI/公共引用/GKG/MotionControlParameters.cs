/// <summary>
/// 运动控制参数
/// </summary>
namespace GKG
{
    /// <summary>
    /// 位置变化事件参数
    /// </summary>
    public class PositionChangedEventArgs : EventArgs
    {
        public AxisConstantValues[] NewPosition { get; }
        public PositionChangedEventArgs(AxisConstantValues[] newPosition)
        {
            NewPosition = newPosition;
        }
    }

    public class MoveParam
    {
        public int AxisCount;
        public int[] logicAxis;
        public Point3D targetPosition;
        public double speed;
        public double acc;
    }

    /// <summary>
    /// 坐标轴常量定义
    /// </summary>
    public static class AxisConstants
    {
        public const int X = 0;
        public const int Y = 1;
        public const int Z = 2;
        public const int Theta = 3;
        public const int Phi = 4;
        public const int X1 = 5;
        public const int Y1 = 6;
        public const int Z1 = 7;
        public const int Theta1 = 8;
        public const int Phi1 = 9;
    }

    /// <summary>
    /// 坐标轴值结构体
    /// </summary>
    public struct AxisConstantValues
    {
        public int Axis;
        public double PositionValue;
    }

    /// <summary>
    /// 二维坐标结构体
    /// </summary>
    public class Point2D
    {
        public double X;
        public double Y;

        public Point2D()
        {
            X = 0;
            Y = 0;
        }

        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    /// <summary>
    /// 三维坐标结构体
    /// </summary>
    public class Point3D : IEquatable<Point3D>
    {
        public double X;
        public double Y;
        public double Z;

        public Point3D()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point3D(Point3D point)
        {
            X = point.X;
            Y = point.Y;
            Z = point.Z;
        }

        public AxisConstantValues[] ToToAxisConstantValues()
        {
            return new AxisConstantValues[3]
           {
                new AxisConstantValues() { Axis = AxisConstants.X, PositionValue = this.X },
                new AxisConstantValues() { Axis = AxisConstants.Y, PositionValue = this.Y },
                new AxisConstantValues() { Axis = AxisConstants.Z, PositionValue = this.Z }
           };
        }

        public static Point3D operator -(Point3D a, Point3D b)
            => new Point3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public static bool operator ==(Point3D? a, Point3D? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;

            const double eps = 1e-9;
            return Math.Abs(a.X - b.X) < eps
                && Math.Abs(a.Y - b.Y) < eps
                && Math.Abs(a.Z - b.Z) < eps;
        }

        public static bool operator !=(Point3D? a, Point3D? b) => !(a == b);

        public override bool Equals(object? obj) => Equals(obj as Point3D);

        public bool Equals(Point3D? other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;

            const double eps = 1e-9;
            return Math.Abs(X - other.X) < eps
                && Math.Abs(Y - other.Y) < eps
                && Math.Abs(Z - other.Z) < eps;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                hash = hash * 23 + Z.GetHashCode();
                return hash;
            }
        }
    }

    /// <summary>
    /// 坐标系定义
    /// </summary>
    public struct CoordinateSystem
    {
        /// <summary>
        /// 坐标系包含的轴
        /// </summary>
        public int[] AxisArray;
    }

    /// <summary>
    /// 逻辑轴物理轴绑定对象
    /// </summary>
    public struct AxisBinding
    {
        /// <summary>
        /// 运控卡ID
        /// </summary>
        public Guid MotionControlCardId { get; set; }

        /// <summary>
        /// int：逻辑轴编号
        /// </summary>
        public int LogicalAxis { get; set; }

        /// <summary>
        /// int：物理轴编号
        /// </summary>
        public int PhysicalAxis { get; set; }

        public AxisBinding()
        { }

        public AxisBinding(Guid motionControlCardId, int logicalAxis, int physicalAxis)
        {
            MotionControlCardId = motionControlCardId;
            LogicalAxis = logicalAxis;
            PhysicalAxis = physicalAxis;
        }
    }

    #region 状态量参数

    public enum EReadWriteMode
    {
        ReadOnly,
        WriteOnly,
        ReadWrite
    };

    ///<summary>
    ///运控卡状态量初始化参数
    ///</summary>
    public class MotionControlCardStatusParameters
    {
        /// <summary>
        /// string：运控卡ID
        /// </summary>
        public string CardId { get; set; } = string.Empty;

        /// <summary>
        /// int：IO通道号
        /// </summary>
        public int IOChannel { get; set; }

        ///<summary>
        ///读写模式枚举
        ///</summary>
        public EReadWriteMode ReadWriteMode { get; set; }
    }

    ///<summary>
    ///运控卡状态量初始化参数列表：运控卡状态量初始化参数列表
    ///</summary>
    public class MotionControlCardStatusParametersList : List<MotionControlCardStatusParameters>
    {
    }

    #endregion 状态量参数

    #region 运控卡轴运动参数

    /// <summary>
    /// 运控卡轴运动初始化参数
    /// </summary>
    public class MotionControlAxisInitParams
    {
        /// <summary>
        /// string：运控卡ID
        /// </summary>
        public string MotionControlCardId { get; set; } = string.Empty;

        /// <summary>
        /// int：轴号
        /// </summary>
        public int AxisId { get; set; }

        /// <summary>
        /// double：速度
        /// </summary>
        public double Speed { get; set; }
    }

    /// <summary>
    /// 运控卡轴运动初始化参数列表：List<运控卡轴运动初始化参数>
    /// </summary>
    public class MotionControlAxisInitParamList : List<MotionControlAxisInitParams>
    {
    }

    #endregion 运控卡轴运动参数

    /// <summary>
    /// 运动曲线类型常量定义(int)
    /// </summary>
    public static class MotionCurveTypeConstants
    {
        /// <summary>
        /// 0：S曲线
        /// </summary>
        public const int SCurve = 0;

        /// <summary>
        ///  1：T曲线
        /// </summary>
        public const int TCurve = 1;
    }

    /// <summary>
    /// 运控卡轴状态类型枚举
    /// </summary>
    public enum MotionControlAxisStatus
    {
        /// <summary>
        /// 原点信号
        /// </summary>
        Origin = 0,

        /// <summary>
        /// 报警信号
        /// </summary>
        Alarm = 1,

        /// <summary>
        /// 正限位信号
        /// </summary>
        PositiveLimit = 2,

        /// <summary>
        /// 负限位信号
        /// </summary>
        NegativeLimit = 3,

        /// <summary>
        /// Inp信号
        /// </summary>
        Inp = 4,

        /// <summary>
        /// EZ信号
        /// </summary>
        EZ = 5,

        /// <summary>
        /// 使能信号
        /// </summary>
        ServoEnable = 6,

        /// <summary>
        /// 准备完成信号
        /// </summary>
        Ready = 7
    }

    /// <summary>
    /// 轴停止类型常量定义(int)
    /// </summary>
    public enum MotionControlAxisStopTypeConstants
    {
        /// <summary>
        /// 0：立即停止
        /// </summary>
        ImmediateStop = 0,

        /// <summary>
        /// 1：减速停止
        /// </summary>
        DecelerationStop = 1,
    }

    /// <summary>
    /// 回零模式枚举
    /// </summary>
    public enum MotionControlAxisHomingMode
    {
        /// <summary>
        /// 1次原点回零
        /// </summary>
        OnceOriginGoHome = 0,

        /// <summary>
        /// 2次原点回零
        /// </summary>
        TwiceOriginGoHome = 1,

        /// <summary>
        /// 负极限回零
        /// </summary>
        NegativeGoHome = 2,

        /// <summary>
        /// EZ回零
        /// </summary>
        EZGoHome = 3,

        /// <summary>
        /// 找一个EZ停止
        /// </summary>
        FindEZStop = 4,

        /// <summary>
        /// 找一个EZ锁存回找
        /// </summary>
        FindEZLatchBack = 5
    }

    /// <summary
    /// 回零方向常量定义(int)
    /// </summary>
    public static class MotionControlAxisHomingDirectionConstants
    {
        /// <summary>
        /// 0：正方向
        /// </summary>
        public const int PositiveDirection = 1;

        /// <summary>
        /// 1：负方向
        /// </summary>
        public const int NegativeDirection = 0;
    }

    /// <summary>
    /// 回零参数
    /// </summary>
    public class MotionControlAxisHomingParameters
    {
        /// <summary>
        /// 回零模式
        /// </summary>
        public MotionControlAxisHomingMode HomingMode { get; set; }

        /// <summary>
        /// 回零方向
        /// </summary>
        public int HomingDirection { get; set; } = MotionControlAxisHomingDirectionConstants.NegativeDirection;

        /// <summary>
        /// double：回零加速时间
        /// </summary>
        public double HomingAccelerationTime { get; set; }

        /// <summary>
        /// double：回零初速度
        /// </summary>
        public double HomingInitialSpeed { get; set; }

        /// <summary>
        /// double：回零最小速度
        /// </summary>
        public double HomingMinimumSpeed { get; set; }

        /// <summary>
        /// double：回零最大速度
        /// </summary>
        public double HomingMaximumSpeed { get; set; }

        /// <summary>
        /// double：后撤距离
        /// </summary>
        public double HomingRetractDistance { get; set; }
    }

    /// <summary>
    /// 轴位置类型枚举
    /// </summary>
    public enum MotionControlAxisPositionType
    {
        /// <summary>
        /// 指令位置
        /// </summary>
        Command = 0,

        /// <summary>
        /// 实际位置
        /// </summary>
        Actual = 1,

        /// <summary>
        /// 目标位置
        /// </summary>
        Target = 2,

        /// <summary>
        /// 编码器内部位置
        /// </summary>
        EncoderInternal = 3,

        /// <summary>
        /// 轴编码器位置
        /// </summary>
        Encoder = 4,
    }

    /// <summary>
    /// 软限位数据结构
    /// </summary>
    public class MotionControlAxisSoftLimit
    {
        /// <summary>
        /// 正向软限位
        /// </summary>
        public double PositiveLimit { get; set; }

        /// <summary>
        /// 负向软限位
        /// </summary>
        public double NegativeLimit { get; set; }
    }

    /// <summary>
    /// 圆弧方向常量定义(int)
    /// </summary>
    public static class MotionControlArcDirectionConstants
    {
        /// <summary>
        /// 0：顺时针
        /// </summary>
        public const int Clockwise = 0;

        /// <summary>
        /// 1：逆时针
        /// </summary>
        public const int CounterClockwise = 1;
    }

    /// <summary>
    /// 前瞻参数
    /// </summary>
    public class MotionControlArcFeedForwardParameters
    {
        /// <summary>
        /// int：前瞻段数
        /// 一般情况下取200（默认值）
        /// </summary>
        public int FeedForwardSegments { get; set; } = 200;

        /// <summary>
        /// 时间常数
        /// </summary>
        public double FeedForwardTimeConstant { get; set; } = 0.01;

        /// <summary>
        /// double：曲率限制调节参数
        /// </summary>
        public double RadiusRatioSquared { get; set; }

        /// <summary>
        /// double[]：各轴的最大速度
        /// </summary>
        public double[]? VMax { get; set; }

        /// <summary>
        /// double[]：各轴的最大加速度
        /// </summary>
        public double[]? AMax { get; set; }

        /// <summary>
        /// double[]：各轴的最大速度变化量
        /// </summary>
        public double[]? VVariationMax { get; set; }

        /// <summary>
        /// double[]：各轴的脉冲当量
        /// </summary>
        public double[]? PULSE { get; set; }

        /// <summary>
        /// short[]:输入坐标和内部坐标的相对关系
        /// </summary>
        public short[]? AxisRelation { get; set; }

        /// <summary>
        /// string：机床配置文件名
        /// </summary>
        public string? MachineConfigFileName { get; set; }

        /// <summary>
        /// short：轨迹预处理功能开关
        /// </summary>
        public bool? TrajectoryPreprocessingSwitch { get; set; }

        /// <summary>
        /// float：轨迹预处理最小角度，保留尖角
        /// </summary>
        public float TrajectoryPreprocessingMinAngle { get; set; }

        /// <summary>
        /// float：轨迹预处理最大角度
        /// </summary>
        public float TrajectoryPreprocessingMaxAngle { get; set; }

        /// <summary>
        /// float：预处理误差，单位:脉冲
        /// </summary>
        public float TrajectoryPreprocessingError { get; set; }

        /// <summary>
        /// short：是否使用指定的向心加速度，默认为0，即不使用
        /// </summary>
        public short? CentripetalAccelerationSwitch { get; set; } = 0;

        /// <summary>
        /// double：向心加速度
        /// </summary>
        public short CentripetalAcceleration { get; set; } = 10;

        /// <summary>
        /// int：是否启用S平滑段
        /// </summary>
        public bool? SmoothingSegmentSwitch { get; set; } = false;

        /// <summary>
        /// float：S平滑段加速度
        /// </summary>
        public float SmoothingSegmentAcceleration { get; set; }

        private float _smoothingSegmentRatio = 0f;

        /// <summary>
        /// float：S平滑段在整个加减速过程所占的比例,取值范围[0,1],0表示没有S段，1表示全部为S段（无匀加速段）
        /// </summary>
        public float SmoothingSegmentRatio
        {
            get => _smoothingSegmentRatio;
            set
            {
                if (value != 0f && value != 1f)
                {
                    _smoothingSegmentRatio = 0f;
                }
                else
                {
                    _smoothingSegmentRatio = value;
                }
            }
        }

        /// <summary>
        /// float：启用平滑的最小的曲线长度，大于该设定值，则使用S曲线规划，单位:脉冲
        /// </summary>
        public float SmoothingSegmentMinLength { get; set; }
    }

    /// <summary>
    /// 位置比较模式枚举
    /// </summary>
    public enum MotionControlPositionComparisonMode
    {
        /// <summary>
        /// 不启用位置比较
        /// </summary>
        None = 0,

        /// <summary>
        /// 1维比较
        /// </summary>
        OneDimensional = 1,

        /// <summary>
        /// 2维单轴比较
        /// </summary>
        TwoDimensionalSingleAxis = 2,

        /// <summary>
        /// 2维双轴比较
        /// </summary>
        TwoDimensionalDualAxis = 3,
    }

    /// <summary>
    /// 位置比较通道类型枚举
    /// </summary>
    public enum MotionControlPositionComparisonChannelType
    {
        /// <summary>
        /// HSIO通道
        /// </summary>
        HSIO = 0,

        /// <summary>
        /// GPO通道
        /// </summary>
        GPO = 1
    }

    /// <summary>
    /// 运控卡坐标系参数
    /// </summary>
    public class MotionControlCoordinateSystemParameters
    {
        /// <summary>
        /// short：维数
        /// </summary>
        public short Dimension { get; set; } = 3;

        /// <summary>
        /// short[]：轴号列表
        /// </summary>
        public short[] AxisList { get; set; } = new short[] { 0, 1, 2 };

        private double _maxCompositeSpeed = 1000;

        /// <summary>
        /// double：最大合成速度(0, 32767)pulse/ms
        /// </summary>
        public double MaxCompositeSpeed
        {
            get => _maxCompositeSpeed;
            set
            {
                if (value <= 0)
                {
                    _maxCompositeSpeed = 0;
                }
                else if (value > 32767)
                {
                    _maxCompositeSpeed = 32767;
                }
                else
                {
                    _maxCompositeSpeed = value;
                }
            }
        }

        private double _maxCompositeAcceleration = 10000;

        /// <summary>
        /// double：最大合成加速度(0, 32767)pulse/ms2
        /// </summary>
        public double MaxCompositeAcceleration
        {
            get => _maxCompositeAcceleration;
            set
            {
                if (value <= 0)
                {
                    _maxCompositeAcceleration = 0;
                }
                else if (value > 32767)
                {
                    _maxCompositeAcceleration = 32767;
                }
                else
                {
                    _maxCompositeAcceleration = value;
                }
            }
        }

        ///<summary>
        ///short：每个插补段的最小匀速时间 ms
        ///</summary>
        public short EvenTime { get; set; } = 10;

        /// <summary>
        /// short：设置原点坐标值标志,0:默认当前规划位置为原点位置;1:用户指定原点位置
        /// </summary>
        public short OriginFlag { get; set; } = 0;

        /// <summary>
        /// long[]：用户指定的原点位置
        /// </summary>
        public int[] UserDefinedOrigin { get; set; }

        public MotionControlCoordinateSystemParameters()
        {
            Dimension = 3;
            AxisList = new short[] { 0, 1, 2 };
            MaxCompositeSpeed = 1000;
            MaxCompositeAcceleration = 10000;
            EvenTime = 20;
            OriginFlag = 0;
            UserDefinedOrigin = new int[] { 0, 0, 0 };
        }
    }

    /// <summary>
    /// 2D位置比较功能的参数
    /// </summary>
    public class MotionControlPositionComparison2DParameters
    {
        /// <summary>
        /// int[]：轴列表
        /// </summary>
        public int[]? AxisList { get; set; }

        /// <summary>
        /// int[]：位置比较通道号列表
        /// </summary>
        public int[]? ChannelList { get; set; }

        /// <summary>
        /// int：位置比较输出到 GPO 端口时有效，指定通道 0 输出端口
        /// </summary>
        public int? GpoChannel0 { get; set; } = 0;

        /// <summary>
        /// int：位置比较输出到 GPO 端口时有效，指定通道 1 输出端口
        /// </summary>
        public int? GpoChannel1 { get; set; } = 1;

        /// <summary>
        /// int：起始电平（0：高电平，1：低电平）
        /// </summary>
        public int StartLevel { get; set; }

        /// <summary>
        /// int：输出方式（0：脉冲，1：电平）
        /// </summary>
        public int OutputMode { get; set; } = 0;

        /// <summary>
        /// int：脉冲方式时的脉冲时间，单位：us
        /// </summary>
        public int PulseWidth { get; set; }

        /// <summary>
        /// int：关闭脉冲时间,单位：us
        /// </summary>
        public int PulseOffTime { get; set; }

        /// <summary>
        /// int：比较位置到位的最大允许误差，取值范围[0,512)，单位：pulse
        /// </summary>
        public int MaxPositionError { get; set; } = 100;

        /// <summary>
        /// int：比较源（0：规划，1：编码器）
        /// </summary>
        public int ComparisonSource { get; set; } = 0;

        /// <summary>
        /// int：当前空闲FIFO
        /// </summary>
        public int? FreeFIFO { get; set; }

        private int? _coordinateSystemId = 0;

        /// <summary>
        /// int：坐标系编号，[0,1]
        /// </summary>
        public int? CoordinateSystemId
        {
            get => _coordinateSystemId;
            set
            {
                if (value.HasValue)
                {
                    if (value.Value != 0 && value.Value != 1)
                    {
                        _coordinateSystemId = 0;
                    }
                    else
                    {
                        _coordinateSystemId = value;
                    }
                }
                else
                {
                    _coordinateSystemId = 0;
                }
            }
        }

        private int? _anticipationCorneringTime;

        /// <summary>
        /// int：前瞻功能初始化的插补拐弯时间[0,10]单位：ms
        /// </summary>
        public int? AnticipationCorneringTime
        {
            get => _anticipationCorneringTime;
            set
            {
                if (value.HasValue)
                {
                    if (value.Value < 0)
                    {
                        _anticipationCorneringTime = 0;
                    }
                    else if (value.Value > 10)
                    {
                        _anticipationCorneringTime = 10;
                    }
                    else
                    {
                        _anticipationCorneringTime = value;
                    }
                }
                else
                {
                    _anticipationCorneringTime = 0;
                }
            }
        }

        /// <summary>
        /// double：最大加速度，单位：pulse/ms^2
        /// </summary>
        public double MaxAcceleration { get; set; } = 0;

        /// <summary>
        /// bool：是否启用比较
        /// </summary>
        public bool IsComparisonEnabled { get; set; } = false;

        /// <summary>
        /// bool[]：通道启用状态列表
        /// </summary>
        public bool[]? ChannelEnableStatus { get; set; }

        /// <summary>
        /// 位置比较模式枚举：位置比较模式
        /// </summary>
        public MotionControlPositionComparisonMode ComparisonMode { get; set; } = MotionControlPositionComparisonMode.None;

        /// <summary>
        /// 运控卡坐标系参数：运控卡坐标系参数
        /// </summary>
        public MotionControlCoordinateSystemParameters? CoordinateSystem { get; set; }
    }

    /// <summary>
    /// 2D补偿量
    /// </summary>
    public struct Struct2DOffsetParameters
    {
        /// <summary>
        /// double：X方向补偿量
        /// </summary>
        public double OffsetX { get; set; }

        /// <summary>
        /// double：Y方向补偿量
        /// </summary>
        public double OffsetY { get; set; }
    }

    /// <summary>
    /// 平面2D补偿参数
    /// </summary>
    public class MotionControl2DOffsetParameters
    {
        /// <summary>
        /// double：标定坐标系与机械坐标系的夹角，单位：度
        /// </summary>
        public double CalibrationAngle { get; set; }

        /// <summary>
        /// int：X方向的数据点数
        /// </summary>
        public int DataPointsX { get; set; }

        /// <summary>
        /// int：Y方向的数据点数
        /// </summary>
        public int DataPointsY { get; set; }

        /// <summary>
        /// double：X方向的起始补偿位置
        /// </summary>
        public double StartOffsetX { get; set; }

        /// <summary>
        /// double：Y方向的起始补偿位置
        /// </summary>
        public double StartOffsetY { get; set; }

        /// <summary>
        /// double：X方向的补偿步长
        /// </summary>
        public double StepOffsetX { get; set; }

        /// <summary>
        /// double：Y方向的补偿步长
        /// </summary>
        public double StepOffsetY { get; set; }

        /// <summary>
        /// 2D补偿量[]：2D补偿量列表
        /// </summary>
        public Struct2DOffsetParameters[]? OffsetList { get; set; }

        /// <summary>
        /// int：补偿坐标系编号
        /// </summary>
        public int CompensationCoordinateSystemId { get; set; }
    }

    /// <summary>
    /// 位置锁存捕获逻辑枚举
    /// </summary>
    public enum MotionControlPositionLatchCaptureLogic
    {
        /// <summary>
        /// 编码器Z相捕获
        /// </summary>
        EncoderZ,

        /// <summary>
        /// IO捕获
        /// </summary>
        IO,

        /// <summary>
        /// IO+Z相捕获
        /// </summary>
        IOPlusZ,

        /// <summary>
        /// 先IO触发再Z相触发捕获
        /// </summary>
        IOPlusZThenEncoderZ
    }

    /// <summary>
    /// 位置锁存信号触发模式枚举
    /// </summary>
    public enum MotionControlPositionLatchSignalTriggerMode
    {
        /// <summary>
        /// 上升沿触发
        /// </summary>
        RisingEdge,

        /// <summary>
        /// 下降沿触发
        /// </summary>
        FallingEdge,

        /// <summary>
        /// IO电平触发
        /// </summary>
        IOLevel
    }

    /// <summary>
    /// 位置比较触发点
    /// </summary>
    public class MotionControlPositionComparisonTriggerPoint
    {
        /// <summary>
        /// double：X方向比较触发位置
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// double：Y方向比较触发位置
        /// </summary>
        public double Y { get; set; }
    }

    /// <summary>
    /// 脉冲输出模式常量定义(int)
    /// </summary>
    public static class MotionControlPulseOutputModeConstants
    {
        /// <summary>
        /// 0：脉冲模式
        /// </summary>
        public const int Pulse = 0;

        /// <summary>
        /// 1：电平模式
        /// </summary>
        public const int Level = 1;
    }

    /// <summary>
    /// 轴报警信息
    /// </summary>
    public class MotionControlAxisAlarmInfo
    {
        /// <summary>
        /// int：轴号
        /// </summary>
        public const int AxisId = 0;

        /// <summary>
        /// int：运控卡报警编码
        /// </summary>
        public const int MotionCardAlarmCode = 0;

        /// <summary>
        /// bool：报警状态
        /// </summary>
        public const bool AlarmStatus = false;

        /// <summary>
        /// int：报警编码
        /// </summary>
        public const int AlarmCode = 0;
    }

    /// <summary>
    /// 延时类型枚举
    /// </summary>
    public enum MotionControlDelayType
    {
        /// <summary>
        /// 在第1段运动完成后延时
        /// </summary>
        AfterFirstSegmentFinished,

        /// <summary>
        /// 在启动第1段运动后延时
        /// </summary>
        AfterFirstSegmentStarted,

        /// <summary>
        /// 延时后开始第1段运动
        /// </summary>
        AfterDelayStartFirstSegment
    }

    /// <summary>
    /// 脉冲比参数
    /// </summary>
    public class MotionControlPulseRatioParameters
    {
        /// <summary>
        /// int：脉冲比，单位：um/pls
        /// </summary>
        public double PulseRatio { get; set; }

        /// <summary>
        /// double：每转脉冲，单位：um/rev
        /// </summary>
        public double PulsesPerRevolution { get; set; }
    }

    /// <summary>
    /// 脉冲输入模式枚举
    /// </summary>
    public enum MotionControlPulseInputMode
    {
        /// <summary>
        /// 4倍AB相
        /// </summary>
        QuadraAB = 0,

        /// <summary>
        /// 2倍AB相
        /// </summary>
        DoubleAB = 1,

        /// <summary>
        /// 1倍AB相
        /// </summary>
        SingleAB = 2,

        /// <summary>
        /// 双脉冲
        /// </summary>
        DoublePulse = 3,

        /// <summary>
        /// 脉冲+方向
        /// </summary>
        PulseDirection = 4
    }

    /// <summary>
    /// 脉冲输出模式枚举
    /// </summary>
    public enum MotionControlPulseOutputMode
    {
        /// <summary>
        /// 脉冲低+方向低
        /// </summary>
        PulseLowAndDirectionLow,

        /// <summary>
        /// 脉冲高+方向高
        /// </summary>
        PulseHighAndDirectionHigh,

        /// <summary>
        /// 脉冲高+方向低
        /// </summary>
        PulseHighAndDirectionLow,

        /// <summary>
        /// 脉冲低+方向高
        /// </summary>
        PulseLowAndDirectionHigh,

        /// <summary>
        /// 双脉冲高
        /// </summary>
        DoublePulseHigh,

        /// <summary>
        /// 双脉冲低
        /// </summary>
        DoublePulseLow,

        /// <summary>
        /// 模拟量
        /// </summary>
        AnalogPulse,
    }

    /// <summary>
    /// 分段速度参数
    /// </summary>
    public struct MotionControlSegmentedSpeedParameters
    {
        /// <summary>
        /// 距离上限
        /// </summary>
        public double DistanceUpperLimit { get; set; }

        /// <summary>
        /// 距离下限
        /// </summary>
        public double DistanceLowerLimit { get; set; }

        /// <summary>
        /// 速度
        /// </summary>
        /// <remarks>
        /// double：速度，单位：mm/s
        /// </remarks>
        public double Speed { get; set; }

        /// <summary>
        /// 加速度
        /// </summary>
        /// <remarks>
        /// double：加速度，单位：mm/s^2
        /// </remarks>
        public double Acceleration { get; set; }
    }

    /// <summary>
    /// 轴状态逻辑参数
    /// </summary>
    public class MotionControlAxisStateLogicParameters
    {
        /// <summary>
        /// 运控卡轴状态类型
        /// </summary>
        public MotionControlAxisStatus AxisStatus = MotionControlAxisStatus.Origin;

        /// <summary>
        /// 状态是否启用
        /// </summary>
        public bool StateEnable { get; set; } = false;

        /// <summary>
        /// 状态是否取反
        /// </summary>
        public bool StateReverse { get; set; } = false;

        /// <summary>
        /// 感应到信号后的反应（EZ/Alarm/Limit）后反应(0：立即停止，1：减速停止)
        /// </summary>
        public int StopReaction { get; set; } = 0;
    }

    /// <summary>
    /// 编码器类型枚举
    /// </summary>
    public enum EncoderType
    {
        /// <summary>
        /// 无外部编码器
        /// </summary>
        None = 0,

        /// <summary>
        /// 有外部编码器
        /// </summary>
        External = 1
    }

    public class MotionControlFactoryParameter
    {
        /// <summary>
        /// 轴号
        /// </summary>
        public int AxisNo { get; set; }

        /// <summary>
        /// 回零参数
        /// </summary>
        public MotionControlAxisHomingParameters AxisHomingParameters { get; set; } = new MotionControlAxisHomingParameters();

        /// <summary>
        /// 轴状态逻辑参数
        /// </summary>
        public MotionControlAxisStateLogicParameters AxisStateLogicParameters { get; set; } = new MotionControlAxisStateLogicParameters();

        /// <summary>
        /// 脉冲比参数
        /// </summary>
        public MotionControlPulseRatioParameters PulseRatioParameters { get; set; } = new MotionControlPulseRatioParameters();

        /// <summary>
        /// 软限位参数
        /// </summary>
        public MotionControlAxisSoftLimit AxisSoftLimit { get; set; } = new MotionControlAxisSoftLimit();

        /// <summary>
        /// 编码器类型
        /// </summary>
        public EncoderType EncoderType = EncoderType.None;

        /// <summary>
        /// 脉冲输入模式
        /// </summary>
        public MotionControlPulseInputMode PulseInputMode = MotionControlPulseInputMode.PulseDirection;

        /// <summary>
        /// 脉冲输出模式
        /// </summary>
        public int PulseOutputMode = MotionControlPulseOutputModeConstants.Pulse;
    }

    /// <summary>
    /// 运控卡出厂参数
    /// </summary>
    public class MotionControlFactoryParameters
    {
        /// <summary>
        /// 配置文件加载路径
        /// </summary>
        public string ConfigPath { get; set; } = string.Empty;

        /// <summary>
        /// 出厂参数列表
        /// </summary>
        public MotionControlFactoryParameter[]? Parameters;

    }

    public class AxisInformation
    {
        /// <summary>    
        /// 轴号    
        /// </summary>    
        public int AxisNo { get; set; }
        /// <summary>  
        /// 轴名称   
        /// </summary>   
        public string AxisName { get; set; } = string.Empty;
        /// <summary>   
        /// 运控卡的GUID    
        /// </summary>  
        public Guid MotionCardGuid { get; set; }
        /// <summary>
        /// 当前绑定的模组
        /// </summary>
        public string BindingModule { get; set; } = string.Empty;
        /// <summary>
        /// 轴的GUID
        /// </summary>
        public Guid AxisGuid { get; set; }
    }
    public class IOStateInformation
    {
        /// <summary>    
        /// IO名称    
        /// </summary>    
        public string IOName { get; set; } = string.Empty;
        /// <summary>    
        /// 设备的GUID (卡的GUID)   
        /// </summary>   
        public Guid DeviceGuid { get; set; }
        /// <summary>    
        /// IO通道号   
        /// </summary> 
        public string ChannelId { get; set; } = string.Empty;
        /// <summary>   
        /// 通道类型（输入/输出）    
        /// </summary> 
        public EReadWriteMode EReadWriteMode { get; set; }
        /// <summary>    
        /// 状态取反    
        /// </summary>    
        public bool StateReverse { get; set; }
        /// <summary>
        /// IO的GUID
        /// </summary>
        public Guid IOGuid { get; set; }
    }
    public class MotionControlInitializationParameter
    {
    }

    /// <summary>
    /// 运控卡初始化参数
    /// </summary>
    public class MotionControlInitializationParameters
    {
        /// <summary>
        /// 初始化参数列表
        /// </summary>
        public MotionControlInitializationParameter[]? Parameters;

        /// <summary>
        /// 配置文件加载路径
        /// </summary>
        public string ConfigPath { get; set; } = string.Empty;
    }
}