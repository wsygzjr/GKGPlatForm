using GF_Gereric;
using Griffins.CompUI.BasicRobot.CompUI_BasicRobot.PageType.DebugPage;
using Griffins.CompUI.BasicRobot.CompUI_BasicRobot.PageType.FactoryCfgPage;
using Griffins.CompUI.BasicRobot.CompUI_BasicRobot.PageType.InitCfgPage;
using Griffins.CompUI.BasicRobot.CompUI_BasicRobot.PageType.RecipeCfgPage;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.BasicRobot.CompUI_BasicRobot
{
    /// <summary>
    /// 基础运动控制机械手 CompUI 插件。
    /// 提供出厂配置、初始化配置、配方配置与调试页的设计态/运行态界面。
    /// </summary>
    [CompUI("BasicRobot", "SubMM")]
    public class BasicRobotCompUI : CompUIBase
    {
        public BasicRobotCompUI()
        {
        }

        #region ICompUI 成员

        protected override string _GetCompName() { return "基础运动控制机械手"; }

        /// <summary>subMMObjID 预留多对象扩展；当前基础机械手仅单一对象，未按 ID 分支。</summary>
        protected override IPageTypeDesignCompUI _GetPageTypeDesignCompUI(PageTypeID pageTypeID, Guid subMMObjID)
        {
            if (pageTypeID == PageTypeID.Parse("FactoryCfgPage"))
                return new FactoryCfgPageTypeDesignCompUI();
            else if (pageTypeID == PageTypeID.Parse("InitCfgPage"))
                return new InitCfgPageTypeDesignCompUI();
            else if (pageTypeID == PageTypeID.Parse("RecipeCfgPage"))
                return new RecipeCfgPageTypeDesignCompUI();
            else if (pageTypeID == PageTypeID.Parse("DebugPage"))
                return new DebugPageTypeDesignCompUI();
            else
                return null;
        }

        protected override IPageTypeRunTimeCompUI _GetPageTypeRunTimeCompUI(PageTypeID pageTypeID, Guid subMMObjID)
        {
            if (pageTypeID == PageTypeID.Parse("InitCfgPage"))
                return new InitCfgPageTypeRunTimeCompUI();
            else
                return null;
        }

        #endregion
    }
}
