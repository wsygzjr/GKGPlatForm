using GF_Gereric;
using Griffins.Map;
using Griffins.Map.UI;

namespace MainFrame
{
    /// <summary>
    /// 主页面框架模板实现
    /// </summary>
    [PageFrameTemplate("MainPageFrameTemplate")]
    public class MainPageFrameTemplate : GriffinsPluginMngClass,IPageFrameTemplate
	{
        /// <summary>
        /// 页面框架模板类型名称
        /// </summary>
        public string Name => "主页面框架";
        /// <summary>
        /// 创建页面框架模板设计时接口实例
        /// </summary>
        /// <returns>页面框架模板设计时接口实例</returns>
        public IPageFrameTemplateDesignTime CreateDesignTime()
        {
            return new MainPageFrameTemplateDesignTime();
        }
        /// <summary>
        /// 创建页面框架模板运行时接口实例
        /// </summary>
        /// <returns>页面框架模板运行时接口实例</returns>
        public IPageFrameTemplateRunTime CreateRunTime()
        {
            return new MainPageFrameTemplateRunTime();
        }
    }

}
