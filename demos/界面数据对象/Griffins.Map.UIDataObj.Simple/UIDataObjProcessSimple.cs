using GF_Gereric;
using Griffins.ApplicServ.NorthIntf;
using Griffins.ImeIOT;
using Griffins.Map;
using Griffins.PF;
using System.Runtime.CompilerServices;

namespace Griffins.Map.UIDataObj.ImeIot
{
	[UIDataObjProcess_ObjKind("Simple")]
	internal class UIDataObjProcessSimple : GriffinsPluginMngClass, IUIDataObjProcess
	{
		private IUIDataObjProcessCallBack callBack;
		/// <summary>
		/// 自定义界面数据对象配置数据
		/// </summary>
		private UIDataObjProcessSimpleCfgInfo cfgInfo;

		#region IUIDataObjProcess 成员
		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="pluginFileName">插件文件名（包括所在的路径）</param>
		/// <param name="cfgData">自定义界面数据对象实例配置参数</param>
		/// <param name="iUIInterfaceCallBack">界面接口插件接口回调接口</param>
		void IUIDataObjProcess.Init(string pluginFileName, byte[] cfgData, IUIDataObjProcessCallBack iUIInterfaceCallBack)
		{
			this.callBack = iUIInterfaceCallBack;
			this.cfgInfo = new UIDataObjProcessSimpleCfgInfo();
			this.cfgInfo.FromBytes(cfgData);
			//测试用
			this.cfgInfo.YS_Alias = new MMAlias("test_ys1");
			this.cfgInfo.BL_Alias = new MMAlias("test_bl1");
			//自定义界面数据对象处理插件的数据来源可以有以下几种
			//第一：模组的界面数据对象属性值改变
			//第二：模组的自定义事件
			//第三：自定义的服务插件
			//订阅机械模组界面数据对象属性值改变处理
			this.callBack.RegisterInformInfoProcessDelegate(InformInfo_MMUIDataObjPropValChangedInfo.InfoKindID, doMMUIDataObjPropValChangedInfo);
			//订阅子机械模组界面数据对象属性值改变处理
			this.callBack.RegisterInformInfoProcessDelegate(InformInfo_SubMMUIDataObjPropValChangedInfo.InfoKindID, doSubMMUIDataObjPropValChangedInfo);
			//订阅机械模组自定义事件信息处理
			this.callBack.RegisterInformInfoProcessDelegate(InformInfo_MMCustomEventInfo.InfoKindID, doReceiveMMCustomEventInfo);
			//订阅子机械模组自定义事件信息处理
			this.callBack.RegisterInformInfoProcessDelegate(InformInfo_SubMMCustomEventInfo.InfoKindID, doReceiveSubCustomMMEventInfo);
		}

		/// <summary>
		/// 获取指定实例别名的界面数据对象属性值
		/// </summary>
		/// <param name="projectID">项目ID，如果用于网关，该项无效</param>
		/// <param name="alias">界面数据对象实例别名</param>
		/// <param name="objKind">界面数据对象种类</param>
		/// <param name="category">界面数据对象类别</param>
		/// <returns>指定实例别名的界面数据对象属性值列表</returns>
		GFBaseTypePropValueList IUIDataObjProcess.GetUIDataObjPropValues(string projectID, string alias, ManagingPointKind objKind, MPCategoryID category)
		{
			GFBaseTypePropValueList gFBaseTypePropValues = new GFBaseTypePropValueList();
			var svrProvider = new SvrImeIOTCallSvrProvider();
			IImeCompUIDataObjPropValRW iImeCompUIDataObjPropValRW = svrProvider.GetImeCompUIDataObjPropValRW();
			//从配置的机械模组实例别名中获取模组界面数据对象实例数据
			GFBaseTypePropValueList ysGFBaseTypePropValues = iImeCompUIDataObjPropValRW.GetUIDataObjPropValues(projectID, this.cfgInfo.YS_Alias.ToString(), UIDataObjProcessSimpleCfgInfo.YS_MPKind, ImeIOTConst.UIDataObjCategory_MM);
			GFBaseTypePropValueList blGFBaseTypePropValues = iImeCompUIDataObjPropValRW.GetUIDataObjPropValues(projectID, this.cfgInfo.BL_Alias.ToString(), UIDataObjProcessSimpleCfgInfo.BL_MPKind, ImeIOTConst.UIDataObjCategory_MM);
			//将模组的界面数据对象属性值转换为自定义界面数据对象属性值
			if (ysGFBaseTypePropValues != null && ysGFBaseTypePropValues.Count > 0)
			{
				foreach (GFBaseTypePropValue gFBaseTypePropValue in ysGFBaseTypePropValues)
				{
					if (gFBaseTypePropValue.PropertyID == MPPropertyID.Parse("Prop1"))
					{
						GFBaseTypePropValue addGFBaseTypePropValue = new GFBaseTypePropValue()
						{
							//将模组的界面数据对象属性值ID，转换为自定义界面数据对象种类的属性值ID
							PropertyID = MPPropertyID.Parse("Param3"),
							//将模组的界面数据对象属性值转换为自定义界面数据对象种类的属性值
							Value = convert(gFBaseTypePropValue.PropertyID, gFBaseTypePropValue.Value)
						};
						gFBaseTypePropValues.Add(addGFBaseTypePropValue);
					}
				}
			}
			//添加默认值
			else
			{
				GFBaseTypePropValue addGFBaseTypePropValue = new GFBaseTypePropValue()
				{
					PropertyID = MPPropertyID.Parse("Param3"),
					Value = new GriffinsBaseValue(0)
				};
				gFBaseTypePropValues.Add(addGFBaseTypePropValue);
			}
			//其他模组的数据待处理
			//
			return gFBaseTypePropValues;
		}
		/// <summary>
		/// 获取指定实例别名列表的界面数据对象属性值
		/// </summary>
		/// <param name="projectID">项目ID，如果用于网关，该项无效</param>
		/// <param name="aliases">界面数据对象实例别名列表</param>
		/// <param name="objKind">界面数据对象种类</param>
		/// <param name="category">界面数据对象类别</param>
		/// <returns>指定实例别名列表的界面数据对象属性值列表</returns>
		UIDataObjPropValueList IUIDataObjProcess.GetUIDataObjPropValues(string projectID, string[] aliases, ManagingPointKind objKind, MPCategoryID category)
		{
			//自定义界面数据对象一般不支持获取多个界面数据对象实例的属性值
			throw new NotSupportedException();
		}
		/// <summary>
		/// 获取指定实例别名的界面数据对象属性当前值
		/// </summary>
		/// <param name="projectID">项目ID，如果用于网关，该项无效</param>
		/// <param name="alias">界面数据对象实例别名</param>
		/// <param name="mPPropertyID">界面数据对象属性ID</param>
		/// <param name="objKind">界面数据对象种类</param>
		/// <param name="category">界面数据对象类别</param>
		/// <returns>指定实例别名的界面数据对象属性当前值</returns>
		GriffinsBaseValue IUIDataObjProcess.GetUIDataObjPropValue(string projectID, string alias, MPPropertyID mPPropertyID, ManagingPointKind objKind, MPCategoryID category)
		{
			//自定义界面数据对象处理程序自己要自己的属性来源
			var svrProvider = new SvrImeIOTCallSvrProvider();
			IImeCompUIDataObjPropValRW iImeCompUIDataObjPropValRW = svrProvider.GetImeCompUIDataObjPropValRW();
			if (mPPropertyID== MPPropertyID.Parse("Param3"))
			{
				GriffinsBaseValue griffinsBaseValue = iImeCompUIDataObjPropValRW.GetUIDataObjPropValue(projectID, this.cfgInfo.YS_Alias.ToString(), MPPropertyID.Parse("Prop1"), UIDataObjProcessSimpleCfgInfo.YS_MPKind, ImeIOTConst.UIDataObjCategory_MM);
				return convert(MPPropertyID.Parse("Prop1"), griffinsBaseValue);
			}
			return null;
		}
		/// <summary>
		/// 获取指定实例别名一组属性当前值
		/// </summary>
		/// <param name="projectID">项目ID，如果用于网关，该项无效</param>
		/// <param name="alias">界面数据对象实例别名</param>
		/// <param name="mPPropertyIDs">界面数据对象属性ID列表</param>
		/// <param name="objKind">界面数据对象种类</param>
		/// <param name="category">界面数据对象类别</param>
		/// <returns>指定实例别名一组属性当前值列表</returns>
		GFBaseTypePropValueList IUIDataObjProcess.GetUIDataObjPropValues(string projectID, string alias, MPPropertyID[] mPPropertyIDs, ManagingPointKind objKind, MPCategoryID category)
		{
			//需要自己实现
			throw new NotSupportedException();
		}
		/// <summary>
		/// 设置界面数据对象属性值
		/// </summary>
		/// <param name="projectID">项目ID，如果用于网关，该项无效</param>
		/// <param name="uiDataObjPropValue">界面数据对象属性值</param>
		/// <param name="objKind">界面数据对象种类</param>
		/// <param name="category">界面数据对象类别</param>
		void IUIDataObjProcess.SetUIDataObjPropValue(string projectID, UIDataObjPropValue uiDataObjPropValue, ManagingPointKind objKind, MPCategoryID category)
		{
			var svrProvider = new SvrImeIOTCallSvrProvider();
			IImeCompUIDataObjPropValRW iImeCompUIDataObjPropValRW = svrProvider.GetImeCompUIDataObjPropValRW();
			if (uiDataObjPropValue.PropertyID == MPPropertyID.Parse("Param3"))
			{
				uiDataObjPropValue.Alias = this.cfgInfo.YS_Alias.ToString();
				uiDataObjPropValue.PropertyID = MPPropertyID.Parse("Prop1");
				uiDataObjPropValue.Value = new GriffinsBaseValue("5");//需要将自定义界面对象属性值转换为模组的
				iImeCompUIDataObjPropValRW.SetUIDataObjPropValue(projectID, uiDataObjPropValue, UIDataObjProcessSimpleCfgInfo.YS_MPKind, ImeIOTConst.UIDataObjCategory_MM);
			}
		}
		/// <summary>
		/// 设置一组界面数据对象属性值
		/// </summary>
		/// <param name="projectID">项目ID，如果用于网关，该项无效</param>
		/// <param name="uiDataObjPropValues">界面数据对象属性值列表</param>
		/// <param name="objKind">界面数据对象种类</param>
		/// <param name="category">界面数据对象类别</param>
		void IUIDataObjProcess.SetUIDataObjPropValues(string projectID, UIDataObjPropValueList uiDataObjPropValues, ManagingPointKind objKind, MPCategoryID category)
		{
			UIDataObjPropValueList ysUIDataObjPropValues = new UIDataObjPropValueList();
			UIDataObjPropValueList blUIDataObjPropValues = new UIDataObjPropValueList();
			foreach (UIDataObjPropValue uIDataObjPropValue in uiDataObjPropValues)
			{
				if (uIDataObjPropValue.PropertyID == MPPropertyID.Parse("Param3"))
				{
					uIDataObjPropValue.Alias = this.cfgInfo.YS_Alias.ToString();
					uIDataObjPropValue.PropertyID = MPPropertyID.Parse("Prop1");
					uIDataObjPropValue.Value = new GriffinsBaseValue("5");//需要将自定义界面对象属性值转换为模组的
					ysUIDataObjPropValues.Add(uIDataObjPropValue);
				}
			}
			var svrProvider = new SvrImeIOTCallSvrProvider();
			IImeCompUIDataObjPropValRW iImeCompUIDataObjPropValRW = svrProvider.GetImeCompUIDataObjPropValRW();
			if (ysUIDataObjPropValues.Count > 0)
			{
				iImeCompUIDataObjPropValRW.SetUIDataObjPropValues(projectID, ysUIDataObjPropValues, UIDataObjProcessSimpleCfgInfo.YS_MPKind, ImeIOTConst.UIDataObjCategory_MM);
			}
			if (blUIDataObjPropValues.Count > 0)
			{
				iImeCompUIDataObjPropValRW.SetUIDataObjPropValues(projectID, blUIDataObjPropValues, UIDataObjProcessSimpleCfgInfo.BL_MPKind, ImeIOTConst.UIDataObjCategory_MM);
			}
		}

		/// <summary>
		///  执行界面数据对象命令
		/// </summary>
		/// <param name="projectID">项目ID，如果用于网关，该项无效</param>
		/// <param name="alias">组件界面数据对象实例别名</param>
		/// <param name="cmdID">命令ID</param>
		/// <param name="cmdParam">命令参数</param>
		/// <param name="objKind">界面数据对象种类</param>
		/// <param name="category">界面数据对象类别</param>
		/// <returns>命令返回值</returns>
		GFBaseTypeParamValueList IUIDataObjProcess.ExecUIDataObjCommand(string projectID, string alias, string cmdID, GFBaseTypeParamValueList cmdParam, ManagingPointKind objKind, MPCategoryID category)
		{
			if (cmdID == "MethID1") 
			{
				var svrProvider = new SvrImeIOTCallSvrProvider();
				IImeCompUIDataObjPropValRW iImeCompUIDataObjPropValRW = svrProvider.GetImeCompUIDataObjPropValRW();
				return iImeCompUIDataObjPropValRW.ExecUIDataObjCommand(projectID, this.cfgInfo.YS_Alias.ToString(), cmdID, cmdParam, UIDataObjProcessSimpleCfgInfo.YS_MPKind, ImeIOTConst.UIDataObjCategory_MM);
			}
			return new GFBaseTypeParamValueList();
		}

		#endregion IUIDataObjProcess 成员

		/// <summary>
		/// 机械模组界面数据对象属性值改变信息
		/// </summary>
		/// <param name="infoKind">信息种类</param>
		/// <param name="infoNo">信息编号</param>
		/// <param name="info">信息内容对象</param>
		private void doMMUIDataObjPropValChangedInfo(GriffinsInfoKindID infoKind, string infoNo, InformInfoBase info)
		{
			//把转发的数据转换回来
			var informInfo = info as InformInfo_MMUIDataObjPropValChangedInfo;
			if (informInfo.MMAlias == this.cfgInfo.YS_Alias)
			{
				if (informInfo.PropertyID == MPPropertyID.Parse("Prop1"))
				{
					var uiDataObjPropValue = new UIDataObjPropValue()
					{
						Alias = this.cfgInfo.YS_Alias.ToString(),
						TimeStamp = informInfo.RaiseTime,
						//将模组的界面数据对象属性值ID，转换为自定义界面数据对象种类的属性值ID
						PropertyID = MPPropertyID.Parse("Param3"),
						//将模组的界面数据对象属性值转换为自定义界面数据对象种类的属性值
						Value = convert(informInfo.PropertyID,informInfo.Val)
					};
					this.callBack.AsynSendUIDataObjPropValueToMapTml("", uiDataObjPropValue);
				}
			}			
		}


		private GriffinsBaseValue convert(MPPropertyID mPPropertyID, GriffinsBaseValue griffinsBaseValue) 
		{
			//将模组界面数据对象属性ID对应的属性值转换为自定义界面数据对象属性值
			return griffinsBaseValue; 
		}

		/// <summary>
		/// 机械模组界面数据对象属性值改变信息
		/// </summary>
		/// <param name="infoKind">信息种类</param>
		/// <param name="infoNo">信息编号</param>
		/// <param name="info">信息内容对象</param>
		private void doSubMMUIDataObjPropValChangedInfo(GriffinsInfoKindID infoKind, string infoNo, InformInfoBase info)
		{
			return;
			//把转发的数据转换回来
			//var informInfo = info as InformInfo_SubMMUIDataObjPropValChangedInfo;
			//var uiDataObjPropValue = new UIDataObjPropValue()
			//{
			//	Alias = informInfo.SubMMAlias.ToString(),
			//	TimeStamp = informInfo.RaiseTime,
			//	PropertyID = informInfo.PropertyID,
			//	Value = informInfo.Val
			//};
			//this.callBack.AsynSendUIDataObjPropValueToMapTml("", uiDataObjPropValue);
		}

		/// <summary>
		/// 机械模组自定义事件信息处理
		/// </summary>
		/// <param name="infoKind"></param>
		/// <param name="infoNo"></param>
		/// <param name="info"></param>
		private void doReceiveMMCustomEventInfo(GriffinsInfoKindID infoKind, string infoNo, InformInfoBase info)
		{
			var informInfo = (InformInfo_MMCustomEventInfo)info;
			if (informInfo.MMAlias == this.cfgInfo.YS_Alias)
			{

			}
		}

		/// <summary>
		/// 子机械模组自定义事件信息处理
		/// </summary>
		/// <param name="infoKind"></param>
		/// <param name="infoNo"></param>
		/// <param name="info"></param>
		private void doReceiveSubCustomMMEventInfo(GriffinsInfoKindID infoKind, string infoNo, InformInfoBase info)
		{
			return;
			//var informInfo = (InformInfo_SubMMCustomEventInfo)info;
			//INorthCall iNorthCall = NorthCallFactory.CreateNorthCall();
			//ISendInfoToMapTml iSendInfoToMapTml = iNorthCall.CreateSendInfoToMapTml();
			//iSendInfoToMapTml.AsynSendToMapTmlOfKind(ImeIOTConst.ImeMonitor, InformInfo_MMEventInfo.InfoKindID, infoNo, informInfo.ToJson());
		}

		#region 内部类型
		/// <summary>
		/// 自定义界面数据对象配置信息
		/// </summary>
		private class UIDataObjProcessSimpleCfgInfo
		{
			/// <summary>
			/// 印刷机械模组界面数据对象种类
			/// </summary>
			public static readonly ManagingPointKind YS_MPKind = ManagingPointKind.Parse("test_ys");
			/// <summary>
			/// 备料机械模组界面数据对象种类
			/// </summary>
			public static readonly ManagingPointKind BL_MPKind = ManagingPointKind.Parse("test_bl");
			/// <summary>
			/// 印刷机械模组实例别名
			/// </summary>
			public MMAlias YS_Alias { get; set; }
			/// <summary>
			/// 备料机械模组实例别名
			/// </summary>
			public MMAlias BL_Alias { get; set; }

			/// <summary>
			/// 转为字节数组
			/// </summary>
			/// <returns>字节数组</returns>
			public byte[] ToBytes()
			{
				return JsonObjConvert.ToJSonBytes(this);
			}

			/// <summary>
			/// 从字节数组转为对象
			/// </summary>
			/// <param name="data"></param>
			public void FromBytes(byte[] data)
			{
				if (data == null)
				{
					return;
				}
				JsonObjConvert.PopulateObject(data, this);
			}
		}

		#endregion
	}
}
