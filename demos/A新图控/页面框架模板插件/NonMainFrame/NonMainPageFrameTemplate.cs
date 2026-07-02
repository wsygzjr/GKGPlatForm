using GF_Gereric;
using Griffins.Map;
using Griffins.Map.UI;

namespace NonMainFrame
{
    /// <summary>
    /// 非主页面框架模板实现
    /// </summary>
    [PageFrameTemplate("NonMainPageFrameTemplate")]
    public class NonMainPageFrameTemplate : GriffinsPluginMngClass, IPageFrameTemplate
    {
        /// <summary>
        /// 页面框架模板类型名称
        /// </summary>
        public string Name => "非主页面框架";
        /// <summary>
        /// 创建页面框架模板设计时接口实例
        /// </summary>
        /// <returns>页面框架模板设计时接口实例</returns>
        public IPageFrameTemplateDesignTime CreateDesignTime()
        {
            return new NonMainPageFrameTemplateDesignTime();
        }
        /// <summary>
        /// 创建页面框架模板运行时接口实例
        /// </summary>
        /// <returns>页面框架模板运行时接口实例</returns>
        public IPageFrameTemplateRunTime CreateRunTime()
        {
            return new NonMainPageFrameTemplateRunTime();
        }
    }

}
