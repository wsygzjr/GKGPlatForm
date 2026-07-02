using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Views;
using Griffins.UI;
using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Models;
using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.ViewModels.Axis;
using ReactiveUI;
using System;

namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.ViewModels
{
    /// <summary>
    /// 运控卡轴配置
    /// </summary>
    public class ControlCardAxisCfgViewModel : ReactiveObject
    {
        private int _axisNo = 0;
        private string _axisName = "";

        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// 轴名称-数据模型
        /// </summary>
        public TextInputViewModel AxisNameViewModel { get; }

        /// <summary>
        /// 轴号-数据模型
        /// </summary>
        public NumericViewModel AxisNoViewModel { get; }

        /// <summary>
        /// 轴号
        /// </summary>
        public int AxisNo
        {
            get
            {
                //_axisNo = (int)axisNoViewModel.Value;
                return _axisNo;
            }
            set
            {
                _axisNo = value;
                //axisNoViewModel.Value = _axisNo;
            }
        }

        /// <summary>
        /// 轴名称
        /// </summary>
        public string AxisName
        {
            get
            {
                //_axisName = axisNameViewModel.Text;
                return _axisName;
            }
            set
            {
                _axisName = value;
                //axisNameViewModel.Text = _axisName;
            }
        }
        ///// <summary>
        ///// 缓存的视图实例（避免切换Tab时重建）
        ///// </summary>
        //public ControlCardAxisCfgView CachedView { get; }

        /// <summary>
        /// 仅标记是否需要保留视图，不持有 View 实例！
        /// </summary>
        internal bool IsViewRecycled { get; set; } = false;
        /// <summary>
        /// 回零参数-视图模型
        /// </summary>
        public ReturnToZeroParamViewModel ReturnToZeroParamViewModel { get; }
        /// <summary>
        /// 轴状态逻辑参数配置-视图模型
        /// </summary>
        public AxisStateLogicParamViewModel AxisStateLogicParamViewModel { get; }
        /// <summary>
        /// 脉冲比参数配置-视图模型
        /// </summary>
        public PulseRatioParamViewModel PulseRatioParamViewModel { get; }
        /// <summary>
        /// 软限位参数配置-视图模型
        /// </summary>
        public SoftLimitParamViewModel SoftLimitParamViewModel { get; }
        /// <summary>
        /// 编码器类型参数配置-视图模型
        /// </summary>
        public EncoderTypeParamViewModel EncoderTypeParamViewModel { get; }
        /// <summary>
        /// 脉冲输入模式配置-视图模型
        /// </summary>
        public PulseInputModeParamViewModel PulseInputModeParamViewModel { get; }
        /// <summary>
        /// 脉冲输出模式配置-视图模型
        /// </summary>
        public PulseOutputModeParamViewModel PulseOutputModeParamViewModel { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ControlCardAxisCfgViewModel()
        {
            AxisNameViewModel = new TextInputViewModel();
            AxisNoViewModel = new NumericViewModel()
            {
                Minimum=1,
                Maximum=255,
                Increment=1,
                DecimalPlaces=0,
            };

            ReturnToZeroParamViewModel =new ReturnToZeroParamViewModel();
            AxisStateLogicParamViewModel = new AxisStateLogicParamViewModel();
            PulseRatioParamViewModel = new PulseRatioParamViewModel();
            SoftLimitParamViewModel = new SoftLimitParamViewModel();
            EncoderTypeParamViewModel = new EncoderTypeParamViewModel();
            PulseInputModeParamViewModel = new PulseInputModeParamViewModel();
            PulseOutputModeParamViewModel = new PulseOutputModeParamViewModel();
            //// 初始化时创建视图实例并缓存
            //CachedView = new ControlCardAxisCfgView { DataContext = this };
            // 订阅值变更事件
            subscribeValueChanges();
        }
        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="controlCardAxisCfgInfo"></param>
        public void CopyFrom(ControlCardAxisCfgInfo controlCardAxisCfgInfo)
        {
            AxisNo = controlCardAxisCfgInfo.AxisNo;
            AxisName=controlCardAxisCfgInfo.AxisName;
            ReturnToZeroParamViewModel.CopyFrom(controlCardAxisCfgInfo.ReturnToZeroParamInfo);
            AxisStateLogicParamViewModel.CopyFrom(controlCardAxisCfgInfo.AxisStateLogicParamInfo);
            PulseRatioParamViewModel.CopyFrom(controlCardAxisCfgInfo.PulseRatioParamInfo);
            SoftLimitParamViewModel.CopyFrom(controlCardAxisCfgInfo.SoftLimitParamInfo);
            EncoderTypeParamViewModel.CopyFrom(controlCardAxisCfgInfo.EncoderTypeParamInfo);
            PulseInputModeParamViewModel.CopyFrom(controlCardAxisCfgInfo.PulseInputModeParamInfo);
            PulseOutputModeParamViewModel.CopyFrom(controlCardAxisCfgInfo.PulseOutputModeParamInfo);
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="controlCardBaseCfg"></param>
        public void CopyTo(ControlCardAxisCfgInfo controlCardAxisCfgInfo)
        {

            ReturnToZeroParamViewModel.CopyTo(controlCardAxisCfgInfo.ReturnToZeroParamInfo);
            AxisStateLogicParamViewModel.CopyTo(controlCardAxisCfgInfo.AxisStateLogicParamInfo);
            PulseRatioParamViewModel.CopyTo(controlCardAxisCfgInfo.PulseRatioParamInfo);
            SoftLimitParamViewModel.CopyTo(controlCardAxisCfgInfo.SoftLimitParamInfo);
            EncoderTypeParamViewModel.CopyTo(controlCardAxisCfgInfo.EncoderTypeParamInfo);
            PulseInputModeParamViewModel.CopyTo(controlCardAxisCfgInfo.PulseInputModeParamInfo);
            PulseOutputModeParamViewModel.CopyTo(controlCardAxisCfgInfo.PulseOutputModeParamInfo);

        }

        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            ReturnToZeroParamViewModel.AfterModified += onAfterModified;
            AxisStateLogicParamViewModel.AfterModified += onAfterModified;
            PulseRatioParamViewModel.AfterModified += onAfterModified;
            SoftLimitParamViewModel.AfterModified += onAfterModified;
            EncoderTypeParamViewModel.AfterModified += onAfterModified;
            PulseInputModeParamViewModel.AfterModified += onAfterModified;
            PulseOutputModeParamViewModel.AfterModified += onAfterModified;

            AxisNameViewModel.ValueChanged += onValueChanged;
            AxisNoViewModel.ValueChanged += onValueChanged;
        }

        /// <summary>
        /// 值改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onValueChanged(object? sender, ValueChangedEventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }
        /// <summary>
        /// 值改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onAfterModified(object? sender, EventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        } 
        #endregion

    }
}