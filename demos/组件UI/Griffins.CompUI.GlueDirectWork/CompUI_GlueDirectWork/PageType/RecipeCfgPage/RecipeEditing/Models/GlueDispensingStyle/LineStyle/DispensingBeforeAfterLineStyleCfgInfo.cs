using Newtonsoft.JsonG.Linq;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models
{
    /// <summary>
    /// 点胶前后线样式配置信息
    /// </summary>
    public class DispensingBeforeAfterLineStyleCfgInfo : DispensingStyleCfgBaseInfo
    {
        /// <summary>
        /// 内部扩展线样式工艺参数
        /// </summary>
        public ExtendLineStyleCfgInfo ExtendLineStyleCfgInfo { get; set; }
        /// <summary>
        /// 内部工艺参数
        /// </summary>
        public InternalLineStyleCfgInfo InternalLineStyleCfgInfo { get; set; }

        public DispensingBeforeAfterLineStyleCfgInfo()
        {
            ExtendLineStyleCfgInfo = new ExtendLineStyleCfgInfo();
            InternalLineStyleCfgInfo = new InternalLineStyleCfgInfo();
        }
        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public new void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;
            base.FromJObject(jObject);
            // 扩展工艺参数
            if (jObject["ExtendLineStyleCfgInfo"] is JObject extendObj)
            {
                ExtendLineStyleCfgInfo ??= new ExtendLineStyleCfgInfo();
                ExtendLineStyleCfgInfo.FromJObject(extendObj);
            }

            // 内部工艺参数
            if (jObject["InternalLineStyleCfgInfo"] is JObject internalObj)
            {
                InternalLineStyleCfgInfo ??= new InternalLineStyleCfgInfo();
                InternalLineStyleCfgInfo.FromJObject(internalObj);
            }
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public new JObject ToJObject()
        {
            var jObject = new JObject();
            jObject = base.ToJObject();

            jObject["ExtendLineStyleCfgInfo"] = ExtendLineStyleCfgInfo?.ToJObject() ?? new JObject();
            jObject["InternalLineStyleCfgInfo"] = InternalLineStyleCfgInfo?.ToJObject() ?? new JObject();

            return jObject;
        }
    }
    /// <summary>
    /// 内部扩展线样式工艺参数
    /// </summary>
    public class ExtendLineStyleCfgInfo
    {
        /// <summary>
        /// 是否要点胶
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 提前距离
        /// </summary>
        public decimal AdvanceDistance { get; set; }

        /// <summary>
        /// 提前加工距离
        /// </summary>
        public decimal AdvanceProcessingDistance { get; set; }

        /// <summary>
        /// 提前加工时间
        /// </summary>
        public decimal AdvanceProcessingTime { get; set; }

        /// <summary>
        /// 延后距离
        /// </summary>
        public decimal DelayDistance { get; set; }

        /// <summary>
        /// 延迟停止加工距离
        /// </summary>
        public decimal DelayStopProcessingDistance { get; set; }

        /// <summary>
        /// 回走距离
        /// </summary>
        public decimal BackDistance { get; set; }

        /// <summary>
        /// 回走高度
        /// </summary>
        public decimal BackHeight { get; set; }

        /// <summary>
        /// 回拉速度
        /// </summary>
        public decimal PullBackSpeed { get; set; }

        /// <summary>
        /// 回走速度
        /// </summary>
        public decimal BackSpeed { get; set; }

        /// <summary>
        /// 回拉高度
        /// </summary>
        public decimal PullBackHeight { get; set; }

        /// <summary>
        /// 点胶高度
        /// </summary>
        public decimal DispensingHeight { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExtendLineStyleCfgInfo()
        {
        }
        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            Enable = jObject["Enable"]?.Value<bool>() ?? false;
            AdvanceDistance = jObject["AdvanceDistance"]?.Value<decimal>() ?? 0;
            AdvanceProcessingDistance = jObject["AdvanceProcessingDistance"]?.Value<decimal>() ?? 0;
            AdvanceProcessingTime = jObject["AdvanceProcessingTime"]?.Value<decimal>() ?? 0;
            DelayDistance = jObject["DelayDistance"]?.Value<decimal>() ?? 0;
            DelayStopProcessingDistance = jObject["DelayStopProcessingDistance"]?.Value<decimal>() ?? 0;
            BackDistance = jObject["BackDistance"]?.Value<decimal>() ?? 0;
            BackHeight = jObject["BackHeight"]?.Value<decimal>() ?? 0;
            PullBackSpeed = jObject["PullBackSpeed"]?.Value<decimal>() ?? 0;
            BackSpeed = jObject["BackSpeed"]?.Value<decimal>() ?? 0;
            PullBackHeight = jObject["PullBackHeight"]?.Value<decimal>() ?? 0;
            DispensingHeight = jObject["DispensingHeight"]?.Value<decimal>() ?? 0;
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public JObject ToJObject()
        {
            var jObject = new JObject();

            jObject["Enable"] = Enable;
            jObject["AdvanceDistance"] = AdvanceDistance;
            jObject["AdvanceProcessingDistance"] = AdvanceProcessingDistance;
            jObject["AdvanceProcessingTime"] = AdvanceProcessingTime;
            jObject["DelayDistance"] = DelayDistance;
            jObject["DelayStopProcessingDistance"] = DelayStopProcessingDistance;
            jObject["BackDistance"] = BackDistance;
            jObject["BackHeight"] = BackHeight;
            jObject["PullBackSpeed"] = PullBackSpeed;
            jObject["BackSpeed"] = BackSpeed;
            jObject["PullBackHeight"] = PullBackHeight;
            jObject["DispensingHeight"] = DispensingHeight;

            return jObject;
        }
    }
    /// <summary>
    /// 内部工艺参数
    /// </summary>
    public class InternalLineStyleCfgInfo
    {
        public InternalLineStyleCfgInfo()
        {
        }
        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public JObject ToJObject()
        {
            var jObject = new JObject();

            return jObject;
        }
    }

}
