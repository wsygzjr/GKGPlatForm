using GF_Gereric;
using GKG;
using GKG.SubMM;
using GKG.UI;
using Griffins.Map.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Griffins.CompUI.PushRod.CompUI_PushRod.PageType.InitCfgPage.MotorPushRodInit.ViewModels
{
    internal class MotorPushRodInitCompUIViewModel : ReactiveObject
    {
        private readonly List<AxisInformation> _axisInformations = new();

        public TextInputViewModel PushRodTimeoutViewModel { get; }
        public TextInputViewModel MaterialJamDetectionTimeViewModel { get; }
        public ComboxViewModel LogicAxisViewModel { get; }

        public event EventHandler AfterModified;

        private MotorPushRodSubMachineModulesInitCfg _data = new();
        private object _viewTag;

        public object ViewTag
        {
            get => _viewTag;
            set => this.RaiseAndSetIfChanged(ref _viewTag, value);
        }

        private bool _readOnly;
        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                this.RaiseAndSetIfChanged(ref _readOnly, value);
                var enabled = !_readOnly;
                PushRodTimeoutViewModel.IsEnabled = enabled;
                MaterialJamDetectionTimeViewModel.IsEnabled = enabled;
                LogicAxisViewModel.IsEnabled = enabled;
            }
        }

        public MotorPushRodInitCompUIViewModel(ICompUIRunTimeCallBack callBack)
        {
            PushRodTimeoutViewModel = new TextInputViewModel();
            MaterialJamDetectionTimeViewModel = new TextInputViewModel();
            LogicAxisViewModel = CreateComboViewModel();

            PushRodTimeoutViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            MaterialJamDetectionTimeViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            LogicAxisViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);

            LoadAxisInfos(callBack);
            ApplyAxisOptions(Guid.Empty);
            ReadOnly = false;
        }

        public void SetData(MotorPushRodSubMachineModulesInitCfg data)
        {
            _data = CloneData(data);
            PushRodTimeoutViewModel.Text = _data.PushRodTimeout.ToString(CultureInfo.InvariantCulture);
            MaterialJamDetectionTimeViewModel.Text = _data.MaterialJamDetectionTime.ToString(CultureInfo.InvariantCulture);
            ApplyAxisOptions(_data.PusherPhysicalAxis);
        }

        public MotorPushRodSubMachineModulesInitCfg GetData()
        {
            _data ??= new MotorPushRodSubMachineModulesInitCfg();
            _data.PushRodTimeout = ParseDoubleOrDefault(PushRodTimeoutViewModel.Text, _data.PushRodTimeout);
            _data.MaterialJamDetectionTime = ParseDoubleOrDefault(
                MaterialJamDetectionTimeViewModel.Text,
                _data.MaterialJamDetectionTime);

            var selectedAxisGuid = GetSelectedAxisGuid(LogicAxisViewModel);
            if (selectedAxisGuid != Guid.Empty)
                _data.PusherPhysicalAxis = selectedAxisGuid;

            return CloneData(_data);
        }

        private static ComboxViewModel CreateComboViewModel()
        {
            return new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                ItemsSource = new ObservableCollection<ComBoxItem>(),
            };
        }

        private void LoadAxisInfos(ICompUIRunTimeCallBack callBack)
        {
            _axisInformations.Clear();
            try
            {
                var result = callBack?.ExecConfigSvrCtlCmd(PushRodSubMachineModulesConst.RtCmdGetAxisOptions, new GFBaseTypeParamValueList());
                var raw = result?["data"]?.ToStringVal();
                if (string.IsNullOrWhiteSpace(raw))
                    raw = result?["Result"]?.ToStringVal();

                var axisInfos = string.IsNullOrWhiteSpace(raw)
                    ? null
                    : JsonObjConvert.FromJSon<List<AxisInformation>>(raw);

                if (axisInfos != null)
                    _axisInformations.AddRange(axisInfos.Where(item => item != null && item.AxisGuid != Guid.Empty));
            }
            catch
            {
            }
        }

        private void ApplyAxisOptions(Guid selectedAxisGuid)
        {
            ApplyAxisOptions(
                LogicAxisViewModel.ItemsSource as ObservableCollection<ComBoxItem>,
                LogicAxisViewModel,
                _axisInformations,
                selectedAxisGuid);
        }

        private static void ApplyAxisOptions(
            ObservableCollection<ComBoxItem> items,
            ComboxViewModel viewModel,
            IEnumerable<AxisInformation> axisOptions,
            Guid selectedAxisGuid)
        {
            items?.Clear();
            if (items == null)
                return;

            foreach (var axisInfo in (axisOptions ?? Enumerable.Empty<AxisInformation>())
                .Where(item => item != null && item.AxisGuid != Guid.Empty)
                .GroupBy(item => item.AxisGuid)
                .Select(group => group.First()))
            {
                items.Add(new ComBoxItem
                {
                    Value = axisInfo,
                    DisplayName = string.IsNullOrWhiteSpace(axisInfo.AxisName) ? axisInfo.AxisGuid.ToString() : axisInfo.AxisName,
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
            SetSelectedAxis(viewModel, items, selectedAxisGuid);
        }

        private static void SetSelectedAxis(ComboxViewModel viewModel, IEnumerable<ComBoxItem> items, Guid axisGuid)
        {
            var target = items.FirstOrDefault(item => (item.Value as AxisInformation)?.AxisGuid == axisGuid);
            if (target != null)
                viewModel.SelectedItem = target;
        }

        private static Guid GetSelectedAxisGuid(ComboxViewModel viewModel)
            => ((viewModel.SelectedItem as ComBoxItem)?.Value as AxisInformation)?.AxisGuid ?? Guid.Empty;

        private static MotorPushRodSubMachineModulesInitCfg CloneData(MotorPushRodSubMachineModulesInitCfg data)
        {
            if (data == null)
                return new MotorPushRodSubMachineModulesInitCfg();

            return JsonObjConvert.FromJSonBytes<MotorPushRodSubMachineModulesInitCfg>(JsonObjConvert.ToJSonBytes(data))
                ?? new MotorPushRodSubMachineModulesInitCfg();
        }

        private static double ParseDoubleOrDefault(string text, double defaultValue)
        {
            return double.TryParse(
                text,
                NumberStyles.Float | NumberStyles.AllowThousands,
                CultureInfo.InvariantCulture,
                out var value)
                ? value
                : defaultValue;
        }
    }
}
