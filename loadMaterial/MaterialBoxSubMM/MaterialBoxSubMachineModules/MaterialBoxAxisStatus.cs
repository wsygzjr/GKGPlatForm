using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG.MM
{
    /// <summary>
    /// 料盒轴状态变更通知参数。
    /// </summary>
    public class MaterialBoxAxisStatus
    {
        /// <summary>
        /// 状态码；0 表示轴位置已变化，前端需主动查询当前坐标。
        /// </summary>
        public int Staus { get; set; }

        /// <summary>
        /// 轴侧别（Load/Unload）。
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// 当前轴坐标；与位置变更通知一并下发，前端可直接刷新显示。
        /// </summary>
        public double Position { get; set; }
    }
}
