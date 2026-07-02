using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Collections.Specialized;
using System.ComponentModel;
using Griffins.CompUI.SL.ComUI_SL.InitCfgPage.Models;
using Griffins.Map;
using ReactiveUI;
using Griffins.Map.UI;

namespace Griffins.CompUI.SL.InitCfgPage.ViewModels
{
    /// <summary>
    /// 单层轨道工位视图模型
    /// </summary>
    public class SingleTrackStationItemCompUIViewModel : ReactiveObject, IDisposable
    {
        #region 私有字段

        private readonly bool isDesign;
        private readonly ICompUIRunTimeCallBack callBack;
        private bool _suppressIsSelectAll;

        #endregion

        #region 响应式属性

        private object _viewTag;
        /// <summary>
        /// 对应View的Tag属性（支持双向绑定）
        /// </summary>
        public object ViewTag
        {
            get => _viewTag;
            set => this.RaiseAndSetIfChanged(ref _viewTag, value);
        }

        private bool _supportShadowStation;
        /// <summary>
        /// 是否支持影子工位
        /// </summary>
        public bool SupportShadowStation
        {
            get => _supportShadowStation;
            set => this.RaiseAndSetIfChanged(ref _supportShadowStation, value);
        }

        private bool _enableProximitySensor;
        /// <summary>
        /// 是否启用接近感应器
        /// </summary>
        public bool EnableProximitySensor
        {
            get => _enableProximitySensor;
            set => this.RaiseAndSetIfChanged(ref _enableProximitySensor, value);
        }

        private bool _enableBoardJamSignal;
        /// <summary>
        /// 是否启用出板卡料信号
        /// </summary>
        public bool EnableBoardJamSignal
        {
            get => _enableBoardJamSignal;
            set => this.RaiseAndSetIfChanged(ref _enableBoardJamSignal, value);
        }

        private ObservableCollection<TrackItemModel>? _gearSpeedList;
        /// <summary>
        /// 速度挡位列表
        /// </summary>
        public ObservableCollection<TrackItemModel>? GearSpeedList
        {
            get => _gearSpeedList;
            set
            {
                if (_gearSpeedList == value) return;
                UnsubscribeFromList(_gearSpeedList);
                this.RaiseAndSetIfChanged(ref _gearSpeedList, value);
                SubscribeToList(_gearSpeedList);
                UpdateIsSelectAll();
            }
        }

        private TrackItemModel? _selectedItem;
        /// <summary>
        /// 当前选中的数据项
        /// </summary>
        public TrackItemModel? SelectedItem
        {
            get => _selectedItem;
            set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
        }

        private bool _isSelectAll;
        /// <summary>
        /// 是否全选（头部复选框）
        /// </summary>
        public bool IsSelectAll
        {
            get => _isSelectAll;
            set
            {
                // 如果值没有变化，直接返回
                if (_isSelectAll == value) return;

                // 先更新本地字段，避免递归调用
                _isSelectAll = value;
                this.RaisePropertyChanged(nameof(IsSelectAll));

                if (GearSpeedList != null && GearSpeedList.Count > 0)
                {
                    try
                    {
                        _suppressIsSelectAll = true; // 阻止递归调用UpdateIsSelectAll

                        if (value)
                        {
                            // 全选：全部设为true
                            foreach (var item in GearSpeedList)
                            {
                                item.IsSelected = true;
                            }
                        }
                        else
                        {
                            // 取消全选：全部设为false
                            foreach (var item in GearSpeedList)
                            {
                                item.IsSelected = false;
                            }
                        }
                    }
                    finally
                    {
                        _suppressIsSelectAll = false;
                    }
                }
            }
        }

        private bool _readOnly;
        /// <summary>
        /// 只读属性
        /// </summary>
        public bool ReadOnly
        {
            get => _readOnly;
            set => this.RaiseAndSetIfChanged(ref _readOnly, value);
        }

        private string _stationName;
        /// <summary>
        /// 工位名称
        /// </summary>
        public string StationName
        {
            get => _stationName;
            set => this.RaiseAndSetIfChanged(ref _stationName, value);
        }

        #endregion

        #region 命令

        /// <summary>
        /// 按钮点击命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> ButtonClickCommand { get; }

        /// <summary>
        /// 添加命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddCommand { get; }

        /// <summary>
        /// 删除命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数（初始化）
        /// </summary>
        public SingleTrackStationItemCompUIViewModel(bool isDesign, ICompUIRunTimeCallBack callBack)
        {
            this.isDesign = isDesign;
            this.callBack = callBack;

            // 初始化响应式属性
            this.SupportShadowStation = false;
            this.EnableProximitySensor = false;
            this.EnableBoardJamSignal = false;
            this.GearSpeedList = new ObservableCollection<TrackItemModel>();

            // 初始化命令绑定
            ButtonClickCommand = ReactiveCommand.Create(OnButtonClicked);
            AddCommand = ReactiveCommand.Create(OnAddItems);
            DeleteCommand = ReactiveCommand.Create(OnDeleteItems);
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 释放非托管资源
        /// </summary>
        public void Dispose()
        {
            UnsubscribeFromList(GearSpeedList);
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 订阅监听集合及集合内项的变化
        /// </summary>
        /// <param name="list">集合</param>
        private void SubscribeToList(ObservableCollection<TrackItemModel>? list)
        {
            if (list == null)
            {
                return;
            }

            list.CollectionChanged += GearSpeedList_CollectionChanged;// 监听集合变化事件，处理新增和删除的项
            foreach (var item in list)
            {
                if (item is INotifyPropertyChanged inpc)
                {
                    inpc.PropertyChanged += TrackItem_PropertyChanged;// 监听每个项的属性变化事件
                }
            }
        }

        /// <summary>
        /// 取消订阅监听集合及集合内项的变化
        /// </summary>
        /// <param name="list">集合</param>
        private void UnsubscribeFromList(ObservableCollection<TrackItemModel>? list)
        {
            if (list == null)
            {
                return;
            }

            list.CollectionChanged -= GearSpeedList_CollectionChanged;
            foreach (var item in list)
            {
                if (item is INotifyPropertyChanged inpc)
                {
                    inpc.PropertyChanged -= TrackItem_PropertyChanged;
                }
            }
        }

        /// <summary>
        /// 集合变化事件处理
        /// </summary>
        private void GearSpeedList_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var newItem in e.NewItems.OfType<TrackItemModel>())
                {
                    if (newItem is INotifyPropertyChanged inpc)
                    {
                        inpc.PropertyChanged += TrackItem_PropertyChanged;// 监听每个新增项的属性变化事件
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (var oldItem in e.OldItems.OfType<TrackItemModel>())
                {
                    if (oldItem is INotifyPropertyChanged inpc)
                    {
                        inpc.PropertyChanged -= TrackItem_PropertyChanged;// 取消监听每个旧项的属性变化事件
                    }
                }
            }

            UpdateIsSelectAll();
        }

        /// <summary>
        /// 处理单项属性变化事件
        /// </summary>
        private void TrackItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TrackItemModel.IsSelected))
            {
                if (_suppressIsSelectAll)
                {
                    return;
                }

                UpdateIsSelectAll();
            }
        {
            if (GearSpeedList == null)
            {
                return;
            }

            int nextIndex = GearSpeedList.Count + 1;
            GearSpeedList.Add(new TrackItemModel { IsSelected = false, Index = nextIndex, Gear = 1, Speed = 100 });
            // 不需要调用UpdateIsSelectAll，因为CollectionChanged事件会处理
        }

        /// <summary>
        /// 删除数据项
        /// </summary>
        private void OnDeleteItems()
        {
            if (GearSpeedList == null)
            {
                return;
            }

            try
            {
                _suppressIsSelectAll = true; // 阻止递归调用UpdateIsSelectAll

                var selectedItems = GearSpeedList.Where(item => item.IsSelected).ToList();
                foreach (var item in selectedItems)
                {
                    GearSpeedList.Remove(item);
                }

                // 重新编号
                for (int i = 0; i < GearSpeedList.Count; i++)
                {
                    GearSpeedList[i].Index = i + 1;
                }
            }
            finally
            {
                _suppressIsSelectAll = false;
                UpdateIsSelectAll(); // 确保同步头部复选框状态
            }
        }

        /// <summary>
        /// 更新是否全选状态
        /// </summary>
        private void UpdateIsSelectAll()
        {
            if (_suppressIsSelectAll)
            {
                return;
            }

            // 如果列表为空，设置为未全选状态
            if (GearSpeedList == null || GearSpeedList.Count == 0)
            {
                if (_isSelectAll != false)
                {
                    _isSelectAll = false;
                    this.RaisePropertyChanged(nameof(IsSelectAll));
                }

                return;
            }

            // 检查是否所有项都被选中
            bool allSelected = GearSpeedList.All(item => item.IsSelected);
            if (_isSelectAll != allSelected)
            {
                _isSelectAll = allSelected;
                this.RaisePropertyChanged(nameof(IsSelectAll));
            }
        }

        #endregion
    }
}