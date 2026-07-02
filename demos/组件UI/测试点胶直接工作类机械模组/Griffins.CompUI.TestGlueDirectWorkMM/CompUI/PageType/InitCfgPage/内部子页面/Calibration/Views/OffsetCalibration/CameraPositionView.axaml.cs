using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.Views
{
    public partial class CameraPositionView : UserControl
    {
        public CameraPositionView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
