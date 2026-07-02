using Griffins.Map.UI;
using Griffins.Map.UI.CustomMenuCmdConfig;

namespace GKG.Map.Page.UIContainer.GridContainer.ViewModels
{
    /// <summary>
    /// 全局回调
    /// </summary>
    public class GlobleCallBack
    {
        /// <summary>
        /// 网格子页面容器模板设计时回调接口
        /// </summary>
        public static ISubPageContainerDesignTimeCallBack designTimeCallBack = null!;

        public static void Init(ISubPageContainerDesignTimeCallBack callBack)
        {
            designTimeCallBack = callBack;
            StaticDataMng.SetGetSubPageInfoesDelegate(GetSubPageInfoes, GetSubPageContainerInstInfoes, null, null);
        }

        /// <summary>
        /// 获取子页面容器实例信息列表
        /// </summary>
        /// <returns></returns>
        public static SubPageContainerInstInfoList GetSubPageContainerInstInfoes()
        {
            return designTimeCallBack.GetSubPageContainerInstInfoes();
        }

        /// <summary>
        /// 获取子页面信息列表
        /// </summary>
        /// <returns></returns>
        public static SubPageInfoList GetSubPageInfoes()
        {
            return designTimeCallBack.GetSubPageInfoes();
        }

    }
}
