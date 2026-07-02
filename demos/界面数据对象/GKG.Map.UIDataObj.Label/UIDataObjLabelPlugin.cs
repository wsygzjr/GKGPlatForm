using GF_Gereric;
using Griffins;
using Griffins.ImeIOT;
using Griffins.Map;

namespace GKG.Map.UIDataObj.Label
{
    [UIDataObjKind(ImeIOTConst.SubSysID_Str, "Label")]
    internal class UIDataObjLabelPlugin : GriffinsPluginMngClass, IUIDataObjKindPlugin
    {
        string IUIDataObjKindPlugin.ObjKindName
        {
            get { return ResourceNames.Label; }
        }


        GFUIPropDefInfoList IUIDataObjKindPlugin.Props
            => GFPropObjBase.GetGFPropDefInfoes<UIDataObjLabel>(ResourceNames.ResourceManager.GetString, null, null);

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
