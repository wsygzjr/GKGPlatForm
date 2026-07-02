using Newtonsoft.JsonG.Linq;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models
{
    /// <summary>
    /// 总面积检测配置信息
    /// </summary>
    public class TotalAreaDetectionCfgInfo : ScriptParamBase
    {
        /// <summary>
        /// 总面积下限
        /// </summary>
        public decimal MinTotalArea { get; set; } = 0m;

        /// <summary>
        /// 总面积上限
        /// </summary>
        public decimal MaxTotalArea { get; set; } = 1000m;

        /// <summary>
        /// 从 JObject 反序列化
        /// </summary>
        public override void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            MinTotalArea = jObject["MinTotalArea"]?.Value<decimal>() ?? MinTotalArea;
            MaxTotalArea = jObject["MaxTotalArea"]?.Value<decimal>() ?? MaxTotalArea;
        }

        /// <summary>
        /// 序列化为 JObject
        /// </summary>
        public override JObject ToJObject()
        {
            return new JObject
            {
                { "MinTotalArea", MinTotalArea },
                { "MaxTotalArea", MaxTotalArea }
            };
        }

        /// <summary>
        /// 复制数据
        /// </summary>
        public void CopyFrom(TotalAreaDetectionCfgInfo source)
        {
            if (source == null) return;

            MinTotalArea = source.MinTotalArea;
            MaxTotalArea = source.MaxTotalArea;
        }
    }
}
