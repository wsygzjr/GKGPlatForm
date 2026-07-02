using Newtonsoft.JsonG.Linq;

namespace GKG.UI.General
{
    /// <summary>
    /// 图像显示设置配置信息
    /// </summary>
    public class ImageDisplayCfgInfo
    {
        /// <summary>
        /// 是否开启刻度
        /// </summary>
        public bool EnabledScale { set; get; } = false;

        /// <summary>
        /// 是否开启参照圆
        /// </summary>
        public bool EnabledReferenceCircle { set; get; } = false;

        /// <summary>
        /// 刻度（mm）
        /// </summary>
        public decimal Scale { set; get; } = 1.0m; // 默认1mm刻度

        /// <summary>
        /// 参照圆（mm）
        /// </summary>
        public decimal ReferenceCircle { set; get; } = 5.0m; // 默认5mm参照圆

        /// <summary>
        /// 是否开启调试亮度
        /// </summary>
        public bool EnabledBrightnessDebugging { set; get; } = false;
        /// <summary>
        /// 构造函数
        /// </summary>
        public ImageDisplayCfgInfo() { }

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            EnabledScale = jObject["EnabledScale"]?.Value<bool>() ?? false;
            EnabledReferenceCircle = jObject["EnabledReferenceCircle"]?.Value<bool>() ?? false;
            EnabledBrightnessDebugging = jObject["EnabledBrightnessDebugging"]?.Value<bool>() ?? false;
            Scale = jObject["Scale"]?.Value<decimal>() ?? 1.0m;
            ReferenceCircle = jObject["ReferenceCircle"]?.Value<decimal>() ?? 5.0m;
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public JObject ToJObject()
        {
            return new JObject
        {
            { "EnabledScale", EnabledScale },
            { "EnabledReferenceCircle", EnabledReferenceCircle },
            { "Scale", Scale },
            { "ReferenceCircle", ReferenceCircle },
            { "EnabledBrightnessDebugging", EnabledBrightnessDebugging }
        };
        }

        /// <summary>
        /// 复制
        /// </summary>
        public void CopyFrom(ImageDisplayCfgInfo source)
        {
            if (source == null) return;

            EnabledScale = source.EnabledScale;
            EnabledReferenceCircle = source.EnabledReferenceCircle;
            Scale = source.Scale;
            ReferenceCircle = source.ReferenceCircle;
            EnabledBrightnessDebugging = source.EnabledBrightnessDebugging;
        }
    }
}
