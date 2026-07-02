using System;

namespace Griffins.CompUI.SL.InitCfgPage.Models
{
    public class TransportMotorItemCompUIModel
    {
        /// <summary>
        /// 前边轨下拉框被选中的项
        /// </summary>
        public OperationModeItem FrontSelectedItem { get; set; }
        /// <summary>
        /// 后边轨下拉框被选中的项
        /// </summary>
        public OperationModeItem BehindSelectedItem { get; set; }
    }

    public class OperationModeItem
    {
        // 下拉框展示的值
        public String DisPlayName { get; set; }
        // 选择下拉框某一项获得的值
        public OperationModeType Type { get; set; }
    }

    public enum OperationModeType
    {
        /// <summary>
        /// 自动
        /// </summary>
        Automatic = 0,
        /// <summary>
        /// 过板
        /// </summary>
        OverBoard = 1,
        /// <summary>
        /// 手动
        /// </summary>
        Manual = 2,
        /// <summary>
        /// 老化
        /// </summary>
        Aging = 3
    }
}
