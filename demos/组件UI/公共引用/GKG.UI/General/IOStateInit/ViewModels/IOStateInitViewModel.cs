using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using ReactiveUI;

namespace GKG.UI.General
{
    /// <summary>
    /// IO状态量初始化-视图模型（基础组件）
    /// 包含：IO设备ID 下拉框；IO通道号 下拉框
    /// </summary>
    public class IOStateInitViewModel : ReactiveObject
    {
        #region 私有字段

        private Control? _viewReference;

        #endregion

        #region UI组件
        /// <summary>
        /// IO设备ID-下拉框视图模型
        /// </summary>
        public ComboxViewModel IODeviceIDViewModel { get; }

        /// <summary>
        /// IO通道号-下拉框视图模型
        /// </summary>
        public ComboxViewModel IOChannelViewModel { get; }

        #endregion

        #region 值改变事件
        /// <summary>
        /// 值改变事件
        /// </summary>
        public event EventHandler? AfterModified;

        #endregion

        #region 响应式属性

        /// <summary>
        /// 选中的IO设备ID
        /// </summary>
        public string SelectedIODeviceID
        {
            get => (string)((IODeviceIDViewModel.SelectedItem as ComBoxItem)?.Value ?? "");
            set
            {
                if (IODeviceIDViewModel.ItemsSource != null)
                {
                    var targetItem = IODeviceIDViewModel.ItemsSource.Cast<ComBoxItem>()
                        .FirstOrDefault(o => (string)o.Value == value);
                    if (targetItem != null)
                        IODeviceIDViewModel.SelectedItem = targetItem;

                    this.RaisePropertyChanged(nameof(SelectedIODeviceID));
                }
            }
        }

        /// <summary>
        /// 选中的IO通道号
        /// </summary>
        public IOChannelType SelectedIOChannel
        {
            get => (IOChannelType)((IOChannelViewModel.SelectedItem as ComBoxItem)?.Value ?? IOChannelType.Input64);
            set
            {
                if (IOChannelViewModel.ItemsSource != null)
                {
                    var targetItem = IOChannelViewModel.ItemsSource.Cast<ComBoxItem>()
                        .FirstOrDefault(o => (IOChannelType)o.Value == value);
                    if (targetItem != null)
                        IOChannelViewModel.SelectedItem = targetItem;

                    this.RaisePropertyChanged(nameof(SelectedIOChannel));
                }
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数 - 使用默认示例项
        /// </summary>
        public IOStateInitViewModel()
        {
            IODeviceIDViewModel = new();
            IOChannelViewModel = new();

            // 初始化数据
            var deviceItems = new List<ComBoxItem>
            {
                new ComBoxItem { Value = "DeviceA", DisplayName = "设备 A" },
                new ComBoxItem { Value = "DeviceB", DisplayName = "设备 B" }
            };
            IODeviceIDViewModel.ItemsSource = deviceItems;
            IODeviceIDViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);

            var channelDisplayNames = new Dictionary<IOChannelType, string>
            {
                { IOChannelType.Input64, "64位输入" },
                { IOChannelType.Output64, "64位输出" },
                { IOChannelType.Input256, "256位输入" },
                { IOChannelType.Output256, "256位输出" }
            };
            IOChannelViewModel.ItemsSource = EnumExtensions.ToEnumItems(channelDisplayNames);
            IOChannelViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);

            // 订阅值变更
            subscribeValueChanges();
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 设置视图引用（用于弹窗等UI操作）
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view;
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        public void CopyFrom(IOStateInitCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            SelectedIODeviceID = model.IODeviceID ?? "";
            SelectedIOChannel = model.IOChannel;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        public void CopyTo(IOStateInitCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            model.IODeviceID = SelectedIODeviceID;
            model.IOChannel = SelectedIOChannel;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 订阅值变更
        /// </summary>
        private void subscribeValueChanges()
        {
            IODeviceIDViewModel.ValueChanged += onValueChanged;
            IOChannelViewModel.ValueChanged += onValueChanged;
        }

        /// <summary>
        /// 值变更事件处理
        /// </summary>
        private void onValueChanged(object? sender, ValueChangedEventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }

        #endregion
    }
}
