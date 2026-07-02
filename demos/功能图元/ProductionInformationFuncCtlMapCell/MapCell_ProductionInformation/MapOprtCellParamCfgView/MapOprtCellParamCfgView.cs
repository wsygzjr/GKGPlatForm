using GKG.Map.ProductionInformationFuncCtlMapCell.View;
using GKG.Map.ProductionInformationFuncCtlMapCell.ViewModel;
using Griffins.Map;
using Griffins.Map.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG.Map.ProductionInformationFuncCtlMapCell.MapCell_ProductionInformation
{
	internal class MapOprtCellParamCfgView: IMapOprtCellParamCfgView
	{
		private ProductionInformationValueMapOprtCellParamView view;
		private ProductionInformationValueMapOprtCellParamViewModel viewModel;
		public MapOprtCellParamCfgView() 
		{
			view = new ProductionInformationValueMapOprtCellParamView();
			viewModel = new ProductionInformationValueMapOprtCellParamViewModel();
			view.DataContext = viewModel;
		}

		#region IMapOprtCellParamCfgView 接口成员
		object IMapOprtCellParamCfgView.View
		{
			get
			{
				return view;
			}
		}

		void IMapOprtCellParamCfgView.SetData(byte[] data)
		{
			viewModel.FromBytes(data);
		}

		byte[] IMapOprtCellParamCfgView.GetData()
		{
			return viewModel.ToBytes();
		}

		#endregion
	}
}
