using Griffins;
using Griffins.ImeIOT;
using System;

namespace GKG
{
	namespace SubMM
	{
		public class RailCommunicateSubMachineModulesConst
		{
			public const string SubMMName = "轨道上下位机通讯";

			public const string SubMMModelStr = "RailCommunicate";

			public readonly static SubMMModel SubMMModel = SubMMModel.Parse(SubMMModelStr);

			public readonly static SubMMObjInfoList SubMMObjInfos = new SubMMObjInfoList()
			{
				new SubMMObjInfo()
				{
					SubMMObjID = Guid.Parse("{72D36E13-B9E0-4961-8F8B-58B9D58B2F30}"),
					SubMMObjName = SubMMName,
				},
			};

			public static readonly ImeCompMethodDefInfoList CompMethods = new ImeCompMethodDefInfoList()
			{

			};

			public static readonly ImeCompEventDefInfoList CompEvents = new ImeCompEventDefInfoList()
			{

			};

            #region 方法ID
            /// <summary>
            /// 进板方法ID
            /// </summary>
            public const string InputPanelMethodID = "InputPanel";
			/// <summary>
			/// 进板完成方法ID
			/// </summary>
            public const string InputPanelSucceededMethodID = "InputPanelSucceeded";
            /// <summary>
            /// 出板方法ID
            /// </summary>
            public const string OutputPanelMethodID = "OutputPanel";
            /// <summary>
            /// 出板完成方法ID
            /// </summary>
            public const string OutputPanelSucceededMethodID = "OutputPanelSucceeded";
            #endregion

            #region 命令
            public const string GetUpperMachineStateMethodID = "GetUpperMachineState";
            public const string GetLowerMachineStateMethodID = "GetLowerMachineState";
            public const string SetMachineNeedPanelMethodID = "SetMachineNeedPanel";
            public const string SetMachineHasPanelMethodID = "SetMachineHasPanel";
			public const string GetIOInfosCtlCmdID = "GetIOInfos";
            #endregion

            public const string RequestPanelEventID = "RequestPanel";
			public const int RequestPanelEventKind = 30001;
            public const string ResponsePanelEventID = "ResponsePanel";
            public const int ResponsePanelEventKind = 30002;

            public static readonly ImeCompMethodDefInfoList normalMethodDefInfos = new ImeCompMethodDefInfoList()
			{
				new ImeCompMethodDefInfo(InputPanelMethodID, "进板要板", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(InputPanelSucceededMethodID, "进板成功关闭要板", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(OutputPanelMethodID, "出板有板", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(OutputPanelSucceededMethodID, "出板成功关闭有板", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
            };
        }
    }
}
