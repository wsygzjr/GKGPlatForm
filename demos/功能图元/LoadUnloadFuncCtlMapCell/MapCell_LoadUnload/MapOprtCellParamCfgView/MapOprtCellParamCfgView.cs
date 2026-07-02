using GKG.Map.LoadUnloadFuncCtlMapCell.ViewModels;
using GKG.Map.LoadUnloadFuncCtlMapCell.Views;
using Griffins.Map.UI;

namespace GKG.Map.LoadUnloadFuncCtlMapCell.MapCell_LoadUnload
{
    /// <summary>
    /// 操作原子参数配置视图的集成包装类
    /// 实现 Griffins 框架要求的接口，打通前端 UI 与底层字节流的数据流转。
    /// </summary>
    internal class MapOprtCellParamCfgView : IMapOprtCellParamCfgView
    {
        private readonly LoadUnloadValueMapOprtCellParamView _view;
        private readonly LoadUnloadValueMapOprtCellParamViewModel _viewModel;

        /// <summary>
        /// 初始化配置视图，并将 ViewModel 强绑定到 DataContext
        /// </summary>
        public MapOprtCellParamCfgView()
        {
            _view = new LoadUnloadValueMapOprtCellParamView();
            _viewModel = new LoadUnloadValueMapOprtCellParamViewModel();
            _view.DataContext = _viewModel;
        }

        #region IMapOprtCellParamCfgView 接口实现

        /// <summary>
        /// 向底层框架暴露 UI 视图对象
        /// </summary>
        object IMapOprtCellParamCfgView.View => _view;

        /// <summary>
        /// 接收框架下发的历史配置字节流，并反序列化注入 ViewModel
        /// </summary>
        void IMapOprtCellParamCfgView.SetData(byte[] data) => _viewModel.FromBytes(data);

        /// <summary>
        /// 从 ViewModel 提取当前配置，序列化为字节流供框架持久化保存
        /// </summary>
        byte[] IMapOprtCellParamCfgView.GetData() => _viewModel.ToBytes();

        #endregion
    }
}