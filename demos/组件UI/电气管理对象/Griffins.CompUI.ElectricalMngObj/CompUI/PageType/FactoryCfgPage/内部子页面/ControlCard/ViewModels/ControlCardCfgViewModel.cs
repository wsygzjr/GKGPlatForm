using Avalonia.Controls;
using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Views;
using GF_Gereric;
using ReactiveUI;
using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Griffins.UI;
using System.Collections.ObjectModel;

namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.ViewModels
{
    /// <summary>
    /// 运控卡配置的视图模型
    /// 负责管理运控卡列表与基础配置视图的联动，以及配置数据的提取与保存
    /// </summary>
    public class ControlCardCfgViewModel : ReactiveObject
    {
        /// <summary>
        /// 内部子页面配置信息（序列化后的字节数组）
        /// </summary>
        private byte[]? _cfgInfo;

        /// <summary>
        /// 页面布局根视图引用（用于UI上下文关联）
        /// </summary>
        private Control? _viewReference;


        private ControlCardBaseCfgViewModel? _selectedItem;
        /// <summary>
        /// 运控卡基础配置集合
        /// </summary>
        public ObservableCollection<ControlCardBaseCfgViewModel> ItemsSource { get; set; }
            = new ObservableCollection<ControlCardBaseCfgViewModel>();

        /// <summary>
        /// 当前选中的运控卡基础配置项
        /// </summary>
        public ControlCardBaseCfgViewModel? SelectedItem
        {
            get => _selectedItem;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedItem, value);
            }
        }



        /// <summary>
        /// 运控卡配置信息模型（数据层核心对象）
        /// </summary>
        private ControlCardCfgInfo _controlCardCfgInfo = new();

        /// <summary>
        /// 缓存运控卡基础配置视图模型（key: CadID）
        /// </summary>
        private readonly Dictionary<Guid, ControlCardBaseCfgViewModel> _dicControlCardBaseCfgViewModel = new();


        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
      
        private ControlCardBaseCfgViewModel? _selectedBaseCfgVm;
        public ControlCardBaseCfgViewModel? SelectedBaseCfgVm
        {
            get => _selectedBaseCfgVm;
            set => this.RaiseAndSetIfChanged(ref _selectedBaseCfgVm, value);
        }
        /// <summary>
        /// 运控卡列表的视图模型（管理列表数据与交互）
        /// </summary>
        public ControlCardListViewModel ControlCardListViewModel { get; }

        /// <summary>
        /// 内部子页面配置信息
        /// </summary>
        public byte[]? CfgInfo
        {
            get
            {
                try
                {
                    extract(_controlCardCfgInfo);
                    _cfgInfo = JsonObjConvert.ToJSonBytes(_controlCardCfgInfo);
                    return _cfgInfo;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 构造函数：初始化列表ViewModel并订阅事件
        /// </summary>
        public ControlCardCfgViewModel()
        {
            ControlCardListViewModel = new ControlCardListViewModel();
            
            // 订阅值变更事件
            subscribeValueChanges();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="cfgInfo">内部子页面配置信息</param>
        public void Init( byte[]? cfgInfo)
        {
            // 清理旧缓存
            clearCaches();

            _cfgInfo = cfgInfo;
            if (cfgInfo != null && cfgInfo.Length > 0)
            {
                try
                {
                    _controlCardCfgInfo = JsonObjConvert.FromJSonBytes<ControlCardCfgInfo>(cfgInfo);
                }
                catch (Exception ex)
                {
                    _controlCardCfgInfo = new ControlCardCfgInfo();
                }
            }

            ControlCardListViewModel.CopyFrom(_controlCardCfgInfo.ControlCardList);

            //为每个控制卡分别实例视图模型和布局控件
            foreach (var controlCard in _controlCardCfgInfo.ControlCardList.ToArray())
            {
                createControlCardBaseViewModel(controlCard);
            }

            // 默认选中第一个运控卡
            if (_controlCardCfgInfo.ControlCardList.Any())
            {
                var firstCardId = _controlCardCfgInfo.ControlCardList.First().CadID;
                var firstCard = ControlCardListViewModel.ControlCardList.FirstOrDefault(c => c.CadID == firstCardId);
                if (firstCard != null)
                {
                    ControlCardListViewModel.SelectedControlCard = firstCard;
                }
            }
        }

        /// <summary>
        /// 设置视图引用（供列表ViewModel使用UI上下文）
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view ?? throw new Exception("视图引用不能为空");
            ControlCardListViewModel.SetViewReference(view);
        }

        /// <summary>
        /// 处理运控卡列表项变更事件（新增/编辑/删除）
        /// </summary>
        /// <param name="changedType"></param>
        /// <param name="cardModel"></param>
        private void onItemChanged(ControlCardChangedType changedType, ControlCardViewModel cardModel)
        {
            try
            {
                if (cardModel == null)
                    throw new Exception("运控卡数据无效");
                switch (changedType)
                {
                    case ControlCardChangedType.Add:
                        //新增时创建对应的基础配置ViewModel和View
                        var newCardInfo = new ControlCardInfo(cardModel.CadID, cardModel.AxisCount);
                        cardModel.CopyTo(newCardInfo);
                        createControlCardBaseViewModel(newCardInfo);
                        break;
                    case ControlCardChangedType.Edit:
                        // 编辑时更新轴数配置
                        if (_dicControlCardBaseCfgViewModel.TryGetValue(cardModel.UniqueID, out var existingVm))
                            existingVm.UpdateAaxisCount(cardModel.AxisCount);
                        else
                            throw new Exception($"未找到ID为{cardModel.CadID}的运控卡配置");
                        break;
                    case ControlCardChangedType.Delete:
                        // 删除时清理缓存并取消订阅事件
                        if (_dicControlCardBaseCfgViewModel.TryGetValue(cardModel.UniqueID, out var vmToRemove))
                        {
                            vmToRemove.AfterModified -= onAfterModified;
                            _dicControlCardBaseCfgViewModel.Remove(cardModel.UniqueID);
                        }
                        break;
                    default:
                        throw new Exception($"未知操作类型：{changedType}");
                }

                onAfterModified(this, null!);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 列表选择控制卡事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="selectedCard">控制卡</param>
        private void onControlCardSelected(object? sender, ControlCardViewModel? selectedCard)
        {
            try
            {
                if (selectedCard == null)
                    throw new Exception("请选择一个运控卡");

                Guid uniqueID = selectedCard.UniqueID;
                _dicControlCardBaseCfgViewModel.TryGetValue(selectedCard.UniqueID, out var vm);
                SelectedBaseCfgVm = vm;
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 提取所有运控卡的配置数据到模型（ViewModel -> 数据模型）
        /// </summary>
        private void extract(ControlCardCfgInfo controlCardCfgInfo)
        {
            if (controlCardCfgInfo == null)
                throw new ArgumentNullException(nameof(controlCardCfgInfo));

            // 提取列表数据
            ControlCardListViewModel.CopyTo(controlCardCfgInfo.ControlCardList);

            // 提取每个运控卡的基础配置
            foreach (var cardInfo in controlCardCfgInfo.ControlCardList)
            {
                if (!_dicControlCardBaseCfgViewModel.TryGetValue(cardInfo.UniqueID, out var baseVm))
                {
                    throw new KeyNotFoundException($"未找到ID为{cardInfo.CadID}的运控卡基础配置ViewModel");
                }
                baseVm.CopyTo(cardInfo.ControlCardBaseCfg);
            }
        }
        

        /// <summary>
        /// 创建运控卡基础配置视图和布局
        /// </summary>
        /// <param name="controlCard"></param>
        private void createControlCardBaseViewModel(ControlCardInfo controlCard)
        {
            if (controlCard == null)
                throw new ArgumentNullException(nameof(controlCard), "运控卡信息不能为空");

            var uniqueID = controlCard.UniqueID;
            if (uniqueID==Guid.Empty)
            {
                uniqueID = Guid.NewGuid();
                controlCard.UniqueID=uniqueID;
            }

            // 避免重复创建
            if (_dicControlCardBaseCfgViewModel.ContainsKey(uniqueID))
                return;


            var baseCfgVm = new ControlCardBaseCfgViewModel();
            baseCfgVm.AfterModified += onAfterModified;
            baseCfgVm.CopyFrom(controlCard.ControlCardBaseCfg, controlCard.AxisCount);
            // 缓存实例
            _dicControlCardBaseCfgViewModel[uniqueID] = baseCfgVm;
        }
       
        /// <summary>
        /// 创建空状态视图
        /// </summary>
        private Control createEmptyStateView(string message)
        {
            return new TextBlock
            {
                Text = message,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Foreground = Avalonia.Media.Brushes.Gray,
                Margin = new Avalonia.Thickness(10)
            };
        }
        /// <summary>
        /// 清理缓存的ViewModel和View（取消事件订阅避免内存泄漏）
        /// </summary>
        private void clearCaches()
        {
            _dicControlCardBaseCfgViewModel.Clear();
        }

        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            // 订阅列表事件
            ControlCardListViewModel.SelectedItemChanged += onControlCardSelected;
            ControlCardListViewModel.ItemChanged += onItemChanged;
        }

        /// <summary>
        /// 值改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onValueChanged(object? sender, ValueChangedEventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }
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