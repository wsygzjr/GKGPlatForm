using GF_Gereric;
using Griffins.Map;
using Griffins.ImeIOT;

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

        GFUIPropDefInfoList IUIDataObjKindPlugin.Props => throw new NotImplementedException();

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
                        MethodID="123",
                        MethodName="测试命令",
                        ParamDefInfoes=new GFParamDefInfoList()
                        {
                            new GFParamDefInfo("Param1","参数1",GriffinsBaseDataType.Integer),
                            new GFParamDefInfo("Param2","参数2",GriffinsBaseDataType.String),
                            new GFParamDefInfo("Param3","参数3",GriffinsBaseDataType.Decimal),
							new GFParamDefInfo("Param4","参数4",GriffinsBaseDataType.DateTime),
							new GFParamDefInfo("Param5","参数5",GriffinsBaseDataType.Guid),
							new GFParamDefInfo("Param6","参数6",GriffinsBaseDataType.Bool),
						},
                        RetValDefInfoes=new GFParamDefInfoList()
                        {
                            new GFParamDefInfo("RetParam1","参数1",GriffinsBaseDataType.String)
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
