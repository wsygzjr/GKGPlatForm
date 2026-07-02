namespace GKG.UI.General
{
    /// <summary>
    /// 双控单限位-模型
    /// </summary>
    public class DoubleControlSingleLimitCfgInfo
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
        public void CopyFrom(DoubleControlSingleLimitCfgInfo src)
        {
            if (src == null)
            {
                return;
            }

            FirstControlModel = src.FirstControlModel;
            SecondControlModel = src.SecondControlModel;
            LimitModel = src.LimitModel;
            CylinderDelayModel = src.CylinderDelayModel;
        }
    }
}
