using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace GKG.CompUI.LoadUnload.ControlPanel.ViewModels
{
    /// <summary>
    /// 料盒视图模型：管理单个料盒的物理状态映射与 UI 交互
    /// </summary>
    public class MagazineViewModel : ReactiveObject
    {
        #region 私有字段

        private string _name = string.Empty;
        private int _rackId = -1;
        private bool _isPresent = false;
        private bool _isClamped = true;
        private bool _hasError;
        private MagazineLoadPhase _loadPhase = MagazineLoadPhase.Ready;
        private MagazineUnloadPhase _unloadPhase = MagazineUnloadPhase.Ready;
        private MagazineInspectPhase _inspectPhase = MagazineInspectPhase.Ready;
        private MagazineMaterialLevel _materialLevel = MagazineMaterialLevel.Empty;

        #endregion

        #region 基础属性

        /// <summary>
        /// 料盒名称或唯一标识
        /// </summary>
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        /// <summary>
        /// 料盒对应的 MaterialRack 编号 (0=上料上层, 1=上料下层, 2=下料上层, 3=下料下层)
        /// </summary>
        public int RackId
        {
            get => _rackId;
            set => this.RaiseAndSetIfChanged(ref _rackId, value);
        }

        /// <summary>
        /// 物理到位状态（由后端推送/查询同步，勿在 UI 层手动赋值）
        /// </summary>
        public bool IsPresent
        {
            get => _isPresent;
            set
            {
                this.RaiseAndSetIfChanged(ref _isPresent, value);
                if (!value)
                {
                    LoadPhase = MagazineLoadPhase.Ready;
                    UnloadPhase = MagazineUnloadPhase.Ready;
                    InspectPhase = MagazineInspectPhase.Ready;
                }

                this.RaisePropertyChanged(nameof(CanUnclamp));
                RaiseSopWorkflowProperties();
            }
        }

        /// <summary>
        /// 夹紧机构状态（由后端推送/查询同步，勿在 UI 层手动赋值）
        /// </summary>
        public bool IsClamped
        {
            get => _isClamped;
            set
            {
                this.RaiseAndSetIfChanged(ref _isClamped, value);
                this.RaisePropertyChanged(nameof(CanUnclamp));
                RaiseSopWorkflowProperties();
            }
        }

        /// <summary>
        /// 异常标记（用于 UI 标红警示操作员）
        /// </summary>
        public bool HasError
        {
            get => _hasError;
            set => this.RaiseAndSetIfChanged(ref _hasError, value);
        }

        #endregion

        #region 料盒级物料状态（截图徽章）

        /// <summary>
        /// 料盒汇总物料等级
        /// </summary>
        public MagazineMaterialLevel MaterialLevel
        {
            get => _materialLevel;
            private set
            {
                this.RaiseAndSetIfChanged(ref _materialLevel, value);
                this.RaisePropertyChanged(nameof(MaterialStatusLabel));
                this.RaisePropertyChanged(nameof(ShouldFlashMaterialStatus));
                this.RaisePropertyChanged(nameof(IsMaterialStatusFull));
                this.RaisePropertyChanged(nameof(IsMaterialStatusHasMaterial));
                this.RaisePropertyChanged(nameof(IsMaterialStatusAlarm));
                this.RaisePropertyChanged(nameof(IsMaterialStatusEmpty));
                this.RaisePropertyChanged(nameof(CanExecuteLoadAction));
            }
        }

        /// <summary>状态徽章文案</summary>
        public string MaterialStatusLabel => MaterialLevel switch
        {
            MagazineMaterialLevel.Full => "满料",
            MagazineMaterialLevel.HasMaterial => "有料",
            MagazineMaterialLevel.MaterialAlarm => "物料报警",
            MagazineMaterialLevel.Empty => "空料",
            _ => string.Empty
        };

        /// <summary>物料报警、空料徽章需持续闪烁</summary>
        public bool ShouldFlashMaterialStatus =>
            MaterialLevel is MagazineMaterialLevel.MaterialAlarm or MagazineMaterialLevel.Empty;

        public bool IsMaterialStatusFull => MaterialLevel == MagazineMaterialLevel.Full;
        public bool IsMaterialStatusHasMaterial => MaterialLevel == MagazineMaterialLevel.HasMaterial;
        public bool IsMaterialStatusAlarm => MaterialLevel == MagazineMaterialLevel.MaterialAlarm;
        public bool IsMaterialStatusEmpty => MaterialLevel == MagazineMaterialLevel.Empty;

        #endregion

        #region 槽位统计属性

        private int _fullSlotCount;
        /// <summary>满料槽位数量</summary>
        public int FullSlotCount
        {
            get => _fullSlotCount;
            set
            {
                this.RaiseAndSetIfChanged(ref _fullSlotCount, value);
                this.RaisePropertyChanged(nameof(HasSlotInfo));
            }
        }

        private int _occupiedSlotCount;
        /// <summary>有料槽位数量</summary>
        public int OccupiedSlotCount
        {
            get => _occupiedSlotCount;
            set
            {
                this.RaiseAndSetIfChanged(ref _occupiedSlotCount, value);
                this.RaisePropertyChanged(nameof(HasSlotInfo));
            }
        }

        private int _materialAlarmSlotCount;
        /// <summary>物料报警槽位数量</summary>
        public int MaterialAlarmSlotCount
        {
            get => _materialAlarmSlotCount;
            set
            {
                this.RaiseAndSetIfChanged(ref _materialAlarmSlotCount, value);
                this.RaisePropertyChanged(nameof(HasSlotInfo));
                this.RaisePropertyChanged(nameof(HasMaterialAlarmSlots));
            }
        }

        private int _emptySlotCount;
        /// <summary>空料槽位数量</summary>
        public int EmptySlotCount
        {
            get => _emptySlotCount;
            set
            {
                this.RaiseAndSetIfChanged(ref _emptySlotCount, value);
                this.RaisePropertyChanged(nameof(HasSlotInfo));
            }
        }

        private int _disabledSlotCount;
        /// <summary>禁用/异常槽位数量</summary>
        public int DisabledSlotCount
        {
            get => _disabledSlotCount;
            set
            {
                this.RaiseAndSetIfChanged(ref _disabledSlotCount, value);
                this.RaisePropertyChanged(nameof(HasSlotInfo));
                this.RaisePropertyChanged(nameof(HasDisabledSlots));
            }
        }

        /// <summary>
        /// 料盒内各槽位明细（倒序渲染时由同步逻辑按层号填充）
        /// </summary>
        public ObservableCollection<SlotViewModel> Slots { get; } = new();

        /// <summary>
        /// 是否拥有槽位信息（防呆：如果底层尚未检测到，传了空列表，则 UI 隐藏该区域）
        /// </summary>
        public bool HasSlotInfo => Slots.Count > 0
            || (FullSlotCount + OccupiedSlotCount + MaterialAlarmSlotCount + EmptySlotCount + DisabledSlotCount) > 0;

        /// <summary>
        /// 是否存在物料报警槽位
        /// </summary>
        public bool HasMaterialAlarmSlots => MaterialAlarmSlotCount > 0
            || Slots.Any(s => s.IsMaterialAlarm);

        /// <summary>
        /// 是否存在禁用槽位（如果有，UI 将标红警示）
        /// </summary>
        public bool HasDisabledSlots => DisabledSlotCount > 0
            || Slots.Any(s => s.IsDisabled);

        #endregion

        #region 构造函数与槽位冒泡

        public MagazineViewModel()
        {
            Slots.CollectionChanged += OnSlotsCollectionChanged;
        }

        private void OnSlotsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var slot in e.NewItems.OfType<SlotViewModel>())
                    slot.PropertyChanged += OnSlotPropertyChanged;
            }

            if (e.OldItems != null)
            {
                foreach (var slot in e.OldItems.OfType<SlotViewModel>())
                    slot.PropertyChanged -= OnSlotPropertyChanged;
            }

            RefreshSlotCountsFromSlots();
            this.RaisePropertyChanged(nameof(HasSlotInfo));
            this.RaisePropertyChanged(nameof(HasDisabledSlots));
            this.RaisePropertyChanged(nameof(HasMaterialAlarmSlots));
        }

        private void OnSlotPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(SlotViewModel.IsOccupied)
                or nameof(SlotViewModel.IsFull)
                or nameof(SlotViewModel.IsEmpty)
                or nameof(SlotViewModel.IsDisabled)
                or nameof(SlotViewModel.IsMaterialAlarm))
            {
                RefreshSlotCountsFromSlots();
            }
        }

        /// <summary>
        /// 根据 <see cref="Slots"/> 明细刷新聚合统计
        /// </summary>
        public void RefreshSlotCountsFromSlots()
        {
            if (Slots.Count == 0)
            {
                RefreshMaterialLevel();
                return;
            }

            FullSlotCount = Slots.Count(s => s.IsFull);
            OccupiedSlotCount = Slots.Count(s => s.IsOccupied);
            MaterialAlarmSlotCount = Slots.Count(s => s.IsMaterialAlarm);
            EmptySlotCount = Slots.Count(s => s.IsEmpty);
            DisabledSlotCount = Slots.Count(s => s.IsDisabled);
            RefreshMaterialLevel();
        }

        /// <summary>
        /// 根据槽位明细推导料盒级状态徽章
        /// </summary>
        public void RefreshMaterialLevel()
        {
            if (HasMaterialAlarmSlots)
            {
                MaterialLevel = MagazineMaterialLevel.MaterialAlarm;
                return;
            }

            if (Slots.Count == 0)
            {
                if (EmptySlotCount > 0 && FullSlotCount == 0 && OccupiedSlotCount == 0)
                    MaterialLevel = MagazineMaterialLevel.Empty;
                else if (FullSlotCount > 0 && EmptySlotCount == 0 && OccupiedSlotCount == 0)
                    MaterialLevel = MagazineMaterialLevel.Full;
                else if (FullSlotCount + OccupiedSlotCount > 0)
                    MaterialLevel = MagazineMaterialLevel.HasMaterial;
                else
                    MaterialLevel = MagazineMaterialLevel.Empty;
                return;
            }

            int activeSlots = Slots.Count(s => !s.IsDisabled);
            if (activeSlots == 0)
            {
                MaterialLevel = MagazineMaterialLevel.Empty;
                return;
            }

            bool allFull = Slots.Where(s => !s.IsDisabled).All(s => s.IsFull);
            bool allEmpty = Slots.Where(s => !s.IsDisabled).All(s => s.IsEmpty);
            bool anyFilled = Slots.Any(s => s.IsFilled);

            if (allFull)
                MaterialLevel = MagazineMaterialLevel.Full;
            else if (allEmpty)
                MaterialLevel = MagazineMaterialLevel.Empty;
            else if (anyFilled)
                MaterialLevel = MagazineMaterialLevel.HasMaterial;
            else
                MaterialLevel = MagazineMaterialLevel.Empty;
        }

        #endregion

        #region 一键上料作业流

        /// <summary>
        /// 当前上料阶段（仅上料向导使用）
        /// </summary>
        public MagazineLoadPhase LoadPhase
        {
            get => _loadPhase;
            set
            {
                this.RaiseAndSetIfChanged(ref _loadPhase, value);
                RaiseSopWorkflowProperties();
            }
        }

        /// <summary>
        /// 当前下料阶段（仅下料向导使用）
        /// </summary>
        public MagazineUnloadPhase UnloadPhase
        {
            get => _unloadPhase;
            set
            {
                this.RaiseAndSetIfChanged(ref _unloadPhase, value);
                RaiseSopWorkflowProperties();
            }
        }

        /// <summary>
        /// 当前抽检阶段（仅抽检向导使用）
        /// </summary>
        public MagazineInspectPhase InspectPhase
        {
            get => _inspectPhase;
            set
            {
                this.RaiseAndSetIfChanged(ref _inspectPhase, value);
                RaiseSopWorkflowProperties();
            }
        }

        /// <summary>
        /// 料盒状态摘要（到位 / 夹紧 / 流程）
        /// </summary>
        public string StatusSummary
        {
            get
            {
                if (!IsPresent)
                    return "未到位";

                string clampText = IsClamped ? "已夹紧" : "已松开";
                if (LoadPhase == MagazineLoadPhase.Finished)
                    return $"到位 · {clampText} · 上料已完成";
                if (UnloadPhase == MagazineUnloadPhase.Finished)
                    return $"到位 · {clampText} · 下料已完成";
                if (InspectPhase == MagazineInspectPhase.Finished)
                    return $"到位 · {clampText} · 抽检已完成";

                return $"到位 · {clampText}";
            }
        }

        /// <summary>
        /// 上料流程主按钮文案
        /// </summary>
        public string LoadActionButtonText => LoadPhase switch
        {
            MagazineLoadPhase.Ready => "开始上料",
            MagazineLoadPhase.AwaitingConfirm => "上料完成",
            MagazineLoadPhase.Finished => "已完成",
            _ => string.Empty
        };

        /// <summary>
        /// 上料流程按钮始终可点（含「已完成」闭环）；具体逻辑由 ViewModel 处理。
        /// </summary>
        public bool CanExecuteLoadAction => true;

        /// <summary>
        /// 上料流程是否已全部完成（用于按钮置灰样式）
        /// </summary>
        public bool IsLoadWorkflowFinished => LoadPhase == MagazineLoadPhase.Finished;

        /// <summary>
        /// 下料流程主按钮文案
        /// </summary>
        public string UnloadActionButtonText => UnloadPhase switch
        {
            MagazineUnloadPhase.Ready => "开始下料",
            MagazineUnloadPhase.AwaitingConfirm => "下料完成",
            MagazineUnloadPhase.Finished => "已完成",
            _ => string.Empty
        };

        /// <summary>
        /// 下料流程按钮始终可点（含「已完成」闭环）；具体逻辑由 ViewModel 处理。
        /// </summary>
        public bool CanExecuteUnloadAction => true;

        /// <summary>
        /// 下料流程是否已全部完成（用于按钮置灰样式）
        /// </summary>
        public bool IsUnloadWorkflowFinished => UnloadPhase == MagazineUnloadPhase.Finished;

        /// <summary>上料主按钮是否处于“上料完成”紫色态</summary>
        public bool IsLoadAwaitingConfirm => LoadPhase == MagazineLoadPhase.AwaitingConfirm;

        /// <summary>下料主按钮是否处于“下料完成”紫色态</summary>
        public bool IsUnloadAwaitingConfirm => UnloadPhase == MagazineUnloadPhase.AwaitingConfirm;

        /// <summary>
        /// 抽检流程主按钮文案
        /// </summary>
        public string InspectActionButtonText => InspectPhase switch
        {
            MagazineInspectPhase.Ready => "开始抽检",
            MagazineInspectPhase.AwaitingConfirm => "抽检完成",
            MagazineInspectPhase.Finished => "已完成",
            _ => string.Empty
        };

        /// <summary>抽检流程按钮始终可点（含「已完成」闭环）</summary>
        public bool CanExecuteInspectAction => true;

        public bool IsInspectWorkflowFinished => InspectPhase == MagazineInspectPhase.Finished;

        /// <summary>抽检主按钮是否处于“抽检完成”紫色态</summary>
        public bool IsInspectAwaitingConfirm => InspectPhase == MagazineInspectPhase.AwaitingConfirm;

        private void RaiseSopWorkflowProperties()
        {
            this.RaisePropertyChanged(nameof(StatusSummary));
            this.RaisePropertyChanged(nameof(LoadActionButtonText));
            this.RaisePropertyChanged(nameof(CanExecuteLoadAction));
            this.RaisePropertyChanged(nameof(IsLoadAwaitingConfirm));
            this.RaisePropertyChanged(nameof(IsLoadWorkflowFinished));
            this.RaisePropertyChanged(nameof(UnloadActionButtonText));
            this.RaisePropertyChanged(nameof(CanExecuteUnloadAction));
            this.RaisePropertyChanged(nameof(IsUnloadWorkflowFinished));
            this.RaisePropertyChanged(nameof(IsUnloadAwaitingConfirm));
            this.RaisePropertyChanged(nameof(InspectActionButtonText));
            this.RaisePropertyChanged(nameof(CanExecuteInspectAction));
            this.RaisePropertyChanged(nameof(IsInspectWorkflowFinished));
            this.RaisePropertyChanged(nameof(IsInspectAwaitingConfirm));
        }

        /// <summary>
        /// 作业闭环完成后刷新料盒物料展示（上料→满料，下料→空料）
        /// </summary>
        public void ApplyWorkflowCompletedMaterialView(bool afterLoad)
        {
            if (Slots.Count > 0)
            {
                foreach (var slot in Slots)
                {
                    if (slot.IsDisabled)
                        continue;

                    if (afterLoad)
                    {
                        slot.IsMaterialAlarm = false;
                        slot.IsOccupied = false;
                        slot.IsEmpty = false;
                        slot.IsFull = true;
                    }
                    else
                    {
                        SlotViewModel.ResetToEmpty(slot);
                    }
                }

                RefreshSlotCountsFromSlots();
                return;
            }

            if (afterLoad)
            {
                FullSlotCount = Math.Max(FullSlotCount, 1);
                OccupiedSlotCount = 0;
                MaterialAlarmSlotCount = 0;
                EmptySlotCount = 0;
            }
            else
            {
                FullSlotCount = 0;
                OccupiedSlotCount = 0;
                MaterialAlarmSlotCount = 0;
                EmptySlotCount = Math.Max(EmptySlotCount, 1);
            }

            MaterialLevel = afterLoad ? MagazineMaterialLevel.Full : MagazineMaterialLevel.Empty;
        }

        #endregion

        #region 派生计算属性

        /// <summary>
        /// 是否允许下发松开指令（防呆判定：必须已物理到位，且当前处于被夹紧状态）
        /// </summary>
        public bool CanUnclamp => IsClamped && IsPresent;

        #endregion
    }
}