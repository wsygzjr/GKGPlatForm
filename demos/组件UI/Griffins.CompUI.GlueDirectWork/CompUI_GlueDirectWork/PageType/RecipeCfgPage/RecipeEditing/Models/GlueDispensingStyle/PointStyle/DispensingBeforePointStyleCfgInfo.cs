using Newtonsoft.JsonG.Linq;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models
{
    /// <summary>
    /// 点胶前点样式配置信息
    /// </summary>
    public class DispensingBeforePointStyleCfgInfo : DispensingStyleCfgBaseInfo
    {
        /// <summary>
        /// 旋转角度（°，建议取值范围 0-360）
        /// </summary>
        public decimal RotationAngle { get; set; }

        /// <summary>
        /// 倾斜角度（°，建议取值范围 -90 至 90）
        /// </summary>
        public decimal TiltAngle { get; set; }

        /// <summary>
        /// 点胶高度（mm，建议取值范围 0.1-50.0）
        /// </summary>
        public decimal DispensingHeight { get; set; }

        /// <summary>
        /// 稳定时间（ms，建议取值范围 0-1000）
        /// </summary>
        public int StabilizationTime { get; set; }

        /// <summary>
        /// 提前开阀时间（ms，建议取值范围 0-500）
        /// </summary>
        public int AdvanceValveOpeningTime { get; set; }

        public DispensingBeforePointStyleCfgInfo()
        {
        }

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        /// <param name="jObject">JSON对象</param>
        public new void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            base.FromJObject(jObject);

            // 反序列化当前类业务字段
            RotationAngle = jObject["RotationAngle"]?.Value<decimal>() ?? 0;
            TiltAngle = jObject["TiltAngle"]?.Value<decimal>() ?? 0;
            DispensingHeight = jObject["DispensingHeight"]?.Value<decimal>() ?? 0;
            StabilizationTime = jObject["StabilizationTime"]?.Value<int>() ?? 0;
            AdvanceValveOpeningTime = jObject["AdvanceValveOpeningTime"]?.Value<int>() ?? 0;
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        /// <returns>JSON对象</returns>
        public new JObject ToJObject()
        {
            var jObject = new JObject();
            jObject = base.ToJObject();

            // 序列化当前类业务字段
            jObject["RotationAngle"] = RotationAngle;
            jObject["TiltAngle"] = TiltAngle;
            jObject["DispensingHeight"] = DispensingHeight;
            jObject["StabilizationTime"] = StabilizationTime;
            jObject["AdvanceValveOpeningTime"] = AdvanceValveOpeningTime;

            return jObject;
        }
    }

}
