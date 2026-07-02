using GF_Gereric;
using GKG;
using GKG.SubMM;
using GKG.UI;
using Griffins.CompUI.RailMotor.CompUI_RailMotor.Interop;
using Griffins.Map.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Griffins.CompUI.RailMotor.CompUI_RailMotor.PageType.InitCfgPage.RailMotorInit.ViewModels
{
    /// <summary>
    /// 运输电机初始化配置 ViewModel：维护运动模式、轴绑定下拉框与 InitCfg 数据同步。
    /// </summary>
    internal class RailMotorInitCompUIViewModel : ReactiveObject
    {
        /// <summary>电机运动模式（独占/共享）的下拉选项定义。</summary>
        private static readonly (ERailMotionMode Mode, string DisplayName)[] MotionModeOptions =
        {
            (ERailMotionMode.ExclusiveMode, "独占"),
            (ERailMotionMode.ShareMode, "共享"),
        };

        /// <summary>从后端加载的可选物理轴列表缓存。</summary>
        private readonly List<AxisInformation> _axisInformations = new();

        /// <summary>电机运动模式（RailMotionMode）下拉框绑定模型。</summary>
        public ComboxViewModel RailMotionModeViewModel { get; }

        /// <summary>运输电机绑定轴（AxisBindingObjID）下拉框绑定模型。</summary>
        public ComboxViewModel AxisBindingViewModel { get; }

        /// <summary>界面数据被用户修改时触发，用于通知页面上报脏数据。</summary>
        public event EventHandler AfterModified;

        /// <summary>当前编辑中的后端初始化配置副本。</summary>
        private RailMotorSubMachineModulesInitCfg _data = new();

        /// <summary>ViewTag 属性后备字段。</summary>
        private object _viewTag;

        /// <summary>框架用于定位视图的标签对象。</summary>
        public object ViewTag
        {
            get => _viewTag;
            set => this.RaiseAndSetIfChanged(ref _viewTag, value);
        }

        /// <summary>ReadOnly 属性后备字段。</summary>
        private bool _readOnly;

        /// <summary>是否以只读方式展示配置项；变更时同步禁用各下拉框。</summary>
        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                this.RaiseAndSetIfChanged(ref _readOnly, value);
                var enabled = !_readOnly;
                RailMotionModeViewModel.IsEnabled = enabled;
                AxisBindingViewModel.IsEnabled = enabled;
            }
        }

        /// <summary>
        /// 初始化下拉框控件，并从配置服务加载可选轴列表。
        /// </summary>
        public RailMotorInitCompUIViewModel(ICompUIRunTimeCallBack callBack)
        {
            RailMotionModeViewModel = CreateComboViewModel();
            AxisBindingViewModel = CreateComboViewModel();

            RailMotionModeViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            AxisBindingViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);

            ApplyMotionModeOptions(ERailMotionMode.ExclusiveMode);
            LoadAxisInfos(callBack);
            ApplyAxisOptions(Guid.Empty);
            ReadOnly = false;
        }

        /// <summary>将后端 InitCfg 数据加载到各界面控件。</summary>
        public void SetData(RailMotorSubMachineModulesInitCfg data)
        {
            _data = CloneData(data);
            ApplyMotionModeOptions(_data.RailMotionMode);
            ApplyAxisOptions(_data.AxisBindingObjID);
        }

        /// <summary>从界面控件读取并返回最新的 InitCfg 数据。</summary>
        public RailMotorSubMachineModulesInitCfg GetData()
        {
            _data ??= new RailMotorSubMachineModulesInitCfg();
            _data.RailMotionMode = GetSelectedMotionMode(RailMotionModeViewModel);

            var selectedAxisGuid = GetSelectedAxisGuid(AxisBindingViewModel);
            if (selectedAxisGuid != Guid.Empty)
                _data.AxisBindingObjID = selectedAxisGuid;

            return CloneData(_data);
        }

        /// <summary>创建带显示路径配置的通用下拉框 ViewModel。</summary>
        private static ComboxViewModel CreateComboViewModel()
        {
            return new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                ItemsSource = new ObservableCollection<ComBoxItem>(),
            };
        }

        /// <summary>填充运动模式下拉选项并选中指定模式。</summary>
        private void ApplyMotionModeOptions(ERailMotionMode selectedMode)
        {
            var items = RailMotionModeViewModel.ItemsSource as ObservableCollection<ComBoxItem>;
            items?.Clear();
            if (items == null)
                return;

            foreach (var option in MotionModeOptions)
            {
                items.Add(new ComBoxItem
                {
                    Value = option.Mode,
                    DisplayName = option.DisplayName,
                });
            }

            var target = items.FirstOrDefault(item => item.Value is ERailMotionMode mode && mode == selectedMode);
            RailMotionModeViewModel.SelectedItem = target ?? items.FirstOrDefault();
        }

        /// <summary>从下拉框读取当前选中的电机运动模式。</summary>
        private static ERailMotionMode GetSelectedMotionMode(ComboxViewModel viewModel)
            => (viewModel.SelectedItem as ComBoxItem)?.Value is ERailMotionMode mode
                ? mode
                : ERailMotionMode.ExclusiveMode;

        /// <summary>调用后端 GetAxisOptions 命令，加载可选物理轴列表。</summary>
        private void LoadAxisInfos(ICompUIRunTimeCallBack callBack)
        {
            _axisInformations.Clear();
            try
            {
                var result = callBack?.ExecConfigSvrCtlCmd(
                    RailMotorInteropConst.CmdGetAxisOptions,
                    new GFBaseTypeParamValueList());
                var raw = result?["Result"]?.ToStringVal();
                if (string.IsNullOrWhiteSpace(raw))
                    raw = result?["data"]?.ToStringVal();

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

        /// <summary>根据缓存的轴列表刷新绑定轴下拉框并选中指定 GUID。</summary>
        private void ApplyAxisOptions(Guid selectedAxisGuid)
        {
            ApplyAxisOptions(
                AxisBindingViewModel.ItemsSource as ObservableCollection<ComBoxItem>,
                AxisBindingViewModel,
                _axisInformations,
                selectedAxisGuid);
        }

        /// <summary>填充轴下拉选项；列表为空时显示占位项，已保存 GUID 不在列表时补项。</summary>
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
            SetSelectedAxis(viewModel, items, selectedAxisGuid);
        }

        /// <summary>在下拉列表中定位并选中指定轴 GUID 对应的项。</summary>
        private static void SetSelectedAxis(ComboxViewModel viewModel, IEnumerable<ComBoxItem> items, Guid axisGuid)
        {
            var target = items.FirstOrDefault(item => (item.Value as AxisInformation)?.AxisGuid == axisGuid);
            if (target != null)
                viewModel.SelectedItem = target;
        }

        /// <summary>从下拉框读取当前选中轴的 GUID。</summary>
        private static Guid GetSelectedAxisGuid(ComboxViewModel viewModel)
            => ((viewModel.SelectedItem as ComBoxItem)?.Value as AxisInformation)?.AxisGuid ?? Guid.Empty;

        /// <summary>通过 JSON 序列化深拷贝 InitCfg，避免引用共享。</summary>
        private static RailMotorSubMachineModulesInitCfg CloneData(RailMotorSubMachineModulesInitCfg data)
        {
            if (data == null)
                return new RailMotorSubMachineModulesInitCfg();

            return JsonObjConvert.FromJSonBytes<RailMotorSubMachineModulesInitCfg>(JsonObjConvert.ToJSonBytes(data))
                ?? new RailMotorSubMachineModulesInitCfg();
        }
    }
}
