using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DispensingPageType.Views.RecipeParamCfgPage.ParametersAndCalculation
{
    public partial class IOWorkAreaView : UserControl
    {
        public IOWorkAreaView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void NumericControl_ActualThemeVariantChanged(object? sender, System.EventArgs e)
        {
        }
    }
}
