using Avalonia.Controls;
using Avalonia.VisualTree;
using GF_Gereric;
using GKG.ElectronicControl;
using BackendEletronicFactoryParameters = GKG.SubMM.EletronicFactoryParameters;
using BackendEletronicManagerSubMachineModulesFactoryCfg = GKG.SubMM.EletronicManagerSubMachineModulesFactoryCfg;
using BackendMotionControlCardInformationList = GKG.SubMM.MotionControlCardInformationList;
using BackendMotionControlCardInformations = GKG.SubMM.MotionControlCardInformations;
using Griffins.Map.UI;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.IODevice.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.ControlCard.ViewModels
{
    public class ControlCardCfgViewModel : ReactiveObject
    {
        private byte[] _cfgInfo = Array.Empty<byte>();
        private GKG.IOStateInitParameters? _powerOnIO;
        private ICompUIRunTimeCallBack? _callBack;
        private bool _hasInitializedCallBack;
        private Control? _viewReference;
        private bool _isInitializing;
        private List<ControlCardViewModel.MotionCardDriverDefinition> _motionCardDriverDefinitions = ControlCardViewModel.CreateFallbackDriverDefinitions();

        public event EventHandler? AfterModified;

        public ControlCardListViewModel ControlCardListViewModel { get; } = new();
        public AxisConfigViewModel AxisConfigViewModel { get; }
        public IODeviceCfgViewModel IODeviceCfgViewModel { get; } = new();

        public ControlCardCfgViewModel()
        {
            AxisConfigViewModel = new AxisConfigViewModel(() => ControlCardListViewModel.ControlCardList);

            ControlCardListViewModel.PropertyChanged += OnListViewModelPropertyChanged;
            ControlCardListViewModel.ControlCardList.CollectionChanged += OnControlCardCollectionChanged;
            ControlCardListViewModel.DriverDefinitionsProvider = () => _motionCardDriverDefinitions;

            foreach (var cardVm in ControlCardListViewModel.ControlCardList)
            {
                AttachControlCard(cardVm);
            }

            RefreshLinkedViews(refreshAxisAndIo: false);
            AxisConfigViewModel.AfterModified += (_, _) =>
            {
                if (_isInitializing)
                    return;

                AfterModified?.Invoke(this, EventArgs.Empty);
            };
            IODeviceCfgViewModel.AfterModified += (_, _) =>
            {
                if (_isInitializing)
                    return;

                AfterModified?.Invoke(this, EventArgs.Empty);
            };
            IODeviceCfgViewModel.SetControlCardsProvider(() => ControlCardListViewModel.ControlCardList);
        }

        public void SetCallBack(ICompUIRunTimeCallBack? callBack)
        {
            if (_hasInitializedCallBack && ReferenceEquals(_callBack, callBack))
                return;

            _callBack = callBack;
            ControlCardListViewModel.SetCallBack(callBack);
            RefreshMotionCardDriverDefinitions();
            _hasInitializedCallBack = true;

        }

        public void SetViewReference(Control view)
        {
            _viewReference = view;
            ControlCardListViewModel.OwnerWindowProvider = TryGetOwnerWindow;
            IODeviceCfgViewModel.SetViewReference(view);

        }

        public void Init(byte[] data)
        {
            _isInitializing = true;

            try
            {
                _cfgInfo = data ?? Array.Empty<byte>();
                var factoryCfg = JsonObjConvert.FromJSonBytes<BackendEletronicManagerSubMachineModulesFactoryCfg>(_cfgInfo) ?? new BackendEletronicManagerSubMachineModulesFactoryCfg();
                var persistedCardNames = ExtractPersistedMotionCardNames(_cfgInfo);
                _powerOnIO = factoryCfg.EletronicFactoryParameters?.PowerOnIO;

                ResetCards();

                var cardInfos = factoryCfg.EletronicFactoryParameters?.MotionControlCardInformations;
                if (cardInfos != null)
                {
                    var usedCardIds = new HashSet<Guid>();
                    var displayIndex = 1;
                    foreach (var info in cardInfos)
                    {
                        var cardVm = CreateControlCardViewModel(
                            info,
                            displayIndex++,
                            factoryCfg,
                            persistedCardNames.TryGetValue(info.MotionCardID, out var persistedName) ? persistedName : null);
                        cardVm.SetPersistedMotionCardName(cardVm.MotionCardName);
                        cardVm.CardCountViewModel.Text = "1";

                        EnsureUniqueMotionCardId(cardVm, usedCardIds);
                        ControlCardListViewModel.ControlCardList.Add(cardVm);
                    }
                }

                if (ControlCardListViewModel.ControlCardList.Count == 0)
                {
                    var defaultCard = new ControlCardViewModel();
                    defaultCard.ApplyDriverDefinitions(_motionCardDriverDefinitions);
                    defaultCard.MotionCardCustomID = ControlCardListViewModel.GetNextObjectId();
                    defaultCard.MotionCardCustomIDViewModel.Text = defaultCard.MotionCardCustomID;
                    EnsureUniqueMotionCardId(defaultCard, new HashSet<Guid>());
                    defaultCard.EnsureObjectId();
                    ControlCardListViewModel.ControlCardList.Add(defaultCard);
                }

                ControlCardListViewModel.SelectedControlCard = ControlCardListViewModel.ControlCardList.FirstOrDefault();
                AxisConfigViewModel.Load(factoryCfg.AxisInformations, axisInfo => ResolveBackendAxisParameter(factoryCfg, axisInfo));
                IODeviceCfgViewModel.Init(_cfgInfo);
            }
            finally
            {
                _isInitializing = false;
            }
        }

        public byte[] GetData()
        {
            IODeviceCfgViewModel.CommitPendingEditorChanges();

            var cardInfos = new BackendMotionControlCardInformationList();
            var axisInfos = AxisConfigViewModel.ToBackendAxisInformations();
            var axisParameterSnapshots = AxisConfigViewModel.ToBackendAxisParameterSnapshots()
                .GroupBy(x => x.MotionCardGuid)
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(x => x.AxisNo).ToDictionary(
                        x => x.Key,
                        x => x.OrderByDescending(y => y.Revision).First()));

            foreach (var cardVm in ControlCardListViewModel.ControlCardList)
            {
                axisParameterSnapshots.TryGetValue(cardVm.CadID, out var selectedAxisParameters);
                var requiredAxisCount = selectedAxisParameters == null || selectedAxisParameters.Count == 0
                    ? cardVm.AxisCount
                    : Math.Max(cardVm.AxisCount, selectedAxisParameters.Keys.Max() + 1);
                var mergedParameters = CreateAxisParametersByAxisConfig(requiredAxisCount);

                if (selectedAxisParameters != null)
                {
                    foreach (var pair in selectedAxisParameters.OrderBy(x => x.Key))
                    {
                        EnsureParameterCapacity(mergedParameters, pair.Key);
                        var parameter = CloneMotionControlFactoryParameter(pair.Value.Parameter);
                        parameter.AxisNo = pair.Key;
                        mergedParameters.Parameters[pair.Key] = parameter;
                    }
                }

                cardInfos.Add(new BackendMotionControlCardInformations
                {
                    MotionCardType = cardVm.GetMotionCardTypeOrDefault(),
                    MotionCardID = cardVm.CadID,
                    MotionControlFactoryParameters = mergedParameters,
                });
            }

            var ioInfos = BuildBackendIoStateInformations(IODeviceCfgViewModel.IODeviceInfoList);

            var factoryCfg = new BackendEletronicManagerSubMachineModulesFactoryCfg
            {
                AxisInformations = axisInfos.ToArray(),
                IOStateInformations = ioInfos.ToArray(),
                EletronicFactoryParameters = new BackendEletronicFactoryParameters
                {
                    PowerOnIO = _powerOnIO,
                    MotionControlCardInformations = cardInfos,
                }
            };

            _cfgInfo = SerializeFactoryCfgWithMotionCardNames(factoryCfg, ControlCardListViewModel.ControlCardList);
            return _cfgInfo ?? Array.Empty<byte>();
        }

        private static List<GKG.IOStateInformation> BuildBackendIoStateInformations(IEnumerable<IODevice.ViewModels.IODeviceInfoViewModel> items)
        {
            var result = new List<GKG.IOStateInformation>();

            foreach (var ioItem in items ?? Enumerable.Empty<IODevice.ViewModels.IODeviceInfoViewModel>())
            {
                var deviceGuid = Guid.TryParse(ioItem.DeviceId, out var parsedDeviceGuid)
                    ? parsedDeviceGuid
                    : Guid.Empty;
                result.Add(ioItem.ToBackendIoStateInformation(deviceGuid));
            }

            return result;
        }

        private static int ParseCountText(string? text, int defaultValue = 0)
        {
            return int.TryParse(text, out var value) ? Math.Max(0, value) : defaultValue;
        }

        private static GKG.MotionControlFactoryParameter? ResolveBackendAxisParameter(
            BackendEletronicManagerSubMachineModulesFactoryCfg factoryCfg,
            GKG.AxisInformation axisInfo)
        {
            return factoryCfg.EletronicFactoryParameters?.MotionControlCardInformations?
                .FirstOrDefault(x => x.MotionCardID == axisInfo.MotionCardGuid)?
                .MotionControlFactoryParameters?
                .Parameters?
                .FirstOrDefault(x => x.AxisNo == axisInfo.AxisNo);
        }

        private static GKG.MotionControlFactoryParameters CloneMotionControlFactoryParameters(GKG.MotionControlFactoryParameters? source, int axisCount)
        {
            var parameters = source?.Parameters?
                .Select(CloneMotionControlFactoryParameter)
                .ToArray()
                ?? Array.Empty<GKG.MotionControlFactoryParameter>();

            var targetLength = Math.Max(1, axisCount);

            var resized = new GKG.MotionControlFactoryParameter[targetLength];
            for (var i = 0; i < resized.Length; i++)
            {
                resized[i] = i < parameters.Length
                    ? parameters[i]
                    : CreateDefaultAxisParameter(i);
            }

            return new GKG.MotionControlFactoryParameters
            {
                Parameters = resized,
            };
        }

        private static void EnsureParameterCapacity(GKG.MotionControlFactoryParameters parameters, int axisNo)
        {
            var current = parameters.Parameters ?? Array.Empty<GKG.MotionControlFactoryParameter>();
            if (axisNo < current.Length)
                return;

            var resized = new GKG.MotionControlFactoryParameter[axisNo + 1];
            for (var i = 0; i < resized.Length; i++)
            {
                resized[i] = i < current.Length
                    ? current[i]
                    : CreateDefaultAxisParameter(i);
            }

            parameters.Parameters = resized;
        }

        private static GKG.MotionControlFactoryParameter CreateDefaultAxisParameter(int axisNo)
        {
            var parameter = new GKG.MotionControlFactoryParameter();
            parameter.AxisNo = axisNo;
            return parameter;
        }

        private static GKG.MotionControlFactoryParameter CloneMotionControlFactoryParameter(GKG.MotionControlFactoryParameter source)
        {
            return MotionControlFactoryParameterViewModel.FromModel(source).ToModel();
        }

        private static GKG.MotionControlFactoryParameters CreateAxisParametersByAxisConfig(int axisCount)
        {
            var normalizedAxisCount = Math.Max(1, axisCount);
            var parameters = new GKG.MotionControlFactoryParameter[normalizedAxisCount];
            for (var i = 0; i < parameters.Length; i++)
            {
                parameters[i] = CreateDefaultAxisParameter(i);
            }

            return new GKG.MotionControlFactoryParameters
            {
                Parameters = parameters,
            };
        }

        private int? ResolveAxisCountForCard(BackendMotionControlCardInformations backendCard, BackendEletronicManagerSubMachineModulesFactoryCfg factoryCfg)
        {
            var currentCard = ControlCardListViewModel.ControlCardList.FirstOrDefault(x => x.CadID == backendCard.MotionCardID);
            if (currentCard != null && currentCard.AxisCount > 0)
                return currentCard.AxisCount;

            var paramCount = backendCard.MotionControlFactoryParameters?.Parameters?.Length ?? 0;
            if (paramCount > 0)
                return paramCount;

            var mappedCount = factoryCfg.AxisInformations?.Count(x => x.MotionCardGuid == backendCard.MotionCardID) ?? 0;
            if (mappedCount > 0)
                return mappedCount;

            var frontCard = _motionCardDriverDefinitions.FirstOrDefault(x => x.MotionCardType == backendCard.MotionCardType);
            return frontCard != null && frontCard.SupportAxisNum > 0
                ? frontCard.SupportAxisNum
                : (int?)null;
        }

        private int ResolveStateChannelCountForCard(BackendMotionControlCardInformations backendCard)
        {
            var currentCard = ControlCardListViewModel.ControlCardList.FirstOrDefault(x => x.CadID == backendCard.MotionCardID);
            if (currentCard != null)
            {
                var currentCount = ParseCountText(currentCard.StateChannelCountInputViewModel.Text);
                if (currentCount >= 0)
                    return currentCount;
            }

            var frontCard = _motionCardDriverDefinitions.FirstOrDefault(x => x.MotionCardType == backendCard.MotionCardType);
            return frontCard != null ? Math.Max(0, frontCard.SupportIoStateNum) : 0;
        }

        private int ResolveAnalogChannelCountForCard(BackendMotionControlCardInformations backendCard)
        {
            var currentCard = ControlCardListViewModel.ControlCardList.FirstOrDefault(x => x.CadID == backendCard.MotionCardID);
            if (currentCard != null)
            {
                var currentCount = ParseCountText(currentCard.AnalogChannelCountInputViewModel.Text);
                if (currentCount >= 0)
                    return currentCount;
            }

            var frontCard = _motionCardDriverDefinitions.FirstOrDefault(x => x.MotionCardType == backendCard.MotionCardType);
            return frontCard != null ? Math.Max(0, frontCard.SupportAnalogNum) : 0;
        }

        private static string BuildFrontCardCustomId(int? displayIndex)
        {
            return displayIndex.HasValue && displayIndex.Value > 0
                ? displayIndex.Value.ToString()
                : "1";
        }

        private void OnListViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ControlCardListViewModel.SelectedControlCard))
                return;

            RefreshLinkedViews(refreshAxisAndIo: false);
        }

        private void OnControlCardCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var cardVm in e.OldItems.OfType<ControlCardViewModel>())
                {
                    DetachControlCard(cardVm);
                }
            }

            if (e.NewItems != null)
            {
                foreach (var cardVm in e.NewItems.OfType<ControlCardViewModel>())
                {
                    cardVm.ApplyDriverDefinitions(_motionCardDriverDefinitions);
                    AttachControlCard(cardVm);
                }
            }

            NormalizeDisplayCardCustomIds();
            RefreshLinkedViews();

            if (_isInitializing)
                return;

            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        private void NormalizeDisplayCardCustomIds()
        {
            var index = 1;
            foreach (var cardVm in ControlCardListViewModel.ControlCardList)
            {
                var displayId = index.ToString();
                index++;

                if (!string.Equals(cardVm.MotionCardCustomID, displayId, StringComparison.Ordinal))
                    cardVm.MotionCardCustomID = displayId;

                if (!string.Equals(cardVm.MotionCardCustomIDViewModel.Text, displayId, StringComparison.Ordinal))
                    cardVm.MotionCardCustomIDViewModel.Text = displayId;
            }
        }

        private void AttachControlCard(ControlCardViewModel cardVm)
        {
            cardVm.EnsureObjectId();
            cardVm.PropertyChanged += OnControlCardPropertyChanged;
        }

        private void DetachControlCard(ControlCardViewModel cardVm)
        {
            cardVm.PropertyChanged -= OnControlCardPropertyChanged;
        }

        private void OnControlCardPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not ControlCardViewModel cardVm)
                return;

            if (e.PropertyName == nameof(ControlCardViewModel.ObjectId))
            {
                if (cardVm.PreviousObjectId.HasValue &&
                    cardVm.PreviousObjectId.Value != cardVm.ObjectId)
                {
                    cardVm.AcceptObjectIdMigration();
                }
                return;
            }

            if (e.PropertyName == nameof(ControlCardViewModel.AxisCount) ||
                e.PropertyName == nameof(ControlCardViewModel.CadID) ||
                e.PropertyName == nameof(ControlCardViewModel.MotionCardName))
            {
                RefreshLinkedViews(refreshSelection: false);
            }

            if (_isInitializing)
                return;

            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        private void RefreshLinkedViews(bool refreshAxisAndIo = true, bool refreshSelection = true)
        {
            if (!refreshAxisAndIo)
                return;

            AxisConfigViewModel.SyncFromCards();
            IODeviceCfgViewModel.RefreshAvailableControlCards();
        }

        private ControlCardViewModel CreateControlCardViewModel(
            BackendMotionControlCardInformations info,
            int displayIndex,
            BackendEletronicManagerSubMachineModulesFactoryCfg factoryCfg,
            string? persistedMotionCardName)
        {
            var savedAxisCount = ResolveAxisCountForCard(info, factoryCfg)
                ?? factoryCfg.AxisInformations?.Count(x => x.MotionCardGuid == info.MotionCardID)
                ?? 1;
            var driverDefinition = _motionCardDriverDefinitions.FirstOrDefault(x => x.MotionCardType == info.MotionCardType);
            var savedStateChannelCount = ResolveStateChannelCountForCard(info);
            var savedAnalogChannelCount = ResolveAnalogChannelCountForCard(info);

            var vm = new ControlCardViewModel
            {
                MotionCardCustomID = BuildFrontCardCustomId(displayIndex),
                MotionCardName = string.IsNullOrWhiteSpace(persistedMotionCardName) ? "新增运控卡" : persistedMotionCardName,
                CadID = info.MotionCardID,
                AxisCount = savedAxisCount,
            };
            var savedMotionCardName = vm.MotionCardName;

            vm.ApplyDriverDefinitions(_motionCardDriverDefinitions);
            vm.SetMotionCardType(info.MotionCardType);
            vm.AxisCount = savedAxisCount;
            vm.MotionCardCustomIDViewModel.Text = vm.MotionCardCustomID;
            vm.CardCountViewModel.Text = "1";
            vm.SupportChannelCountViewModel.Text = savedStateChannelCount.ToString();
            vm.StateChannelCountInputViewModel.Text = savedStateChannelCount.ToString();
            vm.AnalogChannelCountInputViewModel.Text = savedAnalogChannelCount.ToString();
            vm.SupportAxisCountViewModel.Text = savedAxisCount.ToString();
            vm.CadIDViewModel.Text = vm.CadID.ToString();
            vm.AxisCountViewModel.Value = savedAxisCount;
            vm.StateChannelCountViewModel.Value = savedStateChannelCount;
            vm.AnalogChannelCountViewModel.Value = savedAnalogChannelCount;
            vm.SetPersistedMotionCardName(savedMotionCardName);
            vm.EnsureObjectId();
            vm.AcceptObjectIdMigration();

            return vm;
        }

        private static Dictionary<Guid, string> ExtractPersistedMotionCardNames(byte[] data)
        {
            var result = new Dictionary<Guid, string>();
            if (data == null || data.Length == 0)
                return result;

            try
            {
                var root = JsonNode.Parse(data);
                var cardNodes = root?["EletronicFactoryParameters"]?["MotionControlCardInformations"]?.AsArray();
                if (cardNodes == null)
                    return result;

                foreach (var cardNode in cardNodes)
                {
                    if (cardNode is not JsonObject cardObject)
                        continue;

                    var cardIdText = cardObject["MotionCardID"]?.GetValue<string>();
                    var cardName = cardObject["MotionCardName"]?.GetValue<string>();
                    if (Guid.TryParse(cardIdText, out var cardId) && !string.IsNullOrWhiteSpace(cardName))
                        result[cardId] = cardName.Trim();
                }
            }
            catch
            {
                return result;
            }

            return result;
        }

        private static byte[] SerializeFactoryCfgWithMotionCardNames(
            BackendEletronicManagerSubMachineModulesFactoryCfg factoryCfg,
            IEnumerable<ControlCardViewModel> cards)
        {
            var raw = JsonObjConvert.ToJSonBytes(factoryCfg) ?? Array.Empty<byte>();
            if (raw.Length == 0)
                return raw;

            try
            {
                var namesById = cards
                    .Where(x => x != null)
                    .GroupBy(x => x.CadID)
                    .ToDictionary(
                        g => g.Key,
                        g => (g.Last().MotionCardName ?? string.Empty).Trim());

                var root = JsonNode.Parse(raw);
                var cardNodes = root?["EletronicFactoryParameters"]?["MotionControlCardInformations"]?.AsArray();
                if (cardNodes == null)
                    return raw;

                foreach (var cardNode in cardNodes)
                {
                    if (cardNode is not JsonObject cardObject)
                        continue;

                    var cardIdText = cardObject["MotionCardID"]?.GetValue<string>();
                    if (!Guid.TryParse(cardIdText, out var cardId))
                        continue;

                    if (namesById.TryGetValue(cardId, out var cardName))
                        cardObject["MotionCardName"] = cardName;
                }

                return Encoding.UTF8.GetBytes(root.ToJsonString(new JsonSerializerOptions
                {
                    WriteIndented = false
                }));
            }
            catch
            {
                return raw;
            }
        }

        private void ResetCards()
        {
            foreach (var cardVm in ControlCardListViewModel.ControlCardList.ToList())
            {
                DetachControlCard(cardVm);
            }

            ControlCardListViewModel.ControlCardList.Clear();
        }

        private static void EnsureUniqueMotionCardId(ControlCardViewModel cardVm, HashSet<Guid> usedCardIds)
        {
            var current = cardVm.CadID;
            var shouldReplace =
                current == Guid.Empty ||
                IsLegacyTypeDefaultGuid(current) ||
                usedCardIds.Contains(current);

            if (shouldReplace)
            {
                current = Guid.NewGuid();
                cardVm.CadID = current;
                cardVm.CadIDViewModel.Text = current.ToString();
            }

            usedCardIds.Add(current);
        }

        private static bool IsLegacyTypeDefaultGuid(Guid guid)
        {
            return guid == Guid.Parse("A6607528-B306-4498-8076-039477C8F9AA") ||
                   guid == Guid.Parse("D250700C-F406-4440-9E6B-17A3B0183AAB");
        }

        private void RefreshMotionCardDriverDefinitions()
        {
            _motionCardDriverDefinitions = QueryMotionCardDriverDefinitionsFromBackend();
            if (_motionCardDriverDefinitions.Count == 0)
                _motionCardDriverDefinitions = ControlCardViewModel.CreateFallbackDriverDefinitions();

            foreach (var card in ControlCardListViewModel.ControlCardList)
            {
                var axisCount = card.AxisCount;
                var stateChannelCount = card.StateChannelCountInputViewModel.Text;
                var analogChannelCount = card.AnalogChannelCountInputViewModel.Text;
                var supportChannelCount = card.SupportChannelCountViewModel.Text;
                var supportAxisCount = card.SupportAxisCountViewModel.Text;

                card.ApplyDriverDefinitions(_motionCardDriverDefinitions);
                card.AxisCount = axisCount;
                card.SupportAxisCountViewModel.Text = string.IsNullOrWhiteSpace(supportAxisCount)
                    ? axisCount.ToString()
                    : supportAxisCount;
                card.AxisCountViewModel.Value = axisCount;

                card.StateChannelCountInputViewModel.Text = stateChannelCount;
                card.AnalogChannelCountInputViewModel.Text = analogChannelCount;
                card.SupportChannelCountViewModel.Text = supportChannelCount;
            }
        }

        private List<ControlCardViewModel.MotionCardDriverDefinition> QueryMotionCardDriverDefinitionsFromBackend()
        {
            try
            {
                MotionControlPluginManager.Init();
                var dtoList = MotionControlPluginManager
                    .GetAllMontionControlDriverTypeInfo()
                    .Select(x => CreateMotionCardDriverTypeDto(x))
                    .ToList();
                var definitions = dtoList
                    .Where(x => !string.IsNullOrWhiteSpace(x.TypeValue))
                    .Select(x =>
                    {
                        var parsedType = Enum.TryParse(x.TypeValue, true, out GKG.MotionCardType motionCardType)
                            ? motionCardType
                            : x.MotionCardType;
                        /*

                        var kindDisplay = string.IsNullOrWhiteSpace(x.DriverName) ? "动态驱动" : x.DriverName;
                        //
                        var kindDisplay = string.IsNullOrWhiteSpace(x.DriverName) ? "动态驱动" : x.DriverName;
                        */
                        var kindValue = string.IsNullOrWhiteSpace(x.MotionControlCardTypeValue)
                            ? "DynamicDriver"
                            : x.MotionControlCardTypeValue;
                        var kindDisplay = string.IsNullOrWhiteSpace(x.DriverName)
                            ? kindValue
                            : x.DriverName;
                        var typeDisplay = string.IsNullOrWhiteSpace(x.MotionControlName) ? x.TypeValue : x.MotionControlName;

                        return new ControlCardViewModel.MotionCardDriverDefinition
                        {
                            KindValue = kindValue,
                            KindDisplay = kindDisplay,
                            TypeValue = x.TypeValue,
                            TypeDisplay = typeDisplay,
                            MotionCardType = parsedType,
                            DefaultCadId = x.DefaultCadId,
                            SupportAxisNum = x.SupportAxisNum > 0 ? x.SupportAxisNum : 16,
                            SupportIoStateNum = x.SupportIoStateNum >= 0 ? x.SupportIoStateNum : 4,
                            SupportAnalogNum = x.SupportAnalogNum >= 0 ? x.SupportAnalogNum : 4,
                        };
                    })
                    .ToList();

                return definitions;
            }
            catch
            {
                return new List<ControlCardViewModel.MotionCardDriverDefinition>();
            }
        }

        private static MotionCardDriverTypeDto CreateMotionCardDriverTypeDto(object driverInfo)
        {
            var driverType = driverInfo.GetType();
            var motionCardTypeValue = driverType.GetProperty("MotionCardType")?.GetValue(driverInfo);
            var motionCardType = motionCardTypeValue is GKG.MotionCardType cardType
                ? cardType
                : Enum.TryParse(motionCardTypeValue?.ToString(), true, out GKG.MotionCardType parsedType)
                    ? parsedType
                    : GKG.MotionCardType.GMCMINI;

            var defaultConfig = TryGetDefaultConfig(motionCardType);
            var runtimeCapability = TryGetRuntimeCapability(motionCardType);
            var supportAxisNum = ResolveDriverCapability(
                defaultConfig?.SupportAxisNum,
                runtimeCapability?.SupportAxisNum,
                16,
                allowZero: false);
            var supportIoStateNum = ResolveDriverCapability(
                defaultConfig?.SupportIoStateNum,
                runtimeCapability?.SupportIoStateNum,
                0,
                allowZero: true);
            var supportAnalogNum = ResolveDriverCapability(
                defaultConfig?.SupportAnalogNum,
                runtimeCapability?.SupportAnalogNum,
                0,
                allowZero: true);

            return new MotionCardDriverTypeDto
            {
                TypeValue = motionCardType.ToString(),
                MotionControlName = driverType.GetProperty("MotionControlName")?.GetValue(driverInfo)?.ToString() ?? string.Empty,
                DriverName = driverType.GetProperty("DriverName")?.GetValue(driverInfo)?.ToString() ?? string.Empty,
                MotionControlCardTypeValue = driverType.GetProperty("MotionControlCardType")?.GetValue(driverInfo)?.ToString() ?? string.Empty,
                MotionCardType = motionCardType,
                DefaultCadId = Guid.Empty,
                SupportAxisNum = supportAxisNum,
                SupportIoStateNum = supportIoStateNum,
                SupportAnalogNum = supportAnalogNum,
            };
        }

        private static MotionControlDriverDefaultConfig? TryGetDefaultConfig(GKG.MotionCardType motionCardType)
        {
            try
            {
                return MotionControlPluginManager.GetDefaultConfig(motionCardType);
            }
            catch
            {
                return null;
            }
        }

        private static MotionControlDriverRuntimeCapability? TryGetRuntimeCapability(GKG.MotionCardType motionCardType)
        {
            try
            {
                var motionControl = MotionControlPluginManager.GetMotionControl(motionCardType);
                if (motionControl == null)
                    return null;

                return new MotionControlDriverRuntimeCapability
                {
                    SupportAxisNum = Math.Max(0, motionControl.SupportAxisNum),
                    SupportIoStateNum = Math.Max(0, motionControl.SupportIoStateNum),
                    SupportAnalogNum = Math.Max(0, motionControl.SupportAnalogNum),
                };
            }
            catch
            {
                return null;
            }
        }

        private static int ResolveDriverCapability(int? defaultConfigValue, int? runtimeValue, int fallbackValue, bool allowZero)
        {
            if (defaultConfigValue.HasValue)
            {
                if (allowZero)
                {
                    if (defaultConfigValue.Value >= 0)
                        return defaultConfigValue.Value;
                }
                else if (defaultConfigValue.Value > 0)
                {
                    return defaultConfigValue.Value;
                }
            }

            if (runtimeValue.HasValue)
            {
                if (allowZero)
                {
                    if (runtimeValue.Value >= 0)
                        return runtimeValue.Value;
                }
                else if (runtimeValue.Value > 0)
                {
                    return runtimeValue.Value;
                }
            }

            return fallbackValue;
        }

        private sealed class MotionCardDriverTypeDto
        {
            public string TypeValue { get; set; } = string.Empty;
            public string MotionControlName { get; set; } = string.Empty;
            public string DriverName { get; set; } = string.Empty;
            public string MotionControlCardTypeValue { get; set; } = string.Empty;
            public GKG.MotionCardType MotionCardType { get; set; }
            public Guid DefaultCadId { get; set; }
            public int SupportAxisNum { get; set; }
            public int SupportIoStateNum { get; set; }
            public int SupportAnalogNum { get; set; }
        }

        private sealed class MotionControlDriverRuntimeCapability
        {
            public int SupportAxisNum { get; set; }
            public int SupportIoStateNum { get; set; }
            public int SupportAnalogNum { get; set; }
        }

        private Window? TryGetOwnerWindow()
        {
            return _viewReference?.GetVisualRoot() as Window;
        }
    }
}
