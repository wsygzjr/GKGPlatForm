using GF_Gereric;
using Griffins.ImeIOT;
using Griffins.Map;

namespace Griffins.Map.UIDataObj.Simple
{
	[UIDataObjKind(ImeIOTConst.SubSysID_Str, "Simple")]
	internal class UIDataObjSimplePlugin : GriffinsPluginMngClass, IUIDataObjKindPlugin
    {
        /// <summary>
        /// 界面数据对象种类名称
        /// </summary>
        string IUIDataObjKindPlugin.ObjKindName 
		{
			get { return ResourceNames.Simple; } 
		}

		/// <summary>
		///  界面数据对象属性定义信息列表
		/// </summary>
		GFUIPropDefInfoList IUIDataObjKindPlugin.Props 
        {
            get { return GFPropObjBase.GetGFPropDefInfoes<UIDataObjSimple>(ResourceNames.ResourceManager.GetString, UIDataObjSimple.GetValueRangeEnums, UIDataObjSimple.GetValueNamePairs); }
        }
        /// <summary>
        /// 界面数据对象命令定义信息列表
        /// </summary>
        GFMethodDefInfoList IUIDataObjKindPlugin.Commands
        {
			get 
            { 
                return new GFMethodDefInfoList() 
                {
                    new GFMethodDefInfo()
                    {
                        MethodID="MethID1",
                        MethodName="命令1",
                        ParamDefInfoes=new GFParamDefInfoList()
                        {
                            new GFParamDefInfo()
                            {
                                ParamID="Param1",
                                ParamName="参数1",
                                DataType=GriffinsBaseDataType.String,
                                ObjectID=Guid.Empty,
                            }
                        },
                        RetValDefInfoes=new GFParamDefInfoList()
                        {
                            
                        }
                    },
					new GFMethodDefInfo()
					{
						MethodID="MethID2",
						MethodName="命令2",
						ParamDefInfoes=new GFParamDefInfoList()
						{
							new GFParamDefInfo()
							{
								ParamID="Param2",
								ParamName="参数2",
								DataType=GriffinsBaseDataType.String,
								ObjectID=Guid.Empty,
							}
						},
						RetValDefInfoes=new GFParamDefInfoList()
						{

						}
					},
				}; 
            }
        }

        /// <summary>
        /// 创建界面数据对象种类服务接口实例
        /// </summary>
        /// <param name="cfgData">界面数据对象配置信息</param>
        /// <returns>界面数据对象种类服务接口实例</returns>
        IUIDataObjKindDefSvr IUIDataObjKindPlugin.CreateIUIDataObjKindDefSvr(byte[] cfgData)
        {
            return new UIDataObjKindDefSvr(cfgData);
        }

        private class UIDataObjKindDefSvr : IUIDataObjKindDefSvr
        {
            private byte[] cfgData;
            public UIDataObjKindDefSvr(byte[] cfgData) 
            {
                this.cfgData = cfgData;
            }
			/// <summary>
			/// 获取子界面数据对象项名称字典
			/// </summary>
			/// <param name="objInstPropPath">界面数据对象属性路径</param>
			/// <returns>子界面数据对象项名称字典</returns>
			Dictionary<string, string> IUIDataObjKindDefSvr.GetSubUIProObjItemNames(ObjInstPropPath objInstPropPath)
            {
                var proIDPath = objInstPropPath.PropIDPath;
                if (proIDPath == null || proIDPath.Length == 0)
                    return null;
                string objInstPropPathStr = objInstPropPath.ToString();
                //objInstPropPathStr:TestObjs
                if (proIDPath.Length == 1 && nameof(UIDataObjSimple.TestObjs) == proIDPath[0])
                {
                    return createSlotObjNameDic(); ;
                }
                return null;
            }
           
            /// <summary>
            /// 生成料槽ID和名称字典
            /// </summary>
            /// <returns></returns>
            /// <exception cref="NotImplementedException"></exception>
            private Dictionary<string, string> createSlotObjNameDic()
            {
                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                string slotObjName = "测试对象";
                for (int index = 0; index < 2; index++)
                {
                    keyValuePairs.Add($"slotObj{index}", $"{slotObjName}{index}");
                }
                return keyValuePairs;
            }
        }
	}
}
