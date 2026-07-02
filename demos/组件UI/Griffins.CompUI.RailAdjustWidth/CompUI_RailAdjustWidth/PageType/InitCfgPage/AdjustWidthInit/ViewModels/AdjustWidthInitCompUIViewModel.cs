using GF_Gereric;
using GKG;
using GKG.SubMM;
using GKG.UI;
using Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth;
using Griffins.Map.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.InitCfgPage.AdjustWidthInit.ViewModels
{
    internal class AdjustWidthInitCompUIViewModel : ReactiveObject, IDisposable
    {
        private readonly bool isDesign;
        private readonly ICompUIRunTimeCallBack callBack;
        private readonly List<AxisInformation> axisInformations = new();
        private RailAdjustWidthSubMachineModulesInitCfg loadedData = new();
        private bool readOnly;
        private bool isApplyingData;
        private RailAdjustWidthSubMachineModulesFactoryCfg factoryCfg = new RailAdjustWidthSubMachineModulesFactoryCfg();
        public ToggleSwitchViewModel IsEnableCrashDetectionViewModel { get; } = new ToggleSwitchViewModel();
        public ToggleSwitchViewModel IsEnableMaxDistanceAdjustWidthViewModel { get; } = new ToggleSwitchViewModel();
        public TextInputViewModel AdjustWidthSpeedViewModel { get; } = new TextInputViewModel();
        public ComboxViewModel FrontRailAxisBindingViewModel { get; } = CreateComboViewModel();
        public ComboxViewModel BackRailAxisBindingViewModel { get; } = CreateComboViewModel();

        public event EventHandler AfterModified;

        public bool ReadOnly
        {
            get => readOnly;
            set
            {
                this.RaiseAndSetIfChanged(ref readOnly, value);
                UpdateFieldEnabledState();
            }
        }
        public bool ShowAxisBindingSection { get => factoryCfg.FrontRailIsMovable || factoryCfg.BackRailIsMovable || isDesign; }
        public bool ShowFrontRailAxisBinding { get => factoryCfg.FrontRailIsMovable || isDesign; }
        public bool ShowBackRailAxisBinding { get => factoryCfg.BackRailIsMovable || isDesign; }
        public AdjustWidthInitCompUIViewModel()
        {
            isDesign = true;
            SubscribeValueChanges();
            ApplyAxisOptions(FrontRailAxisBindingViewModel, Guid.Empty);
            ApplyAxisOptions(BackRailAxisBindingViewModel, Guid.Empty);
        }

        public AdjustWidthInitCompUIViewModel(bool isDesign, ICompUIRunTimeCallBack callBack)
        {
            this.isDesign = isDesign;
            this.callBack = callBack;
            GFBaseTypeParamValueList result = callBack.ExecConfigSvrCtlCmd(RailAdjustWidthSubMachineModulesConst.RtCmdGetFactoryParams, new GFBaseTypeParamValueList());
            factoryCfg = JsonObjConvert.FromJSon<RailAdjustWidthSubMachineModulesFactoryCfg>(result["Result"].ToStringVal());
            LoadAxisInfos();
            ApplyAxisOptions(FrontRailAxisBindingViewModel, loadedData.FrontRailAxisBindingObjID);
            ApplyAxisOptions(BackRailAxisBindingViewModel, loadedData.BackRailAxisBindingObjID);
            UpdateFieldEnabledState();
            ReadOnly = false;
        }

        public void SetData(RailAdjustWidthSubMachineModulesInitCfg data)
        {
            ApplyWithoutModified(() =>
            {
                loadedData = CloneData(data);
                IsEnableCrashDetectionViewModel.IsChecked = loadedData.IsEnableCrashDetection;
                AdjustWidthSpeedViewModel.Text = loadedData.AdjustWidthSpeed.ToString(CultureInfo.InvariantCulture);
                IsEnableMaxDistanceAdjustWidthViewModel.IsChecked = loadedData.IsEnableMaxDistanceAdjustWidth;
                ApplyAxisOptions(FrontRailAxisBindingViewModel, loadedData.FrontRailAxisBindingObjID);
                ApplyAxisOptions(BackRailAxisBindingViewModel, loadedData.BackRailAxisBindingObjID);
            });
            UpdateFieldEnabledState();
        }

        public RailAdjustWidthSubMachineModulesInitCfg GetData()
        {
            var result = CloneData(loadedData) ?? new RailAdjustWidthSubMachineModulesInitCfg();
            result.IsEnableCrashDetection = IsEnableCrashDetectionViewModel.IsChecked;
            result.IsEnableMaxDistanceAdjustWidth = IsEnableMaxDistanceAdjustWidthViewModel.IsChecked;

            if (int.TryParse(AdjustWidthSpeedViewModel.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var speed))
            {
                result.AdjustWidthSpeed = speed;
            }

            result.FrontRailAxisBindingObjID = GetSelectedAxisGuid(FrontRailAxisBindingViewModel);
            result.BackRailAxisBindingObjID = GetSelectedAxisGuid(BackRailAxisBindingViewModel);

            loadedData = CloneData(result);
            return result;
        }

        public void Dispose()
        {
        }

        private void SubscribeValueChanges()
        {
            IsEnableCrashDetectionViewModel.ValueChanged += (_, __) => NotifyDataModified();
            IsEnableMaxDistanceAdjustWidthViewModel.ValueChanged += (_, __) => NotifyDataModified();
            AdjustWidthSpeedViewModel.ValueChanged += (_, __) => NotifyDataModified();
            FrontRailAxisBindingViewModel.ValueChanged += (_, __) => NotifyDataModified();
            BackRailAxisBindingViewModel.ValueChanged += (_, __) => NotifyDataModified();
        }

        private void UpdateFieldEnabledState()
        {
            var editable = !ReadOnly;
            IsEnableCrashDetectionViewModel.IsEnabled = editable;
            IsEnableMaxDistanceAdjustWidthViewModel.IsEnabled = editable;
            AdjustWidthSpeedViewModel.IsEnabled = editable;
            FrontRailAxisBindingViewModel.IsEnabled = editable && factoryCfg.FrontRailIsMovable;
            BackRailAxisBindingViewModel.IsEnabled = editable && factoryCfg.BackRailIsMovable;
        }

        private void NotifyDataModified()
        {
            if (isApplyingData)
            {
                return;
            }

            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        private void ApplyWithoutModified(Action action)
        {
            isApplyingData = true;
            try
            {
                action();
            }
            finally
            {
                isApplyingData = false;
            }
        }

        private void LoadAxisInfos()
        {
            axisInformations.Clear();
            if (callBack == null)
            {
                return;
            }

            try
            {
                var result = callBack.ExecConfigSvrCtlCmd(
                    RailAdjustWidthSubMachineModulesConst.RtCmdGetAxisOptions,
                    new GFBaseTypeParamValueList());
                var raw = result?["Result"]?.ToStringVal();
                if (string.IsNullOrWhiteSpace(raw))
                {
                    raw = result?["data"]?.ToStringVal();
                }

                var axisInfos = string.IsNullOrWhiteSpace(raw)
                    ? null
                    : JsonObjConvert.FromJSon<List<AxisInformation>>(raw);

                if (axisInfos != null)
                {
                    axisInformations.AddRange(axisInfos.Where(item => item != null && item.AxisGuid != Guid.Empty));
                }
            }
            catch
            {
            }
        }

        private void ApplyAxisOptions(ComboxViewModel viewModel, Guid selectedAxisGuid)
        {
            var items = viewModel.ItemsSource as ObservableCollection<ComBoxItem>;
            items?.Clear();
            if (items == null)
            {
                return;
            }

            foreach (var axisInfo in axisInformations
                .Where(item => item != null && item.AxisGuid != Guid.Empty)
                .GroupBy(item => item.AxisGuid)
                .Select(group => group.First()))
            {
                items.Add(new ComBoxItem
                {
                    Value = axisInfo,
                    DisplayName = string.IsNullOrWhiteSpace(axisInfo.AxisName)
                        ? axisInfo.AxisGuid.ToString()
                        : axisInfo.AxisName,
                });
            }

            if (items.Count == 0)
            {
                items.Add(new ComBoxItem
                {
                    Value = null,
                    DisplayName = "等待后端轴列表",
                });
            }

            if (selectedAxisGuid != Guid.Empty &&
                items.All(item => (item.Value as AxisInformation)?.AxisGuid != selectedAxisGuid))
            {
                items.Add(new ComBoxItem
                {
                    Value = new AxisInformation
                    {
                        AxisGuid = selectedAxisGuid,
                        AxisName = selectedAxisGuid.ToString(),
                    },
                    DisplayName = selectedAxisGuid.ToString(),
                });
            }

            viewModel.SelectedItem = items[0];
            var target = items.FirstOrDefault(item => (item.Value as AxisInformation)?.AxisGuid == selectedAxisGuid);
            if (target != null)
            {
                viewModel.SelectedItem = target;
            }
        }

        private static ComboxViewModel CreateComboViewModel()
        {
            return new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                ItemsSource = new ObservableCollection<ComBoxItem>(),
            };
        }

        private static Guid GetSelectedAxisGuid(ComboxViewModel viewModel)
            => ((viewModel.SelectedItem as ComBoxItem)?.Value as AxisInformation)?.AxisGuid ?? Guid.Empty;

        private static RailAdjustWidthSubMachineModulesInitCfg CloneData(RailAdjustWidthSubMachineModulesInitCfg data)
        {
            if (data == null)
            {
                return new RailAdjustWidthSubMachineModulesInitCfg();
            }

            var clone = new RailAdjustWidthSubMachineModulesInitCfg();
            clone.CopyFrom(data);
            return clone;
        }
    }
}
