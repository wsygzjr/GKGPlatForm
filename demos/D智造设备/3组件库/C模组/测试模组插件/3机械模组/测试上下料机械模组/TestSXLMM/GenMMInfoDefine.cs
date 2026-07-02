using Griffins;
using Griffins.ImeIOT;
using Griffins.IOT;
using Griffins.UI;
using GriffinsGeneralTestMM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSXLMM
{
    public static partial class GenMMInfoDefine
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
            //控制面板定义信息列表
            fillControlPanels();
            //  机械模组的属性信息列表
            fillUIDataObjProps();
            //子机械模组界面数据对象命令定义信息列表
            fillUICommands();
        }

        #region  机械模组包含的子机械模组能力数据列表

        private static void fillSubMMs()
        {

            genMMInfo.SubMMs = new CompContainSubMMDataList()
            {
                new CompContainSubMMData(InnerAliasMaterialBox, SubMMModelMaterialBox)
                {
                    Memo = "料盒子机械模组"
                },
                new CompContainSubMMData(InnerAliasLoadPushRod, SubMMModelPushRod)
                {
                    Memo = "电机型推杆子机械模组"
                },
                new CompContainSubMMData(InnerAliasUnLoadPushRod, SubMMModelPushRod)
                {
                    Memo = "气缸型推杆子机械模组"
                },
            };
        }

        #endregion

        #region  机械模组的机械模组能力事件列表

        private static void fillCabilityEvents()
        {

            genMMInfo.CabilityEvents = new GenCabilityEventDefInfoList()
            {
                //new GenCabilityEventDefInfo(EventUpfeedCompleted,"上料完成"),
                //new GenCabilityEventDefInfo(EventDownfeedCompleted,"下料完成"),
            };
        }

        #endregion

        #region  机械模组的机械模组能力方法列表

        private static void fillCabilityMethods()
        {
            genMMInfo.CabilityMethods = new GenMMCabilityMethodDefInfoList()
            {
                new GenMMCabilityMethodDefInfo(UpfeedMethodID, "上料一次",100,new GFParamDefInfoList(), new GFParamDefInfoList(),null),
                new GenMMCabilityMethodDefInfo(DownfeedMethodID, "下料一次",100,new GFParamDefInfoList(), new GFParamDefInfoList(),null),

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
                //上料推杆推完成事件-》机械模组的上料完成
                //new GenExecSubEventDefInfo(InnerAliasLoadPushRod,TestSubMM_PushRod.GenSubMMInfoDefine.EventPusherBackwardCompleted,EventUpfeedCompleted,null),
            };
        }

        #endregion

        #region   机械模组界面数据对象属性定义信息列表
        private static void fillUIDataObjProps()
        {
            GFUIPropDefInfoList gFUIPropDefInfos = GFPropObjBase.GetGFPropDefInfoes<LoadUnloadUIObj>(
                ResourceNames.ResourceManager.GetString,
                LoadUnloadUIObj.GetValueRangeEnums, 
                LoadUnloadUIObj.GetValueNamePairs);

            genMMInfo.UIDataObjProps = new GenUIDataObjPropDefInfo()
            {
                LabelWidth = 120,
                Props = gFUIPropDefInfos,
            };
        }
        #endregion

        #region  控制面板定义信息列表
        private static void fillControlPanels()
        {

            ControlPanelDefInfoList controlPanelDefInfos = new ControlPanelDefInfoList();
            controlPanelDefInfos.Add(new ControlPanelDefInfo()
            {
                ControlPanelID = "CP_OneClickLoad",
                ControlPanelName = "一键上料控制面板",
                ControlPanelWinSize = new ControlPanelWinSize()
                {
                    Height = 805,
                    Width = 1124
                },
                Commands = new GFMethodDefInfoList()
                {
                    new GFMethodDefInfo()
                    {
                         MethodID = "OneClickLoad",
                        MethodName = "上料",
                        ParamDefInfoes = new Griffins.GFParamDefInfoList() { new Griffins.GFParamDefInfo() { ParamID = "Param1", ParamName = "延时", DataType = GriffinsBaseDataType.Integer } },
                    }
                }
            });
            controlPanelDefInfos.Add(new ControlPanelDefInfo()
            {
                ControlPanelID = "CP_OneClickUnload",
                ControlPanelName = "一键下料控制面板",
                ControlPanelWinSize = new ControlPanelWinSize()
                {
                    Height = 805,
                    Width = 1124
                },
                Commands = new GFMethodDefInfoList()
                {
                    new GFMethodDefInfo()
                    {
                        MethodID = "OneClickUnload",
                        MethodName = "下料",
                        ParamDefInfoes = new Griffins.GFParamDefInfoList() { new Griffins.GFParamDefInfo() { ParamID = "Param1", ParamName = "延时", DataType = GriffinsBaseDataType.Integer } },
                    }
                }
            });
            controlPanelDefInfos.Add(new ControlPanelDefInfo()
            {
                ControlPanelID = "CP_OneClickInspect",
                ControlPanelName = "一键抽检控制面板",
                ControlPanelWinSize = new ControlPanelWinSize()
                {
                    Height = 805,
                    Width = 1124
                },
                Commands = new GFMethodDefInfoList()
                {
                    new GFMethodDefInfo()
                    {
                         MethodID = "OneClickInspect",
                        MethodName = "抽检",
                        ParamDefInfoes = new Griffins.GFParamDefInfoList() { new Griffins.GFParamDefInfo() { ParamID = "Param1", ParamName = "延时", DataType = GriffinsBaseDataType.Integer } },
                    }
                }
            });
            genMMInfo.ControlPanels = new ControlPanelDefInfoList();
            genMMInfo.ControlPanels.AddRange(controlPanelDefInfos);

        }
        #endregion

        #region  机械模组界面数据对象命令定义信息列表
        private static void fillUICommands()
        {
            ImeCompMethodDefInfoList imeCompMethodDefInfos = new ImeCompMethodDefInfoList()
            {
                  new ImeCompMethodDefInfo(
                    StorageOpenMethodID,"料盒张开",
                    new Griffins.GFParamDefInfoList(){
                        new GFParamDefInfo {
                            DataType = GriffinsBaseDataType.String,
                            ParamID =nameof(MaterialContainer.ContainerName) ,
                            ParamName = "容器名"
                        },
                        new GFParamDefInfo {
                            DataType = GriffinsBaseDataType.String,
                            ParamID =nameof(MaterialBox.MaterialBoxName) ,
                            ParamName = "料盒名"
                        }
                    },new Griffins.GFParamDefInfoList(),false),
                new ImeCompMethodDefInfo(
                    StorageCloseMethodID,"料盒夹紧",
                    new Griffins.GFParamDefInfoList(){
                         new GFParamDefInfo {
                            DataType = GriffinsBaseDataType.String,
                            ParamID =nameof(MaterialContainer.ContainerName) ,
                            ParamName = "容器名"
                        },
                        new GFParamDefInfo {
                            DataType = GriffinsBaseDataType.String,
                            ParamID =nameof(MaterialBox.MaterialBoxName) ,
                            ParamName = "料盒名"
                        }
                    },new Griffins.GFParamDefInfoList(),false)
            };
            genMMInfo.UICommands = new ImeCompMethodDefInfoList();
            genMMInfo.UICommands.AddRange(imeCompMethodDefInfos);

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
                LabelWidth = 120,
                ParamInfoes = new GFParamDefInfoList()
                {
                    new GFParamDefInfo("ContinuousEmptyPushThreshold","连续空推次数",GriffinsBaseDataType.Integer), 
                },
            });
        }

        #endregion

        #endregion

        #region 常量定义
        /// <summary>发起完整上料主流程。</summary>
        public const string UpfeedMethodID = "Upfeed";
        /// <summary>发起完整下料主流程。</summary>
        public const string DownfeedMethodID = "Downfeed";

        /// <summary>普通方法：执行指定储料位料盒张开。</summary>
        public const string StorageOpenMethodID = "StorageOpen";
        /// <summary>普通方法：执行指定储料位料盒夹紧。</summary>
        public const string StorageCloseMethodID = "StorageClose";
        public const string GetMaterialStatus = "GetMaterialStatus";

        /// <summary>UIDataObj 中料盒容器状态属性的统一 PropertyID；值为序列化后的 MaterialContainerStatus JSON。</summary>
        public const string MaterialContainerStatusPropertyID = "MaterialContainerStatus";

        /// <summary>总模块内部依赖的子模组模型定义。</summary>
        public readonly static SubMMModel SubMMModelMaterialBox = TestSubMM_MaterialBox.GenSubMMInfoDefine.SubMMModel;
        public readonly static SubMMModel SubMMModelPushRod = TestSubMM_PushRod.GenSubMMInfoDefine.SubMMModel;
        /// <summary>总模块内部使用的子模组别名；用于创建和路由到对应执行器。</summary>

        /// <summary>
        /// 料盒容器
        /// </summary>
        public readonly static InnerAlias InnerAliasMaterialBox = InnerAlias.Parse("MaterialBox");
        /// <summary>
        /// 上料推杆
        /// </summary>
        public readonly static InnerAlias InnerAliasLoadPushRod = InnerAlias.Parse("MotorPushRod");
        /// <summary>
        /// 下料推杆
        /// </summary> 
        public readonly static InnerAlias InnerAliasUnLoadPushRod = InnerAlias.Parse("CylinderPushRod");

        //上料完成事件
        public const string EventUpfeedCompleted = "EventUpfeedCompleted";
        //下料完成事件
        public const string EventDownfeedCompleted = "EventDownfeedCompleted";
        #endregion
    }


}
