using GKG.Map.CameraViewerFuncCtlMapCell.View;
using GKG.Map.CameraViewerFuncCtlMapCell.ViewModel;
using Griffins.Map.UI;

namespace GKG.Map.CameraViewerFuncCtlMapCell
{
	internal class MapOprtCellParamCfgView: IMapOprtCellParamCfgView
	{
		private CameraViewerValueMapOprtCellParamView view;
		private CameraViewerValueMapOprtCellParamViewModel viewModel;
		public MapOprtCellParamCfgView() 
		{
			view = new CameraViewerValueMapOprtCellParamView();
			viewModel = new CameraViewerValueMapOprtCellParamViewModel();
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
