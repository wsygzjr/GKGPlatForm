using Newtonsoft.JsonG.Linq;
using System;
using System.Collections.Generic;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.GlueDispensingStyle.PointStyle
{
    /// <summary>
    /// 点胶后样式配置信息
    /// </summary>
    public class DispensingAfterPointStyleCfgInfo: DispensingStyleCfgBaseInfo
    {
        /// <summary>
        /// 稳定时间（ms，建议取值范围 0-1000）
        /// </summary>
        public int StabilizationTime { get; set; }

        /// <summary>
        /// 回抬高度（mm，建议取值范围 0.1-100.0）
        /// </summary>
        public decimal LiftHeight { get; set; }

        /// <summary>
        /// 回抬速度（mm/s，建议取值范围 1-500）
        /// </summary>
        public decimal LiftSpeed { get; set; }

        public DispensingAfterPointStyleCfgInfo()
        {
            StyleID = Guid.NewGuid();
        }
        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public new  void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            base.FromJObject(jObject);
            StabilizationTime = jObject["StabilizationTime"]?.Value<int>() ?? 0;
            LiftHeight = jObject["LiftHeight"]?.Value<decimal>() ?? 0;
            LiftSpeed = jObject["LiftSpeed"]?.Value<decimal>() ?? 0;
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public new JObject ToJObject()
        {
            var jObject = new JObject();
            jObject=base.ToJObject();
           
            jObject["StabilizationTime"] = StabilizationTime;
            jObject["LiftHeight"] = LiftHeight;
            jObject["LiftSpeed"] = LiftSpeed;

            return jObject;
        }
    }
}