using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins;
using Griffins.CompUI.TestGlueDirectWorkMM.DebugPage;
using Griffins.CompUI.TestGlueDirectWorkMM.FactoryCfgPage;
using Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using GriffinsGeneralTestMM;
using System;
using System.Collections.Generic;
using System.Text;
using TestGlueDirectWorkMM;

namespace Griffins.CompUI.TestGlueDirectWorkMM
{
    [CompUI(GenMMInfoDefine.MMNumberStr, ImeIOTConst.CompType_MMStr)]
    class TestGlueDirectWorkCompUI : CompUIBase
    {

		public TestGlueDirectWorkCompUI()
        {
		
		}
        #region ICompUI 成员
        protected override string _GetCompName() { return GenMMInfoDefine.MMName; }

        protected override IPageTypeDesignCompUI _GetPageTypeDesignCompUI(PageTypeID pageTypeID)
        {
            if (pageTypeID == ImeIOTConst.FactoryCfgPage)
                return new FactoryCfgPageTypeDesignCompUI();
            else if (pageTypeID == ImeIOTConst.InitCfgPage)
                return new InitCfgPageTypeDesignCompUI();
            else if (pageTypeID == ImeIOTConst.PPCfgPage)
                return new RecipeCfgPageTypeDesignCompUI();
            else if (pageTypeID == ImeIOTConst.DebugPage)
                return new DebugPageTypeDesignCompUI();
            else
			{
				return null;
			}
		}

        protected override IPageTypeRunTimeCompUI _GetPageTypeRunTimeCompUI(PageTypeID pageTypeID)
        {
            if (pageTypeID == ImeIOTConst.FactoryCfgPage)
                return new FactoryCfgPageTypeRunTimeCompUI();
            else if (pageTypeID == ImeIOTConst.InitCfgPage)
                return new InitCfgPageTypeRunTimeCompUI();
            else if (pageTypeID == ImeIOTConst.PPCfgPage)
                return new RecipeCfgPageTypeRunTimeCompUI();
            else if (pageTypeID == ImeIOTConst.DebugPage)
                return new DebugPageTypeRunTimeCompUI();
            else
			{
				return null;
			}
		}
        /// <summary>
        /// 创建控制面板实例
        /// </summary>
        /// <returns></returns>
        protected override IControlPanel _CreateControlPanel()
        {
            return new MainControlPanel(GenMMInfoDefine.GenMMInfo.ControlPanels);
        }
        #endregion
    }
}