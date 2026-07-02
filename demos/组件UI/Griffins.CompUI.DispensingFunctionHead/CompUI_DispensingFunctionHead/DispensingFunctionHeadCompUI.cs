using GKG.SubMM.Dispenser;
using Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.FactoryCfgPage;
using Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.InitCfgPage;
using Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.PPPage;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead
{
    /// <summary>
    /// 点胶机功能头组件UI主类
    /// 负责管理和路由出厂配置页面、初始化配置页面和配方页面
    /// </summary>
    [CompUI(DispensingFunctionHeadSubMachineModulesConst.SubMMModelStr, ImeIOTConst.CompType_SubMMStr)]
    public class DispensingFunctionHeadCompUI : CompUIBase
    {
        /// <summary>
        /// 出厂配置页面类型ID
        /// </summary>
        private static readonly PageTypeID FactoryCfgPageTypeID = PageTypeID.Parse("FactoryCfgPage");

        /// <summary>
        /// 配方页面类型ID
        /// </summary>
        private static readonly PageTypeID PPPageTypeID = ImeIOTConst.PPCfgPage;

        /// <summary>
        /// 获取组件名称
        /// </summary>
        /// <returns>点胶机功能头名称</returns>
        protected override string _GetCompName()
        {
            return DispensingFunctionHeadSubMachineModulesConst.SubMMName;
        }

        /// <summary>
        /// 获取设计时页面类型UI
        /// 根据页面类型ID返回相应的设计时CompUI实例
        /// </summary>
        /// <param name="pageTypeID">页面类型ID</param>
        /// <param name="guid">子机械模组对象ID</param>
        /// <returns>设计时CompUI实例，如果不支持则返回null</returns>
        protected override IPageTypeDesignCompUI _GetPageTypeDesignCompUI(PageTypeID pageTypeID, Guid guid)
        {
            // 检查是否为支持的子机械模组
            if (!IsSupportedSubMM(guid))
            {
                return null;
            }

            // 出厂配置页面
            if (pageTypeID == FactoryCfgPageTypeID)
            {
                return new FactoryCfgPageTypeDesignCompUI(guid);
            }

            // 初始化配置页面
            if (pageTypeID == ImeIOTConst.InitCfgPage)
            {
                return new InitCfgPageTypeDesignCompUI(guid);
            }

            // 配方页面
            if (pageTypeID == PPPageTypeID)
            {
                return new PPPageTypeDesignCompUI(guid);
            }

            return null;
        }

        /// <summary>
        /// 获取运行时页面类型UI
        /// 根据页面类型ID返回相应的运行时CompUI实例
        /// </summary>
        /// <param name="pageTypeID">页面类型ID</param>
        /// <param name="guid">子机械模组对象ID</param>
        /// <returns>运行时CompUI实例，如果不支持则返回null</returns>
        protected override IPageTypeRunTimeCompUI _GetPageTypeRunTimeCompUI(PageTypeID pageTypeID, Guid guid)
        {
            // 检查是否为支持的子机械模组
            if (!IsSupportedSubMM(guid))
            {
                return null;
            }

            // 出厂配置页面
            if (pageTypeID == FactoryCfgPageTypeID)
            {
                return new FactoryCfgPageTypeRunTimeCompUI(guid);
            }

            // 初始化配置页面
            if (pageTypeID == ImeIOTConst.InitCfgPage)
            {
                return new InitCfgPageTypeRunTimeCompUI(guid);
            }

            // 配方页面
            if (pageTypeID == PPPageTypeID)
            {
                return new PPPageTypeRunTimeCompUI(guid);
            }

            return null;
        }

        /// <summary>
        /// 检查是否为支持的子机械模组
        /// 遍历常量定义中的子机械模组列表进行匹配
        /// </summary>
        /// <param name="guid">要检查的子机械模组ID</param>
        /// <returns>如果支持返回true，否则返回false</returns>
        private static bool IsSupportedSubMM(Guid guid)
        {
            foreach (var info in DispensingFunctionHeadSubMachineModulesConst.SubMMObjInfos)
            {
                if (info.SubMMObjID == guid)
                    return true;
            }
            return false;
        }
    }
}
