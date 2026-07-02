namespace GKG.Map.UIDataObj.Station
{
    using GF_Gereric;
    using Griffins;
    using Griffins.ImeIOT;
    using Griffins.Map;

    [UIDataObjKind(ImeIOTConst.SubSysID_Str, "Station")]
    internal class UIDataObjStationPlugin : GriffinsPluginMngClass, IUIDataObjKindPlugin
    {
        string IUIDataObjKindPlugin.ObjKindName
        {
            get { return ResourceNames.ResourceManager.GetString("Station") ?? "工位"; }
        }

        GFPropDefInfoList IUIDataObjKindPlugin.Props
        {
            get
            {
                return GFPropObjBase.GetGFPropDefInfoes<UIDataObjStation>(GetResourceStringWithFilter);
            }
        }

        GFMethodDefInfoList IUIDataObjKindPlugin.Commands
        {
            get
            {
                return new GFMethodDefInfoList();
            }
        }

        private static string GetResourceStringWithFilter(string resourceKey)
        {
            string? rawValue = ResourceNames.ResourceManager.GetString(resourceKey);
            return rawValue?.Replace("\b", "") ?? string.Empty;
        }
    }
}
