using GKG.UI;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Tools;
using ReactiveUI;
using System;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    /// <summary>
    /// 点胶前点样式配置-模型
    /// </summary>
    public class DispensingBeforePointStyleCfgInfoModel : DataGridItemBaseViewModel<DispensingBeforePointStyleCfgInfo>
    {
        #region 响应式字段与属性
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

        public NumericViewModel RotationAngleViewModel { get; }
        /// <summary>
        /// 旋转角度（°，建议取值范围 0-360）
        /// </summary>
        public decimal RotationAngle
        {
            get => RotationAngleViewModel.Value;
            set
            {
                RotationAngleViewModel.Value = value;
                this.RaisePropertyChanged(nameof(RotationAngle));
            }
        }

        public NumericViewModel TiltAngleViewModel { get; }
        /// <summary>
        /// 倾斜角度（°，建议取值范围 -90 至 90）
        /// </summary>
        public decimal TiltAngle
        {
            get => TiltAngleViewModel.Value;
            set
            {
                TiltAngleViewModel.Value = value;
                this.RaisePropertyChanged(nameof(TiltAngle));
            }
        }

        public NumericViewModel DispensingHeightViewModel { get; }
        /// <summary>
        /// 点胶高度（mm，建议取值范围 0.1-50.0）
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
        public NumericViewModel AdvanceValveOpeningTimeViewModel { get; }
        /// <summary>
        /// 提前开阀时间（ms，建议取值范围 0-500）
        /// </summary>
        public int AdvanceValveOpeningTime
        {
            get => Convert.ToInt32(AdvanceValveOpeningTimeViewModel.Value);
            set
            {
                AdvanceValveOpeningTimeViewModel.Value = value;
                this.RaisePropertyChanged(nameof(AdvanceValveOpeningTime));
            }
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public DispensingBeforePointStyleCfgInfoModel()
        {
            StyleID = Guid.NewGuid();
            StyleNameViewModel = new TextInputViewModel();
            RotationAngleViewModel = new NumericViewModel { Minimum = 1, Increment = 1, Value = 1 };
            TiltAngleViewModel = new NumericViewModel { Minimum = 1, Increment = 1, Value = 1 };
            DispensingHeightViewModel = new NumericViewModel { Minimum = 1, Increment = 1, Value = 1 };
            StabilizationTimeViewModel = new NumericViewModel { Minimum = 1, Increment = 1, Value = 1 };
            AdvanceValveOpeningTimeViewModel = new NumericViewModel { Minimum = 1, Increment = 1, Value = 1 };
            // 订阅值变更事件
            subscribeValueChanges();
        }

        /// <summary>
        ///从配置复制数据
        /// </summary>
        /// <param name="cfgInfo">点胶前点样式配置</param>
        public override void CopyFrom(DispensingBeforePointStyleCfgInfo cfgInfo)
        {
            if (cfgInfo == null)
                throw new ArgumentNullException(nameof(cfgInfo), "配置模型不能为空");

            base.CopyBasePropertiesFrom(cfgInfo);
            StyleID = cfgInfo.StyleID;
            StyleName = cfgInfo.StyleName;
            RotationAngle = cfgInfo.RotationAngle;
            TiltAngle = cfgInfo.TiltAngle;
            DispensingHeight = cfgInfo.DispensingHeight;
            StabilizationTime = cfgInfo.StabilizationTime;
            AdvanceValveOpeningTime = cfgInfo.AdvanceValveOpeningTime;
        }

        /// <summary>
        /// 复制到配置信息
        /// </summary>
        /// <param name="cfgInfo">待填充的点胶前点样式配置</param>
        public override void CopyTo(DispensingBeforePointStyleCfgInfo cfgInfo)
        {
            if (cfgInfo == null)
                throw new ArgumentNullException(nameof(cfgInfo), "配置不能为空");

            base.CopyBasePropertiesTo(cfgInfo);
            cfgInfo.StyleID = StyleID;
            cfgInfo.StyleName = StyleName;
            cfgInfo.RotationAngle = RotationAngle;
            cfgInfo.TiltAngle = TiltAngle;
            cfgInfo.DispensingHeight = DispensingHeight;
            cfgInfo.StabilizationTime = StabilizationTime;
            cfgInfo.AdvanceValveOpeningTime = AdvanceValveOpeningTime;
        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            StyleNameViewModel.ValueChanged += onValueChanged;
            RotationAngleViewModel.ValueChanged += onValueChanged;
            TiltAngleViewModel.ValueChanged += onValueChanged;
            DispensingHeightViewModel.ValueChanged += onValueChanged;
            StabilizationTimeViewModel.ValueChanged += onValueChanged;
            AdvanceValveOpeningTimeViewModel.ValueChanged += onValueChanged;
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