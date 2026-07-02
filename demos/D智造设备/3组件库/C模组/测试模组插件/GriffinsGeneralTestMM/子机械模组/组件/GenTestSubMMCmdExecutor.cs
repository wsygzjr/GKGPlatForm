using Avalonia.PropertyGrid.Controls;
using Avalonia.Threading;
using GF_Gereric;
using Griffins;
using Griffins.ImeIOT;
using Griffins.UI2;
using Newtonsoft.JsonG.Linq;
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
    public class GenTestSubMMCmdExecutor : ISubMMCmdExecutor, ISubMMManualModeCmdExecutor, ISubMMAutoModeCmdExecutor
	{
        private UctlTestSubMMView uctlTestSubMM;
        private GenSubMMInfo genSubMMInfo;
        private SubMMAlias alias;
        private ISubMMCmdExecutorCallBack callBack;
		private PauseObj pauseObj;
        public event OnAfterInit OnAfterInit;
        public GenTestSubMMCmdExecutor(GenSubMMInfo genSubMMInfo, SubMMAlias alias, UctlTestSubMMView uctlTestSubMM)
        {
            this.genSubMMInfo = genSubMMInfo;
            this.alias = alias;
            this.uctlTestSubMM = uctlTestSubMM;
            this.uctlTestSubMM.CabilityEvent += UctlTestMM_CabilityEvent;
            this.uctlTestSubMM.GenNormalEvent += UctlTestMM_GenNormalEvent;
            this.uctlTestSubMM.AfterPropValueChanged += onAfterPropValueChanged;
            pauseObj = new PauseObj();
		}

      
        private void UctlTestMM_CabilityEvent(object sender, ImeCabilityEventArgs e)
        {
            doCabilityEvent(e);
        }

        private void UctlTestMM_GenNormalEvent(object sender, ImeGenNormalEventArgs e)
        {
            doGenNormalEvent(e);
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

        public UctlTestSubMMView UctlTestSubMM
        {
            get { return this.uctlTestSubMM; }
        }

		#region  ISubMMCmdExecutor 接口实现

		/// <summary>
		/// 初始化前，让实现对象处理哪些初始化前需要做的事情
		/// </summary>
		/// <param name="devicePropValues">设备属性值列表</param>
		void ISubMMCmdExecutor.BeforeInit(GFBaseTypePropValueList devicePropValues)
        {
			uctlTestSubMM.ShowMessage($"初始化之前设置设备属性值列表：{JsonObjConvert.ToJSon(devicePropValues)}");
		}
		/// <summary>
		/// 初始化（在创建子机械模组实例后首先调用）
		/// </summary>
		/// <param name="initCfgInfo">初始化参数，null表示缺省值</param>
		/// <param name="calibrationCfgInfo">标定参数，null表示缺省值</param>
		/// <param name="callBack">子机械模组（复合子机械模组）运行时回调接口</param>
		void ISubMMCmdExecutor.Init(byte[] initCfgInfo, byte[] calibrationCfgInfo, ISubMMCmdExecutorCallBack callBack) 
        {
            this.callBack = callBack;
            uctlTestSubMM.SetInstanceName(callBack.InstanceName);
            uctlTestSubMM.ShowMessage("执行初始化");
			uctlTestSubMM.SetSubMMCmdExecutorCallBack(callBack);
     //       if (this.callBack.Alias.ToString() == "Test_ElectricalMngObj1")
     //       {
     //           Task.Run(async () =>
     //           {
					//while (true)
					//{
     //                   if (GriffinsApplication.Terminated)
     //                       break;
					//	await Task.Delay(5000);
					//	try
					//	{
					//		this.callBack.SendToMapTmlStateChanged("");
					//	}
					//	catch { }
					//}					
     //           });
     //       }
		}

		/// <summary>
		/// 初始化后
		/// </summary>
		void ISubMMCmdExecutor.AfterInit()
		{
            //初始化后：在此处创建界面数据对象；测试插件通过委托创建界面数据对象
            OnAfterInit?.Invoke();
            //测试插件再初始化测试面板的界面数据对象属性值设置界面
            uctlTestSubMM.ViewModel.AfterInit();
            uctlTestSubMM.ShowMessage("执行初始化后");
		}

		/// <summary>
		/// 反初始化
		/// </summary>
		void ISubMMCmdExecutor.UnInit() 
        {
			uctlTestSubMM.ShowMessage("执行反初始化");
		}


		ISubMMManualModeCmdExecutor ISubMMCmdExecutor.GetSubMMManualModeCmdExecutor()
		{
            return this;
		}

		ISubMMAutoModeCmdExecutor ISubMMCmdExecutor.GetSubMMAutoModeCmdExecutor()
		{
            return this;
		}

        #endregion ISubMMCmdExecutor 接口实现

        #region ISubMMManualModeCmdExecutor 接口实现

        /// <summary>
        /// 执行配置时控制命令
        /// </summary>
        /// <param name="cmdID">命令ID</param>
        /// <param name="cmdParam">命令参数</param>
        /// <returns>命令执行结果</returns>
        GFBaseTypeParamValueList ISubMMManualModeCmdExecutor.ExecCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam) 
        {
			string msg = $"执行配置时控制命令：{cmdID},参数：{JsonObjConvert.ToJSon(cmdParam)}";
			this.uctlTestSubMM.ShowMessage(msg);
			return null;
		}

		#endregion

		#region ISubMMAutoModeCmdExecutor 接口实现
		/// <summary>
		/// 准备切换产品配方
		/// </summary>
		void ISubMMAutoModeCmdExecutor.BeforeSwitchPF()
        {
            uctlTestSubMM.ShowMessage("准备切换产品配方");
        }
        /// <summary>
        /// 设置产品配方参数
        /// </summary>
        /// <param name="pfCfgInfo">配方参数，null表示缺省值</param>
        void ISubMMAutoModeCmdExecutor.SetPFCfgInfo(byte[] pfCfgInfo)
        {
            uctlTestSubMM.ShowMessage("执行设置配方参数");
        }

        /// <summary>
        /// 执行运行时控制命令
        /// </summary>
        /// <param name="cmdID">命令ID</param>
        /// <param name="cmdParam">命令参数</param>
        /// <returns>命令执行结果</returns>
        GFBaseTypeParamValueList ISubMMAutoModeCmdExecutor.ExecRuntimeCtlCmd(string cmdID, GFBaseTypeParamValueList cmdParam)
        {
            uctlTestSubMM.ShowMessage($"执行运行时控制命令,命令ID{cmdID}->{JsonObjConvert.ToJSon(cmdParam)}");
            return null;
        }

        /// <summary>
        /// 是否可以开始工作
        /// </summary>
        /// <param name="reasonMsg">不可以开始工作原因</param>
        /// <returns>True:可以开始工作，False:不可以开始工作</returns>
        bool ISubMMAutoModeCmdExecutor.CanStartWork(out string reasonMsg) 
        {
            reasonMsg = string.Empty;
            return true;
		}

		/// <summary>
		/// 通知子机械模组开始工作
		/// </summary>
		void ISubMMAutoModeCmdExecutor.StartWork()
        {
            uctlTestSubMM.ShowMessage("执行通知子机械模组开始工作");
            uctlTestSubMM.SetWorkState("工作中");
            pauseObj.Status = 0;
		}
        /// <summary>
        /// 通知子机械模组停止工作
        /// </summary>
        void ISubMMAutoModeCmdExecutor.StopWork() 
        {
            uctlTestSubMM.ShowMessage("通知子机械模组停止工作");
            uctlTestSubMM.SetWorkState("停止");
			pauseObj.Status = 1;
		}
        /// <summary>
        /// 停止工作后执行
        /// </summary>
        void ISubMMAutoModeCmdExecutor.AfterStopWork()
        {
            uctlTestSubMM.ShowMessage("停止工作后执行");
        }

        /// <summary>
        /// 通知子机械模组暂停工作
        /// </summary>
        void ISubMMAutoModeCmdExecutor.Pause() 
        {
            uctlTestSubMM.ShowMessage("通知子机械模组暂停工作");
            uctlTestSubMM.SetWorkState("暂停");
			pauseObj.Status = 2;
		}
        /// <summary>
        /// 通知子机械模组恢复工作
        /// </summary>
        void ISubMMAutoModeCmdExecutor.Resume() 
        {
            uctlTestSubMM.ShowMessage("通知子机械模组恢复工作");
            uctlTestSubMM.SetWorkState("工作中");
			pauseObj.Status = 3;
		}
        /// <summary>
        /// 通用普通事件
        /// </summary>
        public event ImeGenNormalEventHandler GenNormalEvent;
        private void doGenNormalEvent(ImeGenNormalEventArgs e)
        {
            GenNormalEvent?.Invoke(this, e);
        }

        /// <summary>
        /// 子机械模组能力事件
        /// </summary>
        public event ImeCabilityEventHandler CabilityEvent;
        private void doCabilityEvent(ImeCabilityEventArgs e)
        {
            CabilityEvent?.Invoke(this, e);
        }
        /// <summary>
        /// 接收到子机械模组事件
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
        GFBaseTypeParamValueList ISubMMAutoModeCmdExecutor.ExecMethod(string methodID, GFBaseTypeParamValueList methodParam)
        {
            return execMethod(methodID, methodParam);
        }

        /// <summary>
        /// 异步执行方法
        /// </summary>
        /// <param name="methodID">方法ID</param>
        /// <param name="param">方法参数</param>
        /// <returns>返回结果</returns>
        Task<GFBaseTypeParamValueList> ISubMMAutoModeCmdExecutor.AsynExecMethod(string methodID, GFBaseTypeParamValueList methodParam)
        {
            return Task.Run(() =>
            {
                return execMethod(methodID, methodParam);
            });
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="methodID">方法ID</param>
        /// <param name="methodParam">方法参数</param>
        /// <returns>返回结果</returns>
        private GFBaseTypeParamValueList execMethod(string methodID, GFBaseTypeParamValueList methodParam)
        {
            uctlTestSubMM.ShowExecCount();
            if (execStandardCommand(methodID, methodParam, out GFBaseTypeParamValueList result))
                return result;
            GenMethodDefInfo genMethodDefInfo = this.genSubMMInfo.FindNormalMethodDefInfo(methodID);
            if (genMethodDefInfo == null)
            {
                string errorMsg = $"{methodID}不是本子机械模组的普通方法";
                uctlTestSubMM.ShowErrorMessage(errorMsg);
                throw new Exception(errorMsg);
            }

            string curMethod = $"{genMethodDefInfo.MethodName}({methodID})";
            string msg = $"执行普通方法：{curMethod}";

            if (genMethodDefInfo.IsAsyn)
            {
                Task.Run(() =>
                {
                    execMethod(genMethodDefInfo, curMethod, msg);
                }).ConfigureAwait(false);
                return null;
            }
            else
            {
                execMethod(genMethodDefInfo, curMethod, msg);
                if (genMethodDefInfo.ParamConvertDele == null)
                {
                    result = null;
                }
                else
                {
                    result = genMethodDefInfo.ParamConvertDele(methodParam);
                }
                return result;
            }
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="methodID">方法ID</param>
        /// <param name="methodParam">方法参数</param>
        /// <returns>返回结果</returns>
        GFParamValueList ISubMMAutoModeCmdExecutor.ExecMethod(string methodID, GFParamValueList methodParam)
        {
            return execMethod(methodID, methodParam);
        }

        /// <summary>
        /// 异步执行方法
        /// </summary>
        /// <param name="methodID">方法ID</param>
        /// <param name="methodParam">方法参数</param>
        /// <returns>返回结果</returns>
        Task<GFParamValueList> ISubMMAutoModeCmdExecutor.AsynExecMethod(string methodID, GFParamValueList methodParam)
        {
            return Task.Run(() =>
            {
                return execMethod(methodID, methodParam);
            });
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="methodID">方法ID</param>
        /// <param name="param">方法参数</param>
        /// <returns>返回结果</returns>
        private GFParamValueList execMethod(string methodID, GFParamValueList methodParam)
        {
            string errorMsg = $"{methodID}不是本子机械模组的使用GFParamValueList类型参数的普通方法";
            uctlTestSubMM.ShowErrorMessage(errorMsg);
            throw new Exception(errorMsg);
        }


        private void execMethod(GenMethodDefInfo genMethodDefInfo, string curMethod, string msg)
        {
            uctlTestSubMM.SetExecMethod(true, curMethod);
            try
            {
                uctlTestSubMM.ShowMessage(msg);
                showImeMonitorMsg(msg);
                Task task = waitingAsync(genMethodDefInfo.CurDelyTime);
            }
            finally
            {
                uctlTestSubMM.SetExecMethod(false);
            }
        }

        /// <summary>
        ///  执行标准命令
        /// </summary>
        /// <param name="methodID"></param>
        /// <param name="methodParam"></param>
        /// <param name="result"></param>
        /// <param name="errorMsg"></param>
        /// <param name="errCode"></param>
        /// <returns>是否为标准命令，true：是，false：否</returns>
        private bool execStandardCommand(string methodID, GFBaseTypeParamValueList methodParam, out GFBaseTypeParamValueList result)
        {
            switch (methodID)
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
        GFBaseTypeParamValueList ISubMMAutoModeCmdExecutor.ExecCabilityMethod(string methodID, GFBaseTypeParamValueList methodParam)
        {
			return execCabilityMethod(methodID, methodParam);
          
        }

        /// <summary>
        /// 异步执行能力方法
        /// </summary>
        /// <param name="methodID">能力方法ID</param>
        /// <param name="methodParam">能力方法参数</param>
        /// <returns>返回结果</returns>
        Task<GFBaseTypeParamValueList> ISubMMAutoModeCmdExecutor.AsynExecCabilityMethod(string methodID, GFBaseTypeParamValueList methodParam)
        {
            return Task.Run(() =>
            {
                return execCabilityMethod(methodID, methodParam);
            });
        }

        /// <summary>
        /// 执行能力方法
        /// </summary>
        /// <param name="methodID">能力方法ID</param>
        /// <param name="methodParam">方法参数</param>
        /// <returns>返回结果</returns>
        private GFBaseTypeParamValueList execCabilityMethod(string methodID, GFBaseTypeParamValueList methodParam)
        {
            uctlTestSubMM.ShowExecCount();
			pauseObj.Wait();
			GenMethodDefInfo genMethodDefInfo = this.genSubMMInfo.FindCabilityMethodDefInfo(methodID);
            if (genMethodDefInfo == null)
            {
                string errorMsg = $"{methodID}不是本子机械模组的能力方法";
                uctlTestSubMM.ShowErrorMessage(errorMsg);
                throw new Exception(errorMsg);
            }
             
            string curMethod = $"{genMethodDefInfo.MethodName}({methodID})";
            string msg = $"执行能力方法：{curMethod}";

            execMethod(genMethodDefInfo, curMethod, msg);
			pauseObj.Wait();
			if (genMethodDefInfo.ParamConvertDele == null)
                return null;
            else
                return genMethodDefInfo.ParamConvertDele(methodParam);
        }

        private CompUIDataObjPropValRW _compUIDataObjPropValRW =null;
        private CompUIDataObjPropValRW compUIDataObjPropValRW
        {
            get
            {
                if (_compUIDataObjPropValRW == null)
                    _compUIDataObjPropValRW = new CompUIDataObjPropValRW(alias, uctlTestSubMM, genSubMMInfo);
                return _compUIDataObjPropValRW;
            }
        }
        /// <summary>
        /// 获取界面数据对象属性读写接口实例，如果不支持返回null
        /// </summary>
        /// <returns>界面数据对象属性读写接口实例</returns>
        ICompUIDataObjPropValRW ISubMMAutoModeCmdExecutor.GetUIDataObjPropValRW()
        {
            return compUIDataObjPropValRW;
        }

		/// <summary>
		/// 设置设备运行模式
		/// </summary>
		/// <param name="imeRunMode">设备运行模式</param>
		void ISubMMAutoModeCmdExecutor.SetRunMode(ImeRunMode imeRunMode)
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
			uctlTestSubMM.ShowMessage(runModeStr);
		}

        #endregion ISubMMAutoModeCmdExecutor 接口实现

        ///// <summary>
        ///// 获取子界面数据对象项名称字典
        ///// </summary>
        ///// <param name="objInstPropPath">界面数据对象属性路径</param>
        ///// <returns>子界面数据对象项名称字典</returns>
        //Dictionary<string, string> ISubMMCmdExecutor.GetSubUIProObjItemNames(ObjInstPropPath objInstPropPath)
        //{
        //    return genSubMMInfo.IUIDataObjPropExChange.GetSubUIProObjItemNames(objInstPropPath);
        //}
        private async Task waitingAsync(int waitingTime)
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
                    var imeMonitorMsg = new IGNEventParam_ImeMonitorMsg(DateTime.Now, msg);
                    doGenNormalEvent(new ImeGenNormalEventArgs(IGNEventParam_ImeMonitorMsg.EventKind, imeMonitorMsg.ToGFBaseTypeParamValues()));
                }
                catch { }
            });
        }

		#region 内部类型

		private class CompUIDataObjPropValRW: ICompUIDataObjPropValRW
        {
            private UctlTestSubMMView uctlTestSubMM;
            private GenSubMMInfo genSubMMInfo;
            private SubMMAlias alias;
            public CompUIDataObjPropValRW(SubMMAlias alias,UctlTestSubMMView uctlTestSubMM, GenSubMMInfo genSubMMInfo)
            {
                this.alias = alias;
                this.uctlTestSubMM = uctlTestSubMM;
                this.genSubMMInfo = genSubMMInfo;
            }

            ///// <summary>
            /////  设置界面数据对象属性值
            ///// </summary>
            ///// <param name="propertyID">属性值ID</param>
            ///// <param name="value">属性值</param>
            //void ICompUIDataObjPropValRW.SetUIDataObjPropValue(MPPropertyID propertyID, GriffinsBaseValue value)
            //{
            //    uctlTestSubMM.SetUIDataObjPropValue(propertyID, value);
            //}
            ///// <summary>
            /////  设置一组界面数据对象属性值
            ///// </summary>
            ///// <param name="values">属性值列表</param>
            //void ICompUIDataObjPropValRW.SetUIDataObjPropValues(GFBaseTypePropValueList values)
            //{
            //    foreach(var item in values)
            //    {
            //        uctlTestSubMM.SetUIDataObjPropValue(item.PropertyID, item.Value);
            //    }
            //}
            ///// <summary>
            /////  获取界面数据对象属性值
            ///// </summary>
            ///// <param name="propertyID">属性值ID</param>
            ///// <returns>属性值</returns>
            //GriffinsBaseValue ICompUIDataObjPropValRW.GetUIDataObjPropValue(MPPropertyID propertyID)
            //{
            //    return uctlTestSubMM.GetUIDataObjPropValue(propertyID);
            //}
            ///// <summary>
            /////  获取一组界面数据对象属性值
            ///// </summary>
            ///// <param name="propertyIDs">属性值ID列表</param>
            ///// <returns>属性值列表</returns>
            //GFBaseTypePropValueList ICompUIDataObjPropValRW.GetUIDataObjPropValues(MPPropertyID[] propertyIDs)
            //{
            //    var values = new GFBaseTypePropValueList();
            //    foreach (var propertyID in propertyIDs)
            //    {
            //        values.Add(new GFBaseTypePropValue
            //        {
            //            PropertyID = propertyID,
            //            Value = uctlTestSubMM.GetUIDataObjPropValue(propertyID)
            //        });
            //    }
            //    return values;
            //}
            ///// <summary>
            /////  获取所有界面数据对象属性值
            ///// </summary>
            ///// <returns>所有界面数据对象属性值列表</returns>
            //GFBaseTypePropValueList ICompUIDataObjPropValRW.GetAllUIDataObjPropValues()
            //{
            //    var gfBaseTypePropValues= uctlTestSubMM.GetAllUIDataObjPropValues();
            //    return gfBaseTypePropValues;
            //}

            /// <summary>
            ///  设置界面数据对象属性值
            /// </summary>
            /// <param name="objInstPropPath">属性路径值ID</param>
            /// <param name="value">属性值</param>
            void ICompUIDataObjPropValRW.SetUIDataObjPropPathValue(ObjInstPropPath objInstPropPath, GriffinsBaseValue value)
            {
                GFBaseTypePropValueList gfBaseTypePropValues = uctlTestSubMM.GetAllUIDataObjPropValues();
                GFBaseTypeObjPropPathValueList gFBaseTypeObjPropPathValues = new GFBaseTypeObjPropPathValueList();
                gFBaseTypeObjPropPathValues.Add(new GFBaseTypeObjPropPathValue()
                {
                    ObjInstPropPath = objInstPropPath,
                    Value = value
                });
                gfBaseTypePropValues.Merge(gFBaseTypeObjPropPathValues);
                foreach (var item in gfBaseTypePropValues)
                {
                    //找到当前带路径的属性所属的界面数据对象属性并设置
                    GFBaseTypeObjPropPathValue gFBaseTypeObjPropPathValue = item.GetLeafGFBaseTypeObjPropPathValues().Find(objInstPropPath);
                    if (gFBaseTypeObjPropPathValue != null)
                    {
                        uctlTestSubMM.SetUIDataObjPropValue(item.PropertyID, item.Value);
                        break;
                    }
                }
            }
            /// <summary>
            ///  设置一组界面数据对象属性路径值
            /// </summary>
            /// <param name="values">属性路径值列表</param>
            void ICompUIDataObjPropValRW.SetUIDataObjPropPathValues(GFBaseTypeObjPropPathValueList values)
            {
                GFBaseTypePropValueList gfBaseTypePropValues = uctlTestSubMM.GetAllUIDataObjPropValues();
                gfBaseTypePropValues.Merge(values);
                foreach (var item in gfBaseTypePropValues)
                {
                    uctlTestSubMM.SetUIDataObjPropValue(item.PropertyID, item.Value);
                }
            }
            /// <summary>
            ///  获取界面数据对象属性路径值
            /// </summary>
            /// <param name="objInstPropPath">属性路径值ID</param>
            /// <returns>属性路径值</returns>
            GriffinsBaseValue ICompUIDataObjPropValRW.GetUIDataObjPropPathValue(ObjInstPropPath objInstPropPath)
            {
                GFBaseTypePropValueList gfBaseTypePropValues = uctlTestSubMM.GetAllUIDataObjPropValues();
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
                GFBaseTypePropValueList gfBaseTypePropValues = uctlTestSubMM.GetAllUIDataObjPropValues();
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
                GFBaseTypePropValueList gfBaseTypePropValues = uctlTestSubMM.GetAllUIDataObjPropValues();
                return gfBaseTypePropValues.GetLeafGFBaseTypeObjPropPathValues();
            }
            /// <summary>
            /// 界面数据对象属性值改变事件
            /// </summary>
            public event ImePropValChangedEventHandler UIDataObjPropValChangedEvent;
            //public void DoUIDataObjPropValChanged(GFBaseTypePropValue propVal)
            //{
            //    UIDataObjPropValChangedEvent?.Invoke(this, new ImePropValChangedEventArgs(propVal.PropertyID, propVal.Value,DateTime.Now));
            //}
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
                uctlTestSubMM.ShowMessage($"执行界面数据对象命令：{cmdID}:{JsonObjConvert.ToJSon(cmdParam)}");
                return new GFBaseTypeParamValueList();
            }
		}

        #endregion
    }
}
