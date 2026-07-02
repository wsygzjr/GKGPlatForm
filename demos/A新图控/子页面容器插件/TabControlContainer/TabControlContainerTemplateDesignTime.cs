using Griffins.Map.UI;
using GKG.Map.Page.UIContainer.TabControlContainer.ViewModels;

namespace GKG.Map.Page.UIContainer.TabControlContainer
{
    /// <summary>
    /// 选项卡控件子页面容器设计时接口实现（负责设计时配置和预览功能）
    /// </summary>
    public class TabControlContainerTemplateDesignTime : ISubPageContainerDesignTime
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="callBack">回调接口</param>
        void ISubPageContainerDesignTime.Init(ISubPageContainerDesignTimeCallBack callBack)
        {
            if (callBack == null)
                throw new Exception("设计时回调接口不能为null");
            GlobleCallBack.Init(callBack);
        }
        /// <summary>
        /// 创建子页面容器设计时配置接口实例
        /// </summary>
        /// <returns>子页面容器设计时配置接口实例</returns>
        ISubPageContainerCfg ISubPageContainerDesignTime.CreateCfg()
        {
            return new TabControlContainerTemplateCfg();

        }
        /// <summary>
        /// 创建子页面容器设计时预览接口实例
        /// </summary>
        /// <returns>子页面容器设计时预览接口实例</returns>
        ISubPageContainerPreview ISubPageContainerDesignTime.CreatePreview()
        {
            return new TabControlContainerTemplatePreview();

        }
    }
}
