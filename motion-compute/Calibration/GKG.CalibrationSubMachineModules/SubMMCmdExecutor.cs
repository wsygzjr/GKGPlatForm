//using BasicRobotSubMachineModules;
using DispensingFunctionHeadSubMachineModules;
using GF_Gereric;
using GKG.ElectronicControl;
using GKG.EletronicManager;
using GKG.MotionControl;
using GKG.Vision;
using Griffins;
using Griffins.ImeIOT;
using System;
using static GKG.CalibrationCmdExecutor;

namespace GKG.SubMM
{
    public class CalibrationSubMMCmdExecutor : ISubMMCmdExecutor
    {
        private CalibrationSubMachineModulesFactoryCfg calibrationSubMachineModulesFactoryCfg;
        private CalibrationSubMachineModulesInitCfg calibrationSubMachineModulesInitCfg;
        private CalibrationSubMachineModulesPPCfg calibrationSubMachineModulesPPCfg;
        private ISubMMCmdExecutorCallBack iSubMMCmdExecutorCallBack;
        private SubMMAlias alias;
        private ImeGenNormalEventHandler imeGenNormalEventHandler;
        private ImeCabilityEventHandler imeCabilityEventHandler;
        private ImeAlarmEventHandler imeAlarmEventHandler;
        private CalibrationDictionary calibraters = new CalibrationDictionary();
        private IRobotDriver robotDriver;
        private IVisionDriver visionDriver;
        private IMotionCalculatorDriver motionCalculatorDriver;
        private IMotionControlBase motionControl;
        private Point3D currentPosition = new Point3D();
        private RobotExecutionContext robotExecutionContext = new RobotExecutionContext();

        public CalibrationSubMMCmdExecutor(SubMMAlias alias, byte[] factoryCfgInfo)
        {
            this.alias = alias;
            calibrationSubMachineModulesFactoryCfg = new CalibrationSubMachineModulesFactoryCfg();
            calibrationSubMachineModulesFactoryCfg.FromBytes(factoryCfgInfo);
            calibrationSubMachineModulesFactoryCfg.calibrations.Add(new Calibration()
            {
                ID = "cam01",
                Type = CalibrationType.CameraScale
            });
            calibrationSubMachineModulesFactoryCfg.calibrationInitParams.Add(new CalibrationInitParams()
            {
                ID = "cam01",
                InitParams = JsonObjConvert.ToJSonBytes(
                    new CameraScaleCalibrationInitParams()
                    {
                        FunctionHead = "cam01",
                        CalibrationModelPath = "D:\\Calibration.MatchPattern",
                        CameraHeight = 5,
                        XYMoveParameters = new NonProcessingTrajectoryParameters()
                        {
                            MaxSpeed = 100,
                            Acceleration = 200,
                            Deceleration = 200
                        },
                        ZMoveParameters = new NonProcessingTrajectoryParameters()
                        {
                            MaxSpeed = 50,
                            Acceleration = 100,
                            Deceleration = 100
                        }
                    })
            });
            calibrationSubMachineModulesInitCfg = new CalibrationSubMachineModulesInitCfg();
            calibrationSubMachineModulesPPCfg = new CalibrationSubMachineModulesPPCfg();
        }

        event ImeGenNormalEventHandler ISubMMCmdExecutor.GenNormalEvent
        {
            add { imeGenNormalEventHandler += value; }
            remove { imeGenNormalEventHandler -= value; }
        }

        event ImeCabilityEventHandler ISubMMCmdExecutor.CabilityEvent
        {
            add { imeCabilityEventHandler += value; }
            remove { imeCabilityEventHandler -= value; }
        }

        event ImeAlarmEventHandler ISubMMCmdExecutor.AlarmEvent
        {
            add { imeAlarmEventHandler += value; }
            remove { imeAlarmEventHandler -= value; }
        }

        void ISubMMCmdExecutor.BeforeInit(string[] subMechCompParam)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 初始化（在创建子机械模组实例后首先调用）
        /// </summary>
        /// <param name="initCfgInfo">初始化参数，null表示缺省值</param>
        /// <param name="callBack">子机械模组（复合子机械模组）运行时回调接口</param>
        void ISubMMCmdExecutor.Init(byte[] initCfgInfo, ISubMMCmdExecutorCallBack callBack)
        {
            iSubMMCmdExecutorCallBack = callBack;

            calibrationSubMachineModulesInitCfg.FromBytes(initCfgInfo);

            EletronicManager.EletronicManager.Initialize();
            motionControl = EletronicManager.EletronicManager.GetMotionControl(MotionCardType.GC800);

            robotDriver = RobotPluginManager.GetRobotDriver(RobotDriverNames.CategoryARobot);
            robotDriver.Init(BuildRobotInitParameters(), motionControl);
            robotDriver.PositionChanged -= RobotDriver_PositionChanged;
            robotDriver.PositionChanged += RobotDriver_PositionChanged;

            visionDriver = VisionPluginManager.GetVisionDriver("GVision");
            visionDriver.Init(JsonObjConvert.ToJSonBytes(new VisionInitParameters() { CameraID = 0, EnableFlying = true }));
            IBaseStateIO changeCCDorJet = new StateControlMotionCard();
            IBaseStateIO triggerCCD = new StateControlMotionCard();
            changeCCDorJet.Init(JsonObjConvert.ToJSonBytes(new IOStateInitParameters()
            {
                deviceID = "GC800",
                channelID = "RW14"
            }));
            changeCCDorJet.SetDeviceInstance(motionControl);
            triggerCCD.Init(JsonObjConvert.ToJSonBytes(new IOStateInitParameters()
            {
                deviceID = "GC800",
                channelID = "RW15"
            }));
            triggerCCD.SetDeviceInstance(motionControl);
            List<IBaseStateIO> ioList = new List<IBaseStateIO>()
            {
                changeCCDorJet,
                triggerCCD
            };
            visionDriver.SetIOInstance(ioList);

            motionCalculatorDriver = MotionCalculatorPluginManager.GetMotionCalculatorDriver(MotionCalculatorDriverNames.Plane);
            motionCalculatorDriver.Init(JsonObjConvert.ToJSonBytes(new MotionCalculatorInitParameters
            {
                DriverName = MotionCalculatorDriverNames.Plane
            }));

            robotExecutionContext = BuildRobotExecutionContext();

            foreach (var calibration in calibrationSubMachineModulesFactoryCfg.calibrations)
            {
                switch (calibration.Type)
                {
                    case CalibrationType.Offset:
                        calibraters.TryAdd(calibration.ID, new OffsetCalibration());
                        break;

                    case CalibrationType.LaserHeight:
                        calibraters.TryAdd(calibration.ID, new LaserAndFunctionHeadCalibration());
                        break;

                    case CalibrationType.CameraScale:
                        calibraters.TryAdd(calibration.ID, new CameraScaleCalibration());
                        break;

                    case CalibrationType.FlyingCCD:
                        calibraters.TryAdd(calibration.ID, new FlyingCalibration());
                        break;
                }
            }
            foreach (var initParam in calibrationSubMachineModulesFactoryCfg.calibrationInitParams)
            {
                if (calibraters.TryGetValue(initParam.ID, out var calibrater))
                {
                    calibrater.SetRuntimeContext(new CalibrationRuntimeContext
                    {
                        MotionControl = motionControl,
                        RobotDriver = robotDriver,
                        VisionDriver = visionDriver,
                        MotionCalculatorDriver = motionCalculatorDriver,
                        RobotExecutionContext = robotExecutionContext
                    });
                    calibrater.Init(initParam.InitParams);
                }
            }
        }

        /// <summary>
        /// 设置配方参数
        /// </summary>
        /// <param name="pfCfgInfo">配方参数，null表示缺省值</param>
        void ISubMMCmdExecutor.SetPFCfgInfo(byte[] pfCfgInfo)
        {
            calibrationSubMachineModulesPPCfg.FromBytes(pfCfgInfo);
        }

        void ISubMMCmdExecutor.StartWork() { }
        void ISubMMCmdExecutor.StopWork() { }
        void ISubMMCmdExecutor.Pause() { }
        void ISubMMCmdExecutor.Resume() { }
        void ISubMMCmdExecutor.BeforeSwitchPF() { }
        void ISubMMCmdExecutor.AfterStopWork() { }

        /// <summary>
        /// 运行时控制命令（同步调用）
        /// </summary>
        /// <param name="cmdID"></param>
        /// <param name="cmdParam">包含 "jsParam" 字段的 GFBaseTypeParamValueList</param>
        /// <returns>包含 Result(字符串) 的 GFBaseTypeParamValueList</returns>
        GFBaseTypeParamValueList ISubMMCmdExecutor.ExecRuntimeCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
        {
            GFBaseTypeParamValueList rst = new GFBaseTypeParamValueList();
            rst.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue("")));
            rst.Add(new GFBaseTypeParamValue("errorMsg", new GriffinsBaseValue("")));
            string result = "";
            string errorMsg = "";
            string jsParam = cmdParam["jsParam"].ToJsonStrValue();
            GFBaseTypeParamValueList param = new GFBaseTypeParamValueList();
            switch (cmdID)
            {
                // 针头到
                case CalibrationSubMachineModulesConst.RunTimeCtlCmdNeedleMoveTo:
                    {
                        NeedleMoveTo_Param needleMoveTo_Param = new NeedleMoveTo_Param();
                        needleMoveTo_Param.FromJson(jsParam);
                        ExecuteRobotMove(needleMoveTo_Param.Coordinates);
                    }
                    break;
                // 相机到
                case CalibrationSubMachineModulesConst.RunTimeCtlCmdCamreaMoveTo:
                    {
                        CamreaMoveTo_Param camreaMoveTo_Param = new CamreaMoveTo_Param();
                        camreaMoveTo_Param.FromJson(jsParam);
                        ExecuteRobotMove(camreaMoveTo_Param.Coordinates);
                    }
                    break;
                // 激光到
                case CalibrationSubMachineModulesConst.RunTimeCtlCmdLaserMoveTo:
                    {
                        LaserMoveTo_Param laserMoveTo_Param = new LaserMoveTo_Param();
                        laserMoveTo_Param.FromJson(jsParam);
                        ExecuteRobotMove(laserMoveTo_Param.Coordinates);
                    }
                    break;
                // 出胶
                case CalibrationSubMachineModulesConst.RunTimeCtlCmdOutGlue:
                    {
                        OutGlue_Param outGlue_Param = new OutGlue_Param();
                        outGlue_Param.FromJson(jsParam);
                        param.Add(new GFBaseTypeParamValue("outGlue_Param", new GriffinsBaseValue(jsParam)));
                        iSubMMCmdExecutorCallBack.ExecSubMMMethod(
                            new SubMMAlias(calibrationSubMachineModulesInitCfg.ValveID),
                            DispensingFunctionHeadSubMachineModulesConst.OutGlue,
                            param);
                    }
                    break;
                case CalibrationSubMachineModulesConst.RunTimeCtlCmdCreateModel:
                    {
                        SearchMarkParams searchMarkParams = JsonObjConvert.FromJSon<SearchMarkParams>(jsParam);
                        visionDriver.CreateModel(searchMarkParams);
                        result = "OK";
                    }
                    break;
                case CalibrationSubMachineModulesConst.RunTimeCtlCmdSearchMark:
                    {
                        SearchMarkParams searchMarkParams = JsonObjConvert.FromJSon<SearchMarkParams>(jsParam);
                        SearchMarkResult searchMarkResult = visionDriver.SearchMark(searchMarkParams);
                        result = JsonObjConvert.ToJSon(searchMarkResult);
                    }
                    break;
                // 标定
                case CalibrationSubMachineModulesConst.RunTimeCtlCmdCalibrate:
                    {
                        //Calibrate_Param calibrate_Param = new Calibrate_Param();
                        //calibrate_Param.FromJson(jsParam);
                        Calibrate_Param calibrate_Param = new Calibrate_Param()
                        {
                            CalibrationType = CalibrationType.CameraScale,
                            FunctionHeadID = "cam01",
                            CalibrationParams = new CameraScaleCalibrationParameters()
                            {
                                CameraCoordinates = new Point3D() { X = 0, Y = 0, Z = 0 },
                                FunctionHeadId = "cam01",
                                MotionStep = 1,
                                VisionTemplateData = new byte[0]
                            }
                        };

                        CalibrationParameters calibrationParameters = calibrate_Param.CalibrationParams;
                        CalibrationBase calibrater = calibraters.Find(calibrate_Param.FunctionHeadID, calibrate_Param.CalibrationType);
                        if (calibrater != null && calibrationParameters != null)
                        {
                            calibrater.SetCalibrationParams(calibrationParameters);
                            Calibrate_Response calibrate_Response = new Calibrate_Response();
                            calibrate_Response.CalibrationResults = calibrater.Calibrate();
                            calibrate_Response.CalibrationType = calibrate_Param.CalibrationType;
                            result = calibrate_Response.ToJson();
                        }
                    }
                    break;
                // 获取标定结果
                case CalibrationSubMachineModulesConst.RunTimeCtlCmdGetCalibrationResult:
                    {
                        GetCalibrationResult_Param getCalibrationResult_Param = new GetCalibrationResult_Param();
                        getCalibrationResult_Param.FromJson(jsParam);

                        CalibrationBase calibrater = calibraters.Find(getCalibrationResult_Param.FunctionHeadID, getCalibrationResult_Param.CalibrationType);
                        if (calibrater != null)
                        {
                            GetCalibrationResult_Response getCalibrationResult_Response = new GetCalibrationResult_Response();
                            getCalibrationResult_Response.CalibrationResults = calibrater.GetCalibrationResult();
                            getCalibrationResult_Response.CalibrationType = getCalibrationResult_Param.CalibrationType;
                            result = getCalibrationResult_Response.ToJson();
                        }
                    }
                    break;

                default:
                    break;
            }

            rst["Result"] = new GriffinsBaseValue(result);
            rst["errorMsg"] = new GriffinsBaseValue(errorMsg);
            return rst;
        }

        /// <summary>
        /// 同步能力方法（返回能力方法调用结果）
        /// </summary>
        GFBaseTypeParamValueList ISubMMCmdExecutor.ExecMethod(string methodID, GFBaseTypeParamValueList param)
        {
            GFBaseTypeParamValueList rst = new GFBaseTypeParamValueList();
            rst.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue("0")));
            rst.Add(new GFBaseTypeParamValue("errorMsg", new GriffinsBaseValue("0")));

            string jsParam = param["jsParam"].ToJsonStrValue();
            string errorMsg = "";

            switch (methodID)
            {
                case CalibrationSubMachineModulesConst.GetCalibrationResultMethodID:
                    {
                        GetCalibrationResultParams p = string.IsNullOrEmpty(jsParam) ? null : JsonObjConvert.FromJSon<GetCalibrationResultParams>(jsParam);
                        if (p != null)
                        {
                            foreach (var rstCfg in calibrationSubMachineModulesInitCfg.calibrationResults)
                            {
                                if (rstCfg.ID == p.FunctionHeadID)
                                {
                                    rst["Result"] = new GriffinsBaseValue(JsonObjConvert.ToJSon(rstCfg));
                                    break;
                                }
                            }
                        }
                    }
                    break;

                case CalibrationSubMachineModulesConst.GetCalibrationObjectsMethodID:
                    {
                        var arr = calibrationSubMachineModulesFactoryCfg.calibrations.ToArray();
                        rst["Result"] = new GriffinsBaseValue(JsonObjConvert.ToJSon(arr));
                    }
                    break;

                case CalibrationSubMachineModulesConst.GetFunctionHeadsMethodID:
                    {
                        List<FunctionHeadInfos> functionHeadInfos = new List<FunctionHeadInfos>();
                        foreach (var item in calibraters)
                        {
                            functionHeadInfos.Add(new FunctionHeadInfos()
                            {
                                ID = item.Value.FunctionHead,
                                Name = ""
                            });
                        }
                        rst["Result"] = new GriffinsBaseValue(JsonObjConvert.ToJSon(functionHeadInfos.ToArray()));
                    }
                    break;

                default:
                    break;
            }

            rst["errorMsg"] = new GriffinsBaseValue(errorMsg);
            return rst;
        }

        Task<GFBaseTypeParamValueList> ISubMMCmdExecutor.AsynExecMethod(string methodID, GFBaseTypeParamValueList param)
        {
            throw new NotImplementedException();
        }

        GFParamValueList ISubMMCmdExecutor.ExecMethod(string methodID, GFParamValueList param)
        {
            throw new NotImplementedException();
        }

        Task<GFParamValueList> ISubMMCmdExecutor.AsynExecMethod(string methodID, GFParamValueList param)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 能力方法调用（执行会产生能力事件） - 同步
        /// </summary>
        GFBaseTypeParamValueList ISubMMCmdExecutor.ExecCabilityMethod(string methodID, GFBaseTypeParamValueList param)
        {
            GFBaseTypeParamValueList rst = new GFBaseTypeParamValueList();
            rst.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue("0")));
            rst.Add(new GFBaseTypeParamValue("errorMsg", new GriffinsBaseValue("0")));
            string errorMsg = "";
            string jsParam = param["jsParam"].ToJsonStrValue();

            switch (methodID)
            {
                default:
                    break;
            }

            rst["errorMsg"] = new GriffinsBaseValue(errorMsg);
            return rst;
        }

        Task<GFBaseTypeParamValueList> ISubMMCmdExecutor.AsynExecCabilityMethod(string methodID, GFBaseTypeParamValueList param)
        {
            throw new NotImplementedException();
        }

        public ICompUIDataObjPropValRW GetUIDataObjPropValRW()
        {
            throw new NotImplementedException();
        }

        private byte[] BuildRobotInitParameters()
        {
            Guid motionCardGuid = Guid.NewGuid();
            RobotInitParameters robotInitParameters = new RobotInitParameters
            {
                DriverName = RobotDriverNames.CategoryARobot,
                AxisBindings = new AxisBinding[]
                {
                    new AxisBinding(motionCardGuid, AxisConstants.X, AxisConstants.X),
                    new AxisBinding(motionCardGuid, AxisConstants.Y, AxisConstants.Y),
                    new AxisBinding(motionCardGuid, AxisConstants.Z, AxisConstants.Z),
                }
            };
            return JsonObjConvert.ToJSonBytes(robotInitParameters);
        }

        private RobotExecutionContext BuildRobotExecutionContext()
        {
            Guid motionCardGuid = Guid.NewGuid();
            return new RobotExecutionContext
            {
                CoordinateSystemId = 0,
                MotionControl = motionControl,
                AxisBindingPairs = new Dictionary<int, AxisBinding>
                {
                    [AxisConstants.X] = new AxisBinding(motionCardGuid, AxisConstants.X, AxisConstants.X),
                    [AxisConstants.Y] = new AxisBinding(motionCardGuid, AxisConstants.Y, AxisConstants.Y),
                    [AxisConstants.Z] = new AxisBinding(motionCardGuid, AxisConstants.Z, AxisConstants.Z),
                }
            };
        }

        private void ExecuteRobotMove(Point3D targetPosition)
        {
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
            EventHandler? moveFinishedHandler = null;
            EventHandler? moveFailedHandler = null;

            moveFinishedHandler = (sender, args) =>
            {
                robotDriver.MoveFinished -= moveFinishedHandler;
                robotDriver.MoveFailed -= moveFailedHandler;
                taskCompletionSource.TrySetResult(true);
            };

            moveFailedHandler = (sender, args) =>
            {
                robotDriver.MoveFinished -= moveFinishedHandler;
                robotDriver.MoveFailed -= moveFailedHandler;
                taskCompletionSource.TrySetException(new GKGException(MotionErrCodeConsts.ERR_MOTION_MOVE_FAIL, MotionErr.RobotMoveFailed, MotionErr.RobotMoveFailed));
            };

            robotDriver.MoveFinished += moveFinishedHandler;
            robotDriver.MoveFailed += moveFailedHandler;

            try
            {
                MotionCalculationParameters motionCalculationParameters = BuildRobotMoveCalculationParameters(targetPosition);
                MotionTrajectory motionTrajectory = motionCalculatorDriver.Calculate(motionCalculationParameters);

                robotDriver.Execute(
                    new MotionInstructionSequence()
                    {
                        SequenceType = MotionInstructionSequenceType.StepByStep,
                        Instructions = motionTrajectory.MotionInstructions ?? Array.Empty<MotionInstructionBase>()
                    },
                    robotExecutionContext);

                bool completed = taskCompletionSource.Task.Wait(60000);
                if (!completed)
                {
                    throw new GKGException(MotionErrCodeConsts.ERR_MOTION_ROBOT_MOVE_TIMEOUT, MotionErr.RobotMoveTimeout, MotionErr.RobotMoveTimeout);
                }
            }
            finally
            {
                robotDriver.MoveFinished -= moveFinishedHandler;
                robotDriver.MoveFailed -= moveFailedHandler;
            }
        }

        private MotionCalculationParameters BuildRobotMoveCalculationParameters(Point3D targetPosition)
        {
            double safeZ = Math.Max(currentPosition.Z, calibrationSubMachineModulesInitCfg.SafetyHeight);

            return new MotionCalculationParameters
            {
                ProductProcessingTrajectory = new ProductProcessingTrajectoryItem[]
                {
                    BuildDotTrajectory(
                        new AxisConstantValues[]
                        {
                            new AxisConstantValues() { Axis = AxisConstants.Z, PositionValue = safeZ }
                        },
                        calibrationSubMachineModulesInitCfg.ZMoveParameters),
                    BuildDotTrajectory(
                        new AxisConstantValues[]
                        {
                            new AxisConstantValues() { Axis = AxisConstants.X, PositionValue = targetPosition.X },
                            new AxisConstantValues() { Axis = AxisConstants.Y, PositionValue = targetPosition.Y }
                        },
                        calibrationSubMachineModulesInitCfg.XYMoveParameters),
                    BuildDotTrajectory(
                        new AxisConstantValues[]
                        {
                            new AxisConstantValues() { Axis = AxisConstants.Z, PositionValue = targetPosition.Z }
                        },
                        calibrationSubMachineModulesInitCfg.ZMoveParameters)
                }
            };
        }

        private NonProcessingTrajectory BuildDotTrajectory(AxisConstantValues[] targetAxisValues, NonProcessingTrajectoryParameters moveParameters)
        {
            return new NonProcessingTrajectory
            {
                MotionTrajectory = new DotMotionTrajectory
                {
                    TargetPoint = new MotionTrajectoryPoint
                    {
                        Position = targetAxisValues
                    }
                },
                NonProcessingParameters = moveParameters
            };
        }

        private void RobotDriver_PositionChanged(object? sender, PositionChangedEventArgs e)
        {
            foreach (var position in e.NewPosition)
            {
                switch (position.Axis)
                {
                    case AxisConstants.X:
                        currentPosition.X = position.PositionValue;
                        break;
                    case AxisConstants.Y:
                        currentPosition.Y = position.PositionValue;
                        break;
                    case AxisConstants.Z:
                        currentPosition.Z = position.PositionValue;
                        break;
                }
            }
        }
    }
}