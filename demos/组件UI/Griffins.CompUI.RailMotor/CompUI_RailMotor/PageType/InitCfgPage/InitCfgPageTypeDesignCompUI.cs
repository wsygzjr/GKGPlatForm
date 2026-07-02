using Griffins.CompUI.RailMotor.CompUI_RailMotor.PageType.InitCfgPage.RailMotorInit.Views;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.RailMotor.CompUI_RailMotor.PageType.InitCfgPage
{
    /// <summary>
    /// 初始化配置页设计态宿主：注册视图元数据并供设计器预览界面。
    /// </summary>
    internal class InitCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        /// <summary>返回本页面对应的页面类型 ID。</summary>
        protected override PageTypeID _GetPageTypeID()
        {
            return ImeIOTConst.InitCfgPage;
        }

        /// <summary>返回本页面包含的视图列表及显示名称。</summary>
        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            return new PageTypeCompUIViewInfoList
            {
                new PageTypeCompUIViewInfo
                {
                    ViewID = InitCfgPageTypeConst.ViewID_RailMotorInit,
                    ViewName = "运输电机初始化配置",
                },
            };
        }

        /// <summary>按 ViewID 创建用于设计器预览的 Avalonia 视图实例。</summary>
        protected override object _GetPageTypeCompUIView(string viewID)
        {
            return viewID switch
            {
                InitCfgPageTypeConst.ViewID_RailMotorInit => new RailMotorInitCompUIView(),
                _ => null,
            };
        }
    }
}
