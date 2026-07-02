using Griffins;
using Griffins.ImeIOT;
using Griffins.IOT;
using GriffinsGeneralTestMM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSubMM_ElectricalMngObj;

namespace TestGlueDirectWorkMM
{
    public static class GenMMInfoDefine
    {
        public const string MMName = "测试点胶直接工作类机械模组";
        public const string MMNumberStr = "test_GlueDirectWork";
        public readonly static MMNumber MMNumber = MMNumber.Parse(MMNumberStr);

        static GenMMInfoDefine()
        {
            createGenMMInfo();
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
            //子机械模组界面数据对象命令定义信息列表
            fillUICommands();
            //控制面板定义信息列表
            fillControlPanels();
        }

        #region  机械模组包含的子机械模组能力数据列表

        #region  电气管理对象
        /// <summary>
        /// 电气管理对象实例1【内部别名】
        /// </summary>
        public readonly static InnerAlias InnerAlias_Test_ElectricalMngObj = InnerAlias.Parse("ElectricalMngObj001");

        /// <summary>
        /// 电气管理对象实例2【内部别名】
        /// </summary>
        public readonly static InnerAlias InnerAlias_Test_ElectricalMngObj2 = InnerAlias.Parse("ElectricalMngObj002");
        #endregion
        private static void fillSubMMs()
        {

            genMMInfo.SubMMs = new CompContainSubMMDataList()
            {
                new CompContainSubMMData(InnerAlias_Test_ElectricalMngObj,GenSubMMInfoDefine.SubMMModel),
                new CompContainSubMMData(InnerAlias_Test_ElectricalMngObj2,GenSubMMInfoDefine.SubMMModel),
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

        #region  测试能力方法
        /// <summary>
        /// 测试能力方法ID
        /// </summary>
        public const string MethodID_Test = "Test";
        /// <summary>
        /// 测试完成能力事件ID
        /// </summary>
        public static readonly string FinishedEventID_Test = ImeCompMethodDefInfo.GetFinishedEventID(MethodID_Test);

        internal class TestParamList : GfParamListObjBase
        {
            [GfParam("测试次数")]
            public int TestCount { get; set; }
        }

        internal class TestRetValList : GfParamListObjBase
        {
            [GfParam("已测试次数")]
            public int TestedCount { get; set; }
        }
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
                retValInfoes.TestedCount = 1;
            else
                retValInfoes.TestedCount = TestParams.TestCount + 1;
            return retValInfoes.ToGFBaseTypeParamValues();
        }

        #endregion


        private static void fillCabilityMethods()
        {
            genMMInfo.CabilityMethods = new GenMMCabilityMethodDefInfoList()
            {
                new GenMMCabilityMethodDefInfo(MethodID_Test,"测试",1000,
                GfParamListObjBase.GetParamDefInfoes<TestParamList>(ResourceNames.ResourceManager.GetString),
                GfParamListObjBase.GetParamDefInfoes<TestRetValList>(ResourceNames.ResourceManager.GetString),
                TestMethodParamToFinishedParam),
            };
        }

        #endregion

        #region  机械模组的机械模组普通事件列表
        private static void fillNormalEvents()
        {
            genMMInfo.NormalEvents = new GenNormalEventDefInfoList()
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

        #region  机械模组的机械模组普通方法列表
        private static void fillNormalMethods()
        {
            genMMInfo.NormalMethods = new GenMMNormalMethodDefInfoList()
            {
                new GenMMNormalMethodDefInfo(
                    MethodID_Test,"测试",1000,
                    GfParamListObjBase.GetParamDefInfoes<TestParamList>(ResourceNames.ResourceManager.GetString),
                    false,
                    GfParamListObjBase.GetParamDefInfoes<TestRetValList>(ResourceNames.ResourceManager.GetString),
                    TestMethodParamToFinishedParam
                    ),
                new GenMMNormalMethodDefInfo(
                    "KZMB","控制面板测试",1000,
                    null,
                    false,
                    null,
                    null
                    ),
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
                    new ImeCompPropDefInfo("Prop1","状态1",GfPropReadWrite.ReadWrite, GriffinsBaseDataType.String),
                    new ImeCompPropDefInfo("Prop2","状态2",GfPropReadWrite.ReadWrite, GriffinsBaseDataType.Integer),
                },

            };
        }
        #endregion

        #region  机械模组界面数据对象命令定义信息列表
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
            genMMInfo.UICommands = new ImeCompMethodDefInfoList();
            genMMInfo.UICommands.AddRange(imeCompMethodDefInfos);

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
            genMMInfo.ControlPanels = new ControlPanelDefInfoList();
            genMMInfo.ControlPanels.AddRange(controlPanelDefInfos);

        }
        #endregion

        #endregion

    }
}
