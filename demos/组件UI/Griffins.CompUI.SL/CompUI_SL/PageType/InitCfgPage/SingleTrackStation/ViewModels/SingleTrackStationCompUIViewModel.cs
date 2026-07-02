using System;
using System.Collections.ObjectModel;
using Griffins.CompUI.SL.ComUI_SL.InitCfgPage.Models;
using Griffins.CompUI.SL.InitCfgPage.Models;
using Griffins.Map;
using Griffins.Map.UI;
using ReactiveUI;

namespace Griffins.CompUI.SL.InitCfgPage.ViewModels
{
    /// <summary>
    /// 主界面视图模型
    /// </summary>
    public class SingleTrackStationCompUIViewModel : ReactiveObject, IDisposable
    {
        #region 私有字段

        private readonly bool isDesign;

        private readonly ICompUIRunTimeCallBack callBack;

        private const int STATION_COUNT = 3;

        #endregion  

        #region 响应式属性

        private ObservableCollection<SingleTrackStationItemCompUIViewModel> _stations;
        /// <summary>
        /// 单层轨道工位列表
        /// </summary>
        public ObservableCollection<SingleTrackStationItemCompUIViewModel> Stations
        {
            get => _stations;
            set => this.RaiseAndSetIfChanged(ref _stations, value);
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数（设计时使用）
        /// </summary>
        public SingleTrackStationCompUIViewModel() : this(true, null)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SingleTrackStationCompUIViewModel(bool isDesign, ICompUIRunTimeCallBack callBack)
        {
            this.isDesign = isDesign;
            this.callBack = callBack;

            Stations = new ObservableCollection<SingleTrackStationItemCompUIViewModel>();
            InitializeStations();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 初始化固定数量的工位
        /// </summary>
        private void InitializeStations()
        {
            for (int i = 0; i < STATION_COUNT; i++)
            {
                var stationViewModel = new SingleTrackStationItemCompUIViewModel(isDesign, callBack);
                stationViewModel.StationName = $"工位{i + 1}";
                Stations.Add(stationViewModel);
            }
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 设置数据
        /// </summary>
        public void SetData(SingleTrackStationCompUIModel model)
        {
            if (model?.Stations == null)
            {
                return;
            }

            // 遍历固定的工位列表，尝试从数据模型中加载对应的数据
            for (int i = 0; i < Stations.Count; i++)
            {
                var stationViewModel = Stations[i];

                // 如果模型中有对应的数据，则进行赋值
                if (i < model.Stations.Count)
                {
                    var stationModel = model.Stations[i];
                    stationViewModel.SupportShadowStation = stationModel.SupportShadowStation;
                    stationViewModel.EnableProximitySensor = stationModel.EnableProximitySensor;
                    stationViewModel.EnableBoardJamSignal = stationModel.EnableBoardJamSignal;
                    stationViewModel.GearSpeedList = stationModel.GearSpeedList;
                }
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        public SingleTrackStationCompUIModel GetData()
        {
            var model = new SingleTrackStationCompUIModel();

            foreach (var stationViewModel in Stations)
            {
                var stationModel = new SingleTrackStationItemCompUIModel
                {
                    SupportShadowStation = stationViewModel.SupportShadowStation,
                    EnableProximitySensor = stationViewModel.EnableProximitySensor,
                    EnableBoardJamSignal = stationViewModel.EnableBoardJamSignal,
                    GearSpeedList = stationViewModel.GearSpeedList
                };

                model.Stations.Add(stationModel);
            }

            return model;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (Stations != null)
            {
                foreach (var station in Stations)
                {
                    station.Dispose();
                }
                Stations.Clear();
            }
        }

        #endregion
    }
}