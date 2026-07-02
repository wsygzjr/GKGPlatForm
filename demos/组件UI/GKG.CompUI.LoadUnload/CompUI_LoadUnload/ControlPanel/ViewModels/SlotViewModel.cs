using GKG.MM;
using ReactiveUI;

namespace GKG.CompUI.LoadUnload.ControlPanel.ViewModels
{
    /// <summary>
    /// 槽位视图模型：表示料盒内的最小存储单元（纯状态模型）
    /// </summary>
    public class SlotViewModel : ReactiveObject
    {
        private bool _isFull;
        private bool _isOccupied;
        private bool _isMaterialAlarm;
        private bool _isEmpty;
        private bool _isDisabled;

        /// <summary>
        /// 槽位编号（从 1 起）
        /// </summary>
        public int SlotIndex { get; set; }

        /// <summary>
        /// 满料
        /// </summary>
        public bool IsFull
        {
            get => _isFull;
            set
            {
                this.RaiseAndSetIfChanged(ref _isFull, value);
                this.RaisePropertyChanged(nameof(IsFilled));
                this.RaisePropertyChanged(nameof(StatusDescription));
            }
        }

        /// <summary>
        /// 有料
        /// </summary>
        public bool IsOccupied
        {
            get => _isOccupied;
            set
            {
                this.RaiseAndSetIfChanged(ref _isOccupied, value);
                this.RaisePropertyChanged(nameof(IsFilled));
                this.RaisePropertyChanged(nameof(StatusDescription));
            }
        }

        /// <summary>
        /// 物料报警
        /// </summary>
        public bool IsMaterialAlarm
        {
            get => _isMaterialAlarm;
            set
            {
                this.RaiseAndSetIfChanged(ref _isMaterialAlarm, value);
                this.RaisePropertyChanged(nameof(StatusDescription));
            }
        }

        /// <summary>
        /// 空料
        /// </summary>
        public bool IsEmpty
        {
            get => _isEmpty;
            set
            {
                this.RaiseAndSetIfChanged(ref _isEmpty, value);
                this.RaisePropertyChanged(nameof(StatusDescription));
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public bool IsDisabled
        {
            get => _isDisabled;
            set
            {
                this.RaiseAndSetIfChanged(ref _isDisabled, value);
                this.RaisePropertyChanged(nameof(StatusDescription));
            }
        }

        /// <summary>
        /// 槽位是否显示为“有料”色条（满料或有料）
        /// </summary>
        public bool IsFilled => IsOccupied || IsFull;

        /// <summary>
        /// 状态描述文本
        /// </summary>
        public string StatusDescription
        {
            get
            {
                if (IsDisabled)
                    return "状态: 禁用 🚫";

                if (IsMaterialAlarm)
                    return "状态: 物料报警 ⚠️";

                if (IsFull)
                    return "状态: 满料 🟥";

                if (IsOccupied)
                    return "状态: 有料 🟦";

                if (IsEmpty)
                    return "状态: 空料 ⬜";

                return "状态: 未知";
            }
        }

        /// <summary>
        /// 根据后端物料状态创建槽位视图模型
        /// </summary>
        public static SlotViewModel FromMaterialStatus(int slotIndex, MaterialStatus status)
        {
            var slot = new SlotViewModel { SlotIndex = slotIndex };
            ApplyMaterialStatus(slot, status);
            return slot;
        }

        /// <summary>
        /// 将后端 <see cref="MaterialStatus"/> 映射到 UI 布尔状态（互斥）
        /// </summary>
        public static void ApplyMaterialStatus(SlotViewModel slot, MaterialStatus status)
        {
            slot.IsDisabled = false;
            slot.IsEmpty = false;
            slot.IsOccupied = false;
            slot.IsFull = false;
            slot.IsMaterialAlarm = false;

            switch (status)
            {
                case MaterialStatus.Disable:
                    slot.IsDisabled = true;
                    break;
                case MaterialStatus.Empty:
                    slot.IsEmpty = true;
                    break;
                case MaterialStatus.Full:
                    slot.IsFull = true;
                    break;
            }
        }

        /// <summary>
        /// 料盒拔出或无槽位数据时，将已有槽位壳子重置为空料
        /// </summary>
        public static void ResetToEmpty(SlotViewModel slot)
        {
            ApplyMaterialStatus(slot, MaterialStatus.Empty);
        }
    }
}
