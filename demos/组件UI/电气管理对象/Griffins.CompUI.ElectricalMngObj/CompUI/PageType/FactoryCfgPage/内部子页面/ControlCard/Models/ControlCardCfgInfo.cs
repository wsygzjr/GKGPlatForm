using System;
using System.Collections.Generic;

namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Models
{
    /// <summary>
    /// 运控卡配置信息 
    /// </summary>
    public class ControlCardCfgInfo
    {
        /// <summary>
        /// 运控卡列表
        /// </summary>
        public List<ControlCardInfo> ControlCardList { set; get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ControlCardCfgInfo()
        {
            ControlCardList=new List<ControlCardInfo>();
        }
       
    }

    /// <summary>
    /// 运控卡信息模型
    /// </summary>
    public class ControlCardInfo 
    {
        /// <summary>
        /// 控制卡唯一ID
        /// </summary>
        public Guid UniqueID { set; get; }
        /// <summary>
        /// 控制卡种类
        /// </summary>
        public string ControlCardKind { set; get; } = "";

        /// <summary>
        /// 控制卡类型
        /// </summary>
        public string ControlCardType { set; get; } = "";

        /// <summary>
        /// 控制卡ID
        /// </summary>
        public string CadID { private set; get; } = "";

        /// <summary>
        /// 序号
        /// </summary>
        public int SerialNumber { set; get; }
        /// <summary>
        /// 轴数量
        /// </summary>

        public int AxisCount {private set; get; }

        /// <summary>
        /// 模拟量通道数量
        /// </summary>
        public int AnalogChannelCount { set; get; }

        /// <summary>
        /// 状态量通道数量
        /// </summary>
        public int StateChannelCount { set; get; }

        /// <summary>
        /// 运控卡基础配置
        /// </summary>
        public ControlCardBaseCfg ControlCardBaseCfg { get; set; }
        public ControlCardInfo(string cadID, int axisCount)
        {
            this.UniqueID = Guid.NewGuid();
            this.CadID=cadID;
            this.AxisCount= axisCount;
            ControlCardBaseCfg =new ControlCardBaseCfg();
        }
    }

    /// <summary>
    /// 运控卡基础配置
    /// </summary>
    public class ControlCardBaseCfg
    {
        /// <summary>
        /// 卡号
        /// </summary>
        public string CardNo { get; set; } = "";

        /// <summary>
        /// 运控卡配置文件路径
        /// </summary>
        public string CfgFilePath { get; set; } = "";

        /// <summary>
        /// 运控卡轴配置列表
        /// </summary>
        public List<ControlCardAxisCfgInfo> ControlCardAxisCfgInfoes { get; set; }
        public ControlCardBaseCfg()
        {
            ControlCardAxisCfgInfoes = new List<ControlCardAxisCfgInfo>();
        }

    }
    /// <summary>
    /// 运控卡轴配置
    /// </summary>
    public class ControlCardAxisCfgInfo
    {

        /// <summary>
        /// 轴号
        /// </summary>
        public int AxisNo { get; set; }

        /// <summary>
        /// 轴名称
        /// </summary>
        public string AxisName { get; set; } = "";

        /// <summary>
        /// 回零参数
        /// </summary>
        public ReturnToZeroParamInfo ReturnToZeroParamInfo { get; set; }
        /// <summary>
        /// 轴状态逻辑参数配置
        /// </summary>
        public AxisStateLogicParamInfo AxisStateLogicParamInfo { get; set; }
        /// <summary>
        /// 脉冲比参数配置
        /// </summary>
        public PulseRatioParamInfo PulseRatioParamInfo { get; set; }
        /// <summary>
        /// 软限位参数配置-视图模型
        /// </summary>
        public SoftLimitParamInfo SoftLimitParamInfo { get; set; }
        /// <summary>
        /// 编码器类型参数配置
        /// </summary>
        public EncoderTypeParamInfo EncoderTypeParamInfo { get; set; }
        /// <summary>
        /// 脉冲输入模式配置
        /// </summary>
        public PulseInputModeParamInfo PulseInputModeParamInfo { get; set; }
        /// <summary>
        /// 脉冲输出模式配置
        /// </summary>
        public PulseOutputModeParamInfo PulseOutputModeParamInfo { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ControlCardAxisCfgInfo()
        {
            ReturnToZeroParamInfo=new ReturnToZeroParamInfo();
            AxisStateLogicParamInfo=new AxisStateLogicParamInfo();
            PulseRatioParamInfo = new PulseRatioParamInfo();
            SoftLimitParamInfo = new SoftLimitParamInfo();
            SoftLimitParamInfo = new SoftLimitParamInfo();
            EncoderTypeParamInfo = new EncoderTypeParamInfo();
            EncoderTypeParamInfo = new EncoderTypeParamInfo();
            PulseInputModeParamInfo = new PulseInputModeParamInfo();
            PulseOutputModeParamInfo = new PulseOutputModeParamInfo();
        }
    }

    /// <summary>
    /// 回零参数信息
    /// </summary>
    public class ReturnToZeroParamInfo
    {
        /// <summary>
        /// 回零方向
        /// </summary>
        public ReturnToZeroMode ReturnToZeroMode { get; set; }

        /// <summary>
        /// 回零方向
        /// </summary>
        public ReturnToZeroDirection ReturnToZeroDirection { get; set; }
        /// <summary>
        /// 回零加速时间
        /// </summary>
        public double ReturnToZeroAccTime { get; set; }
        /// <summary>
        /// 回零初速度
        /// </summary>
        public double ReturnToZeroInitSpeed { get; set; }
        /// <summary>
        /// 回零最小速度
        /// </summary>
        public double ReturnToZeroMinSpeed { get; set; }
        /// <summary>
        /// 回零最大速度
        /// </summary>
        public double ReturnToZeroMaxSpeed { get; set; }
        /// <summary>
        /// 后撤距离
        /// </summary>

        public double RetreatDistance { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public ReturnToZeroParamInfo()
        {
        }
    }

    /// <summary>
    /// 轴状态逻辑参数信息
    /// </summary>
    public class AxisStateLogicParamInfo
    {
        /// <summary>
        /// 感应信号后反应方式
        /// </summary>
        public SignalReaction SignalReaction { get; set; }
        /// <summary>
        /// 运控卡轴状态类型
        /// </summary>
        public AxisStateType MotionControlCardAxisStateType { get; set; }

        /// <summary>
        /// 状态是否启用
        /// </summary>
        public bool IsStateEnabled { get; set; }

        /// <summary>
        /// 状态是否取反
        /// </summary>
        public bool IsStateInverted { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public AxisStateLogicParamInfo()
        {

        }
    }

    /// <summary>
    /// 脉冲比参数配置信息
    /// </summary>
    public class PulseRatioParamInfo
    {
        /// <summary>
        /// 脉冲比
        /// </summary>
        public double PulseRatio { get; set; }
        /// <summary>
        /// 每转脉冲
        /// </summary>
        public int PulsesPerRevolution { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public PulseRatioParamInfo()
        {

        }
    }

    /// <summary>
    /// 软限位参数配置信息
    /// </summary>
    public class SoftLimitParamInfo
    {
       
        /// <summary>
        /// 正极限位置
        /// </summary>
        public double PositiveLimitPosition { get; set; }
        /// <summary>
        /// 负极限位置
        /// </summary>
        public double NegativeLimitPosition { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public SoftLimitParamInfo()
        {
        }
    }
    /// <summary>
    /// 编码器类型参数配置信息
    /// </summary>
    public class EncoderTypeParamInfo
    {
        /// <summary>
        /// 选中的编码器类型
        /// </summary>
        public EncoderType EncoderType { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public EncoderTypeParamInfo()
        {
        }
    }
    /// <summary>
    /// 脉冲输入模式配置信息
    /// </summary>
    public class PulseInputModeParamInfo
    {
        /// <summary>
        /// 脉冲输入模式
        /// </summary>
        public PulseInputMode PulseInputMode { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public PulseInputModeParamInfo()
        {
            
        }
    }

    /// <summary>
    /// 脉冲输出模式配置信息
    /// </summary>
    public class PulseOutputModeParamInfo
    {
        /// <summary>
        /// 脉冲输出模式
        /// </summary>
        public PulseOutputMode PulseOutputMode { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public PulseOutputModeParamInfo()
        {
           
        }
    }
}
