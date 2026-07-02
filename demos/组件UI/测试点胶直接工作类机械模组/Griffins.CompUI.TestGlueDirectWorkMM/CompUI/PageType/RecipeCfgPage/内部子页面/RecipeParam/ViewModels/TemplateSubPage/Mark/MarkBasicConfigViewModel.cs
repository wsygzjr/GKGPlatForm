using Griffins.UI;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Mark;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Mark
{
    /// <summary>
    /// Mark基础配置参数-视图模型
    /// </summary>
    public class MarkBasicConfigViewModel : ReactiveObject
    {
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        #region 视图模型组件（对应UI控件）
        /// <summary>
        /// Mark方式-下拉框数据模型
        /// </summary>
        public ComboxViewModel MarkModeModel { get; }

        /// <summary>
        /// 未找到mark时自动跳过-开关按钮数据模型
        /// </summary>
        public ToggleSwitchViewModel AutoSkipWhenNotFoundViewModel { get; }

        /// <summary>
        /// 是否分离-开关按钮数据模型
        /// </summary>
        public ToggleSwitchViewModel IsSeparatedViewModel { get; }

        /// <summary>
        /// 是否使用角度-开关按钮数据模型
        /// </summary>
        public ToggleSwitchViewModel IsUseAnglesViewModel { get; }
        /// <summary>
        /// 偏差值报警-数字输入框数据模型
        /// </summary>
        public NumericViewModel DeviationAlarmValueViewModel { get; }

        /// <summary>
        /// 是否为放置基准-开关按钮数据模型
        /// </summary>
        public ToggleSwitchViewModel IsPlacementReferenceViewModel { get; }

        /// <summary>
        /// 备份Mark数量-数字输入框数据模型
        /// </summary>
        public NumericViewModel BackupMarkCountViewModel { get; }

        /// <summary>
        /// Mark防抖次数-数字输入框数据模型
        /// </summary>
        public NumericViewModel MarkDebounceCountViewModel { get; }

        /// <summary>
        /// Mark失败后重试次数-数字输入框数据模型
        /// </summary>
        public NumericViewModel MarkRetryAfterFailureCountViewModel { get; }
        #endregion

        #region 响应式属性
        /// <summary>
        /// 选中的Mark方式
        /// </summary>
        public MarkMode SelectedMarkMode
        {
            get => (MarkMode)((MarkModeModel.SelectedItem as ComBoxItem)?.Value ?? MarkMode.PositionCompensation);
            set
            {
                if (MarkModeModel.ItemsSource != null)
                {
                    var targetItem = MarkModeModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (MarkMode)o.Value == value);
                    if (targetItem != null)
                        MarkModeModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(SelectedMarkMode));
                }
            }
        }

        /// <summary>
        /// 未找到mark时自动跳过
        /// </summary>
        public bool AutoSkipWhenNotFound
        {
            get => AutoSkipWhenNotFoundViewModel.IsChecked;
            set => AutoSkipWhenNotFoundViewModel.IsChecked = value;
        }

        /// <summary>
        /// 是否分离
        /// </summary>
        public bool IsSeparated
        {
            get => IsSeparatedViewModel.IsChecked;
            set => IsSeparatedViewModel.IsChecked = value;
        }
        /// <summary>
        /// 是否使用角度
        /// </summary>
        public bool IsUseAngles
        {
            get => IsUseAnglesViewModel.IsChecked;
            set => IsUseAnglesViewModel.IsChecked = value;
        }
        /// <summary>
        /// 偏差值报警
        /// </summary>
        public decimal DeviationAlarmValue
        {
            get => (decimal)DeviationAlarmValueViewModel.Value;
            set => DeviationAlarmValueViewModel.Value = value;
        }

        /// <summary>
        /// 是否为放置基准
        /// </summary>
        public bool IsPlacementReference
        {
            get => IsPlacementReferenceViewModel.IsChecked;
            set => IsPlacementReferenceViewModel.IsChecked = value;
        }

        /// <summary>
        /// 备份Mark数量
        /// </summary>
        public int BackupMarkCount
        {
            get => (int)BackupMarkCountViewModel.Value;
            set => BackupMarkCountViewModel.Value = value;
        }

        /// <summary>
        /// Mark防抖次数
        /// </summary>
        public int MarkDebounceCount
        {
            get => (int)MarkDebounceCountViewModel.Value;
            set => MarkDebounceCountViewModel.Value = value;
        }

        /// <summary>
        /// Mark失败后重试次数
        /// </summary>
        public int MarkRetryAfterFailureCount
        {
            get => (int)MarkRetryAfterFailureCountViewModel.Value;
            set => MarkRetryAfterFailureCountViewModel.Value = value;
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public MarkBasicConfigViewModel()
        {
            // 初始化所有视图模型组件
            MarkModeModel = new ComboxViewModel();
            AutoSkipWhenNotFoundViewModel = new ToggleSwitchViewModel { IsChecked = false };
            IsSeparatedViewModel = new ToggleSwitchViewModel { IsChecked = false };
            IsUseAnglesViewModel = new ToggleSwitchViewModel { IsChecked = false };
            DeviationAlarmValueViewModel = new NumericViewModel
            {
                Minimum = 0,
                Maximum = 100,
                DecimalPlaces = 2,
                Value = 0
            };
            IsPlacementReferenceViewModel = new ToggleSwitchViewModel { IsChecked = false };
            BackupMarkCountViewModel = new NumericViewModel
            {
                Minimum = 0,
                Maximum = 10,
                DecimalPlaces = 0,
                Value = 0
            };
            MarkDebounceCountViewModel = new NumericViewModel
            {
                Minimum = 0,
                Maximum = 20,
                DecimalPlaces = 0,
                Value = 3
            };
            MarkRetryAfterFailureCountViewModel = new NumericViewModel
            {
                Minimum = 0,
                Maximum = 10,
                DecimalPlaces = 0,
                Value = 2
            };

            // 初始化下拉框数据源
            initDataSources();
            // 订阅值变更事件
            subscribeValueChanges();
        }

      

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="markBaseConfigInfo"></param>
        public void CopyFrom(MarkBaseConfigInfo markBaseConfigInfo)
        {
            if (markBaseConfigInfo == null)
            {
                resetToDefault(); 
                return;
            }

            SelectedMarkMode = markBaseConfigInfo.MarkMode;
            AutoSkipWhenNotFound = markBaseConfigInfo.AutoSkipWhenNotFound;
            IsSeparated = markBaseConfigInfo.IsSeparated;
            DeviationAlarmValue = markBaseConfigInfo.DeviationAlarmValue;
            IsPlacementReference = markBaseConfigInfo.IsPlacementReference;
            BackupMarkCount = markBaseConfigInfo.BackupMarkCount;
            MarkDebounceCount = markBaseConfigInfo.MarkdebounceCount; 
            MarkRetryAfterFailureCount = markBaseConfigInfo.MarkRetryAfterFailureCount;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="markBaseConfigInfo"></param>
        public void CopyTo(MarkBaseConfigInfo markBaseConfigInfo)
        {
            // 映射所有字段到数据模型
            markBaseConfigInfo.MarkMode = SelectedMarkMode;
            markBaseConfigInfo.AutoSkipWhenNotFound = AutoSkipWhenNotFound;
            markBaseConfigInfo.IsSeparated = IsSeparated;
            markBaseConfigInfo.DeviationAlarmValue = DeviationAlarmValue;
            markBaseConfigInfo.IsPlacementReference = IsPlacementReference;
            markBaseConfigInfo.BackupMarkCount = BackupMarkCount;
            markBaseConfigInfo.MarkdebounceCount = MarkDebounceCount;
            markBaseConfigInfo.MarkRetryAfterFailureCount = MarkRetryAfterFailureCount;
        }
        /// <summary>
        /// 初始化下拉框数据源
        /// </summary>
        private void initDataSources()
        {
            // Mark方式数据源
            var markModeDisplayNames = new Dictionary<MarkMode, string>
            {
                { MarkMode.PositionCompensation, "定位补偿" },
                { MarkMode.TemplateBenchmark, "模板基准" }
            };
            MarkModeModel.ItemsSource = EnumExtensions.ToEnumItems<MarkMode>(markModeDisplayNames);
            MarkModeModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
        }

        /// <summary>
        /// 选中的Mark方式改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onMarkModeModel_ValueChanged(object? sender, ValueChangedEventArgs e)
        {
            //通知UI更新
            this.RaisePropertyChanged(nameof(SelectedMarkMode));
        }
        /// <summary>
        /// 重置为默认值
        /// </summary>
        private void resetToDefault()
        {
            SelectedMarkMode = MarkMode.PositionCompensation;
            AutoSkipWhenNotFound = false;
            IsSeparated = false;
            DeviationAlarmValue = 0;
            IsPlacementReference = false;
            BackupMarkCount = 0;
            MarkDebounceCount = 3;
            MarkRetryAfterFailureCount = 2; 
        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            
            MarkModeModel.ValueChanged += onValueChanged;
            MarkModeModel.ValueChanged += onMarkModeModel_ValueChanged;

            AutoSkipWhenNotFoundViewModel.ValueChanged += onValueChanged;
            IsSeparatedViewModel.ValueChanged += onValueChanged;
            IsUseAnglesViewModel.ValueChanged += onValueChanged;
            DeviationAlarmValueViewModel.ValueChanged += onValueChanged;
            IsPlacementReferenceViewModel.ValueChanged += onValueChanged;
            BackupMarkCountViewModel.ValueChanged += onValueChanged;
            MarkDebounceCountViewModel.ValueChanged += onValueChanged;
            MarkRetryAfterFailureCountViewModel.ValueChanged += onValueChanged;
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