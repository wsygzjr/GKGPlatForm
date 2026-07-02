using GF_Gereric;
using GKG.ElectronicControl;
using GKG.MotionControl;
using GKG.Vision;
using Griffins;
using Griffins.ImeIOT;
using Griffins.PF.Server;
using GKG.VisionSubMachineModules.Properties;

namespace GKG
{
    namespace SubMM
    {
        public class VisionSubMMCmdExecutor : ISubMMCmdExecutor, ISubMMManualModeCmdExecutor, ISubMMAutoModeCmdExecutor
        {
            private VisionSubMachineModulesFactoryCfg visionSubMachineModulesFactoryCfg;
            private VisionSubMachineModulesInitCfg visionSubMachineModulesInitCfg;
            private VisionSubMachineModulesPPCfg visionSubMachineModulesPPCfg;
            private ISubMMCmdExecutorCallBack iSubMMCmdExecutorCallBack;
            private SubMMAlias alias;
            private ImeGenNormalEventHandler imeGenNormalEventHandler;
            private ImeCabilityEventHandler imeCabilityEventHandler;
            private ImeAlarmEventHandler imeAlarmEventHandler;

            private IVisionDriver vision;
            private IRobotDriver robotDriver;
            private IMotionCalculatorDriver motionCalculatorDriver;
            private Task? flyingTask;
            private RobotExecutionContext robotExecutionContext = new RobotExecutionContext();
            private int bindingAxisX = AxisConstants.X;
            private int bindingAxisY = AxisConstants.Y;
            private int bindingAxisZ = AxisConstants.Z;
            Point3D machinePoint = new Point3D();
            private bool exitTask = false;
            public VisionSubMMCmdExecutor(SubMMAlias alias, byte[] factoryCfgInfo)
            {
                this.alias = alias;
                if(factoryCfgInfo!=null)
                {
                    visionSubMachineModulesFactoryCfg = JsonObjConvert.FromJSonBytes<VisionSubMachineModulesFactoryCfg>(factoryCfgInfo);
                    //visionSubMachineModulesFactoryCfg = new VisionSubMachineModulesFactoryCfg();
                }
                else
                {
                    visionSubMachineModulesFactoryCfg = new VisionSubMachineModulesFactoryCfg();
                }
                visionSubMachineModulesInitCfg = new VisionSubMachineModulesInitCfg();
                visionSubMachineModulesPPCfg = new VisionSubMachineModulesPPCfg();
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

            private void OnPositionChanged(object? sender, PositionChangedEventArgs e)
            {
                if (e?.NewPosition == null)
                    return;

                foreach (var position in e.NewPosition)
                {
                    if (position.Axis == bindingAxisX)
                    {
                        machinePoint.X = position.PositionValue;
                    }
                    else if (position.Axis == bindingAxisY)
                    {
                        machinePoint.Y = position.PositionValue;
                    }
                    else if (position.Axis == bindingAxisZ)
                    {
                        machinePoint.Z = position.PositionValue;
                    }
                }
            }

            void ISubMMCmdExecutor.BeforeInit(GFBaseTypePropValueList devicePropValues)
            {

            }

            void ISubMMCmdExecutor.Init(byte[] initCfgInfo, byte[] calibrationCfgInfo, ISubMMCmdExecutorCallBack callBack)
            {
                visionSubMachineModulesInitCfg.FromBytes(initCfgInfo);

                iSubMMCmdExecutorCallBack = callBack;
                vision = VisionPluginManager.GetVisionDriver(visionSubMachineModulesFactoryCfg.VisionDiverName);

                VisionInitParameters visionInitParameters = new VisionInitParameters()
                {
                    EnableFlying = true,
                };
                vision.Init(JsonObjConvert.ToJSonBytes(visionInitParameters));
            }

            void ISubMMCmdExecutor.AfterInit()
            {
                // 根据配置的绑定轴ID获取对应的RobotDriver
                List<Guid> robotAxisIds = new List<Guid>();
                if (visionSubMachineModulesInitCfg.BindingAxisX != Guid.Empty)
                    robotAxisIds.Add(visionSubMachineModulesInitCfg.BindingAxisX);
                if (visionSubMachineModulesInitCfg.BindingAxisY != Guid.Empty)
                    robotAxisIds.Add(visionSubMachineModulesInitCfg.BindingAxisY);
                if (visionSubMachineModulesInitCfg.BindingAxisZ != Guid.Empty)
                    robotAxisIds.Add(visionSubMachineModulesInitCfg.BindingAxisZ);

                if (robotAxisIds.Count == 0)
                    throw new InvalidOperationException(Resources.VisionBindingAxesNotConfigured);

                // 通过交互消息获取RobotDriver实例
                var robotDriverResponse = ServerInnerInfoSender.SendMutualInfo(
                    RobotDriverByAxisIdsRequest.InfoKindID,
                    new RobotDriverByAxisIdsRequest(robotAxisIds));
                if (robotDriverResponse == null || robotDriverResponse.Count == 0 || ((RobotDriverByAxisIdsResponse)robotDriverResponse[0].Response)?.RobotDriver == null)
                    throw new InvalidOperationException(Resources.VisionRobotDriverAcquireFailed);

                robotDriver = ((RobotDriverByAxisIdsResponse)robotDriverResponse[0].Response).RobotDriver;
                robotDriver.PositionChanged += OnPositionChanged;

                motionCalculatorDriver = MotionCalculatorPluginManager.GetMotionCalculatorDriver(MotionCalculatorDriverNames.Plane);
                motionCalculatorDriver.Init(JsonObjConvert.ToJSonBytes(new MotionCalculatorInitParameters
                {
                    DriverName = MotionCalculatorDriverNames.Plane,
                    DriverInitParameters = JsonObjConvert.ToJSonBytes(new OffsetCalibrationResult())
                }));

                // 获取轴信息以构建轴绑定关系
                var axisInfosResponse = ServerInnerInfoSender.SendMutualInfo(AxisInfosRequest.InfoKindID, new AxisInfosRequest());
                if (axisInfosResponse == null || axisInfosResponse.Count == 0)
                    throw new InvalidOperationException(Resources.AxisInfosAcquireFailed);

                // 从响应中提取轴信息并根据配置的绑定轴ID找到对应的轴信息
                var allAxisInfos = ((AxisInfosResponse)axisInfosResponse[0].Response).AxisInformations ?? new List<AxisInformation>();
                AxisInformation? axisInfoX = visionSubMachineModulesInitCfg.BindingAxisX == Guid.Empty ? null : allAxisInfos.FirstOrDefault(x => x != null && x.AxisGuid == visionSubMachineModulesInitCfg.BindingAxisX);
                AxisInformation? axisInfoY = visionSubMachineModulesInitCfg.BindingAxisY == Guid.Empty ? null : allAxisInfos.FirstOrDefault(x => x != null && x.AxisGuid == visionSubMachineModulesInitCfg.BindingAxisY);
                AxisInformation? axisInfoZ = visionSubMachineModulesInitCfg.BindingAxisZ == Guid.Empty ? null : allAxisInfos.FirstOrDefault(x => x != null && x.AxisGuid == visionSubMachineModulesInitCfg.BindingAxisZ);

                if (visionSubMachineModulesInitCfg.BindingAxisX != Guid.Empty && axisInfoX == null)
                    throw new InvalidOperationException(string.Format(Resources.BindingAxisXInfoNotFound, visionSubMachineModulesInitCfg.BindingAxisX));
                if (visionSubMachineModulesInitCfg.BindingAxisY != Guid.Empty && axisInfoY == null)
                    throw new InvalidOperationException(string.Format(Resources.BindingAxisYInfoNotFound, visionSubMachineModulesInitCfg.BindingAxisY));
                if (visionSubMachineModulesInitCfg.BindingAxisZ != Guid.Empty && axisInfoZ == null)
                    throw new InvalidOperationException(string.Format(Resources.BindingAxisZInfoNotFound, visionSubMachineModulesInitCfg.BindingAxisZ));

                Dictionary<int, AxisBinding> axisBindingPairs = new Dictionary<int, AxisBinding>();
                if (axisInfoX != null)
                    axisBindingPairs[bindingAxisX] = new AxisBinding(axisInfoX.MotionCardGuid, bindingAxisX, axisInfoX.AxisNo);
                if (axisInfoY != null)
                    axisBindingPairs[bindingAxisY] = new AxisBinding(axisInfoY.MotionCardGuid, bindingAxisY, axisInfoY.AxisNo);
                if (axisInfoZ != null)
                    axisBindingPairs[bindingAxisZ] = new AxisBinding(axisInfoZ.MotionCardGuid, bindingAxisZ, axisInfoZ.AxisNo);

                // 构建机械手执行上下文
                robotExecutionContext = new RobotExecutionContext
                {
                    CoordinateSystemId = 0,
                    AxisBindingPairs = axisBindingPairs
                };

                // 获取IO实例以供视觉驱动使用
                if (visionSubMachineModulesInitCfg.ChangeCCDOrJetIOGuid == Guid.Empty)
                    throw new InvalidOperationException(Resources.ChangeCCDOrJetIOGuidNotConfigured);
                if (visionSubMachineModulesInitCfg.TriggerCCDIOGuid == Guid.Empty)
                    throw new InvalidOperationException(Resources.TriggerCCDIOGuidNotConfigured);

                // 通过交互消息获取视觉IO实例
                var stateIoResponse = ServerInnerInfoSender.SendMutualInfo(
                    StateIOInstancesByIdsRequest.InfoKindID,
                    new StateIOInstancesByIdsRequest(new List<Guid>
                    {
                        visionSubMachineModulesInitCfg.ChangeCCDOrJetIOGuid,
                        visionSubMachineModulesInitCfg.TriggerCCDIOGuid
                    }));
                if (stateIoResponse == null || stateIoResponse.Count == 0)
                    throw new InvalidOperationException(Resources.VisionIOAcquireFailed);

                var ioList = ((StateIOInstancesByIdsResponse)stateIoResponse[0].Response).StateIOInstances ?? new List<IBaseStateIO>();
                if (ioList.Count == 0)
                    throw new InvalidOperationException(Resources.VisionIOListEmpty);

                vision.SetIOInstance(ioList);
                if (flyingTask == null)
                {
                    flyingTask = Task.Run(() =>
                    {
                        while (true)
                        {
                            if (exitTask)
                            {
                                break;
                            }
                            vision.GrabOne();
                            Thread.Sleep(100);
                        }
                    });
                }
            }

            void ISubMMCmdExecutor.UnInit()
            {
                if(flyingTask!=null)
                {
                    exitTask = true;
                    flyingTask.Wait(500);
                    flyingTask = null;
                }

                if (robotDriver != null)
                {
                    robotDriver.PositionChanged -= OnPositionChanged;
                    robotDriver = null;
                }
            }

            ISubMMManualModeCmdExecutor ISubMMCmdExecutor.GetSubMMManualModeCmdExecutor()
            {
                return this;
            }

            ISubMMAutoModeCmdExecutor ISubMMCmdExecutor.GetSubMMAutoModeCmdExecutor()
            {
                return this;
            }

            GFBaseTypeParamValueList ISubMMManualModeCmdExecutor.ExecCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                return ExecCtlCmdCore(cmdID, cmdParam);
            }
            void ISubMMAutoModeCmdExecutor.SetPFCfgInfo(byte[] pfCfgInfo)
            {
                visionSubMachineModulesPPCfg.FromBytes(pfCfgInfo);
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

            GFBaseTypeParamValueList ISubMMAutoModeCmdExecutor.ExecRuntimeCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                return ExecCtlCmdCore(cmdID, cmdParam);
            }

            private MotionCalculationParameters BuildMachineMoveCalculationParameters(Point3D targetPoint)
            {
                AxisConstantValues[] targetPosition = new AxisConstantValues[]
                {
                    new AxisConstantValues { Axis = bindingAxisX, PositionValue = targetPoint.X },
                    new AxisConstantValues { Axis = bindingAxisY, PositionValue = targetPoint.Y }
                };

                var nonProcessingTrajectory = new NonProcessingTrajectory
                {
                    MotionTrajectory = new DotMotionTrajectory
                    {
                        TargetPoint = new MotionTrajectoryPoint
                        {
                            Position = targetPosition
                        }
                    },
                    NonProcessingParameters = new NonProcessingTrajectoryParameters
                    {
                        StartSpeed = 0,
                        MaxSpeed = visionSubMachineModulesInitCfg.MachineMoveSpeed,
                        Acceleration = visionSubMachineModulesInitCfg.MachineMoveAcceleration,
                        Deceleration = visionSubMachineModulesInitCfg.MachineMoveAcceleration
                    }
                };

                return new MotionCalculationParameters
                {
                    ProductProcessingTrajectory = new ProductProcessingTrajectoryItem[]
                    {
                        nonProcessingTrajectory
                    }
                };
            }

            void ISubMMAutoModeCmdExecutor.AfterStopWork()
            {
            }

            GFBaseTypeParamValueList ISubMMAutoModeCmdExecutor.ExecMethod(string methodID, GFBaseTypeParamValueList param)
            {
                GFBaseTypeParamValueList rst = new GFBaseTypeParamValueList();
                rst.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue("0")));
                rst.Add(new GFBaseTypeParamValue("errorMsg", new GriffinsBaseValue("0")));
                string errorMsg = "";
                string jsParam = param["jsParam"].ToStringVal();

                switch (methodID)
                {
                    case VisionSubMachineModulesConst.SearchMarkMethodID:
                        {
                            var p = string.IsNullOrEmpty(jsParam) ? null : JsonObjConvert.FromJSon<SearchMarkParams>(jsParam);
                            if (p != null)
                            {
                                var rstObj = searchMark(p);
                                rst["Result"] = new GriffinsBaseValue(JsonObjConvert.ToJSon(rstObj));
                            }
                        }
                        break;

                    case VisionSubMachineModulesConst.TWODDetectMethodID:
                        {
                            var p = string.IsNullOrEmpty(jsParam) ? null : JsonObjConvert.FromJSon<SearchBlobParams>(jsParam);
                            if (p != null)
                            {
                                var rstObj = TwoDDetect(p);
                                rst["Result"] = new GriffinsBaseValue(rstObj.ToJson());
                            }
                        }
                        break;

                    case VisionSubMachineModulesConst.ScanCodeMethodID:
                        {
                            var p = string.IsNullOrEmpty(jsParam) ? null : JsonObjConvert.FromJSon<ScanCodeParams>(jsParam);
                            if (p != null)
                            {
                                var rstStr = ScanCode(p);
                                rst["Result"] = new GriffinsBaseValue(rstStr);
                            }
                        }
                        break;

                    case VisionSubMachineModulesConst.CreateModelMethodID:
                        {
                            var p = string.IsNullOrEmpty(jsParam) ? null : JsonObjConvert.FromJSon<SearchMarkParams>(jsParam);
                            if (p != null)
                            {
                                createModel(p);
                                rst["Result"] = new GriffinsBaseValue(string.Empty);
                            }
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
                string jsParam = param["jsParam"].ToStringVal();
                string errorMsg = "";

                switch (methodID)
                {
                    case VisionSubMachineModulesConst.SearchMarkMethodID:
                        {
                            var p = string.IsNullOrEmpty(jsParam) ? null : JsonObjConvert.FromJSon<SearchMarkParams>(jsParam);
                            if (p != null)
                            {
                                var rstObj = searchMark(p);
                                rst["Result"] = new GriffinsBaseValue(JsonObjConvert.ToJSon(rstObj));
                            }
                        }
                        break;

                    case VisionSubMachineModulesConst.TWODDetectMethodID:
                        {
                            var p = string.IsNullOrEmpty(jsParam) ? null : JsonObjConvert.FromJSon<SearchBlobParams>(jsParam);
                            if (p != null)
                            {
                                var rstObj = TwoDDetect(p);
                                rst["Result"] = new GriffinsBaseValue(rstObj.ToJson());
                            }
                        }
                        break;

                    case VisionSubMachineModulesConst.ScanCodeMethodID:
                        {
                            var p = string.IsNullOrEmpty(jsParam) ? null : JsonObjConvert.FromJSon<ScanCodeParams>(jsParam);
                            if (p != null)
                            {
                                var rstStr = ScanCode(p);
                                rst["Result"] = new GriffinsBaseValue(rstStr);
                            }
                        }
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

            private SearchMarkResult searchMark(SearchMarkParams searchMarkParams)
            {
                return vision.SearchMark(searchMarkParams);
            }

            private TwoDResultBase TwoDDetect(SearchBlobParams searchBlobParams)
            {
                return vision.SearchBlob(searchBlobParams);
            }

            private void createModel(SearchMarkParams searchMarkParams)
            {
                vision.CreateModel(searchMarkParams);
            }

            private string ScanCode(ScanCodeParams scanCodeParams)
            {
                return vision.ScanCode(scanCodeParams);
            }

            public ICompUIDataObjPropValRW GetUIDataObjPropValRW()
            {
                return null;
            }
            GFBaseTypeParamValueList ExecCtlCmdCore(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                GFBaseTypeParamValueList rtn = new GFBaseTypeParamValueList();
                rtn.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue("0")));
                rtn.Add(new GFBaseTypeParamValue("errorMsg", new GriffinsBaseValue(string.Empty)));

                try
                {
                    switch (cmdID)
                    {
                        case VisionSubMachineModulesConst.RunTimeCtlCmdGetPluginName:
                            {
                                rtn.Add(new GFBaseTypeParamValue("PluginName", new GriffinsBaseValue(visionSubMachineModulesFactoryCfg.VisionDiverName)));
                            }
                            break;

                        case VisionSubMachineModulesConst.RunTimeCtlCmdMachineMove:
                            {
                                if (vision == null)
                                    throw new InvalidOperationException(Resources.VisionDriverNotInitialized);
                                if (robotDriver == null)
                                    throw new InvalidOperationException(Resources.RobotDriverNotInitialized);
                                if (cmdParam == null)
                                    throw new ArgumentException(Resources.RuntimeCtlCmdParamRequired);

                                var imagePointParam = cmdParam["ImagePoint"];
                                if (imagePointParam == null)
                                    throw new ArgumentException(Resources.RuntimeCtlCmdImagePointRequired);

                                Point2D imagePoint = JsonObjConvert.FromJSon<Point2D>(imagePointParam.ToStringVal());
                                Point2D offsetPoint = vision.CoordinateTransform(imagePoint, Array.Empty<byte>());
                                Point3D targetPoint = new Point3D(
                                    machinePoint.X + offsetPoint.X,
                                    machinePoint.Y + offsetPoint.Y,
                                    machinePoint.Z);

                                MotionCalculationParameters motionCalculationParameters = BuildMachineMoveCalculationParameters(targetPoint);
                                MotionTrajectory motionTrajectory = motionCalculatorDriver.Calculate(motionCalculationParameters);

                                MotionInstructionSequence sequence = new MotionInstructionSequence
                                {
                                    SequenceType = MotionInstructionSequenceType.StepByStep,
                                    Instructions = motionTrajectory.MotionInstructions ?? Array.Empty<MotionInstructionBase>()
                                };

                                robotDriver.Execute(sequence, robotExecutionContext);

                                rtn["Result"] = new GriffinsBaseValue(JsonObjConvert.ToJSon(targetPoint));
                                rtn.Add(new GFBaseTypeParamValue("TargetPoint", new GriffinsBaseValue(JsonObjConvert.ToJSon(targetPoint))));
                            }
                            break;
                        case VisionSubMachineModulesConst.RunTimeCtlGetAxisInfos:
                            {
                                var response = ServerInnerInfoSender.SendMutualInfo(AxisInfosRequest.InfoKindID, new AxisInfosRequest());
                                if (response != null && response.Count > 0)
                                {
                                    rtn["Result"] = new GriffinsBaseValue(JsonObjConvert.ToJSon(((AxisInfosResponse)response[0].Response).AxisInformations));
                                }
                            }
                            break;
                        case VisionSubMachineModulesConst.RunTimeCtlGetIOStateInfos:
                            {
                                var response = ServerInnerInfoSender.SendMutualInfo(IOStateInfosRequest.InfoKindID, new IOStateInfosRequest());
                                if (response != null && response.Count > 0)
                                {
                                    rtn["Result"] = new GriffinsBaseValue(JsonObjConvert.ToJSon(((IOStateInfosResponse)response[0].Response).IOStateInformations));
                                }
                            }
                            break;
                        default:
                            throw new NotSupportedException(string.Format(Resources.RuntimeCtlCmdNotSupported, cmdID));
                    }
                }
                catch (Exception ex)
                {
                    rtn["errorMsg"] = new GriffinsBaseValue(ex.Message);
                }

                return rtn;
            }
        }
    }
}