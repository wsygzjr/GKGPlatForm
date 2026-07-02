using Newtonsoft.JsonG.Linq;

namespace GKG.UI.General
{
    /// <summary>
    /// 相机显示配置信息
    /// </summary>
    public class CameraShowCfgInfo
    {
        /// <summary>
        /// 偏移标定信息（光源配置）
        /// </summary>
        public LightSourceCfgInfo LightSourceCfgInfo { set; get; } = new LightSourceCfgInfo();

        /// <summary>
        /// 坐标轴配置信息
        /// </summary>
        public AxisCfgInfo AxisCfgInfo { set; get; } = new AxisCfgInfo();

        /// <summary>
        /// 图像显示设置配置信息
        /// </summary>
        public ImageDisplayCfgInfo ImageDisplayCfgInfo { set; get; } = new ImageDisplayCfgInfo();
        /// <summary>
        /// 构造函数
        /// </summary>
        public CameraShowCfgInfo()
        {
            LightSourceCfgInfo = new LightSourceCfgInfo();
            AxisCfgInfo = new AxisCfgInfo();
            ImageDisplayCfgInfo = new ImageDisplayCfgInfo();
        }

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            LightSourceCfgInfo.FromJObject(jObject["LightSourceCfgInfo"] as JObject);
            AxisCfgInfo.FromJObject(jObject["AxisCfgInfo"] as JObject);
            ImageDisplayCfgInfo.FromJObject(jObject["ImageDisplayCfgInfo"] as JObject);

        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public JObject ToJObject()
        {
            return new JObject
        {
            { "LightSourceCfgInfo", LightSourceCfgInfo.ToJObject() },
            { "AxisCfgInfo", AxisCfgInfo.ToJObject() },
            { "ImageDisplayCfgInfo", ImageDisplayCfgInfo.ToJObject() }
        };
        }
        /// <summary>
        /// 复制
        /// </summary>
        public void CopyFrom(CameraShowCfgInfo source)
        {
            if (source == null) return;

            LightSourceCfgInfo.CopyFrom(source.LightSourceCfgInfo);
            AxisCfgInfo.CopyFrom(source.AxisCfgInfo);
            ImageDisplayCfgInfo.CopyFrom(source.ImageDisplayCfgInfo);
        }
    }
}
