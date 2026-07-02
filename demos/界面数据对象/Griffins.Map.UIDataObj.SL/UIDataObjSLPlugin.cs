using GF_Gereric;
using Griffins.Map;

namespace Griffins.Map.UIDataObj.SL
{
	[UIDataObjKind(ImeIOTConst.SubSysID_Str, "SL", "Custom")]
	internal class UIDataObjSLPlugin : GriffinsPluginMngClass, IUIDataObjKindPlugin
    {
        private GFMethodDefInfoList? gFMethodDefInfos;

        public UIDataObjSLPlugin()
        {
            createGFMethodDefInfos();
        }
        /// <summary>
        /// 界面数据对象种类名称
        /// </summary>
        string IUIDataObjKindPlugin.ObjKindName 
		{
			get { return ResourceNames.SL; } 
		}

        /// <summary>
        ///  界面数据对象属性定义信息列表
        /// </summary>
        GFPropDefInfoList IUIDataObjKindPlugin.Props 
        {
            get { return GFPropObjBase.GetGFPropDefInfoes<UIDataObjSL>(ResourceNames.ResourceManager.GetString); }
        }
        /// <summary>
        /// 界面数据对象命令定义信息列表
        /// </summary>
        GFMethodDefInfoList? IUIDataObjKindPlugin.Commands
        {
            get { return gFMethodDefInfos; }
        }
      
        private void createGFMethodDefInfos()
        {
            gFMethodDefInfos = new GFMethodDefInfoList();
            GFMethodDefInfo gFMethodDefInfo = new GFMethodDefInfo();
            gFMethodDefInfo.MethodID = "TestCmd";
            gFMethodDefInfo.MethodName = ResourceNames.TestCmd;
            gFMethodDefInfo.ParamDefInfoes=new GFParamDefInfoList();
            gFMethodDefInfo.ParamDefInfoes.Add(new GFParamDefInfo()
            {
                ParamID = "Param1",
                ParamName = ResourceNames.Param1,
                DataType= GriffinsBaseDataType.Integer
            });
            gFMethodDefInfo.ParamDefInfoes.Add(new GFParamDefInfo()
            {
                ParamID = "Param2",
                ParamName = ResourceNames.Param2,
                DataType = GriffinsBaseDataType.String
            });
            gFMethodDefInfo.ParamDefInfoes.Add(new GFParamDefInfo()
            {
                ParamID = "Param3",
                ParamName = ResourceNames.Param4,
                DataType = GriffinsBaseDataType.Bool
            });

            gFMethodDefInfo.RetValDefInfoes = new GFParamDefInfoList();
            gFMethodDefInfo.RetValDefInfoes.Add(new GFParamDefInfo()
            {
                ParamID = "Param1",
                ParamName = ResourceNames.Param1,
                DataType = GriffinsBaseDataType.Integer
            });
            gFMethodDefInfos.Add(gFMethodDefInfo);
        }
    }
}
