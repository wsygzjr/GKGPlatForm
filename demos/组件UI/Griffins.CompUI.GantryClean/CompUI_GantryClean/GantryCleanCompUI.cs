using Griffins.CompUI.GantryClean.CompUI_GantryClean.PageType.InitCfgPage;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using GKG.SubMM;
using System;

namespace Griffins.CompUI.GantryClean.CompUI_GantryClean
{
    /// <summary>
    /// 龙门清洁 CompUI 插件：提供初始化配置页的设计态/运行态界面。
    /// </summary>
    [CompUI("GantryClean", "SubMM")]
    public class GantryCleanCompUI : CompUIBase
    {
        protected override string _GetCompName() { return "清洁位置设置"; }

        protected override IPageTypeDesignCompUI _GetPageTypeDesignCompUI(PageTypeID pageTypeID, Guid subMMObjID)
        {
            if (subMMObjID != GantryCleanSubMachineModulesConst.SubMMObjID)
            {
                return null;
            }

            if (pageTypeID == PageTypeID.Parse("InitCfgPage"))
            {
                return new InitCfgPageTypeDesignCompUI();
            }

            return null;
        }

        protected override IPageTypeRunTimeCompUI _GetPageTypeRunTimeCompUI(PageTypeID pageTypeID, Guid subMMObjID)
        {
            if (subMMObjID != GantryCleanSubMachineModulesConst.SubMMObjID)
            {
                return null;
            }

            if (pageTypeID == PageTypeID.Parse("InitCfgPage"))
            {
                return new InitCfgPageTypeRunTimeCompUI();
            }

            return null;
        }
    }
}
