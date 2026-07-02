using Griffins;
using Griffins.PF.Server;
using System.Collections.Concurrent;

namespace GKG.Station.Server.Online
{
    /// <summary>
    /// 工位配置管理服务实现类
    /// </summary>
    [GriffinsAppServerObject(StationConst.SvrObj_StationConfigMng_Str)]
    internal class StationConfigMng : SingleCallOpSvrBase, IStationConfigMng
    {
        StationConfig IStationConfigMng.ReadStationConfig() => StationConfigBuffer.ReadStationConfig(base.CallProjectID);

        void IStationConfigMng.WriteStationConfig(StationConfig stationConfig) => StationConfigBuffer.WriteStationConfig(base.CallProjectID, stationConfig);
    }

    /// <summary>
    /// 工位配置内存缓冲池
    /// </summary>
    internal static class StationConfigBuffer
    {
        private static readonly ConcurrentDictionary<string, StationConfig> _proStationConfigDict = new();

        internal static StationConfig ReadStationConfig(string projectID)
        {
            var cachedConfig = _proStationConfigDict.GetOrAdd(projectID, id =>
            {
                var newConfig = new StationConfig();
                byte[] data = StationOnlineSvrMain.CallForAppSvr.ReadProjectVar(id, StationConfig.VarID);

                if (data is { Length: > 0 })
                {
                    newConfig.FromBytes(data);
                }
                return newConfig;
            });

            var safeResult = new StationConfig();
            safeResult.CopyFrom(cachedConfig);
            return safeResult;
        }

        internal static void WriteStationConfig(string projectID, StationConfig stationConfig)
        {
            byte[] data = stationConfig.ToBytes();
            StationOnlineSvrMain.CallForAppSvr.WriteProjectVar(projectID, StationConfig.VarID, data);

            var newCacheConfig = new StationConfig();
            newCacheConfig.CopyFrom(stationConfig);

            _proStationConfigDict[projectID] = newCacheConfig;
        }
    }
}