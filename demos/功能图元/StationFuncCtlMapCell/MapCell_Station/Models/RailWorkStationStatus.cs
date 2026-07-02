using GF_Gereric;
using GKG;
using Griffins;
using System;

namespace GKG.Map.StationFuncCtlMapCell.Models
{
    /// <summary>
    /// 气缸物理位置/状态枚举
    /// </summary>
    public enum ECylinderPosType
    {
        /// <summary>伸出状态</summary>
        Stretch,
        /// <summary>缩回状态</summary>
        Retract,
        /// <summary>异常状态</summary>
        UnNormal
    }

    /// <summary>
    /// 后端轨道工位状态模型定义
    /// 实现了 Griffins 的序列化接口。
    /// </summary>
    public class RailWorkStationStatus : IGriffinsBaseValue
    {
        private Guid Object_ID = new Guid("{3F2FC338-8547-4233-B9DA-55860914A5B2}");
        bool IGriffinsBaseValue.IsObject_Byte => false;

        /// <summary>
        /// 是否有料 (料板是否在位)
        /// </summary>
        public bool IsHaveMaterial { get; set; }

        /// <summary>
        /// 左感应器状态
        /// </summary>
        public bool LeftSensorState { get; set; }

        /// <summary>
        /// 右感应器状态
        /// </summary>
        public bool RightSensorState { get; set; }

        /// <summary>
        /// 左气缸(挡板)状态
        /// </summary>
        public ECylinderPosType LeftCylinderState { get; set; }

        /// <summary>
        /// 右气缸(挡板)状态
        /// </summary>
        public ECylinderPosType RightCylinderState { get; set; }

        #region IGriffinsBaseValue 序列化接口实现

        Guid IGriffinsBaseValue.GetObject_ID() => Object_ID;

        GriffinsBaseValue IGriffinsBaseValue.ToBaseValue()
        {
            ObjectValue_Json jsonValue = new ObjectValue_Json(Object_ID)
            {
                JsonVal = ToJson()
            };
            return new GriffinsBaseValue(jsonValue);
        }

        void IGriffinsBaseValue.PopulateFromBaseValue(GriffinsBaseValue baseValue)
        {
            if (baseValue?.Val == null)
                return;

            if (!(baseValue.Val is ObjectValue_Json jsonVal))
                throw new Exception("对象值非预期的 ObjectValue_Json 类型");

            if (jsonVal.Object_ID != Object_ID)
                throw new Exception("对象类型 ID 匹配失败");

            FromJson(jsonVal.JsonVal);
        }

        #endregion

        #region 内部 JSON 辅助方法

        private string ToJson()
        {
            return JsonObjConvert.ToJSon(this);
        }

        private void FromJson(string json)
        {
            RailWorkStationStatus obj = JsonObjConvert.FromJSon<RailWorkStationStatus>(json);
            if (obj != null)
            {
                this.IsHaveMaterial = obj.IsHaveMaterial;
                this.LeftSensorState = obj.LeftSensorState;
                this.RightSensorState = obj.RightSensorState;
                this.LeftCylinderState = obj.LeftCylinderState;
                this.RightCylinderState = obj.RightCylinderState;
            }
        }

        #endregion
    }
}