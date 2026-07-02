using GKG.SubMM;
using Griffins.CompUI.FixingStructure.CompUI_FixingStructure.PageType.InitCfgPage;
using Griffins.CompUI.FixingStructure.CompUI_FixingStructure.PageType.RecipeCfgPage;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.FixingStructure.CompUI_FixingStructure
{
    /// <summary>
    /// 固定机构子机械模组的组件 UI 入口，负责把初始化、配方页面注册给宿主。
    /// </summary>
    [CompUI(FixingStructureSubMachineModulesConst.SubMMModelStr, ImeIOTConst.CompType_SubMMStr)]
    public class FixingStructureCompUI : CompUIBase
    {
        /// <summary>电机固定机构子对象 ID，来自后端 SubMMObjInfos。</summary>
        private static readonly Guid AxisSubMMObjId = FixingStructureSubMachineModulesConst.SubMMObjInfos[1].SubMMObjID;

        protected override string _GetCompName()
        {
            return FixingStructureSubMachineModulesConst.SubMMName;
        }

        protected override IPageTypeDesignCompUI _GetPageTypeDesignCompUI(PageTypeID pageTypeID, Guid guid)
        {
            if (!IsAxisSubMM(guid))
            {
                return null;
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

        protected override IPageTypeRunTimeCompUI _GetPageTypeRunTimeCompUI(PageTypeID pageTypeID, Guid guid)
        {
            if (!IsAxisSubMM(guid))
            {
                return null;
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

        private static bool IsAxisSubMM(Guid guid) => guid == AxisSubMMObjId;
    }
}
