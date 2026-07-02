using ReactiveUI;

namespace GKG.Map.LoadUnloadFuncCtlMapCell.ViewModels
{
    /// <summary>
    /// 槽位视图模型 (ViewModel)
    /// 表示料盒内的最小物理存储单元（纯状态模型），负责维护自身状态并向上层提供可绑定的描述。
    /// </summary>
    public class LayerViewModel : ReactiveObject
    {
        #region 基础属性与状态

        private bool _isOccupied;
        private bool _isEmpty;
        private bool _isDisabled;

        /// <summary>
        /// 物理层级索引 (例如从底向上的 1, 2, 3...)
        /// </summary>
        public int LayerIndex { get; set; }

        /// <summary>
        /// 物理状态：当前槽位是否已存放物料 (有料)
        /// </summary>
        public bool IsOccupied
        {
            get => _isOccupied;
            set
            {
                // 性能优化：仅当真实发生状态翻转时，才触发 UI 文本的重新获取与渲染
                if (this.RaiseAndSetIfChanged(ref _isOccupied, value))
                {
                    this.RaisePropertyChanged(nameof(StatusDescription));
                }
            }
        }

        /// <summary>
        /// 物理状态：当前槽位是否为空 (空槽)
        /// </summary>
        public bool IsEmpty
        {
            get => _isEmpty;
            set
            {
                if (this.RaiseAndSetIfChanged(ref _isEmpty, value))
                {
                    this.RaisePropertyChanged(nameof(StatusDescription));
                }
            }
        }

        /// <summary>
        /// 物理状态：当前槽位是否被禁用 (异常或不可用)
        /// </summary>
        public bool IsDisabled
        {
            get => _isDisabled;
            set
            {
                if (this.RaiseAndSetIfChanged(ref _isDisabled, value))
                {
                    this.RaisePropertyChanged(nameof(StatusDescription));
                }
            }
        }

        #endregion

        #region UI 派生属性

        /// <summary>
        /// 供 UI 绑定的直观状态描述文本 (剔除硬编码 Emoji，确保跨平台工业环境字体安全)
        /// </summary>
        public string StatusDescription
        {
            get
            {
                // 优先判断异常/禁用状态
                if (IsDisabled)
                {
                    return "状态: 禁用";
                }

                // 正常状态判定
                return IsOccupied ? "状态: 有料" : "状态: 空槽";
            }
        }

        #endregion
    }
}