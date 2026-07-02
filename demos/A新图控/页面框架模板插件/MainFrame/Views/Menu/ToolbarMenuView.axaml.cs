using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MainFrameView.Views.Menu
{
    public partial class ToolbarMenuView : UserControl
    {
        public ToolbarMenuView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
