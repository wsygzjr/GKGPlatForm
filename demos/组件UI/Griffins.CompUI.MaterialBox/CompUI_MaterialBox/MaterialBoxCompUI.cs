using Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.FactoryCfgPage;
using Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.InitCfgPage;
using Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.RecipeCfgPage;
using Griffins.CompUI.MaterialBox.CompUI_MaterialBox.SubMM;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.MaterialBox.CompUI_MaterialBox
{
    [CompUI(MaterialBoxSubMMInfo.ModuleModelId, ImeIOTConst.CompType_SubMMStr)]
    /// <summary>
    /// 料盒子机械模组的组件 UI 入口，负责把工厂、初始化、配方页面注册给宿主。
    /// </summary>
    public class MaterialBoxCompUI : CompUIBase
    {
        /// <summary>工厂配置页的固定页面类型 ID。</summary>
        private static readonly PageTypeID FactoryCfgPageTypeID = PageTypeID.Parse("FactoryCfgPage");

        /// <summary>返回组件在宿主中显示的名称。</summary>
        protected override string _GetCompName() { return MaterialBoxSubMMInfo.ModuleDisplayName; }

        /// <summary>根据页面类型创建设计态页面。</summary>
        protected override IPageTypeDesignCompUI _GetPageTypeDesignCompUI(PageTypeID pageTypeID, Guid guid)
        {
            if (pageTypeID == FactoryCfgPageTypeID)
            {
                return new FactoryCfgPageTypeDesignCompUI();
            }

            if (pageTypeID == ImeIOTConst.InitCfgPage)
            {
                return new InitCfgPageTypeDesignCompUI();
            }

            if (pageTypeID == ImeIOTConst.PPCfgPage)
            {
                return new RecipeCfgPageTypeDesignCompUI();
            }

            return null;
        }

        /// <summary>根据页面类型创建运行态页面。</summary>
        protected override IPageTypeRunTimeCompUI _GetPageTypeRunTimeCompUI(PageTypeID pageTypeID, Guid guid)
        {
            if (pageTypeID == FactoryCfgPageTypeID)
            {
                return new FactoryCfgPageTypeRunTimeCompUI();
            }

            if (pageTypeID == ImeIOTConst.InitCfgPage)
            {
                return new InitCfgPageTypeRunTimeCompUI();
            }

            if (pageTypeID == ImeIOTConst.PPCfgPage)
            {
                return new RecipeCfgPageTypeRunTimeCompUI();
            }

            return null;
        }
    }
}
