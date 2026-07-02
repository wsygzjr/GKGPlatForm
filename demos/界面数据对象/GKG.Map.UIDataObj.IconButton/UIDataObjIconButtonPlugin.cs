using GF_Gereric;
using Griffins;
using Griffins.ImeIOT;
using Griffins.Map;

namespace GKG.Map.UIDataObj.IconButton
{
    [UIDataObjKind(ImeIOTConst.SubSysID_Str, "IconButton")]
    internal class UIDataObjIconButtonPlugin : GriffinsPluginMngClass, IUIDataObjKindPlugin
    {
        string IUIDataObjKindPlugin.ObjKindName
        {
            get { return ResourceNames.IconButton; }
        }


        GFUIPropDefInfoList IUIDataObjKindPlugin.Props
            => GFPropObjBase.GetGFPropDefInfoes<UIDataObjIconButton>(ResourceNames.ResourceManager.GetString, null, null);


        GFMethodDefInfoList IUIDataObjKindPlugin.Commands
        {
            get
            {
                return new GFMethodDefInfoList()
                {
                    new GFMethodDefInfo()
                    {
                        MethodID = "Event_MouseDown",
                        MethodName = getResourceStringWithFilter("Event_MouseDown"),
                        ParamDefInfoes = new GFParamDefInfoList(),
                        RetValDefInfoes = new GFParamDefInfoList()
                    },
                    new GFMethodDefInfo()
                    {
                        MethodID = "Event_MouseUp",
                        MethodName = getResourceStringWithFilter("Event_MouseUp"),
                        ParamDefInfoes = new GFParamDefInfoList(),
                        RetValDefInfoes = new GFParamDefInfoList()
                    },
                    new GFMethodDefInfo()
                    {
                        MethodID = "Event_MouseLeave",
                        MethodName = getResourceStringWithFilter("Event_MouseLeave"),
                        ParamDefInfoes = new GFParamDefInfoList(),
                        RetValDefInfoes = new GFParamDefInfoList()
                    },
                    new GFMethodDefInfo()
                    {
                        MethodID = "Event_MouseClick",
                        MethodName = getResourceStringWithFilter("Event_MouseClick"),
                        ParamDefInfoes = new GFParamDefInfoList(),
                        RetValDefInfoes = new GFParamDefInfoList()
                    },
                    new GFMethodDefInfo()
                    {
                        MethodID = "Event_MouseDoubleClick",
                        MethodName = getResourceStringWithFilter("Event_MouseDoubleClick"),
                        ParamDefInfoes = new GFParamDefInfoList(),
                        RetValDefInfoes = new GFParamDefInfoList()
                    },
                    new GFMethodDefInfo()
                    {
                        MethodID = "Event_MouseRightClick",
                        MethodName = getResourceStringWithFilter("Event_MouseRightClick"),
                        ParamDefInfoes = new GFParamDefInfoList(),
                        RetValDefInfoes = new GFParamDefInfoList()
                    }
                };
            }
        }

        private static string getResourceStringWithFilter(string resourceKey)
        {
            string? rawValue = ResourceNames.ResourceManager.GetString(resourceKey);
            string filteredValue = rawValue?.Replace("\b", "") ?? resourceKey;
            return filteredValue;
        }

        IUIDataObjKindDefSvr IUIDataObjKindPlugin.CreateIUIDataObjKindDefSvr(byte[] cfgData)
        {
            throw new NotImplementedException();
        }
    }
}
