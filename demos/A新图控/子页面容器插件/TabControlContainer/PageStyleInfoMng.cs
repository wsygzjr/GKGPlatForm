using GKG.Map.Page.UIContainer.TabControlContainer.Views;
using Griffins.Map.UI.CustomMenuCmdConfig;

namespace GKG.Map.Page.UIContainer.TabControlContainer
{
    /// <summary>
    /// view插件管理
    /// </summary>
    public class PageStyleInfoMng
    {

        /// <summary>
        /// 页面样式信息列表
        /// </summary>
        private static List<PageStyleInfo> _pageStyleInfoSource=new List<PageStyleInfo>();
        public static List<PageStyleInfo> PageStyleInfoSource
        {
            get
            {
                if (_pageStyleInfoSource.Count == 0)
                    loadPageStylePlugins();
                return _pageStyleInfoSource;

            }
        }

        /// <summary>
        /// 加载所有可用样式
        /// </summary>
        private static void loadPageStylePlugins()
        {
            _pageStyleInfoSource.Add(new PageStyleInfo(PageStyleInfo.TabControlContainerPageStyleID, PageStyleInfo.TabControlContainerPageStyleName,new PreviewView()));
        }
    }
}
