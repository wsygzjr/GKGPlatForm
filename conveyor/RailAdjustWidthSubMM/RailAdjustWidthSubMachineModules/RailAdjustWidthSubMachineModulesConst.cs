using Griffins;
using Griffins.ImeIOT;
using System;

namespace GKG
{
	namespace SubMM
	{
		public class RailAdjustWidthSubMachineModulesConst
		{
			public const string SubMMName = "轨道调宽子机械模组";

			public const string SubMMModelStr = "RailAdjustWidth";

			public readonly static SubMMModel SubMMModel = SubMMModel.Parse(SubMMModelStr);

			public readonly static SubMMObjInfoList SubMMObjInfos = new SubMMObjInfoList()
			{
				new SubMMObjInfo()
				{
					SubMMObjID = Guid.Parse("{A1D1D7C1-3C31-4D58-B1C1-9A6E9F2E4C31}"),
					SubMMObjName = SubMMName,
				},
			};

			public static readonly ImeCompMethodDefInfoList CompMethods = new ImeCompMethodDefInfoList()
			{

			};

			public static readonly ImeCompEventDefInfoList CompEvents = new ImeCompEventDefInfoList()
			{

			};

			/// <summary>
			/// 调宽
			/// </summary>
			public const string RailAdjustWidthMethodID = "RailAdjustWidth";
			/// <summary>
			/// 移动
			/// </summary>
			public const string RailContinueMoveMethodID = "RailContinueMove";


			public const string FrontRailPosition = "FrontRailPosition";
			public const string BackRailPosition = "BackRailPosition";

			public const string RtCmdGetAxisOptions = "GetAxisOptions";
			public const string RtCmdGetFactoryParams = "GetFactoryParams";
			/// <summary>
			/// 移动到
			/// </summary>
			public const string RtCmdMoveTo = "MoveTo";
			/// <summary>
			/// 停止
			/// </summary>
			public const string RtCmdStop = "Stop";
			/// <summary>
			/// 回原
			/// </summary>
			public const string RtGoHome = "GoHome";
            public static readonly ImeCompMethodDefInfoList normalMethodDefInfos = new ImeCompMethodDefInfoList()
			{
				new ImeCompMethodDefInfo(RailAdjustWidthMethodID, "轨道调宽", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(RailContinueMoveMethodID, "轨道平移", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
            };
        }
    }
}
