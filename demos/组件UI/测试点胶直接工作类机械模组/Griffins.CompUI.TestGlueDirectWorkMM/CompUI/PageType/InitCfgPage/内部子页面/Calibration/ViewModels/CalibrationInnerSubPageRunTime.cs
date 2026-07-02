//using Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.Views;
//using GF_Gereric;
//using Griffins.UI.General;
//using Griffins.ImeIOT;
//using Griffins.Map.UI;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System;

//namespace Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.ViewModels
//{
//    /// <summary>
//    /// 标定内部子页面运行时接口实现对象
//    /// </summary>
//    public class CalibrationInnerSubPageRunTime : IInnerSubPageRunTime
//    {
//        /// <summary>
//        /// 页面类型运行时回调
//        /// </summary>
//        private IPageTypeRunTimeCallBack? _callBack;
//        private CalibrationCfgPageDesignCfgInfo _calibrationCfgPageDesignCfgInfo=new();

//        private CalibrationSubPageViewModel _calibrationSubPageViewModel;
//        private CalibrationSubPageView _calibrationSubPageView;

//        /// <summary>
//        /// 界面参数数据改变事件
//        /// </summary>
//        public EventHandler? AfterDataModified {  get; set; }

//        /// <summary>
//        /// 构造函数
//        /// </summary>
//        public CalibrationInnerSubPageRunTime()
//        {
//            _calibrationSubPageViewModel = new CalibrationSubPageViewModel();
//            _calibrationSubPageViewModel.AfterModified += onAfterModified;
//            _calibrationSubPageView = new CalibrationSubPageView();
//        }
//        /// <summary>
//        /// 初始化
//        /// </summary>
//        /// <param name="callBack"></param>
//        public void Init(IPageTypeRunTimeCallBack? callBack)
//        {
//            _callBack = callBack;
           
//        }

//        #region 配置信息读取和保存

//        /// <summary>
//        /// 读取配置信息
//        /// </summary>
//        /// <returns></returns>
//        private byte[]? readCfg()
//        {
//            try
//            {
//                if (_callBack == null)
//                    throw new Exception("未设置执行命令回调实例");

//                string mmName = _calibrationCfgPageDesignCfgInfo.ModulesAlias;
//                throw new NotImplementedException();
//                //byte[]? cfgs = _callBack.GetCfgData(mmName);
//                ////返回的是所有模组数据，获取该模组的数据
//                //if (cfgs != null)
//                //{
//                //    ImeConfigData imeConfigData = JsonObjConvert.FromJSonBytes<ImeConfigData>(cfgs);
//                //    cfgs = imeConfigData.GetMMCfgInfo(new MMAlias(mmName));
//                //}
//                //return cfgs;
//            }
//            catch
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// 保存配置信息
//        /// </summary>
//        public void Save()
//        {
//            if (_callBack == null)
//                throw new Exception("未设置执行命令回调实例");

//            byte[]? cfgs = _calibrationSubPageViewModel.CfgInfo;
//            throw new NotImplementedException();
//            //_callBack.SetCfgData(_calibrationCfgPageDesignCfgInfo.ModulesAlias, cfgs);
//        }

//        #endregion

//        /// <summary>
//        /// 值改变事件
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void onAfterModified(object? sender, EventArgs e)
//        {
//            AfterDataModified?.Invoke(sender, e);
//        }

//        #region 内部子页面接口

//        /// <summary>
//        /// 初始化
//        /// </summary>
//        /// <param name="callBack">回调接口</param>
//        /// <param name="cfgInfo">内部子页面配置信息</param>
//        void IInnerSubPageRunTime.Init(IInnerSubPageCallBack callBack, byte[] cfgInfo)
//        {
//            setCfgInfo(cfgInfo);

//            InitCfgPageCommandExecutor.Instance.Init(_callBack);

//            //在运行时初始化接口调用
//            _calibrationSubPageView.DataContext = _calibrationSubPageViewModel;
//            _calibrationSubPageViewModel.SetViewReference(_calibrationSubPageView);

//            //GlobalVisionViewModel.Init(new CameraShowCfgInfo(), new CameraOperationCfgInfo());
//            //GlobalVisionViewModel.SetViewReference(_calibrationSubPageView);

//            byte[]? cfgs = readCfg();
//            _calibrationSubPageViewModel.Init(cfgs);
//        }

//        /// <summary>
//        /// 设置内部子页面配置信息 
//        /// </summary>
//        void IInnerSubPageRunTime.SetCfgInfo(byte[] cfgInfo)
//        {
//            setCfgInfo(cfgInfo);
            
//        }
//        /// <summary>
//        /// 设置内部子页面配置信息
//        /// </summary>
//        void setCfgInfo(byte[] cfgInfo)
//        {
//            try
//            {
//                if(cfgInfo!=null)
//                {
//                    _calibrationCfgPageDesignCfgInfo = JsonObjConvert.FromJSonBytes<CalibrationCfgPageDesignCfgInfo>(cfgInfo);
//                    if (_calibrationCfgPageDesignCfgInfo == null)
//                        throw new Exception("标定设计时配置数据解析失败");
//                    InitCfgPageCommandExecutor.Instance.SetCalibrationTechnicalModulesAlias(_calibrationCfgPageDesignCfgInfo.ModulesAlias);
//                }
//            }
//            catch (Exception)
//            {

//                throw;
//            }
           
//        }

//        /// <summary>
//        ///  内部子页面界面，应该从Control继承
//        /// </summary>
//        object IInnerSubPageRunTime.View
//        {
//            get
//            {
//                return _calibrationSubPageView;
//            }
//        }

//        #endregion
//    }
//}