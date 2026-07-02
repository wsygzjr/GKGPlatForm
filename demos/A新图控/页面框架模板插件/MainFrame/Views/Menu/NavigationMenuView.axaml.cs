using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MainFrameView.Views.Menu
{
    public partial class NavigationMenuView : UserControl
    {
        public NavigationMenuView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
