using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using GKG.CompUI.DeviceManager.ControlPanel.ViewModels;
using GKG.CompUI.DeviceManager.ControlPanel.Views;
using Griffins.Map.UI;

namespace GKG.CompUI.DeviceManager.ControlPanel
{
    /// <summary>
    /// 设备管理控制面板组件
    /// </summary>
    internal class DeviceManagerControlPanelCompUI : IControlPanel
    {
        private const string PANEL_ID_EXEC_MODE = "CP_ExecMode";
        private IControlPanelCallBack? _controlPanelCallBack;

        // 获取控制面板信息列表
        ControlPanelViewInfoList IControlPanel.GetControlPanelViewInfos()
        {
            return new ControlPanelViewInfoList
            {
                new ControlPanelViewInfo(PANEL_ID_EXEC_MODE, "执行模式控制面板")
            };
        }

        // 初始化控制面板
        void IControlPanel.Init(IControlPanelCallBack iControlPanelCallBack)
        {
            _controlPanelCallBack = iControlPanelCallBack;
        }

        // 显示指定的控制面板
        async Task IControlPanel.ShowControlPanelAsync(string controlPanelID, object owner)
        {
            if (controlPanelID != PANEL_ID_EXEC_MODE)
            {
                throw new ArgumentException($"未知的控制面板 ID: {controlPanelID}");
            }

            var viewModel = new ExecModePanelViewModel(_controlPanelCallBack!);
            var panelView = new ExecModePanelView { DataContext = viewModel };

            var hostWindow = new Window
            {
                Title = "执行模式",
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

            var closeSubscription = viewModel.CloseRequested.Subscribe(_ =>
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
                    hostWindow.Closed += (_, _) => tcs.TrySetResult();
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
