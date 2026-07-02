using GKG.Vision;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.Vision.CompUI.PageType.FactoryCfgPage
{
    internal class FactoryCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        protected override PageTypeID _GetPageTypeID() { return PageTypeID.Parse("FactoryCfgPage"); }
        IVisionDriverParameterEditor parameterEditor;
        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            return new PageTypeCompUIViewInfoList()
            {
                new PageTypeCompUIViewInfo() { ViewID = FactoryCfgPageTypeConst.ViewID_Vision, ViewName = "视觉" },
            };
        }

        protected override object _GetPageTypeCompUIView(string viewID)
        {
            //GFBaseTypeParamValueList rtn = CallBack.ExecConfigSvrCtlCmd("GetPluginName", new GFBaseTypeParamValueList());
            string PluginName = "";
            if (parameterEditor == null)
            {
                parameterEditor = VisionPluginManager.GetVisionDriverParameterEditor(PluginName);
            }
            return viewID switch
            {

                FactoryCfgPageTypeConst.ViewID_Vision => parameterEditor.View,
                _ => null,
            };
        }
    }
}
