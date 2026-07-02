using GF_Gereric;
using GKG.ElectronicControl;
using GKG.MotionControl;
using Griffins;
using Griffins.ImeIOT;
using Griffins.IOT;
using Griffins.PF;
using Griffins.PF.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace GKG
{
    namespace SubMM
    {
        /// <summary>
        /// 电子管理子模组命令执行器，负责初始化运控卡、响应运行时控制命令并处理平台互斥消息。
        /// </summary>
        public class EletronicManageSubMMCmdExecutor : ISubMMCmdExecutor, ISubMMManualModeCmdExecutor, ISubMMAutoModeCmdExecutor
        {
            #region 字段

            private EletronicManagerSubMachineModulesFactoryCfg eletronicManagerSubMachineModulesFactoryCfg;

            private EletronicManagerSubMachineModulesInitCfg eletronicManagerSubMachineModulesInitCfg;

            private EletronicManagerSubMachineModulesPPCfg eletronicManagerSubMachineModulesPPCfg;

            private ISubMMCmdExecutorCallBack iSubMMCmdExecutorCallBack;

            private SubMMAlias alias;

            private ImeGenNormalEventHandler imeGenNormalEventHandler;

            private ImeCabilityEventHandler imeCabilityEventHandler;

            private ImeAlarmEventHandler imeAlarmEventHandler;

            // 运控卡注册表：key=运控卡GUID(value来自FactoryCfg)，value=运控卡接口实例
            private readonly Dictionary<Guid, IMotionControlBase> motionCardDict = new Dictionary<Guid, IMotionControlBase>();

            private readonly Dictionary<Guid, MotionCardType> motionControlCardTypeDict = new Dictionary<Guid, MotionCardType>();

            //// 预先创建的运控卡实例字典（key=MotionCardType），用于工厂配置初始化时赋值和参数设置；FactoryCfg里只保留类型，实例从这里取，避免重复创建和初始化
            //private readonly Dictionary<MotionCardType, IMotionControlBase> motionCardDict_ = new Dictionary<MotionCardType, IMotionControlBase>();

            // 运行时映射表：轴别名 -> 轴信息（按卡GUID分组；可读取AxisInformation所有属性）
            private readonly Dictionary<Guid, Dictionary<string, AxisInformation>> axisAliasToAxisInfoByCardGuid =
                new Dictionary<Guid, Dictionary<string, AxisInformation>>();

            // 运行时映射表：IO别名 -> IO信息（按卡GUID分组；可读取IOStateInformation所有属性）
            private readonly Dictionary<Guid, Dictionary<string, IOStateInformation>> ioAliasToIoInfoByCardGuid =
                new Dictionary<Guid, Dictionary<string, IOStateInformation>>();

            private bool initialized;

            #region 交互消息字段
            private bool axisInfoRegistered = false;
            private bool ioStateInfoRegistered = false;
            private bool robotDriverByAxisIdsRegistered = false;
            private bool stateIOInstancesByIdsRegistered = false;
            #endregion

            #endregion

            #region 构造函数

            /// <summary>
            /// 使用别名、对象ID和出厂配置构造子模组命令执行器。
            /// </summary>
            /// <param name="alias">子模组别名。</param>
            /// <param name="subMMObjID">子模组对象ID。</param>
            /// <param name="factoryCfgInfo">出厂配置字节流。</param>
            public EletronicManageSubMMCmdExecutor(SubMMAlias alias, Guid subMMObjID, byte[] factoryCfgInfo)
            {
                this.alias = alias;
                if (factoryCfgInfo != null)
                {
                    eletronicManagerSubMachineModulesFactoryCfg = JsonObjConvert.FromJSonBytes<EletronicManagerSubMachineModulesFactoryCfg>(factoryCfgInfo);
                }
                else
                {
                    eletronicManagerSubMachineModulesFactoryCfg = new EletronicManagerSubMachineModulesFactoryCfg();
                }
            }

            #endregion

            #region 接口事件

            /// <summary>
            /// 普通事件回调注册入口。
            /// </summary>
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

            /// <summary>
            /// 能力事件回调注册入口。
            /// </summary>
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

            /// <summary>
            /// 报警事件回调注册入口。
            /// </summary>
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

            #endregion

            #region ISubMMCmdExecutor 接口实现

            /// <summary>
            /// 初始化执行器并根据工厂配置创建运控卡实例。
            /// </summary>
            /// <param name="initCfgInfo">初始化配置字节流。</param>
            /// <param name="callBack">框架回调接口。</param>
            void ISubMMCmdExecutor.Init(byte[] initCfgInfo, byte[] calibrationCfgInfo, ISubMMCmdExecutorCallBack callBack)
            {
                //  初始化运控卡插件管理器
                MotionControlPluginManager.Init();
                RobotPluginManager.Init();

                // 兼容调用顺序：框架可能未先调用BeforeInit，导致配置对象未实例化
                // 这里做兜底初始化，避免FromBytes时空引用
                if (eletronicManagerSubMachineModulesFactoryCfg == null)
                    eletronicManagerSubMachineModulesFactoryCfg = new EletronicManagerSubMachineModulesFactoryCfg();
                if (eletronicManagerSubMachineModulesInitCfg == null)
                    eletronicManagerSubMachineModulesInitCfg = new EletronicManagerSubMachineModulesInitCfg();
                if (eletronicManagerSubMachineModulesPPCfg == null)
                    eletronicManagerSubMachineModulesPPCfg = new EletronicManagerSubMachineModulesPPCfg();

                // 强制重置回调，确保自动初始化使用的是最新的回调实例
                if (callBack != null) iSubMMCmdExecutorCallBack = callBack;

                //  即使 initCfgInfo 为空，也要执行 FactoryCfg 的初始化（因为运控卡实例在 FactoryCfg 里）
                if (initCfgInfo != null && initCfgInfo.Length > 0)
                {
                    eletronicManagerSubMachineModulesInitCfg.FromBytes(initCfgInfo);
                }

                EletronicFactoryParameters? factoryParams = eletronicManagerSubMachineModulesFactoryCfg.EletronicFactoryParameters;
                if (factoryParams == null)
                {
                    // 实现思路：初始化入口统一通过资源字符串抛错，后续做多语言或统一文案时只改资源文件。
                    throw new InvalidOperationException(Resources.FactoryParametersNull);
                }
                if (factoryParams.MotionControlCardInformations == null || factoryParams.MotionControlCardInformations.Count == 0)
                {
                    // 实现思路：配置缺失属于固定校验错误，异常文案集中放到资源里统一维护。
                    throw new InvalidOperationException(Resources.MotionControlCardInformationsEmpty);
                }

                // 2) 构建运控卡字典：根据类型创建卡对象，注入参数并初始化（统一cardNo=0）
                foreach (MotionControlCardInformations info in factoryParams.MotionControlCardInformations)
                {
                    if (info.MotionCardID == Guid.Empty)
                    {
                        // 实现思路：把动态参数通过 string.Format 拼进资源模板，避免在业务代码里散落硬编码异常文本。
                        throw new InvalidOperationException(string.Format(Resources.MotionCardIdEmpty, info.MotionCardType));
                    }
                    if (motionCardDict.ContainsKey(info.MotionCardID))
                    {
                        // 实现思路：重复ID属于可复用的校验消息，统一使用资源模板输出。
                        throw new InvalidOperationException(string.Format(Resources.MotionCardIdDuplicate, info.MotionCardID));
                    }

                    IMotionControlBase motionCard = MotionControlPluginManager.GetMotionControl(info.MotionCardType);
                    if (info.MotionControlFactoryParameters != null)
                    {
                        motionCard.SetFactoryParameters(info.MotionControlFactoryParameters);
                    }

                    int ret = motionCard.IniMotionCard(0);
                    if (ret != 0)
                    {
                        // 实现思路：底层返回码需要带出详细上下文，资源模板保留占位符，调用点只负责填值。
                        throw new InvalidOperationException(string.Format(Resources.MotionCardInitFailed, info.MotionCardType, info.MotionCardID, ret));
                    }

                    motionCardDict.Add(info.MotionCardID, motionCard);
                    motionControlCardTypeDict.Add(info.MotionCardID, info.MotionCardType);
                    
                }

                //构建运行时映射表：轴别名/IO别名 -> AxisInformation/IOStateInformation
                BuildRuntimeAliasMaps(factoryParams);

                EnsureMutualInfoRegistered();

                initialized = true;
            }
            /// <summary>
            /// 在正式初始化前重置运行时状态和配置对象。
            /// </summary>
            /// <param name="subMechCompParam">子机构补充参数。</param>

            void ISubMMCmdExecutor.BeforeInit(GFBaseTypePropValueList devicePropValues)
            {
            }

            void ISubMMCmdExecutor.AfterInit()
            {
                IBaseStateIO baseStateIO = CreateStateIoInstance(eletronicManagerSubMachineModulesInitCfg.PowerIOStateGuid);
                baseStateIO.Write(true);
                Thread.Sleep(eletronicManagerSubMachineModulesInitCfg.PowerTime);
                foreach (var card in motionCardDict)
                {
                    for (int i = 0; i < card.Value.SupportAxisNum; i++)
                    {
                        Guid guid = card.Value.LockAxis(i, 3000);
                        card.Value.ClearAxisAlarm(guid);
                        card.Value.AxisEnabled(i, false);
                        card.Value.AxisEnabled(i, true);
                        card.Value.UnLockAxis(guid);
                    }
                }
            }

            /// <summary>
            /// 释放命令执行器持有的交互消息注册。
            /// </summary>

            void ISubMMCmdExecutor.UnInit()
            {
                foreach (var card in motionCardDict)
                {
                    card.Value.CloseMotionCard(0);
                }
                ReleaseMutualInfoRegistration();
            }

            ISubMMManualModeCmdExecutor ISubMMCmdExecutor.GetSubMMManualModeCmdExecutor()
            {
                return this;
            }

            ISubMMAutoModeCmdExecutor ISubMMCmdExecutor.GetSubMMAutoModeCmdExecutor()
            {
                return this;
            }
            void ISubMMAutoModeCmdExecutor.SetRunMode(ImeRunMode imeRunMode)
            {
            }

            /// <summary>
            /// 是否可以开始工作流程的校验入口，框架会在调用StartWork前调用此方法，如果返回false则不会调用StartWork，并将reasonMsg提示给用户。
            /// </summary>
            /// <param name="reasonMsg"></param>
            /// <returns></returns>
            bool ISubMMAutoModeCmdExecutor.CanStartWork(out string reasonMsg)
            {
                reasonMsg = string.Empty;
                return true;
            }

            /// <summary>
            /// 设置流程配置。
            /// </summary>
            /// <param name="pfCfgInfo">流程配置字节流。</param>
            void ISubMMAutoModeCmdExecutor.SetPFCfgInfo(byte[] pfCfgInfo)
            {
            }

            /// <summary>
            /// 启动工作流程。
            /// </summary>
            void ISubMMAutoModeCmdExecutor.StartWork()
            {
                PauseObj.Status = 2;
            }

            /// <summary>
            /// 停止工作流程。
            /// </summary>
            void ISubMMAutoModeCmdExecutor.StopWork()
            {
                PauseObj.Status = 1;
            }

            /// <summary>
            /// 暂停工作流程。
            /// </summary>
            void ISubMMAutoModeCmdExecutor.Pause()
            {
                PauseObj.Status = 1;
            }

            /// <summary>
            /// 恢复工作流程。
            /// </summary>
            void ISubMMAutoModeCmdExecutor.Resume()
            {
                PauseObj.Status = 2;
            }

            /// <summary>
            /// 切换流程前的预处理入口。
            /// </summary>
            void ISubMMAutoModeCmdExecutor.BeforeSwitchPF()
            {
                // 实现思路：未实现接口也走资源字符串，保证提示文案统一可维护。
                throw new NotImplementedException(Resources.BeforeSwitchPFNotImplemented);
            }

            /// <summary>
            /// 停止工作后的收尾入口。
            /// </summary>
            void ISubMMAutoModeCmdExecutor.AfterStopWork()
            {
                // 实现思路：未实现接口也走资源字符串，保证提示文案统一可维护。
                throw new NotImplementedException(Resources.AfterStopWorkNotImplemented);
            }

            /// <summary>
            /// 执行基础类型参数方法调用。
            /// </summary>
            /// <param name="methodID">方法标识。</param>
            /// <param name="param">方法参数。</param>
            /// <returns>基础类型参数返回值。</returns>
            GFBaseTypeParamValueList ISubMMAutoModeCmdExecutor.ExecMethod(string methodID, GFBaseTypeParamValueList param)
            {
                switch (methodID)
                {
                    default:
                        break;
                }
                return new GFBaseTypeParamValueList();
            }

            /// <summary>
            /// 异步执行基础类型参数方法调用。
            /// </summary>
            /// <param name="methodID">方法标识。</param>
            /// <param name="param">方法参数。</param>
            /// <returns>异步基础类型参数返回值。</returns>
            Task<GFBaseTypeParamValueList> ISubMMAutoModeCmdExecutor.AsynExecMethod(string methodID, GFBaseTypeParamValueList param)
            {
                return Task.Run(() =>
                {
                    GFBaseTypeParamValueList result = new GFBaseTypeParamValueList();
                    return result;
                });
            }

            /// <summary>
            /// 执行参数值列表方法调用。
            /// </summary>
            /// <param name="methodID">方法标识。</param>
            /// <param name="param">方法参数。</param>
            /// <returns>参数值列表返回值。</returns>
            GFParamValueList ISubMMAutoModeCmdExecutor.ExecMethod(string methodID, GFParamValueList param)
            {
                return new GFParamValueList();
            }

            /// <summary>
            /// 异步执行参数值列表方法调用。
            /// </summary>
            /// <param name="methodID">方法标识。</param>
            /// <param name="param">方法参数。</param>
            /// <returns>异步参数值列表返回值。</returns>
            Task<GFParamValueList> ISubMMAutoModeCmdExecutor.AsynExecMethod(string methodID, GFParamValueList param)
            {
                return Task.Run(() =>
                {
                    GFParamValueList result = new GFParamValueList();
                    Thread.Sleep(10);
                    return result;
                });
            }

            /// <summary>
            /// 执行能力相关方法调用。
            /// </summary>
            /// <param name="methodID">方法标识。</param>
            /// <param name="param">方法参数。</param>
            /// <returns>能力调用结果。</returns>
            GFBaseTypeParamValueList ISubMMAutoModeCmdExecutor.ExecCabilityMethod(string methodID, GFBaseTypeParamValueList param)
            {
                return new GFBaseTypeParamValueList();
            }

            /// <summary>
            /// 异步执行能力相关方法调用。
            /// </summary>
            /// <param name="methodID">方法标识。</param>
            /// <param name="param">方法参数。</param>
            /// <returns>异步能力调用结果。</returns>
            Task<GFBaseTypeParamValueList> ISubMMAutoModeCmdExecutor.AsynExecCabilityMethod(string methodID, GFBaseTypeParamValueList param)
            {
                return Task.Run(() =>
                {
                    GFBaseTypeParamValueList result = new GFBaseTypeParamValueList();
                    return result;
                });
            }


            /// <summary>
            /// 执行运行时控制命令。
            /// </summary>
            /// <param name="cmdID">命令标识。</param>
            /// <param name="cmdParam">命令参数。</param>
            /// <returns>统一格式的运行时命令执行结果。</returns>
            GFBaseTypeParamValueList ISubMMAutoModeCmdExecutor.ExecRuntimeCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                return ExecRuntimeCtlCmdCore(cmdID, cmdParam);
            }
            GFBaseTypeParamValueList ExecRuntimeCtlCmdCore(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                // 运行时控制命令入口（供前端回调调用）：
                // - cmdID：命令路由键（AbsoluteMove/JogMove/ServoOn/...）
                // - cmdParam：命令参数集合（统一要求包含CardGuid，运动类还需AxisIndex等）
                // 返回：GFBaseTypeParamValueList，固定包含Result/errorMsg/data（统一string，前端解析简单稳定）。
                GFBaseTypeParamValueList result = new GFBaseTypeParamValueList();

                // 先初始化固定返回字段，确保前端永远可读到这些key
                result.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue("0")));
                result.Add(new GFBaseTypeParamValue("errorMsg", new GriffinsBaseValue("")));
                result.Add(new GFBaseTypeParamValue("data", new GriffinsBaseValue("")));

                void SetOk(string data = "")
                {
                    result["Result"] = new GriffinsBaseValue("0");
                    result["errorMsg"] = new GriffinsBaseValue("");
                    result["data"] = new GriffinsBaseValue(data ?? "");
                }

                void SetError(string msg, string resultCode = "-1")
                {
                    result["Result"] = new GriffinsBaseValue(resultCode ?? "-1");
                    result["errorMsg"] = new GriffinsBaseValue(msg ?? "");
                }

                #region  封装参数读取的辅助函数：统一从cmdParam读取，返回string或转换成对应类型；如果缺参数或转换失败，返回false并输出错误信息（SetError），调用处根据返回值决定是否继续执行
                string GetString(string key)
                {
                    GriffinsBaseValue v = cmdParam?[key];
                    return v?.ToString();
                }

                bool TryGetGuid(string key, out Guid guid)
                {
                    guid = Guid.Empty;
                    string s = GetString(key);
                    return !string.IsNullOrWhiteSpace(s) && Guid.TryParse(s, out guid);
                }

                bool TryGetInt(string key, out int value)
                {
                    value = 0;
                    string s = GetString(key);
                    return !string.IsNullOrWhiteSpace(s) && int.TryParse(s, out value);
                }

                bool TryGetDouble(string key, out double value)
                {
                    value = 0;
                    string s = GetString(key);
                    return !string.IsNullOrWhiteSpace(s) && double.TryParse(s, out value);
                }

                bool TryGetDecimal(string key, out decimal value)
                {
                    value = 0;
                    string s = GetString(key);
                    return !string.IsNullOrWhiteSpace(s) && decimal.TryParse(s, out value);
                }

                bool TryGetBool(string key, out bool value)
                {
                    value = false;
                    string s = GetString(key);
                    return !string.IsNullOrWhiteSpace(s) && bool.TryParse(s, out value);
                }

                bool TryGetString(string key, out string value)
                {
                    value = GetString(key);
                    return !string.IsNullOrWhiteSpace(value);
                }

                bool TryGetAxisIndex(out int axisIndex)
                {
                    // 1) 数字轴号优先
                    if (TryGetInt("AxisIndex", out axisIndex) || TryGetInt("Axis", out axisIndex))
                        return true;

                    // 2) 轴别名（AxisAlias/AxisName）反查轴号
                    string axisAlias = GetString("AxisAlias");
                    if (string.IsNullOrWhiteSpace(axisAlias))
                        axisAlias = GetString("AxisName");
                    if (string.IsNullOrWhiteSpace(axisAlias))
                        axisAlias = GetString("Axis");

                    if (string.IsNullOrWhiteSpace(axisAlias))
                    {
                        axisIndex = 0;
                        return false;
                    }

                    if (!TryGetGuid("CardGuid", out Guid cardGuid))
                    {
                        axisIndex = 0;
                        return false;
                    }

                    if (axisAliasToAxisInfoByCardGuid.TryGetValue(cardGuid, out var map) &&
                        map != null &&
                        map.TryGetValue(axisAlias, out var axisInfo) &&
                        axisInfo != null)
                    {
                        axisIndex = axisInfo.AxisNo;
                        return true;
                    }

                    axisIndex = 0;
                    return false;
                }

                bool TryGetAxisLockTimeout(out int timeout)
                {
                    if (TryGetInt("AxisLockTimeout", out timeout) || TryGetInt("TimeOut", out timeout))
                    {
                        if (timeout <= 0)
                            timeout = 3000;
                        return true;
                    }

                    timeout = 3000;
                    return true;
                }

                bool TryGetIoChannel(out string ioChannel)
                {
                    // 1) 数字通道号优先
                    if (TryGetString("IOChannel", out ioChannel) || TryGetString("Channel", out ioChannel))
                        return true;
                    return false;
                }

                bool TryGetCard(out IMotionControlBase motionCard)
                {
                    motionCard = null;
                    if (!TryGetGuid("CardGuid", out Guid cardGuid))
                    {
                        SetError("缺少参数CardGuid或格式无效");
                        return false;
                    }

                    if (!motionCardDict.TryGetValue(cardGuid, out motionCard) || motionCard == null)
                    {
                        SetError($"未找到运控卡实例: {cardGuid}");
                        return false;
                    }

                    return true;
                }
                #endregion

                // 运动类命令统一走“轴锁”机制：axis(int) -> axisGuid(Guid)
                // 目的：避免同一轴被并发控制；执行完必须UnLockAxis（外层用finally保证释放）
                Guid LockAxisOrFail(IMotionControlBase motionCard, out bool locked)
                {
                    locked = false;
                    if (!TryGetAxisIndex(out int axisIndex))
                    {
                        SetError("缺少参数AxisIndex");
                        return Guid.Empty;
                    }

                    TryGetAxisLockTimeout(out int timeout);
                    try
                    {
                        Guid axisGuid = motionCard.LockAxis(axisIndex, timeout);
                        locked = true;
                        return axisGuid;
                    }
                    catch (Exception ex)
                    {
                        SetError($"轴锁定失败: {ex.Message}");
                        return Guid.Empty;
                    }
                }

                try
                {
                    if (string.Equals(cmdID, EletronicManagerSubMachineModulesConst.RtCmdGetElectricalFactoryCfg, StringComparison.OrdinalIgnoreCase))
                    {
                        eletronicManagerSubMachineModulesFactoryCfg ??= new EletronicManagerSubMachineModulesFactoryCfg();
                        string factoryCfgJson = JsonObjConvert.ToJSon(eletronicManagerSubMachineModulesFactoryCfg);
                        SetOk(factoryCfgJson);
                        return result;
                    }

                    if (string.Equals(cmdID, EletronicManagerSubMachineModulesConst.RtCmdSaveElectricalFactoryCfg, StringComparison.OrdinalIgnoreCase))
                    {
                        string factoryCfgJson = GetString("FactoryCfgJson");
                        if (string.IsNullOrWhiteSpace(factoryCfgJson))
                        {
                            SetError("缺少参数FactoryCfgJson");
                            return result;
                        }

                        byte[] factoryCfgBytes = Encoding.UTF8.GetBytes(factoryCfgJson);
                        // 保存时直接用本次提交的数据反序列化成新对象，避免对旧实例执行 PopulateObject
                        // 导致集合字段在“再次打开页面再保存”时发生叠加。
                        eletronicManagerSubMachineModulesFactoryCfg =
                            JsonObjConvert.FromJSonBytes<EletronicManagerSubMachineModulesFactoryCfg>(factoryCfgBytes)
                            ?? new EletronicManagerSubMachineModulesFactoryCfg();

                        if (eletronicManagerSubMachineModulesFactoryCfg.EletronicFactoryParameters != null)
                        {
                            BuildRuntimeAliasMaps(eletronicManagerSubMachineModulesFactoryCfg.EletronicFactoryParameters);
                        }

                        SetOk("Saved");
                        return result;
                    }

                    if (!TryGetCard(out IMotionControlBase motionCard))
                        return result;

                    void ExecuteWithAxisLock(IMotionControlBase card, Action<Guid> action)
                    {
                        bool locked;
                        Guid axisGuid = LockAxisOrFail(card, out locked);
                        if (!locked)
                            return;

                        try
                        {
                            action(axisGuid);
                        }
                        finally
                        {
                            card.UnLockAxis(axisGuid);
                        }
                    }

                    // 命令路由表：新增命令时只需在此增加一项，不需要改大段switch结构
                    Dictionary<string, Action<IMotionControlBase>> handlers = new Dictionary<string, Action<IMotionControlBase>>(StringComparer.OrdinalIgnoreCase)
                    {
                        [EletronicManagerSubMachineModulesConst.RtCmdAbsoluteMove] = card =>
                        {
                            // 绝对运动：先锁轴，再调用AbsoluteMove(axisGuid, ...)
                            ExecuteWithAxisLock(card, axisGuid =>
                            {
                                if (!TryGetInt("MotionType", out int motionType)) { SetError("缺少参数MotionType"); return; }
                                if (!TryGetDouble("Pos", out double pos)) { SetError("缺少参数Pos"); return; }
                                if (!TryGetDouble("StartSpeed", out double startSpeed)) { SetError("缺少参数StartSpeed"); return; }
                                if (!TryGetDouble("MaxSpeed", out double maxSpeed)) { SetError("缺少参数MaxSpeed"); return; }
                                if (!TryGetDouble("AccTimeT", out double accTimeT)) { SetError("缺少参数AccTimeT"); return; }
                                if (!TryGetDouble("DecTimeT", out double decTimeT)) { SetError("缺少参数DecTimeT"); return; }
                                if (!TryGetDouble("AccTimeS", out double accTimeS)) { SetError("缺少参数AccTimeS"); return; }
                                if (!TryGetDouble("DecTimeS", out double decTimeS)) { SetError("缺少参数DecTimeS"); return; }

                                int r = card.AbsoluteMove(axisGuid, motionType, pos, startSpeed, maxSpeed, accTimeT, decTimeT, accTimeS, decTimeS);
                                if (r == 0) SetOk(r.ToString()); else SetError($"AbsoluteMove返回错误码: {r}", r.ToString());
                            });
                        },

                        [EletronicManagerSubMachineModulesConst.RtCmdRelativeMove] = card =>
                        {
                            // 相对运动：先锁轴，再调用RelativeMove(axisGuid, ...)
                            ExecuteWithAxisLock(card, axisGuid =>
                            {
                                if (!TryGetInt("MotionType", out int motionType)) { SetError("缺少参数MotionType"); return; }
                                if (!TryGetDouble("Pos", out double pos)) { SetError("缺少参数Pos"); return; }
                                if (!TryGetDouble("StartSpeed", out double startSpeed)) { SetError("缺少参数StartSpeed"); return; }
                                if (!TryGetDouble("MaxSpeed", out double maxSpeed)) { SetError("缺少参数MaxSpeed"); return; }
                                if (!TryGetDouble("AccTimeT", out double accTimeT)) { SetError("缺少参数AccTimeT"); return; }
                                if (!TryGetDouble("DecTimeT", out double decTimeT)) { SetError("缺少参数DecTimeT"); return; }
                                if (!TryGetDouble("AccTimeS", out double accTimeS)) { SetError("缺少参数AccTimeS"); return; }
                                if (!TryGetDouble("DecTimeS", out double decTimeS)) { SetError("缺少参数DecTimeS"); return; }

                                int r = card.RelativeMove(axisGuid, motionType, pos, startSpeed, maxSpeed, accTimeT, decTimeT, accTimeS, decTimeS);
                                if (r == 0) SetOk(r.ToString()); else SetError($"RelativeMove返回错误码: {r}", r.ToString());
                            });
                        },
                        [EletronicManagerSubMachineModulesConst.RtCmdJogMove] = card =>
                        {
                            // 点动/连续运动：这里用VelocityMove实现；Direction可选（用于控制MaxSpeed方向）
                            ExecuteWithAxisLock(card, axisGuid =>
                            {
                                if (!TryGetInt("MotionType", out int motionType)) { SetError("缺少参数MotionType"); return; }
                                if (!TryGetDouble("StartSpeed", out double startSpeed)) { SetError("缺少参数StartSpeed"); return; }
                                if (!TryGetDouble("MaxSpeed", out double maxSpeed)) { SetError("缺少参数MaxSpeed"); return; }
                                if (!TryGetDouble("AccTimeT", out double accTimeT)) { SetError("缺少参数AccTimeT"); return; }
                                if (!TryGetDouble("DecTimeT", out double decTimeT)) { SetError("缺少参数DecTimeT"); return; }
                                if (!TryGetDouble("AccTimeS", out double accTimeS)) { SetError("缺少参数AccTimeS"); return; }
                                if (!TryGetDouble("DecTimeS", out double decTimeS)) { SetError("缺少参数DecTimeS"); return; }

                                if (TryGetDouble("Direction", out double dir))
                                    maxSpeed = Math.Abs(maxSpeed) * (dir >= 0 ? 1 : -1);

                                int r = card.VelocityMove(axisGuid, motionType, startSpeed, maxSpeed, accTimeT, decTimeT, accTimeS, decTimeS);
                                if (r == 0) SetOk(r.ToString()); else SetError($"JogMove返回错误码: {r}", r.ToString());
                            });
                        },

                        [EletronicManagerSubMachineModulesConst.RtCmdServoOn] = card =>
                        {
                            // 使能开关：接口为AxisEnabled(int axis, bool isEnabled)，不需要轴锁Guid
                            if (!TryGetAxisIndex(out int axisIndex)) { SetError("缺少参数AxisIndex"); return; }
                            if (!TryGetBool("IsEnabled", out bool isEnabled) && !TryGetBool("On", out isEnabled))
                            {
                                SetError("缺少参数IsEnabled/On");
                                return;
                            }

                            int r = card.AxisEnabled(axisIndex, isEnabled);
                            if (r == 0) SetOk(r.ToString()); else SetError($"ServoOn返回错误码: {r}", r.ToString());
                        },

                        [EletronicManagerSubMachineModulesConst.RtCmdHomed] = card =>
                        {
                            // 回零：先锁轴，再调用AxisHome(axisGuid)
                            ExecuteWithAxisLock(card, axisGuid =>
                            {
                                int r = card.AxisHome(axisGuid);
                                if (r == 0) SetOk(r.ToString()); else SetError($"Homed返回错误码: {r}", r.ToString());
                            });
                        },

                        [EletronicManagerSubMachineModulesConst.RtCmdAxisStop] = card =>
                        {
                            // 轴停止：先锁轴，再调用AxisStop(axisGuid, stopType)
                            // StopType支持传枚举名或数值；默认DecelerationStop
                            ExecuteWithAxisLock(card, axisGuid =>
                            {
                                MotionControlAxisStopTypeConstants stopType = MotionControlAxisStopTypeConstants.DecelerationStop;
                                string stopTypeStr = GetString("StopType");
                                if (!string.IsNullOrWhiteSpace(stopTypeStr))
                                {
                                    if (int.TryParse(stopTypeStr, out int stopTypeInt))
                                        stopType = (MotionControlAxisStopTypeConstants)stopTypeInt;
                                    else if (!Enum.TryParse(stopTypeStr, true, out stopType))
                                    {
                                        SetError("参数StopType无效");
                                        return;
                                    }
                                }

                                int r = card.AxisStop(axisGuid, stopType);
                                if (r == 0) SetOk(r.ToString()); else SetError($"AxisStop返回错误码: {r}", r.ToString());
                            });
                        },

                        [EletronicManagerSubMachineModulesConst.RtCmdGetAxisLockState] = card =>
                        {
                            // 获取轴锁定状态
                            if (!TryGetAxisIndex(out int axisIndex)) { SetError("缺少参数AxisIndex"); return; }
                            bool v = card.GetAxisLockState(axisIndex);
                            SetOk(v.ToString());
                        },

                        [EletronicManagerSubMachineModulesConst.RtCmdGetAxisState] = card =>
                        {
                            // 读取单轴状态：是否报警/是否使能等（MotionStatus支持枚举名或数值）
                            if (!TryGetAxisIndex(out int axisIndex)) { SetError("缺少参数AxisIndex"); return; }

                            MotionControlAxisStatus motionStatus = MotionControlAxisStatus.Alarm;
                            string statusStr = GetString("MotionStatus");
                            if (string.IsNullOrWhiteSpace(statusStr)) statusStr = GetString("Status");
                            if (!string.IsNullOrWhiteSpace(statusStr))
                            {
                                if (int.TryParse(statusStr, out int sInt))
                                    motionStatus = (MotionControlAxisStatus)sInt;
                                else if (!Enum.TryParse(statusStr, true, out motionStatus))
                                {
                                    SetError("参数MotionStatus/Status无效");
                                    return;
                                }
                            }

                            bool v = card.GetAxisState(axisIndex, motionStatus);
                            SetOk(v.ToString());
                        },

                        [EletronicManagerSubMachineModulesConst.RtCmdGetAxisStates] = card =>
                        {
                            // 一次性读取五态：正限位/原点/负限位/报警/使能
                            if (!TryGetAxisIndex(out int axisIndex)) { SetError("缺少参数AxisIndex"); return; }

                            bool posLimit = card.GetAxisState(axisIndex, MotionControlAxisStatus.PositiveLimit);
                            bool home = card.GetAxisState(axisIndex, MotionControlAxisStatus.Origin);
                            bool negLimit = card.GetAxisState(axisIndex, MotionControlAxisStatus.NegativeLimit);
                            bool alarm = card.GetAxisState(axisIndex, MotionControlAxisStatus.Alarm);
                            bool enabled = card.GetAxisState(axisIndex, MotionControlAxisStatus.ServoEnable);
                            SetOk(string.Join(",",
                                posLimit ? "1" : "0",
                                home ? "1" : "0",
                                negLimit ? "1" : "0",
                                alarm ? "1" : "0",
                                enabled ? "1" : "0"));
                        },

                        [EletronicManagerSubMachineModulesConst.RtCmdGetAxisPos] = card =>
                        {
                            // 获取轴当前位置（PositionType支持枚举名或数值；默认Command）
                            if (!TryGetAxisIndex(out int axisIndex)) { SetError("缺少参数AxisIndex"); return; }

                            MotionControlAxisPositionType posType = MotionControlAxisPositionType.Command;
                            string posTypeStr = GetString("PositionType");
                            if (string.IsNullOrWhiteSpace(posTypeStr)) posTypeStr = GetString("PosType");
                            if (!string.IsNullOrWhiteSpace(posTypeStr))
                            {
                                if (int.TryParse(posTypeStr, out int pInt))
                                    posType = (MotionControlAxisPositionType)pInt;
                                else if (!Enum.TryParse(posTypeStr, true, out posType))
                                {
                                    SetError("参数PositionType/PosType无效");
                                    return;
                                }
                            }

                            double pos = card.GetAxisPos(axisIndex, posType);
                            SetOk(pos.ToString());
                        },

                        [EletronicManagerSubMachineModulesConst.RtCmdSetAxisPos] = card =>
                        {
                            // 设置轴当前位置
                            if (!TryGetAxisIndex(out int axisIndex)) { SetError("缺少参数AxisIndex"); return; }
                            if (!TryGetDouble("AxisPos", out double axisPos) && !TryGetDouble("Pos", out axisPos))
                            {
                                SetError("缺少参数AxisPos/Pos");
                                return;
                            }

                            double r = card.SetAxisPos(axisIndex, axisPos);
                            SetOk(r.ToString());
                        },

                        [EletronicManagerSubMachineModulesConst.RtCmdWaitAxisStop] = card =>
                        {
                            // 等待轴停止运动：先锁轴，再调用WaitAxisStop(axisGuid, timeOut)
                            ExecuteWithAxisLock(card, axisGuid =>
                            {
                                int timeOut = 3000;
                                TryGetInt("TimeOut", out timeOut);
                                int r = card.WaitAxisStop(axisGuid, timeOut);
                                if (r == 0) SetOk(r.ToString()); else SetError($"WaitAxisStop返回错误码: {r}", r.ToString());
                            });
                        },

                        [EletronicManagerSubMachineModulesConst.RtCmdClearAxisAlarm] = card =>
                        {
                            // 清除轴报警：先锁轴，再调用ClearAxisAlarm(axisGuid)
                            ExecuteWithAxisLock(card, axisGuid =>
                            {
                                int r = card.ClearAxisAlarm(axisGuid);
                                if (r == 0) SetOk(r.ToString()); else SetError($"ClearAxisAlarm返回错误码: {r}", r.ToString());
                            });
                        },

                        [EletronicManagerSubMachineModulesConst.RtCmdLockIO] = card =>
                        {
                            // IO锁定：返回锁句柄Guid
                            //if (!TryGetIoChannel(out string ioChannel)) { SetError("缺少参数IOChannel/ChannelID"); return; }
                            //Guid guid = card.LockIO(ioChannel);
                            //SetOk(guid.ToString());
                        },

                        [EletronicManagerSubMachineModulesConst.RtCmdUnLockIO] = card =>
                        {
                            // IO解锁：参数支持IOGuid/Guid
                            if (!TryGetGuid("IOGuid", out Guid ioGuid) && !TryGetGuid("Guid", out ioGuid))
                            {
                                SetError("缺少参数IOGuid/Guid");
                                return;
                            }
                            int r = card.UnLockIO(ioGuid);
                            if (r == 0) SetOk(r.ToString()); else SetError($"UnLockIO返回错误码: {r}", r.ToString());
                        },

                        [EletronicManagerSubMachineModulesConst.RtCmdGetIOState] = card =>
                        {
                            // 获取IO锁定状态（接口命名为GetIOState）
                            if (!TryGetIoChannel(out string ioChannel)) { SetError("缺少参数IOChannel/ChannelID"); return; }
                            if (!TryGetGuid("CardGuid", out Guid cardGuid))
                            {
                                SetError("缺少参数CardGuid或格式无效");
                                return;
                            }
                            IBaseStateIO baseStateIO = CreateStateIoInstance(cardGuid, ioChannel, "0");
                            bool v = baseStateIO.Read();
                            SetOk(v.ToString());
                        },

                        [EletronicManagerSubMachineModulesConst.RtCmdGetInputState] = card =>
                        {
                            // 读输入状态量
                            if (!TryGetIoChannel(out string ioChannel)) { SetError("缺少参数IOChannel/ChannelID"); return; }
                            if (!TryGetGuid("CardGuid", out Guid cardGuid))
                            {
                                SetError("缺少参数CardGuid或格式无效");
                                return;
                            }
                            IBaseStateIO baseStateIO = CreateStateIoInstance(cardGuid, ioChannel, "0");
                            bool v = baseStateIO.Read();
                            SetOk(v.ToString());
                        },

                        [EletronicManagerSubMachineModulesConst.RtCmdGetOutputState] = card =>
                        {
                            // 读输出状态量
                            if (!TryGetIoChannel(out string ioChannel)) { SetError("缺少参数IOChannel/ChannelID"); return; }
                            if (!TryGetGuid("CardGuid", out Guid cardGuid))
                            {
                                SetError("缺少参数CardGuid或格式无效");
                                return;
                            }
                            IBaseStateIO baseStateIO = CreateStateIoInstance(cardGuid, ioChannel, "0");
                            bool v = baseStateIO.Read();
                            SetOk(v.ToString());
                        },

                        [EletronicManagerSubMachineModulesConst.RtCmdGetInOutputState] = card =>
                        {   // 读输入输出状态量
                            if (!TryGetIoChannel(out string ioChannel)) { SetError("缺少参数IOChannel/ChannelID"); return; }

                            if (!TryGetGuid("CardGuid", out Guid cardGuid))
                            {
                                SetError("缺少参数CardGuid或格式无效");
                                return;
                            }
                            // 读输入输出状态量：参数支持IOChannel/ChannelID + CardGuid（用于定位具体卡实例，因为不同卡的IO通道可能重叠）
                            // 实现思路：去掉预创建字典，改成按卡GUID+通道号实时创建状态IO实例，避免状态缓存和装配成本。
                            IBaseStateIO baseStateIO = CreateStateIoInstance(cardGuid, ioChannel,"0");
                            bool i = baseStateIO.Read();
                            SetOk(i.ToString());
                        },

                        [EletronicManagerSubMachineModulesConst.RtCmdSetOutputState] = card =>
                        {
                            // 写输出状态量：参数支持IOChannel/ChannelID + Value/IsHave
                            if (!TryGetIoChannel(out string ioChannel)) { SetError("缺少参数IOChannel/ChannelID"); return; }
                            //需要先锁IO，再验证参数，最后写状态并解锁
                            //Guid guid = card.LockIO(ioChannel);
                            if (!TryGetGuid("CardGuid", out Guid cardGuid))
                            {
                                SetError("缺少参数CardGuid或格式无效");
                                return;
                            }
                            // 实现思路：输出状态量写入也按需创建实例，避免依赖全局状态IO缓存字典。
                            IBaseStateIO baseStateIO = CreateStateIoInstance(cardGuid, ioChannel, "0");

                            if (!TryGetBool("Value", out bool isHave) && !TryGetBool("IsHave", out isHave))
                            {
                                SetError("缺少参数Value/IsHave");
                                return;
                            }

                            //int i = card.UnLockIO(guid);
                            baseStateIO.Write(isHave);
                            baseStateIO.Read();
                        },

                        [EletronicManagerSubMachineModulesConst.RtCmdGetInputNum] = card =>
                        {
                            //// 读输入模拟量
                            //if (!TryGetIoChannel(out string ioChannel)) { SetError("缺少参数IOChannel/ChannelID"); return; }
                            //double v = card.GetInputNum(ioChannel);
                            //SetOk(v.ToString());
                        },

                        [EletronicManagerSubMachineModulesConst.RtCmdGetOutputNum] = card =>
                        {
                            // 读输出模拟量
                            //if (!TryGetIoChannel(out int ioChannel)) { SetError("缺少参数IOChannel/ChannelID"); return; }
                            //double v = card.GetOutputNum(ioChannel);
                            //SetOk(v.ToString());
                        },

                        [EletronicManagerSubMachineModulesConst.RtCmdGetInOutputNum] = card =>
                        {
                            // 读输入输出模拟量：参数支持IOGuid/Guid
                            //if (!TryGetGuid("IOGuid", out Guid guid) && !TryGetGuid("Guid", out guid))
                            //{
                            //    SetError("缺少参数IOGuid/Guid");
                            //    return;
                            //}

                            //double v = card.GetInOutputNum(guid);
                            //SetOk(v.ToString());
                        },

                        [EletronicManagerSubMachineModulesConst.RtCmdSetInOutputNum] = card =>
                        {
                            // 写输入输出模拟量：参数支持IOGuid/Guid + AnalogValue/Value
                            if (!TryGetGuid("IOGuid", out Guid guid) && !TryGetGuid("Guid", out guid))
                            {
                                SetError("缺少参数IOGuid/Guid");
                                return;
                            }
                            if (!TryGetDouble("AnalogValue", out double analogValue) && !TryGetDouble("Value", out analogValue))
                            {
                                SetError("缺少参数AnalogValue/Value");
                                return;
                            }

                            double v = card.SetInOutputNum(guid, analogValue);
                            SetOk(v.ToString());
                        },
                        [EletronicManagerSubMachineModulesConst.RtCmdGetAxisConfigList] = card =>
                        {
                            //获取轴列表
                            var axisInfos = axisAliasToAxisInfoByCardGuid.Select(axis => (axis.Value.Select(x => x.Value)));
                            var jsonStr = JsonObjConvert.ToJSon(axisInfos);
                            SetOk(jsonStr);
                        },
                        [EletronicManagerSubMachineModulesConst.RtCmdGetIoConfigList] = card =>
                        {
                            //获取io列表
                            var ioInfos = ioAliasToIoInfoByCardGuid.Select(io => (io.Value.Select(x => x.Value)));
                            var jsonStr = JsonObjConvert.ToJSon(ioInfos);
                            SetOk(jsonStr);
                        },
                    };

                    if (!handlers.TryGetValue(cmdID ?? string.Empty, out Action<IMotionControlBase> handler))
                    {
                        SetError($"未知命令: {cmdID}");
                        return result;
                    }

                    handler(motionCard);
                    return result;
                }
                catch (Exception ex)
                {
                    SetError(ex.Message);
                    return result;
                }
            }
            /// <summary>
            /// 获取界面数据对象属性读写接口实例，如果不支持则返回 null。
            /// </summary>
            /// <returns>界面数据对象属性读写接口实例。</returns>
            ICompUIDataObjPropValRW ISubMMAutoModeCmdExecutor.GetUIDataObjPropValRW()
            {
                return null;
            }


            #endregion

            #region ISubMMManualModeCmdExecutor接口实现
            GFBaseTypeParamValueList ISubMMManualModeCmdExecutor.ExecCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                return ExecRuntimeCtlCmdCore(cmdID, cmdParam);
            }
            #endregion
            #region 私有方法

            /// <summary>
            /// 根据运控卡GUID和通道号实时创建状态IO实例。
            /// </summary>
            /// <param name="cardGuid">运控卡GUID。</param>
            /// <param name="channelId">状态通道号。</param>
            /// <param name="deviceId">设备内编号。</param>
            /// <returns>实时创建的状态IO实例。</returns>
            private IBaseStateIO CreateStateIoInstance(Guid ioGuid)
            {
                IOStateInformation ioInfo = eletronicManagerSubMachineModulesFactoryCfg.IOStateInformations.FirstOrDefault(x => x != null && x.IOGuid == ioGuid);
                Guid cardGuid = ioInfo.DeviceGuid;
                string channelId = ioInfo.ChannelId;
                if (!motionCardDict.TryGetValue(cardGuid, out IMotionControlBase motionCard) || motionCard == null)
                {
                    // 实现思路：实时创建状态IO实例前先校验运控卡实例是否存在，避免在运行期读写状态量时出现隐蔽空引用。
                    throw new InvalidOperationException(string.Format(Resources.MotionCardInstanceNotFoundByCardGuid, cardGuid));
                }

                if (string.IsNullOrWhiteSpace(channelId))
                {
                    // 实现思路：通道号仍然走统一资源化校验，保证按需创建和原有异常出口一致。
                    throw new InvalidOperationException(string.Format(Resources.IoStateChannelIdNotConfigured, cardGuid));
                }

                IBaseStateIO stateIo = new StateControlMotionCard();
                stateIo.Init(JsonObjConvert.ToJSonBytes(new IOStateInitParameters
                {
                    deviceID = cardGuid.ToString(),
                    channelID = channelId,
                    deviceGuid = cardGuid,
                }));
                stateIo.SetDeviceInstance(motionCard);
                return stateIo;
            }

            private IBaseStateIO CreateStateIoInstance(Guid cardGuid, string channelId, string deviceId = "0")
            {
                if (!motionCardDict.TryGetValue(cardGuid, out IMotionControlBase motionCard) || motionCard == null)
                {
                    // 实现思路：实时创建状态IO实例前先校验运控卡实例是否存在，避免在运行期读写状态量时出现隐蔽空引用。
                    throw new InvalidOperationException(string.Format(Resources.MotionCardInstanceNotFoundByCardGuid, cardGuid));
                }

                if (string.IsNullOrWhiteSpace(channelId))
                {
                    // 实现思路：通道号仍然走统一资源化校验，保证按需创建和原有异常出口一致。
                    throw new InvalidOperationException(string.Format(Resources.IoStateChannelIdNotConfigured, cardGuid));
                }

                IBaseStateIO stateIo = new StateControlMotionCard();
                stateIo.Init(JsonObjConvert.ToJSonBytes(new IOStateInitParameters
                {
                    deviceID = deviceId,
                    channelID = channelId,
                    deviceGuid = cardGuid,
                }));
                stateIo.SetDeviceInstance(motionCard);
                return stateIo;
            }

            /// <summary>
            /// 构建轴别名和IO别名到配置对象的运行时映射表。
            /// </summary>
            /// <param name="factoryParams">工厂配置参数对象。</param>
            private void BuildRuntimeAliasMaps(EletronicFactoryParameters factoryParams)
            {
                axisAliasToAxisInfoByCardGuid.Clear();
                ioAliasToIoInfoByCardGuid.Clear();

                if (factoryParams?.MotionControlCardInformations == null)
                    return;

                foreach (MotionControlCardInformations card in factoryParams.MotionControlCardInformations)
                {
                    if (card == null || card.MotionCardID == Guid.Empty)
                        continue;

                    Guid cardGuid = card.MotionCardID;

                    Dictionary<string, AxisInformation> axisMap;
                    if (!axisAliasToAxisInfoByCardGuid.TryGetValue(cardGuid, out axisMap))
                    {
                        axisMap = new Dictionary<string, AxisInformation>(StringComparer.OrdinalIgnoreCase);
                        axisAliasToAxisInfoByCardGuid[cardGuid] = axisMap;
                    }

                    Dictionary<string, IOStateInformation> ioMap;
                    if (!ioAliasToIoInfoByCardGuid.TryGetValue(cardGuid, out ioMap))
                    {
                        ioMap = new Dictionary<string, IOStateInformation>(StringComparer.OrdinalIgnoreCase);
                        ioAliasToIoInfoByCardGuid[cardGuid] = ioMap;
                    }

                    AxisInformation[] axisInformations = eletronicManagerSubMachineModulesFactoryCfg.AxisInformations;

                    // 轴别名：AxisInformation.AxisName -> AxisInformation
                    if (axisInformations != null)
                    {
                        foreach (var axis in axisInformations)
                        {
                            if (axis == null) continue;
                            if (string.IsNullOrWhiteSpace(axis.AxisName)) continue;
                            axisMap[axis.AxisName] = axis; // 同一别名覆盖（保留全部属性）
                        }
                    }

                    var ioStateInformations = eletronicManagerSubMachineModulesFactoryCfg.IOStateInformations;
                    // IO别名：IOStateInformation.IOName -> IOStateInformation（保留全部属性；通道号按需从ChannelId解析）
                    if (ioStateInformations != null)
                    {
                        foreach (var io in ioStateInformations)
                        {
                            if (io == null) continue;
                            if (string.IsNullOrWhiteSpace(io.IOName)) continue;
                            ioMap[io.IOName] = io; // 同一别名覆盖（保留全部属性）
                        }
                    }
                }
            }

            #endregion

            #region 消息处理

            /// <summary>
            /// 确保交互消息注册
            /// </summary>
            private void EnsureMutualInfoRegistered()
            {
                if (!axisInfoRegistered)
                {
                    ServerInfoProcessRegister.RegisterMutualInfoProcessDelegate(AxisInfosRequest.InfoKindID, OnMutualInfoReceived);
                    axisInfoRegistered = true;
                }

                if (!ioStateInfoRegistered)
                {
                    ServerInfoProcessRegister.RegisterMutualInfoProcessDelegate(IOStateInfosRequest.InfoKindID, OnIOStateMutualInfoReceived);
                    ioStateInfoRegistered = true;
                }

                if (!robotDriverByAxisIdsRegistered)
                {
                    ServerInfoProcessRegister.RegisterMutualInfoProcessDelegate(RobotDriverByAxisIdsRequest.InfoKindID, OnRobotDriverByAxisIdsMutualInfoReceived);
                    robotDriverByAxisIdsRegistered = true;
                }

                if (!stateIOInstancesByIdsRegistered)
                {
                    ServerInfoProcessRegister.RegisterMutualInfoProcessDelegate(StateIOInstancesByIdsRequest.InfoKindID, OnStateIOInstancesByIdsMutualInfoReceived);
                    stateIOInstancesByIdsRegistered = true;
                }
            }

            /// <summary>
            /// 释放已经注册到平台的互斥消息处理委托。
            /// </summary>
            private void ReleaseMutualInfoRegistration()
            {
                if (axisInfoRegistered)
                {
                    ServerInfoProcessRegister.UnRegisterMutualInfoProcessDelegate(AxisInfosRequest.InfoKindID, OnMutualInfoReceived);
                    axisInfoRegistered = false;
                }

                if (ioStateInfoRegistered)
                {
                    ServerInfoProcessRegister.UnRegisterMutualInfoProcessDelegate(IOStateInfosRequest.InfoKindID, OnIOStateMutualInfoReceived);
                    ioStateInfoRegistered = false;
                }

                if (robotDriverByAxisIdsRegistered)
                {
                    ServerInfoProcessRegister.UnRegisterMutualInfoProcessDelegate(RobotDriverByAxisIdsRequest.InfoKindID, OnRobotDriverByAxisIdsMutualInfoReceived);
                    robotDriverByAxisIdsRegistered = false;
                }

                if (stateIOInstancesByIdsRegistered)
                {
                    ServerInfoProcessRegister.UnRegisterMutualInfoProcessDelegate(StateIOInstancesByIdsRequest.InfoKindID, OnStateIOInstancesByIdsMutualInfoReceived);
                    stateIOInstancesByIdsRegistered = false;
                }
            }

            /// <summary>
            /// 收到平台互斥消息后 筛选运控类型，返回轴列表消息响应；如果不是运控消息或参数无效则返回null
            /// </summary>
            private AxisInfosResponse OnMutualInfoReceived(GriffinsInfoKindID infoKind, MutualInfoBase info, System.Threading.CancellationToken token)
            {
                if (token.IsCancellationRequested || info is not AxisInfosRequest mutualInfo)
                {
                    return null;
                }

                AxisInfosResponse axisInfosResponse = new AxisInfosResponse();
                if (eletronicManagerSubMachineModulesFactoryCfg?.AxisInformations == null)
                {
                    return axisInfosResponse;
                }

                foreach (var axisinfo in eletronicManagerSubMachineModulesFactoryCfg.AxisInformations)
                {
                    if (axisinfo != null)
                    {
                        motionControlCardTypeDict.TryGetValue(axisinfo.MotionCardGuid, out MotionCardType cardType);
                        if (mutualInfo.CardType == MotionControlCardType.Normal && MotionControlPluginManager.GetDefaultConfig(cardType).MotionControlCardType == mutualInfo.CardType)
                        {
                            axisInfosResponse.AxisInformations.Add(axisinfo);
                        }
                        else
                        {
                            axisInfosResponse.AxisInformations.Add(axisinfo);
                        }
                    }
                }

                return axisInfosResponse;
            }

            /// <summary>
            /// 收到平台互斥消息后，返回全部IOState信息列表；如果不是IOState查询消息则返回null
            /// </summary>
            private IOStateInfosResponse OnIOStateMutualInfoReceived(GriffinsInfoKindID infoKind, MutualInfoBase info, System.Threading.CancellationToken token)
            {
                if (token.IsCancellationRequested || info is not IOStateInfosRequest)
                {
                    return null;
                }

                IOStateInfosResponse ioStateInfosResponse = new IOStateInfosResponse();
                if (eletronicManagerSubMachineModulesFactoryCfg?.IOStateInformations == null)
                {
                    return ioStateInfosResponse;
                }

                foreach (var ioStateInfo in eletronicManagerSubMachineModulesFactoryCfg.IOStateInformations)
                {
                    if (ioStateInfo != null)
                    {
                        ioStateInfosResponse.IOStateInformations.Add(ioStateInfo);
                    }
                }

                return ioStateInfosResponse;
            }

            /// <summary>
            /// 收到平台互斥消息后，根据轴ID列表创建并返回对应的RobotDriver。
            /// 约束：当前请求中的所有轴必须绑定到同一张运控卡。
            /// </summary>
            private RobotDriverByAxisIdsResponse OnRobotDriverByAxisIdsMutualInfoReceived(GriffinsInfoKindID infoKind, MutualInfoBase info, System.Threading.CancellationToken token)
            {
                if (token.IsCancellationRequested || info is not RobotDriverByAxisIdsRequest mutualInfo)
                {
                    return null;
                }

                RobotDriverByAxisIdsResponse response = new RobotDriverByAxisIdsResponse();
                List<Guid> axisIds = mutualInfo.AxisIds ?? new List<Guid>();
                if (axisIds.Count == 0)
                {
                    return response;
                }

                if (eletronicManagerSubMachineModulesFactoryCfg?.AxisInformations == null)
                {
                    // 实现思路：消息处理入口的配置校验统一走资源文案，方便后续前后端统一提示。
                    throw new InvalidOperationException(Resources.AxisConfigNotLoaded);
                }

                AxisInformation[] allAxisInfos = eletronicManagerSubMachineModulesFactoryCfg.AxisInformations;
                List<AxisInformation> selectedAxisInfos = new List<AxisInformation>();
                foreach (Guid axisId in axisIds)
                {
                    AxisInformation axisInfo = allAxisInfos.FirstOrDefault(x => x != null && x.AxisGuid == axisId);
                    if (axisInfo == null)
                    {
                        // 实现思路：按轴ID逐个校验时，把具体轴ID填入资源模板，便于定位配置问题。
                        throw new InvalidOperationException(string.Format(Resources.AxisInfoNotFoundById, axisId));
                    }
                    selectedAxisInfos.Add(axisInfo);
                }

                Guid cardGuid = selectedAxisInfos[0].MotionCardGuid;
                if (selectedAxisInfos.Any(x => x.MotionCardGuid != cardGuid))
                {
                    // 实现思路：当前实现只支持单卡构建RobotDriver，跨卡请求直接用资源异常阻断。
                    throw new InvalidOperationException(Resources.AxisIdsAcrossMultipleMotionCards);
                }

                if (!motionCardDict.TryGetValue(cardGuid, out IMotionControlBase motionCard) || motionCard == null)
                {
                    // 实现思路：运控卡实例缺失时把卡GUID带出，方便直接回查工厂配置和初始化流程。
                    throw new InvalidOperationException(string.Format(Resources.MotionCardInstanceNotFoundByCardGuid, cardGuid));
                }

                RobotInitParameters initParameters = new RobotInitParameters();
                initParameters.AxisBindings = new AxisBinding[selectedAxisInfos.Count];
                for (int i = 0; i < selectedAxisInfos.Count; i++)
                {
                    initParameters.AxisBindings[i] = new AxisBinding(selectedAxisInfos[i].MotionCardGuid, i, selectedAxisInfos[i].AxisNo);
                }
                initParameters.MotionControlCardType = mutualInfo.MotionCardType;

                // 实现思路：RobotPluginManager 负责按卡类型选驱动并在创建后立即完成 Init，这里只组织初始化参数。
                IRobotDriver robotDriver = RobotPluginManager.GetRobotDriver(mutualInfo.MotionCardType);
                robotDriver.Init(initParameters, motionCard);
                response.RobotDriver = robotDriver;
                return response;
            }

            /// <summary>
            /// 收到平台互斥消息后，根据IO状态量GUID列表返回对应的状态量实例列表。
            /// </summary>
            private StateIOInstancesByIdsResponse OnStateIOInstancesByIdsMutualInfoReceived(GriffinsInfoKindID infoKind, MutualInfoBase info, System.Threading.CancellationToken token)
            {
                if (token.IsCancellationRequested || info is not StateIOInstancesByIdsRequest mutualInfo)
                {
                    return null;
                }

                StateIOInstancesByIdsResponse response = new StateIOInstancesByIdsResponse();
                List<Guid> ioGuids = mutualInfo.IOGuids ?? new List<Guid>();
                if (ioGuids.Count == 0)
                {
                    return response;
                }

                if (eletronicManagerSubMachineModulesFactoryCfg?.IOStateInformations == null)
                {
                    // 实现思路：IO配置前置校验失败直接抛资源异常，避免后面出现更隐晦的空引用问题。
                    throw new InvalidOperationException(Resources.IoStateConfigNotLoaded);
                }

                IOStateInformation[] allIoInfos = eletronicManagerSubMachineModulesFactoryCfg.IOStateInformations;
                foreach (Guid ioGuid in ioGuids)
                {
                    // 实现思路：消息查询时不再依赖运行时缓存表，直接按请求的卡GUID和通道号实时创建状态IO实例。
                    IBaseStateIO stateIo = CreateStateIoInstance(ioGuid);
                    response.StateIOInstances.Add(stateIo);
                }

                return response;
            }

            #endregion
        }
    }
}