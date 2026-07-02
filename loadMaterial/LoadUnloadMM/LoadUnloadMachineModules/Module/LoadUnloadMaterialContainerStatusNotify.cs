namespace GKG.MM
{
    /// <summary>
    /// 上下料料盒容器状态变更通知参数。
    /// </summary>
    public class LoadUnloadMaterialContainerStatusNotify
    {
        /// <summary>
        /// 类型（上料/下料）。
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// 料盒容器状态 JSON。
        /// </summary>
        public string Param { get; set; } = string.Empty;
    }
}
