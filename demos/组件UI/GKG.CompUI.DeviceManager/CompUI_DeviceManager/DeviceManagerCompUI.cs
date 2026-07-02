using GKG.CompUI.DeviceManager.ControlPanel;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace GKG.CompUI.DeviceManager
{
    /// <summary>
    /// 设备管理组件 UI
    /// </summary>
    [CompUI(ImeIOTConst.DevMngMMStr, "MM")]
    public class DeviceManagerCompUI : CompUIBase
    {
        // 获取组件名称
        protected override string _GetCompName() => ImeIOTConst.DevMngMMStr;

        // 获取设计时页面类型组件
        protected override IPageTypeDesignCompUI _GetPageTypeDesignCompUI(PageTypeID pageTypeID, Guid subMMObjID)
        {
            return null!;
        }

        // 获取运行时页面类型组件
        protected override IPageTypeRunTimeCompUI _GetPageTypeRunTimeCompUI(PageTypeID pageTypeID, Guid subMMObjID)
        {
            return null!;
        }

        // 创建控制面板
        protected override IControlPanel? _CreateControlPanel(Guid subMMObjID)
        {
            return new DeviceManagerControlPanelCompUI();
        }
    }
}
