using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG.Map.ProductionInformationFuncCtlMapCell.ViewModel
{
	public class ProductionInformationValueMapOprtCellParamViewModel : ReactiveObject, IDisposable
	{
		private bool _showUnit;
		/// <summary>
		/// 显示单位
		/// </summary>
		public bool ShowUnit
		{
			get
			{
				return _showUnit;
			}
			set
			{
				this.RaiseAndSetIfChanged(ref _showUnit, value);
			}
		}

		private string _unit;
		/// <summary>
		/// 单位
		/// </summary>
		public string Unit
		{
			get
			{
				return _unit;
			}
			set
			{
				this.RaiseAndSetIfChanged(ref _unit, value);
			}
		}

		public void FromBytes(byte[] data)
		{
			if (data != null && data.Length > 0)
			{
				var temp= System.Text.Json.JsonSerializer.Deserialize<ProductionInformationValueMapOprtCellParamViewModel>(Encoding.UTF8.GetString(data));
				this.ShowUnit = temp.ShowUnit;
				this.Unit = temp.Unit;
			}
		}

		public byte[] ToBytes() 
		{
			return Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(this));
		}

		public void Dispose()
		{

		}
	}
}
