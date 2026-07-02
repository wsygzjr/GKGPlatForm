namespace GKG.SubMM.Dispenser
{
    /// <summary>
    /// 称重类型枚举
    /// </summary>
    public enum WeighingType
    {
        /// <summary>
        /// 单点称重
        /// </summary>
        SinglePointWeighing = 0,

        /// <summary>
        /// 流量称重
        /// </summary>
        MassFlowWeighing = 1
    }

    /// <summary>
    /// 单点称重参数
    /// </summary>
    public class SinglePointWeighingParams
    {
        /// <summary>
        /// 喷胶点数
        /// </summary>
        public int PointCount { get; set; }

        /// <summary>
        /// 喷胶次数
        /// </summary>
        public int SprayCount { get; set; }

        /// <summary>
        /// int：单次间隔（s）
        /// </summary>
        public double OnceInterval { get; set; }

        /// <summary>
        /// 重复次数
        /// </summary>
        public int CycleCount { get; set; }

        public double dDotUpperLimit { get; set; }
        public double dDotLowerLimit { get; set; }
    }

    /// <summary>
    /// 流量称重参数
    /// </summary>
    public class MassFlowWeighingParams
    {
        /// <summary>
        /// 喷胶点数
        /// </summary>
        public int PointCount { get; set; }

        /// <summary>
        /// 喷胶次数
        /// </summary>
        public int SprayCount { get; set; }

        /// <summary>
        /// int：单次间隔（s）
        /// </summary>
        public double OnceInterval { get; set; }

        /// <summary>
        /// 重复次数
        /// </summary>
        public int CycleCount { get; set; }
    }

    /// <summary>
    /// 定量称重参数
    /// </summary>
    public class QuantitativeWeighingParameters
    {
        /// <summary>
        /// 称重模式
        /// </summary>
        public WeighingType WeighingMode { get; set; }

        /// <summary>
        /// 喷胶重量(mg)
        /// </summary>
        public double QuantitativeWeight { get; set; }

        /// <summary>
        /// 喷胶次数
        /// </summary>
        public int SprayCount { get; set; }

        /// <summary>
        /// int：单次间隔（s）
        /// </summary>
        public double OnceInterval { get; set; }

        /// <summary>
        /// 重复次数
        /// </summary>
        public int CycleCount { get; set; }

        /// <summary>
        /// 总重量(mg)
        /// </summary>
        public double TotalWeight { get; set; }

        /// <summary>
        /// N倍出胶系数
        /// </summary>
        public double dNRadioWeight { get; set; }

        /// <summary>
        /// 上限
        /// </summary>
        public double dUpperLimit { get; set; }

        /// <summary>
        /// 下限
        /// </summary>
        public double dLowerLimit { get; set; }
    }

    /// <summary>
    /// 称重动作参数
    /// </summary>
    public class WeighingActionParameters
    {
        /// <summary>
        /// 回抹开关
        /// </summary>
        public bool BackRollingEnabled { get; set; } = false;

        /// <summary>
        /// 三维坐标：称重位置
        /// </summary>
        public Point3D WeighingPosition { get; set; } = new Point3D();

        /// <summary>
        /// 回抹次数
        /// </summary>
        public int BackRollingCount { get; set; }

        /// <summary>
        /// 回抹距离（mm）
        /// </summary>
        public double BackRollingDistance { get; set; }

        /// <summary>
        /// 回抹速度（mm/s）
        /// </summary>
        public double BackRollingSpeed { get; set; }

        /// <summary>
        /// 回抹加速度（mm/s2）
        /// </summary>
        public double BackRollingAcc { get; set; }
    }

    /// <summary>
    /// 定时顶点称重时间项
    /// </summary>
    public class WeighingTimeItem
    {
        /// <summary>
        /// 时
        /// </summary>
        public int Hour;

        /// <summary>
        /// 分
        /// </summary>
        public int Minute;
    }

    /// <summary>
    /// 定时定点称重时间表
    /// </summary>
    public class WeighingTimeTable
    {
        /// <summary>
        /// 是否开启
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 定时定点称重时间项
        /// </summary>
        public WeighingTimeItem[] WeighingTimeItems { get; set; }
    }

    public class WeighingParameter
    {
        /// <summary>
        /// 功能头编号
        /// </summary>
        public string FunctionHeadID { get; set; }

        /// <summary>
        /// 称重模式
        /// </summary>
        public WeighingType WeighingMode { get; set; }

        /// <summary>
        /// 称重开关
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 单点称重参数
        /// </summary>
        public SinglePointWeighingParams SinglePointWeighingParams { get; set; }

        /// <summary>
        /// 流量称重参数
        /// </summary>
        public MassFlowWeighingParams MassFlowWeighingParams { get; set; }

        /// <summary>
        /// 定量称重参数
        /// </summary>
        public QuantitativeWeighingParameters QuantitativeWeighingParameters { get; set; }



        /// <summary>
        /// 称重补偿百分比
        /// </summary>
        public double WeightCorrectionPercent { get; set; }

        /// <summary>
        /// 称重位置
        /// </summary>
        public Point3D WeighingPosition { get; set; }

        /// <summary>
        /// 位置教导模式
        /// </summary>
        public PositionTeachMode PositionMode { get; set; }

        /// <summary>
        /// 提前开阀时间
        /// </summary>
        public int PreOpenValveTime { get; set; }

        /// <summary>
        /// 定时定点称重时间表
        /// </summary>
        public WeighingTimeTable WeighingTimeTable { get; set; }

        /// <summary>
        /// 基准单点重量（单位：mg）
        /// </summary>
        public double WeightReference { get; set; }

        public WeighingParameter()
        {
            FunctionHeadID = "";
            SinglePointWeighingParams = new SinglePointWeighingParams();
            MassFlowWeighingParams = new MassFlowWeighingParams();
            QuantitativeWeighingParameters = new QuantitativeWeighingParameters();
            WeighingPosition = new Point3D();
            WeighingTimeTable = new WeighingTimeTable();
        }
    }

    public class WeighingParameterArray : List<WeighingParameter>
    {
        public WeighingParameter Find(string ValveID)
        {
            foreach (var value in this)
            {
                if (value.FunctionHeadID == ValveID)
                    return value;
            }
            throw new GKGException(1, "", "");
        }
    }
}