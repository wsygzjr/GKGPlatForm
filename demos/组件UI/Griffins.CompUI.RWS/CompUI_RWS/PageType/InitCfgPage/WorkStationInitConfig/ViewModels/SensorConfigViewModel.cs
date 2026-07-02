#nullable disable
using GKG;
using GKG.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Griffins.CompUI.RWS.CompUI_RWS.PageType.InitCfgPage.WorkStationInitConfig.ViewModels
{
    internal class SensorConfigViewModel : ReactiveObject
    {
        private readonly List<ComBoxItem> ioItems = new();
        private List<IOStateInformation> ioStateList = new();

        public ComboxViewModel IOChannelViewModel { get; }

        public SensorConfigViewModel()
            : this(null, Guid.Empty)
        {
        }

        public SensorConfigViewModel(IEnumerable<IOStateInformation> ioStates, Guid selectedGuid)
        {
            IOChannelViewModel = new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                ItemsSource = ioItems,
            };

            SetOptions(ioStates, selectedGuid);
        }

        public void SetOptions(IEnumerable<IOStateInformation> ioStates, Guid selectedGuid)
        {
            ioItems.Clear();
            IOChannelViewModel.ItemsSource = null;

            ioStateList = (ioStates ?? Enumerable.Empty<IOStateInformation>())
                .Where(item => item != null && item.IOGuid != Guid.Empty)
                .GroupBy(item => item.IOGuid)
                .Select(group => group.First())
                .ToList();

            foreach (var ioState in ioStateList)
            {
                ioItems.Add(new ComBoxItem
                {
                    Value = ioState,
                    DisplayName = BuildDisplayName(ioState),
                });
            }

            IOChannelViewModel.ItemsSource = ioItems;

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

        private void AppendItemIfNeeded(Guid selectedGuid)
        {
            if (selectedGuid == Guid.Empty || ioItems.Any(item => (item.Value as IOStateInformation)?.IOGuid == selectedGuid))
            {
                return;
            }

            var ioState = ioStateList.FirstOrDefault(item => item.IOGuid == selectedGuid);
            ioItems.Add(new ComBoxItem
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

        public Guid GetSelectedGuid()
        {
            return ((IOChannelViewModel.SelectedItem as ComBoxItem)?.Value as IOStateInformation)?.IOGuid ?? Guid.Empty;
        }

        private bool SetSelectedItem(Guid selectedGuid)
        {
            var target = ioItems.FirstOrDefault(item => (item.Value as IOStateInformation)?.IOGuid == selectedGuid);
            if (target == null)
            {
                return false;
            }

            IOChannelViewModel.SelectedItem = target;
            return true;
        }

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
