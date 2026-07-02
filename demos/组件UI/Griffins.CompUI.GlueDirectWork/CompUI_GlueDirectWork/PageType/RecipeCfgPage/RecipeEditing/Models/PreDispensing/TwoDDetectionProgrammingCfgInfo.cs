using Newtonsoft.JsonG.Linq;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models
{
    /// <summary>
    /// 2D检测编程配置数据模型
    /// </summary>
    public class TwoDDetectionProgrammingCfgInfo
    {
        /// <summary>
        /// 图像预处理配置
        /// </summary>
        public ImagePreProcessCfgInfo ImagePreProcessCfgInfo { get; set; } = new();

        /// <summary>
        /// 通过条件编辑配置
        /// </summary>
        public PassConditionCfgInfo PassConditionCfgInfo { get; set; } = new();

        /// <summary>
        /// 检测结果配置
        /// </summary>
        public DetectionResultCfgInfo DetectionResultCfgInfo { get; set; } = new();

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            ImagePreProcessCfgInfo.FromJObject(jObject["ImagePreProcessCfgInfo"] as JObject);
            PassConditionCfgInfo.FromJObject(jObject["PassConditionCfgInfo"] as JObject);
            DetectionResultCfgInfo.FromJObject(jObject["DetectionResultCfgInfo"] as JObject);
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public JObject ToJObject()
        {
            return new JObject
            {
                { "ImagePreProcessCfgInfo", ImagePreProcessCfgInfo.ToJObject() },
                { "PassConditionCfgInfo", PassConditionCfgInfo.ToJObject() },
                { "DetectionResultCfgInfo", DetectionResultCfgInfo.ToJObject() }
            };
        }

        /// <summary>
        /// 复制
        /// </summary>
        public void CopyFrom(TwoDDetectionProgrammingCfgInfo other)
        {
            if (other == null) return;

            ImagePreProcessCfgInfo.CopyFrom(other.ImagePreProcessCfgInfo);
            PassConditionCfgInfo.CopyFrom(other.PassConditionCfgInfo);
            DetectionResultCfgInfo.CopyFrom(other.DetectionResultCfgInfo);
        }
    }
}
