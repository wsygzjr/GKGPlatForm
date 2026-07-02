using Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.FactoryCfgPage.MaterialBoxFactory.Views;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.FactoryCfgPage
{
    /// <summary>
    /// 工厂配置页设计态入口，负责提供设计时可见的页面信息。
    /// </summary>
    internal class FactoryCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        /// <summary>工厂配置页的页面类型 ID。</summary>
        private static readonly PageTypeID FactoryCfgPageTypeID = PageTypeID.Parse("FactoryCfgPage");

        /// <summary>返回当前设计页对应的页面类型 ID。</summary>
        protected override PageTypeID _GetPageTypeID()
        {
            return FactoryCfgPageTypeID;
        }

        /// <summary>返回工厂配置页下所有可选视图信息。</summary>
        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            return new PageTypeCompUIViewInfoList
            {
                new PageTypeCompUIViewInfo
                {
                    ViewID = FactoryCfgPageTypeConst.ViewID_MaterialBoxFactory,
                    ViewName = "料口配置",
                }
            };
        }

        /// <summary>根据视图 ID 创建对应的设计态界面。</summary>
        protected override object _GetPageTypeCompUIView(string viewID)
        {
            return viewID == FactoryCfgPageTypeConst.ViewID_MaterialBoxFactory
                ? new MaterialBoxFactoryCompUIView()
                : null;
        }
    }
}
