using Griffins.CompUI.FixingStructure.CompUI_FixingStructure.PageType.InitCfgPage.AxisFixInitConfig.Views;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.FixingStructure.CompUI_FixingStructure.PageType.InitCfgPage
{
    /// <summary>
    /// 电机固定机构初始化页设计态入口。
    /// </summary>
    internal class InitCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        protected override PageTypeID _GetPageTypeID()
        {
            return ImeIOTConst.InitCfgPage;
        }

        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            return new PageTypeCompUIViewInfoList
            {
                new PageTypeCompUIViewInfo
                {
                    ViewID = InitCfgPageTypeConst.ViewID_AxisFixInitConfig,
                    ViewName = "电机固定机构初始化配置",
                },
            };
        }

        protected override object _GetPageTypeCompUIView(string viewID)
        {
            return viewID switch
            {
                InitCfgPageTypeConst.ViewID_AxisFixInitConfig => new AxisFixInitConfigCompUIView(),
                _ => null,
            };
        }
    }
}
