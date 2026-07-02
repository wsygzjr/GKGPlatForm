using GF_Gereric;
using GKG.Dispense.Server.Data;
using Griffins;
using Griffins.PF;
using Griffins.PF.Server;
using Griffins.PF.Server.AppServer;

namespace GKG.Dispense.Server.Online
{
    /// <summary>
    /// 点胶在线服务总入口
    /// </summary>
    [GriffinsAppSvrEntrance]
    internal class DispenseOnlineSvrMain : GriffinsPluginMngClass, IGriffinsOnLineAppSvr
    {
        #region 全局应用服务器上下文

        internal static ICallForOnLineAppSvr CallForAppSvr { get; private set; } = null!;

        #endregion

        #region 插件生命周期管理

        void IGriffinsOnLineAppSvr.Init(ICallForOnLineAppSvr iCallForAppSvr, string pluginFileName)
        {
            CallForAppSvr = iCallForAppSvr;

            // _iCallForAppSvr.AfterPFDbHasInited += ... 
            // _iCallForAppSvr.AfterProjectDbHasInited += ... 

            registerInfoProcessDelegates();
            addAutoDataMaintainTask();
        }

        void IGriffinsOnLineAppSvr.Start()
        {
            // 预留
        }

        #endregion

        #region 信息处理

        private static void registerInfoProcessDelegates()
        {
            // 预留
        }

        #endregion

        #region 自动数据维护

        private void addAutoDataMaintainTask()
        {
            GriffinsTimeSpanTaskInfo taskInfo = new(() => new AutoDataMaintain())
            {
                TimeSpanMins = 5,                // 每 5 分钟执行一次
                DoOnLastTimeNotFinished = false, // 防雪崩机制：如果上一次没跑完，这次跳过
                IsBackground = false             // 决定任务是否在线程池后台运行
            };

            CallForAppSvr.AddTask(taskInfo);
        }

        /// <summary>
        /// 数据自动维护守护任务 (清理过期工位信息、归档等)
        /// </summary>
        private class AutoDataMaintain : IGriffinsTask
        {
            void IGriffinsTask.Exec()
            {
                try
                {
                    // TODO: 执行具体的数据库清理操作
                }
                catch (Exception ex)
                {
                    if (CallForAppSvr != null)
                    {
                        CallForAppSvr.PFLogWriter.WriteErrorLog($"[LogOnlineSvrMain] 自动数据维护异常: {ex.Message}");
                    }
                }
            }
        }

        #endregion
    }
}