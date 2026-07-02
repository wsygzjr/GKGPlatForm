using Griffins;
using Griffins.ImeIOT;

namespace GKG.SubMM.Dispenser
{
    public class WeighingBalanceSubMachineModulesConst
    {
        public const string SubMMName = "称重模组";

        public const string SubMMModelStr = "WeighingBalance";

        public static readonly SubMMModel SubMMModel = SubMMModel.Parse(SubMMModelStr);

        public static readonly SubMMObjInfoList SubMMObjInfos = new SubMMObjInfoList()
        {
            new SubMMObjInfo{
                SubMMObjName ="称重模组",
                SubMMObjID = Guid.Parse("{3B80E94E-7BAD-4E02-9882-C62E9A26E3CD}")
            },
        };

        public const string GetWeightCmdID = "GetWeight";
        public const string GetAxisOptionsCmdID = "GetAxisOptions";
        public const string GetValveOptionsCmdID = "GetValveOptions";
        public const string GetInitCfgCmdID = "GetInitCfg";

        /// <summary>
        /// 开始能力方法(异步)
        /// </summary>
        public const string WeighingMethodID = "Weighing";

        /// <summary>
        /// 开始完成能力事件ID
        /// </summary>
        public static readonly string WeighingFinishedEventID = ImeCompMethodDefInfo.GetFinishedEventID(WeighingMethodID);

        /// <summary>
        /// 子机械模组能力方法定义信息列表
        /// 约定能力方法产生的能力事件，用ImeMethodDefInfo.GetFinishedEventID()产生
        /// </summary>
        public static readonly ImeCompMethodDefInfoList Methods = new ImeCompMethodDefInfoList()
        {
            new ImeCompMethodDefInfo(WeighingMethodID,"称重",new GFParamDefInfoList(),new GFParamDefInfoList(),false),
        };

        /// <summary>
        /// 子机械模组能力事件定义信息列表
        /// 不是和能力方法匹配的其他能力事件
        /// </summary>
        public static readonly ImeCompEventDefInfoList Events = new ImeCompEventDefInfoList()
        {
            new ImeCompEventDefInfo(WeighingFinishedEventID,"称重完成",new GFParamDefInfoList()),
        };

        public static readonly ImeCompMethodDefInfoList normalMethodDefInfos = new ImeCompMethodDefInfoList()
        {
            new ImeCompMethodDefInfo(WeighingMethodID, "称重", new GFParamDefInfoList(),new GFParamDefInfoList(), false),
        };
    }
}