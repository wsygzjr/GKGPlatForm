using GF_Gereric;
using Griffins.CompUI.ElectricalMngObj.DebuggingPage;
using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Design;
using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.ViewModels;
using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Views;
using Griffins.Map.UI;
using System;
using System.Collections.Generic;

namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage
{
    /// <summary>
    /// 运控卡配置内部子页面运行时接口实现对象
    /// </summary>
    public class ControlCardInnerSubPageRunTime : IInnerSubPageRunTime
    {
        private ControlCardInnerSubPageDesignCfg? _factoryPageDesignCfgInfo;
        private ControlCardCfgViewModel _controlCardCfgViewModel;
        private ControlCardCfgView? _cachedView;
        private EventHandler? afterDataModified;
        private ICompUIRunTimeCallBack? callBack;
       

        /// <summary>
        /// 构造函数
        /// </summary>
        public ControlCardInnerSubPageRunTime()
        {
            _controlCardCfgViewModel = new ControlCardCfgViewModel();
            _controlCardCfgViewModel.AfterModified += onAfterModified;
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
                if (_cachedView == null)
                {
                    _cachedView = new ControlCardCfgView();
                    _cachedView.DataContext = _controlCardCfgViewModel;
                    _controlCardCfgViewModel.SetViewReference(_cachedView);
                }

                return _cachedView;
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
            if (viewCfgInfo != null)
            {
                _factoryPageDesignCfgInfo = JsonObjConvert.FromJSonBytes<ControlCardInnerSubPageDesignCfg>(viewCfgInfo);
                if (_factoryPageDesignCfgInfo == null)
                    throw new Exception("运控卡配置页面设计时配置数据解析失败");
                //根据配置信息更新界面大小等操作
            }
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
            _controlCardCfgViewModel.Init(data);
        }
        /// <summary>
        /// 获取数据信息，null表示缺省值
        /// </summary>
        /// <returns>数据信息，null表示缺省值</returns>
        byte[] IInnerSubPageRunTime.GetData()
        {
            return _controlCardCfgViewModel.CfgInfo;
        }

        /// <summary>
        /// 检测数据合法性
        /// </summary>
        /// <param name="inValidMsg">不合法时的描述信息列表</param>
        /// <returns>是否合法 true:合法 false 不合法</returns>
        bool IInnerSubPageRunTime.CheckDataValid(out string[] inValidMsg)
        {
            List<string> validMsgList = new List<string>();
            bool result = true;
            if (_controlCardCfgViewModel.ControlCardListViewModel.ControlCardList.Count == 0)
            {
                validMsgList.Add("需要添加至少一个运控卡");
                result = false;
            }
            inValidMsg = validMsgList.ToArray();
            return result;
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