using Griffins;
using Griffins.ImeIOT;

namespace GKG
{
	namespace SubMM
	{
		public class EletronicManagerSubMachineModulesConst
		{
			public const string SubMMName = "电气管理对象组件";

			public const string SubMMModelStr = "MotionControlCardCfg";

			public readonly static SubMMModel SubMMModel = SubMMModel.Parse(SubMMModelStr);

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

			public const string ControlCylinderMethodID = "ControlCylinder";

			public const string CheckMaterialBoxMethodID = "CheckMaterialBox";

			public const string MoveZMethodID = "MoveZ";

			public const string HomedMethodID = "Homed";

			#region 轴调试运行时命令（AxisDebugWindowViewModel Move/Stop/Enable/Disable/ClearStatus/Home/Jog）

			/// <summary>绝对定位运动（MoveCommand）。</summary>
			public const string RtCmdAbsoluteMove = "AbsoluteMove";

			/// <summary>轴停止（StopCommand、JogStop）。</summary>
			public const string RtCmdAxisStop = "AxisStop";

			/// <summary>伺服使能开关（EnableCommand / DisableCommand，参数 IsEnabled/On）。</summary>
			public const string RtCmdServoOn = "ServoOn";

			/// <summary>清除轴报警（ClearStatusCommand）。</summary>
			public const string RtCmdClearAxisAlarm = "ClearAxisAlarm";

			/// <summary>轴回零（HomeCommand；路由键与 HomedMethodID 相同）。</summary>
			public const string RtCmdHomed = HomedMethodID;

			/// <summary>点动/连续运动（JogToggleCommand）。</summary>
			public const string RtCmdJogMove = "JogMove";

			/// <summary>相对定位运动。</summary>
			public const string RtCmdRelativeMove = "RelativeMove";

			#endregion

			#region IO 运行时命令

			/// <summary>设置输出口状态（AxisDebug IO 调试）。</summary>
			public const string RtCmdSetOutputState = "SetOutputState";

			/// <summary>读取输入/输出口状态。</summary>
			public const string RtCmdGetInOutputState = "GetInOutputState";

			/// <summary>读取输入口状态。</summary>
			public const string RtCmdGetInputState = "GetInputState";

			/// <summary>读取输出口状态。</summary>
			public const string RtCmdGetOutputState = "GetOutputState";

			/// <summary>读取 IO 状态（按 IOGuid）。</summary>
			public const string RtCmdGetIOState = "GetIOState";

			/// <summary>锁定 IO。</summary>
			public const string RtCmdLockIO = "LockIO";

			/// <summary>解锁 IO。</summary>
			public const string RtCmdUnLockIO = "UnLockIO";

			/// <summary>读取输入模拟量。</summary>
			public const string RtCmdGetInputNum = "GetInputNum";

			/// <summary>读取输出模拟量。</summary>
			public const string RtCmdGetOutputNum = "GetOutputNum";

			/// <summary>读取输入/输出模拟量。</summary>
			public const string RtCmdGetInOutputNum = "GetInOutputNum";

			/// <summary>设置输入/输出模拟量。</summary>
			public const string RtCmdSetInOutputNum = "SetInOutputNum";

			#endregion

			#region 轴状态/位置查询运行时命令

			/// <summary>读取轴当前位置（refreshSingleAxisPosition）。</summary>
			public const string RtCmdGetAxisPos = "GetAxisPos";

			/// <summary>读取轴状态位（限位/报警/使能等）。</summary>
			public const string RtCmdGetAxisState = "GetAxisState";

			/// <summary>一次性读取轴五态：正限位/原点/负限位/报警/使能。</summary>
			public const string RtCmdGetAxisStates = "GetAxisStates";

			/// <summary>设置轴当前位置。</summary>
			public const string RtCmdSetAxisPos = "SetAxisPos";

			/// <summary>读取轴锁定状态。</summary>
			public const string RtCmdGetAxisLockState = "GetAxisLockState";

			/// <summary>等待轴停止。</summary>
			public const string RtCmdWaitAxisStop = "WaitAxisStop";

			#endregion

			#region 运控卡与配置运行时命令

			/// <summary>读取电气工厂配置 JSON。</summary>
			public const string RtCmdGetElectricalFactoryCfg = "GetElectricalFactoryCfg";

			/// <summary>保存电气工厂配置 JSON。</summary>
			public const string RtCmdSaveElectricalFactoryCfg = "SaveElectricalFactoryCfg";

			/// <summary>读取轴配置列表 JSON（AxisDebug 初始化）。</summary>
			public const string RtCmdGetAxisConfigList = "GetAxisConfigList";

			/// <summary>读取 IO 配置列表 JSON。</summary>
			public const string RtCmdGetIoConfigList = "GetIoConfigList";

			#endregion

            public static readonly ImeCompMethodDefInfoList normalMethodDefInfos = new ImeCompMethodDefInfoList()
			{
				new ImeCompMethodDefInfo(ControlCylinderMethodID, "控制料盒气缸", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(CheckMaterialBoxMethodID, "检测料盒状态", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(MoveZMethodID, "Z轴移动", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(HomedMethodID, "Z轴归零", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
            };
        }
    }
}
