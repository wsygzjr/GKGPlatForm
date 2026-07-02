using Griffins;
using Griffins.PF.Server;
using System.Collections.Concurrent;

namespace GKG.Dispense.Server.Online
{
    /// <summary>
    /// 点胶配置管理服务实现类
    /// </summary>
    [GriffinsAppServerObject(DispenseConst.SvrObj_DispenseConfigMng_Str)]
    internal class DispenseConfigMng : SingleCallOpSvrBase, IDispenseConfigMng
    {
        DispenseConfig IDispenseConfigMng.ReadDispenseConfig() => DispenseConfigBuffer.ReadDispenseConfig(base.CallProjectID);

        void IDispenseConfigMng.WriteDispenseConfig(DispenseConfig config) => DispenseConfigBuffer.WriteDispenseConfig(base.CallProjectID, config);
    }

    /// <summary>
    /// 点胶配置内存缓冲池
    /// </summary>
    internal static class DispenseConfigBuffer
    {
        private static readonly ConcurrentDictionary<string, DispenseConfig> _proDispenseConfigDict = new();

        internal static DispenseConfig ReadDispenseConfig(string projectID)
        {
            var cachedConfig = _proDispenseConfigDict.GetOrAdd(projectID, id =>
            {
                var newConfig = new DispenseConfig();
                byte[] data = DispenseOnlineSvrMain.CallForAppSvr.ReadProjectVar(id, DispenseConfig.VarID);

                if (data is { Length: > 0 })
                {
                    newConfig.FromBytes(data);
                }
                return newConfig;
            });

            var safeResult = new DispenseConfig();
            safeResult.CopyFrom(cachedConfig);
            return safeResult;
        }

        internal static void WriteDispenseConfig(string projectID, DispenseConfig dispenseConfig)
        {
            byte[] data = dispenseConfig.ToBytes();
            DispenseOnlineSvrMain.CallForAppSvr.WriteProjectVar(projectID, DispenseConfig.VarID, data);

            var newCacheConfig = new DispenseConfig();
            newCacheConfig.CopyFrom(dispenseConfig);

            _proDispenseConfigDict[projectID] = newCacheConfig;
        }
    }
}