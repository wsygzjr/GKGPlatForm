using Griffins;

namespace GKG.Log
{
    /// <summary>
    /// 日志管理服务接口
    /// </summary>
    public interface ILogMng
    {
        /// <summary>
        /// 查询指定条件的日志信息列表
        /// </summary>
        /// <param name="whereParameters">查询条件</param>
        /// <returns>指定条件的日志信息列表</returns>
        LogInfoList SearchLogInfos(GriffinsSQLWhereParameterCollection whereParameters);

        /// <summary>
        /// 添加或更新日志信息
        /// </summary>
        /// <param name="logInfo">日志信息</param>
        void UpdateLogInfo(LogInfo logInfo);

        /// <summary>
        /// 删除指定条件的日志信息
        /// </summary>
        /// <param name="whereParameters">删除条件</param>
        void DeleteLogInfos(GriffinsSQLWhereParameterCollection whereParameters);
    }
}