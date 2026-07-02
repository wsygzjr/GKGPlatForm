using GKG.ElectronicControl.Dispenser;
using GKG.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.PPPage.DispensingFunctionHeadPP.ViewModels
{
    /// <summary>
    /// 胶量感应参数ViewModel
    /// 包含时间、重量、板数三种检测方式
    /// </summary>
    public class GlueAmountParamsViewModel : ReactiveObject
    {
        /// <summary>修改后事件</summary>
        public event EventHandler AfterModified;

        #region 时间参数

        public ToggleSwitchViewModel TimeEnableViewModel { get; }
        public TextInputViewModel TotalTimeViewModel { get; }
        public TextInputViewModel RemainingTimeViewModel { get; }

        #endregion

        #region 重量参数

        public ToggleSwitchViewModel WeightEnableViewModel { get; }
        public TextInputViewModel TotalWeightViewModel { get; }
        public TextInputViewModel RemainingWeightViewModel { get; }
        public ComboxViewModel WeightAlarmTypeViewModel { get; }
        public TextInputViewModel WeightAlarmValueViewModel { get; }

        #endregion

        #region 板数参数

        public ToggleSwitchViewModel PcsEnableViewModel { get; }
        public TextInputViewModel TotalPcsViewModel { get; }
        public TextInputViewModel RemainingPcsViewModel { get; }

        #endregion

        public GlueAmountParamsViewModel()
        {
            // 初始化时间参数ViewModels
            TimeEnableViewModel = new ToggleSwitchViewModel();
            TotalTimeViewModel = new TextInputViewModel { Text = "0" };
            RemainingTimeViewModel = new TextInputViewModel { Text = "0" };

            // 初始化重量参数ViewModels
            WeightEnableViewModel = new ToggleSwitchViewModel();
            TotalWeightViewModel = new TextInputViewModel { Text = "0" };
            RemainingWeightViewModel = new TextInputViewModel { Text = "0" };
            WeightAlarmTypeViewModel = new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                ItemsSource = new List<ComBoxItem>
                {
                    new ComBoxItem { Value = GlueAmountWeightAlarmType.Percent, DisplayName = "百分比" },
                    new ComBoxItem { Value = GlueAmountWeightAlarmType.Weight, DisplayName = "重量" }
                }
            };
            WeightAlarmValueViewModel = new TextInputViewModel { Text = "0" };

            // 初始化板数参数ViewModels
            PcsEnableViewModel = new ToggleSwitchViewModel();
            TotalPcsViewModel = new TextInputViewModel { Text = "0" };
            RemainingPcsViewModel = new TextInputViewModel { Text = "0" };

            // 设置默认选中项
            WeightAlarmTypeViewModel.SelectedItem = (WeightAlarmTypeViewModel.ItemsSource as List<ComBoxItem>)?[0];

            // 订阅值变化事件
            SubscribeToChangeEvents();
        }

        private void SubscribeToChangeEvents()
        {
            TimeEnableViewModel.ValueChanged += OnValueChanged;
            TotalTimeViewModel.ValueChanged += OnValueChanged;
            RemainingTimeViewModel.ValueChanged += OnValueChanged;

            WeightEnableViewModel.ValueChanged += OnValueChanged;
            TotalWeightViewModel.ValueChanged += OnValueChanged;
            RemainingWeightViewModel.ValueChanged += OnValueChanged;
            WeightAlarmTypeViewModel.ValueChanged += OnValueChanged;
            WeightAlarmValueViewModel.ValueChanged += OnValueChanged;

            PcsEnableViewModel.ValueChanged += OnValueChanged;
            TotalPcsViewModel.ValueChanged += OnValueChanged;
            RemainingPcsViewModel.ValueChanged += OnValueChanged;
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }

        /// <summary>
        /// 从数据模型加载
        /// </summary>
        public void CopyFrom(GlueAmountParams source)
        {
            if (source == null)
            {
                source = new GlueAmountParams();
            }

            // 加载时间参数
            TimeEnableViewModel.IsChecked = source.GlueAmountTimeParams?.Enable ?? false;
            TotalTimeViewModel.Text = (source.GlueAmountTimeParams?.TotalTime ?? 0).ToString(CultureInfo.InvariantCulture);
            RemainingTimeViewModel.Text = (source.GlueAmountTimeParams?.RemainingTime ?? 0).ToString(CultureInfo.InvariantCulture);

            // 加载重量参数
            WeightEnableViewModel.IsChecked = source.GlueAmountWeightParams?.Enable ?? false;
            TotalWeightViewModel.Text = (source.GlueAmountWeightParams?.TotalWeight ?? 0).ToString(CultureInfo.InvariantCulture);
            RemainingWeightViewModel.Text = (source.GlueAmountWeightParams?.RemainingWeight ?? 0).ToString(CultureInfo.InvariantCulture);
            
            var alarmType = source.GlueAmountWeightParams?.glueAmountWeightAlarm?.WeightAlarmType ?? GlueAmountWeightAlarmType.Percent;
            var items = WeightAlarmTypeViewModel.ItemsSource as List<ComBoxItem>;
            var selectedItem = items?.FirstOrDefault(item => item.Value is GlueAmountWeightAlarmType type && type == alarmType);
            if (selectedItem != null)
            {
                WeightAlarmTypeViewModel.SelectedItem = selectedItem;
            }
            
            WeightAlarmValueViewModel.Text = (source.GlueAmountWeightParams?.glueAmountWeightAlarm?.GlueAmountWeightAlarmValue ?? 0).ToString(CultureInfo.InvariantCulture);

            // 加载板数参数
            PcsEnableViewModel.IsChecked = source.GlueAmountPcsParams?.Enable ?? false;
            TotalPcsViewModel.Text = (source.GlueAmountPcsParams?.TotalPcs ?? 0).ToString(CultureInfo.InvariantCulture);
            RemainingPcsViewModel.Text = (source.GlueAmountPcsParams?.RemainingPcs ?? 0).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 复制到数据模型
        /// </summary>
        public GlueAmountParams ToModel()
        {
            var alarmTypeItem = WeightAlarmTypeViewModel.SelectedItem as ComBoxItem;
            var alarmType = alarmTypeItem?.Value is GlueAmountWeightAlarmType type 
                ? type 
                : GlueAmountWeightAlarmType.Percent;

            return new GlueAmountParams
            {
                GlueAmountTimeParams = new GlueAmountTime
                {
                    Enable = TimeEnableViewModel.IsChecked,
                    TotalTime = ParseDoubleOrDefault(TotalTimeViewModel.Text, 0),
                    RemainingTime = ParseDoubleOrDefault(RemainingTimeViewModel.Text, 0)
                },
                GlueAmountWeightParams = new GlueAmountWeight
                {
                    Enable = WeightEnableViewModel.IsChecked,
                    TotalWeight = ParseDoubleOrDefault(TotalWeightViewModel.Text, 0),
                    RemainingWeight = ParseDoubleOrDefault(RemainingWeightViewModel.Text, 0),
                    glueAmountWeightAlarm = new GlueAmountWeightAlarm
                    {
                        WeightAlarmType = alarmType,
                        GlueAmountWeightAlarmValue = ParseDoubleOrDefault(WeightAlarmValueViewModel.Text, 0)
                    }
                },
                GlueAmountPcsParams = new GlueAmountPcs
                {
                    Enable = PcsEnableViewModel.IsChecked,
                    TotalPcs = ParseIntOrDefault(TotalPcsViewModel.Text, 0),
                    RemainingPcs = ParseIntOrDefault(RemainingPcsViewModel.Text, 0)
                }
            };
        }

        public void UpdateEnabledState(bool enabled)
        {
            TimeEnableViewModel.IsEnabled = enabled;
            TotalTimeViewModel.IsEnabled = enabled;
            RemainingTimeViewModel.IsEnabled = enabled;

            WeightEnableViewModel.IsEnabled = enabled;
            TotalWeightViewModel.IsEnabled = enabled;
            RemainingWeightViewModel.IsEnabled = enabled;
            WeightAlarmTypeViewModel.IsEnabled = enabled;
            WeightAlarmValueViewModel.IsEnabled = enabled;

            PcsEnableViewModel.IsEnabled = enabled;
            TotalPcsViewModel.IsEnabled = enabled;
            RemainingPcsViewModel.IsEnabled = enabled;
        }

        private static double ParseDoubleOrDefault(string text, double defaultValue)
        {
            return double.TryParse(
                text,
                NumberStyles.Float | NumberStyles.AllowThousands,
                CultureInfo.InvariantCulture,
                out var value)
                ? value
                : defaultValue;
        }

        private static int ParseIntOrDefault(string text, int defaultValue)
        {
            return int.TryParse(
                text,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out var value)
                ? value
                : defaultValue;
        }
    }
}
