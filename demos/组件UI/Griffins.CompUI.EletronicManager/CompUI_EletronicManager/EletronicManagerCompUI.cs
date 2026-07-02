using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.InitCfgPage;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.DebugPage;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.SubMM;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System.Threading.Tasks;
using System;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager
{
    [CompUI(EletronicManagerSubMMInfo.ModuleModelId, ImeIOTConst.CompType_SubMMStr)]
    internal class EletronicManagerCompUI : CompUIBase
    {
        protected override string _GetCompName() { return EletronicManagerSubMMInfo.ModuleDisplayName; }

        protected override IPageTypeDesignCompUI _GetPageTypeDesignCompUI(PageTypeID pageTypeID,Guid guid)
        {
            if (pageTypeID == ImeIOTConst.InitCfgPage)
            {
                return new InitCfgPageTypeDesignCompUI();
            }
            else if (pageTypeID == ImeIOTConst.ElectricalMngCfgPage)
            {
                return new ElectricalMngPageTypeDesignCompUI();
            }
            else if (pageTypeID == ImeIOTConst.DebugPage)
            {
                return new DebugPageTypeDesignCompUI();
            }
            else
            {
                return null;
            }
        }

        protected override IPageTypeRunTimeCompUI _GetPageTypeRunTimeCompUI(PageTypeID pageTypeID,Guid guid)
        {
            if (pageTypeID == ImeIOTConst.InitCfgPage)
            {
                return new InitCfgPageTypeRunTimeCompUI();
            }
            else if (pageTypeID == ImeIOTConst.ElectricalMngCfgPage)
            {
                return new ElectricalMngPageTypeRunTimeCompUI();
            }
            else if (pageTypeID == ImeIOTConst.DebugPage)
            {
                return new DebugPageTypeRunTimeCompUI();
            }
            else
            {
                return null;
            }
        }



        private sealed class EmptyControlPanel : IControlPanel
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
