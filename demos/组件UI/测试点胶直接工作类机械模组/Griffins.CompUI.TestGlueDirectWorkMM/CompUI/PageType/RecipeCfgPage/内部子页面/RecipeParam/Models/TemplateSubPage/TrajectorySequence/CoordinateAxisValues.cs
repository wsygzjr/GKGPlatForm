using Newtonsoft.JsonG.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.TrajectorySequence
{
    /// <summary>
    /// 坐标轴值
    /// </summary>
    public class CoordinateAxisValue
    {
        /// <summary>
        /// 坐标轴
        /// </summary>
        public CoordinateAxisConstant Axis { get; set; } = CoordinateAxisConstant.X; 

        /// <summary>
        /// 轴值
        /// </summary>
        public decimal AxisValue { get; set; } = 0.0m;

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            // 枚举类型：空值兜底默认值，避免转换异常
            Axis = jObject["Axis"]?.Value<int>() is int axisInt
                ? (CoordinateAxisConstant)axisInt
                : CoordinateAxisConstant.X;

            // 数值类型：空值兜底0.0m
            AxisValue = jObject["AxisValue"]?.Value<decimal>() ?? 0.0m;
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public JObject ToJObject()
        {
            return new JObject
            {
                ["Axis"] = (int)Axis, 
                ["AxisValue"] = AxisValue
            };
        }

        public CoordinateAxisValue()
        {
        }
    }

    /// <summary>
    /// 坐标轴值列表
    /// </summary>
    public class CoordinateAxisValueList : List<CoordinateAxisValue>
    {
        /// <summary>
        /// 从JArray反序列化
        /// </summary>
        public void FromJObject(JArray? jArray)
        {
            if (jArray == null) return;

            Clear(); 
            foreach (var itemObj in jArray)
            {
                if (itemObj is JObject itemJObject)
                {
                    var item = new CoordinateAxisValue();
                    item.FromJObject(itemJObject);
                    Add(item);
                }
            }
        }

        /// <summary>
        /// 序列化为JArray
        /// </summary>
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

    /// <summary>
    /// 点信息（
    /// </summary>
    public class PointInfo
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int SerialNumber { set; get; } = 0;

        /// <summary>
        /// 点ID（
        /// </summary>
        public Guid PointID { set; get; } = Guid.NewGuid();

        /// <summary>
        /// 坐标轴值列表
        /// </summary>
        public CoordinateAxisValueList CoordinateAxisValues { set; get; } = new CoordinateAxisValueList();

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            SerialNumber = jObject["SerialNumber"]?.Value<int>() ?? 0;

            PointID = Guid.TryParse(jObject["PointID"]?.Value<string>(), out var guid)
                ? guid
                : PointID;

            if (jObject["CoordinateAxisValues"] is JArray valuesArray)
            {
                CoordinateAxisValues ??= new CoordinateAxisValueList();
                CoordinateAxisValues.FromJObject(valuesArray);
            }
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public JObject ToJObject()
        {
            return new JObject
            {
                ["SerialNumber"] = SerialNumber,
                ["PointID"] = PointID.ToString(), 
                ["CoordinateAxisValues"] = CoordinateAxisValues.ToJArray() 
            };
        }

        public PointInfo()
        {
            PointID = Guid.NewGuid();
            CoordinateAxisValues = new CoordinateAxisValueList();
        }
    }

    /// <summary>
    /// 点信息列表
    /// </summary>
    public class PointInfoList : List<PointInfo>
    {
        /// <summary>
        /// 从JArray反序列化
        /// </summary>
        public void FromJObject(JArray? jArray)
        {
            if (jArray == null) return;

            Clear(); // 反序列化前清空列表
            foreach (var itemObj in jArray)
            {
                if (itemObj is JObject itemJObject)
                {
                    var item = new PointInfo();
                    item.FromJObject(itemJObject);
                    Add(item);
                }
            }
        }

        /// <summary>
        /// 序列化为JArray
        /// </summary>
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