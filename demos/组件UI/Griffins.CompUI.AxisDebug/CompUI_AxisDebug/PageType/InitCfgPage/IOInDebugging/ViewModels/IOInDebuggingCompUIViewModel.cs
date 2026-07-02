using Avalonia.Media;
using GKG.UI;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace Griffins.CompUI.AxisDebug.CompUI_AxisDebug.PageType.InitCfgPage.IOInDebugging.ViewModels
{
    public class IOInDebuggingCompUIViewModel : ReactiveObject
    {
        public ToggleSwitchViewModel HasTrackBViewModel { get; } = new ToggleSwitchViewModel();

        public ObservableCollection<IOItemViewModel> CommonSensors { get; } = new();

        public ObservableCollection<IOItemViewModel> TrackASensors { get; } = new();

        public ObservableCollection<IOItemViewModel> TrackBSensors { get; } = new();

        private bool _isTrackBVisible;
        public bool IsTrackBVisible
        {
            get => _isTrackBVisible;
            set => this.RaiseAndSetIfChanged(ref _isTrackBVisible, value);
        }

        public IOInDebuggingCompUIViewModel()
        {
            HasTrackBViewModel.ValueChanged += OnHasTrackBChanged;
            initData();
        }

        private void OnHasTrackBChanged(object sender, ValueChangedEventArgs e)
        {
            if (e.NewValue is bool isChecked)
            {
                IsTrackBVisible = isChecked;
            }
        }

        private void initData()
        {
            CommonSensors.Clear();
            TrackASensors.Clear();
            TrackBSensors.Clear();

            CommonSensors.Add(new IOItemViewModel("启停按钮", true));
            CommonSensors.Add(new IOItemViewModel("复位按钮", false));
            CommonSensors.Add(new IOItemViewModel("急停按钮", true));
            CommonSensors.Add(new IOItemViewModel("门开关", false));
            CommonSensors.Add(new IOItemViewModel("(前门)安全门气缸锁", false));
            CommonSensors.Add(new IOItemViewModel("(后门)安全门气缸锁", false));
            CommonSensors.Add(new IOItemViewModel("气压", true));
            CommonSensors.Add(new IOItemViewModel("测高", false));
            CommonSensors.Add(new IOItemViewModel("清洗纸", false));
            CommonSensors.Add(new IOItemViewModel("清洗圈数", false));
            CommonSensors.Add(new IOItemViewModel("清洗纸2", false));
            CommonSensors.Add(new IOItemViewModel("清洗圈数2", false));
            CommonSensors.Add(new IOItemViewModel("右胶盖检测", false));
            CommonSensors.Add(new IOItemViewModel("右胶水高液位检测", false));
            CommonSensors.Add(new IOItemViewModel("左胶盖检测", false));

            TrackASensors.Add(new IOItemViewModel("进板检测", true));
            TrackASensors.Add(new IOItemViewModel("出板检测", false));
            TrackASensors.Add(new IOItemViewModel("左到板检测", true));
            TrackASensors.Add(new IOItemViewModel("右到板检测", true));
            TrackASensors.Add(new IOItemViewModel("左待料信号", false));
            TrackASensors.Add(new IOItemViewModel("右待料信号", false));
            TrackASensors.Add(new IOItemViewModel("左挡板气缸上限位", false));
            TrackASensors.Add(new IOItemViewModel("左挡板气缸下限位", false));
            TrackASensors.Add(new IOItemViewModel("右挡板气缸上限位", false));
            TrackASensors.Add(new IOItemViewModel("右挡板气缸下限位", false));
            TrackASensors.Add(new IOItemViewModel("顶料气缸下限位", false));
            TrackASensors.Add(new IOItemViewModel("接近感应器", false));
            TrackASensors.Add(new IOItemViewModel("上位机右板信号", false));
            TrackASensors.Add(new IOItemViewModel("下位机左板信号", false));
            TrackASensors.Add(new IOItemViewModel("提前减速信号", false));

            TrackBSensors.Add(new IOItemViewModel("(B轨)进板检测", true));
            TrackBSensors.Add(new IOItemViewModel("(B轨)出板检测", false));
            TrackBSensors.Add(new IOItemViewModel("(B轨)左到板检测", true));
            TrackBSensors.Add(new IOItemViewModel("(B轨)右到板检测", true));
            TrackBSensors.Add(new IOItemViewModel("(B轨)左待料信号", false));
            TrackBSensors.Add(new IOItemViewModel("(B轨)右待料信号", false));
            TrackBSensors.Add(new IOItemViewModel("(B轨)左挡板气缸上限位", false));
            TrackBSensors.Add(new IOItemViewModel("(B轨)左挡板气缸下限位", false));
            TrackBSensors.Add(new IOItemViewModel("(B轨)右挡板气缸上限位", false));
            TrackBSensors.Add(new IOItemViewModel("(B轨)右挡板气缸下限位", false));
            TrackBSensors.Add(new IOItemViewModel("(B轨)顶料气缸下限位", false));
            TrackBSensors.Add(new IOItemViewModel("(B轨)接近感应器", false));
            TrackBSensors.Add(new IOItemViewModel("(B轨)上位机右板信号", false));
            TrackBSensors.Add(new IOItemViewModel("(B轨)下位机左板信号", false));
            TrackBSensors.Add(new IOItemViewModel("(B轨)提前减速信号", false));
        }

        public class IOItemViewModel : ReactiveObject
        {
            public string Name { get; set; }

            private bool _isOn;
            public bool IsOn
            {
                get => _isOn;
                set
                {
                    this.RaiseAndSetIfChanged(ref _isOn, value);
                    this.RaisePropertyChanged(nameof(StatusColor));
                }
            }

            public IBrush StatusColor => IsOn ? Brushes.LightGreen : Brushes.LightGray;

            public IOItemViewModel(string name, bool isOn)
            {
                Name = name;
                IsOn = isOn;
            }
        }
    }
}
