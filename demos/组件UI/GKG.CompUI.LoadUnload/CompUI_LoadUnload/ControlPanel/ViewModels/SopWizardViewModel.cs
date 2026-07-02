using GKG.MM;
using Griffins;
using Griffins.ImeIOT;
using Griffins.Map.UI;
using Griffins.PF;
using Griffins.PF.RichClient;
using Newtonsoft.JsonG;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace GKG.CompUI.LoadUnload.ControlPanel.ViewModels
{
    /// <summary>
    /// 标准作业流程 (SOP) 向导模式
    /// </summary>
    public enum SopWizardMode
    {
        /// <summary>一键上料</summary>
        Load,
        /// <summary>一键下料</summary>
        Unload,
        /// <summary>一键抽检</summary>
        Inspect
    }

    /// <summary>
    /// 一键上料向导视图模型：管理向导流程、硬件交互及 UI 状态机
    /// </summary>
    public class SopWizardViewModel : ReactiveObject
    {
        #region 字段与服务依赖

        private readonly IControlPanelCallBack _controlPanelCallBack;
        private readonly HashSet<MagazineViewModel> _feedActionInProgress = new();

        // 使用 Subject 管理向外发射的事件流
        private readonly Subject<Unit> _closeRequested = new Subject<Unit>();

        /// <summary>
        /// 关闭向导窗口的事件流
        /// </summary>
        public IObservable<Unit> CloseRequested => _closeRequested;

        /// <summary>
        /// 上料容器集合（驱动界面的核心数据源）
        /// </summary>
        public ObservableCollection<StorageContainerViewModel> Containers { get; } = new();

        /// <summary>
        /// 获取所有容器内的所有料盒（扁平化查询便利属性）
        /// </summary>
        private IEnumerable<MagazineViewModel> AllMagazines => Containers.SelectMany(c => c.Magazines);

        #endregion

        #region 动态模式配置属性

        /// <summary>
        /// 当前作业模式
        /// </summary>
        public SopWizardMode CurrentMode { get; }

        /// <summary>
        /// 向导界面的主标题
        /// </summary>
        public string WizardTitle { get; } = string.Empty;

        /// <summary>
        /// 向导界面的 Emoji 图标
        /// </summary>
        public string WizardIcon { get; } = string.Empty;

        /// <summary>
        /// 过滤后端推送数据的 Type 字段
        /// </summary>
        public string BackendStatusFilter { get; } = string.Empty;

        /// <summary>
        /// 是否为一键上料向导（料盒卡片显示上料三步按钮）
        /// </summary>
        public bool IsLoadWizardMode => CurrentMode == SopWizardMode.Load;

        public bool IsUnloadWizardMode => CurrentMode == SopWizardMode.Unload;

        /// <summary>抽检模式：料盒卡片显示抽检三步按钮及到位/夹紧指示</summary>
        public bool IsInspectWizardMode => CurrentMode == SopWizardMode.Inspect;

        /// <summary>兼容旧绑定名</summary>
        public bool IsUnlockWizardMode => IsInspectWizardMode;

        /// <summary>上料/下料模式：卡片不显示到位/夹紧灯，完成时再读到位</summary>
        public bool IsFeedWizardMode => CurrentMode is SopWizardMode.Load or SopWizardMode.Unload;

        #endregion

        #region 底部导航栏与提示属性

        private string _systemMessage = "请直接在需要更换的料盒卡片上点击松开";
        public string SystemMessage
        {
            get => _systemMessage;
            set => this.RaiseAndSetIfChanged(ref _systemMessage, value);
        }

        private bool _isErrorAlert;
        /// <summary>
        /// 是否为错误告警（控制横幅变红）
        /// </summary>
        public bool IsErrorAlert
        {
            get => _isErrorAlert;
            set => this.RaiseAndSetIfChanged(ref _isErrorAlert, value);
        }

        private double _messageOpacity = 1.0;
        /// <summary>
        /// 提示信息的透明度（用于触发 UI 闪烁动画）
        /// </summary>
        public double MessageOpacity
        {
            get => _messageOpacity;
            set => this.RaiseAndSetIfChanged(ref _messageOpacity, value);
        }

        #endregion

        #region 内联弹窗状态属性

        private bool _isDialogOpen;
        public bool IsDialogOpen
        {
            get => _isDialogOpen;
            set => this.RaiseAndSetIfChanged(ref _isDialogOpen, value);
        }

        private string _dialogTitle = string.Empty;
        public string DialogTitle
        {
            get => _dialogTitle;
            set => this.RaiseAndSetIfChanged(ref _dialogTitle, value);
        }

        private string _dialogMessage = string.Empty;
        public string DialogMessage
        {
            get => _dialogMessage;
            set => this.RaiseAndSetIfChanged(ref _dialogMessage, value);
        }

        private bool _isAlertDialogOnly;
        /// <summary>仅提示型弹窗（如料盒未到位），无确认执行</summary>
        public bool IsAlertDialogOnly
        {
            get => _isAlertDialogOnly;
            set => this.RaiseAndSetIfChanged(ref _isAlertDialogOnly, value);
        }

        #endregion


        #region 交互命令 (Commands)

        public ReactiveCommand<Unit, Unit> CancelDialogCommand { get; }
        public ReactiveCommand<MagazineViewModel, Unit> LoadFeedMagazineCommand { get; }
        public ReactiveCommand<MagazineViewModel, Unit> UnloadFeedMagazineCommand { get; }
        public ReactiveCommand<MagazineViewModel, Unit> InspectFeedMagazineCommand { get; }
        public ReactiveCommand<Unit, Unit> CloseCommand { get; }
        public ReactiveCommand<Unit, Unit> ToggleAllContainersCommand { get; }

        #endregion

        #region 构造函数与模式初始化

        public SopWizardViewModel(IControlPanelCallBack controlPanelCallBack, SopWizardMode mode)
        {
            _controlPanelCallBack = controlPanelCallBack;
            CurrentMode = mode;

            // 根据传入的模式，动态配置界面文案与后端过滤条件
            switch (mode)
            {
                case SopWizardMode.Load:
                    WizardTitle = "上料作业向导";
                    WizardIcon = "📥";
                    BackendStatusFilter = "上料";
                    SystemMessage = "各料盒到位且夹紧后，请按卡片按钮：开始上料 → 上料完成";
                    break;
                case SopWizardMode.Unload:
                    WizardTitle = "下料作业向导";
                    WizardIcon = "📤";
                    BackendStatusFilter = "下料";
                    SystemMessage = "各料盒到位且夹紧后，请按卡片按钮：开始下料 → 下料完成";
                    break;
                case SopWizardMode.Inspect:
                    WizardTitle = "抽检作业向导";
                    WizardIcon = "🔍";
                    BackendStatusFilter = "抽检";
                    SystemMessage = "各料盒到位且夹紧后，请按卡片按钮：开始抽检 → 抽检完成";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

            // 注册底层设备状态变更回调
            ClientInfoProcessRegister.RegisterSvrInfoProcessDelegate<InformInfo_StatusChanged>(OnCompStatusChanged);

            // TODO: 测试阶段，生成模拟数据 (对接真机时可移除)
            //GenerateMockData();

            // 基础 UI 命令初始化
            ToggleAllContainersCommand = ReactiveCommand.Create(ExecuteToggleAllContainers);
            CancelDialogCommand = ReactiveCommand.Create(ExecuteCancelDialog);
            CloseCommand = ReactiveCommand.Create(ExecuteCloseRequest);

            LoadFeedMagazineCommand = ReactiveCommand.CreateFromTask<MagazineViewModel>(
                ExecuteLoadFeedMagazineAsync,
                outputScheduler: RxApp.MainThreadScheduler);
            UnloadFeedMagazineCommand = ReactiveCommand.CreateFromTask<MagazineViewModel>(
                ExecuteUnloadFeedMagazineAsync,
                outputScheduler: RxApp.MainThreadScheduler);
            InspectFeedMagazineCommand = ReactiveCommand.CreateFromTask<MagazineViewModel>(
                ExecuteInspectFeedMagazineAsync,
                outputScheduler: RxApp.MainThreadScheduler);

            GetInitDatas(mode);
            ResetFeedWorkflowPhasesOnEnter();
        }

        ~SopWizardViewModel()
        {
            ClientInfoProcessRegister.UnRegisterInfoProcessDelegate(InformInfo_StatusChanged.InfoKindID, OnCompStatusChanged);
        }

        #endregion

        #region 命令执行逻辑 (Command Handlers)

        private void ExecuteToggleAllContainers()
        {
            if (!Containers.Any()) return;
            bool targetState = !Containers.Any(c => c.IsExpanded);
            foreach (var container in Containers) container.IsExpanded = targetState;
        }

        private void ExecuteCancelDialog()
        {
            IsDialogOpen = false;
            IsAlertDialogOnly = false;
        }

        private void ExecuteCloseRequest()
        {
            _closeRequested.OnNext(Unit.Default);
        }

        private void ShowMagazineNotPresentAlert(MagazineViewModel mag, string completeActionName)
        {
            DialogTitle = "料盒未到位";
            DialogMessage = $"【{mag.Name}】到位信号为空，请将料盒推入到位后，再点击「{completeActionName}」。";
            IsAlertDialogOnly = true;
            IsDialogOpen = true;
            ShowMessage($"⚠️【{mag.Name}】料盒未到位，无法夹紧！", true);
        }

        private async Task ExecuteLoadFeedMagazineAsync(MagazineViewModel mag)
        {
            if (mag == null || CurrentMode != SopWizardMode.Load)
                return;

            var parentContainer = Containers.FirstOrDefault(c => c.Magazines.Contains(mag));
            string containerName = parentContainer?.Name ?? "未知容器";

            switch (mag.LoadPhase)
            {
                case MagazineLoadPhase.Ready:
                    if (!_feedActionInProgress.Add(mag))
                        return;
                    try
                    {
                        // 严格按时序：先确认气缸松开，再切换按钮到“上料完成”
                        if (await EnsureMagazineUnclampedAsync(mag, containerName, "上料"))
                        {
                            mag.LoadPhase = MagazineLoadPhase.AwaitingConfirm;
                            ShowMessage($"【{mag.Name}】请取放物料，完成后点击「上料完成」");
                        }
                    }
                    finally
                    {
                        _feedActionInProgress.Remove(mag);
                    }
                    break;

                case MagazineLoadPhase.Finished:
                    mag.LoadPhase = MagazineLoadPhase.Ready;
                    ShowMessage($"【{mag.Name}】可开始新一轮上料，请点击「开始上料」");
                    break;

                case MagazineLoadPhase.AwaitingConfirm:
                    // 允许多次点击「上料完成」，直到到位并夹紧
                    if (!await TryConfirmPresentAndClampAsync(mag, containerName, "上料完成"))
                    {
                        if (!await UpdateMaterialBoxStateAsync(mag, containerName, isFeedingContainer: true))
                        {
                            ShowMessage($"⚠️【{mag.Name}】底层料盒状态更新失败，请检查通信后重试！", true);
                            break;
                        }

                        // 成功后立刻回到“开始上料”（闭环），界面状态以后端回读结果为准
                        mag.LoadPhase = MagazineLoadPhase.Ready;
                        ShowMessage($"【{mag.Name}】上料已完成，料盒状态已同步，可开始新一轮上料，请点击「开始上料」");
                    }
                    break;
            }
        }

        private async Task ExecuteUnloadFeedMagazineAsync(MagazineViewModel mag)
        {
            if (mag == null || CurrentMode != SopWizardMode.Unload)
                return;

            var parentContainer = Containers.FirstOrDefault(c => c.Magazines.Contains(mag));
            string containerName = parentContainer?.Name ?? "未知容器";

            switch (mag.UnloadPhase)
            {
                case MagazineUnloadPhase.Ready:
                    if (!_feedActionInProgress.Add(mag))
                        return;
                    try
                    {
                        // 严格按时序：先确认气缸松开，再切换按钮到“下料完成”
                        if (await EnsureMagazineUnclampedAsync(mag, containerName, "下料"))
                        {
                            mag.UnloadPhase = MagazineUnloadPhase.AwaitingConfirm;
                            ShowMessage($"【{mag.Name}】请取出物料，完成后点击「下料完成」");
                        }
                    }
                    finally
                    {
                        _feedActionInProgress.Remove(mag);
                    }
                    break;

                case MagazineUnloadPhase.Finished:
                    mag.UnloadPhase = MagazineUnloadPhase.Ready;
                    ShowMessage($"【{mag.Name}】可开始新一轮下料，请点击「开始下料」");
                    break;

                case MagazineUnloadPhase.AwaitingConfirm:
                    if (!await TryConfirmPresentAndClampAsync(mag, containerName, "下料完成"))
                    {
                        if (!await UpdateMaterialBoxStateAsync(mag, containerName, isFeedingContainer: false))
                        {
                            ShowMessage($"⚠️【{mag.Name}】底层料盒状态更新失败，请检查通信后重试！", true);
                            break;
                        }

                        // 成功后立刻回到“开始下料”（闭环），界面状态以后端回读结果为准
                        mag.UnloadPhase = MagazineUnloadPhase.Ready;
                        ShowMessage($"【{mag.Name}】下料已完成，料盒状态已同步，可开始新一轮下料，请点击「开始下料」");
                    }
                    break;
            }
        }

        private async Task ExecuteInspectFeedMagazineAsync(MagazineViewModel mag)
        {
            if (mag == null || CurrentMode != SopWizardMode.Inspect)
                return;

            var parentContainer = Containers.FirstOrDefault(c => c.Magazines.Contains(mag));
            string containerName = parentContainer?.Name ?? "未知容器";

            switch (mag.InspectPhase)
            {
                case MagazineInspectPhase.Ready:
                    if (!_feedActionInProgress.Add(mag))
                        return;
                    try
                    {
                        // 严格按时序：先确认气缸松开，再切换按钮到“抽检完成”
                        if (await EnsureMagazineUnclampedAsync(mag, containerName, "抽检"))
                        {
                            mag.InspectPhase = MagazineInspectPhase.AwaitingConfirm;
                            ShowMessage($"【{mag.Name}】请进行抽检操作，完成后点击「抽检完成」");
                        }
                    }
                    finally
                    {
                        _feedActionInProgress.Remove(mag);
                    }
                    break;

                case MagazineInspectPhase.Finished:
                    mag.InspectPhase = MagazineInspectPhase.Ready;
                    ShowMessage($"【{mag.Name}】可开始新一轮抽检，请点击「开始抽检」");
                    break;

                case MagazineInspectPhase.AwaitingConfirm:
                    if (!await TryConfirmPresentAndClampAsync(mag, containerName, "抽检完成"))
                    {
                        // 成功后立刻回到“开始抽检”（闭环）
                        mag.InspectPhase = MagazineInspectPhase.Ready;
                        TryRefreshMagazineFromBackend(mag); // 刷新抽检结果展示
                        ShowMessage($"【{mag.Name}】抽检已完成，可开始新一轮抽检，请点击「开始抽检」");
                    }
                    break;
            }
        }

        /// <summary>
        /// 进入向导时将所有料盒作业按钮重置为「开始…」
        /// </summary>
        private void ResetFeedWorkflowPhasesOnEnter()
        {
            foreach (var mag in AllMagazines)
            {
                if (CurrentMode == SopWizardMode.Load)
                    mag.LoadPhase = MagazineLoadPhase.Ready;
                else if (CurrentMode == SopWizardMode.Unload)
                    mag.UnloadPhase = MagazineUnloadPhase.Ready;
                else if (CurrentMode == SopWizardMode.Inspect)
                    mag.InspectPhase = MagazineInspectPhase.Ready;
            }
        }

        /// <summary>
        /// 松开气缸：已松开则跳过；否则下发指令并短时等待传感器（不阻塞按钮文案切换）
        /// </summary>
        private async Task<bool> EnsureMagazineUnclampedAsync(
            MagazineViewModel mag,
            string containerName,
            string actionLabel)
        {
            // 先用后端刷新一次，避免因本地状态过旧跳过下发
            bool refreshed = TryRefreshMagazineFromBackend(mag);

            // 只有在确认“已松开”时才跳过指令；刷新失败则仍尝试下发一次以保证时序正确
            if (!mag.IsClamped && refreshed)
            {
                ShowMessage($"【{mag.Name}】气缸已处于松开状态");
                return true;
            }

            string storageDeviceName = ResolveStorageDeviceNameForMagazine(mag.Name, containerName);
            ShowMessage($"正在松开【{mag.Name}】气缸...");
            if (!await UnclampMagAsync(storageDeviceName, mag.Name))
            {
                ShowMessage($"⚠️【{mag.Name}】松开指令下发失败，请检查设备！", true);
                return false;
            }

            if (await WaitMagazinePhysicalStateFromBackendAsync(mag, expectClamped: false, timeoutMs: 5000))
            {
                ShowMessage($"【{mag.Name}】气缸已松开");
                return true;
            }

            ShowMessage($"⚠️【{mag.Name}】松开超时，请确认夹紧灯熄灭后继续操作", true);
            return false;
        }

        /// <summary>
        /// 点击「上料/下料完成」：先主动读取到位，未到位则弹窗并中止；到位后再夹紧并等待推送确认。
        /// </summary>
        /// <returns>true 表示应中止后续阶段推进（失败或未到位）</returns>
        private async Task<bool> TryConfirmPresentAndClampAsync(
            MagazineViewModel mag,
            string containerName,
            string completeActionName)
        {
            ShowMessage($"正在读取【{mag.Name}】到位状态...");

            // 夹紧前必须以后端实时 IsEmpty（料盒感应）为准，未到位绝不下发夹紧
            if (!await ConfirmMagazinePresentFromBackendAsync(mag))
            {
                ShowMagazineNotPresentAlert(mag, completeActionName);
                return true;
            }

            string storageDeviceName = ResolveStorageDeviceNameForMagazine(mag.Name, containerName);

            if (!mag.IsClamped)
            {
                // 下发夹紧指令前再次确认到位，防止轮询间隙料盒被取出
                if (!await ConfirmMagazinePresentFromBackendAsync(mag))
                {
                    ShowMagazineNotPresentAlert(mag, completeActionName);
                    return true;
                }

                ShowMessage($"【{mag.Name}】已到位，正在夹紧...");
                if (!await ClampMagAsync(storageDeviceName, mag.Name))
                {
                    ShowMessage($"⚠️【{mag.Name}】夹紧指令下发失败，请检查设备！", true);
                    return true;
                }
            }

            ShowMessage($"等待【{mag.Name}】传感器确认已夹紧...");
            if (await WaitMagazineClampedWithPresentGuardAsync(mag, timeoutMs: 5000))
            {
                string flowName = completeActionName switch
                {
                    "上料完成" => "上料",
                    "下料完成" => "下料",
                    "抽检完成" => "抽检",
                    _ => "作业"
                };
                ShowMessage($"【{mag.Name}】{flowName}夹紧已确认");
                return false;
            }

            if (!mag.IsPresent)
            {
                ShowMagazineNotPresentAlert(mag, completeActionName);
                return true;
            }

            ShowMessage($"⚠️【{mag.Name}】夹紧超时，请检查设备与安全门！", true);
            return true;
        }

        /// <summary>
        /// 主动向后端查询料盒是否物理到位（IsEmpty=false 表示感应到位）。
        /// </summary>
        private async Task<bool> ConfirmMagazinePresentFromBackendAsync(MagazineViewModel mag)
        {
            const int sampleCount = 3;
            const int intervalMs = 120;

            for (int i = 0; i < sampleCount; i++)
            {
                if (!TryFetchBackendMaterialBox(mag, out MaterialBoxStatus? backBox) || backBox == null)
                {
                    if (i == sampleCount - 1)
                    {
                        ShowMessage($"⚠️ 无法读取【{mag.Name}】状态，请检查通信！", true);
                        return false;
                    }

                    await Task.Delay(intervalMs);
                    continue;
                }

                ApplyBackendMaterialBox(mag, backBox);
                if (!IsMagazinePhysicallyPresent(backBox))
                    return false;

                if (i < sampleCount - 1)
                    await Task.Delay(intervalMs);
            }

            return mag.IsPresent;
        }

        /// <summary>
        /// 等待夹紧期间持续刷新；若到位信号丢失则立即中止。
        /// </summary>
        private async Task<bool> WaitMagazineClampedWithPresentGuardAsync(
            MagazineViewModel mag,
            int timeoutMs = 5000,
            int intervalMs = 200)
        {
            int elapsed = 0;
            while (elapsed < timeoutMs)
            {
                if (TryFetchBackendMaterialBox(mag, out MaterialBoxStatus? backBox) && backBox != null)
                {
                    ApplyBackendMaterialBox(mag, backBox);
                    if (!IsMagazinePhysicallyPresent(backBox))
                        return false;
                }

                if (mag.IsClamped)
                    return true;

                await Task.Delay(intervalMs);
                elapsed += intervalMs;
            }

            if (TryFetchBackendMaterialBox(mag, out MaterialBoxStatus? finalBox) && finalBox != null)
            {
                ApplyBackendMaterialBox(mag, finalBox);
                return mag.IsClamped && IsMagazinePhysicallyPresent(finalBox);
            }

            return mag.IsClamped && mag.IsPresent;
        }

        private static bool IsMagazinePhysicallyPresent(MaterialBoxStatus backBox) => !backBox.IsEmpty;

        /// <summary>
        /// 由料盒名解析储料装置名，供 StorageOpen/Close 命令路由。
        /// </summary>
        private static string ResolveStorageDeviceNameForMagazine(string magName, string fallbackContainerName)
        {
            if (magName.StartsWith("LoadStorageDevice", StringComparison.OrdinalIgnoreCase))
                return "LoadStorageDevice";

            if (magName.StartsWith("UnloadStorageDevice", StringComparison.OrdinalIgnoreCase))
                return "UnloadStorageDevice";

            if (string.Equals(fallbackContainerName, "LoadStorageDevice", StringComparison.OrdinalIgnoreCase)
                || string.Equals(fallbackContainerName, "UnloadStorageDevice", StringComparison.OrdinalIgnoreCase))
                return fallbackContainerName;

            return fallbackContainerName;
        }

        /// <summary>
        /// 轮询等待：在等待期间主动刷新后端状态，避免因仅依赖推送导致超时。
        /// </summary>
        private async Task<bool> WaitMagazinePhysicalStateFromBackendAsync(
            MagazineViewModel mag,
            bool expectClamped,
            int timeoutMs = 5000,
            int intervalMs = 200)
        {
            int elapsed = 0;
            while (elapsed < timeoutMs)
            {
                TryRefreshMagazineFromBackend(mag); // best-effort

                if (mag.IsClamped == expectClamped)
                    return true;

                await Task.Delay(intervalMs);
                elapsed += intervalMs;
            }

            TryRefreshMagazineFromBackend(mag); // best-effort
            return mag.IsClamped == expectClamped;
        }

        #endregion

        #region 硬件通信与底层接口

        private async Task<bool> UpdateMaterialBoxStateAsync(
            MagazineViewModel mag,
            string containerName,
            bool isFeedingContainer)
        {
            try
            {
                // 直接使用料盒的 RackId 编号（0-3）
                if (mag.RackId < 0 || mag.RackId > 3)
                {
                    return false;
                }

                var cmdParam = new GFBaseTypeParamValueList
                {
                    new GFBaseTypeParamValue("MaterialRack", new GriffinsBaseValue(mag.RackId.ToString())),
                    new GFBaseTypeParamValue("IsFeeding", new GriffinsBaseValue(isFeedingContainer.ToString()))
                };

                var result = await Task.Run(() =>
                    _controlPanelCallBack.ExecNormalCtlCmd(
                        LoadUnloadMachineModulesConst.UpdateMaterialBoxStateMethodID,
                        cmdParam));

                if (result == null || result["result"]?.ToString() == "-1")
                    return false;

                if (TryApplyMagazineFromResult(mag, result))
                {
                    SyncFeedPhaseWithCylinderState(mag);
                    return true;
                }

                return TryRefreshMagazineFromBackend(mag);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 主动查询后端并刷新指定料盒物理状态（到位/夹紧/槽位），用于「上料/下料完成」前判定。
        /// </summary>
        private bool TryRefreshMagazineFromBackend(MagazineViewModel mag)
        {
            if (!TryFetchBackendMaterialBox(mag, out MaterialBoxStatus? backBox) || backBox == null)
                return false;

            ApplyBackendMaterialBox(mag, backBox);
            SyncFeedPhaseWithCylinderState(mag);
            return true;
        }

        private bool TryApplyMagazineFromResult(MagazineViewModel mag, GFBaseTypeParamValueList result)
        {
            string? jsonStr = ExtractMaterialContainerStatusJson(result);
            var backendContainers = DeserializeMaterialContainers(jsonStr);
            var backBox = FindBackendMaterialBox(backendContainers, mag.Name);
            if (backBox == null)
                return false;

            ApplyBackendMaterialBox(mag, backBox);
            return true;
        }

        /// <summary>
        /// 从后端拉取指定料盒的最新状态（跨容器按料盒名匹配）。
        /// </summary>
        private bool TryFetchBackendMaterialBox(MagazineViewModel mag, out MaterialBoxStatus? backBox)
        {
            backBox = null;
            try
            {
                var cmdParam = new GFBaseTypeParamValueList();
                var result = _controlPanelCallBack.ExecNormalCtlCmd(
                    LoadUnloadMachineModulesConst.GetMaterialStatus, cmdParam);

                if (result == null || result["result"]?.ToString() == "-1")
                    return false;

                string? jsonStr = ExtractMaterialContainerStatusJson(result);
                var backendContainers = DeserializeMaterialContainers(jsonStr);
                backBox = FindBackendMaterialBox(backendContainers, mag.Name);
                return backBox != null;
            }
            catch
            {
                return false;
            }
        }

        private static string? ExtractMaterialContainerStatusJson(GFBaseTypeParamValueList result)
        {
            var value = result[LoadUnloadMachineModulesConst.MaterialContainerStatusPropertyID]?.Val;
            if (value == null)
                return null;

            if (value is ObjectValue_Json jsonValue && !string.IsNullOrWhiteSpace(jsonValue.JsonVal))
                return jsonValue.JsonVal;

            return value.ToString();
        }

        private static List<MaterialContainerStatus>? DeserializeMaterialContainers(string? jsonStr)
        {
            if (string.IsNullOrWhiteSpace(jsonStr))
                return null;

            try
            {
                var list = JsonConvert.DeserializeObject<List<MaterialContainerStatus>>(jsonStr);
                if (list != null && list.Count > 0)
                    return list;
            }
            catch
            {
                // 兼容单容器 JSON
            }

            try
            {
                var single = JsonConvert.DeserializeObject<MaterialContainerStatus>(jsonStr);
                if (single != null)
                    return new List<MaterialContainerStatus> { single };
            }
            catch
            {
                return null;
            }

            return null;
        }

        private static MaterialBoxStatus? FindBackendMaterialBox(
            List<MaterialContainerStatus>? containers,
            string magName)
        {
            if (containers == null || string.IsNullOrWhiteSpace(magName))
                return null;

            foreach (var container in containers)
            {
                if (container.materialBoxStatus == null)
                    continue;

                var box = container.materialBoxStatus.FirstOrDefault(
                    b => string.Equals(b.Name, magName, StringComparison.OrdinalIgnoreCase));
                if (box != null)
                    return box;
            }

            return null;
        }

        /// <summary>
        /// 从料盒名称和容器名称推断 MaterialRack 编号 (0=上料上层, 1=上料下层, 2=下料上层, 3=下料下层)
        /// </summary>
        private static int GetRackIdFromMagazineName(string magName, string containerName)
        {
            // 判断上料侧或下料侧
            bool isLoadSide;
            if (string.Equals(containerName, "LoadStorageDevice", StringComparison.OrdinalIgnoreCase) ||
                magName.StartsWith("LoadStorageDevice", StringComparison.OrdinalIgnoreCase))
            {
                isLoadSide = true;
            }
            else if (string.Equals(containerName, "UnloadStorageDevice", StringComparison.OrdinalIgnoreCase) ||
                     magName.StartsWith("UnloadStorageDevice", StringComparison.OrdinalIgnoreCase))
            {
                isLoadSide = false;
            }
            else
            {
                return -1; // 无法识别
            }

            // 判断上层或下层
            int storageIndex;
            if (magName.EndsWith("UpperRack", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(magName, "UpperRack", StringComparison.OrdinalIgnoreCase))
            {
                storageIndex = 0;
            }
            else if (magName.EndsWith("LowerRack", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(magName, "LowerRack", StringComparison.OrdinalIgnoreCase))
            {
                storageIndex = 1;
            }
            else
            {
                return -1; // 无法识别
            }

            // 映射到 MaterialRack 编号
            return isLoadSide
                ? (storageIndex == 0 ? 0 : 1)  // 上料上层=0, 上料下层=1
                : (storageIndex == 0 ? 2 : 3); // 下料上层=2, 下料下层=3
        }

        private void GetInitDatas(SopWizardMode mode)
        {
            try
            {
                var cmdParam = new GFBaseTypeParamValueList();

                var result = _controlPanelCallBack.ExecNormalCtlCmd(LoadUnloadMachineModulesConst.GetMaterialStatus, cmdParam);
                

                if (result != null && result["result"]?.ToString() != "-1")
                {
                    string? jsonStr = ExtractMaterialContainerStatusJson(result);
                    if (!string.IsNullOrWhiteSpace(jsonStr))
                    {
                        var backendContainers = DeserializeMaterialContainers(jsonStr);
                        if (backendContainers == null || !backendContainers.Any()) return;

                        foreach (var backContainer in backendContainers)
                        {
                            // 筛选上料 下料
                            bool isContinue = false;
                            switch(mode)
                            {
                                case SopWizardMode.Load:
                                    if (!backContainer.IsFeeding)
                                        isContinue = true;
                                    break;
                                case SopWizardMode.Unload:
                                    if (backContainer.IsFeeding)
                                        isContinue = true;
                                    break;
                                case SopWizardMode.Inspect:
                                default:
                                    isContinue = false;
                                    break;
                            }
                            if (isContinue)
                                continue;
                            var frontContainer = Containers.FirstOrDefault(c => c.Name == backContainer.Name);
                            if (frontContainer == null)
                            {
                                frontContainer = new StorageContainerViewModel
                                {
                                    Name = backContainer.Name,
                                    IsExpanded = true
                                };
                                Containers.Add(frontContainer);
                            }

                            foreach (var backBox in backContainer.materialBoxStatus)
                            {
                                var frontBox = frontContainer.Magazines.FirstOrDefault(m => m.Name == backBox.Name);
                                if (frontBox == null)
                                {
                                    frontBox = new MagazineViewModel 
                                    { 
                                        Name = backBox.Name,
                                        RackId = GetRackIdFromMagazineName(backBox.Name, backContainer.Name)
                                    };
                                    frontContainer.Magazines.Add(frontBox);
                                }

                                ApplyBackendMaterialBox(frontBox, backBox);
                                SyncFeedPhaseWithCylinderState(frontBox);
                            }
                        }
                    }
                   
                }
                
            }
            catch (Exception)
            {
                
            }
        }

        private void OnCompStatusChanged(GriffinsInfoKindID infoKind, string infoNo, InformInfoBase info)
        {
            if (info is InformInfo_StatusChanged informInfo)
            {
                // 确保在主线程更新 UI 绑定的集合模型
                RxApp.MainThreadScheduler.Schedule(() => ProcessBackendStatusChange(informInfo));
            }
        }

        /// <summary>
        /// 抽检页兼容后端仍按「下料」类型推送的状态
        /// </summary>
        private bool MatchesBackendStatusFilter(string? notifyType)
        {
            if (string.IsNullOrWhiteSpace(notifyType))
                return false;

            if (notifyType == BackendStatusFilter)
                return true;

            return CurrentMode == SopWizardMode.Inspect
                && (notifyType == "下料" || notifyType == "抽检");
        }

        private void ProcessBackendStatusChange(InformInfo_StatusChanged info)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(info.Param))
                    return;

                var notify = JsonConvert.DeserializeObject<LoadUnloadMaterialContainerStatusNotify>(info.Param);
                if (notify == null || !MatchesBackendStatusFilter(notify.Type) || string.IsNullOrWhiteSpace(notify.Param))
                    return;

                var backendContainers = JsonConvert.DeserializeObject<List<MaterialContainerStatus>>(notify.Param);
                if (backendContainers == null || !backendContainers.Any()) return;

                    foreach (var backContainer in backendContainers)
                    {
                        var frontContainer = Containers.FirstOrDefault(c => c.Name == backContainer.Name);
                        if (frontContainer == null)
                        {
                            frontContainer = new StorageContainerViewModel
                            {
                                Name = backContainer.Name,
                                IsExpanded = true
                            };
                            Containers.Add(frontContainer);
                        }

                        foreach (var backBox in backContainer.materialBoxStatus)
                        {
                            var frontBox = frontContainer.Magazines.FirstOrDefault(m => m.Name == backBox.Name);
                            if (frontBox == null)
                            {
                                frontBox = new MagazineViewModel 
                                { 
                                    Name = backBox.Name,
                                    RackId = GetRackIdFromMagazineName(backBox.Name, backContainer.Name)
                                };
                                frontContainer.Magazines.Add(frontBox);
                            }

                            ApplyBackendMaterialBox(frontBox, backBox);
                            SyncFeedPhaseWithCylinderState(frontBox);
                        }
                    }
            }
            catch (Exception)
            {
                // 记录反序列化异常日志
            }
        }

        private async Task<bool> UnclampMagAsync(string containerName, string magName)
        {
            try
            {
                var cmdParam = new GFBaseTypeParamValueList
                {
                    new GFBaseTypeParamValue("ContainerName", new GriffinsBaseValue(containerName)),
                    new GFBaseTypeParamValue("MagName", new GriffinsBaseValue(magName))
                };

                var result = await Task.Run(() =>
                    _controlPanelCallBack.ExecNormalCtlCmd(LoadUnloadMachineModulesConst.StorageOpenMethodID, cmdParam)
                );

                if (result != null && result["result"]?.ToString() != "-1")
                {
                    ClearError();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> ClampMagAsync(string containerName, string magName)
        {
            try
            {
                var cmdParam = new GFBaseTypeParamValueList
                {
                    new GFBaseTypeParamValue("ContainerName", new GriffinsBaseValue(containerName)),
                    new GFBaseTypeParamValue("MagName", new GriffinsBaseValue(magName))
                };

                var result = await Task.Run(() =>
                    _controlPanelCallBack.ExecNormalCtlCmd(LoadUnloadMachineModulesConst.StorageCloseMethodID, cmdParam)
                );

                return result != null && result["result"]?.ToString() != "-1";
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 将后端料盒状态同步到前端料盒视图模型（含槽位明细与聚合统计）。
        /// 到位、夹紧等物理状态仅由此处根据后端数据写入，UI 不乐观覆盖。
        /// </summary>
        private static void ApplyBackendMaterialBox(MagazineViewModel frontBox, MaterialBoxStatus backBox)
        {
            // IsEmpty：料盒感应状态，true=未检测到料盒（未到位）
            frontBox.IsPresent = IsMagazinePhysicallyPresent(backBox);
            frontBox.IsClamped = backBox.MaterialBoxCylinderStatus;
            var slotList = backBox.SlotStatusList;

            if (!frontBox.IsPresent)
            {
                if (frontBox.Slots.Count > 0)
                {
                    foreach (var slot in frontBox.Slots)
                        SlotViewModel.ResetToEmpty(slot);
                    frontBox.RefreshSlotCountsFromSlots();
                }
                else
                {
                    frontBox.OccupiedSlotCount = 0;
                    frontBox.FullSlotCount = 0;
                    frontBox.MaterialAlarmSlotCount = 0;
                    frontBox.EmptySlotCount = 0;
                    frontBox.DisabledSlotCount = 0;
                }

                frontBox.RefreshMaterialLevel();
                return;
            }

            if (slotList != null && slotList.Any())
            {
                frontBox.Slots.Clear();
                for (int i = slotList.Count - 1; i >= 0; i--)
                {
                    frontBox.Slots.Add(SlotViewModel.FromMaterialStatus(i + 1, slotList[i].MaterialStatus));
                }

                frontBox.RefreshSlotCountsFromSlots();
                return;
            }

            if (frontBox.Slots.Count > 0)
            {
                // 后端有时只推送物理到位/夹紧，不带槽位；保留上次有效槽位状态，避免界面闪退为全空白。
                return;
            }

            if (slotList != null)
            {
                // 无槽位明细条目时仅聚合统计。
                frontBox.FullSlotCount = slotList.Count(s => s.MaterialStatus == MaterialStatus.Full);
                frontBox.OccupiedSlotCount = 0;
                frontBox.MaterialAlarmSlotCount = 0;
                frontBox.EmptySlotCount = slotList.Count(s => s.MaterialStatus == MaterialStatus.Empty);
                frontBox.DisabledSlotCount = slotList.Count(s => s.MaterialStatus == MaterialStatus.Disable);
                frontBox.RefreshMaterialLevel();
            }
        }

        /// <summary>
        /// 预留流程状态同步钩子：当前上/下料均由按钮点击驱动，不自动按气缸状态推进阶段。
        /// </summary>
        private void SyncFeedPhaseWithCylinderState(MagazineViewModel mag)
        {
            _ = mag;
        }

        #endregion

        #region UI 辅助与动画方法

        private void GenerateMockData()
        {
            Containers.Clear();
            Random random = new Random();

            for (int i = 1; i <= 8; i++)
            {
                var container = new StorageContainerViewModel { Name = $"作业容器{i}" };
                int magazineCount = random.Next(2, 7);

                for (int j = 1; j <= magazineCount; j++)
                {
                    var mag = new MagazineViewModel
                    {
                        Name = $"料盒 {i}-{j}",
                        IsPresent = true,
                        IsClamped = true
                    };

                    int slotCount = random.Next(20, 46);
                    for (int k = 1; k <= slotCount; k++)
                    {
                        var status = random.Next(100) switch
                        {
                            < 5 => MaterialStatus.Disable,
                            < 25 => MaterialStatus.Empty,
                            _ => MaterialStatus.Full
                        };
                        var slot = SlotViewModel.FromMaterialStatus(k, status);
                        if (random.Next(100) < 4)
                        {
                            slot.IsOccupied = false;
                            slot.IsFull = true;
                        }
                        else if (random.Next(100) < 2)
                        {
                            slot.IsMaterialAlarm = true;
                            slot.IsOccupied = false;
                            slot.IsEmpty = false;
                        }

                        mag.Slots.Add(slot);
                    }

                    mag.RefreshSlotCountsFromSlots();
                    container.Magazines.Add(mag);
                }
                Containers.Add(container);
            }
        }

        /// <summary>
        /// 显示顶部消息（带视觉呼吸重绘动画）
        /// </summary>
        private async void ShowMessage(string msg, bool isError = false)
        {
            SystemMessage = msg;
            IsErrorAlert = isError;
            MessageOpacity = 0.4;
            await Task.Delay(150); // 触发快速透明度变化，吸引操作员注意
            MessageOpacity = 1.0;
        }

        private void ClearError()
        {
            IsErrorAlert = false;
            foreach (var m in AllMagazines) m.HasError = false;
        }

        #endregion
    }
}
