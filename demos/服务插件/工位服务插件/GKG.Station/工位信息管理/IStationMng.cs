using Griffins;

namespace GKG.Station
{
    /// <summary>
    /// 工位管理服务接口
    /// </summary>
    public interface IStationMng
    {
        /// <summary>
        /// 查询指定条件的工位信息列表
        /// </summary>
        /// <param name="whereParameters">查询条件</param>
        /// <returns>指定条件的工位信息列表</returns>
        StationInfoList SearchStationInfos(GriffinsSQLWhereParameterCollection whereParameters);

        /// <summary>
        /// 添加或更新工位信息
        /// </summary>
        /// <param name="stationInfo">工位信息</param>
        void UpdateStationInfo(StationInfo stationInfo);

        /// <summary>
        /// 删除指定条件的工位信息
        /// </summary>
        /// <param name="whereParameters">删除条件</param>
        void DeleteStationInfos(GriffinsSQLWhereParameterCollection whereParameters);

        /// <summary>
        /// 获取最新的一条工位流水数据
        /// </summary>
        /// <param name="stationAlias">工位别名（传空表示查询所有工位的最新一条）</param>
        /// <returns>最新的一条记录，如果没有则返回 null</returns>
        StationInfo GetLatestStationInfo(string stationAlias);
    }
}