using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GKG.Map.Page.UIContainer.GridContainer.Views
{
    public partial class PreviewView : UserControl
    {

        public PreviewView()
        {
            InitializeComponent();

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
