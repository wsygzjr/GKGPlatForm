using Griffins.PF.Server.DB;
using Griffins.PF.DB;
using GF_Gereric;
using Griffins;

namespace GKG.Dispense.DB
{
    /// <summary>
    /// 点胶运行库表信息注册插件
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
    /// 点胶运行库表信息常量字典
    /// </summary>
    internal static class AppRuntimeTableInfoConst
    {
        #region 全局单例字典初始化

        public static readonly GriffinsTableInfoDict TableInfoDict = new GriffinsTableInfoDict();

        public static GriffinsTempletTableInfoDict TempletTableInfoDict => null!;
        public static GriffinsTimeTableInfoDict TimeTableInfoDict => null!;

        static AppRuntimeTableInfoConst()
        {
            TableInfoDict[DispenseDBConst.TableAlias] = GetDispenseInfo();
        }

        #endregion

        #region 表结构构建逻辑

        /// <summary>
        /// 构建 R_Dispense 表的物理结构蓝图
        /// </summary>
        private static GriffinsTableInfo GetDispenseInfo()
        {
            GriffinsFieldInfoList fieldInfoList = new GriffinsFieldInfoList
            {
                new GriffinsFieldInfo(DispenseDBConst.FIELD_RecordID, DispenseDBConst.FIELD_RecordID, GriffinsDBType.Guid, canNull: false),
                new GriffinsFieldInfo(DispenseDBConst.FIELD_ModuleAlias, DispenseDBConst.FIELD_ModuleAlias, GriffinsDBType.NChar, 50, canNull: false),
                new GriffinsFieldInfo(DispenseDBConst.FIELD_ProductBatch, DispenseDBConst.FIELD_ProductBatch, GriffinsDBType.NChar, 50, canNull: false),
                new GriffinsFieldInfo(DispenseDBConst.FIELD_ProductID, DispenseDBConst.FIELD_ProductID, GriffinsDBType.NChar, 50, canNull: false),
                new GriffinsFieldInfo(DispenseDBConst.FIELD_MarkTime, DispenseDBConst.FIELD_MarkTime, GriffinsDBType.DateTime, canNull: true),
                new GriffinsFieldInfo(DispenseDBConst.FIELD_MarkDuration, DispenseDBConst.FIELD_MarkDuration, GriffinsDBType.Double, canNull: false),
                new GriffinsFieldInfo(DispenseDBConst.FIELD_DispenseTime, DispenseDBConst.FIELD_DispenseTime, GriffinsDBType.DateTime, canNull: true),
                new GriffinsFieldInfo(DispenseDBConst.FIELD_DispenseDuration, DispenseDBConst.FIELD_DispenseDuration, GriffinsDBType.Double, canNull: false),
                new GriffinsFieldInfo(DispenseDBConst.FIELD_ProcessDuration, DispenseDBConst.FIELD_ProcessDuration, GriffinsDBType.Double, canNull: false),
            };

            string[] primaryKeys = new string[] { DispenseDBConst.FIELD_RecordID };

            GriffinsTableInfo tableInfo = new GriffinsTableInfo(DispenseDBConst.TableAlias, fieldInfoList, primaryKeys);

            tableInfo.IndexList.Add(new TableIndex("R_IX_Dispense_ModuleAlias", new string[] { DispenseDBConst.FIELD_ModuleAlias }, false));
            tableInfo.IndexList.Add(new TableIndex("R_IX_Dispense_ProductBatch", new string[] { DispenseDBConst.FIELD_ProductBatch }, false));
            tableInfo.IndexList.Add(new TableIndex("R_IX_Dispense_ProductID", new string[] { DispenseDBConst.FIELD_ProductID }, false));
            tableInfo.IndexList.Add(new TableIndex("R_IX_Dispense_DispenseTime", new string[] { DispenseDBConst.FIELD_DispenseTime }, false));

            return tableInfo;
        }

        #endregion
    }
}