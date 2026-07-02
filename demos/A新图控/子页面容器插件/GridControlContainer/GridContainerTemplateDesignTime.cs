using GKG.Map.Page.UIContainer.GridContainer.ViewModels;
using Griffins.Map.UI;

namespace GKG.Map.Page.UIContainer.GridContainer
{
    /// <summary>
    /// 网格子页面容器设计时接口实现（负责设计时配置和预览功能）
    /// </summary>
    public class GridContainerTemplateDesignTime : ISubPageContainerDesignTime
    {
        void ISubPageContainerDesignTime.Init(ISubPageContainerDesignTimeCallBack callBack)
        {
            if (callBack == null)
                throw new Exception("设计时回调接口不能为null");
            GlobleCallBack.Init(callBack);
        }

        ISubPageContainerCfg ISubPageContainerDesignTime.CreateCfg()
        {
            return new GridContainerTemplateCfg();

        }

        ISubPageContainerPreview ISubPageContainerDesignTime.CreatePreview()
        {
            return new GridContainerTemplatePreview();

        }
    }
}
