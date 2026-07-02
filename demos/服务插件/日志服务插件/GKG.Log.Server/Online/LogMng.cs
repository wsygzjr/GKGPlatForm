using GKG.Log.Server.Data;
using Griffins;
using Griffins.PF;
using Griffins.PF.Server;
using System;

namespace GKG.Log.Server.Online
{
    /// <summary>
    /// 日志管理服务实现类
    /// </summary>
    [GriffinsAppServerObject(LogConst.SvrObj_LogMng_Str)]
    internal class LogMng : SingleCallOpSvrBase, ILogMng
    {
        #region 业务接口实现

        LogInfoList ILogMng.SearchLogInfos(GriffinsSQLWhereParameterCollection whereParameters)
            => ExecuteWithDb(base.CallProjectID, tda => tda.SearchLogInfos(whereParameters));

        void ILogMng.UpdateLogInfo(LogInfo logInfo)
            => UpdateLogInfo(base.CallProjectID, logInfo);

        void ILogMng.DeleteLogInfos(GriffinsSQLWhereParameterCollection whereParameters)
            => ExecuteWithDb(base.CallProjectID, tda => tda.DeleteLogInfos(whereParameters));

        #endregion

        #region 内部静态服务提供

        internal static void UpdateLogInfo(string projectID, LogInfo logInfo)
            => ExecuteWithDb(projectID, tda => tda.UpdateLogInfo(logInfo));

        #endregion

        #region 数据库连接生命周期管家

        /// <summary>
        /// 具有返回值的数据库执行包装器
        /// </summary>
        private static T ExecuteWithDb<T>(string projectID, Func<TDA_LogInfo, T> func)
        {
            TDA_LogInfo tda = new TDA_LogInfo(projectID);
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
        private static void ExecuteWithDb(string projectID, Action<TDA_LogInfo> action)
        {
            TDA_LogInfo tda = new TDA_LogInfo(projectID);
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