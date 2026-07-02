using GKG.SubMM;
using Griffins.CompUI.PushRod.CompUI_PushRod.PageType.FactoryCfgPage;
using Griffins.CompUI.PushRod.CompUI_PushRod.PageType.InitCfgPage;
using Griffins.CompUI.PushRod.CompUI_PushRod.PageType.RecipeCfgPage;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.PushRod.CompUI_PushRod
{
    [CompUI(PushRodSubMachineModulesConst.SubMMModelStr, ImeIOTConst.CompType_SubMMStr)]
    public class CylinderPushRodCompUI : CompUIBase
    {
        private static readonly PageTypeID FactoryCfgPageTypeID = PageTypeID.Parse("FactoryCfgPage");

        protected override string _GetCompName()
        {
            return PushRodSubMachineModulesConst.SubMMName;
        }

        protected override IPageTypeDesignCompUI _GetPageTypeDesignCompUI(PageTypeID pageTypeID, Guid guid)
        {
            if (!IsCylinderSubMM(guid))
            {
                return null;
            }

            if (pageTypeID == FactoryCfgPageTypeID)
            {
                return new FactoryCfgPageTypeDesignCompUI(guid);
            }

            if (pageTypeID == ImeIOTConst.InitCfgPage)
            {
                return new InitCfgPageTypeDesignCompUI(guid);
            }

            if (pageTypeID == ImeIOTConst.PPCfgPage)
            {
                return null;
            }

            return null;
        }

        protected override IPageTypeRunTimeCompUI _GetPageTypeRunTimeCompUI(PageTypeID pageTypeID, Guid guid)
        {
            if (!IsCylinderSubMM(guid))
            {
                return null;
            }

            if (pageTypeID == FactoryCfgPageTypeID)
            {
                return new FactoryCfgPageTypeRunTimeCompUI(guid);
            }

            if (pageTypeID == ImeIOTConst.InitCfgPage)
            {
                return new InitCfgPageTypeRunTimeCompUI(guid);
            }

            if (pageTypeID == ImeIOTConst.PPCfgPage)
            {
                return null;
            }

            return null;
        }

        private static bool IsCylinderSubMM(Guid guid) => guid == PushRodSubMachineModulesConst.CylinderSubMMObjID;
    }
}
