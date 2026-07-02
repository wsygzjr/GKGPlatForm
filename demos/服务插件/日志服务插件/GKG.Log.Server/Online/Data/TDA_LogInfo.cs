using Griffins.PF.DB;
using Griffins.PF.Server.DB;
using Griffins;
using GKG.Log.Server.Online;

namespace GKG.Log.Server.Data
{
    /// <summary>
    /// 日志表数据访问层
    /// </summary>
    internal class TDA_LogInfo : PrjRuntimeTableDataAccessBase
    {
        #region 构造函数

        public TDA_LogInfo(string projectID)
            : base(projectID, LogDBConst.TableAlias, LogOnlineSvrMain.CallForAppSvr.GetCreatePrjRuntimeDbAccessDele())
        {
        }

        public TDA_LogInfo(string projectID, GriffinsDbConnection dbConnection)
            : base(projectID, LogDBConst.TableAlias, dbConnection, LogOnlineSvrMain.CallForAppSvr.GetCreatePrjRuntimeDbAccessDele())
        {
        }

        #endregion

        #region 核心数据操作 (CRUD)

        /// <summary>
        /// 查询指定条件的日志信息列表
        /// </summary>
        public LogInfoList SearchLogInfos(GriffinsSQLWhereParameterCollection whereParameters, GriffinsDbTransaction transaction = null!)
        {
            LogInfoList logInfos = new();
            UnfixedListObjBindInfo listobjBindInfo = new(logInfos, typeof(LogInfo));

            bind(listobjBindInfo);
            TableObjDataAccess.ReadRows(listobjBindInfo, whereParameters, transaction);

            return logInfos;
        }

        /// <summary>
        /// 添加或更新日志信息
        /// </summary>
        public void UpdateLogInfo(LogInfo logInfo, GriffinsDbTransaction transaction = null!)
        {
            //logInfo.UpdateTime = DateTime.UtcNow;

            ObjBindInfo objBindInfo = new(logInfo);
            bind(objBindInfo);
            TableObjDataAccess.UpdateTable(objBindInfo, transaction);
        }

        /// <summary>
        /// 删除日志信息
        /// </summary>
        public void DeleteLogInfos(GriffinsSQLWhereParameterCollection whereParameters, GriffinsDbTransaction dbTransaction = null!)
            => TableObjDataAccess.DeleteRows(whereParameters, dbTransaction);

        #endregion

        #region ORM 映射引擎配置

        /// <summary>
        /// 建立数据库字段与 C# 实体属性的映射桥梁
        /// </summary>
        private void bind(ObjBindInfoBase objBindInfo)
        {
            objBindInfo.Bind(LogDBConst.FIELD_LogID, nameof(LogInfo.LogID), Guid.Empty);
            objBindInfo.Bind(LogDBConst.FIELD_UpdateTime, nameof(LogInfo.UpdateTime), DateTime.UtcNow);
            objBindInfo.Bind(LogDBConst.FIELD_LogLevel, nameof(LogInfo.LogLevel), 0);
            objBindInfo.Bind(LogDBConst.FIELD_ModuleAlias, nameof(LogInfo.ModuleAlias), string.Empty);
            objBindInfo.Bind(LogDBConst.FIELD_ThreadID, nameof(LogInfo.ThreadID), 0);
            objBindInfo.Bind(LogDBConst.FIELD_ErrorCode, nameof(LogInfo.ErrorCode), string.Empty);
            objBindInfo.Bind(LogDBConst.FIELD_LogText, nameof(LogInfo.LogText), string.Empty);
        }

        #endregion
    }
}