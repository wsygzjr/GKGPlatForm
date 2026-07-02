using GF_Gereric;
using GKG.ElectronicControl.General;
using Griffins;
using Griffins.ImeIOT;
using Griffins.PF.Server;

namespace GKG.SubMM.Dispenser
{
    public class WeighingBalanceSubMMCmdExecutor : ISubMMCmdExecutor, ISubMMManualModeCmdExecutor, ISubMMAutoModeCmdExecutor
    {
        private WeighingBalanceSubMachineModulesFactoryCfg weighingBalanceSubMachineModulesFactoryCfg;

        private WeighingBalanceSubMachineModulesInitCfg weighingBalanceSubMachineModulesInitCfg;

        private WeighingBalanceSubMachineModulesPPCfg weighingBalanceSubMachineModulesPPCfg;

        private ISubMMCmdExecutorCallBack iSubMMCmdExecutorCallBack;

        private SubMMAlias alias;

        private ImeGenNormalEventHandler imeGenNormalEventHandler;

        private ImeCabilityEventHandler imeCabilityEventHandler;

        private ImeAlarmEventHandler imeAlarmEventHandler;

        public WeighingBalanceSubMMCmdExecutor(SubMMAlias alias, byte[] factoryCfgInfo)
        {
            this.alias = alias;
            weighingBalanceSubMachineModulesFactoryCfg = new WeighingBalanceSubMachineModulesFactoryCfg();
            if(factoryCfgInfo != null && factoryCfgInfo.Length>0)
            {
                weighingBalanceSubMachineModulesFactoryCfg.FromBytes(factoryCfgInfo);
            }
            weighingBalanceSubMachineModulesInitCfg = new WeighingBalanceSubMachineModulesInitCfg();
            weighingBalanceSubMachineModulesPPCfg = new WeighingBalanceSubMachineModulesPPCfg();
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
        /// 称重天平实例
        /// </summary>
        private IWeighingBalanceBase weighingBalance;

        /// <summary>
        /// 初始化（在创建子机械模组实例后首先调用）
        /// </summary>
        /// <param name="initCfgInfo">初始化参数，null表示缺省值</param>
        /// <param name="callBack">子机械模组（复合子机械模组）运行时回调接口</param>
        public void Init(byte[] initCfgInfo, byte[] calibrationCfgInfo, ISubMMCmdExecutorCallBack callBack)
        {
            if (initCfgInfo != null && initCfgInfo.Length > 0)
            {
                weighingBalanceSubMachineModulesInitCfg = new WeighingBalanceSubMachineModulesInitCfg();
                weighingBalanceSubMachineModulesInitCfg.FromBytes(initCfgInfo);
            }
            iSubMMCmdExecutorCallBack = callBack;
            // 创建天平实例
            weighingBalance = WeighingBalanceBase.CreateWeighingBalance(weighingBalanceSubMachineModulesFactoryCfg.WeighingBalanceType);
            WeighingBalanceApwInitParams weighingBalanceApwInitParams = new WeighingBalanceApwInitParams()
            {
                SerialConfig = JsonObjConvert.ToJSonBytes(new SerialConfig()
                {
                    PortName = "COM5",
                    BaudRate = 38400,
                    DataBits = 8,
                    StopBits = RJCP.IO.Ports.StopBits.One,
                    ModbusType = EModbusType.RS232,
                    IsEnableCRC16 = false,
                })
            };
            weighingBalanceSubMachineModulesInitCfg.WeighingBalanceInitParams = JsonObjConvert.ToJSonBytes(weighingBalanceApwInitParams);
            weighingBalance.Init(weighingBalanceSubMachineModulesInitCfg.WeighingBalanceInitParams);
        }
        void ISubMMCmdExecutor.BeforeInit(GFBaseTypePropValueList devicePropValues)
        {

        }

        void ISubMMCmdExecutor.AfterInit()
        {
            autoWeighingTask = Task.Run(() => { threadAutoWeighing(); });
        }

        void ISubMMCmdExecutor.UnInit()
        {
            exit = true;
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

        bool ISubMMAutoModeCmdExecutor.CanStartWork(out string reasonMsg)
        {
            reasonMsg = string.Empty;
            return true;
        }

        void ISubMMAutoModeCmdExecutor.SetRunMode(ImeRunMode imeRunMode)
        {

        }
        /// <summary>
        /// 设置配方参数
        /// </summary>
        /// <param name="pfCfgInfo">配方参数，null表示缺省值</param>
        public void SetPFCfgInfo(byte[] pfCfgInfo)
        {
            if(pfCfgInfo != null && pfCfgInfo.Length > 0)
            {
                weighingBalanceSubMachineModulesPPCfg = new WeighingBalanceSubMachineModulesPPCfg();
                weighingBalanceSubMachineModulesPPCfg.FromBytes(pfCfgInfo);
            }
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
            // 保持原实现简单空返回结构
            return ExecCtlCmdCore(cmdID, cmdParam);
        }

        public void AfterStopWork()
        {
            exit = true;
            autoWeighingTask?.Wait();
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
                case WeighingBalanceSubMachineModulesConst.WeighingMethodID:
                    {
                        // 执行称重
                        string valveID = jsParam;
                        double singlePointWeight = weighingBalance.GetWeight();//SinglePointWeightJob(valveID);
                        rst["Result"] = new GriffinsBaseValue(singlePointWeight.ToString());
                    }
                    break;

                default:
                    break;
            }

            rst["errorMsg"] = new GriffinsBaseValue(errorMsg);
            return rst;
        }

        public GFBaseTypeParamValueList ExecCabilityMethod(string methodID, GFBaseTypeParamValueList param)
        {
            GFBaseTypeParamValueList rst = new GFBaseTypeParamValueList();
            rst.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue("0")));
            rst.Add(new GFBaseTypeParamValue("errorMsg", new GriffinsBaseValue("0")));

            string jsParam = param["jsParam"].ToStringVal();
            string errorMsg = "";

            switch (methodID)
            {
                case WeighingBalanceSubMachineModulesConst.WeighingMethodID:
                    {
                        // 执行称重
                        string valveID = jsParam;
                        double singlePointWeight = weighingBalance.GetWeight();//SinglePointWeightJob(valveID);

                        rst["Result"] = new GriffinsBaseValue(singlePointWeight.ToString());
                    }
                    break;

                default:
                    break;
            }

            rst["errorMsg"] = new GriffinsBaseValue(errorMsg);
            return rst;
        }

        private bool stop = false;

        private readonly object singlePointWeightJobLock = new object();

        /// <summary>
        /// 单点称重
        /// </summary>
        /// <param name="valveID">阀id</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private double SinglePointWeightJob(string valveID)
        {
            lock (singlePointWeightJobLock)
            {
                int rtn = 0;
                double SinglePointWeight = 0.0;
                //--------------------------------------------
                WeighingType eWorkMode = WeighingType.SinglePointWeighing;
                //--------------------------------------------
                WeighingParameter weighingParameter = weighingBalanceSubMachineModulesPPCfg.WeighingParameters.Find(valveID);
                Point3D sCCDPos = weighingParameter.WeighingPosition;

                PositionTeachMode eWeightPosMode = weighingParameter.PositionMode;
                OffsetCalibrationResult offsetCalibrationResult = new OffsetCalibrationResult();
                //foreach (var value in weighingBalanceSubMachineModulesInitCfg.OffsetCalibrationResults)
                //{
                //    if (value.FunctionHeadId == valveID)
                //    {
                //        offsetCalibrationResult = value;
                //        break;
                //    }
                //}

                // 计算阀头位置
                Point3D sDesPos = new Point3D();
                if (weighingParameter.PositionMode == PositionTeachMode.CCDPosition)
                {
                    sDesPos = (Point3D)offsetCalibrationResult.Calculate(sCCDPos);
                }
                else
                {
                    sDesPos = sCCDPos;
                }

                //-----------------------------------------------------------------
                MoveParam moveParam = new MoveParam()
                {
                    AxisCount = 3,
                    logicAxis = new int[3] { AxisConstants.X, AxisConstants.Y, AxisConstants.Z },
                    targetPosition = new Point3D(sDesPos.X, sDesPos.Y, 5),
                    acc = 0,
                    speed = 0,
                };
                GFBaseTypeParamValueList param = new GFBaseTypeParamValueList();
                param.Add(new GFBaseTypeParamValue("moveParam", new GriffinsBaseValue(JsonObjConvert.ToJSon(moveParam))));
                //iSubMMCmdExecutorCallBack.ExecSubMMMethod(
                //    new SubMMAlias(weighingBalanceSubMachineModulesInitCfg.RobotID),
                //    BasicRobotSubMachineModulesConst.MoveMethodID,
                //    param);
                //if (stop)
                //    return 0;

                moveParam = new MoveParam()
                {
                    AxisCount = 1,
                    logicAxis = new int[1] { AxisConstants.Z },
                    targetPosition = new Point3D(sDesPos.Z, 0, 0),
                    acc = 0,
                    speed = 0,
                };
                param["moveParam"] = new GriffinsBaseValue(JsonObjConvert.ToJSon(moveParam));
                //iSubMMCmdExecutorCallBack.ExecSubMMMethod(
                //    new SubMMAlias(weighingBalanceSubMachineModulesInitCfg.RobotID),
                //    BasicRobotSubMachineModulesConst.MoveMethodID,
                //    param);

                int iRepeatCount = weighingParameter.SinglePointWeighingParams.CycleCount;
                if (iRepeatCount <= 0)
                    iRepeatCount = 1;
                for (int i = 0; i < iRepeatCount; i++)
                {
                    double dStartWeight = 0.0;
                    double dElectronic_StartWeight = 0.0;
                    double dElectronic_EndWeight = 0.0;

                    int iStableTime_ms = (int)(weighingParameter.SinglePointWeighingParams.OnceInterval * 1000);
                    if (iStableTime_ms <= 0) iStableTime_ms = 1000;
                    if (iStableTime_ms >= 60000) iStableTime_ms = 60000;

                    for (int iErrorIndex = 0; iErrorIndex < 5; iErrorIndex++)
                    {
                        try
                        {
                            dStartWeight = 0;
                            dStartWeight = weighingBalance.GetWeight();
                        }
                        catch (Exception)
                        {
                            if (iErrorIndex < 4)
                            {
                                continue;
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }

                    dElectronic_StartWeight = dStartWeight;

                    List<double> vecWeightData = new List<double>();
                    List<double> vecCPKWeightData = new List<double>();

                    if (stop)
                        return 0;

                    if (i == 0)
                    {
                        Thread.Sleep(200);
                    }

                    int dPoints = weighingParameter.SinglePointWeighingParams.PointCount;
                    int iWeightCount = weighingParameter.SinglePointWeighingParams.SprayCount;

                    for (int Index = 0; Index < iWeightCount; Index++)
                    {
                        if (weighingParameter.WeighingMode == WeighingType.MassFlowWeighing)
                        {
                            // mass flow handling omitted
                        }
                        else
                        {
                            weighingBalanceSubMachineModulesInitCfg.ValveID.TryGetValue(valveID, out string valveAlias);
                            param.Add(new GFBaseTypeParamValue("Points", new GriffinsBaseValue(dPoints)));
                            iSubMMCmdExecutorCallBack.ExecSubMMMethod(
                                new SubMMAlias(valveAlias),
                                DispensingFunctionHeadSubMachineModulesConst.OutGlue,
                                param);
                        }

                        if (stop)
                            return 0;

                        Thread.Sleep(iStableTime_ms);
                        double dWeight = 0;
                        for (int iErrorIndex = 0; iErrorIndex < 5; iErrorIndex++)
                        {
                            try
                            {
                                dWeight = 0;
                                dWeight = weighingBalance.GetWeight();
                            }
                            catch
                            {
                                if (iErrorIndex < 4)
                                {
                                    continue;
                                }
                                else
                                {
                                    throw new Exception();
                                }
                            }

                            Thread.Sleep(50);
                        }

                        if (stop)
                            return 0;

                        double dCurWeight = dWeight - dStartWeight;

                        if (dCurWeight <= 0)
                        {
                            return -1;
                        }

                        vecWeightData.Add(dCurWeight);

                        Thread.Sleep(50);

                        dStartWeight = dWeight;
                        dElectronic_EndWeight = dWeight;

                        double dTemDotWeight = (dCurWeight / dPoints) * 1000.0;

                        vecCPKWeightData.Add(dTemDotWeight);
                    }

                    bool bReCalSinglePointWeight = false;
                    if (rtn == 0)
                    {
                        int iDataSize = vecWeightData.Count;
                        if (iDataSize > 0)
                        {
                            double dSumData = 0.0;

                            for (int Index = 0; Index < iDataSize; Index++)
                            {
                                dSumData += vecWeightData[Index] * 1000.0;
                            }
                            double dWeight = dSumData / ((double)iDataSize * (double)dPoints);
                            double dRealSingleWeight = dWeight;

                            double dUpperLimit = weighingParameter.WeightReference + weighingParameter.SinglePointWeighingParams.dDotUpperLimit;
                            double dLowerLimit = weighingParameter.WeightReference - weighingParameter.SinglePointWeighingParams.dDotLowerLimit;

                            if (dWeight >= dLowerLimit && dWeight <= dUpperLimit)
                            {
                                dRealSingleWeight = dWeight;
                                SinglePointWeight = dRealSingleWeight;
                                if (rtn == 0)
                                {
                                    // store or log if needed
                                }
                            }
                        }
                    }
                }

                moveParam = new MoveParam()
                {
                    AxisCount = 1,
                    logicAxis = new int[1] { AxisConstants.Z },
                    targetPosition = new Point3D(sDesPos.Z, 0, 0),
                    acc = 0,
                    speed = 0,
                };
                param["moveParam"] = new GriffinsBaseValue(JsonObjConvert.ToJSon(moveParam));
                //iSubMMCmdExecutorCallBack.ExecSubMMMethod(
                //    new SubMMAlias(weighingBalanceSubMachineModulesInitCfg.RobotID),
                //    BasicRobotSubMachineModulesConst.MoveMethodID,
                //    param);

                return SinglePointWeight;
            }
        }

        private void SpitGlue_ByWeight(string valveID, double dQuantifyWeight)
        {
            // 保留原注释逻辑，实际实现略
        }

        private void Weight_Syringe(string valveID)
        {
        }

        private bool QuantitativeWeightJob(string valveID)
        {
            // 保留原实现体（简化），返回示例值
            return true;
        }

        private bool exit = false;
        private Task autoWeighingTask;

        private void threadAutoWeighing()
        {
            return;
            //while (true)
            //{
            //    if (exit)
            //        break;
            //    foreach (var item in weighingBalanceSubMachineModulesInitCfg.WeighingParameters)
            //    {
            //        DateTime dateTime = DateTime.Now;
            //        if (item.WeighingTimeTable.Enabled)
            //        {
            //            foreach (var time in item.WeighingTimeTable.WeighingTimeItems)
            //            {
            //                if (dateTime.Hour == time.Hour && dateTime.Minute == time.Minute)
            //                {
            //                    SinglePointWeightJob(item.FunctionHeadID);
            //                }
            //            }
            //        }
            //    }
            //    Thread.Sleep(1000);
            //}
        }

        public void BeforeInit(string[] subMechCompParam)
        {

        }

        public Task<GFBaseTypeParamValueList> AsynExecMethod(string methodID, GFBaseTypeParamValueList param)
        {
            return null;
        }

        public GFParamValueList ExecMethod(string methodID, GFParamValueList param)
        {
            return null;
        }

        public Task<GFParamValueList> AsynExecMethod(string methodID, GFParamValueList param)
        {
            return null;
        }

        public Task<GFBaseTypeParamValueList> AsynExecCabilityMethod(string methodID, GFBaseTypeParamValueList param)
        {
            return null;
        }

        public ICompUIDataObjPropValRW GetUIDataObjPropValRW()
        {
            return null;
        }
        private GFBaseTypeParamValueList ExecCtlCmdCore(string cmdID, GFBaseTypeParamValueList cmdParam)
        {
            GFBaseTypeParamValueList rst = new GFBaseTypeParamValueList();
            switch (cmdID)
            {
                case WeighingBalanceSubMachineModulesConst.GetWeightCmdID:
                    {
                        double weight = weighingBalance.Weight;
                        rst.Add(new GFBaseTypeParamValue("Weight", new GriffinsBaseValue(weight.ToString())));
                    }
                    break;
                case WeighingBalanceSubMachineModulesConst.GetAxisOptionsCmdID:
                    {
                        List<AxisInformation> axisInformation;
                        Dictionary<Guid, AxisInformation> axisInfoDict = new Dictionary<Guid, AxisInformation>();
                        var axisInfosResponse = ServerInnerInfoSender.SendMutualInfo(
                            AxisInfosRequest.InfoKindID,
                            new AxisInfosRequest(MotionControlCardType.Normal));

                        if (axisInfosResponse == null || axisInfosResponse.Count == 0)
                            axisInformation = new List<AxisInformation>();

                        AxisInfosResponse response = axisInfosResponse[0].Response as AxisInfosResponse;
                        if (response?.AxisInformations == null)
                            axisInformation = new List<AxisInformation>();

                        foreach (AxisInformation axisInfo in response.AxisInformations)
                        {
                            if (axisInfo != null && axisInfo.AxisGuid != Guid.Empty)
                                axisInfoDict[axisInfo.AxisGuid] = axisInfo;
                        }
                        axisInformation = axisInfoDict.Values.ToList();
                        rst.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue(JsonObjConvert.ToJSon(axisInformation))));
                    }
                    break;
                case WeighingBalanceSubMachineModulesConst.GetValveOptionsCmdID:
                    {
                        Dictionary<string, string> valveInstances = new Dictionary<string, string>();
                        valveInstances.Add("Valve1", "阀1");
                        valveInstances.Add("Valve2", "阀2");
                        rst.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue(JsonObjConvert.ToJSon(valveInstances))));
                    }
                    break;
                case WeighingBalanceSubMachineModulesConst.GetInitCfgCmdID:
                    {
                        rst.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue(JsonObjConvert.ToJSon(weighingBalanceSubMachineModulesInitCfg))));
                    }
                    break;
                default:
                    break;
            }
            return rst;

        }
    }
}