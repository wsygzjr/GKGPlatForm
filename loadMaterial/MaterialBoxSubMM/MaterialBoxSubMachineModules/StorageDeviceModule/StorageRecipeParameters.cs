using System.Collections.Generic;
using System.Linq;

namespace GKG.SubMM.StorageDeviceModule
{
    /// <summary>单个储料位的配方参数。</summary>
    public class StorageRecipeParameters
    {
        /// <summary>首槽位置，通常作为整盒槽位重建的起点。</summary>
        public double FirstSlotPosition { get; set; }

        /// <summary>末槽位置，通常作为整盒槽位重建的终点。</summary>
        public double LastSlotPosition { get; set; }

        /// <summary>槽位总数。</summary>
        public int SlotCount { get; set; }

        /// <summary>余料预警阈值，剩余可用槽位小于等于该值时触发预警。</summary>
        public int SlotWarningCount { get; set; }

        /// <summary>理论槽距，可作为界面录入或槽位重建时的参考值。</summary>
        public double SlotSpacing { get; set; }

        /// <summary>槽位明细配置，当前主要承载每个槽位的启用状态；槽位真实位置以首末槽线性计算为准。</summary>
        public List<StorageSlotRecipeParameters> SlotList { get; set; } = new List<StorageSlotRecipeParameters>();

        /// <summary>储料位初始位置列表，按储料位索引对应。</summary>
        public List<double> InitialPositionList { get; set; } = new List<double>();

        /// <summary>按值复制单个储料位配方，避免共享集合引用。</summary>
        public void CopyFrom(StorageRecipeParameters source)
        {
            if (source == null)
            {
                FirstSlotPosition = 0;
                LastSlotPosition = 0;
                SlotCount = 0;
                SlotWarningCount = 0;
                SlotSpacing = 0;
                SlotList = new List<StorageSlotRecipeParameters>();
                InitialPositionList = new List<double>();
                return;
            }

            FirstSlotPosition = source.FirstSlotPosition;
            LastSlotPosition = source.LastSlotPosition;
            SlotCount = source.SlotCount;
            SlotWarningCount = source.SlotWarningCount;
            SlotSpacing = source.SlotSpacing;
            SlotList = source.SlotList?.Select(slot => new StorageSlotRecipeParameters
            {
                IsEnabled = slot.IsEnabled
            }).ToList() ?? new List<StorageSlotRecipeParameters>();
            InitialPositionList = source.InitialPositionList != null
                ? new List<double>(source.InitialPositionList)
                : new List<double>();
        }
    }
}
