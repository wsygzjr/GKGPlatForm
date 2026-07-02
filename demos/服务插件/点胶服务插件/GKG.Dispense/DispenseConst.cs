using Griffins;

namespace GKG.Dispense
{
    /// <summary>
    /// 点胶全局常量
    /// </summary>
    public static class DispenseConst
    {
        #region 子系统与服务种类标识

        public const string SubSysID_Str = "{AF5067F7-DF42-4795-BD92-EC68DAB31BDE}";
        public static readonly SubSysID SubSysID = SubSysID.Parse(SubSysID_Str);

        public const string SERVERKINDID_Str = "{00157FC0-1968-48EC-BDE1-222623D91553}";
        public static readonly ServerKindID SERVERKINDID = new ServerKindID(SERVERKINDID_Str);

        #endregion

        #region RPC 服务对象门牌号

        // 点胶信息配置管理服务
        public const string SvrObj_DispenseConfigMng_Str = "{7FB27181-EEED-4E55-9A28-4E71E3CB9246}";
        public static readonly ServerObjectID SvrObj_DispenseConfigMng = new ServerObjectID(SvrObj_DispenseConfigMng_Str);

        // 点胶信息管理（增删改查）服务
        public const string SvrObj_DispenseMng_Str = "{B72FE628-17B7-4B6E-A00B-DF55EF1C2DFD}";
        public static readonly ServerObjectID SvrObj_DispenseMng = new ServerObjectID(SvrObj_DispenseMng_Str);

        // 机械模组调用服务
        public const string SvrObjCallForMMProcess_Str = "{0ADF46D4-9C8A-4B9A-B251-A0CEBBEED0DA}";
        public static readonly ServerObjectID SvrObjCallForMMProcess = new ServerObjectID(SvrObjCallForMMProcess_Str);

        #endregion

        #region 指令

        public const string WriteDispenseInfo_CmdID = "WriteDispenseInfo";

        #endregion
    }
}