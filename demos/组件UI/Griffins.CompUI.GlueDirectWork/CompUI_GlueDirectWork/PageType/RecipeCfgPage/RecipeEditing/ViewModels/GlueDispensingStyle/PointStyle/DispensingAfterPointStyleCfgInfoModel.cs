using GKG.UI;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Tools;
using ReactiveUI;
using System;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    /// <summary>
    /// 点胶后样式配置-模型
    /// </summary>
    public class DispensingAfterPointStyleCfgInfoModel : DataGridItemBaseViewModel<DispensingAfterPointStyleCfgInfo>
    {

        #region 响应式字段与属性
        private bool _isEnabled;
        /// <summary>
        /// 是否启用该样式（勾选状态）
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
        }

        private Guid _styleID;
        /// <summary>
        /// 样式ID
        /// </summary>
        public Guid StyleID
        {
            get => _styleID;
            set => this.RaiseAndSetIfChanged(ref _styleID, value);
        }

        public TextInputViewModel StyleNameViewModel { get; }

        private string _styleName = string.Empty;
        /// <summary>
        /// 样式名称
        /// </summary>
        public string StyleName
        {
            get => StyleNameViewModel.Text;
            set
            {
                StyleNameViewModel.Text = value;
                this.RaisePropertyChanged(nameof(StyleName));
            }
        }

        public NumericViewModel StabilizationTimeViewModel { get; }
        /// <summary>
        /// 稳定时间（ms，建议取值范围 0-1000）
        /// </summary>
        public int StabilizationTime
        {
            get => Convert.ToInt32(StabilizationTimeViewModel.Value);
            set
            {
                StabilizationTimeViewModel.Value = value;
                this.RaisePropertyChanged(nameof(StabilizationTime));
            }
        }

        public NumericViewModel LiftHeightViewModel { get; }
        /// <summary>
        /// 回抬高度（mm，建议取值范围 0.1-100.0）
        /// </summary>
        public decimal LiftHeight
        {
            get => LiftHeightViewModel.Value;
            set
            {
                LiftHeightViewModel.Value = value;
                this.RaisePropertyChanged(nameof(LiftHeight));
            }
        }

        public NumericViewModel LiftSpeedViewModel { get; }
        /// <summary>
        /// 回抬速度（mm/s，建议取值范围 1-500）
        /// </summary>
        public decimal LiftSpeed
        {
            get => LiftSpeedViewModel.Value;
            set
            {
                LiftSpeedViewModel.Value = value;
                this.RaisePropertyChanged(nameof(LiftSpeed));
            }
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public DispensingAfterPointStyleCfgInfoModel()
        {
            StyleID = Guid.NewGuid();
            StyleNameViewModel = new TextInputViewModel();
            StabilizationTimeViewModel = new NumericViewModel { Minimum = 1, Increment = 1, Value = 1 };
            LiftHeightViewModel = new NumericViewModel { Minimum = 1, Increment = 1, Value = 1 };
            LiftSpeedViewModel = new NumericViewModel { Minimum = 1, Increment = 1, Value = 1 };
            // 订阅值变更事件
            subscribeValueChanges();
        }

        /// <summary>
        /// 从配置复制数据
        /// </summary>
        /// <param name="cfgInfo">点胶后点样式配置</param>
        public override void CopyFrom(DispensingAfterPointStyleCfgInfo cfgInfo)
        {
            if (cfgInfo == null)
                throw new ArgumentNullException(nameof(cfgInfo), "配置模型不能为空");

            base.CopyBasePropertiesFrom(cfgInfo);
            StyleID = cfgInfo.StyleID;
            StyleName = cfgInfo.StyleName;
            StabilizationTime = cfgInfo.StabilizationTime;
            LiftHeight = cfgInfo.LiftHeight;
            LiftSpeed = cfgInfo.LiftSpeed;
        }

        /// <summary>
        /// 复制到配置信息
        /// </summary>
        /// <param name="cfgInfo">待填充的点胶后点样式配置</param>
        public override void CopyTo(DispensingAfterPointStyleCfgInfo cfgInfo)
        {
            if (cfgInfo == null)
                throw new ArgumentNullException(nameof(cfgInfo), "配置不能为空");

            base.CopyBasePropertiesTo(cfgInfo);
            cfgInfo.StyleID = StyleID;
            cfgInfo.StyleName = StyleName;
            cfgInfo.StabilizationTime = StabilizationTime;
            cfgInfo.LiftHeight = LiftHeight;
            cfgInfo.LiftSpeed = LiftSpeed;
        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            StyleNameViewModel.ValueChanged += onValueChanged;
            StabilizationTimeViewModel.ValueChanged += onValueChanged;
            LiftHeightViewModel.ValueChanged += onValueChanged;
            LiftSpeedViewModel.ValueChanged += onValueChanged;
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