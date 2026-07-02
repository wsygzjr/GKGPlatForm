using GF_Gereric;
using Griffins.ImeIOT;
using Griffins.PF;
using Griffins.PF.Server.AppServer;

namespace GKG.Log.Server.Online
{
    /// <summary>
    /// 日志在线服务总入口
    /// </summary>
    [GriffinsAppSvrEntrance]
    internal class LogOnlineSvrMain : GriffinsPluginMngClass, IGriffinsOnLineAppSvr
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
            Task.Run(async () =>
            {
                var svr = new SvrObjCallForMMProcess();

                int counter = 1;

                while (true)
                {
                    var mockData = new
                    {
                        ModuleAlias = "点胶模组1",
                        LogLevel = 5,          
                        ThreadID = 88,
                        ErrorCode = "ERR-999",

                        LogText = $"[模拟测试] 这是一个来自底层模组的致命报警信号！(第 {counter} 次发送)"
                    };

                    string cmdParamJson = JsonObjConvert.ToJSon(mockData);

                    string result = string.Empty;
                    string errorMsg = string.Empty;

                    try
                    {
                        ((ISvrObjCallForMMProcess)svr).ExecCmd(LogConst.WriteLog_CmdID, cmdParamJson, out result, out errorMsg);

                    }
                    catch (Exception ex)
                    {
                        // 防御性编程：后台线程中的异常如果不 try-catch，可能会导致进程崩溃
                        Console.WriteLine($"[模拟测试异常] {ex.Message}");
                    }

                    counter++;

                    await Task.Delay(1000);
                }
            });
        }

        #endregion

        #region 信息处理

        private static void registerInfoProcessDelegates()
        {
            InformInfoBase.Register<LogInformInfo>(out var infoKindId);
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
        /// 数据自动维护守护任务 (清理过期日志、归档等)
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