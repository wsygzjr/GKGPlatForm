using Avalonia.Controls;
using Avalonia.Threading;
using GF_Gereric;
using Griffins.ImeIOT;
using Griffins.Map;
using Griffins.Map.UIDataObjProcessDesignTime.Simple.Models;
using Griffins.Map.UIDataObjProcessDesignTime.Simple.ViewModels;
using Griffins.Map.UIDataObjProcessDesignTime.Simple.Views;

namespace Griffins.Map.UIDataObj.Simple
{
	[UIDataObjProcessDesignTime("Simple")]
	internal class UIDataObjProcessDesignTimeSimplePlugin : GriffinsPluginMngClass, IUIDataObjProcessDesignTime
	{
		private UIDataObjProcessSimpleCfgInfo _cfgInfo;

		private event EventHandler _afterModified;

		void IUIDataObjProcessDesignTime.Init(string pluginFileName, byte[] cfgInfo)
		{
			_cfgInfo = new UIDataObjProcessSimpleCfgInfo();
			_cfgInfo.FromBytes(cfgInfo);
		}

		byte[] IUIDataObjProcessDesignTime.CfgInfo
		{
			get
			{
				return _cfgInfo.ToBytes();
			}
		}

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
			var uIDataObjProcessSimpleCfgViewModel = new UIDataObjProcessSimpleCfgViewModel(_cfgInfo);
			uIDataObjProcessSimpleCfgViewModel.PropertyChanged += UIDataObjProcessSimpleCfgViewModel_PropertyChanged;
			var uIDataObjProcessSimpleCfgWindow = new UIDataObjProcessSimpleCfgWindow();
			uIDataObjProcessSimpleCfgWindow.DataContext = uIDataObjProcessSimpleCfgViewModel;
			await uIDataObjProcessSimpleCfgWindow.ShowDialog(owner as Window);
		}

		private void UIDataObjProcessSimpleCfgViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			_afterModified?.Invoke(null, null);
		}
	}
}
