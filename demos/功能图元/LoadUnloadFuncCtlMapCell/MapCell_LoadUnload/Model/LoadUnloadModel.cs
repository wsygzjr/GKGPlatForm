using GF_Gereric;
using Griffins;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GKG.Map.LoadUnloadFuncCtlMapCell.Models
{
    public enum MaterialStatus
    {
        /// <summary>
        /// 禁用
        /// </summary>
        Disable,

        /// <summary>
        /// 空料
        /// </summary>
        Empty,

        /// <summary>
        /// 有料
        /// </summary>
        Full,
    }

    /// <summary>
    /// 料槽状态
    /// </summary>
    public class SlotStatus : GFPropObjBase
    {
        public string ID = "";

        /// <summary>
        /// 槽状态：禁用、空料、有料
        /// </summary>
        private MaterialStatus _materialStatus;
        [GFProp(GfPropReadWrite.ReadOnly, GFPropertyEditKind.Select, GriffinsValueRangeDefineMode.Enumeration, "槽状态")]
        public MaterialStatus MaterialStatus
        {
            get => _materialStatus;
            set
            {
                _materialStatus = value;
                base.RaisePropertyChanged(nameof(MaterialStatus));
            }
        }

        /// <summary>
        /// 深拷贝方法
        /// </summary>
        public void CopyFrom(SlotStatus source)
        {
            if (source == null) return;
            this.ID = source.ID;
            this.MaterialStatus = source.MaterialStatus;
        }
    }

    /// <summary>
    /// 槽位状态列表 (带有独立序列化支持)
    /// </summary>
    public class SlotStatuses : List<SlotStatus>, IGriffinsBaseValue
    {
        [JsonIgnore]
        public static readonly Guid Object_ID = new Guid("{3B0DA5A7-7943-42F6-9EEC-A849F51791B5}");
        bool IGriffinsBaseValue.IsObject_Byte => false;

        Guid IGriffinsBaseValue.GetObject_ID()
        {
            return Object_ID;
        }

        private void fromJson(string json)
        {
            this.Clear();
            this.AddRange(JsonObjConvert.FromJSon<List<SlotStatus>>(json));
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

        private string toJson()
        {
            return JsonObjConvert.ToJSon(this);
        }

        GriffinsBaseValue IGriffinsBaseValue.ToBaseValue()
        {
            ObjectValue_Json jsonValue = new ObjectValue_Json(Object_ID)
            {
                JsonVal = toJson()
            };
            return new GriffinsBaseValue(jsonValue);
        }

        /// <summary>
        /// 深拷贝方法：遍历列表逐个拷贝内部对象
        /// </summary>
        public void CopyFrom(SlotStatuses source)
        {
            this.Clear();
            if (source == null) return;

            foreach (var item in source)
            {
                var newSlot = new SlotStatus();
                newSlot.CopyFrom(item);
                this.Add(newSlot);
            }
        }
    }

    /// <summary>
    /// 料盒状态
    /// </summary>
    public class MaterialBoxStatus : GFPropObjBase
    {
        /// <summary>
        /// 料盒名字
        /// </summary>
        private string _name = "";
        [GFProp(GfPropReadWrite.ReadOnly, GFPropertyEditKind.Assignment, GriffinsValueRangeDefineMode.None, "料盒名字")]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                base.RaisePropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        /// 料槽状态列表：每个槽位的状态，顺序对应槽位编号；当料盒未检测到或检测异常时，列表可为空或包含默认状态。
        /// </summary>
        private Dictionary<string, SlotStatuses> _slotStatusList = new Dictionary<string, SlotStatuses>();
        [GFProp(GfPropReadWrite.ReadOnly, GFPropertyEditKind.Assignment, GriffinsValueRangeDefineMode.None, "料槽状态列表", GriffinsBaseDataType.Object_Bytes, GFBaseTypePropValueListDict.Object_IDStr)]
        public Dictionary<string, SlotStatuses> SlotStatusList
        {
            get => _slotStatusList;
            set
            {
                _slotStatusList = value;
                base.RaisePropertyChanged(nameof(SlotStatusList));
            }
        }

        /// <summary>
        /// 料盒感应状态
        /// </summary>
        private bool _isEmpty;
        [GFProp(GfPropReadWrite.ReadOnly, GFPropertyEditKind.Assignment, GriffinsValueRangeDefineMode.None, "料盒感应状态")]
        public bool IsEmpty
        {
            get => _isEmpty;
            set
            {
                _isEmpty = value;
                base.RaisePropertyChanged(nameof(IsEmpty));
            }
        }

        /// <summary>
        /// 料盒气缸状态
        /// </summary>
        private bool _materialBoxCylinderStatus;
        [GFProp(GfPropReadWrite.ReadOnly, GFPropertyEditKind.Assignment, GriffinsValueRangeDefineMode.None, "料盒气缸状态")]
        public bool MaterialBoxCylinderStatus
        {
            get => _materialBoxCylinderStatus;
            set
            {
                _materialBoxCylinderStatus = value;
                base.RaisePropertyChanged(nameof(MaterialBoxCylinderStatus));
            }
        }

        /// <summary>
        /// 是否上料料盒
        /// </summary>
        private bool _isFeeding = true;
        [GFProp(GfPropReadWrite.ReadOnly, GFPropertyEditKind.Assignment, GriffinsValueRangeDefineMode.None, "是否上料料盒")]
        public bool IsFeeding
        {
            get => _isFeeding;
            set
            {
                _isFeeding = value;
                base.RaisePropertyChanged(nameof(IsFeeding));
            }
        }

        /// <summary>
        /// 深拷贝方法：新建字典并调用下级字典 Values 的 CopyFrom
        /// </summary>
        public void CopyFrom(MaterialBoxStatus source)
        {
            if (source == null) return;

            this.Name = source.Name;
            this.IsEmpty = source.IsEmpty;
            this.MaterialBoxCylinderStatus = source.MaterialBoxCylinderStatus;
            this.IsFeeding = source.IsFeeding;

            var newSlotStatusList = new Dictionary<string, SlotStatuses>();
            if (source.SlotStatusList != null)
            {
                foreach (var kv in source.SlotStatusList)
                {
                    var newSlotStatuses = new SlotStatuses();
                    newSlotStatuses.CopyFrom(kv.Value);
                    newSlotStatusList.Add(kv.Key, newSlotStatuses);
                }
            }
            this.SlotStatusList = newSlotStatusList;
        }
    }

    /// <summary>
    /// 物料容器状态：包含多个料盒状态，顺序对应料盒编号；当未检测到任何料盒或检测异常时，列表可为空或包含默认状态。
    /// </summary>
    public class MaterialContainerStatus : GFPropObjBase
    {
        private Dictionary<string, MaterialBoxStatus> _materialBoxes = new Dictionary<string, MaterialBoxStatus>();
        /// <summary>
        /// 料盒状态列表
        /// </summary>
        [GFProp(GfPropReadWrite.ReadOnly, GFPropertyEditKind.Assignment, GriffinsValueRangeDefineMode.None, "料盒状态列表", GriffinsBaseDataType.Object_Bytes, GFBaseTypePropValueListDict.Object_IDStr)]
        public Dictionary<string, MaterialBoxStatus> MaterialBoxes
        {
            get => _materialBoxes;
            set
            {
                _materialBoxes = value;
                base.RaisePropertyChanged(nameof(MaterialBoxes));
            }
        }

        /// <summary>
        /// 容器名字
        /// </summary>
        private string _name;
        [GFProp(GfPropReadWrite.ReadOnly, GFPropertyEditKind.Assignment, GriffinsValueRangeDefineMode.None, "容器名字")]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                base.RaisePropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        /// 是否上料容器
        /// </summary>
        private bool _isFeeding;
        [GFProp(GfPropReadWrite.ReadOnly, GFPropertyEditKind.Assignment, GriffinsValueRangeDefineMode.None, "是否上料容器")]
        public bool IsFeeding
        {
            get => _isFeeding;
            set
            {
                _isFeeding = value;
                base.RaisePropertyChanged(nameof(IsFeeding));
            }
        }

        /// <summary>
        /// 深拷贝方法
        /// </summary>
        public void CopyFrom(MaterialContainerStatus source)
        {
            if (source == null) return;

            this.Name = source.Name;
            this.IsFeeding = source.IsFeeding;

            var newMaterialBoxes = new Dictionary<string, MaterialBoxStatus>();
            if (source.MaterialBoxes != null)
            {
                foreach (var kv in source.MaterialBoxes)
                {
                    var newMaterialBoxStatus = new MaterialBoxStatus();
                    newMaterialBoxStatus.CopyFrom(kv.Value);
                    newMaterialBoxes.Add(kv.Key, newMaterialBoxStatus);
                }
            }
            this.MaterialBoxes = newMaterialBoxes;
        }
    }

    /// <summary>
    /// 全局上下料容器列表聚合对象
    /// </summary>
    public class MaterialContainerStatusList : GFPropObjBase
    {
        private Dictionary<string, MaterialContainerStatus> _materialContainers = new Dictionary<string, MaterialContainerStatus>();

        /// <summary>
        /// 容器列表 key:容器列表中每项的ID, value:容器对象 
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite, GFPropertyEditKind.Assignment, GriffinsValueRangeDefineMode.None, "上下料容器状态列表", GriffinsBaseDataType.Object_Bytes, GFBaseTypePropValueListDict.Object_IDStr)]
        public Dictionary<string, MaterialContainerStatus> MaterialContainers
        {
            get => _materialContainers;
            set
            {
                _materialContainers = value;
                base.RaisePropertyChanged(nameof(MaterialContainers));
            }
        }

    }
}