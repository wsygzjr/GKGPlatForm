using Newtonsoft.JsonG.Linq;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.SubTemplate
{
    /// <summary>
    /// 子模板配置参数
    /// </summary>
    public class SubTemplateConfigInfo
    {
        /// <summary>
        /// 子模板点信息列表
        /// </summary>
        public SubTemplatePointInfoList SubTemplatePointInfoes { get; set; }

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        /// <param name="jObject">JSON对象</param>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            if (jObject["SubTemplatePointInfoes"] is JArray pointArray)
            {
                SubTemplatePointInfoes ??= new SubTemplatePointInfoList();
                SubTemplatePointInfoes.FromJObject(pointArray);
            }
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        /// <returns>JSON对象</returns>
        public JObject ToJObject()
        {
            var jObject = new JObject();
            jObject["SubTemplatePointInfoes"] = SubTemplatePointInfoes?.ToJArray() ?? new JArray();
            return jObject;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SubTemplateConfigInfo()
        {
            SubTemplatePointInfoes = new SubTemplatePointInfoList();
        }
    }
}