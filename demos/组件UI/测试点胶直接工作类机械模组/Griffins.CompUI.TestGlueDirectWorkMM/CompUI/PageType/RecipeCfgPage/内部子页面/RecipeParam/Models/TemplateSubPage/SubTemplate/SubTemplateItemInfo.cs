using Griffins.UI.General;
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.JsonG.Linq;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.TrajectorySequence;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.SubTemplate
{
    /// <summary>
    /// 子模板项信息
    /// </summary>
    public class SubTemplateItemInfo : EntityBase
    {
        /// <summary>
        /// 子模板ID（唯一标识）
        /// </summary>
        public Guid SubTemplateID { set; get; } = Guid.NewGuid();

        /// <summary>
        /// 子模板名称
        /// </summary>
        public string SubTemplateName { set; get; } = "";

        /// <summary>
        /// 所属模板ID（关联父模板）
        /// </summary>
        public Guid BelongTemplateID { set; get; } = Guid.Empty;

        /// <summary>
        /// 坐标轴值列表
        /// </summary>
        public CoordinateAxisValueList CoordinateAxisValues { set; get; } = new CoordinateAxisValueList();


        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        /// <param name="jObject">JSON对象</param>
        public new void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            SerialNumber = jObject["SerialNumber"]?.Value<int>() ?? 0;
            SubTemplateID = Guid.TryParse(jObject["SubTemplateID"]?.Value<string>(), out var subTplGuid) ? subTplGuid : Guid.NewGuid();
            SubTemplateName = jObject["SubTemplateName"]?.Value<string>() ?? "";
            BelongTemplateID = Guid.TryParse(jObject["BelongTemplateID"]?.Value<string>(), out var belongGuid) ? belongGuid : Guid.Empty;

            if (jObject["CoordinateAxisValues"] is JArray axisArray)
            {
                CoordinateAxisValues ??= new CoordinateAxisValueList();
                CoordinateAxisValues.FromJObject(axisArray);
            }
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        /// <returns>JSON对象</returns>
        public new JObject ToJObject()
        {
            var jObject = new JObject();
            jObject["SerialNumber"] = SerialNumber;
            jObject["SubTemplateID"] = SubTemplateID.ToString();
            jObject["SubTemplateName"] = SubTemplateName;
            jObject["BelongTemplateID"] = BelongTemplateID.ToString();
            jObject["CoordinateAxisValues"] = CoordinateAxisValues.ToJArray();
            return jObject;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SubTemplateItemInfo()
        {
            SubTemplateID = Guid.NewGuid();
            CoordinateAxisValues = new CoordinateAxisValueList();
        }
    }

    /// <summary>
    /// 子模板项信息列表
    /// </summary>
    public class SubTemplatePointInfoList : List<SubTemplateItemInfo>
    {
        /// <summary>
        /// 从JArray反序列化
        /// </summary>
        /// <param name="jArray">JSON数组</param>
        public void FromJObject(JArray? jArray)
        {
            if (jArray == null) return;

            Clear();
            foreach (var itemObj in jArray)
            {
                if (itemObj is JObject itemJObject)
                {
                    var item = new SubTemplateItemInfo();
                    item.FromJObject(itemJObject);
                    Add(item);
                }
            }
        }

        /// <summary>
        /// 序列化为JArray
        /// </summary>
        /// <returns>JSON数组</returns>
        public JArray ToJArray()
        {
            var jArray = new JArray();
            foreach (var item in this)
            {
                jArray.Add(item.ToJObject());
            }
            return jArray;
        }
    }
}