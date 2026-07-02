using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.Views
{
    public partial class FlyingPhotoCalibrationView : UserControl
    {
        public FlyingPhotoCalibrationView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
