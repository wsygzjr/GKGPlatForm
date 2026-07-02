using ReactiveUI;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.ControlCard.ViewModels
{
    public class AxisStatusItemViewModel : ReactiveObject
    {
        private string _axisKey = string.Empty;
        public string AxisKey
        {
            get => _axisKey;
            set => this.RaiseAndSetIfChanged(ref _axisKey, value);
        }

        private string _axisName = string.Empty;
        public string AxisName
        {
            get => _axisName;
            set => this.RaiseAndSetIfChanged(ref _axisName, value);
        }

        private double _position;
        public double Position
        {
            get => _position;
            set => this.RaiseAndSetIfChanged(ref _position, value);
        }

        private bool _isPosLimit;
        public bool IsPosLimit
        {
            get => _isPosLimit;
            set => this.RaiseAndSetIfChanged(ref _isPosLimit, value);
        }

        private bool _isNegLimit;
        public bool IsNegLimit
        {
            get => _isNegLimit;
            set => this.RaiseAndSetIfChanged(ref _isNegLimit, value);
        }

        private bool _isHome;
        public bool IsHome
        {
            get => _isHome;
            set => this.RaiseAndSetIfChanged(ref _isHome, value);
        }

        private bool _isAlarm;
        public bool IsAlarm
        {
            get => _isAlarm;
            set => this.RaiseAndSetIfChanged(ref _isAlarm, value);
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
        }

        public AxisStatusItemViewModel()
        {
        }

        public AxisStatusItemViewModel(string name)
        {
            AxisName = name;
        }

        public AxisStatusItemViewModel(string name, string axisKey)
        {
            AxisName = name;
            AxisKey = axisKey;
        }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(AxisName) ? base.ToString() ?? string.Empty : AxisName;
        }
    }
}
