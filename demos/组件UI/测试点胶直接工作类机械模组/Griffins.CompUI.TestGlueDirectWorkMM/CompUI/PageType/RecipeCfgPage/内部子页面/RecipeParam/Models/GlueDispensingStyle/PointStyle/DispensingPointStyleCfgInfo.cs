using DynamicData;
using Griffins.UI.General;
using Newtonsoft.JsonG.Linq;
using System;
using System.Collections.Generic;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.GlueDispensingStyle.PointStyle
{
    /// <summary>
    /// 点胶点样式配置信息
    /// </summary>
    public class DispensingPointStyleCfgInfo
    {
        /// <summary>
        /// 点胶前点样式配置信息
        /// </summary>
        public List<DispensingBeforePointStyleCfgInfo> DispensingBeforePointStyleCfgInfoes { get; set; }
        /// <summary>
        /// 点胶后点样式配置信息
        /// </summary>
        public List<DispensingAfterPointStyleCfgInfo> DispensingAfterPointStyleCfgInfoes { get; set; }
        public DispensingPointStyleCfgInfo()
        {
            DispensingBeforePointStyleCfgInfoes = new List<DispensingBeforePointStyleCfgInfo>();
            DispensingAfterPointStyleCfgInfoes = new List<DispensingAfterPointStyleCfgInfo>();
        }
        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            // 点胶前点样式列表
            if (jObject["DispensingBeforePointStyleCfgInfoes"] is JArray beforePointArray)
            {
                DispensingBeforePointStyleCfgInfoes ??= new List<DispensingBeforePointStyleCfgInfo>(); 
                DispensingBeforePointStyleCfgInfoes.Clear();
                foreach (var itemObj in beforePointArray)
                {
                    if (itemObj is JObject itemJObj)
                    {
                        var item = new DispensingBeforePointStyleCfgInfo();
                        item.FromJObject(itemJObj);
                        DispensingBeforePointStyleCfgInfoes.Add(item);
                    }
                }
            }

            // 点胶后点样式列表
            if (jObject["DispensingAfterPointStyleCfgInfoes"] is JArray afterPointArray)
            {
                DispensingAfterPointStyleCfgInfoes ??= new List<DispensingAfterPointStyleCfgInfo>();
                DispensingAfterPointStyleCfgInfoes.Clear();
                foreach (var itemObj in afterPointArray)
                {
                    if (itemObj is JObject itemJObj)
                    {
                        var item = new DispensingAfterPointStyleCfgInfo();
                        item.FromJObject(itemJObj);
                        DispensingAfterPointStyleCfgInfoes.Add(item);
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

            // 点胶前点样式列表
            var beforePointArray = new JArray();
            foreach (var item in DispensingBeforePointStyleCfgInfoes ?? new List<DispensingBeforePointStyleCfgInfo>())
            {
                beforePointArray.Add(item.ToJObject());
            }
            jObject["DispensingBeforePointStyleCfgInfoes"] = beforePointArray;

            // 点胶后点样式列表
            var afterPointArray = new JArray();
            foreach (var item in DispensingAfterPointStyleCfgInfoes ?? new List<DispensingAfterPointStyleCfgInfo>())
            {
                afterPointArray.Add(item.ToJObject());
            }
            jObject["DispensingAfterPointStyleCfgInfoes"] = afterPointArray;

            return jObject;
        }
    }
}
