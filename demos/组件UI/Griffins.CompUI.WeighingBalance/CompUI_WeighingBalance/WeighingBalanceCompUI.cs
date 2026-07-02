using GF_Gereric;
using Griffins.CompUI.WeighingBalance.CompUI_WeighingBalance.PageType.FactoryCfgPage;
using Griffins.CompUI.WeighingBalance.CompUI_WeighingBalance.PageType.InitCfgPage;
using Griffins.CompUI.WeighingBalance.CompUI_WeighingBalance.PageType.PPCfgPage;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using GKG.SubMM.Dispenser;
using System;

namespace Griffins.CompUI.WeighingBalance.CompUI_WeighingBalance
{
    /// <summary>
    /// 称重模组 CompUI 插件。
    /// 提供初始化配置页的设计态/运行态界面。
    /// </summary>
    [CompUI(WeighingBalanceSubMachineModulesConst.SubMMModelStr, ImeIOTConst.CompType_SubMMStr)]
    public class WeighingBalanceCompUI : CompUIBase
    {
        public WeighingBalanceCompUI()
        {
        }

        #region ICompUI 成员

        protected override string _GetCompName() { return WeighingBalanceSubMachineModulesConst.SubMMName; }

        /// <summary>subMMObjID 预留多对象扩展；当前称重模组仅单一对象，未按 ID 分支。</summary>
        protected override IPageTypeDesignCompUI _GetPageTypeDesignCompUI(PageTypeID pageTypeID, Guid subMMObjID)
        {
            if (pageTypeID == ImeIOTConst.FactoryCfgPage)
                return new FactoryCfgPageTypeDesignCompUI();
            else if (pageTypeID == ImeIOTConst.InitCfgPage)
                return new InitCfgPageTypeDesignCompUI();
            else if (pageTypeID == ImeIOTConst.PPCfgPage)
                return new PPCfgPageTypeDesignCompUI();
            else
                return null;
        }

        protected override IPageTypeRunTimeCompUI _GetPageTypeRunTimeCompUI(PageTypeID pageTypeID, Guid subMMObjID)
        {
            if (pageTypeID == ImeIOTConst.FactoryCfgPage)
                return new FactoryCfgPageTypeRunTimeCompUI();
            else if (pageTypeID == ImeIOTConst.InitCfgPage)
                return new InitCfgPageTypeRunTimeCompUI();
            else if (pageTypeID == ImeIOTConst.PPCfgPage)
                return new PPCfgPageTypeRunTimeCompUI();
            else
                return null;
        }

        #endregion
    }
}
