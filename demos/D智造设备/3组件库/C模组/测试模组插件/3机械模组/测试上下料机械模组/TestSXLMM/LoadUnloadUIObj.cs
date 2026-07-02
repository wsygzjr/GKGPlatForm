using GF_Gereric;
using Griffins;
using Griffins.IOT;
using Griffins.Map;
using System;
using System.Collections.Generic;
using TestSXLMM;

namespace TestSXLMM
{
    /// <summary>
    /// 上下料界面描述对象
    /// </summary>
    public class LoadUnloadUIObj : GFPropObjBase
    {
        /// <summary>
        /// 属性ID对应的值选择范围字典
        /// </summary>
        private static Dictionary<string, List<GriffinsBaseValue>> _propertyIDValueRangeEnumsPairs = new();
        /// <summary>
        /// 属性ID对应的值和值名称键值对
        /// </summary>
        private static Dictionary<string, GriffinsValueNamePairList> _propertyIDValueNamePairs = new();
        static LoadUnloadUIObj()
        {
            initValueRangeData();
        }

        private Dictionary<string, MaterialContainer> _materialContainers;
        /// <summary>
        /// 容器列表 key:容器列表中每项的ID,value:容器对象 
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite, GFPropertyEditKind.Assignment, GriffinsValueRangeDefineMode.None, "上下料容器状态列表", GriffinsBaseDataType.Object_Bytes, GFBaseTypePropValueListDict.Object_IDStr)]
        public Dictionary<string,MaterialContainer> MaterialContainers
        {
            get => _materialContainers;
            set
            {
                _materialContainers = value;
                base.RaisePropertyChanged(nameof(MaterialContainers));
            }
        }
        #region 测试属性

        private MaterialBox _testMaterialBox;
        [GFProp(GfPropReadWrite.ReadWrite, GFPropertyEditKind.Assignment, GriffinsValueRangeDefineMode.None, "测试属性2料盒对象", GriffinsBaseDataType.Object_Bytes, GFBaseTypePropValueList.Object_IDStr)]
        public MaterialBox TestMaterialBox
        {
            get => _testMaterialBox;
            set
            {
                _testMaterialBox = value;
                base.RaisePropertyChanged(nameof(TestMaterialBox));
            }
        }

        private MaterialStatus _TestMaterialStatus;
        [GFProp(GfPropReadWrite.ReadWrite, GFPropertyEditKind.Select, GriffinsValueRangeDefineMode.Enumeration, "测试枚举属性")]
        public MaterialStatus TestMaterialStatus
        {
            get => _TestMaterialStatus;
            set
            {
                _TestMaterialStatus = value;
                base.RaisePropertyChanged(nameof(TestMaterialStatus));
            }
        }

        private int _testParam1;
        [GFProp(GfPropReadWrite.ReadWrite, "测试基础类型属性")]
        public int TestParam1
        {
            get => _testParam1;
            set
            {
                _testParam1 = value;
                base.RaisePropertyChanged(nameof(TestParam1));
            }
        }

        /// <summary>
        /// 获取指定属性ID对应的取值范围
        /// </summary>
        /// <param name="propertyID">定属性ID</param>
        /// <returns>对应的取值范围</returns>
        internal static List<GriffinsBaseValue> GetValueRangeEnums(string propertyID)
        {
            return _propertyIDValueRangeEnumsPairs.ContainsKey(propertyID)
                ? _propertyIDValueRangeEnumsPairs[propertyID]
                : null;
        }
        /// <summary>
        /// 获取指定属性ID对应的取值范围的值和值名称键值对
        /// </summary>
        /// <param name="propertyID">定属性ID</param>
        /// <returns>对应的取值范围的值和值名称键值对</returns>
        internal static GriffinsValueNamePairList GetValueNamePairs(string propertyID)
        {
            return _propertyIDValueNamePairs.ContainsKey(propertyID)
                ? _propertyIDValueNamePairs[propertyID]
                : null;
        }
        #endregion


        #region 初始化取值范围数据
        /// <summary>
        /// 初始化取值范围数据
        /// </summary>
        private static void initValueRangeData()
        {
            //料盒状态属性ID的取值范围定义
            string enumPropertyName = nameof(SlotObj.MaterialStatus);
            List<GriffinsBaseValue> materialStatusEnums = new List<GriffinsBaseValue>();
            GriffinsValueNamePairList materialStatusValueNamePairs = new GriffinsValueNamePairList();

            foreach (MaterialStatus status in Enum.GetValues(typeof(MaterialStatus)))
            {
                materialStatusEnums.Add(new GriffinsBaseValue(status));
                //取值范围值对应的显示名称或值含义名称，需在资源文件中定义
                string cnName = ResourceNames.ResourceManager.GetString(status.ToString());
                materialStatusValueNamePairs.Add(new GriffinsValueNamePair(new GriffinsBaseValue(status), cnName));
            }

            _propertyIDValueRangeEnumsPairs.Add(enumPropertyName, materialStatusEnums);
            _propertyIDValueNamePairs.Add(enumPropertyName, materialStatusValueNamePairs);

            //测试
            string testMaterialStatusName = nameof(LoadUnloadUIObj.TestMaterialStatus);
            _propertyIDValueRangeEnumsPairs.Add(testMaterialStatusName, materialStatusEnums);
            _propertyIDValueNamePairs.Add(testMaterialStatusName, materialStatusValueNamePairs);
        }
        #endregion
    }

    /// <summary>
    /// 容器界面描述对象
    /// </summary>
    public class MaterialContainer : GFPropObjBase
    {
        private string _containerName;
        [GFProp(GfPropReadWrite.ReadOnly, "物料容器名")]
        public string ContainerName
        {
            get => _containerName;
            set
            {
                _containerName = value;
                base.RaisePropertyChanged(nameof(ContainerName));
            }
        }

        private Dictionary<string,MaterialBox> _materialBoxs;
        [GFProp(GfPropReadWrite.ReadWrite, GFPropertyEditKind.Assignment, GriffinsValueRangeDefineMode.None, "料盒状态", GriffinsBaseDataType.Object_Bytes, GFBaseTypePropValueListDict.Object_IDStr)]
        public Dictionary<string, MaterialBox> MaterialBoxs
        {
            get => _materialBoxs;
            set
            {
                _materialBoxs = value;
                base.RaisePropertyChanged(nameof(MaterialBoxs));
            }
        }
    }

    /// <summary>
    /// 料盒
    /// </summary>
    public class MaterialBox : GFPropObjBase
    {
      
        private string _materialBoxName;
        [GFProp(GfPropReadWrite.ReadOnly, "料盒名称")]
        public string MaterialBoxName
        {
            get => _materialBoxName;
            set
            {
                _materialBoxName = value;
                base.RaisePropertyChanged(nameof(MaterialBoxName));
            }
        }

        private bool _isEmpty;
        [GFProp(GfPropReadWrite.ReadWrite, "是否有料盒")]
        public bool IsEmpty
        {
            get => _isEmpty;
            set
            {
                _isEmpty = value;
                base.RaisePropertyChanged(nameof(IsEmpty));
            }
        }

        private bool _materialBoxCylinderStatus;
        [GFProp(GfPropReadWrite.ReadWrite, "料盒气缸状态")]
        public bool MaterialBoxCylinderStatus
        {
            get => _materialBoxCylinderStatus;
            set
            {
                _materialBoxCylinderStatus = value;
                base.RaisePropertyChanged(nameof(MaterialBoxCylinderStatus));
            }
        }

        private Dictionary<string,SlotObj> _slotObjs;
        [GFProp(GfPropReadWrite.ReadWrite, GFPropertyEditKind.Assignment, GriffinsValueRangeDefineMode.None, "料槽状态", GriffinsBaseDataType.Object_Bytes, GFBaseTypePropValueListDict.Object_IDStr)]
        public Dictionary<string, SlotObj> SlotObjs
        {
            get => _slotObjs;
            set
            {
                _slotObjs = value;
                base.RaisePropertyChanged(nameof(SlotObjs));
            }
        }
    }

    /// <summary>
    /// 料槽对象
    /// </summary>
    public class SlotObj : GFPropObjBase
    {

        private MaterialStatus _materialStatus;
        [GFProp(GfPropReadWrite.ReadWrite, GFPropertyEditKind.Select, GriffinsValueRangeDefineMode.Enumeration, "料槽状态")]
        public MaterialStatus MaterialStatus
        {
            get => _materialStatus;
            set
            {
                _materialStatus = value;
                base.RaisePropertyChanged(nameof(MaterialStatus));
            }
        }
    }
    /// <summary>
    /// 料盒状态
    /// </summary>
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
}