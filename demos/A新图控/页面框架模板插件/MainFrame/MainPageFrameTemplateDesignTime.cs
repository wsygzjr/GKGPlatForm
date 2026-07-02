using Griffins.Map;
using Griffins.Map.UI;
using Griffins.Map.UI.CustomMenuCmdConfig;
using MainFrame.ViewModels;
using System;

namespace MainFrame
{
    /// <summary>
    /// 主页面框架模板设计时接口实现
    /// </summary>
    public class MainPageFrameTemplateDesignTime : IPageFrameTemplateDesignTime
    {
        private IPageFrameTemplateDesignTimeCallBack? _callBack;
        /// <summary>
        /// 初始化设计时环境
        /// </summary>
        /// <param name="callBack">回调接口</param>
        public void Init(IPageFrameTemplateDesignTimeCallBack callBack)
        {
            if (callBack == null)
                throw new Exception("设计时回调接口不能为null");
            GlobleCallBack.Init(callBack);
        }


        /// <summary>
        /// 创建页面框架模板设计时配置接口实例
        /// </summary>
        /// <returns>页面框架模板设计时配置接口实例</returns>
        public IPageFrameTemplateCfg CreateCfg()
        {
            return new MainPageFrameTemplateCfg();
        }

        /// <summary>
        /// 创建页面框架模板设计时预览接口实例
        /// </summary>
        /// <returns>页面框架模板设计时预览接口实例</returns>
        public IPageFrameTemplatePreview CreatePreview()
        {
            return new MainPageFrameTemplatePreview();
        }
    }
}
