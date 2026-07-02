using Griffins.Map.PressureFuncCtlMapCell.View;
using Griffins.Map.PressureFuncCtlMapCell.ViewModel;
using Griffins.Map.UI;

namespace Griffins.Map.PressureFuncCtlMapCell.MapCell_Pressure
{
    internal class MapOprtCellParamCfgView : IMapOprtCellParamCfgView
    {
        private PressureValueMapOprtCellParamView view;
        private PressureValueMapOprtCellParamViewModel viewModel;
        public MapOprtCellParamCfgView()
        {
            view = new PressureValueMapOprtCellParamView();
            viewModel = new PressureValueMapOprtCellParamViewModel();
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
