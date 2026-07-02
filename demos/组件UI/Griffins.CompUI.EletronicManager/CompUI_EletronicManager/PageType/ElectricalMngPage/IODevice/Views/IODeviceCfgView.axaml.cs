using Avalonia.Controls;
using Avalonia.VisualTree;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.IODevice.ViewModels;
using System;
using System.Linq;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.IODevice.Views
{
    public partial class IODeviceCfgView : UserControl
    {
        public IODeviceCfgViewModel.IOPageMode PageMode { get; set; } = IODeviceCfgViewModel.IOPageMode.State;
        private bool _isCommittingEditorChanges;

        public IODeviceCfgView()
        {
            InitializeComponent();
            AttachedToVisualTree += (_, _) => ApplyPageMode();
            DataContextChanged += (_, _) => ApplyPageMode();
        }

        private void ApplyPageMode()
        {
            if (DataContext is IODeviceCfgViewModel vm)
                vm.PageMode = PageMode;
        }

        public void CommitPendingEditorChanges()
        {
            if (this.FindControl<DataGrid>("IODeviceDataGrid") is not { } dataGrid)
                return;

            _isCommittingEditorChanges = true;
            try
            {
                try
                {
                    dataGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                    dataGrid.CommitEdit(DataGridEditingUnit.Row, true);
                }
                catch
                {
                }

                foreach (var comboBox in dataGrid.GetVisualDescendants().OfType<ComboBox>())
                {
                    if (comboBox.DataContext is not IODeviceInfoViewModel rowVm)
                        continue;

                    var tag = comboBox.Tag?.ToString() ?? string.Empty;
                    if (string.Equals(tag, "Device", StringComparison.OrdinalIgnoreCase))
                    {
                        var option = comboBox.SelectedItem as GKG.UI.ComBoxItem;
                        if (ShouldIgnoreEmptyDeviceSelection(rowVm, option, comboBox))
                            continue;

                        rowVm.ApplySelectedDeviceItem(option);
                        continue;
                    }

                    if (string.Equals(tag, "Channel", StringComparison.OrdinalIgnoreCase))
                    {
                        var option = comboBox.SelectedItem as GKG.UI.ComBoxItem;
                        if (ShouldIgnoreEmptyChannelSelection(rowVm, option, comboBox))
                            continue;

                        rowVm.ApplySelectedChannelItem(option);
                    }
                }
            }
            finally
            {
                _isCommittingEditorChanges = false;
            }
        }

        private void DeviceComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (sender is not ComboBox comboBox || comboBox.DataContext is not IODeviceInfoViewModel rowVm)
                return;

            var option = comboBox.SelectedItem as GKG.UI.ComBoxItem;
            if (ShouldIgnoreEmptyDeviceSelection(rowVm, option, comboBox))
                return;

            rowVm.ApplySelectedDeviceItem(option);
        }

        private void ChannelComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (sender is not ComboBox comboBox || comboBox.DataContext is not IODeviceInfoViewModel rowVm)
                return;

            var option = comboBox.SelectedItem as GKG.UI.ComBoxItem;
            if (ShouldIgnoreEmptyChannelSelection(rowVm, option, comboBox))
                return;

            rowVm.ApplySelectedChannelItem(option);
        }

        private bool ShouldIgnoreEmptyDeviceSelection(IODeviceInfoViewModel rowVm, GKG.UI.ComBoxItem? option, ComboBox comboBox)
        {
            var isEmptySelection = option == null || string.IsNullOrWhiteSpace(option.Value?.ToString());
            if (!isEmptySelection || string.IsNullOrWhiteSpace(rowVm.DeviceId))
                return false;

            return _isCommittingEditorChanges || !comboBox.IsDropDownOpen;
        }

        private bool ShouldIgnoreEmptyChannelSelection(IODeviceInfoViewModel rowVm, GKG.UI.ComBoxItem? option, ComboBox comboBox)
        {
            if (!string.IsNullOrWhiteSpace(option?.Value?.ToString()) || string.IsNullOrWhiteSpace(rowVm.ChannelSelection))
                return false;

            return _isCommittingEditorChanges || !comboBox.IsDropDownOpen;
        }

        private async void OpenStateAdvancedParameterWindow_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is not IODeviceCfgViewModel vm || !vm.CanOpenAdvancedParameter)
                return;

            var owner = this.GetVisualRoot() as Window;
            if (owner == null)
                return;

            var win = new Window
            {
                Title = "状态量高级参数",
                Width = 900,
                Height = 620,
                MinWidth = 760,
                MinHeight = 520,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content = new Border
                {
                    Margin = new Avalonia.Thickness(16),
                    Padding = new Avalonia.Thickness(16),
                    CornerRadius = new Avalonia.CornerRadius(8),
                    BorderBrush = Avalonia.Media.Brush.Parse("#D0D7E2"),
                    BorderThickness = new Avalonia.Thickness(1),
                    Background = Avalonia.Media.Brush.Parse("#F8FBFD"),
                }
            };

            await win.ShowDialog(owner);
        }
    }
}
