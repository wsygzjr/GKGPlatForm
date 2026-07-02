using Griffins;
using Griffins.PF.Server.AppServer;
using Griffins.PF;
using GF_Gereric;
using GKG.Dispense;

[assembly: Plugin(GriffinsAppServerKindAttribute.PLUGINKIND_Str, "{C37F36D1-78A5-4E78-A221-7A49CB143B83}", "Dispense Info Server")]
[assembly: GriffinsAppServerKind(DispenseConst.SERVERKINDID_Str)]

namespace GKG.Dispense.Server
{
    /// <summary>
    /// 点胶服务基础信息注册
    /// </summary>
    [GriffinsAppServerInfo]
    internal class DispenseInfoSvrInfo : GriffinsPluginMngClass, IGriffinsAppServerInfoMain
    {
        #region IGriffinsAppServerInfoMain 接口实现

        string IGriffinsAppServerInfoMain.ServerKindName => ResourceString.Dispense_ServerKindName;

        #endregion

        #region IGriffinsAppServerInfo 接口实现

        OpMngCellMngGroupInfoOfServerKindList IGriffinsAppServerInfo.GetOpMngCellMngGroupInfo() => new();

        OpMngCellCodeOfServerKindList IGriffinsAppServerInfo.GetOpMngCellCode() => new();

        #endregion
    }

    /// <summary>
    /// 点胶服务系统编码注册
    /// </summary>
    [GriffinsCode]
    internal class DispenseInfoCodes : GriffinsPluginMngClass, IGriffinsCodeInfo
    {
        #region 全局静态编码表初始化

        private static readonly CodeTableList _codeTypes = BuildCodeTypes();

        private static CodeTableList BuildCodeTypes()
        {
            var list = new CodeTableList();

            // 注册当前子系统的全局编码
            var subSysCodeTable = new CodeTable(CodeTableList.CODETYPE_SubSys);
            subSysCodeTable.Add(DispenseConst.SubSysID, ResourceString.Dispense_SubSysName, true);

            list.Add(subSysCodeTable);
            return list;
        }

        #endregion

        #region IGriffinsCodeInfo 接口实现

        CodeTableList IGriffinsCodeInfo.CodeTypes => _codeTypes;

        #endregion
    }
}