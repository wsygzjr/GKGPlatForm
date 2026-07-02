using Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.RecipeCfgPage.MaterialBoxConfig.Views;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.RecipeCfgPage
{
    /// <summary>
    /// 配方页设计态入口，负责向宿主提供配方页面的设计态视图信息。
    /// </summary>
    internal class RecipeCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        /// <summary>返回配方页的页面类型 ID。</summary>
        protected override PageTypeID _GetPageTypeID() { return ImeIOTConst.PPCfgPage; }

        /// <summary>返回配方页下可展示的设计态视图列表。</summary>
        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            return new PageTypeCompUIViewInfoList()
            {
                new PageTypeCompUIViewInfo() { ViewID = RecipeCfgPageTypeConst.ViewID_MaterialBoxConfig, ViewName = "MaterialBox配置" },
            };
        }

        /// <summary>根据视图 ID 创建对应的设计态页面。</summary>
        protected override object _GetPageTypeCompUIView(string viewID)
        {
            return viewID switch
            {
                RecipeCfgPageTypeConst.ViewID_MaterialBoxConfig => new MaterialBoxConfigCompUIView(),
                _ => null,
            };
        }
    }
}
