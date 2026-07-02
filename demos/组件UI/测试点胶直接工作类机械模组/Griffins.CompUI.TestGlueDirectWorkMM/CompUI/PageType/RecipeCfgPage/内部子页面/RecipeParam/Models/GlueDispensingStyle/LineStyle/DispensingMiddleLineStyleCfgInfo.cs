using Newtonsoft.JsonG.Linq;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.GlueDispensingStyle;
using System;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.GlueDispensingStyle.LineStyle
{
    /// <summary>
    /// 点胶中线样式配置信息
    /// </summary>
    public class DispensingMiddleLineStyleCfgInfo : DispensingStyleCfgBaseInfo
    {

        /// <summary>
        /// 点胶高度（mm）
        /// </summary>
        public decimal DispensingHeight { get; set; }

        /// <summary>
        /// 点胶速度（mm/s）
        /// </summary>
        public decimal DispensingSpeed { get; set; }

        public DispensingMiddleLineStyleCfgInfo()
        {
        }
        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public new void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;
            base.FromJObject(jObject);
            DispensingHeight = jObject["DispensingHeight"]?.Value<decimal>() ?? 0;
            DispensingSpeed = jObject["DispensingSpeed"]?.Value<decimal>() ?? 0;
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public new JObject ToJObject()
        {
            var jObject = new JObject();
            jObject = base.ToJObject();
            jObject["DispensingHeight"] = DispensingHeight;
            jObject["DispensingSpeed"] = DispensingSpeed;

            return jObject;
        }
    }
}