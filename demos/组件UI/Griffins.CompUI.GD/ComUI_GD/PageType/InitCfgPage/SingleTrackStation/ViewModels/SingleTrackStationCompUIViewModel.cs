using System.Collections.ObjectModel;
using System.Reactive;
using Griffins.CompUI.GD.ComUI_GD.InitCfgPage.Models;
using Griffins.Map;
using Griffins.Map.UI;
using ReactiveUI;

namespace Griffins.CompUI.GD.InitCfgPage.ViewModels
{
    /// <summary>
    /// 单层轨道工位视图模型
    /// </summary>
    public class SingleTrackStationCompUIViewModel : ReactiveObject, IDisposable
    {
        #region 私有字段

        private readonly bool isDesign;

        private readonly ICompUIRunTimeCallBack callBack;

        #endregion

        #region 响应式属性

        private object? _viewTag;
        /// <summary>
        /// 对应View的Tag属性（支持双向绑定）
        /// </summary>
        public object? ViewTag
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
            set => this.RaiseAndSetIfChanged(ref _gearSpeedList, value);
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



        private bool _readOnly;
        /// <summary>
        /// 只读属性
        /// </summary>
        public bool ReadOnly
        {
            get => _readOnly;
            set => this.RaiseAndSetIfChanged(ref _readOnly, value);
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

        /// <summary>
        /// 全选命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> SelectAllCommand { get; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数（初始化）
        /// </summary>
        public SingleTrackStationCompUIViewModel(bool isDesign, ICompUIRunTimeCallBack callBack)
        {
            this.isDesign = isDesign;
            this.callBack = callBack;

            // 初始化响应式属性
            this.SupportShadowStation = false;
            this.EnableProximitySensor = false;
            this.EnableBoardJamSignal = false;
            this.GearSpeedList = new();

            // 初始化命令绑定
            ButtonClickCommand = ReactiveCommand.Create(OnButtonClicked);
            AddCommand = ReactiveCommand.Create(OnAddItems);
            DeleteCommand = ReactiveCommand.Create(OnDeleteItems);
            SelectAllCommand = ReactiveCommand.Create(OnSelectAllItems);
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 释放非托管资源
        /// </summary>
        public void Dispose()
        {
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 按钮点击事件
        /// </summary>
        private void OnButtonClicked()
        {
            if (isDesign)
            {
                return;
            }

            var response = this.callBack.ExecConfigSvrCtlCmd("SingleTrackStation", null);
        }

        /// <summary>
        /// 添加数据项
        /// </summary>
        private void OnAddItems()
        {
            if (GearSpeedList == null)
            {
                return;
            }

            int nextIndex = GearSpeedList.Count + 1;
            GearSpeedList.Add(new TrackItemModel { IsSelected = false, Index = nextIndex, Gear = 1, Speed = 100 });
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

        /// <summary>
        /// 选中所有数据项
        /// </summary>
        private void OnSelectAllItems()
        {
            if (GearSpeedList == null)
            {
                return;
            }

            bool allSelected = GearSpeedList.All(item => item.IsSelected);
            foreach (var item in GearSpeedList)
            {
                item.IsSelected = !allSelected;
            }
        }
        #endregion
    }
}