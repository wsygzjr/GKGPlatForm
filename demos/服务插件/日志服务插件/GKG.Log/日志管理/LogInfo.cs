using Newtonsoft.JsonG;
using Newtonsoft.JsonG.Converters;

namespace GKG.Log
{
    /// <summary>
    /// 数据库持久化日志记录实体 (POCO)
    /// </summary>
    /// <remarks>
    /// 承载 R_Log 表的完整行数据
    /// </remarks>
    [Serializable]
    public class LogInfo
    {
        #region 核心属性

        /// <summary>
        /// 日志全局唯一ID
        /// </summary>
        public Guid LogID { get; set; } = Guid.Empty;

        private DateTime _updateTime = DateTime.UtcNow;
        /// <summary>
        /// 日志更新/入库时间
        /// </summary>
        /// <remarks>
        /// 【双重架构防御】
        /// 防御 1 (向上/网络层)：强制使用 IsoDateTimeConverter，突破底层恶意的全局 JSON 配置，确保输出带 'Z' 的标准时间
        /// 防御 2 (向下/数据层)：拦截底层 ORM 的反射赋值，无论数据库读出什么，强行盖上 UTC 钢印，防止 Kind 丢失
        /// </remarks>
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime UpdateTime
        {
            get => _updateTime;
            set => _updateTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        /// <summary>
        /// 日志级别
        /// </summary>
        public LogLevel LogLevel { get; set; } = LogLevel.Info;

        /// <summary>
        /// 产生日志的模组或实例别名
        /// </summary>
        public string ModuleAlias { get; set; } = string.Empty;

        /// <summary>
        /// 产生日志的线程ID
        /// </summary>
        public int ThreadID { get; set; } = 0;

        /// <summary>
        /// 日志错误码
        /// </summary>
        public string ErrorCode { get; set; } = string.Empty;

        /// <summary>
        /// 日志内容
        /// </summary>
        public string LogText { get; set; } = string.Empty;


        #endregion

        #region 核心操作

        /// <summary>
        /// 深度值拷贝
        /// </summary>
        /// <param name="source">数据源</param>
        /// <remarks>
        /// 执行防御性拷贝
        /// </remarks>
        public void CopyFrom(LogInfo source)
        {
            if (source == null) return;

            this.LogID = source.LogID;
            this.UpdateTime = source.UpdateTime;
            this.LogLevel = source.LogLevel;
            this.ModuleAlias = source.ModuleAlias;
            this.ThreadID = source.ThreadID;
            this.ErrorCode = source.ErrorCode;
            this.LogText = source.LogText;
        }

        #endregion
    }

    /// <summary>
    /// 日志信息强类型集合
    /// </summary>
    /// <remarks>
    /// 专为适配 Griffins 底层框架的 UnfixedListObjBindInfo 机制而保留的派生类
    /// </remarks>
    [Serializable]
    public class LogInfoList : List<LogInfo>
    {
    }
}