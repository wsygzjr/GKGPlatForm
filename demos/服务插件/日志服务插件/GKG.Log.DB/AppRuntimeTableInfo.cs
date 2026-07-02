using GF_Gereric;
using Griffins;
using Griffins.PF.DB;
using Griffins.PF.Server.DB;

namespace GKG.Log.DB
{
    /// <summary>
    /// 日志运行库表信息注册插件
    /// </summary>
    /// <remarks>
    /// 本类负责向 Griffins 框架注册当前服务所需的所有运行类数据库表结构
    /// </remarks>
    [GriffinsDbTableDescriptionInfo(DataBaseID.PrjRuntime_Str)]
    internal class AppRuntimeTableInfo : GriffinsPluginMngClass, IGriffinsDbTableInfoMng
    {
        #region IGriffinsDbTableInfoMng 接口实现

        /// <summary>
        /// 数据库描述插件管理的数据库表信息字典
        /// </summary>
        GriffinsTableInfoDict IGriffinsDbTableInfoMng.TableInfoDict => AppRuntimeTableInfoConst.TableInfoDict;

        /// <summary>
        /// 数据库描述插件管理的模板数据库表信息字典（暂无，返回 null）
        /// </summary>
        GriffinsTempletTableInfoDict IGriffinsDbTableInfoMng.TempletTableInfoDict => AppRuntimeTableInfoConst.TempletTableInfoDict;

        /// <summary>
        /// 数据库描述插件管理的按时间分区存储的表信息字典（暂无，返回 null）
        /// </summary>
        GriffinsTimeTableInfoDict IGriffinsDbTableInfoMng.TimeTableInfoDict => AppRuntimeTableInfoConst.TimeTableInfoDict;

        /// <summary>
        /// 初始化数据库
        /// </summary>
        void IGriffinsDbTableInfoMng.InitDbdata(Delagaste_CreateDbInitDataUpdate createDbInitDataUpdateDele)
        {
            // 如果后续有初始数据的自动灌入需求，可在此处实现
        }

        #endregion
    }

    /// <summary>
    /// 日志运行库表信息常量字典
    /// </summary>
    internal static class AppRuntimeTableInfoConst
    {
        #region 全局单例字典初始化

        public static readonly GriffinsTableInfoDict TableInfoDict = new GriffinsTableInfoDict();

        public static GriffinsTempletTableInfoDict TempletTableInfoDict => null!;
        public static GriffinsTimeTableInfoDict TimeTableInfoDict => null!;

        static AppRuntimeTableInfoConst()
        {
            TableInfoDict[LogDBConst.TableAlias] = GetLogInfo();
        }

        #endregion

        #region 表结构构建逻辑

        /// <summary>
        /// 构建 R_Log 表的物理结构蓝图
        /// </summary>
        private static GriffinsTableInfo GetLogInfo()
        {
            GriffinsFieldInfoList fieldInfoList = new GriffinsFieldInfoList
            {
                new GriffinsFieldInfo(LogDBConst.FIELD_LogID, LogDBConst.FIELD_LogID, GriffinsDBType.Guid, canNull: false),
                new GriffinsFieldInfo(LogDBConst.FIELD_UpdateTime, LogDBConst.FIELD_UpdateTime, GriffinsDBType.DateTime, canNull: false),
                new GriffinsFieldInfo(LogDBConst.FIELD_LogLevel, LogDBConst.FIELD_LogLevel, GriffinsDBType.Integer, canNull: false),
                new GriffinsFieldInfo(LogDBConst.FIELD_ModuleAlias, LogDBConst.FIELD_ModuleAlias, GriffinsDBType.NChar, fieldSize: 50, canNull: false),
                new GriffinsFieldInfo(LogDBConst.FIELD_ThreadID, LogDBConst.FIELD_ThreadID, GriffinsDBType.Integer, canNull: false),
                new GriffinsFieldInfo(LogDBConst.FIELD_ErrorCode, LogDBConst.FIELD_ErrorCode, GriffinsDBType.NChar, fieldSize: 50, canNull: false),
                new GriffinsFieldInfo(LogDBConst.FIELD_LogText, LogDBConst.FIELD_LogText, GriffinsDBType.NChar, fieldSize: 200, canNull: false)
            };

            string[] primaryKeys = new string[] { LogDBConst.FIELD_LogID };

            GriffinsTableInfo tableInfo = new GriffinsTableInfo(LogDBConst.TableAlias, fieldInfoList, primaryKeys);

            tableInfo.IndexList.Add(new TableIndex("R_IX_Log_ModuleAlias", new string[] { LogDBConst.FIELD_ModuleAlias }, false));
            tableInfo.IndexList.Add(new TableIndex("R_IX_Log_LogLevel", new string[] { LogDBConst.FIELD_LogLevel }, false));
            tableInfo.IndexList.Add(new TableIndex("R_IX_Log_UpdateTime", new string[] { LogDBConst.FIELD_UpdateTime }, false));
            tableInfo.IndexList.Add(new TableIndex("R_IX_Log_ErrorCode", new string[] { LogDBConst.FIELD_ErrorCode }, false));

            return tableInfo;
        }

        #endregion
    }
}