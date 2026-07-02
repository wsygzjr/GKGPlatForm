using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Views.ProgramEditing
{
    public partial class AreaEditingWorkspaceView : UserControl
    {
        public AreaEditingWorkspaceView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnHeaderSelectAllClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is not AreaEditingWorkspaceViewModel vm || sender is not CheckBox cb)
                return;
            var check = cb.IsChecked == true;
            vm.IsAllSelected = check;
            vm.ApplySelectAll(check);
        }
    }
}
