using Griffins.PF.Server.DB;
using Griffins.PF.DB;
using GF_Gereric;
using Griffins;

namespace GKG.Dispense.DB
{
    /// <summary>
    /// 点胶配置库表信息注册插件
    /// </summary>
    /// <remarks>
    /// 本类负责向 Griffins 框架注册当前服务所需的所有配置类数据库表结构
    /// </remarks>
    [GriffinsDbTableDescriptionInfo(DataBaseID.PrjConfig_Str)]
    internal class AppCfgTableInfo : GriffinsPluginMngClass, IGriffinsDbTableInfoMng
    {
        #region IGriffinsDbTableInfoMng 接口实现

        /// <summary>
        /// 数据库描述插件管理的数据库表信息字典
        /// </summary>
        GriffinsTableInfoDict IGriffinsDbTableInfoMng.TableInfoDict => AppCfgTableInfoConst.TableInfoDict;

        /// <summary>
        /// 数据库描述插件管理的模板数据库表信息字典
        /// </summary>
        GriffinsTempletTableInfoDict IGriffinsDbTableInfoMng.TempletTableInfoDict => AppCfgTableInfoConst.TempletTableInfoDict;

        /// <summary>
        /// 数据库描述插件管理的按时间分区存储的表信息字典
        /// </summary>
        GriffinsTimeTableInfoDict IGriffinsDbTableInfoMng.TimeTableInfoDict => AppCfgTableInfoConst.TimeTableInfoDict;

        /// <summary>
        /// 初始化数据库
        /// </summary>
        void IGriffinsDbTableInfoMng.InitDbdata(Delagaste_CreateDbInitDataUpdate createDbInitDataUpdateDele)
        {
            // 若未来需要在数据库创建时自动灌入“默认点胶配置”，可在此处实现
        }

        #endregion
    }

    /// <summary>
    /// 点胶配置库表信息常量字典
    /// </summary>
    internal static class AppCfgTableInfoConst
    {
        #region 全局单例字典初始化

        /// <summary>
        /// 表信息字典
        /// </summary>
        public static readonly GriffinsTableInfoDict TableInfoDict = new GriffinsTableInfoDict();

        /// <summary>
        /// 模板表信息字典
        /// </summary>
        public static GriffinsTempletTableInfoDict TempletTableInfoDict => null!;

        /// <summary>
        /// 按时间分区存储的表信息字典
        /// </summary>
        public static GriffinsTimeTableInfoDict TimeTableInfoDict => null!;

        #endregion
    }
}