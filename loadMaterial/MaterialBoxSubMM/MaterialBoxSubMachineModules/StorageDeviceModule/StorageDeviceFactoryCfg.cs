namespace GKG.SubMM.StorageDeviceModule
{
    /// <summary>储料装置出厂参数。</summary>
    public class StorageDeviceFactoryCfg
    {
        /// <summary>储料装置包含的储料位数量，默认对应上层和下层两个储料位。</summary>
        public int StorageCount { get; set; } = 2;

        /// <summary>按值复制储料装置出厂参数。</summary>
        public void CopyFrom(StorageDeviceFactoryCfg source)
        {
            StorageCount = source?.StorageCount ?? 2;
        }
    }
}
