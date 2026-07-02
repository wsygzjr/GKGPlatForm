using Avalonia.Controls;
using Avalonia.VisualTree;
using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Views;
using Griffins.UI.General;
using MsBox.Avalonia.Enums;
using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Models;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.ViewModels
{
    /// <summary>
    /// 运控卡列表的视图模型
    /// 负责运控卡列表的增删改查、选中状态管理，以及与父ViewModel的事件通信
    /// </summary>
    public class ControlCardListViewModel : ReactiveObject
    {
        #region 字段与属性
        /// <summary>
        /// 运控卡列表数据（绑定到DataGrid）
        /// </summary>
        private ObservableCollection<ControlCardViewModel> _controlCardList = new();
        public ObservableCollection<ControlCardViewModel> ControlCardList
        {
            get => _controlCardList;
            set => this.RaiseAndSetIfChanged(ref _controlCardList, value);
        }

        /// <summary>
        /// 视图引用（用于获取窗口上下文，显示对话框）
        /// </summary>
        private Control? _viewReference;

        /// <summary>
        /// 当前选中的运控卡模型（绑定到DataGrid的SelectedItem）
        /// </summary>
        private ControlCardViewModel? _selectedControlCard;
        public ControlCardViewModel? SelectedControlCard
        {
            get => _selectedControlCard;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedControlCard, value);
                if (value == null)
                    return;
                onSelectedItemChanged(value);
            }
        }

        #region 事件定义
        /// <summary>
        /// 选中项变更事件（通知父ViewModel切换基础配置视图）
        /// </summary>
        public event EventHandler<ControlCardViewModel?>? SelectedItemChanged;

        /// <summary>
        /// 列表项变更事件（通知父ViewModel处理新增/编辑/删除后的ViewModel缓存）
        /// </summary>
        public event ItemChangedDelegate? ItemChanged;

        #endregion

        #region 命令定义
        /// <summary>
        /// 添加运控卡命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddCommand { get; }

        /// <summary>
        /// 编辑运控卡命令
        /// </summary>
        public ReactiveCommand<ControlCardViewModel, Unit> EditCommand { get; }

        /// <summary>
        /// 删除运控卡命令
        /// </summary>
        public ReactiveCommand<ControlCardViewModel, Unit> DeleteCommand { get; }
        #endregion
        #endregion

        /// <summary>
        /// 构造函数：初始化命令与默认配置
        /// </summary>
        public ControlCardListViewModel()
        {
            // 初始化命令
            AddCommand = ReactiveCommand.CreateFromTask(addControlCard);
            EditCommand = ReactiveCommand.Create<ControlCardViewModel>(editControlCard);
            DeleteCommand = ReactiveCommand.Create<ControlCardViewModel>(deleteControlCard);
        }

        /// <summary>
        /// 设置视图引用（用于文件选择器/消息框等UI操作）
        /// </summary>
        public void SetViewReference(Control view) => _viewReference = view;

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="controlCardInfos">数据层运控卡列表</param>
        public void CopyFrom(List<ControlCardInfo> controlCardInfos)
        {
            if (controlCardInfos == null) return;

            ControlCardList.Clear();
            List<string> cardIDs= controlCardInfos.Select(o=>o.CadID).ToList();
            foreach (var info in controlCardInfos)
            {
                var model = new ControlCardViewModel();
                model.CopyFrom(info); 
                model.SetExistingCadIds(cardIDs);
                ControlCardList.Add(model);
            }

            // 初始化时默认选中第一个项
            if (ControlCardList.Count > 0)
            {
                SelectedControlCard = ControlCardList[0];
            }
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="controlCardInfos">待回写的数据层列表</param>
        public void CopyTo(List<ControlCardInfo> controlCardInfos)
        {
            if (controlCardInfos == null)
                throw new ArgumentNullException(nameof(controlCardInfos), "待提取的运控卡列表不能为空");

            controlCardInfos.Clear();
            foreach (var model in ControlCardList)
            {
                var info = new ControlCardInfo(model.CadID, model.AxisCount);
                model.CopyTo(info);
                controlCardInfos.Add(info);
            }
        }

        /// <summary>
        /// 事件触发方法
        /// </summary>
        /// <param name="selectedCard"></param>
        private void onSelectedItemChanged(ControlCardViewModel? selectedCard)
        {
            SelectedItemChanged?.Invoke(this, selectedCard);
        }

        /// <summary>
        /// 添加运控卡：创建新模型并打开编辑窗口
        /// </summary>
        private async Task  addControlCard()
        {
            var newCardModel = new ControlCardViewModel();
            // 打开编辑窗口，确认后添加到列表
            var isConfirmed = await openEditWindow(newCardModel, isNew: true);
            if (isConfirmed)
            {
                ControlCardList.Add(newCardModel);
                ItemChanged?.Invoke(ControlCardChangedType.Add, newCardModel); // 通知父ViewModel创建缓存
                SelectedControlCard = newCardModel;
            }
        }

        /// <summary>
        /// 编辑运控卡：打开编辑窗口更新现有模型
        /// </summary>
        /// <param name="cardModel">待编辑的运控卡模型</param>
        private async void editControlCard(ControlCardViewModel cardModel)
        {
            if (cardModel == null) return;

            // 打开编辑窗口，确认后通知父ViewModel更新缓存
            var isConfirmed = await openEditWindow(cardModel, isNew: false);
            if (isConfirmed)
            {
                ItemChanged?.Invoke(ControlCardChangedType.Edit, cardModel);
            }
        }

        /// <summary>
        /// 删除运控卡：显示确认对话框，确认后从列表移除
        /// </summary>
        /// <param name="cardModel">待删除的运控卡模型</param>
        private async void deleteControlCard(ControlCardViewModel cardModel)
        {
            if (cardModel == null) return;

            var confirmResult = await MessageBox.ShowConfirmDialog(
                title: "删除确认",
                message: $"确定要删除运控卡吗？\n删除后数据不可恢复。",
                _viewReference
            );

            if (confirmResult != ButtonResult.Yes) return;

            ControlCardList.Remove(cardModel);
            ItemChanged?.Invoke(ControlCardChangedType.Delete, cardModel);

            if (ControlCardList.Count > 0)
            {
                SelectedControlCard = ControlCardList[0];
            }
            else
            {
                SelectedControlCard = null;
            }
        }

        /// <summary>
        /// 打开运控卡编辑窗口（新增/编辑通用）
        /// </summary>
        /// <param name="cardModel">待编辑的运控卡模型</param>
        /// <param name="isNew">是否为新增（true=新增，false=编辑）</param>
        /// <returns>窗口确认结果（true=确认，false=取消）</returns>
        private async Task<bool> openEditWindow(ControlCardViewModel cardModel, bool isNew)
        {
            // 校验视图引用是否有效
            var parentWindow = _viewReference?.GetVisualRoot() as Window;
            if (parentWindow == null)
            {
                await MessageBox.ShowErrorDialog("错误", "无法获取窗口上下文，请先初始化视图引用",null);
                return false;
            }

            // 创建编辑窗口与ViewModel
            var editVm = new ControlCardEditWindowViewModel(cardModel, isNew);
            editVm.EditCopy.SetExistingCadIds(ControlCardList.Select(o => o.CadID).ToList());
            var editWindow = new ControlCardEditWindow
            {
                DataContext = editVm,
                Title = isNew ? "添加运控卡" : $"编辑运控卡",
                Width = 500,
                Height = 363,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false
            };

            return await editWindow.ShowDialog<bool>(parentWindow);
        }

        /// <summary>
        /// 列表项变更委托（用于通知父ViewModel处理ViewModel/View缓存）
        /// </summary>
        /// <param name="changedType">变更类型（新增/编辑/删除）</param>
        /// <param name="cardModel">变更的运控卡模型</param>
        public delegate void ItemChangedDelegate(ControlCardChangedType changedType, ControlCardViewModel cardModel);
    }
}