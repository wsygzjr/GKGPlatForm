using GF_Gereric;
using Griffins.CompUI.AxisDebug.CompUI_AxisDebug.PageType.InitCfgPage;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System.Threading.Tasks;

namespace Griffins.CompUI.AxisDebug.CompUI_AxisDebug
{
    [CompUI("AxisDebug", "SubMM")]
    public class AxisDebugCompUI : CompUIBase
    {
        protected override string _GetCompName() { return "轴调试"; }

        protected override IPageTypeDesignCompUI _GetPageTypeDesignCompUI(PageTypeID pageTypeID)
        {
            if (pageTypeID == PageTypeID.Parse("InitCfgPage"))
            {
                return new InitCfgPageTypeDesignCompUI();
            }
            else
            {
                return null;
            }
        }

        protected override IPageTypeRunTimeCompUI _GetPageTypeRunTimeCompUI(PageTypeID pageTypeID)
        {
            if (pageTypeID == PageTypeID.Parse("InitCfgPage"))
            {
                return new InitCfgPageTypeRunTimeCompUI();
            }
            else
            {
                return null;
            }
        }

        protected override IControlPanel _CreateControlPanel()
        {
            return new EmptyControlPanel();
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
                return null;
            }
        }
    }
}
