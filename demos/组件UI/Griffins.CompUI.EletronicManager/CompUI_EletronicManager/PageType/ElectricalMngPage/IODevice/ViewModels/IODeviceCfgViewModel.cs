using Avalonia.Controls;
using Avalonia.VisualTree;
using GF_Gereric;
using BackendEletronicFactoryParameters = GKG.SubMM.EletronicFactoryParameters;
using BackendEletronicManagerSubMachineModulesFactoryCfg = GKG.SubMM.EletronicManagerSubMachineModulesFactoryCfg;
using GKG.UI;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.IODevice.Views;
using Griffins.Map.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.ControlCard.ViewModels;
namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.IODevice.ViewModels
{
    public class IODeviceCfgViewModel : ReactiveObject
    {
        public enum IOPageMode
        {
            State,
            Analog,
        }

        private byte[] _cfgInfo = Array.Empty<byte>();

        private BackendEletronicManagerSubMachineModulesFactoryCfg _factoryCfgData = new BackendEletronicManagerSubMachineModulesFactoryCfg();

        private Control? _viewReference;
        private Func<IEnumerable<ControlCardViewModel>>? _controlCardsProvider;
        private IOPageMode _pageMode = IOPageMode.State;
        private bool _isUpdatingCheckState;
        private bool _isResolvingDuplicate;
        private bool _isSyncingReadWriteModeEditor;
        private bool _isSuppressingItemChangeNotifications;
        private static readonly Regex LegacyRWChannelNameRegex = new(@"^(?:通道|RW)\s*(\d+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex LegacyROChannelNameRegex = new(@"^(?:通道|RO)\s*(\d+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex LegacyWOChannelNameRegex = new(@"^(?:通道|WO)\s*(\d+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex StandardChannelIdRegex = new(@"^(RO|RW|WO)(\d{3})$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly Dictionary<IODeviceInfoViewModel, string> _lastValidIONames = new();
        private readonly Dictionary<IODeviceInfoViewModel, string> _lastValidDeviceIds = new();
        private readonly Dictionary<IODeviceInfoViewModel, string> _lastValidChannelSelections = new();

        public event EventHandler? AfterModified;

        public ObservableCollection<IODeviceInfoViewModel> IODeviceInfoList { get; } = new ObservableCollection<IODeviceInfoViewModel>();
        public ComboxViewModel ReadWriteModeEditorViewModel { get; } = new();

        public IOPageMode PageMode
        {
            get => _pageMode;
            set
            {
                this.RaiseAndSetIfChanged(ref _pageMode, value);
                this.RaisePropertyChanged(nameof(IsStateMode));
                this.RaisePropertyChanged(nameof(IsAnalogMode));
            }
        }

        public bool IsStateMode => PageMode == IOPageMode.State;
        public bool IsAnalogMode => PageMode == IOPageMode.Analog;

        private readonly IODeviceInfoViewModel _fallbackStateParameterItem = new()
        {
            IOName = "未选择状态量",
            IsStateMode = true,
        };

        private IODeviceInfoViewModel? _selectedIODevice;
        public IODeviceInfoViewModel? SelectedIODevice
        {
            get => _selectedIODevice;
            set
            {
                if (_selectedIODevice != null)
                    _selectedIODevice.PropertyChanged -= SelectedIODevice_PropertyChanged;

                this.RaiseAndSetIfChanged(ref _selectedIODevice, value);
                this.RaisePropertyChanged(nameof(CurrentStateParameterItem));

                if (_selectedIODevice != null)
                    _selectedIODevice.PropertyChanged += SelectedIODevice_PropertyChanged;

                this.RaisePropertyChanged(nameof(CanOpenAdvancedParameter));
                SyncReadWriteModeEditor();
            }
        }

        public string SelectedItemDisplayName => SelectedIODevice?.IOName ?? string.Empty;
        public IODeviceInfoViewModel CurrentStateParameterItem => SelectedIODevice ?? _fallbackStateParameterItem;

        public bool CanOpenAdvancedParameter =>
            IsStateMode &&
            SelectedIODevice != null &&
            SelectedIODevice.HasSelectedDevice &&
            SelectedIODevice.HasSelectedChannel;

        private bool _isAllChecked;
        public bool IsAllChecked
        {
            get => _isAllChecked;
            set
            {
                if (this.RaiseAndSetIfChanged(ref _isAllChecked, value) && !_isUpdatingCheckState)
                    SetAllChecked(value);
            }
        }

        public ReactiveCommand<Unit, Unit> AddCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCheckedCommand { get; }
        public ReactiveCommand<IODeviceInfoViewModel, Unit> EditCommand { get; }
        public ReactiveCommand<IODeviceInfoViewModel, Unit> DeleteCommand { get; }

        public IODeviceCfgViewModel()
        {
            AddCommand = ReactiveCommand.CreateFromTask(AddAsync);
            DeleteCheckedCommand = ReactiveCommand.Create(DeleteChecked);
            EditCommand = ReactiveCommand.CreateFromTask<IODeviceInfoViewModel>(EditAsync);
            DeleteCommand = ReactiveCommand.Create<IODeviceInfoViewModel>(Delete);
            IODeviceInfoList.CollectionChanged += IODeviceInfoList_CollectionChanged;

            this.WhenAnyValue(x => x.SelectedIODevice)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(SelectedItemDisplayName)));

            ReadWriteModeEditorViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            ReadWriteModeEditorViewModel.ItemsSource = _fallbackStateParameterItem.ReadWriteModeOptions
                .Select(x => new ComBoxItem
                {
                    Value = x.Value,
                    DisplayName = x.DisplayName,
                })
                .ToList();
            ReadWriteModeEditorViewModel.IsEnabled = false;
            SyncReadWriteModeEditor();
        }

        private void SelectedIODevice_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IODeviceInfoViewModel.IOName))
            {
                this.RaisePropertyChanged(nameof(SelectedItemDisplayName));
            }

            if (e.PropertyName == nameof(IODeviceInfoViewModel.DeviceId) ||
                e.PropertyName == nameof(IODeviceInfoViewModel.ChannelSelection) ||
                e.PropertyName == nameof(IODeviceInfoViewModel.HasSelectedDevice) ||
                e.PropertyName == nameof(IODeviceInfoViewModel.HasSelectedChannel))
            {
                this.RaisePropertyChanged(nameof(CanOpenAdvancedParameter));
            }

            if (e.PropertyName == nameof(IODeviceInfoViewModel.ReadWriteMode))
            {
                SyncReadWriteModeEditor();
            }
        }

        public void SetViewReference(Control view)
        {
            _viewReference = view;
        }

        public void SetCallBack(ICompUIRunTimeCallBack callBack)
        {
        }

        public void SetControlCardsProvider(Func<IEnumerable<ControlCardViewModel>>? provider)
        {
            _controlCardsProvider = provider;
            RefreshAvailableControlCards();
        }

        public void Init(byte[] data)
        {
            _isSuppressingItemChangeNotifications = true;
            try
            {
                _cfgInfo = data ?? Array.Empty<byte>();
                _lastValidIONames.Clear();
                _lastValidDeviceIds.Clear();
                _lastValidChannelSelections.Clear();

                _factoryCfgData = JsonObjConvert.FromJSonBytes<BackendEletronicManagerSubMachineModulesFactoryCfg>(_cfgInfo) ?? new BackendEletronicManagerSubMachineModulesFactoryCfg();

                IODeviceInfoList.Clear();
                foreach (var info in _factoryCfgData.IOStateInformations ?? Array.Empty<GKG.IOStateInformation>())
                {
                    var vm = new IODeviceInfoViewModel();
                    vm.IsStateMode = IsStateMode;
                    vm.CopyFrom(info, ResolveFrontDeviceId(info.DeviceGuid));
                    vm.BindControlCards(BuildControlCardOptions());
                    IODeviceInfoList.Add(vm);
                    UpdateLastValidSnapshots(vm);
                }

                UpdateSequenceNumbers();
                SelectedIODevice = IODeviceInfoList.FirstOrDefault();
                UpdateIsAllChecked();
            }
            finally
            {
                _isSuppressingItemChangeNotifications = false;
            }
        }

        public byte[] GetData()
        {
            CommitPendingEditorChanges();

            _factoryCfgData.IOStateInformations = BuildBackendIoStateInformations(IODeviceInfoList).ToArray();
            _factoryCfgData.EletronicFactoryParameters ??= new BackendEletronicFactoryParameters();
            _cfgInfo = JsonObjConvert.ToJSonBytes(_factoryCfgData);
            return _cfgInfo ?? Array.Empty<byte>();
        }

        public void CommitPendingEditorChanges()
        {
            if (_viewReference is IODeviceCfgView ioDeviceView)
                ioDeviceView.CommitPendingEditorChanges();
        }

        private static List<GKG.IOStateInformation> BuildBackendIoStateInformations(IEnumerable<IODeviceInfoViewModel> items)
        {
            var result = new List<GKG.IOStateInformation>();

            foreach (var ioItem in items ?? Enumerable.Empty<IODeviceInfoViewModel>())
            {
                var motionCardId = Guid.TryParse(ioItem.DeviceId, out var parsedMotionCardId)
                    ? parsedMotionCardId
                    : Guid.Empty;
                result.Add(ioItem.ToBackendIoStateInformation(motionCardId));
            }

            return result;
        }

        private static string ResolveFrontDeviceId(Guid deviceGuid) => deviceGuid == Guid.Empty ? string.Empty : deviceGuid.ToString();

        private async Task AddAsync()
        {
            var newItem = new IODeviceInfoViewModel
            {
                IOGuid = Guid.NewGuid(),
                IsStateMode = IsStateMode,
            };
            var controlCardOptions = BuildControlCardOptions();
            newItem.BindControlCards(controlCardOptions);
            newItem.SelectNoDeviceAndNoChannel();

            if (IsStateMode)
                newItem.IOName = GetNextStateChannelName();

            if (IsAnalogMode)
            {
                var owner = TryGetOwnerWindow();
                if (owner == null)
                    return;

                var dialogVm = new IODeviceEditWindowViewModel(newItem, controlCardOptions);
                var win = new IODeviceEditWindow { DataContext = dialogVm };
                win.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                var ok = await win.ShowDialog<bool>(owner);
                if (!ok)
                    return;
            }

            IODeviceInfoList.Add(newItem);
            UpdateLastValidSnapshots(newItem);
            UpdateSequenceNumbers();
            SelectedIODevice = newItem;
            AfterModified?.Invoke(this, EventArgs.Empty);
            this.RaisePropertyChanged(nameof(SelectedItemDisplayName));
            UpdateIsAllChecked();
        }

        private async Task EditAsync(IODeviceInfoViewModel vm)
        {
            if (vm == null)
                return;

            var owner = TryGetOwnerWindow();
            if (owner == null)
                return;

            var controlCardOptions = BuildControlCardOptions();
            vm.IsStateMode = IsStateMode;
            vm.BindControlCards(controlCardOptions);
            var dialogVm = new IODeviceEditWindowViewModel(vm, controlCardOptions);
            var win = new IODeviceEditWindow { DataContext = dialogVm };
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var ok = await win.ShowDialog<bool>(owner);
            if (!ok)
                return;

            SelectedIODevice = vm;
            AfterModified?.Invoke(this, EventArgs.Empty);
            this.RaisePropertyChanged(nameof(SelectedItemDisplayName));
        }

        private void Delete(IODeviceInfoViewModel vm)
        {
            if (vm == null)
                return;

            IODeviceInfoList.Remove(vm);
            UpdateSequenceNumbers();
            if (SelectedIODevice == vm)
                SelectedIODevice = IODeviceInfoList.FirstOrDefault();

            AfterModified?.Invoke(this, EventArgs.Empty);
            this.RaisePropertyChanged(nameof(SelectedItemDisplayName));
            UpdateIsAllChecked();
        }

        private void DeleteChecked()
        {
            var checkedItems = IODeviceInfoList.Where(x => x.IsChecked).ToList();
            if (checkedItems.Count == 0)
                return;

            foreach (var item in checkedItems)
            {
                IODeviceInfoList.Remove(item);
            }

            UpdateSequenceNumbers();
            SelectedIODevice = IODeviceInfoList.FirstOrDefault();
            AfterModified?.Invoke(this, EventArgs.Empty);
            this.RaisePropertyChanged(nameof(SelectedItemDisplayName));
            UpdateIsAllChecked();
        }

        private Window? TryGetOwnerWindow()
        {
            return _viewReference?.GetVisualRoot() as Window;
        }

        public void RefreshAvailableControlCards()
        {
            _isSuppressingItemChangeNotifications = true;
            try
            {
                var options = BuildControlCardOptions();
                foreach (var item in IODeviceInfoList)
                {
                    item.IsStateMode = IsStateMode;
                    item.BindControlCards(options);
                    UpdateLastValidSnapshots(item);
                }
            }
            finally
            {
                _isSuppressingItemChangeNotifications = false;
            }
        }

        private void UpdateSequenceNumbers()
        {
            for (var i = 0; i < IODeviceInfoList.Count; i++)
            {
                IODeviceInfoList[i].SequenceNo = i + 1;
            }
        }

        private void IODeviceInfoList_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems.OfType<IODeviceInfoViewModel>())
                {
                    item.PropertyChanged -= IODeviceInfo_PropertyChanged;
                    RemoveLastValidSnapshots(item);
                }
            }

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems.OfType<IODeviceInfoViewModel>())
                {
                    item.PropertyChanged += IODeviceInfo_PropertyChanged;
                    UpdateLastValidSnapshots(item);
                }
            }

            UpdateIsAllChecked();
        }

        private void IODeviceInfo_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isResolvingDuplicate || _isSuppressingItemChangeNotifications || sender is not IODeviceInfoViewModel item)
                return;

            if (e.PropertyName == nameof(IODeviceInfoViewModel.IsChecked))
            {
                UpdateIsAllChecked();
                return;
            }

            if (e.PropertyName == nameof(IODeviceInfoViewModel.IOName))
            {
                var previousName = _lastValidIONames.TryGetValue(item, out var savedName) ? savedName : string.Empty;
                ValidateIOName(item);
                if (!string.Equals(previousName, item.GetNormalizedIOName(), StringComparison.Ordinal))
                    AfterModified?.Invoke(this, EventArgs.Empty);
                return;
            }

            if (e.PropertyName == nameof(IODeviceInfoViewModel.DeviceId) ||
                e.PropertyName == nameof(IODeviceInfoViewModel.ChannelSelection))
            {
                var previousDeviceId = _lastValidDeviceIds.TryGetValue(item, out var savedDeviceId) ? savedDeviceId : string.Empty;
                var previousChannel = _lastValidChannelSelections.TryGetValue(item, out var savedChannel) ? savedChannel : string.Empty;
                UpdateLastValidSnapshots(item);
                if (!string.Equals(previousDeviceId, item.DeviceId ?? string.Empty, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(previousChannel, NormalizeChannelId(item.ChannelSelection), StringComparison.OrdinalIgnoreCase))
                {
                    AfterModified?.Invoke(this, EventArgs.Empty);
                }
                return;
            }

            if (e.PropertyName == nameof(IODeviceInfoViewModel.StateReverse) ||
                e.PropertyName == nameof(IODeviceInfoViewModel.ReadWriteMode))
            {
                AfterModified?.Invoke(this, EventArgs.Empty);
            }
        }

        private void ValidateIOName(IODeviceInfoViewModel item)
        {
            var ioName = item.GetNormalizedIOName();
            if (string.IsNullOrWhiteSpace(ioName))
            {
                _lastValidIONames[item] = ioName;
                return;
            }

            var isDuplicate = IODeviceInfoList
                .Where(x => !ReferenceEquals(x, item))
                .Any(x => string.Equals(x.GetNormalizedIOName(), ioName, StringComparison.OrdinalIgnoreCase));

            if (!isDuplicate)
            {
                _lastValidIONames[item] = ioName;
                return;
            }

            _isResolvingDuplicate = true;
            try
            {
                item.RestoreIOName(_lastValidIONames.TryGetValue(item, out var previousName) ? previousName : string.Empty);
            }
            finally
            {
                _isResolvingDuplicate = false;
            }
        }

        private void UpdateLastValidSnapshots(IODeviceInfoViewModel item)
        {
            _lastValidIONames[item] = item.GetNormalizedIOName();
            _lastValidDeviceIds[item] = item.DeviceId ?? string.Empty;
            _lastValidChannelSelections[item] = NormalizeChannelId(item.ChannelSelection);
        }

        private void RemoveLastValidSnapshots(IODeviceInfoViewModel item)
        {
            _lastValidIONames.Remove(item);
            _lastValidDeviceIds.Remove(item);
            _lastValidChannelSelections.Remove(item);
        }

        private void SetAllChecked(bool isChecked)
        {
            _isUpdatingCheckState = true;
            try
            {
                foreach (var item in IODeviceInfoList)
                    item.IsChecked = isChecked;
            }
            finally
            {
                _isUpdatingCheckState = false;
            }

            UpdateIsAllChecked();
        }

        private void UpdateIsAllChecked()
        {
            var isAllChecked = IODeviceInfoList.Count > 0 && IODeviceInfoList.All(x => x.IsChecked);

            _isUpdatingCheckState = true;
            try
            {
                this.RaiseAndSetIfChanged(ref _isAllChecked, isAllChecked);
            }
            finally
            {
                _isUpdatingCheckState = false;
            }
        }

        private IReadOnlyList<IODeviceInfoViewModel.ControlCardChannelOption> BuildControlCardOptions()
        {
            var cards = _controlCardsProvider?.Invoke() ?? Enumerable.Empty<ControlCardViewModel>();
            return cards
                .Where(x => x.CadID != Guid.Empty)
                .Select(x => new IODeviceInfoViewModel.ControlCardChannelOption
                {
                    DeviceId = x.CadID.ToString(),
                    StableCardKey = x.ObjectId.ToString(),
                    DisplayName = x.MotionCardName,
                    Channels = BuildChannelOptions(x.SupportChannelCountViewModel.Text),
                })
                .ToList();
        }

        private string GetNextStateChannelName()
        {
            var index = 0;
            var existingNames = IODeviceInfoList
                .Select(x => NormalizeChannelId(x.IOName))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            while (existingNames.Contains(FormatStateChannelName(index)))
            {
                index++;
            }

            return FormatStateChannelName(index);
        }

        private static IReadOnlyList<string> BuildChannelOptions(string? channelCountText)
        {
            if (!int.TryParse(channelCountText?.Trim(), out var count) || count <= 0)
                return Array.Empty<string>();

            var channels = new List<string>(count * 3);
            channels.AddRange(Enumerable.Range(0, count).Select(FormatROChannelId));
            channels.AddRange(Enumerable.Range(0, count).Select(FormatWOChannelId));
            channels.AddRange(Enumerable.Range(0, count).Select(FormatRWChannelId));
            return channels;
        }

        internal static string NormalizeChannelId(string? channelId)
        {
            var text = channelId?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var match = LegacyRWChannelNameRegex.Match(text);
            if (match.Success)
                return FormatRWChannelId(int.Parse(match.Groups[1].Value));

            match = LegacyROChannelNameRegex.Match(text);
            if (match.Success)
                return FormatROChannelId(int.Parse(match.Groups[1].Value));

            match = LegacyWOChannelNameRegex.Match(text);
            if (match.Success)
                return FormatWOChannelId(int.Parse(match.Groups[1].Value));

            match = StandardChannelIdRegex.Match(text);
            if (match.Success)
                return $"{match.Groups[1].Value.ToUpperInvariant()}{match.Groups[2].Value}";

            return text;
        }

        internal static bool TryGetReadWriteModeFromChannelId(string? channelId, out GKG.EReadWriteMode mode)
        {
            mode = GKG.EReadWriteMode.ReadOnly;
            var normalized = NormalizeChannelId(channelId);
            if (string.IsNullOrWhiteSpace(normalized))
                return false;

            if (normalized.StartsWith("RW", StringComparison.OrdinalIgnoreCase))
            {
                mode = GKG.EReadWriteMode.ReadWrite;
                return true;
            }

            if (normalized.StartsWith("WO", StringComparison.OrdinalIgnoreCase))
            {
                mode = GKG.EReadWriteMode.WriteOnly;
                return true;
            }

            if (normalized.StartsWith("RO", StringComparison.OrdinalIgnoreCase))
            {
                mode = GKG.EReadWriteMode.ReadOnly;
                return true;
            }

            return false;
        }

        private static string FormatRWChannelId(int index) => $"RW{index:D3}";
        private static string FormatROChannelId(int index) => $"RO{index:D3}";
        private static string FormatWOChannelId(int index) => $"WO{index:D3}";
        private static string FormatStateChannelName(int index) => $"State{index:D3}";

        private void SyncReadWriteModeEditor()
        {
            var mode = CurrentStateParameterItem.ReadWriteMode;
            var target = ReadWriteModeEditorViewModel.ItemsSource?
                .OfType<ComBoxItem>()
                .FirstOrDefault(x => x.Value is GKG.EReadWriteMode value && value == mode);

            _isSyncingReadWriteModeEditor = true;
            try
            {
                if (!ReferenceEquals(ReadWriteModeEditorViewModel.SelectedItem, target))
                    ReadWriteModeEditorViewModel.SelectedItem = target;
            }
            finally
            {
                _isSyncingReadWriteModeEditor = false;
            }
        }
    }
}
