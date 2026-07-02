using Griffins.CompUI.CategoryARobot.CompUI_CategoryARobot.PageType.InitCfgPage.MechanicalArm.Views;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.CategoryARobot.CompUI_CategoryARobot.PageType.InitCfgPage
{
    internal class InitCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        protected override PageTypeID _GetPageTypeID() { return PageTypeID.Parse("InitCfgPage"); }

        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            return new PageTypeCompUIViewInfoList()
            {
                new PageTypeCompUIViewInfo() { ViewID = InitCfgPageTypeConst.ViewID_MechanicalArm, ViewName = "机械手" },
            };
        }

        protected override object _GetPageTypeCompUIView(string viewID)
        {
            switch (viewID)
            {
                case InitCfgPageTypeConst.ViewID_MechanicalArm:
                    return new MechanicalArmCompUIView();
                default:
                    return null;
            }
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
