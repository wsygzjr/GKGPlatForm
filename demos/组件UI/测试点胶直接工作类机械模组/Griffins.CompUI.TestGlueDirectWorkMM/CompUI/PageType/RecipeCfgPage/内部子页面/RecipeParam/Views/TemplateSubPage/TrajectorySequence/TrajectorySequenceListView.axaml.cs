using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DispensingPageType.Views.RecipeParamCfgPage.ParametersAndCalculation
{
    public partial class TrajectorySequenceListView : UserControl
    {
        public TrajectorySequenceListView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void DataGrid_ActualThemeVariantChanged(object? sender, System.EventArgs e)
        {
        }

        private void ComboxControl_ActualThemeVariantChanged(object? sender, System.EventArgs e)
        {
        }
    }
}
