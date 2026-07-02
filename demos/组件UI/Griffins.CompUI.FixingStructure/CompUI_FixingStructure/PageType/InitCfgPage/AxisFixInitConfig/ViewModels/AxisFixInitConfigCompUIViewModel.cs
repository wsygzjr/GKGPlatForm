using GF_Gereric;
using GKG;
using GKG.SubMM;
using GKG.UI;
using Griffins.Map.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Griffins.CompUI.FixingStructure.CompUI_FixingStructure.PageType.InitCfgPage.AxisFixInitConfig.ViewModels
{
    /// <summary>
    /// 电机固定机构初始化配置页视图模型，负责轴绑定选择与数据读写。
    /// </summary>
    internal class AxisFixInitConfigCompUIViewModel : ReactiveObject
    {
        private readonly List<AxisInformation> _axisInformations = new();
        private AxisFixSubMachineModulesInitCfg _loadedData = new();
        private bool _isApplyingData;

        /// <summary>页面内容修改后通知宿主。</summary>
        public event EventHandler AfterModified;

        /// <summary>绑定轴下拉框视图模型。</summary>
        public ComboxViewModel AxisBindingViewModel { get; }

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
                AxisBindingViewModel.IsEnabled = !_readOnly;
            }
        }

        public AxisFixInitConfigCompUIViewModel(ICompUIRunTimeCallBack callBack)
        {
            AxisBindingViewModel = new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                ItemsSource = new ObservableCollection<ComBoxItem>(),
            };
            AxisBindingViewModel.ValueChanged += (_, __) => RaiseAfterModified();

            LoadAxisOptions(callBack);
            ReadOnly = false;
        }

        /// <summary>将后端初始化数据加载到界面。</summary>
        public void SetData(AxisFixSubMachineModulesInitCfg data)
        {
            ApplyWithoutModified(() =>
            {
                _loadedData = CloneData(data);
                ApplyAxisOptions(_loadedData.AxisBindingObjID);
            });
        }

        /// <summary>从界面收集初始化数据并同步内部缓存，避免保存时丢数据。</summary>
        public AxisFixSubMachineModulesInitCfg GetData()
        {
            var result = CloneData(_loadedData);
            var selectedAxisGuid = GetSelectedAxisGuid();
            if (selectedAxisGuid != Guid.Empty)
            {
                result.AxisBindingObjID = selectedAxisGuid;
            }

            _loadedData.CopyFrom(result);
            return result;
        }

        /// <summary>通过后端 RtCmdGetAxisOptions 加载轴列表。</summary>
        private void LoadAxisOptions(ICompUIRunTimeCallBack callBack)
        {
            _axisInformations.Clear();
            try
            {
                var result = callBack?.ExecConfigSvrCtlCmd(
                    FixingStructureSubMachineModulesConst.RtCmdGetAxisOptions,
                    new GFBaseTypeParamValueList());

                var raw = result?["data"]?.ToStringVal();
                if (string.IsNullOrWhiteSpace(raw))
                {
                    raw = result?["Result"]?.ToStringVal();
                }

                var axisInfos = string.IsNullOrWhiteSpace(raw)
                    ? null
                    : JsonObjConvert.FromJSon<List<AxisInformation>>(raw);

                if (axisInfos != null)
                {
                    _axisInformations.AddRange(axisInfos.Where(item => item != null && item.AxisGuid != Guid.Empty));
                }
            }
            catch
            {
            }

            ApplyAxisOptions(_loadedData.AxisBindingObjID);
        }

        /// <summary>刷新轴下拉选项并保持当前选中项。</summary>
        private void ApplyAxisOptions(Guid selectedAxisGuid)
        {
            var items = AxisBindingViewModel.ItemsSource as ObservableCollection<ComBoxItem>;
            items?.Clear();
            if (items == null)
            {
                return;
            }

            foreach (var axisInfo in _axisInformations
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

            if (items.Count == 0)
            {
                items.Add(new ComBoxItem
                {
                    Value = null,
                    DisplayName = "等待后端轴列表",
                });
            }

            AxisBindingViewModel.SelectedItem = items[0];
            var target = items.FirstOrDefault(item => (item.Value as AxisInformation)?.AxisGuid == selectedAxisGuid);
            if (target != null)
            {
                AxisBindingViewModel.SelectedItem = target;
            }
        }

        private Guid GetSelectedAxisGuid()
            => ((AxisBindingViewModel.SelectedItem as ComBoxItem)?.Value as AxisInformation)?.AxisGuid ?? Guid.Empty;

        private void ApplyWithoutModified(Action action)
        {
            _isApplyingData = true;
            try
            {
                action?.Invoke();
            }
            finally
            {
                _isApplyingData = false;
            }
        }

        private void RaiseAfterModified()
        {
            if (_isApplyingData)
            {
                return;
            }

            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        private static AxisFixSubMachineModulesInitCfg CloneData(AxisFixSubMachineModulesInitCfg data)
        {
            if (data == null)
            {
                return new AxisFixSubMachineModulesInitCfg();
            }

            var cloned = new AxisFixSubMachineModulesInitCfg();
            cloned.CopyFrom(data);
            return cloned;
        }
    }
}
