using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels;
using System;
using System.ComponentModel;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Views.RecipePlanWorkspaces
{
    public partial class RecipePlanAreaConfigWorkspaceView : UserControl
    {
        private RecipePlanAreaConfigWorkspaceViewModel? _vm;

        public RecipePlanAreaConfigWorkspaceView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnDataContextChanged(object? sender, EventArgs e)
        {
            if (_vm != null)
                _vm.PropertyChanged -= OnVmPropertyChanged;

            _vm = DataContext as RecipePlanAreaConfigWorkspaceViewModel;
            if (_vm != null)
            {
                _vm.PropertyChanged += OnVmPropertyChanged;
                SyncFilterRadios(_vm.FilterMode);
            }
        }

        private void OnVmPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RecipePlanAreaConfigWorkspaceViewModel.FilterMode) && sender is RecipePlanAreaConfigWorkspaceViewModel vm)
                SyncFilterRadios(vm.FilterMode);
        }

        private void SyncFilterRadios(int mode)
        {
            if (RbFilterAll == null)
                return;
            RbFilterAll.IsChecked = mode == 0;
            RbFilterEnabled.IsChecked = mode == 1;
            RbFilterDisabled.IsChecked = mode == 2;
        }

        private void OnFilterRadioChecked(object? sender, RoutedEventArgs e)
        {
            if (DataContext is not RecipePlanAreaConfigWorkspaceViewModel vm || sender is not RadioButton { IsChecked: true } rb || rb.Tag is not string s)
                return;
            var m = s == "1" ? 1 : s == "2" ? 2 : 0;
            if (vm.FilterMode != m)
                vm.FilterMode = m;
        }

        private void OnHeaderSelectAllClick(object? sender, RoutedEventArgs e)
        {
            if (DataContext is not RecipePlanAreaConfigWorkspaceViewModel vm)
                return;

            var isChecked = sender is CheckBox checkBox && checkBox.IsChecked == true;
            vm.ApplySelectAll(isChecked);
        }
    }
}
