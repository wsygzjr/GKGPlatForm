using GF_Gereric;
using GKG.SubMM;
using Griffins.CompUI.Vision.CompUI.PageType.FactoryCfgPage;
using Griffins.CompUI.Vision.CompUI.PageType.InitCfgPage;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.Vision
{
	[CompUI("Vision", "SubMM")]
	public class VisionCompUI : CompUIBase
	{
		public VisionCompUI()
		{
		}

		#region ICompUI 成员

		protected override string _GetCompName()
		{
			return "视觉分析UI组件";
		}

		protected override IControlPanel _CreateControlPanel(Guid subMMObjID)
		{
            if (subMMObjID == VisionSubMachineModulesConst.SubMMObjInfos[0].SubMMObjID)
            {
                return new MainControlPanel();
            }
            return new MainControlPanel();
		}

        protected override IPageTypeDesignCompUI _GetPageTypeDesignCompUI(PageTypeID pageTypeID, Guid subMMObjID)
        {
            if (subMMObjID != VisionSubMachineModulesConst.SubMMObjInfos[0].SubMMObjID)
            {
                return null;
            }

            if (pageTypeID == PageTypeID.Parse("InitCfgPage"))
            {
                return new InitCfgPageTypeDesignCompUI();
            }
            if (pageTypeID == PageTypeID.Parse("FactoryCfgPage"))
            {
                return new FactoryCfgPageTypeDesignCompUI();
            }
            return null;
        }

        protected override IPageTypeRunTimeCompUI _GetPageTypeRunTimeCompUI(PageTypeID pageTypeID, Guid subMMObjID)
        {
            if (subMMObjID != VisionSubMachineModulesConst.SubMMObjInfos[0].SubMMObjID)
            {
                return null;
            }

            if (pageTypeID == PageTypeID.Parse("InitCfgPage"))
            {
                return new InitCfgPageTypeRunTimeCompUI();
            }
            if (pageTypeID == PageTypeID.Parse("FactoryCfgPage"))
            {
                return new FactoryCfgPageTypeRunTimeCompUI();
            }
            return null;
        }

        #endregion
    }
}
