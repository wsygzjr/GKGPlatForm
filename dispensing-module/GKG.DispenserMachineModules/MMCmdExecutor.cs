using DispenserMachineModules;
using GF_Gereric;
using GKG.SubMM;
using Griffins.ImeIOT;
using System.Dynamic;

namespace GKG
{
    namespace MM
    {
        public partial class MMCmdExecutor : IMMCmdExecutor
        {
            private readonly DispenserMachineModulesInitCfg initCfg;

            private readonly DispenserMachineModulesPPCfg pPCfg;

            private IMMCmdExecutorCallBack iMMCmdExecutorCallBack;

            private ImeGenNormalEventHandler mMGenNormalEventHandler;

            public event ImeCabilityEventHandler mMCabilityEventHandler;

            public event ImeAlarmEventHandler mMAlarmEventHandler;

            public MMCmdExecutor(MMAlias alias)
            {
                initCfg = new DispenserMachineModulesInitCfg();
                pPCfg = new DispenserMachineModulesPPCfg();
            }

            /// <summary>
            /// 初始化（在创建机械模组实例后首先调用）
            /// </summary>
            /// <param name="initCfgInfo">初始化参数，null表示缺省值</param>
            /// <param name="callBack">机械模组运行时回调接口</param>
            void IMMCmdExecutor.Init(byte[] initCfgInfo, IMMCmdExecutorCallBack callBack)
            {
                initCfg.FromBytes(initCfgInfo);
                this.iMMCmdExecutorCallBack = callBack;
            }

            /// <summary>
            /// 设置配方参数
            /// </summary>
            /// <param name="pfCfgInfo">配方参数，null表示缺省值</param>
            void IMMCmdExecutor.SetPFCfgInfo(byte[] pfCfgInfo)
            {
                pPCfg.FromBytes(pfCfgInfo);
            }

            /// <summary>
            ///切换配方前调用
            /// </summary>
            /// <exception cref="NotImplementedException"></exception>
            void IMMCmdExecutor.BeforeSwitchPF()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// 停止工作后调用
            /// </summary>
            /// <exception cref="NotImplementedException"></exception>
            void IMMCmdExecutor.AfterStopWork()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// 执行控制命令
            /// </summary>
            /// <param name="cmdID"></param>
            /// <param name="cmdParam"></param>
            /// <returns></returns>
            /// <exception cref="NotImplementedException"></exception>
            string IMMCmdExecutor.ExecRuntimeCtlCmd(string cmdID, string cmdParam)
            {
                switch(cmdID)
                {
                    case "CreateModel":
                        {
                            // 创建模板
                            iMMCmdExecutorCallBack.ExecSubMMMethod(
                                DispenserMachineModulesConst.Vision_Alias,
                                VisionSubMachineModulesConst.CreateModelMethodID,
                                cmdParam,
                                JsonObjConvert.FromJSon<SearchMarkParams>(cmdParam),
                                out string jsResult,
                                out object objResult,
                                out string errorMsg);
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
                return "";
            }

            /// <summary>
            /// 回原
            /// </summary>
            /// <exception cref="NotImplementedException"></exception>
            void IMMCmdExecutor.ReturnToOriginal()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// 通知机械模组开始工作
            /// </summary>
            void IMMCmdExecutor.StartWork()
            {
                PauseObj.Status = 2;
            }

            /// <summary>
            /// 通知机械模组停止工作
            /// </summary>
            void IMMCmdExecutor.StopWork()
            {
                PauseObj.Status = 1;
            }

            /// <summary>
            /// 通知机械模组暂停工作
            /// </summary>
            void IMMCmdExecutor.Pause()
            {
                PauseObj.Status = 1;
            }

            /// <summary>
            /// 通知机械模组恢复工作
            /// </summary>
            void IMMCmdExecutor.Resume()
            {
                PauseObj.Status = 2;
            }

            /// <summary>
            /// 接收到机械模组事件
            /// </summary>
            event ImeGenNormalEventHandler IMMCmdExecutor.GenNormalEvent
            {
                add
                {
                    mMGenNormalEventHandler += value;
                }
                remove
                {
                    mMGenNormalEventHandler -= value;
                }
            }

            /// <summary>
            /// 接收到机械模组能力事件
            /// </summary>
            event ImeCabilityEventHandler IMMCmdExecutor.CabilityEvent
            {
                add
                {
                    mMCabilityEventHandler += value;
                }
                remove
                {
                    mMCabilityEventHandler -= value;
                }
            }

            /// <summary>
            /// 接收到机械模组报警事件
            /// </summary>
            event ImeAlarmEventHandler IMMCmdExecutor.AlarmEvent
            {
                add
                {
                    mMAlarmEventHandler += value;
                }
                remove
                {
                    mMAlarmEventHandler -= value;
                }
            }

            /// <summary>
            /// 执行方法
            /// </summary>
            /// <param name="methodID">方法ID</param>
            /// <param name="methodParam">方法参数（和方法ID对应的Json字符串）</param>
            /// <param name="result">返回结果（和方法ID对应的Json字符串）</param>
            /// <param name="errorMsg">错误信息</param>
            /// <returns>错误码（0：成功， 其它：错误）</returns>
            int IMMCmdExecutor.ExecMethod(string methodID, string methodParam, out string result, out string errorMsg)
            {
                result = string.Empty;
                errorMsg = string.Empty;
                return 0;
            }

            /// <summary>
            /// 执行能力方法
            /// </summary>
            /// <param name="methodID">能力方法ID</param>
            /// <param name="methodParam">能力方法参数（和能力方法ID对应的Json字符串）</param>
            /// <param name="errorMsg">错误信息</param>
            /// <returns>错误码（0：成功， 其它：错误）</returns>
            int IMMCmdExecutor.ExecCabilityMethod(string methodID, string jsParam, out string jsResult, out string errorMsg)
            {
                PauseObj.Wait();
                int code = 0;
                jsResult = "0";
                switch (methodID)
                {
                    default:
                        code = 1;
                        errorMsg = "未定义的命令ID";
                        break;
                }
                return code;
            }

            /// <summary>
            ///  执行接收到所属的子机械模组事件
            /// </summary>
            /// <param name="innerAlias">子机械模组内部别名</param>
            /// <param name="eventID">事件ID</param>
            /// <param name="eventParam">事件参数（和事件ID对于的Json字符串）</param>
            void IMMCmdExecutor.ExecSubMMEvent(InnerAlias innerAlias, int eventID, string eventParam)
            {
            }

            /// <summary>
            ///  执行接收到所属的基础软件组件事件
            /// </summary>
            /// <param name="innerAlias">基础软件组件内部别名</param>
            /// <param name="eventID">事件ID</param>
            /// <param name="eventParam">事件参数（和事件ID对于的Json字符串）</param>
            void IMMCmdExecutor.ExecBscEvent(InnerAlias innerAlias, int eventID, string eventParam)
            {
            }

            /// <summary>
            ///  执行接收到所属的子机械模组能力事件
            /// </summary>
            /// <param name="innerAlias">子机械模组内部别名</param>
            /// <param name="eventID">能力事件ID</param>
            /// <param name="eventParam">能力事件参数（和能力事件ID对于的Json字符串）</param>
            void IMMCmdExecutor.ExecSubMMCabilityEvent(InnerAlias innerAlias, string eventID, string eventParam)
            {
                //if (innerAlias == DispenserMachineModulesConst.YLongMen_Alias)//Y龙门
                //{
                //    if (eventID == YMoveLongmenSubMachineModulesConst.StartFinishedEventID)//开始完成事件
                //    {
                //        //发送开始完成能力事件
                //        mMCabilityEventHandler?.Invoke(this, new MMCabilityEventArgs(DispenserMachineModulesConst.StartFinishedEventID, ""));
                //    }
                //    else if (eventID == YMoveLongmenSubMachineModulesConst.EndFinishedEventID)//结束完成事件
                //    {
                //        //发送结束完成能力事件
                //        mMCabilityEventHandler?.Invoke(this, new MMCabilityEventArgs(DispenserMachineModulesConst.EndFinishedEventID, ""));
                //    }
                //    else if (eventID == YMoveLongmenSubMachineModulesConst.StartFinishedEventID) //移动完成事件
                //    {
                //        //发送移动完成能力事件
                //        mMCabilityEventHandler?.Invoke(this, new MMCabilityEventArgs(DispenserMachineModulesConst.MoveFinishedEventID, ""));
                //    }
                //}
            }

            /// <summary>
            ///  执行接收到所属的基础软件组件能力事件
            /// </summary>
            /// <param name="innerAlias">基础软件组件内部别名</param>
            /// <param name="eventID">能力事件ID</param>
            /// <param name="eventParam">能力事件参数（和事件ID对于的Json字符串）</param>
            void IMMCmdExecutor.ExecBscCabilityEvent(InnerAlias innerAlias, string eventID, string eventParam)
            {
            }

            private void sendImeMonitor()
            {
                Task.Run(() =>
                {
                    // mMEventHandler?.Invoke(this, new MMEventArgs(1, "监视信息"));
                }).ConfigureAwait(false);
            }

            private void sendDevStatus()
            {
                dynamic exObj = new ExpandoObject();
                exObj.Status = 12;
                Task.Run(() =>
                {
                    // mMEventHandler?.Invoke(this, new MMEventArgs(2, GF_Gereric.JsonObjConvert.ToJSon(exObj)));
                }).ConfigureAwait(false);
            }

            private void sendCustomEvent()
            {
                dynamic exObj = new ExpandoObject();
                exObj.Name = "Ab";
                exObj.Message = "消息内容";
                Task.Run(() =>
                {
                    //mMEventHandler?.Invoke(this, new MMEventArgs(10001, GF_Gereric.JsonObjConvert.ToJSon(exObj)));
                }).ConfigureAwait(false);
            }
        }
    }
}