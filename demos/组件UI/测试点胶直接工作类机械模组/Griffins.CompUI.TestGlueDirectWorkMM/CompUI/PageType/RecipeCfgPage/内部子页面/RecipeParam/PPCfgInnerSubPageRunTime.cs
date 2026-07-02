using DispensingPageType.Views.RecipeParamCfgPage.ParametersAndCalculation;
using Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.ViewModels;
using Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.Views;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels;
using Griffins.Map.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage
{
    /// <summary>
    /// 工艺参数与计算轨迹内部子页面运行时接口实现对象
    /// </summary>
    public class PPCfgInnerSubPageRunTime : IInnerSubPageRunTime
    {

        private ParametersAndCalculationViewModel _recipeparamcfgSubPageViewModel;

        private EventHandler? afterDataModified;
        private ICompUIRunTimeCallBack? callBack;
        /// <summary>
        /// 构造函数
        /// </summary>
        public PPCfgInnerSubPageRunTime()
        {
            _recipeparamcfgSubPageViewModel = new ParametersAndCalculationViewModel();
            _recipeparamcfgSubPageViewModel.AfterModified += onAfterModified;
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
                var parametersAndCalculationView = new ParametersAndCalculationView();
                parametersAndCalculationView.DataContext = _recipeparamcfgSubPageViewModel;
                _recipeparamcfgSubPageViewModel.SetViewReference(parametersAndCalculationView);
                return parametersAndCalculationView;
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
            _recipeparamcfgSubPageViewModel.Init(data);
        }
        /// <summary>
        /// 获取数据信息，null表示缺省值
        /// </summary>
        /// <returns>数据信息，null表示缺省值</returns>
        byte[] IInnerSubPageRunTime.GetData()
        {
            return _recipeparamcfgSubPageViewModel.CfgInfo;
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