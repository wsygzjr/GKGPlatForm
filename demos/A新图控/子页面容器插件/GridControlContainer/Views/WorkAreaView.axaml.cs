using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GKG.Map.Page.UIContainer.GridContainer.Views
{
    public partial class WorkAreaView : UserControl
    {

        public WorkAreaView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
