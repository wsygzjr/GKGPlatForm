using Griffins.ImeIOT;
using Griffins.Map.UIDataObjProcessDesignTime.Simple.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffins.Map.UIDataObjProcessDesignTime.Simple.ViewModels
{
	internal class UIDataObjProcessSimpleCfgViewModel : ReactiveObject
	{
		private readonly UIDataObjProcessSimpleCfgInfo _model;

		public event EventHandler AfterModified;

		public UIDataObjProcessSimpleCfgViewModel(UIDataObjProcessSimpleCfgInfo model)
		{
			_model = model;
		}

		/// <summary>
		/// 印刷机械模组实例别名
		/// </summary>
		public MMAlias YS_Alias
		{
			get => _model.YS_Alias;
			set
			{
				_model.YS_Alias = value;
				this.RaisePropertyChanged();
			}
		}

		/// <summary>
		/// 备料机械模组实例别名
		/// </summary>
		public MMAlias BL_Alias
		{
			get => _model.BL_Alias;
			set
			{
				_model.BL_Alias = value;
				this.RaisePropertyChanged();
			}
		}

		/// <summary>
		/// 保存为 byte[]
		/// </summary>
		public byte[] ToBytes() => _model.ToBytes();
	}

}
