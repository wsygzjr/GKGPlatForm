using Avalonia.Controls;
using AvaloniaVisionControl;
using GKG.UI.General;
using Griffins.Map.UI;

namespace Griffins.CompUI.Vision;

public partial class ControlPanelWindow : Window
{
	public ControlPanelWindow()
	{
		InitializeComponent();
	}

	public ControlPanelWindow(IControlPanelCallBack iControlPanelCallBack)
	{
		InitializeComponent();
		DataContext = new ControlPanelViewModel(iControlPanelCallBack, this.FindControl<VisionControlShowView>("CameraShow"));
    }
}
