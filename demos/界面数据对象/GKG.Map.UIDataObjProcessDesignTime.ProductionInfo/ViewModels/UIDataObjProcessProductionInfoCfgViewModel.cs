using Griffins.ImeIOT;
using GKG.Map.UIDataObjProcessDesignTime.ProductionInfo.Models;
using ReactiveUI;

namespace GKG.Map.UIDataObjProcessDesignTime.ProductionInfo.ViewModels
{
	internal class UIDataObjProcessProductionInfoCfgViewModel : ReactiveObject
	{
		private readonly UIDataObjProcessProductionInfoCfgInfo _cfgInfo;

		public event EventHandler AfterModified;

		public UIDataObjProcessProductionInfoCfgViewModel(UIDataObjProcessProductionInfoCfgInfo cfgInfo)
		{
			_cfgInfo = cfgInfo;
		}

		/// <summary>
		/// 印刷机械模组实例别名
		/// </summary>
		public MMAlias DeviceManager_Alias
        {
			get => _cfgInfo.DeviceManager_Alias;
			set
			{
				_cfgInfo.DeviceManager_Alias = value;
				this.RaisePropertyChanged();
			}
		}

		/// <summary>
		/// 保存为 byte[]
		/// </summary>
		public byte[] ToBytes() => _cfgInfo.ToBytes();
	}

}
