using GF_Gereric;
using GKG.ElectronicControl;
using GKG.ElectronicControl.General;
using GKG.SubMM;
using Griffins;
using Griffins.ImeIOT;
using Griffins.PF.Server;
using Newtonsoft.JsonG.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace GKG
{
    namespace SubMM
    {
        public partial class RailWorkStationSubMMCmdExecutor : ISubMMCmdExecutor, ISubMMManualModeCmdExecutor, ISubMMAutoModeCmdExecutor, ICompUIDataObjPropValRW
        {
            #region 字段

            private RailWorkStationSubMachineModulesFactoryCfg railWorkStationSubMachineModulesFactoryCfg;

            private RailWorkStationSubMachineModulesInitCfg railWorkStationSubMachineModulesInitCfg;

            private RailWorkStationSubMachineModulesPPCfg railWorkStationSubMachineModulesPPCfg;

            private ISubMMCmdExecutorCallBack iSubMMCmdExecutorCallBack;

            private SubMMAlias alias;

            private IBaseStateIO leftSensor;

            private IBaseStateIO rightSensor;

            private IBaseCylinder leftCylinder;

            private IBaseCylinder rightCylinder;

            private IBaseStateIO proximitySensor;

            private RailWorkStationStatus railWorkStationStatus;

            private Task sensorReadTask;
            private bool disposed = false;

            private ERailWorkMode workMode = ERailWorkMode.LeftInRightOut;
            private readonly object syncRoot = new object();
            #endregion

            #region 属性

            // 当前类暂无独立属性。

            #endregion

            #region 事件

            private ImeGenNormalEventHandler imeGenNormalEventHandler;

            private ImeCabilityEventHandler imeCabilityEventHandler;
            
            private ImeAlarmEventHandler imeAlarmEventHandler;

            private ImePropValChangedEventHandler uIDataObjPropValChangedEvent;

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

            event ImePropValChangedEventHandler ICompUIDataObjPropValRW.UIDataObjPropValChangedEvent
            {
                add
                {
                    uIDataObjPropValChangedEvent += value;
                }

                remove
                {
                    uIDataObjPropValChangedEvent -= value;
                }
            }

            #endregion

            public RailWorkStationSubMMCmdExecutor(SubMMAlias alias, byte[] factoryCfgInfo)
            {
                this.alias = alias;
                railWorkStationSubMachineModulesFactoryCfg = new RailWorkStationSubMachineModulesFactoryCfg();
                railWorkStationSubMachineModulesFactoryCfg.FromBytes(factoryCfgInfo);
                railWorkStationSubMachineModulesInitCfg = new RailWorkStationSubMachineModulesInitCfg();
                railWorkStationSubMachineModulesPPCfg = new RailWorkStationSubMachineModulesPPCfg();
                railWorkStationStatus = new RailWorkStationStatus();
            }

            #region 接口实现

            void ISubMMCmdExecutor.BeforeInit(GFBaseTypePropValueList devicePropValues)
            {

            }

            void ISubMMCmdExecutor.Init(byte[] initCfgInfo, byte[] calibrationCfgInfo, ISubMMCmdExecutorCallBack callBack)
            {
                if (railWorkStationSubMachineModulesFactoryCfg == null)
                    railWorkStationSubMachineModulesFactoryCfg = new RailWorkStationSubMachineModulesFactoryCfg();
                if (railWorkStationSubMachineModulesInitCfg == null)
                    railWorkStationSubMachineModulesInitCfg = new RailWorkStationSubMachineModulesInitCfg();
                if (railWorkStationSubMachineModulesPPCfg == null)
                    railWorkStationSubMachineModulesPPCfg = new RailWorkStationSubMachineModulesPPCfg();
                if(railWorkStationStatus == null)
                    railWorkStationStatus = new RailWorkStationStatus();
                iSubMMCmdExecutorCallBack = callBack;

                if (initCfgInfo != null && initCfgInfo.Length > 0)
                {
                    railWorkStationSubMachineModulesInitCfg = new RailWorkStationSubMachineModulesInitCfg();
                    railWorkStationSubMachineModulesInitCfg.FromBytes(initCfgInfo);
                }


            }

            void ISubMMCmdExecutor.AfterInit()
            {
                BindStateIOInstancesFromElectronicManager();
                if (sensorReadTask == null)
                {
                    sensorReadTask = Task.Run(() =>
                    {
                        while (true)
                        {
                            if (disposed)
                            {
                                break;
                            }
                            try
                            {
                                StatusChanged(null, null);
                                Thread.Sleep(100);
                            }
                            catch
                            {
                                // 读取状态过程中发生异常，忽略并继续下一轮读取，避免因偶发异常导致状态读取中断。
                            }
                        }
                    });
                }
                BindEventHandler();
            }

            void ISubMMCmdExecutor.UnInit()
            {
                disposed = true;
                leftSensor = null;
                rightSensor = null;
                proximitySensor = null;
                leftCylinder = null;
                rightCylinder = null;
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
                    railWorkStationSubMachineModulesPPCfg.FromBytes(pfCfgInfo);
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
                GFBaseTypeParamValueList retParams = new GFBaseTypeParamValueList();
                switch (methodID)
                {
                    case RailWorkStationSubMachineModulesConst.GetLeftSensorStateMethodID:
                        {
                            bool leftSensorStatus = GetLeftSensorStatus();
                            retParams.Add(new GFBaseTypeParamValue(RailWorkStationSubMachineModulesConst.RetParamSensorStatus, new GriffinsBaseValue(leftSensorStatus)));
                        }
                        break;
                    case RailWorkStationSubMachineModulesConst.GetRightSensorStateMethodID:
                        {
                            bool rightSensorStatus = GetRightSensorStatus();
                            retParams.Add(new GFBaseTypeParamValue(RailWorkStationSubMachineModulesConst.RetParamSensorStatus, new GriffinsBaseValue(rightSensorStatus)));
                        }
                        break;
                    case RailWorkStationSubMachineModulesConst.ControlLeftCylinderMethodID:
                        {
                            bool extend = param[RailWorkStationSubMachineModulesConst.CmdParamCylinder].ToBool();
                            ControlLeftCylinder(extend);
                        }
                        break;
                    case RailWorkStationSubMachineModulesConst.ControlRightCylinderMethodID:
                        {
                            bool extend = param[RailWorkStationSubMachineModulesConst.CmdParamCylinder].ToBool();
                            ControlRightCylinder(extend);
                        }
                        break;
                    case RailWorkStationSubMachineModulesConst.GetProximitySensorStateMethodID:
                        {
                            bool proximitySensorStatus = GetProximitySensorStatus();
                            retParams.Add(new GFBaseTypeParamValue(RailWorkStationSubMachineModulesConst.RetParamSensorStatus, new GriffinsBaseValue(proximitySensorStatus)));
                        }
                        break;
                    case RailWorkStationSubMachineModulesConst.GetTransportSpeedMethodID:
                        {
                            try
                            {
                                var speedgear = railWorkStationSubMachineModulesInitCfg.WorkStationTransSpeedGearList.Find(railWorkStationSubMachineModulesPPCfg.WorkStationTransSpeed.TransSpeedGearID);
                                retParams.Add(new GFBaseTypeParamValue(RailWorkStationSubMachineModulesConst.RetParamTransportAcceleration, new GriffinsBaseValue(speedgear.TransAcceleration)));
                                retParams.Add(new GFBaseTypeParamValue(RailWorkStationSubMachineModulesConst.RetParamTransportSpeed, new GriffinsBaseValue(speedgear.TransSpeed)));
                            }
                            catch
                            {
                                retParams.Add(new GFBaseTypeParamValue(RailWorkStationSubMachineModulesConst.RetParamTransportAcceleration, new GriffinsBaseValue(0)));
                                retParams.Add(new GFBaseTypeParamValue(RailWorkStationSubMachineModulesConst.RetParamTransportSpeed, new GriffinsBaseValue(0)));
                            }
                        }
                        break;
                    case RailWorkStationSubMachineModulesConst.SetWorkModeMethodID:
                        {
                            workMode = Enum.Parse<ERailWorkMode>(param[RailWorkStationSubMachineModulesConst.CmdParamWorkMode].ToStringVal());
                        }
                        break;
                    default:
                        break;
                }
                return retParams;
            }

            Task<GFBaseTypeParamValueList> ISubMMAutoModeCmdExecutor.AsynExecMethod(string methodID, GFBaseTypeParamValueList param)
            {
                return Task.Run(() =>
                {
                    GFBaseTypeParamValueList result = new GFBaseTypeParamValueList();
                    return result;
                });
            }

            GFParamValueList ISubMMAutoModeCmdExecutor.ExecMethod(string methodID, GFParamValueList param)
            {
                return new GFParamValueList();
            }

            Task<GFParamValueList> ISubMMAutoModeCmdExecutor.AsynExecMethod(string methodID, GFParamValueList param)
            {
                return Task.Run(() =>
                {
                    GFParamValueList result = new GFParamValueList();
                    Thread.Sleep(10);
                    return result;
                });
            }

            GFBaseTypeParamValueList ISubMMAutoModeCmdExecutor.ExecCabilityMethod(string methodID, GFBaseTypeParamValueList param)
            {
                return new GFBaseTypeParamValueList();
            }

            Task<GFBaseTypeParamValueList> ISubMMAutoModeCmdExecutor.AsynExecCabilityMethod(string methodID, GFBaseTypeParamValueList param)
            {
                return Task.Run(() =>
                {
                    GFBaseTypeParamValueList result = new GFBaseTypeParamValueList();
                    return result;
                });
            }

            GFBaseTypeParamValueList ISubMMAutoModeCmdExecutor.ExecRuntimeCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                return ExecCtlCmdCore(cmdID, cmdParam);
            }

            GFBaseTypeParamValueList ISubMMManualModeCmdExecutor.ExecCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                return ExecCtlCmdCore(cmdID, cmdParam);
            }

            /// <summary>
            /// 获取界面数据对象属性读写接口实例，如果不支持返回nul
            /// </summary>
            /// <returns>界面数据对象属性读写接口实例</returns>
            ICompUIDataObjPropValRW ISubMMAutoModeCmdExecutor.GetUIDataObjPropValRW()
            {
                return this;
            }

            bool ISubMMAutoModeCmdExecutor.CanStartWork(out string reasonMsg)
            {
                reasonMsg = string.Empty;
                return true;
            }

            #endregion

            #region 界面数据对象属性读写接口实现

            /// <summary>
            /// 返回当前 UIDataObj 的全部属性。
            /// 当前仅维护一个属性：MaterialContainerStatus。
            /// </summary>
            public GFBaseTypePropValueList GetAllUIDataObjPropValues()
            {
                GFBaseTypePropValueList result = new GFBaseTypePropValueList();
                lock (syncRoot)
                {
                    result.Add(new GFBaseTypePropValue(
                        new MPPropertyID(RailWorkStationSubMachineModulesConst.WorkStationStatusDataObjID),
                        ((IGriffinsBaseValue)railWorkStationStatus).ToBaseValue()));
                }
                return result;
            }
            void ICompUIDataObjPropValRW.SetUIDataObjPropPathValue(ObjInstPropPath objInstPropPath, GriffinsBaseValue value)
            {

            }

            void ICompUIDataObjPropValRW.SetUIDataObjPropPathValues(GFBaseTypeObjPropPathValueList values)
            {

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

            GFBaseTypeParamValueList ICompUIDataObjPropValRW.ExecUIDataObjCommand(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                GFBaseTypeParamValueList rst = new GFBaseTypeParamValueList();
                switch (cmdID)
                {
                    case RailWorkStationSubMachineModulesConst.ControlLeftCylinderMethodID:
                        {
                            if (leftCylinder.CylinderPosType == ECylinderPosType.Stretch)
                                ControlLeftCylinder(false);
                            else
                                ControlLeftCylinder(true);
                        }
                        break;
                    case RailWorkStationSubMachineModulesConst.ControlRightCylinderMethodID:
                        {
                            if (rightCylinder.CylinderPosType == ECylinderPosType.Stretch)
                                ControlRightCylinder(false);
                            else
                                ControlRightCylinder(true);
                        }
                        break;
                    default:
                        return rst;
                }
                return rst;
            }

            private void InvokeUIDataObjPropValChangedEvent(ObjInstPropPath objInstPropPath, GriffinsBaseValue value)
            {
                uIDataObjPropValChangedEvent?.Invoke(this, new ImePropValChangedEventArgs(objInstPropPath, value, DateTime.Now));
            }

            #endregion

            #region 子机械模组普通方法

            private bool GetLeftSensorStatus()
            {
                if (leftSensor != null && railWorkStationSubMachineModulesFactoryCfg.WorkStationEleConfigParams.HasLeftSensor)
                {
                    return leftSensor.Read();
                }
                return false;
            }

            private bool GetRightSensorStatus()
            {
                if (rightSensor != null && railWorkStationSubMachineModulesFactoryCfg.WorkStationEleConfigParams.HasRightSensor)
                {
                    return rightSensor.Read();
                }
                return false;
            }

            private void ControlLeftCylinder(bool extend)
            {
                if (leftCylinder != null && railWorkStationSubMachineModulesFactoryCfg.WorkStationEleConfigParams.HasLeftBlock)
                {
                    if (extend)
                    {
                        leftCylinder.Stretch();
                    }
                    else
                    {
                        leftCylinder.Retract();
                    }
                }
            }

            private void ControlRightCylinder(bool extend)
            {
                if (rightCylinder != null && railWorkStationSubMachineModulesFactoryCfg.WorkStationEleConfigParams.HasRightBlock)
                {
                    if (extend)
                    {
                        rightCylinder.Stretch();
                    }
                    else
                    {
                        rightCylinder.Retract();
                    }
                }
            }

            private bool GetProximitySensorStatus()
            {
                if (proximitySensor != null && railWorkStationSubMachineModulesFactoryCfg.WorkStationEleConfigParams.HasProximitySensor)
                {
                    return proximitySensor.Read();
                }
                return false;
            }

            private void BindStateIOInstancesFromElectronicManager()
            {
                leftSensor = null;
                rightSensor = null;
                proximitySensor = null;
                leftCylinder = null;
                rightCylinder = null;

                WorkStationEleConfigParams eleCfg = railWorkStationSubMachineModulesFactoryCfg?.WorkStationEleConfigParams ?? new WorkStationEleConfigParams();
                WorkStationEleInitParams eleInit = railWorkStationSubMachineModulesInitCfg?.WorkStationEleInitParams ?? new WorkStationEleInitParams();

                List<Guid> ioGuids = new List<Guid>();
                int leftSensorCount = 0;
                int rightSensorCount = 0;
                int proximitySensorCount = 0;
                int leftCylinderCount = 0;
                int rightCylinderCount = 0;

                if (eleCfg.HasLeftSensor)
                {
                    if (eleInit.LeftSensorID == Guid.Empty)
                        throw new InvalidOperationException(Resources.LeftSensorIdMissing);

                    ioGuids.Add(eleInit.LeftSensorID);
                    leftSensorCount = 1;
                }

                if (eleCfg.HasRightSensor)
                {
                    if (eleInit.RightSensorID == Guid.Empty)
                        throw new InvalidOperationException(Resources.RightSensorIdMissing);

                    ioGuids.Add(eleInit.RightSensorID);
                    rightSensorCount = 1;
                }

                if (eleCfg.HasProximitySensor)
                {
                    if (eleInit.ProximitySensorID == Guid.Empty)
                        throw new InvalidOperationException(Resources.ProximitySensorIdMissing);

                    ioGuids.Add(eleInit.ProximitySensorID);
                    proximitySensorCount = 1;
                }

                if (eleCfg.HasLeftBlock)
                {
                    List<Guid> leftCylinderGuids = eleInit.LeftBlockCylinderParams?.IOStateGuidList ?? new List<Guid>();
                    if (leftCylinderGuids.Count == 0)
                        throw new InvalidOperationException(Resources.LeftCylinderIoMissing);

                    ioGuids.AddRange(leftCylinderGuids);
                    leftCylinderCount = leftCylinderGuids.Count;
                }

                if (eleCfg.HasRightBlock)
                {
                    List<Guid> rightCylinderGuids = eleInit.RightBlockCylinderParams?.IOStateGuidList ?? new List<Guid>();
                    if (rightCylinderGuids.Count == 0)
                        throw new InvalidOperationException(Resources.RightCylinderIoMissing);

                    ioGuids.AddRange(rightCylinderGuids);
                    rightCylinderCount = rightCylinderGuids.Count;
                }

                List<IBaseStateIO> ioInstances = GetStateIOInstancesByIds(ioGuids);
                int offset = 0;

                if (leftSensorCount > 0)
                    leftSensor = ioInstances[offset++];

                if (rightSensorCount > 0)
                    rightSensor = ioInstances[offset++];

                if (proximitySensorCount > 0)
                    proximitySensor = ioInstances[offset++];

                if (leftCylinderCount > 0)
                {
                    leftCylinder = CylinderFactory.CreateCylinder(eleInit.LeftBlockCylinderParams.eCylinderType);
                    leftCylinder.Init(JsonObjConvert.ToJSonBytes(eleInit.LeftBlockCylinderParams));
                    leftCylinder.SetStateIOInstanceList(ioInstances.Skip(offset).Take(leftCylinderCount).ToList());
                    offset += leftCylinderCount;
                }

                if (rightCylinderCount > 0)
                {
                    rightCylinder = CylinderFactory.CreateCylinder(eleInit.RightBlockCylinderParams.eCylinderType);
                    rightCylinder.Init(JsonObjConvert.ToJSonBytes(eleInit.RightBlockCylinderParams));
                    rightCylinder.SetStateIOInstanceList(ioInstances.Skip(offset).Take(rightCylinderCount).ToList());
                    offset += rightCylinderCount;
                }

                if (offset != ioInstances.Count)
                    throw new InvalidOperationException(Resources.RailIoBindingCountMismatch);

            }

            private void BindEventHandler()
            {
                if(railWorkStationSubMachineModulesFactoryCfg.WorkStationEleConfigParams.HasLeftBlock)
                {
                    leftCylinder.RetractFinished += StatusChanged;
                    leftCylinder.StretchFinished += StatusChanged;
                }
                if(railWorkStationSubMachineModulesFactoryCfg.WorkStationEleConfigParams.HasRightBlock)
                {
                    rightCylinder.RetractFinished += StatusChanged;
                    rightCylinder.StretchFinished += StatusChanged;
                }
            }

            private void StatusChanged(object sender, EventArgs e)
            {
                if (railWorkStationSubMachineModulesFactoryCfg.WorkStationEleConfigParams.HasLeftBlock)
                {
                    ECylinderPosType leftCylinderState = leftCylinder.CylinderPosType;
                    if(railWorkStationStatus.LeftCylinderState != leftCylinderState)
                    {
                        railWorkStationStatus.LeftCylinderState = leftCylinderState;
                        InvokeUIDataObjPropValChangedEvent(new ObjInstPropPath(
                            new string[] { nameof(RailWorkStationStatus.LeftCylinderState) }),
                            new GriffinsBaseValue(leftCylinderState)
                            );
                    }
                }
                if (railWorkStationSubMachineModulesFactoryCfg.WorkStationEleConfigParams.HasRightBlock)
                {
                    ECylinderPosType rightCylinderState = rightCylinder.CylinderPosType;
                    if (railWorkStationStatus.RightCylinderState != rightCylinderState)
                    {
                        railWorkStationStatus.RightCylinderState = rightCylinderState;
                        InvokeUIDataObjPropValChangedEvent(new ObjInstPropPath(
                            new string[] { nameof(RailWorkStationStatus.RightCylinderState) }),
                            new GriffinsBaseValue(rightCylinderState)
                            );
                    }
                }

                if (railWorkStationSubMachineModulesFactoryCfg.WorkStationEleConfigParams.HasLeftSensor)
                {
                    bool leftSensorState = leftSensor.Read();
                    if (leftSensorState != railWorkStationStatus.LeftSensorState)
                    {
                        railWorkStationStatus.LeftSensorState = leftSensorState;
                        InvokeUIDataObjPropValChangedEvent(new ObjInstPropPath(
                            new string[] { nameof(RailWorkStationStatus.LeftSensorState) }),
                            new GriffinsBaseValue(leftSensorState)
                            );
                    }
                }
                else
                    railWorkStationStatus.LeftSensorState = false;

                if (railWorkStationSubMachineModulesFactoryCfg.WorkStationEleConfigParams.HasRightSensor)
                {
                    bool rightSensorState = rightSensor.Read();
                    if (rightSensorState != railWorkStationStatus.RightSensorState)
                    {
                        railWorkStationStatus.RightSensorState = rightSensorState;
                        InvokeUIDataObjPropValChangedEvent(new ObjInstPropPath(
                            new string[] { nameof(RailWorkStationStatus.RightSensorState) }),
                            new GriffinsBaseValue(rightSensorState)
                            );
                    }
                }
                else
                    railWorkStationStatus.RightSensorState = false;

                bool isHaveMaterial = false;
                switch (workMode)
                {
                    case ERailWorkMode.LeftInLeftOut:
                        isHaveMaterial = railWorkStationStatus.RightSensorState;
                        break;
                    case ERailWorkMode.LeftInRightOut:
                        isHaveMaterial = railWorkStationStatus.RightSensorState;
                        break;
                    case ERailWorkMode.RightInLeftOut:
                        isHaveMaterial = railWorkStationStatus.LeftSensorState;
                        break;
                    case ERailWorkMode.RightInRightOut:
                        isHaveMaterial = railWorkStationStatus.LeftSensorState;
                        break;
                    default:
                        isHaveMaterial = railWorkStationStatus.RightSensorState;
                        break;
                }
                if(railWorkStationStatus.IsHaveMaterial != isHaveMaterial)
                {
                    railWorkStationStatus.IsHaveMaterial = isHaveMaterial;
                    InvokeUIDataObjPropValChangedEvent(new ObjInstPropPath(
                            new string[] { nameof(RailWorkStationStatus.IsHaveMaterial) }),
                            new GriffinsBaseValue(isHaveMaterial)
                            );
                }
            }

            private static List<IBaseStateIO> GetStateIOInstancesByIds(List<Guid> ioGuids)
            {
                if (ioGuids == null || ioGuids.Count == 0)
                    return new List<IBaseStateIO>();

                var response = ServerInnerInfoSender.SendMutualInfo(
                    StateIOInstancesByIdsRequest.InfoKindID,
                    new StateIOInstancesByIdsRequest(ioGuids));
                if (response == null || response.Count == 0)
                    throw new InvalidOperationException(Resources.RailIoRequestFailed);

                StateIOInstancesByIdsResponse ioResponse = response[0].Response as StateIOInstancesByIdsResponse;
                List<IBaseStateIO> ioInstances = ioResponse?.StateIOInstances ?? new List<IBaseStateIO>();
                if (ioInstances.Count != ioGuids.Count)
                    throw new InvalidOperationException(string.Format(Resources.RailIoInstanceCountMismatchFormat, ioGuids.Count, ioInstances.Count));

                return ioInstances;
            }

            GFBaseTypeParamValueList ExecCtlCmdCore(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                GFBaseTypeParamValueList rst = new GFBaseTypeParamValueList();
                rst.Add(new GFBaseTypeParamValue(RailWorkStationSubMachineModulesConst.RetParamResult, new GriffinsBaseValue()));
                switch(cmdID)
                {
                    case RailWorkStationSubMachineModulesConst.GetSpeedListCtlCmdID:
                        {
                            if (railWorkStationSubMachineModulesInitCfg != null)
                                rst[RailWorkStationSubMachineModulesConst.RetParamResult] = new GriffinsBaseValue(JsonObjConvert.ToJSon(railWorkStationSubMachineModulesInitCfg.WorkStationTransSpeedGearList));
                        }
                        break;
                    case RailWorkStationSubMachineModulesConst.GetIOInfosCtlCmdID:
                        {
                            List<IOStateInformation> IOInfos;
                            Dictionary<Guid, IOStateInformation> ioStateInfoDict = new Dictionary<Guid, IOStateInformation>();
                            var ioStateInfosResponse = ServerInnerInfoSender.SendMutualInfo(
                                IOStateInfosRequest.InfoKindID,
                                new IOStateInfosRequest());

                            if (ioStateInfosResponse == null || ioStateInfosResponse.Count == 0)
                                IOInfos = new List<IOStateInformation>();

                            IOStateInfosResponse response = ioStateInfosResponse[0].Response as IOStateInfosResponse;
                            if (response?.IOStateInformations == null)
                                IOInfos = new List<IOStateInformation>();

                            foreach (IOStateInformation ioStateInfo in response.IOStateInformations)
                            {
                                if (ioStateInfo != null && ioStateInfo.IOGuid != Guid.Empty)
                                    ioStateInfoDict[ioStateInfo.IOGuid] = ioStateInfo;
                            }
                            IOInfos = ioStateInfoDict.Values.ToList();
                            rst[RailWorkStationSubMachineModulesConst.RetParamResult] = new GriffinsBaseValue(JsonObjConvert.ToJSon(IOInfos));
                        }
                        break;
                    case RailWorkStationSubMachineModulesConst.GetFactoryParamsCmdID:
                        {
                            if (railWorkStationSubMachineModulesInitCfg != null)
                                rst[RailWorkStationSubMachineModulesConst.RetParamResult] = new GriffinsBaseValue(JsonObjConvert.ToJSon(railWorkStationSubMachineModulesFactoryCfg));
                        }
                        break;
                    default:
                        break;
                }
                return rst;
            }

            #endregion
        }
    }
}
