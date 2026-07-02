
 namespace GKG.Map.UIDataObj.StatusInfo
 {
 	using GF_Gereric;
     using Griffins;
     using Griffins.ImeIOT;
 	using Griffins.Map;
 
 	/// <summary>
 	/// 状态信息界面数据对象的“种类插件”
 	/// 负责向框架提供：种类显示名、属性定义列表（Props）、命令定义列表（Commands）
 	/// </summary>
 	[UIDataObjKind(ImeIOTConst.SubSysID_Str, "{7A97DA63-5E4A-4B5F-A6EC-2F4683E686C9}")]
 	internal class UIDataObjStatusInfoPlugin : GriffinsPluginMngClass, IUIDataObjKindPlugin
 	{
 		/// <summary>
 		/// 界面数据对象种类名称（用于界面展示）
 		/// </summary>
 		string IUIDataObjKindPlugin.ObjKindName
 		{
 			get { return ResourceNames.ResourceManager.GetString("StatusInfo") ?? "状态信息"; }
 		}
 
        GFUIPropDefInfoList IUIDataObjKindPlugin.Props
            => GFPropObjBase.GetGFPropDefInfoes<UIDataObjStatusInfo>(ResourceNames.ResourceManager.GetString, null, null);

        /// <summary>
        /// 界面数据对象命令定义信息列表
        /// 当前状态信息图元不提供额外命令
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
