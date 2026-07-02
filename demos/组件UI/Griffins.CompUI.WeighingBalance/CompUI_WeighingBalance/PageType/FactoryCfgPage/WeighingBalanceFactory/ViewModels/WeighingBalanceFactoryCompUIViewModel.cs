using GKG;
using GKG.SubMM.Dispenser;
using GKG.UI;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;

namespace Griffins.CompUI.WeighingBalance.CompUI_WeighingBalance.PageType.FactoryCfgPage.WeighingBalanceFactory.ViewModels
{
    /// <summary>
    /// 称重出厂配置视图模型。
    /// </summary>
    public class WeighingBalanceFactoryCompUIViewModel : ReactiveObject
    {
        private bool readOnly;
        private bool isApplyingData;

        /// <summary>
        /// 修改后事件
        /// </summary>
        public event EventHandler AfterModified;

        /// <summary>
        /// 天平型号下拉项集合
        /// </summary>
        public ObservableCollection<ComBoxItem> WeighingBalanceTypeItems { get; } = new();

        /// <summary>
        /// 天平型号控件模型，页面通过 GKG.UI 的 ComboxControl 绑定它。
        /// </summary>
        public ComboxViewModel WeighingBalanceTypeViewModel { get; } = new()
        {
            DisplayMemberPath = nameof(ComBoxItem.DisplayName),
            PlaceholderText = string.Empty,
        };

        /// <summary>
        /// 是否只读
        /// </summary>
        public bool ReadOnly
        {
            get => readOnly;
            set
            {
                this.RaiseAndSetIfChanged(ref readOnly, value);
                this.RaisePropertyChanged(nameof(CanEdit));
            }
        }

        /// <summary>
        /// 是否允许编辑
        /// </summary>
        public bool CanEdit => !ReadOnly;

        /// <summary>
        /// 当前选中的天平型号
        /// </summary>
        public WeighingBalanceType SelectedWeighingBalanceType
        {
            get => GetSelectedValue(WeighingBalanceTypeViewModel, WeighingBalanceType.APW);
            set
            {
                SetSelectedValue(WeighingBalanceTypeViewModel, value);
                this.RaisePropertyChanged(nameof(SelectedWeighingBalanceType));
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WeighingBalanceFactoryCompUIViewModel()
        {
            // 重点：出厂页也使用 GKG.UI 控件模型，不再让 XAML 直接绑定原生 ComboBox。
            foreach (WeighingBalanceType type in Enum.GetValues(typeof(WeighingBalanceType)))
            {
                WeighingBalanceTypeItems.Add(CreateComboItem(type, type.ToString()));
            }

            WeighingBalanceTypeViewModel.ItemsSource = WeighingBalanceTypeItems;
            WeighingBalanceTypeViewModel.SetWidth("Medium");
            WeighingBalanceTypeViewModel.ValueChanged += OnControlValueChanged;
            SelectedWeighingBalanceType = WeighingBalanceType.APW;
        }

        /// <summary>
        /// 从后端出厂配置加载数据
        /// </summary>
        /// <param name="data">出厂配置</param>
        public void SetData(WeighingBalanceSubMachineModulesFactoryCfg data)
        {
            var cfg = data ?? new WeighingBalanceSubMachineModulesFactoryCfg();
            ApplyWithoutModified(() => SelectedWeighingBalanceType = cfg.WeighingBalanceType);
        }

        /// <summary>
        /// 输出保存到后端的出厂配置
        /// </summary>
        /// <returns>出厂配置</returns>
        public WeighingBalanceSubMachineModulesFactoryCfg GetData()
        {
            return new WeighingBalanceSubMachineModulesFactoryCfg
            {
                WeighingBalanceType = SelectedWeighingBalanceType,
            };
        }

        private void OnControlValueChanged(object sender, EventArgs e)
        {
            this.RaisePropertyChanged(nameof(SelectedWeighingBalanceType));
            NotifyModified();
        }

        /// <summary>
        /// 通知外层页面当前配置已被用户修改
        /// </summary>
        private void NotifyModified()
        {
            if (isApplyingData)
            {
                return;
            }

            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 应用后端数据时临时屏蔽修改事件，避免打开页面或刷新数据时被误判为用户编辑。
        /// </summary>
        /// <param name="action">需要静默执行的数据应用动作</param>
        private void ApplyWithoutModified(Action action)
        {
            isApplyingData = true;
            try
            {
                action();
            }
            finally
            {
                isApplyingData = false;
            }
        }

        /// <summary>
        /// 创建 GKG.UI 下拉项，Value 保留真实枚举值，DisplayName 只负责界面展示。
        /// </summary>
        /// <param name="value">真实值</param>
        /// <param name="displayName">显示文本</param>
        /// <returns>下拉项</returns>
        private static ComBoxItem CreateComboItem(object value, string displayName)
        {
            return new ComBoxItem { Value = value, DisplayName = displayName };
        }

        /// <summary>
        /// 从 GKG.UI 下拉控件中读取真实值，未选中时使用默认值保证保存数据稳定。
        /// </summary>
        /// <typeparam name="T">真实值类型</typeparam>
        /// <param name="viewModel">下拉控件模型</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>选中值或默认值</returns>
        private static T GetSelectedValue<T>(ComboxViewModel viewModel, T defaultValue)
        {
            return viewModel.SelectedItem is ComBoxItem item && item.Value is T value
                ? value
                : defaultValue;
        }

        /// <summary>
        /// 根据真实值反选 GKG.UI 下拉项，保证 SetData 后界面和后端配置一致。
        /// </summary>
        /// <typeparam name="T">真实值类型</typeparam>
        /// <param name="viewModel">下拉控件模型</param>
        /// <param name="value">需要选中的真实值</param>
        private static void SetSelectedValue<T>(ComboxViewModel viewModel, T value)
        {
            if (viewModel.ItemsSource == null)
            {
                return;
            }

            foreach (var item in viewModel.ItemsSource)
            {
                if (item is ComBoxItem comboItem && Equals(comboItem.Value, value))
                {
                    viewModel.SelectedItem = comboItem;
                    return;
                }
            }
        }
    }
}
