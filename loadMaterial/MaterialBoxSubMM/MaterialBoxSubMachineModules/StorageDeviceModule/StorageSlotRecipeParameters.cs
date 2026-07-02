namespace GKG.SubMM.StorageDeviceModule
{
    /// <summary>储料槽配方参数。</summary>
public class StorageSlotRecipeParameters
{
    /// <summary>当前槽位是否启用，禁用后运行时会跳过该槽位。</summary>
    public bool IsEnabled { get; set; } = true;
}
}
