using System.Collections.ObjectModel;
using ReactiveUI;

namespace Griffins.CompUI.GD.ComUI_GD.InitCfgPage.Models
{
    /// <summary>
    /// 单层轨道工位数据模型
    /// </summary>
    public class SingleTrackStationCompUIModel
    {
        /// <summary>
        /// 速度挡位列表
        /// </summary>
        public ObservableCollection<TrackItemModel> GearSpeedList { get; set; } = new();

        /// <summary>
        /// 是否支持影子工位
        /// </summary>
        public bool SupportShadowStation { get; set; } = new();

        /// <summary>
        /// 是否启用接近感应器
        /// </summary>
        public bool EnableProximitySensor { get; set; } = new();

        /// <summary>
        /// 是否启用出板卡料信号
        /// </summary>
        public bool EnableBoardJamSignal { get; set; } = new();
    }

    /// <summary>
    /// 速度挡位数据模型
    /// </summary>
    public class TrackItemModel : ReactiveObject
    {
        private bool _isSelected;
        /// <summary>
        /// 是否勾选
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }

        private int _index;
        /// <summary>
        /// 序号
        /// </summary>
        public int Index
        {
            get => _index;
            set => this.RaiseAndSetIfChanged(ref _index, value);
        }

        private int _gear;
        /// <summary>
        /// 挡位
        /// </summary>
        public int Gear
        {
            get => _gear;
            set => this.RaiseAndSetIfChanged(ref _gear, value);
        }

        private int _speed;
        /// <summary>
        /// 速度
        /// </summary>
        public int Speed
        {
            get => _speed;
            set => this.RaiseAndSetIfChanged(ref _speed, value);
        }
    }
}