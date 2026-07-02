using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.ControlCard.ViewModels;
using System.ComponentModel;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.ControlCard.Views
{
    public partial class AxisConfigView : UserControl
    {
        private AxisConfigViewModel? _boundViewModel;

        public AxisConfigView()
        {
            InitializeComponent();
            AttachedToVisualTree += (_, _) => SyncGridSelectionFromViewModel();
            DataContextChanged += (_, _) =>
            {
                RebindViewModelEvents();
                SyncGridSelectionFromViewModel();
                RefreshAxisEditorHost();
            };

            if (this.FindControl<DataGrid>("AxisConfigDataGrid") is { } dataGrid)
                dataGrid.SelectionChanged += AxisConfigDataGrid_SelectionChanged;
        }

        private void AxisConfigDataGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (sender is not DataGrid dataGrid || DataContext is not AxisConfigViewModel vm)
                return;

            if (dataGrid.SelectedItem is AxisConfigItemViewModel selectedItem)
            {
                if (!ReferenceEquals(vm.SelectedAxisItem, selectedItem))
                    vm.SelectedAxisItem = selectedItem;
                return;
            }

            var fallback = vm.SelectedAxisItem ?? (vm.AxisItems.Count > 0 ? vm.AxisItems[0] : null);
            if (fallback == null)
                return;

            Dispatcher.UIThread.Post(() =>
            {
                if (!ReferenceEquals(dataGrid.SelectedItem, fallback))
                    dataGrid.SelectedItem = fallback;
            }, DispatcherPriority.Background);
        }

        private void SyncGridSelectionFromViewModel()
        {
            if (DataContext is not AxisConfigViewModel vm)
                return;

            if (this.FindControl<DataGrid>("AxisConfigDataGrid") is not { } dataGrid)
                return;

            var fallback = vm.SelectedAxisItem ?? (vm.AxisItems.Count > 0 ? vm.AxisItems[0] : null);
            if (fallback == null)
                return;

            Dispatcher.UIThread.Post(() =>
            {
                if (!ReferenceEquals(dataGrid.SelectedItem, fallback))
                    dataGrid.SelectedItem = fallback;
            }, DispatcherPriority.Background);
        }

        private void RebindViewModelEvents()
        {
            if (_boundViewModel != null)
                _boundViewModel.PropertyChanged -= ViewModel_PropertyChanged;

            _boundViewModel = DataContext as AxisConfigViewModel;

            if (_boundViewModel != null)
                _boundViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AxisConfigViewModel.SelectedAxisItem))
            {
                RefreshAxisEditorHost();
            }
        }

        private void RefreshAxisEditorHost()
        {
            if (this.FindControl<ContentControl>("AxisEditorHost") is not { } host)
                return;

            if (host.Content is not MotionControlAxisEditorView editorView)
            {
                editorView = new MotionControlAxisEditorView();
                host.Content = editorView;
            }

            if (DataContext is not AxisConfigViewModel vm)
            {
                editorView.DataContext = null;
                host.IsEnabled = false;
                return;
            }

            if (!ReferenceEquals(editorView.DataContext, vm.SelectedAxis))
                editorView.DataContext = vm.SelectedAxis;

            host.IsEnabled = vm.SelectedAxisItem != null;
        }

        private async void OpenAxisAdvancedParameterWindow_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is not AxisConfigViewModel vm || !vm.CanOpenAdvancedParameter)
                return;

            var owner = this.GetVisualRoot() as Window;
            if (owner == null)
                return;

            var win = new Window
            {
                Title = "轴驱动高级参数",
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
