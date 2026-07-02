using GKG.Dispense.Server.Data;
using Griffins;
using Griffins.PF.Server;

namespace GKG.Dispense.Server.Online
{
    /// <summary>
    /// 点胶管理服务实现类
    /// </summary>
    [GriffinsAppServerObject(DispenseConst.SvrObj_DispenseMng_Str)]
    internal class DispenseMng : SingleCallOpSvrBase, IDispenseMng
    {
        #region 业务接口实现

        DispenseInfoList IDispenseMng.SearchDispenseInfos(GriffinsSQLWhereParameterCollection whereParameters)
           => ExecuteWithDb(base.CallProjectID, tda => tda.SearchDispenseInfos(whereParameters));

        void IDispenseMng.UpdateDispenseInfo(DispenseInfo dispenseInfo)
            => UpdateDispenseInfo(base.CallProjectID, dispenseInfo);

        void IDispenseMng.DeleteDispenseInfos(GriffinsSQLWhereParameterCollection whereParameters)
            => ExecuteWithDb(base.CallProjectID, tda => tda.DeleteDispenseInfos(whereParameters));

        DispenseInfo IDispenseMng.GetLatestDispenseInfo(string moduleAlias)
        => ExecuteWithDb(base.CallProjectID, tda => tda.GetLatestDispenseInfo(moduleAlias));

        #endregion

        #region 内部静态服务提供

        internal static void UpdateDispenseInfo(string projectID, DispenseInfo dispenseInfo)
            => ExecuteWithDb(projectID, tda => tda.UpdateDispenseInfo(dispenseInfo));

        #endregion

        #region 数据库连接生命周期管家

        /// <summary>
        /// 具有返回值的数据库执行包装器
        /// </summary>
        private static T ExecuteWithDb<T>(string projectID, Func<TDA_DispenseInfo, T> func)
        {
            TDA_DispenseInfo tda = new TDA_DispenseInfo(projectID);
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
        private static void ExecuteWithDb(string projectID, Action<TDA_DispenseInfo> action)
        {
            TDA_DispenseInfo tda = new TDA_DispenseInfo(projectID);
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