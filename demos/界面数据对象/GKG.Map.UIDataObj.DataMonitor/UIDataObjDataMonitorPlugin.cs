namespace GKG.Map.UIDataObj.DataMonitor
{
	using GF_Gereric;
	using Griffins;
	using Griffins.ImeIOT;
	using Griffins.Map;

	/// <summary>
	/// 数据监控界面数据对象的“种类插件”
	/// 负责向框架提供：种类显示名、属性定义列表（Props）、命令定义列表（Commands）
	/// </summary>
	[UIDataObjKind(ImeIOTConst.SubSysID_Str, "{D10A1D44-5E2F-4ED1-80A0-BEF1B5A3E8C6}")]
	internal class UIDataObjDataMonitorPlugin : GriffinsPluginMngClass, IUIDataObjKindPlugin
	{
		/// <summary>
		/// 界面数据对象种类名称（用于界面展示）
		/// </summary>
		string IUIDataObjKindPlugin.ObjKindName
		{
			get { return ResourceNames.ResourceManager.GetString("DataMonitor") ?? "数据监控"; }
		}

        GFUIPropDefInfoList IUIDataObjKindPlugin.Props
            => GFPropObjBase.GetGFPropDefInfoes<UIDataObjDataMonitor>(ResourceNames.ResourceManager.GetString, null, null);

        /// <summary>
        /// 界面数据对象命令定义信息列表
        /// </summary>
        GFMethodDefInfoList IUIDataObjKindPlugin.Commands
		{
			get
			{
				return new GFMethodDefInfoList();
			}
		}

		/// <summary>
		/// 读取资源字符串并做简单过滤
		/// 当前仅用于去除资源中用于分组/排序的 "\b" 标记
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
