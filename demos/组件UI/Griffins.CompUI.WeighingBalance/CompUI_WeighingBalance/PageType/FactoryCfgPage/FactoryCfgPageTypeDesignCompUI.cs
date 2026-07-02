using Griffins.CompUI.WeighingBalance.CompUI_WeighingBalance.PageType.FactoryCfgPage.WeighingBalanceFactory.Views;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.WeighingBalance.CompUI_WeighingBalance.PageType.FactoryCfgPage
{
    /// <summary>
    /// 出厂配置页面设计态组件。
    /// </summary>
    internal class FactoryCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        protected override PageTypeID _GetPageTypeID() { return ImeIOTConst.FactoryCfgPage; }

        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            return new PageTypeCompUIViewInfoList()
            {
                new PageTypeCompUIViewInfo() { ViewID = FactoryCfgPageTypeConst.ViewID_WeighingBalanceFactory, ViewName = "称重出厂配置" },
            };
        }

        protected override object _GetPageTypeCompUIView(string viewID)
        {
            return viewID switch
            {
                FactoryCfgPageTypeConst.ViewID_WeighingBalanceFactory => new WeighingBalanceFactoryCompUIView(),
                _ => null,
            };
        }
    }
}
