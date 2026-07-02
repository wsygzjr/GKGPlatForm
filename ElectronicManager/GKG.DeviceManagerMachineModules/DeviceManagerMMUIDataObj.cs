using GF_Gereric;
using GKG.MM;
using Griffins;
using Griffins.ImeIOT;
using Newtonsoft.JsonG.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG.MM
{
    public class DeviceManagerMMUIDataObj : ICompUIDataObjPropValRW
    {
        private static readonly Dictionary<ImeRunMode, string> imeRunModeDic = new Dictionary<ImeRunMode, string>
        {
            { ImeRunMode.WorkMode, "工作模式" },
            { ImeRunMode.ConfigMode, "配置模式" },
            { ImeRunMode.AgingMode, "老化模式" },
        };
        private static readonly Dictionary<string, ImeRunMode> imeRunModeStrDic = new Dictionary<string, ImeRunMode>
        {
            { "工作模式", ImeRunMode.WorkMode },
            { "配置模式", ImeRunMode.ConfigMode },
            { "老化模式", ImeRunMode.AgingMode },
        };
        private static readonly Dictionary<ImeExecMode, string> execModeDic = new Dictionary<ImeExecMode, string>
        {
            { ImeExecMode.Unknown, "未知" },
            { ImeExecMode.Continuous, "连续执行" },
            { ImeExecMode.SingleStep, "单步执行" },
            { ImeExecMode.SingleCycle, "单工艺流程周期执行" },
        };
        private static readonly Dictionary<string, ImeExecMode> execModeStrDic = new Dictionary<string, ImeExecMode>
        {
            { "未知", ImeExecMode.Unknown },
            { "连续执行", ImeExecMode.Continuous },
            { "单步执行", ImeExecMode.SingleStep },
            { "单工艺流程周期执行", ImeExecMode.SingleCycle },
        };
        private DeviceManagerMMCmdExecutor _deviceManagerMMCmdExecutor;
        private DeviceManagerStatus _deviceManagerStatus = new DeviceManagerStatus();
        private IImeDevMTRunMng imeDevMTRunMng;
        public DeviceManagerMMUIDataObj(DeviceManagerMMCmdExecutor deviceManagerMMCmdExecutor)
        {
            _deviceManagerMMCmdExecutor = deviceManagerMMCmdExecutor;
            SvrImeIOTCallSvrProvider svrProvider = new SvrImeIOTCallSvrProvider();
            IImeDevRunMng imeDevCfgMng = svrProvider.GetImeDevRunMng();
            imeDevMTRunMng = imeDevCfgMng.GetImeDevMTRunMng(1);
            RunModeList runModeList = new RunModeList();
            foreach (var runMode in imeRunModeDic.Values) 
            {
                runModeList.Add(runMode);
            }
            _deviceManagerStatus.RunModeList["RunModeList"] = runModeList;
            _deviceManagerStatus.FormulaNumberList["FormulaNumberList"] = new FormulaNumberList();
        }

        event ImePropValChangedEventHandler uIDataObjPropValChangedEvent;
        event ImePropValChangedEventHandler ICompUIDataObjPropValRW.UIDataObjPropValChangedEvent
        {
            add
            {
                this.uIDataObjPropValChangedEvent += value;
            }

            remove
            {
                this.uIDataObjPropValChangedEvent -= value;
            }
        }

        GFBaseTypeParamValueList ICompUIDataObjPropValRW.ExecUIDataObjCommand(string cmdID, GFBaseTypeParamValueList cmdParam)
        {
            ISvrObjCallForMMProcessClient svrObjCallForMMProcessClient = _deviceManagerMMCmdExecutor._callBack.CreateSvrObjCallForMMProcess(ImeIOTConst.SERVERKINDID, ImeIOTConst.ServerObjectID);
            GFBaseTypeParamValueList rtn = new GFBaseTypeParamValueList();
            rtn.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue(0)));
            string result = "";
            string errorMsg = "";
            svrObjCallForMMProcessClient.Open();
            try
            {
                switch (cmdID)
                {
                    case DeviceManagerMachineModulesConst.MachineStartWork:
                        rtn["Result"] = new GriffinsBaseValue(svrObjCallForMMProcessClient.ExecCmd(SvrForMMProcessCmd.CmdID_StartWork, JsonObjConvert.ToJSon(new SvrForMMProcessCmd.StartWork_Param() { MTNo = 1 }), out result, out errorMsg));
                        break;
                    case DeviceManagerMachineModulesConst.MachineStopWork:
                        rtn["Result"] = new GriffinsBaseValue(svrObjCallForMMProcessClient.ExecCmd(SvrForMMProcessCmd.CmdID_StopWork, JsonObjConvert.ToJSon(new SvrForMMProcessCmd.StopWork_Param() { MTNo = 1 }), out result, out errorMsg));
                        break;
                    case SvrForMMProcessCmd.CmdID_GetCurFormulaNumber:
                        rtn["Result"] = new GriffinsBaseValue(svrObjCallForMMProcessClient.ExecCmd(SvrForMMProcessCmd.CmdID_GetCurFormulaNumber, JsonObjConvert.ToJSon(new SvrForMMProcessCmd.GetCurFormulaNumber_Param() { MTNo = 1 }), out result, out errorMsg));
                        SvrForMMProcessCmd.GetCurFormulaNumber_Response response = JsonObjConvert.FromJSon<SvrForMMProcessCmd.GetCurFormulaNumber_Response>(result);
                        _deviceManagerStatus.CurFormulaNumber = response.FormulaNumber.ToString();
                        break;
                    case SvrForMMProcessCmd.CmdID_SetCurFormulaNumber:
                        _deviceManagerStatus.CurFormulaNumber = cmdParam["RecipeName"].ToStringVal();
                        rtn["Result"] = new GriffinsBaseValue(svrObjCallForMMProcessClient.ExecCmd(SvrForMMProcessCmd.CmdID_SetCurFormulaNumber, JsonObjConvert.ToJSon(new SvrForMMProcessCmd.SetCurFormulaNumber_Param() { MTNo = 1, FormulaNumber = new FormulaNumber(_deviceManagerStatus.CurFormulaNumber) }), out result, out errorMsg));
                        invokeUIDataObjPropValChangedEvent(new ObjInstPropPath(new string[] { nameof(DeviceManagerStatus.CurFormulaNumber) }), cmdParam["RecipeName"]);
                        break;
                    case SvrForMMProcessCmd.CmdID_GetAllFormulaNumberes:
                        svrObjCallForMMProcessClient.ExecCmd(SvrForMMProcessCmd.CmdID_GetAllFormulaNumberes, JsonObjConvert.ToJSon(new SvrForMMProcessCmd.GetAllFormulaNumberes_Param() { MTNo = 1 }), out result, out errorMsg);
                        var AllFormulaNumberes = JsonObjConvert.FromJSon<SvrForMMProcessCmd.GetAllFormulaNumberes_Response>(result);
                        _deviceManagerStatus.FormulaNumberList["FormulaNumberList"].Clear();
                        foreach (var formulaNumber in AllFormulaNumberes.FormulaNumberes)
                        {
                            _deviceManagerStatus.FormulaNumberList["FormulaNumberList"].Add(formulaNumber.ToString());
                        }
                        rtn["Result"] = ((IGriffinsBaseValue)_deviceManagerStatus.FormulaNumberList["FormulaNumberList"]).ToBaseValue();
                        break;
                    case DeviceManagerMachineModulesConst.SetExecMode:
                        {
                            _deviceManagerStatus.ExecMode = (ImeExecMode)cmdParam[DeviceManagerMachineModulesConst.ExecMode].ToInteger();
                            imeDevMTRunMng.SetExecMode(_deviceManagerStatus.ExecMode);
                        }
                        break;
                    case DeviceManagerMachineModulesConst.NextStep:
                        {
                            imeDevMTRunMng.NextStep();
                        }
                        break;
                }
            }
            finally
            {
                svrObjCallForMMProcessClient.Close();
            }
            return rtn;
        }

        void ICompUIDataObjPropValRW.SetUIDataObjPropPathValue(ObjInstPropPath objInstPropPath, GriffinsBaseValue value)
        {
            ISvrObjCallForMMProcessClient svrObjCallForMMProcessClient = _deviceManagerMMCmdExecutor._callBack.CreateSvrObjCallForMMProcess(ImeIOTConst.SERVERKINDID, ImeIOTConst.ServerObjectID);
            if(objInstPropPath.PropIDPath.Length == 1)
            {
                switch (objInstPropPath.PropIDPath[0])
                {
                    case nameof(DeviceManagerStatus.ImeRunMode):
                        _deviceManagerStatus.ImeRunMode = imeRunModeStrDic[value.ToStringVal()];
                        svrObjCallForMMProcessClient.Open();
                        try
                        {
                            svrObjCallForMMProcessClient.ExecCmd(SvrForMMProcessCmd.CmdID_SetRunMode, JsonObjConvert.ToJSon(new SvrForMMProcessCmd.SetRunMode_Param() { ImeRunMode = _deviceManagerStatus.ImeRunMode }), out string result, out string errorMsg);

                        }
                        finally
                        {
                            svrObjCallForMMProcessClient.Close();
                        }
                        invokeUIDataObjPropValChangedEvent(new ObjInstPropPath(new string[] { nameof(DeviceManagerStatus.ImeRunMode) }), value);
                        break;
                    case nameof(DeviceManagerStatus.CurFormulaNumber):
                        _deviceManagerStatus.CurFormulaNumber = value.ToStringVal();
                        svrObjCallForMMProcessClient.Open();
                        try
                        {
                            svrObjCallForMMProcessClient.ExecCmd(SvrForMMProcessCmd.CmdID_SetCurFormulaNumber, JsonObjConvert.ToJSon(new SvrForMMProcessCmd.SetCurFormulaNumber_Param() { MTNo = 1, FormulaNumber = new FormulaNumber(_deviceManagerStatus.CurFormulaNumber) }), out string result, out string errorMsg);

                        }
                        finally
                        {
                            svrObjCallForMMProcessClient.Close();
                        }
                        invokeUIDataObjPropValChangedEvent(new ObjInstPropPath(new string[] { nameof(DeviceManagerStatus.CurFormulaNumber) }), value);
                        break;
                    case nameof(DeviceManagerStatus.ExecMode):
                        _deviceManagerStatus.ExecMode = Enum.Parse<ImeExecMode>(value.ToStringVal());
                        imeDevMTRunMng.SetExecMode(_deviceManagerStatus.ExecMode);
                        invokeUIDataObjPropValChangedEvent(new ObjInstPropPath(new string[] { nameof(DeviceManagerStatus.ExecMode) }), value);
                        break;
                }
            }

        }

        void ICompUIDataObjPropValRW.SetUIDataObjPropPathValues(GFBaseTypeObjPropPathValueList values)
        {
            foreach(var item in values)
            {
                ((ICompUIDataObjPropValRW)this).SetUIDataObjPropPathValue(item.ObjInstPropPath, item.Value);
            }
        }

        private GriffinsBaseValue getUIDataObjPropPathValue(GFBaseTypePropValueList allValues, ObjInstPropPath objInstPropPath)
        {
            foreach (GFBaseTypePropValue item in allValues)
            {
                GFBaseTypeObjPropPathValue target = item.GetLeafGFBaseTypeObjPropPathValues().Find(objInstPropPath);
                if (target != null)
                    return target.Value;
            }

            return null;
        }

        GriffinsBaseValue ICompUIDataObjPropValRW.GetUIDataObjPropPathValue(ObjInstPropPath objInstPropPath)
        {
            GFBaseTypePropValueList allValues = GetAllUIDataObjPropValues();
            return getUIDataObjPropPathValue(allValues, objInstPropPath);
        }

        GFBaseTypeObjPropPathValueList ICompUIDataObjPropValRW.GetUIDataObjPropPathValues(ObjInstPropPath[] objInstPropPaths)
        {
            GFBaseTypeObjPropPathValueList result = new GFBaseTypeObjPropPathValueList();
            GFBaseTypePropValueList allValues = GetAllUIDataObjPropValues();
            foreach (var path in objInstPropPaths)
            {
                var value = getUIDataObjPropPathValue(allValues, path);
                if (value != null)
                {
                    result.Add(new GFBaseTypeObjPropPathValue()
                    {
                        ObjInstPropPath = path,
                        Value = value
                    });
                }
            }
            return result;
        }

        GFBaseTypeObjPropPathValueList ICompUIDataObjPropValRW.GetAllUIDataObjPropPathValues()
        {
            return GetAllUIDataObjPropValues().GetLeafGFBaseTypeObjPropPathValues();
        }

        private GFBaseTypePropValueList GetAllUIDataObjPropValues()
        {
            ISvrObjCallForMMProcessClient svrObjCallForMMProcessClient = _deviceManagerMMCmdExecutor._callBack.CreateSvrObjCallForMMProcess(ImeIOTConst.SERVERKINDID, ImeIOTConst.ServerObjectID);
            GFBaseTypePropValueList allUIDataObjPropValues = new GFBaseTypePropValueList();
            string result = "";
            string errorMsg = "";
            svrObjCallForMMProcessClient.Open();
            try
            {
                svrObjCallForMMProcessClient.ExecCmd(SvrForMMProcessCmd.CmdID_GetAllFormulaNumberes, JsonObjConvert.ToJSon(new SvrForMMProcessCmd.GetAllFormulaNumberes_Param() { MTNo = 1 }), out result, out errorMsg);
                var AllFormulaNumberes = JsonObjConvert.FromJSon<SvrForMMProcessCmd.GetAllFormulaNumberes_Response>(result);
                _deviceManagerStatus.FormulaNumberList["FormulaNumberList"].Clear();
                foreach (var formulaNumber in AllFormulaNumberes.FormulaNumberes)
                {
                    _deviceManagerStatus.FormulaNumberList["FormulaNumberList"].Add(formulaNumber.ToString());
                }
                allUIDataObjPropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DeviceManagerStatus.FormulaNumberList)), ((IGriffinsBaseValue)_deviceManagerStatus.FormulaNumberList["FormulaNumberList"]).ToBaseValue()));

                svrObjCallForMMProcessClient.ExecCmd(SvrForMMProcessCmd.CmdID_GetCurFormulaNumber, JsonObjConvert.ToJSon(new SvrForMMProcessCmd.GetCurFormulaNumber_Param() { MTNo = 1 }), out result, out errorMsg);
                SvrForMMProcessCmd.GetCurFormulaNumber_Response response = JsonObjConvert.FromJSon<SvrForMMProcessCmd.GetCurFormulaNumber_Response>(result);
                _deviceManagerStatus.CurFormulaNumber = response.FormulaNumber.ToString();
                allUIDataObjPropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DeviceManagerStatus.CurFormulaNumber)), new GriffinsBaseValue(_deviceManagerStatus.CurFormulaNumber)));

                svrObjCallForMMProcessClient.ExecCmd(SvrForMMProcessCmd.CmdID_GetRunMode, JsonObjConvert.ToJSon(new SvrForMMProcessCmd.GetRunMode_Param()), out result, out errorMsg);
                SvrForMMProcessCmd.GetRunMode_Response runModeResponse = JsonObjConvert.FromJSon<SvrForMMProcessCmd.GetRunMode_Response>(result);
                _deviceManagerStatus.ImeRunMode = runModeResponse.ImeRunMode;
                allUIDataObjPropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DeviceManagerStatus.ImeRunMode)), new GriffinsBaseValue(imeRunModeDic[_deviceManagerStatus.ImeRunMode])));

                _deviceManagerStatus.ExecMode = imeDevMTRunMng.GetCurExecMode();

            }
            catch(Exception e)
            {

            }
            finally
            {
                svrObjCallForMMProcessClient.Close();
            }
            _deviceManagerStatus.ExecMode = ImeExecMode.Continuous;
            //allUIDataObjPropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DeviceManagerStatus.ImeRunMode)), new GriffinsBaseValue(imeRunModeDic[_deviceManagerStatus.ImeRunMode])));
            allUIDataObjPropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DeviceManagerStatus.RunModeList)), ((IGriffinsBaseValue)_deviceManagerStatus.RunModeList["RunModeList"]).ToBaseValue()));
            allUIDataObjPropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DeviceManagerStatus.ExecMode)), new GriffinsBaseValue(_deviceManagerStatus.ExecMode)));
            return allUIDataObjPropValues;
        }
        private void invokeUIDataObjPropValChangedEvent(ObjInstPropPath propertyID, GriffinsBaseValue value)
        {
            uIDataObjPropValChangedEvent.Invoke(this, new ImePropValChangedEventArgs(propertyID, value, DateTime.Now));
        }

        //public void UpdateUIDataObjProps()
        //{
        //    foreach (var propPathValue in GetAllUIDataObjPropValues().GetLeafGFBaseTypeObjPropPathValues())
        //    {
        //        invokeUIDataObjPropValChangedEvent(propPathValue.ObjInstPropPath, propPathValue.Value);
        //    }
        //}
    }
}
