using ReactiveUI;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace GKG.Map.LoadUnloadFuncCtlMapCell.ViewModels
{
    /// <summary>
    /// 料盒视图模型 (ViewModel)
    /// 反映物理料盒状态（如是否在位、是否夹紧），并向下聚合底层槽位 (Layer) 的实时告警状态。
    /// </summary>
    public class MagazineViewModel : ReactiveObject
    {
        #region 基础属性与状态

        private bool _isPresent;
        private bool _isClamped;

        /// <summary>
        /// 料盒名称或编号
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 物理状态：料盒是否在位（已被检测到放入容器中）
        /// </summary>
        public bool IsPresent
        {
            get => _isPresent;
            set => this.RaiseAndSetIfChanged(ref _isPresent, value);
        }

        /// <summary>
        /// 物理状态：料盒是否已被气缸夹紧
        /// </summary>
        public bool IsClamped
        {
            get => _isClamped;
            set
            {
                if (this.RaiseAndSetIfChanged(ref _isClamped, value))
                {
                    this.RaisePropertyChanged(nameof(CanClamp));
                    this.RaisePropertyChanged(nameof(CanUnclamp));
                }
            }
        }

        /// <summary>
        /// 内部挂载的所有槽位（层）集合
        /// </summary>
        public ObservableCollection<LayerViewModel> Layers { get; } = new();

        #endregion

        #region 派生逻辑与聚合指标

        /// <summary>
        /// 防呆逻辑：是否允许执行夹紧操作 (未夹紧时允许)
        /// </summary>
        public bool CanClamp => !IsClamped;

        /// <summary>
        /// 防呆逻辑：是否允许执行松开操作 (已夹紧时允许)
        /// </summary>
        public bool CanUnclamp => IsClamped;

        /// <summary>
        /// 聚合告警状态：内部是否存在任何异常/禁用的槽位
        /// </summary>
        public bool HasAlert => Layers.Any(l => l.IsDisabled);

        #endregion

        /// <summary>
        /// 实例化料盒视图模型，并初始化子节点事件聚合网络
        /// </summary>
        public MagazineViewModel()
        {
            Layers.CollectionChanged += OnLayersCollectionChanged;
        }

        #region 状态聚合与事件冒泡网络 (Event Bubbling)

        /// <summary>
        /// 当槽位集合发生增删时，动态挂载或卸载属性监听，严防内存泄漏
        /// </summary>
        private void OnLayersCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var newLayer in e.NewItems.OfType<LayerViewModel>())
                {
                    newLayer.PropertyChanged += OnLayerPropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (var oldLayer in e.OldItems.OfType<LayerViewModel>())
                {
                    oldLayer.PropertyChanged -= OnLayerPropertyChanged;
                }
            }

            // 集合结构改变（如清空或重建）时，触发告警指标重新计算
            this.RaisePropertyChanged(nameof(HasAlert));
        }

        /// <summary>
        /// 拦截底层槽位的关键状态变更，并向上冒泡通知 UI 刷新告警指标
        /// </summary>
        private void OnLayerPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // 仅监听对料盒级告警有影响的底层属性
            if (e.PropertyName == nameof(LayerViewModel.IsDisabled))
            {
                this.RaisePropertyChanged(nameof(HasAlert));
            }
        }

        #endregion
    }
}