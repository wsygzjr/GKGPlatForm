using Griffins;
using Griffins.PF;

namespace GKG.Log
{
    /// <summary>
    /// 实时日志推送信息载体
    /// </summary>
    public class LogInformInfo : InformInfoBase
    {
        /// <summary>
        /// 核心日志数据
        /// </summary>
        public LogInfo LogData { get; set; } = new LogInfo();

        /// <summary>
        /// 告知框架当前对象的种类标识
        /// </summary>
        protected override GriffinsInfoKindID _GetInfoKindID()
        {
            return LogConst.InfoKind_RealTimeLog;
        }
    }
}