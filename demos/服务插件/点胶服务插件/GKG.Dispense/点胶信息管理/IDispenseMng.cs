using Griffins;

namespace GKG.Dispense
{
    /// <summary>
    /// 点胶管理服务接口
    /// </summary>
    public interface IDispenseMng
    {
        /// <summary>
        /// 查询指定条件的点胶信息列表
        /// </summary>
        /// <param name="whereParameters">查询条件</param>
        /// <returns>指定条件的点胶信息列表</returns>
        DispenseInfoList SearchDispenseInfos(GriffinsSQLWhereParameterCollection whereParameters);

        /// <summary>
        /// 添加或更新点胶信息
        /// </summary>
        /// <param name="dispenseInfo">点胶信息</param>
        void UpdateDispenseInfo(DispenseInfo dispenseInfo);

        /// <summary>
        /// 删除指定条件的点胶信息
        /// </summary>
        /// <param name="whereParameters">删除条件</param>
        void DeleteDispenseInfos(GriffinsSQLWhereParameterCollection whereParameters);

        /// <summary>
        /// 获取最新的一条点胶流水数据
        /// </summary>
        /// <param name="moduleAlias">模组别名</param>
        /// <returns>最新的一条记录，如果没有则返回 null</returns>
        DispenseInfo GetLatestDispenseInfo(string moduleAlias);
    }
}