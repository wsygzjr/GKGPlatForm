using GKG.UI;
using GKG.UI.General;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.IODevice.ViewModels
{
    public class IODeviceInfoViewModel : ReactiveObject
    {
        private const string EmptyDisplayName = "[无]";
        private const string MissingCardDisplayFormat = "[已删除卡]{0}";
        private const string MissingChannelDisplayFormat = "[已失配通道]{0}";

        private Guid _ioGuid;
        private string _ioName = string.Empty;
        private string _deviceId = string.Empty;
        private string _channelSelection = string.Empty;
        private bool _stateReverse;
        private GKG.EReadWriteMode _readWriteMode = GKG.EReadWriteMode.ReadOnly;
        private bool _isChecked;
        private int _sequenceNo;
        private IReadOnlyList<ControlCardChannelOption> _controlCardOptions = Array.Empty<ControlCardChannelOption>();
        private string _selectedCardStableKey = string.Empty;

        public ComboxViewModel DeviceIdModel { get; }
        public ComboxViewModel ChannelSelectionModel { get; }
        public ComboxViewModel ReadWriteModeModel { get; }
        public TextInputViewModel IONameViewModel { get; }

        public Guid IOGuid
        {
            get => _ioGuid;
            set => this.RaiseAndSetIfChanged(ref _ioGuid, value);
        }

        public string IOName
        {
            get => _ioName;
            set => this.RaiseAndSetIfChanged(ref _ioName, value);
        }

        public string DeviceId
        {
            get => _deviceId;
            set
            {
                this.RaiseAndSetIfChanged(ref _deviceId, value);
                this.RaisePropertyChanged(nameof(HasSelectedDevice));
            }
        }

        public string ChannelSelection
        {
            get => _channelSelection;
            set
            {
                var normalized = IODeviceCfgViewModel.NormalizeChannelId(value);
                if (string.Equals(_channelSelection, normalized, StringComparison.OrdinalIgnoreCase))
                    return;

                this.RaiseAndSetIfChanged(ref _channelSelection, normalized);
                this.RaisePropertyChanged(nameof(HasSelectedChannel));
                SyncReadWriteModeFromChannelSelection();
            }
        }

        public bool StateReverse
        {
            get => _stateReverse;
            set => this.RaiseAndSetIfChanged(ref _stateReverse, value);
        }

        public GKG.EReadWriteMode ReadWriteMode
        {
            get => _readWriteMode;
            set
            {
                this.RaiseAndSetIfChanged(ref _readWriteMode, value);
                var target = ReadWriteModeModel.ItemsSource?
                    .OfType<ComBoxItem>()
                    .FirstOrDefault(x => x.Value is GKG.EReadWriteMode mode && mode == value);
                if (!ReferenceEquals(ReadWriteModeModel.SelectedItem, target))
                    ReadWriteModeModel.SelectedItem = target;
                this.RaisePropertyChanged(nameof(SelectedReadWriteModeOption));
            }
        }

        public bool IsChecked
        {
            get => _isChecked;
            set => this.RaiseAndSetIfChanged(ref _isChecked, value);
        }

        public int SequenceNo
        {
            get => _sequenceNo;
            set => this.RaiseAndSetIfChanged(ref _sequenceNo, value);
        }

        public bool IsStateMode { get; set; } = true;

        public bool HasSelectedDevice => !string.IsNullOrWhiteSpace(DeviceId);

        public bool HasSelectedChannel => !string.IsNullOrWhiteSpace(ChannelSelection);

        public IReadOnlyList<ControlCardChannelOption> ControlCardOptions => _controlCardOptions;

        public ControlCardChannelOption? SelectedControlCardOption
        {
            get => FindMatchedControlCardOption();
            set => ApplySelectedControlCardOption(value);
        }

        public IReadOnlyList<string> AvailableChannels
        {
            get
            {
                var matchedCard = FindMatchedControlCardOption();
                return matchedCard?.Channels?.Select(IODeviceCfgViewModel.NormalizeChannelId).ToArray() ?? Array.Empty<string>();
            }
        }

        public string? SelectedChannelOption
        {
            get => string.IsNullOrWhiteSpace(ChannelSelection) ? null : IODeviceCfgViewModel.NormalizeChannelId(ChannelSelection);
            set => ApplySelectedChannelOption(value);
        }

        public IReadOnlyList<ReadWriteModeOption> ReadWriteModeOptions { get; } = new List<ReadWriteModeOption>
        {
            new ReadWriteModeOption(GKG.EReadWriteMode.ReadOnly, "只读"),
            new ReadWriteModeOption(GKG.EReadWriteMode.WriteOnly, "只写"),
            new ReadWriteModeOption(GKG.EReadWriteMode.ReadWrite, "读写"),
        };

        public ReadWriteModeOption? SelectedReadWriteModeOption
        {
            get => ReadWriteModeOptions.FirstOrDefault(x => x.Value == ReadWriteMode);
            set
            {
                if (value != null)
                    ReadWriteMode = value.Value;
            }
        }

        public IODeviceInfoViewModel()
        {
            IONameViewModel = new TextInputViewModel();

            DeviceIdModel = new ComboxViewModel();
            DeviceIdModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            DeviceIdModel.ValueChanged += (_, __) => ApplySelectedDeviceItem(DeviceIdModel.SelectedItem as ComBoxItem);

            ChannelSelectionModel = new ComboxViewModel();
            ChannelSelectionModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            ChannelSelectionModel.ValueChanged += (_, __) => ApplySelectedChannelItem(ChannelSelectionModel.SelectedItem as ComBoxItem);

            ReadWriteModeModel = new ComboxViewModel();
            ReadWriteModeModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            ReadWriteModeModel.ItemsSource = ReadWriteModeOptions
                .Select(x => new ComBoxItem
                {
                    Value = x.Value,
                    DisplayName = x.DisplayName,
                })
                .ToList();
            ReadWriteModeModel.IsEnabled = false;
            ReadWriteModeModel.SelectedItem = (ReadWriteModeModel.ItemsSource as IEnumerable<ComBoxItem>)?.FirstOrDefault();

            this.WhenAnyValue(x => x.IOName)
                .Subscribe(value =>
                {
                    if (IONameViewModel.Text != value)
                        IONameViewModel.Text = value ?? string.Empty;
                });

            this.WhenAnyValue(x => x.DeviceId)
                .Subscribe(value =>
                {
                    var target = FindByValue(DeviceIdModel.ItemsSource, value);
                    if (!ReferenceEquals(DeviceIdModel.SelectedItem, target))
                        DeviceIdModel.SelectedItem = target;
                });

            this.WhenAnyValue(x => x.ChannelSelection)
                .Subscribe(value =>
                {
                    var target = FindByValue(ChannelSelectionModel.ItemsSource, value);
                    if (!ReferenceEquals(ChannelSelectionModel.SelectedItem, target))
                        ChannelSelectionModel.SelectedItem = target;
                });

            IONameViewModel.WhenAnyValue(x => x.Text)
                .Subscribe(value =>
                {
                    if (IOName != value)
                        IOName = value ?? string.Empty;
                });
        }

        /// <summary>
        /// 运控卡、通道下拉框默认都选“[无]”，与轴管理左侧表格保持一致。
        /// 通道号格式为 RO/RW/WO + 三位数字（如 RO001、RW002、WO003），读写模式随通道前缀自动确定。
        /// </summary>
        public void SelectNoDeviceAndNoChannel()
        {
            static bool IsNoneItem(ComBoxItem? x) =>
                x != null && (x.Value == null || x.Value is string s && s.Length == 0);

            if (DeviceIdModel.ItemsSource is IEnumerable<ComBoxItem> devList)
            {
                var noneDevice = devList.FirstOrDefault(IsNoneItem) ?? devList.FirstOrDefault();
                DeviceIdModel.SelectedItem = noneDevice;
            }

            if (ChannelSelectionModel.ItemsSource is IEnumerable<ComBoxItem> chList)
            {
                var noneCh = chList.FirstOrDefault(IsNoneItem) ?? chList.FirstOrDefault();
                ChannelSelectionModel.SelectedItem = noneCh;
            }
        }

        public void BindControlCards(IEnumerable<ControlCardChannelOption>? options)
        {
            _controlCardOptions = options?.ToList() ?? new List<ControlCardChannelOption>();

            var deviceOptions = new List<ComBoxItem>
            {
                new ComBoxItem
                {
                    Value = string.Empty,
                    DisplayName = EmptyDisplayName,
                }
            };

            deviceOptions.AddRange(_controlCardOptions
                .Select(x => new ComBoxItem
                {
                    Value = x.DeviceId,
                    DisplayName = x.DisplayName,
                })
                .ToList());

            DeviceIdModel.ItemsSource = deviceOptions;

            var matchedCard = FindMatchedControlCardOption();
            if (matchedCard != null)
            {
                _selectedCardStableKey = matchedCard.StableCardKey ?? string.Empty;
                if (!string.Equals(DeviceId, matchedCard.DeviceId, StringComparison.OrdinalIgnoreCase))
                    DeviceId = matchedCard.DeviceId;
            }
            else if (!string.IsNullOrWhiteSpace(DeviceId))
            {
                deviceOptions.Add(new ComBoxItem
                {
                    Value = DeviceId,
                    DisplayName = string.Format(MissingCardDisplayFormat, FormatShortDisplay(DeviceId)),
                });
            }

            var selectedDevice = FindByValue(DeviceIdModel.ItemsSource, DeviceId);
            if (!ReferenceEquals(DeviceIdModel.SelectedItem, selectedDevice))
                DeviceIdModel.SelectedItem = selectedDevice;

            RefreshChannelSelectionItems();
            this.RaisePropertyChanged(nameof(ControlCardOptions));
            this.RaisePropertyChanged(nameof(SelectedControlCardOption));
            this.RaisePropertyChanged(nameof(AvailableChannels));
            this.RaisePropertyChanged(nameof(SelectedChannelOption));
        }

        public void CopyFrom(GKG.IOStateInformation src, string deviceId)
        {
            if (src == null)
                return;

            IOGuid = src.IOGuid == Guid.Empty ? Guid.NewGuid() : src.IOGuid;
            IOName = src.IOName;
            DeviceId = deviceId ?? string.Empty;
            ChannelSelection = IODeviceCfgViewModel.NormalizeChannelId(src.ChannelId);
            StateReverse = src.StateReverse;
            ReadWriteMode = src.EReadWriteMode;
            if (!string.IsNullOrWhiteSpace(ChannelSelection))
                SyncReadWriteModeFromChannelSelection();
        }

        public void CopyFrom(IODeviceInfoViewModel src)
        {
            if (src == null)
                return;

            IOGuid = src.IOGuid == Guid.Empty ? Guid.NewGuid() : src.IOGuid;
            IOName = src.IOName;
            DeviceId = src.DeviceId;
            ChannelSelection = IODeviceCfgViewModel.NormalizeChannelId(src.ChannelSelection);
            StateReverse = src.StateReverse;
            ReadWriteMode = src.ReadWriteMode;
            if (!string.IsNullOrWhiteSpace(ChannelSelection))
                SyncReadWriteModeFromChannelSelection();
        }

        public GKG.IOStateInformation ToBackendIoStateInformation(Guid deviceGuid)
        {
            if (IOGuid == Guid.Empty)
                IOGuid = Guid.NewGuid();

            return new GKG.IOStateInformation
            {
                IOGuid = IOGuid,
                IOName = IOName?.Trim() ?? string.Empty,
                DeviceGuid = deviceGuid,
                ChannelId = IODeviceCfgViewModel.NormalizeChannelId(ChannelSelection),
                StateReverse = StateReverse,
                EReadWriteMode = ReadWriteMode,
            };
        }

        public string GetNormalizedIOName()
        {
            return IOName?.Trim() ?? string.Empty;
        }

        public void RestoreIOName(string ioName)
        {
            IOName = ioName ?? string.Empty;
        }

        public void RestoreDeviceAndChannel(string deviceId, string channelSelection)
        {
            DeviceId = deviceId ?? string.Empty;
            RefreshChannelSelectionItems();
            ChannelSelection = channelSelection ?? string.Empty;
        }

        public void ApplySelectedDeviceItem(ComBoxItem? selectedItem)
        {
            ApplySelectedControlCardOption(selectedItem == null
                ? null
                : _controlCardOptions.FirstOrDefault(x => string.Equals(x.DeviceId, selectedItem.Value?.ToString(), StringComparison.OrdinalIgnoreCase)));
        }

        public void ApplySelectedChannelItem(ComBoxItem? selectedItem)
        {
            ApplySelectedChannelOption(selectedItem?.Value?.ToString());
        }

        public void ApplySelectedControlCardOption(ControlCardChannelOption? selectedOption, bool allowClear = false)
        {
            if (!allowClear &&
                selectedOption == null &&
                !string.IsNullOrWhiteSpace(DeviceId))
            {
                return;
            }

            var selectedValue = selectedOption?.DeviceId ?? string.Empty;
            var selectedStableKey = selectedOption?.StableCardKey ?? string.Empty;
            if (string.Equals(DeviceId, selectedValue, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(_selectedCardStableKey, selectedStableKey, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            DeviceId = selectedValue;
            _selectedCardStableKey = selectedStableKey;
            if (string.IsNullOrWhiteSpace(selectedValue))
                ChannelSelection = string.Empty;
            RefreshChannelSelectionItems();
            this.RaisePropertyChanged(nameof(SelectedControlCardOption));
            this.RaisePropertyChanged(nameof(AvailableChannels));
            this.RaisePropertyChanged(nameof(SelectedChannelOption));
        }

        public void ApplySelectedChannelOption(string? selectedChannel, bool allowClear = false)
        {
            if (!allowClear &&
                string.IsNullOrWhiteSpace(selectedChannel) &&
                !string.IsNullOrWhiteSpace(ChannelSelection))
            {
                return;
            }

            var normalizedChannel = IODeviceCfgViewModel.NormalizeChannelId(selectedChannel);
            if (string.Equals(ChannelSelection, normalizedChannel, StringComparison.OrdinalIgnoreCase))
                return;

            ChannelSelection = normalizedChannel;
            this.RaisePropertyChanged(nameof(SelectedChannelOption));
        }

        internal void SyncReadWriteModeFromChannelSelection()
        {
            if (!IODeviceCfgViewModel.TryGetReadWriteModeFromChannelId(ChannelSelection, out var mode))
                return;

            if (ReadWriteMode != mode)
                ReadWriteMode = mode;
        }

        private void RefreshChannelSelectionItems()
        {
            var matchedCard = FindMatchedControlCardOption();
            var channels = matchedCard?.Channels?.Select(IODeviceCfgViewModel.NormalizeChannelId).ToArray() ?? Array.Empty<string>();
            var channelOptions = new List<ComBoxItem>
            {
                new ComBoxItem
                {
                    Value = string.Empty,
                    DisplayName = EmptyDisplayName,
                }
            };

            channelOptions.AddRange(channels
                .Select(x => new ComBoxItem
                {
                    Value = x,
                    DisplayName = x,
                })
                .ToList());

            ChannelSelectionModel.ItemsSource = channelOptions;

            var normalizedSelection = IODeviceCfgViewModel.NormalizeChannelId(ChannelSelection);
            if (!string.Equals(_channelSelection, normalizedSelection, StringComparison.OrdinalIgnoreCase))
                ChannelSelection = normalizedSelection;

            if (!string.IsNullOrWhiteSpace(normalizedSelection) &&
                !channels.Contains(normalizedSelection, StringComparer.OrdinalIgnoreCase))
            {
                channelOptions.Add(new ComBoxItem
                {
                    Value = normalizedSelection,
                    DisplayName = string.Format(MissingChannelDisplayFormat, normalizedSelection),
                });
            }

            var selectedChannel = FindByValue(ChannelSelectionModel.ItemsSource, ChannelSelection);
            if (!ReferenceEquals(ChannelSelectionModel.SelectedItem, selectedChannel))
                ChannelSelectionModel.SelectedItem = selectedChannel;

            this.RaisePropertyChanged(nameof(SelectedControlCardOption));
            this.RaisePropertyChanged(nameof(AvailableChannels));
            this.RaisePropertyChanged(nameof(SelectedChannelOption));
        }

        private static ComBoxItem? FindByValue(IEnumerable? itemsSource, string? value)
        {
            if (itemsSource == null || string.IsNullOrWhiteSpace(value))
                return null;

            foreach (var item in itemsSource)
            {
                if (item is ComBoxItem cb && string.Equals(cb.Value?.ToString(), value, StringComparison.OrdinalIgnoreCase))
                    return cb;
            }

            return null;
        }

        private static string FormatShortDisplay(string? value)
        {
            var text = value?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            return text.Length > 8 ? text[..8] : text;
        }

        private ControlCardChannelOption? FindMatchedControlCardOption()
        {
            if (!string.IsNullOrWhiteSpace(_selectedCardStableKey))
            {
                var matchedByStableKey = _controlCardOptions.FirstOrDefault(x =>
                    string.Equals(x.StableCardKey, _selectedCardStableKey, StringComparison.OrdinalIgnoreCase));
                if (matchedByStableKey != null)
                    return matchedByStableKey;
            }

            if (!string.IsNullOrWhiteSpace(DeviceId))
            {
                var matchedByDeviceId = _controlCardOptions.FirstOrDefault(x =>
                    string.Equals(x.DeviceId, DeviceId, StringComparison.OrdinalIgnoreCase));
                if (matchedByDeviceId != null)
                    return matchedByDeviceId;
            }

            return null;
        }

        public class ControlCardChannelOption
        {
            public string DeviceId { get; set; } = string.Empty;
            public string StableCardKey { get; set; } = string.Empty;
            public string DisplayName { get; set; } = string.Empty;
            public IReadOnlyList<string> Channels { get; set; } = Array.Empty<string>();
        }

        public class ReadWriteModeOption
        {
        public ReadWriteModeOption(GKG.EReadWriteMode value, string displayName)
        {
            Value = value;
            DisplayName = displayName;
        }

        public GKG.EReadWriteMode Value { get; }

            public string DisplayName { get; }
        }
    }
}
