using GKG.CompUI.LoadUnload.ControlPanel;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace GKG.CompUI.LoadUnload
{
    [CompUI("LoadUnload", "MM")]
    public class LoadUnloadCompUI : CompUIBase
    {
        protected override string _GetCompName() => "LoadUnload";

        protected override IPageTypeDesignCompUI _GetPageTypeDesignCompUI(PageTypeID pageTypeID, Guid subMMObjID)
        {
            return null!;
        }

        protected override IPageTypeRunTimeCompUI _GetPageTypeRunTimeCompUI(PageTypeID pageTypeID, Guid subMMObjID)
        {
            return null!;
        }

        protected override IControlPanel? _CreateControlPanel(Guid subMMObjID)
        {
            return new LoadUnloadControlPanelCompUI();
        }
    }
}
