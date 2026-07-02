using Newtonsoft.JsonG.Linq;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.TrajectorySequence
{
    /// <summary>
    /// 线计算轨迹基类
    /// </summary>
    public class CalculateTrajectoryBase
    {
        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public virtual void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public virtual JObject ToJObject()
        {
            return new JObject();
        }

        public CalculateTrajectoryBase()
        {
        }
    }

    /// <summary>
    /// 点计算轨迹
    /// </summary>
    public class PointCalculateTrajectory : CalculateTrajectoryBase
    {
        /// <summary>
        /// 目标点坐标
        /// </summary>
        public PointInfo TargetPoint { get; set; }

        public override void FromJObject(JObject? jObject)
        {
            base.FromJObject(jObject);
            if (jObject == null) return;

            if (jObject["TargetPoint"] is JObject targetPointObj)
            {
                TargetPoint ??= new PointInfo();
                TargetPoint.FromJObject(targetPointObj);
            }
        }

        public override JObject ToJObject()
        {
            var jObject = base.ToJObject();
            // 嵌套对象序列化
            jObject["TargetPoint"] = TargetPoint?.ToJObject() ?? new JObject();
            return jObject;
        }

        public PointCalculateTrajectory()
        {
            TargetPoint = new PointInfo();
        }
    }

    /// <summary>
    /// 线计算轨迹
    /// </summary>
    public class LineCalculateTrajectory : CalculateTrajectoryBase
    {
        /// <summary>
        /// 起点点坐标
        /// </summary>
        public PointInfo StartPoint { get; set; }

        /// <summary>
        /// 计算轨迹项列表
        /// 如果计算轨迹项列表为空，则表示是点位运动
        /// </summary>
        public List<CalculateTrajectoryItem>? CalculateTrajectoryItem { get; set; }

        public override void FromJObject(JObject? jObject)
        {
            base.FromJObject(jObject);
            if (jObject == null) return;

            if (jObject["StartPoint"] is JObject startPointObj)
            {
                StartPoint ??= new PointInfo();
                StartPoint.FromJObject(startPointObj);
            }

            if (jObject["CalculateTrajectoryItem"] is JArray itemsArray)
            {
                CalculateTrajectoryItem ??= new List<CalculateTrajectoryItem>();
                foreach (var itemObj in itemsArray)
                {
                    if (itemObj is JObject itemJObject)
                    {
                        var item = new CalculateTrajectoryItem();
                        item.FromJObject(itemJObject);
                        CalculateTrajectoryItem.Add(item);
                    }
                }
            }
        }

        public override JObject ToJObject()
        {
            var jObject = base.ToJObject();
            jObject["StartPoint"] = StartPoint?.ToJObject() ?? new JObject();

            if (CalculateTrajectoryItem != null && CalculateTrajectoryItem.Count > 0)
            {
                var itemsArray = new JArray();
                foreach (var item in CalculateTrajectoryItem)
                {
                    itemsArray.Add(item?.ToJObject() ?? new JObject());
                }
                jObject["CalculateTrajectoryItem"] = itemsArray;
            }
            else
            {
                jObject["CalculateTrajectoryItem"] = new JArray(); 
            }

            return jObject;
        }

        public LineCalculateTrajectory()
        {
            StartPoint = new PointInfo();
        }
    }

    /// <summary>
    /// 计算轨迹类型
    /// </summary>
    public enum CalculateTrajectoryType
    {
        /// <summary>
        /// 点
        /// </summary>
        Point = 0,

        /// <summary>
        /// 线
        /// </summary>
        Line = 1,
    }
}