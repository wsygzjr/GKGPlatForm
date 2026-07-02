namespace GKG.Log
{
    /// <summary>
    /// 日志严重级别枚举 (数值越大，严重程度越高)
    /// </summary>
    /// <remarks>
    /// 本枚举的值将被直接持久化到数据库 R_Log 表中
    /// 绝对禁止修改现有成员的整型值！若需增加新级别，请使用未被占用的数字
    /// </remarks>
    public enum LogLevel : byte
    {
        /// <summary>
        /// 未知级别
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 调试日志 (极其详细)
        /// </summary>
        Trace = 1,

        /// <summary>
        /// 调试日志 (开发人员关注的关键节点信息)
        /// </summary>
        Debug = 2,

        /// <summary>
        /// 正常流水
        /// </summary>
        Info = 3,

        /// <summary>
        /// 警告
        /// </summary>
        Warn = 4,

        /// <summary>
        /// 错误
        /// </summary>
        Error = 5,

        /// <summary>
        /// 致命错误
        /// </summary>
        Fatal = 6
    }
}