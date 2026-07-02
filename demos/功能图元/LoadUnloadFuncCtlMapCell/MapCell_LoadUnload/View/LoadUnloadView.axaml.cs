using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using GKG.Map.LoadUnloadFuncCtlMapCell.ViewModels;

namespace GKG.Map.LoadUnloadFuncCtlMapCell.Views
{
    /// <summary>
    /// 上下料实时监控主视图 (View 后台代码)
    /// </summary>
    public partial class LoadUnloadView : ReactiveUserControl<LoadUnloadViewModel>
    {
        public LoadUnloadView()
        {
            InitializeComponent();
        }

        #region 纯 UI 交互事件 (View-Level Logic)

        /// <summary>
        /// 处理下拉框选择变化：快速定位容器，并将其滚动至可视区域。
        /// </summary>
        private void OnQuickNavSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            // 1. 防护与类型模式匹配
            // 在方法末尾会将 SelectedItem 置为 null，这必然会二次触发 SelectionChanged 事件。
            // 这里的 is not 模式匹配完美充当了“递归阻断器（Recursion Breaker）”，遇到 null 时安全、静默地返回。
            if (sender is not ComboBox comboBox ||
                comboBox.SelectedItem is not StorageContainerViewModel targetContainer)
            {
                return;
            }

            // 2. 确保目标容器处于展开状态
            targetContainer.IsExpanded = true;

            // 3. 将滚动物理操作推迟到 Avalonia 的渲染布局队列末尾
            // 必须等待 UI 线程完成由 IsExpanded=true 带来的高度动态变化布局后，滚动定位才能精准命中
            Dispatcher.UIThread.Post(() =>
            {
                // 查找外层列表控件 (结合安全空值校验)
                var itemsControl = this.FindControl<ItemsControl>("ContainersItemsControl");
                if (itemsControl == null) return;

                // 找到对应数据模型所绑定的真实物理 UI 控件
                var containerControl = itemsControl.ContainerFromItem(targetContainer) as Control;

                // 执行 Avalonia 原生滚动定位
                containerControl?.BringIntoView();

                // 重置下拉框选中状态，确保下次用户连续点击同一个容器名也能触发 SelectionChanged
                comboBox.SelectedItem = null;

            }, DispatcherPriority.Loaded);
        }

        #endregion
    }
}