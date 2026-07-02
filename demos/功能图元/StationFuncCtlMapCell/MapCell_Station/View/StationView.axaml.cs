using Avalonia.ReactiveUI;
using GKG.Map.StationFuncCtlMapCell.ViewModel;

namespace GKG.Map.StationFuncCtlMapCell.View
{
    /// <summary>
    /// 工位功能图元 UI 视图代码后置
    /// 继承自 ReactiveUserControl，提供强大的生命周期和自动绑定管理
    /// </summary>
    public partial class StationView : ReactiveUserControl<StationViewModel>
    {
        public StationView()
        {
            InitializeComponent();
        }
    }
}