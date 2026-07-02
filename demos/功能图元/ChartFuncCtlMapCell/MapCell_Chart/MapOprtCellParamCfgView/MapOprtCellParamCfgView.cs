using GKG.Map.ChartFuncCtlMapCell.View;
using GKG.Map.ChartFuncCtlMapCell.ViewModel;
using Griffins.Map.UI;

namespace GKG.Map.ChartFuncCtlMapCell.MapCell_Chart
{
	internal class MapOprtCellParamCfgView: IMapOprtCellParamCfgView
	{
		private ChartValueMapOprtCellParamView view;
		private ChartValueMapOprtCellParamViewModel viewModel;
		public MapOprtCellParamCfgView() 
		{
			view = new ChartValueMapOprtCellParamView();
			viewModel = new ChartValueMapOprtCellParamViewModel();
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
