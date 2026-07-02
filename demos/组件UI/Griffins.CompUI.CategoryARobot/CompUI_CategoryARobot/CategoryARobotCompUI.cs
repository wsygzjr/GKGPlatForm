using GF_Gereric;
using Griffins.CompUI.CategoryARobot.CompUI_CategoryARobot.PageType.InitCfgPage;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.CategoryARobot.CompUI_CategoryARobot
{
    /// <summary>
    /// A 类扩展运动控制机械手 CompUI 插件。
    /// 初始化页包含分段速度、2D 位置比较与运控前瞻参数的结构化编辑。
    /// </summary>
    [CompUI("CategoryARobot", "SubMM")]
    public class CategoryARobotCompUI : CompUIBase
    {
        public CategoryARobotCompUI()
        {
        }

        #region ICompUI 成员

        protected override string _GetCompName() { return "A类扩展运动控制机械手"; }

        /// <summary>subMMObjID 预留多对象扩展；当前 A 类机械手仅单一对象，未按 ID 分支。</summary>
        protected override IPageTypeDesignCompUI _GetPageTypeDesignCompUI(PageTypeID pageTypeID, Guid subMMObjID)
        {
            if (pageTypeID == PageTypeID.Parse("InitCfgPage"))
                return new InitCfgPageTypeDesignCompUI();
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
