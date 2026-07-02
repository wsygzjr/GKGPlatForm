using Griffins.PF;
using System.Collections.Generic;

namespace GKG
{
    /// <summary>
    /// 根据IO状态量ID列表返回对应的状态量实例列表。
    /// </summary>
    public class StateIOInstancesByIdsResponse : MutualInfoResponseBase
    {
        /// <summary>
        /// IO状态量实例列表。
        /// </summary>
        public List<GKG.ElectronicControl.IBaseStateIO> StateIOInstances { get; set; } = new List<GKG.ElectronicControl.IBaseStateIO>();
    }
}
