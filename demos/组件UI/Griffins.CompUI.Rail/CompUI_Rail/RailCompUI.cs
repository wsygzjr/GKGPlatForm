using System;
using Griffins.CompUI.Rail.CompUI_Rail.Interop;
using Griffins.CompUI.Rail.CompUI_Rail.PageType.InitCfgPage;
using Griffins.CompUI.Rail.CompUI_Rail.PageType.RecipeCfgPage;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.Rail.CompUI_Rail
{
    [CompUI(RailInteropConst.ModelName, "MM")]
    public class RailCompUI : CompUIBase
    {
        protected override string _GetCompName()
        {
            return RailInteropConst.DisplayName;
        }

        protected override IPageTypeDesignCompUI _GetPageTypeDesignCompUI(PageTypeID pageTypeID, Guid guid)
        {
            if (pageTypeID == ImeIOTConst.InitCfgPage)
                return new InitCfgPageTypeDesignCompUI();

            if (pageTypeID == ImeIOTConst.PPCfgPage)
                return new RecipeCfgPageTypeDesignCompUI();

            return null;
        }

        protected override IPageTypeRunTimeCompUI _GetPageTypeRunTimeCompUI(PageTypeID pageTypeID, Guid guid)
        {
            if (pageTypeID == ImeIOTConst.InitCfgPage)
                return new InitCfgPageTypeRunTimeCompUI();

            if (pageTypeID == ImeIOTConst.PPCfgPage)
                return new RecipeCfgPageTypeRunTimeCompUI();

            return null;
        }
    }
}
