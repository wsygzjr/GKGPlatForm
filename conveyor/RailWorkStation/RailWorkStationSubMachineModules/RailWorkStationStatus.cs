using GF_Gereric;
using GKG;
using Griffins;
using System;
using System.Text.Json.Serialization;

namespace GKG.SubMM
{
    public class RailWorkStationStatus : GFPropObjBase, IGriffinsBaseValue
    {
        public static readonly Guid Object_ID = new Guid("{3F2FC338-8547-4233-B9DA-55860914A5B2}");

        /// <summary>
        /// 是否有料
        /// </summary>
        private bool _isHaveMaterial;
        [GFProp(GfPropReadWrite.ReadOnly, GFPropertyEditKind.Assignment, GriffinsValueRangeDefineMode.None, "是否有料")]
        public bool IsHaveMaterial
        {
            get => _isHaveMaterial;
            set
            {
                _isHaveMaterial = value;
                base.RaisePropertyChanged(nameof(IsHaveMaterial));
            }
        }

        /// <summary>
        /// 左感应器状态
        /// </summary>
        private bool _leftSensorState;
        [GFProp(GfPropReadWrite.ReadOnly, GFPropertyEditKind.Assignment, GriffinsValueRangeDefineMode.None, "左感应器状态")]
        public bool LeftSensorState
        {
            get => _leftSensorState;
            set
            {
                _leftSensorState = value;
                base.RaisePropertyChanged(nameof(LeftSensorState));
            }
        }

        /// <summary>
        /// 右感应器状态
        /// </summary>
        private bool _rightSensorState;
        [GFProp(GfPropReadWrite.ReadOnly, GFPropertyEditKind.Assignment, GriffinsValueRangeDefineMode.None, "右感应器状态")]
        public bool RightSensorState
        {
            get => _rightSensorState;
            set
            {
                _rightSensorState = value;
                base.RaisePropertyChanged(nameof(RightSensorState));
            }
        }

        /// <summary>
        /// 左气缸状态
        /// </summary>
        private ECylinderPosType _leftCylinderState;
        [GFProp(GfPropReadWrite.ReadOnly, GFPropertyEditKind.Select, GriffinsValueRangeDefineMode.Enumeration, "左气缸状态")]
        public ECylinderPosType LeftCylinderState
        {
            get => _leftCylinderState;
            set
            {
                _leftCylinderState = value;
                base.RaisePropertyChanged(nameof(LeftCylinderState));
            }
        }

        /// <summary>
        /// 右气缸状态
        /// </summary>
        private ECylinderPosType _rightCylinderState;
        [GFProp(GfPropReadWrite.ReadOnly, GFPropertyEditKind.Select, GriffinsValueRangeDefineMode.Enumeration, "右气缸状态")]
        public ECylinderPosType RightCylinderState
        {
            get => _rightCylinderState;
            set
            {
                _rightCylinderState = value;
                base.RaisePropertyChanged(nameof(RightCylinderState));
            }
        }

        [JsonIgnore]
        bool IGriffinsBaseValue.IsObject_Byte => false;

        Guid IGriffinsBaseValue.GetObject_ID()
        {
            return Object_ID;
        }

        GriffinsBaseValue IGriffinsBaseValue.ToBaseValue()
        {
            ObjectValue_Json jsonValue = new ObjectValue_Json(Object_ID)
            {
                JsonVal = toJson()
            };
            return new GriffinsBaseValue(jsonValue);
        }

        private string toJson()
        {
            return JsonObjConvert.ToJSon(this);
        }

        void IGriffinsBaseValue.PopulateFromBaseValue(GriffinsBaseValue baseValue)
        {
            if ((baseValue == null) || (baseValue.Val == null))
                return;
            if (!(baseValue.Val is ObjectValue_Json))
                throw new Exception("对象值不是DashStyleData转换的");
            if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                throw new Exception("对象值不是DashStyleData转换的");
            fromJson((baseValue.Val as ObjectValue_Json).JsonVal);
        }

        private void fromJson(string json)
        {
            RailWorkStationStatus obj = JsonObjConvert.FromJSon<RailWorkStationStatus>(json);
            if (obj != null)
            {
                IsHaveMaterial = obj.IsHaveMaterial;
                LeftSensorState = obj.LeftSensorState;
                RightSensorState = obj.RightSensorState;
                LeftCylinderState = obj.LeftCylinderState;
                RightCylinderState = obj.RightCylinderState;
            }
        }
    }
}
