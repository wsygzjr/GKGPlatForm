using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.Views
{
    public partial class PositionView : UserControl
    {
        public PositionView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void UserControl_ActualThemeVariantChanged(object? sender, System.EventArgs e)
        {
        }
    }
}
