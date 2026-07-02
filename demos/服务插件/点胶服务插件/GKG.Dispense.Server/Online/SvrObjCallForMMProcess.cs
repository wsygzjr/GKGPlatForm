using GKG.Dispense.Server.Online;
using Griffins.ImeIOT;
using Griffins.PF.Server;

namespace GKG.Dispense.Server
{
    /// <summary>
    /// 模组点胶指令接收网关
    /// </summary>
    [GriffinsAppServerObject(DispenseConst.SvrObjCallForMMProcess_Str)]
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
                    case DispenseConst.WriteDispenseInfo_CmdID:
                        DispenseUpInfo dispenseUpInfo = new DispenseUpInfo();
                        dispenseUpInfo.FromJson(cmdParam);

                        writeDispenseInfo(dispenseUpInfo);
                        return 0;

                    default:
                        errorMsg = $"未知的点胶指令 ID: {cmdID}";
                        return -1;
                }
            }
            catch (Exception ex)
            {
                errorMsg = $"指令 [{cmdID}] 解析或分发失败: {ex.Message}";
                DispenseOnlineSvrMain.CallForAppSvr.PFLogWriter.WriteErrorLog(errorMsg);
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

        private void writeDispenseInfo(DispenseUpInfo dispenseUpInfo)
        {
            try
            {
                string projectID = base.CallProjectID;
                DispenseConfig config = DispenseConfigBuffer.ReadDispenseConfig(projectID);

                if (config != null)
                {
                    // 可根据配置项进行过滤
                }

                DispenseInfo logInfo = new DispenseInfo()
                {
                    RecordID = Guid.NewGuid(),
                    ModuleAlias = dispenseUpInfo.ModuleAlias,
                    ProductBatch = dispenseUpInfo.ProductBatch,
                    ProductID = dispenseUpInfo.ProductID,
                    MarkTime = dispenseUpInfo.MarkTime,
                    MarkDuration = dispenseUpInfo.MarkDuration,
                    DispenseTime = dispenseUpInfo.DispenseTime,
                    DispenseDuration = dispenseUpInfo.DispenseDuration,
                    ProcessDuration = dispenseUpInfo.ProcessDuration
                };

                DispenseMng.UpdateDispenseInfo(projectID, logInfo);
            }
            catch (Exception ex)
            {
                DispenseOnlineSvrMain.CallForAppSvr.PFLogWriter.WriteErrorLog(
                    $"致命异常 - 机械模组点胶信息入库失败: {ex.Message} | 丢失数据: 模组：{dispenseUpInfo.ModuleAlias}");
            }
        }

        #endregion
    }
}