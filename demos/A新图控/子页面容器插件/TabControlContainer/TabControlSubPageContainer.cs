using GF_Gereric;
using Griffins.Map.UI;

namespace GKG.Map.Page.UIContainer.TabControlContainer
{
    /// <summary>
    /// 选项卡控件子页面容器实现
    /// </summary>
    [SubPageContainerAttribute("{D73ACE7F-FB62-470D-8281-E2BC57C33770}")]
    public class TabControlSubPageContainer : GriffinsPluginMngClass, ISubPageContainer
    {
        /// <summary>
        /// 子页面容器类型名称
        /// </summary>
        string ISubPageContainer.Name => "选项卡控件子页面容器";

        /// <summary>
        /// 创建子页面容器设计时接口实例
        /// </summary>
        /// <returns>子页面容器设计时接口实例</returns>
        ISubPageContainerDesignTime ISubPageContainer.CreateDesignTime()
        {
            return new TabControlContainerTemplateDesignTime();
        }

        /// <summary>
        /// 创建子页面容器运行时接口实例
        /// </summary>
        /// <returns>子页面容器运行时接口实例</returns>
        ISubPageContainerRunTime ISubPageContainer.CreateRunTime()
        {
            return new TabControlContainerTemplateRunTime();

        }
    }

}
