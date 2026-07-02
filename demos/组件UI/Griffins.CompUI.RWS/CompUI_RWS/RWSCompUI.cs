using Griffins.CompUI.RWS.CompUI_RWS.PageType.DebugPage;
using Griffins.CompUI.RWS.CompUI_RWS.PageType.FactoryCfgPage;
using Griffins.CompUI.RWS.CompUI_RWS.PageType.InitCfgPage;
using Griffins.CompUI.RWS.CompUI_RWS.PageType.RecipeCfgPage;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using GKG.SubMM;
using System;

namespace Griffins.CompUI.RWS.CompUI_RWS
{
    /// <summary>
    /// 轨道工位 CompUI 插件：提供出厂、初始化、配方与调试页的设计态/运行态界面。
    /// </summary>
    [CompUI("RailWorkStation", "SubMM")]
    public class RWSCompUI : CompUIBase
    {
        protected override string _GetCompName() { return "轨道工位"; }

        protected override IPageTypeDesignCompUI _GetPageTypeDesignCompUI(PageTypeID pageTypeID, Guid subMMObjID)
        {
            if (subMMObjID != RailWorkStationSubMachineModulesConst.SubMMObjInfos[0].SubMMObjID)
            {
                return null;
            }

            if (pageTypeID == PageTypeID.Parse("FactoryCfgPage"))
            {
                return new FactoryCfgPageTypeDesignCompUI();
            }
            if (pageTypeID == PageTypeID.Parse("InitCfgPage"))
            {
                return new InitCfgPageTypeDesignCompUI();
            }
            if (pageTypeID == ImeIOTConst.PPCfgPage)
            {
                return new RecipeCfgPageTypeDesignCompUI();
            }
            if (pageTypeID == PageTypeID.Parse("DebugPage"))
            {
                return new DebugPageTypeDesignCompUI();
            }

            return null;
        }

        protected override IPageTypeRunTimeCompUI _GetPageTypeRunTimeCompUI(PageTypeID pageTypeID, Guid subMMObjID)
        {
            if (subMMObjID != RailWorkStationSubMachineModulesConst.SubMMObjInfos[0].SubMMObjID)
            {
                return null;
            }

            if (pageTypeID == PageTypeID.Parse("FactoryCfgPage"))
            {
                return new FactoryCfgPageTypeRunTimeCompUI();
            }
            if (pageTypeID == PageTypeID.Parse("InitCfgPage"))
            {
                return new InitCfgPageTypeRunTimeCompUI();
            }
            if (pageTypeID == ImeIOTConst.PPCfgPage)
            {
                return new RecipeCfgPageTypeRunTimeCompUI();
            }

            return null;
        }
    }
}
