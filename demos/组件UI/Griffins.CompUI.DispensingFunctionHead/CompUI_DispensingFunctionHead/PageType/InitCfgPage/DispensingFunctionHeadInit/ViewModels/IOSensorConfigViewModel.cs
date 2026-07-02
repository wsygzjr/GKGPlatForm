using GKG;
using GKG.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.InitCfgPage.DispensingFunctionHeadInit.ViewModels
{
    /// <summary>
    /// IO传感器配置视图模型
    /// 用于配置单个IO点位（如阀报警IO）
    /// </summary>
    public class IOSensorConfigViewModel : ReactiveObject
    {
        private readonly List<ComBoxItem> _ioItems = new();
        private List<IOStateInformation> _ioStateList = new();

        /// <summary>
        /// IO通道下拉框视图模型
        /// </summary>
        public ComboxViewModel IOChannelViewModel { get; }

        /// <summary>
        /// 修改后事件
        /// </summary>
        public event EventHandler? AfterModified;

        /// <summary>
        /// 构造函数（设计时）
        /// </summary>
        public IOSensorConfigViewModel()
            : this(null, Guid.Empty)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ioStates">IO状态信息列表</param>
        /// <param name="selectedGuid">选中的IO GUID</param>
        public IOSensorConfigViewModel(IEnumerable<IOStateInformation>? ioStates, Guid selectedGuid)
        {
            IOChannelViewModel = new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                ItemsSource = _ioItems,
            };

            // 订阅值变化事件
            IOChannelViewModel.ValueChanged += (sender, args) => AfterModified?.Invoke(sender, args);

            SetOptions(ioStates, selectedGuid);
        }

        /// <summary>
        /// 设置IO选项列表和选中项
        /// </summary>
        /// <param name="ioStates">IO状态信息列表</param>
        /// <param name="selectedGuid">选中的IO GUID</param>
        public void SetOptions(IEnumerable<IOStateInformation>? ioStates, Guid selectedGuid)
        {
            _ioItems.Clear();
            IOChannelViewModel.ItemsSource = null;

            // 过滤并去重IO列表
            _ioStateList = (ioStates ?? Enumerable.Empty<IOStateInformation>())
                .Where(item => item != null && item.IOGuid != Guid.Empty)
                .GroupBy(item => item.IOGuid)
                .Select(group => group.First())
                .ToList();

            // 构建下拉框选项
            foreach (var ioState in _ioStateList)
            {
                _ioItems.Add(new ComBoxItem
                {
                    Value = ioState,
                    DisplayName = BuildDisplayName(ioState),
                });
            }

            IOChannelViewModel.ItemsSource = _ioItems;

            // 设置选中项
            if (selectedGuid != Guid.Empty)
            {
                AppendItemIfNeeded(selectedGuid);
                if (SetSelectedItem(selectedGuid))
                {
                    return;
                }
            }

            IOChannelViewModel.SelectedItem = null;
        }

        /// <summary>
        /// 如果历史配置的IO当前不在列表中，则补充进去
        /// </summary>
        /// <param name="selectedGuid">历史配置的IO GUID</param>
        private void AppendItemIfNeeded(Guid selectedGuid)
        {
            if (selectedGuid == Guid.Empty || _ioItems.Any(item => (item.Value as IOStateInformation)?.IOGuid == selectedGuid))
            {
                return;
            }

            var ioState = _ioStateList.FirstOrDefault(item => item.IOGuid == selectedGuid);
            _ioItems.Add(new ComBoxItem
            {
                Value = ioState ?? new IOStateInformation
                {
                    IOGuid = selectedGuid,
                    IOName = selectedGuid.ToString(),
                    ChannelId = selectedGuid.ToString(),
                },
                DisplayName = ioState != null ? BuildDisplayName(ioState) : selectedGuid.ToString(),
            });
        }

        /// <summary>
        /// 获取当前选中的IO GUID
        /// </summary>
        /// <returns>选中的IO GUID，未选中则返回 Guid.Empty</returns>
        public Guid GetSelectedGuid()
        {
            return ((IOChannelViewModel.SelectedItem as ComBoxItem)?.Value as IOStateInformation)?.IOGuid ?? Guid.Empty;
        }

        /// <summary>
        /// 设置选中项
        /// </summary>
        /// <param name="selectedGuid">要选中的IO GUID</param>
        /// <returns>是否设置成功</returns>
        private bool SetSelectedItem(Guid selectedGuid)
        {
            var target = _ioItems.FirstOrDefault(item => (item.Value as IOStateInformation)?.IOGuid == selectedGuid);
            if (target == null)
            {
                return false;
            }

            IOChannelViewModel.SelectedItem = target;
            return true;
        }

        /// <summary>
        /// 构建IO显示名称
        /// 优先使用IOName，其次ChannelId，最后使用IOGuid
        /// </summary>
        /// <param name="ioState">IO状态信息</param>
        /// <returns>显示名称</returns>
        private static string BuildDisplayName(IOStateInformation ioState)
        {
            if (ioState == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(ioState.IOName))
            {
                return ioState.IOName;
            }

            if (!string.IsNullOrWhiteSpace(ioState.ChannelId))
            {
                return ioState.ChannelId;
            }

            return ioState.IOGuid.ToString();
        }
    }
}
