using System;
using System.Collections.Specialized;
using System.ComponentModel;
using Griffins.CompUI.SL.ComUI_SL.InitCfgPage.Models;
using Griffins.CompUI.SL.InitCfgPage.Models;
using Griffins.Map;
using Griffins.Map.UI;
using ReactiveUI;

namespace Griffins.CompUI.SL.InitCfgPage.ViewModels
{
    public class RegularSingleTrackCompUIViewModel : ReactiveObject, IDisposable
    {
        #region 私有字段

        private readonly bool isDesign;
        private readonly ICompUIRunTimeCallBack callBack;

        private readonly PcCommunicationCompUIViewModel pcCommunicationViewModel;
        private readonly TrackWideningCompUIViewModel trackWideningViewModel;

        #endregion

        #region 子组件ViewModel

        public TrackBasicParamCompUIViewModel TrackBasicParamViewModel { get; }

        public object PcCommunicationViewModel => pcCommunicationViewModel;

        public object TrackWideningViewModel => trackWideningViewModel;

        public TransportMotorCompUIViewModel TransportMotorViewModel { get; }
        public SingleTrackStationCompUIViewModel SingleTrackStationViewModel { get; }

        #endregion

        #region 响应式属性

        private object _viewTag;
        public object ViewTag
        {
            get => _viewTag;
            set
            {
                this.RaiseAndSetIfChanged(ref _viewTag, value);
                PropagateViewTag(value);
            }
        }

        private bool _readOnly;
        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                this.RaiseAndSetIfChanged(ref _readOnly, value);
                PropagateReadOnly(value);
            }
        }

        public RegularSingleTrackCompUIViewModel RegularSingleTrack
        {
            get => this;
            set => this.RaisePropertyChanged(nameof(RegularSingleTrack));
        }

        #endregion

        #region 构造函数

        public RegularSingleTrackCompUIViewModel() : this(true, null)
        {
        }

        public RegularSingleTrackCompUIViewModel(bool isDesign, ICompUIRunTimeCallBack callBack)
        {
            this.isDesign = isDesign;
            this.callBack = callBack;

            TrackBasicParamViewModel = new TrackBasicParamCompUIViewModel(isDesign, callBack);
            pcCommunicationViewModel = new PcCommunicationCompUIViewModel(isDesign, callBack);
            trackWideningViewModel = new TrackWideningCompUIViewModel(isDesign, callBack);
            TransportMotorViewModel = new TransportMotorCompUIViewModel(isDesign, callBack);
            SingleTrackStationViewModel = new SingleTrackStationCompUIViewModel(isDesign, callBack);

            HookChildEvents();
        }

        #endregion

        #region 公共方法

        public void SetData(RegularSingleTrackCompUIModel model)
        {
            model ??= new RegularSingleTrackCompUIModel();

            // TrackBasicParam
            TrackBasicParamViewModel.RefluxNoBoardViewModel.IsChecked = model.TrackBasicParamCompUIModel?.RefluxNoBoard ?? false;
            TrackBasicParamViewModel.SlavePriorityViewModel.IsChecked = model.TrackBasicParamCompUIModel?.SlavePriority ?? false;

            // PcCommunication
            pcCommunicationViewModel.UpperCommunicationViewModel.IsChecked = model.PcCommunicationCompUIModel?.UpperCommunication ?? false;
            pcCommunicationViewModel.SlaveCommunicationViewModel.IsChecked = model.PcCommunicationCompUIModel?.SlaveCommunication ?? false;
            pcCommunicationViewModel.SupSMEMAViewModel.IsChecked = model.PcCommunicationCompUIModel?.SupSMEMA ?? false;

            // TrackWidening
            trackWideningViewModel.CollisionDetectionViewModel.IsChecked = model.TrackWideningCompUIModel?.CollisionDetection ?? false;

            // TransportMotor / SingleTrackStation
            TransportMotorViewModel.SetData(model.TransportMotorCompUIModel);
            SingleTrackStationViewModel.SetData(model.SingleTrackStationCompUIModel);

            PropagateViewTag(ViewTag);
            PropagateReadOnly(ReadOnly);
        }

        public RegularSingleTrackCompUIModel GetData()
        {
            var model = new RegularSingleTrackCompUIModel();

            model.TrackBasicParamCompUIModel = new TrackBasicParamCompUIModel
            {
                RefluxNoBoard = TrackBasicParamViewModel.RefluxNoBoardViewModel.IsChecked,
                SlavePriority = TrackBasicParamViewModel.SlavePriorityViewModel.IsChecked,
            };

            model.PcCommunicationCompUIModel = new PcCommunicationCompUIModel
            {
                UpperCommunication = pcCommunicationViewModel.UpperCommunicationViewModel.IsChecked,
                SlaveCommunication = pcCommunicationViewModel.SlaveCommunicationViewModel.IsChecked,
                SupSMEMA = pcCommunicationViewModel.SupSMEMAViewModel.IsChecked,
            };

            model.TrackWideningCompUIModel = new TrackWideningCompUIModel‌
            {
                CollisionDetection = trackWideningViewModel.CollisionDetectionViewModel.IsChecked,
            };

            model.TransportMotorCompUIModel = TransportMotorViewModel.GetData();
            model.SingleTrackStationCompUIModel = SingleTrackStationViewModel.GetData();

            return model;
        }

        public void Dispose()
        {
            UnhookChildEvents();
            TrackBasicParamViewModel?.Dispose();
            pcCommunicationViewModel?.Dispose();
            trackWideningViewModel?.Dispose();
            TransportMotorViewModel?.Dispose();
            SingleTrackStationViewModel?.Dispose();
        }

        #endregion

        #region 事件绑定

        private void HookChildEvents()
        {
            TrackBasicParamViewModel.PropertyChanged += ChildOnPropertyChanged;
            pcCommunicationViewModel.PropertyChanged += ChildOnPropertyChanged;
            trackWideningViewModel.PropertyChanged += ChildOnPropertyChanged;

            TransportMotorViewModel.PropertyChanged += ChildOnPropertyChanged;
            if (TransportMotorViewModel.Motors != null)
            {
                foreach (var motor in TransportMotorViewModel.Motors)
                {
                    motor.PropertyChanged += ChildOnPropertyChanged;
                }
            }

            SingleTrackStationViewModel.PropertyChanged += ChildOnPropertyChanged;
            if (SingleTrackStationViewModel.Stations != null)
            {
                SingleTrackStationViewModel.Stations.CollectionChanged += StationsOnCollectionChanged;
                foreach (var station in SingleTrackStationViewModel.Stations)
                {
                    HookStation(station);
                }
            }
        }

        private void UnhookChildEvents()
        {
            TrackBasicParamViewModel.PropertyChanged -= ChildOnPropertyChanged;
            pcCommunicationViewModel.PropertyChanged -= ChildOnPropertyChanged;
            trackWideningViewModel.PropertyChanged -= ChildOnPropertyChanged;

            TransportMotorViewModel.PropertyChanged -= ChildOnPropertyChanged;
            if (TransportMotorViewModel.Motors != null)
            {
                foreach (var motor in TransportMotorViewModel.Motors)
                {
                    motor.PropertyChanged -= ChildOnPropertyChanged;
                }
            }

            SingleTrackStationViewModel.PropertyChanged -= ChildOnPropertyChanged;
            if (SingleTrackStationViewModel.Stations != null)
            {
                SingleTrackStationViewModel.Stations.CollectionChanged -= StationsOnCollectionChanged;
                foreach (var station in SingleTrackStationViewModel.Stations)
                {
                    UnhookStation(station);
                }
            }
        }

        private void StationsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var station in e.NewItems)
                {
                    if (station is SingleTrackStationItemCompUIViewModel vm)
                    {
                        HookStation(vm);
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (var station in e.OldItems)
                {
                    if (station is SingleTrackStationItemCompUIViewModel vm)
                    {
                        UnhookStation(vm);
                    }
                }
            }

            this.RaisePropertyChanged(nameof(RegularSingleTrack));
        }

        private void HookStation(SingleTrackStationItemCompUIViewModel station)
        {
            station.PropertyChanged += ChildOnPropertyChanged;

            if (station.GearSpeedList != null)
            {
                station.GearSpeedList.CollectionChanged += StationGearSpeedListOnCollectionChanged;
                foreach (var item in station.GearSpeedList)
                {
                    item.PropertyChanged += ChildOnPropertyChanged;
                }
            }
        }

        private void UnhookStation(SingleTrackStationItemCompUIViewModel station)
        {
            station.PropertyChanged -= ChildOnPropertyChanged;

            if (station.GearSpeedList != null)
            {
                station.GearSpeedList.CollectionChanged -= StationGearSpeedListOnCollectionChanged;
                foreach (var item in station.GearSpeedList)
                {
                    item.PropertyChanged -= ChildOnPropertyChanged;
                }
            }
        }

        private void StationGearSpeedListOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is TrackItemModel m)
                    {
                        m.PropertyChanged += ChildOnPropertyChanged;
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is TrackItemModel m)
                    {
                        m.PropertyChanged -= ChildOnPropertyChanged;
                    }
                }
            }

            this.RaisePropertyChanged(nameof(RegularSingleTrack));
        }

        private void ChildOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(nameof(RegularSingleTrack));
        }

        #endregion

        #region 辅助方法

        private void PropagateReadOnly(bool readOnly)
        {
            TrackBasicParamViewModel.ReadOnly = readOnly;
            pcCommunicationViewModel.ReadOnly = readOnly;
            trackWideningViewModel.ReadOnly = readOnly;

            if (TransportMotorViewModel.Motors != null)
            {
                foreach (var motor in TransportMotorViewModel.Motors)
                {
                    motor.ReadOnly = readOnly;
                }
            }

            if (SingleTrackStationViewModel.Stations != null)
            {
                foreach (var station in SingleTrackStationViewModel.Stations)
                {
                    station.ReadOnly = readOnly;
                }
            }
        }

        private void PropagateViewTag(object viewTag)
        {
            TrackBasicParamViewModel.ViewTag = viewTag;
            pcCommunicationViewModel.ViewTag = viewTag;
            trackWideningViewModel.ViewTag = viewTag;
            TransportMotorViewModel.ViewTag = viewTag;

            if (TransportMotorViewModel.Motors != null)
            {
                foreach (var motor in TransportMotorViewModel.Motors)
                {
                    motor.ViewTag = viewTag;
                }
            }

            if (SingleTrackStationViewModel.Stations != null)
            {
                foreach (var station in SingleTrackStationViewModel.Stations)
                {
                    station.ViewTag = viewTag;
                }
            }
        }

        #endregion
    }
}
