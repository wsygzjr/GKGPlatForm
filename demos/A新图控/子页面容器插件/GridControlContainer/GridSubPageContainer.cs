using GF_Gereric;
using Griffins.Map.UI;

namespace GKG.Map.Page.UIContainer.GridContainer
{
    /// <summary>
    /// 网格子页面容器实现
    /// </summary>
    [SubPageContainerAttribute("{53714C1C-F46A-4708-B338-FE55BC92E320}")]
    public class GridSubPageContainer : GriffinsPluginMngClass, ISubPageContainer
    {
        string ISubPageContainer.Name => "网格子页面容器";

        ISubPageContainerDesignTime ISubPageContainer.CreateDesignTime()
        {
            return new GridContainerTemplateDesignTime();
        }

        ISubPageContainerRunTime ISubPageContainer.CreateRunTime()
        {
            return new GridContainerTemplateRunTime();

        }
    }

}
