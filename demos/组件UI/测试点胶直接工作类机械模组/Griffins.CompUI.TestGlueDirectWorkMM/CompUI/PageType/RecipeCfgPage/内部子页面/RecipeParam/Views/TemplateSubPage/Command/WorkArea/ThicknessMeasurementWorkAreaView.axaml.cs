using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DispensingPageType.Views.RecipeParamCfgPage.ParametersAndCalculation
{
    public partial class ThicknessMeasurementWorkAreaView : UserControl
    {
        public ThicknessMeasurementWorkAreaView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Button_ActualThemeVariantChanged(object? sender, System.EventArgs e)
        {
        }
    }
}
