namespace GKG.Dispense
{
    /// <summary>
    /// 点胶配置管理接口
    /// </summary>
    public interface IDispenseConfigMng
    {
        /// <summary>
        /// 读取点胶配置信息
        /// </summary>
        DispenseConfig ReadDispenseConfig();

        /// <summary>
        /// 写入点胶配置信息
        /// </summary>
        void WriteDispenseConfig(DispenseConfig dispenseConfig);
    }
}