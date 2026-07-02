using GKG.MotionControl;
using GKG.SubMM;
using GKG.SubMM.Dispenser;
using Griffins.ImeIOT;

namespace GKG
{
    namespace MM
    {
        public class DispenserMachineModulesConst
        {
            public const string MMName = "点胶机械模组";

            public const string MMNumberStr = "Dispenser";

            public static readonly MMNumber MMNumber = MMNumber.Parse(MMNumberStr);

            /// <summary>
            /// 开始能力方法ID(异步)
            /// </summary>
            public const string StartMethodID = "Start";

            /// <summary>
            /// 开始完成能力事件ID
            /// </summary>
            public static readonly string StartFinishedEventID = ImeMethodDefInfo.GetFinishedEventID(StartMethodID);

            /// <summary>
            /// 结束能力方法ID(异步)
            /// </summary>
            public const string EndMethodID = "End";

            /// <summary>
            /// 结束完成能力事件ID
            /// </summary>
            public static readonly string EndFinishedEventID = ImeMethodDefInfo.GetFinishedEventID(EndMethodID);

            /// <summary>
            /// 移动能力方法ID(异步)
            /// </summary>
            public const string MoveMethodID = "Move";

            /// <summary>
            /// 移动完成能力事件ID
            /// </summary>
            public static readonly string MoveFinishedEventID = ImeMethodDefInfo.GetFinishedEventID(MoveMethodID);

            /// <summary>
            /// 机械模组能力事件定义信息列表
            /// 不是和能力方法匹配的其他能力事件
            /// </summary>
            public static readonly ImeCabilityEventDefInfoList MMCabilityEventes = new ImeCabilityEventDefInfoList()
            {
            };

            /// <summary>
            /// 机械模组能力方法定义信息列表
            /// 约定能力方法产生的能力事件，ID保持一致
            /// </summary>
            public static readonly ImeCabilityMethodDefInfoList MMCabilityMethodes = new ImeCabilityMethodDefInfoList()
        {
            new ImeCabilityMethodDefInfo(StartMethodID,"开始",new ImeParamObjPropInfoList(),new ImeParamObjPropInfoList()),
            new ImeCabilityMethodDefInfo(EndMethodID,"结束",new ImeParamObjPropInfoList(),new ImeParamObjPropInfoList()),
            new ImeCabilityMethodDefInfo(MoveMethodID,"移动",new ImeParamObjPropInfoList(),new ImeParamObjPropInfoList()),
        };

            /// <summary>
            /// 机械手型号（龙门）
            /// </summary>
            public static readonly SubMMModel RobotSubMMModel = SubMMModel.Parse(CategoryARobotSubMachineModulesConst.SubMMModelStr);

            /// <summary>
            /// 机械手内部别名
            /// </summary>
            public static readonly InnerAlias Robot_Alias = InnerAlias.Parse("Robot01");

            /// <summary>
            /// 视觉型号
            /// </summary>
            public static readonly SubMMModel VisionSubMMModel = SubMMModel.Parse(VisionSubMachineModulesConst.SubMMModelStr);

            /// <summary>
            /// 视觉内部别名
            /// </summary>
            public static readonly InnerAlias Vision_Alias = InnerAlias.Parse("Vision01");

            /// <summary>
            /// 运动计算型号
            /// </summary>
            public static readonly SubMMModel MotionCalculateSubMMModel = SubMMModel.Parse(MotionCalculateSubMachineModulesConst.SubMMModelStr);

            /// <summary>
            /// 运动计算内部别名
            /// </summary>
            public static readonly InnerAlias MotionCalculate_Alias = InnerAlias.Parse("MotionCalculate01");

            /// <summary>
            /// 点胶阀1计算型号
            /// </summary>
            public static readonly SubMMModel Valve1SubMMModel = SubMMModel.Parse(DispensingFunctionHeadSubMachineModulesConst.SubMMModelStr);

            /// <summary>
            /// 点胶阀1内部别名
            /// </summary>
            public static readonly InnerAlias Valve1_Alias = InnerAlias.Parse("Valve01");

            /// <summary>
            /// 点胶阀2计算型号
            /// </summary>
            public static readonly SubMMModel Valve2SubMMModel = SubMMModel.Parse(DispensingFunctionHeadSubMachineModulesConst.SubMMModelStr);

            /// <summary>
            /// 点胶阀2内部别名
            /// </summary>
            public static readonly InnerAlias Valve2_Alias = InnerAlias.Parse("Valve02");
        }
    }
}