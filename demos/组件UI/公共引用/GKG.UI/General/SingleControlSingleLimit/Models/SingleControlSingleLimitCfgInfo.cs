using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG.UI.General
{
    /// <summary>
    /// 单控单限位-模型
    /// </summary>
    public class SingleControlSingleLimitCfgInfo
    {
        /// <summary>
        /// 控制-数据模型
        /// </summary>
        public HorizontalControlCardStateInitCfgInfo? ControlModel { get; set; }

        /// <summary>
        /// 限位-数据模型
        /// </summary>
        public HorizontalControlCardStateInitCfgInfo? LimitModel { get; set; }

        /// <summary>
        /// 气缸超时时间模型
        /// </summary>
        public CylinderDelayCfgInfo? CylinderDelayModel { get; set; }

        /// <summary>
        /// 从另一个实例复制数据
        /// </summary>
        /// <param name="src">数据源</param>
        public void CopyFrom(SingleControlSingleLimitCfgInfo src)
        {
            if (src == null)
            {
                return;
            }

            ControlModel = src.ControlModel;
            LimitModel = src.LimitModel;
            CylinderDelayModel = src.CylinderDelayModel;
        }
    }
}
