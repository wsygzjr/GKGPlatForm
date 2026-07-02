using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Tools;
using Newtonsoft.JsonG.Linq;
using System;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models
{
    /// <summary>
    /// 点胶样式配置信息
    /// </summary>
    public class DispensingStyleCfgInfo
    {
        /// <summary>
        /// 点胶点样式配置信息
        /// </summary>
        public DispensingPointStyleCfgInfo DispensingPointStyleCfgInfo { get; set; }
        /// <summary>
        /// 点胶线样式配置信息
        /// </summary>
        public DispensingLineStyleCfgInfo DispensingLineStyleCfgInfo { get; set; }
        public DispensingStyleCfgInfo()
        {
            DispensingPointStyleCfgInfo = new DispensingPointStyleCfgInfo();
            DispensingLineStyleCfgInfo = new DispensingLineStyleCfgInfo();
        }
        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            // 点胶点样式配置
            if (jObject["DispensingPointStyleCfgInfo"] is JObject pointStyleObj)
            {
                DispensingPointStyleCfgInfo ??= new DispensingPointStyleCfgInfo();
                DispensingPointStyleCfgInfo.FromJObject(pointStyleObj);
            }

            // 点胶线样式配置
            if (jObject["DispensingLineStyleCfgInfo"] is JObject lineStyleObj)
            {
                DispensingLineStyleCfgInfo ??= new DispensingLineStyleCfgInfo();
                DispensingLineStyleCfgInfo.FromJObject(lineStyleObj);
            }
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public JObject ToJObject()
        {
            var jObject = new JObject();

            jObject["DispensingPointStyleCfgInfo"] = DispensingPointStyleCfgInfo?.ToJObject() ?? new JObject();
            jObject["DispensingLineStyleCfgInfo"] = DispensingLineStyleCfgInfo?.ToJObject() ?? new JObject();

            return jObject;
        }
    }

    /// <summary>
    /// 点胶样式配置基础信息
    /// </summary>
    public class DispensingStyleCfgBaseInfo : EntityBase
    {
        /// <summary>
        /// 样式ID
        /// </summary>
        public Guid StyleID { get; set; }

        /// <summary>
        /// 样式名称
        /// </summary>
        public string StyleName { get; set; } = string.Empty;

        public DispensingStyleCfgBaseInfo()
        {
            StyleID = Guid.NewGuid();
        }
        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public new void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            base.FromJObject(jObject);
            StyleID = Guid.TryParse(jObject["StyleID"]?.Value<string>(), out var guid) ? guid : Guid.NewGuid();
            StyleName = jObject["StyleName"]?.Value<string>() ?? string.Empty;
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public new JObject ToJObject()
        {
            var jObject = new JObject();
            jObject = base.ToJObject();
            jObject["StyleID"] = StyleID.ToString();
            jObject["StyleName"] = StyleName;
            return jObject;
        }
    }
}
