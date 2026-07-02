using Avalonia.Controls;
using Avalonia.Threading;
using GF_Gereric;
using Griffins.ImeIOT;
using Griffins.Map;
using GKG.Map.UIDataObjProcessDesignTime.ProductionInfo.Models;
using GKG.Map.UIDataObjProcessDesignTime.ProductionInfo.ViewModels;
using GKG.Map.UIDataObjProcessDesignTime.ProductionInfo.Views;

namespace GKG.Map.UIDataObjProcessDesignTime.ProductionInfo
{
	[UIDataObjProcessDesignTime("ProductionInfo")]
	internal class UIDataObjProcessDesignTimeProductionInfoPlugin : GriffinsPluginMngClass, IUIDataObjProcessDesignTime
	{
		private UIDataObjProcessProductionInfoCfgInfo? _cfgInfo;

		private event EventHandler? _afterModified;

		void IUIDataObjProcessDesignTime.Init(string pluginFileName, byte[] cfgInfo)
		{
			_cfgInfo = new UIDataObjProcessProductionInfoCfgInfo();
			_cfgInfo.FromBytes(cfgInfo);
		}

		byte[] IUIDataObjProcessDesignTime.CfgInfo => _cfgInfo?.ToBytes()?? Array.Empty<byte>();

        event EventHandler IUIDataObjProcessDesignTime.AfterModified
		{
			add
			{
				_afterModified += value;
			}
			remove
			{
				_afterModified -= value;
			}
		}

		async Task IUIDataObjProcessDesignTime.Edit(object owner)
		{
			var uIDataObjProcessProductionInfoCfgViewModel = new UIDataObjProcessProductionInfoCfgViewModel(_cfgInfo);
            uIDataObjProcessProductionInfoCfgViewModel.PropertyChanged += UIDataObjProcessProductionInfoCfgViewModel_PropertyChanged;
			var uIDataObjProcessProductionInfoCfgWindow = new UIDataObjProcessProductionInfoCfgWindow();
            uIDataObjProcessProductionInfoCfgWindow.DataContext = uIDataObjProcessProductionInfoCfgViewModel;
			if (owner is Window window) await uIDataObjProcessProductionInfoCfgWindow.ShowDialog(window);
		}

		private void UIDataObjProcessProductionInfoCfgViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			_afterModified?.Invoke(null, null);
		}
	}
}
