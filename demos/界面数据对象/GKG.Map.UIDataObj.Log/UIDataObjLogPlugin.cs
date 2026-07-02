namespace GKG.Map.UIDataObj.Log
{
    using GF_Gereric;
    using Griffins;
    using Griffins.ImeIOT;
    using Griffins.Map;
    using System.Reflection;

    /// <summary>
    /// 日志界面数据对象的“种类插件”
    /// 负责向框架提供：种类显示名、属性定义列表（Props）、命令定义列表（Commands）
    /// </summary>
	[UIDataObjKind(ImeIOTConst.SubSysID_Str, "Log")]
    internal class UIDataObjLogPlugin : GriffinsPluginMngClass, IUIDataObjKindPlugin
    {
        string IUIDataObjKindPlugin.ObjKindName
        {
            get { return ResourceNames.ResourceManager.GetString("Log") ?? "日志"; }
        }

        GFUIPropDefInfoList IUIDataObjKindPlugin.Props
            => GFPropObjBase.GetGFPropDefInfoes<UIDataObjLog>(ResourceNames.ResourceManager.GetString, null, null);

        GFMethodDefInfoList IUIDataObjKindPlugin.Commands
        {
            get
            {
                return new GFMethodDefInfoList{
                    new GFMethodDefInfo {
                        MethodID = "LoadLog",
                        MethodName = "加载日志",
                        ParamDefInfoes = new GFParamDefInfoList
                        {
                            new GFParamDefInfo
                            {
                                ParamID = "SelectedDate",
                                ParamName = "选中日期",
                                DataType = GriffinsBaseDataType.DateTime,
                                ObjectID = Guid.Empty
                            }
                        },
                        RetValDefInfoes = new GFParamDefInfoList
                        {
                            new GFParamDefInfo
                            {
                                ParamID = "SelectedDateLog",
                                ParamName = "选中日期日志",
                                DataType = GriffinsBaseDataType.String,
                                ObjectID = Guid.Empty
                            }
                        }
                    }
                };
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
