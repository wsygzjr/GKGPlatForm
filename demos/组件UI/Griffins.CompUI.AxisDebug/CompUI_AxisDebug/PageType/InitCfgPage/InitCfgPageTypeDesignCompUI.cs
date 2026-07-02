using Griffins.Map;
using Griffins.Map.UI;
using Griffins.CompUI.AxisDebug.CompUI_AxisDebug.PageType.InitCfgPage.AxisDebugging.Views;
using Griffins.CompUI.AxisDebug.CompUI_AxisDebug.PageType.InitCfgPage.IOInDebugging.Views;
using Griffins.CompUI.AxisDebug.CompUI_AxisDebug.PageType.InitCfgPage.IOOutDebugging.Views;
using Griffins.ImeIOT.Map;

namespace Griffins.CompUI.AxisDebug.CompUI_AxisDebug.PageType.InitCfgPage
{
    internal class InitCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        protected override PageTypeID _GetPageTypeID() { return PageTypeID.Parse("InitCfgPage"); }

        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            return new PageTypeCompUIViewInfoList()
            {
                new PageTypeCompUIViewInfo() { ViewID = InitCfgPageTypeConst.ViewID_AxisDebugging, ViewName = "轴调试" },
                new PageTypeCompUIViewInfo() { ViewID = InitCfgPageTypeConst.ViewID_IOInDebugging, ViewName = "IO输入调试" },
                new PageTypeCompUIViewInfo() { ViewID = InitCfgPageTypeConst.ViewID_IOOutDebugging, ViewName = "IO输出调试" },
            };
        }

        protected override object _GetPageTypeCompUIView(string viewID)
        {
            return viewID switch
            {
                InitCfgPageTypeConst.ViewID_AxisDebugging => new AxisDebuggingCompUIView(),
                InitCfgPageTypeConst.ViewID_IOInDebugging => new IOInDebuggingCompUIView(),
                InitCfgPageTypeConst.ViewID_IOOutDebugging => new IOOutDebuggingCompUIView(),
                _ => null,
            };
        }
    }
}
