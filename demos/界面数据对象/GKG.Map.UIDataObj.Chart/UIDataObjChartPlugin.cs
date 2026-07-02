using GF_Gereric;
using Griffins.Map;
using Griffins.ImeIOT;
using Griffins;

namespace GKG.Map.UIDataObj.Chart
{
	[UIDataObjKind(ImeIOTConst.SubSysID_Str, "Chart")]
	internal class UIDataObjChartPlugin : GriffinsPluginMngClass, IUIDataObjKindPlugin
    {
        /// <summary>
        /// 界面数据对象种类名称
        /// </summary>
        string IUIDataObjKindPlugin.ObjKindName 
		{
			get { return ResourceNames.Chart; } 
		}

        GFUIPropDefInfoList IUIDataObjKindPlugin.Props
            => GFPropObjBase.GetGFPropDefInfoes<UIDataObjChart>(ResourceNames.ResourceManager.GetString, null, null);

        /// <summary>
        /// 界面数据对象命令定义信息列表
        /// </summary>
        GFMethodDefInfoList IUIDataObjKindPlugin.Commands
        {
            get { return null; }
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
