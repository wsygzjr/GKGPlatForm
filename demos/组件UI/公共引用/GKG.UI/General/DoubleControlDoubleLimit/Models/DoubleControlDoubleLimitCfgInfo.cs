using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG.UI.General
{
    /// <summary>
    /// 双控双限位-模型
    /// </summary>
    public class DoubleControlDoubleLimitCfgInfo
    {
        /// <summary>
        /// 控制1-数据模型
        /// </summary>
        public HorizontalControlCardStateInitCfgInfo? FirstControlModel { get; set; }

        /// <summary>
        /// 控制2-数据模型
        /// </summary>
        public HorizontalControlCardStateInitCfgInfo? SecondControlModel { get; set; }

        /// <summary>
        /// 限位1-数据模型
        /// </summary>
        public HorizontalControlCardStateInitCfgInfo? FirstLimitModel { get; set; }

        /// <summary>
        /// 限位2-数据模型
        /// </summary>
        public HorizontalControlCardStateInitCfgInfo? SecondLimitModel { get; set; }

        /// <summary>
        /// 气缸超时时间模型
        /// </summary>
        public CylinderDelayCfgInfo? CylinderDelayModel { get; set; }

        /// <summary>
        /// 从另一个实例复制数据
        /// </summary>
        /// <param name="src">数据源</param>
        public void CopyFrom(DoubleControlDoubleLimitCfgInfo src)
        {
            if (src == null)
            {
                return;
            }

            FirstControlModel = src.FirstControlModel;
            SecondControlModel = src.SecondControlModel;
            FirstLimitModel = src.FirstLimitModel;
            SecondLimitModel = src.SecondLimitModel;
            CylinderDelayModel = src.CylinderDelayModel;
        }
    }
}
