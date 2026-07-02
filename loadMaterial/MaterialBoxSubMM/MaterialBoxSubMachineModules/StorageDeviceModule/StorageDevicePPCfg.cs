using System.Collections.Generic;

namespace GKG.SubMM.StorageDeviceModule
{
    /// <summary>储料装置配方参数。</summary>
    public class StorageDevicePPCfg
    {
        /// <summary>每个储料位对应一份配方参数，按储料位索引访问。</summary>
        public List<StorageRecipeParameters> Storages { get; set; } = new List<StorageRecipeParameters>() { };
        

        /// <summary>按值复制整套储料装置配方，保持每个储料位参数独立。</summary>
        public void CopyFrom(StorageDevicePPCfg source)
        {
            if (source?.Storages == null)
            {
                Storages = new List<StorageRecipeParameters>() { };
                return;
            }

            Storages = new List<StorageRecipeParameters>();
            foreach (StorageRecipeParameters storage in source.Storages)
            {
                StorageRecipeParameters target = new StorageRecipeParameters();
                target.CopyFrom(storage);
                Storages.Add(target);
            }
        }
    }
}
