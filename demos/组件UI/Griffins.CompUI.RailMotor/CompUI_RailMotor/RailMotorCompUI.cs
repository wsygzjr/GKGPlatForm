using GF_Gereric;
using Griffins.CompUI.RailMotor.CompUI_RailMotor.Interop;
using Griffins.CompUI.RailMotor.CompUI_RailMotor.PageType.InitCfgPage;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.RailMotor.CompUI_RailMotor
{
    /// <summary>
    /// 轨道运输电机 CompUI 入口：按页面类型提供初始化配置的设计态与运行态界面。
    /// </summary>
    [CompUI(RailMotorInteropConst.ModelName, ImeIOTConst.CompType_SubMMStr)]
    public class RailMotorCompUI : CompUIBase
    {
        /// <summary>
        /// 返回组件在界面中显示的名称。
        /// </summary>
        protected override string _GetCompName()
        {
            return RailMotorInteropConst.DisplayName;
        }

        /// <summary>
        /// 根据子机械模组 GUID 与页面类型，创建设计态页面宿主。
        /// </summary>
        protected override IPageTypeDesignCompUI _GetPageTypeDesignCompUI(PageTypeID pageTypeID, Guid guid)
        {
            if (!RailMotorInteropConst.IsSupported(guid))
                return null;

            if (pageTypeID == ImeIOTConst.InitCfgPage)
                return new InitCfgPageTypeDesignCompUI();

            return null;
        }

        /// <summary>
        /// 根据子机械模组 GUID 与页面类型，创建运行态页面宿主。
        /// </summary>
        protected override IPageTypeRunTimeCompUI _GetPageTypeRunTimeCompUI(PageTypeID pageTypeID, Guid guid)
        {
            if (!RailMotorInteropConst.IsSupported(guid))
                return null;

            if (pageTypeID == ImeIOTConst.InitCfgPage)
                return new InitCfgPageTypeRunTimeCompUI();

            return null;
        }
    }
}
