using Griffins.CompUI.WeighingBalance.CompUI_WeighingBalance.PageType.InitCfgPage.WeighingBalance.Views;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.WeighingBalance.CompUI_WeighingBalance.PageType.InitCfgPage
{
    internal class InitCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        protected override PageTypeID _GetPageTypeID() { return PageTypeID.Parse("InitCfgPage"); }

        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            return new PageTypeCompUIViewInfoList()
            {
                new PageTypeCompUIViewInfo() { ViewID = InitCfgPageTypeConst.ViewID_WeighingBalance, ViewName = "称重" },
            };
        }

        protected override object _GetPageTypeCompUIView(string viewID)
        {
            switch (viewID)
            {
                case InitCfgPageTypeConst.ViewID_WeighingBalance:
                    return new WeighingBalanceInitCfgCompUIView();
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
