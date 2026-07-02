using GF_Gereric;
using Griffins;
using Griffins.IOT;
using Griffins.Map;
using System;
using System.Collections.Generic;

namespace GKG.Map.UIDataObj.ProductionInfo
{
    /// <summary>
    /// 生产信息自定义界面数据对象
    /// </summary>
    public class UIDataObjProductionInfo : GFPropObjBase
    {
        private ProductionInfoData? _datas;

        [GFProp(GfPropReadWrite.ReadOnly, GFPropertyEditKind.Assignment, GriffinsValueRangeDefineMode.None, "生产信息数据", GriffinsBaseDataType.Object_Bytes, GFBaseTypePropValueList.Object_IDStr)]
        public ProductionInfoData? Datas
        {
            get => _datas;
            set
            {
                _datas = value;
                base.RaisePropertyChanged(nameof(Datas));
            }
        }

        internal static List<GriffinsBaseValue>? GetValueRangeEnums(string propertyID) => null;

        internal static GriffinsValueNamePairList? GetValueNamePairs(string propertyID) => null;
    }

    /// <summary>
    /// 生产信息
    /// </summary>
    public class ProductionInfoData : GFPropObjBase
    {
        [GFProp(GfPropReadWrite.ReadOnly, "当前配方名称", GriffinsBaseDataType.String)]
        public string CurrentRecipeName { get; set; } = string.Empty;

        [GFProp(GfPropReadWrite.ReadOnly, "机器运行时间", GriffinsBaseDataType.String)]
        public string MachineRunTime { get; set; } = "00:00:00";

        [GFProp(GfPropReadWrite.ReadOnly, "稼动率", GriffinsBaseDataType.Decimal)]
        public double UtilizationRate { get; set; }

        [GFProp(GfPropReadWrite.ReadOnly, "UPH", GriffinsBaseDataType.Decimal)]
        public double Uph { get; set; }

        [GFProp(GfPropReadWrite.ReadOnly, "当前生产周期", GriffinsBaseDataType.Decimal)]
        public double CurrentCycleTime { get; set; }

        [GFProp(GfPropReadWrite.ReadOnly, "当前基板处理时间", GriffinsBaseDataType.Decimal)]
        public double CurrentBoardProcessTime { get; set; }

        [GFProp(GfPropReadWrite.ReadOnly, "当前进板时间", GriffinsBaseDataType.Decimal)]
        public double CurrentLoadTime { get; set; }

        [GFProp(GfPropReadWrite.ReadOnly, "当前出板时间", GriffinsBaseDataType.Decimal)]
        public double CurrentUnloadTime { get; set; }

        [GFProp(GfPropReadWrite.ReadOnly, "当前Mark时间", GriffinsBaseDataType.Decimal)]
        public double CurrentMarkTime { get; set; }

        [GFProp(GfPropReadWrite.ReadOnly, "当前点胶时间", GriffinsBaseDataType.Decimal)]
        public double CurrentDispenseTime { get; set; }

        [GFProp(GfPropReadWrite.ReadOnly, "平均生产周期", GriffinsBaseDataType.Decimal)]
        public double AverageCycleTime { get; set; }

        [GFProp(GfPropReadWrite.ReadOnly, "平均基板处理时间", GriffinsBaseDataType.Decimal)]
        public double AverageBoardProcessTime { get; set; }

        [GFProp(GfPropReadWrite.ReadOnly, "平均进板时间", GriffinsBaseDataType.Decimal)]
        public double AverageLoadTime { get; set; }

        [GFProp(GfPropReadWrite.ReadOnly, "平均出板时间", GriffinsBaseDataType.Decimal)]
        public double AverageUnloadTime { get; set; }

        [GFProp(GfPropReadWrite.ReadOnly, "平均Mark时间", GriffinsBaseDataType.Decimal)]
        public double AverageMarkTime { get; set; }

        [GFProp(GfPropReadWrite.ReadOnly, "平均点胶时间", GriffinsBaseDataType.Decimal)]
        public double AverageDispenseTime { get; set; }

        private Dictionary<string, LaneData>? _lanes;
        /// <summary>
        /// 轨道列表
        /// </summary>
        [GFProp(GfPropReadWrite.ReadOnly, GFPropertyEditKind.Assignment, GriffinsValueRangeDefineMode.None, "轨道列表", GriffinsBaseDataType.Object_Bytes, GFBaseTypePropValueListDict.Object_IDStr)]
        public Dictionary<string, LaneData>? Lanes
        {
            get => _lanes;
            set
            {
                _lanes = value;
                base.RaisePropertyChanged(nameof(Lanes));
            }
        }
    }

    /// <summary>
    /// 轨道
    /// </summary>
    public class LaneData : GFPropObjBase
    {
        [GFProp(GfPropReadWrite.ReadOnly, "轨道ID", GriffinsBaseDataType.String)]
        public string LaneId { get; set; } = string.Empty;

        [GFProp(GfPropReadWrite.ReadOnly, "轨道名称", GriffinsBaseDataType.String)]
        public string LaneName { get; set; } = string.Empty;

        [GFProp(GfPropReadWrite.ReadOnly, "大板总数", GriffinsBaseDataType.Integer)]
        public int TotalBigBoardCount { get; set; }

        [GFProp(GfPropReadWrite.ReadOnly, "小板总数", GriffinsBaseDataType.Integer)]
        public int TotalSmallBoardCount { get; set; }

        [GFProp(GfPropReadWrite.ReadOnly, "当前大板数量", GriffinsBaseDataType.Integer)]
        public int CurrentBigBoardCount { get; set; }

        [GFProp(GfPropReadWrite.ReadOnly, "当前小板数量", GriffinsBaseDataType.Integer)]
        public int CurrentSmallBoardCount { get; set; }
    }
}