namespace GKG
{
    /// <summary>
    /// 轨道运转模式枚举
    /// </summary>
    public enum ERailOperatingMode
    {
        /// <summary>
        /// 独占模式
        /// </summary>
        ExclusiveMode,

        /// <summary>
        /// 共享模式
        /// </summary>
        SharedMode,
    }

    /// <summary>
    /// 轨道工作模式枚举
    /// </summary>
    public enum ERailWorkMode
    {
        LeftInRightOut,
        RightInLeftOut,
        LeftInLeftOut,
        RightInRightOut,
    }

    public enum ETransMode
    {
        Normal,
        TwoSpeed,
    }

    /// <summary>
    /// 工位电气初始化参数结构
    /// </summary>
    public class WorkStationEleInitParams
    {
        /// <summary>
        /// 左挡板气缸参数
        /// </summary>
        public byte[]? LeftBlkCylinderParams { get; set; }

        /// <summary>
        /// 右挡板气缸参数
        /// </summary>
        public byte[]? RightBlkCylinderParams { get; set; }

        /// <summary>
        /// 左到位感应器参数
        /// </summary>
        public byte[]? LeftInPositionParams { get; set; }

        /// <summary>
        /// 右到位感应器参数
        /// </summary>
        public byte[]? RightInPositionParams { get; set; }

        /// <summary>
        /// 接近感应器参数
        /// </summary>
        public byte[]? ProximitySensorParams { get; set; }
    }

    /// <summary>
    /// 工位电气配置参数
    /// </summary>
    public class WorkStationEleConfigParams
    {
        /// <summary>
        /// 是否有左感应器
        /// </summary>
        public bool HasLeftSensor { get; set; }

        /// <summary>
        /// 是否有右感应器
        /// </summary>
        public bool HasRightSensor { get; set; }

        /// <summary>
        /// 是否有接近感应器
        /// </summary>
        public bool HasProximitySensor { get; set; }

        /// <summary>
        /// 是否有左挡板气缸
        /// </summary>
        public bool HasLeftBlkCylinder { get; set; }

        /// <summary>
        /// 是否有右挡板气缸
        /// </summary>
        public bool HasRightBlkCylinder { get; set; }
    }

    /// <summary>
    /// 工位能力数据结构
    /// </summary>
    public class WorkStationCapability
    {
        /// <summary>
        /// 是否支持左进
        /// </summary>
        public bool IsSupportLeftIn { get; set; }

        /// <summary>
        /// 是否支持右进
        /// </summary>
        public bool IsSupportRightIn { get; set; }

        /// <summary>
        /// 是否支持左出
        /// </summary>
        public bool IsSupportLeftOut { get; set; }

        /// <summary>
        /// 是否支持右出
        /// </summary>
        public bool IsSupportRightOut { get; set; }

        /// <summary>
        /// 轨道工作模式
        /// </summary>
        public ERailWorkMode RailWorkMode { get; set; }
    }

    public class WorkStationPanelInfo
    {
        /// <summary>
        /// 板子ID
        /// </summary>
        public string? PanelID { get; set; }

        /// <summary>
        /// 板子当前工序编号
        /// </summary>
        public string? CurProcessNum { get; set; }
    }

    /// <summary>
    /// 工位运输速度档位
    /// </summary>
    public class WorkStationTransSpeedGear
    {
        /// <summary>
        /// 运输速度档位
        /// </summary>
        public int TransSpeedGear { get; set; }

        /// <summary>
        /// 运输速度
        /// </summary>
        public double TransSpeed { get; set; }
    }

    public class WorkStationTransSpeedGearList : List<WorkStationTransSpeedGear>
    {
        public double Find(int transSpeedGear)
        {
            foreach (WorkStationTransSpeedGear t in this)
            {
                if (t.TransSpeedGear == transSpeedGear)
                    return t.TransSpeed;
            }
            throw new Exception();
        }
    }

    /// <summary>
    /// 进板运输参数基类
    /// </summary>
    public class InBoardParametersBase
    {
        /// <summary>
        /// 运输超时报警时间
        /// </summary>
        public int InBoardTimeout { get; set; }

        /// <summary>
        /// 是否启用二次进板检测
        /// </summary>
        public bool IsEnableCheckTwoBoard { get; set; }
    }

    /// <summary>
    /// 普通进板运输参数结构
    /// </summary>
    public class NormalInBoardParameters : InBoardParametersBase
    {
        /// <summary>
        /// 默认运输速度档位
        /// </summary>
        public int DefaultTransSpeedGear { get; set; }

        /// <summary>
        /// 进板皮带转动时间
        /// </summary>
        public int InBoardBeltRollTime { get; set; }
    }

    /// <summary>
    /// 两段速进板参数结构
    /// </summary>
    public class TwoSpeedInBoardParameters : InBoardParametersBase
    {
        /// <summary>
        /// 第一段运行的距离
        /// </summary>
        public double FirstTransDist { get; set; }

        /// <summary>
        /// 第一段默认运输速度档位
        /// </summary>
        public int FirstDefaultTransGear { get; set; }

        /// <summary>
        /// 第二段默认运输速度档位
        /// </summary>
        public int SecondDefaultTransGear { get; set; }
    }

    /// <summary>
    /// 停板模式枚举
    /// </summary>
    public enum EStopPanelMode
    {
        Cylinder,
        Sensor,
    }

    /// <summary>
    /// 挡板气缸停板参数
    /// </summary>
    public class CylinderStopBoardParameters
    {
        /// <summary>
        /// 缓冲速度
        /// </summary>
        public double BufferingSpeed { get; set; }

        /// <summary>
        /// 缓冲时间
        /// </summary>
        public int BufferingTime { get; set; }
    }

    /// <summary>
    /// 电眼停板参数
    /// </summary>
    public class SensorStopBoardParameters
    {
        public int ReverseSpeed { get; set; }
    }

    /// <summary>
    /// 停板参数
    /// </summary>
    public class StopBoardParameters
    {
        public EStopPanelMode StopPanel { get; set; }

        public CylinderStopBoardParameters? CylinderStopBoardParams { get; set; }

        public SensorStopBoardParameters? SensorStopBoardParams { get; set; }
    }

    /// <summary>
    /// 出板参数基类
    /// </summary>
    public class OutBoardParametersBase
    {
        public int TryTimeWhenJam { get; set; }
        public int TransTimeout { get; set; }
    }

    /// <summary>
    /// 普通出板参数结构
    /// </summary>
    public class NormalOutBoardParameters : OutBoardParametersBase
    {
        public double OutBoardDelay { get; set; }

        public int BeltRollingTime { get; set; }

        public int DefaultTransGear { get; set; }
    }

    /// <summary>
    /// 两段速出板参数结构
    /// </summary>
    public class TwoSpeedOutBoardParameters : OutBoardParametersBase
    {
        /// <summary>
        /// 第一段运行的距离
        /// </summary>
        public double FirstTransDist { get; set; }

        /// <summary>
        /// 第一段默认运输速度档位
        /// </summary>
        public int FirstDefaultTransGear { get; set; }

        /// <summary>
        /// 第二段默认运输速度档位
        /// </summary>
        public int SecondDefaultTransGear { get; set; }
    }

    public class WorkStationInitParameters
    {
        public bool IsSupportShadowWorkStation { get; set; }

        public bool IsEnableProximitySensor { get; set; }

        public WorkStationTransSpeedGearList WorkStationTransSpeedGears { get; set; }

        public bool IsEnableJammingSensor { get; set; }
    }

    public class WorkStationPfParameters
    {
        public ETransMode TransMode { get; set; }
        public bool IsBigPanelMode { get; set; }
        public InBoardParametersBase? InBoardParams { get; set; }

        public StopBoardParameters? StopBoardParams { get; set; }

        public OutBoardParametersBase? OutBoardParams { get; set; }
    }

    public class RailParametersInitCfg
    {
        public string LeftSensor { get; set; } = null;
        public string RightSensor { get; set; } = null;
        public List<string> LeftCylinder { get; set; } = null;
        public List<string> RightCylinder { get; set; } = null;
        public string ProximitySensor { get; set; } = null;
    }
}