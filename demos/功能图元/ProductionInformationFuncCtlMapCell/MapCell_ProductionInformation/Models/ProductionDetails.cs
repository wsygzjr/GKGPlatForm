using ReactiveUI.Fody.Helpers;

namespace GKG.Map.ProductionInformationFuncCtlMapCell.Models
{
    public class ProductionDetails
    {
        /// <summary>
        /// 时间区间
        /// </summary>
        [Reactive] public string TimeInterval { get; set; } = string.Empty;

        /// <summary>
        /// 程序名称
        /// </summary>
        [Reactive] public string ProgramName { get; set; } = string.Empty;

        /// <summary>
        /// 生产整板数量
        /// </summary>
        [Reactive] public int BigPcs { get; set; }

        /// <summary>
        /// 生产子板数量
        /// </summary>
        [Reactive] public int SmallPcs { get; set; }
    }

}
