using GF_Gereric;
using GKG.ElectronicControl;
using GKG.ElectronicControl.General;


namespace GKG.EletronicManager
{
    public static class EletronicManager
    {
        private static Dictionary<MotionCardType, IMotionControlBase> motionCardInstanceDict = new Dictionary<MotionCardType, IMotionControlBase>();

        private static Dictionary<MotionCardType, Dictionary<string,IBaseStateIO>> motionCardStateIO =new Dictionary <MotionCardType, Dictionary<string,IBaseStateIO>>();

        private static bool initSucced = false;
        public static void Initialize()
        {
            if(initSucced) return;

            motionCardInstanceDict.Clear();
            MotionControlPluginManager.Init();
            motionCardInstanceDict.Add(MotionCardType.GMCMINI, MotionControlPluginManager.GetMotionControl(MotionCardType.GMCMINI));
            motionCardInstanceDict.Add(MotionCardType.GC800, MotionControlPluginManager.GetMotionControl(MotionCardType.GC800));

            motionCardInstanceDict[MotionCardType.GMCMINI].IniMotionCard(0);
            motionCardInstanceDict[MotionCardType.GC800].IniMotionCard(0);

            InitStateIOControl();
            // 整机上电
            motionCardStateIO[MotionCardType.GMCMINI]["RW13"].Write(true);
            Thread.Sleep(5000);

            InitGC800Parameters(new byte[0]);
            InitGMCMiniParameters(new byte[0]);

            // 所有轴使能
            foreach (var card in motionCardInstanceDict.Values)
            {
                for(int i = 0;i<card.SupportAxisNum;i++)
                {
                    Guid guid = card.LockAxis(i,1000);
                    card.ClearAxisAlarm(guid);
                    card.AxisEnabled(i, true);
                    card.UnLockAxis(guid);
                }
            }
            initSucced = true;
        }

        /// <summary>
        /// 状态量IO控制初始化
        /// </summary>
        private static void InitStateIOControl()
        {
            motionCardStateIO.Clear();
            foreach (var motionCard in motionCardInstanceDict)
            {
                if(motionCard.Key == MotionCardType.GC800)
                {
                    motionCardStateIO.Add(MotionCardType.GC800, new Dictionary<string, IBaseStateIO>());
                    IMotionControlCategoryA motionControlGaoChAuto = motionCard.Value as IMotionControlCategoryA;
                    ChannelParametersList channelParametersList = motionCard.Value.StateChannelList;
                    int ioNums = motionControlGaoChAuto.SupportIoStateNum;
                    for(int i = 0; i < ioNums; i++)
                    {
                        StateControlMotionCard inputChannel = new StateControlMotionCard();
                        StateControlMotionCard outputChannel = new StateControlMotionCard();
                        ChannelParameters inputChannelParam = channelParametersList.Find(ReadWriteModeConstStr.ReadOnlyStr + i.ToString("00"));
                        ChannelParameters outputChannelParam = channelParametersList.Find(ReadWriteModeConstStr.ReadWriteStr + i.ToString("00"));
                        IOStateInitParameters inputInitParams = new IOStateInitParameters()
                        {
                            deviceID = "0",
                            channelID = inputChannelParam.channelID
                        };
                        IOStateInitParameters outputInitParams = new IOStateInitParameters()
                        {
                            deviceID = "0",
                            channelID = outputChannelParam.channelID
                        };
                        inputChannel.Init(JsonObjConvert.ToJSonBytes(inputInitParams));
                        outputChannel.Init(JsonObjConvert.ToJSonBytes(outputInitParams));
                        inputChannel.SetDeviceInstance(motionControlGaoChAuto);
                        outputChannel.SetDeviceInstance(motionControlGaoChAuto);
                        motionCardStateIO[motionCard.Key].Add(inputChannelParam.channelID, inputChannel);
                        motionCardStateIO[motionCard.Key].Add(outputChannelParam.channelID, outputChannel);
                    }   
                }
                else if(motionCard.Key == MotionCardType.GMCMINI)
                {
                    motionCardStateIO.Add(MotionCardType.GMCMINI, new Dictionary<string, IBaseStateIO>());
                    IMotionControlBase motionControlGMCMini = motionCard.Value as IMotionControlBase;
                    ChannelParametersList channelParametersList = motionControlGMCMini.StateChannelList;
                    int ioNums = motionControlGMCMini.SupportIoStateNum;
                    for (int i = 0; i < ioNums; i++)
                    {
                        StateControlMotionCard inputChannel = new StateControlMotionCard();
                        StateControlMotionCard outputChannel = new StateControlMotionCard();
                        ChannelParameters inputChannelParam = channelParametersList.Find(ReadWriteModeConstStr.ReadOnlyStr + i.ToString("00"));
                        ChannelParameters outputChannelParam = channelParametersList.Find(ReadWriteModeConstStr.ReadWriteStr + i.ToString("00"));
                        IOStateInitParameters inputInitParams = new IOStateInitParameters()
                        {
                            deviceID = "0",
                            channelID = inputChannelParam.channelID
                        };
                        IOStateInitParameters outputInitParams = new IOStateInitParameters()
                        {
                            deviceID = "0",
                            channelID = outputChannelParam.channelID
                        };
                        inputChannel.Init(JsonObjConvert.ToJSonBytes(inputInitParams));
                        outputChannel.Init(JsonObjConvert.ToJSonBytes(outputInitParams));
                        inputChannel.SetDeviceInstance(motionControlGMCMini);
                        outputChannel.SetDeviceInstance(motionControlGMCMini);
                        motionCardStateIO[motionCard.Key].Add(inputChannelParam.channelID, inputChannel);
                        motionCardStateIO[motionCard.Key].Add(outputChannelParam.channelID, outputChannel);
                    }
                }
            }
        }

        public static void InitGC800Parameters(byte[] initCfg)
        {
            MotionControlFactoryParameters motionControlFactoryParameters = new MotionControlFactoryParameters();
            motionControlFactoryParameters.Parameters = new MotionControlFactoryParameter[motionCardInstanceDict[MotionCardType.GC800].SupportAxisNum];
            for (int i = 0; i < motionCardInstanceDict[MotionCardType.GC800].SupportAxisNum; i++)
            {
                motionControlFactoryParameters.Parameters[i] = new MotionControlFactoryParameter();
                motionControlFactoryParameters.Parameters[i].AxisNo = i;
                motionControlFactoryParameters.Parameters[i].AxisHomingParameters.HomingMode = MotionControlAxisHomingMode.NegativeGoHome;
                motionControlFactoryParameters.Parameters[i].AxisHomingParameters.HomingMinimumSpeed = 1;
                motionControlFactoryParameters.Parameters[i].AxisHomingParameters.HomingMaximumSpeed = 10;
                motionControlFactoryParameters.Parameters[i].AxisHomingParameters.HomingRetractDistance = 3;
                motionControlFactoryParameters.Parameters[i].AxisHomingParameters.HomingAccelerationTime = 1;
                motionControlFactoryParameters.Parameters[i].PulseRatioParameters.PulseRatio = 1;
            }
            motionCardInstanceDict[MotionCardType.GC800].SetFactoryParameters(motionControlFactoryParameters);
            var PositionComparison2DParameters = new MotionControlPositionComparison2DParameters();
            PositionComparison2DParameters.CoordinateSystemId = 0;
            PositionComparison2DParameters.AxisList = new int[] { 0, 1, 3 };
            PositionComparison2DParameters.ChannelList = new int[] { 0, 1 };
            PositionComparison2DParameters.PulseWidth = 1000;
            PositionComparison2DParameters.PulseOffTime = 1000;
            PositionComparison2DParameters.StartLevel = 1;
            PositionComparison2DParameters.MaxAcceleration = 1000;
            PositionComparison2DParameters.CoordinateSystem = new MotionControlCoordinateSystemParameters();
            PositionComparison2DParameters.CoordinateSystem.MaxCompositeSpeed = 1000;
            PositionComparison2DParameters.CoordinateSystem.MaxCompositeAcceleration = 10000;
            motionCardInstanceDict[MotionCardType.GC800].SetAxisMoveParameter(new Guid(), new MotionControlArcFeedForwardParameters(), PositionComparison2DParameters);
        }

        public static void InitGMCMiniParameters(byte[] initCfg)
        {
            MotionControlFactoryParameters motionControlFactoryParameters = new MotionControlFactoryParameters();
            var motionControlGMCMini = motionCardInstanceDict[MotionCardType.GMCMINI];
            motionControlFactoryParameters.Parameters = new MotionControlFactoryParameter[motionCardInstanceDict[MotionCardType.GMCMINI].SupportAxisNum];
            for (int i = 0; i < motionControlGMCMini.SupportAxisNum; i++)
            {
                motionControlFactoryParameters.Parameters[i] = new MotionControlFactoryParameter();
                motionControlFactoryParameters.Parameters[i].AxisNo = i;
                motionControlFactoryParameters.Parameters[i].AxisHomingParameters.HomingMode = MotionControlAxisHomingMode.NegativeGoHome;
                motionControlFactoryParameters.Parameters[i].AxisHomingParameters.HomingMinimumSpeed = 1;
                motionControlFactoryParameters.Parameters[i].AxisHomingParameters.HomingMaximumSpeed = 10;
                motionControlFactoryParameters.Parameters[i].AxisHomingParameters.HomingRetractDistance = 3;
                motionControlFactoryParameters.Parameters[i].AxisHomingParameters.HomingAccelerationTime = 1;
                motionControlFactoryParameters.Parameters[i].PulseRatioParameters.PulseRatio = 1;
            }
        }

        private static IBaseThermostat _thermostat;
        public static IBaseThermostat GetThermostat()
        {
            
            if(_thermostat == null)
            {
                SerialConfig serialConfig = new SerialConfig()
                {
                    PortName = "COM1",
                    BaudRate = 9600,
                    Parity = RJCP.IO.Ports.Parity.None,
                    StopBits = RJCP.IO.Ports.StopBits.One,
                    DataBits = 8,
                    IsEnableCRC16 = false,
                    ModbusType = EModbusType.RS485
                };
                _thermostat = new ThermostatTypeOML();
                byte[] initCfg = JsonObjConvert.ToJSonBytes(serialConfig);
                _thermostat.Init(initCfg);
            }
            return _thermostat;
        }

        public static IMotionControlBase GetMotionControl(MotionCardType motionCardType)
        {
            return motionCardInstanceDict[motionCardType];
        }

        public static IBaseStateIO GetStateIOControl(MotionCardType motionCardType, string channelID)
        {
            return motionCardStateIO[motionCardType][channelID];
        }
    }
}

