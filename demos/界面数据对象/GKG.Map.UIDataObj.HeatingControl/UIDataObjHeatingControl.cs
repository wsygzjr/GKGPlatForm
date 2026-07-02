
using GF_Gereric;
using Griffins;
using Griffins.Map;

namespace GKG.Map.UIDataObj.HeatingControl
{
    /// <summary>
    /// 加热控制功能图元对应的界面数据对象
    /// 用于在前端/后端之间以统一的属性列表形式交换数据
    /// </summary>
    public class UIDataObjHeatingControl : GFPropObjBase
    {
        /// <summary>
        /// 是否双阀模式
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool IsDualValve { get; set; }

        /// <summary>
        /// 是否双轨模式
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool IsDualTrack { get; set; }

        /// <summary>
        /// 右阀点胶头：关/开
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool RightValveDispensingHead_Switch { get; set; }

        /// <summary>
        /// 右阀点胶头：当前温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int RightValveDispensingHead_CurrentTemperature { get; set; }

        /// <summary>
        /// 右阀点胶头：设定温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int RightValveDispensingHead_SetTemperature { get; set; }

        /// <summary>
        /// 右阀点胶头：温度范围
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int RightValveDispensingHead_TemperatureRange { get; set; }

        /// <summary>
        /// 右阀点胶头：空闲关闭间隔
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int RightValveDispensingHead_IdleCloseInterval { get; set; }

        /// <summary>
        /// 右阀点胶头：预热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int RightValveDispensingHead_PreheatTime { get; set; }

        /// <summary>
        /// 右阀点胶头：加热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int RightValveDispensingHead_HeatingTime { get; set; }

        /// <summary>
        /// 右阀点胶头：检测开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool RightValveDispensingHead_DetectionEnabled { get; set; }

        /// <summary>
        /// 右阀点胶头：工作开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool RightValveDispensingHead_WorkEnabled { get; set; }

        /// <summary>
        /// 右阀点胶头：延迟时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int RightValveDispensingHead_DelayTime { get; set; }

        /// <summary>
        /// 右阀胶筒加热：关/开
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool RightValveCartridgeHeating_Switch { get; set; }

        /// <summary>
        /// 右阀胶筒加热：当前温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int RightValveCartridgeHeating_CurrentTemperature { get; set; }

        /// <summary>
        /// 右阀胶筒加热：设定温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int RightValveCartridgeHeating_SetTemperature { get; set; }

        /// <summary>
        /// 右阀胶筒加热：温度范围
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int RightValveCartridgeHeating_TemperatureRange { get; set; }

        /// <summary>
        /// 右阀胶筒加热：空闲关闭间隔
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int RightValveCartridgeHeating_IdleCloseInterval { get; set; }

        /// <summary>
        /// 右阀胶筒加热：预热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int RightValveCartridgeHeating_PreheatTime { get; set; }

        /// <summary>
        /// 右阀胶筒加热：加热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int RightValveCartridgeHeating_HeatingTime { get; set; }

        /// <summary>
        /// 右阀胶筒加热：检测开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool RightValveCartridgeHeating_DetectionEnabled { get; set; }

        /// <summary>
        /// 右阀胶筒加热：工作开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool RightValveCartridgeHeating_WorkEnabled { get; set; }

        /// <summary>
        /// 右阀胶筒加热：延迟时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int RightValveCartridgeHeating_DelayTime { get; set; }

        /// <summary>
        /// 左阀点胶头：关/开
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool LeftValveDispensingHead_Switch { get; set; }

        /// <summary>
        /// 左阀点胶头：当前温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int LeftValveDispensingHead_CurrentTemperature { get; set; }

        /// <summary>
        /// 左阀点胶头：设定温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int LeftValveDispensingHead_SetTemperature { get; set; }

        /// <summary>
        /// 左阀点胶头：温度范围
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int LeftValveDispensingHead_TemperatureRange { get; set; }

        /// <summary>
        /// 左阀点胶头：空闲关闭间隔
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int LeftValveDispensingHead_IdleCloseInterval { get; set; }

        /// <summary>
        /// 左阀点胶头：预热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int LeftValveDispensingHead_PreheatTime { get; set; }

        /// <summary>
        /// 左阀点胶头：加热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int LeftValveDispensingHead_HeatingTime { get; set; }

        /// <summary>
        /// 左阀点胶头：检测开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool LeftValveDispensingHead_DetectionEnabled { get; set; }

        /// <summary>
        /// 左阀点胶头：工作开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool LeftValveDispensingHead_WorkEnabled { get; set; }

        /// <summary>
        /// 左阀点胶头：延迟时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int LeftValveDispensingHead_DelayTime { get; set; }

        /// <summary>
        /// 左阀胶筒加热：关/开
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool LeftValveCartridgeHeating_Switch { get; set; }

        /// <summary>
        /// 左阀胶筒加热：当前温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int LeftValveCartridgeHeating_CurrentTemperature { get; set; }

        /// <summary>
        /// 左阀胶筒加热：设定温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int LeftValveCartridgeHeating_SetTemperature { get; set; }

        /// <summary>
        /// 左阀胶筒加热：温度范围
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int LeftValveCartridgeHeating_TemperatureRange { get; set; }

        /// <summary>
        /// 左阀胶筒加热：空闲关闭间隔
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int LeftValveCartridgeHeating_IdleCloseInterval { get; set; }

        /// <summary>
        /// 左阀胶筒加热：预热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int LeftValveCartridgeHeating_PreheatTime { get; set; }

        /// <summary>
        /// 左阀胶筒加热：加热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int LeftValveCartridgeHeating_HeatingTime { get; set; }

        /// <summary>
        /// 左阀胶筒加热：检测开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool LeftValveCartridgeHeating_DetectionEnabled { get; set; }

        /// <summary>
        /// 左阀胶筒加热：工作开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool LeftValveCartridgeHeating_WorkEnabled { get; set; }

        /// <summary>
        /// 左阀胶筒加热：延迟时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int LeftValveCartridgeHeating_DelayTime { get; set; }

        /// <summary>
        /// A轨预热-左：关/开
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool ARailPreheatLeft_Switch { get; set; }

        /// <summary>
        /// A轨预热-左：当前温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailPreheatLeft_CurrentTemperature { get; set; }

        /// <summary>
        /// A轨预热-左：设定温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailPreheatLeft_SetTemperature { get; set; }

        /// <summary>
        /// A轨预热-左：温度范围
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailPreheatLeft_TemperatureRange { get; set; }

        /// <summary>
        /// A轨预热-左：空闲关闭间隔
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailPreheatLeft_IdleCloseInterval { get; set; }

        /// <summary>
        /// A轨预热-左：预热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailPreheatLeft_PreheatTime { get; set; }

        /// <summary>
        /// A轨预热-左：加热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailPreheatLeft_HeatingTime { get; set; }

        /// <summary>
        /// A轨预热-左：检测开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool ARailPreheatLeft_DetectionEnabled { get; set; }

        /// <summary>
        /// A轨预热-左：工作开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool ARailPreheatLeft_WorkEnabled { get; set; }

        /// <summary>
        /// A轨预热-左：延迟时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailPreheatLeft_DelayTime { get; set; }

        /// <summary>
        /// A轨预热-左(2)：关/开
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool ARailPreheatLeft2_Switch { get; set; }

        /// <summary>
        /// A轨预热-左(2)：当前温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailPreheatLeft2_CurrentTemperature { get; set; }

        /// <summary>
        /// A轨预热-左(2)：设定温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailPreheatLeft2_SetTemperature { get; set; }

        /// <summary>
        /// A轨预热-左(2)：温度范围
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailPreheatLeft2_TemperatureRange { get; set; }

        /// <summary>
        /// A轨预热-左(2)：空闲关闭间隔
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailPreheatLeft2_IdleCloseInterval { get; set; }

        /// <summary>
        /// A轨预热-左(2)：预热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailPreheatLeft2_PreheatTime { get; set; }

        /// <summary>
        /// A轨预热-左(2)：加热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailPreheatLeft2_HeatingTime { get; set; }

        /// <summary>
        /// A轨预热-左(2)：检测开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool ARailPreheatLeft2_DetectionEnabled { get; set; }

        /// <summary>
        /// A轨预热-左(2)：工作开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool ARailPreheatLeft2_WorkEnabled { get; set; }

        /// <summary>
        /// A轨预热-左(2)：延迟时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailPreheatLeft2_DelayTime { get; set; }

        /// <summary>
        /// A轨胶板工位-中：关/开
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool ARailGlueBoardStationMiddle_Switch { get; set; }

        /// <summary>
        /// A轨胶板工位-中：当前温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailGlueBoardStationMiddle_CurrentTemperature { get; set; }

        /// <summary>
        /// A轨胶板工位-中：设定温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailGlueBoardStationMiddle_SetTemperature { get; set; }

        /// <summary>
        /// A轨胶板工位-中：温度范围
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailGlueBoardStationMiddle_TemperatureRange { get; set; }

        /// <summary>
        /// A轨胶板工位-中：空闲关闭间隔
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailGlueBoardStationMiddle_IdleCloseInterval { get; set; }

        /// <summary>
        /// A轨胶板工位-中：预热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailGlueBoardStationMiddle_PreheatTime { get; set; }

        /// <summary>
        /// A轨胶板工位-中：加热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailGlueBoardStationMiddle_HeatingTime { get; set; }

        /// <summary>
        /// A轨胶板工位-中：检测开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool ARailGlueBoardStationMiddle_DetectionEnabled { get; set; }

        /// <summary>
        /// A轨胶板工位-中：工作开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool ARailGlueBoardStationMiddle_WorkEnabled { get; set; }

        /// <summary>
        /// A轨胶板工位-中：延迟时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailGlueBoardStationMiddle_DelayTime { get; set; }

        /// <summary>
        /// A轨点胶工位-中(2)：关/开
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool ARailDispensingStationMiddle2_Switch { get; set; }

        /// <summary>
        /// A轨点胶工位-中(2)：当前温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailDispensingStationMiddle2_CurrentTemperature { get; set; }

        /// <summary>
        /// A轨点胶工位-中(2)：设定温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailDispensingStationMiddle2_SetTemperature { get; set; }

        /// <summary>
        /// A轨点胶工位-中(2)：温度范围
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailDispensingStationMiddle2_TemperatureRange { get; set; }

        /// <summary>
        /// A轨点胶工位-中(2)：空闲关闭间隔
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailDispensingStationMiddle2_IdleCloseInterval { get; set; }

        /// <summary>
        /// A轨点胶工位-中(2)：预热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailDispensingStationMiddle2_PreheatTime { get; set; }

        /// <summary>
        /// A轨点胶工位-中(2)：加热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailDispensingStationMiddle2_HeatingTime { get; set; }

        /// <summary>
        /// A轨点胶工位-中(2)：检测开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool ARailDispensingStationMiddle2_DetectionEnabled { get; set; }

        /// <summary>
        /// A轨点胶工位-中(2)：工作开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool ARailDispensingStationMiddle2_WorkEnabled { get; set; }

        /// <summary>
        /// A轨点胶工位-中(2)：延迟时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailDispensingStationMiddle2_DelayTime { get; set; }

        /// <summary>
        /// A轨预热-右：关/开
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool ARailPreheatRight_Switch { get; set; }

        /// <summary>
        /// A轨预热-右：当前温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailPreheatRight_CurrentTemperature { get; set; }

        /// <summary>
        /// A轨预热-右：设定温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailPreheatRight_SetTemperature { get; set; }

        /// <summary>
        /// A轨预热-右：温度范围
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailPreheatRight_TemperatureRange { get; set; }

        /// <summary>
        /// A轨预热-右：空闲关闭间隔
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailPreheatRight_IdleCloseInterval { get; set; }

        /// <summary>
        /// A轨预热-右：预热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailPreheatRight_PreheatTime { get; set; }

        /// <summary>
        /// A轨预热-右：加热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailPreheatRight_HeatingTime { get; set; }

        /// <summary>
        /// A轨预热-右：检测开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool ARailPreheatRight_DetectionEnabled { get; set; }

        /// <summary>
        /// A轨预热-右：工作开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool ARailPreheatRight_WorkEnabled { get; set; }

        /// <summary>
        /// A轨预热-右：延迟时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailPreheatRight_DelayTime { get; set; }

        /// <summary>
        /// A轨预热-右(2)：关/开
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool ARailPreheatRight2_Switch { get; set; }

        /// <summary>
        /// A轨预热-右(2)：当前温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailPreheatRight2_CurrentTemperature { get; set; }

        /// <summary>
        /// A轨预热-右(2)：设定温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailPreheatRight2_SetTemperature { get; set; }

        /// <summary>
        /// A轨预热-右(2)：温度范围
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailPreheatRight2_TemperatureRange { get; set; }

        /// <summary>
        /// A轨预热-右(2)：空闲关闭间隔
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailPreheatRight2_IdleCloseInterval { get; set; }

        /// <summary>
        /// A轨预热-右(2)：预热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailPreheatRight2_PreheatTime { get; set; }

        /// <summary>
        /// A轨预热-右(2)：加热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailPreheatRight2_HeatingTime { get; set; }

        /// <summary>
        /// A轨预热-右(2)：检测开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool ARailPreheatRight2_DetectionEnabled { get; set; }

        /// <summary>
        /// A轨预热-右(2)：工作开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool ARailPreheatRight2_WorkEnabled { get; set; }

        /// <summary>
        /// A轨预热-右(2)：延迟时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int ARailPreheatRight2_DelayTime { get; set; }

        /// <summary>
        /// B轨预热-左：关/开
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool BRailPreheatLeft_Switch { get; set; }

        /// <summary>
        /// B轨预热-左：当前温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailPreheatLeft_CurrentTemperature { get; set; }

        /// <summary>
        /// B轨预热-左：设定温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailPreheatLeft_SetTemperature { get; set; }

        /// <summary>
        /// B轨预热-左：温度范围
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailPreheatLeft_TemperatureRange { get; set; }

        /// <summary>
        /// B轨预热-左：空闲关闭间隔
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailPreheatLeft_IdleCloseInterval { get; set; }

        /// <summary>
        /// B轨预热-左：预热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailPreheatLeft_PreheatTime { get; set; }

        /// <summary>
        /// B轨预热-左：加热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailPreheatLeft_HeatingTime { get; set; }

        /// <summary>
        /// B轨预热-左：检测开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool BRailPreheatLeft_DetectionEnabled { get; set; }

        /// <summary>
        /// B轨预热-左：工作开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool BRailPreheatLeft_WorkEnabled { get; set; }

        /// <summary>
        /// B轨预热-左：延迟时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailPreheatLeft_DelayTime { get; set; }

        /// <summary>
        /// B轨预热-左(2)：关/开
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool BRailPreheatLeft2_Switch { get; set; }

        /// <summary>
        /// B轨预热-左(2)：当前温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailPreheatLeft2_CurrentTemperature { get; set; }

        /// <summary>
        /// B轨预热-左(2)：设定温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailPreheatLeft2_SetTemperature { get; set; }

        /// <summary>
        /// B轨预热-左(2)：温度范围
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailPreheatLeft2_TemperatureRange { get; set; }

        /// <summary>
        /// B轨预热-左(2)：空闲关闭间隔
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailPreheatLeft2_IdleCloseInterval { get; set; }

        /// <summary>
        /// B轨预热-左(2)：预热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailPreheatLeft2_PreheatTime { get; set; }

        /// <summary>
        /// B轨预热-左(2)：加热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailPreheatLeft2_HeatingTime { get; set; }

        /// <summary>
        /// B轨预热-左(2)：检测开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool BRailPreheatLeft2_DetectionEnabled { get; set; }

        /// <summary>
        /// B轨预热-左(2)：工作开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool BRailPreheatLeft2_WorkEnabled { get; set; }

        /// <summary>
        /// B轨预热-左(2)：延迟时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailPreheatLeft2_DelayTime { get; set; }

        /// <summary>
        /// B轨胶板工位-中：关/开
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool BRailGlueBoardStationMiddle_Switch { get; set; }

        /// <summary>
        /// B轨胶板工位-中：当前温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailGlueBoardStationMiddle_CurrentTemperature { get; set; }

        /// <summary>
        /// B轨胶板工位-中：设定温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailGlueBoardStationMiddle_SetTemperature { get; set; }

        /// <summary>
        /// B轨胶板工位-中：温度范围
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailGlueBoardStationMiddle_TemperatureRange { get; set; }

        /// <summary>
        /// B轨胶板工位-中：空闲关闭间隔
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailGlueBoardStationMiddle_IdleCloseInterval { get; set; }

        /// <summary>
        /// B轨胶板工位-中：预热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailGlueBoardStationMiddle_PreheatTime { get; set; }

        /// <summary>
        /// B轨胶板工位-中：加热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailGlueBoardStationMiddle_HeatingTime { get; set; }

        /// <summary>
        /// B轨胶板工位-中：检测开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool BRailGlueBoardStationMiddle_DetectionEnabled { get; set; }

        /// <summary>
        /// B轨胶板工位-中：工作开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool BRailGlueBoardStationMiddle_WorkEnabled { get; set; }

        /// <summary>
        /// B轨胶板工位-中：延迟时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailGlueBoardStationMiddle_DelayTime { get; set; }

        /// <summary>
        /// B轨点胶工位-中(2)：关/开
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool BRailDispensingStationMiddle2_Switch { get; set; }

        /// <summary>
        /// B轨点胶工位-中(2)：当前温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailDispensingStationMiddle2_CurrentTemperature { get; set; }

        /// <summary>
        /// B轨点胶工位-中(2)：设定温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailDispensingStationMiddle2_SetTemperature { get; set; }

        /// <summary>
        /// B轨点胶工位-中(2)：温度范围
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailDispensingStationMiddle2_TemperatureRange { get; set; }

        /// <summary>
        /// B轨点胶工位-中(2)：空闲关闭间隔
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailDispensingStationMiddle2_IdleCloseInterval { get; set; }

        /// <summary>
        /// B轨点胶工位-中(2)：预热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailDispensingStationMiddle2_PreheatTime { get; set; }

        /// <summary>
        /// B轨点胶工位-中(2)：加热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailDispensingStationMiddle2_HeatingTime { get; set; }

        /// <summary>
        /// B轨点胶工位-中(2)：检测开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool BRailDispensingStationMiddle2_DetectionEnabled { get; set; }

        /// <summary>
        /// B轨点胶工位-中(2)：工作开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool BRailDispensingStationMiddle2_WorkEnabled { get; set; }

        /// <summary>
        /// B轨点胶工位-中(2)：延迟时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailDispensingStationMiddle2_DelayTime { get; set; }

        /// <summary>
        /// B轨预热-右：关/开
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool BRailPreheatRight_Switch { get; set; }

        /// <summary>
        /// B轨预热-右：当前温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailPreheatRight_CurrentTemperature { get; set; }

        /// <summary>
        /// B轨预热-右：设定温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailPreheatRight_SetTemperature { get; set; }

        /// <summary>
        /// B轨预热-右：温度范围
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailPreheatRight_TemperatureRange { get; set; }

        /// <summary>
        /// B轨预热-右：空闲关闭间隔
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailPreheatRight_IdleCloseInterval { get; set; }

        /// <summary>
        /// B轨预热-右：预热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailPreheatRight_PreheatTime { get; set; }

        /// <summary>
        /// B轨预热-右：加热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailPreheatRight_HeatingTime { get; set; }

        /// <summary>
        /// B轨预热-右：检测开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool BRailPreheatRight_DetectionEnabled { get; set; }

        /// <summary>
        /// B轨预热-右：工作开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool BRailPreheatRight_WorkEnabled { get; set; }

        /// <summary>
        /// B轨预热-右：延迟时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailPreheatRight_DelayTime { get; set; }

        /// <summary>
        /// B轨预热-右(2)：关/开
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool BRailPreheatRight2_Switch { get; set; }

        /// <summary>
        /// B轨预热-右(2)：当前温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailPreheatRight2_CurrentTemperature { get; set; }

        /// <summary>
        /// B轨预热-右(2)：设定温度
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailPreheatRight2_SetTemperature { get; set; }

        /// <summary>
        /// B轨预热-右(2)：温度范围
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailPreheatRight2_TemperatureRange { get; set; }

        /// <summary>
        /// B轨预热-右(2)：空闲关闭间隔
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailPreheatRight2_IdleCloseInterval { get; set; }

        /// <summary>
        /// B轨预热-右(2)：预热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailPreheatRight2_PreheatTime { get; set; }

        /// <summary>
        /// B轨预热-右(2)：加热时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailPreheatRight2_HeatingTime { get; set; }

        /// <summary>
        /// B轨预热-右(2)：检测开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool BRailPreheatRight2_DetectionEnabled { get; set; }

        /// <summary>
        /// B轨预热-右(2)：工作开启
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool BRailPreheatRight2_WorkEnabled { get; set; }

        /// <summary>
        /// B轨预热-右(2)：延迟时间
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public int BRailPreheatRight2_DelayTime { get; set; }
    }
}
