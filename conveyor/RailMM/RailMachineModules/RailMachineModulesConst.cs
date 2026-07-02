using GKG.SubMM;
using Griffins;
using Griffins.ImeIOT;

namespace GKG
{
	namespace MM
	{
		public class RailMachineModulesConst
		{
			public const string MMName = "单层轨道机械模组";

			public const string MMModelStr = "Rail";

			public readonly static MMNumber MMModel = MMNumber.Parse(MMModelStr);

            #region 能力方法ID
            // 进板
            public const string InletPanelMethodID = "InletPanel";
            // 进板到工作位
            public const string InletPanelToWorkingStationMethodID = "InletPanelToWorkingStation";
            // 出板
            public const string OutletPanelMethodID = "OutletPanel";
            // 出板到出板位
            public const string OutletPanelToOutStationMethodID = "OutletPanelToOutStation";
            #endregion

            #region 能力事件ID
            // 进板完成事件
            public const string InletPanelFinishedEventID = "InletPanelFinished";
            // 进板到工作位完成事件
            public const string InletPanelToWorkingStationFinishedEventID = "InletPanelToWorkingStationFinished";
            // 出板完成事件
            public const string OutletPanelFinishedEventID = "OutletPanelFinished";
            // 出板到出板位完成事件
            public const string OutletPanelToOutStationFinishedEventID = "OutletPanelToOutStationFinished";

            // 进板完成事件
            public const string InletPanelFailedEventID = "InletPanelFailed";
            // 进板到工作位完成事件
            public const string InletPanelToWorkingStationFailedEventID = "InletPanelToWorkingStationFailed";
            // 出板完成事件
            public const string OutletPanelFailedEventID = "OutletPanelFailed";
            // 出板到出板位完成事件
            public const string OutletPanelToOutStationFailedEventID = "OutletPanelToOutStationFailed";
            #endregion

            public const string GetIOInfosCtlCmdID = "GetIOInfos";

            /// <summary>
            /// 子机械模组能力方法定义信息列表
            /// 约定能力方法产生的能力事件，用ImeMethodDefInfo.GetFinishedEventID()产生
            /// </summary>
            public static readonly ImeCompMethodDefInfoList CompMethods = new ImeCompMethodDefInfoList()
			{
                new ImeCompMethodDefInfo(InletPanelMethodID, "进板", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(InletPanelToWorkingStationMethodID, "进板到工作位", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(OutletPanelMethodID, "出板", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(OutletPanelToOutStationMethodID, "出板到出板位", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
            };

			/// <summary>
			/// 子机械模组能力事件定义信息列表
			/// 不是和能力方法匹配的其他能力事件
			/// </summary>
			public static readonly ImeCompEventDefInfoList CompEvents = new ImeCompEventDefInfoList()
			{
				new ImeCompEventDefInfo(InletPanelFinishedEventID, "进板完成事件", new GFParamDefInfoList()),
				new ImeCompEventDefInfo(InletPanelToWorkingStationFinishedEventID, "进板到工作位完成事件", new GFParamDefInfoList()),
				new ImeCompEventDefInfo(OutletPanelFinishedEventID, "出板完成事件", new GFParamDefInfoList()),
				new ImeCompEventDefInfo(OutletPanelToOutStationFinishedEventID, "出板到出板位完成事件", new GFParamDefInfoList()),
            };

            public static readonly ImeCompMethodDefInfoList normalMethodDefInfos = new ImeCompMethodDefInfoList()
			{	
				new ImeCompMethodDefInfo(InletPanelMethodID, "进板", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
				new ImeCompMethodDefInfo(InletPanelToWorkingStationMethodID, "进板到工作位", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(OutletPanelMethodID, "出板", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(OutletPanelToOutStationMethodID, "出板到出板位", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
            };
            /// <summary>
            /// 轨道运输电机子机械模组
            /// </summary>
			public readonly static SubMMModel SubMMModelRailMotor = SubMMModel.Parse(RailMotorSubMachineModulesConst.SubMMModelStr);
			public readonly static InnerAlias InnerAliasRailMotor = InnerAlias.Parse("RailMotor1");

            /// <summary>
            /// 左工位子机械模组
            /// </summary>
			public readonly static SubMMModel SubMMModelLeftWorkStation = SubMMModel.Parse(RailWorkStationSubMachineModulesConst.SubMMModelStr);
			public readonly static InnerAlias InnerAliasLeftWorkStation = InnerAlias.Parse("LeftWorkStation");

            /// <summary>
            /// 中间工位子机械模组
            /// </summary>
			public readonly static SubMMModel SubMMModelMiddleWorkStation = SubMMModel.Parse(RailWorkStationSubMachineModulesConst.SubMMModelStr);
			public readonly static InnerAlias InnerAliasMiddleWorkStation = InnerAlias.Parse("MiddleWorkStation");

            /// <summary>
            /// 右工位子机械模组
            /// </summary>
            public readonly static SubMMModel SubMMModelRightWorkStation = SubMMModel.Parse(RailWorkStationSubMachineModulesConst.SubMMModelStr);
			public readonly static InnerAlias InnerAliasRightWorkStation = InnerAlias.Parse("RightWorkStation");
        }
    }
}
