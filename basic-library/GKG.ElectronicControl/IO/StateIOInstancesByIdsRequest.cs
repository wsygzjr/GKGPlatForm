using Griffins;
using Griffins.PF;
using System;
using System.Collections.Generic;

namespace GKG
{
    /// <summary>
    /// 根据IO状态量ID列表请求对应的状态量实例列表。
    /// </summary>
    public class StateIOInstancesByIdsRequest : MutualInfoBase
    {
        /// <summary>
        /// 互斥消息种类ID。
        /// </summary>
        public static readonly GriffinsInfoKindID InfoKindID = new GriffinsInfoKindID("{3FC130DA-B4FC-4FBB-84A8-90745AE6D7FA}");

        public StateIOInstancesByIdsRequest()
        {
        }

        public StateIOInstancesByIdsRequest(List<Guid> ioGuids)
        {
            IOGuids = ioGuids ?? new List<Guid>();
        }

        /// <summary>
        /// IO状态量GUID列表（对应 IOStateInformation.IOGuid）。
        /// </summary>
        public List<Guid> IOGuids { get; set; } = new List<Guid>();
    }
}
