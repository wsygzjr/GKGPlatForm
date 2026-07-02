using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using GKG.CompUI.LoadUnload.ControlPanel.ViewModels;
using GKG.CompUI.LoadUnload.ControlPanel.Views;
using Griffins.Map.UI;

namespace GKG.CompUI.LoadUnload.ControlPanel
{
    internal class LoadUnloadControlPanelCompUI : IControlPanel
    {
        private const string PANEL_ID_LOAD = "CP_OneClickLoad";
        private const string PANEL_ID_UNLOAD = "CP_OneClickUnload";
        private const string PANEL_ID_INSPECT = "CP_OneClickInspect";

        private IControlPanelCallBack _controlPanelCallBack = null!;

        ControlPanelViewInfoList IControlPanel.GetControlPanelViewInfos()
        {
            var list = new ControlPanelViewInfoList
            {
                new ControlPanelViewInfo(PANEL_ID_LOAD, "一键上料控制面板"),
                new ControlPanelViewInfo(PANEL_ID_UNLOAD, "一键下料控制面板"),
                new ControlPanelViewInfo(PANEL_ID_INSPECT, "一键抽检控制面板")
            };
            return list;
        }

        void IControlPanel.Init(IControlPanelCallBack iControlPanelCallBack)
        {
            _controlPanelCallBack = iControlPanelCallBack;
        }

        async Task IControlPanel.ShowControlPanelAsync(string controlPanelID, object owner)
        {
            SopWizardMode targetMode;
            switch (controlPanelID)
            {
                case PANEL_ID_LOAD:
                    targetMode = SopWizardMode.Load;
                    break;
                case PANEL_ID_UNLOAD:
                    targetMode = SopWizardMode.Unload;
                    break;
                case PANEL_ID_INSPECT:
                    targetMode = SopWizardMode.Inspect;
                    break;
                default:
                    throw new ArgumentException($"未知的控制面板 ID: {controlPanelID}");
            }

            var wizardVm = new SopWizardViewModel(_controlPanelCallBack, targetMode);

            var panelView = new SopWizardView { DataContext = wizardVm };

            var hostWindow = new Window
            {
                Title = wizardVm.WizardTitle,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false,
                ShowInTaskbar = false,
                ExtendClientAreaToDecorationsHint = true,
                ExtendClientAreaTitleBarHeightHint = 0,
                SystemDecorations = SystemDecorations.None,
                WindowState = WindowState.FullScreen,
                Background = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0)),
                Content = panelView
            };

            var closeSubscription = wizardVm.CloseRequested.Subscribe(_ =>
            {
                Dispatcher.UIThread.InvokeAsync(() => hostWindow.Close());
            });

            try
            {
                if (owner is Window ownerWindow)
                {
                    await hostWindow.ShowDialog(ownerWindow);
                }
                else
                {
                    hostWindow.Show();
                    var tcs = new TaskCompletionSource();
                    hostWindow.Closed += (s, e) => tcs.TrySetResult();
                    await tcs.Task;
                }
            }
            finally
            {
                closeSubscription?.Dispose();
            }
        }
    }
}