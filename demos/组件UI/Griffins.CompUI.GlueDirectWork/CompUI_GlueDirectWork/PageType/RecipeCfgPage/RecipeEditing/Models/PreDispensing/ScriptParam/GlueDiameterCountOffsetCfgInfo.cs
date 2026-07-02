using Newtonsoft.JsonG.Linq;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models
{
    /// <summary>
    /// 胶水直径计数偏移配置信息
    /// </summary>
    public class GlueDiameterCountOffsetCfgInfo : ScriptParamBase
    {
        /// <summary>
        /// 当前胶点直径
        /// </summary>
        public decimal CurrentGlueDiameter { get; set; } = 0m;

        /// <summary>
        /// 当前个数
        /// </summary>
        public int CurrentCount { get; set; } = 0;

        /// <summary>
        /// 当前X偏移
        /// </summary>
        public decimal CurrentXOffset { get; set; } = 0m;

        /// <summary>
        /// 当前Y偏移
        /// </summary>
        public decimal CurrentYOffset { get; set; } = 0m;

        /// <summary>
        /// 胶点直径上限
        /// </summary>
        public decimal MaxGlueDiameter { get; set; } = 10m;

        /// <summary>
        /// 胶点直径下限
        /// </summary>
        public decimal MinGlueDiameter { get; set; } = 0m;

        /// <summary>
        /// 胶点X正偏移
        /// </summary>
        public decimal MaxXOffset { get; set; } = 100m;

        /// <summary>
        /// 胶点X负偏移
        /// </summary>
        public decimal MinXOffset { get; set; } = -100m;

        /// <summary>
        /// 胶点Y正偏移
        /// </summary>
        public decimal MaxYOffset { get; set; } = 100m;

        /// <summary>
        /// 胶点Y负偏移
        /// </summary>
        public decimal MinYOffset { get; set; } = -100m;

        /// <summary>
        /// 从 JObject 反序列化
        /// </summary>
        public override void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            CurrentGlueDiameter = jObject["CurrentGlueDiameter"]?.Value<decimal>() ?? CurrentGlueDiameter;
            CurrentCount = jObject["CurrentCount"]?.Value<int>() ?? CurrentCount;
            CurrentXOffset = jObject["CurrentXOffset"]?.Value<decimal>() ?? CurrentXOffset;
            CurrentYOffset = jObject["CurrentYOffset"]?.Value<decimal>() ?? CurrentYOffset;
            MaxGlueDiameter = jObject["MaxGlueDiameter"]?.Value<decimal>() ?? MaxGlueDiameter;
            MinGlueDiameter = jObject["MinGlueDiameter"]?.Value<decimal>() ?? MinGlueDiameter;
            MaxXOffset = jObject["MaxXOffset"]?.Value<decimal>() ?? MaxXOffset;
            MinXOffset = jObject["MinXOffset"]?.Value<decimal>() ?? MinXOffset;
            MaxYOffset = jObject["MaxYOffset"]?.Value<decimal>() ?? MaxYOffset;
            MinYOffset = jObject["MinYOffset"]?.Value<decimal>() ?? MinYOffset;
        }

        /// <summary>
        /// 序列化为 JObject
        /// </summary>
        public override JObject ToJObject()
        {
            return new JObject
            {
                { "CurrentGlueDiameter", CurrentGlueDiameter },
                { "CurrentCount", CurrentCount },
                { "CurrentXOffset", CurrentXOffset },
                { "CurrentYOffset", CurrentYOffset },
                { "MaxGlueDiameter", MaxGlueDiameter },
                { "MinGlueDiameter", MinGlueDiameter },
                { "MaxXOffset", MaxXOffset },
                { "MinXOffset", MinXOffset },
                { "MaxYOffset", MaxYOffset },
                { "MinYOffset", MinYOffset }
            };
        }

        /// <summary>
        /// 复制数据
        /// </summary>
        public void CopyFrom(GlueDiameterCountOffsetCfgInfo source)
        {
            if (source == null) return;

            CurrentGlueDiameter = source.CurrentGlueDiameter;
            CurrentCount = source.CurrentCount;
            CurrentXOffset = source.CurrentXOffset;
            CurrentYOffset = source.CurrentYOffset;
            MaxGlueDiameter = source.MaxGlueDiameter;
            MinGlueDiameter = source.MinGlueDiameter;
            MaxXOffset = source.MaxXOffset;
            MinXOffset = source.MinXOffset;
            MaxYOffset = source.MaxYOffset;
            MinYOffset = source.MinYOffset;
        }
    }
}