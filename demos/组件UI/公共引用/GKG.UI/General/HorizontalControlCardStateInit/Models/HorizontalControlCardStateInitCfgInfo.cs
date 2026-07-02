namespace GKG.UI.General
{
    /// <summary>
    /// 运控卡状态量初始化 配置模型
    /// </summary>
    public class HorizontalControlCardStateInitCfgInfo
    {
        /// <summary>
        /// 运控卡ID
        /// </summary>
        public string ControlCardID { get; set; } = "";

        public ControlCardType CardType { get; set; } = ControlCardType.GC800;

        /// <summary>
        /// IO通道号
        /// </summary>
        public string channelID { get; set; } = string.Empty;

        /// <summary>
        /// 从另一个实例复制数据
        /// </summary>
        /// <param name="src">数据源</param>
        public void CopyFrom(HorizontalControlCardStateInitCfgInfo src)
        {
            if (src == null)
            {
                return;
            }

            ControlCardID = src.ControlCardID;
            CardType = src.CardType;
            channelID = src.channelID;
        }
    }
}
