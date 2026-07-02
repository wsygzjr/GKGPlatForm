using GF_Gereric;
using Griffins;
using Griffins.Map;

namespace GKG.Map.UIDataObj.AuxiliaryInfo
{
    /// <summary>
    /// 辅助信息功能图元对应的界面数据对象
    /// 用于在前端/后端之间以统一的属性列表形式交换数据
    /// </summary>
    public class UIDataObjAuxiliaryInfo : GFPropObjBase
    {
        /// <summary>
        /// 当前运行/加载的程序名称
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? ProgramName { get; set; }

        /// <summary>
        /// 工单号
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? WorkOrderNo { get; set; }

        /// <summary>
        /// 右阀胶水信息（例如胶水名称/型号等的组合信息）
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? RightValveGlueInfo { get; set; }

        /// <summary>
        /// 右阀胶水包装 ID
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? RightValveGluePackageId { get; set; }

        /// <summary>
        /// 右阀胶水生产批次号
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? RightValveGlueBatchNo { get; set; }

        /// <summary>
        /// 右阀胶水制造料号
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? RightValveGlueMaterialNo { get; set; }

        /// <summary>
        /// 右阀胶水生产日期
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? RightValveGlueProdDate { get; set; }

        /// <summary>
        /// 是否双阀模式（双阀时通常需要显示/维护左右阀两套胶水信息）
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool IsDualValve { get; set; }

        /// <summary>
        /// 左阀胶水信息
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? LeftValveGlueInfo { get; set; }

        /// <summary>
        /// 左阀胶水包装 ID
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? LeftValveGluePackageId { get; set; }

        /// <summary>
        /// 左阀胶水生产批次号
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? LeftValveGlueBatchNo { get; set; }

        /// <summary>
        /// 左阀胶水制造料号
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? LeftValveGlueMaterialNo { get; set; }

        /// <summary>
        /// 左阀胶水生产日期
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? LeftValveGlueProdDate { get; set; }

        /// <summary>
        /// 设备 ID
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? DeviceId { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? DeviceName { get; set; }

        /// <summary>
        /// 设备网卡 Mac 地址
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? MacAddress { get; set; }

        /// <summary>
        /// 设备 IP 地址
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? IpAddress { get; set; }

        /// <summary>
        /// 产品信息（例如产品型号/名称等）
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? ProductInfo { get; set; }

        /// <summary>
        /// 胶水名称
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? GlueName { get; set; }

        /// <summary>
        /// 压电阀序号
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? PiezoValveSerialNo { get; set; }

        /// <summary>
        /// 阀 ID
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? ValveId { get; set; }
    }
}
