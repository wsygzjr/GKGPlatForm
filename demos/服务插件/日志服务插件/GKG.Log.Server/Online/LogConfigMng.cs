using Griffins.PF.Server;
using System.Collections.Concurrent;

namespace GKG.Log.Server.Online
{
    /// <summary>
    /// 日志配置管理服务实现类
    /// </summary>
	[GriffinsAppServerObject(LogConst.SvrObj_LogConfigMng_Str)]
    internal class LogConfigMng : SingleCallOpSvrBase, ILogConfigMng
    {
        LogConfig ILogConfigMng.ReadLogConfig() => LogConfigBuffer.ReadLogConfig(base.CallProjectID);

        void ILogConfigMng.WriteLogConfig(LogConfig logConfig) => LogConfigBuffer.WriteLogConfig(base.CallProjectID, logConfig);
    }

    /// <summary>
    /// 日志配置内存缓冲池
    /// </summary>
    internal static class LogConfigBuffer
    {
        private static readonly ConcurrentDictionary<string, LogConfig> _proLogConfigDict = new();

        internal static LogConfig ReadLogConfig(string projectID)
        {
            if (string.IsNullOrWhiteSpace(projectID))
            {
                return new LogConfig();
            }

            var cachedConfig = _proLogConfigDict.GetOrAdd(projectID, id =>
            {
                var newConfig = new LogConfig();
                byte[] data = LogOnlineSvrMain.CallForAppSvr.ReadProjectVar(id, LogConfig.VarID);

                if (data is { Length: > 0 })
                {
                    newConfig.FromBytes(data);
                }
                return newConfig;
            });

            var safeResult = new LogConfig();
            safeResult.CopyFrom(cachedConfig);
            return safeResult;
        }

        internal static void WriteLogConfig(string projectID, LogConfig logConfig)
        {
            if (string.IsNullOrWhiteSpace(projectID) || logConfig == null) return;

            byte[] data = logConfig.ToBytes();
            LogOnlineSvrMain.CallForAppSvr.WriteProjectVar(projectID, LogConfig.VarID, data);

            var newCacheConfig = new LogConfig();
            newCacheConfig.CopyFrom(logConfig);

            _proLogConfigDict[projectID] = newCacheConfig;
        }
    }
}
