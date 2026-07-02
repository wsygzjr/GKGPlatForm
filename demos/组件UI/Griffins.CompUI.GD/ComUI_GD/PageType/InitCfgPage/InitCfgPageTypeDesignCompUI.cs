using Griffins.CompUI.GD.InitCfgPage.Views;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.GD.InitCfgPage
{
    internal class InitCfgPageTypeDesignCompUI : IPageTypeDesignCompUI
    {
        private ICompUIRunTimeCallBack callBack;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="callBack">回调方法</param>
        public InitCfgPageTypeDesignCompUI(ICompUIRunTimeCallBack callBack)
        {
            this.callBack = callBack;
        }

        /// <summary>
        /// 页面类型ID
        /// </summary>
        PageTypeID IPageTypeDesignCompUI.PageTypeID => PageTypeID.Parse("InitCfgPage");

        /// <summary>
        /// 获取组件实例所有的页面类型组件界面视图信息，null或个数为0表示没有对应的界面
        /// </summary>
        /// <returns>该机械模组所有的出厂配置参数配置界面信息列表</returns>
        PageTypeCompUIViewInfoList IPageTypeDesignCompUI.GetPageTypeCompUIViewInfoes()
        {
            return new PageTypeCompUIViewInfoList
            {
                new PageTypeCompUIViewInfo(){ ViewID = InitCfgPageTypeConst.ViewID_SingleTrackStation, ViewName = ResourceA.ViewName_SingleTrackStation }
            };
        }

        /// <summary>
        /// 获取界面ID对应的页面类型组件界面视图接口实例,null表示不存在界面ID对应的页面类型组件界面
        /// </summary>
        ///  <param name="viewID">界面ID</param>
        /// <returns>界面ID对应的页面类型组件界面视图接口实例</returns>
        object? IPageTypeDesignCompUI.GetPageTypeCompUIView(string viewID)
        {
            return viewID switch
            {
                InitCfgPageTypeConst.ViewID_SingleTrackStation => new SingleTrackStationCompUIView(),
                _ => null,
            };
        }

        public InnerSubPageTypeInfoList GetInnerSubPageTypeInfoes()
        {
            throw new NotImplementedException();
        }

        public IInnerSubPageDesignTime CreateDesignTime(InnerSubPageTypeID innerSubPageTypeID)
        {
            throw new NotImplementedException();
        }

        public void Init(ICompUIDesignCallBack callBack)
        {
            throw new NotImplementedException();
        }
    }
}
