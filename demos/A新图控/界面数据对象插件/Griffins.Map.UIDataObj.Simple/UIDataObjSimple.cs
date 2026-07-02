using GF_Gereric;
using Griffins.IOT;
using Griffins.Map;

namespace Griffins.Map.UIDataObj.Simple
{
	public class UIDataObjSimple: GFPropObjBase
    {
        private static Dictionary<string, List<GriffinsBaseValue>> _propertyIDValueRangeEnumsPairs = new();
        private static Dictionary<string, GriffinsValueNamePairList> _propertyIDValueNamePairs = new();
        static UIDataObjSimple()
        {
            initDicData();
        }

        [GFProp(GfPropReadWrite.ReadWrite)]
		public int Param1 { get; set; }

        [GFProp(GfPropReadWrite.ReadWrite, "Param_2")]
        public decimal Param2 { get; set; }

        [GFProp(GfPropReadWrite.ReadWrite, "Param_3", GriffinsBaseDataType.String)]
        public string Param3 { get; set; }

        [GFProp(GfPropReadWrite.ReadWrite, "Param_4")]
        public bool Param4 { get; set; }
		[GFProp(GfPropReadWrite.ReadWrite, "Param_5")]
		public DateTime Param5 { get; set; }

        private Dictionary<string, TestObj> _testObjs;

        /// <summary>
        /// 列表 key:列表中每项的ID,value:对象
        /// </summary>
        [GFProp(GfPropReadWrite.ReadWrite, GFPropertyEditKind.Assignment, GriffinsValueRangeDefineMode.None, "对象列表", GriffinsBaseDataType.Object_Bytes, GFBaseTypePropValueListDict.Object_IDStr)]
        public Dictionary<string, TestObj> TestObjs
        {
            get => _testObjs;
            set
            {
                _testObjs = value;
                base.RaisePropertyChanged(nameof(TestObjs));
            }
        }

        internal static List<GriffinsBaseValue>? GetValueRangeEnums(string propertyID)
        {
            return _propertyIDValueRangeEnumsPairs.ContainsKey(propertyID)
                ? _propertyIDValueRangeEnumsPairs[propertyID]
                : null;
        }

        internal static GriffinsValueNamePairList? GetValueNamePairs(string propertyID)
        {
            return _propertyIDValueNamePairs.ContainsKey(propertyID)
                ? _propertyIDValueNamePairs[propertyID]
                : null;
        }
        #region 初始化数据
        private static void initDicData()
        {
            string enumPropertyName = nameof(TestObj.MaterialStatus);
            List<GriffinsBaseValue> materialStatusEnums = new List<GriffinsBaseValue>();
            GriffinsValueNamePairList materialStatusValueNamePairs = new GriffinsValueNamePairList();

            foreach (MaterialStatus status in Enum.GetValues(typeof(MaterialStatus)))
            {
                materialStatusEnums.Add(new GriffinsBaseValue(status));
                string cnName = ResourceNames.ResourceManager.GetString(status.ToString());
                materialStatusValueNamePairs.Add(new GriffinsValueNamePair(new GriffinsBaseValue(status), cnName));
            }

            _propertyIDValueRangeEnumsPairs.Add(enumPropertyName, materialStatusEnums);
            _propertyIDValueNamePairs.Add(enumPropertyName, materialStatusValueNamePairs);
        }
        #endregion
    }
    /// <summary>
    /// 对象信息
    /// </summary>
    public class TestObj : GFPropObjBase
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
