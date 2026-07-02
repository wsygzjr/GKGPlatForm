using Avalonia.Controls;
using Avalonia.ReactiveUI;
using GKG.Map.Inspection2DFuncCtlMapCell.ViewModels;

namespace GKG.Map.Inspection2DFuncCtlMapCell.Views
{
    /// <summary>
    /// 2D检测图元视图后台类
    /// </summary>
    public partial class Inspection2DView : ReactiveUserControl<Inspection2DViewModel>
    {
        public Inspection2DView()
        {
            InitializeComponent();
        }
    }
}