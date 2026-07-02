using Griffins.CompUI.GlueDirectWork.PageType.InitCfgPage;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.GlueDirectWork.CompUI_GlueDirectWork
{
    [CompUI("test_GlueDirectWork", "MM")]
    public class GlueDirectWorkCompUI : CompUIBase
    {
        protected override string _GetCompName() { return "GlueDirectWork"; }

        protected override IPageTypeDesignCompUI _GetPageTypeDesignCompUI(PageTypeID pageTypeID, Guid subMMObjID)
        {
            if (pageTypeID == PageTypeID.Parse("InitCfgPage"))
            {
                return null;
            }
            else if (pageTypeID == PageTypeID.Parse("PPCfgPage"))
            {
                return new RecipeCfgPageTypeDesignCompUI();
            }
            else
            {
                return null;
            }
        }

        protected override IPageTypeRunTimeCompUI _GetPageTypeRunTimeCompUI(PageTypeID pageTypeID, Guid subMMObjID)
        {
            if (pageTypeID == PageTypeID.Parse("InitCfgPage"))
            {
                return null;
            }
            else if (pageTypeID == PageTypeID.Parse("PPCfgPage"))
            {
                return new RecipeCfgPageTypeRunTimeCompUI();
            }
            else
            {
                return null;
            }
        }
    }
}
