using System.Collections.Generic;

namespace GKG.SubMM.StorageDeviceModule
{
    /// <summary>储料装置初始化参数。</summary>
    public class StorageDeviceInitCfg
    {
        public StorageDeviceInitCfg()
        {
            DisabledStorageIndexes = new List<int>();
            StorageMechanism = new List<StorageMechanismInitCfg>();
        }
        /// <summary>禁用的储料位索引列表，运行时会跳过这些储料位。</summary>
        public List<int> DisabledStorageIndexes { get; set; } = new List<int>();

        /// <summary>储料装置内部机构初始化参数，包含传感器与气缸接线信息。</summary>
        public List<StorageMechanismInitCfg> StorageMechanism { get; set; } = new List<StorageMechanismInitCfg>();

        /// <summary>按值复制储料装置初始化参数。</summary>
        public void CopyFrom(StorageDeviceInitCfg source)
        {
            DisabledStorageIndexes.Clear();
            DisabledStorageIndexes = source?.DisabledStorageIndexes != null
                ? new List<int>(source.DisabledStorageIndexes)
                : new List<int>();
            StorageMechanism.Clear();
            StorageMechanism = source?.StorageMechanism ?? new List<StorageMechanismInitCfg>();
        }
    }
}
