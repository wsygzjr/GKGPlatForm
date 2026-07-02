namespace GKG
{
    ///<summary>
    ///运动轨迹结构(json数据)
    ///</summary>
    public class MotionInstruction
    {
        /// <summary>
        /// 运动指令类型
        /// </summary>
        public MotionInstructionType InstructionType { get; set; }

        /// <summary>
        /// 运动指令类型数据基类:运动指令类型对应结构
        /// </summary>
        public byte[]? InstructionData { get; set; }
    }

    /// <summary>
    /// 运控常量
    /// </summary>
    public static class MotionConstants
    {
        public static double Teach_ZAxisTestFirstLaserVel = 5;                                    //校正Z轴自动第一次测高速度
        public static double Teach_ZAxisTestSecondLaserVel = 0.1;									//校正Z轴自动第二次测高速度
    }

    /// <summary>
    /// 激光测高正方向枚举
    /// </summary>
    public enum MeasureHeightPositiveDir
    {
        Up,
        Down
    }

    public enum MeasureHeightType
    {
        /// <summary>
        /// 深视智能SD33
        /// </summary>
        SSZNSD33
    }

    public enum WeighingBalanceType
    {
        /// <summary>
        /// 梅特勒
        /// </summary>
        APW
    }

    public enum MotionCardType
    {
        /// <summary>
        /// 雷赛1000B
        /// </summary>
        DMC1000B,

        /// <summary>
        /// 高川8轴卡
        /// </summary>
        GC800,

        /// <summary>
        /// GKG自制运动控制卡
        /// </summary>
        GMCMINI,
    }

    /// <summary>
    /// 位置教导模式
    /// </summary>
    public enum PositionTeachMode
    {
        /// <summary>
        /// 相机教导
        /// </summary>
        CCDPosition,

        /// <summary>
        /// 功能头教导
        /// </summary>
        FunctionHeadPosition
    }
}