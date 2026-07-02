using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.BasicRobot.CompUI_BasicRobot.PageType.RecipeCfgPage
{
    internal class RecipeCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        protected override PageTypeID _GetPageTypeID() { return PageTypeID.Parse("RecipeCfgPage"); }

        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            return new PageTypeCompUIViewInfoList();
        }

        protected override object _GetPageTypeCompUIView(string viewID)
        {
            return null;
        }

        protected override InnerSubPageTypeInfoList _GetInnerSubPageTypeInfoes()
        {
            return new InnerSubPageTypeInfoList();
        }

        protected override IInnerSubPageDesignTime _CreateDesignTime(InnerSubPageTypeID innerSubPageTypeID)
        {
            return null;
        }
    }
}
