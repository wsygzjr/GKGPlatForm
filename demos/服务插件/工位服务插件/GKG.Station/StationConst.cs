using Griffins;

namespace GKG.Station
{
    /// <summary>
    /// 工位全局常量
    /// </summary>
    public static class StationConst
    {
        #region 子系统与服务种类标识

        public const string SubSysID_Str = "{1A2B3C4D-5E6F-7A8B-9C0D-112233445566}";
        public static readonly SubSysID SubSysID = SubSysID.Parse(SubSysID_Str);

        public const string SERVERKINDID_Str = "{A1B2C3D4-E5F6-47A8-8B9C-001122334455}";
        public static readonly ServerKindID SERVERKINDID = new ServerKindID(SERVERKINDID_Str);

        #endregion

        #region RPC 服务对象门牌号

        // 工位信息配置管理服务
        public const string SvrObj_StationConfigMng_Str = "{E1D2C3B4-A5B6-4788-99AA-BBCCDDEEFF00}";
        public static readonly ServerObjectID SvrObj_StationConfigMng = new ServerObjectID(SvrObj_StationConfigMng_Str);

        // 工位信息管理（增删改查）服务
        public const string SvrObj_StationMng_Str = "{F1E2D3C4-B5A6-9788-796A-554433221100}";
        public static readonly ServerObjectID SvrObj_StationMng = new ServerObjectID(SvrObj_StationMng_Str);

        // 机械模组调用服务
        public const string SvrObjCallForMMProcess_Str = "{7A8B9C0D-1122-3344-5566-778899AABBCC}";
        public static readonly ServerObjectID SvrObjCallForMMProcess = new ServerObjectID(SvrObjCallForMMProcess_Str);

        #endregion

        #region 指令

        public const string WriteStationInfo_CmdID = "WriteStationInfo";

        #endregion
    }
}