using Griffins.Map.UI;
using System.Threading.Tasks;

namespace Griffins.CompUI.PushRod.CompUI_PushRod.ControlPanel
{
    internal class PushRodControlPanel : IControlPanel
    {
        private IControlPanelCallBack _callBack;

        void IControlPanel.Init(IControlPanelCallBack iControlPanelCallBack)
        {
            _callBack = iControlPanelCallBack;
        }

        ControlPanelViewInfoList IControlPanel.GetControlPanelViewInfos()
        {
            return new ControlPanelViewInfoList();
        }

        public Task ShowControlPanelAsync(string controlPanelID, object owner)
        {
            return Task.CompletedTask;
        }
    }
}
