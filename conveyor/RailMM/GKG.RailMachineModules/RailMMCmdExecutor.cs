using GF_Gereric;
using GKG.ElectronicControl;
using GKG.ElectronicControl.General;
using GKG.SubMM;
using Griffins;
using Griffins.ImeIOT;
using Griffins.PF.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static GKG.MM.RailMMCmdExecutor;

namespace GKG
{
    namespace MM
    {
        public class RailMMCmdExecutor : IMMCmdExecutor, IMMManualModeCmdExecutor, IMMAutoModeCmdExecutor, ICompUIDataObjPropValRW
        {
            internal enum TransportDir
            {
                Left = 0,
                Right = 1
            }

            private RailMachineModulesFactoryCfg factoryCfg;
            private RailMachineModulesInitCfg initCfg;
            private RailMachineModulesPPCfg pPCfg;

            private IMMCmdExecutorCallBack _callBack;

            private MMAlias alias;

            private ImeGenNormalEventHandler imeGenNormalEventHandler;

            private ImeCabilityEventHandler imeCabilityEventHandler;
            
            private ImeAlarmEventHandler imeAlarmEventHandler;

            private ImePropValChangedEventHandler uIDataObjPropValChangedEvent;
            public RailMMCmdExecutor(MMAlias alias, byte[] factoryCfgInfo)
            {
                this.alias = alias;
                factoryCfg = new RailMachineModulesFactoryCfg();
                factoryCfg.FromBytes(factoryCfgInfo);
                initCfg = new RailMachineModulesInitCfg();
                pPCfg = new RailMachineModulesPPCfg();
            }

            event ImeGenNormalEventHandler IMMAutoModeCmdExecutor.GenNormalEvent
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

            event ImeCabilityEventHandler IMMAutoModeCmdExecutor.CabilityEvent
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

            event ImeAlarmEventHandler IMMAutoModeCmdExecutor.AlarmEvent
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
            /// <summary>
            /// 运行模式：ConfigMode-配置模式，AutoMode-自动模式，ManualMode-手动模式
            /// </summary>
            private ImeRunMode runMode = ImeRunMode.ConfigMode;
            void IMMCmdExecutor.BeforeInit()
            {

            }

            void IMMCmdExecutor.Init(byte[] initCfgInfo, byte[] calibrationCfgInfo, IMMCmdExecutorCallBack callBack)
            {
                _callBack = callBack;
                initCfg.FromBytes(initCfgInfo);
            }

            void IMMCmdExecutor.AfterInit()
            {
                
            }

            void IMMCmdExecutor.UnInit()
            {

            }

            IMMManualModeCmdExecutor IMMCmdExecutor.GetMMManualModeCmdExecutor()
            {
                return this;
            }

            IMMAutoModeCmdExecutor IMMCmdExecutor.GetMMAutoModeCmdExecutor()
            {
                return this;
            }

            void IMMAutoModeCmdExecutor.SetPFCfgInfo(byte[] pfCfgInfo)
            {

            }

            bool IMMAutoModeCmdExecutor.CanStartWork(out string reasonMsg)
            {
                reasonMsg = string.Empty;
                return true;
            }

            void IMMAutoModeCmdExecutor.SetRunMode(ImeRunMode imeRunMode)
            {
                runMode = imeRunMode;
            }

            void IMMAutoModeCmdExecutor.StartWork()
            {
                PauseObj.Status = 2;
            }

            void IMMAutoModeCmdExecutor.StopWork()
            {
                PauseObj.Status = 3;
            }

            void IMMAutoModeCmdExecutor.Pause()
            {
                PauseObj.Status = 1;
            }

            void IMMAutoModeCmdExecutor.Resume()
            {
                PauseObj.Status = 2;
            }

            void IMMAutoModeCmdExecutor.BeforeSwitchPF()
            {

            }

            void IMMAutoModeCmdExecutor.AfterStopWork()
            {

            }

            GFBaseTypeParamValueList IMMAutoModeCmdExecutor.ExecMethod(string methodID, GFBaseTypeParamValueList param)
            {
                GFBaseTypeParamValueList retParams = new GFBaseTypeParamValueList();
                switch (methodID)
                {
                    case RailMachineModulesConst.InletPanelMethodID:
                        {
                            InletPanel();
                        }
                        break;
                    case RailMachineModulesConst.OutletPanelMethodID:
                        OutletPanel();
                        break;
                    default:
                        break;
                }
                return retParams;
            }

            GFParamValueList IMMAutoModeCmdExecutor.ExecMethod(string methodID, GFParamValueList param)
            {
                return new GFParamValueList();
            }

            GFBaseTypeParamValueList IMMAutoModeCmdExecutor.ExecCabilityMethod(string methodID, GFBaseTypeParamValueList param)
            {
                GFBaseTypeParamValueList rst = new GFBaseTypeParamValueList();
                switch (methodID)
                {
                    case RailMachineModulesConst.InletPanelToWorkingStationMethodID:
                        {
                            InletPanelToWorkingStation();
                        }
                        break;
                    case RailMachineModulesConst.OutletPanelToOutStationMethodID:
                        {
                            OutletPanelToOutStation();
                        }
                        break;
                    case RailMachineModulesConst.InletPanelMethodID:
                        {
                            InletPanel();
                        }
                        break;
                    case RailMachineModulesConst.OutletPanelMethodID:
                        {
                            OutletPanel();
                        }
                        break;
                    default:
                        break;
                }
                return rst;
            }

            GFBaseTypeParamValueList IMMAutoModeCmdExecutor.ExecRuntimeCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                return ExecCtlCmdCore(cmdID, cmdParam);
            }

            /// <summary>
            /// 获取界面数据对象属性读写接口实例，如果不支持返回nul
            /// </summary>
            /// <returns>界面数据对象属性读写接口实例</returns>
            #region 界面数据对象
            ICompUIDataObjPropValRW IMMAutoModeCmdExecutor.GetUIDataObjPropValRW()
            {
                return this;
            }

            void ICompUIDataObjPropValRW.SetUIDataObjPropPathValue(ObjInstPropPath objInstPropPath, GriffinsBaseValue value)
            {

            }

            void ICompUIDataObjPropValRW.SetUIDataObjPropPathValues(GFBaseTypeObjPropPathValueList values)
            {

            }

            GriffinsBaseValue ICompUIDataObjPropValRW.GetUIDataObjPropPathValue(ObjInstPropPath objInstPropPath)
            {
                return new GriffinsBaseValue();
            }

            GFBaseTypeObjPropPathValueList ICompUIDataObjPropValRW.GetUIDataObjPropPathValues(ObjInstPropPath[] objInstPropPaths)
            {
                return new GFBaseTypeObjPropPathValueList();
            }

            GFBaseTypeObjPropPathValueList ICompUIDataObjPropValRW.GetAllUIDataObjPropPathValues()
            {
                return new GFBaseTypeObjPropPathValueList();
            }

            GFBaseTypeParamValueList ICompUIDataObjPropValRW.ExecUIDataObjCommand(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                return ExecCtlCmdCore(cmdID, cmdParam);
            }
            #endregion
            void IMMAutoModeCmdExecutor.ExecSubMMEvent(InnerAlias innerAlias, int eventID, GFBaseTypeParamValueList eventParam)
            {

            }

            void IMMAutoModeCmdExecutor.ExecSubMMCabilityEvent(InnerAlias innerAlias, string eventID, GFBaseTypeParamValueList eventParam)
            {

            }

            void IMMAutoModeCmdExecutor.ReturnToOriginal()
            {

            }

            GFBaseTypeParamValueList IMMManualModeCmdExecutor.ExecCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                return ExecCtlCmdCore(cmdID, cmdParam);
            }

            #region 机械模组普通方法
            GFBaseTypeParamValueList ExecCtlCmdCore(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                GFBaseTypeParamValueList rst = new GFBaseTypeParamValueList();
                switch (cmdID)
                {
                    case RailMachineModulesConst.InletPanelMethodID:
                        {
                            Task.Run(() => {
                                InletPanel();
                            });
                        }
                        break;
                    case RailMachineModulesConst.OutletPanelMethodID:
                        {
                            Task.Run(() =>
                            {
                                OutletPanel();
                            });
                        }
                        break;
                    case RailMachineModulesConst.GetIOInfosCtlCmdID:
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
                            rst["Result"] = new GriffinsBaseValue(JsonObjConvert.ToJSon(IOInfos));
                        }
                        break;
                    default:
                        break;
                }
                return rst;
            }

            private TransportDir GetTransportDir()
            {
                TransportDir transportDir = TransportDir.Left;
                switch (pPCfg.TransferDirection)
                {
                    case ERailWorkMode.LeftInRightOut:
                    case ERailWorkMode.LeftInLeftOut:
                        {
                            transportDir = TransportDir.Left;
                        }
                        break;
                    case ERailWorkMode.RightInLeftOut:
                    case ERailWorkMode.RightInRightOut:
                        {
                            transportDir = TransportDir.Right;
                        }
                        break;
                    default:
                        break;
                }
                return transportDir;
            }

            /// <summary>
            /// 从上位机进板到进板口
            /// </summary>
            private void InletPanelFromUpperMachine()
            {
                // 调用上下位机通讯

            }

            /// <summary>
            /// 单工位进板
            /// </summary>
            /// <param name="wordStationInnerAlias">工位的内部别名</param>
            /// <param name="railMotorInnerAlias">运输电机的内部别名（可能有多个，目前是一个）</param>
            private void WorkStationInletPanel(InnerAlias workStationInnerAlias, InnerAlias railMotorInnerAlias, TransportDir transportDir)
            {
                GFBaseTypeParamValueList methodParam = new GFBaseTypeParamValueList();

                string ControlInletCylinderMethodID = "";
                string ControlOutletCylinderMethodID = "";
                string GetArrivedSensorStateMethodID = "";
                bool direction = true;
                // 根据运输方向确定挡板控制参数和感应器
                switch (transportDir)
                {
                    case TransportDir.Left:
                        {
                            ControlInletCylinderMethodID = RailWorkStationSubMachineModulesConst.ControlLeftCylinderMethodID;
                            ControlOutletCylinderMethodID = RailWorkStationSubMachineModulesConst.ControlRightCylinderMethodID;
                            GetArrivedSensorStateMethodID = RailWorkStationSubMachineModulesConst.GetRightSensorStateMethodID;
                            direction = true;
                        }
                        break;
                    case TransportDir.Right:
                        {
                            ControlInletCylinderMethodID = RailWorkStationSubMachineModulesConst.ControlRightCylinderMethodID;
                            ControlOutletCylinderMethodID = RailWorkStationSubMachineModulesConst.ControlLeftCylinderMethodID;
                            GetArrivedSensorStateMethodID = RailWorkStationSubMachineModulesConst.GetLeftSensorStateMethodID;
                            direction = false;
                        }
                        break;
                }
                // 调用左工位左挡板下降
                methodParam.Add(new GFBaseTypeParamValue("Cylinder", new GriffinsBaseValue(false)));
                _callBack.ExecSubMMMethod(
                    workStationInnerAlias,
                    ControlInletCylinderMethodID,
                    methodParam);

                // 调用左工位右挡板上升
                methodParam["Cylinder"] = new GriffinsBaseValue(true);
                _callBack.ExecSubMMMethod(
                    workStationInnerAlias,
                    ControlOutletCylinderMethodID,
                    methodParam);

                // 获取运输速度
                GFBaseTypeParamValueList result = _callBack.ExecSubMMMethod(
                    workStationInnerAlias,
                    RailWorkStationSubMachineModulesConst.GetTransportSpeedMethodID,
                    methodParam);
                //double acc = (double)result["TransportAcceleration"].ToDecimal();
                //double speed = (double)result["TransportSpeed"].ToDecimal();
                double acc = 100;
                double speed = 10;

                // 调用轨道电机运输
                methodParam.Add(new GFBaseTypeParamValue("Acceleration", new GriffinsBaseValue(acc)));
                methodParam.Add(new GFBaseTypeParamValue("Speed", new GriffinsBaseValue(speed)));
                methodParam.Add(new GFBaseTypeParamValue("Direction", new GriffinsBaseValue(direction)));
                _callBack.ExecSubMMMethod(
                    railMotorInnerAlias,
                    RailMotorSubMachineModulesConst.ContinueMoveMethodID,
                    methodParam);

                // 调用工位判断板子是否到位
                while (true)
                {
                    Thread.Sleep(30);
                    GFBaseTypeParamValueList sensorResult = _callBack.ExecSubMMMethod(
                        workStationInnerAlias,
                        GetArrivedSensorStateMethodID,
                        new GFBaseTypeParamValueList());
                    bool isPanelArrived = (bool)sensorResult["SensorStatus"].ToBool();
                    if (isPanelArrived)
                    {
                        break;
                    }
                    if (PauseObj.Status == 3)
                    {                        // 停止运输
                        _callBack.ExecSubMMMethod(
                           railMotorInnerAlias,
                           RailMotorSubMachineModulesConst.StopMoveMethodID,
                           methodParam);
                        return;
                    }
                }
                // 板到位后，调用轨道电机停止
                _callBack.ExecSubMMMethod(
                   railMotorInnerAlias,
                   RailMotorSubMachineModulesConst.StopMoveMethodID,
                   methodParam);
            }

            /// <summary>
            /// 单工位出板
            /// </summary>
            /// <param name="wordStationInnerAlias">工位的内部别名</param>
            /// <param name="railMotorInnerAlias">运输电机的内部别名（可能有多个，目前是一个）</param>
            private void WorkStationOutletPanel(InnerAlias workStationInnerAlias, InnerAlias railMotorInnerAlias, TransportDir transportDir)
            {
                GFBaseTypeParamValueList methodParam = new GFBaseTypeParamValueList();

                string ControlOutletCylinderMethodID = "";
                string GetLeaveSensorStateMethodID = "";
                bool direction = true;

                // 根据运输方向确定挡板控制参数和感应器
                switch (transportDir)
                {
                    case TransportDir.Left:
                        {
                            ControlOutletCylinderMethodID = RailWorkStationSubMachineModulesConst.ControlRightCylinderMethodID;
                            GetLeaveSensorStateMethodID = RailWorkStationSubMachineModulesConst.GetRightSensorStateMethodID;
                            direction = true;
                        }
                        break;
                    case TransportDir.Right:
                        {
                            ControlOutletCylinderMethodID = RailWorkStationSubMachineModulesConst.ControlLeftCylinderMethodID;
                            GetLeaveSensorStateMethodID = RailWorkStationSubMachineModulesConst.GetLeftSensorStateMethodID;
                            direction = false;
                        }
                        break;
                }

                // 调用左工位左挡板下降
                methodParam.Add(new GFBaseTypeParamValue("Cylinder", new GriffinsBaseValue(false)));
                _callBack.ExecSubMMMethod(
                    workStationInnerAlias,
                    ControlOutletCylinderMethodID,
                    methodParam);

                // 获取运输速度
                GFBaseTypeParamValueList result = _callBack.ExecSubMMMethod(
                    workStationInnerAlias,
                    RailWorkStationSubMachineModulesConst.GetTransportSpeedMethodID,
                    methodParam);
                //double acc = (double)result["TransportAcceleration"].ToDecimal();
                //double speed = (double)result["TransportSpeed"].ToDecimal();
                double acc = 100;
                double speed = 10;

                // 调用轨道电机运输
                methodParam.Add(new GFBaseTypeParamValue("Acceleration", new GriffinsBaseValue(acc)));
                methodParam.Add(new GFBaseTypeParamValue("Speed", new GriffinsBaseValue(speed)));
                methodParam.Add(new GFBaseTypeParamValue("Direction", new GriffinsBaseValue(direction)));
                _callBack.ExecSubMMMethod(
                    railMotorInnerAlias,
                    RailMotorSubMachineModulesConst.ContinueMoveMethodID,
                    methodParam);

                // 调用工位判断板子是否离开
                while (true)
                {
                    Thread.Sleep(30);
                    GFBaseTypeParamValueList sensorResult = _callBack.ExecSubMMMethod(
                        workStationInnerAlias,
                        GetLeaveSensorStateMethodID,
                        new GFBaseTypeParamValueList());
                    bool isPanelArrived = (bool)sensorResult["SensorStatus"].ToBool();
                    if (!isPanelArrived)
                    {
                        break;
                    }
                    if (PauseObj.Status == 3)
                    {
                        // 停止运输
                        _callBack.ExecSubMMMethod(
                           railMotorInnerAlias,
                           RailMotorSubMachineModulesConst.StopMoveMethodID,
                           methodParam);
                        return;
                    }
                }
                // 出板延时
                DateTime dateTime = DateTime.Now;
                while(true)
                {
                    if(DateTime.Now.Subtract(dateTime).TotalMilliseconds > pPCfg.OutletPanelDelayTime)
                    {
                        break;
                    }
                    if (PauseObj.Status == 3)
                    {
                        break;
                    }
                }

                // 板离开后，调用轨道电机停止
                _callBack.ExecSubMMMethod(
                   railMotorInnerAlias,
                   RailMotorSubMachineModulesConst.StopMoveMethodID,
                   methodParam);
            }

            /// <summary>
            /// 设置工位工作模式，进板和出板工位都需要设置
            /// </summary>
            private void SetWorkModeToWorkStation()
            {
                _callBack.ExecSubMMMethod(
                    RailMachineModulesConst.InnerAliasLeftWorkStation,
                    RailWorkStationSubMachineModulesConst.SetWorkModeMethodID,
                    new GFBaseTypeParamValueList
                    {
                        new GFBaseTypeParamValue(
                            "WorkMode",
                            new GriffinsBaseValue(pPCfg.TransferDirection.ToString()))
                    });
                _callBack.ExecSubMMMethod(
                    RailMachineModulesConst.InnerAliasMiddleWorkStation,
                    RailWorkStationSubMachineModulesConst.SetWorkModeMethodID,
                    new GFBaseTypeParamValueList
                    {
                        new GFBaseTypeParamValue(
                            "WorkMode",
                            new GriffinsBaseValue(pPCfg.TransferDirection.ToString()))
                    });
                _callBack.ExecSubMMMethod(
                    RailMachineModulesConst.InnerAliasRightWorkStation,
                    RailWorkStationSubMachineModulesConst.SetWorkModeMethodID,
                    new GFBaseTypeParamValueList
                    {
                        new GFBaseTypeParamValue(
                            "WorkMode",
                            new GriffinsBaseValue(pPCfg.TransferDirection.ToString()))
                    });
            }

            /// <summary>
            /// 进板
            /// </summary>
            private void InletPanel()
            {
                SetWorkModeToWorkStation();
                TransportDir transportDir = GetTransportDir();
                switch(pPCfg.InletPanelWorkStation)
                {
                    case EInletPanelWorkStation.StandBy:
                        {
                            if (CheckCanTransport(null, RailMachineModulesConst.InnerAliasLeftWorkStation, transportDir) == false)
                            {
                                imeCabilityEventHandler.Invoke(this, new ImeCabilityEventArgs(RailMachineModulesConst.InletPanelFailedEventID, new GFBaseTypeParamValueList()));
                                return;
                            }
                            InletPanelFromUpperMachine();
                            WorkStationInletPanel(RailMachineModulesConst.InnerAliasLeftWorkStation, RailMachineModulesConst.InnerAliasRailMotor, transportDir);
                        }
                        break;
                    case EInletPanelWorkStation.Working:
                        {
                            InletPanelFromUpperMachine();
                            if (CheckCanTransport(RailMachineModulesConst.InnerAliasLeftWorkStation, RailMachineModulesConst.InnerAliasMiddleWorkStation, transportDir) == false)
                            {
                                imeCabilityEventHandler.Invoke(this, new ImeCabilityEventArgs(RailMachineModulesConst.InletPanelFailedEventID, new GFBaseTypeParamValueList()));
                                return;
                            }
                            WorkStationInletPanel(RailMachineModulesConst.InnerAliasMiddleWorkStation, RailMachineModulesConst.InnerAliasRailMotor, transportDir);
                        }
                        break;
                }
                InvokeCabilityEvent(RailMachineModulesConst.InletPanelFinishedEventID, new GFBaseTypeParamValueList());
            }

            /// <summary>
            /// 进板到工作位
            /// </summary>
            private void InletPanelToWorkingStation()
            {
                SetWorkModeToWorkStation();

                TransportDir transportDir = GetTransportDir();
                if (CheckCanTransport(RailMachineModulesConst.InnerAliasLeftWorkStation, RailMachineModulesConst.InnerAliasMiddleWorkStation, transportDir) == false)
                {
                    imeCabilityEventHandler.Invoke(this, new ImeCabilityEventArgs(RailMachineModulesConst.InletPanelToWorkingStationFailedEventID, new GFBaseTypeParamValueList()));
                    return;
                }
                WorkStationInletPanel(RailMachineModulesConst.InnerAliasMiddleWorkStation, RailMachineModulesConst.InnerAliasRailMotor, transportDir);
                InvokeCabilityEvent(RailMachineModulesConst.InletPanelToWorkingStationFinishedEventID, new GFBaseTypeParamValueList());
            }

            /// <summary>
            /// 出板到下位机
            /// </summary>
            private void OutletPanelToLowerMachine()
            {
                // 调用上下位机通讯

            }

            /// <summary>
            /// 出板
            /// </summary>
            private void OutletPanel()
            {
                SetWorkModeToWorkStation();
                OutletPanelToLowerMachine();
                TransportDir transportDir = GetTransportDir();

                switch (pPCfg.OutletPanelWorkStation)
                {
                    case EOutletPanelWorkStation.Out:
                        {
                            if (CheckCanTransport(null, RailMachineModulesConst.InnerAliasRightWorkStation, transportDir) == true)
                            {
                                imeCabilityEventHandler.Invoke(this, new ImeCabilityEventArgs(RailMachineModulesConst.OutletPanelFailedEventID, new GFBaseTypeParamValueList()));
                                return;
                            }
                            WorkStationOutletPanel(RailMachineModulesConst.InnerAliasRightWorkStation, RailMachineModulesConst.InnerAliasRailMotor, transportDir);
                        }
                        break;
                    case EOutletPanelWorkStation.Working:
                        {
                            if (CheckCanTransport(RailMachineModulesConst.InnerAliasMiddleWorkStation, RailMachineModulesConst.InnerAliasRightWorkStation, transportDir) == false)
                            {
                                imeCabilityEventHandler.Invoke(this, new ImeCabilityEventArgs(RailMachineModulesConst.OutletPanelFailedEventID, new GFBaseTypeParamValueList()));
                                return;
                            }
                            WorkStationInletPanel(RailMachineModulesConst.InnerAliasRightWorkStation, RailMachineModulesConst.InnerAliasRailMotor, transportDir);
                            if (CheckCanTransport(null, RailMachineModulesConst.InnerAliasRightWorkStation, transportDir) == true)
                            {
                                imeCabilityEventHandler.Invoke(this, new ImeCabilityEventArgs(RailMachineModulesConst.OutletPanelFailedEventID, new GFBaseTypeParamValueList()));
                                return;
                            }
                            WorkStationOutletPanel(RailMachineModulesConst.InnerAliasRightWorkStation, RailMachineModulesConst.InnerAliasRailMotor, transportDir);
                        }
                        break;
                }
                InvokeCabilityEvent(RailMachineModulesConst.OutletPanelFinishedEventID, new GFBaseTypeParamValueList());
            }

            /// <summary>
            /// 出板到出板位
            /// </summary>
            private void OutletPanelToOutStation()
            {
                SetWorkModeToWorkStation();

                TransportDir transportDir = GetTransportDir();
                if (CheckCanTransport(RailMachineModulesConst.InnerAliasMiddleWorkStation, RailMachineModulesConst.InnerAliasRightWorkStation, transportDir) == false)
                {
                    imeCabilityEventHandler.Invoke(this, new ImeCabilityEventArgs(RailMachineModulesConst.OutletPanelToOutStationFailedEventID, new GFBaseTypeParamValueList()));
                    return;
                }
                WorkStationInletPanel(RailMachineModulesConst.InnerAliasRightWorkStation, RailMachineModulesConst.InnerAliasRailMotor, transportDir);
                InvokeCabilityEvent(RailMachineModulesConst.OutletPanelToOutStationFinishedEventID, new GFBaseTypeParamValueList());
            }

            /// <summary>
            /// 检测是否可以运输
            /// </summary>
            /// <param name="lastStationInnerAlias"></param>
            /// <param name="workStationInnerAlias"></param>
            /// <param name="transportDir"></param>
            /// <returns></returns>
            private bool CheckCanTransport(InnerAlias? lastStationInnerAlias, InnerAlias workStationInnerAlias, TransportDir transportDir)
            {
                string GetArrivedSensorStateMethodID = "";

                // 根据运输方向确定挡板控制参数和感应器
                switch (transportDir)
                {
                    case TransportDir.Left:
                        {
                            GetArrivedSensorStateMethodID = RailWorkStationSubMachineModulesConst.GetRightSensorStateMethodID;
                        }
                        break;
                    case TransportDir.Right:
                        {
                            GetArrivedSensorStateMethodID = RailWorkStationSubMachineModulesConst.GetLeftSensorStateMethodID;
                        }
                        break;
                }
                GFBaseTypeParamValueList sensorResult;

                if (lastStationInnerAlias.HasValue)
                {
                    sensorResult = _callBack.ExecSubMMMethod(
                    lastStationInnerAlias.Value,
                    GetArrivedSensorStateMethodID,
                    new GFBaseTypeParamValueList());
                    bool isHavePanel = (bool)sensorResult["SensorStatus"].ToBool();
                    if(!isHavePanel)
                    {
                        return false;
                    }
                }


                sensorResult = _callBack.ExecSubMMMethod(
                    workStationInnerAlias,
                    GetArrivedSensorStateMethodID,
                    new GFBaseTypeParamValueList());
                bool isPanelArrived = (bool)sensorResult["SensorStatus"].ToBool();
                return !isPanelArrived;
            }

            /// <summary>
            /// 触发能力事件
            /// </summary>
            /// <param name="eventID"></param>
            /// <param name="eventParam"></param>
            private void InvokeCabilityEvent(string eventID, GFBaseTypeParamValueList eventParam)
            {
                imeCabilityEventHandler?.Invoke(this, new ImeCabilityEventArgs(eventID, eventParam));
            }

            #endregion
        }
    }
}
