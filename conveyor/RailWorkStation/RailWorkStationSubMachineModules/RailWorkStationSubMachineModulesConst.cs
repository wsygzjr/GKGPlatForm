using Griffins;
using Griffins.ImeIOT;

namespace GKG
{
	namespace SubMM
	{
		public class RailWorkStationSubMachineModulesConst
		{
			public const string SubMMName = "单层轨道工位对象";

			public const string SubMMModelStr = "RailWorkStation";

			public readonly static SubMMModel SubMMModel = SubMMModel.Parse(SubMMModelStr);
			public readonly static SubMMObjInfoList SubMMObjInfos = new SubMMObjInfoList()
			{
				new SubMMObjInfo()
				{
					SubMMObjID = Guid.Parse("{14A36FB5-F782-4239-952B-9ECEB75A3CA8}"),
					SubMMObjName = "单层轨道工位对象",
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

			public const string GetLeftSensorStateMethodID = "GetLeftSensorState";

			public const string GetRightSensorStateMethodID = "GetRightSensorState";

			public const string ControlLeftCylinderMethodID = "ControlLeftCylinder";

            public const string ControlRightCylinderMethodID = "ControlRightCylinder";

			public const string GetProximitySensorStateMethodID = "GetProximitySensorState";

			public const string GetTransportSpeedMethodID = "GetTransportSpeed";

			public const string SetWorkModeMethodID = "SetWorkMode";


            public const string GetSpeedListCtlCmdID = "GetSpeedList";
			public const string GetIOInfosCtlCmdID = "GetIOInfos";
			public const string GetFactoryParamsCmdID = "GetFactoryParams";

            #region 界面数据对象
            /// <summary>
            /// 工位状态
            /// </summary>
            public const string WorkStationStatusDataObjID = "WorkStationStatus";
            public const string WorkStationStatusDataObjName = "工位状态信息";
            public const string UICommandMemoControlLeftCylinder = "左挡板气缸操作";
            public const string UICommandMemoControlRightCylinder = "右挡板气缸操作";
            public const string ParamNameLeftCylinderStretch = "左挡板伸出";
            public const string ParamNameLeftCylinderRetract = "左挡板缩回";
            public const string ParamNameRightCylinderStretch = "右挡板伸出";
            public const string ParamNameRightCylinderRetract = "右挡板缩回";

			/// <summary>
			/// 左气缸伸出
			/// </summary>
            public const string LeftCylinderStretch = "LeftCylinderStretch";
            /// <summary>
            /// 左气缸缩回
            /// </summary>
            public const string LeftCylinderRetract = "LeftCylinderRetract";
			/// <summary>
			/// 右气缸伸出
			/// </summary>
            public const string RightCylinderStretch = "RightCylinderStretch";
            /// <summary>
            /// 右气缸缩回
            /// </summary>
            public const string RightCylinderRetract = "RightCylinderRetract";

            #endregion

            public const string RetParamSensorStatus = "SensorStatus";
            public const string CmdParamCylinder = "Cylinder";
            public const string CmdParamWorkMode = "WorkMode";
            public const string RetParamTransportAcceleration = "TransportAcceleration";
            public const string RetParamTransportSpeed = "TransportSpeed";
            public const string RetParamResult = "Result";

            public static readonly ImeCompMethodDefInfoList normalMethodDefInfos = new ImeCompMethodDefInfoList()
			{
				new ImeCompMethodDefInfo(GetLeftSensorStateMethodID, "获取左感应器状态", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(GetRightSensorStateMethodID, "获取右感应器状态", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(ControlLeftCylinderMethodID, "控制左气缸", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(ControlRightCylinderMethodID, "控制右气缸", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(GetProximitySensorStateMethodID, "获取接近感应器状态", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(GetTransportSpeedMethodID, "获取运输速度", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(SetWorkModeMethodID, "设置工作模式", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
            };
        }
    }
}
