using GF_Gereric;
using Griffins;
using Griffins.ApplicServ.NorthIntf;
using Griffins.ImeIOT;
using Griffins.Map;
using Griffins.UI2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks; 

namespace GriffinsGeneralTestMM
{
    public class GenTestMMCmdExecutor : IMMCmdExecutor, IMMManualModeCmdExecutor, IMMAutoModeCmdExecutor
	{
        private UctlTestMMView uctlTestMM;
        private GenMMInfo genMMInfo;
        private MMAlias alias;
        private IMMCmdExecutorCallBack callBack;
        private PauseObj pauseObj;
        public event OnAfterInit OnAfterInit;

        public GenTestMMCmdExecutor(GenMMInfo genMMInfo, MMAlias alias, UctlTestMMView uctlTestMM)
        {
            this.genMMInfo = genMMInfo;
            this.alias = alias;
            this.uctlTestMM = uctlTestMM;
            this.uctlTestMM.CabilityEvent += UctlTestMM_CabilityEvent;
            this.uctlTestMM.GenNormalEvent += UctlTestMM_GenNormalEvent;
            this.uctlTestMM.AfterPropValueChanged += onAfterPropValueChanged;
            pauseObj = new PauseObj();

        }
       public IMMCmdExecutorCallBack IMMCmdExecutorCallBack
        {
            get { return callBack; }
        }

        private void UctlTestMM_CabilityEvent(object sender, ImeCabilityEventArgs e)
        {
            doCabilityEvent(e);
        }

        private void UctlTestMM_GenNormalEvent(object sender, ImeGenNormalEventArgs e)
        {
            doNormalEvent(e);
        }
      
        /// <summary>
        /// 带路径的属性ID的值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onAfterPropValueChanged(object sender, GFBaseTypeObjPropPathValueChangedEventArgs e)
        {
            compUIDataObjPropValRW.DoUIDataObjPropValChanged(e.PropVals);
        }
        #region IMMCmdExecutor 接口实现

        /// <summary>
        /// 初始化前，让实现对象处理哪些初始化前需要做的事情
        /// </summary>
        void IMMCmdExecutor.BeforeInit()
        {
        }
        /// <summary>
        /// 初始化（在创建机械模组实例后首先调用）
        /// </summary>
        /// <param name="initCfgInfo">初始化参数，null表示缺省值</param>
        /// <param name="calibrationCfgInfo">标定参数，null表示缺省值</param>
        /// <param name="callBack">机械模组运行时回调接口</param>
        void IMMCmdExecutor.Init(byte[] initCfgInfo, byte[] calibrationCfgInfo, IMMCmdExecutorCallBack callBack)
        {
            this.callBack = callBack;
            uctlTestMM.SetInstanceName(callBack.InstanceName);
            uctlTestMM.ShowMessage("执行初始化");
            uctlTestMM.SetMMCmdExecutorCallBack(callBack);
            _compUIDataObjPropValRW?.SetCallBack(callBack);

		}

		/// <summary>
		/// 初始化后
		/// </summary>
		void IMMCmdExecutor.AfterInit()
		{
            //初始化后：各继承类创建界面数据对象相关逻辑
            OnAfterInit?.Invoke();
            //再初始化测试面板的界面数据对象属性值设置界面
            uctlTestMM.ViewModel.AfterInit();
            uctlTestMM.ShowMessage("执行初始化后");
		}

		/// <summary>
		/// 反初始化
		/// </summary>
		void IMMCmdExecutor.UnInit()
		{
			uctlTestMM.ShowMessage("执行反初始化");
		}

		IMMManualModeCmdExecutor IMMCmdExecutor.GetMMManualModeCmdExecutor()
		{
            return this;
		}

		IMMAutoModeCmdExecutor IMMCmdExecutor.GetMMAutoModeCmdExecutor()
		{
            return this;
		}

        #endregion IMMCmdExecutor 接口实现

        #region IMMManualModeCmdExecutor 接口实现

        /// <summary>
        /// 执行配置时控制命令
        /// </summary>
        /// <param name="cmdID">命令ID</param>
        /// <param name="cmdParam">命令参数</param>
        /// <returns>命令执行结果</returns>
        GFBaseTypeParamValueList IMMManualModeCmdExecutor.ExecCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam) 
        {
            string msg = $"执行配置时控制命令：{cmdID}";
            this.uctlTestMM.ShowMessage(msg);
            return null;
        }

		#endregion

		#region IMMAutoModeCmdExecutor 接口实现

		/// <summary>
		/// 准备切换产品配方
		/// </summary>
		void IMMAutoModeCmdExecutor.BeforeSwitchPF()
        {
            uctlTestMM.ShowMessage("准备切换产品配方");
        }
        /// <summary>
        /// 设置配方参数
        /// </summary>
        /// <param name="pfCfgInfo">配方参数，null表示缺省值</param>
        void IMMAutoModeCmdExecutor.SetPFCfgInfo(byte[] pfCfgInfo) 
        {
            //testShowPanel();
            uctlTestMM.ShowMessage("执行设置配方参数");
        }
		//private void testShowPanel()
		//{
		//    callBack.ShowControlPanel("Test");

		//}

		/// <summary>
		/// 是否可以开始工作
		/// </summary>
		/// <param name="reasonMsg">不可以开始工作原因</param>
		/// <returns>True:可以开始工作，False:不可以开始工作</returns>
		bool IMMAutoModeCmdExecutor.CanStartWork(out string reasonMsg)
		{
			reasonMsg = string.Empty;
			return true;
		}

		/// <summary>
		/// 通知机械模组开始工作
		/// </summary>
		void IMMAutoModeCmdExecutor.StartWork()
        {
            uctlTestMM.ShowMessage("执行通知机械模组开始工作");
            uctlTestMM.SetWorkState("工作中");
			pauseObj.Status = 0;
		}
        /// <summary>
        /// 通知机械模组停止工作
        /// </summary>
        void IMMAutoModeCmdExecutor.StopWork() 
        {
            uctlTestMM.ShowMessage("通知机械模组停止工作");
            uctlTestMM.SetWorkState("停止");
			pauseObj.Status = 1;
		}

        /// <summary>
        /// 停止工作后执行
        /// </summary>
        void IMMAutoModeCmdExecutor.AfterStopWork()
        {
            uctlTestMM.ShowMessage("停止工作后执行");
        }

        /// <summary>
        /// 通知机械模组暂停工作
        /// </summary>
        void IMMAutoModeCmdExecutor.Pause() 
        {
            uctlTestMM.ShowMessage("通知机械模组暂停工作");
            uctlTestMM.SetWorkState("暂停");
            pauseObj.Status = 2;
		}
        /// <summary>
        /// 通知机械模组恢复工作
        /// </summary>
        void IMMAutoModeCmdExecutor.Resume() 
        {
            uctlTestMM.ShowMessage("通知机械模组恢复工作");
            uctlTestMM.SetWorkState("工作中");
			pauseObj.Status = 3;
		}
        /// <summary>
        /// 接收到机械模组事件
        /// </summary>
        public event ImeGenNormalEventHandler GenNormalEvent;
        private void doNormalEvent(ImeGenNormalEventArgs e)
        {
            GenNormalEvent?.Invoke(this, e);
            //测试：
            doAlarmEvent(new ImeAlarmEventArgs(1, 1, DateTime.Now));
        }

        /// <summary>
        /// 机械模组能力事件
        /// </summary>
        public event ImeCabilityEventHandler CabilityEvent;
        private void doCabilityEvent(ImeCabilityEventArgs e)
        {
            CabilityEvent?.Invoke(this, e);
        }

        /// <summary>
        /// 接收到机械模组事件
        /// </summary>
        public event ImeAlarmEventHandler AlarmEvent;
        private void doAlarmEvent(ImeAlarmEventArgs e)
        {
            AlarmEvent?.Invoke(this, e);
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="methodID">方法ID</param>
        /// <param name="methodParam">方法参数</param>
        /// <returns>返回结果</returns>
        GFBaseTypeParamValueList IMMAutoModeCmdExecutor.ExecMethod(string methodID, GFBaseTypeParamValueList methodParam)
        {
            if (execStandardCommand(methodID, methodParam, out GFBaseTypeParamValueList result))
                return result;
            uctlTestMM.ShowExecCount();

            GenMMMMethodDefInfo genMMMMethodDefInfo = this.genMMInfo.FindNormalMethodDefInfo(methodID);
            if (genMMMMethodDefInfo == null)
            {
                string errorMsg = $"{methodID}不是本机械模组的普通方法";
                uctlTestMM.ShowErrorMessage(errorMsg);
                throw new Exception(errorMsg);
            }

            string curMethod = $"{genMMMMethodDefInfo.MethodName}({methodID})";
            string msg = $"执行普通方法：{curMethod}";
            if (genMMMMethodDefInfo.IsAsyn)
            {
                Task.Run(() =>
                {
                    execMethod(genMMMMethodDefInfo, curMethod, methodParam, msg);

                }).ConfigureAwait(false);
                return null;
            }
            else
            {
                execMethod(genMMMMethodDefInfo, curMethod, methodParam, msg);
                if (genMMMMethodDefInfo.ParamConvertDele == null)
                {
                    result = null;
                }
                else
                {
                    result = genMMMMethodDefInfo.ParamConvertDele(methodParam);
                }
                return result;
            }
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="methodID">方法ID</param>
        /// <param name="param">方法参数</param>
        /// <returns>返回结果</returns>
        GFParamValueList IMMAutoModeCmdExecutor.ExecMethod(string methodID, GFParamValueList param)
        {
            throw new NotSupportedException();
        }

        private void execMethod(GenMMMMethodDefInfo genMMMMethodDefInfo, string curMethod, GFBaseTypeParamValueList methodParam, string msg)
        {
            uctlTestMM.SetExecMethod(true, curMethod);
            try
            {
                uctlTestMM.ShowMessage(msg);
                showImeMonitorMsg(msg);
                waiting(genMMMMethodDefInfo.CurDelyTime);
                if (genMMMMethodDefInfo.ExecDefInfoes != null)
                {
                    for (int i = 0; i < genMMMMethodDefInfo.ExecDefInfoes.Count; i++)
                    {
                        var genMMMMethodExecDefInfo = genMMMMethodDefInfo.ExecDefInfoes[i];
                        string execMsg;
                        string methodKind;
                        if (genMMMMethodExecDefInfo.IsCability)
                            methodKind = "能力";
                        else
                            methodKind = "普通";
                        execMsg = $"=>调用子机械模组{methodKind}方法{genMMMMethodExecDefInfo.SubMethodID}";
                        try
                        {
                            GFBaseTypeParamValueList result = null;
                            GFBaseTypeParamValueList subParam;
                            if (genMMMMethodExecDefInfo.SubParamConvertDele != null)
                                subParam = genMMMMethodExecDefInfo.SubParamConvertDele(methodParam);
                            else
                                subParam = null;

                            try
                            {
                                if (genMMMMethodExecDefInfo.IsCability)
                                {
                                    callBack.ExecSubMMCabilityMethod(genMMMMethodExecDefInfo.InnerAlias, genMMMMethodExecDefInfo.SubMethodID, subParam);
                                    uctlTestMM.ShowMessage($"{execMsg}返回");
                                }
                                else
                                {
                                    result = callBack.ExecSubMMMethod(genMMMMethodExecDefInfo.InnerAlias, genMMMMethodExecDefInfo.SubMethodID, subParam);
                                    uctlTestMM.ShowMessage($"{execMsg}返回结果：" + result);
                                }
                            }
                            catch (Exception e)
                            {
                                uctlTestMM.ShowErrorMessage($"错误：" + e.Message);
                            }
                        }
                        catch (Exception err)
                        {
                            uctlTestMM.ShowErrorMessage($"{execMsg}错误：" + err.Message);
                        }
                    }
                }
            }
            finally
            {
                uctlTestMM.SetExecMethod(false);
            }
        }

        /// <summary>
        ///  执行标准命令
        /// </summary>
        /// <param name="methodID">方法ID</param>
        /// <param name="methodParam">方法参数</param>
        /// <param name="result">返回结果</param>
        /// <returns>是否为标准命令，true：是，false：否</returns>
        private bool execStandardCommand(string methodID, GFBaseTypeParamValueList methodParam, out GFBaseTypeParamValueList result)
        {
            switch(methodID)
            {
               
            }
            result = null;
            return false;
        }

        /// <summary>
        /// 执行能力方法
        /// </summary>
        /// <param name="methodID">能力方法ID</param>
        /// <param name="methodParam">方法参数</param>
        /// <returns>返回结果</returns>
        GFBaseTypeParamValueList IMMAutoModeCmdExecutor.ExecCabilityMethod(string methodID, GFBaseTypeParamValueList methodParam)
        {
            uctlTestMM.ShowExecCount();
            pauseObj.Wait();
			GenMMMMethodDefInfo genMMMMethodDefInfo = this.genMMInfo.FindCabilityMethodDefInfo(methodID);
            if (genMMMMethodDefInfo == null)
            {
                string errorMsg = $"{methodID}不是本机械模组的能力方法";
                uctlTestMM.ShowErrorMessage(errorMsg);
                throw new Exception(errorMsg);
            }
            string curMethod = $"{genMMMMethodDefInfo.MethodName}({methodID})";
            string msg = $"执行能力方法：{curMethod}";
            execMethod(genMMMMethodDefInfo, curMethod, methodParam, msg);
			pauseObj.Wait();
			if (genMMMMethodDefInfo.ParamConvertDele == null)
                return null;
            else
                return genMMMMethodDefInfo.ParamConvertDele(methodParam);
        }


       

        private  void waiting(int waitingTime)
        {
            if (GenTestMMMain.IsTestMode)
            { 
                return;
            }
            if (waitingTime <= 0)
            { 
                return;
            }
            int delyTime = 0;
            while (delyTime < waitingTime)
            {
                Thread.Sleep(5);
                delyTime += 5; 
            }
        }

        private void showImeMonitorMsg(string msg)
        {
            //if (GenTestMMMain.IsTestMode)
            //    return;
            Task.Run(() =>
            {
                try
                {
                    var imeMonitorMsg = new IGNEventParam_ImeMonitorMsg(DateTime.Now,msg);
                    doNormalEvent(new ImeGenNormalEventArgs(IGNEventParam_ImeMonitorMsg.EventKind, imeMonitorMsg.ToGFBaseTypeParamValues()));
                }
                catch { }
            });
        }

        /// <summary>
        ///  执行接收到所属的子机械模组事件
        /// </summary>
        /// <param name="innerAlias">子机械模组内部别名</param>
        /// <param name="eventID">事件ID</param>
        /// <param name="eventParam">事件参数（和事件ID对于的Json字符串）</param>
        void IMMAutoModeCmdExecutor.ExecSubMMEvent(InnerAlias innerAlias, int eventID, GFBaseTypeParamValueList eventParam)
        {
            GenExecSubEventDefInfo genExecSubEventDefInfo = this.genMMInfo.FindGenExecSubEventDefInfo(innerAlias, eventID);
            if (genExecSubEventDefInfo == null)
                return;
            uctlTestMM.ShowMessage($"执行接收到所属的子机械模组事件：{innerAlias}->{eventID}");
            execMMEvent(eventParam, genExecSubEventDefInfo);
        }

        private void execMMEvent(GFBaseTypeParamValueList eventParam, GenExecSubEventDefInfo genExecSubEventDefInfo)
        {
            GFBaseTypeParamValueList mmEventParam;
            if (genExecSubEventDefInfo.ParamConvertDele != null)
                mmEventParam = genExecSubEventDefInfo.ParamConvertDele(eventParam);
            else
                mmEventParam = null;
            if (genExecSubEventDefInfo.IsImeCabilityEvent)
                doCabilityEvent(new ImeCabilityEventArgs(genExecSubEventDefInfo.MMEventID, mmEventParam));
            else
                doNormalEvent(new ImeGenNormalEventArgs(int.Parse(genExecSubEventDefInfo.MMEventID), mmEventParam));
        }

        /// <summary>
        ///  执行接收到所属的子机械模组能力事件
        /// </summary>
        /// <param name="innerAlias">子机械模组内部别名</param>
        /// <param name="eventID">能力事件ID</param>
        /// <param name="eventParam">能力事件参数（和能力事件ID对于的Json字符串）</param>
        void IMMAutoModeCmdExecutor.ExecSubMMCabilityEvent(InnerAlias innerAlias, string eventID, GFBaseTypeParamValueList eventParam) 
        {
            GenExecSubEventDefInfo genExecSubEventDefInfo = this.genMMInfo.FindGenExecSubEventDefInfo(innerAlias, eventID);
            if (genExecSubEventDefInfo == null)
                return;
            uctlTestMM.ShowMessage($"执行接收到所属的子机械模组能力事件：{innerAlias}->{eventID}");
            execMMEvent(eventParam, genExecSubEventDefInfo);
        }


        /// <summary>
        /// 执行运行时控制命令
        /// </summary>
        /// <param name="cmdID">命令ID</param>
        /// <param name="cmdParam">命令参数</param>
        /// <returns>命令执行结果</returns>
        GFBaseTypeParamValueList IMMAutoModeCmdExecutor.ExecRuntimeCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
        {
            uctlTestMM.ShowMessage($"执行运行时控制命令,命令ID{cmdID}->{JsonObjConvert.ToJSon(cmdParam)}");
            return null;
        }
        /// <summary>
        /// 回原
        /// </summary>
        void IMMAutoModeCmdExecutor.ReturnToOriginal()
        {
            uctlTestMM.ShowMessage("回原");
        }

        private CompUIDataObjPropValRW _compUIDataObjPropValRW = null;
        private CompUIDataObjPropValRW compUIDataObjPropValRW
        {
            get
            {
                if (_compUIDataObjPropValRW == null)
                    _compUIDataObjPropValRW = new CompUIDataObjPropValRW(alias, uctlTestMM, genMMInfo);
                return _compUIDataObjPropValRW;
            }
        }
        /// <summary>
        /// 获取界面数据对象属性读写接口实例，如果不支持返回null
        /// </summary>
        /// <returns>界面数据对象属性读写接口实例</returns>
        ICompUIDataObjPropValRW IMMAutoModeCmdExecutor.GetUIDataObjPropValRW()
        {
            return compUIDataObjPropValRW;
        }

        /// <summary>
        /// 设置设备运行模式
        /// </summary>
        /// <param name="imeRunMode">设备运行模式</param>
        void IMMAutoModeCmdExecutor.SetRunMode(ImeRunMode imeRunMode)
        {
            string runModeStr = string.Empty;
            if (imeRunMode == ImeRunMode.ConfigMode)
            {
                runModeStr = "设置运行模式为配置模式";
            }
            else if (imeRunMode == ImeRunMode.WorkMode)
            {
                runModeStr = "设置运行模式为工作模式";
            }
            else
            {
                runModeStr = "设置运行模式为老化模式";
            }
            uctlTestMM.ShowMessage(runModeStr);
        }

        #endregion IMMAutoModeCmdExecutor 接口实现

     
        ///// <summary>
        ///// 获取子界面数据对象项名称字典
        ///// </summary>
        ///// <param name="objInstPropPath">界面数据对象属性路径</param>
        ///// <returns>子界面数据对象项名称字典</returns>
        //Dictionary<string, string> IMMCmdExecutor.GetSubUIProObjItemNames(ObjInstPropPath objInstPropPath)
        //{
        //    return genMMInfo.IUIDataObjPropExChange.GetSubUIProObjItemNames(objInstPropPath);
        //}
        #region 内部类型

        private class CompUIDataObjPropValRW : ICompUIDataObjPropValRW
        {
            private UctlTestMMView uctlTestMM;
            private GenMMInfo genMMInfo;
            private MMAlias alias;
            private IMMCmdExecutorCallBack callBack;

			public CompUIDataObjPropValRW(MMAlias alias, UctlTestMMView uctlTestMM, GenMMInfo genMMInfo)
            {
                this.alias = alias;
                this.uctlTestMM = uctlTestMM;
                this.genMMInfo = genMMInfo;
            }

            public void SetCallBack(IMMCmdExecutorCallBack callBack) 
            {
                this.callBack = callBack;
            }
            //       /// <summary>
            //       ///  设置界面数据对象属性值
            //       /// </summary>
            //       /// <param name="propertyID">属性值ID</param>
            //       /// <param name="value">属性值</param>
            //       void ICompUIDataObjPropValRW.SetUIDataObjPropValue(MPPropertyID propertyID, GriffinsBaseValue value)
            //       {
            //           uctlTestMM.SetUIDataObjPropValue(propertyID, value);
            //       }
            //       /// <summary>
            //       ///  设置一组界面数据对象属性值
            //       /// </summary>
            //       /// <param name="values">属性值列表</param>
            //       void ICompUIDataObjPropValRW.SetUIDataObjPropValues(GFBaseTypePropValueList values)
            //       {   
            //           foreach (var item in values)
            //           {
            //               if (item.PropertyID == MPPropertyID.Parse("CurRunMode") && callBack != null)
            //               {
            //                   var iSvrObjCallForMMProcessClient = callBack.CreateSvrObjCallForMMProcess(ImeIOTConst.SERVERKINDID, ImeIOTConst.ServerObjectID);
            //                   ImeRunMode imeRunMode = (ImeRunMode)Enum.Parse(typeof(ImeRunMode), item.Value.ToStrValue());
            //                   var param = new SvrForMMProcessCmd.SetRunMode_Param()
            //                   {
            //                       //MTNo = 1,
            //                       ImeRunMode = imeRunMode
            //                   };
            //                   try
            //                   {
            //                       iSvrObjCallForMMProcessClient.Open();
            //                       int result = iSvrObjCallForMMProcessClient.ExecCmd(SvrForMMProcessCmd.CmdID_SetRunMode, JsonObjConvert.ToJSon(param), out string responseStr, out string errMsg);
            //                       if (result != 0)
            //                       {
            //                           throw new Exception(errMsg);
            //                       }
            //                   }
            //                   finally
            //                   {
            //                       iSvrObjCallForMMProcessClient.Close();
            //                   }                        
            //               }
            //else if (item.PropertyID == MPPropertyID.Parse("CurFormulaNumber") && callBack != null)
            //{
            //	var iSvrObjCallForMMProcessClient = callBack.CreateSvrObjCallForMMProcess(ImeIOTConst.SERVERKINDID, ImeIOTConst.ServerObjectID);
            //	FormulaNumber formulaNumber = FormulaNumber.Parse(item.Value.ToStrValue());
            //	var param = new SvrForMMProcessCmd.SetCurFormulaNumber_Param()
            //	{
            //		MTNo = 1,
            //		FormulaNumber = formulaNumber
            //	};
            //	try
            //	{
            //		iSvrObjCallForMMProcessClient.Open();
            //		int result = iSvrObjCallForMMProcessClient.ExecCmd(SvrForMMProcessCmd.CmdID_SetCurFormulaNumber, JsonObjConvert.ToJSon(param), out string responseStr, out string errMsg);
            //	}
            //	finally
            //	{
            //		iSvrObjCallForMMProcessClient.Close();
            //	}
            //}
            //else if (item.PropertyID == MPPropertyID.Parse("WorkCtrl") && callBack != null)
            //{
            //	var iSvrObjCallForMMProcessClient = callBack.CreateSvrObjCallForMMProcess(ImeIOTConst.SERVERKINDID, ImeIOTConst.ServerObjectID);
            //	string workCtrl = item.Value.ToStrValue();						
            //	try
            //	{
            //		iSvrObjCallForMMProcessClient.Open();
            //                       if (workCtrl.Equals("Start"))
            //                       {
            //                           var param = new SvrForMMProcessCmd.StartWork_Param()
            //                           {
            //                               MTNo = 1,
            //                           };
            //			int result = iSvrObjCallForMMProcessClient.ExecCmd(SvrForMMProcessCmd.CmdID_StartWork, JsonObjConvert.ToJSon(param), out string responseStr, out string errMsg);
            //		}
            //                       else 
            //                       {
            //			var param = new SvrForMMProcessCmd.StopWork_Param()
            //			{
            //				MTNo = 1,
            //			};
            //			int result = iSvrObjCallForMMProcessClient.ExecCmd(SvrForMMProcessCmd.CmdID_StopWork, JsonObjConvert.ToJSon(param), out string responseStr, out string errMsg);
            //		}														
            //	}
            //	finally
            //	{
            //		iSvrObjCallForMMProcessClient.Close();
            //	}
            //}
            //uctlTestMM.SetUIDataObjPropValue(item.PropertyID, item.Value);
            //           }
            //       }
            //       /// <summary>
            //       ///  获取界面数据对象属性值
            //       /// </summary>
            //       /// <param name="propertyID">属性值ID</param>
            //       /// <returns>属性值</returns>
            //       GriffinsBaseValue ICompUIDataObjPropValRW.GetUIDataObjPropValue(MPPropertyID propertyID)
            //       {
            //           return uctlTestMM.GetUIDataObjPropValue(propertyID);
            //       }
            //       /// <summary>
            //       ///  获取一组界面数据对象属性值
            //       /// </summary>
            //       /// <param name="propertyIDs">属性值ID列表</param>
            //       /// <returns>属性值列表</returns>
            //       GFBaseTypePropValueList ICompUIDataObjPropValRW.GetUIDataObjPropValues(MPPropertyID[] propertyIDs)
            //       {
            //           var values = new GFBaseTypePropValueList();
            //           foreach (var propertyID in propertyIDs)
            //           {
            //               values.Add(new GFBaseTypePropValue
            //               {
            //                   PropertyID = propertyID,
            //                   Value = uctlTestMM.GetUIDataObjPropValue(propertyID)
            //               });
            //           }
            //           return values;
            //       }
            //       /// <summary>
            //       ///  获取所有界面数据对象属性值
            //       /// </summary>
            //       /// <returns>所有界面数据对象属性值列表</returns>
            //       GFBaseTypePropValueList ICompUIDataObjPropValRW.GetAllUIDataObjPropValues()
            //       {
            //           return uctlTestMM.GetAllUIDataObjPropValues() ?? new GFBaseTypePropValueList();
            //       }
            /// <summary>
            ///  设置界面数据对象属性值
            /// </summary>
            /// <param name="objInstPropPath">属性路径值ID</param>
            /// <param name="value">属性值</param>
            void ICompUIDataObjPropValRW.SetUIDataObjPropPathValue(ObjInstPropPath objInstPropPath, GriffinsBaseValue value)
            {
                GFBaseTypePropValueList allGFBaseTypePropValues = uctlTestMM.GetAllUIDataObjPropValues();
                GFBaseTypePropValue gfBaseTypePropValue = getUIDataObjPropPathValue(allGFBaseTypePropValues, objInstPropPath, value);
                if(gfBaseTypePropValue!=null)
                    uctlTestMM.SetUIDataObjPropValue(gfBaseTypePropValue.PropertyID, gfBaseTypePropValue.Value);
            }
            /// <summary>
            /// 填充并获取指定对象实例属性路径的基础类型属性值
            /// </summary>
            /// <param name="gfBaseTypePropValues">所有基础类型属性值</param>
            /// <param name="objInstPropPath">对象实例属性路径</param>
            /// <param name="value">待填充的属性值</param>
            /// <returns>对象实例属性路径的基础类型属性值</returns>
            private GFBaseTypePropValue getUIDataObjPropPathValue(GFBaseTypePropValueList gfBaseTypePropValues,ObjInstPropPath objInstPropPath, GriffinsBaseValue value)
            {
                GFBaseTypeObjPropPathValueList gFBaseTypeObjPropPathValues = new GFBaseTypeObjPropPathValueList();
                gFBaseTypeObjPropPathValues.Add(new GFBaseTypeObjPropPathValue()
                {
                    ObjInstPropPath = objInstPropPath,
                    Value = value
                });
                gfBaseTypePropValues.Merge(gFBaseTypeObjPropPathValues);
                foreach (GFBaseTypePropValue item in gfBaseTypePropValues)
                {
                    //找到当前带路径的属性所属的界面数据对象属性并设置
                    GFBaseTypeObjPropPathValue gFBaseTypeObjPropPathValue = item.GetLeafGFBaseTypeObjPropPathValues().Find(objInstPropPath);
                    if (gFBaseTypeObjPropPathValue != null)
                        return item;
                }
                return null;
            }
            /// <summary>
            ///  设置一组界面数据对象属性路径值
            /// </summary>
            /// <param name="values">属性路径值列表</param>
            void ICompUIDataObjPropValRW.SetUIDataObjPropPathValues(GFBaseTypeObjPropPathValueList values)
            {
                GFBaseTypePropValueList gfBaseTypePropValues = uctlTestMM.GetAllUIDataObjPropValues();
                gfBaseTypePropValues.Merge(values);
                foreach (var item in gfBaseTypePropValues)
                {
                    uctlTestMM.SetUIDataObjPropValue(item.PropertyID, item.Value);
                }
            }
            /// <summary>
            ///  获取界面数据对象属性路径值
            /// </summary>
            /// <param name="objInstPropPath">属性路径值ID</param>
            /// <returns>属性路径值</returns>
            GriffinsBaseValue ICompUIDataObjPropValRW.GetUIDataObjPropPathValue(ObjInstPropPath objInstPropPath)
            {
                GFBaseTypePropValueList gfBaseTypePropValues = uctlTestMM.GetAllUIDataObjPropValues();
                foreach (var item in gfBaseTypePropValues)
                {
                    GFBaseTypeObjPropPathValue gFBaseTypeObjPropPathValue = item.GetLeafGFBaseTypeObjPropPathValues().Find(objInstPropPath);
                    if (gFBaseTypeObjPropPathValue != null)
                    {
                        return gFBaseTypeObjPropPathValue.Value;
                    }
                }
                return null;
            }
            /// <summary>
            ///  获取一组界面数据对象属性路径值
            /// </summary>
            /// <param name="objInstPropPaths">属性路径值ID列表</param>
            /// <returns>属性路径值列表</returns>
            GFBaseTypeObjPropPathValueList ICompUIDataObjPropValRW.GetUIDataObjPropPathValues(ObjInstPropPath[] objInstPropPaths)
            {
                GFBaseTypeObjPropPathValueList gFBaseTypeObjPropPathValues = new GFBaseTypeObjPropPathValueList();
                if (objInstPropPaths == null || objInstPropPaths.Length == 0)
                    return gFBaseTypeObjPropPathValues;
                GFBaseTypePropValueList gfBaseTypePropValues = uctlTestMM.GetAllUIDataObjPropValues();
                foreach (var item in gfBaseTypePropValues)
                {
                    var lists = item.GetLeafGFBaseTypeObjPropPathValues().FindAll(o => objInstPropPaths.Contains(o.ObjInstPropPath));
                    gFBaseTypeObjPropPathValues.AddRange(lists);
                }
                return gFBaseTypeObjPropPathValues;
            }
            /// <summary>
            ///  获取所有界面数据对象属性路径值
            /// </summary>
            /// <returns>所有界面数据对象属性路径值列表</returns>
            GFBaseTypeObjPropPathValueList ICompUIDataObjPropValRW.GetAllUIDataObjPropPathValues()
            {
                GFBaseTypePropValueList gfBaseTypePropValues = uctlTestMM.GetAllUIDataObjPropValues();
                return gfBaseTypePropValues.GetLeafGFBaseTypeObjPropPathValues();
            }
            /// <summary>
            /// 界面数据对象属性值改变事件
            /// </summary>
            public event ImePropValChangedEventHandler UIDataObjPropValChangedEvent;
           
            /// <summary>
            /// 带路径的属性ID的值改变
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            internal void DoUIDataObjPropValChanged(GFBaseTypeObjPropPathValueList propVals)
            {
                foreach (var propVal in propVals)
                {
                    UIDataObjPropValChangedEvent?.Invoke(this, new ImePropValChangedEventArgs(propVal.ObjInstPropPath, propVal.Value, DateTime.Now));
                }
            }

            GFBaseTypeParamValueList ICompUIDataObjPropValRW.ExecUIDataObjCommand(string cmdID, GFBaseTypeParamValueList cmdParam)
            {
                GFBaseTypeParamValueList gFBaseTypeParamValues = new GFBaseTypeParamValueList();
                uctlTestMM.ShowMessage($"执行界面数据对象命令：{cmdID}:{JsonObjConvert.ToJSon(cmdParam)}");
                return gFBaseTypeParamValues;
            }

        }

        #endregion
    }
    /// <summary>
    /// 初始化后委托
    /// </summary>
    public delegate void OnAfterInit();
}
