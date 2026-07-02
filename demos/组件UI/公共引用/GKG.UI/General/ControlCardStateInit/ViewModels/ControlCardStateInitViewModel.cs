using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GKG.UI.General
{
    /// <summary>
    /// 运控卡状态量初始化-视图模型
    /// </summary>
    public class ControlCardStateInitViewModel : ReactiveObject
    {
        private Control? _viewReference;

        /// <summary>
        /// 运控卡ID-下拉框视图模型
        /// </summary>
        public ComboxViewModel ControlCardIDViewModel { get; }

        /// <summary>
        /// IO通道号-下拉框视图模型
        /// </summary>
        public ComboxViewModel IOChannelViewModel { get; }

        /// <summary>
        /// 值改变事件
        /// </summary>
        public event EventHandler? AfterModified;

        /// <summary>
        /// 选中的卡类型
        /// </summary>
        public ControlCardType SelectedCardType
        {
            get => (ControlCardType)((ControlCardIDViewModel.SelectedItem as ComBoxItem)?.Value ?? ControlCardType.GC800);
            set
            {
                if (ControlCardIDViewModel.ItemsSource != null)
                {
                    var targetItem = ControlCardIDViewModel.ItemsSource.Cast<ComBoxItem>()
                        .FirstOrDefault(o => (ControlCardType)o.Value == value);
                    if (targetItem != null)
                        ControlCardIDViewModel.SelectedItem = targetItem;

                    this.RaisePropertyChanged(nameof(SelectedCardType));
                    this.RaisePropertyChanged(nameof(SelectedControlCardID));
                }
            }
        }

        public string SelectedControlCardID
        {
            get => SelectedCardType.ToString();
            set
            {
                if (Enum.TryParse<ControlCardType>(value, out var parsed))
                {
                    SelectedCardType = parsed;
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

        /// <summary>
        /// 构造函数
        /// </summary>
        public ControlCardStateInitViewModel()
        {
            ControlCardIDViewModel = new ComboxViewModel();
            IOChannelViewModel = new ComboxViewModel();

            // 初始化数据
            var cardItems = new List<ComBoxItem>
            {
                new ComBoxItem { Value = ControlCardType.GC800, DisplayName = ControlCardType.GC800.ToString() },
                new ComBoxItem { Value = ControlCardType.GMCMINI, DisplayName = ControlCardType.GMCMINI.ToString() }
            };
            ControlCardIDViewModel.ItemsSource = cardItems;
            ControlCardIDViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            ControlCardIDViewModel.SelectedItem = cardItems.FirstOrDefault();

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

        /// <summary>
        /// 订阅值变更
        /// </summary>
        private void subscribeValueChanges()
        {
            ControlCardIDViewModel.ValueChanged += onValueChanged;
            IOChannelViewModel.ValueChanged += onValueChanged;
        }

        /// <summary>
        /// 值变更处理
        /// </summary>
        private void onValueChanged(object? sender, ValueChangedEventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }

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
        public void CopyFrom(ControlCardStateInitCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            SelectedCardType = model.CardType;
            SelectedIOChannel = model.IOChannel;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        public void CopyTo(ControlCardStateInitCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            model.CardType = SelectedCardType;
            model.ControlCardID = SelectedControlCardID;
            model.IOChannel = SelectedIOChannel;
        }
    }
}
