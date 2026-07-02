using Griffins.PF.Server.DB;
using Griffins.PF.DB;
using GF_Gereric;
using Griffins;
using GKG.Station;

namespace GKG.Station.DB
{
    /// <summary>
    /// 工位运行库表信息注册插件
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
    /// 工位运行库表信息常量字典
    /// </summary>
    internal static class AppRuntimeTableInfoConst
    {
        #region 全局单例字典初始化

        public static readonly GriffinsTableInfoDict TableInfoDict = new GriffinsTableInfoDict();

        public static GriffinsTempletTableInfoDict TempletTableInfoDict => null!;
        public static GriffinsTimeTableInfoDict TimeTableInfoDict => null!;

        static AppRuntimeTableInfoConst()
        {
            TableInfoDict[StationDBConst.TableAlias] = GetStationInfo();
        }

        #endregion

        #region 表结构构建逻辑

        /// <summary>
        /// 构建 R_Station 表的物理结构蓝图
        /// </summary>
        private static GriffinsTableInfo GetStationInfo()
        {
            GriffinsFieldInfoList fieldInfoList = new GriffinsFieldInfoList
            {
                new GriffinsFieldInfo(StationDBConst.FIELD_RecordID, StationDBConst.FIELD_RecordID, GriffinsDBType.Guid, canNull: false),
                
                new GriffinsFieldInfo(StationDBConst.FIELD_StationAlias, StationDBConst.FIELD_StationAlias, GriffinsDBType.NChar, 50, canNull: false),
                new GriffinsFieldInfo(StationDBConst.FIELD_ProductBatch, StationDBConst.FIELD_ProductBatch, GriffinsDBType.NChar, 50, canNull: false),
                new GriffinsFieldInfo(StationDBConst.FIELD_ProductID, StationDBConst.FIELD_ProductID, GriffinsDBType.NChar, 50, canNull: false),

                new GriffinsFieldInfo(StationDBConst.FIELD_InDuration, StationDBConst.FIELD_InDuration, GriffinsDBType.Double, canNull: false),
                new GriffinsFieldInfo(StationDBConst.FIELD_InTime, StationDBConst.FIELD_InTime, GriffinsDBType.DateTime, canNull: true),

                new GriffinsFieldInfo(StationDBConst.FIELD_StopDuration, StationDBConst.FIELD_StopDuration, GriffinsDBType.Double, canNull: false),
                new GriffinsFieldInfo(StationDBConst.FIELD_StopTime, StationDBConst.FIELD_StopTime, GriffinsDBType.DateTime, canNull: true),

                new GriffinsFieldInfo(StationDBConst.FIELD_OutDuration, StationDBConst.FIELD_OutDuration, GriffinsDBType.Double, canNull: false),
                new GriffinsFieldInfo(StationDBConst.FIELD_OutTime, StationDBConst.FIELD_OutTime, GriffinsDBType.DateTime, canNull: true),
            };

            string[] primaryKeys = new string[] { StationDBConst.FIELD_RecordID };

            GriffinsTableInfo tableInfo = new GriffinsTableInfo(StationDBConst.TableAlias, fieldInfoList, primaryKeys);

            tableInfo.IndexList.Add(new TableIndex("R_IX_Station_StationAlias", new string[] { StationDBConst.FIELD_StationAlias }, false));
            tableInfo.IndexList.Add(new TableIndex("R_IX_Station_ProductBatch", new string[] { StationDBConst.FIELD_ProductBatch }, false));
            tableInfo.IndexList.Add(new TableIndex("R_IX_Station_ProductID", new string[] { StationDBConst.FIELD_ProductID }, false));
            tableInfo.IndexList.Add(new TableIndex("R_IX_Station_InTime", new string[] { StationDBConst.FIELD_InTime }, false));

            return tableInfo;
        }

        #endregion
    }
}