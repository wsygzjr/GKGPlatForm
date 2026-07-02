using Avalonia.Controls;
using Griffins.Map.UI;

namespace Griffins.CompUI.Vision
{
	/// <summary>
	/// 控制面板对象
	/// </summary>
	internal class MainControlPanel : IControlPanel
	{
		/// <summary>
		/// 组件控制面板回调接口实例
		/// </summary>
		private IControlPanelCallBack iControlPanelCallBack;

		public MainControlPanel()
		{
		}

		#region IControlPanel 成员

		/// <summary>
		/// 组件控制面板初始化
		/// </summary>
		/// <param name="iControlPanelCallBack">组件控制面板回调接口实例</param>
		void IControlPanel.Init(IControlPanelCallBack iControlPanelCallBack)
		{
			this.iControlPanelCallBack = iControlPanelCallBack;
		}

		/// <summary>
		/// 获取控制面板信息列表，null或个数为0表示没有对应的控制面板
		/// </summary>
		/// <returns>该机械模组的控制面板信息列表</returns>
		ControlPanelViewInfoList IControlPanel.GetControlPanelViewInfos()
		{
			return new ControlPanelViewInfoList()
			{
				new ControlPanelViewInfo()
				{
					ControlPanelID = ControlPanelConst.ControlPanelID_VisionAnalysis,
					ControlPanelName = "视觉分析"
				}
			};
		}

		/// <summary>
		/// 显示控制面板
		/// </summary>
		/// <param name="controlPanelID">控制面板ID</param>
		/// <param name="owner">父窗口</param>
		
		

        public Task ShowControlPanelAsync(string controlPanelID, object owner)
        {
            switch (controlPanelID)
            {
                case ControlPanelConst.ControlPanelID_VisionAnalysis:
                    var window = new ControlPanelWindow(iControlPanelCallBack);
                    return window.ShowDialog((Window)owner);
                   
                default:
                   return Task.CompletedTask;
            }
        }

        #endregion
    }
}
