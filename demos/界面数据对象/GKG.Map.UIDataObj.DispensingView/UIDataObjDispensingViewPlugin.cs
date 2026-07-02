namespace GKG.Map.UIDataObj.DispensingView
{
    using GF_Gereric;
    using Griffins;
    using Griffins.ImeIOT;
    using Griffins.Map;

    /// <summary>
    /// 点胶视图界面数据对象的“种类插件”
    /// 负责向框架提供：种类显示名、属性定义列表（Props）、命令定义列表（Commands）
    /// </summary>
    [UIDataObjKind(ImeIOTConst.SubSysID_Str, "{1C6C6DA1-2BCB-4B1A-9D1A-DB4B537A0A2E}")]
    internal class UIDataObjDispensingViewPlugin : GriffinsPluginMngClass, IUIDataObjKindPlugin
    {
        string IUIDataObjKindPlugin.ObjKindName
        {
            get { return ResourceNames.ResourceManager.GetString("DispensingView") ?? "点胶视图"; }
        }

        GFUIPropDefInfoList IUIDataObjKindPlugin.Props
            => GFPropObjBase.GetGFPropDefInfoes<UIDataObjDispensingView>(ResourceNames.ResourceManager.GetString, null, null);

        GFMethodDefInfoList IUIDataObjKindPlugin.Commands
        {
            get
            {
                return new GFMethodDefInfoList();
            }
        }

        private static string getResourceStringWithFilter(string resourceKey)
        {
            string? rawValue = ResourceNames.ResourceManager.GetString(resourceKey);
            string filteredValue = rawValue?.Replace("\b", "") ?? string.Empty;
            return filteredValue;
        }

        IUIDataObjKindDefSvr IUIDataObjKindPlugin.CreateIUIDataObjKindDefSvr(byte[] cfgData)
        {
            throw new NotImplementedException();
        }
    }
}
