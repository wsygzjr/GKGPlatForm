using GKG;
using GKG.SubMM;
using GKG.UI;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Griffins.CompUI.MeasureHeightFunctionHead.CompUI_MeasureHeightFunctionHead.PageType.FactoryCfgPage.MeasureHeightFactory.ViewModels
{
    /// <summary>
    /// 测高工厂配置视图模型
    /// </summary>
    internal class MeasureHeightFactoryCompUIViewModel : ReactiveObject
    {
        private bool readOnly;
        private MeasureHeightType measureHeightType;

        /// <summary>
        /// 测高类型下拉框视图模型
        /// </summary>
        public ComboxViewModel MeasureHeightTypeViewModel { get; } = CreateComboViewModel();

        /// <summary>
        /// 是否只读
        /// </summary>
        public bool ReadOnly
        {
            get => readOnly;
            set
            {
                this.RaiseAndSetIfChanged(ref readOnly, value);
                var isEnabled = !readOnly;
                MeasureHeightTypeViewModel.IsEnabled = isEnabled;
            }
        }

        /// <summary>
        /// 测高类型
        /// </summary>
        public MeasureHeightType MeasureHeightType
        {
            get => measureHeightType;
            set
            {
                this.RaiseAndSetIfChanged(ref measureHeightType, value);
                UpdateSelectedMeasureHeightType(value);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public MeasureHeightFactoryCompUIViewModel()
        {
            InitializeMeasureHeightTypeOptions();
            MeasureHeightTypeViewModel.ValueChanged += (_, __) =>
            {
                var selectedItem = MeasureHeightTypeViewModel.SelectedItem as ComBoxItem;
                if (selectedItem != null && selectedItem.Value is MeasureHeightType type)
                {
                    measureHeightType = type;
                    this.RaisePropertyChanged(nameof(MeasureHeightType));
                }
            };
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="data">数据</param>
        public void SetData(MeasureHeightFunctionHeadSubMachineModulesFactoryCfg data)
        {
            var cfg = data ?? new MeasureHeightFunctionHeadSubMachineModulesFactoryCfg();
            MeasureHeightType = cfg.MeasureHeightType;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns>数据</returns>
        public MeasureHeightFunctionHeadSubMachineModulesFactoryCfg GetData()
        {
            var cfg = new MeasureHeightFunctionHeadSubMachineModulesFactoryCfg
            {
                MeasureHeightType = this.MeasureHeightType,
            };
            return cfg;
        }

        /// <summary>
        /// 初始化测高类型选项
        /// </summary>
        private void InitializeMeasureHeightTypeOptions()
        {
            var items = MeasureHeightTypeViewModel.ItemsSource as ObservableCollection<ComBoxItem>;
            items?.Clear();

            foreach (MeasureHeightType type in Enum.GetValues(typeof(MeasureHeightType)))
            {
                items?.Add(new ComBoxItem
                {
                    Value = type,
                    DisplayName = GetMeasureHeightTypeDisplayName(type),
                });
            }
        }

        /// <summary>
        /// 更新选中的测高类型
        /// </summary>
        /// <param name="type">测高类型</param>
        private void UpdateSelectedMeasureHeightType(MeasureHeightType type)
        {
            var items = MeasureHeightTypeViewModel.ItemsSource as ObservableCollection<ComBoxItem>;
            if (items == null) return;

            var selectedItem = items.FirstOrDefault(item => item.Value is MeasureHeightType t && t == type);
            if (selectedItem != null)
            {
                MeasureHeightTypeViewModel.SelectedItem = selectedItem;
            }
        }

        /// <summary>
        /// 获取测高类型显示名称
        /// </summary>
        /// <param name="type">测高类型</param>
        /// <returns>显示名称</returns>
        private string GetMeasureHeightTypeDisplayName(MeasureHeightType type)
        {
            return type switch
            {
                MeasureHeightType.SSZNSD33 => "深视智能SD33",
                _ => type.ToString(),
            };
        }

        /// <summary>
        /// 创建下拉框视图模型
        /// </summary>
        /// <returns>下拉框视图模型</returns>
        private static ComboxViewModel CreateComboViewModel()
        {
            return new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                ItemsSource = new ObservableCollection<ComBoxItem>(),
            };
        }
    }

    /// <summary>
    /// 下拉框项
    /// </summary>
    internal class ComBoxItem
    {
        /// <summary>
        /// 值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }
    }
}
