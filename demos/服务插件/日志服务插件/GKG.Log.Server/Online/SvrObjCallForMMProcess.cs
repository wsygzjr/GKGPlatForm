using GKG.Log.Server.Online;
using Griffins.ImeIOT;
using Griffins.PF;
using Griffins.PF.Server;
using System;
using System.Linq;

namespace GKG.Log.Server
{
    /// <summary>
    /// 模组日志指令接收网关
    /// </summary>
    [GriffinsAppServerObject(LogConst.SvrObjCallForMMProcess_Str)]
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
                    case LogConst.WriteLog_CmdID:
                        LogUpInfo logUpInfo = new LogUpInfo();
                        logUpInfo.FromJson(cmdParam);

                        writeLog(logUpInfo);
                        return 0;

                    default:
                        errorMsg = $"未知的日志指令 ID: {cmdID}";
                        return -1;
                }
            }
            catch (Exception ex)
            {
                errorMsg = $"指令 [{cmdID}] 解析或分发失败: {ex.Message}";
                LogOnlineSvrMain.CallForAppSvr.PFLogWriter.WriteErrorLog(errorMsg);
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

        private void writeLog(LogUpInfo logUpInfo)
        {
            try
            {
                string projectID = base.CallProjectID;
                LogConfig config = LogConfigBuffer.ReadLogConfig(projectID);

                if (config != null)
                {
                    if (logUpInfo.LogLevel < config.MinRecordLevel)
                        return;

                    if (config.IgnoreInstances?.Contains(logUpInfo.ModuleAlias) == true)
                        return;

                    if (config.IgnoreKeyWords?.Any(kw => logUpInfo.LogText.Contains(kw)) == true)
                        return;
                }

                LogInfo logInfo = new LogInfo()
                {
                    LogID = Guid.NewGuid(),
                    UpdateTime = logUpInfo.UpdateTime,
                    LogLevel = logUpInfo.LogLevel,
                    ModuleAlias = logUpInfo.ModuleAlias,
                    ThreadID = logUpInfo.ThreadID,
                    ErrorCode = logUpInfo.ErrorCode,
                    LogText = logUpInfo.LogText
                };

                // 冷数据落盘 (持久化到 R_Log 表)
                LogMng.UpdateLogInfo(projectID, logInfo);

                // 热数据广播 (主动推送到前端 UI)
                BroadcastLogToFrontend(projectID, logInfo);
            }
            catch (Exception ex)
            {
                LogOnlineSvrMain.CallForAppSvr.PFLogWriter.WriteErrorLog(
                    $"致命异常 - 机械模组日志入库失败: {ex.Message} | 丢失数据: {logUpInfo.LogText}");
            }
        }

        private void BroadcastLogToFrontend(string projectID, LogInfo logInfo)
        {
            LogInformInfo informInfo = new LogInformInfo
            {
                LogData = logInfo
            };

            string infoNo = projectID;

            ServerInnerInfoSender.AsynSendInformInfo(LogConst.InfoKind_RealTimeLog, infoNo, informInfo);
        }

        #endregion
    }
}