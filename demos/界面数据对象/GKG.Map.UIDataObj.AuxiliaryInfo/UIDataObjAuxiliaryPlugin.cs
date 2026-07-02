namespace GKG.Map.UIDataObj.AuxiliaryInfo
{
	using GF_Gereric;
    using Griffins;
    using Griffins.ImeIOT;
	using Griffins.Map;

	/// <summary>
	/// 辅助信息界面数据对象的“种类插件”
	/// 负责向框架提供：种类显示名、属性定义列表（Props）、命令定义列表（Commands）
	/// </summary>
	[UIDataObjKind(ImeIOTConst.SubSysID_Str, "{B7CFD19C-7B3D-4F1B-B4F5-0F1D2F450F07}")]
	internal class UIDataObjAuxiliaryInfoPlugin : GriffinsPluginMngClass, IUIDataObjKindPlugin
	{
		/// <summary>
		/// 界面数据对象种类名称（用于界面展示）
		/// </summary>
		string IUIDataObjKindPlugin.ObjKindName
		{
			get { return ResourceNames.ResourceManager.GetString("AuxiliaryInfo") ?? "辅助信息"; }
		}

        GFUIPropDefInfoList IUIDataObjKindPlugin.Props
            => GFPropObjBase.GetGFPropDefInfoes<UIDataObjAuxiliaryInfo>(ResourceNames.ResourceManager.GetString, null, null);

        /// <summary>
        /// 界面数据对象命令定义信息列表
        /// 这里的命令主要用于在 UI 层触发功能图元的对应动作（具体执行由图元/后端实现）
        /// </summary>
        GFMethodDefInfoList IUIDataObjKindPlugin.Commands
		{
			get
			{
				return new GFMethodDefInfoList()
				{
					new GFMethodDefInfo()
					{
						MethodID = "SaveParams",
						MethodName = (ResourceNames.ResourceManager.GetString("SaveParams") ?? "保存参数").Replace("\b", ""),
						ParamDefInfoes = new GFParamDefInfoList(),
						RetValDefInfoes = new GFParamDefInfoList()
						{
							new GFParamDefInfo
							{
								ParamID = "Result",
								ParamName = (ResourceNames.ResourceManager.GetString("Result") ?? "结果").Replace("\b", ""),
								DataType = GriffinsBaseDataType.Bool,
								ObjectID = Guid.Empty
							}
						}
					},
					new GFMethodDefInfo()
					{
						MethodID = "VerifyProgram",
						MethodName = (ResourceNames.ResourceManager.GetString("VerifyProgram") ?? "校验程序").Replace("\b", ""),
						ParamDefInfoes = new GFParamDefInfoList(),
						RetValDefInfoes = new GFParamDefInfoList()
						{
							new GFParamDefInfo
							{
								ParamID = "Result",
								ParamName = (ResourceNames.ResourceManager.GetString("Result") ?? "结果").Replace("\b", ""),
								DataType = GriffinsBaseDataType.Bool,
								ObjectID = Guid.Empty
							}
						}
					},
					new GFMethodDefInfo()
					{
						MethodID = "Online",
						MethodName = (ResourceNames.ResourceManager.GetString("Online") ?? "上线").Replace("\b", ""),
						ParamDefInfoes = new GFParamDefInfoList(),
						RetValDefInfoes = new GFParamDefInfoList()
						{
							new GFParamDefInfo
							{
								ParamID = "Result",
								ParamName = (ResourceNames.ResourceManager.GetString("Result") ?? "结果").Replace("\b", ""),
								DataType = GriffinsBaseDataType.Bool,
								ObjectID = Guid.Empty
							}
						}
					},
					new GFMethodDefInfo()
					{
						MethodID = "Offline",
						MethodName = (ResourceNames.ResourceManager.GetString("Offline") ?? "下线").Replace("\b", ""),
						ParamDefInfoes = new GFParamDefInfoList(),
						RetValDefInfoes = new GFParamDefInfoList()
						{
							new GFParamDefInfo
							{
								ParamID = "Result",
								ParamName = (ResourceNames.ResourceManager.GetString("Result") ?? "结果").Replace("\b", ""),
								DataType = GriffinsBaseDataType.Bool,
								ObjectID = Guid.Empty
							}
						}
					}
				};
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
