using GF_Gereric;
using Griffins;
using Griffins.ImeIOT;
using Griffins.IOT;
using Griffins.UI;
using GriffinsGeneralTestMM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSubMM_MaterialBox
{
	public static class GenSubMMInfoDefine
	{
		public const string SubMMName = "测试料盒子机械模组";
		public const string SubMMModelStr = "Test_MaterialBox";
		public readonly static SubMMModel SubMMModel = SubMMModel.Parse(SubMMModelStr);

        public static readonly Guid SubMMObjID = Guid.Parse("{51D1E427-CAD0-4AEF-B867-F442C20D9F7B}");
        public const string SubMMObjName = "N层M排料盒物料容器对象";
        static GenSubMMInfoDefine()
		{
			createGenSubMMInfo();
			createGenSubMMDesignTimeInfo();
		}

		#region 子机械模组信息
		private static GenSubMMInfo genSubMMInfo;
		/// <summary>
		/// 子机械模组信息
		/// </summary>
		public static GenSubMMInfo GenSubMMInfo
		{
			get { return genSubMMInfo; }
		}

		private static void createGenSubMMInfo()
		{
			genSubMMInfo = new GenSubMMInfo();
			// 子机械模组名称
			genSubMMInfo.SubMMName = SubMMName;

			//  子机械模组的子机械模组能力事件列表
			fillCabilityEvents();
			//  子机械模组的子机械模组能力方法列表
			fillCabilityMethods();
			//  子机械模组的子机械模组普通事件列表
			fillNormalEvents();
			// 子机械模组的子机械模组普通方法列表
			fillNormalMethods();
            //  子机械模组界面数据对象属性定义信息列表
            fillUIDataObjProps();
            //子机械模组实现对象列表
            fillSubMMObjInfos();
		}

		#region  子机械模组的子机械模组能力事件列表
		private static void fillCabilityEvents()
		{

			genSubMMInfo.CabilityEvents = new GenCabilityEventDefInfoList()
			{
               new GenCabilityEventDefInfo(EventUpfeedMoveCompleted, "上料料盒移动到下一槽完成"),
                new GenCabilityEventDefInfo(EventUpfeedMoveFailed, "上料料盒移动到下一槽失败"),
                new GenCabilityEventDefInfo(EventUpfeedCassetteEmpty, "上料料盒为空"),
                new GenCabilityEventDefInfo(EventUpfeedNoCassette, "上料位无料盒"),
                new GenCabilityEventDefInfo(EventUpfeedMaterialInsufficientWarning, "上料余料不足预警"),
                new GenCabilityEventDefInfo(EventDownfeedMoveCompleted, "下料料盒移动到下一槽完成"),
                new GenCabilityEventDefInfo(EventDownfeedMoveFailed, "下料料盒移动到下一槽失败"),
                new GenCabilityEventDefInfo(EventDownfeedCassetteFull, "下料料盒已满"),
                new GenCabilityEventDefInfo(EventDownfeedNoCassette, "下料位无料盒"),
                new GenCabilityEventDefInfo(EventDownfeedSlotInsufficientWarning, "下料剩余槽位不足预警"),
                new GenCabilityEventDefInfo(EventLoadStorageMaterialWarning, "上料储料装置余料预警"),
                new GenCabilityEventDefInfo(EventLoadStorageMaterialEmpty, "上料储料装置缺料"),
                new GenCabilityEventDefInfo(EventLoadStorageStretchFinished, "上料储料装置气缸伸出完成"),
                new GenCabilityEventDefInfo(EventLoadStorageRetractFinished, "上料储料装置气缸缩回完成"),
                new GenCabilityEventDefInfo(EventUnloadStorageMaterialWarning, "下料储料装置余料预警"),
                new GenCabilityEventDefInfo(EventUnloadStorageMaterialEmpty, "下料储料装置缺料"),
                new GenCabilityEventDefInfo(EventUnloadStorageStretchFinished, "下料储料装置气缸伸出完成"),
                new GenCabilityEventDefInfo(EventUnloadStorageRetractFinished, "下料储料装置气缸缩回完成"),
                new GenCabilityEventDefInfo(EventLoadTransportPositionChanged, "上料运输机构位置变化"),
                new GenCabilityEventDefInfo(EventLoadTransportMoveFinished, "上料运输机构移动完成"),
                new GenCabilityEventDefInfo(EventUnloadTransportPositionChanged, "下料运输机构位置变化"),
                new GenCabilityEventDefInfo(EventUnloadTransportMoveFinished, "下料运输机构移动完成"),
            };
		}

		#endregion

		#region  子机械模组的子机械模组能力方法列表

        private static void fillCabilityMethods()
		{
			genSubMMInfo.CabilityMethods = new GenCabilityMethodDefInfoList()
			{
                new GenCabilityMethodDefInfo(UpfeedMoveNextSlotMethodID, "上料移动到下一料槽",1000,null,null,null),
                new GenCabilityMethodDefInfo(DownfeedMoveNextSlotMethodID, "下料移动到下一料槽",1000,null,null,null),
            };
		}

		#endregion

		#region  子机械模组的子机械模组普通事件列表
		private static void fillNormalEvents()
		{
			genSubMMInfo.NormalEvents = new GenNormalEventDefInfoList()
			{
			};
		}

		#endregion

		#region  子机械模组的子机械模组普通方法列表
		private static void fillNormalMethods()
		{
			
		}

        #endregion

        #region  子机械模组界面数据对象属性定义信息列表
        private static void fillUIDataObjProps()
		{
          
        }

		#endregion

		#region 子机械模组实现对象列表

		private static void fillSubMMObjInfos()
		{
			genSubMMInfo.SubMMObjInfos = new SubMMObjInfoList()
			{
				new SubMMObjInfo()
				{
					SubMMObjID=SubMMObjID,
					SubMMObjName=SubMMObjName
                },
            };
        }

        #endregion


        #endregion

        #region 子机械模组设计时信息

        /// <summary>
        /// 实现对象对应的设计时信息
        /// </summary>
        public static Dictionary<Guid, GenSubMMDesignTimeInfo> GenSubMMDesignTimeInfoDic { set; get; } = new();
         
		private static void createGenSubMMDesignTimeInfo()
		{
			createFactoryCfgObjDefInfo();
			createInitCfgObjDefInfo();
			createPPCfgObjDefInfo();
            var genSubMMDesignTimeInfoOfMotor = new GenSubMMDesignTimeInfo(FactoryCfgObjDefInfo, InitCfgObjDefInfo, PPCfgObjDefInfo);
            GenSubMMDesignTimeInfoDic.Add(SubMMObjID, genSubMMDesignTimeInfoOfMotor); 
		}

		#region 出厂配置参数
		private static GenCfgViewParamObjectDefInfoList factoryCfgObjDefInfo;
		/// <summary>
		/// 出厂配置参数对象定义信息
		/// </summary>
		public static GenCfgViewParamObjectDefInfoList FactoryCfgObjDefInfo
		{
			get { return factoryCfgObjDefInfo; }
		}
		private static void createFactoryCfgObjDefInfo()
		{
            factoryCfgObjDefInfo = new GenCfgViewParamObjectDefInfoList();
            factoryCfgObjDefInfo.Add(new GenCfgViewParamObjectDefInfo()
			{
				ViewID = "FactoryCfgView",
                ViewName = "出厂配置界面",
                LabelWidth = 150,
                ParamInfoes = new GFParamDefInfoList()
                {
                    new GFParamDefInfo("StorageCount","储料机构个数",GriffinsBaseDataType.Integer), 
                },
            });
		}

		#endregion

		#region 初始化配置参数
		private static GenCfgViewParamObjectDefInfoList initCfgObjDefInfo;
		/// <summary>
		/// 初始化参数对象定义信息
		/// </summary>
		public static GenCfgViewParamObjectDefInfoList InitCfgObjDefInfo
		{
			get { return initCfgObjDefInfo; }
		}
		private static void createInitCfgObjDefInfo()
		{
            initCfgObjDefInfo = new GenCfgViewParamObjectDefInfoList();
			initCfgObjDefInfo.Add(new GenCfgViewParamObjectDefInfo()
			{
                ViewID = "InitCfgView1",
                ViewName = "初始化配置界面1",
                LabelWidth = 150,
                ParamInfoes = new GFParamDefInfoList()
                { 
                    new GFParamDefInfo("SenserIOGuids","料盒有无感应器ID",GriffinsBaseDataType.Guid),
                    new GFParamDefInfo("CylinderInitParameters","料盒气缸初始化参数列表",GriffinsBaseDataType.Object_Json), 
                },
            });
		}
		
		#endregion

		#region 配方配置参数
		private static GenCfgViewParamObjectDefInfoList ppCfgObjDefInfo;
		/// <summary>
		/// 配方参数对象定义信息
		/// </summary>
		public static GenCfgViewParamObjectDefInfoList PPCfgObjDefInfo
		{
			get { return ppCfgObjDefInfo; }
		}
		private static void createPPCfgObjDefInfo()
		{
            ppCfgObjDefInfo = new GenCfgViewParamObjectDefInfoList();
            ppCfgObjDefInfo.Add(new GenCfgViewParamObjectDefInfo()
			{
                ViewID = "PPCfgView1",
                ViewName = "配方配置界面1",
                LabelWidth = 150,
                ParamInfoes = new GFParamDefInfoList()
                { 
                    new GFParamDefInfo("SlotCount","槽位总数",GriffinsBaseDataType.Integer),
                    new GFParamDefInfo("FirstSlotPosition","首槽位置",GriffinsBaseDataType.Decimal),
                    new GFParamDefInfo("LastSlotPosition","末槽位置",GriffinsBaseDataType.Decimal),
                },
            });
		}

        #endregion

        #endregion


        #region 常量定义
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
        /// <summary>普通方法：控制指定料盒位的夹紧气缸动作。</summary>
        public const string ControlCylinderMethodID = "ControlCylinder";
        /// <summary>能力方法：获取料盒子模组初始化参数。</summary>
        public const string GetInitParametersMethodID = "GetInitParameters";
        /// <summary>能力方法：获取料盒子模组配方参数。</summary>
        public const string GetRecipeParametersMethodID = "GetRecipeParameters";

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
        #endregion

    }
}
