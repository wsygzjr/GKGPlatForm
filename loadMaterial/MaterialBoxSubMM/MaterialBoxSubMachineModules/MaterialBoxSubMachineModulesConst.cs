using Griffins;
using Griffins.ImeIOT;

namespace GKG
{
	namespace SubMM
	{
		/// <summary>料盒子模组对外协议常量定义。</summary>
		public class MaterialBoxSubMachineModulesConst
		{
			/// <summary>料盒子模组在宿主中的显示名称。</summary>
			public const string SubMMName = "料盒子机械模组";

			/// <summary>料盒子模组模型编号；用于插件注册与总模块内部引用。</summary>
			public const string SubMMModelStr = "MaterialBox";

			public readonly static SubMMModel SubMMModel = SubMMModel.Parse(SubMMModelStr);

			/// <summary>
			/// 料盒子机械模组自身的能力方法定义信息列表。
			/// 储料装置对象和运输机构对象只保留普通方法，不再在这里声明能力方法。
			/// </summary>
			public static readonly ImeCompMethodDefInfoList CompMethods = new ImeCompMethodDefInfoList()
			{
				new ImeCompMethodDefInfo(UpfeedMoveNextSlotMethodID, "上料移动到下一料槽", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(DownfeedMoveNextSlotMethodID, "下料移动到下一料槽", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
			};

            /// <summary>
            /// 料盒子机械模组对外普通方法定义。
            /// 储料装置对象和运输机构对象的方法全部归类到普通方法，包括原先按能力方法使用的接口。
            /// </summary>
            public static readonly ImeCompMethodDefInfoList normalMethodDefInfos = new ImeCompMethodDefInfoList()
            {
                new ImeCompMethodDefInfo(ControlCylinderMethodID, "控制储料位气缸", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(StorageResetMethodID, "储料装置复位", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(StorageResetStateMethodID, "料盒状态复位", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(StorageOpenMethodID, "料盒张开", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(StorageCloseMethodID, "料盒夹紧", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(StorageGetCountMethodID, "获取料盒数量", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(StorageGetCurrentSlotStateMethodID, "获取当前槽位状态", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(StorageGetAvailableSlotsMethodID, "获取可用槽位列表", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(StorageGetSlotMaterialStateMethodID, "获取指定槽位有料状态", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(StorageSetSlotMaterialStateMethodID, "设置指定槽位有料状态", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(StorageGetInitialPositionMethodID, "获取料盒初始位置", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(TransportMoveMethodID, "运输机构移动", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(TransportGetCurrentPositionMethodID, "获取运输当前位置", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(TransportGetMotionParametersMethodID, "获取运输运动参数", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(GetMaterialStateMethodID, "获取送料口/接料口有料状态", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(GetMaterialContainerStatusMethodID, "获取全部料盒容器状态", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
            };

			/// <summary>额外能力事件定义；当前料盒模组主要通过返回值携带事件号，这里暂未单独注册。</summary>
			public static readonly ImeCompEventDefInfoList CompEvents = new ImeCompEventDefInfoList()
			{
				new ImeCompEventDefInfo(EventUpfeedMoveCompleted, "上料料盒移动到下一槽完成", new GFParamDefInfoList()),
				new ImeCompEventDefInfo(EventUpfeedMoveFailed, "上料料盒移动到下一槽失败", new GFParamDefInfoList()),
				new ImeCompEventDefInfo(EventUpfeedCassetteEmpty, "上料料盒为空", new GFParamDefInfoList()),
				new ImeCompEventDefInfo(EventUpfeedNoCassette, "上料位无料盒", new GFParamDefInfoList()),
				new ImeCompEventDefInfo(EventUpfeedMaterialInsufficientWarning, "上料余料不足预警", new GFParamDefInfoList()),
				new ImeCompEventDefInfo(EventDownfeedMoveCompleted, "下料料盒移动到下一槽完成", new GFParamDefInfoList()),
				new ImeCompEventDefInfo(EventDownfeedMoveFailed, "下料料盒移动到下一槽失败", new GFParamDefInfoList()),
				new ImeCompEventDefInfo(EventDownfeedCassetteFull, "下料料盒已满", new GFParamDefInfoList()),
				new ImeCompEventDefInfo(EventDownfeedNoCassette, "下料位无料盒", new GFParamDefInfoList()),
				new ImeCompEventDefInfo(EventDownfeedSlotInsufficientWarning, "下料剩余槽位不足预警", new GFParamDefInfoList()),
				new ImeCompEventDefInfo(EventLoadStorageMaterialWarning, "上料储料装置余料预警", new GFParamDefInfoList()),
				new ImeCompEventDefInfo(EventLoadStorageMaterialEmpty, "上料储料装置缺料", new GFParamDefInfoList()),
				new ImeCompEventDefInfo(EventLoadStorageStretchFinished, "上料储料装置气缸伸出完成", new GFParamDefInfoList()),
				new ImeCompEventDefInfo(EventLoadStorageRetractFinished, "上料储料装置气缸缩回完成", new GFParamDefInfoList()),
				new ImeCompEventDefInfo(EventUnloadStorageMaterialWarning, "下料储料装置余料预警", new GFParamDefInfoList()),
				new ImeCompEventDefInfo(EventUnloadStorageMaterialEmpty, "下料储料装置缺料", new GFParamDefInfoList()),
				new ImeCompEventDefInfo(EventUnloadStorageStretchFinished, "下料储料装置气缸伸出完成", new GFParamDefInfoList()),
				new ImeCompEventDefInfo(EventUnloadStorageRetractFinished, "下料储料装置气缸缩回完成", new GFParamDefInfoList()),
				new ImeCompEventDefInfo(EventLoadTransportPositionChanged, "上料运输机构位置变化", new GFParamDefInfoList()),
				new ImeCompEventDefInfo(EventLoadTransportMoveFinished, "上料运输机构移动完成", new GFParamDefInfoList()),
				new ImeCompEventDefInfo(EventUnloadTransportPositionChanged, "下料运输机构位置变化", new GFParamDefInfoList()),
				new ImeCompEventDefInfo(EventUnloadTransportMoveFinished, "下料运输机构移动完成", new GFParamDefInfoList()),
			};

            public const string LoadAxisInformType = "Load";
            public const string UnloadAxisInformType = "Unload";
            public const string MaterialContainerName = "MaterialContainer";
            public const string LoadStorageDeviceName = "LoadStorageDevice";
            public const string UnloadStorageDeviceName = "UnloadStorageDevice";
            public const string UpperRackName = "UpperRack";
            public const string LowerRackName = "LowerRack";

            /// <summary>
            /// 获取送料口/接料口当前有料状态的方法 ID。
            /// </summary>
            public const string GetMaterialStateMethodID = "GetFeedPortMaterialState";
            /// <summary>获取全部料盒容器状态。</summary>
            public const string GetMaterialContainerStatusMethodID = "GetMaterialContainerStatus";
            /// <summary>执行上料料盒切换到下一槽位。</summary>
            public const string UpfeedMoveNextSlotMethodID = "UpfeedMoveNextSlot";
			/// <summary>执行下料料盒切换到下一槽位。</summary>
			public const string DownfeedMoveNextSlotMethodID = "DownfeedMoveNextSlot";
            /// <summary>执行上料料盒切换到下一槽位。</summary>
            public const string UpfeedMoveCurrentSlotMethodID = "UpfeedMoveCurrentSlot";
            /// <summary>执行下料料盒切换到下一槽位。</summary>
            public const string DownfeedMoveCurrentSlotMethodID = "DownfeedMoveCurrentSlot";
            /// <summary>普通方法：控制指定料盒位的夹紧气缸动作。</summary>
            public const string ControlCylinderMethodID = "ControlCylinder";
			/// <summary>能力方法：获取料盒子模组初始化参数。</summary>
			public const string GetInitParametersMethodID = "GetInitParameters";
			/// <summary>能力方法：获取料盒子模组配方参数。</summary>
			public const string GetRecipeParametersMethodID = "GetRecipeParameters";
            /// <summary>能力方法：设置当前料盒状态。</summary>
            public const string SetCurrentSlotStatusMethodID = "SetCurrentSlotStatus";
            public const string UpdateMaterialBoxStateMethodID = "UpdateMaterialBoxState";


            /// <summary>普通方法：执行指定储料位复位动作。</summary>
            public const string StorageResetMethodID = "StorageReset";
            /// <summary>普通方法：执行指定储料位状态复位。</summary>
            public const string StorageResetStateMethodID = "StorageResetState";
            /// <summary>普通方法：执行指定储料位料盒张开。</summary>
            public const string StorageOpenMethodID = "StorageOpen";
            /// <summary>普通方法：执行指定储料位料盒夹紧。</summary>
            public const string StorageCloseMethodID = "StorageClose";
            /// <summary>普通方法：获取储料装置当前料盒数量。</summary>
            public const string StorageGetCountMethodID = "StorageGetCount";
            /// <summary>普通方法：获取指定储料位当前槽位状态。</summary>
            public const string StorageGetCurrentSlotStateMethodID = "StorageGetCurrentSlotState";
            /// <summary>普通方法：获取指定储料位可用槽位列表。</summary>
            public const string StorageGetAvailableSlotsMethodID = "StorageGetAvailableSlots";
            /// <summary>普通方法：获取指定槽位当前有料状态。</summary>
            public const string StorageGetSlotMaterialStateMethodID = "StorageGetSlotMaterialState";
            /// <summary>普通方法：设置指定槽位当前有料状态。</summary>
            public const string StorageSetSlotMaterialStateMethodID = "StorageSetSlotMaterialState";
            /// <summary>普通方法：获取指定储料位初始位置。</summary>
            public const string StorageGetInitialPositionMethodID = "StorageGetInitialPosition";
            /// <summary>普通方法：执行运输机构移动。</summary>
            public const string TransportMoveMethodID = "TransportMove";
            /// <summary>普通方法：获取当前运输机构位置。</summary>
            public const string TransportGetCurrentPositionMethodID = "TransportGetCurrentPosition";
            /// <summary>普通方法：获取当前运输机构运动参数。</summary>
            public const string TransportGetMotionParametersMethodID = "TransportGetMotionParameters";

            /// <summary>上料料盒成功移动到下一槽。</summary>
            public const string EventUpfeedMoveCompleted = "UpfeedMoveCompleted";
            /// <summary>上料料盒移动到下一槽失败。</summary>
            public const string EventUpfeedMoveFailed = "UpfeedMoveFailed";
            /// <summary>上料料盒已空，无可继续上料槽位。</summary>
            public const string EventUpfeedCassetteEmpty = "UpfeedCassetteEmpty";
            /// <summary>上料位未检测到料盒。</summary>
            public const string EventUpfeedNoCassette = "UpfeedNoCassette";
            /// <summary>上料剩余物料不足，触发预警。</summary>
            public const string EventUpfeedMaterialInsufficientWarning = "UpfeedMaterialInsufficientWarning";
            /// <summary>下料料盒成功移动到下一槽。</summary>
            public const string EventDownfeedMoveCompleted = "DownfeedMoveCompleted";
            /// <summary>下料料盒移动到下一槽失败。</summary>
            public const string EventDownfeedMoveFailed = "DownfeedMoveFailed";
            /// <summary>下料料盒已满，无可继续下料槽位。</summary>
            public const string EventDownfeedCassetteFull = "DownfeedCassetteFull";
            /// <summary>下料位未检测到料盒。</summary>
            public const string EventDownfeedNoCassette = "DownfeedNoCassette";
            /// <summary>下料剩余槽位不足，触发预警。</summary>
            public const string EventDownfeedSlotInsufficientWarning = "DownfeedSlotInsufficientWarning";
            /// <summary>上料储料装置余料预警。</summary>
            public const string EventLoadStorageMaterialWarning = "LoadStorageMaterialWarning";
            /// <summary>上料储料装置缺料。</summary>
            public const string EventLoadStorageMaterialEmpty = "LoadStorageMaterialEmpty";
            /// <summary>上料储料装置气缸伸出完成。</summary>
            public const string EventLoadStorageStretchFinished = "LoadStorageStretchFinished";
            /// <summary>上料储料装置气缸缩回完成。</summary>
            public const string EventLoadStorageRetractFinished = "LoadStorageRetractFinished";
            /// <summary>下料储料装置余料预警。</summary>
            public const string EventUnloadStorageMaterialWarning = "UnloadStorageMaterialWarning";
            /// <summary>下料储料装置缺料。</summary>
            public const string EventUnloadStorageMaterialEmpty = "UnloadStorageMaterialEmpty";
            /// <summary>下料储料装置气缸伸出完成。</summary>
            public const string EventUnloadStorageStretchFinished = "UnloadStorageStretchFinished";
            /// <summary>下料储料装置气缸缩回完成。</summary>
            public const string EventUnloadStorageRetractFinished = "UnloadStorageRetractFinished";
            /// <summary>上料运输机构位置变化。</summary>
            public const string EventLoadTransportPositionChanged = "LoadTransportPositionChanged";
            /// <summary>上料运输机构移动完成。</summary>
            public const string EventLoadTransportMoveFinished = "LoadTransportMoveFinished";
            /// <summary>下料运输机构位置变化。</summary>
            public const string EventUnloadTransportPositionChanged = "UnloadTransportPositionChanged";
            /// <summary>下料运输机构移动完成。</summary>
            public const string EventUnloadTransportMoveFinished = "UnloadTransportMoveFinished";

			/// <summary>移动到首槽位置。</summary>
			public const string RtCmdMoveToFirstSlot = "RtCmdMoveToFirstSlot";
			/// <summary>移动到末槽位置。</summary>
			public const string RtCmdMoveToLastSlot = "RtCmdMoveToLastSlot";
			/// <summary>移动到指定槽位或指定绝对位置。</summary>
			public const string RtCmdMoveTo = "MoveTo";
			/// <summary>移动到下一个可用槽位。</summary>
			public const string RtCmdMoveToNextSlot = "MoveToNextSlot";
			/// <summary>以连续运动方式驱动运输轴；协议中沿用历史名称 MagazineMotion。</summary>
			public const string RtCmdMagazineMotion = "MagazineMotion";
			/// <summary>运输轴向上移动；Step 大于 0 时执行一次相对位移，否则连续向上运动；协议中沿用历史名称 ZMoveUp。</summary>
			public const string RtCmdZMoveUp = "ZMoveUp";
			/// <summary>运输轴向下移动；Step 大于 0 时执行一次相对位移，否则连续向下运动；协议中沿用历史名称 ZMoveDown。</summary>
			public const string RtCmdZMoveDown = "ZMoveDown";
			/// <summary>立即停止运输轴当前连续运动；协议中沿用历史名称 ZAxisStop。</summary>
			public const string RtCmdZAxisStop = "ZAxisStop";
			/// <summary>夹紧指定储料位料盒；协议中沿用历史名称 MagazineClamp。</summary>
			public const string RtCmdMagazineClamp = "MagazineClamp";
			/// <summary>松开指定储料位料盒；协议中沿用历史名称 MagazineUnclamp。</summary>
			public const string RtCmdMagazineUnclamp = "MagazineUnclamp";
			/// <summary>读取电器管理返回的轴信息列表。</summary>
			public const string RtCmdGetAxisInfos = "GetAxisInfos";
			/// <summary>读取储料装置当前绑定的 IO 信息列表。</summary>
			public const string RtCmdGetIOInfos = "GetIOInfos";
			/// <summary>读取当前运输轴位置；协议中沿用历史名称 GetAxisPos。</summary>
			public const string RtCmdGetAxisPos = "GetAxisPos";
            /// <summary>按当前配方恢复指定料盒位的逻辑状态。</summary>
            public const string RtCmdResetMaterialBoxState = "ResetMaterialBoxState";
            /// <summary>控制指定料盒位回到初始位置。</summary>
            public const string RtCmdMoveToInitialPosition = "MoveToInitialPosition";

        }
    }
}
