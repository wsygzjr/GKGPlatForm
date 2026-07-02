using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using ReactiveUI;

namespace GKG.CompUI.LoadUnload.ControlPanel.ViewModels
{
    /// <summary>
    /// 容器视图模型：管理容器内的料盒集合，并将内部状态冒泡汇总到容器级别
    /// </summary>
    public class StorageContainerViewModel : ReactiveObject
    {
        #region 私有字段

        private bool _isExpanded = true;

        #endregion

        #region 基础属性

        /// <summary>
        /// 容器名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 容器内的料盒集合
        /// </summary>
        public ObservableCollection<MagazineViewModel> Magazines { get; } = new();

        /// <summary>
        /// UI 面板的展开/收起状态
        /// </summary>
        public bool IsExpanded
        {
            get => _isExpanded;
            set => this.RaiseAndSetIfChanged(ref _isExpanded, value);
        }

        #endregion

        #region 聚合计算属性 (状态冒泡)

        /// <summary>
        /// 缺失（未到位）的料盒数量
        /// </summary>
        public int MissingCount => Magazines.Count(m => !m.IsPresent);
        public bool HasMissing => MissingCount > 0;

        /// <summary>
        /// 已到位但处于松开危险状态的料盒数量
        /// </summary>
        public int UnclampedCount => Magazines.Count(m => m.IsPresent && !m.IsClamped);
        public bool HasUnclamped => UnclampedCount > 0;

        #endregion

        #region 命令 (Commands)

        /// <summary>
        /// 切换面板的展开与收起
        /// </summary>
        public ReactiveCommand<Unit, Unit> ToggleExpandCommand { get; }

        #endregion

        #region 构造函数

        public StorageContainerViewModel()
        {
            // 初始化同步无参命令
            ToggleExpandCommand = ReactiveCommand.Create(() => { IsExpanded = !IsExpanded; });

            // 订阅集合增减事件，用于动态绑定/解绑内部料盒的属性监听
            Magazines.CollectionChanged += OnMagazinesCollectionChanged;
        }

        #endregion

        #region 私有方法 (事件流处理)

        private void OnMagazinesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // 新增料盒时，挂载属性监听
            if (e.NewItems != null)
            {
                foreach (var newMag in e.NewItems.OfType<MagazineViewModel>())
                {
                    newMag.PropertyChanged += OnMagazinePropertyChanged;
                }
            }

            // 移除料盒时，必须卸载属性监听，防止内存泄漏和幽灵回调
            if (e.OldItems != null)
            {
                foreach (var oldMag in e.OldItems.OfType<MagazineViewModel>())
                {
                    oldMag.PropertyChanged -= OnMagazinePropertyChanged;
                }
            }

            // 集合数量发生变化，必然影响总数，触发一次冒泡计算
            RefreshCalculatedProperties();
        }

        private void OnMagazinePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // 过滤无效通知，仅当物理核心状态改变时，才触发容器级的聚合计算
            if (e.PropertyName is nameof(MagazineViewModel.IsPresent) or nameof(MagazineViewModel.IsClamped))
            {
                RefreshCalculatedProperties();
            }
        }

        /// <summary>
        /// 手动向 UI 广播所有派生属性的变化
        /// </summary>
        private void RefreshCalculatedProperties()
        {
            this.RaisePropertyChanged(nameof(MissingCount));
            this.RaisePropertyChanged(nameof(HasMissing));
            this.RaisePropertyChanged(nameof(UnclampedCount));
            this.RaisePropertyChanged(nameof(HasUnclamped));
        }

        #endregion
    }
}