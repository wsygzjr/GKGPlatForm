using Avalonia;
using Avalonia.Controls;
using Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.InitCfgPage.AdjustWidthInit.ViewModels;

namespace Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.InitCfgPage.AdjustWidthInit.Views
{
    public partial class AdjustWidthInitCompUIView : UserControl
    {
        public AdjustWidthInitCompUIView()
        {
            InitializeComponent();
            if (Design.IsDesignMode)
            {
                DataContext = new AdjustWidthInitCompUIViewModel();
            }
        }
    }
}
