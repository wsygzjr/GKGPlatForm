using Griffins.UI;
using Newtonsoft.JsonG.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.TrajectorySequence;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Mark
{
    /// <summary>
    /// Mark点信息
    /// </summary>
    public class MarkPointInfo : EntityBase
    {
        /// <summary>
        /// 点ID（默认自动生成）
        /// </summary>
        public Guid PointID { set; get; } = Guid.NewGuid();
        /// <summary>
        /// 点ID名称
        /// </summary>
        public string PointName { set; get; } ="";
        /// <summary>
        /// Mark点位置信息列表 
        /// 默认有一个点的位置信息，一个以上的位置为备份Mark点位置
        /// </summary>
        public MarkPointPositionInfoList MarkPointPositionInfoes { set; get; }

        public MarkPointInfo()
        {
            PointID = Guid.NewGuid();
            MarkPointPositionInfoes = new MarkPointPositionInfoList();
        }

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public new void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            // 基础类型（空值兜底默认值）
            SerialNumber = jObject["SerialNumber"]?.Value<int>() ?? 0;
            PointID = Guid.TryParse(jObject["PointID"]?.Value<string>(), out var guid) ? guid : Guid.NewGuid();
            PointName = jObject["PointName"]?.Value<string>() ?? string.Empty;

            // 嵌套集合对象反序列化
            if (jObject["MarkPointPositionInfoes"] is JArray axisArray)
            {
                MarkPointPositionInfoes.FromJObject(axisArray);
            }

        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public new JObject ToJObject()
        {
            return new JObject
            {
                ["SerialNumber"] = SerialNumber,
                ["PointID"] = PointID.ToString(),
                ["PointName"] = PointName.ToString(),
                ["MarkPointPositionInfoes"] = MarkPointPositionInfoes.ToJArray(),
            };
        }
    }

    /// <summary>
    /// Mark点信息列表（支持JSON序列化/反序列化）
    /// </summary>
    public class MarkPointInfoList : List<MarkPointInfo>
    {
        /// <summary>
        /// 从JArray反序列化（参数支持可空）
        /// </summary>
        public void FromJObject(JArray? jArray)
        {
            if (jArray == null) return;

            Clear();
            foreach (var itemObj in jArray)
            {
                if (itemObj is JObject markPointObj)
                {
                    var markPoint = new MarkPointInfo();
                    markPoint.FromJObject(markPointObj);
                    Add(markPoint);
                }
            }
        }

        /// <summary>
        /// 序列化为JArray
        /// </summary>
        public JArray ToJArray()
        {
            var jArray = new JArray();
            foreach (var markPoint in this)
            {
                jArray.Add(markPoint.ToJObject());
            }
            return jArray;
        }
    }

    /// <summary>
    /// Mark点位置信息
    /// </summary>
    public class MarkPointPositionInfo : EntityBase
    {
        /// <summary>
        /// 坐标轴值列表 
        /// </summary>
        public CoordinateAxisValueList CoordinateAxisValues { set; get; } = new CoordinateAxisValueList();
        /// <summary>
        /// Mark操作类型
        /// </summary>
        public MarkOpKind MarkOpKind { set; get; }
        /// <summary>
        /// Mark识别参数 
        /// </summary>
        public MarkPointRecognizeCfgInfo MarkPointRecognizeCfgInfo { set; get; } = new MarkPointRecognizeCfgInfo();

        public MarkPointPositionInfo()
        {
            CoordinateAxisValues = new CoordinateAxisValueList();
            MarkPointRecognizeCfgInfo = new MarkPointRecognizeCfgInfo();
        }

        /// <summary>
        /// 从JObject反序列化（参数支持可空）
        /// </summary>
        public new  void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            // 嵌套集合对象反序列化
            if (jObject["CoordinateAxisValues"] is JArray axisArray)
            {
                CoordinateAxisValues.FromJObject(axisArray);
            }

            MarkOpKind = jObject["MarkOpKind"] != null
                ? (MarkOpKind)jObject["MarkOpKind"].Value<int>()
                : MarkOpKind.Standard;

            // 嵌套复杂对象反序列化
            if (jObject["MarkPointRecognizeCfgInfo"] is JObject recognizeCfgObj)
            {
                MarkPointRecognizeCfgInfo.FromJObject(recognizeCfgObj);
            }
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public new JObject ToJObject()
        {
            return new JObject
            {
                ["CoordinateAxisValues"] = CoordinateAxisValues.ToJArray(),
                ["MarkOpKind"] = (int)MarkOpKind,
                ["MarkPointRecognizeCfgInfo"] = MarkPointRecognizeCfgInfo.ToJObject()
            };
        }
    }

    /// <summary>
    /// Mark点位置信息列表
    /// </summary>
    public class MarkPointPositionInfoList : List<MarkPointPositionInfo>
    {
        /// <summary>
        /// 从JArray反序列化（参数支持可空）
        /// </summary>
        public void FromJObject(JArray? jArray)
        {
            if (jArray == null) return;

            Clear();
            foreach (var itemObj in jArray)
            {
                if (itemObj is JObject markPointObj)
                {
                    var markPoint = new MarkPointPositionInfo();
                    markPoint.FromJObject(markPointObj);
                    Add(markPoint);
                }
            }
        }

        /// <summary>
        /// 序列化为JArray
        /// </summary>
        public JArray ToJArray()
        {
            var jArray = new JArray();
            foreach (var markPoint in this)
            {
                jArray.Add(markPoint.ToJObject());
            }
            return jArray;
        }
    }
}