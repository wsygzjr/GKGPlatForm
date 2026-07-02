using GF_Gereric;
using Griffins.CompUI.Calibration.Models;
using ReactiveUI;
using System;
using System.Reactive.Linq;

namespace Griffins.CompUI.Calibration.ViewModels
{
    /// <summary>
    /// 标定设计时配置-视图模型
    /// </summary>
    public class CalibrationDesignCfgViewModel : ReactiveObject
    {
        /// <summary>
        /// 内部子页面配置信息
        /// </summary>
        private byte[]? _cfgInfo;
        /// <summary>
        /// 标定设计时配置信息
        /// </summary>
        private CalibrationCfgPageDesignCfgInfo _calibrationCfgPageDesignCfgInfo;
        /// <summary>
        /// 标定设计配置信息模型
        /// </summary>
        public CalibrationCfgPageDesignCfgInfoViewModel CalibrationCfgPageDesignCfgInfoViewModel { get; }
        /// <summary>
        /// 配置变更事件
        /// </summary>
        public event EventHandler? AfterModified;

        /// <summary>
        /// 内部子页面配置信息
        /// </summary>
        public byte[] CfgInfo
        {
            get
            {
                _cfgInfo = JsonObjConvert.ToJSonBytes(_calibrationCfgPageDesignCfgInfo);
                return _cfgInfo;
            }
            set
            {
                _cfgInfo = value;
                if (_cfgInfo != null)
                {
                    _calibrationCfgPageDesignCfgInfo = JsonObjConvert.FromJSonBytes<CalibrationCfgPageDesignCfgInfo>(_cfgInfo);
                    CalibrationCfgPageDesignCfgInfoViewModel.CopyFrom(_calibrationCfgPageDesignCfgInfo);

                }
            }
        }

        public CalibrationDesignCfgViewModel()
        {
            _calibrationCfgPageDesignCfgInfo = new CalibrationCfgPageDesignCfgInfo();
            CalibrationCfgPageDesignCfgInfoViewModel = new CalibrationCfgPageDesignCfgInfoViewModel();

            this.WhenAnyValue(
                    x => x.CalibrationCfgPageDesignCfgInfoViewModel.CalibrationTechnicalModulesAlias
                )
                .Subscribe(_ =>
                {
                    CalibrationCfgPageDesignCfgInfoViewModel.CopyTo(_calibrationCfgPageDesignCfgInfo);
                    onAfterModified();
                });
        }

        /// <summary>
        /// 配置信息改变后事件触发1
        /// </summary>

        private void onAfterModified()
        {
            AfterModified?.Invoke(this, EventArgs.Empty);
        }
    }
}
