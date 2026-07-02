using GF_Gereric;
using GKG;
using GKG.ElectronicControl.General;

using GKG.SubMM;
using GKG.UI;
using Griffins.Map.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Griffins.CompUI.MeasureHeightFunctionHead.CompUI_MeasureHeightFunctionHead.PageType.InitCfgPage.MeasureHeightInit.ViewModels
{
    /// <summary>
    /// 测高初始化配置视图模型
    /// </summary>
    internal class MeasureHeightInitCompUIViewModel : ReactiveObject, IDisposable
    {
        private readonly bool isDesign;
        private readonly ICompUIRunTimeCallBack callBack;
        private MeasureHeightFunctionHeadSubMachineModulesInitCfg loadedData = new();
        private bool readOnly;
        private bool isApplyingData;

        /// <summary>
        /// 测高范围下拉框视图模型
        /// </summary>
        public ComboxViewModel MeasureRangeViewModel { get; }

        /// <summary>
        /// 串口配置视图模型
        /// </summary>
        public SerialPortViewModel SerialPortViewModel { get; }

        /// <summary>
        /// 修改后事件
        /// </summary>
        public event EventHandler AfterModified;

        /// <summary>
        /// 是否只读
        /// </summary>
        public bool ReadOnly
        {
            get => readOnly;
            set
            {
                this.RaiseAndSetIfChanged(ref readOnly, value);
                UpdateFieldEnabledState();
            }
        }

        /// <summary>
        /// 构造函数（设计时）
        /// </summary>
        public MeasureHeightInitCompUIViewModel()
        {
            isDesign = true;
            MeasureRangeViewModel = CreateComboViewModel();
            SerialPortViewModel = new SerialPortViewModel();
            InitializeMeasureRangeOptions();
            SubscribeValueChanges();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isDesign">是否设计模式</param>
        /// <param name="callBack">回调</param>
        public MeasureHeightInitCompUIViewModel(bool isDesign, ICompUIRunTimeCallBack callBack)
        {
            this.isDesign = isDesign;
            this.callBack = callBack;
            MeasureRangeViewModel = CreateComboViewModel();
            SerialPortViewModel = new SerialPortViewModel();
            InitializeMeasureRangeOptions();
            UpdateFieldEnabledState();
            SubscribeValueChanges();
            ReadOnly = false;
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="data">数据</param>
        public void SetData(MeasureHeightFunctionHeadSubMachineModulesInitCfg data)
        {
            ApplyWithoutModified(() =>
            {
                loadedData = CloneData(data);

                // 解析测高初始化参数
                if (loadedData.MeasureHeightInitParams != null && loadedData.MeasureHeightInitParams.Length > 0)
                {
                    try
                    {
                        var initParam = JsonObjConvert.FromJSonBytes<InitParamSSZNSD33>(loadedData.MeasureHeightInitParams);
                        if (initParam != null)
                        {
                            UpdateSelectedMeasureRange(initParam.MeasureRangeInitParam);

                            // 解析串口参数
                            if (initParam.CommunicatorInitParam != null && initParam.CommunicatorInitParam.Length > 0)
                            {
                                var serialCfg = JsonObjConvert.FromJSonBytes<SerialConfig>(initParam.CommunicatorInitParam);
                                SerialPortViewModel.CopyFrom(serialCfg);
                            }
                        }
                    }
                    catch
                    {
                        // 解析失败时使用默认值
                    }
                }
            });
            UpdateFieldEnabledState();
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns>数据</returns>
        public MeasureHeightFunctionHeadSubMachineModulesInitCfg GetData()
        {
            var result = CloneData(loadedData) ?? new MeasureHeightFunctionHeadSubMachineModulesInitCfg();

            // 构建测高初始化参数
            var initParam = new InitParamSSZNSD33();

            // 设置测高范围
            var selectedRangeItem = MeasureRangeViewModel.SelectedItem as ComBoxItem;
            if (selectedRangeItem != null && selectedRangeItem.Value is MeasureRange range)
            {
                initParam.MeasureRangeInitParam = range;
            }

            // 设置串口参数
            var serialCfg = new SerialConfig();
            SerialPortViewModel.CopyTo(ref serialCfg);
            initParam.CommunicatorInitParam = JsonObjConvert.ToJSonBytes(serialCfg);

            // 序列化为字节数组
            result.MeasureHeightInitParams = JsonObjConvert.ToJSonBytes(initParam);

            loadedData = CloneData(result);
            return result;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// 订阅值变化事件
        /// </summary>
        private void SubscribeValueChanges()
        {
            MeasureRangeViewModel.ValueChanged += (_, __) => NotifyDataModified();
            SerialPortViewModel.AfterModified += (_, __) => NotifyDataModified();
        }

        /// <summary>
        /// 更新字段启用状态
        /// </summary>
        private void UpdateFieldEnabledState()
        {
            var editable = !ReadOnly;
            MeasureRangeViewModel.IsEnabled = editable;
            SerialPortViewModel.PortNameViewModel.IsEnabled = editable;
            SerialPortViewModel.BaudRateViewModel.IsEnabled = editable;
            SerialPortViewModel.DataBitsViewModel.IsEnabled = editable;
            SerialPortViewModel.StopBitsViewModel.IsEnabled = editable;
            SerialPortViewModel.ParityViewModel.IsEnabled = editable;
            SerialPortViewModel.EnableCRC16ViewModel.IsEnabled = editable;
            SerialPortViewModel.ModbusTypeViewModel.IsEnabled = editable;
        }

        /// <summary>
        /// 通知数据修改
        /// </summary>
        private void NotifyDataModified()
        {
            if (isApplyingData)
            {
                return;
            }

            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 应用操作而不触发修改事件
        /// </summary>
        /// <param name="action">操作</param>
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
        /// 初始化测高范围选项
        /// </summary>
        private void InitializeMeasureRangeOptions()
        {
            var items = MeasureRangeViewModel.ItemsSource as ObservableCollection<ComBoxItem>;
            items?.Clear();

            foreach (MeasureRange range in Enum.GetValues(typeof(MeasureRange)))
            {
                items?.Add(new ComBoxItem
                {
                    Value = range,
                    DisplayName = GetMeasureRangeDisplayName(range)
                });
            }

            // 默认选中第一个
            if (items?.Count > 0)
            {
                MeasureRangeViewModel.SelectedItem = items[0];
            }
        }

        /// <summary>
        /// 更新选中的测高范围
        /// </summary>
        /// <param name="range">测高范围</param>
        private void UpdateSelectedMeasureRange(MeasureRange range)
        {
            var items = MeasureRangeViewModel.ItemsSource as ObservableCollection<ComBoxItem>;
            if (items == null) return;

            var selectedItem = items.FirstOrDefault(item => item.Value is MeasureRange r && r == range);
            if (selectedItem != null)
            {
                MeasureRangeViewModel.SelectedItem = selectedItem;
            }
        }

        /// <summary>
        /// 获取测高范围显示名称
        /// </summary>
        /// <param name="range">测高范围</param>
        /// <returns>显示名称</returns>
        private string GetMeasureRangeDisplayName(MeasureRange range)
        {
            return range switch
            {
                MeasureRange.Range30 => "30mm",
                MeasureRange.Range50 => "50mm",
                _ => range.ToString()
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
                ItemsSource = new ObservableCollection<ComBoxItem>()
            };
        }

        /// <summary>
        /// 克隆数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>克隆的数据</returns>
        private static MeasureHeightFunctionHeadSubMachineModulesInitCfg CloneData(MeasureHeightFunctionHeadSubMachineModulesInitCfg data)
        {
            if (data == null)
            {
                return new MeasureHeightFunctionHeadSubMachineModulesInitCfg();
            }

            var clone = new MeasureHeightFunctionHeadSubMachineModulesInitCfg();
            clone.CopyFrom(data);
            return clone;
        }
    }
}
