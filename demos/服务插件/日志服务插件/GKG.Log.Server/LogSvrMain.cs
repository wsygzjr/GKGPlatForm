using Griffins;
using Griffins.PF.Server.AppServer;
using Griffins.PF;
using GF_Gereric;
using GKG.Log;

[assembly: Plugin(GriffinsAppServerKindAttribute.PLUGINKIND_Str, "{B56B91BD-A747-4D88-A014-CCDFC17EE5DF}", "Log Server")]
[assembly: GriffinsAppServerKind(LogConst.SERVERKINDID_Str)]

namespace GKG.Log.Server
{
    /// <summary>
    /// 日志服务基础信息注册
    /// </summary>
    [GriffinsAppServerInfo]
    internal class LogServerInfo : GriffinsPluginMngClass, IGriffinsAppServerInfoMain
    {
        #region IGriffinsAppServerInfoMain 接口实现

        string IGriffinsAppServerInfoMain.ServerKindName => ResourceString.Log_ServerKindName;

        #endregion

        #region IGriffinsAppServerInfo 接口实现

        OpMngCellMngGroupInfoOfServerKindList IGriffinsAppServerInfo.GetOpMngCellMngGroupInfo() => new();

        OpMngCellCodeOfServerKindList IGriffinsAppServerInfo.GetOpMngCellCode() => new();

        #endregion
    }

    /// <summary>
    /// 日志服务系统编码注册
    /// </summary>
    [GriffinsCode]
    internal class LogServerCodes : GriffinsPluginMngClass, IGriffinsCodeInfo
    {
        #region 全局静态编码表初始化

        private static readonly CodeTableList _codeTypes = BuildCodeTypes();

        private static CodeTableList BuildCodeTypes()
        {
            var list = new CodeTableList();

            // 注册当前子系统的全局编码
            var subSysCodeTable = new CodeTable(CodeTableList.CODETYPE_SubSys);
            subSysCodeTable.Add(LogConst.SubSysID, ResourceString.Log_SubSysName, true);

            list.Add(subSysCodeTable);
            return list;
        }

        #endregion

        #region IGriffinsCodeInfo 接口实现

        CodeTableList IGriffinsCodeInfo.CodeTypes => _codeTypes;

        #endregion
    }
}