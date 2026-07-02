using ReactiveUI;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive;

namespace GKG.Map.LoadUnloadFuncCtlMapCell.ViewModels
{
    /// <summary>
    /// 容器视图模型 (ViewModel)
    /// 管理容器的纯 UI 状态（如折叠/展开），并向下聚合内部料盒 (Magazine) 的实时统计数据。
    /// </summary>
    public class StorageContainerViewModel : ReactiveObject
    {
        #region 基础属性与状态

        private bool _isExpanded = true;

        /// <summary>
        /// 容器名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// UI 状态：当前容器是否处于展开显示状态
        /// </summary>
        public bool IsExpanded
        {
            get => _isExpanded;
            set => this.RaiseAndSetIfChanged(ref _isExpanded, value);
        }

        /// <summary>
        /// 容器内挂载的料盒集合
        /// </summary>
        public ObservableCollection<MagazineViewModel> Magazines { get; } = new();

        #endregion

        #region 聚合计算指标 (Calculated Properties)

        /// <summary>
        /// 统计：缺失（未在位）的料盒数量
        /// </summary>
        public int MissingCount => Magazines.Count(m => !m.IsPresent);

        /// <summary>
        /// 状态：是否存在缺失的料盒
        /// </summary>
        public bool HasMissing => MissingCount > 0;

        /// <summary>
        /// 统计：在位但未被气缸夹紧的料盒数量
        /// </summary>
        public int UnclampedCount => Magazines.Count(m => m.IsPresent && !m.IsClamped);

        /// <summary>
        /// 状态：是否存在未夹紧的危险料盒
        /// </summary>
        public bool HasUnclamped => UnclampedCount > 0;

        /// <summary>
        /// 状态：容器内是否存在任何料盒触发了报警
        /// </summary>
        public bool HasAlert => Magazines.Any(m => m.HasAlert);

        #endregion

        #region 交互命令

        /// <summary>
        /// 切换容器的展开/折叠状态命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> ToggleExpandCommand { get; }

        #endregion

        /// <summary>
        /// 实例化容器视图模型，并初始化动态事件聚合网络
        /// </summary>
        public StorageContainerViewModel()
        {
            // 修复：加上大括号，强制作为 Action 执行，返回 Unit
            ToggleExpandCommand = ReactiveCommand.Create(() => { IsExpanded = !IsExpanded; });

            // 订阅集合变更，以动态维护对子料盒属性变化的监听
            Magazines.CollectionChanged += OnMagazinesCollectionChanged;
        }

        #region 状态聚合与事件冒泡网络 (Event Bubbling)

        /// <summary>
        /// 当料盒集合发生增删时，动态挂载或卸载子项的属性监听，严防内存泄漏
        /// </summary>
        private void OnMagazinesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var newMag in e.NewItems.OfType<MagazineViewModel>())
                {
                    newMag.PropertyChanged += OnMagazinePropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (var oldMag in e.OldItems.OfType<MagazineViewModel>())
                {
                    oldMag.PropertyChanged -= OnMagazinePropertyChanged;
                }
            }

            // 集合发生结构性变化时，全量刷新一次聚合指标
            RefreshCalculatedProperties();
        }

        /// <summary>
        /// 拦截子料盒的关键状态变更，并向上冒泡通知 UI 刷新聚合指标
        /// </summary>
        private void OnMagazinePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // 采用 C# 9.0 模式匹配，极速过滤无关属性，仅拦截影响统计指标的关键属性
            if (e.PropertyName is nameof(MagazineViewModel.IsPresent)
                                 or nameof(MagazineViewModel.IsClamped)
                                 or nameof(MagazineViewModel.HasAlert))
            {
                RefreshCalculatedProperties();
            }
        }

        /// <summary>
        /// 触发所有聚合指标的 UI 重新求值与渲染
        /// </summary>
        private void RefreshCalculatedProperties()
        {
            this.RaisePropertyChanged(nameof(MissingCount));
            this.RaisePropertyChanged(nameof(HasMissing));
            this.RaisePropertyChanged(nameof(UnclampedCount));
            this.RaisePropertyChanged(nameof(HasUnclamped));
            this.RaisePropertyChanged(nameof(HasAlert));
        }

        #endregion
    }
}