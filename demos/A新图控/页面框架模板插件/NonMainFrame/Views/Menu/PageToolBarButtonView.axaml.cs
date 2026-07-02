using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace NonMainFrameView.Views.Menu
{
    public partial class PageToolBarButtonView : UserControl
    {
        public PageToolBarButtonView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
