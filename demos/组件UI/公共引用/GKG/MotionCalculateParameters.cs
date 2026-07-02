using GF_Gereric;

/// <summary>
/// 运动计算参数
/// </summary>
namespace GKG
{
    #region 工艺参数

    /// <summary>
    /// 计算类型常量定义
    /// </summary>
    public static class CalculationTypeConstants
    {
        /// <summary>
        /// 标准
        /// </summary>
        public const int General = 0;

        /// <summary>
        /// 点胶
        /// </summary>
        public const int Dispense = 1;
    }

    #region 非加工轨迹

    /// <summary>
    /// 非加工轨迹工艺参数结构体
    /// </summary>
    public struct NonProcessingTrajectoryParameters
    {
        /// <summary>
        /// 起始速度
        /// </summary>
        public double StartSpeed;

        /// <summary>
        /// 最大速度
        /// </summary>
        public double MaxSpeed;

        /// <summary>
        /// 加速度
        /// </summary>
        public double Acceleration;

        /// <summary>
        /// 减速度
        /// </summary>
        public double Deceleration;
    }

    #endregion 非加工轨迹

    #region 加工轨迹

    /// <summary>
    /// 加工前后工艺参数结构体
    /// </summary>
    public struct PrePostProcessingParameters
    {
        /// <summary>
        /// 内部工艺参数
        /// </summary>
        public byte[] InternalParameters;

        /// <summary>
        /// 扩展参数
        /// </summary>
        public byte[] ExtendedParameters;
    }

    /// <summary>
    /// 加工中工艺参数结构体
    /// </summary>
    public struct InProcessingParameters
    {
        /// <summary>
        /// 加工速度
        /// </summary>
        public double ProcessingSpeed;

        /// <summary>
        /// 扩展参数
        /// </summary>
        public byte[] ExtendedParameters;
    }

    #region 点胶

    /// <summary>
    /// 点胶加工前后工艺扩展参数（线）
    /// </summary>
    public struct DispensePrePostProcessingParameters
    {
        /// <summary>
        /// 是否点胶
        /// </summary>
        public bool IsDispense;

        /// <summary>
        /// 提前距离
        /// </summary>
        public double AdvanceDistance;

        /// <summary>
        /// 提前开阀距离
        /// </summary>
        public double AdvanceOpenDistance;

        /// <summary>
        /// 提前开阀时间
        /// </summary>
        public double AdvanceOpenTime;

        /// <summary>
        /// 延后距离
        /// </summary>
        public double DelayDistance;

        /// <summary>
        /// 延后关阀距离
        /// </summary>
        public double DelayCloseDistance;

        /// <summary>
        /// 回走距离
        /// </summary>
        public double GoBackDistance;

        /// <summary>
        /// 回走速度
        /// </summary>
        public double GoBackSpeed;

        /// <summary>
        /// 回走高度
        /// </summary>
        public double GoBackHeight;

        /// <summary>
        /// 回抬高度
        /// </summary>
        public double GoLiftHeight;

        /// <summary>
        /// 回抬速度
        /// </summary>
        public double GoLiftSpeed;
    }

    /// <summary>
    /// 点胶加工前后工艺扩展参数（点）
    /// </summary>
    public struct DotPrePostProcessingParameters
    {
        /// <summary>
        /// 稳定时间
        /// </summary>
        public double StabilizationTime { get; set; }

        /// <summary>
        /// 点胶高度
        /// </summary>
        public double DispensingHeight { get; set; }

        /// <summary>
        /// 提前开阀时间
        /// </summary>
        public double PreOpenValveTime { get; set; }

        /// <summary>
        /// 停留时间
        /// </summary>
        public double DwellTime { get; set; }

        /// <summary>
        /// 回抬高度
        /// </summary>
        public double GoLiftHeight { get; set; }

        /// <summary>
        /// 回抬速度
        /// </summary>
        public double GoLiftSpeed { get; set; }
    }

    /// <summary>
    /// 点胶加工中工艺扩展参数
    /// </summary>
    public struct DispenseInProcessingParameters
    {
        /// <summary>
        /// 点胶高度
        /// </summary>
        public double DispenseHeight;
    }

    ///<summary>
    ///内部工艺参数
    /// </summary>
    public struct DispenseInternalParameters
    {
        /// <summary>
        /// 点胶高度
        /// </summary>
        public byte[] InternalParameters;
    }

    public enum MultiValveSpindle
    {
        /// <summary>
        /// 多阀主轴X方向
        /// </summary>
        X,

        /// <summary>
        /// 多阀主轴Y方向
        /// </summary>
        Y
    }

    #endregion 点胶

    #endregion 加工轨迹

    #endregion 工艺参数

    #region 运动计算

    #region 运动计算输入

    /// <summary>
    /// 计算轨迹类型枚举
    /// </summary>
    public enum MotionTrajectoryType
    {
        /// <summary>
        /// 线计算轨迹类型
        /// </summary>
        Linear,

        /// <summary>
        /// 点计算轨迹类型
        /// </summary>
        Dot
    }

    /// <summary>
    /// 胶量计算单位
    /// </summary>
    public enum GlueWeightUnit
    {
       mg,
       mg_mm,
       dot,
       mm_s,
    }

    /// <summary>
    /// 计算轨迹基类
    /// </summary>
    public abstract class MotionTrajectoryBase
    {
        /// <summary>
        /// 计算轨迹类型
        /// </summary>
        public abstract MotionTrajectoryType TrajectoryType { get; }
    }

    /// <summary>
    /// 线计算轨迹
    /// </summary>
    public class LinearMotionTrajectory : MotionTrajectoryBase
    {
        /// <summary>
        /// 计算轨迹类型
        /// </summary>
        public override MotionTrajectoryType TrajectoryType => MotionTrajectoryType.Linear;

        /// <summary>
        /// 起始点坐标
        /// </summary>
        public AxisConstantValues[]? StartPoint { get; set; }

        /// <summary>
        /// 计算轨迹项列表
        /// </summary>
        public LinearMotionTrajectoryItem[]? LinearMotionTrajectoryItems { get; set; }
    }

    /// <summary>
    /// 点计算轨迹
    /// </summary>
    public class DotMotionTrajectory : MotionTrajectoryBase
    {
        /// <summary>
        /// 计算轨迹类型
        /// </summary>
        public override MotionTrajectoryType TrajectoryType => MotionTrajectoryType.Dot;

        /// <summary>
        /// 目标点坐标
        /// </summary>
        public MotionTrajectoryPoint? TargetPoint { get; set; }
    }

    #region 计算轨迹项类型数据结构

    /// <summary>
    /// 线计算轨迹项类型枚举
    /// </summary>
    public enum LinearMotionTrajectoryItemType
    {
        /// <summary>
        /// 直线
        /// </summary>
        StraightLine,

        /// <summary>
        /// 圆弧A
        /// </summary>
        ArcA,

        /// <summary>
        /// 圆弧B
        /// </summary>
        ArcB,

        /// <summary>
        /// 圆A
        /// </summary>
        CircleA
    }

    /// <summary>
    /// 线计算轨迹点结构
    /// </summary>
    public class MotionTrajectoryPoint
    {
        /// <summary>
        /// 点位
        /// </summary>
        public AxisConstantValues[]? Position { get; set; }

        /// <summary>
        /// 加工中样式
        /// </summary>
        public InProcessingParameters? InProcessingParameters { get; set; }
    }

    /// <summary>
    /// 线计算轨迹项类型数据基类
    /// </summary>
    public abstract class LinearMotionTrajectoryItemBase
    {
        /// <summary>
        /// 加工中工艺参数：工艺参数
        /// </summary>
        public InProcessingParameters? InProcessingParameters { get; set; }
    }

    /// <summary>
    /// 直线
    /// </summary>
    public class LinearMotionTrajectoryItemStraightLine : LinearMotionTrajectoryItemBase
    {
        /// <summary>
        /// 坐标轴值[]：终点坐标
        /// </summary>
        public MotionTrajectoryPoint? EndPoint { get; set; }
    }

    /// <summary>
    /// 圆弧A
    /// </summary>
    public class LinearMotionTrajectoryItemArcA : LinearMotionTrajectoryItemBase
    {
        /// <summary>
        /// 坐标轴值[]：中间点坐标
        /// </summary>
        public MotionTrajectoryPoint? MiddlePoint { get; set; }

        /// <summary>
        /// 坐标轴值[]：终点坐标
        /// </summary>
        public MotionTrajectoryPoint? EndPoint { get; set; }
    }

    /// <summary>
    /// 圆弧B
    /// </summary>
    public class LinearMotionTrajectoryItemArcB : LinearMotionTrajectoryItemBase
    {
        /// <summary>
        /// 坐标轴值[]：圆心坐标
        /// </summary>
        public MotionTrajectoryPoint? CenterPoint { get; set; }

        /// <summary>
        /// 坐标轴值[]：终点坐标
        /// </summary>
        public MotionTrajectoryPoint? EndPoint { get; set; }
    }

    /// <summary>
    /// 圆
    /// </summary>
    public class LinearMotionTrajectoryItemCircleA : LinearMotionTrajectoryItemBase
    {
        /// <summary>
        /// 坐标轴值[]：中间点坐标
        /// </summary>
        public MotionTrajectoryPoint? MiddlePoint { get; set; }

        /// <summary>
        /// 坐标轴值[]：终点坐标
        /// </summary>
        public MotionTrajectoryPoint? EndPoint { get; set; }
    }

    /// <summary>
    /// 线计算轨迹项结构
    /// </summary>
    public class LinearMotionTrajectoryItem
    {
        /// <summary>
        /// 轨迹项类型
        /// </summary>
        public LinearMotionTrajectoryItemType ItemType { get; set; }

        /// <summary>
        /// 线计算轨迹类型数据基类:线计算轨迹类型对应结构
        /// </summary>
        public LinearMotionTrajectoryItemBase? ItemBase { get; set; }
    }

    #endregion 计算轨迹项类型数据结构

    /// <summary>
    /// 产品的加工轨迹项
    /// </summary>
    public abstract class ProductProcessingTrajectoryItem
    {
        /// <summary>
        /// 是否加工轨迹
        /// </summary>
        public abstract bool IsProcessing { get; }
    }

    /// <summary>
    /// 加工轨迹
    /// </summary>
    public class ProcessingTrajectory : ProductProcessingTrajectoryItem
    {
        /// <summary>
        /// 是否加工轨迹
        /// </summary>
        public override bool IsProcessing => true;

        /// <summary>
        /// 计算轨迹基类：计算轨迹
        /// </summary>
        public MotionTrajectoryBase? MotionTrajectory { get; set; }

        /// <summary>
        /// 加工前后工艺参数：工艺参数
        /// </summary>
        public PrePostProcessingParameters? PrePostProcessingParameters { get; set; }
    }

    ///<summary>
    /// 非加工轨迹
    /// </summary>
    public class NonProcessingTrajectory : ProductProcessingTrajectoryItem
    {
        /// <summary>
        /// 是否加工轨迹
        /// </summary>
        public override bool IsProcessing => false;

        /// <summary>
        /// 计算轨迹基类：计算轨迹
        /// </summary>
        public MotionTrajectoryBase? MotionTrajectory { get; set; }

        /// <summary>
        /// 非加工轨迹工艺参数：工艺参数
        /// </summary>
        public NonProcessingTrajectoryParameters? NonProcessingParameters { get; set; }
    }

    ///<summary>
    ///运动计算参数
    ///</summary>
    public class MotionCalculationParameters
    {
        /// <summary>
        /// 空间结构[]：产品的障碍参数
        /// </summary>
        public SpaceStructure[]? BarrierParametersOfTheProduct { get; set; }

        /// <summary>
        /// 产品的加工轨迹项[]：产品的加工轨迹
        /// </summary>
        public ProductProcessingTrajectoryItem[]? ProductProcessingTrajectory { get; set; }
    }

    /// <summary>
    /// 机器行程参数
    /// </summary>
    public class MachineTravelParameters
    {
        ///<summary>
        ///坐标轴
        ///</summary>
        private int iAxis { get; set; }

        /// <summary>
        /// 原点值
        /// </summary>
        public double Origin { get; set; }

        /// <summary>
        /// 极限值
        /// </summary>
        public double Limit { get; set; }
    }

    #endregion 运动计算输入

    #region 运动计算输出

    #region 运动指令类型数据结构

    /// <summary>
    /// 运动指令类型枚举
    /// </summary>
    public enum MotionInstructionType
    {
        /// <summary>
        /// 点
        /// </summary>
        Point,

        /// <summary>
        /// 直线
        /// </summary>
        Linear,

        /// <summary>
        /// 圆弧A
        /// </summary>
        ArcA,

        /// <summary>
        /// 圆弧B
        /// </summary>
        ArcB,

        /// <summary>
        /// 圆
        /// </summary>
        Circle,

        /// <summary>
        /// 延时
        /// </summary>
        Delay,

        /// <summary>
        /// BufferIO通道
        /// </summary>
        BufferIO,

        /// <summary>
        /// 等待编码器到位
        /// </summary>
        WaitEncInPosition,

        /// <summary>
        /// 2D位置比较脉冲输出
        /// </summary>
        Buf2DComparePulseExElemData,

        /// <summary>
        /// 连续运动
        /// </summary>
        ContinueMove,

        /// <summary>
        /// 相对运动
        /// </summary>
        RelativeMove,

        /// <summary>
        /// 停止连续运动
        /// </summary>
        StopContinueMove,

        /// <summary>
        /// 2D位置比较输出（复合指令）
        /// </summary>
        PositionComparison2D
    }

    /// <summary>
    /// 运动指令类型数据基类
    /// </summary>
    public abstract class MotionInstructionBase
    {
        public MotionInstructionType InstructionType
        {
            get { return getInstructionType(); }
        }

        protected abstract MotionInstructionType getInstructionType();

        public byte[] ToBytes()
        {
            return toBytes();
        }

        public void FromBytes(byte[] data)
        {
            fromBytes(data);
        }

        protected abstract byte[] toBytes();

        protected abstract void fromBytes(byte[] data);

        public static MotionInstructionBase Create(MotionInstructionType motionInstructionType)
        {
            switch (motionInstructionType)
            {
                case MotionInstructionType.Linear:
                    return new StraightLine();

                case MotionInstructionType.ArcA:
                    return new ArcA();

                case MotionInstructionType.ArcB:
                    return new ArcB();

                case MotionInstructionType.Circle:
                    return new Circle();

                case MotionInstructionType.BufferIO:
                    return new BufferIO();

                case MotionInstructionType.Delay:
                    return new Delay();

                case MotionInstructionType.WaitEncInPosition:
                    throw new NotImplementedException("WaitEncInPosition is not implemented.");
                case MotionInstructionType.Buf2DComparePulseExElemData:
                    return new Buffer2DComparePulse();

                case MotionInstructionType.ContinueMove:
                    return new ContinueMoveInstruction();

                case MotionInstructionType.RelativeMove:
                    return new RelativeMoveInstruction();

                case MotionInstructionType.StopContinueMove:
                    return new StopContinueMoveInstruction();

                case MotionInstructionType.PositionComparison2D:
                    return new PositionComparison2DInstruction();

                default:
                    throw new NotImplementedException("WaitEncInPosition is not implemented.");
            }
        }

        public static MotionInstructionBase Create(MotionInstructionType motionInstructionType, byte[] data)
        {
            MotionInstructionBase obj = Create(motionInstructionType);
            obj.FromBytes(data);
            return obj;
        }
    }

    /// <summary>
    /// 点位：运动指令类型数据基类
    /// </summary>
    public class Point : MotionInstructionBase
    {
        /// <summary>
        /// 坐标轴值[]：目标点坐标
        /// </summary>
        public AxisConstantValues[]? TargetPosition { get; set; }

        /// <summary>
        /// double：速度
        /// </summary>
        public double Speed { get; set; }

        /// <summary>
        /// double：加速度
        /// </summary>
        public double Acceleration { get; set; }

        protected override void fromBytes(byte[] data)
        {
            (this.TargetPosition, this.Speed, this.Acceleration) = JsonObjConvert.FromJSonBytes<Point>(data)
                is var temp ? (temp.TargetPosition, temp.Speed, temp.Acceleration) : (null, 0, 0);
        }

        protected override MotionInstructionType getInstructionType()
        {
            return MotionInstructionType.Point;
        }

        protected override byte[] toBytes()
        {
            return JsonObjConvert.ToJSonBytes(this);
        }
    }

    ///<summary>
    ///直线：运动指令类型数据基类
    /// </summary>
    public class StraightLine : MotionInstructionBase
    {
        ///<summary>
        ///坐标轴值[]：起点坐标
        ///</summary>
        public AxisConstantValues[]? StartPosition { get; set; }

        ///<summary>
        ///坐标轴值[]：终点坐标
        ///</summary>
        public AxisConstantValues[]? EndPosition { get; set; }

        /// <summary>
        /// double：速度
        /// </summary>
        public double Speed { get; set; }

        /// <summary>
        /// double：加速度
        /// </summary>
        public double Acceleration { get; set; }

        protected override void fromBytes(byte[] data)
        {
            (this.StartPosition, this.EndPosition, this.Speed, this.Acceleration) = JsonObjConvert.FromJSonBytes<StraightLine>(data)
                is var temp ? (temp.StartPosition, temp.EndPosition, temp.Speed, temp.Acceleration) : (null, null, 0, 0);
        }

        protected override MotionInstructionType getInstructionType()
        {
            return MotionInstructionType.Linear;
        }

        protected override byte[] toBytes()
        {
            return JsonObjConvert.ToJSonBytes(this);
        }
    }

    /// <summary>
    /// 圆弧A：运动指令类型数据基类
    /// </summary>
    public class ArcA : MotionInstructionBase
    {
        /// <summary>
        /// 坐标轴值[]：起点坐标
        /// </summary>
        public AxisConstantValues[]? StartPosition { get; set; }

        ///<summary>
        ///坐标轴值[]：中间点坐标
        ///</summary>
        public AxisConstantValues[]? MiddlePosition { get; set; }

        /// <summary>
        /// 坐标轴值[]：终点坐标
        /// </summary>
        public AxisConstantValues[]? EndPosition { get; set; }

        /// <summary>
        /// double：速度
        /// </summary>
        public double Speed { get; set; }

        /// <summary>
        /// double：加速度
        /// </summary>
        public double Acceleration { get; set; }

        protected override void fromBytes(byte[] data)
        {
            (this.StartPosition, this.MiddlePosition, this.EndPosition, this.Speed, this.Acceleration) = JsonObjConvert.FromJSonBytes<ArcA>(data)
                is var temp ? (temp.StartPosition, temp.MiddlePosition, temp.EndPosition, temp.Speed, temp.Acceleration) : (null, null, null, 0, 0);
        }

        protected override MotionInstructionType getInstructionType()
        {
            return MotionInstructionType.ArcA;
        }

        protected override byte[] toBytes()
        {
            return JsonObjConvert.ToJSonBytes(this);
        }
    }

    ///<summary>
    ///圆弧B：运动指令类型数据基类
    ///</summary>
    public class ArcB : MotionInstructionBase
    {
        /// <summary>
        /// 坐标轴值[]：起点坐标
        /// </summary>
        public AxisConstantValues[]? StartPosition { get; set; }

        /// <summary>
        /// 坐标轴值[]：圆心坐标
        /// </summary>
        public AxisConstantValues[]? CenterPosition { get; set; }

        /// <summary>
        /// 坐标轴值[]：终点坐标
        /// </summary>
        public AxisConstantValues[]? EndPosition { get; set; }

        /// <summary>
        /// double：速度
        /// </summary>
        public double Speed { get; set; }

        /// <summary>
        /// double：加速度
        /// </summary>
        public double Acceleration { get; set; }

        protected override void fromBytes(byte[] data)
        {
            (this.StartPosition, this.CenterPosition, this.EndPosition, this.Speed, this.Acceleration) = JsonObjConvert.FromJSonBytes<ArcB>(data)
                is var temp ? (temp.StartPosition, temp.CenterPosition, temp.EndPosition, temp.Speed, temp.Acceleration) : (null, null, null, 0, 0);
        }

        protected override MotionInstructionType getInstructionType()
        {
            return MotionInstructionType.ArcB;
        }

        protected override byte[] toBytes()
        {
            return JsonObjConvert.ToJSonBytes(this);
        }
    }

    ///<summary>
    ///圆：运动指令类型数据基类
    ///</summary>
    public class Circle : MotionInstructionBase
    {
        /// <summary>
        /// 坐标轴值[]：中间点坐标
        /// </summary>
        public AxisConstantValues[]? MiddlePosition { get; set; }

        /// <summary>
        /// 坐标轴值[]：起点坐标（结束点坐标）
        /// </summary>
        public AxisConstantValues[]? EndPosition { get; set; }

        /// <summary>
        /// double：速度
        /// </summary>
        public double Speed { get; set; }

        /// <summary>
        /// double：加速度
        /// </summary>
        public double Acceleration { get; set; }

        protected override void fromBytes(byte[] data)
        {
            (this.MiddlePosition, this.EndPosition, this.Speed, this.Acceleration) = JsonObjConvert.FromJSonBytes<Circle>(data)
                is var temp ? (temp.MiddlePosition, temp.EndPosition, temp.Speed, temp.Acceleration) : (null, null, 0, 0);
        }

        protected override MotionInstructionType getInstructionType()
        {
            return MotionInstructionType.Circle;
        }

        protected override byte[] toBytes()
        {
            return JsonObjConvert.ToJSonBytes(this);
        }
    }

    ///<summary>
    ///延时：运动指令类型数据基类
    ///</summary>
    public class Delay : MotionInstructionBase
    {
        /// <summary>
        /// double：延时时间
        /// </summary>
        public int Duration { get; set; }

        protected override void fromBytes(byte[] data)
        {
            this.Duration = JsonObjConvert.FromJSonBytes<Delay>(data) is var temp ? temp.Duration : 0;
        }

        protected override MotionInstructionType getInstructionType()
        {
            return MotionInstructionType.Delay;
        }

        protected override byte[] toBytes()
        {
            return JsonObjConvert.ToJSonBytes(this);
        }
    }

    ///<summary>
    ///IO通道：运动指令类型数据基类
    ///</summary>
    public class BufferIO : MotionInstructionBase
    {
        /// <summary>
        /// int：通道号
        /// </summary>
        public int Channel { get; set; }

        /// <summary>
        /// double：数据
        /// </summary>
        public double Data { get; set; }

        protected override void fromBytes(byte[] data)
        {
            (this.Channel, this.Data) = JsonObjConvert.FromJSonBytes<BufferIO>(data)
                is var temp ? (temp.Channel, temp.Data) : (0, 0);
        }

        protected override MotionInstructionType getInstructionType()
        {
            return MotionInstructionType.BufferIO;
        }

        protected override byte[] toBytes()
        {
            return JsonObjConvert.ToJSonBytes(this);
        }
    }

    /// <summary>
    /// 2D位置比较脉冲输出指令：运动指令类型数据基类
    /// </summary>
    public class Buffer2DComparePulse : MotionInstructionBase
    {
        /// <summary>
        /// 通道号
        /// </summary>
        public short Channel { get; set; }

        /// <summary>
        /// 起始电平
        /// </summary>
        public short StartLevel { get; set; }

        protected override void fromBytes(byte[] data)
        {
            (this.Channel, this.StartLevel) = JsonObjConvert.FromJSonBytes<Buffer2DComparePulse>(data)
                is var temp ? (temp.Channel, temp.StartLevel) : ((short)0, (short)0);
        }

        protected override MotionInstructionType getInstructionType()
        {
            return MotionInstructionType.Buf2DComparePulseExElemData;
        }

        protected override byte[] toBytes()
        {
            return JsonObjConvert.ToJSonBytes(this);
        }
    };

    /// <summary>
    /// 连续运动指令。
    /// </summary>
    public class ContinueMoveInstruction : MotionInstructionBase
    {
        public MoveParam? MoveParam { get; set; }

        protected override void fromBytes(byte[] data)
        {
            MoveParam = JsonObjConvert.FromJSonBytes<ContinueMoveInstruction>(data)?.MoveParam;
        }

        protected override MotionInstructionType getInstructionType()
        {
            return MotionInstructionType.ContinueMove;
        }

        protected override byte[] toBytes()
        {
            return JsonObjConvert.ToJSonBytes(this);
        }
    }

    /// <summary>
    /// 相对运动指令。
    /// </summary>
    public class RelativeMoveInstruction : MotionInstructionBase
    {
        public MoveParam? MoveParam { get; set; }

        protected override void fromBytes(byte[] data)
        {
            MoveParam = JsonObjConvert.FromJSonBytes<RelativeMoveInstruction>(data)?.MoveParam;
        }

        protected override MotionInstructionType getInstructionType()
        {
            return MotionInstructionType.RelativeMove;
        }

        protected override byte[] toBytes()
        {
            return JsonObjConvert.ToJSonBytes(this);
        }
    }

    /// <summary>
    /// 停止连续运动指令。
    /// </summary>
    public class StopContinueMoveInstruction : MotionInstructionBase
    {
        public int LogicAxis { get; set; }

        protected override void fromBytes(byte[] data)
        {
            LogicAxis = JsonObjConvert.FromJSonBytes<StopContinueMoveInstruction>(data)?.LogicAxis ?? 0;
        }

        protected override MotionInstructionType getInstructionType()
        {
            return MotionInstructionType.StopContinueMove;
        }

        protected override byte[] toBytes()
        {
            return JsonObjConvert.ToJSonBytes(this);
        }
    }

    /// <summary>
    /// 2D位置比较输出复合指令。
    /// 指令内部自带位置比较点位与要执行的连续轨迹。
    /// </summary>
    public class PositionComparison2DInstruction : MotionInstructionBase
    {
        public MotionInstructionBase[] Instructions { get; set; } = Array.Empty<MotionInstructionBase>();

        public MotionControlPositionComparisonTriggerPoint[] PositionComparisonTriggerPoints { get; set; } = Array.Empty<MotionControlPositionComparisonTriggerPoint>();

        private class PositionComparison2DInstructionDto
        {
            public MotionInstructionPacket[] Instructions { get; set; } = Array.Empty<MotionInstructionPacket>();

            public MotionControlPositionComparisonTriggerPoint[] PositionComparisonTriggerPoints { get; set; } = Array.Empty<MotionControlPositionComparisonTriggerPoint>();
        }

        private class MotionInstructionPacket
        {
            public MotionInstructionType InstructionType { get; set; }

            public byte[] Data { get; set; } = Array.Empty<byte>();
        }

        protected override void fromBytes(byte[] data)
        {
            var dto = JsonObjConvert.FromJSonBytes<PositionComparison2DInstructionDto>(data);
            if (dto == null)
            {
                Instructions = Array.Empty<MotionInstructionBase>();
                PositionComparisonTriggerPoints = Array.Empty<MotionControlPositionComparisonTriggerPoint>();
                return;
            }

            PositionComparisonTriggerPoints = dto.PositionComparisonTriggerPoints ?? Array.Empty<MotionControlPositionComparisonTriggerPoint>();
            Instructions = (dto.Instructions ?? Array.Empty<MotionInstructionPacket>())
                .Select(p => MotionInstructionBase.Create(p.InstructionType, p.Data))
                .ToArray();
        }

        protected override MotionInstructionType getInstructionType()
        {
            return MotionInstructionType.PositionComparison2D;
        }

        protected override byte[] toBytes()
        {
            var dto = new PositionComparison2DInstructionDto
            {
                PositionComparisonTriggerPoints = PositionComparisonTriggerPoints ?? Array.Empty<MotionControlPositionComparisonTriggerPoint>(),
                Instructions = (Instructions ?? Array.Empty<MotionInstructionBase>())
                    .Select(i => new MotionInstructionPacket
                    {
                        InstructionType = i.InstructionType,
                        Data = i.ToBytes()
                    })
                    .ToArray()
            };
            return JsonObjConvert.ToJSonBytes(dto);
        }
    }

    #endregion 运动指令类型数据结构

    /// <summary>
    /// 轨迹序列类型常量（与 Robot 侧 MotionInstructionSequenceType 的取值保持一致）。
    /// </summary>
    public static class MotionTrajectorySequenceTypeConstants
    {
        public const int StepByStep = 0;
        public const int ContinuousInterpolation = 1;
        public const int PositionComparison2D = 2;
        public const int ManualPositionComparison = 3;
        public const int Calibration = 4;
        public const int RobotCustom = 100;
    }

    ///<summary>
    ///轨迹结构
    ///</summary>
    public class MotionTrajectory
    {
        ///<summary>
        /// 运动轨迹结构[]：运动轨迹
        /// </summary>
        public MotionInstructionBase[]? MotionInstructions { get; set; }

        /// <summary>
        /// 指令序列执行语义。
        /// MotionCalculator 输出应显式声明该值，以便与 Robot.Execute 的 SequenceType 对齐。
        /// 值定义见 MotionTrajectorySequenceTypeConstants。
        /// </summary>
        public int SequenceType { get; set; } = MotionTrajectorySequenceTypeConstants.StepByStep;

        /// <summary>
        /// 序列级扩展参数。
        /// </summary>
        public byte[]? ExtendedParameters { get; set; }

        /// <summary>
        /// 非加工轨迹工艺参数结构体：工艺参数
        /// </summary>
        public NonProcessingTrajectoryParameters? ControlParameters { get; set; }
    }

    #endregion 运动计算输出

    /// <summary>
    /// 空间结构类型枚举
    /// </summary>
    public enum SpaceStructureType
    {
        /// <summary>
        /// 立方体
        /// </summary>
        Cube,

        /// <summary>
        /// 圆柱
        /// </summary>
        Cylindrical,

        /// <summary>
        /// 球体
        /// </summary>
        Sphere
    }

    ///<summary>
    ///空间结构类型数据基类
    /// </summary>
    public abstract class SpaceStructureBase
    {
    }

    /// <summary>
    /// 立方体：空间结构类型数据基类
    /// </summary>
    public class Cube : SpaceStructureBase
    {
        /// <summary>
        /// 坐标轴值[]：前左下顶点坐标
        /// </summary>
        public AxisConstantValues[]? FrontLeftBottomVertex { get; set; }

        /// <summary>
        /// 坐标轴值[]：后左下顶点坐标
        /// </summary>
        public AxisConstantValues[]? BackLeftBottomVertex { get; set; }

        /// <summary>
        /// 坐标轴值[]：后右上顶点坐标
        /// </summary>
        public AxisConstantValues[]? BackRightTopVertex { get; set; }
    }

    /// <summary>
    /// 圆柱体：空间结构类型数据基类
    /// </summary>
    public class Cylindrical : SpaceStructureBase
    {
        /// <summary>
        /// 坐标轴值[]：底面圆点坐标
        /// </summary>
        public AxisConstantValues[]? BaseCenterPoint { get; set; }

        /// <summary>
        /// 坐标轴值[]：顶面圆点坐标
        /// </summary>
        public AxisConstantValues[]? TopCenterPoint { get; set; }

        /// <summary>
        /// 坐标轴值[]：底面半径
        /// </summary>
    }

    /// <summary>
    /// 球体：空间结构类型数据基类
    /// </summary>
    public class Sphere : SpaceStructureBase
    {
        /// <summary>
        /// 坐标轴值[]：球心点坐标
        /// </summary>
        public AxisConstantValues[]? CenterPoint { get; set; }

        /// <summary>
        /// 坐标轴值[]：半径
        /// </summary>
        public double Radius { get; set; }
    }

    /// <summary>
    /// 空间结构
    /// </summary>
    public class SpaceStructure
    {
        /// <summary>
        /// 空间结构类型
        /// </summary>
        public SpaceStructureType StructureType { get; set; }

        /// <summary>
        /// 空间结构类型数据基类:空间结构类型对应结构
        /// </summary>
        public SpaceStructureBase? StructureBase { get; set; }
    }

    #endregion 运动计算
}