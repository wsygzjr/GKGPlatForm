using GF_Gereric;
using Griffins.ImeIOT;
using GKG.MotionControl;
using Griffins;
using System.Threading.Tasks;
using GKG.Maths;
namespace GKG.SubMM
{
    public class MotionCalculateSubMMCmdExecutor : ISubMMCmdExecutor
    {
        private MotionCalculateSubMachineModulesFactoryCfg motionCalculateSubMachineModulesFactoryCfg;
        private MotionCalculateSubMachineModulesInitCfg motionCalculateSubMachineModulesInitCfg;
        private MotionCalculateSubMachineModulesPPCfg motionCalculateSubMachineModulesPPCfg;
        private ISubMMCmdExecutorCallBack iSubMMCmdExecutorCallBack;
        private SubMMAlias alias;
        private ImeGenNormalEventHandler imeGenNormalEventHandler;
        private ImeCabilityEventHandler imeCabilityEventHandler;
        private ImeAlarmEventHandler imeAlarmEventHandler;
        private IMotionCalculatorDriver motionCalculatorDriver;
        private Dictionary<string, Point2D> functionHeadOffset = new Dictionary<string, Point2D>();

        public MotionCalculateSubMMCmdExecutor(SubMMAlias alias, Guid subMMObjID, byte[] factoryCfgInfo)
        {
            this.alias = alias;
            motionCalculateSubMachineModulesFactoryCfg = new MotionCalculateSubMachineModulesFactoryCfg();
            motionCalculateSubMachineModulesFactoryCfg.FromBytes(factoryCfgInfo);

            switch(subMMObjID)
            {
                case var id when id == MotionCalculateSubMachineModulesConst.SubMMObjInfos[0].SubMMObjID:
                    motionCalculatorDriver = MotionCalculatorPluginManager.GetMotionCalculatorDriver(MotionCalculatorDriverNames.StraightAxis);
                    break;
                case var id when id == MotionCalculateSubMachineModulesConst.SubMMObjInfos[1].SubMMObjID:
                    motionCalculatorDriver = MotionCalculatorPluginManager.GetMotionCalculatorDriver(MotionCalculatorDriverNames.Plane);
                    break;
                case var id when id == MotionCalculateSubMachineModulesConst.SubMMObjInfos[2].SubMMObjID:
                    motionCalculatorDriver = MotionCalculatorPluginManager.GetMotionCalculatorDriver(MotionCalculatorDriverNames.XYZ_xyz);
                    break;
                default:
                    throw new Exception($"未知的子机械模组实现对象ID: {subMMObjID}");
            }

            motionCalculateSubMachineModulesInitCfg = new MotionCalculateSubMachineModulesInitCfg();
            motionCalculateSubMachineModulesPPCfg = new MotionCalculateSubMachineModulesPPCfg();
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

        public void BeforeInit(string[] subMechCompParam)
        {
            // 可根据需要实现
        }

        void ISubMMCmdExecutor.Init(byte[] initCfgInfo, ISubMMCmdExecutorCallBack callBack)
        {
            motionCalculateSubMachineModulesInitCfg.FromBytes(initCfgInfo);
            functionHeadOffset.Clear();
            foreach (var offset in motionCalculateSubMachineModulesInitCfg.offsetCalibrationResults)
            {
                functionHeadOffset.Add(offset.FunctionHeadId, offset.OffsetValue);
            }

            iSubMMCmdExecutorCallBack = callBack;
            if (motionCalculatorDriver != null)
            {
                motionCalculatorDriver.Init(initCfgInfo);
            }
        }

        void ISubMMCmdExecutor.SetPFCfgInfo(byte[] pfCfgInfo)
        {
            motionCalculateSubMachineModulesPPCfg.FromBytes(pfCfgInfo);
        }

        void ISubMMCmdExecutor.StartWork() { }
        void ISubMMCmdExecutor.StopWork() { }
        void ISubMMCmdExecutor.Pause() { }
        void ISubMMCmdExecutor.Resume() { }
        void ISubMMCmdExecutor.BeforeSwitchPF() { }
        void ISubMMCmdExecutor.AfterStopWork() { }

        GFBaseTypeParamValueList ISubMMCmdExecutor.ExecMethod(string methodID, GFBaseTypeParamValueList param)
        {
            GFBaseTypeParamValueList rst = new GFBaseTypeParamValueList();
            rst.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue("0")));
            rst.Add(new GFBaseTypeParamValue("errorMsg", new GriffinsBaseValue("0")));
            string errorMsg = "";
            string jsParam = param["jsParam"].ToJsonStrValue();
            switch (methodID)
            {
                case MotionCalculateSubMachineModulesConst.CalculateMethodID:
                    {
                        MotionCalculateParameters motionCalculateParameters = JsonObjConvert.FromJSon<MotionCalculateParameters>(jsParam);
                        Dictionary<string, MotionCalculationParameters> motionCalculationParameters = BuildMotionCalculationParameters(motionCalculateParameters);
                        MotionTrajectory motionTrajectory = motionCalculatorDriver.Calculate(motionCalculationParameters);

                        rst["Result"] = new GriffinsBaseValue(JsonObjConvert.ToJSon(motionTrajectory));
                    }
                    break;
                //case MotionCalculateSubMachineModulesConst.CalculateDoubleValveSpacingMethodID:
                //    {
                //        MotionCalculateParameters motionCalculateParameters = JsonObjConvert.FromJSon<MotionCalculateParameters>(jsParam);
                //        GKGMath.SortAndPairValves(motionCalculateParameters.functionHeadIDs, ref motionCalculateParameters.parameters, out Point2D viceDistance, out Point2D viceTargetPosition);
                //        rst["Result"] = new GriffinsBaseValue(JsonObjConvert.ToJSon(motionCalculateParameters));
                //        rst["viceTargetPosition"] = new GriffinsBaseValue(JsonObjConvert.ToJSon(viceTargetPosition));
                //    }
                //    break;
                default:
                    break;
            }
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

        GFBaseTypeParamValueList ISubMMCmdExecutor.ExecCabilityMethod(string methodID, GFBaseTypeParamValueList param)
        {
            GFBaseTypeParamValueList rst = new GFBaseTypeParamValueList();
            rst.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue("0")));
            rst.Add(new GFBaseTypeParamValue("errorMsg", new GriffinsBaseValue("0")));
            string jsParam = param["jsParam"].ToJsonStrValue();
            switch (methodID)
            {
                case MotionCalculateSubMachineModulesConst.CalculateMethodID:
                    {
                        MotionCalculateParameters motionCalculateParameters = JsonObjConvert.FromJSon<MotionCalculateParameters>(jsParam);
                        Dictionary<string, MotionCalculationParameters> motionCalculationParameters = BuildMotionCalculationParameters(motionCalculateParameters);
                        MotionTrajectory motionTrajectory = motionCalculatorDriver.Calculate(motionCalculationParameters);
                        rst["Result"] = new GriffinsBaseValue(JsonObjConvert.ToJSon(motionTrajectory));
                    }
                    break;
                default:
                    break;
            }
            return rst;
        }

        Task<GFBaseTypeParamValueList> ISubMMCmdExecutor.AsynExecCabilityMethod(string methodID, GFBaseTypeParamValueList param)
        {
            throw new NotImplementedException();
        }

        GFBaseTypeParamValueList ISubMMCmdExecutor.ExecRuntimeCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
        {
            return new GFBaseTypeParamValueList();
        }

        public ICompUIDataObjPropValRW GetUIDataObjPropValRW()
        {
            throw new NotImplementedException();
        }

        private Dictionary<string, MotionCalculationParameters> BuildMotionCalculationParameters(MotionCalculateParameters motionCalculateParameters)
        {
            if (motionCalculateParameters == null)
            {
                throw new ArgumentNullException(nameof(motionCalculateParameters));
            }

            if (motionCalculateParameters.functionHeadIDs == null)
            {
                throw new ArgumentNullException(nameof(motionCalculateParameters.functionHeadIDs));
            }

            if (motionCalculateParameters.parameters == null)
            {
                throw new ArgumentNullException(nameof(motionCalculateParameters.parameters));
            }

            if (motionCalculateParameters.functionHeadIDs.Length != motionCalculateParameters.parameters.Length)
            {
                throw new ArgumentException("functionHeadIDs 与 parameters 数量不一致");
            }

            Dictionary<string, MotionCalculationParameters> result = new Dictionary<string, MotionCalculationParameters>();
            for (int i = 0; i < motionCalculateParameters.functionHeadIDs.Length; i++)
            {
                result[motionCalculateParameters.functionHeadIDs[i]] = motionCalculateParameters.parameters[i];
            }

            return result;
        }
    }
}