using Avalonia.Controls;
using Griffins.CompUI.RWS.CompUI_RWS.PageType.InitCfgPage.WorkStationInitConfig.ViewModels;

namespace Griffins.CompUI.RWS.CompUI_RWS.PageType.InitCfgPage.WorkStationInitConfig.Views;

public partial class WorkStationInitConfigCompUIView : UserControl
{
    public WorkStationInitConfigCompUIView()
    {
        InitializeComponent();
        if (Design.IsDesignMode)
        {
            this.DataContext = new WorkStationInitConfigCompUIViewModel(true, null);
        }
    }
}
