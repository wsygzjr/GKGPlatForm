using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG.UI.General
{
    /// <summary>
    /// 气缸延时-模型
    /// </summary>
    public class CylinderDelayCfgInfo
    {
        /// <summary>
        /// 气缸延时-当前输入值
        /// </summary>
        public decimal DelayNumeric { get; set; } = 100;

        /// <summary>
        /// 从另一个实例拷贝
        /// </summary>
        public void CopyFrom(CylinderDelayCfgInfo source)
        {
            if (source == null)
            {
                return;
            }
            DelayNumeric = source.DelayNumeric;
        }

    }

}
