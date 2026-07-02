using GF_Gereric;
using GKG.ElectronicControl.Dispenser;
using GKG.SubMM.Dispenser;
using GKG.UI;
using Griffins.Map.UI;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.FactoryCfgPage.DispensingFunctionHeadFactory.ViewModels
{
    /// <summary>
    /// 点胶机功能头出厂配置页面ViewModel
    /// 负责管理阀类型和供胶装置类型的选择
    /// 实现MVVM模式的数据绑定和业务逻辑
    /// </summary>
    internal class DispensingFunctionHeadFactoryCompUIViewModel : ReactiveObject
    {
        /// <summary>
        /// 数据修改后触发的事件
        /// 用于通知父级组件数据已发生变化
        /// </summary>
        public event EventHandler AfterModified;

        private object _viewTag;
        /// <summary>
        /// 视图标签，用于视图和ViewModel之间的关联
        /// </summary>
        public object ViewTag
        {
            get => _viewTag;
            set => this.RaiseAndSetIfChanged(ref _viewTag, value);
        }

        private bool _readOnly;
        /// <summary>
        /// 只读模式标志
        /// 当为true时，禁用所有控件的编辑功能
        /// </summary>
        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                this.RaiseAndSetIfChanged(ref _readOnly, value);
                // 更新控件的启用状态
                ValveTypeViewModel.IsEnabled = !value;
                GlueDispensingDeviceTypeViewModel.IsEnabled = !value;
            }
        }

        /// <summary>
        /// 阀类型选择下拉框ViewModel
        /// </summary>
        public ComboxViewModel ValveTypeViewModel { get; }

        /// <summary>
        /// 供胶装置类型选择下拉框ViewModel
        /// </summary>
        public ComboxViewModel GlueDispensingDeviceTypeViewModel { get; }

        /// <summary>
        /// 出厂配置数据对象
        /// </summary>
        private DispensingFunctionHeadSubMachineModulesFactoryCfg _data = new();

        /// <summary>
        /// 构造函数
        /// 初始化所有ViewModel和下拉框选项
        /// </summary>
        /// <param name="callBack">CompUI运行时回调接口，用于与后端通信</param>
        public DispensingFunctionHeadFactoryCompUIViewModel(ICompUIRunTimeCallBack callBack)
        {
            // 创建下拉框ViewModel
            ValveTypeViewModel = CreateComboViewModel();
            GlueDispensingDeviceTypeViewModel = CreateComboViewModel();

            // 初始化下拉框选项
            InitializeValveTypeOptions();
            InitializeGlueDispensingDeviceTypeOptions();

            // 订阅值变更事件
            ValveTypeViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            GlueDispensingDeviceTypeViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);

            ReadOnly = false;
        }

        /// <summary>
        /// 设置页面数据
        /// 从后端加载的配置数据填充到UI控件
        /// </summary>
        /// <param name="data">出厂配置数据</param>
        public void SetData(DispensingFunctionHeadSubMachineModulesFactoryCfg data)
        {
            _data = CreateDataCopy(data);
            
            SetSelectedValveType(_data.ValveType);
            SetSelectedGlueDispensingDeviceType(_data.GlueDispensingDeviceType);
        }

        /// <summary>
        /// 获取页面数据
        /// 从UI控件收集数据并返回给后端
        /// </summary>
        /// <returns>出厂配置数据对象</returns>
        public DispensingFunctionHeadSubMachineModulesFactoryCfg GetData()
        {
            // 确保返回的对象不为null
            var result = new DispensingFunctionHeadSubMachineModulesFactoryCfg
            {
                ValveType = GetSelectedValveType(),
                GlueDispensingDeviceType = GetSelectedGlueDispensingDeviceType()
            };

            return result;
        }

        /// <summary>
        /// 创建下拉框ViewModel
        /// 设置显示成员路径和数据源
        /// </summary>
        /// <returns>配置好的下拉框ViewModel</returns>
        private static ComboxViewModel CreateComboViewModel()
        {
            return new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                ItemsSource = new ObservableCollection<ComBoxItem>(),
            };
        }

        /// <summary>
        /// 初始化阀类型下拉框选项
        /// 添加所有支持的阀类型
        /// </summary>
        private void InitializeValveTypeOptions()
        {
            var items = ValveTypeViewModel.ItemsSource as ObservableCollection<ComBoxItem>;
            items?.Clear();

            items?.Add(new ComBoxItem
            {
                Value = ValveType.GKGPiezoValve,
                DisplayName = "GKG压电阀"
            });

            if (items?.Count > 0)
                ValveTypeViewModel.SelectedItem = items[0];
        }

        /// <summary>
        /// 初始化供胶装置类型下拉框选项
        /// 添加所有支持的供胶装置类型
        /// </summary>
        private void InitializeGlueDispensingDeviceTypeOptions()
        {
            var items = GlueDispensingDeviceTypeViewModel.ItemsSource as ObservableCollection<ComBoxItem>;
            items?.Clear();

            items?.Add(new ComBoxItem
            {
                Value = GlueDispensingDeviceType.GKGGlueDispensingDevice,
                DisplayName = "GKG供胶装置"
            });

            if (items?.Count > 0)
                GlueDispensingDeviceTypeViewModel.SelectedItem = items[0];
        }

        /// <summary>
        /// 设置选中的阀类型
        /// 根据枚举值在下拉框中选中对应项
        /// </summary>
        /// <param name="valveType">阀类型枚举值</param>
        private void SetSelectedValveType(ValveType valveType)
        {
            var items = ValveTypeViewModel.ItemsSource as ObservableCollection<ComBoxItem>;
            var target = items?.FirstOrDefault(item => item.Value is ValveType type && type == valveType);
            if (target != null)
                ValveTypeViewModel.SelectedItem = target;
        }

        /// <summary>
        /// 设置选中的供胶装置类型
        /// 根据枚举值在下拉框中选中对应项
        /// </summary>
        /// <param name="deviceType">供胶装置类型枚举值</param>
        private void SetSelectedGlueDispensingDeviceType(GlueDispensingDeviceType deviceType)
        {
            var items = GlueDispensingDeviceTypeViewModel.ItemsSource as ObservableCollection<ComBoxItem>;
            var target = items?.FirstOrDefault(item => item.Value is GlueDispensingDeviceType type && type == deviceType);
            if (target != null)
                GlueDispensingDeviceTypeViewModel.SelectedItem = target;
        }

        /// <summary>
        /// 获取选中的阀类型
        /// 从下拉框中获取当前选中的枚举值
        /// </summary>
        /// <returns>阀类型枚举值，默认为GKGPiezoValve</returns>
        private ValveType GetSelectedValveType()
        {
            var selected = ValveTypeViewModel.SelectedItem as ComBoxItem;
            return selected?.Value is ValveType type ? type : ValveType.GKGPiezoValve;
        }

        /// <summary>
        /// 获取选中的供胶装置类型
        /// 从下拉框中获取当前选中的枚举值
        /// </summary>
        /// <returns>供胶装置类型枚举值，默认为GKGGlueDispensingDevice</returns>
        private GlueDispensingDeviceType GetSelectedGlueDispensingDeviceType()
        {
            var selected = GlueDispensingDeviceTypeViewModel.SelectedItem as ComBoxItem;
            return selected?.Value is GlueDispensingDeviceType type ? type : GlueDispensingDeviceType.GKGGlueDispensingDevice;
        }

        /// <summary>
        /// 克隆数据对象
        /// 使用JSON序列化实现深拷贝，避免引用传递导致的数据污染
        /// </summary>
        /// <param name="data">要克隆的数据对象</param>
        /// <returns>克隆后的新对象</returns>
        private static DispensingFunctionHeadSubMachineModulesFactoryCfg CreateDataCopy(DispensingFunctionHeadSubMachineModulesFactoryCfg data)
        {
            if (data == null)
            {
                return new DispensingFunctionHeadSubMachineModulesFactoryCfg();
            }

            return new DispensingFunctionHeadSubMachineModulesFactoryCfg
            {
                ValveType = data.ValveType,
                GlueDispensingDeviceType = data.GlueDispensingDeviceType
            };
        }
    }
}
