using GKG.SubMM;
using GKG.UI;
using ReactiveUI;

namespace Griffins.CompUI.RWS.CompUI_RWS.PageType.FactoryCfgPage.WorkStationFactoryConfig.ViewModels
{
    internal class WorkStationFactoryConfigCompUIViewModel : ReactiveObject
    {
        private bool readOnly;
        private bool hasProximitySensor;
        private bool hasLeftSensor;
        private bool hasRightSensor;
        private bool hasLeftBlock;
        private bool hasRightBlock;
        private bool isSupportLeftIn;
        private bool isSupportLeftOut;
        private bool isSupportRightIn;
        private bool isSupportRightOut;

        public ToggleSwitchViewModel HasProximitySensorViewModel { get; } = new ToggleSwitchViewModel();
        public ToggleSwitchViewModel HasLeftSensorViewModel { get; } = new ToggleSwitchViewModel();
        public ToggleSwitchViewModel HasRightSensorViewModel { get; } = new ToggleSwitchViewModel();
        public ToggleSwitchViewModel HasLeftBlockViewModel { get; } = new ToggleSwitchViewModel();
        public ToggleSwitchViewModel HasRightBlockViewModel { get; } = new ToggleSwitchViewModel();
        public ToggleSwitchViewModel IsSupportLeftInViewModel { get; } = new ToggleSwitchViewModel();
        public ToggleSwitchViewModel IsSupportLeftOutViewModel { get; } = new ToggleSwitchViewModel();
        public ToggleSwitchViewModel IsSupportRightInViewModel { get; } = new ToggleSwitchViewModel();
        public ToggleSwitchViewModel IsSupportRightOutViewModel { get; } = new ToggleSwitchViewModel();

        public bool ReadOnly
        {
            get => readOnly;
            set
            {
                this.RaiseAndSetIfChanged(ref readOnly, value);
                var isEnabled = !readOnly;
                HasProximitySensorViewModel.IsEnabled = isEnabled;
                HasLeftSensorViewModel.IsEnabled = isEnabled;
                HasRightSensorViewModel.IsEnabled = isEnabled;
                HasLeftBlockViewModel.IsEnabled = isEnabled;
                HasRightBlockViewModel.IsEnabled = isEnabled;
                IsSupportLeftInViewModel.IsEnabled = isEnabled;
                IsSupportLeftOutViewModel.IsEnabled = isEnabled;
                IsSupportRightInViewModel.IsEnabled = isEnabled;
                IsSupportRightOutViewModel.IsEnabled = isEnabled;
            }
        }

        public bool HasProximitySensor
        {
            get => hasProximitySensor;
            set
            {
                this.RaiseAndSetIfChanged(ref hasProximitySensor, value);
                HasProximitySensorViewModel.IsChecked = value;
            }
        }

        public bool HasLeftSensor
        {
            get => hasLeftSensor;
            set
            {
                this.RaiseAndSetIfChanged(ref hasLeftSensor, value);
                HasLeftSensorViewModel.IsChecked = value;
            }
        }

        public bool HasRightSensor
        {
            get => hasRightSensor;
            set
            {
                this.RaiseAndSetIfChanged(ref hasRightSensor, value);
                HasRightSensorViewModel.IsChecked = value;
            }
        }

        public bool HasLeftBlock
        {
            get => hasLeftBlock;
            set
            {
                this.RaiseAndSetIfChanged(ref hasLeftBlock, value);
                HasLeftBlockViewModel.IsChecked = value;
            }
        }

        public bool HasRightBlock
        {
            get => hasRightBlock;
            set
            {
                this.RaiseAndSetIfChanged(ref hasRightBlock, value);
                HasRightBlockViewModel.IsChecked = value;
            }
        }

        public bool IsSupportLeftIn
        {
            get => isSupportLeftIn;
            set
            {
                this.RaiseAndSetIfChanged(ref isSupportLeftIn, value);
                IsSupportLeftInViewModel.IsChecked = value;
            }
        }

        public bool IsSupportLeftOut
        {
            get => isSupportLeftOut;
            set
            {
                this.RaiseAndSetIfChanged(ref isSupportLeftOut, value);
                IsSupportLeftOutViewModel.IsChecked = value;
            }
        }

        public bool IsSupportRightIn
        {
            get => isSupportRightIn;
            set
            {
                this.RaiseAndSetIfChanged(ref isSupportRightIn, value);
                IsSupportRightInViewModel.IsChecked = value;
            }
        }

        public bool IsSupportRightOut
        {
            get => isSupportRightOut;
            set
            {
                this.RaiseAndSetIfChanged(ref isSupportRightOut, value);
                IsSupportRightOutViewModel.IsChecked = value;
            }
        }

        public WorkStationFactoryConfigCompUIViewModel()
        {
            HasProximitySensorViewModel.ValueChanged += (_, __) => HasProximitySensor = HasProximitySensorViewModel.IsChecked;
            HasLeftSensorViewModel.ValueChanged += (_, __) => HasLeftSensor = HasLeftSensorViewModel.IsChecked;
            HasRightSensorViewModel.ValueChanged += (_, __) => HasRightSensor = HasRightSensorViewModel.IsChecked;
            HasLeftBlockViewModel.ValueChanged += (_, __) => HasLeftBlock = HasLeftBlockViewModel.IsChecked;
            HasRightBlockViewModel.ValueChanged += (_, __) => HasRightBlock = HasRightBlockViewModel.IsChecked;
            IsSupportLeftInViewModel.ValueChanged += (_, __) => IsSupportLeftIn = IsSupportLeftInViewModel.IsChecked;
            IsSupportLeftOutViewModel.ValueChanged += (_, __) => IsSupportLeftOut = IsSupportLeftOutViewModel.IsChecked;
            IsSupportRightInViewModel.ValueChanged += (_, __) => IsSupportRightIn = IsSupportRightInViewModel.IsChecked;
            IsSupportRightOutViewModel.ValueChanged += (_, __) => IsSupportRightOut = IsSupportRightOutViewModel.IsChecked;
        }

        public void SetData(RailWorkStationSubMachineModulesFactoryCfg data)
        {
            var eleConfig = data?.WorkStationEleConfigParams ?? new WorkStationEleConfigParams();
            var capability = data?.WorkStationCapability ?? new WorkStationCapability();
            HasProximitySensor = eleConfig.HasProximitySensor;
            HasLeftSensor = eleConfig.HasLeftSensor;
            HasRightSensor = eleConfig.HasRightSensor;
            HasLeftBlock = eleConfig.HasLeftBlock;
            HasRightBlock = eleConfig.HasRightBlock;
            IsSupportLeftIn = capability.IsSupportLeftIn;
            IsSupportLeftOut = capability.IsSupportLeftOut;
            IsSupportRightIn = capability.IsSupportRightIn;
            IsSupportRightOut = capability.IsSupportRightOut;
        }

        public WorkStationEleConfigParams GetEleConfigData()
        {
            return new WorkStationEleConfigParams
            {
                HasProximitySensor = HasProximitySensor,
                HasLeftSensor = HasLeftSensor,
                HasRightSensor = HasRightSensor,
                HasLeftBlock = HasLeftBlock,
                HasRightBlock = HasRightBlock,
            };
        }

        public WorkStationCapability GetCapabilityData()
        {
            return new WorkStationCapability
            {
                IsSupportLeftIn = IsSupportLeftIn,
                IsSupportLeftOut = IsSupportLeftOut,
                IsSupportRightIn = IsSupportRightIn,
                IsSupportRightOut = IsSupportRightOut,
            };
        }
    }
}
