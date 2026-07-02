using GKG.Map.Page.UIContainer.GridContainer.Views;
using Griffins.Map.UI.CustomMenuCmdConfig;

namespace GKG.Map.Page.UIContainer.GridContainer
{
    /// <summary>
    /// view样式插件管理
    /// </summary>
    public class PageStyleInfoMng
    {
        private static List<PageStyleInfo> _pageStyleInfoSource = new List<PageStyleInfo>();
        /// <summary>
        /// 页面样式信息列表
        /// </summary>
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
            _pageStyleInfoSource.Add(new PageStyleInfo("{CFE3DBCB-3258-4F3C-9CCC-CB37D8500D3F}", "网格子页面容器样式", new PreviewView()));
        }
    }
}
