using Newtonsoft.JsonG.Linq;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models
{
    /// <summary>
    /// 最大胶点面积检测配置信息
    /// </summary>
    public class MaxGlueAreaDetectionCfgInfo : ScriptParamBase
    {
        /// <summary>
        /// 最大区域下限
        /// </summary>
        public int MinMaxArea { get; set; } = 0;

        /// <summary>
        /// 最大区域上限
        /// </summary>
        public int MaxMaxArea { get; set; } = 1000;

        /// <summary>
        /// 从 JObject 反序列化
        /// </summary>
        public override void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            MinMaxArea = jObject["MinMaxArea"]?.Value<int>() ?? MinMaxArea;
            MaxMaxArea = jObject["MaxMaxArea"]?.Value<int>() ?? MaxMaxArea;
        }

        /// <summary>
        /// 序列化为 JObject
        /// </summary>
        public override JObject ToJObject()
        {
            return new JObject
            {
                { "MinMaxArea", MinMaxArea },
                { "MaxMaxArea", MaxMaxArea }
            };
        }

        /// <summary>
        /// 复制数据
        /// </summary>
        public void CopyFrom(MaxGlueAreaDetectionCfgInfo source)
        {
            if (source == null) return;

            MinMaxArea = source.MinMaxArea;
            MaxMaxArea = source.MaxMaxArea;
        }
    }
}

