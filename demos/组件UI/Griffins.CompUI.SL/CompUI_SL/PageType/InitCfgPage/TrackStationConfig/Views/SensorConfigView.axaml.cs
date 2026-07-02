using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Griffins.CompUI.SL.InitCfgPage.Views
{
    public partial class SensorConfigView : UserControl
    {
        public SensorConfigView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
