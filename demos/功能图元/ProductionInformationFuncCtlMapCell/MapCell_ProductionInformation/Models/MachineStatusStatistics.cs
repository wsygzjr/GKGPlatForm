using ReactiveUI.Fody.Helpers;

namespace GKG.Map.ProductionInformationFuncCtlMapCell.Models
{
    public class MachineStatusStatistics
    {
        /// <summary>
        /// 机器状态类型
        /// </summary>
        [Reactive] public string Type { get; set; } = string.Empty;

        /// <summary>
        /// 总时间
        /// </summary>
        [Reactive] public string Time { get; set; } = string.Empty;

        /// <summary>
        /// 总次数
        /// </summary>
        [Reactive] public long Count { get; set; }

        /// <summary>
        /// 百分比
        /// </summary>
        [Reactive] public string Percentage { get; set; } = string.Empty;
    }

}
