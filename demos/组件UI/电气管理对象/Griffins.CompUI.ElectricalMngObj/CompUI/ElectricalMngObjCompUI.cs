using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins;
using Griffins.CompUI.ElectricalMngObj;
using Griffins.CompUI.ElectricalMngObj.DebugPage;
using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using GriffinsGeneralTestMM;
using System;
using System.Collections.Generic;
using System.Text;
using TestSubMM_ElectricalMngObj;

namespace Griffins.CompUI.ElectricalMngObj
{
    [CompUI(GenSubMMInfoDefine.SubMMModelStr, ImeIOTConst.CompType_SubMMStr)]
    class ElectricalMngObjCompUI :CompUIBase
    {

		public ElectricalMngObjCompUI()
        {
		
		}
        #region ICompUI 成员

        protected override string _GetCompName() { return GenSubMMInfoDefine.SubMMName; }

        protected override IPageTypeDesignCompUI _GetPageTypeDesignCompUI(PageTypeID pageTypeID) 
        {
			if (pageTypeID == ImeIOTConst.FactoryCfgPage)
                return new FactoryCfgPageTypeDesignCompUI();
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
            else if (pageTypeID == ImeIOTConst.DebugPage)
                return new DebugPageTypeRunTimeCompUI();
            else
			{
				return null;
			}
		}
        protected override IControlPanel _CreateControlPanel()
        {
            return new MainControlPanel(GenSubMMInfoDefine.GenSubMMInfo.ControlPanels);
        }
        #endregion
    }
}