using GF_Gereric;
using GKG;
using GKG.SubMM;
using GKG.UI;
using GKG.Vision;
using Griffins.Map.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Griffins.CompUI.Vision.CompUI.PageType.InitCfgPage.VisionInitCfg.ViewModels
{
    public class VisionInitCfgViewModel
    {
        private const string WaitingAxisListText = "等待后端轴列表";
        private const string WaitingIOListText = "等待后端IO列表";

        public ComboxViewModel VisionComboxViewModelX { get; }
        public ComboxViewModel VisionComboxViewModelY { get; }
        public ComboxViewModel VisionComboxViewModelZ { get; }
        public ComboxViewModel VisionComboxViewModelChange { get; }
        public ComboxViewModel VisionComboxViewModelTrig { get; }

        public event EventHandler AfterModified;

        public List<ComBoxItem> VisionAxisItems { get; } = new List<ComBoxItem>();
        public List<ComBoxItem> VisionIOItems { get; } = new List<ComBoxItem>();

        private VisionSubMachineModulesInitCfg initCfg = new();
        private readonly ICompUIRunTimeCallBack callBack;
        private readonly List<AxisInformation> axisInformation = new();
        private readonly List<IOStateInformation> ioStateInformation = new();

        public VisionInitCfgViewModel(ICompUIRunTimeCallBack callBack)
        {
            this.callBack = callBack;

            VisionComboxViewModelX = CreateComboViewModel();
            VisionComboxViewModelY = CreateComboViewModel();
            VisionComboxViewModelZ = CreateComboViewModel();
            VisionComboxViewModelChange = CreateComboViewModel();
            VisionComboxViewModelTrig = CreateComboViewModel();

            VisionComboxViewModelX.ValueChanged += OnXValueChanged;
            VisionComboxViewModelY.ValueChanged += OnYValueChanged;
            VisionComboxViewModelZ.ValueChanged += OnZValueChanged;
            VisionComboxViewModelChange.ValueChanged += OnChangeValueChanged;
            VisionComboxViewModelTrig.ValueChanged += OnTrigValueChanged;

            LoadAxisInfos();
            LoadIOStateInfos();
            ApplyAxisOptions();
            ApplyIOOptions();
        }

        internal VisionSubMachineModulesInitCfg GetData()
        {
            return initCfg;
        }

        internal void SetData(VisionSubMachineModulesInitCfg model)
        {
            if (model != null)
            {
                initCfg = model;
            }

            ApplyAxisOptions();
            ApplyIOOptions();
        }

        private static ComboxViewModel CreateComboViewModel()
        {
            return new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
            };
        }

        private void LoadAxisInfos()
        {
            axisInformation.Clear();
            if (callBack == null)
            {
                return;
            }

            try
            {
                var result = callBack.ExecNormalCtlCmd(
                    VisionSubMachineModulesConst.RunTimeCtlGetAxisInfos,
                    new GFBaseTypeParamValueList());
                var raw = result?["Result"]?.ToStringVal();
                if (string.IsNullOrWhiteSpace(raw))
                {
                    raw = result?["data"]?.ToStringVal();
                }

                if (string.IsNullOrWhiteSpace(raw) || raw == "0")
                {
                    return;
                }

                var axisInfos = JsonObjConvert.FromJSon<List<AxisInformation>>(raw);
                if (axisInfos != null)
                {
                    axisInformation.AddRange(axisInfos.Where(item => item != null && item.AxisGuid != Guid.Empty));
                }
            }
            catch
            {
            }
        }

        private void LoadIOStateInfos()
        {
            ioStateInformation.Clear();
            if (callBack == null)
            {
                return;
            }

            try
            {
                var result = callBack.ExecNormalCtlCmd(
                    VisionSubMachineModulesConst.RunTimeCtlGetIOStateInfos,
                    new GFBaseTypeParamValueList());
                var raw = result?["Result"]?.ToStringVal();
                if (string.IsNullOrWhiteSpace(raw))
                {
                    raw = result?["data"]?.ToStringVal();
                }

                if (string.IsNullOrWhiteSpace(raw) || raw == "0")
                {
                    return;
                }

                var ioInfos = JsonObjConvert.FromJSon<List<IOStateInformation>>(raw);
                if (ioInfos != null)
                {
                    ioStateInformation.AddRange(ioInfos.Where(item => item != null && item.IOGuid != Guid.Empty));
                }
            }
            catch
            {
            }
        }

        private void ApplyAxisOptions()
        {
            VisionAxisItems.Clear();

            foreach (var axisInfo in axisInformation
                .Where(item => item != null && item.AxisGuid != Guid.Empty)
                .GroupBy(item => item.AxisGuid)
                .Select(group => group.First()))
            {
                VisionAxisItems.Add(new ComBoxItem
                {
                    Value = axisInfo,
                    DisplayName = string.IsNullOrWhiteSpace(axisInfo.AxisName)
                        ? axisInfo.AxisGuid.ToString()
                        : axisInfo.AxisName,
                });
            }

            AppendSavedAxisIfMissing(VisionAxisItems, initCfg.BindingAxisX);
            AppendSavedAxisIfMissing(VisionAxisItems, initCfg.BindingAxisY);
            AppendSavedAxisIfMissing(VisionAxisItems, initCfg.BindingAxisZ);

            if (VisionAxisItems.Count == 0)
            {
                VisionAxisItems.Add(new ComBoxItem
                {
                    Value = null,
                    DisplayName = WaitingAxisListText,
                });
            }

            VisionComboxViewModelX.ItemsSource = VisionAxisItems;
            VisionComboxViewModelY.ItemsSource = VisionAxisItems;
            VisionComboxViewModelZ.ItemsSource = VisionAxisItems;

            SetSelectedAxis(VisionComboxViewModelX, initCfg.BindingAxisX);
            SetSelectedAxis(VisionComboxViewModelY, initCfg.BindingAxisY);
            SetSelectedAxis(VisionComboxViewModelZ, initCfg.BindingAxisZ);
        }

        private void ApplyIOOptions()
        {
            VisionIOItems.Clear();

            foreach (var ioState in ioStateInformation
                .Where(item => item != null && item.IOGuid != Guid.Empty)
                .GroupBy(item => item.IOGuid)
                .Select(group => group.First()))
            {
                VisionIOItems.Add(new ComBoxItem
                {
                    Value = ioState,
                    DisplayName = BuildIOStateDisplayName(ioState),
                });
            }

            AppendSavedIOIfMissing(VisionIOItems, initCfg.ChangeCCDOrJetIOGuid);
            AppendSavedIOIfMissing(VisionIOItems, initCfg.TriggerCCDIOGuid);

            if (VisionIOItems.Count == 0)
            {
                VisionIOItems.Add(new ComBoxItem
                {
                    Value = null,
                    DisplayName = WaitingIOListText,
                });
            }

            VisionComboxViewModelChange.ItemsSource = VisionIOItems;
            VisionComboxViewModelTrig.ItemsSource = VisionIOItems;

            SetSelectedIO(VisionComboxViewModelChange, initCfg.ChangeCCDOrJetIOGuid);
            SetSelectedIO(VisionComboxViewModelTrig, initCfg.TriggerCCDIOGuid);
        }

        private static void AppendSavedAxisIfMissing(List<ComBoxItem> items, Guid axisGuid)
        {
            if (axisGuid == Guid.Empty || items.Any(item => (item.Value as AxisInformation)?.AxisGuid == axisGuid))
            {
                return;
            }

            items.Add(new ComBoxItem
            {
                Value = new AxisInformation
                {
                    AxisGuid = axisGuid,
                    AxisName = axisGuid.ToString(),
                },
                DisplayName = axisGuid.ToString(),
            });
        }

        private static void AppendSavedIOIfMissing(List<ComBoxItem> items, Guid ioGuid)
        {
            if (ioGuid == Guid.Empty || items.Any(item => (item.Value as IOStateInformation)?.IOGuid == ioGuid))
            {
                return;
            }

            items.Add(new ComBoxItem
            {
                Value = new IOStateInformation
                {
                    IOGuid = ioGuid,
                    IOName = ioGuid.ToString(),
                },
                DisplayName = ioGuid.ToString(),
            });
        }

        private static void SetSelectedAxis(ComboxViewModel viewModel, Guid axisGuid)
        {
            var items = viewModel.ItemsSource as List<ComBoxItem> ?? new List<ComBoxItem>();
            if (items.Count == 0)
            {
                return;
            }

            var target = items.FirstOrDefault(item => (item.Value as AxisInformation)?.AxisGuid == axisGuid)
                ?? items[0];
            viewModel.SelectedItem = target;
        }

        private static void SetSelectedIO(ComboxViewModel viewModel, Guid ioGuid)
        {
            var items = viewModel.ItemsSource as List<ComBoxItem> ?? new List<ComBoxItem>();
            if (items.Count == 0)
            {
                return;
            }

            var target = items.FirstOrDefault(item => (item.Value as IOStateInformation)?.IOGuid == ioGuid)
                ?? items[0];
            viewModel.SelectedItem = target;
        }

        private static string BuildIOStateDisplayName(IOStateInformation ioState)
        {
            if (!string.IsNullOrWhiteSpace(ioState?.IOName))
            {
                return ioState.IOName;
            }

            if (!string.IsNullOrWhiteSpace(ioState?.ChannelId))
            {
                return ioState.ChannelId;
            }

            return ioState?.IOGuid.ToString() ?? string.Empty;
        }

        private void OnXValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (e.NewValue is not ComBoxItem item || item.Value is not AxisInformation axisInfo)
            {
                return;
            }

            initCfg.BindingAxisX = axisInfo.AxisGuid;
            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        private void OnYValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (e.NewValue is not ComBoxItem item || item.Value is not AxisInformation axisInfo)
            {
                return;
            }

            initCfg.BindingAxisY = axisInfo.AxisGuid;
            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        private void OnZValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (e.NewValue is not ComBoxItem item || item.Value is not AxisInformation axisInfo)
            {
                return;
            }

            initCfg.BindingAxisZ = axisInfo.AxisGuid;
            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        private void OnChangeValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (e.NewValue is not ComBoxItem item || item.Value is not IOStateInformation ioState)
            {
                return;
            }

            initCfg.ChangeCCDOrJetIOGuid = ioState.IOGuid;
            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        private void OnTrigValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (e.NewValue is not ComBoxItem item || item.Value is not IOStateInformation ioState)
            {
                return;
            }

            initCfg.TriggerCCDIOGuid = ioState.IOGuid;
            AfterModified?.Invoke(this, EventArgs.Empty);
        }
    }
}
