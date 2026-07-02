using Griffins;
using Griffins.PF;

namespace GKG.Log
{
    /// <summary>
    /// 日志服务 RPC 客户端代理网关 (Provider)
    /// </summary>
    public class SvrLogMngAppSvrProvider : SvrCallSvrProviderBase
    {
        #region 构造函数

        public SvrLogMngAppSvrProvider() : base() { }

        public SvrLogMngAppSvrProvider(string projectID) : base(projectID) { }

        public SvrLogMngAppSvrProvider(string projectID, UserID userID) : base(projectID, userID) { }

        #endregion

        #region 强类型服务接口暴露

        /// <summary>
        /// 获取日志配置服务接口
        /// </summary>
        public ILogConfigMng GetLogConfigMng() => GetService<ILogConfigMng>(LogConst.SvrObj_LogConfigMng);

        /// <summary>
        /// 获取日志管理服务接口
        /// </summary>
        public ILogMng GetLogMng() => GetService<ILogMng>(LogConst.SvrObj_LogMng);

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
            object svrObj = base.GetDataSvr(LogConst.SERVERKINDID, serverObjId);

            return svrObj as T ?? throw new PFException($"RPC 代理获取失败：无法找到 {typeof(T).Name} 服务，或服务不可用。");
        }

        #endregion
    }
}