using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using GKG;

namespace GKG.UI.General
{
    /// <summary>
    /// 运控卡状态量初始化-视图模型
    /// </summary>
    public class HorizontalControlCardStateInitViewModel : ReactiveObject
    {
        private Control? _viewReference;
        private readonly ObservableCollection<ComBoxItem> _channelItems = new();
        private ControlCardType _selectedCardType = ControlCardType.GC800;
        private string _selectedControlCardID = string.Empty;

        /// <summary>
        /// 通道ID-下拉框视图模型
        /// </summary>
        public ComboxViewModel ChannelIDViewModel { get; }

        /// <summary>
        /// 兼容旧代码保留的属性，当前与 ChannelIDViewModel 指向同一个下拉框实例。
        /// </summary>
        public ComboxViewModel ControlCardIDViewModel => ChannelIDViewModel;

        /// <summary>
        /// 兼容旧代码保留的属性，当前与 ChannelIDViewModel 指向同一个下拉框实例。
        /// </summary>
        public ComboxViewModel IOChannelViewModel => ChannelIDViewModel;

        /// <summary>
        /// 值改变事件
        /// </summary>
        public event EventHandler? AfterModified;

        /// <summary>
        /// 选中的卡类型。UI 不再直接编辑，但仍保留用于兼容旧数据结构。
        /// </summary>
        public ControlCardType SelectedCardType
        {
            get => _selectedCardType;
            set
            {
                if (_selectedCardType != value)
                {
                    this.RaiseAndSetIfChanged(ref _selectedCardType, value);
                }
            }
        }

        public string SelectedControlCardID
        {
            get => _selectedControlCardID;
            set
            {
                value ??= string.Empty;
                if (_selectedControlCardID != value)
                {
                    this.RaiseAndSetIfChanged(ref _selectedControlCardID, value);
                }
            }
        }

        /// <summary>
        /// 选中的通道ID（保存/传后端用字符串）
        /// </summary>
        public string SelectedChannelID
        {
            get => (ChannelIDViewModel.SelectedItem as ComBoxItem)?.Value?.ToString() ?? string.Empty;
            set
            {
                if (ChannelIDViewModel.ItemsSource == null)
                {
                    return;
                }

                var targetItem = ChannelIDViewModel.ItemsSource.Cast<object>()
                    .OfType<ComBoxItem>()
                    .FirstOrDefault(o => string.Equals(o.Value?.ToString(), value, StringComparison.OrdinalIgnoreCase));
                if (targetItem != null)
                {
                    ChannelIDViewModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(SelectedChannelID));
                    return;
                }

                appendChannelItemIfNeeded(value);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public HorizontalControlCardStateInitViewModel()
        {
            ChannelIDViewModel = new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                ItemsSource = _channelItems,
            };

            subscribeValueChanges();
        }

        public void SetChannelItems(System.Collections.Generic.IEnumerable<string>? channelIDs)
        {
            _channelItems.Clear();
            foreach (var channelID in (channelIDs ?? Enumerable.Empty<string>())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase))
            {
                _channelItems.Add(new ComBoxItem
                {
                    Value = channelID,
                    DisplayName = channelID,
                });
            }

            if (_channelItems.Count == 0)
            {
                clearChannelItems();
                return;
            }

            ChannelIDViewModel.SelectedItem = _channelItems[0];
            this.RaisePropertyChanged(nameof(SelectedChannelID));
        }

        public void SetChannelItems(System.Collections.Generic.IEnumerable<IOStateInformation>? ioStates)
        {
            _channelItems.Clear();
            foreach (var ioState in (ioStates ?? Enumerable.Empty<IOStateInformation>())
                .Where(x => x != null && x.IOGuid != Guid.Empty)
                .GroupBy(x => x.IOGuid)
                .Select(x => x.First()))
            {
                _channelItems.Add(new ComBoxItem
                {
                    Value = ioState.IOGuid.ToString(),
                    DisplayName = BuildIOStateDisplayName(ioState),
                });
            }

            if (_channelItems.Count == 0)
            {
                clearChannelItems();
                return;
            }

            ChannelIDViewModel.SelectedItem = _channelItems[0];
            this.RaisePropertyChanged(nameof(SelectedChannelID));
        }

        private void clearChannelItems()
        {
            _channelItems.Clear();
            ChannelIDViewModel.SelectedItem = null;
            this.RaisePropertyChanged(nameof(SelectedChannelID));
        }

        private void appendChannelItemIfNeeded(string? channelID)
        {
            if (string.IsNullOrWhiteSpace(channelID))
            {
                return;
            }

            var existing = _channelItems.FirstOrDefault(o =>
                string.Equals(o.Value?.ToString(), channelID, StringComparison.OrdinalIgnoreCase));
            if (existing == null)
            {
                existing = new ComBoxItem
                {
                    Value = channelID,
                    DisplayName = channelID,
                };
                _channelItems.Add(existing);
            }

            ChannelIDViewModel.SelectedItem = existing;
            this.RaisePropertyChanged(nameof(SelectedChannelID));
        }

        private static string BuildIOStateDisplayName(IOStateInformation ioState)
        {
            if (ioState == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(ioState.IOName))
            {
                return ioState.IOName;
            }

            if (!string.IsNullOrWhiteSpace(ioState.ChannelId))
            {
                return ioState.ChannelId;
            }

            return ioState.IOGuid.ToString();
        }

        /// <summary>
        /// 订阅值变更
        /// </summary>
        private void subscribeValueChanges()
        {
            ChannelIDViewModel.ValueChanged += onValueChanged;
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
        public void CopyFrom(HorizontalControlCardStateInitCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            SelectedCardType = model.CardType;
            SelectedControlCardID = model.ControlCardID;
            SelectedChannelID = model.channelID;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        public void CopyTo(HorizontalControlCardStateInitCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            model.CardType = SelectedCardType;
            model.ControlCardID = SelectedControlCardID;
            model.channelID = SelectedChannelID;
        }
    }
}
