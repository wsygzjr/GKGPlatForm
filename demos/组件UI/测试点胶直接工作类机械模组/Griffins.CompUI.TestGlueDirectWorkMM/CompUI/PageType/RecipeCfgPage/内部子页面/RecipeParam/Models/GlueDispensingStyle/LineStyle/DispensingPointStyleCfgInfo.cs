using Griffins.UI.General;
using Newtonsoft.JsonG.Linq;
using System.Diagnostics.Metrics;
using System;
using System.Collections.Generic;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.GlueDispensingStyle.LineStyle
{
    /// <summary>
    /// 点胶线样式配置信息
    /// </summary>
    public class DispensingLineStyleCfgInfo
    {
        /// <summary>
        /// 点胶前后线样式配置信息
        /// </summary>
        public List<DispensingBeforeAfterLineStyleCfgInfo> DispensingBeforeAfterLineStyleCfgInfoes { get; set; }
        /// <summary>
        /// 点胶中线样式配置信息
        /// </summary>
        public List<DispensingMiddleLineStyleCfgInfo> DispensingMiddleLineStyleCfgInfoes { get; set; }
      
        public DispensingLineStyleCfgInfo()
        {
            DispensingBeforeAfterLineStyleCfgInfoes = new List<DispensingBeforeAfterLineStyleCfgInfo>();
            DispensingMiddleLineStyleCfgInfoes = new List<DispensingMiddleLineStyleCfgInfo>();
        }
        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            // 点胶前后线样式列表
            if (jObject["DispensingBeforeAfterLineStyleCfgInfoes"] is JArray beforeAfterArray)
            {
                DispensingBeforeAfterLineStyleCfgInfoes ??= new List<DispensingBeforeAfterLineStyleCfgInfo>();
                DispensingBeforeAfterLineStyleCfgInfoes.Clear();
                foreach (var itemObj in beforeAfterArray)
                {
                    if (itemObj is JObject itemJObj)
                    {
                        var item = new DispensingBeforeAfterLineStyleCfgInfo();
                        item.FromJObject(itemJObj);
                        DispensingBeforeAfterLineStyleCfgInfoes.Add(item);
                    }
                }
            }

            // 点胶中线样式列表
            if (jObject["DispensingMiddleLineStyleCfgInfoes"] is JArray middleArray)
            {
                DispensingMiddleLineStyleCfgInfoes ??= new List<DispensingMiddleLineStyleCfgInfo>();
                DispensingMiddleLineStyleCfgInfoes.Clear();
                foreach (var itemObj in middleArray)
                {
                    if (itemObj is JObject itemJObj)
                    {
                        var item = new DispensingMiddleLineStyleCfgInfo();
                        item.FromJObject(itemJObj);
                        DispensingMiddleLineStyleCfgInfoes.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public JObject ToJObject()
        {
            var jObject = new JObject();

            // 点胶前后线样式列表
            var beforeAfterArray = new JArray();
            foreach (var item in DispensingBeforeAfterLineStyleCfgInfoes ?? new List<DispensingBeforeAfterLineStyleCfgInfo>())
            {
                beforeAfterArray.Add(item.ToJObject());
            }
            jObject["DispensingBeforeAfterLineStyleCfgInfoes"] = beforeAfterArray;

            // 点胶中线样式列表
            var middleArray = new JArray();
            foreach (var item in DispensingMiddleLineStyleCfgInfoes ?? new List<DispensingMiddleLineStyleCfgInfo>())
            {
                middleArray.Add(item.ToJObject());
            }
            jObject["DispensingMiddleLineStyleCfgInfoes"] = middleArray;

            return jObject;
        }
    }
}
