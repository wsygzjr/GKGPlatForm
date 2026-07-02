using Newtonsoft.JsonG.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.TrajectorySequence
{
    /// <summary>
    /// 轨迹序列配置信息
    /// </summary>
    public class TrajectorySequenceCfgInfo : ProcessingTrajectoryItem
    {

        /// <summary>
        /// 轨迹ID（唯一标识）
        /// </summary>
        public Guid TrackID { get; set; }

        /// <summary>
        /// 是否点胶（true-执行点胶，false-仅移动不点胶）
        /// </summary>
        public bool IsDispensing { get; set; }

        /// <summary>
        /// 选中的样式ID
        /// </summary>
        public Guid SelectedStyleID { get; set; }

        /// <summary>
        /// 重量单位
        /// </summary>
        public WeightUnit WeightUnit { get; set; }

        /// <summary>
        /// 重量值
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        /// <param name="jObject">JSON对象</param>
        public new void FromJObject(JObject? jObject)
        {
            base.FromJObject(jObject);
            if (jObject == null) return;

            if (jObject["SerialNumber"] != null)
                SerialNumber = jObject["SerialNumber"].Value<int>();
            if (jObject["TrackID"] != null)
                TrackID = Guid.Parse(jObject["TrackID"].Value<string>());
            if (jObject["IsDispensing"] != null)
                IsDispensing = jObject["IsDispensing"].Value<bool>();
            if (jObject["SelectedStyleID"] != null)
                SelectedStyleID = Guid.Parse(jObject["SelectedStyleID"].Value<string>());
            if (jObject["WeightUnit"] != null)
                WeightUnit = (WeightUnit)jObject["WeightUnit"].Value<int>();
            if (jObject["Weight"] != null)
                Weight = jObject["Weight"].Value<decimal>();
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        /// <returns>JSON对象</returns>
        public new JObject ToJObject()
        {
            var jObject = base.ToJObject();
            jObject["SerialNumber"] = SerialNumber;
            jObject["TrackID"] = TrackID.ToString();
            jObject["IsDispensing"] = IsDispensing;
            jObject["SelectedStyleID"] = SelectedStyleID.ToString();
            jObject["WeightUnit"] = (int)WeightUnit;
            jObject["Weight"] = Weight;
            return jObject;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public TrajectorySequenceCfgInfo() : base()
        {
        }
    }

    /// <summary>
    /// 轨迹序列配置信息列表
    /// </summary>
    public class TrajectorySequenceCfgInfoList : List<TrajectorySequenceCfgInfo>
    {
        /// <summary>
        /// 从JArray反序列化
        /// </summary>
        /// <param name="jArray">JSON数组</param>
        public void FromJObject(JArray? jArray)
        {
            if (jArray == null) return;

            foreach (var itemObj in jArray)
            {
                if (itemObj is JObject itemJObject)
                {
                    var item = new TrajectorySequenceCfgInfo();
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