using Griffins;
using Griffins.ImeIOT;
using Griffins.IOT;
using GriffinsGeneralTestMM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSXLMM
{
	public static class GenMMInfoDefine
	{
		public const string MMName = "测试上下料";
		public const string MMNumberStr = "test_bzsxl";
		public readonly static MMNumber MMNumber = MMNumber.Parse(MMNumberStr);
		static GenMMInfoDefine()
		{
			createGenMMInfo();
			createGenMMDesignTimeInfo();
		}

		#region 机械模组信息
		private static GenMMInfo genMMInfo;
		/// <summary>
		/// 机械模组信息
		/// </summary>
		public static GenMMInfo GenMMInfo
        {
			get { return genMMInfo; }
        }

		private static void createGenMMInfo()
		{
			genMMInfo = new GenMMInfo();
			// 机械模组名称
			genMMInfo.MMName = MMName;
			//  机械模组包含的子机械模组能力数据列表
			fillSubMMs();
			//  机械模组的机械模组能力事件列表
			fillCabilityEvents();
			//  机械模组的机械模组能力方法列表
			fillCabilityMethods();
			//  机械模组的机械模组普通事件列表
			fillNormalEvents();
			// 机械模组的机械模组普通方法列表
			fillNormalMethods();
			// 执行接收到所属的子机械模组事件定义信息列表
			fillExecSubEventDefInfoes();
			//  机械模组的属性信息列表
			fillUIDataObjProps();
		}

        #region  机械模组包含的子机械模组能力数据列表
        #region  料盒
        /// <summary>
        /// 料盒【子机械模组型号】
        /// </summary>
        public readonly static SubMMModel SubMMModel_Test_MaterialBox = SubMMModel.Parse("Test_MaterialBox");
        /// <summary>
        /// 料盒实例1【内部别名】
        /// </summary>
        public readonly static InnerAlias InnerAlias_Test_MaterialBox01 = InnerAlias.Parse("Test_MaterialBox01");
        /// <summary>
        /// 锁定料盒能力方法ID
        /// </summary>
        public const string SubMMMMethodID_LockBox = "LockBox";
        /// <summary>
        /// 解锁料盒能力方法ID
        /// </summary>
        public const string SubMMMMethodID_UnLockBox = "UnLockBox";

        #endregion
        #region  推料
        /// <summary>
        ///  推料【子机械模组型号】
        /// </summary>
        public readonly static SubMMModel SubMMModel_Test_PushRod = SubMMModel.Parse("Test_PushRod");
        /// <summary>
        ///  电机型推料【内部别名】
        /// </summary>
        public readonly static InnerAlias InnerAlias_MotorPushRod = InnerAlias.Parse("MotorPushRod");
        /// <summary>
        ///  气缸型推料【内部别名】
        /// </summary>
        public readonly static InnerAlias InnerAlias_CylinderPushRod = InnerAlias.Parse("CylinderPushRod");
        /// <summary>
        /// 推料方法ID
        /// </summary>
        public const string SubMMMMethodID_TuiLiao = "TuiLiao";

		#endregion


		private static void fillSubMMs()
		{

			//genMMInfo.SubMMs = new CompContainSubMMDataList()
			//{
			//	  new CompContainSubMMData(InnerAlias_Test_MaterialBox01,SubMMModel_Test_MaterialBox),
			//	  new CompContainSubMMData(InnerAlias_MotorPushRod,SubMMModel_Test_PushRod),
		    //	 new CompContainSubMMData(InnerAlias_CylinderPushRod,SubMMModel_Test_PushRod),
			//};
            genMMInfo.SubMMs = new CompContainSubMMDataList()
            {
                  new CompContainSubMMData(InnerAlias_Test_MaterialBox01,SubMMModel_Test_MaterialBox),
                  new CompContainSubMMData(InnerAlias_MotorPushRod,SubMMModel_Test_PushRod),
                  new CompContainSubMMData(InnerAlias_CylinderPushRod,SubMMModel_Test_PushRod),
            };
        }

		#endregion

		#region  机械模组的机械模组能力事件列表

		private static void fillCabilityEvents()
		{

			genMMInfo.CabilityEvents = new GenCabilityEventDefInfoList()
			{
			};
		}

		#endregion

		#region  机械模组的机械模组能力方法列表

		#region  上料能力方法
		/// <summary>
		/// 上料方法ID
		/// </summary>
		public const string MethodID_ShangLiao = "ShangLiao";
		/// <summary>
		/// 上料完成能力事件ID
		/// </summary>
		public static readonly string FinishedEventID_ShangLiao = ImeCompMethodDefInfo.GetFinishedEventID(MethodID_ShangLiao);

		/// <summary>
		/// 上料方法参数
		/// </summary>
		internal class ShangLiaoMethodParam : GfParamListObjBase
        {
            [GfParam("属性1")]
            public int Prop_1 { get; set; }
            [GfParam("属性2")]
            public string Prop_2 { get; set; }
        }

		#endregion

		#region  下料能力方法
		/// <summary>
		/// 下料方法ID
		/// </summary>
		public const string MethodID_XiaLiao = "XiaLiao";
		/// <summary>
		/// 下料完成能力事件ID
		/// </summary>
		public static readonly string FinishedEventID_XiaLiao = ImeCompMethodDefInfo.GetFinishedEventID(MethodID_XiaLiao);

		/// <summary>
		/// 下料方法参数
		/// </summary>
		internal class XiaLiaoMethodParam : ShangLiaoMethodParam
		{
			public XiaLiaoMethodSubParam Sub { get; set; }
		}
		/// <summary>
		/// 下料方法参数
		/// </summary>
		internal class XiaLiaoMethodSubParam : GfParamListObjBase
        {
            [GfParam("子属性1")]
            public int SubProp_1 { get; set; }
            [GfParam("子属性2")]
            public string SubProp_2 { get; set; }
        }
		#endregion

		private static void fillCabilityMethods()
		{
			genMMInfo.CabilityMethods = new GenMMCabilityMethodDefInfoList()
			{
				new GenMMCabilityMethodDefInfo(MethodID_ShangLiao,"上料",1000,GfParamListObjBase.GetParamDefInfoes<ShangLiaoMethodParam>(ResourceNames.ResourceManager.GetString),null,null,
					new GenMMMMethodExecDefInfoList()
					{
						new GenMMMMethodExecDefInfo(InnerAlias_MotorPushRod,SubMMMMethodID_TuiLiao,true,null),
					}),
				new GenMMCabilityMethodDefInfo(MethodID_XiaLiao,"下料",1000,GfParamListObjBase.GetParamDefInfoes<XiaLiaoMethodSubParam>(ResourceNames.ResourceManager.GetString),null,null,
					new GenMMMMethodExecDefInfoList()
					{
						new GenMMMMethodExecDefInfo(InnerAlias_CylinderPushRod,SubMMMMethodID_TuiLiao,true,null),
					}),

			};
		}

		#endregion

		#region  机械模组的机械模组普通事件列表
		private static void fillNormalEvents()
		{
			genMMInfo.NormalEvents = new GenNormalEventDefInfoList()
			{
			};
		}

		#endregion

		#region  机械模组的机械模组普通方法列表
		private static void fillNormalMethods()
		{
			genMMInfo.NormalMethods = new GenMMNormalMethodDefInfoList()
			{
			};
		}

		#endregion

		#region   执行接收到所属的子机械模组事件定义信息列表
		private static void fillExecSubEventDefInfoes()
		{
			genMMInfo.ExecSubEventDefInfoes = new GenExecSubEventDefInfoList()
			{
			};
		}

        #endregion

        #region   机械模组界面数据对象属性定义信息列表
        private static void fillUIDataObjProps()
		{
			genMMInfo.UIDataObjProps = new GenUIDataObjPropDefInfo()
			{
				LabelWidth = 48,
                Props = new ImeCompPropDefInfoList()
                {
                    new ImeCompPropDefInfo("Prop1","状态1",GfPropReadWrite.ReadWrite,GriffinsBaseDataType.String),
                },

            };
		}

		#endregion


		#endregion

		#region 机械模组设计时信息
		private static GenMMDesignTimeInfo genMMDesignTimeInfo;
		/// <summary>
		/// 机械模组设计时信息
		/// </summary>
		public static GenMMDesignTimeInfo GenMMDesignTimeInfo
		{
			get { return genMMDesignTimeInfo; }
		}
		private static void createGenMMDesignTimeInfo()
		{
			createFactoryCfgObjDefInfo();
			createInitCfgObjDefInfo();
			createPPCfgObjDefInfo();

			genMMDesignTimeInfo = new GenMMDesignTimeInfo(FactoryCfgObjDefInfo, InitCfgObjDefInfo, PPCfgObjDefInfo);
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
                ViewID = "FactoryCfgView1",
                ViewName = "出厂配置界面1",
                LabelWidth = 72,
                ParamInfoes = new GFParamDefInfoList()
                {
                    new GFParamDefInfo("Param1","出厂配置参数1",GriffinsBaseDataType.Integer),
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
                LabelWidth = 72,
                ParamInfoes = new GFParamDefInfoList()
                {
                    new GFParamDefInfo("Param1","初始化配置参数1",GriffinsBaseDataType.Integer),
                    new GFParamDefInfo("Param2","初始化配置参数2",GriffinsBaseDataType.String),
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
                LabelWidth = 72,
                ParamInfoes = new GFParamDefInfoList()
                {
                    new GFParamDefInfo("Param1","配方参数1",GriffinsBaseDataType.Integer),
                },
            });
		}

		#endregion

		#endregion
	}


}
