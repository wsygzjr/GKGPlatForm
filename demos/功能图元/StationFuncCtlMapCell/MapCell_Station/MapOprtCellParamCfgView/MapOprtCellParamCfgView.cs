using GKG.Map.StationFuncCtlMapCell.View;
using GKG.Map.StationFuncCtlMapCell.ViewModel;
using Griffins.Map.UI;

namespace GKG.Map.StationFuncCtlMapCell.MapCell_Station
{
    /// <summary>
    /// 实现操作原子参数视图配置界面包装类
    /// </summary>
    internal class MapOprtCellParamCfgView : IMapOprtCellParamCfgView
    {
        private StationMapOprtCellParamView view;
        private StationMapOprtCellParamViewModel viewModel;

        public MapOprtCellParamCfgView()
        {
            view = new StationMapOprtCellParamView();
            viewModel = new StationMapOprtCellParamViewModel();
            view.DataContext = viewModel;
        }

        #region IMapOprtCellParamCfgView 接口成员

        object IMapOprtCellParamCfgView.View => view;

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