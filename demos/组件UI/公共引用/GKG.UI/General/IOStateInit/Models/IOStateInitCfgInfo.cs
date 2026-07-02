using Newtonsoft.JsonG.Linq;

namespace GKG.UI.General
{
    /// <summary>
    /// IO状态量初始化 配置模型
    /// </summary>
    public class IOStateInitCfgInfo
    {
        /// <summary>
        /// IO设备ID
        /// </summary>
        public string IODeviceID { get; set; } = "";

        /// <summary>
        /// IO通道号（枚举）
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

            IODeviceID = jObject["IODeviceID"]?.Value<string>() ?? IODeviceID;
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
                { "IODeviceID", IODeviceID },
                { "IOChannel", (int)IOChannel }
            };

            return obj;
        }

        /// <summary>
        /// 从另一个实例复制数据
        /// </summary>
        /// <param name="src">数据源</param>
        public void CopyFrom(IOStateInitCfgInfo src)
        {
            if (src == null)
            {
                return;
            }

            IODeviceID = src.IODeviceID;
            IOChannel = src.IOChannel;
        }
    }
}