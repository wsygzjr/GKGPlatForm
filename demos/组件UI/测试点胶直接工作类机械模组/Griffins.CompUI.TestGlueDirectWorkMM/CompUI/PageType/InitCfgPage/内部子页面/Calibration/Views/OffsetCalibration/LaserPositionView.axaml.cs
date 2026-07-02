using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.Views
{
    public partial class LaserPositionView : UserControl
    {
        public LaserPositionView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
