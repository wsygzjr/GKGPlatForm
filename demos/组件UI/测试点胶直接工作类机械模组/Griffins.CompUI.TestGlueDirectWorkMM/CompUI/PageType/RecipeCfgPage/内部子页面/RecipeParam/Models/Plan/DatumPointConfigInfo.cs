
using Griffins.CompUI.TestGlueDirectWorkMM.CompUI.PageType;
using Newtonsoft.JsonG.Linq;
using System;
using System.Collections.Generic;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Plan
{
    /// <summary>
    /// 基准点参数配置信息
    /// </summary>
    public class DatumPointConfigInfo
    {
        /// <summary>
        /// 配方整板偏移量
        /// </summary>
        public List<Point2D> Points { get; set; }

        public DatumPointConfigInfo()
        {
            Points = new List<Point2D>();
        }

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            if (jObject["Points"] is JArray pointsArray)
            {
                Points.Clear();
                foreach (var itemObj in pointsArray)
                {
                    if (itemObj is JObject itemJObj)
                    {
                        var point = new Point2D();
                        //待在Point2D中添加该方法
                        //point.FromJObject(itemJObj);
                        Points.Add(point);
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
            var pointsArray = new JArray();
            foreach (var point in Points)
            {
                ////pointsArray.Add(point.ToJObject());
            }
            jObject["Points"] = pointsArray;
            return jObject;
        }
    }
}
