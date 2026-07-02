using Avalonia.Controls;
using DynamicData;
using Griffins.UI.General;
using MsBox.Avalonia.Enums;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels
{
    /// <summary>
    /// DataGrid列表基类（支持全选、单选、批量删除、单条删除、新增、上移、下移）
    /// 泛型约束：
    /// - TItemVm：项视图模型类型（继承DataGridItemBaseViewModel<TItemEntity>）
    /// - TItemEntity：实体类型（继承EntityBase）
    /// </summary>
    public abstract class DataGridListBaseViewModel<TItemVm, TItemEntity> : ReactiveObject
        where TItemVm : DataGridItemBaseViewModel<TItemEntity>, new()
        where TItemEntity : EntityBase, new()
    {
        #region 强类型辅助方法
        /// <summary>
        /// 获取IsSelected值
        /// </summary>
        private bool getIsSelected(TItemVm item) => item.IsSelected;

        /// <summary>
        /// 设置IsSelected值
        /// </summary>
        private void setIsSelected(TItemVm item, bool value) => item.IsSelected = value;

        /// <summary>
        /// 绑定是否选中的变更事件
        /// </summary>
        private void bindIsSelectedChangedEvent(TItemVm item) => item.IsSelectedChanged += onItemModified;

        /// <summary>
        /// 解绑是否选中的变更事件
        /// </summary>
        private void unbindIsSelectedChangedEvent(TItemVm item) => item.IsSelectedChanged -= onItemModified;
        #endregion

        #region 全选状态控制
        private bool _isSelectAllChangingByItem;
        private bool _isSelectAll;

        /// <summary>
        /// 全选开关（绑定DataGrid表头复选框）
        /// </summary>
        public bool IsSelectAll
        {
            get => _isSelectAll;
            set
            {
                if (_isSelectAll == value) return;
                this.RaiseAndSetIfChanged(ref _isSelectAll, value);

                if (value)
                {
                    // 全选逻辑
                    foreach (var item in ItemsSource)
                    {
                        unbindIsSelectedChangedEvent(item);
                        setIsSelected(item, true);
                    }
                    SelectedItems.Clear();
                    foreach (var item in ItemsSource)
                    {
                        SelectedItems.Add(item);
                        bindIsSelectedChangedEvent(item);
                    }
                }
                else
                {
                    // 取消全选逻辑
                    if (_isSelectAllChangingByItem)
                    {
                        _isSelectAllChangingByItem = false;
                        return;
                    }

                    var itemsCopy = ItemsSource.ToList();
                    foreach (var item in itemsCopy)
                    {
                        unbindIsSelectedChangedEvent(item);
                        setIsSelected(item, false);
                    }
                    SelectedItems.Clear();
                    foreach (var item in itemsCopy)
                    {
                        bindIsSelectedChangedEvent(item);
                    }
                }
            }
        }
        #endregion

        #region 公共属性
        private readonly ObservableCollection<TItemVm> _itemsSource = new();
        private readonly ObservableCollection<TItemVm> _selectedItems = new();
        private TItemVm? _selectedItem;
        protected Control? _viewReference;

        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// 数据源列表（绑定DataGrid.ItemsSource）
        /// </summary>
        public ObservableCollection<TItemVm> ItemsSource => _itemsSource;

        /// <summary>
        /// 选中的多条项（绑定DataGrid.SelectedItems）
        /// </summary>
        public ObservableCollection<TItemVm> SelectedItems => _selectedItems;

        /// <summary>
        /// 选中前的选中项
        /// </summary>
        public TItemVm? PreSelectedItem { get; private set; }

        /// <summary>
        /// 当前选中项（绑定DataGrid.SelectedItem）
        /// </summary>
        public TItemVm? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (value == null || value == _selectedItem) return;
                PreSelectedItem = _selectedItem;
                this.RaiseAndSetIfChanged(ref _selectedItem, value);
            }
        }

        /// <summary>
        /// 批量删除按钮是否可用（选中项>0）
        /// </summary>
        public bool CanBatchDelete => SelectedItems.Count > 0;
        #endregion

        #region 公共命令
        /// <summary>
        /// 新增项命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddCommand { get; }

        /// <summary>
        /// 单条删除命令
        /// </summary>
        public ReactiveCommand<TItemVm, bool> DeleteCommand { get; }

        /// <summary>
        /// 批量删除命令
        /// </summary>
        public ReactiveCommand<Unit, bool> BatchDeleteCommand { get; }

        /// <summary>
        /// 上移命令
        /// </summary>
        public ReactiveCommand<TItemVm, Unit> UpMoveCommand { get; }

        /// <summary>
        /// 下移命令
        /// </summary>
        public ReactiveCommand<TItemVm, Unit> DownMoveCommand { get; }
        
        #endregion


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="minDeleteCount">最小保留项数（防止删除全部）</param>
        protected DataGridListBaseViewModel(int minDeleteCount = 0)
        {
            // 初始化命令
            var canDelete = this.WhenAnyValue(
                x => x.ItemsSource.Count,
                count => count > minDeleteCount
            );

            AddCommand = ReactiveCommand.CreateFromTask(_addItem);
            DeleteCommand = ReactiveCommand.CreateFromTask<TItemVm, bool>(_deleteItem, canDelete);
            BatchDeleteCommand = ReactiveCommand.CreateFromTask(_batchDeleteItems, canDelete);

            // 上移命令：仅当选中单个项且不是第1项时可用
            UpMoveCommand = ReactiveCommand.Create<TItemVm>(
                executeUpMove,
                this.WhenAnyValue(
                    vm => vm.SelectedItem,
                    vm => vm.ItemsSource.Count,
                    (selectedItem, count) => selectedItem != null && count > 1 && ItemsSource.IndexOf(selectedItem) > 0
                )
            );

            // 下移命令：仅当选中单个项且不是最后1项时可用
            DownMoveCommand = ReactiveCommand.Create<TItemVm>(
                executeDownMove,
                this.WhenAnyValue(
                    vm => vm.SelectedItem,
                    vm => vm.ItemsSource.Count,
                    (selectedItem, count) => selectedItem != null && count > 1 && ItemsSource.IndexOf(selectedItem) < count - 1
                )
            );

            // 监听选中项数量变化，更新批量删除按钮状态
            this.WhenAnyValue(vm => vm.SelectedItems.Count)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(CanBatchDelete)));

            // 监听列表变化，绑定/解绑事件 + 更新全选状态
            ItemsSource.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (TItemVm newItem in e.NewItems)
                    {
                        bindIsSelectedChangedEvent(newItem);
                    }
                }
                if (e.OldItems != null)
                {
                    foreach (TItemVm oldItem in e.OldItems)
                    {
                        unbindIsSelectedChangedEvent(oldItem);
                        SelectedItems.Remove(oldItem);
                    }
                }
                updateSelectAllState();
                onAfterModified(this, new EventArgs());
            };
        }

        #region 全选状态同步
        /// <summary>
        /// 单行状态变化触发
        /// </summary>
        private void onItemModified(object? sender, EventArgs e)
        {
            if (sender is not TItemVm item) return;

            // 同步SelectedItems集合
            var isSelected = getIsSelected(item);
            if (isSelected && !SelectedItems.Contains(item))
            {
                SelectedItems.Add(item);
            }
            else if (!isSelected && SelectedItems.Contains(item))
            {
                SelectedItems.Remove(item);
            }

            // 更新全选框状态
            updateSelectAllState(fromItemChange: true);
        }

        /// <summary>
        /// 更新全选框状态
        /// </summary>
        private void updateSelectAllState(bool fromItemChange = false)
        {
            if (!ItemsSource.Any())
            {
                if (IsSelectAll != false)
                {
                    if (fromItemChange) _isSelectAllChangingByItem = true;
                    IsSelectAll = false;
                    if (fromItemChange) _isSelectAllChangingByItem = false;
                }
                return;
            }

            bool shouldSelectAll = ItemsSource.All(item => getIsSelected(item));
            if (IsSelectAll != shouldSelectAll)
            {
                if (fromItemChange) _isSelectAllChangingByItem = true;
                IsSelectAll = shouldSelectAll;
                if (fromItemChange) _isSelectAllChangingByItem = false;
            }
        }
        #endregion

        #region 抽象方法
        /// <summary>
        /// 新增项
        /// </summary>
        protected abstract Task _addItem();

        /// <summary>
        /// 单条删除项
        /// </summary>
        protected virtual async Task<bool> _deleteItem(TItemVm item)
        {
            try
            {
                if (item == null) return false;

                var confirmResult = await MessageBox.ShowConfirmDialog(
                    "删除确认",
                    $"确定要删除吗？删除后不可恢复。",
                    _viewReference
                );

                if (confirmResult == ButtonResult.Yes)
                {
                    item.AfterModified -= onAfterModified;
                    ItemsSource.Remove(item);
                    refreshSerialNumber();
                    SelectedItem = ItemsSource.Count > 0 ? ItemsSource[0] : null;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                await MessageBox.ShowErrorDialog("错误", $"{ex.Message}", _viewReference);
                return false;
            }
            
        }

        /// <summary>
        /// 批量删除项
        /// </summary>
        protected virtual async Task<bool> _batchDeleteItems()
        {
            try
            {
                if (SelectedItems.Count == 0) return false;

                var confirmResult = await MessageBox.ShowConfirmDialog(
                    "批量删除确认",
                    $"确定要删除选中的{SelectedItems.Count}条项吗？删除后不可恢复。",
                    _viewReference
                );

                if (confirmResult == ButtonResult.Yes)
                {
                    foreach (var item in SelectedItems.ToArray())
                    {
                        item.AfterModified -= onAfterModified;
                        ItemsSource.Remove(item);
                    }
                    SelectedItems.Clear();
                    refreshSerialNumber();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                await MessageBox.ShowErrorDialog("错误", $"{ex.Message}", _viewReference);
                return false;
            }

        }

        /// <summary>
        /// 执行上移操作
        /// </summary>
        protected virtual void executeUpMove(TItemVm itemView)
        {
            try
            {
                if (SelectedItem == null) return;

                int currentIndex = ItemsSource.IndexOf(itemView);
                if (currentIndex <= 0) return;

                // 交换位置
                ItemsSource.Move(currentIndex, currentIndex - 1);
                refreshSerialNumber();
                SelectedItem = ItemsSource[currentIndex - 1];
                //重新添加元素：解决问题：界面没有刷新元素排序
                var source = ItemsSource.ToList();
                ItemsSource.Clear();
                ItemsSource.AddRange(source);
            }
            catch (Exception ex)
            {
                Task task=  MessageBox.ShowErrorDialog("错误", $"{ex.Message}", _viewReference);
            }
            
        }

        /// <summary>
        /// 执行下移操作
        /// </summary>
        protected virtual void executeDownMove(TItemVm itemView)
        {
            try
            {
                if (SelectedItem == null) return;

                int currentIndex = ItemsSource.IndexOf(itemView);
                if (currentIndex >= ItemsSource.Count - 1) return;

                // 交换位置
                ItemsSource.Move(currentIndex, currentIndex + 1);
                
                refreshSerialNumber();
                SelectedItem = ItemsSource[currentIndex + 1];
                //重新添加元素：解决问题：界面没有刷新元素排序
                var source = ItemsSource.ToList();
                ItemsSource.Clear();
                ItemsSource.AddRange(source);
            }
            catch (Exception ex)
            {
                Task task = MessageBox.ShowErrorDialog("错误", $"{ex.Message}", _viewReference);
            }
            
        }

        #endregion

        #region 公共方法
        /// <summary>
        /// 设置视图引用（用于弹窗）
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view;
        }

        /// <summary>
        /// 初始化列表数据（从实体集合加载）
        /// </summary>
        public void CopyFrom(List<TItemEntity> entityList)
        {
            //为空则返回，防止覆盖默认生成的一项数据
            if (entityList == null|| entityList.Count==0) return;

            ItemsSource.Clear();

            foreach (var entity in entityList)
            {
                var vm = new TItemVm();
                vm.SetViewReference(_viewReference!);
                vm.AfterModified += onAfterModified;
                vm.CopyFrom(entity);
                ItemsSource.Add(vm);
            }
            refreshSerialNumber();
        }

        /// <summary>
        /// 提取列表数据到实体集合
        /// </summary>
        public void CopyTo(List<TItemEntity> targetList)
        {
            if (targetList == null) return;

            targetList.Clear();
            foreach (var vm in ItemsSource)
            {
                var entity = new TItemEntity();
                vm.CopyTo(entity);
                targetList.Add(entity);
            }
        }

        /// <summary>
        /// 刷新所有项的序号
        /// </summary>
        private void refreshSerialNumber()
        {
            int newSerialNumber = 1;
            foreach (var item in ItemsSource)
            {
                item.SerialNumber = newSerialNumber++;
            }
        }
        #endregion

        #region 值改变事件
        /// <summary>
        /// 值改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onAfterModified(object? sender, EventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }
        #endregion
    }
}