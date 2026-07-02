using Newtonsoft.JsonG.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Command;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Mark;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.SubTemplate;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.TrajectorySequence;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage
{
    /// <summary>
    /// 模板配置信息（包含轨迹、Mark、子模板、指令序列等完整配置）
    /// </summary>
    public class TemplateCfgInfo
    {
        /// <summary>
        /// 模板ID（唯一标识）
        /// </summary>
        public Guid TemplateID { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 模板名称
        /// </summary>
        public string TemplateName { get; set; } = "";

        /// <summary>
        /// 轨迹序列配置信息列表（加工轨迹集合）
        /// </summary>
        public TrajectorySequenceCfgInfoList TrajectorySequenceCfgInfoes { get; set; } = new TrajectorySequenceCfgInfoList();

        /// <summary>
        /// Mark配置信息（视觉定位相关配置）
        /// </summary>
        public MarkConfigInfo MarkConfigInfo { get; set; } = new MarkConfigInfo();

        /// <summary>
        /// 子模板配置参数（子模板集合及偏移配置）
        /// </summary>
        public SubTemplateConfigInfo SubTemplateConfigInfo { get; set; } = new SubTemplateConfigInfo();

        /// <summary>
        /// 指令序列配置信息列表（设备控制指令集合）
        /// </summary>
        public CommandSequenceCfgInfoList CommandSequenceCfgInfoes { get; set; } = new CommandSequenceCfgInfoList();

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        /// <param name="jObject">JSON对象</param>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            TemplateID = Guid.TryParse(jObject["TemplateID"]?.Value<string>(), out var tplGuid) ? tplGuid : Guid.NewGuid();
            TemplateName = jObject["TemplateName"]?.Value<string>() ?? "";

            // 轨迹序列反序列化
            if (jObject["TrajectorySequenceCfgInfoes"] is JArray trajectoryArray)
            {
                TrajectorySequenceCfgInfoes ??= new TrajectorySequenceCfgInfoList();
                TrajectorySequenceCfgInfoes.FromJObject(trajectoryArray);
            }

            // Mark配置反序列化
            if (jObject["MarkConfigInfo"] is JObject markObj)
            {
                MarkConfigInfo ??= new MarkConfigInfo();
                MarkConfigInfo.FromJObject(markObj);
            }

            // 子模板配置反序列化
            if (jObject["SubTemplateConfigInfo"] is JObject subTplObj)
            {
                SubTemplateConfigInfo ??= new SubTemplateConfigInfo();
                SubTemplateConfigInfo.FromJObject(subTplObj);
            }

            // 指令序列反序列化
            if (jObject["CommandSequenceCfgInfoes"] is JArray commandArray)
            {
                CommandSequenceCfgInfoes ??= new CommandSequenceCfgInfoList();
                CommandSequenceCfgInfoes.FromJObject(commandArray);
            }
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        /// <returns>JSON对象</returns>
        public JObject ToJObject()
        {
            var jObject = new JObject();
            jObject["TemplateID"] = TemplateID.ToString();
            jObject["TemplateName"] = TemplateName;
            jObject["TrajectorySequenceCfgInfoes"] = TrajectorySequenceCfgInfoes?.ToJArray() ?? new JArray();
            jObject["MarkConfigInfo"] = MarkConfigInfo?.ToJObject() ?? new JObject();
            jObject["SubTemplateConfigInfo"] = SubTemplateConfigInfo?.ToJObject() ?? new JObject();
            jObject["CommandSequenceCfgInfoes"] = CommandSequenceCfgInfoes?.ToJArray() ?? new JArray();
            return jObject;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public TemplateCfgInfo()
        {
            TemplateID = Guid.NewGuid();
            TrajectorySequenceCfgInfoes = new TrajectorySequenceCfgInfoList();
            MarkConfigInfo = new MarkConfigInfo();
            SubTemplateConfigInfo = new SubTemplateConfigInfo();
            CommandSequenceCfgInfoes = new CommandSequenceCfgInfoList();
        }
    }

    /// <summary>
    /// 模板配置信息列表
    /// </summary>
    public class TemplateCfgInfoList : List<TemplateCfgInfo>
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
                    var item = new TemplateCfgInfo();
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