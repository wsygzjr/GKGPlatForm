using GKG.SubMM;
using GKG.UI;
using ReactiveUI;
using System;
using System.Globalization;

namespace Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.FactoryCfgPage.AdjustWidthFactory.ViewModels
{
    internal class AdjustWidthFactoryCompUIViewModel : ReactiveObject
    {
        private bool readOnly;
        private bool frontRailIsMovable;
        private bool backRailIsMovable;
        private string maxWidthText = string.Empty;
        private string minWidthText = string.Empty;

        public ToggleSwitchViewModel FrontRailIsMovableViewModel { get; } = new ToggleSwitchViewModel();
        public ToggleSwitchViewModel BackRailIsMovableViewModel { get; } = new ToggleSwitchViewModel();
        public TextInputViewModel MaxWidthViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel MinWidthViewModel { get; } = new TextInputViewModel();

        public bool ReadOnly
        {
            get => readOnly;
            set
            {
                this.RaiseAndSetIfChanged(ref readOnly, value);
                var isEnabled = !readOnly;
                FrontRailIsMovableViewModel.IsEnabled = isEnabled;
                BackRailIsMovableViewModel.IsEnabled = isEnabled;
                MaxWidthViewModel.IsEnabled = isEnabled;
                MinWidthViewModel.IsEnabled = isEnabled;
            }
        }

        public bool FrontRailIsMovable
        {
            get => frontRailIsMovable;
            set
            {
                this.RaiseAndSetIfChanged(ref frontRailIsMovable, value);
                FrontRailIsMovableViewModel.IsChecked = value;
            }
        }

        public bool BackRailIsMovable
        {
            get => backRailIsMovable;
            set
            {
                this.RaiseAndSetIfChanged(ref backRailIsMovable, value);
                BackRailIsMovableViewModel.IsChecked = value;
            }
        }

        public string MaxWidthText
        {
            get => maxWidthText;
            set
            {
                this.RaiseAndSetIfChanged(ref maxWidthText, value);
                MaxWidthViewModel.Text = value;
            }
        }

        public string MinWidthText
        {
            get => minWidthText;
            set
            {
                this.RaiseAndSetIfChanged(ref minWidthText, value);
                MinWidthViewModel.Text = value;
            }
        }

        public AdjustWidthFactoryCompUIViewModel()
        {
            FrontRailIsMovableViewModel.ValueChanged += (_, __) => FrontRailIsMovable = FrontRailIsMovableViewModel.IsChecked;
            BackRailIsMovableViewModel.ValueChanged += (_, __) => BackRailIsMovable = BackRailIsMovableViewModel.IsChecked;
            MaxWidthViewModel.ValueChanged += (_, __) => MaxWidthText = MaxWidthViewModel.Text;
            MinWidthViewModel.ValueChanged += (_, __) => MinWidthText = MinWidthViewModel.Text;
        }

        public void SetData(RailAdjustWidthSubMachineModulesFactoryCfg data)
        {
            var cfg = data ?? new RailAdjustWidthSubMachineModulesFactoryCfg();
            FrontRailIsMovable = cfg.FrontRailIsMovable;
            BackRailIsMovable = cfg.BackRailIsMovable;
            MaxWidthText = cfg.MaxWidth.ToString(CultureInfo.InvariantCulture);
            MinWidthText = cfg.MinWidth.ToString(CultureInfo.InvariantCulture);
        }

        public RailAdjustWidthSubMachineModulesFactoryCfg GetData()
        {
            var cfg = new RailAdjustWidthSubMachineModulesFactoryCfg
            {
                FrontRailIsMovable = FrontRailIsMovable,
                BackRailIsMovable = BackRailIsMovable,
            };

            if (double.TryParse(MaxWidthText, NumberStyles.Float, CultureInfo.InvariantCulture, out var maxWidth))
            {
                cfg.MaxWidth = maxWidth;
            }

            if (double.TryParse(MinWidthText, NumberStyles.Float, CultureInfo.InvariantCulture, out var minWidth))
            {
                cfg.MinWidth = minWidth;
            }

            return cfg;
        }
    }
}
