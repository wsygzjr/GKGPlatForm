using System;
using System.Collections.Generic;

namespace MaterialBoxSubMachineModules.FeedPort
{
    /// <summary>
    /// 送料口/接料口初始化配置。
    /// </summary>
    public class FeedPortInitCfg
    {
        /// <summary>
        /// 料口绑定的检测 IO 集合。
        /// 当前通常只配置 1 个，后续可扩展为多个。
        /// </summary>
        public List<Guid> SensorIOGuids { get; set; } = new List<Guid>();
    }
}
