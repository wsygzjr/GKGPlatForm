using GF_Gereric;
using Griffins;
using Griffins.ImeIOT;
using Griffins.Map;
using System.Reflection;

namespace GKG.Map.UIDataObj.ProductionInfo
{
	[UIDataObjKind(ImeIOTConst.SubSysID_Str, "ProductionInfo")]
	internal class UIDataObjProductionInfoPlugin : GriffinsPluginMngClass, IUIDataObjKindPlugin
    {
        /// <summary>
        /// 界面数据对象种类名称
        /// </summary>
        string IUIDataObjKindPlugin.ObjKindName => ResourceNames.ProductionInformation;

        /// <summary>
        ///  界面数据对象属性定义信息列表
        /// </summary>
        GFUIPropDefInfoList IUIDataObjKindPlugin.Props
            => GFPropObjBase.GetGFPropDefInfoes<UIDataObjProductionInfo>(ResourceNames.ResourceManager.GetString, UIDataObjProductionInfo.GetValueRangeEnums, UIDataObjProductionInfo.GetValueNamePairs);
        
        /// <summary>
        /// 界面数据对象命令定义信息列表
        /// </summary>
        GFMethodDefInfoList IUIDataObjKindPlugin.Commands 
            => new GFMethodDefInfoList
            {
                new GFMethodDefInfo
                (
                    methodID: "GetAllFormulaNumberes", 
                    methodName: "获取所有配方编号", 
                    paramDefInfoes: new GFParamDefInfoList(),
                    retValDefInfoes: new GFParamDefInfoList()
                ),

                new GFMethodDefInfo
                (
                    methodID: "SetCurFormulaNumber",
                    methodName: "设置当前配方编号",
                    paramDefInfoes: new GFParamDefInfoList
                                    {
                                        new GFParamDefInfo("FormulaNumber", "配方编号", GriffinsBaseDataType.String)
                                    },
                    retValDefInfoes: new GFParamDefInfoList()
                )
            };


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
                    return null!;
                string objInstPropPathStr = objInstPropPath.ToString();

                if (proIDPath.Length == 2 && nameof(UIDataObjProductionInfo.Datas.Lanes) == proIDPath[1])
                {
                    return createSlotObjNameDic(); ;
                }
                return null!;
            }

            /// <summary>
            /// 生成料槽ID和名称字典
            /// </summary>
            /// <returns></returns>
            /// <exception cref="NotImplementedException"></exception>
            private Dictionary<string, string> createSlotObjNameDic()
            {
                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                string laneObjName = "测试对象";
                for (int index = 0; index < 2; index++)
                {
                    keyValuePairs.Add($"laneObj{index}", $"{laneObjName}{index}");
                }
                return keyValuePairs;
            }
        }

    }
}
