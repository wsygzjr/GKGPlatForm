
using GF_Gereric;
using Griffins;
using Griffins.ImeIOT;
using Griffins.Map;

namespace GKG.Map.UIDataObj.Communication
{
	/// <summary>
	/// 通信界面数据对象的“种类插件”
	/// 负责向框架提供：种类显示名、属性定义列表（Props）、命令定义列表（Commands）
	/// </summary>
	[UIDataObjKind(ImeIOTConst.SubSysID_Str, "{C508D61E-5B7E-4BA5-8112-910BD11A8CC9}")]
	internal class UIDataObjCommunicationPlugin : GriffinsPluginMngClass, IUIDataObjKindPlugin
	{
		/// <summary>
		/// 界面数据对象种类名称（用于界面展示）
		/// </summary>
		string IUIDataObjKindPlugin.ObjKindName
		{
			get { return ResourceNames.ResourceManager.GetString("Communication") ?? "通信"; }
		}

        GFUIPropDefInfoList IUIDataObjKindPlugin.Props
            => GFPropObjBase.GetGFPropDefInfoes<UIDataObjCommunication>(ResourceNames.ResourceManager.GetString, null, null);

        /// <summary>
        /// 界面数据对象命令定义信息列表
        /// </summary>
        GFMethodDefInfoList IUIDataObjKindPlugin.Commands
		{
			get
			{
				string cmdName = ResourceNames.ResourceManager.GetString("Communication_Command") ?? "通讯";
				return new GFMethodDefInfoList()
				{
					new GFMethodDefInfo()
					{
						MethodID = "Communication",
						MethodName = cmdName.Replace("\b", ""),
						ParamDefInfoes = new GFParamDefInfoList(),
						RetValDefInfoes = new GFParamDefInfoList()
					}
				};
			}
		}

		/// <summary>
		/// 获取资源字符串并过滤掉资源里可能包含的控制符
		/// </summary>
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
