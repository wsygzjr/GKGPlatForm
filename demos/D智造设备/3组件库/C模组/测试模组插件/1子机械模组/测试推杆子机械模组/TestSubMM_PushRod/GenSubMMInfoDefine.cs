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

namespace TestSubMM_PushRod
{
	public static class GenSubMMInfoDefine
	{
		public const string SubMMName = "测试推杆";
		public const string SubMMModelStr = "Test_PushRod";
		public readonly static SubMMModel SubMMModel = SubMMModel.Parse(SubMMModelStr);


        public static readonly Guid MotorSubMMObjID = Guid.Parse("34A3A81E-A741-458C-AECE-4706811DFECA");
        public static readonly Guid CylinderSubMMObjID = Guid.Parse("4CCC3359-F92C-4372-B35D-EE57E26ADCE6");

        public const string MotorSubMMObjName = "电机推杆1";
        public const string CylinderSubMMObjName = "气缸推杆1";

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
            //设备属性
            fillDeviceProps();

        }

		#region  子机械模组的子机械模组能力事件列表
		private static void fillCabilityEvents()
		{

			genSubMMInfo.CabilityEvents = new GenCabilityEventDefInfoList()
			{
				new GenCabilityEventDefInfo()
				{
					EventID=EventPusherForwardCompleted,
					EventName="伸出完成"
                },
                new GenCabilityEventDefInfo()
                {
                    EventID=EventPusherBackwardCompleted,
                    EventName="缩回完成"
                },
            };
		}

        #endregion

        #region  子机械模组的子机械模组能力方法列表

        private static void fillCabilityMethods()
		{
			genSubMMInfo.CabilityMethods = new GenCabilityMethodDefInfoList()
			{
                 new GenCabilityMethodDefInfo(ExtendMethodID, "伸出", 1000,null,null,null),
                new GenCabilityMethodDefInfo(RetractMethodID, "缩回", 1000,null,null,null),
                new GenCabilityMethodDefInfo(CheckHasMaterialMethodID, "检测是否有料", 1000,null,null,null),
                new GenCabilityMethodDefInfo(SetLoadFinishedMethodID, "设置上料完成", 1000,null,null,null),
                new GenCabilityMethodDefInfo(SetUnloadFinishedMethodID, "设置下料完成", 1000,null,null,null),
            };
		}

		#endregion

		#region  子机械模组的子机械模组普通事件列表
		private static void fillNormalEvents()
		{
		
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
            genSubMMInfo.UIDataObjProps = new GenUIDataObjPropDefInfo()
			{
				LabelWidth = 150,
				Props = new GFUIPropDefInfoList()
				{
				},
			};
        }

		#endregion

		#region 子机械模组实现对象列表

		private static void fillSubMMObjInfos()
		{
			genSubMMInfo.SubMMObjInfos = new SubMMObjInfoList()
			{
                 new SubMMObjInfo
                {
                    SubMMObjID = MotorSubMMObjID,
                    SubMMObjName = MotorSubMMObjName,
                },
                new SubMMObjInfo
                {
                    SubMMObjID =CylinderSubMMObjID,
                    SubMMObjName = CylinderSubMMObjName,
                }
            };
        }

        #endregion
        #region 设备属性列表

        private static void fillDeviceProps()
        {
            genSubMMInfo.DeviceProps = new DevicePropertyInfoList()
            {
                new DevicePropertyInfo()
                {
                    PropertyID=new MPPropertyID("DevModel"),
                    PropertyName="设备型号",
                   DataType=GriffinsBaseDataType.Integer,
				   ValueRange=GriffinsValueRange.CreateEnumValueRange(new List<GriffinsBaseValue>()
				   {
					   new GriffinsBaseValue(1), new GriffinsBaseValue(2),new GriffinsBaseValue(3)
				   }),
				   ValueNamePairs=new GriffinsValueNamePairList()
				   {
					   new GriffinsValueNamePair()
					   {
						   Val=new GriffinsBaseValue(1),
						   Name="型号1",
					   },
                        new GriffinsValueNamePair()
                       {
                           Val=new GriffinsBaseValue(2),
                           Name="型号2",
                       },
                         new GriffinsValueNamePair()
                       {
                           Val=new GriffinsBaseValue(3),
                           Name="型号3",
                       }
                   }
                }
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
            //电机推杆实现对象配置参数
            var factoryCfgObjDefInfoOfMotor = createFactoryCfgObjDefInfo();
			var initCfgObjDefInfoOfMotor = createInitCfgObjDefInfo();
			var ppCfgObjDefInfoOfMotor = createPPCfgObjDefInfo();
			var genSubMMDesignTimeInfoOfMotor = new GenSubMMDesignTimeInfo(factoryCfgObjDefInfoOfMotor, initCfgObjDefInfoOfMotor, ppCfgObjDefInfoOfMotor);
			GenSubMMDesignTimeInfoDic.Add(MotorSubMMObjID, genSubMMDesignTimeInfoOfMotor);

            //气缸推杆实现对象
            var factoryCfgObjDefInfoOfCylinder = createFactoryCfgObjDefInfoOfCylinder();
            var initCfgObjDefInfoOfCylinder = createInitCfgObjDefInfoOfCylinder();
            var ppCfgObjDefInfoOfCylinder = createPPCfgObjDefInfoOfCylinder();
            var genSubMMDesignTimeInfoOfCylinder = new GenSubMMDesignTimeInfo(factoryCfgObjDefInfoOfCylinder, initCfgObjDefInfoOfCylinder, ppCfgObjDefInfoOfCylinder);
            GenSubMMDesignTimeInfoDic.Add(CylinderSubMMObjID, genSubMMDesignTimeInfoOfCylinder); 
        }

        #region 气缸推杆实现对象
        #region 出厂配置参数
        private static GenCfgViewParamObjectDefInfoList createFactoryCfgObjDefInfoOfCylinder()
        {
            var factoryCfgObjDefInfo = new GenCfgViewParamObjectDefInfoList();
            factoryCfgObjDefInfo.Add(new GenCfgViewParamObjectDefInfo()
            {
                ViewID = "FactoryCfgView1",
                ViewName = "出厂配置界面",
                LabelWidth = 150,
                ParamInfoes = new GFParamDefInfoList()
                {
                    new GFParamDefInfo("IsSupportHasMaterialCheck","是否支持有料感应",GriffinsBaseDataType.Bool), 
                },
            });
            return factoryCfgObjDefInfo;
        }

        #endregion
        #region 初始化配置参数
        private static GenCfgViewParamObjectDefInfoList createInitCfgObjDefInfoOfCylinder()
        {
            var initCfgObjDefInfo = new GenCfgViewParamObjectDefInfoList();
            initCfgObjDefInfo.Add(new GenCfgViewParamObjectDefInfo()
            {
                ViewID = "InitCfgView1",
                ViewName = "初始化配置界面",
                LabelWidth = 150,
                ParamInfoes = new GFParamDefInfoList()
                {
                    new GFParamDefInfo("CylinderInitParameters","推料气缸初始化参数",GriffinsBaseDataType.Object_Json),
                    new GFParamDefInfo("PushCylinderSolenoidValveLiftDelay","推料气缸电磁阀升降延时",GriffinsBaseDataType.Integer),
                },
            });
            return initCfgObjDefInfo;
        }

        #endregion
        #region 配方配置参数
        private static GenCfgViewParamObjectDefInfoList createPPCfgObjDefInfoOfCylinder()
        {
            var ppCfgObjDefInfo = new GenCfgViewParamObjectDefInfoList(); 
            return ppCfgObjDefInfo;
        }
        #endregion

        #endregion 气缸推杆实现对象


        #region 电机推杆实现对象
        #region 出厂配置参数
        private static GenCfgViewParamObjectDefInfoList createFactoryCfgObjDefInfo()
		{
            var factoryCfgObjDefInfo = new GenCfgViewParamObjectDefInfoList(); 
			return factoryCfgObjDefInfo;
        }

		#endregion
		#region 初始化配置参数
		private static GenCfgViewParamObjectDefInfoList createInitCfgObjDefInfo()
		{
            var initCfgObjDefInfo = new GenCfgViewParamObjectDefInfoList();
			initCfgObjDefInfo.Add(new GenCfgViewParamObjectDefInfo()
			{
                ViewID = "InitCfgView1",
                ViewName = "初始化配置界面",
                LabelWidth = 150,
                ParamInfoes = new GFParamDefInfoList()
                { 
                    new GFParamDefInfo("PusherPhysicalAxis","推杆轴物理轴号ID",GriffinsBaseDataType.Guid),
                    new GFParamDefInfo("PushRodTimeout","推杆超时时间",GriffinsBaseDataType.Integer), 
                },
            });
			return initCfgObjDefInfo;
        }
		
		#endregion
		#region 配方配置参数
		private static GenCfgViewParamObjectDefInfoList createPPCfgObjDefInfo()
		{
            var ppCfgObjDefInfo = new GenCfgViewParamObjectDefInfoList();
            ppCfgObjDefInfo.Add(new GenCfgViewParamObjectDefInfo()
			{
                ViewID = "PPCfgView1",
                ViewName = "配方配置界面1",
                LabelWidth = 150,
                ParamInfoes = new GFParamDefInfoList()
                { 
                    new GFParamDefInfo("PushDistance","推料距离",GriffinsBaseDataType.Decimal),
                    new GFParamDefInfo("PushAxisSpeed","推杆轴速度",GriffinsBaseDataType.Decimal),
                    new GFParamDefInfo("PushAxisAcceleration","推杆轴加速度",GriffinsBaseDataType.Decimal),
                },
            });
			return ppCfgObjDefInfo;
        }
        #endregion

        #endregion 电机推杆实现对象

        #endregion
        #region 常量定义
        public const string ExtendMethodID = "Extend";
		public const string RetractMethodID = "Retract";
		public const string CheckHasMaterialMethodID = "CheckHasMaterial";
		public const string GetInitParametersMethodID = "GetInitParameters";
		public const string GetRecipeParametersMethodID = "GetRecipeParameters";

		public const string LoadMaterialMethodID = ExtendMethodID;
		public const string UnloadMaterialMethodID = RetractMethodID;

		public const string SetLoadFinishedMethodID = "SetLoadFinished";
		public const string SetUnloadFinishedMethodID = "SetUnloadFinished";

		public const string EventPusherForwardCompleted = "PusherForwardCompleted";
		public const string EventPusherBackwardCompleted = "PusherBackwardCompleted";
		public const string EventPusherExtendFailed = "PusherExtendFailed";
		public const string EventPusherRetractFailed = "PusherRetractFailed";
		public const string EventPushJam = "PushJam";

		public const string RtCmdPusherForward = "PusherForward";
		public const string RtCmdPusherBackward = "PusherBackward";
		public const string RtCmdPusherHomed = "PusherHomed";
		public const string RtCmdGetStatus = "GetStatus";
		public const string RtCmdGetCylinderIOChannelOptions = "GetIOInfos";
		public const string RtCmdGetAxisOptions = "GetAxisInfos"; 
		#endregion
	}
}
