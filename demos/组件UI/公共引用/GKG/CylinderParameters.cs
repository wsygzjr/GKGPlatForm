using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG
{
    public enum ECylinderType
    {
        SingleControlSingleLimit = 0,   //单控单限
        SingleControlDoubleLimit = 1,   //单控双限
        DoubleControlSingleLimit = 2,   //双控单限
        DoubleControlDoubleLimit = 3    //双控双限
    }

    /// <summary>
    /// 气缸初始化参数
    /// </summary>
    public class CylinderInitParameters
    {
        public ECylinderType eCylinderType { get; set; } = ECylinderType.SingleControlSingleLimit;
        /// <summary>
        /// 状态量IO列表
        /// </summary>
        public List<Guid> IOStateGuidList { get; set; } = new List<Guid>();
        /// <summary>
        /// 气缸超时时间，单位ms
        /// </summary>
        public int CylinderDelay { get; set; } = 100;
    }
}
