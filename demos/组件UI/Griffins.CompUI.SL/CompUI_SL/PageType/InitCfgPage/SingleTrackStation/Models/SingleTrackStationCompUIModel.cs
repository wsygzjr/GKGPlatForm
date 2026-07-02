using System.Collections.ObjectModel;
using Griffins.CompUI.SL.ComUI_SL.InitCfgPage.Models;

namespace Griffins.CompUI.SL.InitCfgPage.Models
{
    /// <summary>
    /// 主界面数据模型
    /// </summary>
    public class SingleTrackStationCompUIModel
    {
        /// <summary>
        /// 单层轨道工位列表
        /// </summary>
        public ObservableCollection<SingleTrackStationItemCompUIModel> Stations { get; set; } = new();
    }
}