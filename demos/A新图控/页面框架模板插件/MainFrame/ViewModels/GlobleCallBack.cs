using Griffins.Map;
using Griffins.Map.UI;
using Griffins.Map.UI.CustomMenuCmdConfig;

namespace MainFrame.ViewModels
{
    /// <summary>
    /// 全局回调
    /// </summary>
    public class GlobleCallBack
    {
        /// <summary>
        /// 选项卡控件子页面容器模板设计时回调接口
        /// </summary>
        private static IPageFrameTemplateDesignTimeCallBack designTimeCallBack;
        public static void Init(IPageFrameTemplateDesignTimeCallBack callBack)
        {
            designTimeCallBack = callBack;
            StaticDataMng.SetGetSubPageInfoesDelegate(GetSubPageInfoes, GetSubPageContainerInstInfoes, GetSubPageInfoesByPageID, GetPageInfoes );
        }
        /// <summary>
        /// 获取页面信息
        /// </summary>
        /// <returns></returns>
        public static PageInfoList GetPageInfoes()
        {
            return designTimeCallBack.GetPageInfoes();
        }
        /// <summary>
        /// 获取容器实例
        /// </summary>
        /// <returns></returns>
        public static SubPageContainerInstInfoList GetSubPageContainerInstInfoes()
        {
            return designTimeCallBack.GetSubPageContainerInstInfoes();
        }
        /// <summary>
        /// 获取子页面信息
        /// </summary>
        /// <returns></returns>
        public static SubPageInfoList GetSubPageInfoes()
        {
            return designTimeCallBack.GetSubPageInfoes();
        }
      
        /// <summary>
        /// 获取指定页面的子页面信息列表
        /// </summary>
        /// <param name="pageID">页面实例ID</param>
        /// <returns>指定页面的子页面信息列表</returns>
        public static  SubPageInfoList GetSubPageInfoesByPageID(PageID pageID)
        {
            return designTimeCallBack.GetSubPageInfoes(pageID);

        }
    }
}
