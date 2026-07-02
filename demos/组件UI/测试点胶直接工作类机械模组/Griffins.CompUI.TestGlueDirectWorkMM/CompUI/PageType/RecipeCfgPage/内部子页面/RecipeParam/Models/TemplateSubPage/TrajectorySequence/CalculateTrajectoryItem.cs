using Newtonsoft.JsonG.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.TrajectorySequence
{
    /// <summary>
    /// 计算轨迹项
    /// </summary>
    public class CalculateTrajectoryItem : EntityBase
    {
        /// <summary>
        /// 线计算轨迹类型
        /// </summary>
        public LineCalculateTrajectoryType LineCalculateTrajectoryType { get; set; }

        /// <summary>
        /// 线计算轨迹（根据轨迹类型动态实例化）
        /// </summary>
        public LineCalculateTrajectoryItemBase CalculateTrajectory { get; set; }

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        /// <param name="jObject">JSON对象</param>
        public new void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            if (jObject["LineCalculateTrajectoryType"] != null)
                LineCalculateTrajectoryType = (LineCalculateTrajectoryType)jObject["LineCalculateTrajectoryType"].Value<int>();

            if (jObject["CalculateTrajectory"] is JObject trajectoryObj)
            {
                CalculateTrajectory ??= LineCalculateTrajectoryType switch
                {
                    LineCalculateTrajectoryType.StraightLine => new StraightLineACalculateTrajectory(),
                    LineCalculateTrajectoryType.CircularArcA => new CircularArcACalculateTrajectory(),
                    LineCalculateTrajectoryType.CircularArcB => new CircularArcBCalculateTrajectory(),
                    LineCalculateTrajectoryType.Circle => new CircleCalculateTrajectory(),
                    _ => new LineCalculateTrajectoryItemBase()
                };
                CalculateTrajectory.FromJObject(trajectoryObj);
            }
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        /// <returns>JSON对象</returns>
        public new JObject ToJObject()
        {
            var jObject = new JObject();
            jObject["LineCalculateTrajectoryType"] = (int)LineCalculateTrajectoryType;
            jObject["CalculateTrajectory"] = CalculateTrajectory?.ToJObject() ?? new JObject();
            return jObject;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CalculateTrajectoryItem()
        {
            CalculateTrajectory = new LineCalculateTrajectoryItemBase();
        }
    }

    /// <summary>
    /// 线计算轨迹类型枚举
    /// </summary>
    public enum LineCalculateTrajectoryType
    {
        /// <summary>
        /// 直线
        /// </summary>
        StraightLine = 0,
        /// <summary>
        /// 圆弧A（通过终点+中间点定义）
        /// </summary>
        CircularArcA = 1,
        /// <summary>
        /// 圆弧B（通过终点+圆心定义）
        /// </summary>
        CircularArcB = 2,
        /// <summary>
        /// 圆（闭合轨迹）
        /// </summary>
        Circle = 3,
    }
}