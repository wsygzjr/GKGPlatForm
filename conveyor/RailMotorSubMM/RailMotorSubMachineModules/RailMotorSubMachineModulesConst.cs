using Griffins;
using Griffins.ImeIOT;
using System;

namespace GKG
{
	namespace SubMM
	{
		public class RailMotorSubMachineModulesConst
		{
			public const string SubMMName = "轨道运输电机";

			public const string SubMMModelStr = "RailMotor";

			public readonly static SubMMModel SubMMModel = SubMMModel.Parse(SubMMModelStr);

			public readonly static SubMMObjInfoList SubMMObjInfos = new SubMMObjInfoList()
			{
				new SubMMObjInfo()
				{
					SubMMObjID = Guid.Parse("{7D7A868F-3C40-4AA5-A2C1-E6C072EEA3D1}"),
					SubMMObjName = SubMMName,
				},
			};

			/// <summary>
			/// 子机械模组能力方法定义信息列表
			/// 约定能力方法产生的能力事件，用ImeMethodDefInfo.GetFinishedEventID()产生
			/// </summary>
			public static readonly ImeCompMethodDefInfoList CompMethods = new ImeCompMethodDefInfoList()
			{

			};

			/// <summary>
			/// 子机械模组能力事件定义信息列表
			/// 不是和能力方法匹配的其他能力事件
			/// </summary>
			public static readonly ImeCompEventDefInfoList CompEvents = new ImeCompEventDefInfoList()
			{

			};
			/// <summary>
			/// 连续运动
			/// </summary>
			public const string ContinueMoveMethodID = "ContinueMove";
			/// <summary>
			/// 相对运动
			/// </summary>
			public const string RelativeMoveMethodID = "RelativeMove";
			/// <summary>
			/// 停止运动
			/// </summary>
			public const string StopMoveMethodID = "StopMove";

            /// <summary>
            /// 获取轴信息列表
            /// </summary>
            public const string RtCmdGetAxisOptions = "GetAxisOptions";

            public static readonly ImeCompMethodDefInfoList normalMethodDefInfos = new ImeCompMethodDefInfoList()
			{
				new ImeCompMethodDefInfo(ContinueMoveMethodID, "连续运动", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(RelativeMoveMethodID, "相对运动", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(StopMoveMethodID, "停止运动", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
            };
        }
    }
}
