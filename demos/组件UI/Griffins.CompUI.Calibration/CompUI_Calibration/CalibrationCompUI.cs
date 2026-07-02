using Griffins.CompUI.Calibration.PageType.InitCfgPage;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.GlueDirectWork.CompUI_GlueDirectWork
{
    [CompUI("Calibration", ImeIOTConst.CompType_SubMMStr)]
    public class GlueDirectWorkCompUI : CompUIBase
    {
        protected override string _GetCompName() { return "标定"; }

        protected override IPageTypeDesignCompUI _GetPageTypeDesignCompUI(PageTypeID pageTypeID, Guid subMMObjID)
        {
            if (pageTypeID == PageTypeID.Parse("InitCfgPage"))
            {
                return new InitCfgPageTypeDesignCompUI();
            }
            else if (pageTypeID == PageTypeID.Parse("PPCfgPage"))
            {
                return null;
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
                return new InitCfgPageTypeRunTimeCompUI();
            }
            else if (pageTypeID == PageTypeID.Parse("PPCfgPage"))
            {
                return null;
            }
            else
            {
                return null;
            }
        }
    }
}
