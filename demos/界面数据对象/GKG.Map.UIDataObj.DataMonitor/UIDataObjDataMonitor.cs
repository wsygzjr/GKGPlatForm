using GF_Gereric;
using Griffins;
using Griffins.Map;

namespace GKG.Map.UIDataObj.DataMonitor
{
    /// <summary>
    /// 数据监控功能图元对应的界面数据对象
    /// 用于在前端/后端之间以统一的属性列表形式交换数据
    /// </summary>
    public class UIDataObjDataMonitor : GFPropObjBase
    {
        /// <summary>
        /// 供阀气压栏状态切换
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool SupplyValvePressureStatusSwitch { get; set; }

        /// <summary>
        /// 供阀气压栏状态名称
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? SupplyValvePressureStatusName { get; set; }

        /// <summary>
        /// 供阀气压值
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? SupplyValvePressureStatusValue { get; set; }

        /// <summary>
        /// 供阀气压单位
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? SupplyValvePressureStatusUnit { get; set; }

        /// <summary>
        /// 供胶气压栏状态切换
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool SupplyGluePressureStatusSwitch { get; set; }

        /// <summary>
        /// 供胶气压栏状态名称
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? SupplyGluePressureStatusName { get; set; }

        /// <summary>
        /// 供胶气压值
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? SupplyGluePressureStatusValue { get; set; }

        /// <summary>
        /// 供胶气压单位
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? SupplyGluePressureStatusUnit { get; set; }

        /// <summary>
        /// 喷嘴加热栏状态切换
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool NozzleHeatingStatusSwitch { get; set; }

        /// <summary>
        /// 喷嘴加热栏状态名称
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? NozzleHeatingStatusName { get; set; }

        /// <summary>
        /// 喷嘴加热温度值
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? NozzleHeatingStatusValue { get; set; }

        /// <summary>
        /// 喷嘴加热单位
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? NozzleHeatingStatusUnit { get; set; }

        /// <summary>
        /// 安全门状态
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool SafetyDoorStatus { get; set; }

        /// <summary>
        /// 总气压状态
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool TotalPressureStatus { get; set; }

        /// <summary>
        /// 清洁布状态
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool CleaningClothStatus { get; set; }

        /// <summary>
        /// 是否双阀模式
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool IsDualValve { get; set; }

        /// <summary>
        /// 胶水余量
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? GlueRemaining { get; set; }

        /// <summary>
        /// 左阀胶水余量
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? LeftGlueRemaining { get; set; }

        /// <summary>
        /// 校正
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? Calibration { get; set; }

        /// <summary>
        /// 左阀校正
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? LeftCalibration { get; set; }

        /// <summary>
        /// 阀体值
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? ValveBodyValue { get; set; }

        /// <summary>
        /// 左阀阀体值
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? LeftValveBodyValue { get; set; }

        /// <summary>
        /// 密封圈值
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? SealingRingValue { get; set; }

        /// <summary>
        /// 左阀密封圈值
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? LeftSealingRingValue { get; set; }
    }
}
