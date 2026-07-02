using Griffins;
using Griffins.ImeIOT;

namespace GKG.SubMM
{
    public class MotionCalculateSubMachineModulesConst
    {
        /// <summary>
        /// 点胶运动计算
        /// </summary>
        public const string SubMMName = "运动计算";

        public const string SubMMModelStr = "MotionCalculate";

        public readonly static SubMMModel SubMMModel = SubMMModel.Parse(SubMMModelStr);

        /// <summary>
        /// 计算
        /// </summary>
        public const string CalculateMethodID = "Calculate";

        /// <summary>
        /// 计算完成能力事件ID（由能力方法产生的完成事件）
        /// </summary>
        public static readonly string CalculateFinishedEventID = ImeCompMethodDefInfo.GetFinishedEventID(CalculateMethodID);

        /// <summary>
        /// 计算
        /// </summary>
        public const string CalculateDoubleValveSpacingMethodID = "CalculateDoubleValveSpacing";

        /// <summary>
        /// 计算完成能力事件ID（由能力方法产生的完成事件）
        /// </summary>
        public static readonly string CalculateDoubleValveSpacingFinishedEventID = ImeCompMethodDefInfo.GetFinishedEventID(CalculateDoubleValveSpacingMethodID);

        /// <summary>
        /// 子机械模组能力方法定义信息列表
        /// 约定能力方法产生的能力事件，用 ImeCompMethodDefInfo.GetFinishedEventID() 产生
        /// </summary>
        public static readonly ImeCompMethodDefInfoList Methods = new ImeCompMethodDefInfoList()
        {
            new ImeCompMethodDefInfo(CalculateMethodID, "计算", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
            new ImeCompMethodDefInfo(CalculateDoubleValveSpacingMethodID, "计算双阀间距", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
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
            new ImeCompMethodDefInfo(CalculateMethodID, "计算", new GFParamDefInfoList(), new GFParamDefInfoList(), true),
            new ImeCompMethodDefInfo(CalculateDoubleValveSpacingMethodID, "计算双阀间距", new GFParamDefInfoList(), new GFParamDefInfoList(), true),
        };
        public static SubMMObjInfoList SubMMObjInfos => new SubMMObjInfoList
        {
            new SubMMObjInfo()
            {
                SubMMObjID = Guid.Parse("{778DB1A2-695A-40B4-81E8-19D29F8F0288}"),
                SubMMObjName = "直线单轴运动计算",
            },
            new SubMMObjInfo()
            {
                SubMMObjID = Guid.Parse("{4445E636-DFDB-4A56-87CB-1B9FE8BDD367}"),
                SubMMObjName = "平面运动计算",
            },
            new SubMMObjInfo()
            {
                SubMMObjID = Guid.Parse("{EA38B019-8D1B-4802-9176-5B679E1C01AF}"),
                SubMMObjName = "XYZ-xyz运动计算",
            },

        };
    }
}
