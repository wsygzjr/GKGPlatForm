using DynamicData;
using GF_Gereric;
using Griffins;
using Griffins.ImeIOT;
using Griffins.IOT;
using GriffinsGeneralTestMM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSubMM_ElectricalMngObj
{
	public static class GenSubMMInfoDefine
	{
		public const string SubMMName = "电气管理对象";
		public const string SubMMModelStr = "Test_ElectricalMngObj";
		public readonly static SubMMModel SubMMModel = SubMMModel.Parse(SubMMModelStr);

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
            //子机械模组界面数据对象命令定义信息列表
            fillUICommands();
            //控制面板定义信息列表
            fillControlPanels();
        }

		#region  子机械模组的子机械模组能力事件列表
		private static void fillCabilityEvents()
		{

			genSubMMInfo.CabilityEvents = new GenCabilityEventDefInfoList()
			{
			};
		}

		#endregion

		#region  子机械模组的子机械模组能力方法列表

		#region  测试能力方法
		/// <summary>
		/// 测试能力方法ID
		/// </summary>
		public const string TestMethodID = "Test";
		/// <summary>
		/// 测试能力事件ID
		/// </summary>
		public static readonly string TestFinishedEventID = ImeCompMethodDefInfo.GetFinishedEventID(TestMethodID);

		/// <summary>
		///  测试方法参数转换为测试完成事件参数
		/// </summary>
		/// <param name="methodParam"> 测试方法参数</param>
		/// <returns>测试完成事件参数</returns>
		private static GFBaseTypeParamValueList TestMethodParamToFinishedParam(GFBaseTypeParamValueList methodParam)
		{
			var TestParams = GfParamListObjBase.FromGFBaseTypeParamValues<TestParamList>(methodParam);

			var retValInfoes = new TestRetValList();
			if (TestParams == null)
                retValInfoes.TestedCount =  1;
			else
				retValInfoes.TestedCount = TestParams.TestCount + 1;
			return retValInfoes.ToGFBaseTypeParamValues();
		}
      

		internal class TestParamList:GfParamListObjBase
		{
            [GfParam("测试次数")]
			public int TestCount { get; set; }
        }

        internal class TestRetValList : GfParamListObjBase
        {
            [GfParam("已测试次数")]
            public int TestedCount { get; set; }
        }

        #endregion

        private static void fillCabilityMethods()
		{
			genSubMMInfo.CabilityMethods = new GenCabilityMethodDefInfoList()
			{
				new GenCabilityMethodDefInfo(TestMethodID,"测试",1000,
				GfParamListObjBase.GetParamDefInfoes<TestParamList>(ResourceNames.ResourceManager.GetString),
				GfParamListObjBase.GetParamDefInfoes<TestRetValList>(ResourceNames.ResourceManager.GetString),
				TestMethodParamToFinishedParam),
			};
		}

		#endregion

		#region  子机械模组的子机械模组普通事件列表
		private static void fillNormalEvents()
		{
			genSubMMInfo.NormalEvents = new GenNormalEventDefInfoList()
			{
                 new GenNormalEventDefInfo()
                {
                    EventKind=1,
                    EventName="事件1",
                    ParamObjDefInfo=new GenParamObjectDefInfo()
                    {
                        ParamInfoes=new GFParamDefInfoList()
                        {
                            new GFParamDefInfo()
                            {
                                ParamID="Param1",
                                ParamName="参数1",
                                DataType=GriffinsBaseDataType.Bool
                            }
                        }
                    }
                }
            };
		}

		#endregion

		#region  子机械模组的子机械模组普通方法列表
		private static void fillNormalMethods()
		{
			genSubMMInfo.NormalMethods = new GenNormalMethodDefInfoList()
			{
				new GenNormalMethodDefInfo(
					TestMethodID,"测试",1000,
					GfParamListObjBase.GetParamDefInfoes<TestParamList>(ResourceNames.ResourceManager.GetString),
					false,
                    GfParamListObjBase.GetParamDefInfoes<TestRetValList>(ResourceNames.ResourceManager.GetString),
					TestMethodParamToFinishedParam
                    ),
                new GenNormalMethodDefInfo(
                    "KZMB","控制面板测试",1000,
                    null,
                    false,
                    null,
                    null
                    ),
            };
		}

        #endregion

        #region  子机械模组界面数据对象属性定义信息列表
        private static void fillUIDataObjProps()
		{
            genSubMMInfo.UIDataObjProps = new GenUIDataObjPropDefInfo()
            {
                LabelWidth = 48,
                Props = new ImeCompPropDefInfoList()
                {
                    new ImeCompPropDefInfo("Prop1","属性1",GfPropReadWrite.ReadWrite,GriffinsBaseDataType.String),
                    new ImeCompPropDefInfo("Prop2","属性2",GfPropReadWrite.ReadWrite,GriffinsBaseDataType.Integer),
                    new ImeCompPropDefInfo("Prop3","属性3",GfPropReadWrite.ReadWrite,GriffinsBaseDataType.Object_Json),
                },
            };
		}

        #endregion

        #region  子机械模组界面数据对象命令定义信息列表
        private static void fillUICommands()
        {
            ImeCompMethodDefInfoList imeCompMethodDefInfos = new ImeCompMethodDefInfoList();
            imeCompMethodDefInfos.Add(new ImeCompMethodDefInfo()
            {
                MethodID = "TestCmdID1",
                MethodName = "测试命令1",
                ParamDefInfoes = new Griffins.GFParamDefInfoList() { new Griffins.GFParamDefInfo() { ParamID = "Param1", ParamName = "参数1", DataType = GriffinsBaseDataType.Integer } },
                RetValDefInfoes = new Griffins.GFParamDefInfoList() { new Griffins.GFParamDefInfo() { ParamID = "Param1", ParamName = "返回参数1", DataType = GriffinsBaseDataType.String } }
            });
            imeCompMethodDefInfos.Add(new ImeCompMethodDefInfo()
            {
                MethodID = "TestCmdID2",
                MethodName = "测试命令2",
                ParamDefInfoes = new Griffins.GFParamDefInfoList() { new Griffins.GFParamDefInfo() { ParamID = "Param1", ParamName = "参数1", DataType = GriffinsBaseDataType.Integer } },
                RetValDefInfoes = new Griffins.GFParamDefInfoList() { new Griffins.GFParamDefInfo() { ParamID = "Param1", ParamName = "返回参数1", DataType = GriffinsBaseDataType.String } }
            });
			genSubMMInfo.UICommands = new ImeCompMethodDefInfoList();
			genSubMMInfo.UICommands.AddRange(imeCompMethodDefInfos);
            
        }

        #endregion

        #region  控制面板定义信息列表
        private static void fillControlPanels()
        {
            GFMethodDefInfoList gFMethodDefInfos = new GFMethodDefInfoList();
            gFMethodDefInfos.Add(new GFMethodDefInfo()
            {
                MethodID = "TestCmdID1",
                MethodName = "测试命令1",
                ParamDefInfoes = new Griffins.GFParamDefInfoList() { new Griffins.GFParamDefInfo() { ParamID = "Param1", ParamName = "参数1", DataType = GriffinsBaseDataType.Integer } },
                RetValDefInfoes = new Griffins.GFParamDefInfoList() { new Griffins.GFParamDefInfo() { ParamID = "Param1", ParamName = "返回参数1", DataType = GriffinsBaseDataType.String } }
            });
            gFMethodDefInfos.Add(new GFMethodDefInfo()
            {
                MethodID = "TestCmdID2",
                MethodName = "测试命令2",
                ParamDefInfoes = new Griffins.GFParamDefInfoList() { new Griffins.GFParamDefInfo() { ParamID = "Param1", ParamName = "参数1", DataType = GriffinsBaseDataType.Integer } },
                RetValDefInfoes = new Griffins.GFParamDefInfoList() { new Griffins.GFParamDefInfo() { ParamID = "Param1", ParamName = "返回参数1", DataType = GriffinsBaseDataType.String } }
            });

            ControlPanelDefInfoList controlPanelDefInfos = new ControlPanelDefInfoList();
            controlPanelDefInfos.Add(new ControlPanelDefInfo()
            {
                ControlPanelID = "Test1",
                ControlPanelName = "控制面板1",
                ControlPanelWinSize = new ControlPanelWinSize()
                {
                    Height = 805,
                    Width = 1124
                },
                Commands = gFMethodDefInfos
            });
            controlPanelDefInfos.Add(new ControlPanelDefInfo()
            {
                ControlPanelID = "Test2",
                ControlPanelName = "控制面板2",
                ControlPanelWinSize = new ControlPanelWinSize()
                {
                    Height = 805,
                    Width = 1124
                },
                Commands = gFMethodDefInfos
            });
            genSubMMInfo.ControlPanels = new ControlPanelDefInfoList();
            genSubMMInfo.ControlPanels.AddRange(controlPanelDefInfos);

        }
        #endregion
        #endregion

        #region 子机械模组设计时信息
        private static GenSubMMDesignTimeInfo genSubMMDesignTimeInfo;
		/// <summary>
		/// 子机械模组设计时信息
		/// </summary>
		public static GenSubMMDesignTimeInfo GenSubMMDesignTimeInfo
		{
			get { return genSubMMDesignTimeInfo; }
		}
		private static void createGenSubMMDesignTimeInfo()
		{
			createFactoryCfgObjDefInfo();
			createInitCfgObjDefInfo();
			createPPCfgObjDefInfo();

			genSubMMDesignTimeInfo = new GenSubMMDesignTimeInfo(FactoryCfgObjDefInfo, InitCfgObjDefInfo, PPCfgObjDefInfo);
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
                    new GFParamDefInfo("Param2","出厂配置参数2",GriffinsBaseDataType.String),
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
            initCfgObjDefInfo.Add(new GenCfgViewParamObjectDefInfo
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
                    new GFParamDefInfo("Param2","配方参数2",GriffinsBaseDataType.String),
                },
            });
		}

		#endregion

		#endregion
	}
}
