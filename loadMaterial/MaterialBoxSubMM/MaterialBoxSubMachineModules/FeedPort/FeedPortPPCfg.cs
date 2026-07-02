namespace MaterialBoxSubMachineModules.FeedPort
{
    /// <summary>
    /// 送料口/接料口配方配置。
    /// </summary>
    public class FeedPortPPCfg
    {
        /// <summary>
        /// 物料到位感应时间，单位 ms。
        /// </summary>
        public double MaterialArrivedSenseTime { get; set; }

        /// <summary>
        /// 从另一份送料口/接料口配方配置复制参数。
        /// </summary>
        /// <param name="source">复制源。</param>
        public void CopyFrom(FeedPortPPCfg source)
        {
            if (source == null)
                return;

            MaterialArrivedSenseTime = source.MaterialArrivedSenseTime;
        }
    }
}
