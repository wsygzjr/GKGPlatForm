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
    /// 日志配置管理北向命令执行对象
    /// </summary>
	public class LogConfigMngNorthCmdExector : NorthIntfCmdExectorBase
    {
        /// <summary>
        /// 日志配置管理北向接口功能分类ID
        /// </summary>
        public const string FuncCategoryID = "LogConfigMng";

        /// <summary>
        /// 创建 LogConfigMngNorthCmdExector 实例
        /// </summary>
        /// <param name="iNorthSvrCommandExec">北向接口命令执行接口实例</param>
        public LogConfigMngNorthCmdExector(INorthSvrCommandExec iNorthSvrCommandExec)
            : base(FuncCategoryID, iNorthSvrCommandExec)
        {
        }

        #region	读取日志配置信息

        /// <summary>
        /// 读取日志配置信息
        /// </summary>
        /// <param name="maxWaitingTime">等待时间</param>
        /// <returns>日志配置信息</returns>
        public LogConfig ReadLogConfig(int maxWaitingTime = Timeout.Infinite)
        {
            var param = new ReadLogConfig_Param();
            var response = _ExecTokenCommand<ReadLogConfig_Param, ReadLogConfig_Response>("ReadLogConfig", param, maxWaitingTime);
            return response.Data;
        }
        #region 参数与返回值

        public class ReadLogConfig_Param : Cmd_Param_Base
        {

        }

        public class ReadLogConfig_Response : Cmd_Response_Base
        {
            public LogConfig Data { get; set; }
        }
        #endregion
        #endregion

        #region	写日志配置信息

        /// <summary>
        /// 写日志配置信息
        /// </summary>
        /// <param name="logConfig">日志配置信息</param>
        /// <param name="maxWaitingTime">等待时间</param>
        public void WriteLogConfig(LogConfig logConfig, int maxWaitingTime = Timeout.Infinite)
        {
            var param = new WriteLogConfig_Param()
            {
                LogConfig = logConfig
            };
            _ExecTokenCommand<WriteLogConfig_Param, WriteLogConfig_Response>("WriteLogConfig", param, maxWaitingTime);
        }
        #region 参数与返回值

        public class WriteLogConfig_Param : Cmd_Param_Base
        {
            public LogConfig LogConfig { get; set; }
        }

        public class WriteLogConfig_Response : Cmd_Response_Base
        {
            
        }
        #endregion
        #endregion
    }
}
