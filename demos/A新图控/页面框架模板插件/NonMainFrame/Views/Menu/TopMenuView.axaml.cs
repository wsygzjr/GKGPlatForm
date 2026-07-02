using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace NonMainFrameView.Views.Menu
{
    public partial class TopMenuView : UserControl
    {
        public TopMenuView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
