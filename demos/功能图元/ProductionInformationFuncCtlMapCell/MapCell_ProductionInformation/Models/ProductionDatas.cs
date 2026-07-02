using GF_Gereric;
using Griffins;
using PropertyModels.ComponentModel;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace GKG.Map.ProductionInformationFuncCtlMapCell.Models
{
    /// <summary>
    /// 生产信息
    /// </summary>
    public class ProductionInfoData : GFPropObjBase
    {
        public static readonly ProductionInfoData DefaultEmpty = new ProductionInfoData();

        // 基础运行数据
        [GFProp(GfPropReadWrite.ReadOnly, "当前配方名称", GriffinsBaseDataType.String)]
        public string CurrentRecipeName { get; set; } = string.Empty;

        [GFProp(GfPropReadWrite.ReadOnly, "机器运行时间", GriffinsBaseDataType.String)]
        public string MachineRunTime { get; set; } = "00:00:00";

        [GFProp(GfPropReadWrite.ReadOnly, "稼动率", GriffinsBaseDataType.Decimal)]
        public double UtilizationRate { get; set; }

        [GFProp(GfPropReadWrite.ReadOnly, "UPH", GriffinsBaseDataType.Decimal)]
        public double Uph { get; set; }

        // 当前板时间统计
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

        // 平均板时间统计
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

        // 轨道列表
        private Dictionary<string, LaneData>? _lanes;
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

        public ProductionInfoData()
        {
            CurrentRecipeName = string.Empty;
            MachineRunTime = "00:00:00";
            UtilizationRate = 0.0;
            Uph = 0.0;

            CurrentCycleTime = 0.0;
            CurrentBoardProcessTime = 0.0;
            CurrentLoadTime = 3.5;
            CurrentUnloadTime = 0.0;
            CurrentMarkTime = 1.4;
            CurrentDispenseTime = 0.0;

            AverageCycleTime = 0.0;
            AverageBoardProcessTime = 0.0;
            AverageLoadTime = 0.0;
            AverageUnloadTime = 0.0;
            AverageMarkTime = 0.0;
            AverageDispenseTime = 0.0;

            Lanes = new();
        }

        public static ProductionInfoData GetMockData()
        {
            return new ProductionInfoData
            {
                CurrentRecipeName = "示例配方",
                MachineRunTime = "12:34:56",
                UtilizationRate = 85.5,
                Uph = 120.0,
                CurrentCycleTime = 30.0,
                CurrentBoardProcessTime = 25.0,
                CurrentLoadTime = 5.0,
                CurrentUnloadTime = 3.0,
                CurrentMarkTime = 2.0,
                CurrentDispenseTime = 1.0,
                AverageCycleTime = 28.0,
                AverageBoardProcessTime = 23.0,
                AverageLoadTime = 4.5,
                AverageUnloadTime = 2.5,
                AverageMarkTime = 1.5,
                AverageDispenseTime = 0.8,
                Lanes = new Dictionary<string, LaneData>
                {
                    ["A"] = new LaneData
                    {
                        LaneId = "Lane1",
                        LaneName = "轨道1",
                        TotalBigBoardCount = 100,
                        TotalSmallBoardCount = 200,
                        CurrentBigBoardCount = 50,
                        CurrentSmallBoardCount = 120
                    },

                    ["B"] = new LaneData
                    {
                        LaneId = "Lane2",
                        LaneName = "轨道2",
                        TotalBigBoardCount = 80,
                        TotalSmallBoardCount = 160,
                        CurrentBigBoardCount = 40,
                        CurrentSmallBoardCount = 90
                    }
                }
            };
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
