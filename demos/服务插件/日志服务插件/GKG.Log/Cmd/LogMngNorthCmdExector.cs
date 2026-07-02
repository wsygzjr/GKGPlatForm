using GF_Gereric;
using Griffins;
using Griffins.PF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GKG.Log.Cmd
{
    /// <summary>
    /// 日志管理北向命令执行对象
    /// </summary>
	public class LogMngNorthCmdExector : NorthIntfCmdExectorBase
    {
        /// <summary>
        /// 日志管理北向接口功能分类ID
        /// </summary>
        public const string FuncCategoryID = "LogMng";

        /// <summary>
        /// 创建 LogMngNorthCmdExector 实例
        /// </summary>
        /// <param name="iNorthSvrCommandExec">北向接口命令执行接口实例</param>
        public LogMngNorthCmdExector(INorthSvrCommandExec iNorthSvrCommandExec)
            : base(FuncCategoryID, iNorthSvrCommandExec)
        {
        }

        #region	查询日志信息列表

        /// <summary>
        /// 查询日志信息列表
        /// </summary>
        /// <param name="logLevel">日志级别</param>
        /// <param name="logText">模糊查询日志内容 </param>
        /// <param name="startTime">查询开始时间 </param>
        /// <param name="endTime">查询结束时间 </param>
        /// <param name="maxWaitingTime">等待时间</param>
        /// <returns>日志信息列表</returns>
        public LogInfoList SearchLogInfos(LogLevel logLevel, string logText, DateTime startTime, DateTime endTime, int maxWaitingTime = Timeout.Infinite)
        {
            var param = new SearchLogInfos_Param()
            {
                LogLevel = logLevel,
                LogText = logText,
                StartTime = startTime,
                EndTime = endTime
            };
            var response = _ExecTokenCommand<SearchLogInfos_Param, SearchLogInfos_Response>("SearchLogInfos", param, maxWaitingTime);
            return response.Data;
        }
        #region 参数与返回值

        public class SearchLogInfos_Param : Cmd_Param_Base
        {
            /// <summary>
            /// 日志级别
            /// </summary>
            public LogLevel LogLevel { get; set; }
            /// <summary>
            /// 模糊查询日志内容
            /// </summary>
            public string LogText { get; set; }
            /// <summary>
            /// 查询开始时间
            /// </summary>
            public DateTime StartTime { get; set; }
            /// <summary>
            /// 查询结束时间
            /// </summary>
            public DateTime EndTime { get; set; }
        }

        public class SearchLogInfos_Response : Cmd_Response_Base
        {
            public LogInfoList Data { get; set; }
        }
        #endregion
        #endregion

        #region	添加日志信息

        /// <summary>
        /// 添加日志信息
        /// </summary>
        /// <param name="logInfo">日志信息</param>
        /// <param name="maxWaitingTime">等待时间</param>
        public void UpdateLogInfo(LogInfo logInfo, int maxWaitingTime = Timeout.Infinite)
        {
            var param = new UpdateLogInfo_Param()
            {
                LogInfo = logInfo
            };
            _ExecTokenCommand<UpdateLogInfo_Param, UpdateLogInfo_Response>("UpdateLogInfo", param, maxWaitingTime);
        }
        #region 参数与返回值

        public class UpdateLogInfo_Param : Cmd_Param_Base
        {
            public LogInfo LogInfo { get; set; }
        }

        public class UpdateLogInfo_Response : Cmd_Response_Base
        {
            
        }
        #endregion
        #endregion
    }
}
