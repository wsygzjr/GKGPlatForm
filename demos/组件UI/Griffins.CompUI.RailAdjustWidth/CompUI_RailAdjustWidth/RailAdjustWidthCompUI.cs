using GF_Gereric;
using Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.FactoryCfgPage;
using Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.InitCfgPage;
using Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.RecipeCfgPage;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;
using System.Threading.Tasks;

namespace Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth
{
    [CompUI("RailAdjustWidth", ImeIOTConst.CompType_SubMMStr)]
    public class RailAdjustWidthCompUI : CompUIBase
    {
        protected override string _GetCompName() { return "轨道调宽子机械模块"; }

        protected override IPageTypeDesignCompUI _GetPageTypeDesignCompUI(PageTypeID pageTypeID, Guid guid)
        {
            if (pageTypeID == ImeIOTConst.FactoryCfgPage)
            {
                return new FactoryCfgPageTypeDesignCompUI();
            }
            else if (pageTypeID == ImeIOTConst.InitCfgPage)
            {
                return new InitCfgPageTypeDesignCompUI();
            }
            else if (pageTypeID == ImeIOTConst.PPCfgPage)
            {
                return new RecipeCfgPageTypeDesignCompUI();
            }
            else
            {
                return null;
            }
        }

        protected override IPageTypeRunTimeCompUI _GetPageTypeRunTimeCompUI(PageTypeID pageTypeID, Guid guid)
        {
            if (pageTypeID == ImeIOTConst.FactoryCfgPage)
            {
                return new FactoryCfgPageTypeRunTimeCompUI();
            }
            else if (pageTypeID == ImeIOTConst.InitCfgPage)
            {
                return new InitCfgPageTypeRunTimeCompUI();
            }
            else if (pageTypeID == ImeIOTConst.PPCfgPage)
            {
                return new RecipeCfgPageTypeRunTimeCompUI();
            }
            else
            {
                return null;
            }
        }

        private class EmptyControlPanel : IControlPanel
        {
            void IControlPanel.Init(IControlPanelCallBack iControlPanelCallBack)
            {
            }

            ControlPanelViewInfoList IControlPanel.GetControlPanelViewInfos()
            {
                return new ControlPanelViewInfoList();
            }

            public Task ShowControlPanelAsync(string controlPanelID, object owner)
            {
                return Task.CompletedTask;
            }
        }
    }
}
