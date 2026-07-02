using Griffins;
using Griffins.PF;

namespace GKG.Dispense
{
    /// <summary>
    /// 点胶服务 RPC 客户端代理网关 (Provider)
    /// </summary>
    public class SvrDispenseMngAppSvrProvider : SvrCallSvrProviderBase
    {
        #region 构造函数

        public SvrDispenseMngAppSvrProvider() : base() { }

        public SvrDispenseMngAppSvrProvider(string projectID) : base(projectID) { }

        public SvrDispenseMngAppSvrProvider(string projectID, UserID userID) : base(projectID, userID) { }

        #endregion

        #region 强类型服务接口暴露

        /// <summary>
        /// 获取点胶配置服务接口
        /// </summary>
        public IDispenseConfigMng GetDispenseConfigMng() => GetService<IDispenseConfigMng>(DispenseConst.SvrObj_DispenseConfigMng);

        /// <summary>
        /// 获取点胶管理服务接口
        /// </summary>
        public IDispenseMng GetDispenseMng() => GetService<IDispenseMng>(DispenseConst.SvrObj_DispenseMng);

        #endregion

        #region 底层泛型工厂

        /// <summary>
        /// 泛型服务获取器
        /// </summary>
        /// <typeparam name="T">期望获得的服务接口类型</typeparam>
        /// <param name="serverObjectIdStr">服务对象标识 GUID</param>
        /// <returns>强类型的服务代理实例</returns>
        private T GetService<T>(ServerObjectID serverObjId) where T : class
        {
            // 向底层基类请求服务实例
            object svrObj = base.GetDataSvr(DispenseConst.SERVERKINDID, serverObjId);

            return svrObj as T ?? throw new PFException($"RPC 代理获取失败：无法找到 {typeof(T).Name} 服务，或服务不可用。");
        }

        #endregion
    }
}