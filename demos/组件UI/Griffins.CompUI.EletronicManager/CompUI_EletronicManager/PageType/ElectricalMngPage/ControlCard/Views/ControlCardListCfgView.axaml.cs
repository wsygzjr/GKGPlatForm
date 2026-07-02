using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.ControlCard.ViewModels;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.ControlCard.Views
{
    public partial class ControlCardListCfgView : UserControl
    {
        public ControlCardListCfgView()
        {
            InitializeComponent();
            AttachedToVisualTree += (_, _) =>
            {
                BindOwnerWindowProvider();
                SyncGridSelectionFromViewModel();
            };
            DataContextChanged += (_, _) =>
            {
                BindOwnerWindowProvider();
                SyncGridSelectionFromViewModel();
            };

            if (this.FindControl<DataGrid>("ControlCardDataGrid") is { } dataGrid)
                dataGrid.SelectionChanged += ControlCardDataGrid_OnSelectionChanged;
        }

        private void ControlCardDataGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (sender is not DataGrid dataGrid || DataContext is not ControlCardListViewModel vm)
                return;

            if (dataGrid.SelectedItem is ControlCardViewModel selectedCard)
            {
                if (!ReferenceEquals(vm.SelectedControlCard, selectedCard))
                    vm.SelectedControlCard = selectedCard;
                return;
            }

            var fallback = vm.SelectedControlCard ?? vm.LastNonNullSelectedControlCard ?? (vm.ControlCardList.Count > 0 ? vm.ControlCardList[0] : null);
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
            if (DataContext is not ControlCardListViewModel vm)
                return;

            if (this.FindControl<DataGrid>("ControlCardDataGrid") is not { } dataGrid)
                return;

            var fallback = vm.SelectedControlCard ?? vm.LastNonNullSelectedControlCard ?? (vm.ControlCardList.Count > 0 ? vm.ControlCardList[0] : null);
            if (fallback == null)
                return;

            Dispatcher.UIThread.Post(() =>
            {
                if (!ReferenceEquals(dataGrid.SelectedItem, fallback))
                    dataGrid.SelectedItem = fallback;
            }, DispatcherPriority.Background);
        }

        private void BindOwnerWindowProvider()
        {
            if (DataContext is not ControlCardListViewModel vm)
                return;

            vm.OwnerWindowProvider = () => this.GetVisualRoot() as Window;
        }
    }
}
