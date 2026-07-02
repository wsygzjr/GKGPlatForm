using Newtonsoft.JsonG.Linq;
using System;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.TrajectorySequence
{
    /// <summary>
    /// 点改变事件结构
    /// </summary>
    public class PointChangedEventArgs : EventArgs
    {
        /// <summary>
        /// X轴
        /// </summary>
        public decimal X { get; set; }

        /// <summary>
        /// Y轴
        /// </summary>
        public decimal Y { get; set; }

        /// <summary>
        /// Z轴
        /// </summary>
        public decimal Z { get; set; }
    }
}