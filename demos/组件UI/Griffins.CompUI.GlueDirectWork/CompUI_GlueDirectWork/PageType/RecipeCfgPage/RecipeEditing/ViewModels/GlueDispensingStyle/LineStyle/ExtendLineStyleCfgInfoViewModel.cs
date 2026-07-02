using GKG.UI;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models;
using ReactiveUI;
using System;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    /// <summary>
    /// 内部扩展线样式工艺参数-视图模型
    /// </summary>
    public class ExtendLineStyleCfgInfoViewModel : ReactiveObject
    {
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;

        #region 响应式字段与UI控件ViewModel

        public ToggleSwitchViewModel EnableViewModel { get; }
        /// <summary>
        /// 是否要点胶
        /// </summary>
        public bool Enable
        {
            get => EnableViewModel.IsChecked;
            set
            {
                EnableViewModel.IsChecked = value;
                this.RaisePropertyChanged(nameof(Enable));
            }
        }

        public NumericViewModel AdvanceDistanceViewModel { get; }
        /// <summary>
        /// 提前距离
        /// </summary>
        public decimal AdvanceDistance
        {
            get => AdvanceDistanceViewModel.Value;
            set
            {
                AdvanceDistanceViewModel.Value = value;
                this.RaisePropertyChanged(nameof(AdvanceDistance));
            }
        }

        public NumericViewModel AdvanceProcessingDistanceViewModel { get; }
        /// <summary>
        /// 提前加工距离
        /// </summary>
        public decimal AdvanceProcessingDistance
        {
            get => AdvanceProcessingDistanceViewModel.Value;
            set
            {
                AdvanceProcessingDistanceViewModel.Value = value;
                this.RaisePropertyChanged(nameof(AdvanceProcessingDistance));
            }
        }

        public NumericViewModel AdvanceProcessingTimeViewModel { get; }
        /// <summary>
        /// 提前加工时间
        /// </summary>
        public decimal AdvanceProcessingTime
        {
            get => AdvanceProcessingTimeViewModel.Value;
            set
            {
                AdvanceProcessingTimeViewModel.Value = value;
                this.RaisePropertyChanged(nameof(AdvanceProcessingTime));
            }
        }

        public NumericViewModel DelayDistanceViewModel { get; }
        /// <summary>
        /// 延后距离
        /// </summary>
        public decimal DelayDistance
        {
            get => DelayDistanceViewModel.Value;
            set
            {
                DelayDistanceViewModel.Value = value;
                this.RaisePropertyChanged(nameof(DelayDistance));
            }
        }

        public NumericViewModel DelayStopProcessingDistanceViewModel { get; }
        /// <summary>
        /// 延迟停止加工距离
        /// </summary>
        public decimal DelayStopProcessingDistance
        {
            get => DelayStopProcessingDistanceViewModel.Value;
            set
            {
                DelayStopProcessingDistanceViewModel.Value = value;
                this.RaisePropertyChanged(nameof(DelayStopProcessingDistance));
            }
        }

        public NumericViewModel BackDistanceViewModel { get; }
        /// <summary>
        /// 回走距离
        /// </summary>
        public decimal BackDistance
        {
            get => BackDistanceViewModel.Value;
            set
            {
                BackDistanceViewModel.Value = value;
                this.RaisePropertyChanged(nameof(BackDistance));
            }
        }

        public NumericViewModel BackHeightViewModel { get; }
        /// <summary>
        /// 回走高度
        /// </summary>
        public decimal BackHeight
        {
            get => BackHeightViewModel.Value;
            set
            {
                BackHeightViewModel.Value = value;
                this.RaisePropertyChanged(nameof(BackHeight));
            }
        }

        public NumericViewModel PullBackSpeedViewModel { get; }
        /// <summary>
        /// 回拉速度
        /// </summary>
        public decimal PullBackSpeed
        {
            get => PullBackSpeedViewModel.Value;
            set
            {
                PullBackSpeedViewModel.Value = value;
                this.RaisePropertyChanged(nameof(PullBackSpeed));
            }
        }

        public NumericViewModel BackSpeedViewModel { get; }
        /// <summary>
        /// 回走速度
        /// </summary>
        public decimal BackSpeed
        {
            get => BackSpeedViewModel.Value;
            set
            {
                BackSpeedViewModel.Value = value;
                this.RaisePropertyChanged(nameof(BackSpeed));
            }
        }

        public NumericViewModel PullBackHeightViewModel { get; }
        /// <summary>
        /// 回拉高度
        /// </summary>
        public decimal PullBackHeight
        {
            get => PullBackHeightViewModel.Value;
            set
            {
                PullBackHeightViewModel.Value = value;
                this.RaisePropertyChanged(nameof(PullBackHeight));
            }
        }

        public NumericViewModel DispensingHeightViewModel { get; }
        /// <summary>
        /// 点胶高度
        /// </summary>
        public decimal DispensingHeight
        {
            get => DispensingHeightViewModel.Value;
            set
            {
                DispensingHeightViewModel.Value = value;
                this.RaisePropertyChanged(nameof(DispensingHeight));
            }
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExtendLineStyleCfgInfoViewModel()
        {
            EnableViewModel = new ToggleSwitchViewModel { IsChecked = true };

            AdvanceDistanceViewModel = new NumericViewModel { Minimum = 0, Increment = 0.1m, Value = 0 };
            AdvanceProcessingDistanceViewModel = new NumericViewModel { Minimum = 0, Increment = 0.1m, Value = 0 };
            DelayDistanceViewModel = new NumericViewModel { Minimum = 0, Increment = 0.1m, Value = 0 };
            DelayStopProcessingDistanceViewModel = new NumericViewModel { Minimum = 0, Increment = 0.1m, Value = 0 };
            BackDistanceViewModel = new NumericViewModel { Minimum = 0, Increment = 0.1m, Value = 0 };
            BackHeightViewModel = new NumericViewModel { Minimum = 0, Increment = 0.1m, Value = 0 };
            DispensingHeightViewModel = new NumericViewModel { Minimum = 0.1m, Maximum = 50.0m, Increment = 0.1m, Value = 1.0m };

            AdvanceProcessingTimeViewModel = new NumericViewModel { Minimum = 0, Increment = 1, Value = 0 };
            PullBackSpeedViewModel = new NumericViewModel { Minimum = 0, Increment = 0.1m, Value = 0 };
            BackSpeedViewModel = new NumericViewModel { Minimum = 0, Increment = 0.1m, Value = 0 };
            PullBackHeightViewModel = new NumericViewModel { Minimum = 0, Increment = 0.1m, Value = 0 };
            // 订阅值变更事件
            subscribeValueChanges();
        }

        /// <summary>
        /// 从配置模型复制数据
        /// </summary>
        /// <param name="cfgInfo">扩展线样式工艺参数配置</param>
        public void CopyFrom(ExtendLineStyleCfgInfo cfgInfo)
        {
            if (cfgInfo == null)
                throw new ArgumentNullException(nameof(cfgInfo), "扩展线样式配置模型不能为空");


            // 同步所有属性值
            Enable = cfgInfo.Enable;
            AdvanceDistance = cfgInfo.AdvanceDistance;
            AdvanceProcessingDistance = cfgInfo.AdvanceProcessingDistance;
            AdvanceProcessingTime = cfgInfo.AdvanceProcessingTime;
            DelayDistance = cfgInfo.DelayDistance;
            DelayStopProcessingDistance = cfgInfo.DelayStopProcessingDistance;
            BackDistance = cfgInfo.BackDistance;
            BackHeight = cfgInfo.BackHeight;
            PullBackSpeed = cfgInfo.PullBackSpeed;
            BackSpeed = cfgInfo.BackSpeed;
            PullBackHeight = cfgInfo.PullBackHeight;
            DispensingHeight = cfgInfo.DispensingHeight;
        }

        /// <summary>
        /// 复制到配置模型
        /// </summary>
        /// <param name="cfgInfo">待填充的扩展线样式工艺参数配置</param>
        public void CopyTo(ExtendLineStyleCfgInfo cfgInfo)
        {
            if (cfgInfo == null)
                throw new ArgumentNullException(nameof(cfgInfo), "扩展线样式配置不能为空");


            cfgInfo.Enable = Enable;
            cfgInfo.AdvanceDistance = AdvanceDistance;
            cfgInfo.AdvanceProcessingDistance = AdvanceProcessingDistance;
            cfgInfo.AdvanceProcessingTime = AdvanceProcessingTime;
            cfgInfo.DelayDistance = DelayDistance;
            cfgInfo.DelayStopProcessingDistance = DelayStopProcessingDistance;
            cfgInfo.BackDistance = BackDistance;
            cfgInfo.BackHeight = BackHeight;
            cfgInfo.PullBackSpeed = PullBackSpeed;
            cfgInfo.BackSpeed = BackSpeed;
            cfgInfo.PullBackHeight = PullBackHeight;
            cfgInfo.DispensingHeight = DispensingHeight;
        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            EnableViewModel.ValueChanged += onValueChanged;
            AdvanceDistanceViewModel.ValueChanged += onValueChanged;
            AdvanceProcessingDistanceViewModel.ValueChanged += onValueChanged;
            DelayDistanceViewModel.ValueChanged += onValueChanged;
            DelayStopProcessingDistanceViewModel.ValueChanged += onValueChanged;
            BackDistanceViewModel.ValueChanged += onValueChanged;
            BackHeightViewModel.ValueChanged += onValueChanged;
            DispensingHeightViewModel.ValueChanged += onValueChanged;
            AdvanceProcessingTimeViewModel.ValueChanged += onValueChanged;
            PullBackSpeedViewModel.ValueChanged += onValueChanged;
            BackSpeedViewModel.ValueChanged += onValueChanged;
            PullBackHeightViewModel.ValueChanged += onValueChanged;
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

        #endregion
    }
}