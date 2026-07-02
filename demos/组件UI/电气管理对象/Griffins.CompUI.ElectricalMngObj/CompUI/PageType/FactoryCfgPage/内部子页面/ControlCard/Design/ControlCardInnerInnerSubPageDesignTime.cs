using Avalonia.Controls;
using Avalonia.Threading;
using Griffins.Map.UI;
using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Design;
using System;

namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage
{
    /// <summary>
    ///运控卡配置内部子页面设计时接口实现对象
    /// </summary>
    public class ControlCardInnerInnerSubPageDesignTime : IInnerSubPageDesignTime
    {
        /// <summary>
        /// 信息修改事件
        /// </summary>
        private event EventHandler? afterModified;
        private DesignCfgViewModel _designCfgViewModel;
        /// <summary>
        /// 构造函数
        /// </summary>
        public ControlCardInnerInnerSubPageDesignTime()
        {
            _designCfgViewModel = new DesignCfgViewModel();
            _designCfgViewModel.AfterModified += onafterModified;
        }
        #region ISubPageDesignTime接口
        /// <summary>
		/// 初始化
		/// </summary>
		/// <param name="viewCfgInfo">内部子页面界面配置信息</param>
		void ISubPageDesignTime.Init(byte[] viewCfgInfo)
        {
            _designCfgViewModel.CfgInfo = viewCfgInfo;
        }

        byte[] ISubPageDesignTime.ViewCfgInfo
        {
			get
			{
				return _designCfgViewModel.CfgInfo;
			}
		}

		event EventHandler ISubPageDesignTime.AfterModified
		{
			add
			{
				afterModified += value;
			}

			remove
			{
				afterModified -= value;
			}
		}

		void ISubPageDesignTime.Edit(object ower)
		{
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                ControlCardInnerSubPageDesignCfgWindow factoryCfgPageDesignCfgWindow = new ControlCardInnerSubPageDesignCfgWindow();
                factoryCfgPageDesignCfgWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                factoryCfgPageDesignCfgWindow.DataContext = _designCfgViewModel;
                bool result = await factoryCfgPageDesignCfgWindow.ShowDialog<bool>((Window)ower);
            });

            //ControlCardInnerSubPageDesignCfgWindow factoryCfgPageDesignCfgWindow = new ControlCardInnerSubPageDesignCfgWindow();
			//factoryCfgPageDesignCfgWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			//factoryCfgPageDesignCfgWindow.DataContext = _designCfgViewModel;
			//Task task=factoryCfgPageDesignCfgWindow.ShowDialog((Window)ower);
			//task.Wait(-1);

        }
		#endregion

		private void onafterModified(object? sender,EventArgs e)
		{
			afterModified?.Invoke(sender, e);

        }

    }
}