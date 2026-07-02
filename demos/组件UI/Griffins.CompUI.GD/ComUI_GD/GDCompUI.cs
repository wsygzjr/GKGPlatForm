using GF_Gereric;
using Griffins.CompUI.GD.InitCfgPage;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.GD
{
    [CompUI("GD", "MMModel")]
    class GDCompUI : GriffinsPluginMngClass, ICompUI
    {
        private ICompUIRunTimeCallBack? callBack;

        public GDCompUI()
        {

        }

        #region ICompUI 成员

        string ICompUI.CompName { get { return "单层轨道"; } }

        public void Init(ICompUIRunTimeCallBack callBack)
        {
            this.callBack = callBack;
        }

        IPageTypeDesignCompUI ICompUI.GetPageTypeDesignCompUI(PageTypeID pageTypeID)
        {
            if (this.callBack is null)
            {
                return null;
            }

            if (pageTypeID == PageTypeID.Parse("FactoryCfgPage"))
            {
                return null;
            }
            else if (pageTypeID == PageTypeID.Parse("InitCfgPage"))
            {
                return new InitCfgPageTypeDesignCompUI(this.callBack);
            }
            else if (pageTypeID == PageTypeID.Parse("RecipeCfgPage"))
            {
                return null;
            }
            else if (pageTypeID == PageTypeID.Parse("DebugPage"))
            {
                return null;
            }
            else
            {
                return null;
            }
        }

        IPageTypeRunTimeCompUI ICompUI.GetPageTypeRunTimeCompUI(PageTypeID pageTypeID)
        {
            if (this.callBack is null)
            {
                return null;
            }

            if (pageTypeID == PageTypeID.Parse("FactoryCfgPage"))
            {
                return null;
            }
            else if (pageTypeID == PageTypeID.Parse("InitCfgPage"))
            {
                return new InitCfgPageTypeRunTimeCompUI(this.callBack);
            }
            else if (pageTypeID == PageTypeID.Parse("RecipeCfgPage"))
            {
                return null;
            }
            else if (pageTypeID == PageTypeID.Parse("DebugPage"))
            {
                return null;
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
