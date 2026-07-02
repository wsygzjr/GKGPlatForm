using GF_Gereric;
using Griffins.ImeIOT;
using Griffins;
using System;
using GKG.ElectronicControl;
using GKG.ElectronicControl.General;
using System.Threading.Tasks;

namespace GKG
{
    namespace SubMM
    {
        public class MeasureHeightFunctionHeadSubMMCmdExecutor : ISubMMCmdExecutor, ISubMMManualModeCmdExecutor, ISubMMAutoModeCmdExecutor
        {
            private MeasureHeightFunctionHeadSubMachineModulesFactoryCfg factoryCfg;

            private MeasureHeightFunctionHeadSubMachineModulesInitCfg initCfg;

            private MeasureHeightFunctionHeadSubMachineModulesPPCfg ppCfg;

            private ISubMMCmdExecutorCallBack iSubMMCmdExecutorCallBack;

            private SubMMAlias alias;

            private ImeGenNormalEventHandler imeGenNormalEventHandler;

            private ImeCabilityEventHandler imeCabilityEventHandler;

            private ImeAlarmEventHandler imeAlarmEventHandler;

            IMeasureHeightBase measureHeight;

            private bool exit = false;
            private Task updateHeightTask;
            public MeasureHeightFunctionHeadSubMMCmdExecutor(SubMMAlias alias, byte[] factoryCfgInfo)
            {
                this.alias = alias;
                factoryCfg = new MeasureHeightFunctionHeadSubMachineModulesFactoryCfg();
                if (factoryCfgInfo != null && factoryCfgInfo.Length > 0)
                {
                    factoryCfg.FromBytes(factoryCfgInfo);
                }
                initCfg = new MeasureHeightFunctionHeadSubMachineModulesInitCfg();
                ppCfg = new MeasureHeightFunctionHeadSubMachineModulesPPCfg();
            }

            event ImeGenNormalEventHandler ISubMMAutoModeCmdExecutor.GenNormalEvent
            {
                add
                {
                    imeGenNormalEventHandler += value;
                }
                remove
                {
                    imeGenNormalEventHandler -= value;
                }
            }

            event ImeCabilityEventHandler ISubMMAutoModeCmdExecutor.CabilityEvent
            {
                add
                {
                    imeCabilityEventHandler += value;
                }
                remove
                {
                    imeCabilityEventHandler -= value;
                }
            }

            event ImeAlarmEventHandler ISubMMAutoModeCmdExecutor.AlarmEvent
            {
                add
                {
                    imeAlarmEventHandler += value;
                }
                remove
                {
                    imeAlarmEventHandler -= value;
                }
            }

            /// <summary>
            /// 异常编号
            /// </summary>
            public int ErrorCode { get; set; } = 0;

            public void BeforeInit(string[] subMechCompParam)
            {

            }

            /// <summary>
            /// 初始化（在创建子机械模组实例后首先调用）
            /// </summary>
            /// <param name="initCfgInfo">初始化参数，null表示缺省值</param>
            /// <param name="callBack">子机械模组（复合子机械模组）运行时回调接口</param>
            void ISubMMCmdExecutor.Init(byte[] initCfgInfo, byte[] calibrationCfgInfo, ISubMMCmdExecutorCallBack callBack)
            {
                if (initCfgInfo != null && initCfgInfo.Length > 0)
                {
                    initCfg = new MeasureHeightFunctionHeadSubMachineModulesInitCfg();
                    initCfg.FromBytes(initCfgInfo);
                }
                iSubMMCmdExecutorCallBack = callBack;
                //SerialConfig serialConfig = new SerialConfig()
                //{
                //    PortName = "COM1",
                //    BaudRate = 115200,
                //    DataBits = 8,
                //    StopBits = RJCP.IO.Ports.StopBits.One,
                //    Parity = RJCP.IO.Ports.Parity.None,
                //    ModbusType = EModbusType.RS485,
                //    IsEnableCRC16 = true
                //};
                //InitParamSSZNSD33 initParamSSZNSD33 = new InitParamSSZNSD33()
                //{
                //    CommunicatorInitParam = JsonObjConvert.ToJSonBytes(serialConfig),
                //    MeasureRangeInitParam = MeasureRange.Range30
                //};
                //initCfg.MeasureHeightInitParams = JsonObjConvert.ToJSonBytes(initParamSSZNSD33);
                measureHeight = MeasureHeightFactory.CreateMeasureHeight(factoryCfg.MeasureHeightType);
                measureHeight.Init(initCfg.MeasureHeightInitParams);

            }
            
            public ICompUIDataObjPropValRW GetUIDataObjPropValRW()
            {
                return null;
            }
            void ISubMMCmdExecutor.AfterInit()
            {
                updateHeightTask = Task.Run(() =>
                {
                    while (true)
                    {
                        if (exit)
                            break;
                        Task.Delay(100);
                        iSubMMCmdExecutorCallBack.SendToMapTmlStateChanged(JsonObjConvert.ToJSon(
                            new InformInfo_StatusChanged(
                                alias.ToString(),
                                MeasureHeightFunctionHeadSubMachineModulesConst.HeightParamID,
                                measureHeight.CurrentHeight.ToString()
                                )
                            ));
                    }
                });
            }
            void ISubMMCmdExecutor.BeforeInit(GFBaseTypePropValueList devicePropValues)
            {

            }

            void ISubMMCmdExecutor.UnInit()
            {

            }

            ISubMMManualModeCmdExecutor ISubMMCmdExecutor.GetSubMMManualModeCmdExecutor()
            {
                return this;
            }

            ISubMMAutoModeCmdExecutor ISubMMCmdExecutor.GetSubMMAutoModeCmdExecutor()
            {
                return this;
            }

            /// <summary>
            /// 设置配方参数
            /// </summary>
            /// <param name="pfCfgInfo">配方参数，null表示缺省值</param>
            void ISubMMAutoModeCmdExecutor.SetPFCfgInfo(byte[] pfCfgInfo)
            {
                if (pfCfgInfo != null && pfCfgInfo.Length > 0)
                {
                    ppCfg.FromBytes(pfCfgInfo);
                }
            }

            bool ISubMMAutoModeCmdExecutor.CanStartWork(out string reasonMsg)
            {
                reasonMsg = string.Empty;
                return true;
            }

            ICompUIDataObjPropValRW ISubMMAutoModeCmdExecutor.GetUIDataObjPropValRW()
            {
                return null;
            }

            void ISubMMAutoModeCmdExecutor.SetRunMode(ImeRunMode imeRunMode)
            {

            }

            /// <summary>
            /// 通知子机械模组开始工作
            /// </summary>
            void ISubMMAutoModeCmdExecutor.StartWork()
            {
                PauseObj.Status = 2;
            }

            /// <summary>
            /// 通知子机械模组停止工作
            /// </summary>
            void ISubMMAutoModeCmdExecutor.StopWork()
            {
                PauseObj.Status = 1;
            }

            /// <summary>
            /// 通知子机械模组暂停工作
            /// </summary>
            void ISubMMAutoModeCmdExecutor.Pause()
            {
                PauseObj.Status = 1;
            }

            /// <summary>
            /// 通知子机械模组恢复工作
            /// </summary>
            void ISubMMAutoModeCmdExecutor.Resume()
            {
                PauseObj.Status = 2;
            }

            void ISubMMAutoModeCmdExecutor.BeforeSwitchPF()
            {
            }

            GFBaseTypeParamValueList ISubMMAutoModeCmdExecutor.ExecRuntimeCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                return ExecCtlCmdCore(cmdID, cmdParam);
            }

            void ISubMMAutoModeCmdExecutor.AfterStopWork()
            {
            }

            GFBaseTypeParamValueList ISubMMAutoModeCmdExecutor.ExecMethod(string methodID, GFBaseTypeParamValueList param)
            {
                return ExecMethodCore(methodID, param);
            }

            Task<GFBaseTypeParamValueList> ISubMMAutoModeCmdExecutor.AsynExecMethod(string methodID, GFBaseTypeParamValueList param)
            {
                return null;
            }

            GFParamValueList ISubMMAutoModeCmdExecutor.ExecMethod(string methodID, GFParamValueList param)
            {
                return null;
            }

            Task<GFParamValueList> ISubMMAutoModeCmdExecutor.AsynExecMethod(string methodID, GFParamValueList param)
            {
                return null;
            }

            GFBaseTypeParamValueList ISubMMAutoModeCmdExecutor.ExecCabilityMethod(string methodID, GFBaseTypeParamValueList param)
            {
                return ExecMethodCore(methodID, param);
            }

            Task<GFBaseTypeParamValueList> ISubMMAutoModeCmdExecutor.AsynExecCabilityMethod(string methodID, GFBaseTypeParamValueList param)
            {
                return null;
            }

            GFBaseTypeParamValueList ISubMMManualModeCmdExecutor.ExecCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                return ExecCtlCmdCore(cmdID, cmdParam);
            }

            GFBaseTypeParamValueList ExecCtlCmdCore(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                var rst = new GFBaseTypeParamValueList();
                rst.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue("")));
                rst.Add(new GFBaseTypeParamValue("errorMsg", new GriffinsBaseValue("")));
                string errorMsg = "";

                switch (cmdID)
                {
                    case MeasureHeightFunctionHeadSubMachineModulesConst.MeasureHeightMethodID:
                        {
                            double height = measureHeight.ReadHeight();
                            rst["Result"] = new GriffinsBaseValue(height);
                        }
                        break;
                    default:
                        break;
                }

                rst["errorMsg"] = new GriffinsBaseValue(errorMsg);
                return rst;
            }

            GFBaseTypeParamValueList ExecMethodCore(string methodID, GFBaseTypeParamValueList param)
            {
                GFBaseTypeParamValueList rst = new GFBaseTypeParamValueList();
                rst.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue("0")));
                rst.Add(new GFBaseTypeParamValue("errorMsg", new GriffinsBaseValue("0")));
                string jsParam = param["jsParam"].ToJsonStrValue();
                string errorMsg = "";

                switch (methodID)
                {
                    default:
                        break;
                }

                rst["errorMsg"] = new GriffinsBaseValue(errorMsg);
                return rst;
            }
        }
    }
}