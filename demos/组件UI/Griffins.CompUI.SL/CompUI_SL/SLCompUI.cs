using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins;
using Griffins.CompUI.SL.DebugPage;
using Griffins.CompUI.SL.FactoryCfgPage;
using Griffins.CompUI.SL.InitCfgPage;
using Griffins.CompUI.SL.RecipeCfgPage;
using Griffins.Map;
using Griffins.Map.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Griffins.CompUI.SL
{
    [CompUI("SL", "MMModel")]
    class SLCompUI : GriffinsPluginMngClass, ICompUI
    {
        private ICompUIRunTimeCallBack callBack;

        public SLCompUI()
        {

        }
        #region ICompUI 成员

        string ICompUI.CompName { get { return "上料"; } }

        public void Init(ICompUIRunTimeCallBack callBack)
        {
            this.callBack = callBack;
        }

        IPageTypeDesignCompUI ICompUI.GetPageTypeDesignCompUI(PageTypeID pageTypeID)
        {
            if (pageTypeID == PageTypeID.Parse("FactoryCfgPage"))
            {
                return new FactoryCfgPageTypeDesignCompUI(this.callBack);
            }
            else if (pageTypeID == PageTypeID.Parse("InitCfgPage"))
            {
                return new InitCfgPageTypeDesignCompUI(this.callBack);
            }
            else if (pageTypeID == PageTypeID.Parse("RecipeCfgPage"))
            {
                return new RecipeCfgPageTypeDesignCompUI(this.callBack);
            }
            else if (pageTypeID == PageTypeID.Parse("DebugPage"))
            {
                return new DebugPageTypeDesignCompUI(this.callBack);
            }
            else
            {
                return null;
            }
        }

        IPageTypeRunTimeCompUI ICompUI.GetPageTypeRunTimeCompUI(PageTypeID pageTypeID)
        {
            if (pageTypeID == PageTypeID.Parse("FactoryCfgPage"))
            {
                return new FactoryCfgPageTypeRunTimeCompUI(this.callBack);
            }
            else if (pageTypeID == PageTypeID.Parse("InitCfgPage"))
            {
                return new InitCfgPageTypeRunTimeCompUI(this.callBack);
            }
            else if (pageTypeID == PageTypeID.Parse("RecipeCfgPage"))
            {
                return new RecipeCfgPageTypeRunTiemCompUI(this.callBack);
            }
            else if (pageTypeID == PageTypeID.Parse("DebugPage"))
            {
                return new DebugPageTypeRunTimeCompUI(this.callBack);
            }
            else
            {
                return null;
            }
        }

        public IControlPanel CreateControlPanel()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}