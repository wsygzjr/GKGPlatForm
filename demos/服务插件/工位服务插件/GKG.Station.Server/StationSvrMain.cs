using Griffins;
using Griffins.PF.Server.AppServer;
using Griffins.PF;
using GF_Gereric;
using GKG.Station;

[assembly: Plugin(GriffinsAppServerKindAttribute.PLUGINKIND_Str, "{C3D4E5F6-1122-3344-5566-778899AABBCC}", "Station Info Server")]
[assembly: GriffinsAppServerKind(StationConst.SERVERKINDID_Str)]

namespace GKG.Station.Server
{
    /// <summary>
    /// 工位服务基础信息注册
    /// </summary>
    [GriffinsAppServerInfo]
    internal class StationInfoSvrInfo : GriffinsPluginMngClass, IGriffinsAppServerInfoMain
    {
        #region IGriffinsAppServerInfoMain 接口实现

        string IGriffinsAppServerInfoMain.ServerKindName => ResourceString.Station_ServerKindName;

        #endregion

        #region IGriffinsAppServerInfo 接口实现

        OpMngCellMngGroupInfoOfServerKindList IGriffinsAppServerInfo.GetOpMngCellMngGroupInfo() => new();

        OpMngCellCodeOfServerKindList IGriffinsAppServerInfo.GetOpMngCellCode() => new();

        #endregion
    }

    /// <summary>
    /// 工位服务系统编码注册
    /// </summary>
    [GriffinsCode]
    internal class StationInfoCodes : GriffinsPluginMngClass, IGriffinsCodeInfo
    {
        #region 全局静态编码表初始化

        private static readonly CodeTableList _codeTypes = BuildCodeTypes();

        private static CodeTableList BuildCodeTypes()
        {
            var list = new CodeTableList();

            // 注册当前子系统的全局编码
            var subSysCodeTable = new CodeTable(CodeTableList.CODETYPE_SubSys);
            subSysCodeTable.Add(StationConst.SubSysID, ResourceString.Station_SubSysName, true);

            list.Add(subSysCodeTable);
            return list;
        }

        #endregion

        #region IGriffinsCodeInfo 接口实现

        CodeTableList IGriffinsCodeInfo.CodeTypes => _codeTypes;

        #endregion
    }
}