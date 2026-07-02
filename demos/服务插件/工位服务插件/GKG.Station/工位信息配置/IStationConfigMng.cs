namespace GKG.Station
{
    /// <summary>
    /// 工位配置管理接口
    /// </summary>
    public interface IStationConfigMng
    {
        /// <summary>
        /// 读取工位配置信息
        /// </summary>
        /// <returns>工位配置信息</returns>
        StationConfig ReadStationConfig();

        /// <summary>
        /// 写工位配置信息
        /// </summary>
        /// <param name="stationConfig">工位配置信息</param>
        void WriteStationConfig(StationConfig stationConfig);
    }
}