using Griffins;

namespace GKG.Log
{
    /// <summary>
    /// 日志全局常量
    /// </summary>
    public static class LogConst
    {
        // 实时日志的信息种类ID定义：
        public const string InfoKind_RealTimeLog_Str = "{BD3740E1-116B-485C-992E-0FA8B5C6CD96}";
        public static readonly GriffinsInfoKindID InfoKind_RealTimeLog = new GriffinsInfoKindID(InfoKind_RealTimeLog_Str);

        #region 子系统与服务种类标识

        public const string SubSysID_Str = "{62FE2DCF-4CEF-482B-AF63-005A1A04E6AF}";
        public static readonly SubSysID SubSysID = SubSysID.Parse(SubSysID_Str);

        public const string SERVERKINDID_Str = "{EFDD842C-59B1-46D7-BB8B-75D814226E5F}";
        public static readonly ServerKindID SERVERKINDID = new ServerKindID(SERVERKINDID_Str);

        #endregion

        #region RPC 服务对象门牌号

        // 日志配置管理服务
        public const string SvrObj_LogConfigMng_Str = "{6EC6E868-B943-4ADF-9CB4-D28E10E605B2}";
        public static readonly ServerObjectID SvrObj_LogConfigMng = new ServerObjectID(SvrObj_LogConfigMng_Str);

        // 日志管理（增删改查）服务
        public const string SvrObj_LogMng_Str = "{51505CA5-6C04-429E-8CE9-447805E50D87}";
        public static readonly ServerObjectID SvrObj_LogMng = new ServerObjectID(SvrObj_LogMng_Str);

        // 机械模组调用服务
        public const string SvrObjCallForMMProcess_Str = "{6F2AC7E4-5318-42F1-8F15-50BDF4E6EEB8}";
        public static readonly ServerObjectID SvrObjCallForMMProcess = new ServerObjectID(SvrObjCallForMMProcess_Str);

        #endregion

        #region 指令

        public const string WriteLog_CmdID = "WriteLog";

        #endregion
    }
}