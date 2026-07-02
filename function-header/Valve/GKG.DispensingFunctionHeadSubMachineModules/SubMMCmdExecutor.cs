using GF_Gereric;
using Griffins.ImeIOT;
using GKG.ElectronicControl.Dispenser;
using Griffins;
using System;
using GKG.ElectronicControl;
using System.Threading.Tasks;

namespace GKG.SubMM.Dispenser
{
    public class DispensingFunctionHeadSubMMCmdExecutor : ISubMMCmdExecutor, ISubMMManualModeCmdExecutor, ISubMMAutoModeCmdExecutor
    {
        private DispensingFunctionHeadSubMachineModulesFactoryCfg factoryCfg;
        private DispensingFunctionHeadSubMachineModulesInitCfg initCfg;

        private DispensingFunctionHeadSubMachineModulesPPCfg ppCfg;

        private ISubMMCmdExecutorCallBack iSubMMCmdExecutorCallBack;

        private SubMMAlias alias;

        private ImeGenNormalEventHandler imeGenNormalEventHandler;

        private ImeCabilityEventHandler imeCabilityEventHandler;

        private ImeAlarmEventHandler imeAlarmEventHandler;

        private IValveBase valve;

        private IGlueDispensingDeviceBase glueDispensingDevic;

        public DispensingFunctionHeadSubMMCmdExecutor(SubMMAlias alias, byte[] factoryCfgInfo)
        {
            this.alias = alias;
            factoryCfg = new DispensingFunctionHeadSubMachineModulesFactoryCfg();
            if(factoryCfgInfo != null && factoryCfgInfo.Length > 0)
            {
                factoryCfg.FromBytes(factoryCfgInfo);
            }
            initCfg = new DispensingFunctionHeadSubMachineModulesInitCfg();
            ppCfg = new DispensingFunctionHeadSubMachineModulesPPCfg();
        }

        ~DispensingFunctionHeadSubMMCmdExecutor()
        {
            if (glueDispensingDevic != null)
            {
                glueDispensingDevic.EquipmentException -= GlueDispensingDevice_EquipmentException;
                glueDispensingDevic.LowRemainingGlueTimeAlarm -= GlueDispensingDevice_EquipmentException;
                glueDispensingDevic.LowRemainingGlueWeightAlarm -= GlueDispensingDevice_EquipmentException;
                glueDispensingDevic.LowRemainingGluePcsAlarm -= GlueDispensingDevice_EquipmentException;
                glueDispensingDevic.GluePressureOutOfRangeAlarm -= GlueDispensingDevice_EquipmentException;
                glueDispensingDevic.GlueEmptyAlarm -= GlueDispensingDevice_EquipmentException;
                glueDispensingDevic.GlueLevelChanged -= GlueDispensingDevice_GlueLevelChanged;
                glueDispensingDevic.Dispose();
            }
            if (valve != null)
            {
                valve.Dispose();
            }
        }

        event ImeGenNormalEventHandler ISubMMAutoModeCmdExecutor.GenNormalEvent
        {
            add { imeGenNormalEventHandler += value; }
            remove { imeGenNormalEventHandler -= value; }
        }

        event ImeCabilityEventHandler ISubMMAutoModeCmdExecutor.CabilityEvent
        {
            add { imeCabilityEventHandler += value; }
            remove { imeCabilityEventHandler -= value; }
        }

        event ImeAlarmEventHandler ISubMMAutoModeCmdExecutor.AlarmEvent
        {
            add { imeAlarmEventHandler += value; }
            remove { imeAlarmEventHandler -= value; }
        }

        /// <summary>
        /// 工作状态
        /// </summary>
        public WorkState WorkStatus { get; set; } = WorkState.Idle;

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
            if(initCfgInfo != null && initCfgInfo.Length > 0)
            {
                initCfg = new DispensingFunctionHeadSubMachineModulesInitCfg();
                initCfg.FromBytes(initCfgInfo);
            }
            iSubMMCmdExecutorCallBack = callBack;

            valve = ValveFactory.CreateValve(factoryCfg.ValveType);
            initCfg.ValveInitParams = JsonObjConvert.ToJSonBytes(new GKGPiezoValveInitParams()
            {
                Name = "MainValve",
                ID = "112233",
                CommunicatorParams = new byte[0],
                PressureControlParams = new byte[0],
                ValveAlarmIOInitParams = new byte[0],
                ValveAlarmIOParams = new byte[0],
                Channel = 0,
                OpenValveTimeMs = 1.5,
                CloseValveTimeMs = 1.5,
                AlarmCountDetect = 100,
                IsDetectAlarmCount = false,
                ManualCleaningOfTheNozzleTimeS = 6000,
                IsManualCleaning = false
            });
            valve.Init(initCfg.ValveInitParams);

            glueDispensingDevic = GlueDispensingDeviceFactory.CreateGlueDispensingDevice(factoryCfg.GlueDispensingDeviceType);
            glueDispensingDevic.Init(initCfg.GlueDispensingDeviceInitParams);
            glueDispensingDevic.EquipmentException += GlueDispensingDevice_EquipmentException;
            glueDispensingDevic.LowRemainingGlueTimeAlarm += GlueDispensingDevice_EquipmentException;
            glueDispensingDevic.LowRemainingGlueWeightAlarm += GlueDispensingDevice_EquipmentException;
            glueDispensingDevic.LowRemainingGluePcsAlarm += GlueDispensingDevice_EquipmentException;
            glueDispensingDevic.GluePressureOutOfRangeAlarm += GlueDispensingDevice_EquipmentException;
            glueDispensingDevic.GlueEmptyAlarm += GlueDispensingDevice_EquipmentException;
            glueDispensingDevic.GlueLevelChanged += GlueDispensingDevice_GlueLevelChanged;
        }

        void ISubMMCmdExecutor.BeforeInit(GFBaseTypePropValueList devicePropValues)
        {

        }
        void ISubMMCmdExecutor.AfterInit()
        {
            
        }
        void ISubMMCmdExecutor.UnInit()
        {
            valve.Dispose();
            glueDispensingDevic.Dispose();
        }
        public ICompUIDataObjPropValRW GetUIDataObjPropValRW()
        {
            return null;
        }

        #region 运行时接口
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
        public void SetPFCfgInfo(byte[] pfCfgInfo)
        {
            if (pfCfgInfo != null && pfCfgInfo.Length > 0)
            {
                ppCfg = new DispensingFunctionHeadSubMachineModulesPPCfg();
                ppCfg.FromBytes(pfCfgInfo);
                valve.SetFormulaParams(ppCfg.GKGPiezoValveFormulaParams);
                glueDispensingDevic.SetGlueAmountParams(ppCfg.glueAmountParams);
            }
        }

        bool ISubMMAutoModeCmdExecutor.CanStartWork(out string reasonMsg)
        {
            reasonMsg = string.Empty;
            return true;
        }

        void ISubMMAutoModeCmdExecutor.SetRunMode(ImeRunMode imeRunMode)
        {

        }

        /// <summary>
        /// 通知子机械模组开始工作
        /// </summary>
        public void StartWork()
        {
            PauseObj.Status = 2;
        }

        /// <summary>
        /// 通知子机械模组停止工作
        /// </summary>
        public void StopWork()
        {
            PauseObj.Status = 1;
        }

        /// <summary>
        /// 通知子机械模组暂停工作
        /// </summary>
        public void Pause()
        {
            PauseObj.Status = 1;
        }

        /// <summary>
        /// 通知子机械模组恢复工作
        /// </summary>
        public void Resume()
        {
            PauseObj.Status = 2;
        }

        public void BeforeSwitchPF()
        {
        }

        public GFBaseTypeParamValueList ExecRuntimeCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
        {
            return ExecCtlCmdCore(cmdID, cmdParam);
        }

        public void AfterStopWork()
        {
        }

        public GFBaseTypeParamValueList ExecMethod(string methodID, GFBaseTypeParamValueList param)
        {
            GFBaseTypeParamValueList rst = new GFBaseTypeParamValueList();
            rst.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue("0")));
            rst.Add(new GFBaseTypeParamValue("errorMsg", new GriffinsBaseValue("0")));
            string jsParam = param["jsParam"].ToStringVal();
            string errorMsg = "";

            switch (methodID)
            {
                case DispensingFunctionHeadSubMachineModulesConst.CanStartAction:
                    rst["Result"] = new GriffinsBaseValue(true);
                    break;

                case DispensingFunctionHeadSubMachineModulesConst.CanStopAction:
                    rst["Result"] = new GriffinsBaseValue(true);
                    break;

                case DispensingFunctionHeadSubMachineModulesConst.OutGlue:
                    {
                        int pointCount = 0;
                        if (!string.IsNullOrEmpty(jsParam))
                        {
                            int.TryParse(jsParam, out pointCount);
                        }
                        outGlue(pointCount);
                    }
                    break;

                case DispensingFunctionHeadSubMachineModulesConst.StopGlue:
                    stopGlue();
                    break;

                case DispensingFunctionHeadSubMachineModulesConst.ChangeGlue:
                    try
                    {
                        changeGlue();
                    }
                    catch (NotImplementedException)
                    {
                        errorMsg = "Not implemented";
                    }
                    break;

                case DispensingFunctionHeadSubMachineModulesConst.RefreshGlueAmount:
                    refreshGlueAmount();
                    break;

                case DispensingFunctionHeadSubMachineModulesConst.ReadIsLackOfGlue:
                    rst["Result"] = new GriffinsBaseValue(readIsLackOfGlue());
                    break;

                case DispensingFunctionHeadSubMachineModulesConst.GetGlueAirPressure:
                    rst["Result"] = new GriffinsBaseValue(getGluePressure());
                    break;

                case DispensingFunctionHeadSubMachineModulesConst.SetGlueAirPressure:
                    if (double.TryParse(jsParam, out double p))
                    {
                        setGluePressure(p);
                    }
                    else
                    {
                        errorMsg = "invalid parameter";
                    }
                    break;

                case DispensingFunctionHeadSubMachineModulesConst.GetValveAirPressure:
                    rst["Result"] = new GriffinsBaseValue(getValvePressure());
                    break;

                case DispensingFunctionHeadSubMachineModulesConst.SetValveAirPressure:
                    if (!string.IsNullOrEmpty(jsParam))
                    {
                        var dp = JsonObjConvert.FromJSon<DetectPressureParams>(jsParam);
                        setValvePressure(dp);
                    }
                    break;

                default:
                    break;
            }

            rst["errorMsg"] = new GriffinsBaseValue(errorMsg);
            return rst;
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
            GFBaseTypeParamValueList rst = new GFBaseTypeParamValueList();
            rst.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue("0")));
            rst.Add(new GFBaseTypeParamValue("errorMsg", new GriffinsBaseValue("0")));
            string jsParam = param["jsParam"].ToJsonStrValue();
            string errorMsg = "";

            switch (methodID)
            {
                case DispensingFunctionHeadSubMachineModulesConst.StartActionMethodID:
                    outGlue();
                    break;

                case DispensingFunctionHeadSubMachineModulesConst.StopActionMethodID:
                    stopGlue();
                    break;

                default:
                    break;
            }

            rst["errorMsg"] = new GriffinsBaseValue(errorMsg);
            return rst;
        }

        Task<GFBaseTypeParamValueList> ISubMMAutoModeCmdExecutor.AsynExecCabilityMethod(string methodID, GFBaseTypeParamValueList param)
        {
            return null;
        }

        #endregion

        #region 配置时接口

        GFBaseTypeParamValueList ISubMMManualModeCmdExecutor.ExecCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
        {
            return ExecCtlCmdCore(cmdID, cmdParam);
        }

        #endregion

        #region 私有方法
        GFBaseTypeParamValueList ExecCtlCmdCore(string cmdID, GFBaseTypeParamValueList cmdParam)
        {
            var rst = new GFBaseTypeParamValueList();
            rst.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue("")));
            rst.Add(new GFBaseTypeParamValue("errorMsg", new GriffinsBaseValue("")));
            string jsParam = cmdParam["jsParam"].ToJsonStrValue();
            string errorMsg = "";

            switch (cmdID)
            {
                case "OutGlue":
                    outGlue();
                    break;

                case "StopGlue":
                    stopGlue();
                    break;

                case "SetGluePressure":
                    if (double.TryParse(jsParam, out double pressure))
                    {
                        setGluePressure(pressure);
                    }
                    else
                    {
                        errorMsg = "invalid parameter";
                    }
                    break;

                default:
                    break;
            }

            rst["Result"] = new GriffinsBaseValue(string.Empty);
            rst["errorMsg"] = new GriffinsBaseValue(errorMsg);
            return rst;
        }

        private void GlueDispensingDevice_EquipmentException(object? sender, EquipmentExceptionEventArgs e)
        {
            imeAlarmEventHandler?.Invoke(this, new ImeAlarmEventArgs(e.EventID, e.EventSeverity, DateTime.Now));
        }

        private void GlueDispensingDevice_GlueLevelChanged(object? sender, string e)
        {
            invokeGenNormalEvent(EventIDDef.GlueLevelChangedEventID, e);
        }

        private void outGlue()
        {
            invokeGenNormalEvent(EventIDDef.BeforeStartActionEventID, "true");
            valve.Open();
            WorkStatus = WorkState.Working;
            invokeCabilityEvent(DispensingFunctionHeadSubMachineModulesConst.WorkStateChanged, WorkStatus.ToString());
        }

        private void outGlue(int pointCount)
        {
            invokeGenNormalEvent(EventIDDef.BeforeStopActionEventID, "true");
            valve.Open(pointCount);
            WorkStatus = WorkState.Working;
            invokeCabilityEvent(DispensingFunctionHeadSubMachineModulesConst.WorkStateChanged, WorkStatus.ToString());
        }

        private void stopGlue()
        {
            valve.Close();
            WorkStatus = WorkState.Idle;
            invokeCabilityEvent(DispensingFunctionHeadSubMachineModulesConst.WorkStateChanged, WorkStatus.ToString());
        }

        private void changeGlue()
        {
            //glueDispensingDevic.RefreshGlueAmount();
            throw new NotImplementedException();
        }

        private void refreshGlueAmount()
        {
            glueDispensingDevic.RefreshGlueAmount();
        }

        private bool readIsLackOfGlue()
        {
            return glueDispensingDevic.ReadIsLackOfGlue();
        }

        private double getGluePressure()
        {
            return glueDispensingDevic.GetGlueAirPressure();
        }

        private void setGluePressure(double value)
        {
            glueDispensingDevic.SetGlueAirPressure(value);
        }

        private double getValvePressure()
        {
            return valve.GetValveAirPressure();
        }

        private void setValvePressure(DetectPressureParams value)
        {
            valve.SetValveAirPressure(value);
        }

        private void invokeGenNormalEvent(int id, string param)
        {
            GFBaseTypeParamValueList eventParam = new GFBaseTypeParamValueList();
            eventParam.Add(new GFBaseTypeParamValue("param", new GriffinsBaseValue(param)));
            imeGenNormalEventHandler?.Invoke(this, new ImeGenNormalEventArgs(id, eventParam));
        }

        private void invokeCabilityEvent(string id, string param)
        {
            GFBaseTypeParamValueList eventParam = new GFBaseTypeParamValueList();
            eventParam.Add(new GFBaseTypeParamValue("param", new GriffinsBaseValue(param)));
            imeCabilityEventHandler?.Invoke(this, new ImeCabilityEventArgs(id, eventParam));
        }
        #endregion
    }
}