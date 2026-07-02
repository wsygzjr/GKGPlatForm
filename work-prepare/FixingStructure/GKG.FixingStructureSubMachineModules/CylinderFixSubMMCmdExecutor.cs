using GF_Gereric;
using GKG.ElectronicControl;
using GKG.ElectronicControl.General;
using Griffins;
using Griffins.ImeIOT;
using Griffins.PF.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GKG.SubMM
{
    internal class CylinderFixSubMMCmdExecutor : ISubMMCmdExecutor, ISubMMManualModeCmdExecutor, ISubMMAutoModeCmdExecutor
    {
        private CylinderFixSubMachineModulesFactoryCfg jackingFixSubMachineModulesFactoryCfg;
        private CylinderFixSubMachineModulesInitCfg jackingFixSubMachineModulesInitCfg;
        private CylinderFixSubMachineModulesPPCfg jackingFixSubMachineModulesPPCfg;
        private ISubMMCmdExecutorCallBack? iSubMMCmdExecutorCallBack;
        private readonly SubMMAlias alias;
        private ImeGenNormalEventHandler? imeGenNormalEventHandler;
        private ImeCabilityEventHandler? imeCabilityEventHandler;
        private ImeAlarmEventHandler? imeAlarmEventHandler;
        private IBaseCylinder? fixingCylinder;
        private bool isInitialized;
        private bool isFixed;

        public CylinderFixSubMMCmdExecutor(SubMMAlias alias, byte[] factoryCfgInfo)
        {
            this.alias = alias;
            jackingFixSubMachineModulesFactoryCfg = new CylinderFixSubMachineModulesFactoryCfg();
            jackingFixSubMachineModulesFactoryCfg.FromBytes(factoryCfgInfo);
            jackingFixSubMachineModulesInitCfg = new CylinderFixSubMachineModulesInitCfg();
            jackingFixSubMachineModulesPPCfg = new CylinderFixSubMachineModulesPPCfg();
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

        void ISubMMCmdExecutor.BeforeInit(GFBaseTypePropValueList devicePropValues)
        {
        }

        void ISubMMCmdExecutor.Init(byte[] initCfgInfo, byte[] calibrationCfgInfo, ISubMMCmdExecutorCallBack callBack)
        {
            jackingFixSubMachineModulesFactoryCfg ??= new CylinderFixSubMachineModulesFactoryCfg();
            jackingFixSubMachineModulesInitCfg ??= new CylinderFixSubMachineModulesInitCfg();
            jackingFixSubMachineModulesPPCfg ??= new CylinderFixSubMachineModulesPPCfg();
            iSubMMCmdExecutorCallBack = callBack;

            if (initCfgInfo != null && initCfgInfo.Length > 0)
                jackingFixSubMachineModulesInitCfg.FromBytes(initCfgInfo);

            byte[]? factoryCfgBytes = iSubMMCmdExecutorCallBack?.GetFactoryCfgInfo();
            if (factoryCfgBytes != null && factoryCfgBytes.Length > 0)
                jackingFixSubMachineModulesFactoryCfg.FromBytes(factoryCfgBytes);

        }

        void ISubMMCmdExecutor.AfterInit()
        {
            EnsureFixingCylinderInitialized();
        }

        void ISubMMCmdExecutor.UnInit()
        {
            if (fixingCylinder != null)
            {
                fixingCylinder.StretchFinished -= FixingCylinder_Finished;
                fixingCylinder.RetractFinished -= FixingCylinder_Finished;
                fixingCylinder = null;
            }

            isInitialized = false;
            isFixed = false;
        }

        ISubMMManualModeCmdExecutor ISubMMCmdExecutor.GetSubMMManualModeCmdExecutor()
        {
            return this;
        }

        ISubMMAutoModeCmdExecutor ISubMMCmdExecutor.GetSubMMAutoModeCmdExecutor()
        {
            return this;
        }

        void ISubMMAutoModeCmdExecutor.SetPFCfgInfo(byte[] pfCfgInfo)
        {
            if (pfCfgInfo != null && pfCfgInfo.Length > 0)
                jackingFixSubMachineModulesPPCfg.FromBytes(pfCfgInfo);
        }

        void ISubMMAutoModeCmdExecutor.SetRunMode(ImeRunMode imeRunMode)
        {
        }

        void ISubMMAutoModeCmdExecutor.StartWork()
        {
            PauseObj.Status = 2;
        }

        void ISubMMAutoModeCmdExecutor.StopWork()
        {
            PauseObj.Status = 1;
        }

        void ISubMMAutoModeCmdExecutor.Pause()
        {
            PauseObj.Status = 1;
        }

        void ISubMMAutoModeCmdExecutor.Resume()
        {
            PauseObj.Status = 2;
        }

        void ISubMMAutoModeCmdExecutor.BeforeSwitchPF()
        {
        }

        void ISubMMAutoModeCmdExecutor.AfterStopWork()
        {
        }

        GFBaseTypeParamValueList ISubMMAutoModeCmdExecutor.ExecMethod(string methodID, GFBaseTypeParamValueList param)
        {
            return ExecMethodCore(methodID);
        }

        Task<GFBaseTypeParamValueList> ISubMMAutoModeCmdExecutor.AsynExecMethod(string methodID, GFBaseTypeParamValueList param)
        {
            return Task.Run(() => ExecMethodCore(methodID));
        }

        GFParamValueList ISubMMAutoModeCmdExecutor.ExecMethod(string methodID, GFParamValueList param)
        {
            return new GFParamValueList();
        }

        Task<GFParamValueList> ISubMMAutoModeCmdExecutor.AsynExecMethod(string methodID, GFParamValueList param)
        {
            return Task.Run(() => new GFParamValueList());
        }

        GFBaseTypeParamValueList ISubMMAutoModeCmdExecutor.ExecCabilityMethod(string methodID, GFBaseTypeParamValueList param)
        {
            return ExecMethodCore(methodID);
        }

        Task<GFBaseTypeParamValueList> ISubMMAutoModeCmdExecutor.AsynExecCabilityMethod(string methodID, GFBaseTypeParamValueList param)
        {
            return Task.Run(() => ExecMethodCore(methodID));
        }

        GFBaseTypeParamValueList ISubMMAutoModeCmdExecutor.ExecRuntimeCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
        {
            return ExecCtlCmdCore(cmdID);
        }

        GFBaseTypeParamValueList ISubMMManualModeCmdExecutor.ExecCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
        {
            return ExecCtlCmdCore(cmdID);
        }

        ICompUIDataObjPropValRW ISubMMAutoModeCmdExecutor.GetUIDataObjPropValRW()
        {
            return null!;
        }

        bool ISubMMAutoModeCmdExecutor.CanStartWork(out string reasonMsg)
        {
            reasonMsg = string.Empty;
            return true;
        }

        private GFBaseTypeParamValueList ExecMethodCore(string methodID)
        {
            GFBaseTypeParamValueList rst = new GFBaseTypeParamValueList();
            switch (methodID)
            {
                case FixingStructureSubMachineModulesConst.FixingMethodID:
                    Fixing();
                    break;
                case FixingStructureSubMachineModulesConst.ReleaseFixingMethodID:
                    ReleaseFixing();
                    break;
                default:
                    break;
            }

            return rst;
        }

        private GFBaseTypeParamValueList ExecCtlCmdCore(string cmdID)
        {
            GFBaseTypeParamValueList rst = new GFBaseTypeParamValueList();
            switch (cmdID)
            {
                case FixingStructureSubMachineModulesConst.RtCmdGetFixingState:
                    rst.Add(new GFBaseTypeParamValue(FixingStructureSubMachineModulesConst.FixingState, new GriffinsBaseValue(fixingCylinder.CylinderPosType == ECylinderPosType.Stretch)));
                    break;
                case FixingStructureSubMachineModulesConst.RtCmdGetIOOptions:
                    {
                        var ioStateInfosResponse = ServerInnerInfoSender.SendMutualInfo(
                            IOStateInfosRequest.InfoKindID,
                            new IOStateInfosRequest());

                        if (ioStateInfosResponse == null || ioStateInfosResponse.Count == 0)
                        {
                            rst.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue(JsonObjConvert.ToJSon(new List<IOStateInformation>()))));
                            break;
                        }

                        IOStateInfosResponse? response = ioStateInfosResponse[0].Response as IOStateInfosResponse;
                        if (response?.IOStateInformations == null)
                        {
                            rst.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue(JsonObjConvert.ToJSon(new List<IOStateInformation>()))));
                            break;
                        }

                        rst.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue(JsonObjConvert.ToJSon(response?.IOStateInformations))));
                    }
                    break;
                case FixingStructureSubMachineModulesConst.RtCmdGetAxisOptions:
                    {
                        var axisInfosResponse = ServerInnerInfoSender.SendMutualInfo(
                            AxisInfosRequest.InfoKindID,
                            new AxisInfosRequest());

                        if (axisInfosResponse == null || axisInfosResponse.Count == 0)
                        {
                            rst.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue(JsonObjConvert.ToJSon(new List<IOStateInformation>()))));
                            break;
                        }

                        AxisInfosResponse? response = axisInfosResponse[0].Response as AxisInfosResponse;
                        if (response?.AxisInformations == null)
                        {
                            rst.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue(JsonObjConvert.ToJSon(new List<IOStateInformation>()))));
                            break;
                        }

                        rst.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue(JsonObjConvert.ToJSon(response?.AxisInformations))));
                    }
                    break;
                case FixingStructureSubMachineModulesConst.FixingMethodID:
                    Fixing();
                    break;
                case FixingStructureSubMachineModulesConst.ReleaseFixingMethodID:
                    ReleaseFixing();
                    break;
                default:
                    break;
            }

            return rst;
        }

        private void EnsureFixingCylinderInitialized()
        {
            if (isInitialized)
                return;

            CylinderInitParameters fixingCylinderParams = jackingFixSubMachineModulesInitCfg.FixingCylinderParams;
            if (fixingCylinderParams == null || fixingCylinderParams.IOStateGuidList == null || fixingCylinderParams.IOStateGuidList.Count == 0)
                throw new InvalidOperationException("固定气缸未配置 IOStateGuidList，无法完成初始化。");

            List<IBaseStateIO> ioInstances = ElectronicFunc.GetStateIOInstancesByIds(fixingCylinderParams.IOStateGuidList);
            fixingCylinder = CylinderFactory.CreateCylinder(fixingCylinderParams.eCylinderType);
            fixingCylinder.Init(JsonObjConvert.ToJSonBytes(fixingCylinderParams));
            fixingCylinder.SetStateIOInstanceList(ioInstances);
            fixingCylinder.StretchFinished += FixingCylinder_Finished;
            fixingCylinder.RetractFinished += FixingCylinder_Finished;

            isFixed = fixingCylinder.CylinderPosType == ECylinderPosType.Stretch;
            isInitialized = true;
        }

        private void FixingCylinder_Finished(object? sender, EventArgs e)
        {
            if(isFixed == true)
                imeCabilityEventHandler.Invoke(this, new ImeCabilityEventArgs(FixingStructureSubMachineModulesConst.FixingFinishedEventID, new GFBaseTypeParamValueList()));
            else
                imeCabilityEventHandler.Invoke(this, new ImeCabilityEventArgs(FixingStructureSubMachineModulesConst.ReleaseFixingFinishedEventID, new GFBaseTypeParamValueList()));
        }

        private void Fixing()
        {
            EnsureFixingCylinderInitialized();
            isFixed = true;
            fixingCylinder?.Stretch();
        }

        private void ReleaseFixing()
        {
            EnsureFixingCylinderInitialized();
            isFixed = false;
            fixingCylinder?.Retract();
        }
    }
}
