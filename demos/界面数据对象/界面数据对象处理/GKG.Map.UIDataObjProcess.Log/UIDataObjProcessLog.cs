using GF_Gereric;
using GKG.Log;
using GKG.Map.UIDataObj.Log;
using Griffins;
using Griffins.ImeIOT;
using Griffins.Map;
using Griffins.Map.Cmd;
using Griffins.PF;
using System.Text.Json;

namespace GKG.Map.UIDataObjProcess.Log
{
    [UIDataObjProcess_ObjKind("Log")]
    internal class UIDataObjProcessLog : GriffinsPluginMngClass, IUIDataObjProcess
    {
        private IUIDataObjProcessCallBack callBack = null!;
        private UIDataObjProcessProductionInfoCfgInfo cfgInfo = null!;
        private string newLog  = string.Empty;

        // 将高频使用的 PropertyID 提取为静态只读常量，避免每次 Parse
        private static readonly MPPropertyID Prop_RunningLog = MPPropertyID.Parse("RunningLog");

        #region IUIDataObjProcess 成员

        void IUIDataObjProcess.Init(string pluginFileName, byte[] cfgData, IUIDataObjProcessCallBack iUIInterfaceCallBack)
        {
            this.callBack = iUIInterfaceCallBack;
            this.cfgInfo = new UIDataObjProcessProductionInfoCfgInfo();
            this.cfgInfo.FromBytes(cfgData);

            InformInfoBase.Register<LogInformInfo>(out var infoKindId);
            callBack.RegisterInformInfoProcessDelegate(LogConst.InfoKind_RealTimeLog, OnRealTimeLogReceived);
        }

        GFBaseTypeObjPropPathValueList IUIDataObjProcess.GetUIDataObjPropPathValues(string projectID, string alias, ManagingPointKind objKind, MPCategoryID category)
        {
            throw new NotImplementedException();
        }

        UIDataObjPropPathValueList IUIDataObjProcess.GetUIDataObjPropPathValues(string projectID, string[] aliases, ManagingPointKind objKind, MPCategoryID category)
        {
            throw new NotImplementedException();
        }

        GriffinsBaseValue IUIDataObjProcess.GetUIDataObjPropPathValue(string projectID, string alias, ObjInstPropPath objInstPropPath, ManagingPointKind objKind, MPCategoryID category)
        {
            throw new NotImplementedException();
        }

        GFBaseTypeObjPropPathValueList IUIDataObjProcess.GetUIDataObjPropPathValues(string projectID, string alias, ObjInstPropPath[] objInstPropPaths, ManagingPointKind objKind, MPCategoryID category)
        {
            throw new NotImplementedException();
        }

        void IUIDataObjProcess.SetUIDataObjPropPathValue(string projectID, UIDataObjPropPathValue uIDataObjPropPathValue, ManagingPointKind objKind, MPCategoryID category)
        {
            throw new NotImplementedException();
        }

        void IUIDataObjProcess.SetUIDataObjPropPathValues(string projectID, UIDataObjPropPathValueList uIDataObjPropPathValues, ManagingPointKind objKind, MPCategoryID category)
        {
            throw new NotImplementedException();
        }


        GFBaseTypeParamValueList IUIDataObjProcess.ExecUIDataObjCommand(string projectID, string alias, string cmdID, GFBaseTypeParamValueList cmdParam, ManagingPointKind objKind, MPCategoryID category)
            => new GFBaseTypeParamValueList();

        #endregion IUIDataObjProcess 成员

        #region 事件监听与推送

        private void OnRealTimeLogReceived(GriffinsInfoKindID infoKind, string infoNo, InformInfoBase info)
        {
            if (infoKind != LogConst.InfoKind_RealTimeLog)
                return;

            if (info is LogInformInfo logInform)
            {
                GKG.Log.LogInfo newLog = logInform.LogData;

                // 获取格式化后的纯文本日志
                this.newLog = Convert(newLog);

                var uiDataObjPropValue = new UIDataObjPropValue()
                {
                    Alias = this.callBack.Alias,
                    TimeStamp = DateTime.Now,
                    PropertyID = Prop_RunningLog,
                    Value = new GriffinsBaseValue(this.newLog)
                };
                this.callBack.AsynSendUIDataObjPropValueToMapTml("", uiDataObjPropValue);
            }
        }


        #endregion

        private string Convert(GKG.Log.LogInfo log)
        {
            var parts = new List<string>();

            parts.Add(log.UpdateTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff"));

            if (!string.IsNullOrWhiteSpace(log.ModuleAlias))
            {
                parts.Add($"[{log.ModuleAlias}]");
            }

            parts.Add($"[{log.LogLevel}]");

            if (!string.IsNullOrWhiteSpace(log.ErrorCode))
            {
                parts.Add($"[{log.ErrorCode}]");
            }

            if (log.ThreadID > 0)
            {
                parts.Add($"[T{log.ThreadID}]");
            }

            return $"{string.Join(" ", parts)}: {log.LogText}";
        }

        

        #region 内部配置类

        private class UIDataObjProcessProductionInfoCfgInfo
        {
            public byte[] ToBytes() => JsonObjConvert.ToJSonBytes(this);
            public void FromBytes(byte[] data)
            {
                if (data != null) JsonObjConvert.PopulateObject(data, this);
            }
        }

        #endregion
    }
}
