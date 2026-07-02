using DispensingPageType.Views.DebuggingPage.IOInDebugging;
using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.ViewModels;
using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Views;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.ElectricalMngObj.DebuggingPage
{
    /// <summary>
    /// IOIN调试配置内部子页面运行时接口实现对象
    /// </summary>
    public class IOInDebuggingInnerSubPageRunTime : IInnerSubPageRunTime
    {
        private IOInDebuggingSubPageViewModel _iOInDebuggingSubPageViewModel;
        private EventHandler? afterDataModified; 
        private ICompUIRunTimeCallBack? callBack;
        /// <summary>
        /// 构造函数
        /// </summary>
        public IOInDebuggingInnerSubPageRunTime()
        {
            _iOInDebuggingSubPageViewModel = new IOInDebuggingSubPageViewModel();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="callBack">回调接口</param>
        public void Init(ICompUIRunTimeCallBack callBack)
        {
            this.callBack = callBack;
        }
        #region ISubPageRunTime接口实现

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="viewCfgInfo">内部子页面界面配置信息</param>
        void ISubPageRunTime.Init(byte[] viewCfgInfo)
        {
            setCfgInfo(viewCfgInfo);
        }
        /// <summary>
        /// 内部子页面界面
        /// </summary>
        object ISubPageRunTime.View
        {
            get
            {
                var iOInDebuggingSubPageView = new IOInDebuggingSubPageView();
                iOInDebuggingSubPageView.DataContext = _iOInDebuggingSubPageViewModel;
                _iOInDebuggingSubPageViewModel.SetViewReference(iOInDebuggingSubPageView);
                return iOInDebuggingSubPageView;
            }
        }
        /// <summary>
        /// 设置内部子页面配置信息
        /// </summary>
        /// <param name="viewCfgInfo">内部子页面界面配置信息</param>
        void ISubPageRunTime.SetViewCfgInfo(byte[] viewCfgInfo)
        {
            setCfgInfo(viewCfgInfo);
        }
        /// <summary>
        /// 设置内部子页面配置信息
        /// </summary>
        /// <param name="viewCfgInfo">内部子页面界面配置信息</param>
        void setCfgInfo(byte[] viewCfgInfo)
        {

        }
        #endregion

        #region IInnerSubPageRunTime接口实现
        /// <summary>
        /// 界面参数数据改变事件
        /// </summary>
        event EventHandler IInnerSubPageRunTime.AfterModified
        {
            add
            {
                afterDataModified += value;
            }
            remove
            {
                afterDataModified -= value;
            }
        }

        /// <summary>
        /// 在初始化时调用
        /// </summary>
        void IInnerSubPageRunTime.OnInit()
        {
        }

        /// <summary>
		/// 设置数据信息
		/// </summary>
		/// <param name="data">数据信息，null表示缺省值</param>
		void IInnerSubPageRunTime.SetData(byte[] data)
        {
            //_iOInDebuggingSubPageViewModel.Init(data);
        }
        /// <summary>
        /// 获取数据信息，null表示缺省值
        /// </summary>
        /// <returns>数据信息，null表示缺省值</returns>
        byte[] IInnerSubPageRunTime.GetData()
        {
            return null;
            //return _iOInDebuggingSubPageViewModel.CfgInfo;
        }

        /// <summary>
        /// 检测数据合法性
        /// </summary>
        /// <param name="inValidMsg">不合法时的描述信息列表</param>
        /// <returns>是否合法 true:合法 false 不合法</returns>
        bool IInnerSubPageRunTime.CheckDataValid(out string[] inValidMsg)
        {
            inValidMsg = null;
            return true;
        }
        #endregion

        /// <summary>
        /// 值改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onAfterModified(object? sender, EventArgs e)
        {
            afterDataModified?.Invoke(sender, e);
        }
    }
}