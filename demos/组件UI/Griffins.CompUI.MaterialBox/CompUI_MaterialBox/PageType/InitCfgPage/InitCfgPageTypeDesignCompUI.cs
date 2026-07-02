using Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.InitCfgPage.MaterialBoxInitConfig.Views;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.InitCfgPage
{
    /// <summary>
    /// 初始化页设计态入口，负责提供初始化页面的设计态视图。
    /// </summary>
    internal class InitCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        /// <summary>返回初始化页的页面类型 ID。</summary>
        protected override PageTypeID _GetPageTypeID()
        {
            return ImeIOTConst.InitCfgPage;
        }

        /// <summary>返回初始化页下的视图列表。</summary>
        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            return new PageTypeCompUIViewInfoList
            {
                new PageTypeCompUIViewInfo
                {
                    ViewID = InitCfgPageTypeConst.ViewID_MaterialBoxInitConfig,
                    ViewName = "MaterialBox初始化配置",
                }
            };
        }

        /// <summary>根据视图 ID 创建对应的初始化设计态界面。</summary>
        protected override object _GetPageTypeCompUIView(string viewID)
        {
            return viewID switch
            {
                InitCfgPageTypeConst.ViewID_MaterialBoxInitConfig => new MaterialBoxInitConfigCompUIView(),
                _ => null,
            };
        }
    }
}
