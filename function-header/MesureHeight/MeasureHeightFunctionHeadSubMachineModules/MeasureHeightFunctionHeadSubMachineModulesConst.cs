using Griffins;
using Griffins.ImeIOT;

namespace GKG
{
    namespace SubMM
    {
        public class MeasureHeightFunctionHeadSubMachineModulesConst
        {
            public const string SubMMName = "测高功能头";

            public const string SubMMModelStr = "MeasureHeightFunctionHead";

            public static readonly SubMMModel SubMMModel = SubMMModel.Parse(SubMMModelStr);

            public readonly static SubMMObjInfoList SubMMObjInfos = new SubMMObjInfoList
        {
            new SubMMObjInfo
            {
                SubMMObjID = Guid.Parse("{11E0E84F-450A-4B0B-9E14-91008D51CA0B}"),
                SubMMObjName = "测高功能头"
            },
        };

            public const string HeightParamID = "Height";

            /// <summary>
            /// 测高
            /// </summary>
            public const string MeasureHeightMethodID = "MeasureHeight";

            public static readonly ImeCompMethodDefInfoList Methods = new ImeCompMethodDefInfoList()
        {
            new ImeCompMethodDefInfo(MeasureHeightMethodID,"测高",new GFParamDefInfoList(),new GFParamDefInfoList()
            {
                new GFParamDefInfo(HeightParamID, "高度", GriffinsBaseDataType.Decimal)
            },false),
        };

            /// <summary>
            /// 子机械模组能力事件定义信息列表
            /// 不是和能力方法匹配的其他能力事件
            /// </summary>
            public static readonly ImeCompEventDefInfoList Events = new ImeCompEventDefInfoList()
            {
            };

            public static readonly ImeCompMethodDefInfoList normalMethodDefInfos = new ImeCompMethodDefInfoList()
        {
            new ImeCompMethodDefInfo(MeasureHeightMethodID,"测高",new GFParamDefInfoList(),new GFParamDefInfoList()
            {
                new GFParamDefInfo(HeightParamID, "高度", GriffinsBaseDataType.Decimal)
            },false),
        };
        }
    }
}