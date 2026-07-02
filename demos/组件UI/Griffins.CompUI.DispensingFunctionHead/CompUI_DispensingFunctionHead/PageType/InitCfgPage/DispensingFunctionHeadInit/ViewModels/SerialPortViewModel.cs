using Avalonia.Controls;
using GKG;
using GKG.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using RJCP.IO.Ports;

namespace Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.InitCfgPage.DispensingFunctionHeadInit.ViewModels
{
    /// <summary>
    /// 串口配置视图模型
    /// 封装串口参数的所有配置项，将端口号、波特率、数据位都改为下拉框选择
    /// 提供与SerialConfig数据结构的双向转换
    /// </summary>
    public class SerialPortViewModel : ReactiveObject
    {
        private Control? _viewReference;

        /// <summary>串口号下拉框视图模型</summary>
        public ComboxViewModel PortNameViewModel { get; }

        /// <summary>波特率下拉框视图模型</summary>
        public ComboxViewModel BaudRateViewModel { get; }

        /// <summary>数据位下拉框视图模型</summary>
        public ComboxViewModel DataBitsViewModel { get; }

        /// <summary>停止位下拉框视图模型</summary>
        public ComboxViewModel StopBitsViewModel { get; }

        /// <summary>校验位下拉框视图模型</summary>
        public ComboxViewModel ParityViewModel { get; }

        /// <summary>是否启用CRC16开关视图模型</summary>
        public ToggleSwitchViewModel EnableCRC16ViewModel { get; }

        /// <summary>Modbus类型下拉框视图模型</summary>
        public ComboxViewModel ModbusTypeViewModel { get; }

        /// <summary>修改后事件</summary>
        public event EventHandler? AfterModified;

        /// <summary>
        /// 构造函数
        /// 初始化所有下拉框和开关控件，设置默认值
        /// </summary>
        public SerialPortViewModel()
        {
            // 初始化串口号下拉框，动态读取本机可用串口
            PortNameViewModel = new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                ItemsSource = BuildPortNameItems()
            };

            // 初始化波特率下拉框，提供常用波特率列表
            BaudRateViewModel = new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                ItemsSource = BuildBaudRateItems()
            };

            // 初始化数据位下拉框，仅允许 5/6/7/8
            DataBitsViewModel = new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                ItemsSource = BuildDataBitsItems()
            };

            // 初始化停止位下拉框
            StopBitsViewModel = new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                ItemsSource = new List<ComBoxItem>
                {
                    new ComBoxItem { Value = RJCP.IO.Ports.StopBits.One, DisplayName = "1" },
                    new ComBoxItem { Value = RJCP.IO.Ports.StopBits.One5, DisplayName = "1.5" },
                    new ComBoxItem { Value = RJCP.IO.Ports.StopBits.Two, DisplayName = "2" }
                }
            };

            // 初始化校验位下拉框
            ParityViewModel = new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                ItemsSource = new List<ComBoxItem>
                {
                    new ComBoxItem { Value = RJCP.IO.Ports.Parity.None, DisplayName = "None" },
                    new ComBoxItem { Value = RJCP.IO.Ports.Parity.Odd, DisplayName = "Odd" },
                    new ComBoxItem { Value = RJCP.IO.Ports.Parity.Even, DisplayName = "Even" },
                    new ComBoxItem { Value = RJCP.IO.Ports.Parity.Mark, DisplayName = "Mark" },
                    new ComBoxItem { Value = RJCP.IO.Ports.Parity.Space, DisplayName = "Space" }
                }
            };

            // 初始化CRC16开关
            EnableCRC16ViewModel = new ToggleSwitchViewModel
            {
                IsChecked = false
            };

            // 初始化Modbus类型下拉框
            ModbusTypeViewModel = new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                ItemsSource = new List<ComBoxItem>
                {
                    new ComBoxItem { Value = EModbusType.RS232, DisplayName = "RS232" },
                    new ComBoxItem { Value = EModbusType.RS485, DisplayName = "RS485" }
                }
            };

            // 初始化默认选中项
            UpdateSelectedPortName(string.Empty);
            UpdateSelectedBaudRate(9600);
            UpdateSelectedDataBits(8);
            UpdateSelectedStopBits(RJCP.IO.Ports.StopBits.One);
            UpdateSelectedParity(RJCP.IO.Ports.Parity.None);
            UpdateSelectedModbusType(EModbusType.RS232);

            SubscribeValueChanges();
        }

        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view;
        }

        /// <summary>
        /// 从后端 SerialConfig 复制数据到视图模型
        /// </summary>
        /// <param name="source">源SerialConfig对象，可以为null</param>
        public void CopyFrom(SerialConfig? source)
        {
            if (source == null || !source.HasValue)
            {
                // 如果source为null，使用默认值
                source = new SerialConfig();
            }

            var config = source.Value;

            // 刷新串口列表，并保留已保存但当前未枚举到的串口名
            RefreshPortNameItems(config.PortName);

            UpdateSelectedPortName(config.PortName ?? string.Empty);
            UpdateSelectedBaudRate(config.BaudRate);
            UpdateSelectedDataBits(config.DataBits);
            UpdateSelectedStopBits(config.StopBits);
            UpdateSelectedParity(config.Parity);
            EnableCRC16ViewModel.IsChecked = config.IsEnableCRC16;
            UpdateSelectedModbusType(config.ModbusType);
        }

        /// <summary>
        /// 从视图模型复制数据到后端 SerialConfig
        /// </summary>
        /// <param name="target">目标SerialConfig对象</param>
        public void CopyTo(ref SerialConfig target)
        {
            target.PortName = GetSelectedPortName();
            target.BaudRate = GetSelectedBaudRate();
            target.DataBits = GetSelectedDataBits();
            target.StopBits = GetSelectedStopBits();
            target.Parity = GetSelectedParity();
            target.IsEnableCRC16 = EnableCRC16ViewModel.IsChecked;
            target.ModbusType = GetSelectedModbusType();
        }

        /// <summary>
        /// 订阅所有控件的值变化事件
        /// </summary>
        private void SubscribeValueChanges()
        {
            PortNameViewModel.ValueChanged += OnValueChanged;
            BaudRateViewModel.ValueChanged += OnValueChanged;
            DataBitsViewModel.ValueChanged += OnValueChanged;
            StopBitsViewModel.ValueChanged += OnValueChanged;
            ParityViewModel.ValueChanged += OnValueChanged;
            EnableCRC16ViewModel.ValueChanged += OnValueChanged;
            ModbusTypeViewModel.ValueChanged += OnValueChanged;
        }

        /// <summary>
        /// 值变化事件处理
        /// </summary>
        private void OnValueChanged(object? sender, EventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }

        /// <summary>
        /// 构建串口号下拉项
        /// 使用系统API获取本机可用串口
        /// </summary>
        /// <returns>串口号下拉项列表</returns>
        private static List<ComBoxItem> BuildPortNameItems()
        {
            var portNames = GetLocalPortNames();
            var items = new List<ComBoxItem>();

            foreach (var portName in portNames)
            {
                items.Add(new ComBoxItem { Value = portName, DisplayName = portName });
            }

            return items;
        }

        /// <summary>
        /// 构建波特率下拉项
        /// 提供常用的波特率选项
        /// </summary>
        /// <returns>波特率下拉项列表</returns>
        private static List<ComBoxItem> BuildBaudRateItems()
        {
            var baudRates = new[]
            {
                50, 75, 110, 150, 300, 600, 1200, 1800, 2400,
                4800, 9600, 14400, 19200, 28800, 38400,
                57600, 115200, 230400, 460800, 921600
            };

            var items = new List<ComBoxItem>();
            foreach (var baudRate in baudRates)
            {
                items.Add(new ComBoxItem { Value = baudRate, DisplayName = baudRate.ToString() });
            }

            return items;
        }

        /// <summary>
        /// 构建数据位下拉项
        /// 仅提供5/6/7/8四个选项
        /// </summary>
        /// <returns>数据位下拉项列表</returns>
        private static List<ComBoxItem> BuildDataBitsItems()
        {
            var dataBitsList = new[] { 5, 6, 7, 8 };
            var items = new List<ComBoxItem>();

            foreach (var dataBits in dataBitsList)
            {
                items.Add(new ComBoxItem { Value = dataBits, DisplayName = dataBits.ToString() });
            }

            return items;
        }

        /// <summary>
        /// 获取本机串口名称列表
        /// 按端口号排序便于用户选择
        /// </summary>
        /// <returns>已排序的串口名称列表</returns>
        private static IEnumerable<string> GetLocalPortNames()
        {
            try
            {
                return new SerialPortStream().GetPortNames()
                    .OrderBy(GetPortSortOrder)
                    .ThenBy(portName => portName, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
            }
            catch
            {
                // 枚举失败时返回空列表
                return Array.Empty<string>();
            }
        }

        /// <summary>
        /// 刷新串口号下拉项
        /// 如果历史配置中的串口当前未被枚举到，则补入下拉列表
        /// </summary>
        /// <param name="savedPortName">已保存的串口名</param>
        private void RefreshPortNameItems(string? savedPortName)
        {
            if (PortNameViewModel.ItemsSource is not List<ComBoxItem> items)
            {
                return;
            }

            items.Clear();

            foreach (var item in BuildPortNameItems())
            {
                items.Add(item);
            }

            // 补入历史串口名，避免加载后丢失原值
            if (!string.IsNullOrWhiteSpace(savedPortName) &&
                items.All(item => item.Value is not string value || !string.Equals(value, savedPortName, StringComparison.OrdinalIgnoreCase)))
            {
                items.Add(new ComBoxItem { Value = savedPortName, DisplayName = savedPortName });
            }
        }

        /// <summary>
        /// 获取串口排序值
        /// COM后的数字优先排序
        /// </summary>
        /// <param name="portName">串口名</param>
        /// <returns>排序值</returns>
        private static int GetPortSortOrder(string portName)
        {
            if (!string.IsNullOrWhiteSpace(portName) &&
                portName.StartsWith("COM", StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(portName.Substring(3), out var portNumber))
            {
                return portNumber;
            }

            return int.MaxValue;
        }

        #region 选项更新和获取方法

        private void UpdateSelectedPortName(string portName)
        {
            var items = PortNameViewModel.ItemsSource as List<ComBoxItem>;
            if (items == null) return;

            var selectedItem = items.FirstOrDefault(item => item.Value is string s && s == portName);
            if (selectedItem != null)
            {
                PortNameViewModel.SelectedItem = selectedItem;
            }
            else if (items.Count > 0)
            {
                PortNameViewModel.SelectedItem = items[0];
            }
        }

        private string GetSelectedPortName()
        {
            var selectedItem = PortNameViewModel.SelectedItem as ComBoxItem;
            if (selectedItem != null && selectedItem.Value is string portName)
            {
                return portName;
            }

            var items = PortNameViewModel.ItemsSource as List<ComBoxItem>;
            var firstItem = items?.FirstOrDefault();
            return firstItem?.Value as string ?? string.Empty;
        }

        private void UpdateSelectedBaudRate(int baudRate)
        {
            var items = BaudRateViewModel.ItemsSource as List<ComBoxItem>;
            if (items == null) return;

            var selectedItem = items.FirstOrDefault(item => item.Value is int i && i == baudRate);
            if (selectedItem != null)
            {
                BaudRateViewModel.SelectedItem = selectedItem;
            }
            else if (items.Count > 0)
            {
                BaudRateViewModel.SelectedItem = items.FirstOrDefault(item => item.Value is int i && i == 9600) ?? items[0];
            }
        }

        private int GetSelectedBaudRate()
        {
            var selectedItem = BaudRateViewModel.SelectedItem as ComBoxItem;
            if (selectedItem != null && selectedItem.Value is int baudRate)
            {
                return baudRate;
            }
            return 9600;
        }

        private void UpdateSelectedDataBits(int dataBits)
        {
            var items = DataBitsViewModel.ItemsSource as List<ComBoxItem>;
            if (items == null) return;

            var selectedItem = items.FirstOrDefault(item => item.Value is int i && i == dataBits);
            if (selectedItem != null)
            {
                DataBitsViewModel.SelectedItem = selectedItem;
            }
            else if (items.Count > 0)
            {
                DataBitsViewModel.SelectedItem = items.FirstOrDefault(item => item.Value is int i && i == 8) ?? items[items.Count - 1];
            }
        }

        private int GetSelectedDataBits()
        {
            var selectedItem = DataBitsViewModel.SelectedItem as ComBoxItem;
            if (selectedItem != null && selectedItem.Value is int dataBits)
            {
                return dataBits;
            }
            return 8;
        }

        private void UpdateSelectedStopBits(RJCP.IO.Ports.StopBits stopBits)
        {
            var items = StopBitsViewModel.ItemsSource as List<ComBoxItem>;
            if (items == null) return;

            var selectedItem = items.FirstOrDefault(item => item.Value is RJCP.IO.Ports.StopBits s && s == stopBits);
            if (selectedItem != null)
            {
                StopBitsViewModel.SelectedItem = selectedItem;
            }
            else if (items.Count > 0)
            {
                StopBitsViewModel.SelectedItem = items[0];
            }
        }

        private RJCP.IO.Ports.StopBits GetSelectedStopBits()
        {
            var selectedItem = StopBitsViewModel.SelectedItem as ComBoxItem;
            if (selectedItem != null && selectedItem.Value is RJCP.IO.Ports.StopBits stopBits)
            {
                return stopBits;
            }
            return RJCP.IO.Ports.StopBits.One;
        }

        private void UpdateSelectedParity(RJCP.IO.Ports.Parity parity)
        {
            var items = ParityViewModel.ItemsSource as List<ComBoxItem>;
            if (items == null) return;

            var selectedItem = items.FirstOrDefault(item => item.Value is RJCP.IO.Ports.Parity p && p == parity);
            if (selectedItem != null)
            {
                ParityViewModel.SelectedItem = selectedItem;
            }
            else if (items.Count > 0)
            {
                ParityViewModel.SelectedItem = items[0];
            }
        }

        private RJCP.IO.Ports.Parity GetSelectedParity()
        {
            var selectedItem = ParityViewModel.SelectedItem as ComBoxItem;
            if (selectedItem != null && selectedItem.Value is RJCP.IO.Ports.Parity parity)
            {
                return parity;
            }
            return RJCP.IO.Ports.Parity.None;
        }

        private void UpdateSelectedModbusType(EModbusType modbusType)
        {
            var items = ModbusTypeViewModel.ItemsSource as List<ComBoxItem>;
            if (items == null) return;

            var selectedItem = items.FirstOrDefault(item => item.Value is EModbusType m && m == modbusType);
            if (selectedItem != null)
            {
                ModbusTypeViewModel.SelectedItem = selectedItem;
            }
            else if (items.Count > 0)
            {
                ModbusTypeViewModel.SelectedItem = items[0];
            }
        }

        private EModbusType GetSelectedModbusType()
        {
            var selectedItem = ModbusTypeViewModel.SelectedItem as ComBoxItem;
            if (selectedItem != null && selectedItem.Value is EModbusType modbusType)
            {
                return modbusType;
            }
            return EModbusType.RS232;
        }

        #endregion
    }
}
