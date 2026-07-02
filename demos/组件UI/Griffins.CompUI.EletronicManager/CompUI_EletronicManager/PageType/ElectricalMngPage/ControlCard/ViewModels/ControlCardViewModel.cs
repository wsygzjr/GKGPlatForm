using GKG.UI;
using GKG.UI.General;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.ControlCard.ViewModels
{
    public class ControlCardViewModel : ReactiveObject
    {
        private const int DefaultAxisCount = 16;
        private const int DefaultChannelCount = 4;
        private const string DefaultCardKind = "Legacy";
        private const string DefaultCardType = "GMCMINI";
        private const string DefaultMotionCardName = "\u65B0\u589E\u8FD0\u63A7\u5361";
        private List<MotionCardDriverDefinition> _driverDefinitions = CreateFallbackDriverDefinitions();

        private string _motionCardName = DefaultMotionCardName;
        public string MotionCardName
        {
            get => _motionCardName;
            set => this.RaiseAndSetIfChanged(ref _motionCardName, value);
        }

        private string _motionCardCustomID = string.Empty;
        public string MotionCardCustomID
        {
            get => _motionCardCustomID;
            set => this.RaiseAndSetIfChanged(ref _motionCardCustomID, value);
        }

        private Guid _cadID;
        public Guid CadID
        {
            get => _cadID;
            set => this.RaiseAndSetIfChanged(ref _cadID, value);
        }

        private Guid _objectId;
        public Guid ObjectId
        {
            get => _objectId;
            set => this.RaiseAndSetIfChanged(ref _objectId, value);
        }

        private Guid? _previousObjectId;
        public Guid? PreviousObjectId
        {
            get => _previousObjectId;
            private set => this.RaiseAndSetIfChanged(ref _previousObjectId, value);
        }

        private int _axisCount;
        public int AxisCount
        {
            get => _axisCount;
            set
            {
                var normalized = NormalizeAxisCount(value);
                this.RaiseAndSetIfChanged(ref _axisCount, normalized);

                var axisText = normalized.ToString();
                if (SupportAxisCountViewModel.Text != axisText)
                    SupportAxisCountViewModel.Text = axisText;
            }
        }

        private bool _isDummy;
        public bool IsDummy
        {
            get => _isDummy;
            set => this.RaiseAndSetIfChanged(ref _isDummy, value);
        }

        private bool _canDelete;
        public bool CanDelete
        {
            get => _canDelete;
            set => this.RaiseAndSetIfChanged(ref _canDelete, value);
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set => this.RaiseAndSetIfChanged(ref _isChecked, value);
        }

        public TextInputViewModel MotionCardCustomIDViewModel { get; } = new();
        public TextInputViewModel MotionControlInterfaceTypeViewModel { get; } = new();
        public TextInputViewModel CardCountViewModel { get; } = new();
        public TextInputViewModel SupportChannelCountViewModel { get; } = new();
        public TextInputViewModel SupportAxisCountViewModel { get; } = new();
        public TextInputViewModel StateChannelCountInputViewModel { get; } = new();
        public TextInputViewModel AnalogChannelCountInputViewModel { get; } = new();

        public TextInputViewModel MotionCardNameViewModel { get; } = new();
        public TextInputViewModel CadIDViewModel { get; } = new();
        public NumericViewModel AxisCountViewModel { get; } = new();
        public NumericViewModel AnalogChannelCountViewModel { get; } = new();
        public NumericViewModel StateChannelCountViewModel { get; } = new();

        public ComboxViewModel ControlCardKindModel { get; } = new();
        public ComboxViewModel ControlCardTypeModel { get; } = new();

        public string SelectedControlCardKind
        {
            get => (string)((ControlCardKindModel.SelectedItem as ComBoxItem)?.Value ?? string.Empty);
            set
            {
                if (ControlCardKindModel.ItemsSource is not IEnumerable<ComBoxItem> items)
                    return;

                var target = items.FirstOrDefault(o => string.Equals((string)o.Value, value, StringComparison.OrdinalIgnoreCase));
                if (target != null)
                    ControlCardKindModel.SelectedItem = target;

                this.RaisePropertyChanged(nameof(SelectedControlCardKind));
                this.RaisePropertyChanged(nameof(SelectedControlCardKindDisplay));
            }
        }

        public string SelectedControlCardType
        {
            get => (string)((ControlCardTypeModel.SelectedItem as ComBoxItem)?.Value ?? string.Empty);
            set
            {
                if (ControlCardTypeModel.ItemsSource is not IEnumerable<ComBoxItem> items)
                    return;

                var target = items.FirstOrDefault(o => string.Equals((string)o.Value, value, StringComparison.OrdinalIgnoreCase));
                if (target != null)
                    ControlCardTypeModel.SelectedItem = target;

                this.RaisePropertyChanged(nameof(SelectedControlCardType));
                this.RaisePropertyChanged(nameof(SelectedControlCardTypeDisplay));
            }
        }

        public string SelectedControlCardKindDisplay =>
            (ControlCardKindModel.SelectedItem as ComBoxItem)?.DisplayName ?? string.Empty;

        public string SelectedControlCardTypeDisplay =>
            (ControlCardTypeModel.SelectedItem as ComBoxItem)?.DisplayName ?? string.Empty;

        public string MotionControlInterfaceTypeDisplay => SelectedControlCardKindDisplay;

        public string BoardTypeDisplay
        {
            get
            {
                var kind = SelectedControlCardKindDisplay?.Trim() ?? string.Empty;
                var type = SelectedControlCardTypeDisplay?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(kind))
                    return type;

                if (string.IsNullOrWhiteSpace(type))
                    return kind;

                return $"{kind}/{type}";
            }
        }

        public string AxisCountDisplay => NormalizeDisplayNumber(SupportAxisCountViewModel.Text, AxisCount);

        public string StateChannelCountDisplay => NormalizeDisplayNumber(StateChannelCountInputViewModel.Text, (int)StateChannelCountViewModel.Value);

        public string AnalogChannelCountDisplay => NormalizeDisplayNumber(AnalogChannelCountInputViewModel.Text, (int)AnalogChannelCountViewModel.Value);

        public ControlCardViewModel()
        {
            InitDefaults();
        }

        public IReadOnlyList<MotionCardDriverDefinition> DriverDefinitions => _driverDefinitions;

        public static List<MotionCardDriverDefinition> CreateFallbackDriverDefinitions()
        {
            return new List<MotionCardDriverDefinition>
            {
                new MotionCardDriverDefinition
                {
                    KindValue = "Legacy",
                    KindDisplay = "兼容驱动",
                    TypeValue = GKG.MotionCardType.GMCMINI.ToString(),
                    TypeDisplay = "GMCMINI",
                    MotionCardType = GKG.MotionCardType.GMCMINI,
                    DefaultCadId = Guid.Empty,
                    SupportAxisNum = DefaultAxisCount,
                    SupportIoStateNum = DefaultChannelCount,
                    SupportAnalogNum = DefaultChannelCount,
                },
                new MotionCardDriverDefinition
                {
                    KindValue = "Legacy",
                    KindDisplay = "兼容驱动",
                    TypeValue = GKG.MotionCardType.GC800.ToString(),
                    TypeDisplay = "GC800",
                    MotionCardType = GKG.MotionCardType.GC800,
                    DefaultCadId = Guid.Empty,
                    SupportAxisNum = DefaultAxisCount,
                    SupportIoStateNum = DefaultChannelCount,
                    SupportAnalogNum = DefaultChannelCount,
                },
            };
        }

        public void ApplyDriverDefinitions(IEnumerable<MotionCardDriverDefinition>? definitions)
        {
            var normalized = NormalizeDriverDefinitions(definitions);
            _driverDefinitions = normalized.Count > 0 ? normalized : CreateFallbackDriverDefinitions();

            var selectedType = SelectedControlCardType;
            var selectedKind = SelectedControlCardKind;

            ControlCardKindModel.ItemsSource = BuildControlCardKindItems(_driverDefinitions);
            SelectedControlCardKind = string.IsNullOrWhiteSpace(selectedKind) ? _driverDefinitions[0].KindValue : selectedKind;

            RefreshControlCardTypeItemsSource(SelectedControlCardKind);

            if (!string.IsNullOrWhiteSpace(selectedType))
                SelectedControlCardType = selectedType;

            ApplySelectedDriverDefaults(forceCardGuid: true, forceAxisAndChannelCount: false);
        }

        public void InitDefaults()
        {
            ControlCardKindModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            ControlCardTypeModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);

            if (ControlCardKindModel.ItemsSource == null)
            {
                ControlCardKindModel.ItemsSource = BuildControlCardKindItems(_driverDefinitions);
                ControlCardKindModel.ValueChanged += ControlCardKindModel_ValueChanged;
            }

            if (ControlCardTypeModel.ItemsSource == null)
            {
                RefreshControlCardTypeItemsSource(SelectedControlCardKind);
                ControlCardTypeModel.ValueChanged += ControlCardTypeModel_ValueChanged;
            }

            if (ControlCardKindModel.SelectedItem == null && ControlCardKindModel.ItemsSource is IEnumerable<ComBoxItem> kindItems)
                ControlCardKindModel.SelectedItem = kindItems.FirstOrDefault();

            if (ControlCardTypeModel.SelectedItem == null && ControlCardTypeModel.ItemsSource is IEnumerable<ComBoxItem> typeItems)
                ControlCardTypeModel.SelectedItem = typeItems.FirstOrDefault();

            MotionCardCustomIDViewModel.ValueChanged += (_, _) => MotionCardCustomID = MotionCardCustomIDViewModel.Text?.Trim() ?? string.Empty;
            CardCountViewModel.ValueChanged += (_, _) => SyncDisplayFieldsToData();
            SupportChannelCountViewModel.ValueChanged += (_, _) => SyncDisplayFieldsToData();
            SupportAxisCountViewModel.ValueChanged += (_, _) => SyncDisplayFieldsToData();
            StateChannelCountInputViewModel.ValueChanged += (_, _) => SyncDisplayFieldsToData();
            AnalogChannelCountInputViewModel.ValueChanged += (_, _) => SyncDisplayFieldsToData();
            CadIDViewModel.ValueChanged += (_, _) => TrySetGuid(CadIDViewModel.Text, value => CadID = value);
            MotionCardNameViewModel.ValueChanged += (_, _) =>
            {
                var text = MotionCardNameViewModel.Text?.Trim() ?? string.Empty;
                MotionCardName = string.IsNullOrWhiteSpace(text) ? DefaultMotionCardName : text;
            };
            AxisCountViewModel.ValueChanged += (_, _) => AxisCount = NormalizeAxisCount((int)AxisCountViewModel.Value);

            MotionCardCustomIDViewModel.IsEnabled = false;
            MotionControlInterfaceTypeViewModel.IsEnabled = false;
            CardCountViewModel.IsEnabled = true;
            SupportChannelCountViewModel.IsEnabled = true;
            SupportAxisCountViewModel.IsEnabled = false;
            StateChannelCountInputViewModel.IsEnabled = false;
            AnalogChannelCountInputViewModel.IsEnabled = false;
            CadIDViewModel.IsEnabled = false;
            MotionCardNameViewModel.IsEnabled = true;
            AxisCountViewModel.IsEnabled = true;

            AxisCountViewModel.Minimum = 1;
            AxisCountViewModel.Maximum = 64;
            AxisCountViewModel.Value = DefaultAxisCount;

            AnalogChannelCountViewModel.Minimum = 0;
            AnalogChannelCountViewModel.Maximum = 128;

            StateChannelCountViewModel.Minimum = 0;
            StateChannelCountViewModel.Maximum = 128;

            if (string.IsNullOrWhiteSpace(MotionCardCustomID))
                MotionCardCustomID = BuildDefaultCustomId();

            MotionCardCustomIDViewModel.Text = MotionCardCustomID;
            MotionControlInterfaceTypeViewModel.Text = MotionControlInterfaceTypeDisplay;

            if (string.IsNullOrWhiteSpace(CardCountViewModel.Text))
                CardCountViewModel.Text = "1";

            if (string.IsNullOrWhiteSpace(SupportChannelCountViewModel.Text))
                SupportChannelCountViewModel.Text = DefaultChannelCount.ToString();

            if (string.IsNullOrWhiteSpace(SupportAxisCountViewModel.Text))
                SupportAxisCountViewModel.Text = DefaultAxisCount.ToString();

            if (string.IsNullOrWhiteSpace(StateChannelCountInputViewModel.Text))
                StateChannelCountInputViewModel.Text = DefaultChannelCount.ToString();

            if (string.IsNullOrWhiteSpace(AnalogChannelCountInputViewModel.Text))
                AnalogChannelCountInputViewModel.Text = DefaultChannelCount.ToString();

            if (string.IsNullOrWhiteSpace(SelectedControlCardKind))
                SelectedControlCardKind = DefaultCardKind;

            RefreshControlCardTypeItemsSource(SelectedControlCardKind);

            if (string.IsNullOrWhiteSpace(SelectedControlCardType))
                SelectedControlCardType = DefaultCardType;

            ApplySelectedDriverDefaults(forceCardGuid: true, forceAxisAndChannelCount: true);
            ApplyDefaultCardGuidByTypeIfEmpty();
            MotionCardNameViewModel.Text = MotionCardName;
            EnsureObjectId();
        }

        public void EnsureObjectId()
        {
            if (ObjectId == Guid.Empty)
                ObjectId = Guid.NewGuid();
        }

        public void AcceptObjectIdMigration()
        {
            PreviousObjectId = null;
        }

        public void SetMotionCardType(GKG.MotionCardType motionCardType)
        {
            var mapping = _driverDefinitions.FirstOrDefault(x => x.MotionCardType == motionCardType);
            if (mapping == null)
                mapping = _driverDefinitions.FirstOrDefault();

            if (mapping == null)
                return;

            SelectedControlCardKind = mapping.KindValue;
            RefreshControlCardTypeItemsSource(mapping.KindValue);
            SelectedControlCardType = mapping.TypeValue;
            ApplySelectedDriverDefaults(forceCardGuid: true, forceAxisAndChannelCount: true);
        }

        public GKG.MotionCardType GetMotionCardTypeOrDefault()
        {
            var selected = FindSelectedDriverDefinition();
            if (selected != null)
                return selected.MotionCardType;

            return Enum.TryParse(SelectedControlCardType, out GKG.MotionCardType motionCardType)
                ? motionCardType
                : GKG.MotionCardType.GMCMINI;
        }

        public ControlCardViewModel Clone()
        {
            var vm = new ControlCardViewModel();
            vm.CopyFrom(this);
            vm.AcceptObjectIdMigration();
            return vm;
        }

        public void CopyFrom(ControlCardViewModel src)
        {
            var sourceMotionCardName = src.MotionCardName;
            var sourceMotionCardNameEditor = src.MotionCardNameViewModel.Text;

            MotionCardCustomID = src.MotionCardCustomID;
            CadID = src.CadID;

            if (ObjectId != Guid.Empty && src.ObjectId != ObjectId)
                PreviousObjectId = ObjectId;

            ObjectId = src.ObjectId;
            AxisCount = NormalizeAxisCount(src.AxisCount);
            IsDummy = src.IsDummy;

            ApplyDriverDefinitions(src.DriverDefinitions);

            SelectedControlCardKind = src.SelectedControlCardKind;
            RefreshControlCardTypeItemsSource(SelectedControlCardKind);
            SelectedControlCardType = src.SelectedControlCardType;

            MotionCardCustomIDViewModel.Text = MotionCardCustomID;
            CardCountViewModel.Text = src.CardCountViewModel.Text;
            SupportChannelCountViewModel.Text = src.SupportChannelCountViewModel.Text;
            SupportAxisCountViewModel.Text = src.SupportAxisCountViewModel.Text;
            StateChannelCountInputViewModel.Text = src.StateChannelCountInputViewModel.Text;
            AnalogChannelCountInputViewModel.Text = src.AnalogChannelCountInputViewModel.Text;
            CadIDViewModel.Text = CadID.ToString();
            MotionControlInterfaceTypeViewModel.Text = src.MotionControlInterfaceTypeDisplay;
            AxisCountViewModel.Value = AxisCount;
            AnalogChannelCountViewModel.Value = src.AnalogChannelCountViewModel.Value;
            StateChannelCountViewModel.Value = src.StateChannelCountViewModel.Value;

            MotionCardName = sourceMotionCardName;
            MotionCardNameViewModel.Text = string.IsNullOrWhiteSpace(sourceMotionCardNameEditor)
                ? sourceMotionCardName
                : sourceMotionCardNameEditor;

            MotionControlInterfaceTypeViewModel.IsEnabled = false;
            StateChannelCountInputViewModel.IsEnabled = false;
            AnalogChannelCountInputViewModel.IsEnabled = false;
        }

        private void SyncDisplayFieldsToData()
        {
            AxisCount = ParsePositiveOrDefault(SupportAxisCountViewModel.Text, AxisCount > 0 ? AxisCount : DefaultAxisCount);
            AxisCountViewModel.Value = AxisCount;
            var stateChannelCount = ParseNonNegativeOrDefault(StateChannelCountInputViewModel.Text, DefaultChannelCount);
            var analogChannelCount = ParseNonNegativeOrDefault(AnalogChannelCountInputViewModel.Text, DefaultChannelCount);
            StateChannelCountViewModel.Value = stateChannelCount;
            AnalogChannelCountViewModel.Value = analogChannelCount;
            SupportChannelCountViewModel.Text = stateChannelCount.ToString();
            this.RaisePropertyChanged(nameof(AxisCountDisplay));
            this.RaisePropertyChanged(nameof(StateChannelCountDisplay));
            this.RaisePropertyChanged(nameof(AnalogChannelCountDisplay));
        }

        public void SetPersistedMotionCardName(string? name)
        {
            var finalName = string.IsNullOrWhiteSpace(name) ? DefaultMotionCardName : name.Trim();
            MotionCardName = finalName;
            MotionCardNameViewModel.Text = finalName;
        }

        private static string BuildDefaultCustomId()
        {
            return "1";
        }

        private static int ExtractSequenceNumber(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 1;

            var trimmed = text.Trim();
            if (trimmed.All(char.IsDigit))
                return int.TryParse(trimmed, out var sequenceNo) && sequenceNo > 0 ? sequenceNo : 1;

            var match = Regex.Match(text, @"(\d+)$");
            if (match.Success)
                return int.TryParse(match.Groups[1].Value, out var suffixNo) && suffixNo > 0 ? suffixNo : 1;

            var digits = new string(text.Where(char.IsDigit).ToArray());
            if (int.TryParse(digits, out var digitNo) && digitNo > 0)
                return digitNo;

            return 1;
        }

        private void ControlCardKindModel_ValueChanged(object? sender, ValueChangedEventArgs e)
        {
            this.RaisePropertyChanged(nameof(SelectedControlCardKind));
            this.RaisePropertyChanged(nameof(SelectedControlCardKindDisplay));
            this.RaisePropertyChanged(nameof(MotionControlInterfaceTypeDisplay));
            this.RaisePropertyChanged(nameof(BoardTypeDisplay));
            MotionControlInterfaceTypeViewModel.Text = MotionControlInterfaceTypeDisplay;
            ApplySelectedDriverDefaults(forceCardGuid: true, forceAxisAndChannelCount: false);
        }

        private void ControlCardTypeModel_ValueChanged(object? sender, ValueChangedEventArgs e)
        {
            var selected = _driverDefinitions.FirstOrDefault(x =>
                string.Equals(x.TypeValue, SelectedControlCardType, StringComparison.OrdinalIgnoreCase));
            if (selected != null &&
                !string.Equals(SelectedControlCardKind, selected.KindValue, StringComparison.OrdinalIgnoreCase))
            {
                SelectedControlCardKind = selected.KindValue;
            }

            this.RaisePropertyChanged(nameof(SelectedControlCardType));
            this.RaisePropertyChanged(nameof(SelectedControlCardTypeDisplay));
            this.RaisePropertyChanged(nameof(MotionControlInterfaceTypeDisplay));
            this.RaisePropertyChanged(nameof(BoardTypeDisplay));
            MotionControlInterfaceTypeViewModel.Text = MotionControlInterfaceTypeDisplay;
            ApplySelectedDriverDefaults(forceCardGuid: true, forceAxisAndChannelCount: true);
        }

        public void EnsureCadId()
        {
            ApplyDefaultCardGuidByTypeIfEmpty(forceWhenCurrentIsKnownDefault: true);
        }

        private void ApplyDefaultCardGuidByTypeIfEmpty(bool forceWhenCurrentIsKnownDefault = false)
        {
            var shouldApply =
                CadID == Guid.Empty ||
                string.IsNullOrWhiteSpace(CadIDViewModel.Text) ||
                (forceWhenCurrentIsKnownDefault && IsKnownTypeDefaultGuid(CadID));

            if (!shouldApply)
                return;

            var generatedGuid = Guid.NewGuid();
            CadID = generatedGuid;
            CadIDViewModel.Text = generatedGuid.ToString();
        }

        private static int NormalizeAxisCount(int value)
        {
            return Math.Max(1, value);
        }

        private static int ParsePositiveOrDefault(string? text, int defaultValue)
        {
            return int.TryParse(text?.Trim(), out var value) && value > 0 ? value : defaultValue;
        }

        private static int ParseNonNegativeOrDefault(string? text, int defaultValue)
        {
            return int.TryParse(text?.Trim(), out var value) && value >= 0 ? value : defaultValue;
        }

        private static string NormalizeDisplayNumber(string? text, int fallback)
        {
            if (int.TryParse(text?.Trim(), out var value) && value >= 0)
                return value.ToString();

            return Math.Max(0, fallback).ToString();
        }

        private static void TrySetGuid(string? text, Action<Guid> setter)
        {
            if (Guid.TryParse(text?.Trim(), out var guid))
                setter(guid);
        }

        private void RefreshControlCardTypeItemsSource(string controlCardKind)
        {
            var items = _driverDefinitions
                .GroupBy(x => x.TypeValue ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .Select(info => new ComBoxItem { Value = info.TypeValue, DisplayName = info.TypeDisplay })
                .ToList();

            var currentSelection = SelectedControlCardType;

            ControlCardTypeModel.ItemsSource = items;

            var matched = items.FirstOrDefault(x => string.Equals((string)x.Value, currentSelection, StringComparison.OrdinalIgnoreCase));
            ControlCardTypeModel.SelectedItem = matched ?? items.FirstOrDefault();

            this.RaisePropertyChanged(nameof(SelectedControlCardType));
            this.RaisePropertyChanged(nameof(SelectedControlCardTypeDisplay));
            this.RaisePropertyChanged(nameof(BoardTypeDisplay));
        }

        private MotionCardDriverDefinition? FindSelectedDriverDefinition()
        {
            return _driverDefinitions.FirstOrDefault(x =>
                string.Equals(x.KindValue, SelectedControlCardKind, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(x.TypeValue, SelectedControlCardType, StringComparison.OrdinalIgnoreCase));
        }

        private void ApplySelectedDriverDefaults(bool forceCardGuid, bool forceAxisAndChannelCount)
        {
            var selected = FindSelectedDriverDefinition();
            if (selected == null)
                return;

            if (forceCardGuid && CadID == Guid.Empty)
            {
                var guidToUse = selected.DefaultCadId != Guid.Empty
                    ? selected.DefaultCadId
                    : Guid.NewGuid();
                CadID = guidToUse;
                CadIDViewModel.Text = guidToUse.ToString();
            }

            if (!forceAxisAndChannelCount)
                return;

            var axisCount = Math.Max(1, selected.SupportAxisNum);
            var stateCount = Math.Max(0, selected.SupportIoStateNum);
            var analogCount = Math.Max(0, selected.SupportAnalogNum);

            SupportAxisCountViewModel.Text = axisCount.ToString();
            AxisCountViewModel.Value = axisCount;
            AxisCount = axisCount;

            StateChannelCountInputViewModel.Text = stateCount.ToString();
            StateChannelCountViewModel.Value = stateCount;

            AnalogChannelCountInputViewModel.Text = analogCount.ToString();
            AnalogChannelCountViewModel.Value = analogCount;

            SupportChannelCountViewModel.Text = stateCount.ToString();
            this.RaisePropertyChanged(nameof(AxisCountDisplay));
            this.RaisePropertyChanged(nameof(StateChannelCountDisplay));
            this.RaisePropertyChanged(nameof(AnalogChannelCountDisplay));
        }

        private static List<ComBoxItem> BuildControlCardKindItems(IEnumerable<MotionCardDriverDefinition> definitions)
        {
            return definitions
                .GroupBy(x => x.KindValue ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .Select(g =>
                {
                    var first = g.First();
                    return new ComBoxItem
                    {
                        Value = first.KindValue ?? string.Empty,
                        DisplayName = string.IsNullOrWhiteSpace(first.KindDisplay) ? (first.KindValue ?? string.Empty) : first.KindDisplay
                    };
                })
                .ToList();
        }

        private static List<MotionCardDriverDefinition> NormalizeDriverDefinitions(IEnumerable<MotionCardDriverDefinition>? definitions)
        {
            if (definitions == null)
                return new List<MotionCardDriverDefinition>();

            return definitions
                .Where(x => x != null)
                .Select(x =>
                {
                    var kindValue = string.IsNullOrWhiteSpace(x.KindValue) ? "Dynamic" : x.KindValue;
                    var typeValue = string.IsNullOrWhiteSpace(x.TypeValue) ? x.MotionCardType.ToString() : x.TypeValue;
                    return new MotionCardDriverDefinition
                    {
                        KindValue = kindValue,
                        KindDisplay = string.IsNullOrWhiteSpace(x.KindDisplay) ? kindValue : x.KindDisplay,
                        TypeValue = typeValue,
                        TypeDisplay = string.IsNullOrWhiteSpace(x.TypeDisplay) ? typeValue : x.TypeDisplay,
                        MotionCardType = x.MotionCardType,
                        DefaultCadId = x.DefaultCadId,
                        SupportAxisNum = x.SupportAxisNum <= 0 ? DefaultAxisCount : x.SupportAxisNum,
                        SupportIoStateNum = x.SupportIoStateNum < 0 ? DefaultChannelCount : x.SupportIoStateNum,
                        SupportAnalogNum = x.SupportAnalogNum < 0 ? DefaultChannelCount : x.SupportAnalogNum,
                    };
                })
                .ToList();
        }

        public sealed class MotionCardDriverDefinition
        {
            public string KindValue { get; set; } = string.Empty;
            public string KindDisplay { get; set; } = string.Empty;
            public string TypeValue { get; set; } = string.Empty;
            public string TypeDisplay { get; set; } = string.Empty;
            public GKG.MotionCardType MotionCardType { get; set; }
            public Guid DefaultCadId { get; set; } = Guid.NewGuid();
            public int SupportAxisNum { get; set; } = DefaultAxisCount;
            public int SupportIoStateNum { get; set; } = DefaultChannelCount;
            public int SupportAnalogNum { get; set; } = DefaultChannelCount;
        }

        private static bool IsKnownTypeDefaultGuid(Guid guid)
        {
            return guid == Guid.Parse("A6607528-B306-4498-8076-039477C8F9AA") ||
                   guid == Guid.Parse("D250700C-F406-4440-9E6B-17A3B0183AAB");
        }
    }
}
