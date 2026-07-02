using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using GKG.CompUI.LoadUnload.ControlPanel.ViewModels;

namespace GKG.CompUI.LoadUnload.ControlPanel.Views
{
    /// <summary>
    /// SOP 向导主视图 (View 后台代码)
    /// </summary>
    public partial class SopWizardView : ReactiveUserControl<SopWizardViewModel>
    {
        public SopWizardView()
        {
            InitializeComponent();
        }

        #region 纯 UI 交互事件

        /// <summary>
        /// 处理下拉框选择变化：快速定位容器，并将其平滑滚动至可视区域
        /// </summary>
        private void OnQuickNavSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            // 1. 防呆拦截与类型解构
            if (sender is not ComboBox comboBox ||
                comboBox.SelectedItem is not StorageContainerViewModel targetContainer)
            {
                return;
            }

            // 2. 数据驱动：修改 ViewModel 状态，触发展开动画
            targetContainer.IsExpanded = true;

            // 3. 将滚动操作推迟到 Avalonia 的渲染布局队列末尾
            Dispatcher.UIThread.Post(() =>
            {
                // 通过名字找到外层列表，再找到对应数据的物理控件
                var itemsControl = this.FindControl<ItemsControl>("ContainersItemsControl");
                var containerControl = itemsControl?.ContainerFromItem(targetContainer) as Control;

                // 执行 Avalonia 原生滚动定位
                containerControl?.BringIntoView();

                // 4. 重置下拉框选中状态，确保下次用户点击同一个容器也能触发该事件
                comboBox.SelectedItem = null;

            }, DispatcherPriority.Loaded);
        }

        #endregion
    }
}