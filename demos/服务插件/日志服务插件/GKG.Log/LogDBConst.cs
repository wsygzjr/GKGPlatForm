namespace GKG.Log
{
    /// <summary>
    /// 日志数据库常量字典
    /// </summary>
    /// <remarks>
    /// 集中管理物理表名与字段名
    /// </remarks>
    public static class LogDBConst
    {
        public const string TableAlias = "R_Log";

        public const string FIELD_LogID = "LogID";
        public const string FIELD_UpdateTime = "UpdateTime";
        public const string FIELD_LogLevel = "LogLevel";
        public const string FIELD_ModuleAlias = "ModuleAlias";
        public const string FIELD_ThreadID = "ThreadID";
        public const string FIELD_ErrorCode = "ErrorCode";
        public const string FIELD_LogText = "LogText";

    }
}