using GKG.Station.Server.Data;
using Griffins;
using Griffins.PF.Server;

namespace GKG.Station.Server.Online
{
    /// <summary>
    /// 工位管理服务实现类
    /// </summary>
    [GriffinsAppServerObject(StationConst.SvrObj_StationMng_Str)]
    internal class StationMng : SingleCallOpSvrBase, IStationMng
    {
        #region 业务接口实现

        StationInfoList IStationMng.SearchStationInfos(GriffinsSQLWhereParameterCollection whereParameters)
           => ExecuteWithDb(base.CallProjectID, tda => tda.SearchStationInfos(whereParameters));

        void IStationMng.UpdateStationInfo(StationInfo stationInfo)
            => UpdateStationInfo(base.CallProjectID, stationInfo);

        void IStationMng.DeleteStationInfos(GriffinsSQLWhereParameterCollection whereParameters)
            => ExecuteWithDb(base.CallProjectID, tda => tda.DeleteStationInfos(whereParameters));

        StationInfo IStationMng.GetLatestStationInfo(string stationAlias)
        => ExecuteWithDb(base.CallProjectID, tda => tda.GetLatestStationInfo(stationAlias));

        #endregion

        #region 内部静态服务提供

        internal static void UpdateStationInfo(string projectID, StationInfo stationInfo)
            => ExecuteWithDb(projectID, tda => tda.UpdateStationInfo(stationInfo));

        #endregion

        #region 数据库连接生命周期管家

        /// <summary>
        /// 具有返回值的数据库执行包装器
        /// </summary>
        private static T ExecuteWithDb<T>(string projectID, Func<TDA_StationInfo, T> func)
        {
            TDA_StationInfo tda = new TDA_StationInfo(projectID);
            tda.DbConnection.Open();
            try
            {
                return func(tda);
            }
            finally
            {
                tda.DbConnection.Close();
            }
        }

        /// <summary>
        /// 无返回值的数据库执行包装器
        /// </summary>
        private static void ExecuteWithDb(string projectID, Action<TDA_StationInfo> action)
        {
            TDA_StationInfo tda = new TDA_StationInfo(projectID);
            tda.DbConnection.Open();
            try
            {
                action(tda);
            }
            finally
            {
                tda.DbConnection.Close();
            }
        }

        #endregion
    }
}