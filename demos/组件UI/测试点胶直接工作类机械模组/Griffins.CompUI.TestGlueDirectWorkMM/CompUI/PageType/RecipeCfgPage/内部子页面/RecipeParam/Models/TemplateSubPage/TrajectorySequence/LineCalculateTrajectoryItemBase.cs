using Newtonsoft.JsonG.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.TrajectorySequence
{
    /// <summary>
    /// 线计算轨迹项基类
    /// </summary>
    public class LineCalculateTrajectoryItemBase
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int SerialNumber { set; get; }

        /// <summary>
        /// 线轨迹ID（唯一标识）
        /// </summary>
        public Guid TrackID { set; get; }

        /// <summary>
        /// 线加工中参数配置
        /// </summary>
        public LineProcessingCfgInfo LineProcessingCfgInfo { get; set; }

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        /// <param name="jObject">JSON对象</param>
        public virtual void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            if (jObject["SerialNumber"] != null)
                SerialNumber = jObject["SerialNumber"].Value<int>();
            if (jObject["TrackID"] != null)
                TrackID = Guid.Parse(jObject["TrackID"].Value<string>());
            if (jObject["LineProcessingCfgInfo"] is JObject cfgObj)
            {
                LineProcessingCfgInfo ??= new LineProcessingCfgInfo();
                LineProcessingCfgInfo.FromJObject(cfgObj);
            }
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        /// <returns>JSON对象</returns>
        public virtual JObject ToJObject()
        {
            var jObject = new JObject();
            jObject["SerialNumber"] = SerialNumber;
            jObject["TrackID"] = TrackID.ToString();
            jObject["LineProcessingCfgInfo"] = LineProcessingCfgInfo?.ToJObject() ?? new JObject();
            return jObject;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public LineCalculateTrajectoryItemBase()
        {
            LineProcessingCfgInfo = new LineProcessingCfgInfo();
        }
    }

    /// <summary>
    /// 直线A计算轨迹
    /// </summary>
    public class StraightLineACalculateTrajectory : LineCalculateTrajectoryItemBase
    {
        /// <summary>
        /// 终点坐标
        /// </summary>
        public PointInfo EndpointPoint { get; set; }

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        /// <param name="jObject">JSON对象</param>
        public override void FromJObject(JObject? jObject)
        {
            base.FromJObject(jObject);
            if (jObject == null) return;

            if (jObject["EndpointPoint"] is JObject endPointObj)
            {
                EndpointPoint ??= new PointInfo();
                EndpointPoint.FromJObject(endPointObj);
            }
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        /// <returns>JSON对象</returns>
        public override JObject ToJObject()
        {
            var jObject = base.ToJObject();
            jObject["EndpointPoint"] = EndpointPoint?.ToJObject() ?? new JObject();
            return jObject;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public StraightLineACalculateTrajectory()
        {
            EndpointPoint = new PointInfo();
            EndpointPoint.SerialNumber = 1;
        }
    }

    /// <summary>
    /// 圆弧A计算轨迹
    /// </summary>
    public class CircularArcACalculateTrajectory : LineCalculateTrajectoryItemBase
    {
        /// <summary>
        /// 终点坐标
        /// </summary>
        public PointInfo EndpointPoint { get; set; }

        /// <summary>
        /// 中间点坐标
        /// </summary>
        public PointInfo MiddlePoint { get; set; }

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        /// <param name="jObject">JSON对象</param>
        public override void FromJObject(JObject? jObject)
        {
            base.FromJObject(jObject);
            if (jObject == null) return;

            if (jObject["EndpointPoint"] is JObject endPointObj)
            {
                EndpointPoint ??= new PointInfo();
                EndpointPoint.FromJObject(endPointObj);
            }
            if (jObject["MiddlePoint"] is JObject middlePointObj)
            {
                MiddlePoint ??= new PointInfo();
                MiddlePoint.FromJObject(middlePointObj);
            }
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        /// <returns>JSON对象</returns>
        public override JObject ToJObject()
        {
            var jObject = base.ToJObject();
            jObject["EndpointPoint"] = EndpointPoint?.ToJObject() ?? new JObject();
            jObject["MiddlePoint"] = MiddlePoint?.ToJObject() ?? new JObject();
            return jObject;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CircularArcACalculateTrajectory()
        {
            EndpointPoint = new PointInfo();
            MiddlePoint = new PointInfo();
            EndpointPoint.SerialNumber = 1;
            MiddlePoint.SerialNumber = 2;
        }
    }

    /// <summary>
    /// 圆弧B计算轨迹
    /// </summary>
    public class CircularArcBCalculateTrajectory : LineCalculateTrajectoryItemBase
    {
        /// <summary>
        /// 终点坐标
        /// </summary>
        public PointInfo EndpointPoint { get; set; }

        /// <summary>
        /// 圆心点坐标
        /// </summary>
        public PointInfo CenterCirclePoint { get; set; }

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        /// <param name="jObject">JSON对象</param>
        public override void FromJObject(JObject? jObject)
        {
            base.FromJObject(jObject);
            if (jObject == null) return;

            if (jObject["EndpointPoint"] is JObject endPointObj)
            {
                EndpointPoint ??= new PointInfo();
                EndpointPoint.FromJObject(endPointObj);
            }
            if (jObject["CenterCirclePoint"] is JObject centerPointObj)
            {
                CenterCirclePoint ??= new PointInfo();
                CenterCirclePoint.FromJObject(centerPointObj);
            }
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        /// <returns>JSON对象</returns>
        public override JObject ToJObject()
        {
            var jObject = base.ToJObject();
            jObject["EndpointPoint"] = EndpointPoint?.ToJObject() ?? new JObject();
            jObject["CenterCirclePoint"] = CenterCirclePoint?.ToJObject() ?? new JObject();
            return jObject;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CircularArcBCalculateTrajectory()
        {
            EndpointPoint = new PointInfo();
            CenterCirclePoint = new PointInfo();
            EndpointPoint.SerialNumber = 1;
            CenterCirclePoint.SerialNumber = 2;
        }
    }

    /// <summary>
    /// 圆计算轨迹
    /// </summary>
    public class CircleCalculateTrajectory : LineCalculateTrajectoryItemBase
    {
        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        /// <param name="jObject">JSON对象</param>
        public override void FromJObject(JObject? jObject)
        {
            base.FromJObject(jObject);
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        /// <returns>JSON对象</returns>
        public override JObject ToJObject()
        {
            return base.ToJObject();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CircleCalculateTrajectory()
        {
        }
    }

    /// <summary>
    /// 线加工中参数配置
    /// </summary>
    public class LineProcessingCfgInfo
    {
        /// <summary>
        /// 是否启用加工中参数
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 点胶中样式ID（关联样式配置）
        /// </summary>
        public Guid DispensingMiddleStyleID { get; set; }

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
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            if (jObject["IsEnabled"] != null)
                IsEnabled = jObject["IsEnabled"].Value<bool>();
            if (jObject["DispensingMiddleStyleID"] != null)
                DispensingMiddleStyleID = Guid.Parse(jObject["DispensingMiddleStyleID"].Value<string>());
            if (jObject["WeightUnit"] != null)
                WeightUnit = (WeightUnit)jObject["WeightUnit"].Value<int>();
            if (jObject["Weight"] != null)
                Weight = jObject["Weight"].Value<decimal>();
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        /// <returns>JSON对象</returns>
        public JObject ToJObject()
        {
            var jObject = new JObject();
            jObject["IsEnabled"] = IsEnabled;
            jObject["DispensingMiddleStyleID"] = DispensingMiddleStyleID.ToString();
            jObject["WeightUnit"] = (int)WeightUnit;
            jObject["Weight"] = Weight;
            return jObject;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public LineProcessingCfgInfo()
        {
        }
    }

   
}