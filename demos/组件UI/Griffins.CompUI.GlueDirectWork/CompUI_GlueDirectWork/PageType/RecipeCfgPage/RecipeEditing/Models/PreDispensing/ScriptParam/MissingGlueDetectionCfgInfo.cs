using Newtonsoft.JsonG.Linq;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models
{
    /// <summary>
    /// 漏点胶检测配置信息
    /// </summary>
    public class MissingGlueDetectionCfgInfo : ScriptParamBase
    {
        /// <summary>
        /// 点胶个数下限
        /// </summary>
        public int MinDispensingCount { get; set; } = 0;

        /// <summary>
        /// 点胶个数上限
        /// </summary>
        public int MaxDispensingCount { get; set; } = 100;

        /// <summary>
        /// 从 JObject 反序列化
        /// </summary>
        public override void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            MinDispensingCount = jObject["MinDispensingCount"]?.Value<int>() ?? MinDispensingCount;
            MaxDispensingCount = jObject["MaxDispensingCount"]?.Value<int>() ?? MaxDispensingCount;
        }

        /// <summary>
        /// 序列化为 JObject
        /// </summary>
        public override JObject ToJObject()
        {
            return new JObject
            {
                { "MinDispensingCount", MinDispensingCount },
                { "MaxDispensingCount", MaxDispensingCount }
            };
        }

        /// <summary>
        /// 复制数据
        /// </summary>
        public void CopyFrom(MissingGlueDetectionCfgInfo source)
        {
            if (source == null) return;

            MinDispensingCount = source.MinDispensingCount;
            MaxDispensingCount = source.MaxDispensingCount;
        }
    }
}
