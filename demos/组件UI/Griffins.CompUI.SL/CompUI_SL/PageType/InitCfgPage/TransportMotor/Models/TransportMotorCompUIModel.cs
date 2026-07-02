using Griffins.CompUI.SL.ComUI_SL.InitCfgPage.Models;
using System;
using System.Collections.ObjectModel;

namespace Griffins.CompUI.SL.InitCfgPage.Models
{
    public class TransportMotorCompUIModel
    {
        /// <summary>
        /// 单层轨道电机列表
        /// </summary>
        public ObservableCollection<TransportMotorItemCompUIModel> Motors { get; set; } = new ObservableCollection<TransportMotorItemCompUIModel>();
    }

    
}
