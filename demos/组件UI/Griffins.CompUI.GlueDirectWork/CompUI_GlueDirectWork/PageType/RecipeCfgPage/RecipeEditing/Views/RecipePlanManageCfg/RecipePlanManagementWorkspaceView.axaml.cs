using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Views.RecipePlanWorkspaces
{
    public partial class RecipePlanManagementWorkspaceView : UserControl
    {
        public RecipePlanManagementWorkspaceView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnHeaderSelectAllClick(object? sender, RoutedEventArgs e)
        {
            if (DataContext is not RecipePlanManagementWorkspaceViewModel vm)
                return;

            var isChecked = sender is CheckBox checkBox && checkBox.IsChecked == true;
            vm.ApplySelectAll(isChecked);
        }
    }
}
