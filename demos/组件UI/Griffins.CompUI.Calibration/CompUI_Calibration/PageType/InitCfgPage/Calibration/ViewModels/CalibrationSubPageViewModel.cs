using Avalonia.Controls;
using GF_Gereric;
using GKG.UI.General;
using Griffins.CompUI.Calibration.Models;
using ReactiveUI;
using System;

namespace Griffins.CompUI.Calibration.ViewModels
{
    /// <summary>
    /// 标定子页面的视图模型
    /// </summary>
    public class CalibrationSubPageViewModel : ReactiveObject
    {
        /// <summary>
        /// 内部子页面配置信息
        /// </summary>
        private byte[]? _cfgInfo;
        /// <summary>
        /// 页面布局对象
        /// </summary>
        private Control? _viewReference;
        private FunctionHeadCalibrationCfgInfoList _calibrationCfgInfo;
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// 标定-视图模型
        /// 实现多种标定类型对于的视图模型
        /// </summary>
        public CalibrationViewModel CalibrationViewModel { get; }

        /// <summary>
        /// 标定结果-视图模型
        /// </summary>
        public CalibrationResultViewModel CalibrationResultViewModel { get; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public CalibrationSubPageViewModel()
        {
            _calibrationCfgInfo = new FunctionHeadCalibrationCfgInfoList();
            CalibrationViewModel = new CalibrationViewModel();
            CalibrationResultViewModel = new CalibrationResultViewModel();
            CalibrationViewModel.OnCalibrationValueChanged += onCalibrationResultInfoChangedEventArgs;
        }

        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Control view)
        {
            CalibrationViewModel.SetViewReference(view);
            GlobalVisionViewModel.CameraOperationViewModel.SetViewReference(view);
            _viewReference = view;
        }
        private void onCalibrationResultInfoChangedEventArgs(object? sender, CalibrationResultInfoChangedEventArgs e)
        {
            CalibrationResultViewModel?.SetCalibrationResultInfo(e);
            AfterModified?.Invoke(sender, e);
        }
        #region 内部子页面接口

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="cfgInfo">内部子页面配置信息</param>
        public void Init(byte[]? cfgInfo)
        {
            _cfgInfo = cfgInfo;
            loadCfgInfo(cfgInfo);
        }

        /// <summary>
        /// 内部子页面配置信息
        /// </summary>
        public byte[]? CfgInfo
        {
            get
            {
                _calibrationCfgInfo = extract();
                _cfgInfo = JsonObjConvert.ToJSonBytes(_calibrationCfgInfo);
                return _cfgInfo;
            }
        }
        private FunctionHeadCalibrationCfgInfoList extract()
        {
            updateToCache();
            //从缓存中读取标定信息
            return CalibrationCacheDataMng.GetCalibration();
        }
        /// <summary>
        /// 将数据回写到缓存
        /// 因为没有切换阀，所以当前选中的阀的界面配置信息和标定结果没有写到缓存的
        /// </summary>
        public void updateToCache()
        {
            CalibrationViewModel.UpdateToCache();
        }
        /// <summary>
        /// 加载配置信息
        /// </summary>
        /// <param name="cfgInfo"></param>
        private void loadCfgInfo(byte[]? cfgInfo)
        {
            if (cfgInfo != null)
            {
                try
                {
                    _calibrationCfgInfo = JsonObjConvert.FromJSonBytes<FunctionHeadCalibrationCfgInfoList>(cfgInfo);
                }
                catch
                {
                    _calibrationCfgInfo = new FunctionHeadCalibrationCfgInfoList();
                }
            }
            CalibrationCacheDataMng.SetCalibration(_calibrationCfgInfo);
        }
        #endregion
    }
}