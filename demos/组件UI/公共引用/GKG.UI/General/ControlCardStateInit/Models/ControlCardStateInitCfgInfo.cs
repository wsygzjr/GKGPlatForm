using Newtonsoft.JsonG.Linq;

namespace GKG.UI.General
{
    /// <summary>
    /// 运控卡状态量初始化 配置模型
    /// </summary>
    public class ControlCardStateInitCfgInfo
    {
        /// <summary>
        /// 运控卡ID
        /// </summary>
        public string ControlCardID { get; set; } = "";

        public ControlCardType CardType { get; set; } = ControlCardType.GC800;

        /// <summary>
        /// IO通道号
        /// </summary>
        public IOChannelType IOChannel { get; set; } = IOChannelType.Input64;

        /// <summary>
        /// 从 JObject 反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null)
            {
                return;
            }

            ControlCardID = jObject["ControlCardID"]?.Value<string>() ?? ControlCardID;
            if (jObject["CardType"] != null)
            {
                CardType = (ControlCardType)jObject["CardType"]!.Value<int>();
            }
            IOChannel = jObject["IOChannel"] != null
                ? (IOChannelType)jObject["IOChannel"]!.Value<int>()
                : IOChannel;
        }

        /// <summary>
        /// 序列化为 JObject
        /// </summary>
        public JObject ToJObject()
        {
            var obj = new JObject
            {
                { "ControlCardID", ControlCardID },
                { "CardType", (int)CardType },
                { "IOChannel", (int)IOChannel }
            };

            return obj;
        }

        /// <summary>
        /// 从另一个实例复制数据
        /// </summary>
        /// <param name="src">数据源</param>
        public void CopyFrom(ControlCardStateInitCfgInfo src)
        {
            if (src == null)
            {
                return;
            }

            ControlCardID = src.ControlCardID;
            CardType = src.CardType;
            IOChannel = src.IOChannel;
        }
    }
}