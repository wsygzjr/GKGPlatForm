using Newtonsoft.JsonG.Linq;

namespace GKG.UI.General
{
    /// <summary>
    /// 光源信息
    /// </summary>
    public class LightSourceCfgInfo
    {
        /// <summary>
        /// R通道亮度（0-255）
        /// </summary>
        public decimal R { set; get; } = 0.0m;

        /// <summary>
        /// G通道亮度（0-255）
        /// </summary>
        public decimal G { set; get; } = 0.0m;

        /// <summary>
        /// B通道亮度（0-255）
        /// </summary>
        public decimal B { set; get; } = 0.0m;

        /// <summary>
        /// 
        /// </summary>
        public decimal D { set; get; } = 0.0m; 
        /// <summary>
        /// D通道亮度（0-255）
        /// </summary>
        public LightSourceCfgInfo() { }

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            R = jObject["R"]?.Value<decimal>() ?? 0.0m;
            G = jObject["G"]?.Value<decimal>() ?? 0.0m;
            B = jObject["B"]?.Value<decimal>() ?? 0.0m;
            D = jObject["D"]?.Value<decimal>() ?? 0.0m;
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public JObject ToJObject()
        {
            return new JObject
        {
            { "R", R },
            { "G", G },
            { "B", B },
            { "D", D }
        };
        }
        /// <summary>
        /// 复制
        /// </summary>
        public void CopyFrom(LightSourceCfgInfo source)
        {
            if (source == null) return;

            R = source.R;
            G = source.G;
            B = source.B;
            D = source.D;
        }
    }
}
