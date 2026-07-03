using Griffins;
using Griffins.PF;
using System;
using System.Collections.Generic;

namespace GKG
{
    /// <summary>
    /// 根据轴ID列表请求构建 RobotDriver。
    /// </summary>
    public class RobotDriverByAxisIdsRequest : MutualInfoBase
    {
        /// <summary>
        /// 互斥消息种类ID。
        /// </summary>
        public static readonly GriffinsInfoKindID InfoKindID = new GriffinsInfoKindID("{8E9C6B73-6DF6-4E5C-AF90-8A0B7EEA5191}");

        public RobotDriverByAxisIdsRequest()
        {
        }

        public RobotDriverByAxisIdsRequest(List<Guid> axisIds)
        {
            AxisIds = axisIds ?? new List<Guid>();
        }

        /// <summary>
        /// 轴ID列表（对应 AxisInformation.AxisGuid）。
        /// </summary>
        public List<Guid> AxisIds { get; set; } = new List<Guid>();
        /// <summary>
        /// 运控卡接口类型，对应了机械手类型，默认为 Normal（普通机械手）。不同类型的机械手可能需要不同的驱动实现。
        /// </summary>
        public MotionControlCardType MotionCardType { get; set; } = MotionControlCardType.Normal;
    }
}
