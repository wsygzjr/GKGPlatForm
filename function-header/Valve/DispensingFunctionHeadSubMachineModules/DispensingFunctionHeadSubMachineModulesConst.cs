using Griffins;
using Griffins.ImeIOT;

namespace GKG.SubMM.Dispenser
{
    public class DispensingFunctionHeadSubMachineModulesConst
    {
        public const string SubMMName = "点胶机功能头";

        public const string SubMMModelStr = "DispensingFunctionHead";

        public static readonly SubMMModel SubMMModel = SubMMModel.Parse(SubMMModelStr);

        public static readonly SubMMObjInfoList SubMMObjInfos = new SubMMObjInfoList
        {
            new SubMMObjInfo
            {
                SubMMObjID = Guid.Parse("{E341F30A-810B-41FC-99A3-745C34A88639}"),
                SubMMObjName = "GKG点胶阀"
            }
        };
        /// <summary>
        /// 开始工作能力方法
        /// </summary>
        public const string StartActionMethodID = "StartAction";

        /// <summary>
        /// 开始工作完成能力事件ID
        /// </summary>
        public static readonly string StartActionFinishedEventID = ImeCompMethodDefInfo.GetFinishedEventID(StartActionMethodID);

        /// <summary>
        /// 停止工作能力方法
        /// </summary>
        public const string StopActionMethodID = "StopAction";

        /// <summary>
        /// 停止工作完成能力事件ID
        /// </summary>
        public static readonly string StopActionFinishedEventID = ImeCompMethodDefInfo.GetFinishedEventID(StopActionMethodID);

        /// <summary>
        /// 可以开始工作普通方法
        /// </summary>
        public const string CanStartAction = "CanStartAction";

        /// <summary>
        /// 可以停止工作普通方法
        /// </summary>
        public const string CanStopAction = "CanStopAction";

        /// <summary>
        /// 开始工作前普通事件
        /// </summary>
        public const string BeforeStartAction = "BeforeStartAction";

        /// <summary>
        /// 停止工作前普通事件
        /// </summary>
        public const string BeforeStopAction = "BeforeStopAction";

        /// <summary>
        /// 工作状态变化能力事件
        /// </summary>
        public const string WorkStateChanged = "WorkStateChanged";

        /// <summary>
        /// 胶水液位变化普通事件
        /// </summary>
        public const string GlueLevelChanged = "GlueLevelChanged";

        /// <summary>
        /// 子机械模组能力方法定义信息列表
        /// 约定能力方法产生的能力事件，用ImeMethodDefInfo.GetFinishedEventID()产生
        /// </summary>

        /// <summary>
        /// 出胶
        /// </summary>
        public const string OutGlue = "OutGlue";

        /// <summary>
        /// 关胶
        /// </summary>
        public const string StopGlue = "StopGlue";

        /// <summary>
        /// 换胶
        /// </summary>
        public const string ChangeGlue = "ChangeGlue";

        /// <summary>
        /// 刷新胶量
        /// </summary>
        public const string RefreshGlueAmount = "RefreshGlueAmount";

        /// <summary>
        /// 读是否缺胶
        /// </summary>
        public const string ReadIsLackOfGlue = "ReadIsLackOfGlue";

        /// <summary>
        /// 读取胶水气压
        /// </summary>
        public const string GetGlueAirPressure = "GetGlueAirPressure";

        /// <summary>
        /// 设置胶水气压
        /// </summary>
        public const string SetGlueAirPressure = "SetGlueAirPressure";

        /// <summary>
        /// 读取胶阀气压
        /// </summary>
        public const string GetValveAirPressure = "GetValveAirPressure";

        /// <summary>
        /// 设置胶阀气压
        /// </summary>
        public const string SetValveAirPressure = "SetValveAirPressure";

        public static readonly ImeCompMethodDefInfoList Methods = new ImeCompMethodDefInfoList()
        {
            new ImeCompMethodDefInfo(StartActionMethodID,"开始工作",new GFParamDefInfoList(),new GFParamDefInfoList(),false),
            new ImeCompMethodDefInfo(StopActionMethodID,"停止工作",new GFParamDefInfoList(),new GFParamDefInfoList(),false),
        };

        /// <summary>
        /// 子机械模组能力事件定义信息列表
        /// 不是和能力方法匹配的其他能力事件
        /// </summary>
        public static readonly ImeCompEventDefInfoList Events = new ImeCompEventDefInfoList()
        {
            new ImeCompEventDefInfo(WorkStateChanged,"工作状态变化事件",new GFParamDefInfoList()),
        };

        public static readonly ImeCompMethodDefInfoList normalMethodDefInfos = new ImeCompMethodDefInfoList()
        {
            new ImeCompMethodDefInfo(CanStartAction, "可以开始工作", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
            new ImeCompMethodDefInfo(CanStopAction, "可以停止工作", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
            new ImeCompMethodDefInfo(OutGlue, "出胶", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
            new ImeCompMethodDefInfo(StopGlue, "关胶", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
            new ImeCompMethodDefInfo(ChangeGlue, "换胶", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
            new ImeCompMethodDefInfo(RefreshGlueAmount, "刷新胶量", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
            new ImeCompMethodDefInfo(ReadIsLackOfGlue, "读是否缺胶", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
            new ImeCompMethodDefInfo(GetGlueAirPressure, "读取胶水气压", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
            new ImeCompMethodDefInfo(SetGlueAirPressure, "设置胶水气压", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
            new ImeCompMethodDefInfo(GetValveAirPressure, "读取胶阀气压", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
            new ImeCompMethodDefInfo(SetValveAirPressure, "设置胶阀气压", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
        };
    }
}