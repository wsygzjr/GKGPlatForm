using GKG.Station.Server.Online;
using Griffins.ImeIOT;
using Griffins.PF.Server;
using System;

namespace GKG.Station.Server
{
    /// <summary>
    /// 模组工位指令接收网关
    /// </summary>
    [GriffinsAppServerObject(StationConst.SvrObjCallForMMProcess_Str)]
    internal class SvrObjCallForMMProcess : SingleCallOpSvrBase, ISvrObjCallForMMProcess
    {
        #region ISvrObjCallForMMProcess 接口实现

        int ISvrObjCallForMMProcess.ExecCmd(string cmdID, string cmdParam, out string result, out string errorMsg)
        {
            result = string.Empty;
            errorMsg = string.Empty;

            try
            {
                switch (cmdID)
                {
                    case StationConst.WriteStationInfo_CmdID:
                        StationUpInfo stationUpInfo = new StationUpInfo();
                        stationUpInfo.FromJson(cmdParam);

                        writeStationInfo(stationUpInfo);
                        return 0;

                    default:
                        errorMsg = $"未知的工位指令 ID: {cmdID}";
                        return -1;
                }
            }
            catch (Exception ex)
            {
                errorMsg = $"指令 [{cmdID}] 解析或分发失败: {ex.Message}";
                StationOnlineSvrMain.CallForAppSvr.PFLogWriter.WriteErrorLog(errorMsg);
                return -1;
            }
        }

        int ISvrObjCallForMMProcess.ExecBigDataCmd(string cmdID, string cmdParam, byte[] bigData, out string result, out byte[] responseBigData, out string errorMsg)
        {
            // 预留接口：目前无大数据流需求
            result = string.Empty;
            errorMsg = string.Empty;
            responseBigData = null!;
            return 0;
        }

        #endregion

        #region 核心业务逻辑 

        private void writeStationInfo(StationUpInfo stationUpInfo)
        {
            try
            {
                string projectID = base.CallProjectID;
                StationConfig config = StationConfigBuffer.ReadStationConfig(projectID);

                if (config != null)
                {
                    // 可根据配置项进行过滤
                }

                StationInfo logInfo = new StationInfo()
                {
                    RecordID = Guid.NewGuid(),
                    StationAlias = stationUpInfo.StationAlias,
                    ProductBatch = stationUpInfo.ProductBatch,
                    ProductID = stationUpInfo.ProductID,
                    InDuration = stationUpInfo.InDuration,
                    InTime = stationUpInfo.InTime,
                    StopDuration = stationUpInfo.StopDuration,
                    StopTime = stationUpInfo.StopTime,
                    OutDuration = stationUpInfo.OutDuration,
                    OutTime = stationUpInfo.OutTime
                };

                StationMng.UpdateStationInfo(projectID, logInfo);
            }
            catch (Exception ex)
            {
                StationOnlineSvrMain.CallForAppSvr.PFLogWriter.WriteErrorLog(
                    $"致命异常 - 机械模组工位信息入库失败: {ex.Message} | 丢失数据: 工位：{stationUpInfo.StationAlias}");
            }
        }

        #endregion
    }
}